namespace Lavalink4NET;

using System;

/// <summary>
///     Represents the information for a discord voice server.
/// </summary>
public sealed class VoiceServer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VoiceServer"/> class.
    /// </summary>
    /// <param name="guildId">the guild snowflake identifier the update is for</param>
    /// <param name="token">
    ///     the voice server token that is required for connecting to the voice endpoint
    /// </param>
    /// <param name="endpoint">the address of the voice server to connect to</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="token"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="endpoint"/> is <see langword="null"/>.
    /// </exception>
    public VoiceServer(ulong guildId, string token, string endpoint)
    {
        GuildId = guildId;
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    /// <summary>
    ///     Gets the guild snowflake identifier the update is for.
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    ///     Gets the voice server token that is required for connecting to the voice endpoint.
    /// </summary>
    public string Token { get; }

    /// <summary>
    ///     Gets the address of the voice server to connect to.
    /// </summary>
    public string Endpoint { get; }
}
