using Microsoft.EntityFrameworkCore;
using ReaderBackend.Models;

namespace ReaderBackend.Repositories
{
    public class ReaderContext : DbContext
    {
        public ReaderContext(DbContextOptions<ReaderContext> opt) : base(opt)
        {

        }

        public DbSet<WebPage> WebPages { get; set; }
    }
}
