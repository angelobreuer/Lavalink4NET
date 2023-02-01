namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models;

public sealed record class RotationFilterOptions(float? Frequency = null) : IFilterOptions
{
    public bool IsDefault => Frequency is null;

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            Rotation = new RotationFilterModel(Frequency),
        };
    }
}
