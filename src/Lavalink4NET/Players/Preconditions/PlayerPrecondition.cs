namespace Lavalink4NET.Players.Preconditions;

using System.Collections.Immutable;

public static class PlayerPrecondition
{
    public static IPlayerPrecondition Playing { get; } = new SimplePrecondition(PlayerState.Playing);

    public static IPlayerPrecondition NotPlaying { get; } = new SimplePrecondition(PlayerState.NotPlaying);

    public static IPlayerPrecondition Paused { get; } = new SimplePrecondition(PlayerState.Paused);

    public static IPlayerPrecondition NotPaused { get; } = new NotPausedPrecondition();

    public static IPlayerPrecondition QueueEmpty { get; } = new QueueEmptyPrecondition();

    public static IPlayerPrecondition QueueNotEmpty { get; } = new QueueNotEmptyPrecondition();

    public static IPlayerPrecondition HistoryEmpty { get; } = new HistoryEmptyPrecondition();

    public static IPlayerPrecondition HistoryNotEmpty { get; } = new HistoryNotEmptyPrecondition();

    public static IPlayerPrecondition Any(ImmutableArray<IPlayerPrecondition> preconditions)
        => new AggregateAnyPrecondition(preconditions);

    public static IPlayerPrecondition Any(params IPlayerPrecondition[] preconditions)
        => new AggregateAnyPrecondition(preconditions.ToImmutableArray());

    public static IPlayerPrecondition All(ImmutableArray<IPlayerPrecondition> preconditions)
        => new AggregateAllPrecondition(preconditions);

    public static IPlayerPrecondition All(params IPlayerPrecondition[] preconditions)
        => new AggregateAllPrecondition(preconditions.ToImmutableArray());

    public static IPlayerPrecondition Status(ImmutableArray<PlayerState> states)
        => new PlayerStatePrecondition(states);

    public static IPlayerPrecondition Status(params PlayerState[] states)
        => new PlayerStatePrecondition(states.ToImmutableArray());
}
