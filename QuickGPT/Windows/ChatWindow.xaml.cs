using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using QuickGPT.Logic;

namespace QuickGPT
{
    public partial class ChatWindow : Window
    {
        private readonly Chat chat;
        private TextBox? currentTextBox; // used for editing, usually null except it's being used

        public ChatWindow(string prompt)
        {
            InitializeComponent();

            SendMessage(prompt, true);
            MessageTextBox.Focus();

            chat = new(this);
            _ = chat.StreamResponseAsync(prompt);
        }

        public async Task StreamResponseCallback(string chunk)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (currentTextBox == null)
                {
                    currentTextBox = SendMessage(chunk, false);
                    return;
                }
                EditMessage(currentTextBox, chunk);
            }, DispatcherPriority.Render);
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

            _ = chat.StreamResponseAsync(MessageTextBox.Text);

            MessageTextBox.Clear();
        }

        private TextBox SendMessage(string message, bool isUser)
        {
            if (isUser)
            {
                currentTextBox = null;
            }

            Border messageBorder = new()
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = isUser ? Brushes.Gray : Brushes.DimGray,
                BorderThickness = new Thickness(0)
            };

            TextBox messageTextBlock = new()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = message,
                IsReadOnly = true,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent
            };

            messageBorder.Child = messageTextBlock;

            MessagesStackPanel.Children.Add(messageBorder);

            MessagesScrollViewer.ScrollToBottom();

            return messageTextBlock;
        }

        private void EditMessage(TextBox textBlock, string chunk)
        {
            textBlock.Text += chunk;
        }
    }
}
