namespace Lavalink4NET.Tests;

using System.Threading.Tasks;
using Lavalink4NET.Events;
using Xunit;

/// <summary>
///     Contains tests for the asynchronous event handler ( <see cref="AsyncEventHandler"/>).
/// </summary>
public sealed class AsyncEventHandlerTests
{
    private event AsyncEventHandler Event;

    /// <summary>
    ///     Tests that the asynchronous event handler waits until all attached handlers were executed.
    /// </summary>
    [Fact]
    public async Task TestAwaiting()
    {
        Event = null;

        // add handlers
        Event += (sender, args) => Task.CompletedTask;
        Event += (sender, args) => Task.Delay(100);
        Event += (sender, args) => Task.Delay(1000);

        var task = Event.InvokeAsync(this).AsTask();

        // ensure that the task did not complete in 300 milliseconds
        Assert.NotSame(task, await Task.WhenAny(Task.Delay(300), task));
    }
}
