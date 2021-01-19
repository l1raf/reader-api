using ReaderBackend.Models;
using ReaderBackend.Repositories;
using System;
using System.Collections.Generic;

namespace ReaderBackend.Services
{
    public class WebPageService : IWebPageService
    {
        private readonly IWebPageRepository _webPageRepository;

        public WebPageService(IWebPageRepository webPageRepository)
        {
            _webPageRepository = webPageRepository;
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
                    error = "Failed to save changes.";
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

        public (string error, WebPage webPage) GetWebPageById(int id)
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
