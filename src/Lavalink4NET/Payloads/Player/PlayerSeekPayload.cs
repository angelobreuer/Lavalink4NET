/*
 *  File:   PlayerSeekPayload.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2021
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

namespace Lavalink4NET.Payloads.Player;

using System;
using Newtonsoft.Json;

/// <summary>
///     The strongly-typed representation of a player seek payload sent to the lavalink node (in
///     serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class PlayerSeekPayload : IPayload, IPlayerPayload
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerSeekPayload"/> class.
    /// </summary>
    /// <param name="guildId">the guild snowflake identifier the voice update is for</param>
    /// <param name="position">the seek position</param>
    public PlayerSeekPayload(ulong guildId, TimeSpan position)
    {
        GuildId = guildId.ToString();
        Position = (int)position.TotalMilliseconds;
    }

    /// <summary>
    ///     Gets the operation code for the payload.
    /// </summary>
    [JsonRequired, JsonProperty("op")]
    public OpCode OpCode => OpCode.PlayerSeek;

    /// <summary>
    ///     Gets the guild snowflake identifier the player update is for.
    /// </summary>
    [JsonRequired, JsonProperty("guildId")]
    public string GuildId { get; internal set; }

    /// <summary>
    ///     Gets a value indicating whether the player should be paused.
    /// </summary>
    [JsonRequired, JsonProperty("position")]
    public int Position { get; internal set; }
}
