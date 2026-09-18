using SQLite;

namespace SleepHaven.Tests;

public sealed class DatabaseMigrationTests : IAsyncLifetime
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"SleepHavenTests-{Guid.NewGuid():N}");

    public Task InitializeAsync()
    {
        Directory.CreateDirectory(_directory);
        SQLitePCL.Batteries_V2.Init();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Directory.Delete(_directory, recursive: true);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task FavoriteStatePersistsAcrossServiceInstances()
    {
        var path = Path.Combine(_directory, "catalog.db3");
        await using var first = new DatabaseService(path);
        await first.SetFavoriteAsync("p001", true);

        await using var second = new DatabaseService(path);
        var product = await second.GetProductByIdAsync("p001");

        Assert.NotNull(product);
        Assert.True(product.IsFavorite);
    }

    [Fact]
    public async Task LegacySchemaMigrationPreservesFavorites()
    {
        var legacyPath = Path.Combine(_directory, "SleepHaven_v8.db3");
        var currentPath = Path.Combine(_directory, "SleepHaven.db3");
        var legacy = new SQLiteAsyncConnection(legacyPath);
        await legacy.ExecuteAsync(
            "CREATE TABLE Product (Id varchar(64) primary key, Name varchar(255), Price varchar(32), Category varchar(128), Description text, ThumbnailUrl text, LandscapeUrl text, IsFavorite integer)");
        await legacy.ExecuteAsync(
            "INSERT INTO Product (Id, Name, Price, Category, Description, ThumbnailUrl, LandscapeUrl, IsFavorite) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
            "p003", "Legacy product", "$220.00", "BeddingSets - Summer", "Legacy", "old.png", "old.png", 1);
        await legacy.CloseAsync();

        await using var service = new DatabaseService(currentPath, legacyPath);
        var migrated = await service.GetProductByIdAsync("p003");

        Assert.NotNull(migrated);
        Assert.True(migrated.IsFavorite);
        Assert.Equal(220m, migrated.Price);
        Assert.Equal(ProductType.BeddingSet, migrated.ProductType);
        Assert.Equal(ProductSeason.Summer, migrated.Season);
        Assert.Equal(DatabaseService.CurrentSchemaVersion, await service.GetSchemaVersionAsync());
    }
}
