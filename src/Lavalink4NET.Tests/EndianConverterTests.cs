namespace Lavalink4NET.Tests
{
    using Lavalink4NET.Decoding;
    using Xunit;

    /// <summary>
    ///     Contains tests for load balancing strategies ( <see cref="EndianConverter"/>).
    /// </summary>
    public sealed class EndianConverterTests
    {
        /// <summary>
        ///     Tests the endian converter endian conversion.
        /// </summary>
        /// <param name="source">the source data array</param>
        /// <param name="expect">the expected data array</param>
        /// <param name="sourceEndianness">
        ///     the source endianness of the specified <paramref name="source"/> data array
        /// </param>
        /// <param name="targetEndianness">
        ///     the target endianness of the specified <paramref name="expect"/> data array
        /// </param>
        [Theory]
        [InlineData(new byte[] { 1, 2 }, new byte[] { 1, 2 }, Endianness.BigEndian, Endianness.BigEndian)]
        [InlineData(new byte[] { 1, 2 }, new byte[] { 2, 1 }, Endianness.BigEndian, Endianness.LittleEndian)]
        [InlineData(new byte[] { 2, 1 }, new byte[] { 1, 2 }, Endianness.BigEndian, Endianness.LittleEndian)]
        [InlineData(new byte[] { 1, 2 }, new byte[] { 1, 2 }, Endianness.LittleEndian, Endianness.LittleEndian)]
        public void TestConvert(byte[] source, byte[] expect, Endianness sourceEndianness, Endianness targetEndianness)
        {
            // swap endian of the source
            EndianConverter.Convert(source, sourceEndianness, targetEndianness);

            // compare source -> swapped and expected array
            Assert.Equal(source, expect);
        }
    }
}