namespace Lavalink4NET.Players;

using System;
using System.Diagnostics.CodeAnalysis;
using Lavalink4NET.Tracks;

public readonly record struct TrackReference
{
    private readonly object _value; // either string or LavalinkTrack

    public TrackReference(LavalinkTrack track)
    {
        ArgumentNullException.ThrowIfNull(track);

        _value = track;
    }

    public TrackReference(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        _value = identifier;
    }

    [MemberNotNullWhen(true, nameof(Track))]
    public bool IsPresent => _value is LavalinkTrack;

    public LavalinkTrack? Track => _value as LavalinkTrack;

    public string? Identifier => _value as string;
}