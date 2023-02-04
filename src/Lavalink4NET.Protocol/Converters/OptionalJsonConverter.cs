namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class OptionalJsonConverter<TModel, TConverter> : JsonConverter<Optional<TModel>> where TConverter : JsonConverter<TModel>, new()
{
    private static readonly TConverter _converter = new();

    public override bool HandleNull => true;

    public override Optional<TModel> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        return new Optional<TModel>(_converter.Read(ref reader, typeToConvert, options)!);
    }

    public override void Write(Utf8JsonWriter writer, Optional<TModel> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        if (!value.IsPresent)
        {
            OptionalJsonConverter.ThrowAttemptToSerializeNonPresent();
        }

        _converter.Write(writer, value.Value, options);
    }
}

internal sealed class OptionalJsonConverter<TModel> : JsonConverter<Optional<TModel>>
{
    public override bool HandleNull => true;

    public override Optional<TModel> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        return new Optional<TModel>(JsonSerializer.Deserialize<TModel>(ref reader, options)!);
    }

    public override void Write(Utf8JsonWriter writer, Optional<TModel> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        if (!value.IsPresent)
        {
            OptionalJsonConverter.ThrowAttemptToSerializeNonPresent();
        }

        JsonSerializer.Serialize(writer, value.Value, typeof(TModel), options);
    }
}

file static class OptionalJsonConverter
{
    public static void ThrowAttemptToSerializeNonPresent()
    {
        throw new InvalidOperationException("An Optional JSON property must have the JsonIgnore condition set to WhenWritingDefault.");
    }
}