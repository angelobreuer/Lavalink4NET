namespace Lavalink4NET.SponsorBlock;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

internal sealed class SkipCategoriesCollection : ISkipCategories
{
    private readonly ISponsorBlockIntegration _integration;
    private HashSet<SegmentCategory>? _skipCategories;

    public SkipCategoriesCollection(ISponsorBlockIntegration integration)
    {
        _integration = integration ?? throw new ArgumentNullException(nameof(integration));
    }

    /// <inheritdoc/>
    public int Count => _skipCategories is null ? 0 : _skipCategories.Count;

    /// <inheritdoc/>
    public bool IsDefault => _skipCategories is null;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(SegmentCategory item)
    {
        _skipCategories ??= new HashSet<SegmentCategory>();
        _skipCategories.Add(item);
    }

    /// <inheritdoc/>
    public void AddAll()
    {
        _skipCategories ??= new HashSet<SegmentCategory>();
        _skipCategories.Add(SegmentCategory.Sponsor);
        _skipCategories.Add(SegmentCategory.SelfPromotion);
        _skipCategories.Add(SegmentCategory.Interaction);
        _skipCategories.Add(SegmentCategory.Intro);
        _skipCategories.Add(SegmentCategory.Outro);
        _skipCategories.Add(SegmentCategory.Preview);
        _skipCategories.Add(SegmentCategory.OfftopicMusic);
        _skipCategories.Add(SegmentCategory.Filler);
    }

    /// <inheritdoc/>
    public void AddRange(IEnumerable<SegmentCategory> categories)
    {
        _skipCategories ??= new HashSet<SegmentCategory>();

        foreach (var category in categories)
        {
            _skipCategories.Add(category);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _skipCategories ??= new HashSet<SegmentCategory>();
        _skipCategories.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(SegmentCategory item)
    {
        if (_skipCategories is null)
        {
            return false;
        }

        return _skipCategories.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(SegmentCategory[] array, int arrayIndex)
    {
        if (_skipCategories is null)
        {
            return;
        }

        _skipCategories.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<SegmentCategory> GetEnumerator()
    {
        if (_skipCategories is null)
        {
            return Enumerable.Empty<SegmentCategory>().GetEnumerator();
        }

        return ((IEnumerable<SegmentCategory>)_skipCategories).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(SegmentCategory item)
    {
        if (_skipCategories is null)
        {
            return false;
        }

        return _skipCategories.Remove(item);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _skipCategories = null;
    }

    /// <inheritdoc/>
    public ImmutableArray<SegmentCategory> Resolve()
    {
        return _skipCategories is null
            ? _integration.DefaultSkipCategories
            : _skipCategories.ToImmutableArray();
    }
}
