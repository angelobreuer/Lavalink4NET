---
sidebar_position: 2
---

# Inactivity trackers

Inactivity trackers are used to determine if a player is active or inactive. Lavalink4NET provides two inactivity trackers out-of-the-box:

## IdleInactivityTracker

The `IdleInactivityTracker` will report the player as inactive if the player is idle, e.g. if the player is paused or if the player is not playing any track.

## UsersInactivityTracker

The `UsersInactivityTracker` will report the player as inactive if all users left the voice channel. You can specify a threshold indicating how many users must be in the voice channel to report the player as active.
