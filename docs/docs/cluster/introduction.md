---
sidebar_position: 1
---

# Introduction

Clusters are a great way to improve the load balancing and availability of your application. They are also a great way to scale your application horizontally. In this guide, we will learn how to create a lavalink node cluster.

## When do I need clustering?

Clustering makes sense when you have a large number of users and you want to distribute the load across multiple nodes. Lavalink can start to consume huge resources when you have a lot of users using the music feature. In case, a single server is unable to load, you can start by utilizing multiple nodes distributed on different dedicates servers to handle the load.

## When I don't need clustering?

If you are running a small bot with a few users, you don't need to use clustering. Clustering is only useful when you have a large number of users. Nearly all bots are able to run on a single node without any issues. If you are running a large bot, you can start by using a single node and then scale to multiple nodes when you need it. Lavalink4NET recommends to start with using no clustering when you first once created your bot. Only use clustering if your needs become a requirement since clustering for small bots introduced a lot of complexity and is mostly considered premature optimization for small bots.

## How does clustering work?

To use clustering you need a set of highly available lavalink nodes. Those nodes are registered while configuring Lavalink4NET. Lavalink4NET will start off by using the first node if needed. Once more players are utilized, Lavalink4NET will start using the other nodes. Lavalink4NET will automatically distribute the load across all nodes. If a node is unavailable, Lavalink4NET will automatically detect the node as degraded and will stop using it. Once the node is available again, Lavalink4NET will automatically start using it again. This makes it possible to have a highly available lavalink cluster.
