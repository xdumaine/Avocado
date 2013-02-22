namespace MetroMVVM.Extensions
{
    using System;

    /// <summary>
    /// Extension Methods useful to raise events
    /// </summary>
    public static class EventsExtensions
    {
        /// <summary>
        /// Raise an event without EventArgs
        /// </summary>
        /// <param name="eventHandler">The event to raise</param>
        /// <param name="sender">The sender of the event</param>
        public static void Raise(this EventHandler eventHandler, object sender)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raise an Event with a generic T EventArgs
        /// </summary>
        /// <typeparam name="T">The EventArgs type</typeparam>
        /// <param name="event">The event to raise</param>
        /// <param name="sender">The sender of the event</param>
        /// <param name="eventArgs">The EventArgs to use</param>
        public static void Raise<T>(this EventHandler<T> @event, object sender, T eventArgs)
            where T : EventArgs
        {
            if (@event != null)
            {
                @event(sender, eventArgs);
            }
        }

        /// <summary>
        /// Raise an Event with an EventArgs of T
        /// </summary>
        /// <typeparam name="T">The type contained in the EventArgs</typeparam>
        /// <param name="event">The event to raise</param>
        /// <param name="sender">The sender of the event</param>
        /// <param name="value">The value of the EventArgs</param>
        public static void Raise<T>(this EventHandler<EventArgs<T>> @event, object sender, T value)
        {
            Raise(@event, sender, new EventArgs<T>(value));
        }
    }
}
