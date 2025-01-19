using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using NHotkey;
using NHotkey.Wpf;
using QuickGPT.Windows;

namespace QuickGPT
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            CreateTrayIcon();

            HotkeyManager.Current.AddOrReplace("QuickGPT_GlobalHotkey", System.Windows.Input.Key.S, System.Windows.Input.ModifierKeys.Alt, HotKeyPressed);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HotkeyManager.Current.Remove("QuickGPT_GlobalHotkey");
            base.OnExit(e);
        }

        private void HotKeyPressed(object? sender, HotkeyEventArgs e)
        {
            PromptWindow promptWindow = new();
            promptWindow.Show();
            promptWindow.Activate();
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
