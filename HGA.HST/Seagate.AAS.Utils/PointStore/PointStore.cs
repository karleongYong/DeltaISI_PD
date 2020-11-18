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


namespace Seagate.AAS.Utils.PointStore
{
	/// <summary>
	/// Summary description for PointStore.
	/// </summary>
	[Serializable()]
    public class PointStore
	{

        // Nested declarations -------------------------------------------------
        

        // Member variables ----------------------------------------------------
        private Hashtable points = new Hashtable();
        private string    fileName;

        public delegate void UpdateHandler();
        public event UpdateHandler OnUpdate;
        
        // Constructors & Finalizers -------------------------------------------
        public PointStore()
        {
            fileName = "PointStore.xml";
        }        

        // Properties ----------------------------------------------------------
        public string FileName
        {
            get 
            { return fileName; }
            set 
            { this.fileName = value; }
        }
        public Hashtable Points
        {
            get {return points;}
        }

        // Methods -------------------------------------------------------------
        public void CreatePoint(string pointName, int queueSize, params string[] dimensionNames)
        {
            int dimension = dimensionNames.Length;
            PointStorage pointStorage = new PointStorage(pointName, dimension, queueSize, dimensionNames);
            points.Add(pointName, pointStorage);

            pointStorage.OnAddPoint += new Seagate.AAS.Utils.PointStore.PointStorage.OnAddPointHandler(PublishUpdateEvent);
    
        }

        public void UpdatePointValue(string pointName, params double[] valueList)
        {
            PointStorage point = GetPoint(pointName);
            if (point != null) 
            {
                point.AddRobotPoint(valueList);
            }
        }

        public PointStorage GetPoint(string pointName)
        {
            if (points.Contains(pointName))
            {
                return (PointStorage) points[pointName];
            }
            else
                return null;
        }

        public PointStorage GetPoint(string pointName, string[] dimensionNames, double[] defaultValues)
        {
            if (!points.Contains(pointName))
            {
                CreatePoint(pointName,10,dimensionNames);
                GetPoint(pointName).AddRobotPoint(defaultValues);
                Save();
            }
            else 
            {
                if (((PointStorage)points[pointName]).Dimension != dimensionNames.Length)
                {
                    PointStorage oldPoint = (PointStorage) points[pointName];

                    // remove old point from list
                    points.Remove(pointName);

                    // create new point
                    CreatePoint(pointName, 10, dimensionNames);
                    PointStorage newPoint = GetPoint(pointName);

                    // copy old values to new point
                    for (int j = oldPoint.History.Count - 1; j >= 0; j-- )
                    {
                        RobotPoint p = (RobotPoint) oldPoint.History[j];

                        double[] val = new double[newPoint.Dimension];
                        for (int i = 0; i < val.Length; i++)
                        {
                            if (i < p.Coordinate.Length)
                                val[i] = p.Coordinate[i];
                            else
                                val[i] = defaultValues[i];
                        }
                        newPoint.AddRobotPoint(val);

                    }
                    Save();

                }
            }
            return (PointStorage) points[pointName];
        }

		// added to make creating new points after deployment easier -- HG
		public RobotPoint GetRobotPoint(string pointName, string[] dimensionNames, double[] defaultValues)
		{
			if(!points.Contains(pointName))
			{
				CreatePoint(pointName,10,dimensionNames);
				GetPoint(pointName).AddRobotPoint(defaultValues);
				Save();
			}
            return GetPoint(pointName).CurrentPoint;
        }

		// added to make getting point values less verbose for classes that just want access to the current point value (diagnostic screens, et.c.) -- HG
		public double GetCurrentPointValue(string pointName, string coordName)
		{
			PointStorage point = GetPoint(pointName);
			// find index of coordName
			for(int i=0;i<point.Dimension;i++)
			{
				if(point.DimensionName[i] == coordName)
					return point.CurrentPoint[i];
			}
			//string msg = string.Format("{0} is not a valid dimension in for Point {1}",coordName,pointName);
			//throw new ParselException(msg);
			return 0.0;
		}

        // Serializes the class to the config file
        // if any of the settings have changed.
        public void Save()
        {

            ArrayList pointsList = new ArrayList();
            foreach (PointStorage ps in points.Values)
            {
                pointsList.Add(ps);            
            }

            StreamWriter myWriter = null;
            XmlSerializer mySerializer = null;
            try
            {
                // Create an XmlSerializer for the 
                // PersistentXML type.
                Type [] additionalTypes = new Type[2];
                additionalTypes[0] = typeof(PointStorage);
                additionalTypes[1] = typeof(RobotPoint);
                mySerializer = new XmlSerializer(typeof(ArrayList), additionalTypes);
                myWriter = new StreamWriter(fileName, false);
                // Serialize this instance of the PersistentXML 
                // class to the config file.
                mySerializer.Serialize(myWriter, pointsList);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to serialize. Reason: " + ex.Message);                
            }
            finally
            {
                // If the FileStream is open, close it.
                if(myWriter != null)
                {
                    myWriter.Close();
                }
            }


//            FileStream fs = new FileStream(fileName, FileMode.Create);
//
//            // Construct a BinaryFormatter and use it to serialize the data to the stream.
//            BinaryFormatter formatter = new BinaryFormatter();
//            try 
//            {
//                formatter.Serialize(fs, points);
//            }
//            catch (SerializationException e) 
//            {
//                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
//                throw;
//            }
//            finally 
//            {
//                fs.Close();
//            }
            //PublishUpdateEvent();  // no changes occurred

        }

		public bool Load(string filename)
		{
			this.FileName = filename;
			return Load();
		}
        // Deserializes the class from the config file.
        public bool Load()
        {
            bool loaded = false;

            XmlSerializer mySerializer = null;
            FileStream myFileStream = null;

            try
            {
                // Create an XmlSerializer for the PersistentXML type.
                Type [] additionalTypes = new Type[2];
                additionalTypes[0] = typeof(PointStorage);
                additionalTypes[1] = typeof(RobotPoint);

                mySerializer = new XmlSerializer(typeof(ArrayList), additionalTypes);
                FileInfo fi = new FileInfo(fileName);
                
                // If the config file exists, open it.
                if(fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    // Create a new instance of the PersistentXML by
                    // deserializing the config file.
                    ArrayList list = (ArrayList) mySerializer.Deserialize(myFileStream);
                    // copy arrayList to hashtable
                    points.Clear();
                    foreach (PointStorage ps in list)
                    {
                        ps.Recalculate();
                        ps.OnAddPoint += new Seagate.AAS.Utils.PointStore.PointStorage.OnAddPointHandler(PublishUpdateEvent);

                        points.Add(ps.Name, ps);

                    }
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

//            FileStream fs = null;
//            // Declare the hashtable reference.
//            // Open the file containing the data that you want to deserialize.
//            try 
//            {
//                fs = new FileStream(fileName, FileMode.Open);
//
//                BinaryFormatter formatter = new BinaryFormatter();
//
//                // Deserialize the object from the file and 
//                // assign the reference to the local variable.
//                points = (Hashtable) formatter.Deserialize(fs);
//                loaded = true;
//            }
//            catch (SerializationException e) 
//            {
//                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
//            }
//            finally 
//            {
//                if (fs != null)
//                    fs.Close();
//            }

            if (loaded)
                PublishUpdateEvent();

            return loaded;
        }


        // Internal methods ----------------------------------------------------       
        private void PublishUpdateEvent()
        {
            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }

	}
}
