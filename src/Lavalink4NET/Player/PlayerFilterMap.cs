namespace Lavalink4NET.Player
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Lavalink4NET.Filters;
    using Lavalink4NET.Payloads.Player;

    public sealed class PlayerFilterMap
    {
        private readonly Dictionary<string, IFilterOptions> _filters;
        private readonly LavalinkPlayer _player;
        private bool _changesToCommit;

        internal PlayerFilterMap(LavalinkPlayer player)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _filters = new Dictionary<string, IFilterOptions>();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_changesToCommit)
            {
                return;
            }

            var payload = new PlayerFiltersPayload(
                guildId: _player.GuildId,
                filters: _filters);

            await _player.LavalinkSocket
                .SendPayloadAsync(payload)
                .ConfigureAwait(false);
        }

        public VolumeFilterOptions? Volume
        {
            get => this[VolumeFilterOptions.Name] as VolumeFilterOptions;
            set => this[VolumeFilterOptions.Name] = value;
        }

        public IFilterOptions? this[string name]
        {
            get
            {
                return _filters.TryGetValue(name, out var options) ? options : null;
            }

            set
            {
                if (value is null)
                {
                    if (_filters.Remove(name))
                    {
                        _changesToCommit = true;
                    }

                    return;
                }

                _filters[name] = value!;
                _changesToCommit = true;
            }
        }
    }
}
