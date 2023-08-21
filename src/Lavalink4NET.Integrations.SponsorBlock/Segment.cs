namespace Lavalink4NET.Integrations.SponsorBlock;

using System;

public readonly record struct Segment(
    SegmentCategory Category,
    TimeSpan StartOffset,
    TimeSpan EndOffset);