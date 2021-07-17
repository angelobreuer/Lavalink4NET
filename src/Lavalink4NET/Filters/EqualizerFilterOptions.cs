namespace Lavalink4NET.Filters
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [JsonConverter(typeof(EqualizerFilterOptionsJsonConverter))]
    public sealed class EqualizerFilterOptions : IFilterOptions
    {
        public const string Name = "eqaulizer";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        public IReadOnlyList<EqualizerBand> Bands { get; set; } = Array.Empty<EqualizerBand>();
    }
}
