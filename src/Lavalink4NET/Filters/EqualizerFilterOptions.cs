namespace Lavalink4NET.Filters;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[JsonConverter(typeof(EqualizerFilterOptionsJsonConverter))]
public sealed class EqualizerFilterOptions : IFilterOptions
{
    public const string Name = "equalizer";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    public IReadOnlyList<EqualizerBand> Bands { get; set; } = Array.Empty<EqualizerBand>();
}
