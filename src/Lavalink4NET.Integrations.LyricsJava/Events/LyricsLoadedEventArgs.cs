namespace Lavalink4NET.Integrations.LyricsJava.Events;

public class LyricsLoadedEventArgs : EventArgs
{
    public LyricsLoadedEventArgs(ulong guildId, Lyrics lyrics)
    {
        GuildId = guildId;
        Lyrics = lyrics;
    }
    
    public ulong GuildId { get; }
    
    public Lyrics Lyrics { get; }
}