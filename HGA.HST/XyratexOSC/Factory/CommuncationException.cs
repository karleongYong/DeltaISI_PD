using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Represents a facility communication error
    /// </summary>
    public class CommuncationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommuncationException"/> class.
        /// </summary>
        public CommuncationException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommuncationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommuncationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommuncationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CommuncationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
