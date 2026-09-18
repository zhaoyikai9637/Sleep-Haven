using System.Net;
using System.Text;

namespace SleepHaven.Tests;

public sealed class WeatherServiceTests
{
    private const string ResponseJson = """
        {
          "current": { "time": "2026-10-15T12:00", "temperature_2m": 18.5 },
          "daily": { "temperature_2m_min": [12.0, 9.5] }
        }
        """;

    [Fact]
    public async Task RecentSuccessfulForecastIsReused()
    {
        var handler = new CountingHandler((_, _) => Task.FromResult(JsonResponse()));
        var service = new WeatherService(new HttpClient(handler));

        var first = await service.GetComfortForecastAsync("Qingdao");
        var second = await service.GetComfortForecastAsync("Qingdao");

        Assert.Equal(first, second);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task CallerCancellationCancelsWeatherRequest()
    {
        var handler = new CountingHandler(async (_, cancellationToken) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            return JsonResponse();
        });
        var service = new WeatherService(new HttpClient(handler));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(25));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            service.GetComfortForecastAsync("Qingdao", cancellation.Token));
    }

    private static HttpResponseMessage JsonResponse() => new(HttpStatusCode.OK)
    {
        Content = new StringContent(ResponseJson, Encoding.UTF8, "application/json")
    };

    private sealed class CountingHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            return responder(request, cancellationToken);
        }
    }
}
