namespace Lavalink4NET.Integrations.SponsorBlock.Events;

using System;
using System.Collections.Immutable;

public sealed class ChaptersLoadedEventArgs : EventArgs
{
    public ChaptersLoadedEventArgs(ulong guildId, ImmutableArray<Chapter> chapters)
    {
        GuildId = guildId;
        Chapters = chapters;
    }

    public ulong GuildId { get; }

    public ImmutableArray<Chapter> Chapters { get; }
}
