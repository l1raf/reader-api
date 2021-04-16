using ReaderBackend.Models;
using System;
using System.Threading.Tasks;

namespace ReaderBackend.Scraper
{
    public interface IArticleScraper
    {
        public Task<Article> GetPageContent(Uri uri);
    }
}
