namespace Lavalink4NET.Clients;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

/// <summary>
///     The interface for implementing a discord client wrapper for usage with the lavalink
///     audio service.
/// </summary>
public interface IDiscordClientWrapper
{
    /// <summary>
    ///     An asynchronous event which is triggered when the voice server was updated.
    /// </summary>
    event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    /// <summary>
    ///     An asynchronous event which is triggered when a user voice state was updated.
    /// </summary>
    event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    /// <summary>
    ///     Gets the snowflake identifier values of the users in the voice channel specified by
    ///     <paramref name="voiceChannelId"/> (the snowflake identifier of the voice channel).
    /// </summary>
    /// <param name="guildId">the guild identifier snowflake where the channel is in</param>
    /// <param name="voiceChannelId">the snowflake identifier of the voice channel</param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the snowflake identifier values of the users in the voice channel</para>
    /// </returns>
    ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(
        ulong guildId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends a voice channel state update asynchronously.
    /// </summary>
    /// <param name="guildId">the guild snowflake identifier</param>
    /// <param name="voiceChannelId">
    ///     the snowflake identifier of the voice channel to join (if <see langword="null"/> the
    ///     client should disconnect from the voice channel).
    /// </param>
    /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
    /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    ValueTask SendVoiceUpdateAsync(
        ulong guildId,
        ulong? voiceChannelId,
        bool selfDeaf = false,
        bool selfMute = false,
        CancellationToken cancellationToken = default);

    ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default);
}
