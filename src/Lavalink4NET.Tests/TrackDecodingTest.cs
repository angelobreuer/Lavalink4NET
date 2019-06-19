namespace Lavalink4NET.Tests
{
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
        public void TestTrackDecoding(string base64)
        {
            // verify the header of the base64 encoded track
            TrackDecoder.DecodeTrack(base64, verify: true);
        }
    }
}