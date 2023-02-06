namespace Lavalink4NET.Players;

using System;
using Lavalink4NET.Clients;
using Lavalink4NET.Rest;

internal sealed record class PlayerContext(
    IServiceProvider? ServiceProvider,
    ILavalinkApiClient ApiClient,
    IDiscordClientWrapper DiscordClient,
    string SessionId);
