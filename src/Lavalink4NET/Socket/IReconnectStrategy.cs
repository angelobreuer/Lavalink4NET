namespace Lavalink4NET.Socket;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IReconnectStrategy
{
    ValueTask<TimeSpan?> GetNextDelayAsync(DateTimeOffset interruptedAt, int attempt, CancellationToken cancellationToken = default);
}