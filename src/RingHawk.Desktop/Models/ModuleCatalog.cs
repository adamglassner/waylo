namespace RingHawk.Models;

/// <summary>
/// Every section of RingHawk, in sidebar order (matches the RingHawk mockup's Hawkline sidebar).
/// Chats has its own preview page (made-up sample conversations); everything else uses the
/// generic ComingSoonView built from this data. Add a module here and it appears in the sidebar.
/// </summary>
public static class ModuleCatalog
{
    public static readonly ModuleInfo Home = new()
    {
        Key = "chats", Title = "Chats", Glyph = "", IsComingSoon = false,
        Tagline = "Your conversations with family and friends.",
    };

    public static readonly IReadOnlyList<ModuleInfo> Sidebar =
    [
        Home,
        new()
        {
            Key = "calls", Title = "Calls", Glyph = "",
            Tagline = "Voice and video calls, and your call history.",
            Description = "Call anyone in your contacts with one click, by voice or video, and switch from voice to video in the middle of a call. Calls are end-to-end encrypted, and your recent calls, including missed ones, are listed here so you can call back.",
            PrimaryAction = "New call",
            Kpis = ["Calls today", "Missed", "Talk time this week", "Call quality"],
            Columns = ["Name", "Type", "Duration", "When"],
            Features =
            [
                "Voice and HD video calls",
                "Switch from voice to video mid-call",
                "Group calls with the whole family",
                "Screen sharing",
                "Mute, speaker, hold and keypad",
                "Recent and missed calls with call back",
                "End-to-end encryption",
                "Call quality indicator",
            ],
        },
        new()
        {
            Key = "contacts", Title = "Contacts", Glyph = "",
            Tagline = "The people you talk to, and whether they're around.",
            Description = "Add people by their RingHawk name, email or an invite link, see who's available, away or busy at a glance, and keep your favorites pinned to the top of the sidebar.",
            PrimaryAction = "Add contact",
            Kpis = ["Contacts", "Online now", "Favorites", "Pending invites"],
            Columns = ["Name", "Status", "Last seen", "Groups"],
            Features =
            [
                "Available, away, busy and offline status",
                "Favorites pinned in the sidebar",
                "Invite people with a link",
                "Contact groups like Family",
                "Block and report",
                "Custom status message",
            ],
        },
        new()
        {
            Key = "meetings", Title = "Meetings", Glyph = "",
            Tagline = "Group video calls you can plan ahead.",
            Description = "Start a group video call right now or schedule one for later, and share a link so everyone joins at the right time. In the meeting you get a grid of everyone's video, a chat on the side, raise hand, reactions and screen sharing.",
            PrimaryAction = "Start meeting",
            Kpis = ["Upcoming", "This week", "People invited", "Hours together"],
            Columns = ["Meeting", "When", "People", "Link"],
            Features =
            [
                "Meet now or schedule for later",
                "Share a link to join",
                "Video grid with the active speaker highlighted",
                "In-call chat and people list",
                "Raise hand and reactions",
                "Screen sharing",
                "Reminders before it starts",
                "End-to-end encryption",
            ],
        },
        new()
        {
            Key = "voicemail", Title = "Voicemail", Glyph = "",
            Tagline = "Messages people leave when you miss a call.",
            Description = "When you can't pick up, callers can leave a voice or video message. Listen to them here, read a written version of each one, and call back with a click.",
            PrimaryAction = "Voicemail settings",
            Kpis = ["New", "Saved", "Total length", "Storage"],
            Columns = ["From", "Length", "Received", "Status"],
            Features =
            [
                "Voice and video messages",
                "Written version of each message",
                "Call back from the message",
                "Your own greeting",
                "Unread count in the sidebar",
            ],
        },
    ];

    public static ModuleInfo? Find(string key) => Sidebar.FirstOrDefault(m => m.Key == key);
}
