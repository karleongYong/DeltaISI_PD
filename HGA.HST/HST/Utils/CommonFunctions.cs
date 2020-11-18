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
using System.Reflection;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.UI;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using Seagate.AAS.HGA.HST.Utils;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.Parsel.Services;

namespace Seagate.AAS.HGA.HST.Utils
{
    public class CommonFunctions
    {
        // Lai: Use this file to detect whether HST application is properly close or not.
        //      If application is properly close, this file will be deleted.
        public const string TempFileWhenRunningHSTApplication = "C:\\Seagate\\HGA.HST\\RunningHSTApplication";
        public const string TempFileToIndicateDownloadConfigToProcessorClicked = "C:\\Seagate\\HGA.HST\\DownloadConfigToProcessorClicked";
        public const string UNKNOWN = "Unknown";
        public const string FAIL = "Fail";
        public const string PASS = "Pass";
        public const string NOT_AVAILABLE = "N/A";
        public const char HST_STATION_CODE = 'V';
        public const char TEST_PASS_CODE = 'A';
        public const char SKIP_TEST = 'B';
        public const char RISK_OF_BRIDGE_CODE = 'E';
        public const char TEST_FAIL_CODE = 'F';
        public const char LDU_FAIL_SHORT = 'X';
        public const string RecipeExt = ".HST";

        public const int SIMULATION_DELAY = 100;
        public DateTime PreviousPrecisorNestAtInputPositionTimeStamp = DateTime.Now;
        public DateTime PreviousPrecisorNestAtStandbyPositionTimeStamp = DateTime.Now;

        public MeasurementTestRecipe MeasurementTestRecipe;
        public ConfigurationSetupRecipe ConfigurationSetupRecipe;
        public CompensationInfo CompensationInfo;
        public FunctionalTestsRecipe FunctionalTestsRecipe;
        public int ProductID = 1;  // Default test electronics' product ID
        public int ConversionBoardID = 1;  // Default test electronics' conversion board ID
        public string ProductRecipeName = string.Empty;
        public string ProductName = String.Empty;
        public bool SystemInitializationCompleted = false;
        public string strErrorMessageCode = "0";
        private static Object _carrierListLock = new object();
        public CarrierCycleTime CarrierCycleTime;
        public MachineUPH MachineUPH;
        public int ConsecutiveBoatsWithTICError = 0;
        public int ConsecutiveBoatsWithFailPickupByInputEE = 0;
        public bool StopMachineRunDueToTICError = false;
        public bool StopMachineRunDueToGetputError = false;
        public bool IsOutputWorkingProcess = false;
        public string TICErrorMessage = "";
        public string CRDLErrorMessage = "";
        public string GetputErrorMessage = "";
        public string RecipeChangeErrorMessage = "";
        public string AlertRecipeChangeErrorMessage;                                  
        public bool powerOnConveyor = true;
        public bool powerOffConveyor = false;
        public string visionError = "NoError";

        public Queue<Carrier> TestedCarrierQueue;
        public List<string> SpacialUser;
        public Queue<CarrierCycleTime> CarrierCycleTimeQueue
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> OverallHGATestProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> TestProbeHandlerMovesDownToPrecisorNestProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> CollisionAvoidanceBetweenTestProbeHandlerAndPrecisorNestProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> PrecisorNestMovesFromPrecisorStationToOutputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> PrecisorNestStabilityAtInputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingInputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> BoatArrivesAtOutputStationFromInputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> PrecisorNestStabilityAtOutputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> CollisionAvoidanceBetweenOutputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> CollisionAvoidanceBetweenPrecisorNestAndOutputEEMovingDownToOutputCarrierProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> OutputStationHGADetectionStabilityProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        public Queue<ProcessStopWatch> BoatArrivesAtOutputTurnStationFromOutputStationProcessCycleTimeStopWatch
        {
            get;
            set;
        }

        #region Data

        public Queue<Carrier> InputCarriers
        {
            get;
            set;
        }

        public Queue<Carrier> OutputCarriersQueue
        {
            get;
            set;
        }

        #endregion

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
            TestedCarrierQueue = new Queue<Carrier>();
            InputCarriers = new Queue<Carrier>();
            MachineUPH = new MachineUPH();
            OutputCarriersQueue = new Queue<Carrier>();
            FunctionalTestsRecipe = new Data.FunctionalTestsRecipe();
            OverallHGATestProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            TestProbeHandlerMovesDownToPrecisorNestProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            CollisionAvoidanceBetweenTestProbeHandlerAndPrecisorNestProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            PrecisorNestMovesFromPrecisorStationToOutputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            PrecisorNestStabilityAtInputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingInputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            BoatArrivesAtOutputStationFromInputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            PrecisorNestStabilityAtOutputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            CollisionAvoidanceBetweenOutputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            CollisionAvoidanceBetweenPrecisorNestAndOutputEEMovingDownToOutputCarrierProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            OutputStationHGADetectionStabilityProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            BoatArrivesAtOutputTurnStationFromOutputStationProcessCycleTimeStopWatch = new Queue<ProcessStopWatch>();
            CarrierCycleTimeQueue = new Queue<CarrierCycleTime>();
            SpacialUser = new List<string>();
        }

        #endregion

        #region Lock

        public Object InputCarriersLock
        {
            get
            {
                return _carrierListLock;
            }
            set
            {
                _carrierListLock = value;
            }
        }
        #endregion

        public Queue<TestProbeAPICommand> OutgoingTestProbeDataAPIs
        {
            get;
            set;
        }

        public bool ActivePopupCRDLerrorMessage { get; set; }
        public bool ActivePopupGetputErrorMessage { get; set; }
        public bool ActivePopupRecipeChangedErrorMessage { get; set; }
        public bool IsDoubleTestSpecLoaded { get; set; }
        public bool IsRunningWithNewTSR { get; set; }
        public string GetMachineConfigGlobalInfo { get { return System.IO.Path.Combine(@"" + 
            HSTSettings.Instance.Directory.McConfigGlobalPath + "\\" + MeasurementTestSettings.MachineConfigFileName);
} }

        public void LogProcessCycleTime(string FileName, TimeSpan ElapsedTime, string CarrierID = "", string StopWatchStartTime = "", string StopWatchStopTime = "")
        {
            double ElapsedTimeInSeconds = (ElapsedTime.Hours * 3600) + (ElapsedTime.Minutes * 60) + ElapsedTime.Seconds + ElapsedTime.Milliseconds / 1000.0;
            if (ElapsedTimeInSeconds == 0.0 /*|| HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog == false*/)
            {
                return;
            }

            string ProcessCycleTimeFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath + "\\ProcessCycleTime", FileName);
            try
            {
                using (StreamWriter ProcessCycleTimeFile = new StreamWriter(ProcessCycleTimeFilePath, true))
                {
                    string ProcessCycleTimeRecord = String.Join(",",
                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                       ElapsedTimeInSeconds,
                       CarrierID,
                       StopWatchStartTime,
                       StopWatchStopTime
                       );

                    ProcessCycleTimeFile.WriteLine(ProcessCycleTimeRecord);
                }
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Error in logging process cycle time into '" + ProcessCycleTimeFilePath + "'", ex);
            }
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

        public void PromptExceptionErrorMessage(string sourceName, Exception ex)
        {
            ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);

            ActiveProcessHST.ErrorMessageHST errorMessageHST = new ActiveProcessHST.ErrorMessageHST(HSTMachine.Workcell.Process.MonitorProcess, btnlst, ex);
            errorMessageHST.Format(ex);
            errorMessageHST.SourceProcess = "";
            errorMessageHST.SourceState = "N/A";
            errorMessageHST.Format();
            Log.Error(this, "{0}, ProcessName:{1}, Error:{2}", LoggerCategory.StateTransition, sourceName, ex.ToString());

            ServiceManager.ErrorHandler.RegisterMessage(errorMessageHST);
        }

        /// <summary>
        /// Check recipe file between global path and local path
        /// </summary>
        /// <param name="recipeName"></param>
        /// <returns></returns>
        private bool CheckRecipeFile(string recipeName)
        {
            bool isFileReady = false;
            try
            {
                var getRecipefile = HSTSettings.Instance.Directory.TSRRecipLocalPath + "\\" + recipeName;
                if (!File.Exists(getRecipefile))
                {
                    var getRecipeFileFromGlobal = HSTSettings.Instance.Directory.TSRRecipeGlobalPath + "\\" + recipeName;
                    if (File.Exists(getRecipeFileFromGlobal))
                    {
                        CopyRecipeFileFromGlobalToLocal(recipeName);
                    }
                    else
                    {
                        string msg = String.Format("File recipe not found, please check recipe name <{0}> in global path ==> <{1}>", recipeName, getRecipeFileFromGlobal);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    CheckCompareRecipeFileBetweenGlobalAndLocal(recipeName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return isFileReady;
        }

        /// <summary>
        /// Check comparable recipe file in golbal and local and force to replace from global
        /// </summary>
        /// <param name="fileName"></param>
        public bool CheckCompareRecipeFileBetweenGlobalAndLocal(string fileName)
        {
            bool isUpdated = false;
            try
            {
                var localFileName = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TSRRecipLocalPath + "\\" + fileName);
                var globalFileName = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TSRRecipeGlobalPath + "\\" + fileName);

                FileInfo serverInfo = new FileInfo(globalFileName);
                FileInfo localInfo = new FileInfo(localFileName);

                if (localInfo.LastWriteTime.CompareTo(serverInfo.LastWriteTime) != 0)
                {
                    CopyRecipeFileFromGlobalToLocal(fileName);
                    isUpdated = true;
                }
            }
            catch (Exception)
            {
            }

            return isUpdated;
        }

        /// <summary>
        /// Copy recipe file from golbal path to local path in overwrite condition
        /// </summary>
        /// <param name="fileName"></param>
        private void CopyRecipeFileFromGlobalToLocal(string fileName)
        {
            try
            {
                var movefrom = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TSRRecipeGlobalPath + "\\" + fileName);
                var moveToPath = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TSRRecipLocalPath);
                var moveToFile = System.IO.Path.Combine(@"" + moveToPath + "\\" + fileName);

                if (!Directory.Exists(moveToPath))
                {
                    Directory.CreateDirectory(moveToPath);
                }
                if (File.Exists(movefrom))
                {
                    FileInfo originalFile = new FileInfo(movefrom);
                    FileInfo newFile = new FileInfo(moveToFile);
                    File.Copy(movefrom, moveToFile, true);
                }
            }
            catch (Exception)
            {
            }
        }

        private bool CheckConfigFile(string configName)
        {
            bool isFileReady = false;
            try
            {
                if (!File.Exists(configName))
                {
                    if (File.Exists(configName))
                    {
                        CopyConfigFileFromGlobalToLocal(configName);
                    }
                    else
                    {
                        string msg = String.Format("File machine config not found, please check recipe name <{0}>", configName);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    CheckCompareConfigFileBetweenGlobalAndLocal(configName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return isFileReady;
        }

        public bool CheckCompareConfigFileBetweenGlobalAndLocal(string fileName)
        {
            bool isUpdated = false;
            try
            {
                var split = fileName.Split('\\');
                var localFileName = System.IO.Path.Combine(@"" +"C:\\Seagate\\HGA.HST" + "\\" + split[2] + "\\" + split[3] + "\\" + split[4] + "\\" + split[5]);
                var globalFileName = fileName;

                FileInfo serverInfo = new FileInfo(globalFileName);
                FileInfo localInfo = new FileInfo(localFileName);

                if (localInfo.LastWriteTime.CompareTo(serverInfo.LastWriteTime) != 0)
                {
                    CopyConfigFileFromGlobalToLocal(fileName);
                    isUpdated = true;
                }
            }
            catch (Exception)
            {
            }
            return isUpdated;
        }


        private void CopyConfigFileFromGlobalToLocal(string fileName)
        {
            try
            {

                var split = fileName.Split('\\');
                var moveToPath = System.IO.Path.Combine(@"" + "C:\\Seagate\\HGA.HST" + "\\" + split[2] + "\\" + split[3] + "\\" + split[4]);
                var movefrom = fileName;
                var moveToFile = System.IO.Path.Combine(@"" + "C:\\Seagate\\HGA.HST" + "\\" + split[2] + "\\" + split[3] + "\\" + split[4] + "\\" + split[5]);

                if (!Directory.Exists(moveToPath))
                {
                    Directory.CreateDirectory(moveToPath);
                }
                if (File.Exists(movefrom))
                {
                    FileInfo originalFile = new FileInfo(movefrom);
                    FileInfo newFile = new FileInfo(moveToFile);
                    File.Copy(movefrom, moveToFile, true);
                }
            }
            catch (Exception)
            {
            }
        }


        public void LoadMeasurementTestRecipe(string ProductName)
        {
            string fileName = ProductName;
            string getRecipefile = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TSRRecipLocalPath + "\\" + fileName);

            CheckRecipeFile(fileName);

            SettingsXml _xml = new SettingsXml(getRecipefile);

            var section = "ProductInformation";
            _xml.OpenSection(section);
            MeasurementTestRecipe.ProductName = _xml.Read("ProductName", String.Empty);
            MeasurementTestRecipe.SuspensionType = _xml.Read("SuspensionType", String.Empty);
            MeasurementTestRecipe.SliderForm = _xml.Read("SliderForm", String.Empty);
            MeasurementTestRecipe.TgaPnUp = _xml.Read("TGAPNUP", String.Empty);
            MeasurementTestRecipe.TgaPnDn = _xml.Read("TGAPNDN", String.Empty);
            MeasurementTestRecipe.InterPad = _xml.Read("InterPad", String.Empty);
            MeasurementTestRecipe.PadWidth = _xml.Read("PadWidth", String.Empty);
            MeasurementTestRecipe.PadWidth = _xml.Read("PadPitch", String.Empty);
            _xml.CloseSection();

            section = "TSRInformation";
            _xml.OpenSection(section);
            MeasurementTestRecipe.ProductID = int.Parse(_xml.Read("ProductID", "0"));
            MeasurementTestRecipe.TSRName = _xml.Read("TSRName", "");
            MeasurementTestRecipe.TSRNumber = _xml.Read("TSRNumber", "");
            MeasurementTestRecipe.TSRGroup = _xml.Read("TSRGroup", "");
            MeasurementTestRecipe.SpecNumber = double.Parse(_xml.Read("SpecNumber", "0.0"));
            MeasurementTestRecipe.SpecVersion = _xml.Read("SpecVersion", "");
            MeasurementTestRecipe.ParamID = _xml.Read("ParamID", "");
            MeasurementTestRecipe.ScriptName = _xml.Read("ScriptName", "");
            MeasurementTestRecipe.ScriptDate = _xml.Read("ScriptDate", "");
            MeasurementTestRecipe.Radius = double.Parse(_xml.Read("Radius", "0.0"));
            MeasurementTestRecipe.RPM = double.Parse(_xml.Read("RPM", "0"));
            MeasurementTestRecipe.SkewAngle = double.Parse(_xml.Read("SkewAngle", "0"));
            _xml.CloseSection();

            section = "DeltaWriterHSTSDETForWriterBridging";
            _xml.OpenSection(section);
            MeasurementTestRecipe.WriterResistanceSpecUP = double.Parse(_xml.Read("DeltaWriterUp", "0"));
            MeasurementTestRecipe.WriterResistanceSpecDN = double.Parse(_xml.Read("DeltaWriterDn", "0"));
            _xml.CloseSection();

            section = "OffsetReaderHSTISI";
            _xml.OpenSection(section);
            MeasurementTestRecipe.ReaderImpedanceR1Spec = double.Parse(_xml.Read("OffsetReader1", "0"));
            MeasurementTestRecipe.ReaderImpedanceR2Spec = double.Parse(_xml.Read("OffsetReader2", "0"));
            _xml.CloseSection();

            _xml.OpenSection("ResistanceTest/ResistanceSpec");
            MeasurementTestRecipe.Ch1WriterResistanceMin = double.Parse(_xml.Read("Ch1WriterResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch1WriterResistanceMax = double.Parse(_xml.Read("Ch1WriterResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch1WriterResistanceMin = {0}, MeasurementTestRecipe.Ch1WriterResistanceMax = {1}.", MeasurementTestRecipe.Ch1WriterResistanceMin, MeasurementTestRecipe.Ch1WriterResistanceMax);
            }
            MeasurementTestRecipe.Ch2TAResistanceMin = double.Parse(_xml.Read("Ch2TAResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch2TAResistanceMax = double.Parse(_xml.Read("Ch2TAResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch2TAResistanceMin = {0}, MeasurementTestRecipe.Ch2TAResistanceMax = {1}.", MeasurementTestRecipe.Ch2TAResistanceMin, MeasurementTestRecipe.Ch2TAResistanceMax);
            }
            MeasurementTestRecipe.Ch3WHResistanceMin = double.Parse(_xml.Read("Ch3WHResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch3WHResistanceMax = double.Parse(_xml.Read("Ch3WHResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch3WHResistanceMin = {0}, MeasurementTestRecipe.Ch3WHResistanceMax = {1}.", MeasurementTestRecipe.Ch3WHResistanceMin, MeasurementTestRecipe.Ch3WHResistanceMax);
            }
            MeasurementTestRecipe.Ch4RHResistanceMin = double.Parse(_xml.Read("Ch4RHResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch4RHResistanceMax = double.Parse(_xml.Read("Ch4RHResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch4RHResistanceMin = {0}, MeasurementTestRecipe.Ch4RHResistanceMax = {1}.", MeasurementTestRecipe.Ch4RHResistanceMin, MeasurementTestRecipe.Ch4RHResistanceMax);
            }
            MeasurementTestRecipe.Ch5R1ResistanceMin = double.Parse(_xml.Read("Ch5R1ResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch5R1ResistanceMax = double.Parse(_xml.Read("Ch5R1ResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch5R1ResistanceMin = {0}, MeasurementTestRecipe.Ch5R1ResistanceMax = {1}.", MeasurementTestRecipe.Ch5R1ResistanceMin, MeasurementTestRecipe.Ch5R1ResistanceMax);
            }
            MeasurementTestRecipe.Ch6R2ResistanceMin = double.Parse(_xml.Read("Ch6R2ResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch6R2ResistanceMax = double.Parse(_xml.Read("Ch6R2ResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch6R2ResistanceMin = {0}, MeasurementTestRecipe.Ch6R2ResistanceMax = {1}.", MeasurementTestRecipe.Ch6R2ResistanceMin, MeasurementTestRecipe.Ch6R2ResistanceMax);
            }

            MeasurementTestRecipe.Ch6LDUResistanceMin = double.Parse(_xml.Read("Ch6LDUResistanceMin", "0.0"));
            MeasurementTestRecipe.Ch6LDUResistanceMax = double.Parse(_xml.Read("Ch6LDUResistanceMax", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.Ch6LDUResistanceMin = {0}, MeasurementTestRecipe.Ch6LDUResistanceMax = {1}.", MeasurementTestRecipe.Ch6LDUResistanceMin, MeasurementTestRecipe.Ch6LDUResistanceMax);
            }
            _xml.CloseSection();

            _xml.OpenSection("ResistanceTest/OpenShortSpec");
            MeasurementTestRecipe.Ch1WriterOpenShortMin = double.Parse(_xml.Read("Ch1WriterOpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch1WriterOpenShortMax = double.Parse(_xml.Read("Ch1WriterOpenShortMax", "0.0"));
            MeasurementTestRecipe.Ch2TAOpenShortMin = double.Parse(_xml.Read("Ch2TAOpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch2TAOpenShortMax = double.Parse(_xml.Read("Ch2TAOpenShortMax", "0.0"));
            MeasurementTestRecipe.Ch3WHOpenShortMin = double.Parse(_xml.Read("Ch3WHOpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch3WHOpenShortMax = double.Parse(_xml.Read("Ch3WHOpenShortMax", "0.0"));
            MeasurementTestRecipe.Ch4RHOpenShortMin = double.Parse(_xml.Read("Ch4RHOpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch4RHOpenShortMax = double.Parse(_xml.Read("Ch4RHOpenShortMax", "0.0"));
            MeasurementTestRecipe.Ch5R1OpenShortMin = double.Parse(_xml.Read("Ch5R1OpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch5R1OpenShortMax = double.Parse(_xml.Read("Ch5R1OpenShortMax", "0.0"));
            MeasurementTestRecipe.Ch6R2OpenShortMin = double.Parse(_xml.Read("Ch6R2OpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch6R2OpenShortMax = double.Parse(_xml.Read("Ch6R2OpenShortMax", "0.0"));
            MeasurementTestRecipe.Ch6LDUOpenShortMin = double.Parse(_xml.Read("Ch6LDUOpenShortMin", "0.0"));
            MeasurementTestRecipe.Ch6LDUOpenShortMax = double.Parse(_xml.Read("Ch6LDUOpenShortMax", "0.0"));
            _xml.CloseSection();

            _xml.OpenSection("OffsetHSTET");
            MeasurementTestRecipe.Ch1WriterResistanceOffset = double.Parse(_xml.Read("Ch1WriterResistanceOffset", "0.0"));
            MeasurementTestRecipe.Ch2TAResistanceOffset = double.Parse(_xml.Read("Ch2TAResistanceOffset", "-2.0"));
            MeasurementTestRecipe.Ch3WHResistanceOffset = double.Parse(_xml.Read("Ch3WHResistanceOffset", "4.0"));
            MeasurementTestRecipe.Ch4RHResistanceOffset = double.Parse(_xml.Read("Ch4RHResistanceOffset", "11.0"));
            MeasurementTestRecipe.Ch5R1ResistanceOffset = double.Parse(_xml.Read("Ch5R1ResistanceOffset", "4.0"));
            MeasurementTestRecipe.Ch6R2ResistanceOffset = double.Parse(_xml.Read("Ch6R2ResistanceOffset", "4.0"));
            MeasurementTestRecipe.Ch6LDUResistanceOffset = double.Parse(_xml.Read("Ch6LDUResistanceOffset", "0.0"));
            _xml.CloseSection();

            _xml.OpenSection("BaselineVoltageShortTest");
            MeasurementTestRecipe.Ch1WriterResistanceVoltRatio = double.Parse(_xml.Read("Ch1WriterResistanceVoltRatio", "0.0"));
            MeasurementTestRecipe.Ch2TAResistanceVoltRatio = double.Parse(_xml.Read("Ch2TAResistanceVoltRatio", "0.0"));
            MeasurementTestRecipe.Ch3WHResistanceVoltRatio = double.Parse(_xml.Read("Ch3WHResistanceVoltRatio", "0.0"));
            MeasurementTestRecipe.Ch4RHResistanceVoltRatio = double.Parse(_xml.Read("Ch4RHResistanceVoltRatio", "0.0"));
            MeasurementTestRecipe.Ch5R1ResistanceVoltRatio = double.Parse(_xml.Read("Ch5R1ResistanceVoltRatio", "0.0"));
            MeasurementTestRecipe.Ch6R2ResistanceVoltRatio = double.Parse(_xml.Read("Ch6R2ResistanceVoltRatio", "0.0"));
            MeasurementTestRecipe.Ch6LDUResistanceVoltRatio = double.Parse(_xml.Read("Ch6LDUResistanceVoltRatio", "0.0"));
            _xml.CloseSection();

            _xml.OpenSection("ANCYieldTriggering");
            MeasurementTestRecipe.TestRunCount = double.Parse(_xml.Read("TestRunCount", "500.0"));
            MeasurementTestRecipe.YieldLimit = double.Parse(_xml.Read("YieldLimit", "99.0"));
            MeasurementTestRecipe.YieldTarget = double.Parse(_xml.Read("YieldTarget", "99.40"));
            MeasurementTestRecipe.CountLimit = double.Parse(_xml.Read("CountLimit", "3.0"));
            MeasurementTestRecipe.GoodBetweenBad = double.Parse(_xml.Read("GoodBetweenBad", "20.0"));
            _xml.CloseSection();

            _xml.OpenSection("PadVoltageDetection");

            var checkforNewTsr = _xml.Read("PadVoltageDelta2LowerDetectionThreshold", string.Empty);
            if (!string.IsNullOrEmpty(checkforNewTsr)) IsRunningWithNewTSR = true;
            else IsRunningWithNewTSR = false;

            MeasurementTestRecipe.VolThreshold1HiLimit = double.Parse(_xml.Read("PadVoltageDelta1UpperDetectionThreshold", "40.0"));
            MeasurementTestRecipe.VolThreshold1LowLimit = double.Parse(_xml.Read("PadVoltageDelta1LowerDetectionThreshold", "15.0"));
            MeasurementTestRecipe.VolThreshold2HiLimit = double.Parse(_xml.Read("PadVoltageDelta2UpperDetectionThreshold", "4.0"));
            MeasurementTestRecipe.VolThreshold2LowLimit = double.Parse(_xml.Read("PadVoltageDelta2LowerDetectionThreshold", "1.0"));
            _xml.CloseSection();

            _xml.OpenSection("ToleranceSpecShortTest");
            MeasurementTestRecipe.Ch1WriterResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch1WriterResistanceToleranceUpperSpec", "40.0"));
            MeasurementTestRecipe.Ch2TAResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch2TAResistanceToleranceUpperSpec", "40.0"));
            MeasurementTestRecipe.Ch3WHResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch3WHResistanceToleranceUpperSpec", "40.0"));
            MeasurementTestRecipe.Ch4RHResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch4RHResistanceToleranceUpperSpec", "40.0"));
            MeasurementTestRecipe.Ch5R1ResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch5R1ResistanceToleranceUpperSpec", "40.0"));
            MeasurementTestRecipe.Ch6R2ResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch6R2ResistanceToleranceUpperSpec", "40.0"));
            MeasurementTestRecipe.Ch6LDUResistanceToleranceUpperSpec = double.Parse(_xml.Read("Ch6LDUResistanceToleranceUpperSpec", "0.0"));

            MeasurementTestRecipe.Ch1WriterResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch1WriterResistanceToleranceLowerSpec", "40.0"));
            MeasurementTestRecipe.Ch2TAResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch2TAResistanceToleranceLowerSpec", "40.0"));
            MeasurementTestRecipe.Ch3WHResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch3WHResistanceToleranceLowerSpec", "40.0"));
            MeasurementTestRecipe.Ch4RHResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch4RHResistanceToleranceLowerSpec", "40.0"));
            MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch5R1ResistanceToleranceLowerSpec", "40.0"));
            MeasurementTestRecipe.Ch6R2ResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch6R2ResistanceToleranceLowerSpec", "40.0"));
            MeasurementTestRecipe.Ch6LDUResistanceToleranceLowSpec = double.Parse(_xml.Read("Ch6LDUResistanceToleranceLowerSpec", "0.0"));
            _xml.CloseSection();

            _xml.OpenSection("BiasCurrentSupply");
            MeasurementTestRecipe.BiasCurrentCh1Writer = double.Parse(_xml.Read("BiasCurrentCh1Writer", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh2TA = double.Parse(_xml.Read("BiasCurrentCh2TA", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh3WH = double.Parse(_xml.Read("BiasCurrentCh3WH", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh4RH = double.Parse(_xml.Read("BiasCurrentCh4RH", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh5R1 = double.Parse(_xml.Read("BiasCurrentCh5R1", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh6R2 = double.Parse(_xml.Read("BiasCurrentCh6R2", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint = double.Parse(_xml.Read("BiasCurrentCh6LDU1stPoint", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint = double.Parse(_xml.Read("BiasCurrentCh6LDU2ndPoint", "0.0"));
            MeasurementTestRecipe.BiasCurrentCh6LDUStep = double.Parse(_xml.Read("BiasCurrentCh6LDUStep", "0.0"));
            MeasurementTestRecipe.BiasCurrentLEDCh6LDU1stPoint = double.Parse(_xml.Read("BiasCurrentLEDCh6LDU1stPoint", "0.0"));
            MeasurementTestRecipe.BiasCurrentLEDCh6LDU2ndPoint = double.Parse(_xml.Read("BiasCurrentLEDCh6LDU2ndPoint", "0.0"));
            MeasurementTestRecipe.BiasCurrentLEDCh6LDUStep = double.Parse(_xml.Read("BiasCurrentLEDCh6LDUStep", "0.0"));
            MeasurementTestRecipe.BiasCurrent3rdPointforIThreshold = double.Parse(_xml.Read("BiasCurrent3rdPointforIThreshold", "0.0"));
            MeasurementTestRecipe.BiasCurrent4thPointforIThreshold = double.Parse(_xml.Read("BiasCurrent4thPointforIThreshold", "0.0"));

            _xml.CloseSection();

            //Delta ISI
            _xml.OpenSection("DeltaISI");
            MeasurementTestRecipe.DeltaISI_Enable = true;
            MeasurementTestRecipe.DeltaISISpec1 = double.Parse(_xml.Read("DeltaISI_Reader1", "0.0"));
            MeasurementTestRecipe.DeltaISISpec2 = double.Parse(_xml.Read("DeltaISI_Reader2", "0.0"));
            MeasurementTestRecipe.DeltaISISpec1P = double.Parse(_xml.Read("DeltaISI_Reader1P", "0.0"));
            MeasurementTestRecipe.DeltaISISpec1N = double.Parse(_xml.Read("DeltaISI_Reader1N", "0.0"));
            MeasurementTestRecipe.DeltaISISpec2P = double.Parse(_xml.Read("DeltaISI_Reader2P", "0.0"));
            MeasurementTestRecipe.DeltaISISpec2N = double.Parse(_xml.Read("DeltaISI_Reader2N", "0.0"));
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "MeasurementTestRecipe.DeltaISI_Enable = {0}, MeasurementTestRecipe.DeltaISISpec1 = {1}, MeasurementTestRecipe.DeltaISISpec2 = {2}.", MeasurementTestRecipe.DeltaISI_Enable, MeasurementTestRecipe.DeltaISISpec1, MeasurementTestRecipe.DeltaISISpec2);
            }
            _xml.CloseSection();
            //Mar 2020 new salesforce 
            _xml.OpenSection("DeltaWriterHSTSDETForWriterBridging");
            MeasurementTestRecipe.MinDeltaWriter = double.Parse(_xml.Read("MinDeltaWriter", "0.0"));
            MeasurementTestRecipe.MaxDeltaWriter = double.Parse(_xml.Read("MaxDeltaWriter", "7.0"));
            MeasurementTestRecipe.Percentile = double.Parse(_xml.Read("Percentile", (HSTMachine.Workcell.HSTSettings.Sampledata.getCurrentZ * 100).ToString()));
            MeasurementTestRecipe.DeltaWriterUp = double.Parse(_xml.Read("DeltaWriterUp", "0.0"));
            MeasurementTestRecipe.DeltaWriterDn = double.Parse(_xml.Read("DeltaWriterDn", "0.0"));
            MeasurementTestRecipe.DistributionSample = double.Parse(_xml.Read("DistributionSample", HSTMachine.Workcell.HSTSettings.Sampledata.getSampleSize.ToString()));
            _xml.CloseSection();
            //Mar 2020 Set Sampling data
            HSTMachine.Workcell.HSTSettings._SamplingData.getSampleSize = double.Parse(MeasurementTestRecipe.DistributionSample.ToString());
            HSTMachine.Workcell.HSTSettings._SamplingData.getCurrentZ = double.Parse(MeasurementTestRecipe.Percentile.ToString())/100;
            HSTMachine.Workcell.HSTSettings._SamplingData.getHeightest = double.Parse(MeasurementTestRecipe.MaxDeltaWriter.ToString());
            HSTMachine.Workcell.HSTSettings._SamplingData.getLowest = double.Parse(MeasurementTestRecipe.MinDeltaWriter.ToString());
            HSTMachine.Workcell.HSTSettings._SamplingData.Save();

            _xml.OpenSection("SamplingET");
            MeasurementTestRecipe.SamplingETOnDisk = int.Parse(_xml.Read("SamplingETOnDisk", "0"));
            MeasurementTestRecipe.WriterETOnDisk = double.Parse(_xml.Read("WriterETOnDisk", "3.0"));
            _xml.CloseSection();

            MeasurementTestRecipe.EnableSortData = true; // for one recipe fix to be true

            _xml.OpenSection("RiskyPolarity");
            MeasurementTestRecipe.OffsetR1HSTSDET = double.Parse(_xml.Read("OffsetR1HSTSDET", "0.0"));
            MeasurementTestRecipe.DeltaR1SpecMoreThan = double.Parse(_xml.Read("DeltaR1SpecMoreThan", "0.0"));
            MeasurementTestRecipe.DeltaR1SpecLessThan = double.Parse(_xml.Read("DeltaR1SpecLessThan", "0.0"));
            MeasurementTestRecipe.OffsetR2HSTSDET = double.Parse(_xml.Read("OffsetR2HSTSDET", "0.0"));
            MeasurementTestRecipe.DeltaR2SpecMoreThan = double.Parse(_xml.Read("DeltaR2SpecMoreThan", "0.0"));
            MeasurementTestRecipe.DeltaR2SpecLessThan = double.Parse(_xml.Read("DeltaR2SpecLessThan", "0.0"));
            _xml.CloseSection();
            
            _xml.OpenSection("BiasCurrentSupply");
            // BiasCurrent
            ConfigurationSetupRecipe.Ch1BiasCurrent = int.Parse(_xml.Read("BiasCurrentCh1Writer", "0"));
            ConfigurationSetupRecipe.Ch2BiasCurrent = int.Parse(_xml.Read("BiasCurrentCh2TA", "0"));
            ConfigurationSetupRecipe.Ch3BiasCurrent = int.Parse(_xml.Read("BiasCurrentCh3WH", "0"));
            ConfigurationSetupRecipe.Ch4BiasCurrent = int.Parse(_xml.Read("BiasCurrentCh4RH", "0"));
            ConfigurationSetupRecipe.Ch5BiasCurrent = int.Parse(_xml.Read("BiasCurrentCh5R1", "0"));
            ConfigurationSetupRecipe.Ch6BiasCurrent = int.Parse(_xml.Read("BiasCurrentCh6R2", "0"));

            ConfigurationSetupRecipe.BiasCurrentCh6LDU1stPoint = int.Parse(_xml.Read("BiasCurrentCh6LDU1stPoint", "0"));//Isweep1
            ConfigurationSetupRecipe.BiasCurrentCh6LDU2ndPoint = int.Parse(_xml.Read("BiasCurrentCh6LDU2ndPoint", "0"));//Isweep2
            ConfigurationSetupRecipe.BiasCurrentCh6LDUStep = int.Parse(_xml.Read("BiasCurrentCh6LDUStep", "0"));
            //ykl
            ConfigurationSetupRecipe.BiasCurrent3rdPointforIThreshold = double.Parse(_xml.Read("BiasCurrent3rdPointforIThreshold", "0"));//Isweep3
            ConfigurationSetupRecipe.BiasCurrent4thPointforIThreshold = double.Parse(_xml.Read("BiasCurrent4thPointforIThreshold", "0"));//Isweep4

            ConfigurationSetupRecipe.BiasCurrentLEDCh6LDU1stPoint = double.Parse(_xml.Read("BiasCurrentLEDCh6LDU1stPoint", "0"));
            ConfigurationSetupRecipe.BiasCurrentLEDCh6LDU2ndPoint = double.Parse(_xml.Read("BiasCurrentLEDCh6LDU2ndPoint", "0"));
            ConfigurationSetupRecipe.BiasCurrentLEDCh6LDUStep = double.Parse(_xml.Read("BiasCurrentLEDCh6LDUStep", "0"));     

            _xml.CloseSection();
            //ykl
            _xml.OpenSection("LaserIThresholdSpec");
            MeasurementTestRecipe.PDVoltageMinSpec = _xml.Read("PDVoltageLower", 1.0);
            MeasurementTestRecipe.PDVoltageMaxSpec = _xml.Read("PDVoltageUpper", 2.0);            
            MeasurementTestRecipe.IThresholdSpecLower = _xml.Read("IThresholdSpecLower", 10.0);
            MeasurementTestRecipe.IThresholdSpecUpper = _xml.Read("IThresholdSpecUpper", 13.0);
            MeasurementTestRecipe.DeltaIThresholdNegativeSpec = _xml.Read("DeltaIThresholdNegativeSpec", 0.0);
            MeasurementTestRecipe.DeltaIThresholdPositiveSpec = _xml.Read("DeltaIThresholdPositiveSpec", 0.0);
            MeasurementTestRecipe.Gompertz_IThresholdSpecLower = _xml.Read("IThresholdSpecLower", 10.1);
            MeasurementTestRecipe.Gompertz_IThresholdSpecUpper = _xml.Read("IThresholdSpecUpper", 14.0);
            _xml.CloseSection();
            //ykl
            _xml.OpenSection("LEDIntercept");
            MeasurementTestRecipe.LEDInterceptSpecLower = _xml.Read("LEDInterceptSpecLower", 0.0);
            MeasurementTestRecipe.LEDInterceptSpecUpper = _xml.Read("LEDInterceptSpecUpper", 0.0);
            _xml.CloseSection();

            _xml.OpenSection("HSTHardwareInfo");
            MeasurementTestRecipe.RecipeTailType = _xml.Read("TailType", string.Empty);
            MeasurementTestRecipe.RecipeProbeType = _xml.Read("ProbeHeadType", string.Empty);
            _xml.CloseSection();

            _xml.OpenSection("ResistanceValueReport");
            MeasurementTestRecipe.MinReportSpec = _xml.Read("MinReport", 1.0);
            MeasurementTestRecipe.MaxReportSpec = _xml.Read("MaxReport", 1500.0);
            _xml.CloseSection();

            MeasurementTestRecipe.AdjacentPadsList = new AdjacentPadList(0);
            MeasurementTestRecipe.AdjacentPadsList.Load("AdjacentPad", _xml);

            MeasurementTestRecipe.SortGradingsList = new SortGradingList(8);
            MeasurementTestRecipe.SortGradingsList.Load("SortGrading", _xml);

            if (MeasurementTestRecipe.SamplingETOnDisk > 0)
                CalculateSamplingETOnDisk();
            //}
        }

        public void LoadConfigurationRecipe()
        {

            string getRecipefile = CommonFunctions.Instance.GetMachineConfigGlobalInfo;

            CheckConfigFile(getRecipefile);

            if (!File.Exists(getRecipefile)) 
            {
                throw new Exception("Cannot find machine configulation file!");
                return;
            }

            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Load configuration & setup recipe from {0} for Conversion Board ID:{1} and Product Recipe:{2}.", getRecipefile, ConversionBoardID, ProductName);
                }
            SettingsXml _xml = new SettingsXml(getRecipefile);

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

                _xml.OpenSection("LDU");
                ConfigurationSetupRecipe.LDUEnable = Convert.ToBoolean(_xml.Read("LDUEnable", "False").Equals("True"));
                _xml.CloseSection();

            _xml.OpenSection("SwapCH3ANDCH4");
            ConfigurationSetupRecipe.SwapEnable = _xml.Read("SwapEnable", false);

                _xml.CloseSection();

                _xml.OpenSection("SweepLDUMode");
                ConfigurationSetupRecipe.SweepEnable = _xml.Read("SweepEnable", false);
                ConfigurationSetupRecipe.Trend1StopPoint = _xml.Read("Trend1StopPoint", 4);
                ConfigurationSetupRecipe.Trend2StartPoint = _xml.Read("Trend2StartPoint", 6);
            _xml.CloseSection();

            //Cleare current setting
            byte columnIndex = 0;
                ConfigurationSetupRecipe.WPlusPairing = columnIndex;
                ConfigurationSetupRecipe.WMinusPairing = columnIndex;
                ConfigurationSetupRecipe.TAPlusPairing = columnIndex;
                ConfigurationSetupRecipe.TAMinusPairing = columnIndex;
                ConfigurationSetupRecipe.WHPlusPairing = columnIndex;
                ConfigurationSetupRecipe.WHMinusPairing = columnIndex;
                ConfigurationSetupRecipe.RHPlusPairing = columnIndex;
                ConfigurationSetupRecipe.RHMinusPairing = columnIndex;
                ConfigurationSetupRecipe.R1PlusPairing = columnIndex;
                ConfigurationSetupRecipe.R1MinusPairing = columnIndex;
                ConfigurationSetupRecipe.R2PlusPairing = columnIndex;
                ConfigurationSetupRecipe.R2MinusPairing = columnIndex;

                if (CommonFunctions.Instance.MeasurementTestRecipe.AdjacentPadsList != null)
                    foreach (var item in CommonFunctions.Instance.MeasurementTestRecipe.AdjacentPadsList.DataPadList)
                    {
                        int indexLoop = 0;
                        var unknownString = Enum.GetName(typeof(AdjacentPadTopic), AdjacentPadTopic.Unknown);
                        foreach (var data in Enum.GetValues(typeof(AdjacentPadTopic)))
                        {
                            var type = data.GetType();
                            string name = Enum.GetName(type, data);
                            if (name != null)
                            {
                                FieldInfo field = type.GetField(name);
                                if (field != null)
                                {
                                    DescriptionAttribute attr =
                                           Attribute.GetCustomAttribute(field,
                                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                                    if (attr != null)
                                    {
                                        if (attr.Description != unknownString)
                                        {
                                            indexLoop++;
                                            if (item.ColTopic == attr.Description)
                                                columnIndex = Convert.ToByte(indexLoop);
                                        }

                                    }
                                }
                            }
                        }

                        if (item.RowTopic == AdjacentPadTopic.W_Plush.GetEnumDescription())
                            ConfigurationSetupRecipe.WPlusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.W_Minus.GetEnumDescription())
                            ConfigurationSetupRecipe.WMinusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.TA_Plush.GetEnumDescription())
                            ConfigurationSetupRecipe.TAPlusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.TA_Minus.GetEnumDescription())
                            ConfigurationSetupRecipe.TAMinusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.WH_Plush.GetEnumDescription())
                            ConfigurationSetupRecipe.WHPlusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.WH_Minus.GetEnumDescription())
                            ConfigurationSetupRecipe.WHMinusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.RH_Plush.GetEnumDescription())
                            ConfigurationSetupRecipe.RHPlusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.RH_Minus.GetEnumDescription())
                            ConfigurationSetupRecipe.RHMinusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.R1_Plush.GetEnumDescription())
                            ConfigurationSetupRecipe.R1PlusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.R1_Minus.GetEnumDescription())
                            ConfigurationSetupRecipe.R1MinusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.R2_Plush.GetEnumDescription())
                            ConfigurationSetupRecipe.R2PlusPairing = columnIndex;
                        else if (item.RowTopic == AdjacentPadTopic.R2_Minus.GetEnumDescription())
                            ConfigurationSetupRecipe.R2MinusPairing = columnIndex;
                    }
            //}

            //Load function test recipe
            try
            {
                LoadFunctionalTestsRecipe();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void LoadFunctionalTestsRecipe()
        {
            string FunctionalTestsRecipeFilePath = "C:\\Seagate\\HGA.HST\\Recipes\\BenchTestTool\\FunctionalTestsRecipe.rcp";

            if (!File.Exists(FunctionalTestsRecipeFilePath))
            {
                Notify.PopUp("Load Recipe Error", "Functional test recipe file not found!", "", "OK");
                return;
            }

            if (File.Exists(FunctionalTestsRecipeFilePath))
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
                CommonFunctions.Instance.IsDoubleTestSpecLoaded = true;
            }
            else
            {
                CommonFunctions.Instance.IsDoubleTestSpecLoaded = false;
                string errormsg =
                    string.Format(
                        "File <FunctionalTestsRecipe.rcp> at <C:\\Seagate\\HGA.HST\\Recipes\\BenchTestTool\\> " +
                        "not found, this file is needed for machine performance test, please check befor running machine");
                throw new Exception(errormsg);
            }
        }

        public void LoadSpacialUser()
        {
            string getUserfile = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.SpacialUserGlobalPath + MeasurementTestSettings.SpacialUserFileName);

            if (!File.Exists(getUserfile))
            {
                return;
            }

            SettingsXml _xml = new SettingsXml(getUserfile);

            int index = 1;
            bool endloop = false;
            string gid = string.Empty;
            do
            {
                _xml.OpenSection("SpacialUser");
                gid = _xml.Read("User" + index, string.Empty);
                _xml.CloseSection();

                if (string.IsNullOrEmpty(gid))
                    endloop = true;
                else
                    CommonFunctions.Instance.SpacialUser.Add(gid);
                index++;
            } while (!endloop);

        }

        private void CalculateSamplingETOnDisk()
        {
            var percentFromRecipe = MeasurementTestRecipe.SamplingETOnDisk;
            var countPerCarrier = Decimal.Divide((percentFromRecipe * 10), 100);
            int carrierCount = 0;

            if (countPerCarrier < 1)
            {
                carrierCount = (int)(1 / countPerCarrier);
                countPerCarrier = 1;
            }
            else
            {
                carrierCount = 1;
            }

            CommonFunctions.Instance.ConfigurationSetupRecipe.CarrierCounterToSampling = carrierCount;
            CommonFunctions.Instance.ConfigurationSetupRecipe.CounterSamplingPerCarrier = (int)countPerCarrier;
        }
    }
}
