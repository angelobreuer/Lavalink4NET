namespace Lavalink4NET.Players;

using Lavalink4NET.Clients;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Rest;

public sealed record class PlayerProperties(
    ILavalinkApiClient ApiClient,
    IDiscordClientWrapper Client,
    string SessionId,
    PlayerInformationModel Model,
    ulong VoiceChannelId,
    bool DisconnectOnStop);
