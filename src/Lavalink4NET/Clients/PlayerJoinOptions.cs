namespace Lavalink4NET.Clients;

public readonly record struct PlayerJoinOptions(
    bool? ConnectToVoiceChannel = null,
    MemberVoiceStateBehavior VoiceStateBehavior = MemberVoiceStateBehavior.Ignore);