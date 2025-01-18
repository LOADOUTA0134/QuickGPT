using System.Windows;

namespace QuickGPT
{
    public partial class PromtWindow : Window
    {
        public PromtWindow()
        {
            InitializeComponent();

            TextBoxPromt.Focus();
        }

        private void TextBoxPromt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }

            // TODO Open normal chat window with request
        }

        private void Window_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Close();
        }
    }
}
