using SQLite;

namespace SleepHaven;

public sealed class DatabaseService
{
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static SQLiteAsyncConnection? _database;

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database is not null)
        {
            return _database;
        }

        await InitializationLock.WaitAsync();
        try
        {
            if (_database is null)
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "SleepHaven_v8.db3");
                _database = new SQLiteAsyncConnection(path);
                await _database.CreateTableAsync<Product>();

                if (await _database.Table<Product>().CountAsync() == 0)
                {
                    await _database.InsertAllAsync(ProductSeedData.All);
                }
            }

            return _database;
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        var database = await GetDatabaseAsync();
        return await database.Table<Product>().ToListAsync();
    }

    public async Task<List<Product>> GetFavoriteProductsAsync()
    {
        var database = await GetDatabaseAsync();
        return await database.Table<Product>().Where(product => product.IsFavorite).ToListAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        var database = await GetDatabaseAsync();
        await database.UpdateAsync(product);
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        var database = await GetDatabaseAsync();
        return await database.Table<Product>().FirstOrDefaultAsync(product => product.Id == id);
    }
}
