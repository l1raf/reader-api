namespace ReaderBackend.Scraper.Models.ArticleElements
{
    public class TextElement : IArticleElement
    {
        public ElementType Type { get; set; }

        public string Text { get; set; }

        public TextElement(string text, ElementType elementType)
        {
            Type = elementType;
            Text = text;
        }
    }
}
