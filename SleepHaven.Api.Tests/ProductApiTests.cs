using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SleepHaven.Api.Tests;

public sealed class ProductApiTests(PostgreSqlApiFactory factory)
    : IClassFixture<PostgreSqlApiFactory>
{
    [Fact]
    public async Task HealthEndpointConfirmsPostgreSqlConnection()
    {
        using var client = factory.CreateClient();

        var health = await client.GetFromJsonAsync<DatabaseHealth>("/health/database");

        Assert.NotNull(health);
        Assert.Equal("Healthy", health.Status);
        Assert.Equal("PostgreSQL", health.Database);
    }

    [Fact]
    public async Task SeededProductsAndFavoritesRoundTripThroughPostgreSql()
    {
        using var clientA = factory.CreateClient();
        using var clientB = factory.CreateClient();
        var storeA = new BackendProductStore(
            clientA,
            new BackendConnectionOptions(clientA.BaseAddress!.ToString(), $"test-a-{Guid.NewGuid():N}"));
        var storeB = new BackendProductStore(
            clientB,
            new BackendConnectionOptions(clientB.BaseAddress!.ToString(), $"test-b-{Guid.NewGuid():N}"));

        var products = await storeA.GetAllProductsAsync();
        Assert.NotEmpty(products);
        var product = products[0];

        await storeA.SetFavoriteAsync(product.Id, true);

        Assert.Contains(await storeA.GetFavoriteProductsAsync(), item => item.Id == product.Id && item.IsFavorite);
        Assert.DoesNotContain(await storeB.GetFavoriteProductsAsync(), item => item.Id == product.Id);
    }

    private sealed record DatabaseHealth(string Status, string Database);
}

public sealed class PostgreSqlApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("SLEEPHAVEN_TEST_POSTGRES")
        ?? throw new InvalidOperationException(
            "SLEEPHAVEN_TEST_POSTGRES must point to a disposable PostgreSQL test database.");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Catalog"] = _connectionString
            }));
    }
}
