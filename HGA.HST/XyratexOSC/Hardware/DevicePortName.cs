using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Represents a device controller port that is addressed by a name.
    /// </summary>
    public class DevicePortName : IDevicePort
    {
        /// <summary>
        /// Gets or sets the port name.
        /// </summary>
        /// <value>
        /// The port name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePortName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DevicePortName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            DevicePortName dpn = obj as DevicePortName;
            if (object.ReferenceEquals(dpn, null))
                return false;

            if (String.IsNullOrEmpty(this.Name))
                return false;

            return (this.Name.Equals(dpn.Name));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
