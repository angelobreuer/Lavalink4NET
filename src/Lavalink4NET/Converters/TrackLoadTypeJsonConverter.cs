namespace Lavalink4NET.Converters;

using Lavalink4NET.Rest;

internal sealed class TrackLoadTypeJsonConverter : StaticJsonStringEnumConverter<TrackLoadType>
{
    /// <inheritdoc/>
    protected override void RegisterMappings(RegistrationContext registrationContext)
    {
        registrationContext.Register("TRACK_LOADED", TrackLoadType.TrackLoaded);
        registrationContext.Register("PLAYLIST_LOADED", TrackLoadType.PlaylistLoaded);
        registrationContext.Register("SEARCH_RESULT", TrackLoadType.SearchResult);
        registrationContext.Register("NO_MATCHES", TrackLoadType.NoMatches);
        registrationContext.Register("LOAD_FAILED", TrackLoadType.LoadFailed);
    }
}
