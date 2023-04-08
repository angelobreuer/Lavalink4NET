namespace Lavalink4NET.Players;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Lavalink4NET.Filters;
using Lavalink4NET.Protocol.Models.Filters;

internal sealed class PlayerFilterMap : IPlayerFilters
{
    private readonly Dictionary<Type, IFilterOptions> _filters;
    private int _dirtyState; // 0 = none, 1 = dirty

    public PlayerFilterMap()
    {
        _filters = new Dictionary<Type, IFilterOptions>();
    }

    internal PlayerFilterMapModel? BuildFilterMap()
    {
        var dirtyState = Interlocked.Exchange(ref _dirtyState, 0);

        if (dirtyState is 0)
        {
            return null;
        }

        var filterMap = new PlayerFilterMapModel();

        foreach (var (_, filterOptions) in _filters.Where(x => !x.Value.IsDefault))
        {
            filterOptions.Apply(ref filterMap);
        }

        return filterMap;
    }

    public ChannelMixFilterOptions? ChannelMix
    {
        get => GetFilter<ChannelMixFilterOptions>();
        set => SetFilter(value);
    }

    public DistortionFilterOptions? Distortion
    {
        get => GetFilter<DistortionFilterOptions>();
        set => SetFilter(value);
    }

    public EqualizerFilterOptions? Equalizer
    {
        get => GetFilter<EqualizerFilterOptions>();
        set => SetFilter(value);
    }

    public KaraokeFilterOptions? Karaoke
    {
        get => GetFilter<KaraokeFilterOptions>();
        set => SetFilter(value);
    }

    public LowPassFilterOptions? LowPass
    {
        get => GetFilter<LowPassFilterOptions>();
        set => SetFilter(value);
    }

    public RotationFilterOptions? Rotation
    {
        get => GetFilter<RotationFilterOptions>();
        set => SetFilter(value);
    }

    public TimescaleFilterOptions? Timescale
    {
        get => GetFilter<TimescaleFilterOptions>();
        set => SetFilter(value);
    }

    public TremoloFilterOptions? Tremolo
    {
        get => GetFilter<TremoloFilterOptions>();
        set => SetFilter(value);
    }

    public VibratoFilterOptions? Vibrato
    {
        get => GetFilter<VibratoFilterOptions>();
        set => SetFilter(value);
    }

    public VolumeFilterOptions? Volume
    {
        get => GetFilter<VolumeFilterOptions>();
        set => SetFilter(value);
    }

    public void Clear()
    {
        if (_filters.Count is 0)
        {
            return;
        }

        _filters.Clear();
        _dirtyState = 1;
    }

    public T? GetFilter<T>() where T : IFilterOptions
    {
        return (T?)_filters.GetValueOrDefault(typeof(T));
    }

    public T GetRequiredFilter<T>() where T : IFilterOptions
    {
        return (T)_filters[typeof(T)];
    }

    public void SetFilter<T>(T? filterOptions) where T : IFilterOptions
    {
        if (filterOptions is null)
        {
            if (_filters.Remove(typeof(T)))
            {
                _dirtyState = 1;
            }
        }
        else
        {
            ref var reference = ref CollectionsMarshal.GetValueRefOrAddDefault(
                dictionary: _filters,
                key: typeof(T),
                exists: out var exists);

            if (!exists || !ReferenceEquals(reference, filterOptions))
            {
                reference = filterOptions;
                _dirtyState = 1;
            }
        }
    }

    public bool TryRemove<T>() where T : IFilterOptions
    {
        if (_filters.Remove(typeof(T)))
        {
            _dirtyState = 1;
            return true;
        }

        return false;
    }
}
