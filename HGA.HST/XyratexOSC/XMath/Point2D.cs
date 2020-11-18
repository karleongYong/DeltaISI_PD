using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XyratexOSC.UI;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 2D point with double precision coordinates.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DataConverter<Point2D>))]
    public class Point2D
    {
        /// <summary>
        /// The precision for testing equality.
        /// </summary>
        protected int _precision = 3;

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        /// <value>
        /// The X coordinate.
        /// </value>
        public double X
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
        public double Y
        {
            get;
            set;
        }

        /// <summary>
        /// Represents an empty point (0,0).
        /// </summary>
        /// <value>
        /// The empty point.
        /// </value>
        public static Point2D Empty
        {
            get
            {
                return new Point2D();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class.
        /// </summary>
        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class.
        /// </summary>
        /// <param name="point">The point to copy.</param>
        public Point2D(Point2D point)
        {
            _precision = point._precision;
            X = point.X;
            Y = point.Y;
        }

        /// <summary>
        /// Sets the number of significant digits (3 by default) that define the tolerance for Point equality.
        /// </summary>
        /// <param name="precision">The number of significant digits.</param>
        /// <remarks>
        /// When two Point2D's are compared the higher precision value is used for determining equality.
        /// This value allows you to define importance of positions, and if they require low/high precision for equality.
        /// </remarks>
        public void SetPrecision(int precision)
        {
            _precision = precision;
        }

        /// <summary>
        /// Determines whether the specified point is null or empty.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the point is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(Point2D point)
        {
            if (point == null)
                return true;

            if (point.X == 0 && point.Y == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a copy of this point offset by the X,Y coordinates of the specified point.
        /// </summary>
        /// <param name="offsets">The point offsets.</param>
        /// <returns></returns>
        public Point2D Offset(Point2D offsets)
        {
            return Offset(offsets.X, offsets.Y);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns></returns>
        public Point2D Offset(double x, double y)
        {
            return new Point2D(this.X + x, this.Y + y);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified X coordinate.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <returns></returns>
        public Point2D OffsetX(double x)
        {
            return Offset(x, 0);
        }

        /// <summary>
        /// Returns a copy of this point offset by the specified Y coordinate.
        /// </summary>
        /// <param name="y">The y offset.</param>
        /// <returns></returns>
        public Point2D OffsetY(double y)
        {
            return Offset(0, y);
        }

        /// <summary>
        /// Determines if this point is equal to the specified point using this point's precision.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool Equals(Point2D point)
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
        public bool Equals(Point2D point, int precision)
        {
            if ((object)point == null)
                return false;
            
            // Return true if the fields match:
            return X.EqualsRoughly(point.X, precision) && Y.EqualsRoughly(point.Y, precision);
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
        /// Computes the dot product AB . AC
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="pointC"></param>
        /// <returns></returns>
        public static double DotProduct(Point2D pointA, Point2D pointB, Point2D pointC)
        {
            double[] AB = new double[2];
            double[] BC = new double[2];
            AB[0] = pointB.X - pointA.X;
            AB[1] = pointB.Y - pointA.Y;
            BC[0] = pointC.X - pointB.X;
            BC[1] = pointC.Y - pointB.Y;

            double dot = AB[0] * BC[0] + AB[1] * BC[1];
            return dot;
        }

        /// <summary>
        /// Computes the cross product AB x AC
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="pointC"></param>
        /// <returns></returns>
        public static double CrossProduct(Point2D pointA, Point2D pointB, Point2D pointC)
        {
            double[] AB = new double[2];
            double[] AC = new double[2];
            AB[0] = pointB.X - pointA.X;
            AB[1] = pointB.Y - pointA.Y;
            AC[0] = pointC.X - pointA.X;
            AC[1] = pointC.Y - pointA.Y;

            double cross = AB[0] * AC[1] - AB[1] * AC[0];
            return cross;
        }

        /// <summary>
        /// Determines the distance between Point 1 and Point 2.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns></returns>
        public static double Delta(Point2D point1, Point2D point2)
        {
            double dX = point2.X - point1.X;
            double dY = point2.Y - point1.Y;
            double value = Math.Sqrt(dX * dX + dY * dY);

            if (double.IsNaN(value))
                value = 0;

            return value;
        }

        /// <summary>
        /// Determines the distance between this point and the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public double DistanceTo(Point2D point)
        {
            return Delta(this, point);
        }

        /// <summary>
        /// Determines the shortest distance from this point to the specified line segment.
        /// </summary>
        /// <param name="line">The line segment.</param>
        /// <returns></returns>
        public double DistanceTo(LineSeg2D line)
        {
            double dist = CrossProduct(line.Start, line.End, this) / Delta(line.Start, line.End);
            return Math.Abs(dist);
        }

        /// <summary>
        /// Determines the shortest distance from this point to the specified line segment.
        /// </summary>
        /// <param name="line">The line segment.</param>
        /// <param name="sign">-1 if the point resides above the line, or 1 if the point resides below the line.</param>
        /// <returns></returns>
        public double DistanceTo(LineSeg2D line, out int sign)
        {
            double dist = CrossProduct(line.Start, line.End, this) / Delta(line.Start, line.End);

            if (dist < 0)
                sign = -1;
            else
                sign = 1;

            return Math.Abs(dist);
        }

        /// <summary>
        /// Determines the point after a rotation around the specified center point, by the specified theta.
        /// </summary>
        /// <param name="theta">The theta angle.</param>
        /// <param name="center">The origin of the rotation.</param>
        /// <returns></returns>
        public Point2D RotateAround(double theta, Point2D center)
        {
            Point2D rotatedPoint = new Point2D();
            rotatedPoint.X = Math.Cos(theta) * (this.X - center.X) - Math.Sin(theta) * (this.Y - center.Y) + center.X;
            rotatedPoint.Y = Math.Sin(theta) * (this.X - center.X) + Math.Cos(theta) * (this.Y - center.Y) + center.Y;

            return rotatedPoint;
        }

        /// <summary>
        /// Finds the centroid of the specified points.
        /// </summary>
        /// <param name="vertices">The vertices of this 'polygon'.</param>
        /// <returns></returns>
        public static Point2D FindCentroid(params Point2D[] vertices)
        {
            Point2D centroid = new Point2D();

            double signedArea = 0.0;
            double x0 = 0.0; // Current vertex X
            double y0 = 0.0; // Current vertex Y
            double x1 = 0.0; // Next vertex X
            double y1 = 0.0; // Next vertex Y
            double a = 0.0;  // Partial signed area

            // For all vertices except last
            for (int i = 0; i < vertices.Length - 1; ++i)
            {
                x0 = vertices[i].X;
                y0 = vertices[i].Y;
                x1 = vertices[i+1].X;
                y1 = vertices[i+1].Y;
                a = x0*y1 - x1*y0;
                signedArea += a;
                centroid.X += (x0 + x1)*a;
                centroid.Y += (y0 + y1)*a;
            }

            // Do last vertex
            x0 = vertices[vertices.Length - 1].X;
            y0 = vertices[vertices.Length - 1].Y;
            x1 = vertices[0].X;
            y1 = vertices[0].Y;
            a = x0*y1 - x1*y0;
            signedArea += a;
            centroid.X += (x0 + x1)*a;
            centroid.Y += (y0 + y1)*a;

            signedArea *= 0.5;
            centroid.X /= (6*signedArea);
            centroid.Y /= (6*signedArea);

            return centroid;
        }

        /// <summary>
        /// Negates the specified point.
        /// </summary>
        /// <param name="a">A</param>
        /// <returns></returns>
        public static Point2D operator -(Point2D a)
        {
            if (a == null)
                a = Point2D.Empty;

            return new Point2D(-a.X, -a.Y);
        }

        /// <summary>
        /// Subtracts Point B from Point A.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static Point2D operator -(Point2D a, Point2D b)
        {
            if (a == null)
                a = Point2D.Empty;

            if (b == null)
                b = Point2D.Empty;

            return a.Offset(-b.X, -b.Y);
        }

        /// <summary>
        /// Adds Point A and Point B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static Point2D operator +(Point2D a, Point2D b)
        {
            if (a == null)
                a = Point2D.Empty;

            if (b == null)
                b = Point2D.Empty;

            return a.Offset(b.X, b.Y);
        }

        /// <summary>
        /// Multiplies Point B by scalar A.
        /// </summary>
        /// <param name="a">Scalar A</param>
        /// <param name="b">Point B</param>
        /// <returns></returns>
        public static Point2D operator *(double a, Point2D b)
        {
            if (b == null)
                b = Point2D.Empty;

            return new Point2D(a * b.X, a * b.Y);
        }

        /// <summary>
        /// Multiplies Point A by scalar B
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Scalar B</param>
        /// <returns></returns>
        public static Point2D operator *(Point2D a, double b)
        {
            return b * a;
        }

        /// <summary>
        /// Divides Point A by scalar B.
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Scalar B</param>
        /// <returns></returns>
        public static Point2D operator /(Point2D a, double b)
        {
            if (a == null)
                a = Point2D.Empty;

            return new Point2D(a.X / b, a.Y / b);
        }

        /// <summary>
        /// Determines if Point A equals Point B (using Point A precision).
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns></returns>
        public static bool operator ==(Point2D a, Point2D b)
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
        public static bool operator !=(Point2D a, Point2D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Converts to a <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point((int)X, (int)Y);
        }

        /// <summary>
        /// Converts to a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.PointF ToPointF()
        {
            return new System.Drawing.PointF((float)X, (float)Y);
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
        /// Implicit conversion from <see cref="System.Drawing.PointF"/> to <see cref="Point2D"/>
        /// </summary>
        /// <param name="pointF">The floating-point.</param>
        /// <returns></returns>
        public static implicit operator Point2D(System.Drawing.PointF pointF)
        {
            return new Point2D(pointF.X, pointF.Y);
        }

        /// <summary>
        /// Implicit conversion from <see cref="System.Drawing.Point"/> to <see cref="Point2D"/>
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static implicit operator Point2D(System.Drawing.Point point)
        {
            return new Point2D(point.X, point.Y);
        }

        /// <summary>
        /// Implicit conversion from <see cref="Size2D"/> to <see cref="Point2D"/>
        /// </summary>
        /// <param name="size">The size 2D.</param>
        /// <returns></returns>
        /// <remarks>
        /// Width to X, Height to Y
        /// </remarks>
        public static implicit operator Point2D(Size2D size)
        {
            return new Point2D(size.Width, size.Height);
        }

        #endregion
    }
}
