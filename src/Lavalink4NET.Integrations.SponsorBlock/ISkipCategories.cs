namespace Lavalink4NET.Integrations.SponsorBlock;

using System.Collections.Generic;
using System.Collections.Immutable;

public interface ISkipCategories : ICollection<SegmentCategory>
{
    /// <summary>
    ///     Gets a value indicating whether no skip categories has been configured and the default skip categories will be used.
    /// </summary>
    bool IsDefault { get; }

    void AddRange(IEnumerable<SegmentCategory> categories);

    void AddAll();

    void Reset();

    /// <summary>
    ///     Resolves the actual categories to be skipped including the default categories.
    /// </summary>
    ImmutableArray<SegmentCategory> Resolve();
}
