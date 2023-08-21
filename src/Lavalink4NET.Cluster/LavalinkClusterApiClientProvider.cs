namespace Lavalink4NET.Cluster;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.LoadBalancing;
using Lavalink4NET.Rest;

internal sealed class LavalinkClusterApiClientProvider : ILavalinkApiClientProvider
{
	private readonly ILavalinkClusterLoadBalancer _loadBalancer;

	public LavalinkClusterApiClientProvider(ILavalinkClusterLoadBalancer loadBalancer)
	{
		ArgumentNullException.ThrowIfNull(loadBalancer);

		_loadBalancer = loadBalancer;
	}

	public async ValueTask<ILavalinkApiClient> GetClientAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var node = await _loadBalancer
			.GetPreferredNodeAsync(cancellationToken)
			.ConfigureAwait(false);

		return node.ApiClient;
	}
}
