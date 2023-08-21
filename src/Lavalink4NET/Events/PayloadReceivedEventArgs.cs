namespace Lavalink4NET.Events;

using System;
using Lavalink4NET.Protocol.Payloads;

/// <summary>
///     The event arguments for the <see cref="LavalinkSocket.PayloadReceived"/> event.
/// </summary>
public sealed class PayloadReceivedEventArgs : EventArgs
{
    public PayloadReceivedEventArgs(IPayload payload)
    {
        Payload = payload;
    }

    public IPayload Payload { get; }
}
