using System.Globalization;

namespace SleepHaven;

public class Product
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CurrencyCode Currency { get; set; } = CurrencyCode.USD;
    public ProductType ProductType { get; set; }
    public ProductSeason Season { get; set; }
    public ProductMaterial Material { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string LandscapeUrl { get; set; } = string.Empty;

    public bool IsFavorite { get; set; }

    public string FormattedPrice => Currency switch
    {
        CurrencyCode.USD => Price.ToString("C2", CultureInfo.GetCultureInfo("en-US")),
        _ => $"{Currency} {Price:0.00}"
    };

    public string CategoryDisplay => $"{ProductType.ToDisplayName()} · {Season}";
}
