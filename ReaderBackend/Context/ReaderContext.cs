using Microsoft.EntityFrameworkCore;
using ReaderBackend.Models;

namespace ReaderBackend.Context
{
    public class ReaderContext : DbContext
    {
        public ReaderContext(DbContextOptions<ReaderContext> opt) : base(opt)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebPage>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId);
        }

        public DbSet<WebPage> WebPages { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
