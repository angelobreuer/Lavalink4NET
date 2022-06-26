/*
 *  File:   DiscordClientWrapperTests.cs
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

namespace Lavalink4NET.Tests;

using System;
using Discord.WebSocket;
using Lavalink4NET.DiscordNet;
using Xunit;

public sealed class DiscordClientWrapperTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void TestConstructorThrowsIfInvalidShardCountPassed(int shards)
    {
        var client = new DiscordShardedClient();
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiscordClientWrapper(client, shards));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(100)]
    public void TestConstructorDoesNotThrowIfValidShardCountPassed(int shards)
    {
        var client = new DiscordShardedClient();
        _ = new DiscordClientWrapper(client, shards);
    }

    [Fact]
    public void TestUnavailableAccessThrows()
    {
        var client = new DiscordShardedClient();

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = new DiscordClientWrapper(client).ShardCount;
        });
    }
}
