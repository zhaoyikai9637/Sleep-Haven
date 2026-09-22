using Microsoft.EntityFrameworkCore;
using SleepHaven;

namespace SleepHaven.Api.Data;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<Product>();
        product.ToTable("products");
        product.HasKey(item => item.Id);
        product.Property(item => item.Id).HasMaxLength(64);
        product.Property(item => item.Name).HasMaxLength(255).IsRequired();
        product.Property(item => item.Price).HasPrecision(12, 2);
        product.Property(item => item.Description).IsRequired();
        product.Property(item => item.ThumbnailUrl).HasMaxLength(255).IsRequired();
        product.Property(item => item.LandscapeUrl).HasMaxLength(255).IsRequired();
        product.Ignore(item => item.IsFavorite);
        product.Ignore(item => item.FormattedPrice);
        product.Ignore(item => item.CategoryDisplay);

        var favorite = modelBuilder.Entity<Favorite>();
        favorite.ToTable("favorites");
        favorite.HasKey(item => new { item.ClientId, item.ProductId });
        favorite.Property(item => item.ClientId).HasMaxLength(128);
        favorite.Property(item => item.ProductId).HasMaxLength(64);
        favorite.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        favorite.HasIndex(item => item.ProductId);
    }
}
