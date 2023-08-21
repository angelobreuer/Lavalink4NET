namespace Lavalink4NET.Integrations.ExtraFilters.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;
using Lavalink4NET.Protocol.Models.Filters;

public sealed class EchoFilterOptionsTests
{
    [Fact]
    public void TestDefault()
    {
        // Arrange
        var options = new EchoFilterOptions(
            Delay: null,
            Decay: null);

        // Act
        var isDefault = options.IsDefault;

        // Assert
        Assert.True(isDefault);
    }

    [Fact]
    public void TestNonDefault()
    {
        // Arrange
        var options = new EchoFilterOptions(
            Delay: 1.0F,
            Decay: 12.0F);

        // Act
        var isDefault = options.IsDefault;

        // Assert
        Assert.False(isDefault);
    }

    [Fact]
    public void TestApplyFilter()
    {
        // Arrange
        var options = new EchoFilterOptions(
            Delay: 1.0F,
            Decay: 12.0F);

        var filterMap = new PlayerFilterMapModel();

        // Act
        options.Apply(ref filterMap);

        // Assert
        var model = JsonSerializer.Serialize(filterMap);

        var content = """
            {
                "echo": {
                    "delay": 1,
                    "decay": 12
                }
            }
            """;

        Assert.Equal(
            expected: JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonNode>(model)),
            actual: JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonNode>(content)));
    }
}