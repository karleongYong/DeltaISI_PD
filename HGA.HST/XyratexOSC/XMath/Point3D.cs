using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XyratexOSC.UI;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 3D point with double precision coordinates.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DataConverter<Point3D>))]
    public class Point3D : Point2D
    {
        /// <summary>
        /// Gets or sets the Z coordinate.
        /// </summary>
        /// <value>
        /// The Z coordinate.
        /// </value>
        public double Z
        {
            get;
            set;
        }

        /// <summary>
        /// Represents an empty point (0,0,0).
        /// </summary>
        /// <value>
        /// The empty point.
        /// </value>
        public static new Point3D Empty
        {
            get
            {
                return new Point3D();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        public Point3D()
            : base()
        {
            Z = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Point3D(double x, double y, double z)
            : base(x, y)
        {
            Z = z;
        }

        /// <summary>
        /// Determines whether the specified point is null or empty.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(Point3D point)
        {
            if (point == null)
                return true;

            if (point.X == 0 && point.Y == 0 && point.Z == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a copy of this point offset by the X,Y,Z coordinates of the specified point.
        /// </summary>
        /// <param name="offsets">The point offsets.</param>
        /// <returns></returns>
        public Point3D Offset(Point3D offsets)
        {
            return Offset(offsets.X, offsets.Y, offsets.Z);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified X,Y,Z offsets.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <param name="z">The z offset.</param>
        /// <returns></returns>
        public Point3D Offset(double x, double y, double z)
        {
            return new Point3D(this.X + x, this.Y + y, this.Z + z);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified Z offset.
        /// </summary>
        /// <param name="z">The z offset.</param>
        /// <returns></returns>
        public Point3D OffsetZ(double z)
        {
            return Offset(0, 0, z);
        }

        /// <summary>
        /// Determines if this point is equal to the specified point (using this Point's precision).
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool Equals(Point3D point)
        {
            if ((object)point == null)
                return false;

            return Equals(point, Math.Max(point._precision, this._precision));
        }

        /// <summary>
        /// Determines if this point is equal to the specified point using the specified precision.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public bool Equals(Point3D point, int precision)
        {
            if ((object)point == null)
                return false;

            // Return true if the fields match:
            return base.Equals((Point2D)point, precision) && Z.EqualsRoughly(point.Z, precision);
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
            Point3D point = obj as Point3D;
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
            return new { X, Y, Z }.GetHashCode();
        }

        /// <summary>
        /// Determines the distance between Point 1 and Point 2.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns></returns>
        public static double Delta(Point3D point1, Point3D point2)
        {
            double part1 = Math.Pow((point2.X - point1.X), 2);
            double part2 = Math.Pow((point2.Y - point1.Y), 2);
            double part3 = Math.Pow((point2.Z - point1.Z), 2);
            return Math.Sqrt(part1 + part2 + part3);
        }

        /// <summary>
        /// Negates the specified point.
        /// </summary>
        /// <param name="a">A</param>
        public static Point3D operator -(Point3D a)
        {
            if (a == null)
                a = Point3D.Empty;

            return new Point3D(-a.X, -a.Y, -a.Z);
        }

        /// <summary>
        /// Subtracts Point B from Point A.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static Point3D operator -(Point3D a, Point3D b)
        {
            if (a == null)
                a = Point3D.Empty;

            if (b == null)
                b = Point3D.Empty;

            return a.Offset(-b.X, -b.Y, -b.Z);
        }

        /// <summary>
        /// Adds Point A and Point B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static Point3D operator +(Point3D a, Point3D b)
        {
            if (a == null)
                a = Point3D.Empty;

            if (b == null)
                b = Point3D.Empty;

            return a.Offset(b.X, b.Y, b.Z);
        }

        /// <summary>
        /// Determines if Point A equals Point B (using Point A precision).
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns></returns>
        public static bool operator ==(Point3D a, Point3D b)
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
        /// Determines if Point A is not equal to Point B (using Point A precision).
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns></returns>
        public static bool operator !=(Point3D a, Point3D b)
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
            return string.Format("({0:F4},{1:F4},{2:F4})", X, Y, Z);
        }
    }
}
