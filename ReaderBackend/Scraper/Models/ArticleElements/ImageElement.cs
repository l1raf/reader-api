using System;

namespace ReaderBackend.Scraper.Models.ArticleElements
{
    public class ImageElement : IArticleElement
    {
        public ElementType Type => ElementType.Image;

        public string Url { get; set; }

        public ImageElement(string uri)
        {
            Url = uri;
        }
    }
}
