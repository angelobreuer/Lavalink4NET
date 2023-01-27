namespace Lavalink4NET.Rest;

using Microsoft.Extensions.Options;

public sealed record class LavalinkApiClientOptions
{
    private const string DefaultPassphrase = "youshallnotpass";

    private static readonly Uri _defaultBaseAddress = new("http://localhost:2333/");

    public string Passphrase { get; set; } = DefaultPassphrase;

    public string HttpClientName { get; set; } = Options.DefaultName;

    public Uri BaseAddress { get; set; } = _defaultBaseAddress;
}
