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
using System.Data;

namespace Seagate.AAS.Utils.PointStore
{
    /// <summary>
    /// TeachPointHistory stores data related to a single point with n dimensions.
    /// </summary>
    public class TeachPointHistory 
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        private ArrayList  history = new ArrayList();
        public int         dimension = 0;
        public int         queueSize = 0;
        private DataTable _dtHist;
        private DataTable _dtStat;

        // statistics
        private double[]    average;
        private double[]    stdev;
        private double[]    sum;
        private double[]    sumX2;
        private double[]    min;
        private double[]    max;

        // Constructors & Finalizers -------------------------------------------

        public TeachPointHistory()
        {}

        public TeachPointHistory(TeachPoint teachPoint, int historySize)
        {
            this.dimension = teachPoint.Dimension;
            this.queueSize = historySize;

            average = new double[dimension];
            stdev   = new double[dimension];
            sum   = new double[dimension];
            sumX2 = new double[dimension];
            min   = new double[dimension];
            max   = new double[dimension];

            ResetStatistics();

            CreateDBTable(teachPoint);
        }
        
        // Properties ----------------------------------------------------------

        public int HistorySize   
        { 
            get {return queueSize;} 
            set {queueSize = value;}
        }

        [XmlArray()]
        public ArrayList History
        {
            get {return history;}
            set {history = value;}
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
        public DataTable HistoryTable
        { get { return _dtHist; } }
        public DataTable StatisticsTable
        { get { return _dtStat; } }


        // Methods -------------------------------------------------------------
        
        public void ArchiveTeachPoint(TeachPoint point)
        {
            NPoint pt = new NPoint(point);
            history.Add(pt);

            UpdateStatistics(point);

            // remove oldest element(s) added (fifo)
            while (history.Count > queueSize) 
                history.RemoveAt(0);

            LoadHistoryTable();
        }
        
        public void LoadInit(TeachPoint point)
        {
            average = new double[dimension];
            stdev   = new double[dimension];
            sum     = new double[dimension];
            sumX2   = new double[dimension];
            min     = new double[dimension];
            max     = new double[dimension];

            Recalculate();
            CreateDBTable(point);
            LoadHistoryTable();
            LoadStatisticsTable();
        }

        // Internal methods ----------------------------------------------------

        private void ResetStatistics()
        {
            for (int i=0; i<dimension; i++)
            {
                sum[i]   = 0.0;
                sumX2[i] = 0.0;
                max[i]   = 0.0;
                min[i]   = 0.0;
            }
        }

        public void Recalculate()
        {
            ResetStatistics();

            for (int index=0; index<history.Count; index++)
            {
                NPoint pt = ((NPoint)history[index]);
                for (int i=0; i<dimension; i++)
                {
                    if (index == 0) 
                    {
                        sum[i]   = pt.Coordinate[i];
                        sumX2[i] = pt.Coordinate[i] * pt.Coordinate[i];
                        max[i]   = pt.Coordinate[i];
                        min[i]   = pt.Coordinate[i];
                    }
                    else
                    {
                        sum[i]   += pt.Coordinate[i];
                        sumX2[i] += (pt.Coordinate[i] * pt.Coordinate[i]);
                        if (pt.Coordinate[i] > max[i]) max[i] = pt.Coordinate[i];
                        if (pt.Coordinate[i] < min[i]) min[i] = pt.Coordinate[i];
                    }
                }

            }

            double tempVal = 0;
            for (int i=0; i<dimension; i++)
            {
                average[i] = sum[i]/history.Count;
                if (history.Count>1)
                {
                    tempVal = (history.Count*sumX2[i] - sum[i]*sum[i])/(history.Count*(history.Count-1));
                }
                if (tempVal > 0)
                    stdev[i] = Math.Sqrt(tempVal);
                else
                    stdev[i] = 0;
            }
        }
        
        private void UpdateStatistics(TeachPoint pt)   
        {
            for (int i=0; i<pt.Dimension; i++)
            {
                if (history.Count == 1) 
                {
                    sum[i]   = pt.Coordinate[i];
                    sumX2[i] = pt.Coordinate[i] * pt.Coordinate[i];
                    max[i]   = pt.Coordinate[i];
                    min[i]   = pt.Coordinate[i];
                }
                else
                {
                    sum[i]   += pt.Coordinate[i];
                    sumX2[i] += (pt.Coordinate[i] * pt.Coordinate[i]);
                    if (pt.Coordinate[i] > max[i]) max[i] = pt.Coordinate[i];
                    if (pt.Coordinate[i] < min[i]) min[i] = pt.Coordinate[i];
                }
            }

            double tempVal = 0;
            for (int i=0; i<pt.Dimension; i++)
            {
                average[i] = sum[i]/history.Count;

                if (history.Count > 2)
                    tempVal = (history.Count*sumX2[i] - sum[i]*sum[i])/(history.Count*(history.Count-1));
                if (tempVal > 0)
                    stdev[i] = Math.Sqrt(tempVal);
                else
                    stdev[i] = 0;
            }

            LoadStatisticsTable();
        }

        private void CreateDBTable(TeachPoint tpoint)
        {
            // Create a new DataTable titled 'Points.'
            _dtHist = new DataTable("History"); 
           
            // check/add column(s); add dimensions
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint[dim];

                int colIdx;
                for (colIdx = 0; colIdx < _dtHist.Columns.Count; colIdx++)
                {
                    string sColName = _dtHist.Columns[colIdx].Caption;
                    if (sColName == dimName)
                        break;
                }
                if (colIdx == _dtHist.Columns.Count)
                {
                    // not found, add it in
                    DataColumn axisPositionColumn = new DataColumn(); 
                    axisPositionColumn.DataType = System.Type.GetType("System.Double"); 
                    axisPositionColumn.Caption = dimName; 
                    axisPositionColumn.ColumnName = dimName; 
                    axisPositionColumn.AllowDBNull = true; 
                    _dtHist.Columns.Add(axisPositionColumn);
                }
            }

            /*
                        DataColumn categoryColumn = new DataColumn();
                        categoryColumn.DataType = System.Type.GetType("System.String");
                        categoryColumn.Caption = "Category"; 
                        categoryColumn.ColumnName = "Category";
                        categoryColumn.AllowDBNull = false; 
                        _dtHist.Columns.Add(categoryColumn);

                        DataColumn pointIDColumn = new  DataColumn();
                        pointIDColumn.DataType = System.Type.GetType("System.Int32");
                        pointIDColumn.Caption = "ID"; 
                        pointIDColumn.ColumnName = "ID";
                        _dtHist.Columns.Add(pointIDColumn);
            */

            DataColumn timestampColumn = new DataColumn();
            timestampColumn.DataType = System.Type.GetType("System.DateTime");
            timestampColumn.Caption = "Time"; 
            timestampColumn.ColumnName = "Time";
            timestampColumn.AllowDBNull = false; 
            _dtHist.Columns.Add(timestampColumn);

            DataColumn guidColumn = new DataColumn();
            guidColumn.DataType = System.Type.GetType("System.String");
            guidColumn.Caption = "GUID"; 
            guidColumn.ColumnName = "GUID";
            guidColumn.AllowDBNull = false; 
            _dtHist.Columns.Add(guidColumn);


            /////////////////////////////
            // create Statistics table

            _dtStat = new DataTable("Statistics"); 

            DataColumn statItemCol = new DataColumn();
            statItemCol.DataType = System.Type.GetType("System.String");
            statItemCol.Caption = "Item"; 
            statItemCol.ColumnName = "Item";
            statItemCol.AllowDBNull = true; 
            _dtStat.Columns.Add(statItemCol);

            // check/add column(s); add dimensions
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint[dim];

                int colIdx;
                for (colIdx = 0; colIdx < _dtStat.Columns.Count; colIdx++)
                {
                    string sColName = _dtStat.Columns[colIdx].Caption;
                    if (sColName == dimName)
                        break;
                }
                if (colIdx == _dtStat.Columns.Count)
                {
                    DataColumn axisStatColumn = new DataColumn(); 
                    axisStatColumn.DataType = System.Type.GetType("System.Double"); 
                    axisStatColumn.Caption = dimName; 
                    axisStatColumn.ColumnName = dimName; 
                    axisStatColumn.AllowDBNull = true; 
                    _dtStat.Columns.Add(axisStatColumn);
                }
            }
        }

        private void LoadHistoryTable()
        {
            _dtHist.Clear();

            foreach(NPoint tpoint in history) 
            {
                DataRow dr = _dtHist.NewRow();

                // check/add column(s); add dimensions
                for(int dim = 0; dim < tpoint.Dimension; dim++)
                {
                    string dimName  = tpoint.DimensionNames[dim];
                    double dimValue = tpoint.Coordinate[dim];
                    dr[dimName] = dimValue.ToString();
                }

                //dr["Name"]     = tpoint.Name;                
                //dr["ID"]       = tpoint.ID;
                //dr["Category"] = "";
                dr["GUID"]     = tpoint.MiscText;
                dr["Time"]     = DateTime.Now;
                
                _dtHist.Rows.Add(dr);
            }
        }

        private void LoadStatisticsTable()
        {
            if (history.Count == 0)
                return;

            NPoint tpoint = (NPoint)history[0];
            
            _dtStat.Clear();
            DataRow dr = null;

            // Average
            dr = _dtStat.NewRow();
            dr["Item"] = "Average";                
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint.Coordinate[dim];
                dr[dimName] = average[dim].ToString("F3");
            }
            _dtStat.Rows.Add(dr);

            // stdev
            dr = _dtStat.NewRow();
            dr["Item"] = "stdev";                
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint.Coordinate[dim];
                dr[dimName] = stdev[dim].ToString("F3");
            }
            _dtStat.Rows.Add(dr);

            // sum
            dr = _dtStat.NewRow();
            dr["Item"] = "sum";                
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint.Coordinate[dim];
                dr[dimName] = sum[dim].ToString("F3");
            }
            _dtStat.Rows.Add(dr);

            // sumX2
            dr = _dtStat.NewRow();
            dr["Item"] = "sumX2";                
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint.Coordinate[dim];
                dr[dimName] = sumX2[dim].ToString("F3");
            }
            _dtStat.Rows.Add(dr);

            // min
            dr = _dtStat.NewRow();
            dr["Item"] = "min";                
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint.Coordinate[dim];
                dr[dimName] = min[dim].ToString();
            }
            _dtStat.Rows.Add(dr);

            // max
            dr = _dtStat.NewRow();
            dr["Item"] = "max";                
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint.Coordinate[dim];
                dr[dimName] = max[dim].ToString();
            }
            _dtStat.Rows.Add(dr);
        }
    }
}
