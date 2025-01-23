using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MdXaml;
using QuickGPT.Logic;

namespace QuickGPT
{
    public partial class ChatWindow : Window
    {
        private readonly Chat chat;
        private readonly Markdown engine;
        private RichTextBox? currentRichTextBox; // used for editing, usually null except it's being used

        /**
         * Constructor is called on first prompt
         */
        public ChatWindow(string prompt)
        {
            InitializeComponent();

            engine = new Markdown();

            Show();

            SendMessage(prompt, true);
            MessageTextBox.Focus();

            chat = new(this);

            _ = chat.StreamResponseAsync(prompt);
        }

        /**
         * Callback method, gets called over and over while receiving streaming response
         * Creates new message or edits existing one
         */
        public async Task StreamResponseCallback(string message)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (currentRichTextBox == null)
                {
                    currentRichTextBox = SendMessage(message, false);
                    return;
                }
                EditMessage(currentRichTextBox, message);
            }, DispatcherPriority.Render);
        }

        public void ErrorCallback(string message)
        {
            SendMessage(message, false);
        }

        /**
         * KeyDown Event, waits for enter so the new message can be sent
         */
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

        /**
         * Called when first chunk of new message is received
         * This creates a whole new text bubble on either right or left side
         * Returns RichTextBox so it can be edited when new chunk comes
         * Sets currentRichTextBox to null if message is from user,
         * if this doesnt happen the next stream will edit the previous message
         */
        private RichTextBox SendMessage(string message, bool isUser)
        {
            if (isUser)
            {
                currentRichTextBox = null;
            }

            Border messageBorder = new()
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = new SolidColorBrush(Color.FromRgb(84, 84, 84)),
                BorderThickness = new Thickness(0)
            };

            RichTextBox messageRichTextBox = new()
            {
                IsReadOnly = true,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Document = engine.Transform(message)
            };

            messageBorder.Child = messageRichTextBox;

            MessagesStackPanel.Children.Add(messageBorder);

            MessagesScrollViewer.ScrollToBottom();

            return messageRichTextBox;
        }

        /**
         * Edits an existing message
         * Called when first chunk of streaming response already created the message (and RichTextBox)
         * and it only has to be updated
         */
        private void EditMessage(RichTextBox richTextBox, string newMessage)
        {
            richTextBox.Document = engine.Transform(newMessage);
        }
    }
}
