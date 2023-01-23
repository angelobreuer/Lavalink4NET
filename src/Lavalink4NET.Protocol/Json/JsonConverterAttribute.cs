namespace Lavalink4NET.Protocol;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

internal sealed class JsonConverterAttribute<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]  T> : JsonConverterAttribute
{
    public JsonConverterAttribute() 
        : base(typeof(T))
    {
    }
}