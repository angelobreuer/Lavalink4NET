namespace Lavalink4NET;

using Lavalink4NET.Clients;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Socket;

internal sealed record class LavalinkNodeServiceContext(
	IDiscordClientWrapper ClientWrapper,
	ILavalinkSocketFactory LavalinkSocketFactory,
	IIntegrationManager IntegrationManager,
	IPlayerManager PlayerManager,
	ILavalinkNodeListener NodeListener);
