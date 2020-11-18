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
    public delegate double MinFunction(double x);

    // method to find the minimum value of function in the interval p1-p2.
    // adapted from Numerical Recipes in C, pages 397-402.
    public class FunctionMinimum
    {
        private const double GOLD = 1.618034;
        private const double GLIMIT = 100.0;
        private const double TINY = 1.0e-20;
        private const double R = 0.61803399;
        private const double C = 1.0 - R;

        private MinFunction function = null;
        private double ax = 0.0;
        private double bx = 0.0;
        private double cx = 0.0;
        private double fa = 0.0;
        private double fb = 0.0;
        private double fc = 0.0;

        public FunctionMinimum()
        {
        }

        // value representing a practical limit to algorithm resolution
        // recomended value is square root of machine's floating point precision, but
        // that is still rediculously small (5e-324)
        private double tolerance = 1e-10;
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        public double FindMinimum(MinFunction fun, double p1, double p2)
        {
            function = fun;
            ax = p1;
            bx = p2;

            BracketMinimum();

            double xmin;
            double minval = GoldenSectionSearch(out xmin);
            return minval;
        }

        private void BracketMinimum()
        {
            double ulim, u, r, q, fu = 0.0;

            fa = function(ax);
            fb = function(bx);
            if (fb > fa)
            {
                double temp = ax;
                ax = bx;
                bx = temp;

                temp = fb;
                fb = fa;
                fa = temp;
            }
            cx = bx + GOLD * (bx - ax);
            fc = function(cx);
            while (fb > fc)
            {
                r = (bx - ax) * (fb - fc);
                q = (bx - cx) * (fb - fa);
                u = bx - ((bx - cx) * q - (bx - ax) * r) / (2.0 * SIGN(Math.Max(Math.Abs(q - r), TINY), q - r));
                ulim = bx + GLIMIT * (cx - bx);

                if ((bx - u) * (u - cx) > 0.0)
                {
                    fu = function(u);
                    if (fu < fc)
                    {
                        ax = bx;
                        bx = u;
                        fa = fb;
                        fb = fu;
                        return;
                    }
                    else if (fu > fb)
                    {
                        cx = u;
                        fc = fu;
                        return;
                    }
                    u = cx + GOLD * (cx - bx);
                    fu = function(u);
                }
                else if ((cx - u) * (u - ulim) > 0.0)
                {
                    fu = function(u);
                    if (fu < fc)
                    {
                        bx = cx;
                        cx = u;
                        u = cx + GOLD * (cx - bx);

                        fb = fc;
                        fc = fu;
                        fu = function(u);
                    }
                }
                else if ((u - ulim) * (ulim - cx) >= 0.0)
                {
                    u = ulim;
                    fu = function(u);
                }
                else
                {
                    u = cx + GOLD * (cx - bx);
                    fu = function(u);
                }
                ax = bx;
                bx = cx;
                cx = u;

                fa = fb;
                fb = fc;
                fc = fu;
            }
        }

        private double GoldenSectionSearch(out double xmin)
        {
            double f1, f2, x0, x1, x2, x3;
            
            xmin = 0;
            x0 = ax;
            x3 = cx;
            if (Math.Abs(cx - bx) > Math.Abs(bx - ax))
            {
                x1 = bx;
                x2 = bx + C * (cx - bx);
            }
            else
            {
                x2 = bx;
                x1 = bx - C * (bx - ax);
            }
            f1 = function(x1);
            f2 = function(x2);

            while (Math.Abs(x3 - x0) > tolerance * (Math.Abs(x1) + Math.Abs(x2)))
            {
                if (f2 < f1)
                {
                    x0 = x1;
                    x1 = x2;
                    x2 = R * x1 + C * x3;

                    f1 = f2;
                    f2 = function(x2);
                }
                else
                {
                    x3 = x2;
                    x2 = x1;
                    x1 = R * x2 + C * x0;

                    f2 = f1;
                    f1 = function(x1);
                }
            }
            if (f1 < f2)
            {
                xmin = x1;
                return f1;
            }
            else
            {
                xmin = x2;
                return f2;
            }
        }

        private double SIGN(double a, double b)
        {
            return (b > 0.0) ? Math.Abs(a) : -Math.Abs(a);
        }

        public void DebugTest()
        {
            // should return -3.0
            double min = FindMinimum(new MinFunction(this.Function1), -5.0, 5.0);

            // should return about -2135
            min = FindMinimum(new MinFunction(this.Function2), 0.0, 15.0);
        }

        private double Function1(double x)
        {
            // y = x^2 - 3;
            return Math.Pow(x, 2.0) - 3.0;
        }

        private double Function2(double x)
        {
            // y = x^4 - 12x^3 + (x-2)^2 + 3
            return Math.Pow(x, 4.0) - 12 * Math.Pow(x, 3.0) + Math.Pow(x - 2, 2.0) + 3;
        }

    }
}
