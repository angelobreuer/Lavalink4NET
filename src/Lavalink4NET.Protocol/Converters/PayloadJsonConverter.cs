namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Protocol.Payloads.Events;

internal sealed class PayloadJsonConverter : JsonConverter<IPayload>
{
    private static readonly Dictionary<Type, string> _events;
    private static readonly Dictionary<string, Type> _eventsMap;
    private static readonly Dictionary<Type, string> _payloads;
    private static readonly Dictionary<string, Type> _payloadsMap;
    private static readonly object _syncRoot;

    static PayloadJsonConverter()
    {
        _payloads = new Dictionary<Type, string>();
        _payloadsMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        _events = new Dictionary<Type, string>();
        _eventsMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        _syncRoot = new object();

        RegisterPayloadInternal<ReadyPayload>("ready");
        RegisterPayloadInternal<PlayerUpdatePayload>("playerUpdate");
        RegisterPayloadInternal<StatisticsPayload>("stats");

        RegisterEventInternal<TrackStartEventPayload>("TrackStartEvent");
        RegisterEventInternal<TrackEndEventPayload>("TrackEndEvent");
        RegisterEventInternal<TrackExceptionEventPayload>("TrackExceptionEvent");
        RegisterEventInternal<TrackStuckEventPayload>("TrackStuckEvent");
        RegisterEventInternal<WebSocketClosedEventPayload>("WebSocketClosedEvent");
    }

    public static void RegisterEvent<T>(string eventName) where T : IEventPayload
    {
        lock (_syncRoot)
        {
            RegisterEventInternal<T>(eventName);
        }
    }

    public static void RegisterPayload<T>(string operationCode) where T : IPayload
    {
        lock (_syncRoot)
        {
            RegisterPayloadInternal<T>(operationCode);
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

        var payloadType = default(Type?);
        if (operationCode.Equals("event", StringComparison.OrdinalIgnoreCase))
        {
            var eventName = node["event"]?.GetValue<string>();

            if (eventName is null || !_eventsMap.TryGetValue(eventName, out payloadType))
            {
                ThrowInvalidEventName();
            }
        }
        else if (!_payloadsMap.TryGetValue(operationCode, out payloadType))
        {
            ThrowInvalidOpCode();
        }

        return (IPayload?)node.Deserialize(payloadType, options);

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
        var node = JsonSerializer.SerializeToNode(value, payloadType, options);
        var newNode = new JsonObject();

        if (!_payloads.TryGetValue(payloadType, out var opCode))
        {
            if (!_events.TryGetValue(payloadType, out var eventName))
            {
                throw new InvalidOperationException("The payload type is not registered.");
            }

            newNode["op"] = "event";
            newNode["event"] = eventName;
        }
        else
        {
            newNode["op"] = opCode;
        }

        foreach (var (propertyName, propertyValue) in node!.AsObject())
        {
            newNode[propertyName] = propertyValue;
        }

        newNode.WriteTo(writer, options);
    }

    private static void RegisterEventInternal<T>(string eventName) where T : IEventPayload
    {
        _events[typeof(T)] = eventName;
        _eventsMap[eventName] = typeof(T);
    }

    private static void RegisterPayloadInternal<T>(string operationCode) where T : IPayload
    {
        _payloads[typeof(T)] = operationCode;
        _payloadsMap[operationCode] = typeof(T);
    }
}
