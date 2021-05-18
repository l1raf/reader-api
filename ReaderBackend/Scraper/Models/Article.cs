using ReaderBackend.Scraper.Models.ArticleElements;
using System;
using System.Collections.Generic;

namespace ReaderBackend.Scraper.Models
{
    public class Article
    {
        public string Title { get; set; }

        public string Image { get; set; }

        public Uri Url { get; set; }

        public string Author { get; set; }

        public bool Favorite { get; set; }

        public string Description { get; set; }

        public List<IArticleElement> Content { get; set; }

        public Article()
        {
            Content = new List<IArticleElement>();
        }
    }
}
