using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SleepHaven.Tests;

public sealed class BackendProductStoreTests
{
    [Fact]
    public async Task GetAllProductsUsesBackendApiAndClientIdentity()
    {
        HttpRequestMessage? capturedRequest = null;
        var expected = ProductSeedData.All.Take(2).ToList();
        using var client = new HttpClient(new StubHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expected)
            };
        }));
        var store = new BackendProductStore(
            client,
            new BackendConnectionOptions("https://api.sleep.test/", "client-123"));

        var products = await store.GetAllProductsAsync();

        Assert.Equal(expected.Select(product => product.Id), products.Select(product => product.Id));
        Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
        Assert.Equal("https://api.sleep.test/api/products", capturedRequest.RequestUri!.ToString());
        Assert.Equal("client-123", capturedRequest.Headers.GetValues(BackendProductStore.ClientHeaderName).Single());
    }

    [Fact]
    public async Task SetFavoriteSendsTypedUpdateToBackend()
    {
        string? requestBody = null;
        using var client = new HttpClient(new StubHandler(async request =>
        {
            requestBody = await request.Content!.ReadAsStringAsync();
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("https://api.sleep.test/api/products/p001/favorite", request.RequestUri!.ToString());
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }));
        var store = new BackendProductStore(
            client,
            new BackendConnectionOptions("https://api.sleep.test/", "client-123"));

        await store.SetFavoriteAsync("p001", true);

        using var json = JsonDocument.Parse(requestBody!);
        Assert.True(json.RootElement.GetProperty("isFavorite").GetBoolean());
    }

    [Fact]
    public async Task BackendFailureIsNotSilentlyConvertedToEmptyCatalogue()
    {
        using var client = new HttpClient(new StubHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
        var store = new BackendProductStore(
            client,
            new BackendConnectionOptions("https://api.sleep.test/", "client-123"));

        await Assert.ThrowsAsync<HttpRequestException>(() => store.GetFavoriteProductsAsync());
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            : this(request => Task.FromResult(handler(request)))
        {
        }

        public StubHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler) => _handler = handler;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => _handler(request);
    }
}
