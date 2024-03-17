namespace Lavalink4NET.Rest.Tests;

using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Protocol.Responses;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

public class LavalinkApiClientTests
{
    [Theory]

    // Throw
    [InlineData(StrictSearchBehavior.Throw, "abc", "ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Throw, "abc", "scsearch:abc", "scsearch")]
    [InlineData(StrictSearchBehavior.Throw, "abc", "abc", null)]
    [InlineData(StrictSearchBehavior.Throw, "https://youtube.com/", "https://youtube.com/", null)]
    [InlineData(StrictSearchBehavior.Throw, "other search:abc", "othersearch:other search:abc", "othersearch")]
    [InlineData(StrictSearchBehavior.Throw, "ABC https://www.youtube.com/watch?v=ABC&t=248", "ytsearch:ABC https://www.youtube.com/watch?v=ABC&t=248", "ytsearch")]

    // Resolve
    [InlineData(StrictSearchBehavior.Resolve, "abc", "ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "PR: abc", "ytsearch:PR: abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "ytsearch:abc", "ytsearch:ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "spsearch:abc", "ytsearch:spsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "https://example.com/test.mp3", "https://example.com/test.mp3", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "https://youtube.com/watch?v=abc", "https://youtube.com/watch?v=abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "https://open.spotify.com/playlist/abc", "https://open.spotify.com/playlist/abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "ftp://example.com", "ytsearch:ftp://example.com", "ytsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "abc", "spsearch:abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "PR: abc", "spsearch:PR: abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "ytsearch:abc", "spsearch:ytsearch:abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "spsearch:abc", "spsearch:spsearch:abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "https://example.com/test.mp3", "https://example.com/test.mp3", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "https://youtube.com/watch?v=abc", "https://youtube.com/watch?v=abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "https://open.spotify.com/playlist/abc", "https://open.spotify.com/playlist/abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Resolve, "ftp://example.com", "spsearch:ftp://example.com", "spsearch")]

    // Implicit
    [InlineData(StrictSearchBehavior.Implicit, "abc", "spsearch:abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "def", "spsearch:def", "spsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "abc", "ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "def", "ytsearch:def", "ytsearch")]

    // Explicit
    [InlineData(StrictSearchBehavior.Explicit, "abc", "spsearch:abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "def", "spsearch:def", "spsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "abc", "ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "def", "ytsearch:def", "ytsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "https://example.com/test.mp3", "spsearch:https://example.com/test.mp3", "spsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "https://youtube.com/watch?v=abc", "spsearch:https://youtube.com/watch?v=abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "https://open.spotify.com/playlist/abc", "spsearch:https://open.spotify.com/playlist/abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Explicit, "ftp://example.com", "spsearch:ftp://example.com", "spsearch")]

    // Passthrough
    [InlineData(StrictSearchBehavior.Passthrough, "abc", "abc", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "ytsearch:abc", "ytsearch:abc", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "scsearch:abc", "scsearch:abc", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "othersearch:abc", "othersearch:abc", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "abc", "ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "abc", "scsearch:abc", "scsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://youtube.com/", "https://youtube.com/", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "other search:abc", "othersearch:other search:abc", "othersearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "ABC https://www.youtube.com/watch?v=ABC&t=248", "ytsearch:ABC https://www.youtube.com/watch?v=ABC&t=248", "ytsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://example.com/test.mp3", "https://example.com/test.mp3", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "https://youtube.com/watch?v=abc", "https://youtube.com/watch?v=abc", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "https://open.spotify.com/playlist/abc", "https://open.spotify.com/playlist/abc", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "ftp://example.com", "ftp://example.com", null)]
    [InlineData(StrictSearchBehavior.Passthrough, "https://example.com/test.mp3", "https://example.com/test.mp3", "ytsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://youtube.com/watch?v=abc", "https://youtube.com/watch?v=abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://open.spotify.com/playlist/abc", "https://open.spotify.com/playlist/abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "ftp://example.com", "ftp://example.com", "ytsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://example.com/test.mp3", "https://example.com/test.mp3", "spsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://youtube.com/watch?v=abc", "https://youtube.com/watch?v=abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "https://open.spotify.com/playlist/abc", "https://open.spotify.com/playlist/abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Passthrough, "ftp://example.com", "ftp://example.com", "spsearch")]
    public void TestBuildIdentifier(StrictSearchBehavior searchBehavior, string identifier, string expected, string searchMode)
    {
        // Arrange
        var loadOptions = new TrackLoadOptions(
            SearchMode: new TrackSearchMode(searchMode),
            SearchBehavior: searchBehavior);

        // Act
        var actual = LavalinkApiClient.BuildIdentifier(identifier, loadOptions);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]

    // Throw
    [InlineData(StrictSearchBehavior.Throw, "ytsearch:abc", null)]
    [InlineData(StrictSearchBehavior.Throw, "scsearch:abc", null)]
    [InlineData(StrictSearchBehavior.Throw, "othersearch:abc", null)]
    [InlineData(StrictSearchBehavior.Throw, "ytsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Throw, "scsearch:abc", "scsearch")]
    [InlineData(StrictSearchBehavior.Throw, "scsearch:abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Throw, "ftp://example.com/", "ytsearch")]

    // Implicit
    [InlineData(StrictSearchBehavior.Implicit, "abc", null)]
    [InlineData(StrictSearchBehavior.Implicit, "", null)]
    [InlineData(StrictSearchBehavior.Implicit, "https://example.com/test.mp3", "ytsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "https://youtube.com/watch?v=abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "https://open.spotify.com/playlist/abc", "ytsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "https://example.com/test.mp3", "spsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "https://youtube.com/watch?v=abc", "spsearch")]
    [InlineData(StrictSearchBehavior.Implicit, "https://open.spotify.com/playlist/abc", "spsearch")]
    public void TestBuildIdentifierFail(StrictSearchBehavior searchBehavior, string identifier, string? searchMode)
    {
        // Arrange
        var loadOptions = new TrackLoadOptions(
            SearchMode: new TrackSearchMode(searchMode),
            SearchBehavior: searchBehavior);

        // Act
        void Action() => LavalinkApiClient.BuildIdentifier(identifier, loadOptions);

        // Assert
        Assert.ThrowsAny<Exception>(Action);
    }

    [Fact]
    public async Task TestRetrieveVersion()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/version", (HttpContext context) =>
        {
            return TypedResults.Text("1.0.0");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var version = await client
            .RetrieveVersionAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Equal("1.0.0", version);
    }

    [Fact]
    public async Task TestRetrieveInformation()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/info", (HttpContext context) =>
        {
            return TypedResults.Text("""
                {
                  "version": {
                    "semver": "3.7.0-rc.1+test",
                    "major": 3,
                    "minor": 7,
                    "patch": 0,
                    "preRelease": "rc.1",
                    "build": "test"
                  },
                  "buildTime": 1664223916812,
                  "git": {
                    "branch": "master",
                    "commit": "85c5ab5",
                    "commitTime": 1664223916812
                  },
                  "jvm": "18.0.2.1",
                  "lavaplayer": "1.3.98.4-original",
                  "sourceManagers": [
                    "youtube",
                    "soundcloud"
                  ],
                  "filters": [
                    "equalizer",
                    "karaoke",
                    "timescale",
                    "channelMix"
                  ],
                  "plugins": [
                    {
                      "name": "some-plugin",
                      "version": "1.0.0"
                    },
                    {
                      "name": "foo-plugin",
                      "version": "1.2.3"
                    }
                  ]
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        _ = await client
            .RetrieveServerInformationAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestRetrieveStatistics()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/stats", (HttpContext context) =>
        {
            return TypedResults.Text("""
                {
                  "players": 1,
                  "playingPlayers": 1,
                  "uptime": 123456789,
                  "memory": {
                    "free": 123456789,
                    "used": 123456789,
                    "allocated": 123456789,
                    "reservable": 123456789
                  },
                  "cpu": {
                    "cores": 4,
                    "systemLoad": 0.5,
                    "lavalinkLoad": 0.5
                  }
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        _ = await client
            .RetrieveStatisticsAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestLoadTrackAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/loadtracks", (HttpContext context) =>
        {
            Assert.Equal("dQw4w9WgXcQ", context.Request.Query["identifier"]);

            return TypedResults.Text("""
                {
                    "loadType": "track",
                    "data": {
                        "encoded": "QAAA3wMAPFJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAgKE9mZmljaWFsIE11c2ljIFZpZGVvKQALUmljayBBc3RsZXkAAAAAAAM8IAALZFF3NHc5V2dYY1EAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kUXc0dzlXZ1hjUQEAOmh0dHBzOi8vaS55dGltZy5jb20vdmlfd2VicC9kUXc0dzlXZ1hjUS9tYXhyZXNkZWZhdWx0LndlYnAAAAd5b3V0dWJlAAAAAAAAAAA=",
                        "info": {
                            "identifier": "dQw4w9WgXcQ",
                            "isSeekable": true,
                            "author": "Rick Astley",
                            "length": 212000,
                            "isStream": false,
                            "position": 0,
                            "title": "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                            "uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                            "sourceName": "youtube",
                            "artworkUrl": "https://i.ytimg.com/vi_webp/dQw4w9WgXcQ/maxresdefault.webp",
                            "isrc": null
                        },
                        "pluginInfo": {}
                    }
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var track = await client
            .LoadTrackAsync("dQw4w9WgXcQ")
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(track);
        Assert.Contains("Rick Astley", track.Title);
    }

    [Fact]
    public async Task TestLoadPlaylistAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/loadtracks", (HttpContext context) =>
        {
            Assert.Equal("https://www.youtube.com/playlist?list=PLbpi6ZahtOH6NHu2SGpjLqW_3mYg3S3hw", context.Request.Query["identifier"]);

            return TypedResults.Text("""
                {
                	"loadType": "playlist",
                	"data": {
                		"info": {
                			"name": "Top Songs of 2022",
                			"selectedTrack": -1
                		},
                		"pluginInfo": {},
                		"tracks": [
                			{
                				"encoded": "QAAAywMAKldlIERvbid0IFRhbGsgQWJvdXQgQnJ1bm8gKEZyb20gIkVuY2FudG8iKQAPRGlzbmV5TXVzaWNWRVZPAAAAAAADYzAAC2J2V1JNQVU2Vi1jAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9YnZXUk1BVTZWLWMBADRodHRwczovL2kueXRpbWcuY29tL3ZpL2J2V1JNQVU2Vi1jL21heHJlc2RlZmF1bHQuanBnAAAHeW91dHViZQAAAAAAAAAA",
                				"info": {
                					"identifier": "bvWRMAU6V-c",
                					"isSeekable": true,
                					"author": "DisneyMusicVEVO",
                					"length": 222000,
                					"isStream": false,
                					"position": 0,
                					"title": "We Don't Talk About Bruno (From \"Encanto\")",
                					"uri": "https://www.youtube.com/watch?v=bvWRMAU6V-c",
                					"sourceName": "youtube",
                					"artworkUrl": "https://i.ytimg.com/vi/bvWRMAU6V-c/maxresdefault.jpg",
                					"isrc": null
                				},
                				"pluginInfo": {}
                			}
                		]
                	}
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var result = await client
            .LoadTracksAsync("https://www.youtube.com/playlist?list=PLbpi6ZahtOH6NHu2SGpjLqW_3mYg3S3hw")
            .ConfigureAwait(false);

        // Assert
        Assert.True(result.IsPlaylist);
        Assert.Equal("Top Songs of 2022", result.Playlist.Name);
    }

    [Fact]
    public async Task TestSearchTrackAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/loadtracks", (HttpContext context) =>
        {
            Assert.Equal("ytsearch:Never Gonna Give You Up", context.Request.Query["identifier"]);

            return TypedResults.Text("""
                {
                	"loadType": "search",
                	"data": [
                		{
                			"encoded": "QAAA2QMAPFJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAgKE9mZmljaWFsIE11c2ljIFZpZGVvKQALUmljayBBc3RsZXkAAAAAAANACAALZFF3NHc5V2dYY1EAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kUXc0dzlXZ1hjUQEANGh0dHBzOi8vaS55dGltZy5jb20vdmkvZFF3NHc5V2dYY1EvbWF4cmVzZGVmYXVsdC5qcGcAAAd5b3V0dWJlAAAAAAAAAAA=",
                			"info": {
                				"identifier": "dQw4w9WgXcQ",
                				"isSeekable": true,
                				"author": "Rick Astley",
                				"length": 213000,
                				"isStream": false,
                				"position": 0,
                				"title": "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                				"uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                				"sourceName": "youtube",
                				"artworkUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
                				"isrc": null
                			},
                			"pluginInfo": {}
                		},
                		{
                			"encoded": "QAAAwgMALVJpY2sgQXN0bGV5IE5ldmVyIGdvbm5hIGdpdmUgeW91IHVwIGx5cmljcyEhIQAHSmF5c2VhbgAAAAAAA3a4AAs2X2I3UkR1THdjSQABACtodHRwczovL3d3dy55b3V0dWJlLmNvbS93YXRjaD92PTZfYjdSRHVMd2NJAQAwaHR0cHM6Ly9pLnl0aW1nLmNvbS92aS82X2I3UkR1THdjSS9tcWRlZmF1bHQuanBnAAAHeW91dHViZQAAAAAAAAAA",
                			"info": {
                				"identifier": "6_b7RDuLwcI",
                				"isSeekable": true,
                				"author": "Jaysean",
                				"length": 227000,
                				"isStream": false,
                				"position": 0,
                				"title": "Rick Astley Never gonna give you up lyrics!!!",
                				"uri": "https://www.youtube.com/watch?v=6_b7RDuLwcI",
                				"sourceName": "youtube",
                				"artworkUrl": "https://i.ytimg.com/vi/6_b7RDuLwcI/mqdefault.jpg",
                				"isrc": null
                			},
                			"pluginInfo": {}
                		}
                	]
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.YouTube,
            SearchBehavior: StrictSearchBehavior.Throw);

        // Act
        var track = await client
            .LoadTrackAsync("Never Gonna Give You Up", loadOptions)
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(track);
        Assert.Contains("Rick Astley", track.Title);
    }

    [Fact]
    public async Task TestRoutePlannerGetStatusAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/routeplanner/status", (HttpContext context) =>
        {
            return TypedResults.Text("""
                {
                  "class": "RotatingNanoIpRoutePlanner",
                  "details": {
                    "ipBlock": {
                      "type": "Inet6Address",
                      "size": "1208925819614629174706176"
                    },
                    "failingAddresses": [
                      {
                        "failingAddress": "/1.0.0.0",
                        "failingTimestamp": 1573520707545,
                        "failingTime": "Mon Nov 11 20:05:07 EST 2019"
                      }
                    ],
                    "blockIndex": "0",
                    "currentAddressIndex": "36792023813"
                  }
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var routePlannerInformation = await client
            .GetRoutePlannerInformationAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(routePlannerInformation);
    }

    [Fact]
    public async Task TestRoutePlannerGetStatusWhenDisabledAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/routeplanner/status", (HttpContext context) =>
        {
            return TypedResults.NoContent();
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var routePlannerInformation = await client
            .GetRoutePlannerInformationAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Null(routePlannerInformation);
    }

    [Fact]
    public async Task TestUnmarkAddressAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        var called = false;

        httpClientFactory.Application.MapPost("/v4/routeplanner/free/address", async (HttpContext context) =>
        {
            using var streamReader = new StreamReader(context.Request.Body);
            var body = await streamReader.ReadToEndAsync().ConfigureAwait(false);

            Assert.Equal("""{"address":"1.0.0.1"}""", body);

            called = true;
            return TypedResults.NoContent();
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        var failedAddress = "1.0.0.1";

        // Act
        await client
            .UnmarkFailedAddressAsync(new AddressUnmarkProperties { Address = failedAddress, })
            .ConfigureAwait(false);

        // Assert
        Assert.True(called);
    }

    [Fact]
    public async Task TestUnmarkAddressFailsIfDisabledAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapPost("/v4/routeplanner/free/address", (HttpContext context) =>
        {
            var error = new HttpErrorResponse(
                Timestamp: DateTimeOffset.UtcNow,
                StatusCode: 400,
                ReasonPhrase: "Bad Request",
                StackTrace: null,
                ErrorMessage: "Can't access disabled route planner",
                RequestPath: "/v4/routeplanner/free/address");

            return TypedResults.Json(error, statusCode: error.StatusCode);
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        var failedAddress = "1.0.0.1";

        // Act
        async Task Action() => await client
            .UnmarkFailedAddressAsync(new AddressUnmarkProperties { Address = failedAddress, })
            .ConfigureAwait(false);

        // Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(Action);
        Assert.Contains("Can't access disabled route planner", exception.Message);
    }

    [Fact]
    public async Task TestUnmarkAddressesAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        var called = false;

        httpClientFactory.Application.MapPost("/v4/routeplanner/free/all", (HttpContext context) =>
        {
            called = true;
            return TypedResults.NoContent();
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        await client
            .UnmarkAllFailedAddressesAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.True(called);
    }

    [Fact]
    public async Task TestUnmarkAddressesFailsIfDisabledAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapPost("/v4/routeplanner/free/all", (HttpContext context) =>
        {
            var error = new HttpErrorResponse(
                Timestamp: DateTimeOffset.UtcNow,
                StatusCode: 400,
                ReasonPhrase: "Bad Request",
                StackTrace: null,
                ErrorMessage: "Can't access disabled route planner",
                RequestPath: "/v4/routeplanner/free/address");

            return TypedResults.Json(error, statusCode: error.StatusCode);
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        async Task Action() => await client
            .UnmarkAllFailedAddressesAsync()
            .ConfigureAwait(false);

        // Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(Action);
        Assert.Contains("Can't access disabled route planner", exception.Message);
    }

    [Fact]
    public async Task TestUpdatePlayerAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        var called = false;

        httpClientFactory.Application.MapPatch("/v4/sessions/abc/players/1234567890", async (HttpContext context) =>
        {
            using var streamReader = new StreamReader(context.Request.Body);
            var body = await streamReader.ReadToEndAsync().ConfigureAwait(false);

            Assert.Equal("""{"paused":true}""", body);

            called = true;
            return TypedResults.Text(
                """
                {
                    "guildId": 123,
                    "track": {
                        "encoded": "QAAAjQIAJVJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAADlJpY2tBc3RsZXlWRVZPAAAAAAADPCAAC2RRdzR3OVdnWGNRAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9ZFF3NHc5V2dYY1EAB3lvdXR1YmUAAAAAAAAAAA==",
                        "info": {
                          "identifier": "dQw4w9WgXcQ",
                          "isSeekable": true,
                          "author": "RickAstleyVEVO",
                          "length": 212000,
                          "isStream": false,
                          "position": 60000,
                          "title": "Rick Astley - Never Gonna Give You Up",
                          "uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                          "artworkUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
                          "isrc": null,
                          "sourceName": "youtube"
                        },
                        "pluginInfo": {}
                    },
                    "volume": 1,
                    "paused": true,
                    "voice": {
                      "token": "...",
                      "endpoint": "...",
                      "sessionId": "..."
                    },
                    "filters": {}
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
            optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        var playerUpdate = new PlayerUpdateProperties { IsPaused = true, };

        // Act
        await client
            .UpdatePlayerAsync("abc", 1234567890, playerUpdate)
            .ConfigureAwait(false);

        // Assert
        Assert.True(called);
    }

    [Fact]
    public async Task TestDestroyPlayerAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        var called = false;

        httpClientFactory.Application.MapDelete("/v4/sessions/abc/players/1234567890", async (HttpContext context) =>
        {
            called = true;
            return TypedResults.NoContent();
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
            optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        await client
            .DestroyPlayerAsync("abc", 1234567890)
            .ConfigureAwait(false);

        // Assert
        Assert.True(called);
    }

    [Fact]
    public async Task TestGetPlayerAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        var called = false;

        httpClientFactory.Application.MapGet("/v4/sessions/abc/players/1234567890", async (HttpContext context) =>
        {
            called = true;

            return TypedResults.Text(
                """
                {
                    "guildId": 123,
                    "track": {
                        "encoded": "QAAAjQIAJVJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAADlJpY2tBc3RsZXlWRVZPAAAAAAADPCAAC2RRdzR3OVdnWGNRAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9ZFF3NHc5V2dYY1EAB3lvdXR1YmUAAAAAAAAAAA==",
                        "info": {
                          "identifier": "dQw4w9WgXcQ",
                          "isSeekable": true,
                          "author": "RickAstleyVEVO",
                          "length": 212000,
                          "isStream": false,
                          "position": 60000,
                          "title": "Rick Astley - Never Gonna Give You Up",
                          "uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                          "artworkUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
                          "isrc": null,
                          "sourceName": "youtube"
                        },
                        "pluginInfo": {}
                    },
                    "volume": 1,
                    "paused": true,
                    "voice": {
                      "token": "...",
                      "endpoint": "...",
                      "sessionId": "..."
                    },
                    "filters": {}
                }
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
            optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var model = await client
            .GetPlayerAsync("abc", 1234567890)
            .ConfigureAwait(false);

        // Assert
        Assert.True(called);
        Assert.NotNull(model);
        Assert.Equal(123UL, model.GuildId);
    }

    [Fact]
    public async Task TestGetPlayerNullIfNotFoundAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
            optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var model = await client
            .GetPlayerAsync("abc", 1234567890)
            .ConfigureAwait(false);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public async Task TestGetPlayersAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();
        var called = false;

        httpClientFactory.Application.MapGet("/v4/sessions/abc/players", async (HttpContext context) =>
        {
            called = true;

            return TypedResults.Text(
                """
                [
                    {
                        "guildId": 123,
                        "track": {
                            "encoded": "QAAAjQIAJVJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAADlJpY2tBc3RsZXlWRVZPAAAAAAADPCAAC2RRdzR3OVdnWGNRAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9ZFF3NHc5V2dYY1EAB3lvdXR1YmUAAAAAAAAAAA==",
                            "info": {
                              "identifier": "dQw4w9WgXcQ",
                              "isSeekable": true,
                              "author": "RickAstleyVEVO",
                              "length": 212000,
                              "isStream": false,
                              "position": 60000,
                              "title": "Rick Astley - Never Gonna Give You Up",
                              "uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                              "artworkUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
                              "isrc": null,
                              "sourceName": "youtube"
                            },
                            "pluginInfo": {}
                        },
                        "volume": 1,
                        "paused": true,
                        "voice": {
                          "token": "...",
                          "endpoint": "...",
                          "sessionId": "..."
                        },
                        "filters": {}
                    },
                    {
                        "guildId": 124,
                        "track": {
                            "encoded": "QAAAjQIAJVJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAADlJpY2tBc3RsZXlWRVZPAAAAAAADPCAAC2RRdzR3OVdnWGNRAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9ZFF3NHc5V2dYY1EAB3lvdXR1YmUAAAAAAAAAAA==",
                            "info": {
                              "identifier": "dQw4w9WgXcQ",
                              "isSeekable": true,
                              "author": "RickAstleyVEVO",
                              "length": 212000,
                              "isStream": false,
                              "position": 120000,
                              "title": "Rick Astley - Never Gonna Give You Up",
                              "uri": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                              "artworkUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
                              "isrc": null,
                              "sourceName": "youtube"
                            },
                            "pluginInfo": {}
                        },
                        "volume": 120,
                        "paused": false,
                        "voice": {
                          "token": "...",
                          "endpoint": "...",
                          "sessionId": "..."
                        },
                        "filters": {}
                    }
                ]
                """, "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
            optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var model = await client
            .GetPlayersAsync("abc")
            .ConfigureAwait(false);

        // Assert
        Assert.True(called);
        Assert.Equal(2, model.Length);
    }
}