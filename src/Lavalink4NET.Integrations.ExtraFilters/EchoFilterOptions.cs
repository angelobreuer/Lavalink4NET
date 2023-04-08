namespace Lavalink4NET.Integrations.ExtraFilters;

using System.Collections.Generic;
using System.Text.Json;
using Lavalink4NET.Filters;
using Lavalink4NET.Protocol.Models.Filters;

public sealed record class EchoFilterOptions(
    float? Delay = null,
    float? Decay = null) : IFilterOptions
{
    public bool IsDefault => this is { Delay: null, Decay: null, };

    public void Apply(ref PlayerFilterMapModel filterMap)
    {
        var additionalFilters = filterMap.AdditionalFilters is null
            ? new Dictionary<string, JsonElement>()
            : new Dictionary<string, JsonElement>(filterMap.AdditionalFilters);

        var model = new EchoFilterModel(Delay, Decay);

        additionalFilters["echo"] = JsonSerializer.SerializeToElement(
            value: model,
            jsonTypeInfo: FilterJsonSerializerContext.Default.EchoFilterModel);

        filterMap = filterMap with { AdditionalFilters = additionalFilters, };
    }
}
