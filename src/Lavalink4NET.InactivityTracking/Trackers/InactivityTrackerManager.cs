namespace Lavalink4NET.InactivityTracking.Trackers;

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

internal sealed class InactivityTrackerManager : IInactivityTrackerManager
{
    private readonly IServiceProvider _serviceProvider;
    private ImmutableArray<IInactivityTracker>? _trackers;

    public InactivityTrackerManager(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public ImmutableArray<IInactivityTracker> Trackers => _trackers ??= _serviceProvider
        .GetServices<IInactivityTracker>()
        .ToImmutableArray();
}
