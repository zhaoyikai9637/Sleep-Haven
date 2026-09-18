using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SleepHaven;

public sealed class WeatherService
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(30);
    private static readonly IReadOnlyDictionary<string, (double Latitude, double Longitude)> Locations =
        new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Singapore"] = (1.3521, 103.8198),
            ["Qingdao"] = (36.0671, 120.3826)
        };

    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly ConcurrentDictionary<string, CachedWeather> _cache = new(StringComparer.OrdinalIgnoreCase);

    public WeatherService(
        HttpClient httpClient,
        ILogger<WeatherService>? logger = null,
        TimeProvider? timeProvider = null)
    {
        _httpClient = httpClient;
        _logger = logger ?? NullLogger<WeatherService>.Instance;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<WeatherSnapshot> GetComfortForecastAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        if (!Locations.TryGetValue(cityName, out var location))
        {
            throw new ArgumentException($"Unsupported city: {cityName}", nameof(cityName));
        }

        var now = _timeProvider.GetUtcNow();
        if (_cache.TryGetValue(cityName, out var cached) && now - cached.StoredAt < CacheLifetime)
        {
            return cached.Snapshot;
        }

        var url = FormattableString.Invariant(
            $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m&daily=temperature_2m_min&timezone=auto&forecast_days=2&temperature_unit=celsius");

        try
        {
            var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, cancellationToken)
                ?? throw new InvalidOperationException("Weather service returned an empty response.");
            var localTime = DateTime.ParseExact(
                response.Current.Time,
                "yyyy-MM-dd'T'HH:mm",
                CultureInfo.InvariantCulture);
            var nightIndex = localTime.Hour < 7 ? 0 : 1;
            if (response.Daily.MinimumTemperature.Length <= nightIndex)
            {
                throw new InvalidOperationException("Weather service did not provide the upcoming night forecast.");
            }

            var snapshot = new WeatherSnapshot(
                cityName,
                response.Current.Temperature,
                response.Daily.MinimumTemperature[nightIndex],
                DateOnly.FromDateTime(localTime),
                Math.Abs(location.Latitude) <= 23.5);
            _cache[cityName] = new CachedWeather(snapshot, now);
            return snapshot;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Weather lookup failed for supported city {CityName}.", cityName);
            throw;
        }
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

    private sealed record CachedWeather(WeatherSnapshot Snapshot, DateTimeOffset StoredAt);

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
