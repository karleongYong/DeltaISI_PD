using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC
{
    /// <summary>
    /// Operation event data. This includes the operation activity name, a message about the operation, and the total time elapsed of the operation.
    /// </summary>
    public class OperationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the operation activity name.
        /// </summary>
        /// <value>
        /// The activity name.
        /// </value>
        public string Activity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the detailed operation message.
        /// </summary>
        /// <value>
        /// The operation message.
        /// </value>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time elapsed for the operation to complete.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public TimeSpan Elapsed
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationEventArgs"/> class.
        /// </summary>
        /// <param name="activity">The activity name.</param>
        /// <param name="message">The message message details.</param>
        /// <param name="elapsed">The elapsed time.</param>
        public OperationEventArgs(string activity, string message, TimeSpan elapsed)
        {
            Activity = activity;
            Message = message;
            Elapsed = elapsed;
        }
    }
}
