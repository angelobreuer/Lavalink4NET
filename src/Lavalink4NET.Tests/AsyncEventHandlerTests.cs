/*
 *  File:   AsyncEventHandlerTests.cs
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

namespace Lavalink4NET.Tests
{
    using System.Threading.Tasks;
    using Lavalink4NET.Events;
    using Xunit;

    /// <summary>
    ///     Contains tests for the asynchronous event handler ( <see cref="AsyncEventHandler"/>).
    /// </summary>
    public sealed class AsyncEventHandlerTests
    {
        private event AsyncEventHandler Event;

        /// <summary>
        ///     Tests that the asynchronous event handler waits until all attached handlers were executed.
        /// </summary>
        [Fact]
        public async Task TestAwaiting()
        {
            Event = null;

            // add handlers
            Event += (sender, args) => Task.CompletedTask;
            Event += (sender, args) => Task.Delay(100);
            Event += (sender, args) => Task.Delay(1000);

            var task = Event.InvokeAsync(this);

            // ensure that the task did not complete in 300 milliseconds
            Assert.NotSame(task, await Task.WhenAny(Task.Delay(300), task));
        }
    }
}
