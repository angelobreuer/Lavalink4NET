namespace Lavalink4NET.Rest.Tests;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebSockets;

[ExcludeFromCodeCoverage]
internal sealed class HttpClientFactory : IHttpClientFactory, IAsyncDisposable
{
    private int _state; // 0 = build, 1 = run, 2 = disposed
    private readonly WebApplication _application;

    public HttpClientFactory()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddWebSockets(x => { });

        _application = builder.Build();
    }

    public void Start()
    {
        var state = Interlocked.CompareExchange(ref _state, 1, 0);
        ObjectDisposedException.ThrowIf(state is 2, this);

        if (state is 1)
        {
            throw new InvalidOperationException("The application is already running.");
        }

        Debug.Assert(state is 0);
        _ = _application.RunAsync();
    }

    public WebApplication Application
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state is 2, this);

            if (_state is not 0)
            {
                throw new InvalidOperationException("The application can not be accessed after starting it.");
            }

            return _application;
        }
    }

    public HttpClient CreateClient(string name)
    {
        ObjectDisposedException.ThrowIf(_state is 2, this);

        if (_state is not 1)
        {
            throw new InvalidOperationException("The application must be started before creating a client.");
        }

        return _application.GetTestServer().CreateClient();
    }

    public ValueTask DisposeAsync()
    {
        var state = Interlocked.CompareExchange(ref _state, 2, 1);

        if (state is not 1)
        {
            return default;
        }

        return _application.DisposeAsync();
    }
}
