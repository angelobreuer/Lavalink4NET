namespace Lavalink4NET.Integrations.LyricsJava.Players;

using Lavalink4NET.Players;

public interface ILavaLyricsPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifyLyricsLoadedAsync(Lyrics lyrics, CancellationToken cancellationToken = default);
}