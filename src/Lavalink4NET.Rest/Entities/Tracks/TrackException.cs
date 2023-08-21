namespace Lavalink4NET.Rest.Entities.Tracks;

using Lavalink4NET.Protocol.Models;

public readonly record struct TrackException(
    ExceptionSeverity Severity,
    string? Message,
    string? Cause);