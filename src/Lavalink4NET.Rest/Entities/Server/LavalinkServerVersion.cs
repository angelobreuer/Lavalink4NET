namespace Lavalink4NET.Rest.Entities.Server;

using System;

public readonly record struct LavalinkServerVersion(Version Version, string? PreRelease);
