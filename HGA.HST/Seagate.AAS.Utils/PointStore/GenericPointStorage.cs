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
    public enum StoreOrder
    {
        OldestFirst,
        OldestLast,
    }

    /// <summary>
    /// GenericPointStorage stores a history of NPoints of m dimensions.
    /// </summary>
    //[XmlInclude(typeof(NPoint))]
    public class GenericPointStorage 
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        protected string      _storageName = "";          // storage name
        protected int         _maxSize = 0;               // max number of points; fifo
        protected ArrayList   _values = new ArrayList();  // the points
        protected int         _dimension;                 // number of coordinates
        protected StoreOrder  _order;                     // order of storage
        private string[]      _name;                      // names of the coordinates
     
        public delegate void OnAddPointHandler();
        public event OnAddPointHandler OnAddPoint;

        // Constructors & Finalizers -------------------------------------------
        public GenericPointStorage()
        {
        }
        public GenericPointStorage(string name, int dimension, int maxSize, params string[] dimensionNames)
        {
            Init(name, dimension, maxSize, dimensionNames);
        }
        
        public GenericPointStorage(string name, int maxSize, params string[] dimensionNames)
        {
            Init(name, dimensionNames.Length, maxSize, dimensionNames);            
        }
        
        private void Init(string name, int dimension, int maxSize, params string[] dimensionNames)
        {
            _storageName = name;
            _dimension = dimension;
            _maxSize = maxSize;
            _name = dimensionNames;
            _order = StoreOrder.OldestLast;
        }

        public void Copy(GenericPointStorage src)
        {
            _storageName = src._storageName;
            _dimension = src._dimension;
            _maxSize = src._maxSize;
            _name = src._name;
            _order = src._order;
            
            _values = src._values.Clone() as ArrayList;     // shallow copy

            // make it a deep copy
            for(int i=0; i<_values.Count; i++)
                _values[i] = new NPoint(src._values[i] as NPoint);
        }

        // Properties ----------------------------------------------------------
        #region Properties

        public string   Name        
        { 
            get{return _storageName;}      
            set{_storageName = value;}      
        }
        
        public int Dimension   
        { 
            get {return _dimension;} 
            set {_dimension = value;}
        }

        public string[] DimensionName
        { 
            get {return _name;} 
            set {_name = value;}
        }

        public int MaxSize   
        { 
            get {return _maxSize;} 
            set {_maxSize = value;}
        }

        public int QueueSize   
        { 
            get {return _maxSize;} 
            set {_maxSize = value;}
        }

        public int Size
        { 
            get {return _values.Count;}
        }

        public ArrayList Values
        {
            get {return _values;}
        }        

        public ArrayList History
        {
            get {return _values;}
        }

        public StoreOrder Order
        {
            get {return _order;}
            set {_order = value;}
        }

        public NPoint this[int index]
        {
            get
            {
                if(index >= 0 && index < _values.Count)
                    return _values[index] as NPoint;
                else
                {
                    string msg = string.Format("index {0} is not between 0 and {1}!!!",index,_dimension);
                    throw new Exception(msg);
                }
            }           
        }

        public NPoint CurrentPoint
        {
            get
            {
                if(_order == StoreOrder.OldestLast)
                    return (NPoint) _values[0];
                else
                    return (NPoint) _values[_values.Count -1]; 
            }
        }

        #endregion

        // Methods -------------------------------------------------------------
                       
        public NPoint AddPoint(params double[] doubleList)
        {           
            NPoint point = new NPoint(_dimension);
            point.Coordinate = doubleList;
          
            Add(point);
            CallHandler();

            return point;
        }                 

        public NPoint AddPoint(String name, params double[] doubleList)
        {           
            NPoint point = new NPoint(name, _dimension);
            point.Coordinate = doubleList;
          
            Add(point);
            CallHandler();

            return point;
        }   

        public void AddPoint(NPoint point)
        {           
           Add(point);           
           CallHandler();
        }   

        public void RemovePoint(int index)
        {           
            if(index < _values.Count && index >= 0)
                _values.RemoveAt(index);            
        }

        protected void Add(NPoint point)
        {           
            if(_order == StoreOrder.OldestFirst)
            {                
                _values.Add(point);
                
                if(_values.Count > _maxSize) 
                    _values.RemoveAt(0);                   // remove oldest element (fifo)
            }
            else
            {
                _values.Insert(0, point);
                
                if(_values.Count > _maxSize) 
                    _values.RemoveAt(_values.Count -1);    // remove oldest element (fifo)
            }
        }

        protected NPoint Add(params double[] doubleList)
        {           
            NPoint point = new NPoint(_dimension);
            point.Coordinate = doubleList;
            
            Add(point);

            return point;
        }

        public void RemoveAll()
        {                       
            _values.Clear();
        }

        protected void CallHandler()
        {           
            if (OnAddPoint != null)
            {
                OnAddPoint();
            }
        }
    }


    /// <summary>
    /// StatPointStorage adds statistics to GenericPointStorage.
    /// </summary>
    //[XmlInclude(typeof(NPoint))]
    public class StatPointStorage : GenericPointStorage
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        protected double[]    _average;                   // the average for any one coordinate contained in all the points in the N space
        protected double[]    _stdev;                     // the standard deviation for "     "       "
        protected double[]    _sum;                       // the sum for "        "       "
        protected double[]    _sumX2;                     // sum of the coordinates squared   "       "       "
        protected double[]    _min;                       // minimum value for all the coordinates    "       "       " 
        protected double[]    _max;                       // maximum value for all the coordinates    "       "       "
           
        // Constructors & Finalizers -------------------------------------------
        public StatPointStorage()
        {
        }
        public StatPointStorage(string name, int dimension, int maxSize, params string[] dimensionNames) :
            base(name, dimension, maxSize, dimensionNames)
        {
            Init();
        }
        
        public StatPointStorage(string name, int maxSize, params string[] dimensionNames) :
            base(name, maxSize, dimensionNames)
        {
            Init();
        }
        
        private void Init()
        {            
            _average = new double[_dimension];
            _stdev   = new double[_dimension];
            _sum   = new double[_dimension];
            _sumX2 = new double[_dimension];
            _min   = new double[_dimension];
            _max   = new double[_dimension];
        }

        public void Copy(StatPointStorage src)
        {
            base.Copy(src);

            Init();

            for (int i=0; i<_dimension; i++)
            {
                _sum[i]   = src._sum[i];
                _sumX2[i] = src._sumX2[i];
                _max[i]   = src._max[i];
                _min[i]   = src._min[i];
                _average[i] = src._average[i];
                _stdev[i] = src._stdev[i];
            }
        }

        // Properties ----------------------------------------------------------
        #region Properties       

        public double[] Average
        {
            get {return _average;}
        }
        public double[] Stdev
        {
            get {return _stdev;}
        }
        public double[] Min
        {
            get {return _min;}
        }
        public double[] Max
        {
            get {return _max;}
        }
     
        #endregion

        // Methods -------------------------------------------------------------
        new public NPoint AddPoint(params double[] doubleList)
        {           
            NPoint point = Add(doubleList);

            Calculate();

            CallHandler();
      
            return point;
        }
        
        public void ResetStatistics()
        {
            for (int i=0; i<_dimension; i++)
            {
                _sum[i]   = 0;
                _sumX2[i] = 0;
                _max[i]   = 0;
                _min[i]   = 0;
                _average[i] = 0;
                _stdev[i] = 0;
            }
        }

        public void Recalculate()
        {
            Init();
            Calculate();       
        }        
        
        public void Calculate()
        {            
            ResetStatistics();

            UpdateSums();    
  
            CalculateStatistics();          
        }   

        protected void UpdateSums()   
        {       
            int dimension;                         // size of array
            double [] values;                      // coordinate array
       
            // "initial" values 
            if(_values.Count > 0) 
            {
                values = ((NPoint)_values[0]).Coordinate;
                dimension = ((NPoint)_values[0]).Dimension;
    
                for (int i=0; i<dimension; i++)
                {
                    _sum[i]   = values[i];
                    _sumX2[i] = values[i] * values[i];
                    _max[i]   = values[i];
                    _min[i]   = _max[i];
                }
            }
            
            // get the rest
            int size = _values.Count;
            for(int index=1; index<size; index++)
            {
                values = ((NPoint)_values[index]).Coordinate;          
                dimension = ((NPoint)_values[index]).Dimension;

                for (int i=0; i<dimension; i++)
                {
                    _sum[i]   += values[i];
                    _sumX2[i] += values[i] * values[i];
                    if (values[i] > _max[i]) _max[i] = values[i];
                    if (values[i] < _min[i]) _min[i] = values[i];
                }
            }                 
        }

        protected void CalculateStatistics()
        {           
            int count = _values.Count;             // history size

            double tempVal = 0;
            for(int i=0; i<_dimension; i++)
            {
                _average[i] = _sum[i]/count;         // should always be at least 1 when called
                if (count>1)
                {
                    tempVal = (count*_sumX2[i] - _sum[i]*_sum[i])/(count*(count-1));
                }
                if (tempVal > 0)
                    _stdev[i] = Math.Sqrt(tempVal);
                else
                    _stdev[i] = 0;
            }                  
        }
    }
}
