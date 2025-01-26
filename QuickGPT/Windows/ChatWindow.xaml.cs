using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using QuickGPT.Classes;
using QuickGPT.Logic;

namespace QuickGPT
{
    public partial class ChatWindow : Window
    {
        private readonly Chat chat;
        private readonly MarkdownManager markdownManager;
        private RichTextBox? currentRichTextBox; // used for editing, usually null except it's being used

        /**
         * Constructor is called on first prompt
         */
        public ChatWindow(string prompt)
        {
            InitializeComponent();

            markdownManager = new();

            Width = SystemParameters.WorkArea.Width / 2;
            Height = SystemParameters.WorkArea.Height / 1.5;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            Show();

            SendMessage(prompt);
            MessageTextBox.Focus();

            chat = new(this);

            _ = chat.StreamResponseAsync(prompt);
        }

        /**
         * Callback method, gets called over and over while receiving streaming response
         * Creates new message or edits existing one
         */
        public async Task StreamResponseCallback(string chunk)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (currentRichTextBox == null)
                {
                    currentRichTextBox = SendMessage(chunk);
                    return;
                }
                EditMessage(currentRichTextBox, chunk);
            }, DispatcherPriority.Render);
        }

        /**
         * Called when the stream ends to do some things like markdown support
         */
        public async Task StreamEndCallback()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (currentRichTextBox != null)
                {
                    TextRange textRange = new(currentRichTextBox.Document.ContentStart, currentRichTextBox.Document.ContentEnd);
                    string content = textRange.Text;
                    textRange.Text = string.Empty;
                    currentRichTextBox.Document = markdownManager.Markdown2FlowDocument(content);
                    currentRichTextBox = null;
                }
            });
        }

        /**
         * When an error occurs while a response is streaming, this is called
         */
        public void ErrorCallback(string message)
        {
            SendMessage(message);
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

            SendMessage(MessageTextBox.Text);

            _ = chat.StreamResponseAsync(MessageTextBox.Text);

            MessageTextBox.Clear();
        }

        /**
         * Called when first chunk of new message is received
         * Returns RichTextBox so it can be edited when new chunk comes
         */
        private RichTextBox SendMessage(string message)
        {
            Border messageBorder = new()
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(84, 84, 84)),
                BorderThickness = new Thickness(0)
            };

            RichTextBox messageRichTextBox = new()
            {
                IsReadOnly = true,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
            };
            messageRichTextBox.AppendText(message);

            messageBorder.Child = messageRichTextBox;

            MessagesStackPanel.Children.Add(messageBorder);

            MessagesScrollViewer.ScrollToBottom();

            return messageRichTextBox;
        }

        /**
         * Appends an existing message
         * Called when first chunk of streaming response already created the message (and RichTextBox)
         * and it only has to be updated
         */
        private void EditMessage(RichTextBox richTextBox, string chunk)
        {
            richTextBox.AppendText(chunk);
        }
    }
}
