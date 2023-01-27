namespace Lavalink4NET.Rest.Tests;

using System;
using System.Net.Http;

internal sealed class DefaultHttpClientFactory : IHttpClientFactory, IDisposable
{
    private readonly HttpMessageHandler _httpMessageHandler;

    public DefaultHttpClientFactory()
    {
        _httpMessageHandler = new SocketsHttpHandler();
    }

    public HttpClient CreateClient(string name)
    {
        return new HttpClient(_httpMessageHandler, disposeHandler: false);
    }

    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }
}
