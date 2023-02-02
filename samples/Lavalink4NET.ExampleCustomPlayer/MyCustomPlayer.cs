namespace Lavalink4NET.ExampleCustomPlayer;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

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
    private readonly static Dictionary<ulong, float> _volumes = new();

    public override async ValueTask SetVolumeAsync(float volume = 1, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        // call the base method, without using it the volume would remain the same.
        await base
            .SetVolumeAsync(volume, cancellationToken)
            .ConfigureAwait(false);

        // store the volume of the player
        _volumes[GuildId] = volume;
    }

    /// <summary>
    ///     Asynchronously triggered when the player has connected to a voice channel.
    /// </summary>
    /// <param name="voiceServer">the voice server connected to</param>
    /// <param name="voiceState">the voice state</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public async override Task OnConnectedAsync(VoiceServer voiceServer, VoiceState voiceState)
    {
        // check if a volume has been stored
        if (_volumes.TryGetValue(GuildId, out var volume) && Volume != volume)
        {
            // here the volume is restored
            await SetVolumeAsync(volume);
        }

        await base.OnConnectedAsync(voiceServer, voiceState);
    }
}
