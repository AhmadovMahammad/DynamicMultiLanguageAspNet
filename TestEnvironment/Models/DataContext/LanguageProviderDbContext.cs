using Microsoft.EntityFrameworkCore;
using TestEnvironment.Models.Entities;

namespace TestEnvironment.Models.DataContext
{
    public class LanguageProviderDbContext : DbContext
    {
        public LanguageProviderDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Faq> Faqs { get; set; }
    }
}
