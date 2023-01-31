namespace Lavalink4NET.Protocol.Payloads.Events;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

[JsonConverter(typeof(TrackEndReasonJsonConverter))]
public enum TrackEndReason : byte
{
    Finished,
    LoadFailed,
    Stopped,
    Replaced,
    Cleanup,
}
