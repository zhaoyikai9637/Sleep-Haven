namespace SleepHaven.Tests;

public sealed class AsyncNavigationGuardTests
{
    [Fact]
    public async Task ConcurrentNavigationIsIgnoredUntilCurrentNavigationCompletes()
    {
        var guard = new AsyncNavigationGuard();
        var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = guard.TryRunAsync(() => releaseFirst.Task);

        var secondExecuted = false;
        var secondAccepted = await guard.TryRunAsync(() =>
        {
            secondExecuted = true;
            return Task.CompletedTask;
        });
        releaseFirst.SetResult();

        Assert.True(await first);
        Assert.False(secondAccepted);
        Assert.False(secondExecuted);
    }

    [Fact]
    public async Task NavigationGuardReleasesAfterFailure()
    {
        var guard = new AsyncNavigationGuard();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            guard.TryRunAsync(() => throw new InvalidOperationException("Navigation failed")));

        Assert.True(await guard.TryRunAsync(() => Task.CompletedTask));
    }
}
