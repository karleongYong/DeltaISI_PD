using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 2D line segment represented by two points (Start and End). Inherits from <see cref="Line2D"/>
    /// </summary>
    public class LineSeg2D : Line2D
    {
        /// <summary>
        /// Gets the slope of the line.
        /// </summary>
        /// <value>
        /// The slope.
        /// </value>
        public override double Slope
        {
            get
            {
                double denom = (End.X - Start.X);

                if (denom == 0) //x = b
                    return double.PositiveInfinity;
                else //y = mx + b
                    return (End.Y - Start.Y) / denom;
            }
        }

        /// <summary>
        /// Gets the Y-intercept of the line, unless the line is vertical in which case it returns the X-intercept.
        /// </summary>
        /// <value>
        /// The intercept.
        /// </value>
        public override double Intercept
        {
	        get 
	        { 
                double denom = (End.X - Start.X);

                if (denom == 0) //x = b
                    return Start.X;
                else
                    return Start.Y - (Slope * Start.X);
	        }
        }

        /// <summary>
        /// Gets or sets the start point of the line segment.
        /// </summary>
        /// <value>
        /// The starting point.
        /// </value>
        public Point2D Start
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end point of the line segment.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public Point2D End
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the X-coordinate of the <see cref="Start"/> point.
        /// </summary>
        /// <value>
        /// The <see cref="Start"/> point X-coordinate.
        /// </value>
        public double X1
        {
            get
            {
                return Start.X;
            }
            set
            {
                Start.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y-coordinate of the <see cref="Start"/> point.
        /// </summary>
        /// <value>
        /// The <see cref="Start"/> point Y-coordinate.
        /// </value>
        public double Y1
        {
            get
            {
                return Start.Y;
            }
            set
            {
                Start.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the X-coordinate of the <see cref="End"/> point.
        /// </summary>
        /// <value>
        /// The <see cref="End"/> point X-coordinate.
        /// </value>
        public double X2
        {
            get
            {
                return End.X;
            }
            set
            {
                End.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y-coordinate of the <see cref="End"/> point.
        /// </summary>
        /// <value>
        /// The <see cref="End"/> point Y-coordinate.
        /// </value>
        public double Y2
        {
            get
            {
                return End.Y;
            }
            set
            {
                End.Y = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeg2D"/> class.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        public LineSeg2D(Point2D start, Point2D end)
            : base(start, end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeg2D"/> class.
        /// </summary>
        /// <param name="X1">The start X-coordinate.</param>
        /// <param name="Y1">The start Y-coordinate.</param>
        /// <param name="X2">The end X-coordinate.</param>
        /// <param name="Y2">The end Y-coordinate.</param>
        public LineSeg2D(double X1, double Y1, double X2, double Y2)
            : this (new Point2D(X1, Y1), new Point2D(X2, Y2))
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0},X1,{1:F3},Y1,{2:F3},X2,{3:F3},Y2,{4:F3}", base.ToString(), X1, Y1, X2, Y2);
        }

        /// <summary>
        /// Gets the length of this line segment.
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return MathExtensions.Hypotenuse(System.Math.Abs(X2 - X1), System.Math.Abs(Y2 - Y1));
        }

        /// <summary>
        /// Gets the center point of this line segment.
        /// </summary>
        /// <returns></returns>
        public Point2D GetCenterPoint()
        {
            return new Point2D((X1 + X2) / 2, (Y1 + Y2) / 2);
        }

        /// <summary>
        /// Determines the shortest distance from anywhere on the line segment to the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public double DistanceTo(Point2D point)
        {
            double dist = Point2D.CrossProduct(this.Start, this.End, point) / Point2D.Delta(this.Start, this.End);
            return System.Math.Abs(dist);
        }

        /// <summary>
        /// Determines the line that best-fits to the specified pointws (using the Area method).
        /// </summary>
        /// <param name="points">The points to fit.</param>
        /// <param name="fitPoints">The calculated best-fit points.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">
        /// Best-Fit lines require two points.
        /// or
        /// Best-Fit lines require two unique points.
        /// </exception>
        public static new LineSeg2D FromBestFit(IEnumerable<Point2D> points, out IList<Point2D> fitPoints)
        {
            fitPoints = PerformRobustFit(points.ToList(), 0);

            Point2D start = fitPoints.First();
            Point2D last = fitPoints.Last();

            foreach (Point2D point in fitPoints)
            {
                if (point.X < start.X)
                    start = point;

                if (point.X > last.X)
                    last = point;
            }

            if (start != last)
                return new LineSeg2D(start, last);

            throw new ArithmeticException("Best-Fit lines require two unique points.");
        }

        /// <summary>
        /// Determines the line that best-fits to the specified points (using bisquare regression).
        /// </summary>
        /// <param name="points">The points to fit.</param>
        /// <param name="fitPoints">The calculated best-fit points.</param>
        /// <param name="maxIterations">The maximum fit iterations.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">Best-Fit lines require two points.
        /// or
        /// Best-Fit lines require two unique points.</exception>
        public static new LineSeg2D FromRobustFit(IEnumerable<Point2D> points, out IList<Point2D> fitPoints, int maxIterations = 5)
        {
            fitPoints = PerformRobustFit(points.ToList(), maxIterations);

            Point2D start = fitPoints.First();
            Point2D last = fitPoints.Last();

            foreach (Point2D point in fitPoints)
            {
                if (point.X < start.X)
                    start = point;

                if (point.X > last.X)
                    last = point;
            }

            if (start != last)
                return new LineSeg2D(start, last);

            throw new ArithmeticException("Best-Fit lines require two unique points.");
        }

        /// <summary>
        /// Determines the best-fit line of the specified points using Deming regression.
        /// </summary>
        /// <param name="points">The points to fit.</param>
        /// <param name="fitPoints">The calculated best-fit points.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">Best-Fit lines require two points.</exception>
        public static new LineSeg2D FromDemingBestFit(IList<Point2D> points, out IList<Point2D> fitPoints)
        {
            int n = points.Count;

            // Inputs
            Matrix x = new Matrix(n, 1);
            Matrix y = new Matrix(n, 1);

            double lambda = 1.0;
            double m_x = 0; //mean x
            double m_y = 0; //mean y

            for (int i = 0; i < n; i++)
            {
                x[i, 0] = points[i].X;
                y[i, 0] = points[i].Y;
                m_x += points[i].X;
                m_y += points[i].Y;
            }

            m_x /= n;
            m_y /= n;

            // Calculate the covariance matrix of X and Y
            Matrix xx = x - m_x;
            Matrix yy = y - m_y;
            double covXY = (1 / (double)(n - 1)) * (xx.Transpose() * yy)[0,0];
            double varX = (1 / (double)(n - 1)) * (xx.Transpose() * xx)[0,0];
            double varY = (1 / (double)(n - 1)) * (yy.Transpose() * yy)[0,0];

            // Assign slope an intercept (in closed-form)

            double b2 = (varY - lambda*varX +
                        System.Math.Sqrt(System.Math.Pow(varY - lambda * varX, 2) + System.Math.Pow(4 * lambda * covXY, 2))) 
                        / (2*covXY);

            double b1  = m_y - b2*m_x;

            // Assign x/y estimated values
            Matrix x_est = x + (b2 / (b2 * b2 + lambda)) * (y - b1 - (b2 * x));
            Matrix y_est = b1 + (b2 * x_est);
            
            // Determine sigma2
            //double sigma2_x  = sum(lambda*(x-x_est).^2 + (y - b[0,0] - b[1,0]*x_est).^2)./(2*lambda*(n-2));

            fitPoints = new List<Point2D>();

            for (int i = 0; i < n; i++)
                fitPoints.Add(new Point2D(x_est[i, 0], y_est[i, 0]));

            if (fitPoints.First() != fitPoints.Last())
                return new LineSeg2D(fitPoints.First(), fitPoints.Last());

            throw new ArithmeticException("Best-Fit lines require two unique points.");
        }

        /// <summary>
        /// Determines the point of intersection between the specified lines.
        /// </summary>
        /// <param name="line1">Line 1.</param>
        /// <param name="line2">Line 2.</param>
        /// <returns>The point of intersection if the lines intersect; otherwise, null.</returns>
        public static Point2D IntersectAt(LineSeg2D line1, LineSeg2D line2)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (line2.Y2 - line2.Y1) * (line1.X2 - line1.X1)
               -
               (line2.X2 - line2.X1) * (line1.Y2 - line1.Y1);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (line2.X2 - line2.X1) * (line1.Y1 - line2.Y1)
               -
               (line2.Y2 - line2.Y1) * (line1.X1 - line2.X1);

            double n_b =
                (line1.X2 - line1.X1) * (line1.Y1 - line2.Y1) 
                - 
                (line1.Y2 - line1.Y1) * (line1.X1 - line2.X1);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.
            if (d == 0)
                return null;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;

            Point2D intersection = null;

            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                intersection = new Point2D();
                intersection.X = line1.X1 + (ua * (line1.X2 - line1.X1));
                intersection.Y = line1.Y1 + (ua * (line1.Y2 - line1.Y1));
            }

            return intersection;
        }

        /// <summary>
        /// Determines the angle between line 1 and line 2.
        /// </summary>
        /// <param name="line1">Line 1.</param>
        /// <param name="line2">Line 2.</param>
        /// <returns></returns>
        public static double AngleBetween(LineSeg2D line1, LineSeg2D line2)
        {
            return System.Math.Atan2(line1.Y2 - line1.Y1, line1.X2 - line1.X1) - System.Math.Atan2(line2.Y2 - line2.Y1, line2.X2 - line2.X1);
        }
    }
}
