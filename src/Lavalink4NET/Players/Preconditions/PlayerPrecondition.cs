namespace Lavalink4NET.Players.Preconditions;

public static class PlayerPrecondition
{
    public static IPlayerPrecondition Playing { get; } = new SimplePrecondition(PlayerState.Playing);

    public static IPlayerPrecondition NotPlaying { get; } = new SimplePrecondition(PlayerState.NotPlaying);

    public static IPlayerPrecondition Paused { get; } = new SimplePrecondition(PlayerState.Paused);

    public static IPlayerPrecondition NotPaused { get; } = new NotPausedPrecondition();

    public static IPlayerPrecondition QueueEmpty { get; } = new QueueEmptyPrecondition();

    public static IPlayerPrecondition QueueNotEmpty { get; } = new QueueNotEmptyPrecondition();
}
