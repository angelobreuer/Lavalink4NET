namespace Lavalink4NET.Clients.Events;

using System;
using Lavalink4NET.Clients;

public sealed class VoiceStateUpdatedEventArgs : EventArgs
{
    public VoiceStateUpdatedEventArgs(ulong guildId, ulong userId, bool isCurrentUser, VoiceState voiceState, VoiceState oldVoiceState)
    {
        GuildId = guildId;
        UserId = userId;
        IsCurrentUser = isCurrentUser;
        OldVoiceState = oldVoiceState;
        VoiceState = voiceState;
    }

    public ulong GuildId { get; }

    public ulong UserId { get; }

    public bool IsCurrentUser { get; }
    public VoiceState VoiceState { get; }

    public VoiceState OldVoiceState { get; }
}
