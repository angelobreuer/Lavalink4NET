/*
 *  File:   PlayerDestroyPayload.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
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

using Newtonsoft.Json;

/// <summary>
///     The strongly-typed representation of a player destroy payload sent to the lavalink node
///     (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class PlayerDestroyPayload : IPlayerPayload
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerDestroyPayload"/> class.
    /// </summary>
    /// <param name="guildId">
    ///     the guild snowflake identifier where the player should be destroyed
    /// </param>
    public PlayerDestroyPayload(ulong guildId)
        => GuildId = guildId.ToString();

    /// <summary>
    ///     Gets the operation code for the payload.
    /// </summary>
    [JsonRequired, JsonProperty("op")]
    public OpCode OpCode => OpCode.PlayerDestroy;

    /// <summary>
    ///     Gets the guild snowflake identifier where the player should be destroyed.
    /// </summary>
    [JsonRequired, JsonProperty("guildId")]
    public string GuildId { get; internal set; } = null!;
}
