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
//  [9/9/2005]
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Data;


namespace Seagate.AAS.Utils.PointStore
{
    /// <summary>
    /// Point store
    /// </summary>
    public class TeachPointStore
    {

        // Nested declarations -------------------------------------------------
        

        // Member variables ----------------------------------------------------
        
        public delegate void PointStoreContentChangedEventHandler();
        public event PointStoreContentChangedEventHandler PointStoreChanged;

        private ArrayList _teachPoints = new ArrayList();
        private string    _fileName;
        private DataTable _dt;
        
        // Constructors & Finalizers -------------------------------------------

        public TeachPointStore()
        {
            _fileName = "TeachPointStore.xml";

            // Create a new DataTable titled 'Points.'
            _dt = new DataTable("Points"); 

            // Add columns
            DataColumn nameColumn = new DataColumn();
            nameColumn.DataType = System.Type.GetType("System.String");
            nameColumn.Caption = "Name"; 
            nameColumn.ColumnName = "Name";
            nameColumn.DefaultValue = "";
            nameColumn.AllowDBNull = false; 
            _dt.Columns.Add(nameColumn);

/*
            DataColumn categoryColumn = new DataColumn();
            categoryColumn.DataType = System.Type.GetType("System.String");
            categoryColumn.Caption = "Category"; 
            categoryColumn.ColumnName = "Category";
            categoryColumn.AllowDBNull = false; 
            _dt.Columns.Add(categoryColumn);

            DataColumn pointIDColumn = new  DataColumn();
            pointIDColumn.DataType = System.Type.GetType("System.Int32");
            pointIDColumn.Caption = "ID"; 
            pointIDColumn.ColumnName = "ID";
            _dt.Columns.Add(pointIDColumn);

            DataColumn timestampColumn = new DataColumn();
            timestampColumn.DataType = System.Type.GetType("System.DateTime");
            timestampColumn.Caption = "Time"; 
            timestampColumn.ColumnName = "Time";
            timestampColumn.AllowDBNull = false; 
            _dt.Columns.Add(timestampColumn);

            DataColumn guidColumn = new DataColumn();
            guidColumn.DataType = System.Type.GetType("System.String");
            guidColumn.Caption = "GUID"; 
            guidColumn.ColumnName = "GUID";
            guidColumn.AllowDBNull = false; 
            _dt.Columns.Add(guidColumn);
*/

            // Define keys
            DataColumn[] keys = new DataColumn[1];
            keys[0] = nameColumn;
            _dt.PrimaryKey = keys;
        }

        // Properties ----------------------------------------------------------

        public string FileName
        {
            get { return _fileName; }
            set { this._fileName = value; }
        }

        [XmlArray()]
        public ArrayList Points
        {
            get {return _teachPoints;}
        }

        public DataTable TeachPointTable
        { get { return _dt; } }

        // Methods -------------------------------------------------------------

        public TeachPoint CreateTeachPoint(int pointID, string name, int historySize, string[] dimensionNames, double[] defaultValues)
        {
            TeachPoint teachPoint = GetPoint(pointID);
            Debug.Assert(teachPoint == null);
            teachPoint = new TeachPoint(pointID, name, historySize, dimensionNames, defaultValues);
            _teachPoints.Add(teachPoint);
            DBInsertPoint(teachPoint);

            teachPoint.PointChanged += new TeachPoint.PointChangedEventHandler(this.OnPointChange);

            if(PointStoreChanged != null) 
                PointStoreChanged();

            return teachPoint;
        }

        public TeachPoint CreateTeachPoint(string name, int historySize, string[] dimensionNames, double[] defaultValues)
        {
            TeachPoint teachPoint = GetPoint(name);
            Debug.Assert(teachPoint == null);
            teachPoint = new TeachPoint(name, historySize, dimensionNames, defaultValues);
            _teachPoints.Add(teachPoint);
            DBInsertPoint(teachPoint);

            teachPoint.PointChanged += new TeachPoint.PointChangedEventHandler(this.OnPointChange);

            if(PointStoreChanged != null) 
                PointStoreChanged();

            return teachPoint;
        }

        public TeachPoint GetPoint(int pointID)
        {
            TeachPoint tp = null;
            bool found = false;
            for (int tpidx = 0; tpidx <_teachPoints.Count; tpidx++)
            {
                tp = (TeachPoint)_teachPoints[tpidx];
                if (tp.ID == pointID)
                {
                    found = true;
                    break;
                }
            }
            if (found)
                return tp;
            else
                return null;
        }

        public TeachPoint GetPoint(string pointName)
        {
            TeachPoint tp = null;
            bool found = false;
            for (int tpidx = 0; tpidx <_teachPoints.Count; tpidx++)
            {
                tp = (TeachPoint)_teachPoints[tpidx];
                if (tp.Name == pointName)
                {
                    found = true;
                    break;
                }
            }
            if (found)
                return tp;
            else
                return null;
        }    
  
        public void OnPointChange()
        {
            DBUpdate();
            if (PointStoreChanged != null)
                PointStoreChanged();
            Save();
        }
        
        // Serializes the class to the config file
        // if any of the settings have changed.
        public void Save()
        {
            StreamWriter myWriter = null;
            XmlSerializer mySerializer = null;
            try
            {
                myWriter = new StreamWriter(_fileName, false);

                // Create a Type array.
                Type [] extraTypes= new Type[3];
                extraTypes[0] = typeof(NPoint);
                extraTypes[1] = typeof(TeachPoint);
                extraTypes[2] = typeof(TeachPointHistory);

                // Create the XmlSerializer instance.
                mySerializer = new XmlSerializer(typeof(TeachPointStore), extraTypes);
                mySerializer.Serialize(myWriter,this);
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.InnerException);
				
            }
            finally
            {
                // If the FileStream is open, close it.
                if(myWriter != null)
                {
                    myWriter.Close();
                }
            }
        }

        // Deserializes the class from the config file.
        public bool Load()
        {
            bool loaded = false;

            XmlSerializer mySerializer = null;
            FileStream myFileStream = null;

            try
            {
                // Create a Type array.
                Type [] extraTypes= new Type[3];
                extraTypes[0] = typeof(NPoint);
                extraTypes[1] = typeof(TeachPoint);
                extraTypes[2] = typeof(TeachPointHistory);

                // Create the XmlSerializer instance.
                mySerializer = new XmlSerializer(typeof(TeachPointStore), extraTypes);
                FileInfo fi = new FileInfo(_fileName);
                
                // If the config file exists, open it.
                if(fi.Exists)
                {
                    // Reading a file requires a FileStream.
                    myFileStream = fi.OpenRead();

                    // Create a new instance of the PersistentXML by deserializing the config file.
                    TeachPointStore pointStoreCopy = (TeachPointStore) mySerializer.Deserialize(myFileStream);

                    foreach (TeachPoint tpoint in pointStoreCopy.Points)
                    {
                        TeachPoint localTeachPoint = GetPoint(tpoint.Name);
                        if (localTeachPoint != null)
                        {
                            localTeachPoint.ShallowClone(tpoint);
                        }
                    }

                    DBUpdate();
                    if (PointStoreChanged != null)
                        PointStoreChanged();
                    loaded = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + ex.Message);                
            }
            finally
            {
                // If the FileStream is open, close it.
                if(myFileStream != null)
                {
                    myFileStream.Close();
                }
            }

            return loaded;
        }

        // Internal methods ----------------------------------------------------       
        
        private void DBUpdate()
        {
            foreach(TeachPoint tpoint in _teachPoints)
            {
                DBUpdatePoint(tpoint);
            }
        }

        private void DBInsertPoint(TeachPoint tpoint)
        {
            // insert data
            DataRow dr = _dt.NewRow();
            dr["Name"]     = tpoint.Name;
            //dr["ID"]       = tpoint.ID;
            //dr["Category"] = "";
            //dr["GUID"]     = tpoint.SavedBy;
            //dr["Time"]     = DateTime.Now;

            // check/add column(s)
            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                double dimValue = tpoint[dim];

                int colIdx;
                for (colIdx = 0; colIdx < _dt.Columns.Count; colIdx++)
                {
                    string sColName = _dt.Columns[colIdx].Caption;
                    if (sColName == dimName)
                        break;
                }
                if (colIdx == _dt.Columns.Count)
                {
                    // not found, add it in
                    DataColumn axisPositionColumn = new DataColumn(); 
                    axisPositionColumn.DataType = System.Type.GetType("System.Double"); 
                    axisPositionColumn.Caption = dimName; 
                    axisPositionColumn.ColumnName = dimName; 
                    axisPositionColumn.AllowDBNull = true; 
                    _dt.Columns.Add(axisPositionColumn);
                }

                dr[dimName] = dimValue.ToString("F3");
            }

            _dt.Rows.Add(dr);
        }

        private void DBUpdatePoint(TeachPoint tpoint)
        {
            // find data row
            DataRow dr = _dt.Rows.Find(tpoint.Name);
            if (dr == null) return;

            //dr["ID"]       = tpoint.ID;
            //dr["Category"] = "";
            //dr["GUID"]     = tpoint.SavedBy;
            //dr["Time"]     = DateTime.Now;

            for(int dim = 0; dim < tpoint.Dimension; dim++)
            {
                string dimName  = tpoint.DimensionNames[dim];
                for (int colIdx = 0; colIdx < _dt.Columns.Count; colIdx++)
                {
                    string sColName = _dt.Columns[colIdx].Caption;
                    if (sColName == dimName)
                        dr[colIdx] = tpoint[dim].ToString("F3");
                }
            }
        }
    }
}
