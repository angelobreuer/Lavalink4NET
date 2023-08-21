namespace Lavalink4NET.Protocol.Requests;

using System.Text.Json.Serialization;

public sealed record class AddressUnmarkProperties
{
    [JsonRequired]
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
}
