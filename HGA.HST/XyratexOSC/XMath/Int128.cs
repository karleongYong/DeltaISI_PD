using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    //------------------------------------------------------------------------------
    // Int128 struct (enables safe math on signed 64bit integers)
    // eg Int128 val1((Int64)9223372036854775807); //ie 2^63 -1
    //    Int128 val2((Int64)9223372036854775807);
    //    Int128 val3 = val1 * val2;
    //    val3.ToString => "85070591730234615847396907784232501249" (8.5e+37)
    //------------------------------------------------------------------------------

    /// <summary>
    /// Int128 struct (enables safe math on signed 64bit integers).
    /// </summary>
    /// <remarks>
    /// eg. Int128 val1((Int64)9223372036854775807); //ie 2^63 -1
    ///     Int128 val2((Int64)9223372036854775807);
    ///     Int128 val3 = val1 * val2;
    ///     val3.ToString => "85070591730234615847396907784232501249" (8.5e+37)
    /// </remarks>
    public struct Int128
    {
        private Int64 hi;
        private Int64 lo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int128"/> struct.
        /// </summary>
        /// <param name="lo">The 64-bit lo value.</param>
        public Int128(Int64 lo)
        {
            this.lo = lo;
            if (lo < 0)
                this.hi = -1;
            else
                this.hi = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int128"/> struct.
        /// </summary>
        /// <param name="lo">The 64-bit lo value.</param>
        /// <param name="hi">The 64-bit hi value.</param>
        public Int128(Int64 lo, Int64 hi)
        {
            this.lo = lo;
            this.hi = hi;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int128"/> struct.
        /// </summary>
        /// <param name="val">The 128-bit value.</param>
        public Int128(Int128 val)
        {
            hi = val.hi;
            lo = val.lo;
        }

        /// <summary>
        /// Determines whether this 128-bit Integer is negative.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this 128-bit Integer is negative; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNegative()
        {
            return hi < 0;
        }

        /// <summary>
        /// Determines if the two 128-bit integers are equal.
        /// </summary>
        /// <param name="val1">Int128 value 1</param>
        /// <param name="val2">Int128 value 2</param>
        /// <returns>True, if equal; otherwise, false.</returns>
        public static bool operator ==(Int128 val1, Int128 val2)
        {
            if ((object)val1 == (object)val2) return true;
            else if ((object)val1 == null || (object)val2 == null) return false;
            return (val1.hi == val2.hi && val1.lo == val2.lo);
        }

        /// <summary>
        /// Determines if the two 128-bit integers are not equal.
        /// </summary>
        /// <param name="val1">Int128 value 1</param>
        /// <param name="val2">Int128 value 2</param>
        /// <returns>True, if not equal; otherwise, false.</returns>
        public static bool operator !=(Int128 val1, Int128 val2)
        {
            return !(val1 == val2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this 128-bit Integer instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(System.Object obj)
        {
            if (obj == null || !(obj is Int128))
                return false;
            Int128 i128 = (Int128)obj;
            return (i128.hi == hi && i128.lo == lo);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return hi.GetHashCode() ^ lo.GetHashCode();
        }

        /// <summary>
        /// Determines if 128-bit integer value 1 is greater than value 2.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        /// <returns>True, if greater than; otherwize, false.</returns>
        public static bool operator >(Int128 val1, Int128 val2)
        {
            if (val1.hi != val2.hi)
                return val1.hi > val2.hi;
            else
                return val1.lo > val2.lo;
        }

        /// <summary>
        /// Determines if 128-bit integer value 1 is less than value 2.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        /// <returns>True, if less than; otherwize, false.</returns>
        public static bool operator <(Int128 val1, Int128 val2)
        {
            if (val1.hi != val2.hi)
                return val1.hi < val2.hi;
            else
                return val1.lo < val2.lo;
        }

        /// <summary>
        /// Adds the two 128-bit integer values.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static Int128 operator +(Int128 lhs, Int128 rhs)
        {
            lhs.hi += rhs.hi;
            lhs.lo += rhs.lo;
            if ((UInt64)lhs.lo < (UInt64)rhs.lo) lhs.hi++;
            return lhs;
        }

        /// <summary>
        /// Subtracts the two 128-bit integer values.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static Int128 operator -(Int128 lhs, Int128 rhs)
        {
            return lhs + -rhs;
        }

        /// <summary>
        /// Subtracts the specified 128-bit integer value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static Int128 operator -(Int128 val)
        {
            if (val.lo == 0)
            {
                if (val.hi == 0) return val;
                return new Int128(0, -val.hi);
            }
            else return new Int128(-val.lo, ~val.hi);
        }

        /// <summary>
        /// Multiplies the two specified Int128s.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static Int128 Int128Mul(Int64 lhs, Int64 rhs)
        {
            bool negate = (lhs < 0) != (rhs < 0);
            if (lhs < 0) lhs = -lhs;
            if (rhs < 0) rhs = -rhs;
            UInt64 int1Hi = (UInt64)lhs >> 32;
            UInt64 int1Lo = (UInt64)lhs & 0xFFFFFFFF;
            UInt64 int2Hi = (UInt64)rhs >> 32;
            UInt64 int2Lo = (UInt64)rhs & 0xFFFFFFFF;

            UInt64 a = int1Hi * int2Hi;
            UInt64 b = int1Lo * int2Lo;
            UInt64 c = int1Hi * int2Lo + int1Lo * int2Hi;

            Int64 lo, hi;
            hi = (Int64)(a + (c >> 32));

            lo = (Int64)(c << 32);
            lo += (Int64)b;
            if ((UInt64)lo < b) hi++;
            var result = new Int128(lo, hi);
            return negate ? -result : result;
        }

        /// <summary>
        /// Divides the two specified Int128s.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns></returns>
        public static Int128 operator /(Int128 lhs, Int128 rhs)
        {
            if (rhs.lo == 0 && rhs.hi == 0)
                throw new Exception("Int128: divide by zero");

            bool negate = (rhs.hi < 0) != (lhs.hi < 0);
            Int128 result = new Int128(lhs), denom = new Int128(rhs);
            if (result.hi < 0) result = -result;
            if (denom.hi < 0) denom = -denom;
            if (denom > result) return new Int128(0); //result is only a fraction of 1
            denom = -denom;

            Int128 p = new Int128(0), p2 = new Int128(0);
            for (int i = 0; i < 128; ++i)
            {
                p.hi = p.hi << 1;
                if (p.lo < 0) p.hi++;
                p.lo = (Int64)p.lo << 1;
                if (result.hi < 0) p.lo++;
                result.hi = result.hi << 1;
                if (result.lo < 0) result.hi++;
                result.lo = (Int64)result.lo << 1;
                if (p.hi >= 0)
                {
                    p += denom;
                    result.lo++;
                }
            }
            return negate ? -result : result;
        }

        /// <summary>
        /// Converts this 128-bit integer to a <see cref="double"/>
        /// </summary>
        /// <returns>A double conversion of this integer.</returns>
        public double ToDouble()
        {
            const double shift64 = 18446744073709551616.0; //2^64
            const double bit64 = 9223372036854775808.0;
            if (hi < 0)
            {
                Int128 tmp = new Int128(this);
                tmp = -tmp;
                if (tmp.lo < 0)
                    return (double)tmp.lo - bit64 - tmp.hi * shift64;
                else
                    return -(double)tmp.lo - tmp.hi * shift64;
            }
            else if (lo < 0)
                return -(double)lo + bit64 + hi * shift64;
            else
                return (double)lo + (double)hi * shift64;
        }
    }
}
