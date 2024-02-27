namespace Lavalink4NET.Integrations.SponsorBlock;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock.Payloads;

[JsonSerializable(typeof(ImmutableArray<SegmentCategory>))]
[JsonSerializable(typeof(SegmentsLoadedEventPayload))]
[JsonSerializable(typeof(SegmentSkippedEventPayload))]
[JsonSerializable(typeof(ChaptersLoadedEventPayload))]
[JsonSerializable(typeof(ChapterStartedEventPayload))]
internal sealed partial class SponsorBlockJsonSerializerContext : JsonSerializerContext
{
}
