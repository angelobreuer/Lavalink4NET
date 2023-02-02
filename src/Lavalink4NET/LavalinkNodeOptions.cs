namespace Lavalink4NET;

using System;
using Lavalink4NET.Socket;
using Microsoft.Extensions.Options;

public sealed record class LavalinkNodeOptions
{
    public Uri Uri { get; init; } = LavalinkSocketOptions.DefaultAddress;

    public string HttpClientName { get; init; } = Options.DefaultName;
}
