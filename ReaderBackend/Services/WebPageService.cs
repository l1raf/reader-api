using ReaderBackend.Models;
using ReaderBackend.Repositories;
using ReaderBackend.Scraper;
using ReaderBackend.Scraper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public class WebPageService : IWebPageService
    {
        private readonly IWebPageRepository _webPageRepository;
        private readonly IArticleScraper _articleScraper;

        public WebPageService(IWebPageRepository webPageRepository, IArticleScraper articleScraper)
        {
            _articleScraper = articleScraper;
            _webPageRepository = webPageRepository;
        }

        public async Task<IEnumerable<Article>> GetAllUserArticles(IEnumerable<WebPage> webPages)
        {
            List<Article> articles = new();

            foreach (WebPage webPage in webPages)
            {
                var (error, article) = await GetArticle(webPage.Uri);

                if (error is null && article is not null)
                {
                    article.Favorite = webPage.Favorite;
                    articles.Add(article);
                }
            }

            return articles;
        }

        public async Task<(string error, Article article)> GetArticle(Uri uri)
        {
            try
            {
                return (null, await _articleScraper.GetPageContent(uri));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public async Task<string> AddWebPage(WebPage webPage)
        {
            string error = null;

            try
            {
                if (webPage == null)
                    throw new ArgumentNullException(nameof(webPage));

                await _webPageRepository.AddWebPage(webPage);

                if (!(await _webPageRepository.SaveChanges()))
                    return "Failed to save changes.";
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return error;
        }

        public async Task<string> DeleteWebPage(WebPage webPage)
        {
            try
            {
                await _webPageRepository.DeleteWebPage(webPage);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<(string error, IEnumerable<WebPage> webPages)> GetWebPagesByUserId(Guid id)
        {
            try
            {
                return (null, await _webPageRepository.GetWebPagesByUserId(id));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }


        public async Task<(string error, IEnumerable<WebPage> webPages)> GetWebPagesByUserId(Guid id, int page)
        {
            try
            {
                return (null, await _webPageRepository.GetWebPagesByUserId(id, page));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public async Task<(string error, IEnumerable<WebPage> webPages)> GetAllWebPages()
        {
            try
            {
                return (null, await _webPageRepository.GetAllWebPages());
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public async Task<(string error, WebPage webPage)> GetWebPageById(Guid id)
        {
            try
            {
                return (null, await _webPageRepository.GetWebPageById(id));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public async Task<string> UpdateWebPage(WebPage webPage)
        {
            string error = null;

            try
            {
                _webPageRepository.UpdateWebPage(webPage);

                if (!(await _webPageRepository.SaveChanges()))
                    error = "Failed to save changes.";
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return error;
        }

        public async Task<(string error, WebPage webPage)> GetUserWebPageByUri(Uri uri, Guid userId)
        {
            try
            {
                return (null, await _webPageRepository.GetUserWebPageByUri(uri, userId));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }
    }
}
