namespace Lavalink4NET.InactivityTracking;

using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

public sealed class InactivityTrackerScope : IDisposable
{
    private readonly InactivityTrackerContext _inactivityTrackerContext;
    private readonly Dictionary<ulong, InactivityTrackerEntry> _entries;
    private readonly HashSet<ulong> _activatedPlayers;
    private readonly ImmutableDictionary<ulong, InactivityTrackerEntry>.Builder _trackedPlayers;
    private readonly int _counter;
    private bool _disposed;

    internal InactivityTrackerScope(InactivityTrackerContext inactivityTrackerContext, IImmutableDictionary<ulong, InactivityTrackerEntry> entries, int counter)
    {
        _inactivityTrackerContext = inactivityTrackerContext;
        _entries = entries.ToDictionary(x => x.Key, x => x.Value);
        _trackedPlayers = ImmutableDictionary.CreateBuilder<ulong, InactivityTrackerEntry>();
        _activatedPlayers = new HashSet<ulong>();
        _counter = counter;
    }

    public void MarkInactive(ulong guildId, TimeSpan? timeout = null)
    {
        ThrowIfDisposed();

        ref var entry = ref CollectionsMarshal.GetValueRefOrAddDefault(
            dictionary: _entries,
            key: guildId,
            exists: out bool exists);

        if (exists)
        {
            entry = new InactivityTrackerEntry(
                inactiveSince: entry.InactiveSince,
                timeout: timeout);
        }
        else
        {
            entry = new InactivityTrackerEntry(
                inactiveSince: _inactivityTrackerContext.SystemClock.UtcNow,
                timeout: timeout);
        }

        _activatedPlayers.Remove(guildId);
        _trackedPlayers[guildId] = entry;
    }

    public void MarkActive(ulong guildId)
    {
        ThrowIfDisposed();

        _entries.Remove(guildId);
        _activatedPlayers.Add(guildId);
        _trackedPlayers.Remove(guildId);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        var entries = ImmutableDictionary.CreateRange(_entries);

        _inactivityTrackerContext.ReturnScope(
            counter: _counter,
            entries: entries,
            activePlayers: _activatedPlayers.ToImmutableHashSet(),
            trackedPlayers: _trackedPlayers.ToImmutable());
    }

    private void ThrowIfDisposed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
#endif
    }
}