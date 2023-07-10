namespace Lavalink4NET.Rest;

using Microsoft.Extensions.Options;

public record class LavalinkApiClientOptions
{
    internal const string DefaultPassphrase = "youshallnotpass";
    internal static readonly Uri DefaultBaseAddress = new("http://localhost:2333/");

    public string? Label { get; init; }

    public string Passphrase { get; init; } = DefaultPassphrase;

    public string HttpClientName { get; init; } = Options.DefaultName;

    public Uri BaseAddress { get; init; } = DefaultBaseAddress;
}
