using System;
using System.Windows.Forms;

namespace SpotifyHotkeyController
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable modern visual styles for Windows Forms
            ApplicationConfiguration.Initialize();
            
            // Run the main form which will handle hotkeys and system tray
            Application.Run(new MainForm());
        }
    }
}
