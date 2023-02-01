namespace Lavalink4NET.Socket;

using System;
using Microsoft.Extensions.Options;

public sealed record class LavalinkSocketOptions
{
    private static readonly Uri _defaultAddress = new("ws://localhost:2333/");

    public Uri Uri { get; init; } = _defaultAddress;

    public string HttpClientName { get; init; } = Options.DefaultName;

    public int ShardCount { get; init; } = 1;

    public ulong UserId { get; init; }
}
