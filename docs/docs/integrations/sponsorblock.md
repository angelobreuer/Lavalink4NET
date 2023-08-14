# SponsorBlock

SponsorBlock is an open-source crowdsourced browser extension and service that skips sponsor segments in YouTube videos. It is available as a browser extension for Firefox, Chrome, and Edge.

The SponsorBlock plugin for Lavalink allows you to skip sponsor segments in YouTube videos. The plugin will automatically detect sponsor segments and will skip them.

Lavalink4NET provides an integration for the SponsorBlock plugin with the [`Lavalink4NET.Integrations.SponsorBlock`](https://www.nuget.org/packages/Lavalink4NET.Integrations.SponsorBlock) package.

## Installation

For using SponsorBlock, you need to install the [`Lavalink4NET.Integrations.SponsorBlock`](https://www.nuget.org/packages/Lavalink4NET.Integrations.SponsorBlock) package.

## Usage

First, you need to integrate the SponsorBlock plugin with Lavalink4NET. You can do this by calling `UseSponsorBlock` on either the host or the audio service:

```csharp
var app = builder.Build();

app.UseSponsorBlock();

await app.RunAsync();
```

That's it! The SponsorBlock plugin is now integrated with Lavalink4NET.

### Skipping segments

Now, we need to configure players to skip certain segments. We can do this by using the `UpdateSponsorBlockCategoriesAsync` method.

```csharp
var categories = ImmutableArray.Create(
    SegmentCategory.Intro,
    SegmentCategory.Sponsor,
    SegmentCategory.SelfPromotion);

await player
    .UpdateSponsorBlockCategoriesAsync(categories)
    .ConfigureAwait(false);
```

The `UpdateSponsorBlockCategoriesAsync` method will update the categories of the player. The categories are used to determine which segments should be skipped. Here, we are updating the categories to skip intro, sponsor, and self-promotion segments.

### Resetting categories

You can reset the categories of a player by using the `ResetSponsorBlockCategoriesAsync` method.

```csharp
await player
    .ResetSponsorBlockCategoriesAsync()
    .ConfigureAwait(false);
```
