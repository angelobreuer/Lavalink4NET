namespace Lavalink4NET.Players;

using System.Collections.Immutable;
using Lavalink4NET.Clients;
using Lavalink4NET.Players.Preconditions;

public readonly record struct PlayerRetrieveOptions(
    PlayerChannelBehavior ChannelBehavior = PlayerChannelBehavior.None,
    MemberVoiceStateBehavior VoiceStateBehavior = MemberVoiceStateBehavior.Ignore,
    ImmutableArray<IPlayerPrecondition> Preconditions = default);