namespace Lavalink4NET.Rest.Entities.Tracks;

public enum StrictSearchBehavior : byte
{
    /// <summary>
    ///     Denotes that strict search will throw if a query is found that might be ambiguous.
    /// </summary>
    /// <remarks>
    ///     <example>
    ///         The following queries would throw an exception: 
    ///         <list type="bullet">
    ///             <item>`ytsearch:abc`</item>
    ///             <item>`spsearch:abc`</item>
    ///         </list>
    ///         
    ///         The following query would not throw an exception: 
    ///         <list type="bullet">
    ///             <item>`abc`</item>
    ///             <item>`https://example.com/test.mp3`</item>
    ///             <item>`https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://open.spotify.com/playlist/[...]`</item>
    ///         </list>
    ///     </example>
    ///     
    ///     <example>
    ///         The following queries will be converted to resolve as the following queries:
    ///         <list type="bullet">
    ///             <item>(Search Mode: `ytsearch`) `abc` -> `ytsearch:abc`;</item>
    ///             <item>(Search Mode: `&lt;none&gt;`) `abc` -> `abc`;</item>
    ///         </list>
    ///     </example>
    /// </remarks>
    Throw,

    /// <summary>
    ///     Denotes that if strict search would be triggered, the query will be prefixed with
    ///     the specified search mode to avoid ambiguous results.
    /// </summary>
    /// <remarks>
    ///     <example>
    ///         The following queries will be converted to resolve as the following queries (Search Mode here is ytsearch):
    ///         <list type="bullet">
    ///             <item>`abc` -> `ytsearch:abc`;</item>
    ///             <item>`PR: abc` -> `ytsearch:PR: abc`;</item>
    ///             <item>`ytsearch:abc` -> `ytsearch:ytsearch:abc`</item>
    ///             <item>`spsearch:abc` -> `ytsearch:spsearch:abc`</item>
    ///             <item>`https://example.com/test.mp3` -> `https://example.com/test.mp3`</item>
    ///             <item>`https://youtube.com/watch?v=[...]` -> `https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://open.spotify.com/playlist/[...]` -> `https://open.spotify.com/playlist/[...]`</item>
    ///         </list>
    ///     </example>
    /// </remarks>
    Resolve,

    /// <summary>
    ///     Denotes that strict search will throw if a query is found that might be not using the specified search mode.
    ///     <see cref="Implicit"/> always requires an explicit search mode to be specified.
    /// </summary>
    /// <remarks>
    ///     <example>
    ///         The following queries would throw an exception: 
    ///         <list type="bullet">
    ///             <item>`ytsearch:abc`</item>
    ///             <item>`spsearch:abc`</item>
    ///             <item>`a:abc`</item>
    ///             <item>`https://example.com/test.mp3`</item>
    ///             <item>`https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://open.spotify.com/playlist/[...]`</item>
    ///         </list>
    ///         
    ///         The following queries would not throw an exception: 
    ///         <list type="bullet">
    ///             <item>`abc`</item>
    ///             <item>`def`</item>
    ///         </list>
    ///     </example>
    /// </remarks>
    Implicit,

    /// <summary>
    ///     Denotes that if strict search would be triggered, the query will be prefixed with
    ///     the specified search mode to avoid ambiguous results.
    /// </summary>
    /// <remarks>
    ///     <example>
    ///         The following queries will be converted to resolve as the following queries (Search Mode here is `ytsearch`):
    ///         <list type="bullet">
    ///             <item>`abc` -> `ytsearch:abc`;</item>
    ///             <item>`PR: abc` -> `ytsearch:PR: abc`;</item>
    ///             <item>`ytsearch:abc` -> `ytsearch:ytsearch:abc`</item>
    ///             <item>`spsearch:abc` -> `ytsearch:spsearch:abc`</item>
    ///             <item>`https://example.com/test.mp3` -> `ytsearch:https://example.com/test.mp3`</item>
    ///             <item>`https://youtube.com/watch?v=[...]` -> `ytsearch:https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://youtube.com/watch?v=[...]` -> `ytsearch:https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://open.spotify.com/playlist/[...]` -> `ytsearch:https://open.spotify.com/playlist/[...]`</item>
    ///         </list>
    ///     </example>
    /// </remarks>
    Explicit,

    /// <summary>
    ///     Denotes that strict search will be disabled. If no search mode is specified, the query will be passed through. If a search
    ///     mode is specified and none set in the query, the query will be prefixed with the specified search mode, in case the query is not a direct URI.
    /// </summary>
    /// <remarks>
    ///     If you want to have full control over your queries, choose <see cref="Passthrough"/> and specify no search mode, in
    ///     that way Lavalink4NET will not process your query in any way.
    /// 
    ///     <example>
    ///         The following queries will be converted to resolve as the following queries (Search Mode here is `ytsearch`):
    ///         <list type="bullet">
    ///             <item>`abc` -> `ytsearch:abc`;</item>
    ///             <item>`PR: abc` -> `PR: abc`;</item>
    ///             <item>`ytsearch:abc` -> `ytsearch:abc`</item>
    ///             <item>`spsearch:abc` -> `spsearch:abc`</item>
    ///             <item>`https://example.com/test.mp3` -> `https://example.com/test.mp3`</item>
    ///             <item>`https://youtube.com/watch?v=[...]` -> `https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://youtube.com/watch?v=[...]` -> `https://youtube.com/watch?v=[...]`</item>
    ///             <item>`https://open.spotify.com/playlist/[...]` -> `https://open.spotify.com/playlist/[...]`</item>
    ///         </list>
    ///     </example>
    /// </remarks>
    Passthrough,
}
