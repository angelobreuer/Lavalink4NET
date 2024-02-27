namespace Lavalink4NET.Integrations.SponsorBlock;

using System;

public sealed record class Chapter(
    string Name,
    TimeSpan Start,
    TimeSpan End,
    TimeSpan Duration);