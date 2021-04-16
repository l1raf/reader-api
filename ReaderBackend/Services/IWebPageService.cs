using System;
using ReaderBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReaderBackend.Services
{
    public interface IWebPageService
    {
        (string error, IEnumerable<WebPage> webPages) GetAllWebPages();

        (string error, WebPage webPage) GetWebPageById(Guid id);

        string AddWebPage(WebPage webPage);

        string UpdateWebPage(WebPage webPageModel);

        string DeleteWebPage(WebPage webPage);
        
        (string error, IEnumerable<WebPage> webPages) GetWebPagesByUserId(Guid id);

         Task<Article> GetArticle(Uri uri);
    }
}
