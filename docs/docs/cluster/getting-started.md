---
sidebar_position: 2
---

# Getting Started

Now that we are clear to start, we only need to prepare a single thing before starting assuming you already have multiple lavalink nodes at hand.

## Installation

First, we need to install the `Lavalink4NET.Cluster` package. You can do this by using the following command:

```bash
dotnet add package Lavalink4NET.Cluster
```

Alternatively, use the NuGet package manager in your IDE, if you are using one.

## Configuration

Now that we have installed the package, we need to configure it. There is not much difference between the single node audio service and the clustered one. You will only need to make changes in a few points. We start by adding the clustered audio service:

```csharp
// Replace this with your call to AddLavalink()
builder.Services.AddLavalinkCluster<DiscordClientWrapper>();

// Replace this with your call to ConfigureLavalink()
builder.Services.ConfigureLavalinkCluster(x =>
{
    x.Nodes = ImmutableArray.Create(
        new LavalinkClusterNodeOptions { BaseAddress = new Uri("http://localhost:2333/"), },
        new LavalinkClusterNodeOptions { BaseAddress = new Uri("http://localhost:2334/"), },
        new LavalinkClusterNodeOptions { BaseAddress = new Uri("http://localhost:2335/"), });
});
```

In your application itself, you don't really need to make any changes besides that. Now you are ready to go and can start using the clustered audio service.

:::info
You can also assign your nodes a label to identify them while debugging or in logs. You can do this by setting the `Label` property on the `LavalinkClusterNodeOptions` object.
:::