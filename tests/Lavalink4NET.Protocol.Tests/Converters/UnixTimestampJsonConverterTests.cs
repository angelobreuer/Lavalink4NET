namespace Lavalink4NET.Protocol.Tests.Converters;

using System.Buffers;
using System.Text.Json;

public sealed class UnixTimestampJsonConverterTests
{
    [Fact]
    public void TestRead()
    {
        // Arrange
        var data = "1667857581613"u8;
        var utf8JsonReader = new Utf8JsonReader(data);
        var jsonConverter = new UnixTimestampJsonConverter();
        _ = utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(
            reader: ref utf8JsonReader,
            typeToConvert: typeof(DateTimeOffset),
            options: JsonSerializerOptions.Default);

        // Assert
        Assert.Equal(
            expected: DateTimeOffset.Parse("2022-11-07T21:46:21.6130000+00:00"),
            actual: result);
    }

    [Fact]
    public void TestWrite()
    {
        // Arrange
        var expectedData = "1667857581613"u8;
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var jsonConverter = new UnixTimestampJsonConverter();

        // Act
        using (var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter))
        {
            jsonConverter.Write(
                writer: utf8JsonWriter,
                value: DateTimeOffset.Parse("2022-11-07T21:46:21.6130000+00:00"),
                options: JsonSerializerOptions.Default);
        }

        // Assert
        Assert.Equal(
            expected: expectedData.ToArray(),
            actual: arrayBufferWriter.WrittenSpan.ToArray());
    }
}