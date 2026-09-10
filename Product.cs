using SQLite; // Introduce the SQLite library

namespace SleepHaven;

public class Product
{
    // [PrimaryKey]: This is the unique identification number for this row of data. It must not be duplicated.
    [PrimaryKey]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Price { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }

    // Horizontal and vertical screen dual-image attribute
    public string ThumbnailUrl { get; set; } // Vertical image / thumbnail: Used for the list on the homepage and the vertical screen details.
    public string LandscapeUrl { get; set; } // Horizontal/panoramic image: Specifically designed for display on mobile devices in landscape mode.

    // Record in the local database whether the user has clicked the heart icon for this product.
    [Column("IsFavorite")]
    public bool IsFavorite { get; set; }
}