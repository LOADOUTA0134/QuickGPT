using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
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

            foreach (Block block in document.Blocks)
            {
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
                        ModifyTextEditor(textEditor);
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
            foreach (var listItem in list.ListItems)
            {
                foreach (var listItemBlock in listItem.Blocks)
                {
                    if (listItemBlock is BlockUIContainer blockUIContainer)
                    {
                        if (blockUIContainer.Child is TextEditor textEditor)
                        {
                            ModifyTextEditor(textEditor);
                        }
                    }
                    else if (listItemBlock is List innerList)
                    {
                        HandleList(innerList); // recursive call
                    }
                }
            }
        }

        /**
         * Here the changes to the given TextEditor happen
         */
        private void ModifyTextEditor(TextEditor textEditor)
        {
            textEditor.Background = new SolidColorBrush(Color.FromRgb(54, 54, 54));
            textEditor.FontFamily = new("Consolas");
            textEditor.Foreground = new SolidColorBrush(Colors.White);
            textEditor.FontWeight = FontWeights.Regular;
            textEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textEditor.Padding = new Thickness(10);

            if (textEditor.SyntaxHighlighting != null)
            {
                foreach (var item in textEditor.SyntaxHighlighting.NamedHighlightingColors)
                {
                    item.Foreground = new SimpleHighlightingBrush(Colors.White);
                    item.FontWeight = FontWeights.Regular;
                }

                ChangeHighlightingColor(textEditor, "String", Colors.Gray);
                ChangeHighlightingColor(textEditor, "NumberLiteral", Colors.Blue);
                ChangeHighlightingColor(textEditor, "Comment", Colors.Green);
                ChangeHighlightingColor(textEditor, "MethodCall", Colors.LightYellow);
                ChangeHighlightingColor(textEditor, "Keywords", Colors.LightBlue);
            }
        }

        /**
         * Changes the SyntaxHighlighting of, for example, a string inside a TextEditor
         */
        private void ChangeHighlightingColor(TextEditor textEditor, string item, Color color)
        {
            HighlightingColor highlightingColor = textEditor.SyntaxHighlighting.GetNamedColor(item);
            if (highlightingColor != null)
            {
                highlightingColor.Foreground = new SimpleHighlightingBrush(color);
            }
        }
    }
}
