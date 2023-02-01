namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models;

public interface IFilterOptions
{
    bool IsDefault { get; }

    void Apply(ref PlayerFilterMapModel filterMap);
}
