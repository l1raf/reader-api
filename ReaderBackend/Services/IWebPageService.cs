using ReaderBackend.Models;
using ReaderBackend.Scraper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public interface IWebPageService
    {
        Task<(string error, IEnumerable<WebPage> webPages)> GetAllWebPages();

        Task<(string error, WebPage webPage)> GetWebPageById(Guid id);

        Task<string> AddWebPage(WebPage webPage);

        Task<string> UpdateWebPage(WebPage webPageModel);

        Task<string> DeleteWebPage(WebPage webPage);

        Task<(string error, IEnumerable<WebPage> webPages)> GetWebPagesByUserId(Guid id);

        Task<Article> GetArticle(Uri uri);
    }
}