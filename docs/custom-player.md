# Custom Players

Lavalink4NET comes with **three players out of the box**:

##### LavalinkPlayer
The default implementation of the lavalink player. All players depend on this player.
Supports basic operations like Play, Stop, Pause, Resume, etc.

##### QueuedLavalinkPlayer
Supports track queuing.

##### VoteLavalinkPlayer
Supports track skip voting.

___

Lavalink4NET supports **custom players**, so you can handle specific events, such as when a title ends up with you:

You need to create your own class for a player as shown below:

```csharp
namespace [...]
{
    using System.Threading.Tasks;
    using Lavalink4NET.Events;
    using Lavalink4NET.Player;

    internal sealed class MyPlayer : LavalinkPlayer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MyPlayer"/> class.
        /// </summary>
        /// <param name="lavalinkSocket">the lavalink socket</param>
        /// <param name="client">the discord client</param>
        /// <param name="guildId">the identifier of the guild that is controlled by the player</param>
        public MyPlayer(LavalinkSocket lavalinkSocket, IDiscordClientWrapper client, ulong guildId)
            : base(lavalinkSocket, client, guildId)
        {
        }

        protected override async Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        {
            // check if the next track should be started
            if (eventArgs.MayStartNext)
            {
                // load track
                var track = [...] // <-- This track will be played

                // play track
                await PlayAsync(track);
            }

            await base.OnTrackEndAsync(eventArgs);
        }
    }
}
```

The player will start playing the track after a track ends.