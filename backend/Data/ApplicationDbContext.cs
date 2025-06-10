using Microsoft.EntityFrameworkCore;
using AssetWeb.Models;

namespace AssetWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Site> Sites { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Company>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Company>(c => c.UserId);

            modelBuilder.Entity<Site>()
                .HasOne(s => s.Company)
                .WithMany(c => c.Sites)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Site)
                .WithMany()
                .HasForeignKey(l => l.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Company)
                .WithMany()
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
