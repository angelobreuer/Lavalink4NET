namespace Lavalink4NET.Events;

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
    public static ValueTask InvokeAsync(this AsyncEventHandler? eventHandler, object sender)
    {
        ArgumentNullException.ThrowIfNull(sender);

        return InvokeAsync(eventHandler, sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes an asynchronous event.
    /// </summary>
    /// <param name="eventHandler">the asynchronous event handler</param>
    /// <param name="sender">the object that is firing the event</param>
    /// <param name="eventArgs">
    ///     the event parameters (if <see langword="null"/><see cref="EventArgs.Empty"/> is used)
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public static ValueTask InvokeAsync(this AsyncEventHandler? eventHandler, object sender, EventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventHandler is null)
        {
            return default;
        }

        return new(eventHandler.Invoke(sender, eventArgs));
    }

    /// <summary>
    ///     Invokes an asynchronous event.
    /// </summary>
    /// <typeparam name="TEventArgs">the type of the event parameters</typeparam>
    /// <param name="eventHandler">the asynchronous event handler</param>
    /// <param name="sender">the object that is firing the event</param>
    /// <param name="eventArgs">the event parameters</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public static ValueTask InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs>? eventHandler, object sender, TEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventHandler is null)
        {
            return default;
        }

        return new(eventHandler.Invoke(sender, eventArgs));
    }
}
