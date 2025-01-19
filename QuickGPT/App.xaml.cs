using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using QuickGPT.Classes;
using QuickGPT.Logic;
using QuickGPT.Windows;

namespace QuickGPT
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Settings settings = SettingsManager.GetSettings();
            ShortcutManager.RegisterHotkeyFromString(settings.SHORTCUT);

            CreateTrayIcon();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ShortcutManager.RemoveHotkey();
            base.OnExit(e);
        }

        private void CreateTrayIcon()
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
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Shutdown());
        }
    }
}
