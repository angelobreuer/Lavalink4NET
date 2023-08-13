---
sidebar_position: 3
---

# Installation

Lavalink4NET comes with a lot of features and is divided into multiple packages. You can choose which packages you want to use. We will start with the basic package you need.

When you start developing your own bot, you have decided what discord bot framework you use. Lavalink4NET supports Discord.Net and DSharpPlus out-of-the-box.

If you use Discord.Net, please install the [`Lavalink4NET.Discord.NET`](https://www.nuget.org/packages/Lavalink4NET.Discord.NET) package. If you use DSharpPlus, please install the [`Lavalink4NET.DSharpPlus`](https://www.nuget.org/packages/Lavalink4NET.DSharpPlus) package. If your favorite discord client is not supported, please open an issue on GitHub.

:::info

You may see the `Lavalink4NET` package. This package is used if you want to implement client support and contains all core features. Normally, you don't need to install this package directly, as the client packages already depend on it.

:::

---

If you use Visual Studio Code, or any other IDE that supports NuGet, you can install the packages directly from the IDE. If you use the .NET CLI, you can install the packages using the following command:

```bash
dotnet add package Lavalink4NET.Discord.NET
```

---

🚀 That's it! You have successfully installed Lavalink4NET. Now you can continue with integrating Lavalink4NET into your bot.
