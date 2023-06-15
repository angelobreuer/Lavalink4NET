namespace Lavalink4NET;

using System;
using Microsoft.Extensions.Options;

public sealed record class LavalinkNodeOptions
{
    public string Passphrase { get; set; } = "youshallnotpass";

    public Uri? WebSocketUri { get; set; }

    public string HttpClientName { get; set; } = Options.DefaultName;

    public TimeSpan ReadyTimeout { get; set; } = TimeSpan.FromSeconds(10);
}
