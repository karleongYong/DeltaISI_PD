using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Settings;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Represents a physical hardware unit that is associated with a hardware controller. Examples include a single Motion axis, or a single Light control
    /// </summary>
    public interface IPart : INamed, IDisposable
    {
        /// <summary>
        /// Gets the hardware controller device that owns this <see cref="IPart"/>.
        /// </summary>
        /// <value>
        /// The IDevice.
        /// </value>
        IDevice Device { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IPart"/> is simulated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulated; otherwise, <c>false</c>.
        /// </value>
        bool Simulated { get; }
    }
}
