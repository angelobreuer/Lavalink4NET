namespace Lavalink4NET.Integrations;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Payloads;

public interface ILavalinkIntegration
{
    ValueTask ProcessPayloadAsync(IPayload payload, CancellationToken cancellationToken = default);
}
