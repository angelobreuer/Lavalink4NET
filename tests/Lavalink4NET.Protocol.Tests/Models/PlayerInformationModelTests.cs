namespace Lavalink4NET.Protocol.Tests.Models;

using System.Text.Json;
using System.Text.Json.Nodes;
using Lavalink4NET.Protocol.Models;

public sealed class PlayerInformationModelTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
                "guildId": "123",
                "track": {
                    "encoded": "QAAAjQIAJVJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAADlJpY2tBc3RsZXlWRVZPAAAAAAADPCAAC2RRdzR3OVdnWGNRAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9ZFF3NHc5V2dYY1EAB3lvdXR1YmUAAAAAAAAAAA==",
                    "info": {
                      "identifier": "dQw4w9WgXcQ",
                      "isSeekable": true,
                      "author": "RickAstleyVEVO",
                      "length": 212000,
                      "isStream": false,
                      "position": 0,
                      "title": "Rick Astley - Never Gonna Give You Up",
                      "uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                      "artworkUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
                      "isrc": null,
                      "sourceName": "youtube"
                    }
                },
                "volume": 100,
                "paused": false,
                "voice": {
                    "token": "...",
                    "endpoint": "...",
                    "sessionId": "..."
                },
                "filters": { }
            }
            """);

        // Act
        var payload = node.Deserialize<PlayerInformationModel>();
        var serializedString = JsonSerializer.Serialize(payload);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
