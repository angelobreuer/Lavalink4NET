namespace Lavalink4NET.Tests
{
    using System.IO;
    using Lavalink4NET.Decoding;
    using Xunit;

    /// <summary>
    ///     Contains tests for load balancing strategies ( <see cref="DataInputReader"/>).
    /// </summary>
    public sealed class DataInputReaderTests
    {
        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(true, new byte[] { 1 })]
        [InlineData(false, new byte[] { 0 })]
        [InlineData(false, new byte[] { 20 })]
        [InlineData(false, new byte[] { 50 })]
        public void TestBool(bool expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadBoolean());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(0, new byte[] { 0, 0 })]
        [InlineData(12, new byte[] { 0, 12 })]
        [InlineData(3072, new byte[] { 12, 0 })]
        [InlineData(-1, new byte[] { 255, 255 })]
        public void TestInt16(short expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadInt16());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(0, new byte[] { 0, 0, 0, 0 })]
        [InlineData(12, new byte[] { 0, 0, 0, 12 })]
        [InlineData(3072, new byte[] { 0, 0, 12, 0 })]
        [InlineData(65535, new byte[] { 0, 0, 255, 255 })]
        public void TestInt32(int expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadInt32());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(12, new byte[] { 0, 0, 0, 0, 0, 0, 0, 12 })]
        [InlineData(3072, new byte[] { 0, 0, 0, 0, 0, 0, 12, 0 })]
        [InlineData(65535, new byte[] { 0, 0, 0, 0, 0, 0, 255, 255 })]
        public void TestInt64(long expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadInt64());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData((sbyte)0, new byte[] { 0 })]
        [InlineData((sbyte)50, new byte[] { 50 })]
        [InlineData((sbyte)-127, new byte[] { 129 })]
        public void TestInt8(sbyte expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadSByte());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(12, new byte[] { 0, 12 })]
        [InlineData(3072, new byte[] { 12, 0 })]
        [InlineData(ushort.MinValue, new byte[] { 0, 0 })]
        [InlineData(ushort.MaxValue, new byte[] { 255, 255 })]
        public void TestUInt16(ushort expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadUInt16());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(0, new byte[] { 0, 0, 0, 0 })]
        [InlineData(12, new byte[] { 0, 0, 0, 12 })]
        [InlineData(3072, new byte[] { 0, 0, 12, 0 })]
        [InlineData(65535, new byte[] { 0, 0, 255, 255 })]
        public void TestUInt32(uint expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadUInt32());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData(0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(12, new byte[] { 0, 0, 0, 0, 0, 0, 0, 12 })]
        [InlineData(3072, new byte[] { 0, 0, 0, 0, 0, 0, 12, 0 })]
        [InlineData(65535, new byte[] { 0, 0, 0, 0, 0, 0, 255, 255 })]
        public void TestUInt64(ulong expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadUInt64());
            }
        }

        /// <summary>
        ///     Tests whether the <paramref name="data"/> represents the <paramref name="expected"/> value.
        /// </summary>
        /// <param name="expected">the expected value</param>
        /// <param name="data">the data representation (big-endian)</param>
        [Theory]
        [InlineData((byte)8, new byte[] { 8 })]
        [InlineData((byte)96, new byte[] { 96 })]
        [InlineData(byte.MaxValue, new byte[] { byte.MaxValue })]
        [InlineData(byte.MinValue, new byte[] { byte.MinValue })]
        public void TestUInt8(byte expected, byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new DataInputReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(expected, reader.ReadByte());
            }
        }
    }
}