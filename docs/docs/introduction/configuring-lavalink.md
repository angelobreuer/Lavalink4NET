---
sidebar_position: 6
---

# Configuring Lavalink4NET

Lavalink4NET can be configured by utilizing dependency injection. This allows you to configure Lavalink4NET in a central place and use it in your bot.

You can use the `.ConfigureLavalink` on the `IServiceCollection` to configure Lavalink4NET. The following example shows how to configure Lavalink4NET with the default settings:

```csharp
services.ConfigureLavalink(config =>
{
    config.... = ...;
});
```

## Options

## Base Address

The base address is the address of the Lavalink node. This is the address that is used to connect to the Lavalink node.

The default value is to use a v4 lavalink node on the local machine with the port 2333. If you want to use a different address, you can configure it using the `BaseAddress` property.

```csharp
services.ConfigureLavalink(config =>
{
    config.BaseAddress = new Uri("http://localhost:2333");
});
```

## WebSocket URI

The WebSocket URI is the URI that is used to connect to the Lavalink node. If `null` is specified, Lavalink4NET will use the `BaseAddress` to generate the WebSocket URI.

:::info
If you configure the WebSocket URI, keep in mind that the URI often ends in `/v4/websocket`.
:::

```csharp
services.ConfigureLavalink(config =>
{
    config.WebSocketUri = new Uri("ws://localhost:2333/v4/websocket");
});
```

## Ready Timeout

The ready timeout is the time Lavalink4NET waits for the Lavalink node to become ready. If the node does not become ready in the specified time, Lavalink4NET will throw an exception.

```csharp
services.ConfigureLavalink(config =>
{
    config.ReadyTimeout = TimeSpan.FromSeconds(10);
});
```

## Resumption Options

Lavalink4NET supports resuming of audio sessions. This means that if the Lavalink node disconnects, the audio session will be resumed after the node has reconnected. This is useful if you want to keep the queue of your bot.

Session resumption is enabled by default with a resumption timeout of 60 seconds. This means that if the node does not reconnect within 60 seconds, the session will be destroyed and the queue will be cleared.

You can configure this behavior or disable session resumption by using the `LavalinkSessionResumptionOptions` struct.

```csharp
services.ConfigureLavalink(config =>
{
    config.SessionResumption = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(60));
});
```

## Label

The label is used to identify the Lavalink node. This is useful if you have multiple nodes and want to identify them. Also, the label is used to identify the node in the logs.

```csharp
services.ConfigureLavalink(config =>
{
    config.Label = "Node 1";
});
```

## Passphrase

The passphrase is used to authenticate with the Lavalink node. If you have not set a specific passphrase in the Lavalink configuration, Lavalink4NET will use the default passphrase `youshallnotpass`.

:::info
Using the default passphrase is not recommended. Please set a passphrase in the Lavalink configuration and use it in Lavalink4NET.
:::

```csharp
services.ConfigureLavalink(config =>
{
    config.Passphrase = "youshallnotpass";
});
```

## Http Client Name

Lavalink4NET uses an `HttpClient` to communicate with the Lavalink node. You can configure the name of the `HttpClient` that is used by Lavalink4NET. This is useful if you want to use a custom `HttpClient` returned by the `IHttpClientFactory`.

```csharp
services.ConfigureLavalink(config =>
{
    config.HttpClientName = "LavalinkHttpClient";
});
```
