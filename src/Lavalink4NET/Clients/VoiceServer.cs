namespace Lavalink4NET.Clients;

/// <summary>
///     Represents the information for a discord voice server.
/// </summary>
public readonly record struct VoiceServer(string Token, string Endpoint);