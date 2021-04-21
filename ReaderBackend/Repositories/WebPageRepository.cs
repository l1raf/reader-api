using Microsoft.EntityFrameworkCore;
using ReaderBackend.Context;
using ReaderBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReaderBackend.Repositories
{
    public class WebPageRepository : IWebPageRepository
    {
        private readonly ReaderContext _context;

        public WebPageRepository(ReaderContext context)
        {
            _context = context;
        }

        public async Task AddWebPage(WebPage webPage)
        {
            await _context.AddAsync(webPage);
        }

        public async Task<IEnumerable<WebPage>> GetAllWebPages()
        {
            return await _context.WebPages.ToListAsync();
        }

        public async Task<WebPage> GetWebPageById(Guid id)
        {
            return await _context.WebPages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task DeleteWebPage(WebPage webPage)
        {
            if (webPage == null) 
                throw new ArgumentNullException(nameof(webPage));
            
            _context.WebPages.Remove(webPage);
            await SaveChanges();
        }

        public void UpdateWebPage(WebPage webPage)
        {
            //Nothing here
        }

        public async Task<IEnumerable<WebPage>> GetWebPagesByUserId(Guid id)
        {
            return await _context.WebPages.Where(x => x.UserId == id).ToListAsync();
        }
    }
}
