namespace Lavalink4NET.Protocol.Tests.Models;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class PlayerFilterMapModelTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "volume": 1,
              "equalizer": [
                {
                  "band": 0,
                  "gain": 0.2
                }
              ],
              "karaoke": {
                "level": 1,
                "monoLevel": 1,
                "filterBand": 220,
                "filterWidth": 100
              },
              "timescale": {
                "speed": 1,
                "pitch": 1,
                "rate": 1
              },
              "tremolo": {
                "frequency": 2,
                "depth": 0
              },
              "vibrato": {
                "frequency": 2,
                "depth": 0
              },
              "rotation": {
                "rotationHz": 0
              },
              "distortion": {
                "sinOffset": 0,
                "sinScale": 1,
                "cosOffset": 0,
                "cosScale": 1,
                "tanOffset": 0,
                "tanScale": 1,
                "offset": 0,
                "scale": 1
              },
              "channelMix": {
                "leftToLeft": 1,
                "leftToRight": 0,
                "rightToLeft": 0,
                "rightToRight": 1
              },
              "lowPass": {
                "smoothing": 20
              }
            }
            """);

        // Act
        var payload = node.Deserialize(ProtocolSerializerContext.Default.PlayerFilterMapModel);
        var serializedString = JsonSerializer.Serialize(payload, ProtocolSerializerContext.Default.PlayerFilterMapModel!);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
