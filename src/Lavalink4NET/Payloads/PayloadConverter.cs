/*
 *  File:   PayloadConverter.cs
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

namespace Lavalink4NET.Payloads
{
    using System;
    using Lavalink4NET.Payloads.Events;
    using Lavalink4NET.Payloads.Node;
    using Lavalink4NET.Payloads.Player;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     An utility class for converting lavalink payloads.
    /// </summary>
    internal static class PayloadConverter
    {
        /// <summary>
        ///     Gets the implementation type for the specified <paramref name="eventType"/>.
        /// </summary>
        /// <param name="eventType">the type of the event</param>
        /// <returns>the implementation type</returns>
        public static Type GetEventType(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.TrackStuck:
                    return typeof(TrackStuckEvent);

                case EventType.TrackEnd:
                    return typeof(TrackEndEvent);

                case EventType.TrackException:
                    return typeof(TrackExceptionEvent);

                case EventType.WebSocketClosedEvent:
                    return typeof(WebSocketClosedEvent);

                default:
                    throw new Exception("Invalid event type.");
            }
        }

        /// <summary>
        ///     Gets the implementation type for the specified <paramref name="opCode"/>.
        /// </summary>
        /// <param name="opCode">the operation code of the event</param>
        /// <returns>the implementation type</returns>
        public static Type GetPayloadType(OpCode opCode)
        {
            switch (opCode)
            {
                case OpCode.GuildVoiceUpdate:
                    return typeof(VoiceUpdatePayload);

                case OpCode.PlayerPause:
                    return typeof(PlayerPausePayload);

                case OpCode.PlayerSeek:
                    return typeof(PlayerSeekPayload);

                case OpCode.PlayerStop:
                    return typeof(PlayerStopPayload);

                case OpCode.PlayerPlay:
                    return typeof(PlayerPlayPayload);

                case OpCode.PlayerUpdate:
                    return typeof(PlayerUpdatePayload);

                case OpCode.NodeStats:
                    return typeof(StatsUpdatePayload);

                case OpCode.Event:
                    throw new Exception("Events are specially handled.");

                default:
                    throw new Exception("Invalid operation code.");
            }
        }

        /// <summary>
        ///     Reads a lavalink payload from the specified json.
        /// </summary>
        /// <param name="json">the json to deserialize to a payload</param>
        /// <returns>the deserialized payload</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="json"/> is blank.
        /// </exception>
        public static IPayload ReadPayload(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("The specified JSON is blank.", nameof(json));
            }

            var jObject = JsonConvert.DeserializeObject<JObject>(json);

            if (!jObject.TryGetValue("op", out var opCodeToken))
            {
                throw new Exception("Invalid JSON: Expected 'op' in json object.");
            }

            var opCode = opCodeToken.ToObject<OpCode>();

            if (opCode == OpCode.Event)
            {
                if (!jObject.TryGetValue("type", out var eventTypeToken))
                {
                    throw new Exception("Invalid JSON: Expected 'type' in json object.");
                }

                var eventType = eventTypeToken.ToObject<EventType>();
                return (IPayload)jObject.ToObject(GetEventType(eventType));
            }

            var payloadType = GetPayloadType(opCode);
            return (IPayload)jObject.ToObject(payloadType);
        }
    }
}