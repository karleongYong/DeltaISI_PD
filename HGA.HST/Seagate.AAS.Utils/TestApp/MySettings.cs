using System;
using System.Collections;
using System.Drawing;


namespace Seagate.AAS.Utils
{
    public class Config
    {
        public string name;
        public long    id;
        public override string ToString()
        {
            return name + "," + id.ToString();
        }

    }

    /// <summary>
	/// Summary description for MySettings.
	/// ad XmlInclude for each type to be serialized in addition to derived PersistentXML
	/// </summary>
	[System.Xml.Serialization.XmlInclude(typeof(Config))]  
	public class MySettings : Seagate.AAS.Utils.PersistentXML
	{
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        private double timerValue;
        private Point formLocation;
        private DateTime timeStamp;
        private ArrayList arrayTest = new ArrayList();

        // Constructors & Finalizers -------------------------------------------
        public MySettings()
        {
            //
            // TODO: Add constructor logic here
            //
            FileName = "MySettings.config";
        }

        // Properties ----------------------------------------------------------
        public double TimerValue 
        { 
            get {return timerValue;} 
            set {timerValue = value;}
        }

        public Point FormLocation
        {
            get {return formLocation;} 
            set {formLocation = value;}
        }

        public DateTime TimeStamp
        {
            get {return timeStamp;} 
            set {timeStamp = value; }
        }

        public ArrayList ArrayTest
        {
            get {return arrayTest;} 
        }

        // Methods -------------------------------------------------------------
        public void AddMyConfig(string name, long id)
        {
            Config cfg = new Config();
            cfg.name = name;
            cfg.id = id;
            arrayTest.Add(cfg);
//            arrayTest.Add(id);
        }

        public void ClearMyConfig()
        {
            arrayTest.Clear();
        }
        // Internal methods ----------------------------------------------------
        //protected override void UpdateMemberVariables(Seagate.AAS.Utils.PersistentXML xml)
        //{
        //    this.timerValue = ((MySettings)xml).timerValue;
        //    this.formLocation = ((MySettings)xml).formLocation;
        //    this.timeStamp = ((MySettings)xml).timeStamp;
        //    this.arrayTest = null;
        //    this.arrayTest = ((MySettings)xml).arrayTest;
        //}

        public override string ToString()
        {
            string output;
            output  = "MySettings:" + Environment.NewLine ;
            output += "Timer =  " + this.TimerValue.ToString() + Environment.NewLine;
            output += "Form Location = " + this.FormLocation.ToString() + Environment.NewLine;
            output += "Time Stamp: " + this.TimeStamp.ToString() + Environment.NewLine;
            output += "Array: " + Environment.NewLine;
            for (int i=0; i<this.arrayTest.Count; i++)
            {
                output += string.Format("[{0}] = {1}", i, arrayTest[i].ToString()) + Environment.NewLine;
            }
            return output;
        }


        

	}
}
