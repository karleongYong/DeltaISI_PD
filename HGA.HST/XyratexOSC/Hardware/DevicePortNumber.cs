using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Represents a device controller port that is addressed by a integer.
    /// </summary>
    public class DevicePortNumber : IDevicePort, IComparable
    {
        /// <summary>
        /// Gets the number of the port.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public int Number
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePortNumber"/> class.
        /// </summary>
        /// <param name="number">The port number.</param>
        public DevicePortNumber(int number)
        {
            Number = number;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Number.ToString();
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
            DevicePortNumber dpn = obj as DevicePortNumber;
            if (object.ReferenceEquals(dpn, null))
                return false;

            return (this.Number == dpn.Number);
        }

        public override int GetHashCode()
        {
            return Number;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            DevicePortNumber dps = value as DevicePortNumber;
            if (object.ReferenceEquals(dps, null))
                return 1;

            return (Number.CompareTo(dps.Number));
        }

        /// <summary>
        /// Determines if the left hand port is less than the right hand port.
        /// </summary>
        /// <param name="port1">The port1.</param>
        /// <param name="port2">The port2.</param>
        /// <returns></returns>
        public static bool operator <(DevicePortNumber port1, DevicePortNumber port2)
        {
            if (port1 == null)
                return false;

            return port1.CompareTo(port2) < 0;
        }

        /// <summary>
        /// Determines if the left hand port is greater than the right hand port.
        /// </summary>
        /// <param name="port1">The port1.</param>
        /// <param name="port2">The port2.</param>
        /// <returns></returns>
        public static bool operator >(DevicePortNumber port1, DevicePortNumber port2)
        {
            if (port1 == null)
                return false;

            return port1.CompareTo(port2) > 0;
        }
    }
}
