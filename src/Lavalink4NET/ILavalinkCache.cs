namespace Lavalink4NET
{
    using System;

    /// <summary>
    ///     An interface for an optional cache for making requests.
    /// </summary>
    public interface ILavalinkCache
    {
        /// <summary>
        ///     Adds an item to the cache.
        /// </summary>
        /// <param name="key">the item cache scope / the location of the item</param>
        /// <param name="item">the item to cache</param>
        /// <param name="absoluteExpiration">
        ///     the Coordinated Universal Time (UTC) time point offset when the cache item will
        ///     expire and is marked to be removed from the cache.
        /// </param>
        void AddItem(string key, object item, DateTimeOffset absoluteExpiration);

        /// <summary>
        ///     Tries to retrieve an item from the cache.
        /// </summary>
        /// <typeparam name="T">the type of the cached object</typeparam>
        /// <param name="key">the item cache scope / the location of the item</param>
        /// <param name="item">
        ///     the item that was found in cache; or default if the item was not in cache
        /// </param>
        /// <returns>a value indicating whether the item was in cache</returns>
        bool TryGetItem<T>(string key, out T item);
    }
}