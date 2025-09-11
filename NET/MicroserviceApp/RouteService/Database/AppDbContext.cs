using Microsoft.EntityFrameworkCore;
using RouteService.Database.Model;

namespace RouteService.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .ToTable("cities");

            modelBuilder.Entity<Connection>()
                .ToTable("connections");

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
