namespace Lavalink4NET.Protocol.Tests.Payloads;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class TrackStuckEventPayloadTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "op": "event",
              "type": "TrackStuckEvent",
              "guildId": "123",
              "encodedTrack": "...",
              "thresholdMs": 123456789
            }
            """);

        // Act
        var payload = node.Deserialize(ProtocolSerializerContext.Default.IPayload);
        var serializedString = JsonSerializer.Serialize(payload);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
