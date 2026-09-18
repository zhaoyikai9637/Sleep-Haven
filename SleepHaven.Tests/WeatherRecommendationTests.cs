namespace SleepHaven.Tests;

public sealed class WeatherRecommendationTests
{
    public static TheoryData<double, bool, int, string> Boundaries => new()
    {
        { 21.9, true, 7, "Spring" },
        { 22.0, true, 7, "Summer" },
        { 9.9, false, 10, "Winter" },
        { 10.0, false, 10, "Autumn" },
        { 17.9, false, 12, "Winter" },
        { 18.0, false, 12, "Autumn" },
        { 26.9, false, 10, "Autumn" },
        { 27.0, false, 10, "Summer" },
        { 22.0, false, 4, "Spring" },
        { 22.0, false, 7, "Summer" }
    };

    [Theory]
    [MemberData(nameof(Boundaries))]
    public void RecommendationUsesClimateMonthAndTemperatureBoundaries(
        double nightLow,
        bool isTropical,
        int month,
        string expected)
    {
        var weather = new WeatherSnapshot(
            "Test",
            20,
            nightLow,
            new DateOnly(2026, month, 15),
            isTropical);

        Assert.Equal(expected, WeatherService.GetRecommendedSeason(weather));
    }
}
