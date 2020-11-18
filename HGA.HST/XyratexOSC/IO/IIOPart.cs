using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;

namespace XyratexOSC.IO
{
    /// <summary>
    /// Represents a single I/O channel.
    /// </summary>
    public interface IIOPart : IPart
    {
        /// <summary>
        /// Gets or sets a value indicating whether this channel is read-only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read-only; otherwise, <c>false</c>.
        /// </value>
        bool ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the I/O channel address.
        /// </summary>
        /// <value>
        /// The I/O channel address.
        /// </value>
        int Channel
        {
            get;
            set;
        }
    }
}
