/*
 *  File:   AsyncEventExtensions.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
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

namespace Lavalink4NET.Events
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     Extension method for asynchronous events.
    /// </summary>
    public static class AsyncEventExtensions
    {
        /// <summary>
        ///     Invokes an asynchronous event.
        /// </summary>
        /// <param name="eventHandler">the asynchronous event handler</param>
        /// <param name="sender">the object that is firing the event</param>
        /// <param name="eventArgs">
        ///     the event parameters (if <see langword="null"/><see cref="EventArgs.Empty"/> is used)
        /// </param>
        /// <returns>a task that represents the asynchronous operation</returns>
        public static Task InvokeAsync(this AsyncEventHandler? eventHandler, object sender, EventArgs? eventArgs = null)
            => eventHandler?.Invoke(sender, eventArgs ?? EventArgs.Empty) ?? Task.CompletedTask;

        /// <summary>
        ///     Invokes an asynchronous event.
        /// </summary>
        /// <typeparam name="TEventArgs">the type of the event parameters</typeparam>
        /// <param name="eventHandler">the asynchronous event handler</param>
        /// <param name="sender">the object that is firing the event</param>
        /// <param name="eventArgs">the event parameters</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        public static Task InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs>? eventHandler, object sender, TEventArgs eventArgs)
            => eventHandler?.Invoke(sender, eventArgs) ?? Task.CompletedTask;
    }
}
