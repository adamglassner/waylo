using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RingHawk.Models;
using RingHawk.Services;

namespace RingHawk.Views;

public partial class ComingSoonView : UserControl
{
    private static readonly string[] DefaultColumns = ["Name", "Details", "Updated", "Status"];

    public ComingSoonView(ModuleInfo module)
    {
        InitializeComponent();
        DataContext = module;
        KpiPanel.Visibility = module.Kpis.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        BuildSkeletonTable(module.Columns.Length > 0 ? module.Columns : DefaultColumns, module.Key);
    }

    private static Border Bar(double width, Thickness margin)
    {
        var bar = new Border
        {
            Height = 9,
            Width = width,
            CornerRadius = new CornerRadius(4.5),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = margin,
        };
        bar.SetResourceReference(Border.BackgroundProperty, "Brush.Skeleton");
        return bar;
    }

    /// <summary>Draws a table header and grey placeholder rows behind the "Nothing here yet" message.</summary>
    private void BuildSkeletonTable(string[] columns, string seedKey)
    {
        var random = new Random(seedKey.Length * 31 + columns.Length);
        var grid = new Grid { Opacity = 0.9 };
        foreach (var _ in columns) grid.ColumnDefinitions.Add(new ColumnDefinition());

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        var header = new Border();
        header.SetResourceReference(Border.BackgroundProperty, "Brush.SurfaceAlt");
        Grid.SetColumnSpan(header, columns.Length);
        grid.Children.Add(header);

        for (int c = 0; c < columns.Length; c++)
        {
            var text = new TextBlock
            {
                Text = columns[c].ToUpperInvariant(),
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(20, 12, 8, 12),
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            text.SetResourceReference(TextBlock.ForegroundProperty, "Brush.TextMuted");
            Grid.SetColumn(text, c);
            grid.Children.Add(text);
        }

        const int rows = 6;
        for (int r = 1; r <= rows; r++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var line = new Border { BorderThickness = new Thickness(0, 1, 0, 0) };
            line.SetResourceReference(Border.BorderBrushProperty, "Brush.Border");
            Grid.SetRow(line, r);
            Grid.SetColumnSpan(line, columns.Length);
            grid.Children.Add(line);

            for (int c = 0; c < columns.Length; c++)
            {
                var bar = Bar(random.Next(36, 110), new Thickness(20, 16, 8, 16));
                Grid.SetRow(bar, r);
                Grid.SetColumn(bar, c);
                grid.Children.Add(bar);
            }
        }

        TableHost.Children.Insert(0, grid);
    }

    private void Soon_Click(object sender, RoutedEventArgs e) => Toaster.Soon(sender);

    private void Search_Click(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        Toaster.Show("Search is coming soon.");
    }
}
