/*
 *  File:   InactivityTrackingService.cs
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

namespace Lavalink4NET.Tracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Lavalink4NET.Events;
    using Lavalink4NET.Player;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     A service that tracks not-playing players to reduce the usage of the Lavalink nodes.
    /// </summary>
    public class InactivityTrackingService : IDisposable
    {
        private readonly IAudioService _audioService;
        private readonly IDiscordClientWrapper _clientWrapper;
        private readonly ILogger<InactivityTrackingService> _logger;
        private readonly InactivityTrackingOptions _options;
        private readonly IDictionary<ulong, DateTimeOffset> _players;
        private readonly IList<InactivityTracker> _trackers;
        private readonly object _trackersLock;
        private Timer _timer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InactivityTrackingService"/> class.
        /// </summary>
        /// <param name="audioService">the audio service where the players should be tracked</param>
        /// <param name="clientWrapper">the discord client wrapper</param>
        /// <param name="options">the tracking options</param>
        /// <param name="logger">the optional logger</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="clientWrapper"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public InactivityTrackingService(IAudioService audioService, IDiscordClientWrapper clientWrapper,
            InactivityTrackingOptions options, ILogger<InactivityTrackingService> logger)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            _clientWrapper = clientWrapper ?? throw new ArgumentNullException(nameof(clientWrapper));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _players = new Dictionary<ulong, DateTimeOffset>();

            _trackers = new List<InactivityTracker>();
            _trackersLock = new object();

            // add default trackers
            _trackers.Add(DefaultInactivityTrackers.UsersInactivityTracker);
            _trackers.Add(DefaultInactivityTrackers.ChannelInactivityTracker);

            if (options.TrackInactivity)
            {
                BeginTracking();
            }
        }

        /// <summary>
        ///     An asynchronously event that is triggered when an inactive player was found.
        /// </summary>
        public event AsyncEventHandler<InactivePlayerEventArgs> InactivePlayer;

        /// <summary>
        ///     Gets a value indicating whether the service is tracking inactive players.
        /// </summary>
        public bool IsTracking => _timer != null;

        /// <summary>
        ///     Gets all trackers.
        /// </summary>
        public IReadOnlyList<InactivityTracker> Trackers
        {
            get
            {
                lock (_trackersLock)
                {
                    return _trackers.ToList().AsReadOnly();
                }
            }
        }

        /// <summary>
        ///     Adds a tracker to the track list dynamically.
        /// </summary>
        /// <param name="tracker">the tracker to add</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="tracker"/> is <see langword="null"/>.
        /// </exception>
        public void AddTracker(InactivityTracker tracker)
        {
            if (tracker is null)
            {
                throw new ArgumentNullException(nameof(tracker));
            }

            lock (_trackersLock)
            {
                _trackers.Add(tracker);
            }
        }

        /// <summary>
        ///     Beings tracking of inactive players.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service is already tracking inactive players.
        /// </exception>
        public void BeginTracking()
        {
            if (_timer != null)
            {
                throw new InvalidOperationException("Already tracking.");
            }

            // initialize the timer that polls inactive players
            _timer = new Timer(_ => _ = PollAsync(), null,
               _options.DelayFirstTrack ? _options.PollInterval : TimeSpan.Zero, _options.PollInterval);
        }

        /// <summary>
        ///     Disposes the underlying timer.
        /// </summary>
        public void Dispose() => _timer?.Dispose();

        /// <summary>
        ///     Gets the tracking status of the specified <paramref name="player"/>.
        /// </summary>
        /// <param name="player">the player</param>
        /// <returns>the inactivity tracking status of the player</returns>
        public InactivityTrackingStatus GetStatus(LavalinkPlayer player)
        {
            if (!_players.TryGetValue(player.GuildId, out var dateTimeOffset))
            {
                // there are no tracking entries for the player
                return InactivityTrackingStatus.Untracked;
            }

            // the player has exceeded the stop delay
            if (DateTimeOffset.UtcNow > dateTimeOffset)
            {
                return InactivityTrackingStatus.Inactive;
            }

            //player is tracked for inactivity, but not removed
            return InactivityTrackingStatus.Tracked;
        }

        /// <summary>
        ///     Force polls tracking of all inactive players asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public virtual async Task PollAsync()
        {
            // get all created player instances in the audio service
            var players = _audioService.GetPlayers<LavalinkPlayer>();

            // iterate through players that are connected to a voice channel
            foreach (var player in players.Where(s => s.VoiceChannelId.HasValue))
            {
                // check if the player is inactive
                if (await IsInactiveAsync(player))
                {
                    // add the player to tracking list
                    if (_players.TryAdd(player.GuildId, DateTimeOffset.UtcNow + _options.DisconnectDelay))
                    {
                        _logger.LogTrace("Tracked player {PlayerId} as inactive.", player.GuildId);
                    }
                }
                else
                {
                    // the player is active again, remove from tracking list
                    if (_players.Remove(player.GuildId))
                    {
                        _logger.LogTrace("Removed player {PlayerId} from tracking list.", player.GuildId);
                    }
                }
            }

            // remove all inactive, tracked players where the disconnect delay was exceeded
            foreach (var player in _players.ToArray())
            {
                // check if player is inactive and the delay was exceeded
                if (player.Value < DateTimeOffset.UtcNow)
                {
                    var trackedPlayer = _audioService.GetPlayer<LavalinkPlayer>(player.Key);

                    // player does not exists, remove it from the tracking list and continue.
                    if (trackedPlayer == null)
                    {
                        _players.Remove(player.Key);
                        continue;
                    }

                    var eventArgs = new InactivePlayerEventArgs(_audioService, trackedPlayer);
                    await OnInactivePlayerAsync(eventArgs);

                    // it is wanted that the player should not stop.
                    if (!eventArgs.ShouldStop)
                    {
                        continue;
                    }

                    _logger.LogTrace("Destroyed player {GuildId} due inactivity.", player.Key);

                    // dispose the player
                    trackedPlayer.Dispose();
                }
            }
        }

        /// <summary>
        ///     Removes a tracker from the tracker list dynamically.
        /// </summary>
        /// <param name="tracker">the tracker to remove</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="tracker"/> is <see langword="null"/>.
        /// </exception>
        public void RemoveTracker(InactivityTracker tracker)
        {
            if (tracker is null)
            {
                throw new ArgumentNullException(nameof(tracker));
            }

            lock (_trackersLock)
            {
                _trackers.Remove(tracker);
            }
        }

        /// <summary>
        ///     Stops tracking of inactive players.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service is not tracking inactive players.
        /// </exception>
        public void StopTracking()
        {
            if (_timer == null)
            {
                throw new InvalidOperationException("Not tracking.");
            }

            _timer.Dispose();
            _timer = null;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified <paramref name="player"/> is inactive asynchronously.
        /// </summary>
        /// <param name="player">the player to check</param>
        /// <returns>
        ///     a task that represents the asynchronous operation. The task result is a value
        ///     indicating whether the specified <paramref name="player"/> is inactive.
        /// </returns>
        protected virtual async Task<bool> IsInactiveAsync(LavalinkPlayer player)
        {
            // iterate through the trackers
            foreach (var tracker in Trackers)
            {
                // check if the player is inactivity
                if (await tracker(player, _clientWrapper))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Triggers the <see cref="InactivePlayer"/> event asynchronously.
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        protected virtual Task OnInactivePlayerAsync(InactivePlayerEventArgs eventArgs)
            => InactivePlayer.InvokeAsync(this, eventArgs);
    }
}