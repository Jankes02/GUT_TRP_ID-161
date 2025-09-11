using Microsoft.EntityFrameworkCore;
using MonolithicApp.Database.Model;

namespace MonolithicApp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Connection>()
                .HasOne(c => c.FromCity)
                .WithMany()
                .HasForeignKey(c => c.FromCityId);

            modelBuilder.Entity<Connection>()
                .HasOne(c => c.ToCity)
                .WithMany()
                .HasForeignKey(c => c.ToCityId);
        }
    }
}
