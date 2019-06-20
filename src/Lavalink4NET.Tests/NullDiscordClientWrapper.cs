using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Events;

namespace Lavalink4NET.Tests
{
    internal class NullDiscordClientWrapper : IDiscordClientWrapper
    {
        public ulong CurrentUserId => throw new NotImplementedException();

        public int ShardCount => throw new NotImplementedException();

        public event AsyncEventHandler<VoiceStateUpdateEventArgs> VoiceStateUpdated;

        public event AsyncEventHandler<VoiceServer> VoiceServerUpdated;

        public Task<IEnumerable<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId)
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false)
        {
            throw new NotImplementedException();
        }
    }
}