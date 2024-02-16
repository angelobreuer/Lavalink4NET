namespace Lavalink4NET.Rest;

using Microsoft.Extensions.Options;

public record class LavalinkApiClientOptions
{
    internal const string DefaultPassphrase = "youshallnotpass";
    internal static readonly Uri DefaultBaseAddress = new("http://localhost:2333/");

    public string? Label { get; set; }

    public string Passphrase { get; set; } = DefaultPassphrase;

    public string HttpClientName { get; set; } = Options.DefaultName;

    public Uri BaseAddress { get; set; } = DefaultBaseAddress;

    public int BufferSize { get; set; } = 64 * 1024; // 64 KiB

    public LavalinkTrackCacheOptions TrackCacheOptions { get; set; } = LavalinkTrackCacheOptions.Default;
}
