namespace Lavalink4NET.Filters;

using System;
using System.Collections.Immutable;
using Lavalink4NET.Protocol.Models.Filters;

public sealed record class EqualizerFilterOptions(Equalizer Equalizer) : IFilterOptions
{
    public bool IsDefault
    {
        get
        {
            for (var index = 0; index < Equalizer.Bands; index++)
            {
                if (Equalizer[index] is not 0.0F)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        ArgumentNullException.ThrowIfNull(filterMap);

        var equalizer = ImmutableArray.CreateBuilder<EqualizerBandModel>();

        for (var index = 0; index < Equalizer.Bands; index++)
        {
            var gain = Equalizer[index];

            if (gain is not 0.0F)
            {
                equalizer.Add(new EqualizerBandModel(index, gain));
            }
        }

        filterMap = filterMap with { Equalizer = equalizer.ToImmutable(), };
    }
}
