namespace Lavalink4NET.Protocol.Payloads;

public interface IEventPayload : IPayload
{
    ulong GuildId { get; }
}
