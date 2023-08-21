# Troubleshooting

Writing discord music bots is hard and there are a lot of things that can go wrong. This page will try to help you with some of the most common problems.

## I am using `QueuedLavalinkPlayer` and after a track ended the next track is not played

You are probably using a custom player implementation and have overriden the `NotifyTrackEndedAsync` method. If you do so, you have to call `base.NotifyTrackEndedAsync` in your implementation. Otherwise the player will not know that the track has ended and will not play the next track.
