# Logging

Lavalink4NET provides additional logging:

To enable it specify a logger implementation in the constructor:

```csharp
var audioService = new LavalinkNode(new LavalinkNodeOptions{[...]}, 
    new DiscordClientWrapper(client),
    [Your Logger Implementation]);
```

