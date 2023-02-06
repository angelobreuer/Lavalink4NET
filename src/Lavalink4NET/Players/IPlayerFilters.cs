namespace Lavalink4NET.Players;

using Lavalink4NET.Filters;

public interface IPlayerFilters
{
    ChannelMixFilterOptions? ChannelMix { get; set; }

    DistortionFilterOptions? Distortion { get; set; }

    EqualizerFilterOptions? Equalizer { get; set; }

    KaraokeFilterOptions? Karaoke { get; set; }

    LowPassFilterOptions? LowPass { get; set; }

    RotationFilterOptions? Rotation { get; set; }

    TimescaleFilterOptions? Timescale { get; set; }

    TremoloFilterOptions? Tremolo { get; set; }

    VibratoFilterOptions? Vibrato { get; set; }

    VolumeFilterOptions? Volume { get; set; }

    void Clear();

    T? GetFilter<T>() where T : IFilterOptions;

    T GetRequiredFilter<T>() where T : IFilterOptions;

    void SetFilter<T>(T? filterOptions) where T : IFilterOptions;

    bool TryRemove<T>() where T : IFilterOptions;
}
