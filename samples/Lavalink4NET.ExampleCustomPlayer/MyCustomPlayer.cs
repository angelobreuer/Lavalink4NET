namespace Lavalink4NET.ExampleCustomPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lavalink4NET.Player;

    /// <summary>
    ///     A custom player that saves the last used volume.
    /// </summary>
    public sealed class MyCustomPlayer : LavalinkPlayer
    {
        /// <summary>
        ///     The dictionary holding the last used volumes.
        /// </summary>
        /// <remarks>
        ///     This can be replaced with a database or something else to keep the values persistent.
        /// </remarks>
        private readonly static IDictionary<ulong, float> _volumes = new Dictionary<ulong, float>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkPlayer"/> class.
        /// </summary>
        /// <param name="lavalinkSocket">the lavalink socket</param>
        /// <param name="client">the discord client</param>
        /// <param name="guildId">the identifier of the guild that is controlled by the player</param>
        public MyCustomPlayer(LavalinkSocket lavalinkSocket, IDiscordClientWrapper client, ulong guildId)
            : base(lavalinkSocket, client, guildId)
        {
            // This constructor pattern must be always used for a custom player. You can not add nor
            // remove any constructor parameters. The player class will be instantiated using System.Activator.
        }

        /// <summary>
        ///     Updates the player volume asynchronously.
        /// </summary>
        /// <param name="volume">the player volume (0f - 10f)</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the player is not connected to a voice channel
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified <paramref name="volume"/> is out of range (0f - 10f)
        /// </exception>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public override Task SetVolumeAsync(float volume = 1)
        {
            EnsureNotDestroyed();
            EnsureConnected();

            // store the volume of the player
            _volumes[GuildId] = volume;

            // call the base method, without using it the volume would remain the same.
            return base.SetVolumeAsync(volume);
        }

        /// <summary>
        ///     Asynchronously triggered when the player has connected to a voice channel.
        /// </summary>
        /// <param name="voiceServer">the voice server connected to</param>
        /// <param name="voiceState">the voice state</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        protected async override Task OnConnectedAsync(VoiceServer voiceServer, VoiceState voiceState)
        {
            // check if a volume has been stored
            if (_volumes.TryGetValue(GuildId, out var volume) && Volume != volume)
            {
                // here the volume is restored, we fire-and-forget the volume update.
                await SetVolumeAsync(volume);
            }

            await base.OnConnectedAsync(voiceServer, voiceState);
        }
    }
}