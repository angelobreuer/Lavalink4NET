namespace Lavalink4NET.Protocol.Tests.Payloads;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class PlayerUpdatePayloadTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "op": "playerUpdate",
              "guildId": "123",
              "state": {
                "time": 1500467109,
                "position": 60000,
                "connected": true,
                "ping": 50
              }
            }
            """);

        // Act
        var payload = node.Deserialize(ProtocolSerializerContext.Default.IPayload);
        var serializedString = JsonSerializer.Serialize(payload);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
