using System.IO;
using Newtonsoft.Json;
using QuickGPT.Classes;

namespace QuickGPT.Logic
{
    internal class SettingsManager
    {
        private static readonly string SETTINGS_DIRECTORY_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuickGPT");
        private static readonly string SETTINGS_FILE_PATH = Path.Combine(SETTINGS_DIRECTORY_PATH, "settings.json");

        private static Settings? settings;

        public static Settings GetSettings()
        {
            if (settings == null)
            {
                settings = LoadSettings();
            }
            return settings;
        }

        public static void SaveSettings(Settings newSettings)
        {
            settings = newSettings;
            string json = JsonConvert.SerializeObject(newSettings);
            File.WriteAllText(SETTINGS_FILE_PATH, json);
        }

        private static Settings LoadSettings()
        {
            if (!Directory.Exists(SETTINGS_DIRECTORY_PATH))
            {
                Directory.CreateDirectory(SETTINGS_DIRECTORY_PATH);
            }
            if (!File.Exists(SETTINGS_FILE_PATH))
            {
                Settings defaultSettings = GetDefaultSettings();
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            string json = File.ReadAllText(SETTINGS_FILE_PATH);
            Settings? settings = JsonConvert.DeserializeObject<Settings>(json);

            if (settings == null)
            {
                Settings defaultSettings = GetDefaultSettings();
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            return settings;
        }

        private static Settings GetDefaultSettings()
        {
            Settings defaultSettings = new()
            {
                OPENAI_API_KEY = "your-api-key",
                OPENAI_API_URL = "https://api.openai.com/v1/chat/completions",
                OPENAI_MODEL = "gpt-3.5-turbo",
                SYSTEM_MESSAGE = "You are a helpful assistant. Keep your answers short, simple and informative.",
                UPDATE_INTERVAL = 30
            };
            return defaultSettings;
        }
    }
}
