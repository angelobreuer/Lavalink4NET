namespace Lavalink4NET.Payloads;

using System.Text.Json;
using Lavalink4NET.Player;

public sealed class PayloadContext
{
    public PayloadContext(LavalinkSocket socket, OpCode opCode, EventType? eventType, JsonElement rootElement, ulong? guildId = null)
    {
        Socket = socket;
        OpCode = opCode;
        EventType = eventType;
        RootElement = rootElement;
        GuildId = guildId;
    }

    public EventType? EventType { get; }

    public bool IsEvent => EventType is not null;

    public LavalinkSocket Socket { get; }

    public LavalinkNode? Node => Socket as LavalinkNode;

    public LavalinkPlayer? AssociatedPlayer => GuildId is null ? null : Node?.GetPlayer(GuildId.Value);

    public OpCode OpCode { get; }

    public JsonElement RootElement { get; }

    public ulong? GuildId { get; }

    public T DeserializeAs<T>(JsonSerializerOptions? options = null)
    {
        return RootElement.Deserialize<T>(options)!;
    }
}
