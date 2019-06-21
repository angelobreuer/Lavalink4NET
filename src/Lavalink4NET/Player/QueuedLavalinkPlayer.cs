/*
 *  File:   QueuedLavalinkPlayer.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2019
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

namespace Lavalink4NET.Player
{
    using System;
    using System.Threading.Tasks;
    using Lavalink4NET.Events;

    /// <summary>
    ///     A lavalink player with a queuing system.
    /// </summary>
    public class QueuedLavalinkPlayer : LavalinkPlayer
    {
        private readonly bool _disconnectOnStop;

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueuedLavalinkPlayer"/> class.
        /// </summary>
        /// <param name="lavalinkSocket">the lavalink socket</param>
        /// <param name="client">the discord client</param>
        /// <param name="guildId">the identifier of the guild that is controlled by the player</param>
        /// <param name="disconnectOnStop">
        ///     a value indicating whether the player should stop after the track finished playing
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="lavalinkSocket"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        public QueuedLavalinkPlayer(LavalinkSocket lavalinkSocket, IDiscordClientWrapper client, ulong guildId, bool disconnectOnStop)
            : base(lavalinkSocket, client, guildId, false /* this player handles this on itself */)
        {
            Queue = new LavalinkQueue();
            _disconnectOnStop = disconnectOnStop;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the current playing track should be looped.
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        ///     Gets the track queue.
        /// </summary>
        public LavalinkQueue Queue { get; }

        /// <summary>
        ///     Asynchronously triggered when a track ends.
        /// </summary>
        /// <param name="eventArgs">the track event arguments</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        public override async Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        {
            if (eventArgs.MayStartNext)
            {
                await SkipAsync();
            }

            await base.OnTrackEndAsync(eventArgs);
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
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the position in the track queue ( <c>0</c> = now playing)</para>
        /// </returns>
        public new virtual Task<int> PlayAsync(LavalinkTrack track, TimeSpan? startTime = null,
            TimeSpan? endTime = null, bool noReplace = false)
            => PlayAsync(track, true, startTime, endTime, noReplace);

        /// <summary>
        ///     Plays the specified <paramref name="track"/> asynchronously.
        /// </summary>
        /// <param name="track">the track to play</param>
        /// <param name="enqueue">
        ///     a value indicating whether the track should be enqueued in the track queue
        /// </param>
        /// <param name="startTime">the track start position</param>
        /// <param name="endTime">the track end position</param>
        /// <param name="noReplace">
        ///     a value indicating whether the track play should be ignored if the same track is
        ///     currently playing
        /// </param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the position in the track queue ( <c>0</c> = now playing)</para>
        /// </returns>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public virtual async Task<int> PlayAsync(LavalinkTrack track, bool enqueue,
            TimeSpan? startTime = null, TimeSpan? endTime = null, bool noReplace = false)
        {
            EnsureNotDestroyed();
            EnsureConnected();

            if (enqueue && State == PlayerState.Playing)
            {
                Queue.Add(track);

                if (State == PlayerState.NotPlaying)
                {
                    await SkipAsync();
                }

                return Queue.Count;
            }

            await base.PlayAsync(track, startTime, endTime, noReplace);
            return 0;
        }

        /// <summary>
        ///     Plays the specified <paramref name="track"/> at the top of the queue asynchronously.
        /// </summary>
        /// <param name="track">the track to play</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public virtual async Task PlayTopAsync(LavalinkTrack track)
        {
            EnsureNotDestroyed();

            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            // play track if none is playing
            if (State == PlayerState.NotPlaying)
            {
                await PlayAsync(track, enqueue: false);
            }
            // the player is currently playing a track, enqueue the track at top
            else
            {
                Queue.Insert(0, track);
            }
        }

        /// <summary>
        ///     Pushes a track between the current asynchronously.
        /// </summary>
        /// <param name="track">the track to push between the current</param>
        /// <param name="push">
        ///     a value indicating whether the track should only played when a track is playing currently.
        /// </param>
        /// <remarks>
        ///     Note: This feature is experimental. This will stop playing the current track and
        ///     start playing the specified <paramref name="track"/> after the track is finished the
        ///     track will restart at the stopped position. This can be useful for example
        ///     soundboards (playing an air-horn or something).
        /// </remarks>
        /// <returns>
        ///     a task that represents the asynchronous operation. The task result is a value
        ///     indicating whether the track was pushed between the current ( <see langword="true"/>)
        ///     or the specified track was simply started ( <see langword="false"/>), because there
        ///     is no track playing.
        /// </returns>
        public virtual async Task<bool> PushTrackAsync(LavalinkTrack track, bool push = false)
        {
            // star track immediately
            if (State == PlayerState.NotPlaying)
            {
                if (push)
                {
                    return false;
                }

                await PlayAsync(track, enqueue: false);
                return false;
            }

            // create clone and set starting position
            var oldTrack = CurrentTrack.WithPosition(TrackPosition);

            // enqueue old track with starting position
            Queue.Add(oldTrack);

            // push track
            await PlayAsync(track, enqueue: false);
            return true;
        }

        /// <summary>
        ///     Skips the current track asynchronously.
        /// </summary>
        /// <param name="count">the number of tracks to skip</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public virtual Task SkipAsync(int count = 1)
        {
            // no tracks to skip
            if (count <= 0)
            {
                return Task.CompletedTask;
            }

            EnsureNotDestroyed();
            EnsureConnected();

            // the looping option is enabled, repeat current track, does not matter how often we skip
            if (IsLooping && CurrentTrack != null)
            {
                return PlayAsync(CurrentTrack, false);
            }
            // tracks are enqueued
            else if (!Queue.IsEmpty)
            {
                LavalinkTrack track = null;

                while (count-- > 0)
                {
                    // no more tracks in queue
                    if (Queue.Count < 1)
                    {
                        // no tracks found
                        return DisconnectAsync();
                    }

                    // dequeue track
                    track = Queue.Dequeue();
                }

                // a track to play was found, dequeue and play
                return PlayAsync(track, false);
            }
            // no tracks queued, disconnect if wanted
            else if (_disconnectOnStop)
            {
                return DisconnectAsync();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Stops playing the current track asynchronously.
        /// </summary>
        /// <param name="disconnect">
        ///     a value indicating whether the connection to the voice server should be closed
        /// </param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public override Task StopAsync(bool disconnect = false)
        {
            Queue.Clear();
            return base.StopAsync(disconnect);
        }
    }
}