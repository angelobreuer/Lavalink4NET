namespace Lavalink4NET.Rest;

using System;

internal sealed class LavalinkApiClientEndpoints
{
    public const int CurrentVersion = 3;

    public LavalinkApiClientEndpoints(Uri baseAddress, int version = CurrentVersion)
    {
        Uri Build(string relativeUri) => new(baseAddress, relativeUri);

        Version = Build("version");
        Statistics = Build($"v{version}/stats");
        Information = Build($"v{version}/info");
        Sessions = Build($"v{version}/sessions");
        LoadTracks = Build($"v{version}/loadtracks");
    }

    public Uri Version { get; }

    public Uri Sessions { get; }

    public Uri Statistics { get; }

    public Uri Information { get; }

    public Uri LoadTracks { get; }

    public Uri Player(string sessionId, ulong guildId)
    {
        // /v3/sessions/{sessionId}/players/{guildId}
        ArgumentNullException.ThrowIfNull(sessionId);

        var sessionIdValue = Uri.EscapeDataString(sessionId);
        var guildIdValue = Uri.EscapeDataString(guildId.ToString());

        return new Uri(Sessions, $"{sessionIdValue}/players/{guildIdValue}");
    }

    public Uri Players(string sessionId)
    {
        // /v3/sessions/{sessionId}/players
        ArgumentNullException.ThrowIfNull(sessionId);

        var sessionIdValue = Uri.EscapeDataString(sessionId);

        return new Uri(Sessions, $"{sessionIdValue}/players");
    }

    public Uri Session(string sessionId)
    {
        // /v3/sessions/{sessionId}
        ArgumentNullException.ThrowIfNull(sessionId);

        var sessionIdValue = Uri.EscapeDataString(sessionId);
        return new Uri(Sessions, $"{sessionIdValue}");
    }
}
