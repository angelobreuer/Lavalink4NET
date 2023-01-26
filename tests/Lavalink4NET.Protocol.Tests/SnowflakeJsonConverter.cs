namespace Lavalink4NET.Protocol.Tests;

using System.Buffers;
using System.Text.Json;
using Lavalink4NET.Protocol.Converters;

public sealed class SnowflakeJsonConverterTests
{
    [Fact]
    public void TestReadFromNumeric()
    {
        // Arrange
        var data = "1667857581613"u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new SnowflakeJsonConverter();
        _ = utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(
            reader: ref utf8JsonReader,
            typeToConvert: typeof(ulong),
            options: JsonSerializerOptions.Default);

        // Assert
        Assert.Equal(
            expected: 1667857581613UL,
            actual: result);
    }

    [Fact]
    public void TestReadFromString()
    {
        // Arrange
        var data = "\"1667857581613\""u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new SnowflakeJsonConverter();
        _ = utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(
            reader: ref utf8JsonReader,
            typeToConvert: typeof(DateTimeOffset),
            options: JsonSerializerOptions.Default);

        // Assert
        Assert.Equal(
            expected: 1667857581613UL,
            actual: result);
    }

    [Fact]
    public void TestWrite()
    {
        // Arrange
        var expectedData = "1667857581613"u8;
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var jsonConverter = new SnowflakeJsonConverter();

        // Act
        using (var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter))
        {
            jsonConverter.Write(
                writer: utf8JsonWriter,
                value: 1667857581613UL,
                options: JsonSerializerOptions.Default);
        }

        // Assert
        Assert.Equal(
            expected: expectedData.ToArray(),
            actual: arrayBufferWriter.WrittenSpan.ToArray());
    }
}