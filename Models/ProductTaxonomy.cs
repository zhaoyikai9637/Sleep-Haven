namespace SleepHaven;

public enum CurrencyCode
{
    USD = 1
}

public enum ProductType
{
    BeddingSet = 1,
    Quilt = 2,
    Pillow = 3,
    Mattress = 4
}

public enum ProductSeason
{
    Spring = 1,
    Summer = 2,
    Autumn = 3,
    Winter = 4
}

public enum ProductMaterial
{
    Cotton = 1,
    Silk = 2,
    TencelCotton = 3,
    GooseDown = 4,
    FiberBlend = 5,
    Buckwheat = 6,
    MemoryFoam = 7,
    BambooCharcoal = 8
}

public static class ProductTaxonomyExtensions
{
    public static string ToDisplayName(this ProductType productType) => productType switch
    {
        ProductType.BeddingSet => "Bedding sets",
        ProductType.Quilt => "Quilts",
        ProductType.Pillow => "Pillows",
        ProductType.Mattress => "Mattresses",
        _ => productType.ToString()
    };

    public static string ToDisplayName(this ProductMaterial material) => material switch
    {
        ProductMaterial.TencelCotton => "Tencel cotton",
        ProductMaterial.GooseDown => "Goose down",
        ProductMaterial.FiberBlend => "Fibre blend",
        ProductMaterial.Buckwheat => "Buckwheat",
        ProductMaterial.MemoryFoam => "Memory foam",
        ProductMaterial.BambooCharcoal => "Bamboo charcoal",
        _ => material.ToString()
    };
}
