/*
 *  File:   TimeSpanConverterTests.cs
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

namespace Lavalink4NET.Tests
{
    using System;
    using Lavalink4NET.Util;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    ///     Contains tests for <see cref="TimeSpanConverter"/>.
    /// </summary>
    public sealed class TimeSpanConverterTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(5865151610)]
        public void TestConvert(long milliseconds)
        {
            var model = new { milliseconds };
            var payload = JsonConvert.SerializeObject(model);
            var actual = JsonConvert.DeserializeObject<TestModel>(payload).Milliseconds;

            Assert.Equal(milliseconds, actual.TotalMilliseconds);
        }

        [Theory]
        [InlineData(long.MaxValue)]
        public void TestConvertInfinite(long milliseconds)
        {
            var model = new { milliseconds };
            var payload = JsonConvert.SerializeObject(model);
            var actual = JsonConvert.DeserializeObject<TestModel>(payload).Milliseconds;

            Assert.Equal(TimeSpan.MaxValue, actual);
        }

        /// <summary>
        ///     A test JSON model for testing the JSON <see cref="TimeSpanConverter"/>.
        /// </summary>
        private sealed class TestModel
        {
            /// <summary>
            ///     Gets or sets the test value.
            /// </summary>
            [JsonConverter(typeof(TimeSpanConverter))]
            [JsonRequired, JsonProperty("milliseconds")]
            public TimeSpan Milliseconds { get; set; }
        }
    }
}
