namespace Lavalink4NET.Converters;

using Lavalink4NET.Player;

internal sealed class TrackEndReasonJsonConverter : StaticJsonStringEnumConverter<TrackEndReason>
{
    /// <inheritdoc/>
    protected override void RegisterMappings(RegistrationContext registrationContext)
    {
        registrationContext.Register("FINISHED", TrackEndReason.Finished);
        registrationContext.Register("LOAD_FAILED", TrackEndReason.LoadFailed);
        registrationContext.Register("STOPPED", TrackEndReason.Stopped);
        registrationContext.Register("REPLACED", TrackEndReason.Replaced);
        registrationContext.Register("CLEANUP", TrackEndReason.CleanUp);
    }
}
