namespace Lavalink4NET.Protocol.Tests;

using System.Buffers;
using System.Text.Json;
using Lavalink4NET.Protocol.Converters;

public sealed class NullableDurationJsonConverterTests
{
    [Fact]
    public void TestRead()
    {
        // Arrange
        var data = "1000"u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new NullableDurationJsonConverter();
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
        var jsonConverter = new NullableDurationJsonConverter();

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

    [Fact]
    public void TestReadNull()
    {
        // Arrange
        var data = "-1"u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new NullableDurationJsonConverter();
        _ = utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(
            reader: ref utf8JsonReader,
            typeToConvert: typeof(TimeSpan),
            options: JsonSerializerOptions.Default);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TestWriteNull()
    {
        // Arrange
        var expectedData = "-1"u8;
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var jsonConverter = new NullableDurationJsonConverter();

        // Act
        using (var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter))
        {
            jsonConverter.Write(
                writer: utf8JsonWriter,
                value: null,
                options: JsonSerializerOptions.Default);
        }

        // Assert
        Assert.Equal(
            expected: expectedData.ToArray(),
            actual: arrayBufferWriter.WrittenSpan.ToArray());
    }

    [Fact]
    public void TestWriteNegativeThrows()
    {
        // Arrange
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var jsonConverter = new NullableDurationJsonConverter();

        // Act
        void Action()
        {
            using var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter!);

            jsonConverter!.Write(
                writer: utf8JsonWriter,
                value: TimeSpan.FromSeconds(-1),
                options: JsonSerializerOptions.Default);
        }

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Action);
    }
}