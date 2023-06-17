namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.Clients;
using Lavalink4NET.Players;

public sealed record class InactivityTrackingContext(
    IInactivityTrackingService InactivityTrackingService,
    IDiscordClientWrapper Client,
    ILavalinkPlayer Player);