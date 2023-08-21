namespace Lavalink4NET.Lyrics;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;

public interface ILyricsService
{
    ValueTask<string?> GetLyricsAsync(LavalinkTrack track, CancellationToken cancellationToken = default);

    ValueTask<string?> GetLyricsAsync(string artist, string title, CancellationToken cancellationToken = default);
}