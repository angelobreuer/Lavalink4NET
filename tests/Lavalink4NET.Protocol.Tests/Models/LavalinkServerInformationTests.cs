namespace Lavalink4NET.Protocol.Tests.Models;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class LavalinkServerInformationTests
{
    [Fact]
    public void TestRoundtrip()
    {
        // Arrange
        var node = JsonNode.Parse("""
            {
              "version": {
                "semver": "3.7.0-rc.1",
                "major": 3,
                "minor": 7,
                "patch": 0,
                "preRelease": "rc.1"
              },
              "buildTime": 1664223916812,
              "git": {
                "branch": "master",
                "commit": "85c5ab5",
                "commitTime": 1664223916812
              },
              "jvm": "18.0.2.1",
              "lavaplayer": "1.3.98.4-original",
              "sourceManagers": [
                "youtube",
                "soundcloud"
              ],
              "filters": [
                "equalizer",
            	"karaoke",
            	"timescale",
            	"channelMix"
              ],
              "plugins": [
                {
                  "name": "some-plugin",
                  "version": "1.0.0"
                },
                {
                  "name": "foo-plugin",
                  "version": "1.2.3"
                }
              ]
            }
            """);

        // Act
        var payload = node.Deserialize(ProtocolSerializerContext.Default.LavalinkServerInformationModel);
        var serializedString = JsonSerializer.Serialize(payload, ProtocolSerializerContext.Default.LavalinkServerInformationModel!);

        // Assert
        Assert.Equal(node!.ToJsonString(), serializedString);
    }
}
