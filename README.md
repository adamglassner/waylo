# Waylo

<img src="assets/waylo-256.png" alt="Waylo icon" width="96" align="right">

**Video calls, voice calls and chat with your family and friends, in the spirit of Skype.**

By Bytehawk Studio. Windows first.

> **Early preview (0.1.0).** This is the app's shell: the sidebar, the Chats preview from the Waylo mockup
> (with made-up sample people and messages), Settings and light/dark mode all work. Every other section is a
> "coming soon" page describing what it will do. Calling and chatting aren't built yet.

| Section | Status |
|---|---|
| **Chats** | Preview with sample conversations: chat list, an open conversation, call buttons and the message box |
| Calls | Coming soon (voice and video calls, recent and missed calls) |
| Contacts | Coming soon (available, away, busy and offline status, favorites, invites) |
| Meetings | Coming soon (group video calls now or scheduled, with a link to join) |
| Voicemail | Coming soon (voice and video messages when you miss a call) |
| Settings | Light, dark or match Windows works now. Account, audio and video, notifications and sounds, and privacy are coming soon |

Each coming-soon page lists its planned features. They are defined in
[`Models/ModuleCatalog.cs`](src/Waylo.Desktop/Models/ModuleCatalog.cs).

## Download

Windows 10/11 (x64). Install it from the **[Bytehawk Studio Launcher](https://github.com/adamglassner/bytehawklauncher)**,
which downloads the newest release, installs it and keeps it up to date. Or get it from the
[latest release](../../releases/latest):

- **`Waylo-<version>-Setup.exe`**: installer with Start Menu shortcut, optional desktop shortcut and an
  uninstaller. Installs just for you by default (no administrator prompt).
- **`Waylo-<version>-win-x64.zip`**: portable. Unzip anywhere and run `Waylo.exe`.

Neither needs .NET installed. Settings are saved in `%AppData%\Waylo\`.

## Releasing a new version

Change `<Version>` in `src/Waylo.Desktop/Waylo.Desktop.csproj` (for example to `0.1.1`), add a
[CHANGELOG](CHANGELOG.md) entry and push to `main`. GitHub Actions ([build-release.yml](.github/workflows/build-release.yml))
then, on a Windows machine:

1. builds the single-file exe, the portable zip and the installer,
2. runs the self-test (`Waylo.exe --self-test result.txt`): every page in light and dark, plus navigation checks,
3. silently installs the Setup .exe to a custom folder (the way the launcher does), starts it, and uninstalls it,
4. takes light/dark screenshots (attached to the run as artifacts),
5. publishes GitHub Release `v<version>` with the Setup .exe and the zip.

Installed copies pick up the new version through the Bytehawk Studio Launcher. Pushes that don't change the
version, and pull requests, run steps 1 to 4 only.

## Building it yourself

Requirements: Windows 10/11, the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), and
[Inno Setup 6](https://jrsoftware.org/isinfo.php) for the installer (`winget install JRSoftware.InnoSetup`).

- Run from source: double-click `run.bat`, run `dotnet run --project src\Waylo.Desktop`, or open `Waylo.sln`
  in Visual Studio 2022 and press F5.
- Build everything: `.\build.ps1` makes `dist\Waylo.exe`, `dist\Waylo-<version>-win-x64.zip` and
  `dist\Waylo-<version>-Setup.exe`. Use `-SkipInstaller` to skip Inno Setup.

Command-line options: `--page <section>` opens a page at startup (for example `--page calls` or `--page settings`),
and `--self-test <file>` runs the checks above and exits with the number of failures.

## Project layout

```
src/Waylo.Desktop/
  App.xaml(.cs)          app startup + light/dark theme switching
  MainWindow.xaml(.cs)   Hawkline sidebar, top bar, page host, status bar, toast
  Assets/                app icon (.ico + .png)
  Themes/                Light.xaml, Dark.xaml (colors), Controls.xaml (styles)
  Models/                ModuleCatalog.cs — every section, its planned features
  Views/                 ChatsView (sample-conversation preview), ComingSoonView, SettingsView
  Services/              settings, Windows theme detection, toast helper, self-test
installer/Waylo.iss      Inno Setup script (+ wizard images)
build.ps1                exe + portable zip + installer
.github/workflows/       CI: build, self-test, installer test, screenshots, release
assets/                  the Waylo logo (waylo.svg) and PNG exports
tools/make_icon.py       regenerates the icon from assets/waylo.svg (Python + Pillow + rsvg-convert)
tools/make_wizard_images.py  regenerates the installer wizard images
```

To add a coming-soon section, add an entry to `Models/ModuleCatalog.cs`.
To build a section for real, give it its own view in `Views/`, set `IsComingSoon = false` and show that view
from `MainWindow.Show`.

## Design

Waylo uses Bytehawk Studio's **Hawkline** style (the navy left sidebar from Guardrail EHS and WillowWallet) with
Waylo blue (`#1A6FD1`). The Chats page follows the Waylo mockup's Chats screen; the mockup's video call and voice
call screens are the plan for Calls and Meetings.

The logo is a white speech bubble with a "W" drawn as a wave: wave + hello = Waylo.

## License

MIT. © 2026 Bytehawk Studio.
