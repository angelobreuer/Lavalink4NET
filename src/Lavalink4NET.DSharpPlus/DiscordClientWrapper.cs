namespace Lavalink4NET.DSharpPlus;

using System;
using global::DSharpPlus;
using global::DSharpPlus.Entities;

/// <summary>
///     A wrapper for the discord client from the "DSharpPlus" discord client library. (https://github.com/DSharpPlus/DSharpPlus)
/// </summary>
public class DiscordClientWrapper : DiscordClientWrapperBase, IDisposable
{
    private readonly DiscordClient _client;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
    /// </summary>
    /// <param name="client">the sharded discord client</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
    /// </exception>
    public DiscordClientWrapper(DiscordClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));

        _client.VoiceStateUpdated += OnVoiceStateUpdated;
        _client.VoiceServerUpdated += OnVoiceServerUpdated;
    }

    /// <inheritdoc/>
    public override int ShardCount
    {
        get
        {
            EnsureNotDisposed();
            return _client.ShardCount;
        }
    }

    /// <inheritdoc/>
    protected override DiscordUser CurrentUser
    {
        get
        {
            EnsureNotDisposed();
            return _client.CurrentUser;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the <see
    ///     cref="DiscordShardedClientWrapper"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <see langword="true"/> to release both managed and unmanaged resources; <see
    ///     langword="false"/> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _client.VoiceStateUpdated -= OnVoiceStateUpdated;
            _client.VoiceServerUpdated -= OnVoiceServerUpdated;
        }

        _disposed = true;
    }

    /// <inheritdoc/>
    protected override DiscordClient GetClient(ulong guildId) => _client;

    /// <summary>
    ///     Throws an <see cref="ObjectDisposedException"/> if the <see
    ///     cref="DiscordClientWrapper"/> is disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DiscordClientWrapper));
        }
    }
}
