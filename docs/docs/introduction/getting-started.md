---
sidebar_position: 4
---

# Getting Started

In the previous chapters, we have set up Lavalink4NET and Lavalink. Now we will create a simple bot that can play music using Discord.Net. We will start with a simple bot that can play music. After that, we will add some more features to our bot.

:::note
Since Lavalink4NET v4 a lot of the boilerplate required to integrate Lavalink4NET into your bot has been removed.
:::

Lavalink4NET is integrated to your bot using Dependency Injection. Configure your services and add the Lavalink4NET services to your service collection.

```csharp
using Lavalink4NET.Extensions;

// Integrate Lavalink4NET to your bot
builder.Services.AddLavalink();
```

This single statement will hook up everything required to use Lavalink4NET. It will register an `IAudioService` service descriptor which you can use to access the audio service and handle everything needed later to play music using your bot.

:::tip
If you need an overview of the code used in this chapter, you can find the complete source code for the bot [here](https://github.com/angelobreuer/Lavalink4NET/tree/feature/angelobreuer/lavalink-v4/samples/Lavalink4NET.Discord_NET.ExampleBot).
:::

---

**About `IAudioService`**

The `IAudioService` interface is the main entry point to Lavalink4NET. It provides methods to connect to Lavalink, create players, and more. You should resolve the `IAudioService` using your dependency injection container when you need it.
