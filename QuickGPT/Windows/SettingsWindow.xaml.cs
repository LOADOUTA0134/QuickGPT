using System.Windows;
using QuickGPT.Classes;
using QuickGPT.Logic;

namespace QuickGPT.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings settings = SettingsManager.GetSettings();
            TextBoxOpenAiApiKey.Text = settings.OPENAI_API_KEY;
            TextBoxOpenAiApiUrl.Text = settings.OPENAI_API_URL;
            TextBoxOpenAiModel.Text = settings.OPENAI_MODEL;
            TextBoxSystemMessage.Text = settings.SYSTEM_MESSAGE;
            TextBoxUpdateInterval.Text = settings.UPDATE_INTERVAL.ToString();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TextBoxUpdateInterval.Text, out int updateInterval))
            {
                MessageBox.Show("Update Interval must be a number!");
                return;
            }

            Settings settings = new()
            {
                OPENAI_API_KEY = TextBoxOpenAiApiKey.Text,
                OPENAI_API_URL = TextBoxOpenAiApiUrl.Text,
                OPENAI_MODEL = TextBoxOpenAiModel.Text,
                SYSTEM_MESSAGE = TextBoxSystemMessage.Text,
                UPDATE_INTERVAL = updateInterval
            };

            SettingsManager.SaveSettings(settings);

            MessageBox.Show("Settings have been saved.");
        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            SettingsManager.ResetSettings();
            LoadSettings();

            MessageBox.Show("Settings have been reset.");
        }
    }
}
