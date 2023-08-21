namespace Lavalink4NET.InactivityTracking.Trackers.Users;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

public sealed class UsersInactivityTracker : IInactivityTracker
{
	private readonly UsersInactivityTrackerOptions? _options;
	private readonly string? _label;
	private readonly IPlayerManager _playerManager;
	private readonly IDiscordClientWrapper _discordClient;

	public UsersInactivityTracker(
		IDiscordClientWrapper discordClient,
		IPlayerManager playerManager,
		IOptions<UsersInactivityTrackerOptions>? options = null)
	{
		_label = options?.Value.Label ?? "Users Inactivity Tracker";
		_options = options?.Value;

		_playerManager = playerManager;
		_discordClient = discordClient;
	}

	public InactivityTrackerOptions Options => InactivityTrackerOptions.Realtime(_label);

	public ValueTask RunAsync(IInactivityTrackerContext trackerContext, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ArgumentNullException.ThrowIfNull(trackerContext);

		var context = new UsersInactivityTrackerContext(
			discordClient: _discordClient,
			trackerContext: trackerContext,
			playerManager: _playerManager,
			options: _options);

		return context.RunAsync(cancellationToken);
	}
}

file sealed class UsersInactivityTrackerContext
{
	private readonly IDiscordClientWrapper _discordClient;
	private readonly IInactivityTrackerContext _trackerContext;
	private readonly IPlayerManager _playerManager;
	private readonly bool _excludeBots;
	private readonly int _threshold;
	private readonly TimeSpan? _timeout;

	public UsersInactivityTrackerContext(
		IDiscordClientWrapper discordClient,
		IInactivityTrackerContext trackerContext,
		IPlayerManager playerManager,
		UsersInactivityTrackerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(discordClient);
		ArgumentNullException.ThrowIfNull(trackerContext);
		ArgumentNullException.ThrowIfNull(playerManager);

		_discordClient = discordClient;
		_trackerContext = trackerContext;
		_playerManager = playerManager;

		_excludeBots = options?.ExcludeBots ?? true;
		_threshold = options?.Threshold ?? 1;
		_timeout = options?.Timeout;
	}

	private async Task VoiceStateUpdated(object sender, VoiceStateUpdatedEventArgs eventArgs)
	{
		ArgumentNullException.ThrowIfNull(sender);
		ArgumentNullException.ThrowIfNull(eventArgs);

		if (!_playerManager.HasPlayer(eventArgs.GuildId))
		{
			return; // ignore, no player allocated for this guild
		}

		var scope = default(InactivityTrackerScope?);
		try
		{
			if (eventArgs.OldVoiceState.VoiceChannelId is not null)
			{
				scope ??= _trackerContext.CreateScope();

				await UpdateChannelAsync(
					scope: scope,
					guildId: eventArgs.GuildId,
					voiceChannelId: eventArgs.OldVoiceState.VoiceChannelId.Value)
					.ConfigureAwait(false);
			}

			if (eventArgs.VoiceState.VoiceChannelId is not null)
			{
				scope ??= _trackerContext.CreateScope();

				await UpdateChannelAsync(
					scope: scope,
					guildId: eventArgs.GuildId,
					voiceChannelId: eventArgs.VoiceState.VoiceChannelId.Value)
					.ConfigureAwait(false);
			}
		}
		finally
		{
			scope?.Dispose();
		}
	}

	private async ValueTask UpdateChannelAsync(InactivityTrackerScope scope, ulong guildId, ulong voiceChannelId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var users = await _discordClient
			.GetChannelUsersAsync(guildId, voiceChannelId, !_excludeBots, cancellationToken)
			.ConfigureAwait(false);

		if (users.Length >= _threshold)
		{
			scope.MarkActive(guildId);
		}
		else
		{
			scope.MarkInactive(guildId, _timeout);
		}
	}

	public async ValueTask RunAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_discordClient.VoiceStateUpdated += VoiceStateUpdated;

		try
		{
			var taskCompletionSource = new TaskCompletionSource<object?>(
				creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

			using var cancellationTokenRegistration = cancellationToken.Register(
				state: taskCompletionSource,
				callback: taskCompletionSource.SetResult);

			await taskCompletionSource.Task.ConfigureAwait(false);
		}
		finally
		{
			_discordClient.VoiceStateUpdated -= VoiceStateUpdated;
		}
	}
}