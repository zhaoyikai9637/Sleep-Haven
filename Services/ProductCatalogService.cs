namespace SleepHaven;

public sealed class ProductCatalogService
{
    private readonly IProductStore _store;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private IReadOnlyList<Product>? _cachedProducts;

    public ProductCatalogService(IProductStore store) => _store = store;

    public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedProducts is not null)
        {
            return _cachedProducts;
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            _cachedProducts ??= await _store.GetAllProductsAsync(cancellationToken);
            return _cachedProducts;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var normalized = query.Trim();
        if (normalized.Length == 0)
        {
            return [];
        }

        var products = await GetProductsAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return products.Where(product =>
                product.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                product.Description.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                product.Material.ToDisplayName().Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                product.Season.ToString().Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                product.ProductType.ToDisplayName().Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    public async Task<IReadOnlyList<Product>> GetBySeasonAsync(ProductSeason season, CancellationToken cancellationToken = default)
    {
        var products = await GetProductsAsync(cancellationToken);
        return products.Where(product => product.Season == season).ToArray();
    }

    public async Task<IReadOnlyList<Product>> GetByTypeAsync(ProductType type, CancellationToken cancellationToken = default)
    {
        var products = await GetProductsAsync(cancellationToken);
        return products.Where(product => product.ProductType == type).ToArray();
    }

    public void Invalidate() => _cachedProducts = null;
}
