namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models;

public sealed record class VibratoFilterOptions(
    float? Frequency = null,
    float? Depth = null) : IFilterOptions
{
    public bool IsDefault => this is { Frequency: null, Depth: null, };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            Vibrato = new VibratoFilterModel(Frequency, Depth),
        };
    }
}
