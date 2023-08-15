# Text To Speech

Text to speech (TTS) is a feature that allows you to play synthesized speech from text. This feature is useful for bots that need to communicate with users.

:::caution
You may want to use the Lavasrc plugin instead of the Google Cloud TTS plugin as you require a Google Cloud API key to use the Google Cloud TTS plugin.
Lavasrc uses Flowery TTS which is a free and open source TTS engine. However, Google Cloud TTS offers more options to configure.
:::

## Installation

For using text to speech, you need to install the [`Lavalink4NET.Integrations.TextToSpeech`](https://www.nuget.org/packages/Lavalink4NET.Integrations.TextToSpeech) package.

:::caution
You need to have the [Google Cloud TTS Plugin](https://github.com/DuncteBot/tts-plugin) installed on your Lavalink server.
:::

## Usage

The text to speech integration provides an extension method which hooks into the `ITrackManager` interface. You can use this method to play text as speech:

```csharp
var track = await audioService.Tracks
    .GetTextToSpeechTrackAsync("Say hello!")
    .ConfigureAwait(false);

await player
    .PlayAsync(track!)
    .ConfigureAwait(false);
```
