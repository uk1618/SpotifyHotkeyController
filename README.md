# Spotify Hotkey Controller

A lightweight Windows Forms application (.NET 8) that allows you to control Spotify (or any media player) while gaming using global hotkeys that simulate Windows media keys.

## Features

- **Global Hotkeys** - Control Spotify from anywhere, even when the app is in the background:
  - `Ctrl + F9` → Play / Pause
  - `Ctrl + F10` → Next track
  - `Ctrl + F8` → Previous track

- **Silent Background Operation** - Runs invisibly with no window or startup flicker

- **System Tray Icon** - Minimal tray presence with right-click menu for quick exit

- **Native Windows APIs** - Uses only built-in Windows APIs (no external dependencies)

## How It Works

The application uses Windows API functions via P/Invoke:

1. **RegisterHotKey** (user32.dll) - Registers global hotkeys with Windows
2. **WndProc Override** - Listens for `WM_HOTKEY` messages when hotkeys are pressed
3. **keybd_event** (user32.dll) - Simulates media key presses:
   - `VK_MEDIA_PLAY_PAUSE` (0xB3)
   - `VK_MEDIA_NEXT_TRACK` (0xB0)
   - `VK_MEDIA_PREV_TRACK` (0xB1)

## Building and Running

### Prerequisites

- Windows 10/11
- .NET 8.0 SDK or later
- Visual Studio 2022 (recommended) or Visual Studio Code

### Build with Visual Studio

1. Open the project folder in Visual Studio 2022
2. Visual Studio will automatically recognize the `.csproj` file
3. Press `F5` to build and run, or `Ctrl+Shift+B` to build only
4. The application will start minimized in the system tray

### Build with Command Line

```powershell
# Navigate to the project directory
cd c:\Users\umutk\OneDrive\Desktop\0\projects\new\SpotifyHotkeyController

# Build the project
dotnet build

# Run the application
dotnet run
```

### Build Release Version

```powershell
# Build optimized release version
dotnet build -c Release

# The executable will be in:
# bin\Release\net8.0-windows\SpotifyHotkeyController.exe
```

## Usage

1. **Start the Application** - Run the executable or press F5 in Visual Studio
2. **Look for the Tray Icon** - A small white circle icon will appear in the system tray
3. **Use the Hotkeys**:
   - Press `Ctrl + F9` to play/pause Spotify
   - Press `Ctrl + F10` to skip to the next track
   - Press `Ctrl + F8` to go to the previous track
4. **Configure Hotkeys** - Right-click the tray icon and select "Settings..." to change key bindings
5. **Exit** - Right-click the tray icon and select "Exit"

## Customization

You can change hotkeys directly via the Settings UI in the application.

Configuration is saved to: `%APPDATA%\SpotifyHotkeyController\config.json`

## Troubleshooting

**Hotkeys not working:**
- Make sure no other application has registered the same hotkeys
- Try running the application as Administrator
- Check if Spotify is running

**Application won't start:**
- Ensure you have .NET 8.0 Runtime installed
- Check Windows Event Viewer for error messages

**Tray icon not visible:**
- Check if the system tray is set to hide icons
- Look in the "Show hidden icons" overflow area

## Technical Details

- **Framework**: .NET 8.0
- **UI Framework**: Windows Forms
- **Target OS**: Windows 10/11
- **Architecture**: Any CPU
- **Dependencies**: None (uses only native Windows APIs)

## License

This is a demonstration project. Feel free to use and modify as needed.
