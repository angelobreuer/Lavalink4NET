namespace Lavalink4NET.Protocol.Models.RoutePlanners;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

[JsonConverter(typeof(RoutePlannerIpBlockTypeJsonConverter))]
public enum RoutePlannerIpBlockType : byte
{
    Inet4Address,
    Inet6Address,
}
