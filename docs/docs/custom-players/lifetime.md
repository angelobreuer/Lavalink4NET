---
sidebar_position: 2
---

# Lifetime

The lifetime of a player is the time between the creation of the player and the disposal of the player.

A player is always bound to a specific guild. This means that a player can only be used in the guild it was created for. If you want to use a player in another guild, you need to create a new player for that guild.

Once a player is created, it will be kept in memory until it is disposed. This means that you can use the player instance to play tracks, pause, resume, stop, and so on. If you want to dispose the player, you can call the `DisposeAsync` method. This will stop the player and remove it from memory.

This means that if you store information in the player instance, it will be kept in memory until the player is disposed. This can be useful if you want to store information about the player, for example, a text channel bound to the player for messages.
