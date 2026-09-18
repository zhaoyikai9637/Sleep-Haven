namespace SleepHaven.Tests;

public sealed class ProductCatalogServiceTests
{
    [Fact]
    public async Task RepeatedSearchesShareOneCatalogueRead()
    {
        var store = new CountingProductStore([
            new Product
            {
                Id = "p1",
                Name = "Silk quilt",
                Description = "Light layer",
                Price = 10m,
                Currency = CurrencyCode.USD,
                ProductType = ProductType.Quilt,
                Season = ProductSeason.Summer,
                Material = ProductMaterial.Silk
            }
        ]);
        var catalog = new ProductCatalogService(store);

        Assert.Single(await catalog.SearchAsync("silk"));
        Assert.Single(await catalog.SearchAsync("summer"));
        Assert.Equal(1, store.ReadCount);
    }

    private sealed class CountingProductStore(IReadOnlyList<Product> products) : IProductStore
    {
        public int ReadCount { get; private set; }

        public Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            ReadCount++;
            return Task.FromResult(products.ToList());
        }

        public Task<List<Product>> GetFavoriteProductsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new List<Product>());

        public Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(products.FirstOrDefault(product => product.Id == id));

        public Task SetFavoriteAsync(string id, bool isFavorite, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
