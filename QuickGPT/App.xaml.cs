using System.Windows;

namespace QuickGPT
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Wait for keyboard shortcut

            PromptWindow promtWindow = new();
            promtWindow.Show();
        }
    }

}
