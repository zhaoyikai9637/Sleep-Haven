using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SleepHaven;

public sealed class WeatherService
{
    private static readonly IReadOnlyDictionary<string, (double Latitude, double Longitude)> Locations =
        new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Singapore"] = (1.3521, 103.8198),
            ["Qingdao"] = (36.0671, 120.3826)
        };

    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<WeatherSnapshot> GetCurrentAsync(string cityName, CancellationToken cancellationToken = default)
    {
        if (!Locations.TryGetValue(cityName, out var location))
        {
            throw new ArgumentException($"Unsupported city: {cityName}", nameof(cityName));
        }

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m,relative_humidity_2m,weather_code";
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, cancellationToken)
            ?? throw new InvalidOperationException("Weather service returned an empty response.");

        return new WeatherSnapshot(
            cityName,
            response.Current.Temperature,
            response.Current.Humidity,
            IsRain(response.Current.WeatherCode));
    }

    private static bool IsRain(int code) =>
        code is >= 51 and <= 67 or >= 80 and <= 82 or >= 95 and <= 99;

    private sealed class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public required CurrentWeather Current { get; init; }
    }

    private sealed class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; init; }

        [JsonPropertyName("relative_humidity_2m")]
        public int Humidity { get; init; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; init; }
    }
}

public sealed record WeatherSnapshot(string CityName, double Temperature, int Humidity, bool IsRainy);
