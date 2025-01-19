using System.Windows;
using QuickGPT.Classes;
using QuickGPT.Logic;

namespace QuickGPT
{
    public partial class App : Application
    {
        private static Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mutex = new Mutex(true, "QuickGPT", out bool isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("Application already running.");
                Shutdown();
                return;
            }

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Settings settings = SettingsManager.GetSettings();
            ShortcutManager.RegisterHotkeyFromString(settings.SHORTCUT);

            TrayIconManager.CreateTrayIcon(this);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ShortcutManager.RemoveHotkey();
            _mutex?.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
