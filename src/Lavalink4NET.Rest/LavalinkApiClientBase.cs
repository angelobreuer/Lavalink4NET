namespace Lavalink4NET.Rest;

using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public abstract class LavalinkApiClientBase
{
    private static int _insecurePassphraseNoticeSent;

    private readonly IOptions<LavalinkApiClientOptions> _options;
    private readonly ILogger<LavalinkApiClientBase> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthenticationHeaderValue _authenticationHeaderValue;

    public LavalinkApiClientBase(
        IHttpClientFactory httpClientFactory,
        IOptions<LavalinkApiClientOptions> options,
        ILogger<LavalinkApiClientBase> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);

        if (options.Value.Passphrase.Equals("youshallnotpass", StringComparison.Ordinal) &&
            Interlocked.CompareExchange(ref _insecurePassphraseNoticeSent, 1, 0) is 0)
        {
            logger.LogWarning("You are using the default password for Lavalink. Please change");
        }

        _options = options;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _authenticationHeaderValue = new AuthenticationHeaderValue(options.Value.Passphrase);
    }

    public HttpClient CreateHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient(_options.Value.HttpClientName);
        httpClient.DefaultRequestHeaders.Authorization = _authenticationHeaderValue;
        return httpClient;
    }
}
