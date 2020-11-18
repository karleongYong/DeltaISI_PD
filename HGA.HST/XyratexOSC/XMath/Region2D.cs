using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XyratexOSC.UI;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 2D region with double-precision location and double-precision size.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DataConverter<Region2D>))]
    public class Region2D
    {
        private Point2D _location;
        private Size2D _size;

        /// <summary>
        /// The number of significant digits (3 by default) that define the tolerance for Point equality.
        /// </summary>
        /// <value>
        /// The number of significant digits.
        /// </value>
        /// <remarks>
        /// When two Point2D's are compared the higher precision value is used for determining equality.
        /// This value allows you to define importance of positions, and if they require low/high precision for equality.
        /// </remarks>
        [Browsable(false)]
        public int Precision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top-left location of the region.
        /// </summary>
        /// <value>
        /// The top-left location.
        /// </value>
        public Point2D Location
        {
            get
            {
                if (_location == null)
                    _location = Point2D.Empty;

                return _location;
            }
            set
            {
                if (value == null)
                    _location = Point2D.Empty;
                else
                    _location = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the region.
        /// </summary>
        /// <value>
        /// The region size.
        /// </value>
        public Size2D Size
        {
            get
            {
                if (_size == null)
                    _size = Size2D.Empty;

                return _size;
            }
            set
            {
                if (value == null)
                    _size = Size2D.Empty;
                else
                    _size = value;
            }
        }

        /// <summary>
        /// Gets or sets the left X coordinate of the region.
        /// </summary>
        /// <value>
        /// The left coordinate.
        /// </value>
        public double X
        {
            get
            {
                return Location.X;
            }
            set
            {
                Location.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the top Y coordinate of the region.
        /// </summary>
        /// <value>
        /// The top coordinate.
        /// </value>
        public double Y
        {
            get
            {
                return Location.Y;
            }
            set
            {
                Location.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the top Y coordinate of the region.
        /// </summary>
        /// <value>
        /// The top coordinate.
        /// </value>
        public double Top
        {
            get
            {
                return Location.Y;
            }
            set
            {
                Location.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom Y coordinate of the region.
        /// </summary>
        /// <value>
        /// The bottom coordinate.
        /// </value>
        public double Bottom
        {
            get
            {
                return Location.Y + Size.Height;
            }
            set
            {
                Location.Y = value - Size.Height;
            }
        }

        /// <summary>
        /// Gets or sets the left X coordinate of the region.
        /// </summary>
        /// <value>
        /// The left coordinate.
        /// </value>
        public double Left
        {
            get
            {
                return Location.X;
            }
            set
            {
                Location.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the right X coordinate of the region.
        /// </summary>
        /// <value>
        /// The right coordinate.
        /// </value>
        public double Right
        {
            get
            {
                return Location.X + Size.Width;
            }
            set
            {
                Location.X = value - Size.Width;
            }
        }

        /// <summary>
        /// Gets the point in the center of the region.
        /// </summary>
        /// <value>
        /// The center point.
        /// </value>
        public Point2D Middle
        {
            get
            {
                return new Point2D(Location.X + (Size.Width / 2), Location.Y + (Size.Height / 2));
            }
        }

        /// <summary>
        /// Gets or sets the width of the region.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get
            {
                return Size.Width;
            }
            set
            {
                Size.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the region.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get
            {
                return Size.Height;
            }
            set
            {
                Size.Height = value;
            }
        }

        /// <summary>
        /// Represents an Empty region (0,0) [0,0].
        /// </summary>
        /// <value>
        /// The empty.
        /// </value>
        public static Region2D Empty
        {
            get
            {
                return new Region2D();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region2D"/> class.
        /// </summary>
        public Region2D()
        {
            _location = Point2D.Empty;
            _size = Size2D.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region2D"/> class.
        /// </summary>
        /// <param name="x">The left location.</param>
        /// <param name="y">The top location.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Region2D(double x, double y, double width, double height)
        {
            _location = new Point2D(x, y);
            _size = new Size2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region2D"/> class.
        /// </summary>
        /// <param name="location">The top-left location.</param>
        /// <param name="size">The size.</param>
        public Region2D(Point2D location, Size2D size)
        {
            _location = location;
            _size = size;
        }

        /// <summary>
        /// Determines if this region intersects the specified region.
        /// </summary>
        /// <param name="region">The other region.</param>
        /// <returns></returns>
        public bool Intersects(Region2D region)
        {
            bool xOverlap = valueInRange(this.X, region.X, region.X + region.Width) ||
                            valueInRange(region.X, this.X, this.X + this.Width);

            bool yOverlap = valueInRange(this.Y, region.Y, region.Y + region.Height) ||
                            valueInRange(region.Y, this.Y, this.Y + this.Height);

            return xOverlap && yOverlap;
        }

        private bool valueInRange(double value, double min, double max)
        { 
            return (value >= min) && (value <= max); 
        }

        /// <summary>
        /// Determines whether the specified point is contained within the region.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the specified point is within the region; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Point2D point)
        {
            if (point.X < X || point.Y < Y)
                return false;

            if (point.X > X + Width || point.Y > Y + Height)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if this region is equal to the specified region (using this region's <see cref="Precision"/>.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public bool Equals(Region2D region)
        {
            if ((object)region == null)
                return false;

            return Equals(region, Math.Max(region.Precision, this.Precision));
        }

        /// <summary>
        /// Determines if this region is equal to the specified region using the specified precision.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public bool Equals(Region2D region, int precision)
        {
            if ((object)region == null)
                return false;

            // Return true if the fields match:
            return Location.Equals(region.Location, precision) && Size.Equals(region.Size, precision);
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
            Region2D region = obj as Region2D;
            if ((object)region == null)
                return false;

            return Equals(region);
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
            return new { Location, Size }.GetHashCode();
        }

        /// <summary>
        /// Determines if Region A is equal to Region B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static bool operator ==(Region2D a, Region2D b)
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
        /// Determines if Region A is not equal to Region B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static bool operator !=(Region2D a, Region2D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Multiplies the specified region by the specified constant.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="constant">The constant.</param>
        public static Region2D Multiply(Region2D region, double constant)
        {
            return new Region2D(region.X * constant, region.Y * constant, region.Width * constant, region.Height * constant);
        }

        /// <summary>
        /// Multiplies the specified region by the specified constant.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="constant">The constant.</param>
        public static Region2D operator *(Region2D region, double constant)
        {
            return Multiply(region, constant);
        }

        /// <summary>
        /// Multiplies the specified region by the specified constant.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <param name="region">The region.</param>
        public static Region2D operator *(double constant, Region2D region)
        {
            return Multiply(region, constant);
        }

        /// <summary>
        /// Divides the specified region by the specified constant.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="constant">The constant.</param>
        public static Region2D Divide(Region2D region, double constant)
        {
            return new Region2D(region.X / constant, region.Y / constant, region.Width / constant, region.Height / constant);
        }

        /// <summary>
        /// Divides the specified region by the specified constant.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="constant">The constant.</param>
        public static Region2D operator /(Region2D region, double constant)
        {
            return Divide(region, constant);
        }

        /// <summary>
        /// Converts this to a <see cref="System.Drawing.Rectangle"/>.
        /// There will be a loss in precision.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Rectangle ToRectangle()
        {
            return new System.Drawing.Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        /// <summary>
        /// Converts this to a <see cref="System.Drawing.RectangleF"/>.
        /// There will be a loss in precision.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.RectangleF ToRectangleF()
        {
            return new System.Drawing.RectangleF((float)X, (float)Y, (float)Width, (float)Height);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("({0:F4},{1:F4}) [{2:F4},{3:F4}]", X, Y, Width, Height);
        }

        #region Implicit Casts

        /// <summary>
        /// Implicit conversion of a <see cref="System.Drawing.RectangleF"/> to a <see cref="Region2D"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns></returns>
        public static implicit operator Region2D(System.Drawing.RectangleF rectangle)
        { 
            return new Region2D(rectangle.Location, rectangle.Size);
        }

        /// <summary>
        /// Implicit conversion of a <see cref="System.Drawing.Rectangle"/> to a <see cref="Region2D"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns></returns>
        public static implicit operator Region2D(System.Drawing.Rectangle rectangle)
        {
            return new Region2D(rectangle.Location, rectangle.Size);
        }

        #endregion
    }
}
