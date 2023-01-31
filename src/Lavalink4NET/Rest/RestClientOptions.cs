namespace Lavalink4NET.Rest;

using System;

/// <summary>
///     An abstraction for the options for a RESTful HTTP client.
/// </summary>
public abstract class RestClientOptions
{
    /// <summary>
    ///     Gets or sets the time how long a request should be cached.
    /// </summary>
    /// <remarks>
    ///     Note higher time spans can cause more memory usage, but reduce the number of requests made.
    /// </remarks>
    /// <remarks>This property defaults to <c>TimeSpan.FromMinutes(5)</c>.</remarks>
    public TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Gets or sets a value indicating whether the HTTP client accepts compressed payloads.
    /// </summary>
    /// <remarks>This property defaults to <see langword="true"/>.</remarks>
    public bool Decompression { get; set; } = true;

    /// <summary>
    ///     Gets or sets the RESTful HTTP api endpoint.
    /// </summary>
    /// <remarks>This property defaults to <c>http://localhost:8080/</c>.</remarks>
    public abstract string RestUri { get; set; }

    /// <summary>
    ///     Gets or sets the user-agent for HTTP requests (use <see langword="null"/> to disable
    ///     the custom user-agent header).
    /// </summary>
    /// <remarks>This property defaults to <c>"Lavalink4NET"</c>.</remarks>
    public string UserAgent { get; set; } = "Lavalink4NET";
}
