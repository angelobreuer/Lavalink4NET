# Track decoding

Lavalink4NET provides a high efficient track encoder and decoder to encode and decode Lavalink's native track identifiers.

You can use the `LavalinkTrack#Parse` method to parse v2 and v3 track identifiers:

```csharp
var track = await audioService.Tracks
    .LoadTrackAsync("Never gonna give you up", TrackSearchMode.YouTube);
    .ConfigureAwait(false);

// QAAA2QMAPFJpY2sgQXN0bGV5IC0gTmV2ZXIgR29ubmEgR2l2ZSBZb3UgVXAgKE9mZmljaWFsIE11c2ljIFZpZGVvKQALUmljayBBc3RsZXkAAAAAAANACAALZFF3NHc5V2dYY1EAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kUXc0dzlXZ1hjUQEANGh0dHBzOi8vaS55dGltZy5jb20vdmkvZFF3NHc5V2dYY1EvbWF4cmVzZGVmYXVsdC5qcGcAAAd5b3V0dWJlAAAAAAAAAAA=
var identifier = track.ToString();

var restoredTrack = LavalinkTrack.Parse(identifier, provider: null);
```

If you want to encode a track identifier, you can use the `LavalinkTrack#ToString` method.
