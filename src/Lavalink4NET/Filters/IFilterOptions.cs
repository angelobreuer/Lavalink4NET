namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models.Filters;

public interface IFilterOptions
{
    bool IsDefault { get; }

    void Apply(ref PlayerFilterMapModel filterMap);
}
