using System.Globalization;
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

    public async Task<WeatherSnapshot> GetComfortForecastAsync(string cityName, CancellationToken cancellationToken = default)
    {
        if (!Locations.TryGetValue(cityName, out var location))
        {
            throw new ArgumentException($"Unsupported city: {cityName}", nameof(cityName));
        }

        var url = FormattableString.Invariant($"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m&daily=temperature_2m_min&timezone=auto&forecast_days=2&temperature_unit=celsius");
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, cancellationToken)
            ?? throw new InvalidOperationException("Weather service returned an empty response.");

        var localTime = DateTime.ParseExact(response.Current.Time, "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);
        var nightIndex = localTime.Hour < 7 ? 0 : 1;
        if (response.Daily.MinimumTemperature.Length <= nightIndex)
        {
            throw new InvalidOperationException("Weather service did not provide the upcoming night forecast.");
        }

        return new WeatherSnapshot(
            cityName,
            response.Current.Temperature,
            response.Daily.MinimumTemperature[nightIndex],
            DateOnly.FromDateTime(localTime),
            Math.Abs(location.Latitude) <= 23.5);
    }

    public static string GetRecommendedSeason(WeatherSnapshot weather)
    {
        var nightLow = weather.NightLowTemperature;
        if (weather.IsTropical)
        {
            return nightLow >= 22 ? "Summer" : "Spring";
        }

        if (nightLow < 10)
        {
            return "Winter";
        }

        if (nightLow >= 27)
        {
            return "Summer";
        }

        return weather.LocalDate.Month switch
        {
            >= 9 and <= 11 => "Autumn",
            12 or 1 or 2 => nightLow < 18 ? "Winter" : "Autumn",
            >= 3 and <= 5 => "Spring",
            _ => nightLow < 18 ? "Spring" : "Summer"
        };
    }

    private sealed class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public required CurrentWeather Current { get; init; }

        [JsonPropertyName("daily")]
        public required DailyWeather Daily { get; init; }
    }

    private sealed class CurrentWeather
    {
        [JsonPropertyName("time")]
        public required string Time { get; init; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; init; }
    }

    private sealed class DailyWeather
    {
        [JsonPropertyName("temperature_2m_min")]
        public required double[] MinimumTemperature { get; init; }
    }
}

public sealed record WeatherSnapshot(
    string CityName,
    double Temperature,
    double NightLowTemperature,
    DateOnly LocalDate,
    bool IsTropical);
