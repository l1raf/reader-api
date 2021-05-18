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

        Task<(string error, WebPage webPage)> GetUserWebPageByUri(Uri uri, Guid userId);

        Task<(string error, IEnumerable<WebPage> webPages)> GetWebPagesByUserId(Guid id);

        Task<(string error, IEnumerable<WebPage> webPages)> GetWebPagesByUserId(Guid id, int page);

        Task<IEnumerable<Article>> GetAllUserArticles(IEnumerable<WebPage> webPages);

        Task<(string error, Article article)> GetArticle(Uri uri);
    }
}