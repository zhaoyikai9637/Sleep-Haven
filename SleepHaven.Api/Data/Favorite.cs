namespace SleepHaven.Api.Data;

public sealed class Favorite
{
    public string ClientId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public Product Product { get; set; } = null!;
}
