namespace Lavalink4NET.Players;

using System;
using Lavalink4NET.Clients;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public interface IPlayerProperties<out TPlayer, out TOptions>
    where TOptions : LavalinkPlayerOptions
    where TPlayer : ILavalinkPlayer
{
    public ILavalinkApiClient ApiClient { get; }

    public IDiscordClientWrapper DiscordClient { get; }

    public PlayerInformationModel InitialState { get; }

    public string Label { get; }

    public ILogger<TPlayer> Logger { get; }

    public IOptions<TOptions> Options { get; }

    public IServiceProvider? ServiceProvider { get; }

    public string SessionId { get; }

    public ulong VoiceChannelId { get; }
}
