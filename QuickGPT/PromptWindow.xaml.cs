using System.Windows;

namespace QuickGPT
{
    public partial class PromptWindow : Window
    {
        public PromptWindow()
        {
            InitializeComponent();

            TextBoxPrompt.Focus();
        }

        private void TextBoxPrompt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }

            ChatWindow chatWindow = new(TextBoxPrompt.Text);
            chatWindow.Show();

            // This is not needed here because focus is lost when opening new window.
            // Keep in mind that this is needed maybe if some handling with the lost focus is changed
            //Close();
        }

        private void Window_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Close();
        }
    }
}
