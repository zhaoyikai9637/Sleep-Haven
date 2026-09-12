namespace SleepHaven;

public sealed class SeasonSection
{
    public SeasonSection(string key, string title, string summary, IEnumerable<Product> products)
    {
        Key = key;
        Title = title;
        Summary = summary;
        Products = products.ToList();
    }

    public string Key { get; }
    public string Title { get; }
    public string Summary { get; }
    public IReadOnlyList<Product> Products { get; }
}

public sealed record CarouselItem(
    string Id,
    string ImageUrl,
    string Number,
    string Eyebrow,
    string Title,
    string Summary);
