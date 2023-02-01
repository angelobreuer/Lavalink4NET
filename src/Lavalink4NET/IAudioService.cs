namespace Lavalink4NET;

using System;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;

/// <summary>
///     The interface for a lavalink audio provider service.
/// </summary>
public interface IAudioService : IDisposable
{
    IIntegrationCollection Integrations { get; }

    IPlayerManager Players { get; }
}
