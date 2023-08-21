namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.Players;

public interface IInactivityTrackerContext
{
    IInactivityTracker InactivityTracker { get; }

    InactivityTrackerScope CreateScope();

    InactivityTrackerEntry? GetEntry(ILavalinkPlayer player);

    InactivityTrackerEntry? GetEntry(ulong guildId);
}