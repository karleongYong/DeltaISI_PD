using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Licensing
{
    /// <summary>
    /// Represents a licensing error that occurred during a license verification.
    /// </summary>
    public class LicensingException : Exception
    {
        private int sentinelError;

        /// <summary>
        /// Gets the Sentinel SafeNet error code. 
        /// </summary>
        /// <value>The error code.</value>
        public int ErrorCode
        {
            get { return sentinelError; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingException"/> class.
        /// </summary>
        public LicensingException() : this("Failed to verify Xyratex Licensing.", -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingException"/> class with a 
        /// specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public LicensingException(string message) : this(message, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingException"/> class with a 
        /// specified error message and Sentinel <see cref="ErrorCode"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errCode">The Sentinel UltraPro err code.</param>
        public LicensingException(string message, int errCode)
            : base(message + " (" + errCode + ")")
        {
            sentinelError = errCode;
        }
    }
}
