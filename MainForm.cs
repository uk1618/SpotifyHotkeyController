using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpotifyHotkeyController
{
    public partial class MainForm : Form
    {
        #region Windows API Declarations

        // P/Invoke declarations for Windows API functions

        /// <summary>
        /// Registers a global hotkey with Windows.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        /// <summary>
        /// Unregisters a previously registered hotkey.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Simulates a key press event.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // Constants for hotkey modifiers
        private const uint MOD_CONTROL = 0x0002;

        // Virtual key codes for F8, F9, F10
        private const uint VK_F8 = 0x77;
        private const uint VK_F9 = 0x78;
        private const uint VK_F10 = 0x79;

        // Virtual key codes for media keys
        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        private const byte VK_MEDIA_PREV_TRACK = 0xB1;

        // Key event flags
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Windows message for hotkey
        private const int WM_HOTKEY = 0x0312;

        // Hotkey IDs (must be unique for this application)
        private const int HOTKEY_ID_PREV = 1;  // Ctrl + F8
        private const int HOTKEY_ID_PLAY_PAUSE = 2;  // Ctrl + F9
        private const int HOTKEY_ID_NEXT = 3;  // Ctrl + F10

        #endregion

        #region System Tray

        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;
        private HotkeyConfig config;

        #endregion

        public MainForm()
        {
            InitializeComponent();
            
            // Load configuration
            config = HotkeyConfig.Load();
            
            // Configure form to start hidden and not show in taskbar
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            
            // Set up system tray icon
            SetupSystemTray();
        }

        /// <summary>
        /// Called after the form handle is created and the form is loaded.
        /// This is the appropriate place to register hotkeys since we need a valid handle.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // Make sure the handle is created
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            
            // Now register global hotkeys with the valid handle
            RegisterHotkeys();
            
            // Hide the form after everything is set up
            this.Visible = false;
        }

        /// <summary>
        /// Sets up the system tray icon and context menu.
        /// </summary>
        private void SetupSystemTray()
        {
            // Create context menu for tray icon
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Spotify Hotkey Controller", null, null).Enabled = false;
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Settings...", null, OnSettings);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Exit", null, OnExit);

            // Create tray icon
            trayIcon = new NotifyIcon
            {
                Text = "Spotify Hotkey Controller",
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            // Create a simple icon (white circle on transparent background)
            // In production, you'd use a proper .ico file
            using (var bmp = new System.Drawing.Bitmap(16, 16))
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.Clear(System.Drawing.Color.Transparent);
                g.FillEllipse(System.Drawing.Brushes.White, 2, 2, 12, 12);
                g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Gray, 1), 2, 2, 12, 12);
                trayIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
            }

            // Double-click shows info
            trayIcon.DoubleClick += (s, e) =>
            {
                MessageBox.Show(
                    "Spotify Hotkey Controller is running.\n\n" +
                    "Hotkeys:\n" +
                    "• Ctrl + F8: Previous Track\n" +
                    "• Ctrl + F9: Play/Pause\n" +
                    "• Ctrl + F10: Next Track\n\n" +
                    "Right-click the tray icon to exit.",
                    "Spotify Hotkey Controller",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
        }

        /// <summary>
        /// Registers all global hotkeys with Windows using the current configuration.
        /// </summary>
        private void RegisterHotkeys()
        {
            // Previous Track
            if (!RegisterHotKey(this.Handle, HOTKEY_ID_PREV, 
                config.PreviousTrack.ToWinApiModifiers(), 
                (uint)config.PreviousTrack.Key))
            {
                MessageBox.Show($"Failed to register {config.PreviousTrack} hotkey.\nIt may already be in use.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Play/Pause
            if (!RegisterHotKey(this.Handle, HOTKEY_ID_PLAY_PAUSE, 
                config.PlayPause.ToWinApiModifiers(), 
                (uint)config.PlayPause.Key))
            {
                MessageBox.Show($"Failed to register {config.PlayPause} hotkey.\nIt may already be in use.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Next Track
            if (!RegisterHotKey(this.Handle, HOTKEY_ID_NEXT, 
                config.NextTrack.ToWinApiModifiers(), 
                (uint)config.NextTrack.Key))
            {
                MessageBox.Show($"Failed to register {config.NextTrack} hotkey.\nIt may already be in use.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Unregisters all global hotkeys.
        /// </summary>
        private void UnregisterHotkeys()
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_PREV);
            UnregisterHotKey(this.Handle, HOTKEY_ID_PLAY_PAUSE);
            UnregisterHotKey(this.Handle, HOTKEY_ID_NEXT);
        }

        /// <summary>
        /// Simulates a media key press.
        /// Sends both key down and key up events to properly trigger the media key.
        /// </summary>
        /// <param name="vkMediaKey">Virtual key code of the media key to simulate</param>
        private void SimulateMediaKey(byte vkMediaKey)
        {
            // Press the key down
            keybd_event(vkMediaKey, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
            
            // Release the key
            keybd_event(vkMediaKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        /// <summary>
        /// Overrides the window procedure to handle WM_HOTKEY messages.
        /// This is called by Windows when a registered hotkey is pressed.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            // Check if this is a hotkey message
            if (m.Msg == WM_HOTKEY)
            {
                // The wParam contains the hotkey ID
                int hotkeyId = m.WParam.ToInt32();

                // Handle each hotkey by simulating the corresponding media key
                switch (hotkeyId)
                {
                    case HOTKEY_ID_PREV:
                        // Ctrl + F8 → Previous Track
                        SimulateMediaKey(VK_MEDIA_PREV_TRACK);
                        break;

                    case HOTKEY_ID_PLAY_PAUSE:
                        // Ctrl + F9 → Play/Pause
                        SimulateMediaKey(VK_MEDIA_PLAY_PAUSE);
                        break;

                    case HOTKEY_ID_NEXT:
                        // Ctrl + F10 → Next Track
                        SimulateMediaKey(VK_MEDIA_NEXT_TRACK);
                        break;
                }
            }

            // Always call the base implementation
            base.WndProc(ref m);
        }

        /// <summary>
        /// Handles the Settings menu item click.
        /// </summary>
        private void OnSettings(object? sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(config);
            settingsForm.HotkeysChanged += (s, args) =>
            {
                // Reload config and re-register hotkeys
                UnregisterHotkeys();
                config = HotkeyConfig.Load();
                RegisterHotkeys();
                
                MessageBox.Show(
                    "Hotkeys updated successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            settingsForm.ShowDialog();
        }

        /// <summary>
        /// Handles the Exit menu item click.
        /// </summary>
        private void OnExit(object? sender, EventArgs e)
        {
            // Clean up and exit
            Application.Exit();
        }

        /// <summary>
        /// Clean up resources when the form is closing.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Unregister all hotkeys
            UnregisterHotkeys();

            // Clean up tray icon
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            base.OnFormClosing(e);
        }


    }
}
