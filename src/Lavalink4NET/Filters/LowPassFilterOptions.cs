namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models.Filters;

public sealed record class LowPassFilterOptions(float? Smoothing) : IFilterOptions
{
    public bool IsDefault => Smoothing is null or <= 1.0F;

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            LowPass = new LowPassFilterModel(Smoothing),
        };
    }
}
