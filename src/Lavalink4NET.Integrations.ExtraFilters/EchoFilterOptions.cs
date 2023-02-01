namespace Lavalink4NET.Integrations.ExtraFilters;

using Lavalink4NET.Filters;
using Lavalink4NET.Protocol.Models;

public sealed record class EchoFilterOptions(
    float? Delay = null,
    float? Decay = null) : IFilterOptions
{
    public bool IsDefault => this is { Delay: null, Decay: null, };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        throw new System.NotImplementedException(); // TODO
    }
}
