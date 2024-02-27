namespace Lavalink4NET.Integrations.SponsorBlock.Events;

using System;

public sealed class ChapterStartedEventArgs
{
    public ChapterStartedEventArgs(ulong guildId, Chapter chapter)
    {
        ArgumentNullException.ThrowIfNull(chapter);

        GuildId = guildId;
        Chapter = chapter;
    }

    public ulong GuildId { get; }

    public Chapter Chapter { get; }
}
