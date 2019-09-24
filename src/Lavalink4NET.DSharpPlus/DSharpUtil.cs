/*
 *  File:   DSharpUtil.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2019
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

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
        // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/Entities/DiscordVoiceState.cs#L70
        private static readonly PropertyInfo _sessionIdProperty = typeof(DiscordVoiceState)
            .GetProperty("SessionId", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        ///     The internal "VoiceToken" property info in <see cref="VoiceServerUpdateEventArgs"/>.
        /// </summary>
        // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/EventArgs/VoiceServerUpdateEventArgs.cs#L24
        private static readonly PropertyInfo _voiceTokenProperty = typeof(VoiceServerUpdateEventArgs)
            .GetProperty("VoiceToken", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        ///     The internal "_webSocketClient" field info in <see cref="value"/>.
        /// </summary>
        // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/DiscordClient.cs#L39
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
        ///     Gets the "VoiceToken" property value of the specified <paramref name="voiceServerUpdateEventArgs"/>.
        /// </summary>
        /// <param name="voiceServerUpdateEventArgs">the instance</param>
        /// <returns>the "VoiceToken" value</returns>
        public static string GetVoiceToken(this VoiceServerUpdateEventArgs voiceServerUpdateEventArgs)
            => (string)_voiceTokenProperty.GetValue(voiceServerUpdateEventArgs);

        /// <summary>
        ///     Gets the internal "_webSocketClient" field value of the specified <paramref name="client"/>.
        /// </summary>
        public static BaseWebSocketClient GetWebSocketClient(this DiscordClient client)
            => (BaseWebSocketClient)_webSocketClientField.GetValue(client);
    }
}