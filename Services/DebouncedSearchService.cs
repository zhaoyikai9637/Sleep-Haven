namespace SleepHaven;

public sealed class DebouncedSearchService<T> : IDisposable
{
    private readonly TimeSpan _delay;
    private readonly object _gate = new();
    private CancellationTokenSource? _pendingSearch;

    public DebouncedSearchService(TimeSpan? delay = null)
    {
        _delay = delay ?? TimeSpan.FromMilliseconds(250);
    }

    public async Task<IReadOnlyList<T>> SearchAsync(
        string query,
        Func<string, CancellationToken, Task<IReadOnlyList<T>>> searchOperation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(searchOperation);

        CancellationTokenSource current;
        lock (_gate)
        {
            _pendingSearch?.Cancel();
            _pendingSearch = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            current = _pendingSearch;
        }

        try
        {
            await Task.Delay(_delay, current.Token);
            return await searchOperation(query, current.Token);
        }
        finally
        {
            lock (_gate)
            {
                if (ReferenceEquals(_pendingSearch, current))
                {
                    _pendingSearch = null;
                }
            }

            current.Dispose();
        }
    }

    public void Cancel()
    {
        lock (_gate)
        {
            _pendingSearch?.Cancel();
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            _pendingSearch?.Cancel();
            _pendingSearch?.Dispose();
            _pendingSearch = null;
        }
    }
}
