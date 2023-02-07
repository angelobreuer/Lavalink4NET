namespace Lavalink4NET.Clients;

using System;
using System.Diagnostics.CodeAnalysis;
using Lavalink4NET.Players;

public readonly record struct PlayerJoinResult<TPlayer> where TPlayer : class, ILavalinkPlayer
{
    public PlayerJoinStatus Status { get; }

    public TPlayer? Player { get; }

    [MemberNotNullWhen(true, nameof(Player))]
    public bool IsSuccess => Player is not null;

    public PlayerJoinResult(TPlayer player)
    {
        Player = player;
        Status = PlayerJoinStatus.Success;
    }

    public PlayerJoinResult(PlayerJoinStatus status)
    {
        if (status is not PlayerJoinStatus.Success)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(status),
                actualValue: status,
                message: "The status 'Success' is not valid.");
        }

        Player = null;
        Status = status;
    }
}
