using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Secs II 64-bit unsigned integer data structure
    /// </summary>
    [Serializable]
    public sealed class SecsUInt64 : ISecsValue, IComparable<SecsUInt64>
    {
        private readonly UInt64 _value;
        private const string _format = "U8";
        private const int _length = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsUInt64"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SecsUInt64(UInt64 value)
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
            return CompareTo((SecsUInt64)obj);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="SecsUInt64"/> object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(SecsUInt64 value)
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
        /// Determines whether the specified <see cref="System.Object" /> is equal to this <see cref="SecsUInt64"/> instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this <see cref="SecsUInt64"/> instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this <see cref="SecsUInt64"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SecsUInt64))
                return false;

            SecsUInt64 sobj = obj as SecsUInt64;
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
        /// Determines if the two <see cref="SecsUInt64"/> values are equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if equal; otherwise, false.</returns>
        public static bool operator ==(SecsUInt64 val1, SecsUInt64 val2)
        {
            if ((object)val1 == (object)val2) 
                return true;
            else if ((object)val1 == null || (object)val2 == null)
                return false;

            return (val1._value == val2._value);
        }

        /// <summary>
        /// Determines if the two <see cref="SecsUInt64"/> values are not equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if not equal; otherwise, false.</returns>
        public static bool operator !=(SecsUInt64 val1, SecsUInt64 val2)
        {
            return !(val1 == val2);
        }

        /// <summary>
        /// Determines if <see cref="SecsUInt64"/> value 1 is greater than value 2.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        /// <returns>True, if greater than; otherwize, false.</returns>
        public static bool operator >(SecsUInt64 val1, SecsUInt64 val2)
        {
            return val1._value > val2._value;
        }

        /// <summary>
        /// Determines if <see cref="SecsUInt64"/> value 1 is less than value 2.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        /// <returns>True, if less than; otherwize, false.</returns>
        public static bool operator <(SecsUInt64 val1, SecsUInt64 val2)
        {
            return val1._value < val2._value;
        }

        /// <summary>
        /// Adds the two <see cref="SecsUInt64"/> values.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static SecsUInt64 operator +(SecsUInt64 lhs, SecsUInt64 rhs)
        {
            return new SecsUInt64(lhs._value + rhs._value);
        }

        /// <summary>
        /// Subtracts the two <see cref="SecsUInt64"/> values.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static SecsUInt64 operator -(SecsUInt64 lhs, SecsUInt64 rhs)
        {
            return new SecsUInt64(lhs._value - rhs._value);
        }

        /// <summary>
        /// Subtracts the specified <see cref="SecsUInt64"/> value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static SecsUInt64 operator -(SecsUInt64 val)
        {
            return 0 - val;
        }

        /// <summary>
        /// Implicit conversion from <see cref="System.UInt64"/> to <see cref="SecsUInt64"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator SecsUInt64(UInt64 value)
        {
            return new SecsUInt64(value);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SecsUInt64"/> to <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator UInt64(SecsUInt64 value)
        {
            return value._value;
        }
    }
}
