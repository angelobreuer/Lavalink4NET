namespace Lavalink4NET.Protocol.Tests.Converters;

using System.Buffers;
using System.Text.Json;
using Lavalink4NET.Protocol.Converters;

public sealed class DurationJsonConverterTests
{
    [Fact]
    public void TestRead()
    {
        // Arrange
        var data = "1000"u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new DurationJsonConverter();
        _ = utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(
            reader: ref utf8JsonReader,
            typeToConvert: typeof(TimeSpan),
            options: JsonSerializerOptions.Default);

        // Assert
        Assert.Equal(
            expected: TimeSpan.FromSeconds(1),
            actual: result);
    }

    [Fact]
    public void TestWrite()
    {
        // Arrange
        var expectedData = "1000"u8;
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var jsonConverter = new DurationJsonConverter();

        // Act
        using (var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter))
        {
            jsonConverter.Write(
                writer: utf8JsonWriter,
                value: TimeSpan.FromSeconds(1),
                options: JsonSerializerOptions.Default);
        }

        // Assert
        Assert.Equal(
            expected: expectedData.ToArray(),
            actual: arrayBufferWriter.WrittenSpan.ToArray());
    }
}