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
//  [5/22/2006]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

namespace Seagate.AAS.Utils.PointStore
{
    public class NPoint
    {
        // Nested declarations -------------------------------------------------
    
        // Member variables ----------------------------------------------------
        
        protected double[]    space;
        protected string[]    dimensionNames;
        protected DateTime    timeStamp;
        protected int         dimension;
        protected string      name;
        protected int         pointID;
        protected string      misc;

        // Constructors & Finalizers -------------------------------------------
        public NPoint() 
        {
        }

        public NPoint(int dimension)
        {
            this.dimension = dimension;
            this.timeStamp = DateTime.Now;
			this.space = new double[dimension];
			this.dimensionNames = new string[dimension];
			this.pointID = -1;
            this.name = "";
			this.misc = "";
        }

        public NPoint(String name, int dimension)
        {
            this.dimension = dimension;
            this.timeStamp = DateTime.Now;
			this.space = new double[dimension];
			this.dimensionNames = new string[dimension];
			this.pointID = -1;
            this.name = name;
			this.misc = "";
        }

        public NPoint(string name, string[] dimNames, double[] defaultValues) 
        {
            this.name           = name;
			this.pointID		= -1;
            this.dimension      = dimNames.Length;
            this.timeStamp      = DateTime.Now;
            this.space          = new double[dimension];
            this.dimensionNames = new string[dimension];

            for(int i=0; i<dimension; i++)
            {
                this.space[i]          = defaultValues[i];
                this.dimensionNames[i] = dimNames[i];
            }
        }

        public NPoint(NPoint point) 
        {
            this.name           = point.name;
            this.pointID        = point.pointID;
            this.dimension      = point.dimension;
            this.space          = new double[dimension];
            this.dimensionNames = new string[dimension];

            for(int i=0; i<dimension; i++)
            {
                this.space[i]          = point.space[i];
                this.dimensionNames[i] = point.dimensionNames[i];
            }

            this.timeStamp = new DateTime(timeStamp.Ticks);
            this.misc      = point.misc;
        }

        // Properties ----------------------------------------------------------
        
        public int ID
        { 
            get { return pointID; }
            set { pointID = value; }
        }

        public DateTime TimeStamp
        { get { return timeStamp; } }

        public int Dimension
        { 
            get { return dimension; } 
            set { dimension = value; } 
        }

        public string[] DimensionNames
        { 
            get {return dimensionNames;} 
            set {dimensionNames = value;}
        }

        public string Name
        {
            get { return name; }
            set { name = value; } 
        }

        public double[] Coordinate
        { 
            get { return space; } 
            set { space = value; } 
        }        

        public double this[int index]
        {
            get { return GetCoordinate(index); }
            set { SetCoordinate(index, value); }
        }

        public string MiscText
        {
            get { return misc; }
            set { misc = value; } 
        }

        // Methods -------------------------------------------------------------

        public static bool operator==(NPoint l, NPoint r )
        {
            if( object.ReferenceEquals( l, r ) )
                return true;
            else if(   object.ReferenceEquals( l, null )
                    || object.ReferenceEquals( r, null ) )
                return false;
            else
            {
                bool samePoint = true;
                if (l.Dimension == r.Dimension)
                {
                    // compare coordinates
                    for (int dim = 0; dim < l.Dimension; dim++)
                    {
                        if (l[dim] != r[dim])
                        {
                            samePoint = false;
                            break;
                        }
                    }
                }
                return samePoint;
            }
        }

        public static bool operator!=( NPoint l, NPoint r )
        {
            return !(l == r);
        }

        public override bool Equals(object obj)
        {
            if( obj == null )
                return false;
        
            NPoint o = obj as NPoint;
        
            if( o != null )    
                return this == o;
            return false;
        }

        public override string ToString()
        {
            string s = (dimension > 0) ? space[0].ToString() : "";

            for(int i=1; i<dimension; i++) 
                s += "," + space[i].ToString();

            return s;
        }

        public bool IsSameCoordinates(params double[] coordinates)
        {
            bool sameCoords = true;
            for(int dim = 0; dim < coordinates.Length; dim++) 
            {
                if (space[dim] != coordinates[dim])
                {
                    sameCoords = false;
                    break;
                }
            }
            return sameCoords;
        }

        public void Set(params double[] coordinates)
        {
            Debug.Assert(coordinates.Length == dimension);

            for(int dim = 0; dim < coordinates.Length; dim++) 
            {
                space[dim] = coordinates[dim];
            }

            this.timeStamp = DateTime.Now;
        }

        public void ShallowClone(NPoint point)
        {
            this.space          = point.space         ;
            this.dimensionNames = point.dimensionNames;
            this.timeStamp      = point.timeStamp     ;
            this.dimension      = point.dimension     ;
            this.name           = point.name          ;
            this.pointID        = point.pointID       ;
            this.misc           = point.misc          ;
        }

        // Internal methods ----------------------------------------------------

        protected double GetCoordinate(int index)
        {
            if(index >= 0 && index < space.Length)
                return space[index];
            else
            {
                string msg = string.Format("index {0} is not between 0 and {1}!!!",index,dimension);
                throw new Exception(msg);
            }
        }

        protected void SetCoordinate(int index, double cordinateValue)
        {
            if(index >= 0 && index < space.Length)
                space[index] = cordinateValue;
            else
            {
                string msg = string.Format("index {0} is not between 0 and {1}!!!",index,dimension);
                throw new Exception(msg);
            }        
        }
    }
}
