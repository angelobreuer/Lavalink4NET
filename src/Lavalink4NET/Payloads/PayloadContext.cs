namespace Lavalink4NET.Payloads;

using System.Text.Json;

public sealed class PayloadContext
{
    public PayloadContext(OpCode opCode, EventType? eventType, JsonElement rootElement, ulong? guildId = null)
    {
        OpCode = opCode;
        EventType = eventType;
        RootElement = rootElement;
        GuildId = guildId;
    }

    public EventType? EventType { get; }

    public bool IsEvent => EventType is not null;

    public OpCode OpCode { get; }

    public JsonElement RootElement { get; }

    public ulong? GuildId { get; }

    public T DeserializeAs<T>(JsonSerializerOptions? options = null)
    {
        return RootElement.Deserialize<T>(options)!;
    }
}
