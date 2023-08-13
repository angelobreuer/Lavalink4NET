namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Payloads.Events;

internal sealed class TrackEndReasonJsonConverter : JsonConverter<TrackEndReason>
{
    public override TrackEndReason Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetString()!;

        return value.ToUpperInvariant() switch
        {
            "FINISHED" => TrackEndReason.Finished,
            "LOADFAILED" or "LOAD_FAILED" => TrackEndReason.LoadFailed,
            "STOPPED" => TrackEndReason.Stopped,
            "REPLACED" => TrackEndReason.Replaced,
            "CLEANUP" => TrackEndReason.Cleanup,
            _ => throw new JsonException("Invalid track end reason."),
        };
    }

    public override void Write(Utf8JsonWriter writer, TrackEndReason value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        var strValue = value switch
        {
            TrackEndReason.Finished => "FINISHED",
            TrackEndReason.LoadFailed => "LOAD_FAILED",
            TrackEndReason.Stopped => "STOPPED",
            TrackEndReason.Replaced => "REPLACED",
            TrackEndReason.Cleanup => "CLEANUP",
            _ => throw new ArgumentException("Invalid track end reason."),
        };

        writer.WriteStringValue(strValue);
    }
}
