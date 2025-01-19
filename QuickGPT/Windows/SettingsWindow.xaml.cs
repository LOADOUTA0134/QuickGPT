using System.Windows;
using System.Windows.Input;
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
            CheckBoxAutoStart.IsChecked = settings.AUTOSTART;
            TextBoxShortcut.Text = settings.SHORTCUT;
            TextBoxOpenAiApiKey.Text = settings.OPENAI_API_KEY;
            TextBoxOpenAiApiUrl.Text = settings.OPENAI_API_URL;
            TextBoxOpenAiModel.Text = settings.OPENAI_MODEL;
            TextBoxSystemMessage.Text = settings.SYSTEM_MESSAGE;
            TextBoxUpdateInterval.Text = settings.UPDATE_INTERVAL.ToString();
        }

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

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TextBoxUpdateInterval.Text, out int updateInterval))
            {
                System.Windows.MessageBox.Show("Update Interval must be a number!");
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

            System.Windows.MessageBox.Show("Settings have been saved.");
        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            SettingsManager.ResetSettings();
            LoadSettings();

            System.Windows.MessageBox.Show("Settings have been reset.");
        }
    }
}
