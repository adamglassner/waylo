; Inno Setup script for RingHawk
; Build: run ..\build.ps1, or compile this file with Inno Setup 6 (ISCC.exe) after publishing.
;
; Installs per-user by default (no administrator prompt). The first page lets people choose
; "all users" instead, which installs under Program Files.
;
; The Bytehawk Studio Launcher runs this setup silently to install and update RingHawk:
;   RingHawk-<version>-Setup.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP- /NOICONS /DIR="<folder>"

#define MyAppName "RingHawk"
#ifndef MyAppVersion
  #define MyAppVersion "0.1.0"
#endif
#define MyAppPublisher "Bytehawk Studio"
#define MyAppExeName "RingHawk.exe"
#define MyAppUrl "https://github.com/adamglassner/ringhawk"
#ifndef PublishDir
  #define PublishDir "..\dist\publish"
#endif

[Setup]
AppId={{A5A5766E-4DBA-4628-8D00-FD74C4C0587D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppUrl}
AppSupportURL={#MyAppUrl}/issues
AppUpdatesURL={#MyAppUrl}/releases
AppCopyright=© 2026 Bytehawk Studio
VersionInfoVersion={#MyAppVersion}.0
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} Setup
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
DefaultDirName={autopf}\RingHawk
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
OutputDir=..\dist
OutputBaseFilename=RingHawk-{#MyAppVersion}-Setup
SetupIconFile=..\src\RingHawk.Desktop\Assets\ringhawk.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
LicenseFile=..\LICENSE
WizardStyle=modern
WizardImageFile=wizard-large-1.bmp,wizard-large-2.bmp
WizardSmallImageFile=wizard-small-55.bmp,wizard-small-110.bmp
Compression=lzma2/max
SolidCompression=yes
; Ask to close RingHawk if it's running (Windows Restart Manager)
CloseApplications=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "{#PublishDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; DestName: "LICENSE.txt"; Flags: ignoreversion

[InstallDelete]
; RingHawk was called Waylo in 0.1.0. Same AppId, so this upgrades a Waylo install:
; remove the old exe and its Start Menu / desktop shortcuts.
Type: files; Name: "{app}\Waylo.exe"
Type: filesandordirs; Name: "{autoprograms}\Waylo"
Type: files; Name: "{autodesktop}\Waylo.lnk"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

; NOTE: There is deliberately no [UninstallDelete] section.
; Uninstalling removes the program and its shortcuts only. Settings in
; %APPDATA%\RingHawk\ are left alone so a reinstall picks up where you left off.
