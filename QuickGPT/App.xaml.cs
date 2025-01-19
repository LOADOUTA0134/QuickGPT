using System.Windows;
using QuickGPT.Classes;
using QuickGPT.Logic;

namespace QuickGPT
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Settings settings = SettingsManager.GetSettings();
            ShortcutManager.RegisterHotkeyFromString(settings.SHORTCUT);

            TrayIconManager.CreateTrayIcon(this);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ShortcutManager.RemoveHotkey();
            base.OnExit(e);
        }
    }
}
