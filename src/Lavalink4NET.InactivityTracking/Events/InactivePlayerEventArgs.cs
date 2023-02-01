namespace Lavalink4NET.InactivityTracking.Events;

using System;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;

/// <summary>
///     The event arguments for the <see cref="InactivityTrackingService.InactivePlayer"/>.
/// </summary>
public sealed class InactivePlayerEventArgs : EventArgs
{
    /// <summary>
    ///     Initialize a new instance of the <see cref="InactivePlayerEventArgs"/> class.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="player">the affected player</param>
    public InactivePlayerEventArgs(IAudioService audioService, ILavalinkPlayer player)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(player);

        AudioService = audioService;
        Player = player;
    }

    /// <summary>
    ///     Gets the audio service.
    /// </summary>
    public IAudioService AudioService { get; }

    /// <summary>
    ///     Gets the affected player.
    /// </summary>
    public ILavalinkPlayer Player { get; }

    /// <summary>
    ///     Gets a value indicating whether the player should be stopped.
    /// </summary>
    public bool ShouldStop { get; set; } = true;
}
