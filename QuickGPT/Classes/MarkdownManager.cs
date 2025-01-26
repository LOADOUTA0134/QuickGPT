using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using MdXaml;

namespace QuickGPT.Classes
{
    internal class MarkdownManager
    {
        private const string FONT_FAMILY = "Arial";
        private const double FONT_SIZE = 16.0;

        private readonly Markdown engine;
        private FlowDocument document;

        /**
         * Inits the engine and sets the code style
         */
        public MarkdownManager()
        {
            engine = new();

            var codeStyle = new Style(typeof(Run));
            codeStyle.Setters.Add(new Setter(Run.FontFamilyProperty, new FontFamily("Consolas")));
            codeStyle.Setters.Add(new Setter(Run.BackgroundProperty, new SolidColorBrush(Color.FromArgb(60, 255, 255, 255))));
            codeStyle.Setters.Add(new Setter(Run.ForegroundProperty, new SolidColorBrush(Colors.White)));

            engine.CodeStyle = codeStyle;

            document = new();
        }

        /**
         * Creates a FlowDocument from a markdown string
         * Transforms the markdown by the library
         * Also applys custom changes to heading 1-3 and code blocks
         */
        public FlowDocument Markdown2FlowDocument(string markdown)
        {
            document = engine.Transform(markdown);
            document.FontFamily = new(FONT_FAMILY);
            document.FontSize = FONT_SIZE;

            TransformBlocks();

            return document;
        }

        /**
         * Transforms the headings and code blocks by looping through the document blocks
         * Calls the other private methods below
         */
        private void TransformBlocks()
        {
            Style heading1Style = new(typeof(Paragraph));
            heading1Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, 22.0));
            heading1Style.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));

            Style heading2Style = new(typeof(Paragraph));
            heading2Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, 20.0));
            heading2Style.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.SemiBold));

            Style heading3Style = new(typeof(Paragraph));
            heading3Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, 18.0));

            for (int i = 0; i < document.Blocks.Count; i++)
            {
                Block block = document.Blocks.ElementAt(i);

                if (block is Paragraph paragraph)
                {
                    switch (paragraph.Tag?.ToString())
                    {
                        case "Heading1": paragraph.Style = heading1Style; break;
                        case "Heading2": paragraph.Style = heading2Style; break;
                        case "Heading3": paragraph.Style = heading2Style; break;
                    }
                }
                else if (block is BlockUIContainer blockUIContainer)
                {
                    if (blockUIContainer.Child is TextEditor textEditor)
                    {
                        document.Blocks.InsertAfter(blockUIContainer, GetCodeParagraph(textEditor.Text));
                        document.Blocks.Remove(blockUIContainer);
                    }
                }
                else if (block is List list)
                {
                    HandleList(list);
                }
            }
        }

        /**
         * Gets the code blocks (TextEditor) in the list and changes the style of them
         * Is recursive, so it works for a code block in a list in a list in a list ...
         */
        private void HandleList(List list)
        {
            for (int i = 0; i < list.ListItems.Count; i++)
            {
                ListItem listItem = list.ListItems.ElementAt(i);
                for (int j = 0; j < listItem.Blocks.Count; j++)
                {
                    Block listItemBlock = listItem.Blocks.ElementAt(j);
                    if (listItemBlock is BlockUIContainer blockUIContainer)
                    {
                        if (blockUIContainer.Child is TextEditor textEditor)
                        {
                            document.Blocks.InsertAfter(blockUIContainer, GetCodeParagraph(textEditor.Text));
                            document.Blocks.Remove(blockUIContainer);
                        }
                    }
                    else if (listItemBlock is List innerList)
                    {
                        HandleList(innerList); // recursive self call
                    }
                }
            }
        }

        private Paragraph GetCodeParagraph(string text)
        {
            Paragraph p = new()
            {
                Background = new SolidColorBrush(Color.FromRgb(54, 54, 54)),
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Consolas"),
                Padding = new Thickness(10),
                Margin = new Thickness(5)
            };
            p.Inlines.Add(new Run(text));

            return p;
        }
    }
}
