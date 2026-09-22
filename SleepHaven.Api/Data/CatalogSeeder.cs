using Microsoft.EntityFrameworkCore;
using SleepHaven;

namespace SleepHaven.Api.Data;

public static class CatalogSeeder
{
    public static async Task SeedAsync(
        CatalogDbContext database,
        CancellationToken cancellationToken = default)
    {
        var existing = await database.Products
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        foreach (var seed in ProductSeedData.All)
        {
            if (!existing.TryGetValue(seed.Id, out var product))
            {
                product = new Product { Id = seed.Id };
                database.Products.Add(product);
            }

            product.Name = seed.Name;
            product.Price = seed.Price;
            product.Currency = seed.Currency;
            product.ProductType = seed.ProductType;
            product.Season = seed.Season;
            product.Material = seed.Material;
            product.Description = seed.Description;
            product.ThumbnailUrl = seed.ThumbnailUrl;
            product.LandscapeUrl = seed.LandscapeUrl;
        }

        await database.SaveChangesAsync(cancellationToken);
    }
}
