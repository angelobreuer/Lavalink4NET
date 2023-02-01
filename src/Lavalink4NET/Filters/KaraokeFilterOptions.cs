namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models;

public sealed record class KaraokeFilterOptions(
    float? Level = null,
    float? MonoLevel = null,
    float? FilterBand = null,
    float? FilterWidth = null) : IFilterOptions
{
    public bool IsDefault => this is
    {
        Level: null or 0F,
        MonoLevel: null or 0F,
        FilterBand: null,
        FilterWidth: null,
    };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            Karaoke = new KaraokeFilterModel(
                Level: Level,
                MonoLevel: MonoLevel,
                FilterBand: FilterBand,
                FilterWidth: FilterWidth),
        };
    }
}
