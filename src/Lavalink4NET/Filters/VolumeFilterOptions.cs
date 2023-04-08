namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models.Filters;

public sealed record class VolumeFilterOptions(float? Volume = null) : IFilterOptions
{
    public bool IsDefault => Volume is null or 1.0F;

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with { Volume = Volume, };
    }
}
