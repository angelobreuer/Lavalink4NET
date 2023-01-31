namespace Lavalink4NET.Events;

using System;
using System.Threading.Tasks;

/// <summary>
///     An asynchronous event handler.
/// </summary>
/// <typeparam name="TEventArgs">the type of the event arguments</typeparam>
/// <param name="sender">the object that fired the event</param>
/// <param name="eventArgs">the event arguments</param>
/// <returns>a task that represents the asynchronous operation</returns>
public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs eventArgs);

/// <summary>
///     An asynchronous event handler.
/// </summary>
/// <param name="sender">the object that fired the event</param>
/// <param name="eventArgs">the event arguments</param>
/// <returns>a task that represents the asynchronous operation</returns>
public delegate Task AsyncEventHandler(object sender, EventArgs eventArgs);
