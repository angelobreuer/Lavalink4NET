namespace Lavalink4NET.Protocol.Tests.Payloads;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class WebSocketClosedEventPayloadTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "op": "event",
              "type": "WebSocketClosedEvent",
              "guildId": "123",
              "code": 4006,
              "reason": "Your session is no longer valid.",
              "byRemote": true
            }
            """);

        // Act
        var payload = node.Deserialize(ProtocolSerializerContext.Default.IPayload);
        var serializedString = JsonSerializer.Serialize(payload);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
