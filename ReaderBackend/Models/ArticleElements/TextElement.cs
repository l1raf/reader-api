namespace ReaderBackend.Models.ArticleElements
{
    public class TextElement : IArticleElement
    {
        public ElementType Type => ElementType.Text;

        public string Text { get; set; }

        public TextElement(string text)
        {
            Text = text;
        }
    }
}
