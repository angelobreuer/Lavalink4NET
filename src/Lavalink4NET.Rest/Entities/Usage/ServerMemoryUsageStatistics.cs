namespace Lavalink4NET.Rest.Entities.Usage;

public sealed record class ServerMemoryUsageStatistics(
    long FreeMemory,
    long UsedMemory,
    long AllocatedMemory,
    long ReservableMemory);