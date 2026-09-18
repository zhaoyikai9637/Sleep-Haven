using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SQLite;

namespace SleepHaven;

public interface IProductStore
{
    Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> GetFavoriteProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default);
    Task SetFavoriteAsync(string id, bool isFavorite, CancellationToken cancellationToken = default);
}

public sealed class DatabaseService : IProductStore, IAsyncDisposable
{
    public const int CurrentSchemaVersion = 2;

    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private readonly string _databasePath;
    private readonly string? _legacyDatabasePath;
    private readonly ILogger<DatabaseService> _logger;
    private SQLiteAsyncConnection? _database;

    public DatabaseService(
        string databasePath,
        string? legacyDatabasePath = null,
        ILogger<DatabaseService>? logger = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        _databasePath = databasePath;
        _legacyDatabasePath = legacyDatabasePath;
        _logger = logger ?? NullLogger<DatabaseService>.Instance;
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync(CancellationToken cancellationToken)
    {
        if (_database is not null)
        {
            return _database;
        }

        await _initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_database is not null)
            {
                return _database;
            }

            ImportLegacyDatabaseIfNeeded();
            var database = new SQLiteAsyncConnection(_databasePath);
            await MigrateAsync(database);
            _database = database;
            return database;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private void ImportLegacyDatabaseIfNeeded()
    {
        if (File.Exists(_databasePath) || string.IsNullOrWhiteSpace(_legacyDatabasePath) || !File.Exists(_legacyDatabasePath))
        {
            return;
        }

        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.Copy(_legacyDatabasePath, _databasePath);
        _logger.LogInformation("Imported the legacy SleepHaven catalogue database for schema migration.");
    }

    private async Task MigrateAsync(SQLiteAsyncConnection database)
    {
        var tableInfo = await database.QueryAsync<TableColumn>("PRAGMA table_info('Product')");
        var hasLegacySchema = tableInfo.Any(column =>
            column.Name.Equals("Category", StringComparison.OrdinalIgnoreCase));
        var favorites = new Dictionary<string, bool>(StringComparer.Ordinal);

        if (tableInfo.Count > 0)
        {
            var favoriteRows = await database.QueryAsync<FavoriteRow>("SELECT Id, IsFavorite FROM Product");
            favorites = favoriteRows.ToDictionary(row => row.Id, row => row.IsFavorite, StringComparer.Ordinal);
        }

        if (hasLegacySchema)
        {
            await database.DropTableAsync<Product>();
            tableInfo.Clear();
            _logger.LogInformation("Migrated the product catalogue from the legacy combined category schema.");
        }

        if (tableInfo.Count == 0)
        {
            await database.CreateTableAsync<Product>();
        }

        await SynchronizeSeedDataAsync(database, favorites);
        await database.ExecuteAsync($"PRAGMA user_version = {CurrentSchemaVersion}");
    }

    private static async Task SynchronizeSeedDataAsync(
        SQLiteAsyncConnection database,
        IReadOnlyDictionary<string, bool> preservedFavorites)
    {
        var existingFavorites = (await database.Table<Product>().ToListAsync())
            .ToDictionary(product => product.Id, product => product.IsFavorite, StringComparer.Ordinal);

        foreach (var product in ProductSeedData.All)
        {
            var seedProduct = CloneSeedProduct(product);
            seedProduct.IsFavorite = preservedFavorites.TryGetValue(product.Id, out var migratedFavorite)
                ? migratedFavorite
                : existingFavorites.TryGetValue(product.Id, out var existingFavorite) && existingFavorite;
            await database.InsertOrReplaceAsync(seedProduct);
        }
    }

    private static Product CloneSeedProduct(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Currency = product.Currency,
        ProductType = product.ProductType,
        Season = product.Season,
        Material = product.Material,
        Description = product.Description,
        ThumbnailUrl = product.ThumbnailUrl,
        LandscapeUrl = product.LandscapeUrl
    };

    public async Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var database = await GetDatabaseAsync(cancellationToken);
        var products = await database.Table<Product>().ToListAsync();
        cancellationToken.ThrowIfCancellationRequested();
        return products;
    }

    public async Task<List<Product>> GetFavoriteProductsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var database = await GetDatabaseAsync(cancellationToken);
        var products = await database.Table<Product>().Where(product => product.IsFavorite).ToListAsync();
        cancellationToken.ThrowIfCancellationRequested();
        return products;
    }

    public async Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var database = await GetDatabaseAsync(cancellationToken);
        var product = await database.Table<Product>().FirstOrDefaultAsync(item => item.Id == id);
        cancellationToken.ThrowIfCancellationRequested();
        return product;
    }

    public async Task SetFavoriteAsync(string id, bool isFavorite, CancellationToken cancellationToken = default)
    {
        var database = await GetDatabaseAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await database.ExecuteAsync("UPDATE Product SET IsFavorite = ? WHERE Id = ?", isFavorite, id);
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        await SetFavoriteAsync(product.Id, product.IsFavorite, cancellationToken);
    }

    public async Task<int> GetSchemaVersionAsync(CancellationToken cancellationToken = default)
    {
        var database = await GetDatabaseAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return await database.ExecuteScalarAsync<int>("PRAGMA user_version");
    }

    public async ValueTask DisposeAsync()
    {
        if (_database is not null)
        {
            await _database.CloseAsync();
            _database = null;
        }

        _initializationLock.Dispose();
    }

    private sealed class TableColumn
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class FavoriteRow
    {
        public string Id { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }
}
