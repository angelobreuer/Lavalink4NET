namespace Lavalink4NET.Protocol;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Models.Usage;
using Lavalink4NET.Protocol.Responses;
using Lavalink4NET.Protocol.Tests.Models;

[JsonSerializable(typeof(HttpErrorResponse))]
[JsonSerializable(typeof(PlayerFilterMapModel))]
[JsonSerializable(typeof(LavalinkServerInformationModel))]
[JsonSerializable(typeof(LavalinkServerStatisticsModel))]
internal sealed partial class ProtocolSerializerContext : JsonSerializerContext
{
}
