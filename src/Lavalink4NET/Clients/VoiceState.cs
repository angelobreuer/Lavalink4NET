namespace Lavalink4NET.Discord;

/// <summary>
///     Represents the information for a discord user voice state.
/// </summary>
public readonly record struct VoiceState(ulong? VoiceChannelId, string SessionId);