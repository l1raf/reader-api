using ReaderBackend.Scraper.Models.ArticleElements;
using System.Collections.Generic;

namespace ReaderBackend.Scraper.Models
{
    public class Article
    {
        public string Title { get; set; }

        public List<IArticleElement> Content { get; set; }

        public Article()
        {
            Content = new List<IArticleElement>();
        }
    }
}
