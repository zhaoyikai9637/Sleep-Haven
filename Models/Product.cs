using SQLite; // Introduce the SQLite library

namespace SleepHaven;

public class Product
{
    // [PrimaryKey]: This is the unique identification number for this row of data. It must not be duplicated.
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Horizontal and vertical screen dual-image attribute
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string LandscapeUrl { get; set; } = string.Empty;

    // Record in the local database whether the user has clicked the heart icon for this product.
    [Column("IsFavorite")]
    public bool IsFavorite { get; set; }
}
