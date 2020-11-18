//
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
//  [9/8/2005]
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Xml.Serialization;

namespace Seagate.AAS.Utils.PointStore
{
    [Serializable()]
    public class RobotPoint
    {
        // Nested declarations -------------------------------------------------
    
        // Member variables ----------------------------------------------------
        private double[] coordinate;
        public DateTime   timeStamp;
        private int       dimension;

        // Constructors & Finalizers -------------------------------------------
        public RobotPoint() 
        {
        }
        public RobotPoint(int dimension)
        {
            this.dimension = dimension;
            timeStamp = DateTime.Now;
            coordinate = new double[dimension];
        }
		public RobotPoint(RobotPoint copy)
		{
			this.dimension = copy.dimension;
			this.timeStamp = DateTime.Now;
			this.coordinate = new double[copy.Coordinate.Length];
			for(int i=0; i<coordinate.Length; i++)
			{
				coordinate[i] = copy[i];
			}
		}

        // Properties ----------------------------------------------------------
        public DateTime TimeStamp { get {return timeStamp;}}

        public double[] Coordinate { get {return coordinate;} set {coordinate = value;}}

        public double this[int index]
        {
            get
            {
                if(index >= 0 && index < coordinate.Length)
                    return coordinate[index];
                else
                {
                    string msg = string.Format("index {0} is not between 0 and {1}!!!",index,dimension);
                    throw new Exception(msg);
                }
            }
        }

        // Methods -------------------------------------------------------------
        public override string ToString()
        {
            string s;
            s = "";
            for ( int i=0; i<dimension; i++) 
            {
                s += coordinate[i].ToString();
                if (i<dimension-1)
                    s+= ",";
            }
            return s;
        }

        // Internal methods ----------------------------------------------------
    }


    /// <summary>
    /// PointStorage stores data related to a single point with n dimensions.
    /// </summary>
    //[XmlInclude(typeof(RobotPoint))]
    [Serializable()]
    public class PointStorage 
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        private string      name = "";
        private int         dimension = 0;
        private int         queueSize = 0;
        private ArrayList   history = new ArrayList();
        private string[]    dimensionName;
        private double[]    average;
        private double[]    stdev;
        private double[]    sum;
        private double[]    sumX2;
        private double[]    min;
        private double[]    max;
        private long        count = 0;

        public delegate void OnAddPointHandler();
        public event OnAddPointHandler OnAddPoint;

        // Constructors & Finalizers -------------------------------------------
        public PointStorage()
        {
        }
        public PointStorage(string name, int dimension, int queueSize, params string[] dimensionNames)
        {
            this.name = name;
            this.dimension = dimension;
            this.queueSize = queueSize;
            this.dimensionName = dimensionNames;

            average = new double[dimension];
            stdev   = new double[dimension];
            sum   = new double[dimension];
            sumX2 = new double[dimension];
            min   = new double[dimension];
            max   = new double[dimension];
        }
        
        
        // Properties ----------------------------------------------------------
        public string   Name        
        { 
            get{return name;}      
            set{name = value;}      
        }
        
        public int Dimension   
        { 
            get {return dimension;} 
            set {dimension = value;}
        }

        public string[] DimensionName
        { 
            get {return dimensionName;} 
            set {dimensionName = value;}
        }


        public int QueueSize   
        { 
            get {return queueSize;} 
            set {queueSize = value;}
        }

        public ArrayList History
        {
            get {return history;}
        }

        public double[] Average
        {
            get {return average;}
        }
        public double[] Stdev
        {
            get {return stdev;}
        }
        public double[] Min
        {
            get {return min;}
        }
        public double[] Max
        {
            get {return max;}
        }

        public RobotPoint    CurrentPoint{ get{return (RobotPoint) history[0];} }

        // Methods -------------------------------------------------------------
        public RobotPoint AddRobotPoint(params double[] doubleList)
        {
            UpdateStatistics(doubleList);

            RobotPoint point = new RobotPoint(dimension);
            point.Coordinate = doubleList;

            history.Insert(0, point);

            // remove oldest element(s) added (fifo)
            while (history.Count > queueSize) 
            {
                history.RemoveAt(history.Count-1);  
            }

            if (OnAddPoint != null)
            {
                OnAddPoint();
            }

            return point;
        }
        
        public void ResetStatistics()
        {
            count = 0;
            for (int i=0; i<dimension; i++)
            {
                sum[i]   = 0;
                sumX2[i] = 0;
                max[i]   = 0;
                min[i]   = 0;
            }
        }

        public void Recalculate()
        {
            average = new double[dimension];
            stdev   = new double[dimension];
            sum   = new double[dimension];
            sumX2 = new double[dimension];
            min   = new double[dimension];
            max   = new double[dimension];

            ResetStatistics();

            for (int index=0; index<history.Count; index++)
            {
                double [] doubleList = ((RobotPoint) history[index]).Coordinate;
                for (int i=0; i<dimension; i++)
                {
                    if (index == 0) 
                    {
                        sum[i]   = doubleList[i];
                        sumX2[i] = doubleList[i] * doubleList[i];
                        max[i]   = doubleList[i];
                        min[i]   = doubleList[i];
                    }
                    else
                    {
                        sum[i]   += doubleList[i];
                        sumX2[i] += (doubleList[i] * doubleList[i]);
                        if (doubleList[i] > max[i]) max[i] = doubleList[i];
                        if (doubleList[i] < min[i]) min[i] = doubleList[i];
                    }
                }

            }

            double tempVal = 0;
            count = history.Count;
            for (int i=0; i<dimension; i++)
            {
                average[i] = sum[i]/count;
                if (count>1)
                {
                    tempVal = (count*sumX2[i] - sum[i]*sum[i])/(count*(count-1));
                }
                if (tempVal > 0)
                    stdev[i] = Math.Sqrt(tempVal);
                else
                    stdev[i] = 0;
            }
        }
        // Internal methods ----------------------------------------------------
        private void UpdateStatistics(params double[] doubleList)   
        {
            for (int i=0; i<dimension; i++)
            {
                if (count == 0) 
                {
                    sum[i]   = doubleList[i];
                    sumX2[i] = doubleList[i] * doubleList[i];
                    max[i]   = doubleList[i];
                    min[i]   = doubleList[i];
                }
                else
                {
                    sum[i]   += doubleList[i];
                    sumX2[i] += (doubleList[i] * doubleList[i]);
                    if (doubleList[i] > max[i]) max[i] = doubleList[i];
                    if (doubleList[i] < min[i]) min[i] = doubleList[i];
                }
            }

            double tempVal = 0;
            count++;
            for (int i=0; i<dimension; i++)
            {
                average[i] = sum[i]/count;
                if (count>1)
                {
                    tempVal = (count*sumX2[i] - sum[i]*sum[i])/(count*(count-1));
                }
                if (tempVal > 0)
                    stdev[i] = Math.Sqrt(tempVal);
                else
                    stdev[i] = 0;
            }

        }


    }
}
