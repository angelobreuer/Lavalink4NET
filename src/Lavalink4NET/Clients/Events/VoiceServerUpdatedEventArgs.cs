namespace Lavalink4NET.Clients.Events;

using System;
using Lavalink4NET.Discord;

public sealed class VoiceServerUpdatedEventArgs : EventArgs
{
    public VoiceServerUpdatedEventArgs(ulong guildId, VoiceServer voiceServer)
    {
        GuildId = guildId;
        VoiceServer = voiceServer;
    }

    public ulong GuildId { get; }

    public VoiceServer VoiceServer { get; }
}
