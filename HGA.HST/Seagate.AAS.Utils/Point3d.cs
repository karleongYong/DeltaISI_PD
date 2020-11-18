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
//  [4/14/2006]
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace CAAL.Workcell.SBJ
{
	/// <summary>
	/// Summary description for Point3D.
	/// </summary>
	public class Point3D
	{        
        // Member variables ----------------------------------------------------
        
        public double x;
        public double y;
        public double z;
        
        // Constructors & Finalizers -------------------------------------------

        public Point3D()
        {
        }

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3D(Point3D pt)
        {
            this.x = pt.x;
            this.y = pt.y;
            this.z = pt.z;
        }
         
        // Methods -------------------------------------------------------------
	}
}
