namespace Lavalink4NET.Converters;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

internal abstract class StaticJsonStringEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    private static Dictionary<string, T>? _mapToEnumDictionary;
    private static Dictionary<T, string>? _mapToStringDictionary;

    protected internal StaticJsonStringEnumConverter()
    {
        if (_mapToEnumDictionary is null)
        {
            _mapToEnumDictionary = new Dictionary<string, T>();
            _mapToStringDictionary = new Dictionary<T, string>();

            RegisterMappings(new RegistrationContext(_mapToEnumDictionary, _mapToStringDictionary));
        }
    }

    /// <inheritdoc/>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!_mapToEnumDictionary!.TryGetValue(reader.GetString()!, out var value))
        {
            throw new JsonException($"Could not interpret the value '{reader.GetString()}'.");
        }

        return value;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (!_mapToStringDictionary!.TryGetValue(value, out var stringValue))
        {
            throw new JsonException($"Could not find a string representation for enum value {value}.");
        }

        writer.WriteStringValue(stringValue);
    }

    protected abstract void RegisterMappings(RegistrationContext registrationContext);

    protected readonly struct RegistrationContext
    {
        private readonly Dictionary<string, T> _mapToEnumDictionary;
        private readonly Dictionary<T, string> _mapToStringDictionary;

        public RegistrationContext(Dictionary<string, T> mapToEnumDictionary, Dictionary<T, string> mapToStringDictionary)
        {
            _mapToEnumDictionary = mapToEnumDictionary;
            _mapToStringDictionary = mapToStringDictionary;
        }

        public void Register(string key, T value)
        {
            _mapToEnumDictionary.Add(key, value);
            _mapToStringDictionary.Add(value, key);
        }
    }
}
