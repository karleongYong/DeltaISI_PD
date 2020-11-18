//
//  © Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [June 2006 to present, Seagate HDA Automation Team]
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Utils.Maths
{
    public class LeastSquaresFit
    {
    }

    /// <summary>
    /// Summary description for SimpleLinearRegression
    /// Class to perform simple linear regression (least squares) as described in 
    /// Applied Statistics and Probability for Engineers (Montgomery & Runger)
    /// p. 435 - 464
    /// </summary>
    public class SimpleLinearRegression
    {
        private double[,] data;
        private double m;					// slope
        private double b;					// intercept
        private const int X = 0;
        private const int Y = 1;
        private double[] residuals;
        private double rSquared = 0.0;

        public SimpleLinearRegression(double[,] points)
        {
            data = points;
            Solve();
        }

        public SimpleLinearRegression(double[] x, double[] y)
        {
            // data = new double[,]{x,y}; // creates a 2xn array, not nx2
            data = new double[x.Length, 2];
            for (int i = 0; i < x.Length; i++)
            {
                data[i, X] = x[i];
                data[i, Y] = y[i];
            }
            Solve();
        }

        public double Slope
        {
            get { return m; }
        }

        public double Intercept
        {
            get { return b; }
        }

        public double[] Residuals
        {
            get { return residuals; }
        }

        public double RSquared
        {
            get { return rSquared; }
        }

        private void Solve()
        {
            // solve m first.  Need to compute sums: 
            // 1) x*y for each point
            // 2) sum of x's & y's
            // 3) x^2 for each point
            
            double XiYi = 0;
            double Yi = 0;
            double Xi = 0;
            double XiSquared = 0;
            double n = (double)data.Length/2;

            for(int i=0; i<data.Length/2; i++)
            {
                XiYi += data[i,X] * data[i,Y];
                XiSquared += data[i,X] * data[i,X];
                Xi += data[i,X];
                Yi += data[i,Y];
            }

            // now compute m (slope)
            double Sxy = (XiYi - (Xi * Yi / n));
            m = Sxy / (XiSquared - (Xi * Xi) / n);

            // and b (intercept)
            b = Yi/n - m * Xi/n;

            // now compute residual values... yi - (mx+b)
            double SSe = 0.0;       // error sum of squares
            residuals = new double[data.Length / 2];
            for (int i = 0; i < residuals.Length; i++)
            {
                double yhat = b + m * data[i, X];
                residuals[i] = data[i, Y] - yhat;
                SSe += Math.Pow(residuals[i], 2.0);
            }

            // now we can compute R^2 where
            // R^2 = 1 - SSe / SSt, and
            // SSt = SSe + m * Sxy, so
            // R^2 = 1 - SSe / (SSe + m * Sxy)
            rSquared = 1 - SSe / (SSe + m * Sxy);
        }
    }
}
