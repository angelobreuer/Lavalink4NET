namespace Lavalink4NET.Protocol;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Models.Filters;
using Lavalink4NET.Protocol.Models.Server;
using Lavalink4NET.Protocol.Models.Usage;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Protocol.Responses;

[JsonSerializable(typeof(HttpErrorResponse))]
[JsonSerializable(typeof(PlayerFilterMapModel))]
[JsonSerializable(typeof(LavalinkServerInformationModel))]
[JsonSerializable(typeof(LavalinkServerStatisticsModel))]
[JsonSerializable(typeof(IPayload))]
[JsonSerializable(typeof(ReadyPayload))]
[JsonSerializable(typeof(PlayerUpdatePayload))]
[JsonSerializable(typeof(StatisticsPayload))]
[JsonSerializable(typeof(TrackStartEventPayload))]
[JsonSerializable(typeof(TrackEndEventPayload))]
[JsonSerializable(typeof(TrackExceptionEventPayload))]
[JsonSerializable(typeof(TrackStuckEventPayload))]
[JsonSerializable(typeof(WebSocketClosedEventPayload))]
[JsonSerializable(typeof(ImmutableArray<PlayerInformationModel>))]
[JsonSerializable(typeof(TrackLoadResponse))]
[JsonSerializable(typeof(PlayerUpdateProperties))]
internal sealed partial class ProtocolSerializerContext : JsonSerializerContext
{
}
