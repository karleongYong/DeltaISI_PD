using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;
using XyratexOSC.IO.Devices;

namespace XyratexOSC.IO
{
    /// <summary>
    /// Represents errors that occur within the <see cref="IOComponent"/>.
    /// </summary>
    [Serializable]
    public class IOException : DeviceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOException" /> class.
        /// </summary>
        /// <param name="device">The controller device.</param>
        public IOException(IIODevice device) : base(device) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IOException" /> class.
        /// </summary>
        /// <param name="device">The controller device.</param>
        /// <param name="message">The message.</param>
        public IOException(IIODevice device, string message) : base(device, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IOException" /> class.
        /// </summary>
        /// <param name="device">The controller device.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public IOException(IIODevice device, string message, Exception innerException) : base(device, message, innerException) { }
    }
}
