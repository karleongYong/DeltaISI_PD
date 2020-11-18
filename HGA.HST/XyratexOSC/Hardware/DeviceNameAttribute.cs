using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Attribute for defining the valid device names for configuring an <see cref="IDevice"/>.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class DeviceNamesAttribute : System.Attribute
    {
        /// <summary>
        /// Gets or sets the valid device names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public string[] Names
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceNamesAttribute"/> class.
        /// </summary>
        /// <param name="names">The names.</param>
        public DeviceNamesAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
