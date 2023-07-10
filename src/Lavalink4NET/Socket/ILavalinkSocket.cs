namespace Lavalink4NET.Socket;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Payloads;

public interface ILavalinkSocket : IDisposable
{
    string Label { get; }

    ValueTask<IPayload> ReceiveAsync(CancellationToken cancellationToken = default);

    ValueTask RunAsync(CancellationToken cancellationToken = default);
}
