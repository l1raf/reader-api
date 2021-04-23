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

        public async Task<Article> GetArticle(Uri uri)
        {
            try
            {
                return await _articleScraper.GetPageContent(uri);
            }
            catch (Exception)
            {
                return null;
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
    }
}
