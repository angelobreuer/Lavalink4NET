/*
 *  File:   DataInputReader.cs
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

namespace Lavalink4NET.Decoding
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Text;

    /// <summary>
    ///     A C# port for the DataInput stream (used to decode Lavalink tracks).
    /// </summary>
    public sealed class DataInputReader : IDisposable
    {
        private readonly bool _leaveOpen;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataInputReader"/> class.
        /// </summary>
        /// <param name="baseStream">the base stream to read from</param>
        /// <param name="endianness">the input stream endianness</param>
        /// <param name="leaveOpen">
        ///     a value indicating whether the stream should be left open when disposing
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="baseStream"/> is <see langword="null"/>.
        /// </exception>
        public DataInputReader(Stream baseStream, Endianness endianness = Endianness.BigEndian, bool leaveOpen = false)
        {
            BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            Endianness = endianness;

            _leaveOpen = leaveOpen;

            if (!baseStream.CanRead)
            {
                throw new ArgumentException("The specified baseStream is not read-able.", nameof(baseStream));
            }
        }

        /// <summary>
        ///     Gets the base stream to read from.
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        ///     Gets the source stream endianness.
        /// </summary>
        public Endianness Endianness { get; }

        /// <summary>
        ///     Reads bytes from the stream.
        /// </summary>
        /// <param name="count">the number of bytes to read</param>
        /// <returns>the read bytes</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of the <see cref="BaseStream"/> was reached.
        /// </exception>
        public byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];

            if (BaseStream.Read(buffer, 0, count) < count)
            {
                throw new EndOfStreamException("Reached end of stream while reading bytes.");
            }

            return buffer;
        }

        /// <summary>
        ///     Reads a <see cref="short"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public short ReadInt16() => Read(sizeof(short), BitConverter.ToInt16);

        /// <summary>
        ///     Reads a <see cref="int"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public int ReadInt32() => Read(sizeof(int), BitConverter.ToInt32);

        /// <summary>
        ///     Reads a <see cref="long"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public long ReadInt64() => Read(sizeof(long), BitConverter.ToInt64);

        /// <summary>
        ///     Reads a <see cref="float"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public float ReadSingle() => Read(sizeof(float), BitConverter.ToSingle);

        /// <summary>
        ///     Reads an <see cref="ushort"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public ushort ReadUInt16() => Read(sizeof(ushort), BitConverter.ToUInt16);

        /// <summary>
        ///     Reads an <see cref="uint"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public uint ReadUInt32() => Read(sizeof(uint), BitConverter.ToUInt32);

        /// <summary>
        ///     Reads an <see cref="ulong"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public ulong ReadUInt64() => Read(sizeof(ulong), BitConverter.ToUInt64);

        /// <summary>
        ///     Reads a <see cref="bool"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public bool ReadBoolean() => ReadBytes(1)[0] == 1;

        /// <summary>
        ///     Reads a <see cref="byte"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public byte ReadByte()
        {
            var b = BaseStream.ReadByte();

            if (b == -1)
            {
                throw new EndOfStreamException("Reached end of stream while reading single byte.");
            }

            return (byte)b;
        }

        /// <summary>
        ///     Reads a <see cref="sbyte"/> from the stream.
        /// </summary>
        /// <returns>the read value</returns>
        /// <exception cref="EndOfStreamException">
        ///     thrown if the end of stream was reached while reading the value
        /// </exception>
        public sbyte ReadSByte() => (sbyte)ReadByte();

        /// <summary>
        ///     Reads an UTF-8 string from the stream.
        /// </summary>
        /// <returns>the read string</returns>
        public string ReadString()
        {
            var length = ReadUInt16();
            var pooledBuffer = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                if (BaseStream.Read(pooledBuffer, 0, length) < length)
                {
                    throw new EndOfStreamException("Unexpected end of stream while reading string.");
                }

                return Encoding.UTF8.GetString(pooledBuffer, 0, length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pooledBuffer);
            }
        }

        private T Read<T>(int count, Func<byte[], int, T> converterFunc)
            => Read(count, converterFunc, EndianConverter.SystemEndianess);

        private T Read<T>(int count, Func<byte[], int, T> converterFunc, Endianness endianness)
        {
            var bytes = ReadBytes(count);
            EndianConverter.Convert(bytes, Endianness, endianness);
            return converterFunc(bytes, 0);
        }

        /// <summary>
        ///     Disposes the underlying <see cref="BaseStream"/>, if specified in constructor (!leaveOpen).
        /// </summary>
        public void Dispose()
        {
            if (!_leaveOpen)
            {
                BaseStream.Dispose();
            }
        }
    }
}