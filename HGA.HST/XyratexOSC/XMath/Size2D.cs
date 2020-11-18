using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XyratexOSC.UI;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 2D size using double-precision dimensions.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DataConverter<Size2D>))]
    public class Size2D
    {
        /// <summary>
        /// The precision for determining equality
        /// </summary>
        protected int _precision = 3;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// Returns an empty size [0,0].
        /// </summary>
        /// <value>
        /// The empty.
        /// </value>
        public static Size2D Empty
        {
            get
            {
                return new Size2D();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size2D"/> class.
        /// </summary>
        public Size2D()
        {
            Width = 0;
            Height = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size2D"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size2D(double width, double height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Sets the number of significant digits (3 by default) that define the tolerance for Size equality.
        /// </summary>
        /// <param name="precision">The number of significant digits.</param>
        /// <remarks>
        /// When two Size2D's are compared the higher precision value is used for determining equality.
        /// This value allows you to define importance of positions, and if they require low/high precision for equality.
        /// </remarks>
        public void SetPrecision(int precision)
        {
            _precision = precision;
        }

        /// <summary>
        /// Determines whether the specified size is null or empty.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///   <c>true</c> if is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(Size2D size)
        {
            if (size == null)
                return true;

            if (size.Width == 0 && size.Height == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a copy of this size offset by the specified width and height.
        /// </summary>
        /// <param name="width">The width offset.</param>
        /// <param name="height">The height offset.</param>
        /// <returns></returns>
        public Size2D Offset(double width, double height)
        {
            return new Size2D(this.Width + width, this.Height + height);
        }

        /// <summary>
        /// Returns a copy of this size offset by the specified width.
        /// </summary>
        /// <param name="width">The width offset.</param>
        /// <returns></returns>
        public Size2D OffsetWidth(double width)
        {
            return Offset(width, 0);
        }

        /// <summary>
        /// Returns a copy of this size offset by the specified height.
        /// </summary>
        /// <param name="height">The height offset.</param>
        /// <returns></returns>
        public Size2D OffsetHeight(double height)
        {
            return Offset(0, height);
        }

        /// <summary>
        /// Determines if this size is equal to the specified size (using this size's precision).
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool Equals(Size2D size)
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
        public bool Equals(Size2D size, int precision)
        {
            if ((object)size == null)
                return false;

            // Return true if the fields match:
            return Width.EqualsRoughly(size.Width, precision) && Height.EqualsRoughly(size.Height, precision);
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
            Size2D size = obj as Size2D;
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
            return new { Width, Height }.GetHashCode();
        }

        /// <summary>
        /// Subtracts Size B from Size A.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Size2D operator -(Size2D a, Size2D b)
        {
            if (a == null)
                a = Size2D.Empty;

            if (b == null)
                b = Size2D.Empty;

            return a.Offset(-b.Width, -b.Height);
        }

        /// <summary>
        /// Adds Size A to Size B.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Size2D operator +(Size2D a, Size2D b)
        {
            if (a == null)
                a = Size2D.Empty;

            if (b == null)
                b = Size2D.Empty;

            return a.Offset(b.Width, b.Height);
        }

        /// <summary>
        /// Multiplies Size B by scalar A.
        /// </summary>
        /// <param name="a">Scalar A.</param>
        /// <param name="b">Size B.</param>
        /// <returns></returns>
        public static Size2D operator *(double a, Size2D b)
        {
            if (b == null)
                b = Size2D.Empty;
            
            return new Size2D(a * b.Width, a * b.Height);
        }

        /// <summary>
        /// Multiplies Size A by scalar B.
        /// </summary>
        /// <param name="a">Size A.</param>
        /// <param name="b">Scalar B.</param>
        /// <returns></returns>
        public static Size2D operator *(Size2D a, double b)
        {
            return b * a;
        }

        /// <summary>
        /// Divides Size A by scalar B.
        /// </summary>
        /// <param name="a">Size A.</param>
        /// <param name="b">Scalar B.</param>
        /// <returns></returns>
        public static Size2D operator /(Size2D a, double b)
        {
            if (a == null)
                a = Size2D.Empty;

            return new Size2D(a.Width / b, a.Height / b);
        }

        /// <summary>
        /// Determines if Size A is equal to Size B.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static bool operator ==(Size2D a, Size2D b)
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
        /// Determines if Size A is not equal to Size B.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static bool operator !=(Size2D a, Size2D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Converts this size to a <see cref="System.Drawing.Size"/>.
        /// There will be a loss in precision.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Size ToSize()
        {
            return new System.Drawing.Size((int)Width, (int)Height);
        }

        /// <summary>
        /// Converts this size to a <see cref="System.Drawing.SizeF"/>.
        /// There will be a loss in precision.
        /// </summary>
        public System.Drawing.SizeF ToSizeF()
        {
            return new System.Drawing.SizeF((float)Width, (float)Height);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0:F4},{1:F4}]", Width, Height);
        }

        #region Implicit Casts

        /// <summary>
        /// Implicit conversion of <see cref="System.Drawing.SizeF"/> to a <see cref="Size2D"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static implicit operator Size2D(System.Drawing.SizeF size)
        {
            return new Size2D(size.Width, size.Height);
        }

        /// <summary>
        /// Implicit conversion of <see cref="System.Drawing.Size"/> to a <see cref="Size2D"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static implicit operator Size2D(System.Drawing.Size size)
        {
            return new Size2D(size.Width, size.Height);
        }

        #endregion
    }
}
