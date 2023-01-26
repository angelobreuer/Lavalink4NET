namespace Lavalink4NET.Protocol.Tests;

using System.Buffers;
using System.Text.Json;
using Lavalink4NET.Protocol.Converters;

public sealed class VolumeJsonConverterTests
{
    [Fact]
    public void TestRead()
    {
        // Arrange
        var data = "1000"u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new VolumeJsonConverter();
        _ = utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(
            reader: ref utf8JsonReader,
            typeToConvert: typeof(float),
            options: JsonSerializerOptions.Default);

        // Assert
        Assert.Equal(
            expected: 10.0F,
            actual: result);
    }

    [Fact]
    public void TestWrite()
    {
        // Arrange
        var expectedData = "1000"u8;
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var jsonConverter = new VolumeJsonConverter();

        // Act
        using (var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter))
        {
            jsonConverter.Write(
                writer: utf8JsonWriter,
                value: 10.0F,
                options: JsonSerializerOptions.Default);
        }

        // Assert
        Assert.Equal(
            expected: expectedData.ToArray(),
            actual: arrayBufferWriter.WrittenSpan.ToArray());
    }
}