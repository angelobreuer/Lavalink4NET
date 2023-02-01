namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models;

public sealed record class ChannelMixFilterOptions(
    float? LeftToLeft = null,
    float? LeftToRight = null,
    float? RightToLeft = null,
    float? RightToRight = null) : IFilterOptions
{
    public bool IsDefault => this is
    {
        LeftToLeft: 1F or null,
        LeftToRight: 0F or null,
        RightToLeft: 0F or null,
        RightToRight: 1F or null,
    };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            ChannelMix = new ChannelMixFilterModel(
                LeftToLeft: LeftToLeft,
                LeftToRight: LeftToRight,
                RightToLeft: RightToLeft,
                RightToRight: RightToRight),
        };
    }
}
