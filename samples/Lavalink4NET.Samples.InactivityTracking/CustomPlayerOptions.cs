namespace Lavalink4NET.Samples.InactivityTracking;

using Discord;
using Lavalink4NET.Players.Queued;

public sealed record class CustomPlayerOptions(ITextChannel? TextChannel) : QueuedLavalinkPlayerOptions;
