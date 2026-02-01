using System;
using System.IO;
using System.Text.Json;

namespace SpotifyHotkeyController
{
    /// <summary>
    /// Configuration class for storing hotkey settings.
    /// </summary>
    public class HotkeyConfig
    {
        public HotkeyDefinition PlayPause { get; set; } = new HotkeyDefinition
        {
            Modifiers = ModifierKeys.Control,
            Key = System.Windows.Forms.Keys.F9
        };

        public HotkeyDefinition NextTrack { get; set; } = new HotkeyDefinition
        {
            Modifiers = ModifierKeys.Control,
            Key = System.Windows.Forms.Keys.F10
        };

        public HotkeyDefinition PreviousTrack { get; set; } = new HotkeyDefinition
        {
            Modifiers = ModifierKeys.Control,
            Key = System.Windows.Forms.Keys.F8
        };

        private static string ConfigFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SpotifyHotkeyController", "config.json");

        /// <summary>
        /// Loads configuration from file or returns default config.
        /// </summary>
        public static HotkeyConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    return JsonSerializer.Deserialize<HotkeyConfig>(json) ?? new HotkeyConfig();
                }
            }
            catch
            {
                // If load fails, return default config
            }

            return new HotkeyConfig();
        }

        /// <summary>
        /// Saves configuration to file.
        /// </summary>
        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath)!;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Failed to save configuration: {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Represents a single hotkey definition.
    /// </summary>
    public class HotkeyDefinition
    {
        public ModifierKeys Modifiers { get; set; }
        public System.Windows.Forms.Keys Key { get; set; }

        public override string ToString()
        {
            var parts = new System.Collections.Generic.List<string>();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                parts.Add("Ctrl");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                parts.Add("Alt");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                parts.Add("Shift");
            if (Modifiers.HasFlag(ModifierKeys.Win))
                parts.Add("Win");

            parts.Add(Key.ToString());

            return string.Join(" + ", parts);
        }

        /// <summary>
        /// Converts to the Windows API modifier flags.
        /// </summary>
        public uint ToWinApiModifiers()
        {
            uint mods = 0;
            if (Modifiers.HasFlag(ModifierKeys.Control)) mods |= 0x0002; // MOD_CONTROL
            if (Modifiers.HasFlag(ModifierKeys.Alt)) mods |= 0x0001;     // MOD_ALT
            if (Modifiers.HasFlag(ModifierKeys.Shift)) mods |= 0x0004;   // MOD_SHIFT
            if (Modifiers.HasFlag(ModifierKeys.Win)) mods |= 0x0008;     // MOD_WIN
            return mods;
        }
    }

    /// <summary>
    /// Modifier keys that can be used in hotkey combinations.
    /// </summary>
    [Flags]
    public enum ModifierKeys
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
