[Setup]
AppName=Spotify Hotkey Controller
AppVersion=1.0
AppPublisher=UmutK
DefaultDirName={autopf}\SpotifyHotkeyController
DefaultGroupName=Spotify Hotkey Controller
AllowNoIcons=yes
OutputBaseFilename=SpotifyHotkeyControllerSetup
Compression=lzma2
SolidCompression=yes
OutputDir=Installer
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startup"; Description: "Automatically start on login"; GroupDescription: "Additional tasks:"; Flags: unchecked

[Files]
; IMPORTANT: GitHub Action must build to this path first
Source: "bin\Release\net8.0-windows\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Spotify Hotkey Controller"; Filename: "{app}\SpotifyHotkeyController.exe"
Name: "{autodesktop}\Spotify Hotkey Controller"; Filename: "{app}\SpotifyHotkeyController.exe"; Tasks: desktopicon
; Startup shortcut (if task selected)
Name: "{userstartup}\Spotify Hotkey Controller"; Filename: "{app}\SpotifyHotkeyController.exe"; Tasks: startup

[Run]
Filename: "{app}\SpotifyHotkeyController.exe"; Description: "{cm:LaunchProgram,Spotify Hotkey Controller}"; Flags: nowait postinstall skipifsilent
