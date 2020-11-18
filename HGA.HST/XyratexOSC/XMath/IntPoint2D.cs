using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// Represents a 2D point of 64-bit integers. Avoids floating point arithmetic with 64-bit precision.
    /// </summary>
    public class IntPoint2D
    {
        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        /// <value>
        /// The X coordinate.
        /// </value>
        public Int64 X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        /// <value>
        /// The Y coordinate.
        /// </value>
        public Int64 Y
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPoint2D"/> class.
        /// </summary>
        public IntPoint2D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPoint2D"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public IntPoint2D(Int64 x, Int64 y)
        {
            X = x; 
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPoint2D"/> class.
        /// </summary>
        /// <param name="pt">The point to copy.</param>
        public IntPoint2D(IntPoint2D pt)
        {
            X = pt.X; 
            Y = pt.Y;
        }

        /// <summary>
        /// Returns a copy of this point offset by X and Y from the specified point.
        /// </summary>
        /// <param name="offsets">The specified offsets.</param>
        /// <returns>An offset copy of this point.</returns>
        public IntPoint2D Offset(IntPoint2D offsets)
        {
            return Offset(offsets.X, offsets.Y);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified X and Y.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>An offset copy of this point.</returns>
        public IntPoint2D Offset(Int64 x, Int64 y)
        {
            return new IntPoint2D(this.X + x, this.Y + y);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified X.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <returns>An offset copy of this point.</returns>
        public IntPoint2D OffsetX(Int64 x)
        {
            return Offset(x, 0);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified Y.
        /// </summary>
        /// <param name="y">The y offset.</param>
        /// <returns>An offset copy of this point.</returns>
        public IntPoint2D OffsetY(Int64 y)
        {
            return Offset(0, y);
        }

        /// <summary>
        /// Determines if this point is equal to the specified point.
        /// </summary>
        /// <param name="point">The other point.</param>
        /// <returns>True, if equal; otherwise, false</returns>
        public bool Equals(IntPoint2D point)
        {
            if ((object)point == null)
                return false;

            // Return true if the fields match:
            return X == point.X && Y == point.Y;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(Object obj)
        {
            Point2D point = obj as Point2D;
            if ((object)point == null)
                return false;

            return Equals(point);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            //Return hash code from anonymous type
            return new { X, Y }.GetHashCode();
        }

        /// <summary>
        /// Returns the distance between the two specified points.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns>The distance between the two points.</returns>
        public static double Delta(IntPoint2D point1, IntPoint2D point2)
        {
            double part1 = Math.Pow((point2.X - point1.X), 2);
            double part2 = Math.Pow((point2.Y - point1.Y), 2);
            return Math.Sqrt(part1 + part2);
        }

        /// <summary>
        /// Subtracts the specified value.
        /// </summary>
        /// <param name="a">Value to subtract</param>
        /// <returns></returns>
        public static IntPoint2D operator -(IntPoint2D a)
        {
            if (a == null)
                a = new IntPoint2D();

            return new IntPoint2D(-a.X, -a.Y);
        }

        /// <summary>
        /// Subtracts b from a.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static IntPoint2D operator -(IntPoint2D a, IntPoint2D b)
        {
            if (a == null)
                a = new IntPoint2D();

            if (b == null)
                b = new IntPoint2D();

            return a.Offset(-b.X, -b.Y);
        }

        /// <summary>
        /// Adds a to b.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static IntPoint2D operator +(IntPoint2D a, IntPoint2D b)
        {
            if (a == null)
                a = new IntPoint2D();

            if (b == null)
                b = new IntPoint2D();

            return a.Offset(b.X, b.Y);
        }

        /// <summary>
        /// Multiplies A and B
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static IntPoint2D operator *(Int64 a, IntPoint2D b)
        {
            if (b == null)
                b = new IntPoint2D();

            return new IntPoint2D(a * b.X, a * b.Y);
        }

        /// <summary>
        /// Multiplies A and B
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static IntPoint2D operator *(IntPoint2D a, Int64 b)
        {
            return b * a;
        }

        /// <summary>
        /// Divides B from A
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static IntPoint2D operator /(IntPoint2D a, Int64 b)
        {
            if (a == null)
                a = new IntPoint2D();

            return new IntPoint2D(a.X / b, a.Y / b);
        }

        /// <summary>
        /// Determines if A equals B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static bool operator ==(IntPoint2D a, IntPoint2D b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
                return false;

            // Return true if the fields match:
            return a.Equals(b);
        }

        /// <summary>
        /// Determines if A is not equal to B
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static bool operator !=(IntPoint2D a, IntPoint2D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Converts this to a <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point((int)X, (int)Y);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("({0:F4},{1:F4})", X, Y);
        }

        #region Implicit Casts

        /// <summary>
        /// Implicit cast from <see cref="System.Drawing.Point"/> to a 64-bit precision point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static implicit operator IntPoint2D(System.Drawing.Point point)
        {
            return new IntPoint2D(point.X, point.Y);
        }

        #endregion
    }
}
