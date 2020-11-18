using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using XyratexOSC.Logging;
using XyratexOSC.Utilities;

namespace XyratexOSC
{
    /// <summary>
    /// When used in a using block, provides the scope for an "activity" so that it will be logged automatically (with timing info) once the operation scope is left.
    /// An event handler can also be provided, so that the event is automatically fired once the operation is finished.
    /// </summary>
    /// <remarks>
    /// Since this used within a using block, the operation logging and event triggering is guaranteed to execute regardless of an unexpected exception.
    /// </remarks>
    public class Operation : IDisposable
    {
        private Stopwatch _sw;
        private object _source;
        private string _activity;
        private string _message;
        private EventHandler<OperationEventArgs> _handler;
        
        private Operation(EventHandler<OperationEventArgs> handler, object source, string activity, string messageFormat, params object[] args)
        {
            _handler = handler;
            _source = source;
            _activity = activity;

            if (!String.IsNullOrEmpty(activity)) // Only log when an activity has been specified
            {
                if (String.IsNullOrEmpty(messageFormat))
                {
                    Log.Info(_source, String.Format("{0},Started", _activity));
                }
                else
                {
                    if (args.Length == 0)
                        _message = messageFormat;
                    else
                        _message = String.Format(messageFormat, args);

                    Log.Info(_source, String.Format("{0},{1},Started", _activity, _message));
                }
            }

            _sw = Stopwatch.StartNew();
    
            if (_handler != null)
                _handler.SafeInvoke(_source, new OperationEventArgs(_activity, _message, TimeSpan.Zero));
        }

        /// <summary>
        /// Starts an operation scope, and initiates an internal stopwatch for gathering elapsed timing info.
        /// </summary>
        /// <param name="source">The source of the operation.</param>
        /// <param name="activity">The activity name.</param>
        /// <param name="messageFormat">The message as a formatting string. See <see cref="String.Format"/>.</param>
        /// <param name="args">The message arguments.</param>
        /// <returns>A disposable operation object.</returns>
        public static Operation Start(object source, string activity, string messageFormat, params object[] args)
        {
            return new Operation(null, source, activity, messageFormat, args);
        }

        /// <summary>
        /// Starts an operation scope, and initiates an internal stopwatch for gathering elapsed timing info.
        /// </summary>
        /// <param name="source">The source of the operation.</param>
        /// <param name="activity">The activity name.</param>
        /// <returns>A disposable operation object.</returns>
        public static Operation Start(object source, string activity)
        {
            return new Operation(null, source, activity, "");
        }

        /// <summary>
        /// Starts an operation scope, and initiates an internal stopwatch for gathering elapsed timing info.
        /// </summary>
        /// <param name="update">The update event that will be fired once the operation scope is left.</param>
        /// <param name="source">The source of the operation.</param>
        /// <param name="activity">The activity name.</param>
        /// <returns>
        /// A disposable operation object.
        /// </returns>
        public static Operation Start(EventHandler<OperationEventArgs> update, object source, string activity)
        {
            return new Operation(update, source, activity, "");
        }

        /// <summary>
        /// Starts an operation scope, and initiates an internal stopwatch for gathering elapsed timing info.
        /// </summary>
        /// <param name="update">The update event that will be fired once the operation scope is left.</param>
        /// <param name="source">The source of the operation.</param>
        /// <param name="activity">The activity name.</param>
        /// <param name="messageFormat">The message as a formatting string. See <see cref="String.Format"/>.</param>
        /// <param name="args">The message arguments.</param>
        /// <returns>
        /// A disposable operation object.
        /// </returns>
        public static Operation Start(EventHandler<OperationEventArgs> update, object source, string activity, string messageFormat, params object[] args)
        {
            return new Operation(update, source, activity, messageFormat, args);
        }

        /// <summary>
        /// Gathers the elapsed time of the operation, logs the operation data and timing to the Application Log, and invokes the associated event (if provided).
        /// </summary>
        public void Dispose()
        {
            TimeSpan elapsed = _sw.Elapsed;

            if (!String.IsNullOrEmpty(_activity)) // Only log when an activity has been specified
            {
                if (String.IsNullOrEmpty(_message))
                    Log.Info(_source, String.Format("{0},Ended,{1:g}", _activity, elapsed));
                else
                    Log.Info(_source, String.Format("{0},{1},Ended,{2:g}", _activity, _message, elapsed));
            }

            if (_handler != null)
                _handler.SafeInvoke(_source, new OperationEventArgs(_activity, _message, elapsed));
        }
    }
}
