using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// Provides extension methods for standard math equations.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Calculates the standard deviation of a set of double values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static double StdDev(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        /// <summary>
        /// Returns a value indicating whether this instance and the specified System.Double object
        /// are equivalent when rounded by the specified significantDigits
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <param name="precision">The number of significant digits considered.</param>
        /// <returns>
        /// True if equal; otherwise, false.
        /// </returns>
        public static bool EqualsRoughly(this double a, double b, int precision)
        {
            if (precision < 0)
                precision = 0;

            return Math.Abs(a - b) <= Math.Pow(10, -precision);
        }

        /// <summary>
        /// Square Root of (a^2 + b^2) without overflow.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Hypotenuse(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + r * r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }
    }
}
