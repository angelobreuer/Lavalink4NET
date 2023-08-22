namespace Lavalink4NET;

using System;
using Lavalink4NET.Rest;

public abstract record class LavalinkNodeOptions : LavalinkApiClientOptions
{
    public Uri? WebSocketUri { get; set; } = null;

    public TimeSpan ReadyTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public LavalinkSessionResumptionOptions ResumptionOptions { get; set; } = new LavalinkSessionResumptionOptions(timeout: TimeSpan.FromSeconds(60));
}
