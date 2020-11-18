//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [3/3/2006] Tom Chuang
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Seagate.AAS.Utils
{
    /// <summary>
    /// math utilities for 3D vectors 
    /// </summary>
    public class VectorMath
    {
        private const double NEAR_ZERO = 1e-8;

        public VectorMath()
        {
        }

        public static void Copy(double[] v1, double[] v2)
        {
            v2[0] = v1[0];
            v2[1] = v1[1];
            v2[2] = v1[2];
        }

        // Resulting vector goes from v2 to v1
        // v3 = v1 - v2
        public static void Subtract(double[] v1, double[] v2, double[] v3)
        {
            v3[0] = v1[0] - v2[0];
            v3[1] = v1[1] - v2[1];
            v3[2] = v1[2] - v2[2];
        }

        public static void Add(double[] v1, double[] v2, double[] v3)
        {
            v3[0] = v1[0] + v2[0];
            v3[1] = v1[1] + v2[1];
            v3[2] = v1[2] + v2[2];
        }

        public static double Length(double[] v)
        {
            return (Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]));
        }


        public static double DotProduct(double[] v1, double[] v2)
        {
            return (v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2]);
        }

        public static double Projection(double[] v1, double[] v2) // returns length of v1 along v2 direction
        {
            return (Length(v1) * (DotProduct(v1, v2) / (Length(v1) * Length(v2))));
        }



        public static void CrossProduct(double[] v1, double[] v2, double[] v3)
        {
            v3[0] = v1[1] * v2[2] - v1[2] * v2[1];
            v3[1] = v1[2] * v2[0] - v1[0] * v2[2];
            v3[2] = v1[0] * v2[1] - v1[1] * v2[0];
        }


        // returns angle in radians
        public static double AngleBetween(double[] u, double[] v)
        {
            double denom;

            denom = Length(u) * Length(v);

            if (denom != 0.0)
                return (Math.Acos(DotProduct(u, v) / denom));
            else
                return (0.0);
        }


        public static void CopyMatrix(double[,] a, double[,] b)
        {
            // a = b
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    a[i, j] = b[i, j];
                }
            }
        }

        public static double Determinant(double[,] m)
        {
            double dval = 0;
            dval = m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]);
            dval -= m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]);
            dval += m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
            return dval;
        }

        public static void ScalarMultiply(double[] v1, double scalar, double[] v2)
        {
            v2[0] = v1[0] * scalar;
            v2[1] = v1[1] * scalar;
            v2[2] = v1[2] * scalar;
        }
    }
}
