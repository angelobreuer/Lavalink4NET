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
        private readonly InactivityTrackingOptions _options;
        private readonly ILogger<InactivityTrackingService> _logger;
        private readonly IDictionary<ulong, DateTimeOffset> _players;
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

            // track inactive players by users in voice channel
            if (_options.TrackingMode.HasFlag(InactivityTrackingMode.User))
            {
                // iterate through players that are connected to a voice channel
                foreach (var player in players.Where(s => s.VoiceChannelId.HasValue))
                {
                    // check if the player is inactive
                    if (await IsInactiveAsync(player))
                    {
                        // add the player to tracking list
                        if (_players.TryAdd(player.GuildId, DateTimeOffset.UtcNow + _options.DisconnectDelay))
                        {
                            _logger.LogTrace("Added logger to tracking list: {GuildId}, removing in {Time}.", player.GuildId, _options.DisconnectDelay);
                        }
                    }
                    else
                    {
                        // the player is active again, remove from tracking list
                        if (_players.Remove(player.GuildId))
                        {
                            _logger.LogTrace("Player got active again: {GuildId}, removed from tracking list.", player.GuildId);
                        }
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
                        _logger.LogTrace("Inactivity disposing was stopped by event for player: {PlayerId}.", player.Key);
                        continue;
                    }

                    _logger.LogTrace("Disposing player {GuildId} due to inactivity.", player.Key);

                    // dispose the player
                    trackedPlayer.Dispose();
                }
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
            // check if user-tracking is enabled
            if (_options.TrackingMode.HasFlag(InactivityTrackingMode.User))
            {
                // count the users in the player voice channel (bot excluded)
                var userCount = (await _clientWrapper.GetChannelUsersAsync(player.GuildId, player.VoiceChannelId.Value))
                    .Where(s => s != _clientWrapper.CurrentUserId)
                    .Count();

                // check if there are no users in the channel (bot excluded)
                if (userCount == 0)
                {
                    return true;
                }
            }

            // check if track-tracking is enabled
            if (_options.TrackingMode.HasFlag(InactivityTrackingMode.Track))
            {
                // check if no track is playing
                if (player.State != PlayerState.Playing)
                {
                    return true;
                }
            }

            // the player is active
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