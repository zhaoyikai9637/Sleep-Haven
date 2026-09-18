namespace SleepHaven;

public sealed class AsyncNavigationGuard
{
    private int _isNavigating;

    public async Task<bool> TryRunAsync(Func<Task> navigation)
    {
        ArgumentNullException.ThrowIfNull(navigation);
        if (Interlocked.CompareExchange(ref _isNavigating, 1, 0) != 0)
        {
            return false;
        }

        try
        {
            await navigation();
            return true;
        }
        finally
        {
            Volatile.Write(ref _isNavigating, 0);
        }
    }
}
