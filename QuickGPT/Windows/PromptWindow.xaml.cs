using System.Windows;

namespace QuickGPT
{
    public partial class PromptWindow : Window
    {
        public PromptWindow()
        {
            InitializeComponent();

            TextBoxPrompt.Focus();

            Show();
            Activate();
        }

        /**
         * Events
         */

        /**
         * Waits for enter to open chat window that will then perform the openai call etc.
         */
        private void TextBoxPrompt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                try
                {
                    Close();
                }
                catch { }
                return;
            }
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(TextBoxPrompt.Text.Trim()))
            {
                return;
            }

            _ = new ChatWindow(TextBoxPrompt.Text);

            // Close() is not needed here because focus is lost when opening new window (and closes this window then).
            // Keep in mind that Close() is needed maybe if some handling with the lost focus is changed
            //Close();
        }

        /**
         * Close window if focus is lost
         */
        private void Window_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            try
            {
                Close();
            }
            catch { }
        }
    }
}
