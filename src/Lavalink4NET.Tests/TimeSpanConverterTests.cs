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