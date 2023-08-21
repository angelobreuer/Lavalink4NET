namespace Lavalink4NET.Protocol.Models.RoutePlanners;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

[JsonConverter(typeof(RoutePlannerTypeJsonConverter))]
public enum RoutePlannerType : byte
{
    RotatingIpRoutePlanner,
    NanoIpRoutePlanner,
    RotatingNanoIpRoutePlanner,
    BalancingIpRoutePlanner,
}
