using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// The Loess Interpolator implements the local regression algorithm (locally weighted scatterplot smoothing).
    /// </summary>
    public class LoessInterpolator
    {
        /// <summary>
        /// The default bandwidth value (0.3).
        /// </summary>
        public static double DEFAULT_BANDWIDTH = 0.3;

        /// <summary>
        /// The default robustness iterations value (2).
        /// </summary>
        public static int DEFAULT_ROBUSTNESS_ITERS = 2;

        /// <summary>
        /// When computing the loess fit at a particular point, this fraction of source points closest
        /// to the current point is taken into account for computing a least-squares regression. 
        /// A sensible value is usually 0.25 to 0.5.
        /// </summary>
        private double bandwidth;

        /// <summary>
        /// The number of robustness iterations to perform. Typically 0 - 4.
        /// </summary>
        private int robustnessIters;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoessInterpolator"/> class.
        /// </summary>
        public LoessInterpolator()
        {
            this.bandwidth = DEFAULT_BANDWIDTH;
            this.robustnessIters = DEFAULT_ROBUSTNESS_ITERS;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoessInterpolator"/> class.
        /// </summary>
        /// <param name="bandwidth">
        /// When computing the loess fit at a particular point, this fraction of source points closest
        /// to the current point is taken into account for computing a least-squares regression. 
        /// A sensible value is usually 0.25 to 0.5. This must be between 0 - 1.
        /// </param>
        /// <param name="robustnessIters">
        /// The number of robustness iterations to perform. Typically 0 - 4.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// bandwidth is out of range 
        /// or
        /// robustness iterations must be non-negative
        /// </exception>
        public LoessInterpolator(double bandwidth, int robustnessIters)
        {
            if (bandwidth < 0 || bandwidth > 1)
                throw new ArgumentOutOfRangeException(string.Format("bandwidth must be in the interval [0,1], but got {0}", bandwidth));

            this.bandwidth = bandwidth;
            if (robustnessIters < 0)
                throw new ArgumentOutOfRangeException(string.Format("the number of robustness iterations must be non-negative, but got {0}", robustnessIters));

            this.robustnessIters = robustnessIters;
        }

        /// <summary>
        /// Computes a loess fit on the specified points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">
        /// Loess expects at least 1 point
        /// </exception>
        public double[] Smooth(params Point2D[] points)
        {
            double[] xval = points.Select(p => p.X).ToArray();
            double[] yval = points.Select(p => p.Y).ToArray();

            int n = xval.Length;
            if (n == 0)
                throw new ApplicationException("Loess expects at least 1 point");

            CheckAllFiniteReal(xval, true);
            CheckAllFiniteReal(yval, false);
            CheckStrictlyIncreasing(xval);

            if (n == 1)
            {
                return new double[] { yval[0] };
            }

            if (n == 2)
            {
                return new double[] { yval[0], yval[1] };
            }

            int bandwidthInPoints = (int)(bandwidth * n);

            if (bandwidthInPoints < 2)
            {
                throw new ApplicationException(string.Format("the bandwidth must be large enough to accomodate at least 2 points. There are {0} " +
                    " data points, and bandwidth must be at least {1} but it is only {2}",
                    n, 2.0 / n, bandwidth
                ));
            }

            double[] res = new double[n];

            double[] residuals = new double[n];
            double[] sortedResiduals = new double[n];

            double[] robustnessWeights = new double[n];

            // Do an initial fit and 'robustnessIters' robustness iterations.
            // This is equivalent to doing 'robustnessIters+1' robustness iterations
            // starting with all robustness weights set to 1.
            for (int i = 0; i < robustnessWeights.Length; i++) robustnessWeights[i] = 1;
            for (int iter = 0; iter <= robustnessIters; ++iter)
            {
                int[] bandwidthInterval = { 0, bandwidthInPoints - 1 };
                // At each x, compute a local weighted linear regression
                for (int i = 0; i < n; ++i)
                {
                    double x = xval[i];

                    // Find out the interval of source points on which
                    // a regression is to be made.
                    if (i > 0)
                    {
                        UpdateBandwidthInterval(xval, i, bandwidthInterval);
                    }

                    int ileft = bandwidthInterval[0];
                    int iright = bandwidthInterval[1];

                    // Compute the point of the bandwidth interval that is
                    // farthest from x
                    int edge;
                    if (xval[i] - xval[ileft] > xval[iright] - xval[i])
                    {
                        edge = ileft;
                    }
                    else
                    {
                        edge = iright;
                    }

                    // Compute a least-squares linear fit weighted by
                    // the product of robustness weights and the tricube
                    // weight function.
                    // See http://en.wikipedia.org/wiki/Linear_regression
                    // (section "Univariate linear case")
                    // and http://en.wikipedia.org/wiki/Weighted_least_squares
                    // (section "Weighted least squares")
                    double sumWeights = 0;
                    double sumX = 0, sumXSquared = 0, sumY = 0, sumXY = 0;
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
            }

            return res;
        }

        private static void UpdateBandwidthInterval(double[] xval, int i, int[] bandwidthInterval)
        {
            int left = bandwidthInterval[0];
            int right = bandwidthInterval[1];
            // The right edge should be adjusted if the next point to the right
            // is closer to xval[i] than the leftmost point of the current interval
            if (right < xval.Length - 1 &&
               xval[right + 1] - xval[i] < xval[i] - xval[left])
            {
                bandwidthInterval[0]++;
                bandwidthInterval[1]++;
            }
        }

        private static double Tricube(double x)
        {
            double tmp = 1 - x * x * x;
            return tmp * tmp * tmp;
        }

        private static void CheckAllFiniteReal(double[] values, bool isAbscissae)
        {
            for (int i = 0; i < values.Length; i++)
            {
                double x = values[i];
                if (Double.IsInfinity(x) || Double.IsNaN(x))
                {
                    string pattern = isAbscissae ?
                            "all abscissae must be finite real numbers, but {0}-th is {1}" :
                            "all ordinatae must be finite real numbers, but {0}-th is {1}";
                    throw new ApplicationException(string.Format(pattern, i, x));
                }
            }
        }

        private static void CheckStrictlyIncreasing(double[] xval)
        {
            for (int i = 1; i < xval.Length; ++i)
            {
                if (xval[i - 1] >= xval[i])
                {
                    throw new ApplicationException(string.Format(
                            "the abscissae array must be sorted in a strictly " +
                            "increasing order, but the {0}-th element is {1} " +
                            "whereas {2}-th is {3}",
                            i - 1, xval[i - 1], i, xval[i]));
                }
            }
        }
    }
}
