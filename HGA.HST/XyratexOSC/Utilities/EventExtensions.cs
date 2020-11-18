using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// Provides extension methods for simplifying safe event invoking. 
    /// If nothing is subscribed to the event, then it will not be fired.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Fires this event only if any listeners are attached
        /// </summary>
        /// <typeparam name="T">EventArgs Type</typeparam>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        public static void SafeInvoke<T>(this EventHandler<T> eventHandler, object sender, T e)
            where T : EventArgs
        {
            EventHandler<T> handler = eventHandler;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Fires this event only if any listeners are attached
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        public static void SafeInvoke(this EventHandler eventHandler, object sender, EventArgs e)
        {
            EventHandler handler = eventHandler;
            if (handler != null)
                handler(sender, e);
        }

        public static void LoadInvoke(this EventHandler eventHandler, object sender, EventArgs e)
        {
            EventHandler handler = eventHandler;
            if (handler != null)
                handler(sender, e);
        }
    }
}
