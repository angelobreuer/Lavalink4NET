namespace Lavalink4NET.Rest;

/// <summary>
///     The options for a lavalink rest client ( <see cref="ILavalinkRestClient"/>).
/// </summary>
public class LavalinkRestOptions : RestClientOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether payload I/O (including rest) should be logged
    ///     to the logger (This should be only used for development)
    /// </summary>
    /// <remarks>This property defaults to <see langword="false"/>.</remarks>
    public bool DebugPayloads { get; set; } = false;

    /// <summary>
    ///     Gets or sets the Lavalink Node Password.
    /// </summary>
    /// <remarks>This property defaults to <c>"youshallnotpass"</c>.</remarks>
    public string Password { get; set; } = "youshallnotpass";

    /// <summary>
    ///     Gets or sets the Lavalink Node restful HTTP api URI.
    /// </summary>
    /// <remarks>This property defaults to <c>http://localhost:8080/</c>.</remarks>
    public override string RestUri { get; set; } = "http://localhost:8080/";
}
