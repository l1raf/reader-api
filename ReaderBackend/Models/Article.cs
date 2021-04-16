using ReaderBackend.Models.ArticleElements;
using System.Collections.Generic;

namespace ReaderBackend.Models
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
