using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using Seagate.AAS.Utils;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using DesktopTester.UI;
using DesktopTester.Data;
using DesktopTester.Data.IncomingTestProbeData;
using DesktopTester.Data.OutgoingTestProbeData;
using DesktopTester.Utils;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace DesktopTester.Utils
{
    public class CommonFunctions
    {
        public FunctionalTestsRecipe FunctionalTestsRecipe;
        public ConfigurationSetupRecipe ConfigurationSetupRecipe;
        public bool GetTemperatureContinuous = false;
        public bool GetADCVoltageContinuous = false;
        public bool BenchTestsContinuous = false;
        public bool BenchTestsMultiple = false;
        public bool DesktopTestsContinuous = false;
        public bool DesktopTestsMultiple = false;
        public int DelayInBetweenCommandBatch = 500;
        public string strProductID = "0";
        public string RecipeFileDirectory = "C:\\Seagate\\HGA.HST\\Recipes\\DesktopTester\\";
        public string MeasurementTestFileDirectory = "C:\\Seagate\\HGA.HST\\Data\\DesktopTester\\";
        public string COMPortSettingsFileDirectory = "C:\\Seagate\\HGA.HST\\Setup\\";
        public string _usersPath = "C:\\Seagate\\HGA.HST\\Setup\\Users.config";
        public string strManualCalibrationClickedButton = "";

        #region Singleton

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<CommonFunctions> _instance
             = new Lazy<CommonFunctions>(() => new CommonFunctions());

        // accessor for instance
        public static CommonFunctions Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public CommonFunctions()
        {
            OutgoingTestProbeDataAPIs = new Queue<TestProbeAPICommand>();   
        }        

        #endregion       

        public Queue<TestProbeAPICommand> OutgoingTestProbeDataAPIs
        {
            get;
            set;
        }


        public byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);

            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }
        
        public void LoadFunctionalTestsRecipe()
        {            
            string FunctionalTestsRecipeFilePath = string.Format("{0}FunctionalTestsRecipe.rcp", RecipeFileDirectory);
            if (!Directory.Exists(RecipeFileDirectory))
            {
                Directory.CreateDirectory(RecipeFileDirectory);
            }

            if (!File.Exists(FunctionalTestsRecipeFilePath))
            {
                File.Create(FunctionalTestsRecipeFilePath).Dispose();
                return;
            }
            else
            {
                Log.Info("Startup", "Retrieving recipe for functional tests from {0}.", FunctionalTestsRecipeFilePath);

                SettingsXml _xml = new SettingsXml(FunctionalTestsRecipeFilePath);
                
                _xml.OpenSection("ZeroOhm");
                // 0Ohm
                FunctionalTestsRecipe.Ch1WriterResistance0Ohm = int.Parse(_xml.Read("Ch1WriterResistance0Ohm", "0"));
                FunctionalTestsRecipe.Ch2TAResistance0Ohm = int.Parse(_xml.Read("Ch2TAResistance0Ohm", "0"));
                FunctionalTestsRecipe.Ch3WHResistance0Ohm = int.Parse(_xml.Read("Ch3WHResistance0Ohm", "0"));
                FunctionalTestsRecipe.Ch4RHResistance0Ohm = int.Parse(_xml.Read("Ch4RHResistance0Ohm", "0"));
                FunctionalTestsRecipe.Ch5R1Resistance0Ohm = int.Parse(_xml.Read("Ch5R1Resistance0Ohm", "0"));
                FunctionalTestsRecipe.Ch6R2Resistance0Ohm = int.Parse(_xml.Read("Ch6R2Resistance0Ohm", "0"));
                _xml.CloseSection();

                _xml.OpenSection("TenOhm");
                // 10Ohm
                FunctionalTestsRecipe.Ch1WriterResistance10Ohm = int.Parse(_xml.Read("Ch1WriterResistance10Ohm", "0"));
                FunctionalTestsRecipe.Ch2TAResistance10Ohm = int.Parse(_xml.Read("Ch2TAResistance10Ohm", "0"));
                FunctionalTestsRecipe.Ch3WHResistance10Ohm = int.Parse(_xml.Read("Ch3WHResistance10Ohm", "0"));
                FunctionalTestsRecipe.Ch4RHResistance10Ohm = int.Parse(_xml.Read("Ch4RHResistance10Ohm", "0"));
                FunctionalTestsRecipe.Ch5R1Resistance10Ohm = int.Parse(_xml.Read("Ch5R1Resistance10Ohm", "0"));
                FunctionalTestsRecipe.Ch6R2Resistance10Ohm = int.Parse(_xml.Read("Ch6R2Resistance10Ohm", "0"));
                _xml.CloseSection();

                _xml.OpenSection("OneHundredOhm");
                // 100Ohm
                FunctionalTestsRecipe.Ch1WriterResistance100Ohm = int.Parse(_xml.Read("Ch1WriterResistance100Ohm", "0"));
                FunctionalTestsRecipe.Ch2TAResistance100Ohm = int.Parse(_xml.Read("Ch2TAResistance100Ohm", "0"));
                FunctionalTestsRecipe.Ch3WHResistance100Ohm = int.Parse(_xml.Read("Ch3WHResistance100Ohm", "0"));
                FunctionalTestsRecipe.Ch4RHResistance100Ohm = int.Parse(_xml.Read("Ch4RHResistance100Ohm", "0"));
                FunctionalTestsRecipe.Ch5R1Resistance100Ohm = int.Parse(_xml.Read("Ch5R1Resistance100Ohm", "0"));
                FunctionalTestsRecipe.Ch6R2Resistance100Ohm = int.Parse(_xml.Read("Ch6R2Resistance100Ohm", "0"));
                _xml.CloseSection();

                _xml.OpenSection("FiveHundredOhm");
                // 500Ohm
                FunctionalTestsRecipe.Ch1WriterResistance500Ohm = int.Parse(_xml.Read("Ch1WriterResistance500Ohm", "0"));
                FunctionalTestsRecipe.Ch2TAResistance500Ohm = int.Parse(_xml.Read("Ch2TAResistance500Ohm", "0"));
                FunctionalTestsRecipe.Ch3WHResistance500Ohm = int.Parse(_xml.Read("Ch3WHResistance500Ohm", "0"));
                FunctionalTestsRecipe.Ch4RHResistance500Ohm = int.Parse(_xml.Read("Ch4RHResistance500Ohm", "0"));
                FunctionalTestsRecipe.Ch5R1Resistance500Ohm = int.Parse(_xml.Read("Ch5R1Resistance500Ohm", "0"));
                FunctionalTestsRecipe.Ch6R2Resistance500Ohm = int.Parse(_xml.Read("Ch6R2Resistance500Ohm", "0"));
                _xml.CloseSection();

                _xml.OpenSection("OneThousandOhm");
                // 1000Ohm
                FunctionalTestsRecipe.Ch1WriterResistance1000Ohm = int.Parse(_xml.Read("Ch1WriterResistance1000Ohm", "0"));
                FunctionalTestsRecipe.Ch2TAResistance1000Ohm = int.Parse(_xml.Read("Ch2TAResistance1000Ohm", "0"));
                FunctionalTestsRecipe.Ch3WHResistance1000Ohm = int.Parse(_xml.Read("Ch3WHResistance1000Ohm", "0"));
                FunctionalTestsRecipe.Ch4RHResistance1000Ohm = int.Parse(_xml.Read("Ch4RHResistance1000Ohm", "0"));
                FunctionalTestsRecipe.Ch5R1Resistance1000Ohm = int.Parse(_xml.Read("Ch5R1Resistance1000Ohm", "0"));
                FunctionalTestsRecipe.Ch6R2Resistance1000Ohm = int.Parse(_xml.Read("Ch6R2Resistance1000Ohm", "0"));
                _xml.CloseSection();

                _xml.OpenSection("TenThousandOhm");
                // 10000Ohm
                FunctionalTestsRecipe.Ch1WriterResistance10000Ohm = int.Parse(_xml.Read("Ch1WriterResistance10000Ohm", "0"));
                FunctionalTestsRecipe.Ch2TAResistance10000Ohm = int.Parse(_xml.Read("Ch2TAResistance10000Ohm", "0"));
                FunctionalTestsRecipe.Ch3WHResistance10000Ohm = int.Parse(_xml.Read("Ch3WHResistance10000Ohm", "0"));
                FunctionalTestsRecipe.Ch4RHResistance10000Ohm = int.Parse(_xml.Read("Ch4RHResistance10000Ohm", "0"));
                FunctionalTestsRecipe.Ch5R1Resistance10000Ohm = int.Parse(_xml.Read("Ch5R1Resistance10000Ohm", "0"));
                FunctionalTestsRecipe.Ch6R2Resistance10000Ohm = int.Parse(_xml.Read("Ch6R2Resistance10000Ohm", "0"));
                _xml.CloseSection();

                _xml.OpenSection("Capacitance");
                // Capacitance
                FunctionalTestsRecipe.Capacitance100pF = int.Parse(_xml.Read("Capacitance100pF", "0"));
                FunctionalTestsRecipe.Capacitance270pF = int.Parse(_xml.Read("Capacitance270pF", "0"));
                FunctionalTestsRecipe.Capacitance470pF = int.Parse(_xml.Read("Capacitance470pF", "0"));
                FunctionalTestsRecipe.Capacitance680pF = int.Parse(_xml.Read("Capacitance680pF", "0"));
                FunctionalTestsRecipe.Capacitance820pF = int.Parse(_xml.Read("Capacitance820pF", "0"));
                FunctionalTestsRecipe.Capacitance10nF = int.Parse(_xml.Read("Capacitance10nF", "0"));
                _xml.CloseSection();

                _xml.OpenSection("Temperature");
                // Temperature
                FunctionalTestsRecipe.Ch1Temperature = int.Parse(_xml.Read("Ch1Temperature", "0"));
                FunctionalTestsRecipe.Ch2Temperature = int.Parse(_xml.Read("Ch2Temperature", "0"));
                FunctionalTestsRecipe.Ch3Temperature = int.Parse(_xml.Read("Ch3Temperature", "0"));
                _xml.CloseSection();

            }
        }

        public void LoadConfigurationSetupRecipe()
        {
            string ConfigurationSetupRecipeFilePath = string.Format("{0}ConfigurationSetupRecipe.rcp", RecipeFileDirectory);
            if (!Directory.Exists(RecipeFileDirectory))
            {
                Directory.CreateDirectory(RecipeFileDirectory);
            }

            if (!File.Exists(ConfigurationSetupRecipeFilePath))
            {
                File.Create(ConfigurationSetupRecipeFilePath).Dispose();
                return;
            }
            else
            {
                Log.Info("Startup", "Retrieving recipe for configuration & setup from {0}.", ConfigurationSetupRecipeFilePath);

                SettingsXml _xml = new SettingsXml(ConfigurationSetupRecipeFilePath);

                _xml.OpenSection("Resistance");
                // BiasCurrent
                ConfigurationSetupRecipe.Ch1BiasCurrent = int.Parse(_xml.Read("BiasCurrentChannel1", "0"));
                ConfigurationSetupRecipe.Ch2BiasCurrent = int.Parse(_xml.Read("BiasCurrentChannel2", "0"));
                ConfigurationSetupRecipe.Ch3BiasCurrent = int.Parse(_xml.Read("BiasCurrentChannel3", "0"));
                ConfigurationSetupRecipe.Ch4BiasCurrent = int.Parse(_xml.Read("BiasCurrentChannel4", "0"));
                ConfigurationSetupRecipe.Ch5BiasCurrent = int.Parse(_xml.Read("BiasCurrentChannel5", "0"));
                ConfigurationSetupRecipe.Ch6BiasCurrent = int.Parse(_xml.Read("BiasCurrentChannel6", "0"));
                ConfigurationSetupRecipe.BiasCurrentSampleCountForAverage = byte.Parse(_xml.Read("ResistanceSampleCount", "0"));
                _xml.CloseSection();

                _xml.OpenSection("Temperature");
                // Temperature
                ConfigurationSetupRecipe.TimeConstant = byte.Parse(_xml.Read("TemperatureTimeConstant", "0"));                
                _xml.CloseSection();

                _xml.OpenSection("Capacitance");
                // Capacitance
                ConfigurationSetupRecipe.Frequency = int.Parse(_xml.Read("Frequency", "0"));
                ConfigurationSetupRecipe.BiasVoltage = int.Parse(_xml.Read("BiasVoltage", "0"));
                ConfigurationSetupRecipe.PeakVoltage = int.Parse(_xml.Read("Peak2PeakVoltage", "0"));
                ConfigurationSetupRecipe.BiasVoltageSampleCountForAverage = byte.Parse(_xml.Read("CapacitanceSampleCount", "0"));
                _xml.CloseSection();

                _xml.OpenSection("EnableHGAChannel");
                // EnableHGAChannel
                ConfigurationSetupRecipe.ResistanceCh1Writer = Convert.ToByte(_xml.Read("EnableHGAChannel1", "False").Equals("True"));
                ConfigurationSetupRecipe.ResistanceCh2TA = Convert.ToByte(_xml.Read("EnableHGAChannel2", "False").Equals("True"));
                ConfigurationSetupRecipe.ResistanceCh3WriteHeater = Convert.ToByte(_xml.Read("EnableHGAChannel3", "False").Equals("True"));
                ConfigurationSetupRecipe.ResistanceCh4ReadHeater = Convert.ToByte(_xml.Read("EnableHGAChannel4", "False").Equals("True"));
                ConfigurationSetupRecipe.ResistanceCh5Read1 = Convert.ToByte(_xml.Read("EnableHGAChannel5", "False").Equals("True"));
                ConfigurationSetupRecipe.ResistanceCh6Read2 = Convert.ToByte(_xml.Read("EnableHGAChannel6", "False").Equals("True"));
                ConfigurationSetupRecipe.CapacitanceCh1 = Convert.ToByte(_xml.Read("EnableHGACapacitance1", "False").Equals("True"));
                ConfigurationSetupRecipe.CapacitanceCh2 = Convert.ToByte(_xml.Read("EnableHGACapacitance2", "False").Equals("True"));
                _xml.CloseSection();

                _xml.OpenSection("EnableHGA");
                // EnableHGA                
                ConfigurationSetupRecipe.HGA1 = Convert.ToByte(_xml.Read("EnableHGA1", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA2 = Convert.ToByte(_xml.Read("EnableHGA2", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA3 = Convert.ToByte(_xml.Read("EnableHGA3", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA4 = Convert.ToByte(_xml.Read("EnableHGA4", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA5 = Convert.ToByte(_xml.Read("EnableHGA5", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA6 = Convert.ToByte(_xml.Read("EnableHGA6", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA7 = Convert.ToByte(_xml.Read("EnableHGA7", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA8 = Convert.ToByte(_xml.Read("EnableHGA8", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA9 = Convert.ToByte(_xml.Read("EnableHGA9", "False").Equals("True"));
                ConfigurationSetupRecipe.HGA10 = Convert.ToByte(_xml.Read("EnableHGA10", "False").Equals("True"));
                _xml.CloseSection();

                _xml.OpenSection("ShortCircuitDetection");
                // W+ Pairing (Row 1)
                byte row1 = 0;
                if(_xml.Read("EnableR1C2", "False").Equals("True"))
                {
                    row1 = 2;
                }
                if (_xml.Read("EnableR1C3", "False").Equals("True"))
                {
                    row1 = 3;
                }
                if (_xml.Read("EnableR1C4", "False").Equals("True"))
                {
                    row1 = 4;
                }
                if (_xml.Read("EnableR1C5", "False").Equals("True"))
                {
                    row1 = 5;
                }
                if (_xml.Read("EnableR1C6", "False").Equals("True"))
                {
                    row1 = 6;
                }
                if (_xml.Read("EnableR1C7", "False").Equals("True"))
                {
                    row1 = 7;
                }
                if (_xml.Read("EnableR1C8", "False").Equals("True"))
                {
                    row1 = 8;
                }
                if (_xml.Read("EnableR1C9", "False").Equals("True"))
                {
                    row1 = 9;
                }
                if (_xml.Read("EnableR1C10", "False").Equals("True"))
                {
                    row1 = 10;
                }
                if (_xml.Read("EnableR1C11", "False").Equals("True"))
                {
                    row1 = 11;
                }
                if (_xml.Read("EnableR1C12", "False").Equals("True"))
                {
                    row1 = 12;
                }
                ConfigurationSetupRecipe.WPlusPairing = row1;

                // W- Pairing (Row 2)
                byte row2 = 0;
                if (_xml.Read("EnableR2C1", "False").Equals("True"))
                {
                    row2 = 1;
                }
                if (_xml.Read("EnableR2C3", "False").Equals("True"))
                {
                    row2 = 3;
                }
                if (_xml.Read("EnableR2C4", "False").Equals("True"))
                {
                    row2 = 4;
                }
                if (_xml.Read("EnableR2C5", "False").Equals("True"))
                {
                    row2 = 5;
                }
                if (_xml.Read("EnableR2C6", "False").Equals("True"))
                {
                    row2 = 6;
                }
                if (_xml.Read("EnableR2C7", "False").Equals("True"))
                {
                    row2 = 7;
                }
                if (_xml.Read("EnableR2C8", "False").Equals("True"))
                {
                    row2 = 8;
                }
                if (_xml.Read("EnableR2C9", "False").Equals("True"))
                {
                    row2 = 9;
                }
                if (_xml.Read("EnableR2C10", "False").Equals("True"))
                {
                    row2 = 10;
                }
                if (_xml.Read("EnableR2C11", "False").Equals("True"))
                {
                    row2 = 11;
                }
                if (_xml.Read("EnableR2C12", "False").Equals("True"))
                {
                    row2 = 12;
                }
                ConfigurationSetupRecipe.WMinusPairing = row2;

                // TA+ Pairing (Row 3)
                byte row3 = 0;
                if (_xml.Read("EnableR3C1", "False").Equals("True"))
                {
                    row3 = 1;
                }
                if (_xml.Read("EnableR3C2", "False").Equals("True"))
                {
                    row3 = 2;
                }
                if (_xml.Read("EnableR3C4", "False").Equals("True"))
                {
                    row3 = 4;
                }
                if (_xml.Read("EnableR3C5", "False").Equals("True"))
                {
                    row3 = 5;
                }
                if (_xml.Read("EnableR3C6", "False").Equals("True"))
                {
                    row3 = 6;
                }
                if (_xml.Read("EnableR3C7", "False").Equals("True"))
                {
                    row3 = 7;
                }
                if (_xml.Read("EnableR3C8", "False").Equals("True"))
                {
                    row3 = 8;
                }
                if (_xml.Read("EnableR3C9", "False").Equals("True"))
                {
                    row3 = 9;
                }
                if (_xml.Read("EnableR3C10", "False").Equals("True"))
                {
                    row3 = 10;
                }
                if (_xml.Read("EnableR3C11", "False").Equals("True"))
                {
                    row3 = 11;
                }
                if (_xml.Read("EnableR3C12", "False").Equals("True"))
                {
                    row3 = 12;
                }
                ConfigurationSetupRecipe.TAPlusPairing = row3;

                // TA- Pairing (Row 4)
                byte row4 = 0;
                if (_xml.Read("EnableR4C1", "False").Equals("True"))
                {
                    row4 = 1;
                }
                if (_xml.Read("EnableR4C2", "False").Equals("True"))
                {
                    row4 = 2;
                }
                if (_xml.Read("EnableR4C3", "False").Equals("True"))
                {
                    row4 = 3;
                }
                if (_xml.Read("EnableR4C5", "False").Equals("True"))
                {
                    row4 = 5;
                }
                if (_xml.Read("EnableR4C6", "False").Equals("True"))
                {
                    row4 = 6;
                }
                if (_xml.Read("EnableR4C7", "False").Equals("True"))
                {
                    row4 = 7;
                }
                if (_xml.Read("EnableR4C8", "False").Equals("True"))
                {
                    row4 = 8;
                }
                if (_xml.Read("EnableR4C9", "False").Equals("True"))
                {
                    row4 = 9;
                }
                if (_xml.Read("EnableR4C10", "False").Equals("True"))
                {
                    row4 = 10;
                }
                if (_xml.Read("EnableR4C11", "False").Equals("True"))
                {
                    row4 = 11;
                }
                if (_xml.Read("EnableR4C12", "False").Equals("True"))
                {
                    row4 = 12;
                }
                ConfigurationSetupRecipe.TAMinusPairing = row4;

                // WH+ Pairing (Row 5)
                byte row5 = 0;
                if (_xml.Read("EnableR5C1", "False").Equals("True"))
                {
                    row5 = 1;
                }
                if (_xml.Read("EnableR5C2", "False").Equals("True"))
                {
                    row5 = 2;
                }
                if (_xml.Read("EnableR5C3", "False").Equals("True"))
                {
                    row5 = 3;
                }
                if (_xml.Read("EnableR5C4", "False").Equals("True"))
                {
                    row5 = 4;
                }
                if (_xml.Read("EnableR5C6", "False").Equals("True"))
                {
                    row5 = 6;
                }
                if (_xml.Read("EnableR5C7", "False").Equals("True"))
                {
                    row5 = 7;
                }
                if (_xml.Read("EnableR5C8", "False").Equals("True"))
                {
                    row5 = 8;
                }
                if (_xml.Read("EnableR5C9", "False").Equals("True"))
                {
                    row5 = 9;
                }
                if (_xml.Read("EnableR5C10", "False").Equals("True"))
                {
                    row5 = 10;
                }
                if (_xml.Read("EnableR5C11", "False").Equals("True"))
                {
                    row5 = 11;
                }
                if (_xml.Read("EnableR5C12", "False").Equals("True"))
                {
                    row5 = 12;
                }
                ConfigurationSetupRecipe.WHPlusPairing = row5;

                // WH- Pairing (Row 6)
                byte row6 = 0;
                if (_xml.Read("EnableR6C1", "False").Equals("True"))
                {
                    row6 = 1;
                }
                if (_xml.Read("EnableR6C2", "False").Equals("True"))
                {
                    row6 = 2;
                }
                if (_xml.Read("EnableR6C3", "False").Equals("True"))
                {
                    row6 = 3;
                }
                if (_xml.Read("EnableR6C4", "False").Equals("True"))
                {
                    row6 = 4;
                }
                if (_xml.Read("EnableR6C5", "False").Equals("True"))
                {
                    row6 = 5;
                }
                if (_xml.Read("EnableR6C7", "False").Equals("True"))
                {
                    row6 = 7;
                }
                if (_xml.Read("EnableR6C8", "False").Equals("True"))
                {
                    row6 = 8;
                }
                if (_xml.Read("EnableR6C9", "False").Equals("True"))
                {
                    row6 = 9;
                }
                if (_xml.Read("EnableR6C10", "False").Equals("True"))
                {
                    row6 = 10;
                }
                if (_xml.Read("EnableR6C11", "False").Equals("True"))
                {
                    row6 = 11;
                }
                if (_xml.Read("EnableR6C12", "False").Equals("True"))
                {
                    row6 = 12;
                }
                ConfigurationSetupRecipe.WHMinusPairing = row6;

                // RH+ Pairing (Row 7)
                byte row7 = 0;
                if (_xml.Read("EnableR7C1", "False").Equals("True"))
                {
                    row7 = 1;
                }
                if (_xml.Read("EnableR7C2", "False").Equals("True"))
                {
                    row7 = 2;
                }
                if (_xml.Read("EnableR7C3", "False").Equals("True"))
                {
                    row7 = 3;
                }
                if (_xml.Read("EnableR7C4", "False").Equals("True"))
                {
                    row7 = 4;
                }
                if (_xml.Read("EnableR7C5", "False").Equals("True"))
                {
                    row7 = 5;
                }
                if (_xml.Read("EnableR7C6", "False").Equals("True"))
                {
                    row7 = 6;
                }
                if (_xml.Read("EnableR7C8", "False").Equals("True"))
                {
                    row7 = 8;
                }
                if (_xml.Read("EnableR7C9", "False").Equals("True"))
                {
                    row7 = 9;
                }
                if (_xml.Read("EnableR7C10", "False").Equals("True"))
                {
                    row7 = 10;
                }
                if (_xml.Read("EnableR7C11", "False").Equals("True"))
                {
                    row7 = 11;
                }
                if (_xml.Read("EnableR7C12", "False").Equals("True"))
                {
                    row7 = 12;
                }
                ConfigurationSetupRecipe.RHPlusPairing = row7;

                // RH- Pairing (Row 8)
                byte row8 = 0;
                if (_xml.Read("EnableR8C1", "False").Equals("True"))
                {
                    row8 = 1;
                }
                if (_xml.Read("EnableR8C2", "False").Equals("True"))
                {
                    row8 = 2;
                }
                if (_xml.Read("EnableR8C3", "False").Equals("True"))
                {
                    row8 = 3;
                }
                if (_xml.Read("EnableR8C4", "False").Equals("True"))
                {
                    row8 = 4;
                }
                if (_xml.Read("EnableR8C5", "False").Equals("True"))
                {
                    row8 = 5;
                }
                if (_xml.Read("EnableR8C6", "False").Equals("True"))
                {
                    row8 = 6;
                }
                if (_xml.Read("EnableR8C7", "False").Equals("True"))
                {
                    row8 = 7;
                }
                if (_xml.Read("EnableR8C9", "False").Equals("True"))
                {
                    row8 = 9;
                }
                if (_xml.Read("EnableR8C10", "False").Equals("True"))
                {
                    row8 = 10;
                }
                if (_xml.Read("EnableR8C11", "False").Equals("True"))
                {
                    row8 = 11;
                }
                if (_xml.Read("EnableR8C12", "False").Equals("True"))
                {
                    row8 = 12;
                }
                ConfigurationSetupRecipe.RHMinusPairing = row8;

                // R1+ Pairing (Row 9)
                byte row9 = 0;
                if (_xml.Read("EnableR9C1", "False").Equals("True"))
                {
                    row9 = 1;
                }
                if (_xml.Read("EnableR9C2", "False").Equals("True"))
                {
                    row9 = 2;
                }
                if (_xml.Read("EnableR9C3", "False").Equals("True"))
                {
                    row9 = 3;
                }
                if (_xml.Read("EnableR9C4", "False").Equals("True"))
                {
                    row9 = 4;
                }
                if (_xml.Read("EnableR9C5", "False").Equals("True"))
                {
                    row9 = 5;
                }
                if (_xml.Read("EnableR9C6", "False").Equals("True"))
                {
                    row9 = 6;
                }
                if (_xml.Read("EnableR9C7", "False").Equals("True"))
                {
                    row9 = 7;
                }
                if (_xml.Read("EnableR9C8", "False").Equals("True"))
                {
                    row9 = 8;
                }
                if (_xml.Read("EnableR9C10", "False").Equals("True"))
                {
                    row9 = 10;
                }
                if (_xml.Read("EnableR9C11", "False").Equals("True"))
                {
                    row9 = 11;
                }
                if (_xml.Read("EnableR9C12", "False").Equals("True"))
                {
                    row9 = 12;
                }
                ConfigurationSetupRecipe.R1PlusPairing = row9;

                // R1- Pairing (Row 10)
                byte row10 = 0;
                if (_xml.Read("EnableR10C1", "False").Equals("True"))
                {
                    row10 = 1;
                }
                if (_xml.Read("EnableR10C2", "False").Equals("True"))
                {
                    row10 = 2;
                }
                if (_xml.Read("EnableR10C3", "False").Equals("True"))
                {
                    row10 = 3;
                }
                if (_xml.Read("EnableR10C4", "False").Equals("True"))
                {
                    row10 = 4;
                }
                if (_xml.Read("EnableR10C5", "False").Equals("True"))
                {
                    row10 = 5;
                }
                if (_xml.Read("EnableR10C6", "False").Equals("True"))
                {
                    row10 = 6;
                }
                if (_xml.Read("EnableR10C7", "False").Equals("True"))
                {
                    row10 = 7;
                }
                if (_xml.Read("EnableR10C8", "False").Equals("True"))
                {
                    row10 = 8;
                }
                if (_xml.Read("EnableR10C9", "False").Equals("True"))
                {
                    row10 = 9;
                }
                if (_xml.Read("EnableR10C11", "False").Equals("True"))
                {
                    row10 = 11;
                }
                if (_xml.Read("EnableR10C12", "False").Equals("True"))
                {
                    row10 = 12;
                }
                ConfigurationSetupRecipe.R1MinusPairing = row10;

                // R2+ Pairing (Row 11)
                byte row11 = 0;
                if (_xml.Read("EnableR11C1", "False").Equals("True"))
                {
                    row11 = 1;
                }
                if (_xml.Read("EnableR11C2", "False").Equals("True"))
                {
                    row11 = 2;
                }
                if (_xml.Read("EnableR11C3", "False").Equals("True"))
                {
                    row11 = 3;
                }
                if (_xml.Read("EnableR11C4", "False").Equals("True"))
                {
                    row11 = 4;
                }
                if (_xml.Read("EnableR11C5", "False").Equals("True"))
                {
                    row11 = 5;
                }
                if (_xml.Read("EnableR11C6", "False").Equals("True"))
                {
                    row11 = 6;
                }
                if (_xml.Read("EnableR11C7", "False").Equals("True"))
                {
                    row11 = 7;
                }
                if (_xml.Read("EnableR11C8", "False").Equals("True"))
                {
                    row11 = 8;
                }
                if (_xml.Read("EnableR11C9", "False").Equals("True"))
                {
                    row11 = 9;
                }
                if (_xml.Read("EnableR11C10", "False").Equals("True"))
                {
                    row11 = 10;
                }
                if (_xml.Read("EnableR11C12", "False").Equals("True"))
                {
                    row11 = 12;
                }
                ConfigurationSetupRecipe.R2PlusPairing = row11;

                // R2- Pairing (Row 12)
                byte row12 = 0;
                if (_xml.Read("EnableR12C1", "False").Equals("True"))
                {
                    row12 = 1;
                }
                if (_xml.Read("EnableR12C2", "False").Equals("True"))
                {
                    row12 = 2;
                }
                if (_xml.Read("EnableR12C3", "False").Equals("True"))
                {
                    row12 = 3;
                }
                if (_xml.Read("EnableR12C4", "False").Equals("True"))
                {
                    row12 = 4;
                }
                if (_xml.Read("EnableR12C5", "False").Equals("True"))
                {
                    row12 = 5;
                }
                if (_xml.Read("EnableR12C6", "False").Equals("True"))
                {
                    row12 = 6;
                }
                if (_xml.Read("EnableR12C7", "False").Equals("True"))
                {
                    row12 = 7;
                }
                if (_xml.Read("EnableR12C8", "False").Equals("True"))
                {
                    row12 = 8;
                }
                if (_xml.Read("EnableR12C9", "False").Equals("True"))
                {
                    row12 = 9;
                }
                if (_xml.Read("EnableR12C10", "False").Equals("True"))
                {
                    row12 = 10;
                }
                if (_xml.Read("EnableR12C11", "False").Equals("True"))
                {
                    row12 = 11;
                }
                ConfigurationSetupRecipe.R2MinusPairing = row12;

                _xml.CloseSection();
            }
        }        
    }
}
