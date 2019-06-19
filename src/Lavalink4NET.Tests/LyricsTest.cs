namespace Lavalink4NET.Tests
{
    using System;
    using System.Threading.Tasks;
    using Lavalink4NET.Lyrics;
    using Xunit;

    /// <summary>
    ///     Contains test for the <see cref="LyricsService"/>.
    /// </summary>
    public sealed class LyricsTest
    {
        /// <summary>
        ///     Tests getting lyrics using the <see cref="LyricsService"/> asynchronously.
        /// </summary>
        /// <param name="artist">the track artist to query</param>
        /// <param name="title">the track title to query</param>
        /// <param name="keywords">
        ///     the lyrics keywords that the lyrics must contain to fulfill the test assertion
        /// </param>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Theory]
        [InlineData("Lukas Graham", "Love Someone", "I am scared", "love someone", "open up your heart")]
        [InlineData("Coldplay", "Adventure of a Lifetime", "your magic", "dream away", "We are diamonds", "I feel")]
        public async Task LyricsGetAsync(string artist, string title, params string[] keywords)
        {
            using (var service = new LyricsService(new LyricsOptions()))
            {
                var lyrics = await service.GetLyricsAsync(artist, title);

                foreach (var keyword in keywords)
                {
                    Assert.Contains(keyword, lyrics, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }
    }
}