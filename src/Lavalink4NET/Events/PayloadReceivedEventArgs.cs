/* 
 *  File:   PayloadReceivedEventArgs.cs
 *  Author: Angelo Breuer
 *  
 *  The MIT License (MIT)
 *  
 *  Copyright (c) Angelo Breuer 2019
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
    using Payloads;

    /// <summary>
    ///     The event arguments for the <see cref="LavalinkSocket.PayloadReceived"/> event.
    /// </summary>
    public sealed class PayloadReceivedEventArgs
        : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PayloadReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="payload">the payload that was received</param>
        public PayloadReceivedEventArgs(IPayload payload) => Payload = payload;

        /// <summary>
        ///     Gets the payload that was received.
        /// </summary>
        public IPayload Payload { get; }
    }
}