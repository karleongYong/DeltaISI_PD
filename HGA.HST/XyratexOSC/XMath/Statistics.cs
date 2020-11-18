using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// Provides statistical helper methods
    /// </summary>
    public class Statistics
    {
        #region Nested Class : Peak

        /// <summary>
        /// Represents a peak in a histogram
        /// </summary>
        public class Peak
        {
            /// <summary>
            /// Gets or sets the X value.
            /// </summary>
            /// <value>
            /// The X value.
            /// </value>
            public int X { get; set; }
            /// <summary>
            /// Gets or sets the Y value of the peak.
            /// </summary>
            /// <value>
            /// The Y value.
            /// </value>
            public int Y { get; set; }
            /// <summary>
            /// Gets or sets the width of the peak.
            /// </summary>
            /// <value>
            /// The width.
            /// </value>
            public int Width { get; set; }
            /// <summary>
            /// Gets or sets the height from the Y value to the average of the neighboring valley Y values.
            /// </summary>
            /// <value>
            /// The height.
            /// </value>
            public int Height { get; set; }
            /// <summary>
            /// Gets or sets the steepness score.
            /// </summary>
            /// <value>
            /// The steepness score.
            /// </value>
            public double Steepness { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Peak"/> class.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="steepness">The steepness.</param>
            public Peak(int x, int y, int width, int height, double steepness)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Steepness = steepness;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return String.Format("({0},{1}) [{2},{3}] {4:F4}", X, Y, Width, Height, Steepness);
            }
        }

        #endregion

        /// <summary>
        /// Smoothes the specified histogram using the specified mask.
        /// </summary>
        /// <param name="originalValues">The original histogram.</param>
        /// <param name="mask">The mask.</param>
        public static int[] SmoothHist(int[] originalValues, double[] mask)
        {
            int[] smoothedValues = new int[originalValues.Length];

            int maskHalf = mask.Length / 2;

            for (int bin = 0; bin < maskHalf; bin++)
                smoothedValues[bin] = originalValues[bin];

            for (int bin = maskHalf; bin < originalValues.Length - maskHalf; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                    smoothedValue += originalValues[bin - maskHalf + i] * mask[i];

                smoothedValues[bin] = (int)smoothedValue;
            }

            for (int bin = originalValues.Length - maskHalf; bin < originalValues.Length; bin++)
                smoothedValues[bin] = originalValues[bin];

            return smoothedValues;
        }

        /// <summary>
        /// Finds the peaks of the specified histogram.
        /// </summary>
        /// <param name="histogram">The histogram.</param>
        /// <param name="peaks">The peaks.</param>
        /// <param name="valleyPercent">The valley percent.</param>
        public static void FindPeaks(int[] histogram, ref List<Peak> peaks, double valleyPercent = 0.3)
        {
            FindPeaks(histogram, ref peaks, 0, valleyPercent);
        }

        /// <summary>
        /// Finds the peaks of the specified histogram.
        /// </summary>
        /// <param name="histogram">The histogram.</param>
        /// <param name="peaks">The peaks.</param>
        /// <param name="iteration">The current iteration.</param>
        /// <param name="valleyPercent">The valley percent.</param>
        private static void FindPeaks(int[] histogram, ref List<Peak> peaks, int iteration, double valleyPercent = 0.3)
        {
            if (iteration > 50)
                return;

            int maxThresh = histogram.Max();
            int minAvg = 0;

            if (maxThresh <= 0)
                return;

            var nonZeroHist = histogram.Where(i => i > 0).ToList();
            if (nonZeroHist.Count > 0)
            {
                nonZeroHist.Sort();

                int minAvgCount = 1;
                minAvg = nonZeroHist[0];

                for (int i = 1; i < nonZeroHist.Count; i++)
                {
                    if (nonZeroHist[i] > (nonZeroHist[i - 1] * 1.2))
                        break;

                    minAvg += nonZeroHist[i];
                    minAvgCount++;
                }

                minAvg = minAvg / minAvgCount;

                int average = (int)nonZeroHist.Average();
                if (minAvg > average)
                    minAvg = average;
            }

            int spread = maxThresh - minAvg;
            if (spread < (maxThresh * 0.3))
                return;

            int valleyThresh = (int)(minAvg + (valleyPercent * spread));

            bool maxReached = false;
            int[] maskedHist = new int[histogram.Length];

            Point startPoint = Point.Empty;
            Point peakPoint = Point.Empty;
            Point endPoint = Point.Empty;
            Point lastPeak = Point.Empty;
            Point lastValley = Point.Empty;
            Point minSincePeak = Point.Empty;
            int minValleyToPeak = 40;

            for (int i = 0; i < histogram.Length; i++)
                maskedHist[i] = histogram[i];

            for (int i = 0; i < histogram.Length; i++)
            {
                if (!maxReached && histogram[i] == maxThresh)
                {
                    startPoint = new Point(i, histogram[i]);
                    maxReached = true;
                }
                else if (maxReached)
                {
                    if (histogram[i] < maxThresh)
                    {
                        endPoint = new Point(i - 1, histogram[i - 1]);
                        break;
                    }

                    if (i == histogram.Length - 1)
                        endPoint = new Point(i, histogram[i]);
                }
            }

            if (!maxReached)
                return;

            int centeredPeak = (startPoint.X + endPoint.X) / 2;
            peakPoint = new Point(centeredPeak, histogram[centeredPeak]);

            int rightCounts = 0;
            int leftCounts = 0;
            int consecutiveUps = 0;
            Point minReached = Point.Empty;

            for (int i = peakPoint.X + 1; i < histogram.Length; i++)
            {
                rightCounts++;

                if (histogram[i] < 1)
                    break;

                if (histogram[i] > histogram[i - 1])
                    consecutiveUps++;
                else
                    consecutiveUps = 0;

                if (minReached == Point.Empty || histogram[i] < minReached.Y)
                    minReached = new Point(i, histogram[i]);

                if (histogram[i] > minReached.Y + minValleyToPeak || consecutiveUps > 20)
                {
                    rightCounts = minReached.X - peakPoint.X;
                    break;
                }
            }

            consecutiveUps = 0;
            minReached = Point.Empty;

            for (int i = peakPoint.X; i > 0; i--)
            {
                leftCounts++;

                if (histogram[i] < 1)
                    break;

                if (histogram[i - 1] > histogram[i])
                    consecutiveUps++;
                else
                    consecutiveUps = 0;

                if (minReached == Point.Empty || histogram[i] < minReached.Y)
                    minReached = new Point(i, histogram[i]);

                if (histogram[i] > minReached.Y + minValleyToPeak || consecutiveUps > 20)
                {
                    leftCounts = peakPoint.X - minReached.X;
                    break;
                }
            }

            int peakWidth = leftCounts + rightCounts;

            int peakHeightLeft = peakPoint.Y - histogram[peakPoint.X - leftCounts];
            int peakHeightRight = peakPoint.Y - histogram[peakPoint.X + rightCounts];

            int peakHeight = Math.Max(peakHeightLeft, peakHeightRight);

            int peakSurroundLeft = Math.Min(leftCounts, 100);
            int peakSurroundRight = Math.Min(rightCounts, 100);

            peakHeightLeft = peakPoint.Y - histogram[peakPoint.X - peakSurroundLeft];
            peakHeightRight = peakPoint.Y - histogram[peakPoint.X + peakSurroundRight];

            int peakSurroundHeight = Math.Max(peakHeightLeft, peakHeightRight);
            double steepness = peakSurroundHeight / (double)peakHeight;

            peaks.Add(new Peak(peakPoint.X, peakPoint.Y, peakWidth, peakHeight, steepness));

            int maskStart = peakPoint.X - leftCounts;
            int maskEnd = peakPoint.X + rightCounts;

            maskStart = Math.Max(maskStart, 0);
            maskEnd = Math.Min(maskEnd, maskedHist.Length);

            for (int i = maskStart; i < maskEnd; i++)
                maskedHist[i] = 0;

            FindPeaks(maskedHist, ref peaks, ++iteration, valleyPercent);
        }
    }
}
