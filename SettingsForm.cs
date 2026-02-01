using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpotifyHotkeyController
{
    public partial class SettingsForm : Form
    {
        private HotkeyConfig config;
        private HotkeyTextBox txtPlayPause = null!;
        private HotkeyTextBox txtNextTrack = null!;
        private HotkeyTextBox txtPreviousTrack = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Button btnReset = null!;

        public event EventHandler? HotkeysChanged;

        public SettingsForm(HotkeyConfig currentConfig)
        {
            config = new HotkeyConfig
            {
                PlayPause = new HotkeyDefinition
                {
                    Modifiers = currentConfig.PlayPause.Modifiers,
                    Key = currentConfig.PlayPause.Key
                },
                NextTrack = new HotkeyDefinition
                {
                    Modifiers = currentConfig.NextTrack.Modifiers,
                    Key = currentConfig.NextTrack.Key
                },
                PreviousTrack = new HotkeyDefinition
                {
                    Modifiers = currentConfig.PreviousTrack.Modifiers,
                    Key = currentConfig.PreviousTrack.Key
                }
            };

            InitializeComponent();
            LoadCurrentHotkeys();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Hotkey Settings";
            this.Size = new Size(500, 380);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(32, 32, 36);
            this.ForeColor = Color.White;

            // Title label
            var lblTitle = new Label
            {
                Text = "Configure Hotkeys",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 25)
            };
            this.Controls.Add(lblTitle);

            // Subtitle
            var lblSubtitle = new Label
            {
                Text = "Click on a field and press your desired key combination",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                AutoSize = true,
                Location = new Point(30, 62)
            };
            this.Controls.Add(lblSubtitle);

            int yPos = 110;
            int spacing = 75;

            // Play/Pause hotkey
            CreateHotkeyRow("Play / Pause", ref txtPlayPause, yPos);
            yPos += spacing;

            // Next Track hotkey
            CreateHotkeyRow("Next Track", ref txtNextTrack, yPos);
            yPos += spacing;

            // Previous Track hotkey
            CreateHotkeyRow("Previous Track", ref txtPreviousTrack, yPos);

            // Buttons
            btnReset = new Button
            {
                Text = "Reset to Default",
                Location = new Point(30, 305),
                Size = new Size(130, 36),
                BackColor = Color.FromArgb(50, 50, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 74);
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 305),
                Size = new Size(90, 36),
                BackColor = Color.FromArgb(50, 50, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 74);
            this.Controls.Add(btnCancel);

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(380, 305),
                Size = new Size(90, 36),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            this.CancelButton = btnCancel;
            this.AcceptButton = btnSave;
        }

        private void CreateHotkeyRow(string labelText, ref HotkeyTextBox textBox, int yPos)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, yPos)
            };
            this.Controls.Add(label);

            textBox = new HotkeyTextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(440, 35),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11),
                ReadOnly = true,
                Cursor = Cursors.Hand
            };
            this.Controls.Add(textBox);
        }

        private void LoadCurrentHotkeys()
        {
            txtPlayPause.Hotkey = config.PlayPause;
            txtNextTrack.Hotkey = config.NextTrack;
            txtPreviousTrack.Hotkey = config.PreviousTrack;
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Reset all hotkeys to default values?",
                "Reset Hotkeys",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var defaultConfig = new HotkeyConfig();
                txtPlayPause.Hotkey = defaultConfig.PlayPause;
                txtNextTrack.Hotkey = defaultConfig.NextTrack;
                txtPreviousTrack.Hotkey = defaultConfig.PreviousTrack;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validate all hotkeys are set
            if (txtPlayPause.Hotkey.Key == System.Windows.Forms.Keys.None ||
                txtNextTrack.Hotkey.Key == System.Windows.Forms.Keys.None ||
                txtPreviousTrack.Hotkey.Key == System.Windows.Forms.Keys.None)
            {
                MessageBox.Show(
                    "Please set all hotkeys before saving.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Update config
            config.PlayPause = txtPlayPause.Hotkey;
            config.NextTrack = txtNextTrack.Hotkey;
            config.PreviousTrack = txtPreviousTrack.Hotkey;

            // Save to file
            config.Save();

            // Notify main form
            HotkeysChanged?.Invoke(this, EventArgs.Empty);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    /// <summary>
    /// Custom TextBox that captures hotkey combinations.
    /// </summary>
    public class HotkeyTextBox : TextBox
    {
        private HotkeyDefinition hotkey = new HotkeyDefinition();

        public HotkeyDefinition Hotkey
        {
            get => hotkey;
            set
            {
                hotkey = value;
                UpdateDisplay();
            }
        }

        public HotkeyTextBox()
        {
            this.KeyDown += HotkeyTextBox_KeyDown;
            this.Enter += (s, e) => this.BackColor = Color.FromArgb(60, 60, 64);
            this.Leave += (s, e) => this.BackColor = Color.FromArgb(45, 45, 48);
        }

        private void HotkeyTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;

            // Ignore modifier-only presses - using full type names to avoid ambiguity
            Keys keyCode = e.KeyCode;
            if (keyCode == Keys.ControlKey || 
                keyCode == Keys.ShiftKey ||
                keyCode == Keys.Menu || 
                keyCode == Keys.LWin || 
                keyCode == Keys.RWin)
            {
                return;
            }

            // Build modifiers
            SpotifyHotkeyController.ModifierKeys mods = SpotifyHotkeyController.ModifierKeys.None;
            if (e.Control) mods |= SpotifyHotkeyController.ModifierKeys.Control;
            if (e.Alt) mods |= SpotifyHotkeyController.ModifierKeys.Alt;
            if (e.Shift) mods |= SpotifyHotkeyController.ModifierKeys.Shift;

            // Set hotkey
            hotkey.Modifiers = mods;
            hotkey.Key = keyCode;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            this.Text = hotkey.ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return true; // Prevent default processing
        }
    }
}
