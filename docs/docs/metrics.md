# Metrics

Lavalink4NET exposes metrics about player management, track resolution and more. You can use `dotnet-monitor` or `dotnet-counters` to view the metrics.

## Available metrics

The following metrics are available:

| Name            | Description                         |
|-----------------|-------------------------------------|
| active-players  | The number of active players.       |
| pending-handles | The number of pending handles.      |
| player-handles  | The number of player handles.       |
| resolved-tracks | The number of resolved tracks.      |
| failed-queries  | The number of failed track queries. |

## Usage

You can use the `dotnet-counters` tool to monitor metrics in real-time:

```bash
dotnet tool install --global dotnet-counters
dotnet counters monitor -n <exe name> --counters Lavalink4NET
```

```bash
Press p to pause, r to resume, q to quit.
    Status: Running

[Lavalink4NET]
    active-players (Players)                                               116
    pending-handles (Handles)                                              4
    player-handles (Handles)                                               120
    resolved-tracks (Tracks / 1 sec)
        Identifier=Never gonna give you up                                 0
```

Here you can see, that there are 116 active players and 4 pending handles. The `resolved-tracks` counter is a rate counter, which means that it will show the number of resolved tracks in the last second. In this case, there were no tracks resolved in the last second.
