/*
 *  File:   DiscordClientWrapperBase.cs
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

namespace Lavalink4NET.DSharpPlus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using global::DSharpPlus;
using global::DSharpPlus.Entities;
using global::DSharpPlus.EventArgs;
using global::DSharpPlus.Exceptions;
using Lavalink4NET.Events;

/// <summary>
///     An abstraction used to implement a wrapper for the discord client from the "DSharpPlus"
///     discord client library. (https://github.com/DSharpPlus/DSharpPlus)
/// </summary>
public abstract class DiscordClientWrapperBase : IDiscordClientWrapper
{
    /// <inheritdoc/>
    public event AsyncEventHandler<VoiceServer>? VoiceServerUpdated;

    /// <inheritdoc/>
    public event AsyncEventHandler<Events.VoiceStateUpdateEventArgs>? VoiceStateUpdated;

    /// <inheritdoc/>
    public ulong CurrentUserId
    {
        get
        {
            if (CurrentUser is null)
            {
                throw new InvalidOperationException("Current user not available.");
            }

            return CurrentUser.Id;
        }
    }

    /// <inheritdoc/>
    public abstract int ShardCount { get; }

    /// <summary>
    ///     Gets the current user.
    /// </summary>
    /// <value>the current user.</value>
    protected abstract DiscordUser? CurrentUser { get; }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task<IEnumerable<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId)
    {
        DiscordChannel channel;
        try
        {
            channel = await GetClient(guildId)
                .GetChannelAsync(voiceChannelId)
                .ConfigureAwait(false);
        }
        catch (UnauthorizedException)
        {
            // The channel was possibly deleted
            return Enumerable.Empty<ulong>();
        }

        if (channel is null)
        {
            return Enumerable.Empty<ulong>();
        }

        return channel.Users.Select(s => s.Id);
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task InitializeAsync()
    {
        var startTime = DateTimeOffset.UtcNow;

        // await until client is ready
        while (CurrentUser is null)
        {
            await Task.Delay(10).ConfigureAwait(false);

            // timeout exceeded
            if (DateTimeOffset.UtcNow - startTime > TimeSpan.FromSeconds(10))
            {
                throw new TimeoutException("Waited 10 seconds for current user to arrive! Make sure you start " +
                    "the discord client, before initializing the discord wrapper!");
            }
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false)
    {
        var payload = new JsonObject();
        var data = new VoiceStateUpdatePayload(guildId, voiceChannelId, selfMute, selfDeaf);

        payload.Add("op", 4);
        payload.Add("d", JsonSerializer.SerializeToNode(data));

        await GetClient(guildId).GetWebSocketClient().SendMessageAsync(payload.ToString()).ConfigureAwait(false);
    }

    /// <summary>
    ///     Gets the client that serves the guild specified by <paramref name="guildId"/>.
    /// </summary>
    /// <param name="guildId">the snowflake identifier of the guild.</param>
    /// <returns>the client that serves the guild specified by <paramref name="guildId"/>.</returns>
    protected abstract DiscordClient GetClient(ulong guildId);

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    protected Task OnVoiceServerUpdated(DiscordClient _, VoiceServerUpdateEventArgs voiceServer)
    {
        var args = new VoiceServer(voiceServer.Guild.Id, voiceServer.GetVoiceToken(), voiceServer.Endpoint);
        return VoiceServerUpdated.InvokeAsync(this, args);
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    protected Task OnVoiceStateUpdated(DiscordClient _, global::DSharpPlus.EventArgs.VoiceStateUpdateEventArgs eventArgs)
    {
        // session id is the same as the resume key so DSharpPlus should be able to give us the
        // session key in either before or after voice state
        var sessionId = eventArgs.Before?.GetSessionId() ?? eventArgs.After.GetSessionId();

        // create voice state
        var voiceState = new VoiceState(
            voiceChannelId: eventArgs.After?.Channel?.Id,
            guildId: eventArgs.Guild.Id,
            voiceSessionId: sessionId);

        // invoke event
        return VoiceStateUpdated.InvokeAsync(this,
            new Events.VoiceStateUpdateEventArgs(eventArgs.User.Id, voiceState));
    }
}
