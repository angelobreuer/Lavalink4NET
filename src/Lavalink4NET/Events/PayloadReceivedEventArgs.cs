namespace Lavalink4NET.Events;

using System;
using Payloads;

/// <summary>
///     The event arguments for the <see cref="LavalinkSocket.PayloadReceived"/> event.
/// </summary>
public sealed class PayloadReceivedEventArgs
    : EventArgs
{
    public PayloadReceivedEventArgs(PayloadContext payloadContext)
    {
        PayloadContext = payloadContext;
    }

    public PayloadContext PayloadContext { get; }
}
