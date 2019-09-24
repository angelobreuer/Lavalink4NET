namespace Lavalink4NET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lavalink4NET.Events;

    /// <summary>
    ///     An dummy <see cref="IDiscordClientWrapper"/> implementation.
    /// </summary>
    /// <remarks>
    ///     All actions done with this <see cref="IDiscordClientWrapper"/> will result in a
    ///     <see cref="NotImplementedException"/> exception (except <see cref="VoiceServerUpdated"/>
    ///     and <see cref="VoiceStateUpdated"/>)
    /// </remarks>
    internal sealed class NullDiscordClientWrapper : IDiscordClientWrapper
    {
#pragma warning disable CS0067

        /// <summary>
        ///     An asynchronous event which is triggered when the voice server was updated.
        /// </summary>
        public event AsyncEventHandler<VoiceServer> VoiceServerUpdated;

        /// <summary>
        ///     An asynchronous event which is triggered when a user voice state was updated.
        /// </summary>
        public event AsyncEventHandler<VoiceStateUpdateEventArgs> VoiceStateUpdated;

#pragma warning restore CS0067

        /// <summary>
        ///     Gets the current user snowflake identifier value.
        /// </summary>
        public ulong CurrentUserId => throw new NotImplementedException();

        /// <summary>
        ///     Gets the number of total shards the bot uses.
        /// </summary>
        public int ShardCount => throw new NotImplementedException();

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
        public Task<IEnumerable<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId)
            => throw new NotImplementedException();

        /// <summary>
        ///     Awaits the initialization of the discord client asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public Task InitializeAsync() => throw new NotImplementedException();

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
        public Task SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false)
            => throw new NotImplementedException();
    }
}