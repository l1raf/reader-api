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

        public async Task<WebPage> GetUserWebPageByUri(Uri uri, Guid userId)
        {
            return await _context.WebPages.FirstOrDefaultAsync(x => x.Uri == uri && x.UserId == userId);
        }

        public async Task AddWebPage(WebPage webPage)
        {
            var webPageWithSameUrl = (from webpage in _context.WebPages
                                      where webpage.UserId == webPage.UserId && webpage.Uri == webPage.Uri
                                      select webpage).FirstOrDefault();

            if (webPageWithSameUrl is not null)
            {
                webPageWithSameUrl.Title = webPage.Title;
                webPage.Id = webPageWithSameUrl.Id;
            }
            else
            {
                await _context.WebPages.AddAsync(webPage);
            }
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

        public async Task<IEnumerable<WebPage>> GetWebPagesByUserId(Guid id, int page)
        {
            return await _context.WebPages.Where(x => x.UserId == id).Skip(5 * page).Take(5).ToListAsync();
        }
    }
}
