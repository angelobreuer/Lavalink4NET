/*
 *  File:   OpCode.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
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
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     The supported lavalink operation codes for payloads.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OpCode
    {
        /// <summary>
        ///     Provide an intercepted voice server update. This causes the server to connect to the
        ///     voice channel.
        /// </summary>
        [EnumMember(Value = "voiceUpdate")]
        GuildVoiceUpdate,

        /// <summary>
        ///     Cause the player to play a track.
        /// </summary>
        [EnumMember(Value = "play")]
        PlayerPlay,

        /// <summary>
        ///     Cause the player to stop.
        /// </summary>
        [EnumMember(Value = "stop")]
        PlayerStop,

        /// <summary>
        ///     Set player pause.
        /// </summary>
        [EnumMember(Value = "pause")]
        PlayerPause,

        /// <summary>
        ///     Make the player seek to a position of the track.
        /// </summary>
        [EnumMember(Value = "seek")]
        PlayerSeek,

        /// <summary>
        ///     Set player volume.
        /// </summary>
        [EnumMember(Value = "volume")]
        PlayerVolume,

        /// <summary>
        ///     Updates the player filters.
        /// </summary>
        [EnumMember(Value = "filters")]
        PlayerFilters,

        /// <summary>
        ///     Tell the server to potentially disconnect from the voice server and potentially
        ///     remove the player with all its data. This is useful if you want to move to a new node
        ///     for a voice connection. Calling this op does not affect voice state, and you can send
        ///     the same VOICE_SERVER_UPDATE to a new node.
        /// </summary>
        [EnumMember(Value = "destroy")]
        PlayerDestroy,

        /// <summary>
        ///     Configures resuming for the connection.
        /// </summary>
        [EnumMember(Value = "configureResuming")]
        ConfigureResuming,

        /// <summary>
        ///     Position information about a player.
        /// </summary>
        [EnumMember(Value = "playerUpdate")]
        PlayerUpdate,

        /// <summary>
        ///     A collection of stats sent every minute.
        /// </summary>
        [EnumMember(Value = "stats")]
        NodeStats,

        /// <summary>
        ///     An event was emitted.
        /// </summary>
        [EnumMember(Value = "event")]
        Event
    }
}