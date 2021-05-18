using ReaderBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReaderBackend.Repositories
{
    public interface IWebPageRepository
    {
        Task<bool> SaveChanges();

        Task<IEnumerable<WebPage>> GetAllWebPages();

        Task<WebPage> GetWebPageById(Guid id);

        Task<WebPage> GetUserWebPageByUri(Uri uri, Guid userId);

        Task AddWebPage(WebPage webPage);

        Task DeleteWebPage(WebPage webPage);

        void UpdateWebPage(WebPage webPage);
        
        Task<IEnumerable<WebPage>> GetWebPagesByUserId(Guid id);

        Task<IEnumerable<WebPage>> GetWebPagesByUserId(Guid id, int page);
    }
}
