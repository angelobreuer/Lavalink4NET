namespace Lavalink4NET.Integrations.LyricsJava;

using Lavalink4NET.Events;
using Lavalink4NET.Integrations.LyricsJava.Events;

public interface ILyricsJavaIntegration
{
    event AsyncEventHandler<LyricsLoadedEventArgs>? LyricsLoaded;
}