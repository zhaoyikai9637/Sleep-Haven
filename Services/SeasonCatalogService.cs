namespace SleepHaven;

public sealed class SeasonCatalogService(ProductCatalogService catalogService)
{
    private static readonly SeasonDefinition[] Definitions =
    [
        new(ProductSeason.Spring, "Spring layers", "Breathable cotton and soft structure"),
        new(ProductSeason.Summer, "Summer lightness", "Silk and cooling natural fibres"),
        new(ProductSeason.Autumn, "Autumn balance", "Comfort for cooler, drier nights"),
        new(ProductSeason.Winter, "Winter warmth", "Insulating loft without excess weight")
    ];

    public async Task<IReadOnlyList<SeasonSection>> GetSectionsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await catalogService.GetProductsAsync(cancellationToken);
        return Definitions.Select(definition => new SeasonSection(
                definition.Season.ToString(),
                definition.Title,
                definition.Summary,
                products.Where(product => product.Season == definition.Season)))
            .ToArray();
    }

    private sealed record SeasonDefinition(ProductSeason Season, string Title, string Summary);
}
