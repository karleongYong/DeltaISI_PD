using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Secs II boolean data structure
    /// </summary>
    [Serializable]
    public sealed class SecsBool : ISecsValue, IComparable<SecsBool>
    {
        private readonly Boolean _value;
        private readonly string _format = "BOOL";
        private readonly int _length = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsBool"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SecsBool(Boolean value)
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
        /// Compares this instance to a specified object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo((SecsBool)obj);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="SecsBool"/> object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(SecsBool value)
        {
            if (value == null)
                return 1;

            return _value.CompareTo(value._value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this <see cref="SecsBool"/> instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this <see cref="SecsBool"/> instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this <see cref="SecsBool"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SecsBool))
                return false;

            SecsBool sobj = obj as SecsBool;
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
        /// Determines if the two <see cref="SecsBool"/> values are equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if equal; otherwise, false.</returns>
        public static bool operator ==(SecsBool val1, SecsBool val2)
        {
            if ((object)val1 == (object)val2)
                return true;
            else if ((object)val1 == null || (object)val2 == null)
                return false;

            return (val1._value == val2._value);
        }

        /// <summary>
        /// Determines if the two <see cref="SecsBool"/> values are not equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if not equal; otherwise, false.</returns>
        public static bool operator !=(SecsBool val1, SecsBool val2)
        {
            return !(val1 == val2);
        }

        /// <summary>
        /// Performs logical negation on the <see cref="SecsBool"/> value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>True, if not equal; otherwise, false.</returns>
        public static bool operator !(SecsBool value)
        {
            return new SecsBool(!value._value);
        }
        
        /// <summary>
        /// Implicit conversion from <see cref="System.Boolean"/> to <see cref="SecsBool"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator SecsBool(Boolean value)
        {
            return new SecsBool(value);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SecsBool"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator Boolean(SecsBool value)
        {
            return value._value;
        }
    }
}
