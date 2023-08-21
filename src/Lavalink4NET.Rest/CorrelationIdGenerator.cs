namespace Lavalink4NET.Rest;

using System;

internal static class CorrelationIdGenerator
{
    private static ReadOnlySpan<char> Base32Characters => "0123456789ABCDEFGHIJKLMNOPQRSTUV";
    private static long _lastId = DateTime.UtcNow.Ticks;

    public static string GetNextId() => GenerateId(Interlocked.Increment(ref _lastId));

    private static string GenerateId(long id) => string.Create(13, id, (buffer, value) =>
    {
        var base32Characters = Base32Characters;

        buffer[0] = base32Characters[(int)((value >> 60) & 31)];
        buffer[1] = base32Characters[(int)((value >> 55) & 31)];
        buffer[2] = base32Characters[(int)((value >> 50) & 31)];
        buffer[3] = base32Characters[(int)((value >> 45) & 31)];
        buffer[4] = base32Characters[(int)((value >> 40) & 31)];
        buffer[5] = base32Characters[(int)((value >> 35) & 31)];
        buffer[6] = base32Characters[(int)((value >> 30) & 31)];
        buffer[7] = base32Characters[(int)((value >> 25) & 31)];
        buffer[8] = base32Characters[(int)((value >> 20) & 31)];
        buffer[9] = base32Characters[(int)((value >> 15) & 31)];
        buffer[10] = base32Characters[(int)((value >> 10) & 31)];
        buffer[11] = base32Characters[(int)((value >> 5) & 31)];
        buffer[12] = base32Characters[(int)(value & 31)];
    });
}
