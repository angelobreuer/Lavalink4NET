namespace Lavalink4NET.Protocol.Tests.Payloads;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class StatisticsPayloadTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "op": "stats",
              "players": 1,
              "playingPlayers": 1,
              "uptime": 123456789,
              "memory": {
                "free": 123456789,
                "used": 123456789,
                "allocated": 123456789,
                "reservable": 123456789
              },
              "cpu": {
                "cores": 4,
                "systemLoad": 0.5,
                "lavalinkLoad": 0.5
              },
              "frameStats": {
                "sent": 123456789,
                "nulled": 123456789,
                "deficit": 123456789
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
