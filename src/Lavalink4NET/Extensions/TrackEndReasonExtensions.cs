namespace Lavalink4NET.Extensions;

using Lavalink4NET.Protocol.Payloads.Events;

public static class TrackEndReasonExtensions
{
    public static bool MayStartNext(this TrackEndReason endReason)
    {
        return endReason is TrackEndReason.Finished or TrackEndReason.LoadFailed;
    }
}
