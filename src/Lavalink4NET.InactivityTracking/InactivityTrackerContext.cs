namespace Lavalink4NET.InactivityTracking;

using System;
using System.Collections.Immutable;
using System.Threading;
using Lavalink4NET.Players;
using Microsoft.Extensions.Internal;

internal sealed class InactivityTrackerContext : IInactivityTrackerContext
{
    private readonly IInactivityTrackingService _inactivityTrackingService;
    private IImmutableDictionary<ulong, InactivityTrackerEntry> _entries;
    private int _counter;
    private int _scopeState;

    public InactivityTrackerContext(
        IInactivityTrackingService inactivityTrackingService,
        IInactivityTracker inactivityTracker,
        ISystemClock systemClock)
    {
        ArgumentNullException.ThrowIfNull(inactivityTrackingService);
        ArgumentNullException.ThrowIfNull(inactivityTracker);
        ArgumentNullException.ThrowIfNull(systemClock);

        _inactivityTrackingService = inactivityTrackingService;
        InactivityTracker = inactivityTracker;
        SystemClock = systemClock;
        _entries = ImmutableDictionary<ulong, InactivityTrackerEntry>.Empty;
    }

    public InactivityTrackerEntry? GetEntry(ulong guildId)
    {
        return _entries.TryGetValue(guildId, out var entry)
            ? entry
            : null;
    }

    public InactivityTrackerEntry? GetEntry(ILavalinkPlayer player) => GetEntry(player.GuildId);

    internal ISystemClock SystemClock { get; }

    public IInactivityTracker InactivityTracker { get; }

    internal void ReturnScope(
        int counter,
        IImmutableDictionary<ulong, InactivityTrackerEntry> entries,
        IImmutableSet<ulong> activePlayers,
        IImmutableDictionary<ulong, InactivityTrackerEntry> trackedPlayers)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(activePlayers);
        ArgumentNullException.ThrowIfNull(trackedPlayers);

        if (_counter != counter || Interlocked.CompareExchange(ref _scopeState, ScopeState.None, ScopeState.Allocated) is not ScopeState.Allocated)
        {
            throw new InvalidOperationException("The scope is not active.");
        }

        _inactivityTrackingService.Report(InactivityTracker, activePlayers, trackedPlayers);
        _entries = entries;
    }

    public InactivityTrackerScope CreateScope()
    {
        if (Interlocked.CompareExchange(ref _scopeState, ScopeState.Allocated, ScopeState.None) is not ScopeState.None)
        {
            throw new InvalidOperationException("A scope is already active.");
        }

        var counter = Interlocked.Increment(ref _counter);
        return new InactivityTrackerScope(this, _entries, counter);
    }
}

file static class ScopeState
{
    public const int None = 0;
    public const int Allocated = 1;
}
