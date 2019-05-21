# Cluster

Lavalink4NET supports Node clustering. This means that it provides the option for load balancing players on different nodes.

Here is an example for creating a node cluster:

```csharp
new LavalinkCluster(new LavalinkClusterOptions
{
    Nodes = new[] {
        new LavalinkNodeOptions {[...]},
        [...]
    }
}, new DiscordClientWrapper(client));
```

By default the cluster uses the round-robin load balancing strategy, so it favors the node that has not been used the longest.

