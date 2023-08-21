namespace Lavalink4NET.Filters;

using Lavalink4NET.Protocol.Models.Filters;

public sealed record class TimescaleFilterOptions(
    float? Speed = null,
    float? Pitch = null,
    float? Rate = null) : IFilterOptions
{
    public bool IsDefault => this is
    {
        Speed: null or 1.0F,
        Pitch: null or 1.0F,
        Rate: null or 1.0F,
    };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        filterMap = filterMap with
        {
            Timescale = new TimescaleFilterModel(Speed, Pitch, Rate),
        };
    }
}
