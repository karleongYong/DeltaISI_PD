using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Secs II 8-bit unsigned integer data structure
    /// </summary>
    [Serializable]
    public sealed class SecsUInt8 : ISecsValue, IComparable<SecsUInt8>
    {
        private readonly Byte _value;
        private readonly string _format = "U1";
        private const int _length = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsUInt8"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SecsUInt8(Byte value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the SECS format string.
        /// </summary>
        /// <returns></returns>
        public string GetSECSFormat()
        {
            return _format;
        }

        public int GetSECSLength()
        {
            return _length;
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo((SecsUInt8)obj);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="SecsUInt8"/> object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(SecsUInt8 value)
        {
            if (value == null)
                return 1;

            return _value.CompareTo(value._value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this <see cref="SecsUInt8"/> instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this <see cref="SecsUInt8"/> instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this <see cref="SecsUInt8"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SecsUInt8))
                return false;

            SecsUInt8 sobj = obj as SecsUInt8;
            return _value.Equals(sobj._value);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <summary>
        /// Determines if the two <see cref="SecsUInt8"/> values are equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if equal; otherwise, false.</returns>
        public static bool operator ==(SecsUInt8 val1, SecsUInt8 val2)
        {
            if ((object)val1 == (object)val2)
                return true;
            else if ((object)val1 == null || (object)val2 == null)
                return false;

            return (val1._value == val2._value);
        }

        /// <summary>
        /// Determines if the two <see cref="SecsUInt8"/> values are not equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if not equal; otherwise, false.</returns>
        public static bool operator !=(SecsUInt8 val1, SecsUInt8 val2)
        {
            return !(val1 == val2);
        }

        /// <summary>
        /// Determines if <see cref="SecsUInt8"/> value 1 is greater than value 2.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        /// <returns>True, if greater than; otherwize, false.</returns>
        public static bool operator >(SecsUInt8 val1, SecsUInt8 val2)
        {
            return val1._value > val2._value;
        }

        /// <summary>
        /// Determines if <see cref="SecsUInt8"/> value 1 is less than value 2.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        /// <returns>True, if less than; otherwize, false.</returns>
        public static bool operator <(SecsUInt8 val1, SecsUInt8 val2)
        {
            return val1._value < val2._value;
        }

        /// <summary>
        /// Adds the two <see cref="SecsUInt8"/> values.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static SecsInt32 operator +(SecsUInt8 lhs, SecsUInt8 rhs)
        {
            return new SecsInt32(lhs._value + rhs._value);
        }

        /// <summary>
        /// Subtracts the two <see cref="SecsUInt8"/> values.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static SecsInt32 operator -(SecsUInt8 lhs, SecsUInt8 rhs)
        {
            return new SecsInt32(lhs._value - rhs._value);
        }

        /// <summary>
        /// Subtracts the specified <see cref="SecsUInt8"/> value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static SecsInt32 operator -(SecsUInt8 val)
        {
            return 0 - val;
        }

        /// <summary>
        /// Implicit conversion from <see cref="System.Byte"/> to <see cref="SecsUInt8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator SecsUInt8(Byte value)
        {
            return new SecsUInt8(value);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SecsUInt8"/> to <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator Byte(SecsUInt8 value)
        {
            return value._value;
        }
    }
}
