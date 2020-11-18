using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Represents an error that occured on a specific hardware device
    /// </summary>
    public class DeviceException : Exception
    {
        /// <summary>
        /// Gets or sets the hardware device.
        /// </summary>
        /// <value>
        /// The hardware device.
        /// </value>
        public IDevice Device
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceException"/> class.
        /// </summary>
        /// <param name="device">The hardware device.</param>
        public DeviceException(IDevice device) :
            base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceException"/> class.
        /// </summary>
        /// <param name="device">The hardware device.</param>
        /// <param name="message">The exception message.</param>
        public DeviceException(IDevice device, string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceException"/> class.
        /// </summary>
        /// <param name="device">The hardware device.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DeviceException(IDevice device, string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
