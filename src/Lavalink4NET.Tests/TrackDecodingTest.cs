/*
 *  File:   TrackDecodingTest.cs
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

using System.Buffers;
using Lavalink4NET.Decoding;
using Xunit;

/// <summary>
///     Contains tests for the <see cref="TrackDecoder"/> class.
/// </summary>
public sealed class TrackDecodingTest
{
    /// <summary>
    ///     Tests track decoding.
    /// </summary>
    /// <param name="base64">the base64 encoded track data</param>
    [Theory]
    [InlineData("QAAAjAIAJFZhbmNlIEpveSAtICdSaXB0aWRlJyBPZmZpY2lhbCBWaWRlbwAObXVzaHJvb212aWRlb3MAAAAAAAMgyAALdUpfMUhNQUdiNGsAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj11Sl8xSE1BR2I0awAHeW91dHViZQAAAAAAAAAA")]
    [InlineData("QAAAjQIAK1JpdGEgT3JhIC0gWW91ciBTb25nIChPZmZpY2lhbCBMeXJpYyBWaWRlbykACFJpdGEgT3JhAAAAAAACwwgAC2k5NU5sYjdraVBvAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9aTk1TmxiN2tpUG8AB3lvdXR1YmUAAAAAAAAAAA==")]
    [InlineData("QAAAmAIAMkx1a2FzIEdyYWhhbSAtIExvdmUgU29tZW9uZSBbT0ZGSUNJQUwgTVVTSUMgVklERU9dAAxMdWthcyBHcmFoYW0AAAAAAAOhsAALZE40NHhwSGpOeEUAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kTjQ0eHBIak54RQAHeW91dHViZQAAAAAAAAAA")]
    [InlineData("QAAAiwIAC0RlYXRoIG9mIE1lAApTQUlOVCBQSE5YAAAAAAAAdVgAKGh0dHBzOi8vd3d3Lm1ib3hkcml2ZS5jb20vc3RhcnRtdXNpYy5tcDMAAQAoaHR0cHM6Ly93d3cubWJveGRyaXZlLmNvbS9zdGFydG11c2ljLm1wMwAEaHR0cAADbXAzAAAAAAAAAAA=")]
    public void TestTrackDecoding(string base64)
    {
        // verify the header of the base64 encoded track
        var operationStatus = TrackDecoder.TryDecodeTrack(base64, out var track);
        Assert.Equal(OperationStatus.Done, operationStatus);
    }
}
