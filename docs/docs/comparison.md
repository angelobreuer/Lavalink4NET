# Comparison

This guide will help you in your choice to choose between the .NET Lavalink client libraries. We know it is a hard decision as there a few libraries to choose from. We will try to explain the differences between the libraries and what you should choose.

## Overview

| -                        | Lavalink4NET                                                                                                                              | Victoria                                                                                                       | DisCatSharp                                                                                                                                | Nomia                                                                                                       |
|--------------------------|-------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------|
| Maximum Lavalink version | 4.0.0-beta3                                                                                                                               | 3.7.8                                                                                                          | 4.0.0-beta3                                                                                                                                | 3.7.8                                                                                                       |
| Minimum .NET version     | [6.0](https://github.com/angelobreuer/Lavalink4NET/blob/30c3cdc474a18399e9b79baa2dd87f0ff8cd3343/src/Lavalink4NET/Lavalink4NET.csproj#L5) | [6.0](https://github.com/Yucked/Victoria/blob/a57501af2af03d1fcdde18275dd925bb74491504/src/Victoria.csproj#L5) | [6.0](https://github.com/Aiko-IT-Systems/DisCatSharp/blob/50a71fec3d60fa1bfc709c08e1cf2dcbb6a55b30/DisCatSharp.Targets/Library.targets#L5) | [6.0](https://github.com/DHCPCD9/Nomia/blob/597d15d0136466eb6ba4397313f449dcc95ef7c3/Nomia/Nomia.csproj#L5) |
| Supported clients        | Discord.Net/DSharpPlus/Remora.Discord                                                                                                     | Discord.Net                                                                                                    | DisCatSharp                                                                                                                                | DSharpPlus                                                                                                  |
| Test coverage            | 67.8 %                                                                                                                                    | 0.0%                                                                                                           | 0.0%                                                                                                                                       | 0.0%                                                                                                        |

## Features

| -                             | Lavalink4NET | Victoria | DisCatSharp | Nomia |
|-------------------------------|--------------|----------|-------------|-------|
| Built-in queue support        | ✅            | ✅        | ✅           | ❌     |
| v3 backwards compatible       | ❌            | -        | ❌           | ❌     |
| Track decoding (built-in)     | ✅            | ✅        | ❌           | ❌     |
| Track decoding (external)     | ❌            | ❌        | ✅           | ❌     |
| Track search                  | ✅            | ✅        | ✅           | ✅     |
| Pausing/resuming              | ✅            | ✅        | ✅           | ✅     |
| Filter support                | ✅            | ✅        | ✅           | ✅     |
| Inactivity tracking           | ✅            | ❌        | ❌           | ❌     |
| Lyrics                        | ✅            | ✅        | ❌           | ❌     |
| V4 initial play optimization  | ✅            | ❌        | ❌           | ❌     |
| Caching support               | ✅            | ❌        | ❌           | ❌     |
| External queue support        | ✅            | ❌        | ❌           | ❌     |
| Built-in ExtraFilters support | ✅            | ❌        | ❌           | ❌     |
| Built-in LavaSearch support   | ✅            | ❌        | ❌           | ❌     |
| Built-in LavaSrc support      | ✅            | ❌        | ❌           | ❌     |
| Built-in SponsorBlock support | ✅            | ❌        | ❌           | ❌     |
| Built-in TextToSpeech support | ✅            | ❌        | ❌           | ❌     |
| Player preconditions          | ✅            | ❌        | ❌           | ❌     |
| Vote player implementation    | ✅            | ❌        | ❌           | ❌     |
| Artwork resolution (native)   | ✅            | ✅        | ❌           | ❌     |
| Artwork resolution (Lavalink) | ✅            | ❌        | ❌           | ✅     |
| Load balacing/Clustering      | ✅            | ❌        | ❌           | ❌     |
| Payload interception          | ✅            | ❌        | ❌           | ❌     |
| Proxy support                 | ✅*           | ❌        | ✅           | ❌     |
| Custom players                | ✅            | 🟨       | ❌           | ✅     |
| Session resumption            | ✅            | ✅        | ❌           | ✅     |
| NativeAOT ready\*\*           | ✅            | ❌        | ❌           | ❌     |

- \* Proxy support indirectly provided by IHttpClientFactory
- \** Assuming discord library is also NativeAOT ready

:::info
This list is not comprehensive and may be missing some features. If you find any missing features, please open an issue in the Lavalink4NET issue tracker, or open a pull request to add it to the documentation.
:::
