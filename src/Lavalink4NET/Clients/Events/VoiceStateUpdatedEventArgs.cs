namespace Lavalink4NET.Clients.Events;

using System;
using Lavalink4NET.Clients;

public sealed class VoiceStateUpdatedEventArgs : EventArgs
{
    public VoiceStateUpdatedEventArgs(ulong guildId, VoiceState voiceState)
    {
        GuildId = guildId;
        VoiceState = voiceState;
    }

    public ulong GuildId { get; }

    public VoiceState VoiceState { get; }
}
