namespace Lavalink4NET.Players;

using System;
using Lavalink4NET.Clients;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Internal;

internal sealed record class PlayerContext(
    IServiceProvider? ServiceProvider,
    ILavalinkApiClient ApiClient,
    ILavalinkSessionProvider SessionProvider,
    IDiscordClientWrapper DiscordClient,
    ISystemClock SystemClock);
