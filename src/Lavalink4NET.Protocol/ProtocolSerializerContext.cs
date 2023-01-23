namespace Lavalink4NET.Protocol;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Responses;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(HttpErrorResponse))]
internal sealed partial class ProtocolSerializerContext : JsonSerializerContext
{
}
