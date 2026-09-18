namespace SleepHaven.Tests;

public sealed class SeasonCatalogServiceTests
{
    [Fact]
    public async Task SectionsContainOnlyProductsFromTheirOwnSeason()
    {
        var catalog = new ProductCatalogService(new InMemoryProductStore(ProductSeedData.All));
        var service = new SeasonCatalogService(catalog);

        var sections = await service.GetSectionsAsync();

        Assert.Equal(["Spring", "Summer", "Autumn", "Winter"], sections.Select(section => section.Key));
        Assert.All(sections, section => Assert.All(
            section.Products,
            product => Assert.Equal(section.Key, product.Season.ToString())));
    }

    private sealed class InMemoryProductStore(IReadOnlyList<Product> products) : IProductStore
    {
        public Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(products.ToList());

        public Task<List<Product>> GetFavoriteProductsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(products.Where(product => product.IsFavorite).ToList());

        public Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(products.FirstOrDefault(product => product.Id == id));

        public Task SetFavoriteAsync(string id, bool isFavorite, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
