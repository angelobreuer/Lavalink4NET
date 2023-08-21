namespace Lavalink4NET.ExampleCustomPlayer;

public interface IVolumeService
{
    float GetVolume(ulong guildId);

    void SetVolume(ulong guildId, float volume);
}
