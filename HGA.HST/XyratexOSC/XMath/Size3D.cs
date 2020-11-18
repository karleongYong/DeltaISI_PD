using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XyratexOSC.UI;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 3D size of double precision
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DataConverter<Size3D>))]
    public class Size3D : Size2D
    {
        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public double Depth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an empty 3D size.
        /// </summary>
        /// <value>
        /// The empty 3D size.
        /// </value>
        public static new Size3D Empty
        {
            get
            {
                return new Size3D();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size3D"/> class.
        /// </summary>
        public Size3D()
            : base()
        {
            Depth = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size3D"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        public Size3D(double width, double height, double depth)
            : base(width, height)
        {
            Depth = Depth;
        }

        /// <summary>
        /// Determines whether the specified 3D size is null or empty.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///   <c>true</c> if is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(Size3D size)
        {
            if (size == null)
                return true;

            if (size.Width == 0 && size.Height == 0 && size.Depth == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Creates a copy of this size with the specified offsets
        /// </summary>
        /// <param name="width">The width offset.</param>
        /// <param name="height">The height offset.</param>
        /// <param name="depth">The depth offset.</param>
        /// <returns></returns>
        public Size3D Offset(double width, double height, double depth)
        {
            return ((Size3D)this.Offset(width, height)).OffsetDepth(depth);
        }

        /// <summary>
        /// Creates a copy of this size with the specified depth offset
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        public Size3D OffsetDepth(double depth)
        {
            return Offset(0, 0, depth);
        }

        /// <summary>
        /// Determines if this size is equal to the specified size (using default precision).
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool Equals(Size3D size)
        {
            if ((object)size == null)
                return false;

            return Equals(size, Math.Max(size._precision, this._precision));
        }

        /// <summary>
        /// Determines if this size is equal to the specified size using the specified precision.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public bool Equals(Size3D size, int precision)
        {
            if ((object)size == null)
                return false;

            // Return true if the fields match:
            return base.Equals((Size2D)size, precision) && Depth.EqualsRoughly(size.Depth, precision);
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
            Size3D size = obj as Size3D;
            if ((object)size == null)
                return false;

            return Equals(size);
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
            return new { Width, Height, Depth }.GetHashCode();
        }

        /// <summary>
        /// Subtracts two 3D sizes.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Size3D operator -(Size3D a, Size3D b)
        {
            if (a == null)
                a = Size3D.Empty;

            if (b == null)
                b = Size3D.Empty;

            return a.Offset(-b.Width, -b.Height, -b.Depth);
        }

        /// <summary>
        /// Adds two 3D sizes.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Size3D operator +(Size3D a, Size3D b)
        {
            if (a == null)
                a = Size3D.Empty;

            if (b == null)
                b = Size3D.Empty;

            return a.Offset(b.Width, b.Height, b.Depth);
        }

        /// <summary>
        /// Determines equality between two 3D sizes.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static bool operator ==(Size3D a, Size3D b)
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
        /// Determines inequality between two 3D sizes.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static bool operator !=(Size3D a, Size3D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0:F4},{1:F4},{2:F4}]", Width, Height, Depth);
        }
    }
}
