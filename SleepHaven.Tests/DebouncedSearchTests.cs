namespace SleepHaven.Tests;

public sealed class DebouncedSearchTests
{
    [Fact]
    public async Task NewSearchCancelsThePreviousPendingSearch()
    {
        var executedQueries = new List<string>();
        var search = new DebouncedSearchService<string>(TimeSpan.FromMilliseconds(40));

        var first = search.SearchAsync("sil", (query, _) =>
        {
            executedQueries.Add(query);
            return Task.FromResult<IReadOnlyList<string>>([query]);
        });
        await Task.Delay(5);
        var second = search.SearchAsync("silk", (query, _) =>
        {
            executedQueries.Add(query);
            return Task.FromResult<IReadOnlyList<string>>([query]);
        });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => first);
        Assert.Equal(["silk"], await second);
        Assert.Equal(["silk"], executedQueries);
    }

    [Fact]
    public async Task CallerCancellationStopsPendingSearch()
    {
        using var cancellation = new CancellationTokenSource();
        var search = new DebouncedSearchService<string>(TimeSpan.FromSeconds(1));
        var pending = search.SearchAsync(
            "cotton",
            (query, _) => Task.FromResult<IReadOnlyList<string>>([query]),
            cancellation.Token);

        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);
    }
}
