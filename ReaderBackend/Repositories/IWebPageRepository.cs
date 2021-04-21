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

        Task AddWebPage(WebPage webPage);

        Task DeleteWebPage(WebPage webPage);

        void UpdateWebPage(WebPage webPage);
        
        Task<IEnumerable<WebPage>> GetWebPagesByUserId(Guid id);
    }
}
