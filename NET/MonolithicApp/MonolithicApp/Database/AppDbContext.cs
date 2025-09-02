using Microsoft.EntityFrameworkCore;
using MonolithicApp.Database.Model;

namespace MonolithicApp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<RouteFragment> RouteFragments { get; set; }
    }
}
