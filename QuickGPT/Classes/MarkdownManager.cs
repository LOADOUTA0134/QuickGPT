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
        private const double FONT_SIZE_HEADING_1 = 22.0;
        private const double FONT_SIZE_HEADING_2 = 20.0;
        private const double FONT_SIZE_HEADING_3 = 18.0;

        private readonly Markdown engine;
        private FlowDocument document;

        private readonly Style heading1Style;
        private readonly Style heading2Style;
        private readonly Style heading3Style;

        /**
         * Inits the engine and sets the code style
         */
        public MarkdownManager()
        {
            engine = new();

            heading1Style = new(typeof(Paragraph));
            heading1Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, FONT_SIZE_HEADING_1));
            heading1Style.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));

            heading2Style = new(typeof(Paragraph));
            heading2Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, FONT_SIZE_HEADING_2));
            heading2Style.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.SemiBold));

            heading3Style = new(typeof(Paragraph));
            heading3Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, FONT_SIZE_HEADING_3));

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
         * Calls private method for transforming more in the document
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
         * Transforms the headings code blocks and hyperlinks by looping through the document blocks
         */
        private void TransformBlocks()
        {
            for (int i = 0; i < document.Blocks.Count; i++)
            {
                Block block = document.Blocks.ElementAt(i);

                if (block is Paragraph paragraph)
                {
                    switch (paragraph.Tag?.ToString())
                    {
                        case "Heading1": paragraph.Style = heading1Style; break;
                        case "Heading2": paragraph.Style = heading2Style; break;
                        case "Heading3": paragraph.Style = heading3Style; break;
                    }
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Hyperlink hyperlink)
                        {
                            hyperlink.Foreground = new SolidColorBrush(Colors.LightBlue);
                        }
                    }
                }
                else if (block is BlockUIContainer blockUIContainer)
                {
                    if (blockUIContainer.Child is TextEditor textEditor)
                    {
                        document.Blocks.InsertAfter(blockUIContainer, GetNewCodeParagraph(textEditor.Text));
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
         * Gets items blocks in list and applys changes for headings, code blocks and hyperlinks
         * Is recursive for lists in lists
         */
        private void HandleList(List list)
        {
            for (int i = 0; i < list.ListItems.Count; i++)
            {
                ListItem listItem = list.ListItems.ElementAt(i);
                for (int j = 0; j < listItem.Blocks.Count; j++)
                {
                    Block listItemBlock = listItem.Blocks.ElementAt(j);
                    if (listItemBlock is Paragraph paragraph)
                    {
                        switch (paragraph.Tag?.ToString())
                        {
                            case "Heading1": paragraph.Style = heading1Style; break;
                            case "Heading2": paragraph.Style = heading2Style; break;
                            case "Heading3": paragraph.Style = heading2Style; break;
                        }
                        foreach (Inline inline in paragraph.Inlines)
                        {
                            if (inline is Hyperlink hyperlink)
                            {
                                hyperlink.Foreground = new SolidColorBrush(Colors.LightBlue);
                            }
                        }
                    }
                    else if (listItemBlock is BlockUIContainer blockUIContainer)
                    {
                        if (blockUIContainer.Child is TextEditor textEditor)
                        {
                            document.Blocks.InsertAfter(blockUIContainer, GetNewCodeParagraph(textEditor.Text));
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

        private Paragraph GetNewCodeParagraph(string text)
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
