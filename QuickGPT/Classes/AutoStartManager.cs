using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;

namespace QuickGPT.Classes
{
    internal class AutoStartManager
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string appName = "QuickGPT";

        public static void EnableAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey, true);
                if (key == null)
                {
                    throw new InvalidOperationException("Unable to access the registry key.");
                }
                ProcessModule? mainModule = Process.GetCurrentProcess().MainModule;
                if (mainModule == null)
                {
                    throw new InvalidOperationException("Cannot access own MainModule");
                }
                key.SetValue(appName, $"\"{mainModule.FileName}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to enable auto start");
            }
        }

        public static void DisableAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey, true);
                if (key == null)
                {
                    throw new InvalidOperationException("Unable to access the registry key.");
                }
                key.DeleteValue(appName, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to disable auto start");
            }
        }
    }
}
