using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Represents an 8-bit ASCII Character
    /// </summary>
    public struct SecsASCIIChar : IComparable, IComparable<SecsASCIIChar>
    {
        /// <summary>
        /// An ASCII space character
        /// </summary>
        public static readonly SecsASCIIChar Space = new SecsASCIIChar((byte)32);

        private readonly Byte _value;

        public SecsASCIIChar(Byte value)
        {
            _value = value;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo((SecsASCIIChar)obj);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(SecsASCIIChar value)
        {
            return _value.CompareTo(value._value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SecsASCIIChar))
                return false;

            SecsASCIIChar sobj = (SecsASCIIChar)obj;
            return Equals(sobj);
        }

        public bool Equals(SecsASCIIChar value)
        {
            return _value.Equals(value._value);
        }

        public bool IsControl()
        {
            return SecsASCIIChar.IsControl(this._value);
        }

        public bool IsDigit()
        {
            return SecsASCIIChar.IsDigit(this._value);
        }

        public bool IsLetter()
        {
            return SecsASCIIChar.IsLetter(this._value);
        }

        public bool IsLetterOrDigit()
        {
            return SecsASCIIChar.IsLetterOrDigit(this._value);
        }

        public bool IsLower()
        {
            return SecsASCIIChar.IsLower(this._value);
        }

        public bool IsPunctuation()
        {
            return SecsASCIIChar.IsPunctuation(this._value);
        }

        public bool IsSymbol()
        {
            return SecsASCIIChar.IsSymbol(this._value);
        }

        public bool IsUpper()
        {
            return SecsASCIIChar.IsUpper(this._value);
        }

        public bool IsWhitespace()
        {
            return SecsASCIIChar.IsWhitespace(this._value);
        }

        public SecsASCIIChar ToLower()
        {
            if (this.IsUpper())
                return new SecsASCIIChar((byte)(_value + 32));

            return this;
        }

        public byte ToByte()
        {
            return _value;
        }

        public char ToChar()
        {
            return (char)_value;
        }

        public override string ToString()
        {
            return this.ToChar().ToString();
        }

        public SecsASCII ToASCIIString()
        {
            return new SecsASCII(new byte[] { _value });
        }

        public SecsASCIIChar ToUpper()
        {
            if (this.IsLower())
                return new SecsASCIIChar((byte)(_value - 32));

            return this;
        }

        public static bool IsControl(Byte value)
        {
            return (value >= 0 && value <= 31) ||
                value == 127;
        }

        public static bool IsDigit(Byte value)
        {
            return value >= 48 && value <= 57;
        }

        public static bool IsLetter(Byte value)
        {
            return (value >= 65 && value <= 90) ||
                (value >= 97 && value <= 122);
        }

        public static bool IsLetterOrDigit(Byte value)
        {
            return (value >= 48 && value <= 57) ||
                (value >= 65 && value <= 90) ||
                (value >= 97 && value <= 122);
        }

        public static bool IsLower(Byte value)
        {
            return value >= 97 && value <= 122;
        }

        public static bool IsPunctuation(Byte value)
        {
            return (value >= 33 && value <= 35) ||
                (value >= 37 && value <= 42) ||
                (value >= 44 && value <= 47) ||
                (value >= 58 && value <= 59) ||
                (value >= 63 && value <= 64) ||
                (value >= 91 && value <= 93) ||
                value == 95 ||
                value == 123 ||
                value == 125;
        }

        public static bool IsSymbol(Byte value)
        {
            return value == 36 ||
                value == 43 ||
                (value >= 60 && value <= 62) ||
                value == 94 ||
                value == 96 ||
                value == 124 ||
                value == 126;
        }

        public static bool IsUpper(Byte value)
        {
            return value >= 65 && value <= 90;
        }

        public static bool IsWhitespace(Byte value)
        {
            return value == 0 || (value >= 9 && value <= 13) || value == 32;
        }

        public static byte ToLower(byte value)
        {
            if (SecsASCIIChar.IsUpper(value)) 
                return (byte)(value - 32);

            return value;
        }

        public static byte ToUpper(byte value)
        {
            if (SecsASCIIChar.IsLower(value)) 
                return (byte)(value + 32);

            return value;
        }

        /// <summary>
        /// Implicit conversion from <see cref="System.Byte"/> to <see cref="SecsASCIIChar"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator SecsASCIIChar(Byte value)
        {
            return new SecsASCIIChar(value);
        }

        /// <summary>
        /// Implicit conversion from <see cref="SecsASCIIChar"/> to <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator Byte(SecsASCIIChar value)
        {
            return value._value;
        }
    }
}
