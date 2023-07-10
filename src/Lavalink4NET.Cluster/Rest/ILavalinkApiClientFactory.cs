namespace Lavalink4NET.Cluster.Rest;

using Lavalink4NET.Rest;
using Microsoft.Extensions.Options;

public interface ILavalinkApiClientFactory
{
    ILavalinkApiClient Create(IOptions<LavalinkApiClientOptions> options);
}
