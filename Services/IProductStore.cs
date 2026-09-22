namespace SleepHaven;

public interface IProductStore
{
    Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> GetFavoriteProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default);
    Task SetFavoriteAsync(string id, bool isFavorite, CancellationToken cancellationToken = default);
}
