namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.Clients;

public sealed record class InactivityTrackingContext(
    IInactivityTrackingService InactivityTrackingService,
    IDiscordClientWrapper Client);