using Microsoft.EntityFrameworkCore;
using SleepHaven;
using SleepHaven.Api;
using SleepHaven.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CatalogDbContext>((services, options) =>
{
    var connectionString = services.GetRequiredService<IConfiguration>()
        .GetConnectionString("Catalog")
        ?? throw new InvalidOperationException(
            "ConnectionStrings:Catalog is required. Set ConnectionStrings__Catalog in the environment.");
    options.UseNpgsql(connectionString, postgres => postgres.EnableRetryOnFailure());
});
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await database.Database.MigrateAsync();
    await CatalogSeeder.SeedAsync(database);
}

app.MapGet("/health/database", async (CatalogDbContext database, CancellationToken cancellationToken) =>
{
    await database.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
    return Results.Ok(new { Status = "Healthy", Database = "PostgreSQL" });
});

app.MapGet("/api/products", async (
    HttpRequest request,
    CatalogDbContext database,
    CancellationToken cancellationToken) =>
{
    if (!ClientIdentity.TryRead(request, out var clientId, out var error))
    {
        return error;
    }

    var products = await database.Products
        .AsNoTracking()
        .OrderBy(product => product.Id)
        .ToListAsync(cancellationToken);
    await ApplyFavoritesAsync(database, clientId, products, cancellationToken);
    return Results.Ok(products);
});

app.MapGet("/api/products/{id}", async (
    string id,
    HttpRequest request,
    CatalogDbContext database,
    CancellationToken cancellationToken) =>
{
    if (!ClientIdentity.TryRead(request, out var clientId, out var error))
    {
        return error;
    }

    var product = await database.Products.AsNoTracking()
        .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
    if (product is null)
    {
        return Results.NotFound();
    }

    product.IsFavorite = await database.Favorites.AsNoTracking()
        .AnyAsync(item => item.ClientId == clientId && item.ProductId == id, cancellationToken);
    return Results.Ok(product);
});

app.MapGet("/api/favorites", async (
    HttpRequest request,
    CatalogDbContext database,
    CancellationToken cancellationToken) =>
{
    if (!ClientIdentity.TryRead(request, out var clientId, out var error))
    {
        return error;
    }

    var products = await database.Favorites.AsNoTracking()
        .Where(favorite => favorite.ClientId == clientId)
        .OrderBy(favorite => favorite.ProductId)
        .Select(favorite => favorite.Product)
        .ToListAsync(cancellationToken);
    products.ForEach(product => product.IsFavorite = true);
    return Results.Ok(products);
});

app.MapPut("/api/products/{id}/favorite", async (
    string id,
    FavoriteUpdate update,
    HttpRequest request,
    CatalogDbContext database,
    CancellationToken cancellationToken) =>
{
    if (!ClientIdentity.TryRead(request, out var clientId, out var error))
    {
        return error;
    }

    if (!await database.Products.AnyAsync(product => product.Id == id, cancellationToken))
    {
        return Results.NotFound();
    }

    var favorite = await database.Favorites.FindAsync(new object[] { clientId, id }, cancellationToken);
    if (update.IsFavorite && favorite is null)
    {
        database.Favorites.Add(new Favorite
        {
            ClientId = clientId,
            ProductId = id,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }
    else if (!update.IsFavorite && favorite is not null)
    {
        database.Favorites.Remove(favorite);
    }

    await database.SaveChangesAsync(cancellationToken);
    return Results.NoContent();
});

app.Run();

static async Task ApplyFavoritesAsync(
    CatalogDbContext database,
    string clientId,
    List<Product> products,
    CancellationToken cancellationToken)
{
    var favoriteIds = await database.Favorites.AsNoTracking()
        .Where(favorite => favorite.ClientId == clientId)
        .Select(favorite => favorite.ProductId)
        .ToHashSetAsync(cancellationToken);
    products.ForEach(product => product.IsFavorite = favoriteIds.Contains(product.Id));
}

public sealed record FavoriteUpdate(bool IsFavorite);

public partial class Program;
