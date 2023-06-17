namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;

public sealed record class InactivityTrackingContext(
    IInactivityTrackingService InactivityTrackingService,
    IDiscordClientWrapper Client,
    ILavalinkPlayer Player);