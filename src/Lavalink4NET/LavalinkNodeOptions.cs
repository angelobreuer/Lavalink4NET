namespace Lavalink4NET;

using System;
using Microsoft.Extensions.Options;

public sealed record class LavalinkNodeOptions
{
    public string Passphrase { get; init; } = "youshallnotpass";

    public Uri? WebSocketUri { get; init; }

    public string HttpClientName { get; init; } = Options.DefaultName;
}
