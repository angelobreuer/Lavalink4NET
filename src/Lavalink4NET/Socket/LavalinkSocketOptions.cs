namespace Lavalink4NET.Socket;

using System;
using Microsoft.Extensions.Options;

public sealed record class LavalinkSocketOptions
{
    internal static readonly Uri DefaultAddress = new("ws://localhost:2333/");

    public Uri Uri { get; init; } = DefaultAddress;

    public string HttpClientName { get; init; } = Options.DefaultName;

    public int ShardCount { get; init; } = 1;

    public ulong UserId { get; init; }
}
