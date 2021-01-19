using ReaderBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReaderBackend.Repositories
{
    public class WebPageRepository : IWebPageRepository
    {
        private readonly ReaderContext _context;

        public WebPageRepository(ReaderContext context)
        {
            _context = context;
        }

        public void AddWebPage(WebPage webPage)
        {
            _context.Add(webPage);
        }

        public IEnumerable<WebPage> GetAllWebPages()
        {
            return _context.WebPages.ToList();
        }

        public WebPage GetWebPageById(int id)
        {
            return _context.WebPages.FirstOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public void DeleteWebPage(WebPage webPage)
        {
            if (webPage == null) 
                throw new ArgumentNullException(nameof(webPage));

            _context.WebPages.Remove(webPage);
            SaveChanges();
        }

        public void UpdateWebPage(WebPage webPage)
        {
            //Nothing here
        }
    }
}
