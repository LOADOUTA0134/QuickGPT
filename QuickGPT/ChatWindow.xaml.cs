using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuickGPT
{
    public partial class ChatWindow : Window
    {
        public ChatWindow(string prompt)
        {
            InitializeComponent();

            SendMessage(prompt, true);
            MessageTextBox.Focus();

            // TODO Prompt openai and get message
        }

        private void MessageTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                return;
            }

            SendMessage(MessageTextBox.Text, true);
            MessageTextBox.Clear();

            // TODO Prompt openai and get message
        }

        private void SendMessage(string message, bool isUser)
        {
            Border messageBorder = new()
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = isUser ? Brushes.DimGray : Brushes.Gray,
                BorderThickness = new Thickness(0)
            };

            TextBlock messageTextBlock = new()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = message,
                Foreground = Brushes.White
            };

            messageBorder.Child = messageTextBlock;

            MessagesStackPanel.Children.Add(messageBorder);

            MessagesScrollViewer.ScrollToBottom();
        }
    }
}
