namespace Lavalink4NET.ExampleCustomPlayer;

using System.Collections.Concurrent;
using System.Collections.Generic;

internal sealed class VolumeService : IVolumeService
{
    private readonly ConcurrentDictionary<ulong, float> _volumeStore = new();

    public float GetVolume(ulong guildId)
    {
        return _volumeStore.GetValueOrDefault(guildId, 1F);
    }

    public void SetVolume(ulong guildId, float volume)
    {
        if (volume is 1F)
        {
            _volumeStore.Remove(guildId, out _);
        }
        else
        {
            _volumeStore[guildId] = volume;
        }
    }
}
