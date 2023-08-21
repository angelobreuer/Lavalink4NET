namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models.Filters;

public sealed record class TremoloFilterOptions(
    float? Frequency = null,
    float? Depth = null) : IFilterOptions
{
    public bool IsDefault => this is { Frequency: null, Depth: null, };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            Tremolo = new TremoloFilterModel(
                Frequency: Frequency,
                Depth: Depth),
        };
    }
}
