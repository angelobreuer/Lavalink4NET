namespace Lavalink4NET.Protocol.Tests.Responses;

using System.Text.Json;
using System.Text.Json.Nodes;
using Lavalink4NET.Protocol.Responses;

public sealed class HttpErrorResponseTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "timestamp": 1667857581613,
              "status": 404,
              "error": "Not Found",
              "trace": "...",
              "message": "Session not found",
              "path": "/v3/sessions/xtaug914v9k5032f/players/817327181659111454"
            }
            """);

        // Act
        var payload = node.Deserialize<HttpErrorResponse>();
        var serializedString = JsonSerializer.Serialize(payload);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
