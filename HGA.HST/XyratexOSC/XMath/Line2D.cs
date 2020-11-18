using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A 2D line represented in Slope-YIntercept form (unless vertical in which case the line is represented in X-intercept form).
    /// </summary>
    public class Line2D
    {
        /// <summary>
        /// Gets the slope of the line.
        /// </summary>
        /// <value>
        /// The slope.
        /// </value>
        public virtual double Slope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Y-intercept of the line, unless the line is vertical in which case it returns the X-intercept.
        /// </summary>
        /// <value>
        /// The intercept.
        /// </value>
        public virtual double Intercept
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this line is vertical or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this line is vertical; otherwise, <c>false</c>.
        /// </value>
        public bool IsVertical
        {
            get
            {
                return double.IsInfinity(Slope);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this line is horizontal or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this line is horizontal; otherwise, <c>false</c>.
        /// </value>
        public bool IsHorizontal
        {
            get
            {
                return (Slope == 0);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line2D"/> class.
        /// </summary>
        /// <param name="slope">The slope of the line.</param>
        /// <param name="intercept">The Y-intercept of the line; unless the slope is an infinity, then X = Intercept.</param>
        public Line2D(double slope, double intercept)
        {
            Slope = slope;
            Intercept = intercept;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line2D"/> class, using point-slope form.
        /// </summary>
        /// <param name="point">A point on the line.</param>
        /// <param name="slope">The slope of the line.</param>
        public Line2D(Point2D point, double slope)
        {
            Slope = slope;

            if (double.IsInfinity(slope))
                Intercept = point.X;
            else
                Intercept = -(slope * point.X) + point.Y; //y - y1 = m(x - x1)
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line2D"/> class through two given points.
        /// </summary>
        /// <param name="point1">Point 1 on the line.</param>
        /// <param name="point2">Point 2 on the line.</param>
        public Line2D(Point2D point1, Point2D point2)
        {
            if (point1 == null)
                throw new ArgumentNullException("point1", "Line cannot be created because first point is empty");

            if (point2 == null)
                throw new ArgumentNullException("point2", "Line cannot be created because last point is empty");

            double denom = (point2.X - point1.X);

            if (denom == 0) //x = b
            {
                Slope = double.PositiveInfinity;
                Intercept = point1.X;
            }
            else //y = mx + b
            {
                double slope = (point2.Y - point1.Y) / denom;

                Slope = slope;
                Intercept = point1.Y - (slope * point1.X);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (IsVertical)
                return String.Format("Slope,∞,X-Int,{0:F4}", Intercept);
            else
                return String.Format("Slope,{0:F4},Y-Int,{1:F4}", Slope, Intercept);
        }

        /// <summary>
        /// Solves for Y given X.
        /// </summary>
        /// <param name="x">X value</param>
        /// <returns>Y value</returns>
        public double GetY(double x)
        {
            //TODO: error handling for vertical lines
            return this.Slope * x + Intercept;
        }
        
        /// <summary>
        /// Solves for X given Y.
        /// </summary>
        /// <param name="y">Y value</param>
        /// <returns>X Value</returns>
        public double GetX(double y)
        {
            if (IsVertical)
                return Intercept;

            return (y - Intercept) / this.Slope;
        }

        /// <summary>
        /// Determines the line that best-fits to the specified points (using the Area method).
        /// </summary>
        /// <param name="points">The points to fit.</param>
        /// <param name="fitPoints">The calculated best-fit points.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">
        /// Best-Fit lines require two points.
        /// or
        /// Best-Fit lines require two unique points.
        /// </exception>
        public static Line2D FromBestFit(IEnumerable<Point2D> points, out IList<Point2D> fitPoints)
        {
            fitPoints = PerformRobustFit(points.ToList(), 0);

            Point2D start = fitPoints.First();

            foreach (Point2D point in fitPoints)
                if (start.X != point.X)
                    return new Line2D(start, point);

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
        public static Line2D FromRobustFit(IEnumerable<Point2D> points, out IList<Point2D> fitPoints, int maxIterations = 5)
        {
            fitPoints = PerformRobustFit(points.ToList(), maxIterations);

            Point2D start = fitPoints.First();

            foreach (Point2D point in fitPoints)
                if (start.X != point.X)
                    return new Line2D(start, point);

            throw new ArithmeticException("Best-Fit lines require two unique points.");
        }

        /*
        /// <summary>
        /// Perform a bilinear weighted least squares regression fit.
        /// http://www.mathworks.com/help/curvefit/least-squares-fitting.html
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="maxIterations">The maximum iterations.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">Best-Fit lines require two points.</exception>
        protected static IList<Point2D> PerformRobustFit(List<Point2D> points, int maxIterations)
        {
            int n = points.Count;
            int m = 2;              // linear equation

            if (n < 2)
                throw new ArithmeticException("Best-Fit lines require two points.");

            Matrix A = new Matrix(n, m);
            Matrix y = new Matrix(n, 1);
            Matrix w = new Matrix(n, n);

            for (int i = 0; i < n; i++)
            {
                y[i, 0] = points[i].Y;
                A[i, 0] = points[i].X;
                A[i, 1] = 1;
                w[i, i] = 1;
            }

            for (int j = 0; j <= maxIterations; j++)
            {
                Matrix e = new Matrix(n, 1);
                
                //b = (A.Transpose() * w * A).Invert() * (A.Transpose() * w * y);

                QRDecomposition qr = A.QR();

                Matrix b = (w * qr.R.Invert()) * (qr.Q.Transpose() * w * y);

                Matrix yfit = A * b;
                Matrix residuals = y - yfit;
                Matrix absResiduals = Matrix.Abs(residuals);

                List<double> sortedAbsRes = absResiduals.GetColumn(0).OrderBy(r => r).ToList();
                double mid = (n - 1) / 2.0;
                double median = sortedAbsRes[(int)mid] + sortedAbsRes[(int)(mid + 0.5)] / 2;
                double absResScale = 6.9460 * median; //6.9460 is a tuning constant (4.685/0.6745)
                
                Matrix H = A * (A.Transpose() * A).Invert() * A.Transpose();

                for (int i = 0; i < n; i++)
                {
                    double u = absResiduals[i, 0] / absResScale;
                    w[i,i] = (u < 1) ? Math.Pow((1 - (u * u)), 2) : 0;
                }

                Matrix weights = absResiduals
                Matrix residualsAdj = absResiduals
            }

        }
        */

        /// <summary>
        /// Performs a robust best-fit to the specified points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="maxIterations">The maximum fit iterations.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">Best-Fit lines require two points.</exception>
        protected static IList<Point2D> PerformRobustFit(List<Point2D> points, int maxIterations)
        {
            int n = points.Count;

            if (n < 2)
                throw new ArithmeticException("Best-Fit lines require two points.");

            Point2D[] fitPoints = new Point2D[n];
            double[] absResiduals = new double[n];
            double[] weights = new double[n];

            for (int i = 0; i < n; i++) 
                weights[i] = 1;

            double lastA = double.MinValue;
            double lastB = double.MinValue;

            for (int j = 0; j <= maxIterations; j++) // includes an initial fit
            {
                double sumWeights = 0;
                double sumX = 0;
                double sumY = 0;
                double sumXX = 0;
                double sumXY = 0;

                for (int i = 0; i < n; i++)
                {
                    sumWeights += weights[i];

                    sumX += weights[i] * points[i].X;
                    sumY += weights[i] * points[i].Y;

                    sumXX += weights[i] * points[i].X * points[i].X;
                    sumXY += weights[i] * points[i].X * points[i].Y;
                }

                double meanX = sumX / sumWeights;
                double meanY = sumY / sumWeights;
                double meanXX = sumXX / sumWeights;
                double meanXY = sumXY / sumWeights;
                
                double a = (meanXY - (meanX * meanY)) / (meanXX - (meanX * meanX));
                double b = (meanY - a * meanX);

                for (int i = 0; i < n; i++)
                {
                    Point2D fitPoint = new Point2D(points[i].X, a * points[i].X + b);
                    fitPoints[i] = fitPoint;
                    absResiduals[i] = Math.Abs(points[i].Y - fitPoint.Y);
                }

                if (j == maxIterations)
                    break;

                List<double> sortedAbsRes = absResiduals.OrderBy(x => x).ToList();
                double mid = (n - 1) / 2.0;
                double median = sortedAbsRes[(int)mid] + sortedAbsRes[(int)(mid + 0.5)] / 2;
                double absResScale = 4 * median; //6 is the default tuning constant
                double stdDevResiduals = absResiduals.StdDev();

                for (int i = 0; i < n; i++)
                {
                    double u = absResiduals[i] / absResScale;

                    if (absResiduals[i] < stdDevResiduals * 1.5)
                    {
                        weights[i] = 1;
                    }
                    else if (u < 1)
                    {
                        weights[i] = Math.Pow((1 - (u * u)), 2);
                    }
                    else
                    {
                        weights[i] = 0;
                    }
                }

                double errY = Math.Sqrt(1 / weights.Sum());

                if (b.EqualsRoughly(lastB, 7) && a.EqualsRoughly(lastA, 6))
                {
                    break;
                }

                lastA = a;
                lastB = b;
            }

            return fitPoints;
        }
        
        /*
        protected static Point2D[] WeightedFit(ref Point2D[] points, ref double[] weights)
        {
            int n = points.Length;

            Point2D[] fitPoints = new Point2D[n];
            double[] residuals = new double[n];

            double sumX = 0;
            double sumY = 0;
            double sumXX = 0;
            double sumXY = 0;
            double meanX = 0;
            double meanY = 0;

            for (int i = 0; i < n; i++)
            {
                residuals[i] = Math.Abs(points.Y - fitPoint.Y);
            }

            List<double> sortedAbsRes = absRes.OrderBy(x => x).ToList();
            double mid = (n - 1) / 2.0;
            double median = sortedAbsRes[(int)mid] + sortedAbsRes[(int)(mid + 0.5)] / 2;
            double absResScale = 6.9460 * median; //6.9460 is a tuning constant (4.685/0.6745)

            for (int i = 0; i < n; ++i)
            {
                Point2D point = points[i];

                double w = weights[i];
                sumX += point.Y
                double denom = Math.Abs(1.0 / (xval[edge] - x));
                for (int k = ileft; k <= iright; ++k)
                {
                    double xk = xval[k];
                    double yk = yval[k];
                    double dist;
                    if (k < i)
                    {
                        dist = (x - xk);
                    }
                    else
                    {
                        dist = (xk - x);
                    }
                    double w = Tricube(dist * denom) * robustnessWeights[k];
                    double xkw = xk * w;
                    sumWeights += w;
                    sumX += xkw;
                    sumXSquared += xk * xkw;
                    sumY += yk * w;
                    sumXY += yk * xkw;
                }

                double sumXX = fitPoints.Sum(point => point.X * point.X);
                double sumXY = fitPoints.Sum(point => point.X * point.Y);

                double meanX = fitPoints.Average(point => point.X);
                double meanY = fitPoints.Average(point => point.Y);

                double a = (sumXY / n - meanX * meanY) / (sumXX / n - meanX * meanX);
                double b = (a * meanX - meanY);

                double meanX = sumX / sumWeights;
                double meanY = sumY / sumWeights;
                double meanXY = sumXY / sumWeights;
                double meanXSquared = sumXSquared / sumWeights;

                double beta;
                if (meanXSquared == meanX * meanX)
                {
                    beta = 0;
                }
                else
                {
                    beta = (meanXY - meanX * meanY) / (meanXSquared - meanX * meanX);
                }

                double alpha = meanY - beta * meanX;

                res[i] = beta * x + alpha;
                residuals[i] = Math.Abs(yval[i] - res[i]);
            }

            // No need to recompute the robustness weights at the last
            // iteration, they won't be needed anymore
            if (iter == robustnessIters)
            {
                break;
            }

            // Recompute the robustness weights.

            // Find the median residual.
            // An arraycopy and a sort are completely tractable here, 
            // because the preceding loop is a lot more expensive
            System.Array.Copy(residuals, sortedResiduals, n);
            //System.arraycopy(residuals, 0, sortedResiduals, 0, n);
            Array.Sort<double>(sortedResiduals);
            double medianResidual = sortedResiduals[n / 2];

            if (medianResidual == 0)
            {
                break;
            }

            for (int i = 0; i < n; ++i)
            {
                double arg = residuals[i] / (6 * medianResidual);
                robustnessWeights[i] = (arg >= 1) ? 0 : Math.Pow(1 - arg * arg, 2);
            }


            return res;
        }
        */

        /// <summary>
        /// Determines the best-fit line of the specified points using Deming regression.
        /// </summary>
        /// <param name="points">The points to fit.</param>
        /// <param name="fitPoints">The calculated best-fit points.</param>
        /// <returns></returns>
        /// <exception cref="System.ArithmeticException">Best-Fit lines require two points.</exception>
        public static Line2D FromDemingBestFit(IList<Point2D> points, out IList<Point2D> fitPoints)
        {
            int n = points.Count;

            if (n < 2)
                throw new ArithmeticException("Best-Fit lines require two points.");

            if (n == 2)
            {
                fitPoints = points.ToList();
                return new Line2D(fitPoints[0], fitPoints[1]);
            }

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
            double covXY = (1 / (double)(n - 1)) * (xx.Transpose() * yy)[0, 0];
            double varX = (1 / (double)(n - 1)) * (xx.Transpose() * xx)[0, 0];
            double varY = (1 / (double)(n - 1)) * (yy.Transpose() * yy)[0, 0];

            // Assign slope an intercept (in closed-form)

            double b2 = (varY - lambda * varX +
                        Math.Sqrt(Math.Pow(varY - lambda * varX, 2) + Math.Pow(4 * lambda * covXY, 2)))
                        / (2 * covXY);

            double b1 = m_y - b2 * m_x;

            // Assign x/y estimated values
            Matrix x_est = x + (b2 / (b2 * b2 + lambda)) * (y - b1 - (b2 * x));
            Matrix y_est = b1 + (b2 * x_est);

            // Determine sigma2
            //double sigma2_x  = sum(lambda*(x-x_est).^2 + (y - b[0,0] - b[1,0]*x_est).^2)./(2*lambda*(n-2));

            fitPoints = new List<Point2D>();

            for (int i = 0; i < n; i++)
                fitPoints.Add(new Point2D(x_est[i, 0], y_est[i, 0]));

            if (fitPoints.First() != fitPoints.Last())
                return new Line2D(fitPoints.First(), fitPoints.Last());
            else
                return new Line2D(fitPoints.First(), double.PositiveInfinity);
        }

        /// <summary>
        /// Determines the point of intersection between the specified lines.
        /// </summary>
        /// <param name="line1">Line 1.</param>
        /// <param name="line2">Line 2.</param>
        /// <returns>The point of intersection if the lines intersect; otherwise, null.</returns>
        public static Point2D IntersectAt(Line2D line1, Line2D line2)
        {
            if (line1.IsVertical || line2.IsVertical)
            {
                if (line1.IsVertical && line2.IsVertical)
                    return null;

                if (line1.IsVertical)
                    return new Point2D(line1.Intercept, line2.GetY(line1.Intercept));
                else
                    return new Point2D(line2.Intercept, line1.GetY(line2.Intercept));
            }

            double denom = (line1.Slope - line2.Slope);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.
            if (denom == 0)
                return null;

            double x = (line2.Intercept - line1.Intercept) / denom;
            double y = (line1.Slope * x) + line1.Intercept;

            return new Point2D(x, y);
        }

        /// <summary>
        /// Determines the angle between line 1 and line 2.
        /// </summary>
        /// <param name="line1">Line 1.</param>
        /// <param name="line2">Line 2.</param>
        /// <returns></returns>
        public static double AngleBetween(Line2D line1, Line2D line2)
        {
            return Math.Atan((line1.Slope - line2.Slope) / (1 + (line1.Slope * line2.Slope)));
        }
    }
}
