namespace Lavalink4NET.Protocol.Payloads;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

[JsonConverter(typeof(PayloadJsonConverter))]
public interface IPayload
{
}