namespace SleepHaven.Tests;

public sealed class ProductSeedDataTests
{
    [Fact]
    public void ProductIdsAreUnique()
    {
        var duplicateIds = ProductSeedData.All
            .GroupBy(product => product.Id, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        Assert.Empty(duplicateIds);
    }

    [Fact]
    public void ProductImageReferencesExist()
    {
        var imageDirectory = Path.Combine(FindRepositoryRoot(), "Resources", "Images");
        var missingFiles = ProductSeedData.All
            .SelectMany(product => new[] { product.ThumbnailUrl, product.LandscapeUrl })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(fileName => !File.Exists(Path.Combine(imageDirectory, fileName)))
            .ToArray();

        Assert.Empty(missingFiles);
    }

    [Fact]
    public void ProductCommercialFieldsAreValid()
    {
        Assert.All(ProductSeedData.All, product =>
        {
            Assert.True(product.Price >= 0m, $"{product.Id} has a negative price.");
            Assert.True(Enum.IsDefined(product.ProductType), $"{product.Id} has an invalid product type.");
            Assert.True(Enum.IsDefined(product.Season), $"{product.Id} has an invalid season.");
            Assert.True(Enum.IsDefined(product.Currency), $"{product.Id} has an invalid currency.");
            Assert.True(Enum.IsDefined(product.Material), $"{product.Id} has an invalid material.");
        });
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "SleepHaven.csproj")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
