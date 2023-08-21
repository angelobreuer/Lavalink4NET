namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

[JsonConverter(typeof(ExceptionSeverityJsonConverter))]
public enum ExceptionSeverity : byte
{
    Common,
    Suspicious,
    Fatal,
    Fault,
}
