/*
 *  File:   ILavalinkCache.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET;

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
    void AddItem(string key, object? item, DateTimeOffset absoluteExpiration);

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
