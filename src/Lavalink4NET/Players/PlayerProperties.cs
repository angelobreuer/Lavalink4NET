namespace Lavalink4NET.Players;

using Lavalink4NET.Clients;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Rest;

public sealed record class PlayerProperties(
    ILavalinkApiClient ApiClient,
    IDiscordClientWrapper Client,
    ulong GuildId,
    ulong VoiceChannelId,
    string SessionId,
    PlayerInformationModel Model,
    bool DisconnectOnStop);
