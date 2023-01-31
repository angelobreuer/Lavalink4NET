namespace Lavalink4NET.Player;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Filters;
using Lavalink4NET.Payloads;
using Lavalink4NET.Payloads.Player;

public sealed class PlayerFilterMap : FilterMapBase
{
    private readonly LavalinkPlayer _player;
    private bool _changesToCommit;

    internal PlayerFilterMap(LavalinkPlayer player)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public override IFilterOptions? this[string name]
    {
        get
        {
            return Filters.TryGetValue(name, out var options) ? options : null;
        }

        set
        {
            if (value is null)
            {
                if (Filters.Remove(name))
                {
                    _changesToCommit = true;
                }

                return;
            }

            Filters[name] = value!;
            _changesToCommit = true;
        }
    }

    public void Clear()
    {
        if (Filters.Count is 0)
        {
            return;
        }

        Filters.Clear();
        _changesToCommit = true;
    }

    public async Task CommitAsync(bool force = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_changesToCommit && !force)
        {
            return;
        }

        var payload = new PlayerFiltersPayload
        {
            GuildId = _player.GuildId,
            Filters = Filters.ToDictionary(x => x.Key, x => (object?)x.Value),
        };

        await _player.LavalinkSocket
            .SendPayloadAsync(OpCode.PlayerFilters, payload, forceSend: false, cancellationToken)
            .ConfigureAwait(false);
    }
}
