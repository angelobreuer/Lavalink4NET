namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;

/// <summary>
///     Event arguments for the <see cref="LavalinkNode.PlayerConnected"/> event.
/// </summary>
public sealed class PlayerConnectedEventArgs : PlayerEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerConnectedEventArgs"/> class.
    /// </summary>
    /// <param name="player">the affected player</param>
    /// <param name="voiceChannelId">
    ///     the snowflake identifier of the voice channel connected to
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
    /// </exception>
    public PlayerConnectedEventArgs(ILavalinkPlayer player, ulong voiceChannelId) : base(player)
    {
        VoiceChannelId = voiceChannelId;
    }

    /// <summary>
    ///     Gets the snowflake identifier of the voice channel connected to.
    /// </summary>
    public ulong VoiceChannelId { get; }
}
