namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lavalink4NET.Tracks;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
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

    public string? Identifier => _value switch
    {
        string identifier => identifier,
        LavalinkTrack track => track.Identifier,
        _ => null,
    };

    public override string ToString() => Identifier ?? string.Empty;

    internal string GetDebuggerDisplay() => Track?.GetDebuggerDisplay() ?? ToString();
}