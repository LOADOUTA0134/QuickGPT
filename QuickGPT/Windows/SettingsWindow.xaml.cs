using System.Windows;
using System.Windows.Input;
using QuickGPT.Classes;
using QuickGPT.Logic;

namespace QuickGPT.Windows
{
    public partial class SettingsWindow : Window
    {
        private static SettingsWindow? instance;

        /**
         * Loads settings
         * If another settings window is open the new windows closes with a MessageBox info
         */
        public SettingsWindow()
        {
            InitializeComponent();

            if (instance != null)
            {
                MessageBox.Show("Close existing setting window to open new one");
                Close();
                return;
            }

            instance = this;

            Show();

            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings settings = SettingsManager.GetSettings();
            CheckBoxAutoStart.IsChecked = settings.AUTOSTART;
            TextBoxShortcut.Text = settings.SHORTCUT;
            TextBoxOpenAiApiKey.Text = settings.OPENAI_API_KEY;
            TextBoxOpenAiApiUrl.Text = settings.OPENAI_API_URL;
            TextBoxOpenAiModel.Text = settings.OPENAI_MODEL;
            TextBoxSystemMessage.Text = settings.SYSTEM_MESSAGE;
            TextBoxUpdateInterval.Text = settings.UPDATE_INTERVAL.ToString();
        }

        /**
         * Events
         */

        /**
         * Listen for hotkey to set it
         */
        private void TextBoxShortcut_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;

            Key key;
            ModifierKeys modifiers;

            key = e.Key;
            modifiers = Keyboard.Modifiers;

            if (key == Key.Escape)
            {
                TextBoxShortcut.Clear();
                return;
            }
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            string shortcutString = ShortcutManager.HotkeyToString(key, modifiers);

            switch (key)
            {
                case Key.LeftCtrl: break;
                case Key.RightCtrl: break;
                case Key.LeftAlt: break;
                case Key.RightAlt: break;
                case Key.LeftShift: break;
                case Key.RightShift: break;
                case Key.LWin: break;
                case Key.RWin: break;
                default: TextBoxShortcut.Text = shortcutString; break;
            }
        }

        /**
         * Save settings when button pressed
         */
        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TextBoxUpdateInterval.Text, out int updateInterval))
            {
                MessageBox.Show("Update Interval must be a number!");
                return;
            }

            bool autoStart = false;
            if (CheckBoxAutoStart.IsChecked != null)
            {
                autoStart = (bool)CheckBoxAutoStart.IsChecked;
            }

            Settings settings = new()
            {
                AUTOSTART = autoStart,
                SHORTCUT = TextBoxShortcut.Text,
                OPENAI_API_KEY = TextBoxOpenAiApiKey.Text,
                OPENAI_API_URL = TextBoxOpenAiApiUrl.Text,
                OPENAI_MODEL = TextBoxOpenAiModel.Text,
                SYSTEM_MESSAGE = TextBoxSystemMessage.Text,
                UPDATE_INTERVAL = updateInterval
            };

            SettingsManager.SaveSettings(settings);

            MessageBox.Show("Settings have been saved.");
        }

        /**
         * Reset settings
         */
        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            SettingsManager.ResetSettings();
            LoadSettings();

            MessageBox.Show("Settings have been reset.");
        }

        /**
         * Set instance to null when closing window
         * This is because only 1 settings window can be open
         * As long as an instance is there a new window cannot be opened
         */
        private void Window_Closed(object sender, EventArgs e)
        {
            instance = null;
        }
    }
}
