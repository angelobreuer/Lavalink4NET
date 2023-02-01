namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models;

public sealed record class DistortionFilterOptions(
    float? SinOffset = null,
    float? SinScale = null,
    float? CosOffset = null,
    float? CosScale = null,
    float? TanOffset = null,
    float? TanScale = null,
    float? Offset = null,
    float? Scale = null) : IFilterOptions
{
    public bool IsDefault => this is
    {
        SinOffset: null,
        SinScale: null,
        CosOffset: null,
        CosScale: null,
        TanOffset: null,
        TanScale: null,
        Offset: null,
        Scale: null,
    };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            Distortion = new DistortionFilterModel(
                SinOffset: SinOffset,
                SinScale: SinScale,
                CosOffset: CosOffset,
                CosScale: CosScale,
                TanOffset: TanOffset,
                TanScale: TanScale,
                Offset: Offset,
                Scale: Scale),
        };
    }
}
