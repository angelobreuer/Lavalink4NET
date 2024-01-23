namespace Lavalink4NET.DSharpPlus;

using System.Reflection;
using global::DSharpPlus.Entities;

/// <summary>
///     An utility for getting internal / private fields from DSharpPlus WebSocket Gateway Payloads.
/// </summary>
public static partial class DSharpPlusUtilities
{
    /// <summary>
    ///     The internal "SessionId" property info in <see cref="DiscordVoiceState"/>.
    /// </summary>
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/Entities/Voice/DiscordVoiceState.cs#L95
    private static readonly PropertyInfo _sessionIdProperty = typeof(DiscordVoiceState)
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        .GetProperty("SessionId", BindingFlags.NonPublic | BindingFlags.Instance)!;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    /// <summary>
    ///     Gets the internal "SessionId" property value of the specified <paramref name="voiceState"/>.
    /// </summary>
    /// <param name="voiceState">the instance</param>
    /// <returns>the "SessionId" value</returns>
    public static string GetSessionId(this DiscordVoiceState voiceState)
        => (string)_sessionIdProperty.GetValue(voiceState)!;
}
