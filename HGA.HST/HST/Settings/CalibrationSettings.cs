using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.IO.Serial;
using System.IO.Ports;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class CalibrationSettings
    {                
        public SettingsEditor _settingsEditor;
        protected SettingsDocument _settingsDoc;                 
       
        public EventHandler OnSettingsChanged;
           

        #region Singleton

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<CalibrationSettings> _instance
             = new Lazy<CalibrationSettings>(() => new CalibrationSettings());

        // accessor for instance
        public static CalibrationSettings Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        
        public CalibrationSettings()
        {                       
            RFID = new RFIDPortSettings();
            
            _rfidPortSettings = new SerialPortSettings();            
            _safetyControllerPortSettings = new SerialPortSettings();

            Vision = new VisionSettings();

            MeasurementTest = new MeasurementTestSettings();

            SafetyController = new SafetyControllerPortSettings();
            //HgaProducts = new HGAProductSetting();
            //HgaProducts = new HGAProductSetting();
            //HgaProduct = new HGAProductInfo();
            Load();            
        }        

        #endregion       

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("RFIDs")]
        public RFIDPortSettings RFID
        {
            get;
            private set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Vision Settings")]
        public VisionSettings Vision
        {
            get;
            private set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Test Electronics")]
        public MeasurementTestSettings MeasurementTest
        {
            get;
            private set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Safety Controller")]
        public SafetyControllerPortSettings SafetyController
        {
            get;
            private set;
        }

        private SettingsXml _xml;                
        
        private int _rfidWriteCounterLimit;
        public int RfidWriteCounterLimit
        {
            get { return _rfidWriteCounterLimit; }
            set { _rfidWriteCounterLimit = value; }
        }

        private SerialPortSettings _rfidPortSettings;
        public SerialPortSettings getRfidPortSettings()
        {
            return _rfidPortSettings;
        }                

        private SerialPortSettings _safetyControllerPortSettings;
        public SerialPortSettings getSafetyControllerPortSettings()
        {
            return _safetyControllerPortSettings;
        }                       

        public void Load()
        {
            if (HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath == null)
            {
                return;
            }

            if (!File.Exists(HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath))
            {
                File.Create(HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath).Dispose();
                return;
            }

            _xml = new SettingsXml(HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath);
            _xml.OpenSection("Config");

            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (!pi.CanWrite) // Avoid putting data to read only property such as IsAutoRecipe
                    continue;

                // Loop through all properties and save them into config.
                if (pi.PropertyType == typeof(String))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (String)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType == typeof(Int32))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (Int32)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType == typeof(Double))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (Double)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType == typeof(Boolean))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (Boolean)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType.IsEnum == true)
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (int)pi.GetValue(this, null)), null);
                }
            }            
            _xml.CloseSection();
                       
            _xml.OpenSection("RFID");
            RFID.RFIDCOMPort = (COMPortList)Enum.Parse(typeof(COMPortList), _xml.Read("RFIDCOMPort", COMPortList.Unknown.ToString()));
            RFID.RFIDBaudRate = int.Parse(_xml.Read("RFIDBaudRate", "0"));
            RFID.RFIDParity = (Parity)Enum.Parse(typeof(Parity), _xml.Read("RFIDParity", Parity.None.ToString()));
            RFID.RFIDDataBits = int.Parse(_xml.Read("RFIDDataBits", "0"));
            RFID.RFIDStopBits = (StopBits)Enum.Parse(typeof(StopBits), _xml.Read("RFIDStopBits", StopBits.One.ToString()));           

            RFID.RfidWriteCounterLimit = int.Parse(_xml.Read("RfidWriteCounterLimit", "0"));
            _xml.CloseSection();

            _xml.OpenSection("MeasurementTest");
            MeasurementTest.COMPort = (COMPortList)Enum.Parse(typeof(COMPortList), _xml.Read("COMPort", COMPortList.Unknown.ToString()));
            MeasurementTest.BaudRate = int.Parse(_xml.Read("BaudRate", "19200"));
            MeasurementTest.Parity = (Parity)Enum.Parse(typeof(Parity), _xml.Read("Parity", Parity.None.ToString()));
            MeasurementTest.DataBits = int.Parse(_xml.Read("DataBits", "8"));
            MeasurementTest.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _xml.Read("StopBits", StopBits.One.ToString()));

            MeasurementTest.CurrentInstalledTestProbeType = _xml.Read("CurrentInstalledTestProbeType", CommonFunctions.UNKNOWN);
            MeasurementTest.TestProbeTypes.Clear();

            for (int i = 0; ; i++)
            {
                string CurrentTestProbe = "TestProbe" + i;
                TestProbeType TestProbeType = new TestProbeType(_xml.Read(CurrentTestProbe + "TestProbeName", CommonFunctions.UNKNOWN));
                if (String.Compare(TestProbeType.Name, CommonFunctions.UNKNOWN, true) != 0)
                {
                    MeasurementTest.TestProbeTypes.Add(TestProbeType);
                }
                else
                {
                    break;
                }
            } 
            _xml.CloseSection();                

            _xml.OpenSection("Camera");
            Vision.InputCamera.Recipe = _xml.Read("InputCamera_Recipe", CommonFunctions.UNKNOWN);
            Vision.InputCamera.ImagesOutputPath = _xml.Read("InputCamera_ImagesOutputPath", CommonFunctions.UNKNOWN);
            Vision.InputCamera.SaveAllImages = (bool)_xml.Read("InputCamera_SaveAllImages", Convert.ToBoolean(false));
            Vision.InputCamera.SaveImagesLessThanTenHGAs = (bool)_xml.Read("InputCamera_SaveImagesLessThanTenHGAs", Convert.ToBoolean(false));
            Vision.InputCamera.CameraSerialNumber = _xml.Read("InputCamera_SerialNumber", CommonFunctions.UNKNOWN);
            Vision.InputCamera.TotalDayToStoreImage = _xml.Read("Input_TotalDayToStoreImage", 7);
            if (Vision.InputCamera.TotalDayToStoreImage == 0)
                Vision.InputCamera.TotalDayToStoreImage = 7;

            Vision.OutputCamera.Recipe = _xml.Read("OutputCamera_Recipe", CommonFunctions.UNKNOWN);
            Vision.OutputCamera.ImagesOutputPath = _xml.Read("OutputCamera_ImagesOutputPath", CommonFunctions.UNKNOWN);
            Vision.OutputCamera.SaveAllImages = (bool)_xml.Read("OutputCamera_SaveAllImages", Convert.ToBoolean(false));
            Vision.OutputCamera.SaveImagesLessThanTenHGAs = (bool)_xml.Read("OutputCamera_SaveImagesLessThanTenHGAs", Convert.ToBoolean(false));
            Vision.OutputCamera.CameraSerialNumber = _xml.Read("OutputCamera_SerialNumber", CommonFunctions.UNKNOWN);
            Vision.OutputCamera.TotalDayToStoreImage = _xml.Read("Output_TotalDayToStoreImage", 7);

            if (Vision.OutputCamera.TotalDayToStoreImage == 0)
                Vision.OutputCamera.TotalDayToStoreImage = 7;
            _xml.CloseSection();

            _xml.OpenSection("SafetyController");
            SafetyController.COMPort = (COMPortList)Enum.Parse(typeof(COMPortList), _xml.Read("COMPort", COMPortList.Unknown.ToString()));
            SafetyController.BaudRate = int.Parse(_xml.Read("BaudRate", "19200"));
            SafetyController.Parity = (Parity)Enum.Parse(typeof(Parity), _xml.Read("Parity", Parity.None.ToString()));
            SafetyController.DataBits = int.Parse(_xml.Read("DataBits", "8"));
            SafetyController.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _xml.Read("StopBits", StopBits.One.ToString()));
            _xml.CloseSection();      

            _rfidPortSettings.PortName = "" + RFID.RFIDCOMPort;
            _rfidPortSettings.BaudRate = RFID.RFIDBaudRate;
            _rfidPortSettings.Parity = RFID.RFIDParity;
            _rfidPortSettings.DataBits = RFID.RFIDDataBits;
            _rfidPortSettings.StopBits = RFID.RFIDStopBits;
            
            _rfidWriteCounterLimit = RFID.RfidWriteCounterLimit;

            _safetyControllerPortSettings.PortName = "" + SafetyController.COMPort;
            _safetyControllerPortSettings.BaudRate = SafetyController.BaudRate;
            _safetyControllerPortSettings.Parity = SafetyController.Parity;
            _safetyControllerPortSettings.DataBits = SafetyController.DataBits;
            _safetyControllerPortSettings.StopBits = SafetyController.StopBits;


            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
        }

        public void Save()
        {            
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);

            File.Create(HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath).Dispose();

            _xml = new SettingsXml(HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath);
            _xml.OpenSection("Config");

            // Loop through all properties of this type and read from
            // config based on property's name.
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (!pi.CanWrite) // Avoid saving readonly property to config file such as IsAutoRecipe
                    continue;

                if (pi.PropertyType == typeof(String))
                {
                    _xml.Write(pi.Name, (String)pi.GetValue(this, null));
                }
                if (pi.PropertyType == typeof(Int32))
                {
                    _xml.Write(pi.Name, (Int32)pi.GetValue(this, null));
                }

                if (pi.PropertyType == typeof(Double))
                {
                    _xml.Write(pi.Name, (Double)pi.GetValue(this, null));
                }

                if (pi.PropertyType == typeof(Boolean))
                {
                    _xml.Write(pi.Name, (Boolean)pi.GetValue(this, null));
                }
                if (pi.PropertyType.IsEnum == true)
                {
                    _xml.Write(pi.Name, (int)pi.GetValue(this, null));
                }
            }
            
            _xml.OpenSection("RFID");
            _xml.Write("RFIDCOMPort", RFID.RFIDCOMPort.ToString());
            _xml.Write("RFIDBaudRate", RFID.RFIDBaudRate.ToString());
            _xml.Write("RFIDStopBits", RFID.RFIDStopBits.ToString());
            _xml.Write("RFIDParity", RFID.RFIDParity.ToString());
            _xml.Write("RFIDDataBits", RFID.RFIDDataBits.ToString());            
            _xml.CloseSection();

            _xml.OpenSection("MeasurementTest");
            _xml.Write("COMPort", MeasurementTest.COMPort.ToString());
            _xml.Write("BaudRate", MeasurementTest.BaudRate.ToString());
            _xml.Write("StopBits", MeasurementTest.StopBits.ToString());
            _xml.Write("Parity", MeasurementTest.Parity.ToString());
            _xml.Write("DataBits", MeasurementTest.DataBits.ToString());
            _xml.Write("CurrentInstalledTestProbeType", MeasurementTest.CurrentInstalledTestProbeType.ToString());

            int i = 0;
            foreach (TestProbeType TestProbeType in MeasurementTest.TestProbeTypes)
            {
                string CurrentTestProbe = "TestProbe" + (i++);

                _xml.Write(CurrentTestProbe + "TestProbeName", TestProbeType.Name);
            }
            _xml.CloseSection();

            _xml.OpenSection("Camera");
            _xml.Write("InputCamera_Recipe", Vision.InputCamera.Recipe.ToString());
            _xml.Write("InputCamera_ImagesOutputPath", Vision.InputCamera.ImagesOutputPath.ToString());
            _xml.Write("InputCamera_SaveAllImages", Vision.InputCamera.SaveAllImages.ToString());
            _xml.Write("InputCamera_SaveImagesLessThanTenHGAs", Vision.InputCamera.SaveImagesLessThanTenHGAs.ToString());
            _xml.Write("InputCamera_SerialNumber", Vision.InputCamera.CameraSerialNumber.ToString());
            _xml.Write("Input_TotalDayToStoreImage", Vision.InputCamera.TotalDayToStoreImage.ToString());

            _xml.Write("OutputCamera_Recipe", Vision.OutputCamera.Recipe.ToString());
            _xml.Write("OutputCamera_ImagesOutputPath", Vision.OutputCamera.ImagesOutputPath.ToString());
            _xml.Write("OutputCamera_SaveAllImages", Vision.OutputCamera.SaveAllImages.ToString());
            _xml.Write("OutputCamera_SaveImagesLessThanTenHGAs", Vision.OutputCamera.SaveImagesLessThanTenHGAs.ToString());
            _xml.Write("OutputCamera_SerialNumber", Vision.OutputCamera.CameraSerialNumber.ToString());
            _xml.Write("Output_TotalDayToStoreImage", Vision.OutputCamera.TotalDayToStoreImage.ToString());
            _xml.CloseSection();

            _xml.OpenSection("SafetyController");
            _xml.Write("COMPort", SafetyController.COMPort.ToString());
            _xml.Write("BaudRate", SafetyController.BaudRate.ToString());
            _xml.Write("StopBits", SafetyController.StopBits.ToString());
            _xml.Write("Parity", SafetyController.Parity.ToString());
            _xml.Write("DataBits", SafetyController.DataBits.ToString());
            _xml.CloseSection();
            
            _xml.CloseSection();     
            _xml.Save();

            HSTMachine.Instance.MainForm.getPanelCommand().DebugButtonsVisibility();
        }

        public void SaveSettingsToFile(object sender, EventArgs e)
        {
            bool foundMatchingTestProbeType = false;
            foreach (TestProbeType TestProbeType in MeasurementTest.TestProbeTypes)
            {
                if (TestProbeType.Name == MeasurementTest.CurrentInstalledTestProbeType)
                {
                    foundMatchingTestProbeType = true;
                    break;
                }
            }
            
            if (!foundMatchingTestProbeType)
            {
                Notify.PopUp("Invalid current installed test probe type", String.Format("The type of current installed test probe '{0}' is not available in the list of test probe type.", CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType) +
                    "\n\nPlease ensure the installed test probe is selected from the list of Test Probe types.", "", "OK");

                return;
            }

            Save();

        }        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Config]");
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(String) ||
                    pi.PropertyType == typeof(Boolean) ||
                    pi.PropertyType == typeof(Double) ||
                    pi.PropertyType == typeof(Int32))
                {
                    sb.AppendLine(pi.Name + "," + pi.GetValue(this, null));
                }
            }
            return sb.ToString();
        }        
    }

}
