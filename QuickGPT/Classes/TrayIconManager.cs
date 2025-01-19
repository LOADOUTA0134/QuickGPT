using System.Drawing;
using System.Windows.Forms;
using QuickGPT.Windows;

namespace QuickGPT.Classes
{
    internal class TrayIconManager
    {
        public static void CreateTrayIcon(System.Windows.Application application)
        {
            NotifyIcon trayIcon = new()
            {
                Icon = SystemIcons.Application, // TODO
                Text = "QuickGPT",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            trayIcon.ContextMenuStrip.Items.Add("Open Prompt", null, (s, e) =>
            {
                PromptWindow promtWindow = new();
                promtWindow.Show();
            });

            trayIcon.ContextMenuStrip.Items.Add("Settings", null, (s, e) =>
            {
                SettingsWindow settingsWindow = new();
                settingsWindow.Show();
            });
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => application.Shutdown());
        }
    }
}
