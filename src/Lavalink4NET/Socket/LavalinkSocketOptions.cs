namespace Lavalink4NET.Socket;

using System;
using Microsoft.Extensions.Options;

public sealed record class LavalinkSocketOptions
{
    internal static readonly Uri DefaultAddress = new("ws://localhost:2333/");

    public string? Label { get; set; }

    public Uri Uri { get; init; } = DefaultAddress;

    public string Passphrase { get; set; } = "youshallnotpass";

    public string HttpClientName { get; init; } = Options.DefaultName;

    public int ShardCount { get; init; } = 1;

    public ulong UserId { get; init; }

    public string? SessionId { get; init; }

    public int BufferSize { get; init; } = 64 * 1024; // 64 KiB
}
