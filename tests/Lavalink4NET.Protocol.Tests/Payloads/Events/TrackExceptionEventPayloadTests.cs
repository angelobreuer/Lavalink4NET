namespace Lavalink4NET.Protocol.Tests.Payloads;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class TrackExceptionEventPayloadTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "op": "event",
              "type": "TrackExceptionEvent",
              "guildId": "123",
              "encodedTrack": "...",
              "exception": {
                "message": "...",
                "severity": "COMMON",
                "cause": "..."
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
