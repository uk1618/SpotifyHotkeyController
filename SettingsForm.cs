using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SpotifyHotkeyController
{
    public partial class SettingsForm : Form
    {
        // Modern Color Palette
        private readonly Color BackColorDark = Color.FromArgb(32, 32, 32);
        private readonly Color SurfaceColor = Color.FromArgb(45, 45, 48);
        private readonly Color BorderColor = Color.FromArgb(60, 60, 60);
        private readonly Color AccentColor = Color.FromArgb(0, 122, 204); // VS Blue
        private readonly Color AccentHoverColor = Color.FromArgb(28, 151, 234);
        private readonly Color TextPrimary = Color.FromArgb(240, 240, 240);
        private readonly Color TextSecondary = Color.FromArgb(160, 160, 160);
        private readonly Color ErrorColor = Color.FromArgb(231, 76, 60);

        private HotkeyConfig config;
        
        // Custom Controls
        private ModernHotkeyControl txtPlayPause = null!;
        private ModernHotkeyControl txtNextTrack = null!;
        private ModernHotkeyControl txtPreviousTrack = null!;
        private ModernButton btnSave = null!;
        private ModernButton btnCancel = null!;
        private ModernButton btnReset = null!;

        public event EventHandler? HotkeysChanged;

        public SettingsForm(HotkeyConfig currentConfig)
        {
            // Clone config
            config = new HotkeyConfig
            {
                PlayPause = new HotkeyDefinition { Modifiers = currentConfig.PlayPause.Modifiers, Key = currentConfig.PlayPause.Key },
                NextTrack = new HotkeyDefinition { Modifiers = currentConfig.NextTrack.Modifiers, Key = currentConfig.NextTrack.Key },
                PreviousTrack = new HotkeyDefinition { Modifiers = currentConfig.PreviousTrack.Modifiers, Key = currentConfig.PreviousTrack.Key }
            };

            InitializeComponent();
            LoadCurrentHotkeys();
        }

        private void InitializeComponent()
        {
            // Form Setup
            this.Text = "Hotkey Settings";
            this.Size = new Size(600, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BackColorDark;
            this.ForeColor = TextPrimary;
            this.DoubleBuffered = true;

            // Padding container
            int px = 40;
            int py = 30;

            // 1. Header Section
            var lblTitle = new Label
            {
                Text = "Controller Settings",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(px, py)
            };
            this.Controls.Add(lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Customize your global media shortcuts.",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(px, py + 40)
            };
            this.Controls.Add(lblSubtitle);

            // 2. Hotkey Section
            int startY = 110;
            int spacing = 85;
            int inputOffsetX = 260;

            // Play/Pause
            CreateHotkeySection("Play / Pause", "Toggles playback state", ref txtPlayPause, px, startY, inputOffsetX);
            
            // Next
            CreateHotkeySection("Next Track", "Skips to the next song", ref txtNextTrack, px, startY + spacing, inputOffsetX);
            
            // Prev
            CreateHotkeySection("Previous Track", "Returns to previous song", ref txtPreviousTrack, px, startY + spacing * 2, inputOffsetX);

            // 3. Action Buttons (Bottom)
            int btnHeight = 40;
            int btnY = 460;

            // Reset (Left)
            btnReset = new ModernButton
            {
                Text = "Reset Defaults",
                Location = new Point(px, btnY),
                Size = new Size(130, btnHeight),
                BackColor = SurfaceColor,
                ForeColor = TextSecondary,
                HoverColor = Color.FromArgb(60, 60, 65)
            };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            // Save (Right)
            btnSave = new ModernButton
            {
                Text = "Save Changes",
                Location = new Point(this.ClientSize.Width - px - 150, btnY),
                Size = new Size(150, btnHeight),
                BackColor = AccentColor,
                ForeColor = Color.White,
                HoverColor = AccentHoverColor, // Bright Blue
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel (Left of Save)
            btnCancel = new ModernButton
            {
                Text = "Cancel",
                Location = new Point(btnSave.Left - 110 - 20, btnY),
                Size = new Size(110, btnHeight),
                BackColor = SurfaceColor, // Match Reset button style
                ForeColor = TextSecondary,
                HoverColor = Color.FromArgb(60, 60, 65)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.CancelButton = btnCancel;
            this.AcceptButton = btnSave;
        }

        private void CreateHotkeySection(string title, string desc, ref ModernHotkeyControl control, int x, int y, int offsetX)
        {
            // Title Label
            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 11),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(x, y)
            };
            this.Controls.Add(lbl);

            // Control
            control = new ModernHotkeyControl
            {
                Location = new Point(x + offsetX, y - 5), // Use the new wider offset
                Size = new Size(240, 40),
                ParentBackColor = this.BackColor
            };
            
            // Description under title
            var lblDesc = new Label
            {
                Text = desc,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(x, y + 25)
            };
            this.Controls.Add(lblDesc);

            this.Controls.Add(control);
        }

        private void LoadCurrentHotkeys()
        {
            txtPlayPause.Hotkey = config.PlayPause;
            txtNextTrack.Hotkey = config.NextTrack;
            txtPreviousTrack.Hotkey = config.PreviousTrack;
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Reset all hotkeys?", "Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var def = new HotkeyConfig();
                txtPlayPause.Hotkey = def.PlayPause;
                txtNextTrack.Hotkey = def.NextTrack;
                txtPreviousTrack.Hotkey = def.PreviousTrack;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validate
            if (txtPlayPause.Hotkey.Key == System.Windows.Forms.Keys.None ||
                txtNextTrack.Hotkey.Key == System.Windows.Forms.Keys.None ||
                txtPreviousTrack.Hotkey.Key == System.Windows.Forms.Keys.None)
            {
                MessageBox.Show("All hotkeys must be set.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save
            config.PlayPause = txtPlayPause.Hotkey;
            config.NextTrack = txtNextTrack.Hotkey;
            config.PreviousTrack = txtPreviousTrack.Hotkey;
            config.Save();

            HotkeysChanged?.Invoke(this, EventArgs.Empty);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // --- Custom Painting for Form Background (Optional: Gradient) ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Subtle gradient for premium feel
            using (var brush = new LinearGradientBrush(this.ClientRectangle, 
                   Color.FromArgb(32, 32, 32), 
                   Color.FromArgb(28, 28, 28), 
                   LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }

    /// <summary>
    /// Modern Button with rounded corners and hover animations
    /// </summary>
    public class ModernButton : Button
    {
        public Color HoverColor { get; set; } = Color.Gray;
        private bool isHovered = false;

        public ModernButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 9);
            
            this.MouseEnter += (s, e) => { isHovered = true; Invalidate(); };
            this.MouseLeave += (s, e) => { isHovered = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);
            var color = isHovered ? HoverColor : BackColor;

            // 1. Fill the entire rectangle with the PARENT's background color first
            // This "erases" the default button background behind the rounded corners
            if (Parent != null)
            {
                using (var bgBrush = new SolidBrush(Parent.BackColor))
                {
                    g.FillRectangle(bgBrush, rect);
                }
            }
            else
            {
                 g.Clear(Color.Black); // Fallback
            }

            // 2. Draw Rounded Button Background
            using (var path = GetRoundedPath(rect, 8))
            using (var brush = new SolidBrush(color))
            {
                g.FillPath(brush, path);
            }

            // 3. Draw Text
            TextRenderer.DrawText(g, Text, Font, rect, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// A visual control to look like a modern input chip for hotkeys.
    /// Captures keys and displays them nicely.
    /// </summary>
    public class ModernHotkeyControl : Control
    {
        private HotkeyDefinition hotkey = new HotkeyDefinition();
        private bool isFocused = false;
        private SolidBrush bgBrush;
        private Pen borderPen;

        public Color ParentBackColor { get; set; } = Color.Black;

        public HotkeyDefinition Hotkey
        {
            get => hotkey;
            set { hotkey = value; Invalidate(); }
        }

        public ModernHotkeyControl()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable, true);
            this.Size = new Size(200, 40);
            this.Cursor = Cursors.Hand;
            
            bgBrush = new SolidBrush(Color.FromArgb(45, 45, 48));
            borderPen = new Pen(Color.FromArgb(60, 60, 60), 1);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
        }

        protected override void OnEnter(EventArgs e)
        {
            isFocused = true;
            borderPen.Color = Color.FromArgb(0, 122, 204); // Highlight color
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            isFocused = false;
            borderPen.Color = Color.FromArgb(60, 60, 60); // Normal color
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;

            // Filter
            var k = e.KeyCode;
            if (k == System.Windows.Forms.Keys.ControlKey || k == System.Windows.Forms.Keys.ShiftKey || 
                k == System.Windows.Forms.Keys.Menu || k == System.Windows.Forms.Keys.LWin || k == System.Windows.Forms.Keys.RWin)
                return;

            // Build
            SpotifyHotkeyController.ModifierKeys mods = SpotifyHotkeyController.ModifierKeys.None;
            if (e.Control) mods |= SpotifyHotkeyController.ModifierKeys.Control;
            if (e.Alt) mods |= SpotifyHotkeyController.ModifierKeys.Alt;
            if (e.Shift) mods |= SpotifyHotkeyController.ModifierKeys.Shift;
            if (HasWindowsMod(e.Modifiers)) mods |= SpotifyHotkeyController.ModifierKeys.Win; // Basic check

            hotkey.Modifiers = mods;
            hotkey.Key = k;
            Invalidate();
        }

        private bool HasWindowsMod(Keys mods) { return false; } // WinForms doesn't easily expose Win key modifier in standard KEA without hooks, simplified for now.

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(1, 1, Width - 3, Height - 3);
            
            // Background
            using (var path = GetRoundedPath(rect, 6))
            {
                g.FillPath(bgBrush, path);
                g.DrawPath(borderPen, path);
            }

            // Text
            string text = hotkey.ToString();
            if (string.IsNullOrEmpty(text) || text == " + None") text = "Press Keys...";

            // Draw "Chip" style or just clean text
            // Let's do clean text with maybe a focused indicator
            Color textColor = isFocused ? Color.White : Color.FromArgb(220, 220, 220);
            TextRenderer.DrawText(g, text, new Font("Segoe UI Semibold", 10), ClientRectangle, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // Protect from arrow keys moving focus
        protected override bool IsInputKey(Keys keyData) { return true; }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
