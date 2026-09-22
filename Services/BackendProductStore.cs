using System.Net;
using System.Net.Http.Json;

namespace SleepHaven;

public sealed record BackendConnectionOptions(string BaseUrl, string ClientId);

public sealed class BackendProductStore : IProductStore
{
    public const string ClientHeaderName = "X-SleepHaven-Client";

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;
    private readonly string _clientId;

    public BackendProductStore(HttpClient httpClient, BackendConnectionOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.BaseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ClientId);

        _httpClient = httpClient;
        _baseUri = new Uri(EnsureTrailingSlash(options.BaseUrl), UriKind.Absolute);
        _clientId = options.ClientId;
    }

    public Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default) =>
        GetListAsync("api/products", cancellationToken);

    public Task<List<Product>> GetFavoriteProductsAsync(CancellationToken cancellationToken = default) =>
        GetListAsync("api/favorites", cancellationToken);

    public async Task<Product?> GetProductByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        using var request = CreateRequest(HttpMethod.Get, $"api/products/{Uri.EscapeDataString(id)}");
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>(cancellationToken)
            ?? throw new InvalidDataException("The backend returned an empty product response.");
    }

    public async Task SetFavoriteAsync(
        string id,
        bool isFavorite,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        using var request = CreateRequest(HttpMethod.Put, $"api/products/{Uri.EscapeDataString(id)}/favorite");
        request.Content = JsonContent.Create(new FavoriteUpdate(isFavorite));
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<List<Product>> GetListAsync(string path, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, path);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Product>>(cancellationToken)
            ?? throw new InvalidDataException("The backend returned an empty product list.");
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, new Uri(_baseUri, path));
        request.Headers.Add(ClientHeaderName, _clientId);
        return request;
    }

    private static string EnsureTrailingSlash(string value) =>
        value.EndsWith("/", StringComparison.Ordinal) ? value : $"{value}/";

    private sealed record FavoriteUpdate(bool IsFavorite);
}
