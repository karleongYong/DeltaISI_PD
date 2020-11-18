using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.HGA.HST.Machine;
using System.Xml;

namespace Seagate.AAS.HGA.HST.Recipe
{
    public class TeachPointRecipe
    {
        string _name = "";
        string _fullPath = "";
        string _version = "";
        string _description = "";

        //Input_EE_Z
        double _inputEESafeHeight = 0;
        double _inputEEPickHeight = 0;
        double _inputEEPlaceHeight_UpTab = 0;
        double _inputEEPlaceHeight_DownTab = 0;
        double _inputEEDycemHeight = 0;

        //Test_Probe_Z
        double _testProbeSafeHeight = 0;
        double _testProbeTestHeight_UpTab = 0;
        double _testProbeTestHeight_DownTab = 0;

        //Output_EE_Z
        double _outputEESafeHeight = 0;
        double _outputEEPickHeight_UpTab = 0;
        double _outputEEPickHeight_DownTab = 0;
        double _outputEEPlaceHeight = 0;
        double _outputEEDycemHeight = 0;

        //Precisor Station
        double _precisorSafePositionX = 0;
        double _precisorSafePositionY = 0;
        double _precisorSafePositionTheta = 0;

        double _precisorInputStationPositionX_UpTab = 0;
        double _precisorInputStationPositionY_UpTab = 0;
        double _precisorInputStationPositionTheta_UpTab = 0;

        double _precisorInputStationPositionX_DownTab = 0;
        double _precisorInputStationPositionY_DownTab = 0;
        double _precisorInputStationPositionTheta_DownTab = 0;

        double _precisorTestStationPositionX_UpTab = 0;
        double _precisorTestStationPositionY_UpTab = 0;
        double _precisorTestStationPositionTheta_UpTab = 0;

        double _precisorTestStationPositionX_DownTab = 0;
        double _precisorTestStationPositionY_DownTab = 0;
        double _precisorTestStationPositionTheta_DownTab = 0;

        double _precisorOutputStationPositionX_UpTab = 0;
        double _precisorOutputStationPositionY_UpTab = 0;
        double _precisorOutputStationPositionTheta_UpTab = 0;

        double _precisorOutputStationPositionX_DownTab = 0;
        double _precisorOutputStationPositionY_DownTab = 0;
        double _precisorOutputStationPositionTheta_DownTab = 0;

        // Properties ----------------------------------------------------------
        public string Name { get { return _name; } set { _name = value; } }
        public string FullPath { get { return _fullPath; } set { _fullPath = value; } }
        public string Version { get { return _version; } set { _version = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string Ext = ".TeachPointRcp";

        //Input_EE_Z
        public double InputEESafeHeight { get { return _inputEESafeHeight; } set { _inputEESafeHeight = value; } }
        public double InputEEPickHeight { get { return _inputEEPickHeight; } set { _inputEEPickHeight = value; } }
        public double InputEEPlaceHeight_UpTab { get { return _inputEEPlaceHeight_UpTab; } set { _inputEEPlaceHeight_UpTab = value; } }
        public double InputEEPlaceHeight_DownTab { get { return _inputEEPlaceHeight_DownTab; } set { _inputEEPlaceHeight_DownTab = value; } }
        public double InputEEDycemHeight { get { return _inputEEDycemHeight; } set { _inputEEDycemHeight = value; } }

        //Test_Probe_Z
        public double TestProbeSafeHeight { get { return _testProbeSafeHeight; } set { _testProbeSafeHeight = value; } }
        public double TestProbeTestHeight_UpTab { get { return _testProbeTestHeight_UpTab; } set { _testProbeTestHeight_UpTab = value; } }
        public double TestProbeTestHeight_DownTab { get { return _testProbeTestHeight_DownTab; } set { _testProbeTestHeight_DownTab = value; } }        

        //Output_EE_Z
        public double OutputEESafeHeight { get { return _outputEESafeHeight; } set { _outputEESafeHeight = value; } }
        public double OutputEEPickHeight_UpTab { get { return _outputEEPickHeight_UpTab; } set { _outputEEPickHeight_UpTab = value; } }
        public double OutputEEPickHeight_DownTab { get { return _outputEEPickHeight_DownTab; } set { _outputEEPickHeight_DownTab = value; } }
        public double OutputEEPlaceHeight { get { return _outputEEPlaceHeight; } set { _outputEEPlaceHeight = value; } }
        public double OutputEEDycemHeight { get { return _outputEEDycemHeight; } set { _outputEEDycemHeight = value; } }

        //Precisor Station
        public double PrecisorSafePositionX { get { return _precisorSafePositionX; } set { _precisorSafePositionX = value; } }
        public double PrecisorSafePositionY { get { return _precisorSafePositionY; } set { _precisorSafePositionY = value; } }
        public double PrecisorSafePositionTheta { get { return _precisorSafePositionTheta; } set { _precisorSafePositionTheta = value; } }

        public double PrecisorInputStationPositionX_UpTab { get { return _precisorInputStationPositionX_UpTab; } set { _precisorInputStationPositionX_UpTab = value; } }
        public double PrecisorInputStationPositionY_UpTab { get { return _precisorInputStationPositionY_UpTab; } set { _precisorInputStationPositionY_UpTab = value; } }
        public double PrecisorInputStationPositionTheta_UpTab { get { return _precisorInputStationPositionTheta_UpTab; } set { _precisorInputStationPositionTheta_UpTab = value; } }

        public double PrecisorInputStationPositionX_DownTab { get { return _precisorInputStationPositionX_DownTab; } set { _precisorInputStationPositionX_DownTab = value; } }
        public double PrecisorInputStationPositionY_DownTab { get { return _precisorInputStationPositionY_DownTab; } set { _precisorInputStationPositionY_DownTab = value; } }
        public double PrecisorInputStationPositionTheta_DownTab { get { return _precisorInputStationPositionTheta_DownTab; } set { _precisorInputStationPositionTheta_DownTab = value; } }

        public double PrecisorTestStationPositionX_UpTab { get { return _precisorTestStationPositionX_UpTab; } set { _precisorTestStationPositionX_UpTab = value; } }
        public double PrecisorTestStationPositionY_UpTab { get { return _precisorTestStationPositionY_UpTab; } set { _precisorTestStationPositionY_UpTab = value; } }
        public double PrecisorTestStationPositionTheta_UpTab { get { return _precisorTestStationPositionTheta_UpTab; } set { _precisorTestStationPositionTheta_UpTab = value; } }

        public double PrecisorTestStationPositionX_DownTab { get { return _precisorTestStationPositionX_DownTab; } set { _precisorTestStationPositionX_DownTab = value; } }
        public double PrecisorTestStationPositionY_DownTab { get { return _precisorTestStationPositionY_DownTab; } set { _precisorTestStationPositionY_DownTab = value; } }
        public double PrecisorTestStationPositionTheta_DownTab { get { return _precisorTestStationPositionTheta_DownTab; } set { _precisorTestStationPositionTheta_DownTab = value; } }

        public double PrecisorOutputStationPositionX_UpTab { get { return _precisorOutputStationPositionX_UpTab; } set { _precisorOutputStationPositionX_UpTab = value; } }
        public double PrecisorOutputStationPositionY_UpTab { get { return _precisorOutputStationPositionY_UpTab; } set { _precisorOutputStationPositionY_UpTab = value; } }
        public double PrecisorOutputStationPositionTheta_UpTab { get { return _precisorOutputStationPositionTheta_UpTab; } set { _precisorOutputStationPositionTheta_UpTab = value; } }

        public double PrecisorOutputStationPositionX_DownTab { get { return _precisorOutputStationPositionX_DownTab; } set { _precisorOutputStationPositionX_DownTab = value; } }
        public double PrecisorOutputStationPositionY_DownTab { get { return _precisorOutputStationPositionY_DownTab; } set { _precisorOutputStationPositionY_DownTab = value; } }
        public double PrecisorOutputStationPositionTheta_DownTab { get { return _precisorOutputStationPositionTheta_DownTab; } set { _precisorOutputStationPositionTheta_DownTab = value; } }

 
         // Constructors & Finalizers -------------------------------------------
        public TeachPointRecipe()
        {
        }

        // Methods -------------------------------------------------------------

        public void Load(XmlDocument doc)
        {
            this.Version = GetValue("Version", doc).ToString();
            this.Description = GetValue("Description", doc).ToString();
            
            string section = "Input_EE_Z";
            this.InputEESafeHeight = Convert.ToDouble(GetValue(section, "SafeHeight", doc));
            this.InputEEPickHeight = Convert.ToDouble(GetValue(section, "PickHeight", doc));
            double inputEEPlaceHeight = Convert.ToDouble(GetValue(section, "PlaceHeight", doc)); // old setting
            double inputEEPlaceHeight_UpTab = Convert.ToDouble(GetValue(section, "PlaceHeight_UpTab", doc)); // new setting
            double inputEEPlaceHeight_DownTab = Convert.ToDouble(GetValue(section, "PlaceHeight_DownTab", doc)); // new setting
            this.InputEEPlaceHeight_UpTab = (inputEEPlaceHeight_UpTab > 0) ? inputEEPlaceHeight_UpTab : inputEEPlaceHeight;
            this.InputEEPlaceHeight_DownTab = (inputEEPlaceHeight_DownTab > 0) ? inputEEPlaceHeight_DownTab : inputEEPlaceHeight;
            this.InputEEDycemHeight = Convert.ToDouble(GetValue(section, "DycemHeight", doc));
            RemoveValue(section, "PlaceHeight", doc); //Lai: remove old setting from setting file.

            section = "Test_Probe_Z";
            this.TestProbeSafeHeight = Convert.ToDouble(GetValue(section, "SafeHeight", doc));
            double testProbeTestHeight = Convert.ToDouble(GetValue(section, "TestHeight", doc)); // old setting
            double testProbeTestHeight_UpTab = Convert.ToDouble(GetValue(section, "TestHeight_UpTab", doc)); // new setting
            double testProbeTestHeight_DownTab = Convert.ToDouble(GetValue(section, "TestHeight_DownTab", doc)); // new setting
            this.TestProbeTestHeight_UpTab = (testProbeTestHeight_UpTab > 0) ? testProbeTestHeight_UpTab : testProbeTestHeight;
            this.TestProbeTestHeight_DownTab = (testProbeTestHeight_DownTab > 0) ? testProbeTestHeight_DownTab : testProbeTestHeight;
            RemoveValue(section, "TestHeight", doc); //Lai: remove old setting from setting file.

            section = "Output_EE_Z";
            this.OutputEESafeHeight = Convert.ToDouble(GetValue(section, "SafeHeight", doc));
            double outputEEPickHeight = Convert.ToDouble(GetValue(section, "PickHeight", doc)); // old setting
            double outputEEPickHeight_UpTab = Convert.ToDouble(GetValue(section, "PickHeight_UpTab", doc)); // new setting
            double outputEEPickHeight_DownTab = Convert.ToDouble(GetValue(section, "PickHeight_DownTab", doc)); // new setting
            this.OutputEEPickHeight_UpTab = (outputEEPickHeight_UpTab > 0) ? outputEEPickHeight_UpTab : outputEEPickHeight;
            this.OutputEEPickHeight_DownTab = (outputEEPickHeight_DownTab > 0) ? outputEEPickHeight_DownTab : outputEEPickHeight;
            this.OutputEEPlaceHeight = Convert.ToDouble(GetValue(section, "PlaceHeight", doc));
            this.OutputEEDycemHeight = Convert.ToDouble(GetValue(section, "DycemHeight", doc));
            RemoveValue(section, "PickHeight", doc); //Lai: remove old setting from setting file.

            section = "Precisor_Station";
            this.PrecisorSafePositionX = Convert.ToDouble(GetValue(section, "SafePositionX", doc));
            this.PrecisorSafePositionY = Convert.ToDouble(GetValue(section, "SafePositionY", doc));
            this.PrecisorSafePositionTheta = Convert.ToDouble(GetValue(section, "SafePositionTheta", doc));

            this.PrecisorInputStationPositionX_UpTab = Convert.ToDouble(GetValue(section, "InputStationPositionX_UpTab", doc));
            this.PrecisorInputStationPositionY_UpTab = Convert.ToDouble(GetValue(section, "InputStationPositionY_UpTab", doc));
            this.PrecisorInputStationPositionTheta_UpTab = Convert.ToDouble(GetValue(section, "InputStationPositionTheta_UpTab", doc));

            this.PrecisorInputStationPositionX_DownTab = Convert.ToDouble(GetValue(section, "InputStationPositionX_DownTab", doc));
            this.PrecisorInputStationPositionY_DownTab = Convert.ToDouble(GetValue(section, "InputStationPositionY_DownTab", doc));
            this.PrecisorInputStationPositionTheta_DownTab = Convert.ToDouble(GetValue(section, "InputStationPositionTheta_DownTab", doc));

            this.PrecisorTestStationPositionX_UpTab = Convert.ToDouble(GetValue(section, "TestStationPositionX_UpTab", doc));
            this.PrecisorTestStationPositionY_UpTab = Convert.ToDouble(GetValue(section, "TestStationPositionY_UpTab", doc));
            this.PrecisorTestStationPositionTheta_UpTab = Convert.ToDouble(GetValue(section, "TestStationPositionTheta_UpTab", doc));

            this.PrecisorTestStationPositionX_DownTab = Convert.ToDouble(GetValue(section, "TestStationPositionX_DownTab", doc));
            this.PrecisorTestStationPositionY_DownTab = Convert.ToDouble(GetValue(section, "TestStationPositionY_DownTab", doc));
            this.PrecisorTestStationPositionTheta_DownTab = Convert.ToDouble(GetValue(section, "TestStationPositionTheta_DownTab", doc));

            this.PrecisorOutputStationPositionX_UpTab = Convert.ToDouble(GetValue(section, "OutputStationPositionX_UpTab", doc));
            this.PrecisorOutputStationPositionY_UpTab = Convert.ToDouble(GetValue(section, "OutputStationPositionY_UpTab", doc));
            this.PrecisorOutputStationPositionTheta_UpTab = Convert.ToDouble(GetValue(section, "OutputStationPositionTheta_UpTab", doc));

            this.PrecisorOutputStationPositionX_DownTab = Convert.ToDouble(GetValue(section, "OutputStationPositionX_DownTab", doc));
            this.PrecisorOutputStationPositionY_DownTab = Convert.ToDouble(GetValue(section, "OutputStationPositionY_DownTab", doc));
            this.PrecisorOutputStationPositionTheta_DownTab = Convert.ToDouble(GetValue(section, "OutputStationPositionTheta_DownTab", doc));
        }

        private object GetValue(string section, string key, XmlDocument doc)
        {
            try
            {
                XmlElement root = doc.DocumentElement;
                XmlNode secNode = doc.SelectSingleNode("TeachPointRecipe" + "/" + section);
                XmlNode valNode = secNode.SelectSingleNode(key);
                return valNode.InnerText;
            }
            catch
            {
                return null;
            }
        }

        private object GetValue(string key, XmlDocument doc)
        {
            try
            {
                XmlElement root = doc.DocumentElement;
                XmlNode secNode = doc.SelectSingleNode("TeachPointRecipe" + "/" + key);
                return secNode.InnerText;
            }
            catch
            {
                return null;
            }
        }

        private void RemoveValue(string section, string key, XmlDocument doc)
        {
            try
            {
                XmlElement root = doc.DocumentElement;
                XmlNode secNode = doc.SelectSingleNode("TeachPointRecipe" + "/" + section);
                XmlNode valNode = secNode.SelectSingleNode(key);
                secNode.RemoveChild(valNode);
            }
            catch
            {
            }
        }
    }
}
