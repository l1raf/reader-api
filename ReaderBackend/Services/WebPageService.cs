using ReaderBackend.Models;
using ReaderBackend.Scraper;
using ReaderBackend.Repositories;
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
            ArticleScraper scraper = new ArticleScraper();

            return await scraper.GetPageContent(uri);
        }

        public string AddWebPage(WebPage webPage)
        {
            string error = null;

            try
            {
                if (webPage == null)
                    throw new ArgumentNullException(nameof(webPage));

                _webPageRepository.AddWebPage(webPage);

                if (!_webPageRepository.SaveChanges())
                    return "Failed to save changes.";
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return error;
        }

        public string DeleteWebPage(WebPage webPage)
        {
            try
            {
                _webPageRepository.DeleteWebPage(webPage);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public (string error, IEnumerable<WebPage> webPages) GetWebPagesByUserId(Guid id)
        {
            try
            {
                return (null, _webPageRepository.GetWebPagesByUserId(id));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public (string error, IEnumerable<WebPage> webPages) GetAllWebPages()
        {
            try
            {
                return (null, _webPageRepository.GetAllWebPages());
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public (string error, WebPage webPage) GetWebPageById(Guid id)
        {
            try
            {
                return (null, _webPageRepository.GetWebPageById(id));
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }

        public string UpdateWebPage(WebPage webPage)
        {
            string error = null;

            try
            {
                _webPageRepository.UpdateWebPage(webPage);

                if (!_webPageRepository.SaveChanges())
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
