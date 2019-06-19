namespace Lavalink4NET.Tests
{
    using System;
    using System.Threading.Tasks;
    using Lavalink4NET.Lyrics;
    using Xunit;

    public sealed class LyricsTest
    {
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