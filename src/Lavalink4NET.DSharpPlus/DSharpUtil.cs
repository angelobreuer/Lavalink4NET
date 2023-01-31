namespace Lavalink4NET.DSharpPlus
{
    using System.Reflection;
    using global::DSharpPlus;
    using global::DSharpPlus.Entities;
    using global::DSharpPlus.EventArgs;
    using global::DSharpPlus.Net.WebSocket;

    /// <summary>
    ///     An utility for getting internal / private fields from DSharpPlus WebSocket Gateway Payloads.
    /// </summary>
    public static class DSharpUtil
    {
        /// <summary>
        ///     The internal "SessionId" property info in <see cref="DiscordVoiceState"/>.
        /// </summary>
        // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/Entities/Voice/DiscordVoiceState.cs#L95
        private static readonly PropertyInfo _sessionIdProperty = typeof(DiscordVoiceState)
            .GetProperty("SessionId", BindingFlags.NonPublic | BindingFlags.Instance);

     
        /// <summary>
        ///     The internal "_webSocketClient" field info in <see cref="DiscordClient"/>.
        /// </summary>
        // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/Clients/DiscordClient.WebSocket.cs#L54
        private static readonly FieldInfo _webSocketClientField = typeof(DiscordClient)
            .GetField("_webSocketClient", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        ///     Gets the internal "SessionId" property value of the specified <paramref name="voiceState"/>.
        /// </summary>
        /// <param name="voiceState">the instance</param>
        /// <returns>the "SessionId" value</returns>
        public static string GetSessionId(this DiscordVoiceState voiceState)
            => (string)_sessionIdProperty.GetValue(voiceState);


        /// <summary>
        ///     Gets the internal "_webSocketClient" field value of the specified <paramref name="client"/>.
        /// </summary>
        public static IWebSocketClient GetWebSocketClient(this DiscordClient client)
            => (IWebSocketClient)_webSocketClientField.GetValue(client);
    }
}
