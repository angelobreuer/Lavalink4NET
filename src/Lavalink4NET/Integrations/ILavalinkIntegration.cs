namespace Lavalink4NET.Integrations;

using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Payloads;

public interface ILavalinkIntegration
{
    ValueTask<bool> ProcessPayloadAsync(PayloadContext payloadContext, CancellationToken cancellationToken = default);

    ValueTask InterceptPayloadAsync(JsonNode jsonNode, CancellationToken cancellationToken = default);
}
