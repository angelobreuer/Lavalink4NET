namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;

/// <summary>
///     Event arguments for the <see cref="LavalinkNodeBase.PlayerDisconnected"/> event.
/// </summary>
public sealed class PlayerDisconnectedEventArgs : PlayerEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerDisconnectedEventArgs"/> class.
    /// </summary>
    /// <param name="player">the affected player</param>
    /// <param name="voiceChannelId">
    ///     the snowflake identifier of the voice channel disconnected from
    /// </param>
    /// <param name="disconnectCause">the reason why the player disconnected</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
    /// </exception>
    public PlayerDisconnectedEventArgs(ILavalinkPlayer player, ulong voiceChannelId, PlayerDisconnectCause disconnectCause) : base(player)
    {
        VoiceChannelId = voiceChannelId;
        DisconnectCause = disconnectCause;
    }

    /// <summary>
    ///     Gets the reason why the player disconnected.
    /// </summary>
    public PlayerDisconnectCause DisconnectCause { get; }

    /// <summary>
    ///     Gets the snowflake identifier of the voice channel disconnected from.
    /// </summary>
    public ulong VoiceChannelId { get; }
}
