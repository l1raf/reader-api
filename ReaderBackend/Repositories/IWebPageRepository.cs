using ReaderBackend.Models;
using System.Collections.Generic;

namespace ReaderBackend.Repositories
{
    public interface IWebPageRepository
    {
        bool SaveChanges();

        IEnumerable<WebPage> GetAllWebPages();

        WebPage GetWebPageById(int id);

        void AddWebPage(WebPage webPage);

        void DeleteWebPage(WebPage webPage);

        void UpdateWebPage(WebPage webPage);
    }
}
