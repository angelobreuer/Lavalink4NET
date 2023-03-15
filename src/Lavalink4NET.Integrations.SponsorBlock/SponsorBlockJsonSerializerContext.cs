namespace Lavalink4NET.Integrations.SponsorBlock;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(ImmutableArray<SegmentCategory>))]
internal sealed partial class SponsorBlockJsonSerializerContext : JsonSerializerContext
{
}
