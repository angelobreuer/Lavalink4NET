namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Lavalink4NET.Protocol.Payloads;

public sealed class PayloadJsonConverter : JsonConverter<IPayload>
{
    private static readonly Dictionary<Type, (string EventName, JsonTypeInfo JsonTypeInfo)> _events;
    private static readonly Dictionary<string, JsonTypeInfo> _eventsMap;
    private static readonly Dictionary<Type, (string OpCode, JsonTypeInfo JsonTypeInfo)> _payloads;
    private static readonly Dictionary<string, JsonTypeInfo> _payloadsMap;
    private static readonly object _syncRoot;

    static PayloadJsonConverter()
    {
        _payloads = new Dictionary<Type, (string EventName, JsonTypeInfo JsonTypeInfo)>();
        _payloadsMap = new Dictionary<string, JsonTypeInfo>(StringComparer.OrdinalIgnoreCase);

        _events = new Dictionary<Type, (string OpCode, JsonTypeInfo JsonTypeInfo)>();
        _eventsMap = new Dictionary<string, JsonTypeInfo>(StringComparer.OrdinalIgnoreCase);

        _syncRoot = new object();

        RegisterPayloadInternal("ready", ProtocolSerializerContext.Default.ReadyPayload);
        RegisterPayloadInternal("playerUpdate", ProtocolSerializerContext.Default.PlayerUpdatePayload);
        RegisterPayloadInternal("stats", ProtocolSerializerContext.Default.StatisticsPayload);

        RegisterEventInternal("TrackStartEvent", ProtocolSerializerContext.Default.TrackStartEventPayload);
        RegisterEventInternal("TrackEndEvent", ProtocolSerializerContext.Default.TrackEndEventPayload);
        RegisterEventInternal("TrackExceptionEvent", ProtocolSerializerContext.Default.TrackExceptionEventPayload);
        RegisterEventInternal("TrackStuckEvent", ProtocolSerializerContext.Default.TrackStuckEventPayload);
        RegisterEventInternal("WebSocketClosedEvent", ProtocolSerializerContext.Default.WebSocketClosedEventPayload);
    }

    public static void RegisterEvent<T>(string eventName, JsonTypeInfo<T> jsonTypeInfo) where T : IEventPayload
    {
        lock (_syncRoot)
        {
            RegisterEventInternal(eventName, jsonTypeInfo);
        }
    }

    public static void RegisterPayload<T>(string operationCode, JsonTypeInfo<T> jsonTypeInfo) where T : IPayload
    {
        lock (_syncRoot)
        {
            RegisterPayloadInternal(operationCode, jsonTypeInfo);
        }
    }

    public override IPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var node = JsonNode.Parse(ref reader);
        Debug.Assert(node is not null); // We're in a converter, we are protected against null tokens

        var operationCode = node["op"]?.GetValue<string>();

        if (string.IsNullOrWhiteSpace(operationCode))
        {
            ThrowInvalidOpCode();
        }

        var jsonTypeInfo = default(JsonTypeInfo?);
        if (operationCode.Equals("event", StringComparison.OrdinalIgnoreCase))
        {
            var eventName = node["type"]?.GetValue<string>();

            if (eventName is null || !_eventsMap.TryGetValue(eventName, out jsonTypeInfo))
            {
                ThrowInvalidEventName();
            }
        }
        else if (!_payloadsMap.TryGetValue(operationCode, out jsonTypeInfo))
        {
            ThrowInvalidOpCode();
        }

        return (IPayload?)node.Deserialize(jsonTypeInfo.Type, jsonTypeInfo.Options);

        [DoesNotReturn]
        static void ThrowInvalidOpCode() => throw new JsonException("Missing or invalid 'op' (operation code) property in payload.");

        [DoesNotReturn]
        static void ThrowInvalidEventName() => throw new JsonException("Missing or invalid 'event' (event name) property in payload.");
    }

    public override void Write(Utf8JsonWriter writer, IPayload value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(options);

        var payloadType = value.GetType();
        var newNode = new JsonObject();

        if (!_payloads.TryGetValue(payloadType, out var pair))
        {
            if (!_events.TryGetValue(payloadType, out pair))
            {
                throw new InvalidOperationException("The payload type is not registered.");
            }

            newNode["op"] = "event";
            newNode["type"] = pair.OpCode;
        }
        else
        {
            newNode["op"] = pair.OpCode;
        }

        var node = JsonSerializer.SerializeToNode(value, value.GetType(), pair.JsonTypeInfo.Options);

        foreach (var (propertyName, propertyValue) in node!.AsObject())
        {
            newNode[propertyName] = propertyValue.Deserialize<JsonNode>(); // TODO
        }

        newNode.WriteTo(writer, options);
    }

    private static void RegisterEventInternal<T>(string eventName, JsonTypeInfo<T> jsonTypeInfo) where T : IEventPayload
    {
        _events[typeof(T)] = (eventName, jsonTypeInfo);
        _eventsMap[eventName] = jsonTypeInfo;
    }

    private static void RegisterPayloadInternal<T>(string operationCode, JsonTypeInfo<T> jsonTypeInfo) where T : IPayload
    {
        _payloads[typeof(T)] = (operationCode, jsonTypeInfo);
        _payloadsMap[operationCode] = jsonTypeInfo;
    }
}
