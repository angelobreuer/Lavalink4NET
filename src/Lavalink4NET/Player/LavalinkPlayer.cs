/*
 *  File:   LavalinkPlayer.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.Player;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Lavalink4NET.Filters;
using Lavalink4NET.Payloads;
using Lavalink4NET.Payloads.Events;
using Lavalink4NET.Payloads.Node;
using Lavalink4NET.Payloads.Player;

/// <summary>
///     Controls a remote Lavalink Audio Player.
/// </summary>
public class LavalinkPlayer : IDisposable, IAsyncDisposable
{
    private PlayerFilterMap? _filterMap;

    /// <summary>
    ///     Gets the discord client wrapper.
    /// </summary>
    public IDiscordClientWrapper Client { get; private set; } = null!; // Lazy-initialized

    /// <summary>
    ///     Gets the track that is currently playing.
    /// </summary>
    public LavalinkTrack? CurrentTrack { get; private set; }

    public PlayerFilterMap Filters => _filterMap ??= new PlayerFilterMap(this);

    internal bool HasFilters => _filterMap != null;

    /// <summary>
    ///     Gets the identifier snowflake value of the guild the player is for.
    /// </summary>
    public ulong GuildId { get; private set; }

    /// <summary>
    ///     Gets the current player state.
    /// </summary>
    public PlayerState State { get; private set; } = PlayerState.NotConnected;

    /// <summary>
    ///     Gets the current track position.
    /// </summary>
    /// <value>the current track position.</value>
    public TrackPosition Position { get; private set; }

    /// <summary>
    ///     Gets the current track position.
    /// </summary>
    [Obsolete("This property will be removed in a future version, please use the Position property instead.")]
    public TimeSpan TrackPosition => Position.Position;

    /// <summary>
    ///     Gets the voice channel id the player is connected to.
    /// </summary>
    public ulong? VoiceChannelId { get; private set; }

    /// <summary>
    ///     Gets the current player volume.
    /// </summary>
    public float Volume { get; private set; } = 1f;

    /// <summary>
    ///     Gets or sets the reason why the player disconnected.
    /// </summary>
    internal PlayerDisconnectCause DisconnectCause { get; set; }

    /// <summary>
    ///     Gets the communication lavalink socket.
    /// </summary>
    public LavalinkSocket LavalinkSocket { get; internal set; } = null!; // Lazy-initialized

    internal VoiceServer? VoiceServer { get; set; }

    internal VoiceState? VoiceState { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the player should stop after the track
    ///     finished playing.
    /// </summary>
    protected bool DisconnectOnStop { get; set; }

    /// <summary>
    ///     Joins the voice channel specified by <paramref name="voiceChannelId"/> asynchronously.
    /// </summary>
    /// <param name="voiceChannelId">the voice channel identifier to join</param>
    /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
    /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual async Task ConnectAsync(ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false)
    {
        await Client
            .SendVoiceUpdateAsync(GuildId, voiceChannelId, selfDeaf, selfMute)
            .ConfigureAwait(false);

        VoiceChannelId = voiceChannelId;

        State = PlayerState.NotPlaying;
        CurrentTrack = null;
    }

    /// <summary>
    ///     Destroys the player asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    public async Task DestroyAsync()
    {
        EnsureNotDestroyed();

        // destroy player
        State = PlayerState.Destroyed;

        // send destroy payload
        var destroyPayload = new PlayerDestroyPayload { GuildId = GuildId, };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerDestroy, destroyPayload)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Disconnects the player asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    public virtual Task DisconnectAsync()
    {
        EnsureConnected();
        return DisconnectAsync(PlayerDisconnectCause.Stop);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (State is PlayerState.Destroyed || !disposing)
        {
            return;
        }

        // Disconnect from voice channel and send destroy player payload to the lavalink node
        DisconnectAsync(PlayerDisconnectCause.Dispose).Wait();
        DestroyAsync().Wait();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (State is PlayerState.Destroyed)
        {
            return;
        }

        // Disconnect from voice channel and send destroy player payload to the lavalink node
        await DisconnectAsync(PlayerDisconnectCause.Dispose).ConfigureAwait(false);
        await DestroyAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously triggered when the player has connected to a voice channel.
    /// </summary>
    /// <param name="voiceServer">the voice server connected to</param>
    /// <param name="voiceState">the voice state</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual Task OnConnectedAsync(VoiceServer voiceServer, VoiceState voiceState)
        => Task.CompletedTask;

    /// <summary>
    ///     Asynchronously triggered when the player has changed the node.
    /// </summary>
    /// <param name="eventArgs">the socket change event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual Task OnSocketChanged(SocketChangedEventArgs eventArgs) 
        => Task.CompletedTask;

    /// <summary>
    ///     Asynchronously triggered when a track ends.
    /// </summary>
    /// <remarks>
    ///     When overriding this method without supering / base calling it, the disconnect on
    ///     stop function will be prevent.
    /// </remarks>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
    {
        if (DisconnectOnStop)
        {
            return DisconnectAsync();
        }
        else
        {
            // The track ended, set to not playing
            State = PlayerState.NotPlaying;
            CurrentTrack = null;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously triggered when an exception occurred while playing a track.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual Task OnTrackExceptionAsync(TrackExceptionEventArgs eventArgs)
        => Task.CompletedTask;

    /// <summary>
    ///     Asynchronously triggered when a track started.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual Task OnTrackStartedAsync(TrackStartedEventArgs eventArgs)
        => Task.CompletedTask;

    /// <summary>
    ///     Asynchronously triggered when a track got stuck.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public virtual Task OnTrackStuckAsync(TrackStuckEventArgs eventArgs)
        => Task.CompletedTask;

    /// <summary>
    ///     Pauses the current playing track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the current playing track is already paused.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task PauseAsync()
    {
        EnsureNotDestroyed();
        EnsureConnected();

        if (State != PlayerState.Playing)
        {
            throw new InvalidOperationException("The current playing track is not playing.");
        }

        var pausePayload = new PlayerPausePayload { GuildId = GuildId, Pause = true, };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerPause, pausePayload)
            .ConfigureAwait(false);

        State = PlayerState.Paused;
    }

    /// <summary>
    ///     Plays the specified <paramref name="track"/> asynchronously.
    /// </summary>
    /// <param name="track">the track to play</param>
    /// <param name="startTime">the track start position</param>
    /// <param name="endTime">the track end position</param>
    /// <param name="noReplace">
    ///     a value indicating whether the track play should be ignored if the same track is
    ///     currently playing
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task PlayAsync(
        LavalinkTrack track,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        bool noReplace = false)
    {
        EnsureNotDestroyed();
        EnsureConnected();

        CurrentTrack = track ?? throw new ArgumentNullException(nameof(track));

        if (startTime is null && track.Position.Ticks is not 0)
        {
            startTime = track.Position;
        }

        var playPayload = new PlayerPlayPayload
        {
            GuildId = GuildId,
            TrackIdentifier = track.Identifier,
            StartTime = startTime,
            EndTime = endTime,
            NoReplace = noReplace,
        };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerPlay, playPayload)
            .ConfigureAwait(false);

        State = PlayerState.Playing;
    }

    /// <summary>
    ///     Replays the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    public virtual Task ReplayAsync()
    {
        EnsureNotDestroyed();
        EnsureConnected();

        if (CurrentTrack is null || State == PlayerState.NotPlaying)
        {
            throw new InvalidOperationException("No track is playing.");
        }

        return PlayAsync(CurrentTrack, startTime: TimeSpan.Zero);
    }

    /// <summary>
    ///     Resumes the current playing track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the current playing track is not paused
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task ResumeAsync()
    {
        EnsureNotDestroyed();
        EnsureConnected();

        if (State != PlayerState.Paused)
        {
            throw new InvalidOperationException("There is no track paused.");
        }

        var pausePayload = new PlayerPausePayload { GuildId = GuildId, Pause = false, };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerPause, pausePayload)
            .ConfigureAwait(false);

        State = PlayerState.Playing;
    }

    /// <summary>
    ///     Seeks the current playing track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///     thrown if the current playing track does not support seeking.
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task SeekPositionAsync(TimeSpan position)
    {
        EnsureNotDestroyed();
        EnsureConnected();

        if (State is not PlayerState.Paused and not PlayerState.Playing)
        {
            throw new InvalidOperationException("There is no track paused or playing.");
        }

        var seekPayload = new PlayerSeekPayload { GuildId = GuildId, Position = position, };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerSeek, seekPayload)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Updates the player volume asynchronously.
    /// </summary>
    /// <param name="volume">the player volume (0f - 10f)</param>
    /// <param name="normalize">
    ///     a value indicating whether if the <paramref name="volume"/> is out of range (0f -
    ///     10f) it should be normalized in its range. For example 11f will be mapped to 10f and
    ///     -20f to 0f.
    /// </param>
    /// <param name="force">
    ///     a value indicating whether to send the update regardless of whether the current
    ///     volume is the same as the specified <paramref name="volume"/>.
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="volume"/> is out of range (0f - 10f)
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task SetVolumeAsync(float volume = 1f, bool normalize = false, bool force = false)
    {
        EnsureNotDestroyed();
        EnsureConnected();

        if (volume is > 10f or < 0f)
        {
            if (!normalize)
            {
                throw new ArgumentOutOfRangeException(nameof(volume), volume, "Volume is out of range (0f - 10f)");
            }

            // bring the values into range (0f - 10f)
            volume = Math.Max(0f, volume);
            volume = Math.Min(10f, volume);
        }

        // check if the volume is already the same as wanted
        if (!force && Volume == volume)
        {
            return;
        }

        var volumePayload = new PlayerVolumePayload
        {
            GuildId = GuildId,
            Volume = (int)Math.Round(volume * 100F),
        };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerVolume, volumePayload)
            .ConfigureAwait(false);

        Volume = volume;
    }

    /// <summary>
    ///     Stops playing the current track asynchronously.
    /// </summary>
    /// <param name="disconnect">
    ///     a value indicating whether the connection to the voice server should be closed
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task StopAsync(bool disconnect = false)
    {
        EnsureNotDestroyed();
        EnsureConnected();

        var stopPayload = new PlayerStopPayload { GuildId = GuildId, };

        await LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerStop, stopPayload)
            .ConfigureAwait(false);

        if (disconnect)
        {
            await DisconnectAsync(PlayerDisconnectCause.Stop).ConfigureAwait(false);
        }

        State = PlayerState.NotPlaying;
        CurrentTrack = null;
    }

    /// <summary>
    ///     Updates the player equalizer asynchronously.
    /// </summary>
    /// <param name="bands">the bands</param>
    /// <param name="reset">
    ///     a value indicating whether the equalizer bands should be overridden ( <see
    ///     langword="false"/>) or replaced ( <see langword="true"/>).
    /// </param>
    /// <param name="force">
    ///     a value indicating whether to send the update regardless of whether the current
    ///     equalizer bands ( <see cref="Bands"/>) are configured the same as the specified
    ///     <paramref name="bands"/>.
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    [Obsolete("This member may be removed in a future version, use Filters.Equalizer instead.")]
    public virtual Task UpdateEqualizerAsync(IEnumerable<EqualizerBand> bands, bool reset = true, bool force = false)
    {
        EnsureNotDestroyed();

        Filters.Equalizer ??= new EqualizerFilterOptions();
        Filters.Equalizer.Bands = bands.ToArray();

        return Filters.CommitAsync();
    }

    /// <summary>
    ///     Initializes a new instance of a player.
    /// </summary>
    /// <param name="playerFactory">the player factory</param>
    /// <param name="socket">the lavalink socket</param>
    /// <param name="client">the discord client</param>
    /// <param name="guildId">the identifier of the guild that is controlled by the player</param>
    /// <param name="disconnectOnStop">
    ///     a value indicating whether the player should stop after the track finished playing
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="playerFactory"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
    /// </exception>
    internal static T CreatePlayer<T>(
        PlayerFactory<T> playerFactory, LavalinkSocket socket, IDiscordClientWrapper client,
        ulong guildId, bool disconnectOnStop) where T : LavalinkPlayer
    {
        if (playerFactory is null)
        {
            throw new ArgumentNullException(nameof(playerFactory));
        }

        var player = playerFactory();

        player.LavalinkSocket = socket ?? throw new ArgumentNullException(nameof(socket));
        player.Client = client ?? throw new ArgumentNullException(nameof(client));
        player.GuildId = guildId;
        player.DisconnectOnStop = disconnectOnStop;

        return player;
    }

    /// <summary>
    ///     Disconnects the player asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    internal async Task DisconnectAsync(PlayerDisconnectCause disconnectCause)
    {
        if (State == PlayerState.NotConnected)
        {
            return;
        }

        // keep old channel in memory
        var channel = VoiceChannelId;

        // disconnect from channel
        await Client.SendVoiceUpdateAsync(GuildId, null);
        VoiceChannelId = null;
        State = PlayerState.NotConnected;

        // only trigger event if disconnected from a channel
        if (channel.HasValue)
        {
            // notify disconnect
            await LavalinkSocket.NotifyDisconnectAsync(
                new PlayerDisconnectedEventArgs(this, channel.Value, disconnectCause));
        }
    }

    /// <summary>
    ///     Updates the voice server and sends the data to the Lavalink Node if the voice state
    ///     is also provided.
    /// </summary>
    /// <param name="voiceServer">the voice server data</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    internal Task UpdateAsync(VoiceServer voiceServer)
    {
        EnsureNotDestroyed();

        VoiceServer = voiceServer;
        return UpdateAsync();
    }

    /// <summary>
    ///     Updates the voice state and sends the data to the Lavalink Node if the voice server
    ///     is also provided.
    /// </summary>
    /// <param name="voiceState">the voice state data</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    internal Task UpdateAsync(VoiceState voiceState)
    {
        EnsureNotDestroyed();

        VoiceState = voiceState;
        VoiceChannelId = voiceState.VoiceChannelId;
        return UpdateAsync();
    }

    /// <summary>
    ///     Sends the voice state and server data to the Lavalink Node if both is provided.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    internal async Task UpdateAsync()
    {
        if (VoiceServer is null || VoiceState is null)
        {
            // voice state or server is missing
            return;
        }

        var voiceServerUpdateEvent = new VoiceServerUpdateEvent(VoiceServer);

        var voiceUpdatePayload = new VoiceUpdatePayload
        {
            GuildId = GuildId,
            SessionId = VoiceState.VoiceSessionId,
            VoiceServerUpdateEvent = voiceServerUpdateEvent,
        };

        // send voice update payload
        await LavalinkSocket
            .SendPayloadAsync(OpCode.GuildVoiceUpdate, voiceUpdatePayload)
            .ConfigureAwait(false);

        if (State is PlayerState.NotConnected or PlayerState.Destroyed)
        {
            // set initial player state to connected, if player was not connected or destroyed,
            // see: https://github.com/angelobreuer/Lavalink4NET/issues/28
            State = PlayerState.NotPlaying;
            CurrentTrack = null;
        }

        // trigger event
        await OnConnectedAsync(VoiceServer, VoiceState).ConfigureAwait(false);
    }

    internal void UpdateTrackPosition(DateTimeOffset positionUpdateTime, TimeSpan position)
    {
        var timeStretchFactor = 1F;
        var timescaleFilter = _filterMap?.Timescale;

        if (timescaleFilter is not null)
        {
            timeStretchFactor *= timescaleFilter.Rate * timescaleFilter.Speed;
        }

        Position = new TrackPosition(positionUpdateTime, position, timeStretchFactor);
    }

    /// <summary>
    ///     Throws an <see cref="InvalidOperationException"/> when the player is not connected
    ///     to a voice channel.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the player is not connected to a voice channel
    /// </exception>
    protected void EnsureConnected()
    {
        if (State == PlayerState.NotConnected)
        {
            throw new InvalidOperationException("The player is not connected to a voice channel.");
        }
    }

    /// <summary>
    ///     Throws an <see cref="InvalidOperationException"/> when the player is destroyed.
    /// </summary>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    protected void EnsureNotDestroyed()
    {
        if (State == PlayerState.Destroyed)
        {
            throw new InvalidOperationException("The player is destroyed.");
        }
    }
}
