namespace Lavalink4NET.Players;

using System;
using System.Diagnostics.CodeAnalysis;
using Lavalink4NET.Players.Preconditions;

public readonly record struct PlayerResult<TPlayer> where TPlayer : class, ILavalinkPlayer
{
    private PlayerResult(TPlayer? player, PlayerRetrieveStatus status, IPlayerPrecondition? precondition)
    {
        Player = player;
        Status = status;
        Precondition = precondition;
    }

    public PlayerRetrieveStatus Status { get; }

    public IPlayerPrecondition? Precondition { get; }

    public TPlayer? Player { get; }

    [MemberNotNullWhen(true, nameof(Player))]
    public bool IsSuccess => Status is PlayerRetrieveStatus.Success && Player is not null;

    public static PlayerResult<TPlayer> Success(TPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return new(player, PlayerRetrieveStatus.Success, null);
    }

    public static PlayerResult<TPlayer> UserNotInVoiceChannel => new(null, PlayerRetrieveStatus.UserNotInVoiceChannel, null);

    public static PlayerResult<TPlayer> VoiceChannelMismatch => new(null, PlayerRetrieveStatus.VoiceChannelMismatch, null);

    public static PlayerResult<TPlayer> BotNotConnected => new(null, PlayerRetrieveStatus.BotNotConnected, null);

    public static PlayerResult<TPlayer> PreconditionFailed(TPlayer player, IPlayerPrecondition precondition)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(precondition);
        return new(player, PlayerRetrieveStatus.PreconditionFailed, precondition);
    }
}
