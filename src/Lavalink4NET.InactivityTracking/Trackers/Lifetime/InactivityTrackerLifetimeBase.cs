namespace Lavalink4NET.InactivityTracking.Trackers.Lifetime;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

internal abstract class InactivityTrackerLifetimeBase : IInactivityTrackerLifetime, IDisposable
{
    private readonly ILogger<InactivityTrackerLifetimeBase> _logger;
    private Task? _executeTask;
    private CancellationTokenSource? _shutdownCancellationTokenSource;
    private int _trackerState;
    private bool _disposed;

    protected InactivityTrackerLifetimeBase(
        string label,
        IInactivityTracker inactivityTracker,
        ILogger<InactivityTrackerLifetimeBase> logger)
    {
        ArgumentNullException.ThrowIfNull(inactivityTracker);
        ArgumentNullException.ThrowIfNull(logger);
        Label = label;
        InactivityTracker = inactivityTracker;
        _logger = logger;
    }

    public string Label { get; }

    public IInactivityTracker InactivityTracker { get; }

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Interlocked.CompareExchange(ref _trackerState, State.Starting, State.Stopped) is not State.Stopped)
        {
            return default;
        }

        _logger.InactivityTrackerStarting(Label);

        try
        {
            _shutdownCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executeTask = RunAsync(_shutdownCancellationTokenSource.Token).AsTask();

            Volatile.Write(ref _trackerState, State.Started);
            _logger.InactivityTrackerStarted(Label);
        }
        catch (Exception ex)
        {
            _logger.InactivityTrackerStartError(Label, ex);
            Volatile.Write(ref _trackerState, State.Stopped);
        }

        return default;
    }

    protected abstract ValueTask RunAsync(CancellationToken cancellationToken = default);

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Interlocked.CompareExchange(ref _trackerState, State.Stopping, State.Started) is not State.Started)
        {
            return;
        }

        _logger.InactivityTrackerStopping(Label);

        try
        {
            try
            {
                _shutdownCancellationTokenSource!.Cancel();
            }
            finally
            {
                await Task
                    .WhenAny(_executeTask!, Task.Delay(Timeout.Infinite, cancellationToken))
                    .ConfigureAwait(false);
            }

            Volatile.Write(ref _trackerState, State.Stopped);
            _logger.InactivityTrackerStopped(Label);
        }
        catch (Exception ex)
        {
            _logger.InactivityTrackerStopError(Label, ex);
            Volatile.Write(ref _trackerState, State.Started);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            _shutdownCancellationTokenSource?.Cancel();
            _shutdownCancellationTokenSource?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

internal static partial class Logging
{
    [LoggerMessage(EventId = 1, EventName = nameof(InactivityTrackerStarting), Level = LogLevel.Information, Message = "Starting inactivity tracker '{Label}'.")]
    public static partial void InactivityTrackerStarting(this ILogger<InactivityTrackerLifetimeBase> logger, string label);

    [LoggerMessage(EventId = 2, EventName = nameof(InactivityTrackerStarted), Level = LogLevel.Information, Message = "Started inactivity tracker '{Label}'.")]
    public static partial void InactivityTrackerStarted(this ILogger<InactivityTrackerLifetimeBase> logger, string label);

    [LoggerMessage(EventId = 3, EventName = nameof(InactivityTrackerStartError), Level = LogLevel.Error, Message = "Error while starting inactivity tracker '{Label}'.")]
    public static partial void InactivityTrackerStartError(this ILogger<InactivityTrackerLifetimeBase> logger, string label, Exception exception);

    [LoggerMessage(EventId = 4, EventName = nameof(InactivityTrackerStopping), Level = LogLevel.Information, Message = "Stopping inactivity tracker '{Label}'.")]
    public static partial void InactivityTrackerStopping(this ILogger<InactivityTrackerLifetimeBase> logger, string label);

    [LoggerMessage(EventId = 5, EventName = nameof(InactivityTrackerStopped), Level = LogLevel.Information, Message = "Stopped inactivity tracker '{Label}'.")]
    public static partial void InactivityTrackerStopped(this ILogger<InactivityTrackerLifetimeBase> logger, string label);

    [LoggerMessage(EventId = 6, EventName = nameof(InactivityTrackerStopError), Level = LogLevel.Error, Message = "Error while stopping inactivity tracker '{Label}'.")]
    public static partial void InactivityTrackerStopError(this ILogger<InactivityTrackerLifetimeBase> logger, string label, Exception exception);
}

file static class State
{
    public const int Stopped = 0;
    public const int Starting = 1;
    public const int Started = 2;
    public const int Stopping = 3;
    public const int Disposed = 4;
}
