namespace Lavalink4NET.DSharpPlus;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using global::DSharpPlus;
using global::DSharpPlus.Entities;
using global::DSharpPlus.EventArgs;
using global::DSharpPlus.Exceptions;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

public sealed class DiscordClientWrapper : IDiscordClientWrapper, IDisposable
{
    private readonly object _client; // either DiscordShardedClient or DiscordClient
    private readonly TaskCompletionSource<ClientInformation> _readyTaskCompletionSource;
    private bool _disposed;

    public DiscordClientWrapper(DiscordClient discordClient)
    {
        ArgumentNullException.ThrowIfNull(discordClient);

        _client = discordClient;
        _readyTaskCompletionSource = new TaskCompletionSource<ClientInformation>(TaskCreationOptions.RunContinuationsAsynchronously);

        discordClient.VoiceStateUpdated += OnVoiceStateUpdated;
        discordClient.VoiceServerUpdated += OnVoiceServerUpdated;
        discordClient.Ready += OnClientReady;
    }

    public DiscordClientWrapper(DiscordShardedClient discordClient)
    {
        ArgumentNullException.ThrowIfNull(discordClient);

        _client = discordClient;
        _readyTaskCompletionSource = new TaskCompletionSource<ClientInformation>(TaskCreationOptions.RunContinuationsAsynchronously);

        discordClient.VoiceStateUpdated += OnVoiceStateUpdated;
        discordClient.VoiceServerUpdated += OnVoiceServerUpdated;
        discordClient.Ready += OnClientReady;
    }

    /// <inheritdoc/>
    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    /// <inheritdoc/>
    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_client is DiscordClient discordClient)
        {
            discordClient.VoiceStateUpdated -= OnVoiceStateUpdated;
            discordClient.VoiceServerUpdated -= OnVoiceServerUpdated;
            discordClient.Ready -= OnClientReady;
        }
        else
        {
            var shardedClient = Unsafe.As<object, DiscordShardedClient>(ref Unsafe.AsRef(_client));

            shardedClient.VoiceStateUpdated -= OnVoiceStateUpdated;
            shardedClient.VoiceServerUpdated -= OnVoiceServerUpdated;
            shardedClient.Ready -= OnClientReady;
        }

    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DiscordChannel channel;
        try
        {
            channel = await GetClientForGuild(guildId)
                .GetChannelAsync(voiceChannelId)
                .ConfigureAwait(false);
        }
        catch (UnauthorizedException)
        {
            // The channel was possibly deleted
            return ImmutableArray<ulong>.Empty;
        }

        if (channel is null)
        {
            return ImmutableArray<ulong>.Empty;
        }

        return channel.Users
            .Where(x => !x.IsBot)
            .Select(s => s.Id)
            .ToImmutableArray();
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async ValueTask SendVoiceUpdateAsync(
        ulong guildId,
        ulong? voiceChannelId,
        bool selfDeaf = false,
        bool selfMute = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = new JsonObject();
        var data = new VoiceStateUpdatePayload(guildId, voiceChannelId, selfMute, selfDeaf);

        payload.Add("op", 4);
        payload.Add("d", JsonSerializer.SerializeToNode(data));

        await GetClientForGuild(guildId).GetWebSocketClient().SendMessageAsync(payload.ToString()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new(_readyTaskCompletionSource.Task.WaitAsync(cancellationToken));
    }

    private DiscordClient GetClientForGuild(ulong guildId)
    {
        if (_client is DiscordClient discordClient)
        {
            return discordClient;
        }

        return Unsafe.As<object, DiscordShardedClient>(ref Unsafe.AsRef(_client)).GetShard(guildId);
    }

    private Task OnClientReady(DiscordClient discordClient, ReadyEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(eventArgs);

        var clientInformation = new ClientInformation(
            Label: "DSharpPlus",
            CurrentUserId: discordClient.CurrentUser.Id,
            ShardCount: discordClient.ShardCount);

        _readyTaskCompletionSource.TrySetResult(clientInformation);

        return Task.CompletedTask;
    }

    private Task OnVoiceServerUpdated(DiscordClient discordClient, VoiceServerUpdateEventArgs voiceServerUpdateEventArgs)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(voiceServerUpdateEventArgs);

        var server = new VoiceServer(
            Token: voiceServerUpdateEventArgs.VoiceToken,
            Endpoint: voiceServerUpdateEventArgs.Endpoint);

        var eventArgs = new VoiceServerUpdatedEventArgs(
            guildId: voiceServerUpdateEventArgs.Guild.Id,
            voiceServer: server);

        return VoiceServerUpdated.InvokeAsync(this, eventArgs).AsTask();
    }
    private Task OnVoiceStateUpdated(DiscordClient discordClient, VoiceStateUpdateEventArgs voiceStateUpdateEventArgs)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(voiceStateUpdateEventArgs);

        // session id is the same as the resume key so DSharpPlus should be able to give us the
        // session key in either before or after voice state
        var sessionId = voiceStateUpdateEventArgs.Before?.GetSessionId()
            ?? voiceStateUpdateEventArgs.After.GetSessionId();

        // create voice state
        var voiceState = new VoiceState(
            VoiceChannelId: voiceStateUpdateEventArgs.After?.Channel?.Id,
            SessionId: sessionId);

        // invoke event
        var eventArgs = new VoiceStateUpdatedEventArgs(
            guildId: voiceStateUpdateEventArgs.Guild.Id,
            voiceState: voiceState);

        return VoiceStateUpdated.InvokeAsync(this, eventArgs).AsTask();
    }
}
