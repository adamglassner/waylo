using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Waylo.Services;

namespace Waylo.Views;

/// <summary>
/// The Chats preview from the Waylo mockup: a conversation list and one open conversation.
/// Every person and message here is made up; real chats arrive when messaging is built.
/// </summary>
public partial class ChatsView : UserControl
{
    /// <summary>Presence is a theme brush key, so the dots follow light and dark mode.</summary>
    private sealed record SampleChat(string Name, string Initials, string Color, string Presence, string Preview, string Time, int Unread);

    private const string Online = "Brush.LiveDot";
    private const string Away = "Brush.Away";
    private const string Busy = "Brush.Busy";
    private const string Offline = "Brush.Offline";

    private static readonly SampleChat[] Chats =
    [
        new("Maya Ortiz", "MO", "#E8833A", Online, "Maya is typing…", "2:06 PM", 0),
        new("Family", "FM", "#2BA88A", Online, "Mom: See everyone at 7:30 PM!", "1:40 PM", 3),
        new("Dan Kowalski", "DK", "#8E6CE0", Away, "Missed voice call", "11:15 AM", 1),
        new("Priya Nair", "PN", "#D9566F", Busy, "You: Sounds good, talk soon", "Yesterday", 0),
        new("Game Night Crew", "GN", "#1A6FD1", Online, "Leo shared a photo", "Yesterday", 0),
        new("Sam Whitfield", "SW", "#5A7184", Offline, "Call ended · 45 min", "Mon", 0),
        new("Rosa Delgado", "RD", "#B5561A", Offline, "Thanks for the help!", "Sun", 0),
    ];

    public ChatsView()
    {
        InitializeComponent();

        HeaderAvatar.Content = Avatar("MO", "#E8833A", 44, Online);

        for (int i = 0; i < Chats.Length; i++) ChatList.Children.Add(ChatRow(Chats[i], selected: i == 0));

        Messages.Children.Add(DayDivider("Today"));
        Messages.Children.Add(Incoming("Hey! Are we still on for the call with everyone tonight?", "1:52 PM"));
        Messages.Children.Add(Outgoing("Yep, 7:30 PM. I'll send the meeting link to the group.", "1:54 PM · Seen"));
        Messages.Children.Add(CallEnded());
        Messages.Children.Add(IncomingFile("trip-itinerary.pdf", "PDF · 1.2 MB"));
        Messages.Children.Add(Incoming("Here's the plan we talked about. Can you look it over before tonight?", "2:06 PM"));
        Messages.Children.Add(Typing());
    }

    // ----- Conversation list -----

    private UIElement ChatRow(SampleChat chat, bool selected)
    {
        var row = new DockPanel { Margin = new Thickness(10) };

        var avatar = Avatar(chat.Initials, chat.Color, 44, chat.Presence);
        avatar.Margin = new Thickness(0, 0, 12, 0);
        DockPanel.SetDock(avatar, Dock.Left);
        row.Children.Add(avatar);

        var lines = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

        var top = new DockPanel();
        var time = MutedText(chat.Time, 12);
        DockPanel.SetDock(time, Dock.Right);
        top.Children.Add(time);
        top.Children.Add(new TextBlock
        {
            Text = chat.Name, FontWeight = FontWeights.SemiBold, FontSize = 14,
            TextTrimming = TextTrimming.CharacterEllipsis, Margin = new Thickness(0, 0, 8, 0),
        });
        lines.Children.Add(top);

        var bottom = new DockPanel { Margin = new Thickness(0, 3, 0, 0) };
        if (chat.Unread > 0)
        {
            var badge = new Border
            {
                CornerRadius = new CornerRadius(10), MinWidth = 20, Height = 20, Padding = new Thickness(6, 0, 6, 0),
                Margin = new Thickness(8, 0, 0, 0),
                Child = new TextBlock
                {
                    Text = chat.Unread.ToString(), FontSize = 11, FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
                },
            };
            badge.SetResourceReference(Border.BackgroundProperty, "Brush.Accent");
            ((TextBlock)badge.Child).SetResourceReference(TextBlock.ForegroundProperty, "Brush.AccentText");
            DockPanel.SetDock(badge, Dock.Right);
            bottom.Children.Add(badge);
        }
        var preview = MutedText(chat.Preview, 13);
        preview.TextWrapping = TextWrapping.NoWrap;
        preview.TextTrimming = TextTrimming.CharacterEllipsis;
        bottom.Children.Add(preview);
        lines.Children.Add(bottom);

        row.Children.Add(lines);

        var button = new Button
        {
            Content = row,
            Tag = "Opening chats",
            Margin = new Thickness(0, 0, 0, 2),
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        button.SetResourceReference(StyleProperty, "CardButton");
        if (selected) button.SetResourceReference(BackgroundProperty, "Brush.Selected");
        AutomationProperties.SetName(button, chat.Name);
        button.Click += Soon_Click;
        return button;
    }

    // ----- Messages -----

    private static UIElement DayDivider(string text)
    {
        var pill = new Border
        {
            CornerRadius = new CornerRadius(10), Padding = new Thickness(12, 3, 12, 3),
            HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 14),
            Child = MutedText(text, 12, FontWeights.SemiBold),
        };
        pill.SetResourceReference(Border.BackgroundProperty, "Brush.Skeleton");
        return pill;
    }

    private static Border Bubble(UIElement content, bool outgoing)
    {
        var bubble = new Border
        {
            Child = content,
            Padding = new Thickness(14, 10, 14, 10),
            CornerRadius = outgoing ? new CornerRadius(16, 16, 4, 16) : new CornerRadius(16, 16, 16, 4),
            BorderThickness = new Thickness(outgoing ? 0 : 1),
        };
        bubble.SetResourceReference(Border.BackgroundProperty, outgoing ? "Brush.Accent" : "Brush.BubbleIn");
        if (!outgoing) bubble.SetResourceReference(Border.BorderBrushProperty, "Brush.BubbleInBorder");
        return bubble;
    }

    private static TextBlock BubbleText(string text, bool outgoing)
    {
        var tb = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, FontSize = 14, LineHeight = 20 };
        if (outgoing) tb.SetResourceReference(TextBlock.ForegroundProperty, "Brush.AccentText");
        return tb;
    }

    /// <summary>Message from Maya: small avatar on the left, bubble, time underneath.</summary>
    private static UIElement IncomingRow(UIElement bubble, string time)
    {
        var row = new DockPanel { Margin = new Thickness(0, 0, 0, 14), HorizontalAlignment = HorizontalAlignment.Left, MaxWidth = 560 };
        var avatar = Avatar("MO", "#E8833A", 30, null);
        avatar.VerticalAlignment = VerticalAlignment.Bottom;
        avatar.Margin = new Thickness(0, 0, 10, 20);
        DockPanel.SetDock(avatar, Dock.Left);
        row.Children.Add(avatar);

        var stack = new StackPanel();
        stack.Children.Add(bubble);
        stack.Children.Add(MutedText(time, 11, margin: new Thickness(2, 4, 0, 0)));
        row.Children.Add(stack);
        return row;
    }

    private static UIElement Incoming(string text, string time) =>
        IncomingRow(Bubble(BubbleText(text, false), false), time);

    private static UIElement Outgoing(string text, string time)
    {
        var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right, MaxWidth = 520, Margin = new Thickness(0, 0, 0, 14) };
        stack.Children.Add(Bubble(BubbleText(text, true), true));
        var t = MutedText(time, 11, margin: new Thickness(0, 4, 2, 0));
        t.HorizontalAlignment = HorizontalAlignment.Right;
        stack.Children.Add(t);
        return stack;
    }

    private static UIElement IncomingFile(string name, string detail)
    {
        var file = new DockPanel();
        var icon = new Border
        {
            Width = 40, Height = 40, CornerRadius = new CornerRadius(10), Margin = new Thickness(0, 0, 12, 0),
            Background = Hex("#FDEEE3"),
            Child = new TextBlock
            {
                Text = "", FontSize = 18, Foreground = Hex("#B5561A"),
                HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
            },
        };
        ((TextBlock)icon.Child).SetResourceReference(TextBlock.FontFamilyProperty, "IconFont");
        DockPanel.SetDock(icon, Dock.Left);
        file.Children.Add(icon);
        var text = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        text.Children.Add(new TextBlock { Text = name, FontWeight = FontWeights.SemiBold, FontSize = 14 });
        text.Children.Add(MutedText(detail, 12));
        file.Children.Add(text);
        return IncomingRow(Bubble(file, false), "2:05 PM");
    }

    /// <summary>The Skype-style "call ended" line in the middle of the conversation.</summary>
    private UIElement CallEnded()
    {
        var card = new Border
        {
            CornerRadius = new CornerRadius(12), BorderThickness = new Thickness(1), Padding = new Thickness(12, 8, 8, 8),
            HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 14),
        };
        card.SetResourceReference(Border.BackgroundProperty, "Brush.Surface");
        card.SetResourceReference(Border.BorderBrushProperty, "Brush.Border");

        var row = new StackPanel { Orientation = Orientation.Horizontal };
        var icon = new Border
        {
            Width = 30, Height = 30, CornerRadius = new CornerRadius(15), Margin = new Thickness(0, 0, 10, 0),
            Child = new TextBlock { Text = "", FontSize = 13, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        };
        icon.SetResourceReference(Border.BackgroundProperty, "Brush.AccentSoft");
        ((TextBlock)icon.Child).SetResourceReference(TextBlock.ForegroundProperty, "Brush.AccentInk");
        ((TextBlock)icon.Child).SetResourceReference(TextBlock.FontFamilyProperty, "IconFont");
        row.Children.Add(icon);

        var text = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        text.Children.Add(new TextBlock { Text = "Video call ended", FontWeight = FontWeights.SemiBold, FontSize = 13 });
        text.Children.Add(MutedText("12 min 34 sec · 2:01 PM", 12));
        row.Children.Add(text);

        var callBack = new Button { Content = "Call back", Tag = "Video calls", Margin = new Thickness(14, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        callBack.SetResourceReference(StyleProperty, "LinkButton");
        callBack.Click += Soon_Click;
        row.Children.Add(callBack);

        card.Child = row;
        return card;
    }

    private static UIElement Typing()
    {
        var dots = new StackPanel { Orientation = Orientation.Horizontal };
        for (int i = 0; i < 3; i++)
        {
            var dot = new Ellipse { Width = 7, Height = 7, Margin = new Thickness(i == 0 ? 0 : 4, 0, 0, 0) };
            dot.SetResourceReference(Shape.FillProperty, "Brush.Offline");
            dots.Children.Add(dot);
        }
        var bubble = Bubble(dots, false);
        bubble.Padding = new Thickness(14, 12, 14, 12);

        var row = new StackPanel { Orientation = Orientation.Horizontal };
        var avatar = Avatar("MO", "#E8833A", 30, null);
        avatar.Margin = new Thickness(0, 0, 10, 0);
        row.Children.Add(avatar);
        row.Children.Add(bubble);
        var label = MutedText("Maya is typing…", 12, margin: new Thickness(10, 0, 0, 0));
        label.VerticalAlignment = VerticalAlignment.Center;
        row.Children.Add(label);
        return row;
    }

    // ----- Helpers -----

    /// <summary>Colored circle with initials and, optionally, a presence dot in the corner.</summary>
    private static FrameworkElement Avatar(string initials, string color, double size, string? presenceKey)
    {
        var grid = new Grid { Width = size, Height = size, VerticalAlignment = VerticalAlignment.Center };
        grid.Children.Add(new Ellipse { Fill = Hex(color) });
        grid.Children.Add(new TextBlock
        {
            Text = initials, Foreground = Brushes.White, FontWeight = FontWeights.Bold,
            FontSize = Math.Round(size * 0.34),
            HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
        });
        if (presenceKey != null)
        {
            double d = Math.Round(size * 0.29);
            var dot = new Ellipse
            {
                Width = d, Height = d, StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, -1, -1),
            };
            dot.SetResourceReference(Shape.FillProperty, presenceKey);
            dot.SetResourceReference(Shape.StrokeProperty, "Brush.Surface");
            grid.Children.Add(dot);
        }
        return grid;
    }

    private static TextBlock MutedText(string text, double size, FontWeight? weight = null, Thickness? margin = null)
    {
        var tb = new TextBlock
        {
            Text = text, FontSize = size, FontWeight = weight ?? FontWeights.Normal,
            Margin = margin ?? new Thickness(0),
        };
        tb.SetResourceReference(TextBlock.ForegroundProperty, "Brush.TextMuted");
        return tb;
    }

    private static SolidColorBrush Hex(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));

    private void Soon_Click(object sender, RoutedEventArgs e) => Toaster.Soon(sender);

    private void Search_Click(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        Toaster.Show("Search is coming soon.");
    }

    private void Compose_Click(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        Toaster.Show("Sending messages is coming soon.");
    }
}
