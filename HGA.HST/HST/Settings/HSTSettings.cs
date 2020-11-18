using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Models;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class HSTSettings
    {
        string _usersPath;
        public SettingsEditor _settingsEditor;
        protected SettingsDocument _settingsDoc;
        protected string _filePathUsers;
        protected string _filePathHSTConfig;
        protected string _filePathCalibration;
        protected string _filePathSetup;

        public delegate void OnJobStateChangedEventHandler(JobStates jobState);
        public event OnJobStateChangedEventHandler OnJobStateChanged;
        public EventHandler OnSettingsChanged;

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("User Access")]
        private UserAccessSettings UserAccess
        {
            get;
            set;
        }

        public UserAccessSettings getUserAccessSettings()
        {
            return UserAccess;
        }

        #region Singleton

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<HSTSettings> _instance
             = new Lazy<HSTSettings>(() => new HSTSettings());

        // accessor for instance
        public static HSTSettings Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public HSTSettings()
        {
            Install = new InstallSettings();
            Directory = new DirectorySettings();
            SimulatedPart = new SimulatedPartSettings();
            UserAccess = new UserAccessSettings();
            _directories = new Directories();
            ServiceManager.FormLayout.SetupFolder = Directories.Setup;
            _configPerformance = new ConfigPerformanceBase();
            _triggeringConfig = new TriggeringConfig();
            _cccParameterSetting = new CCCParameter();
            _cccCrdlParameterSetting = new CCCParameter();
            _resistanceCheckingConfig = new ResistanceCheckConfig();
            _configPerformance.Initialize();
            _configPerformance.StartTracking();
            _hstCCCCounter = new HSTCCCCounter();
            _crdlCCCCounter = new HSTCCCCounter();
            _SamplingData = new SamplingData();
            _ancsetting = new ANCSetting();
            Load();
        }

        #endregion

        public string HSTSettingsFilePath
        {
            get
            {
                return _filePathHSTConfig;
            }

            set
            {
                _filePathHSTConfig = value;
            }
        }

        public string CalibrationSettingsFilePath
        {
            get
            {
                return _filePathCalibration;
            }

            set
            {
                _filePathCalibration = value;
            }
        }

        public string UsersSettingsFilePath
        {
            get
            {
                return _filePathUsers;
            }

            set
            {
                _filePathUsers = value;
            }
        }

        public string SetupSettingsFilePath
        {
            get
            {
                return _filePathSetup;
            }

            set
            {
                _filePathSetup = value;
            }
        }

        public string UsersPath { get { return _usersPath; } }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Installation")]
        public InstallSettings Install
        {
            get;
            private set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Directories")]
        public DirectorySettings Directory
        {
            get;
            private set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("SimulatedPart")]
        public SimulatedPartSettings SimulatedPart
        {
            get;
            private set;
        }

        [ReadOnly(true)]
        [Browsable(false)]
        [DisplayName("SamplingData")]
        public SamplingData _SamplingData
        {
            get;
            private set;
        }

        public class Directories : Folders
        {
            public Directories()
            {
                ServiceManager.DirectoryLocator.RegisterPath(App, @"C:\Seagate\HGA.HST");
                ServiceManager.DirectoryLocator.RegisterPath(Bin, @"C:\Seagate\HGA.HST\Bin");
                ServiceManager.DirectoryLocator.RegisterPath(Doc, @"C:\Seagate\HGA.HST\Documents");
                ServiceManager.DirectoryLocator.RegisterPath(Setup, @"C:\Seagate\HGA.HST\Setup");
            }

            public static string AppDir { get { return ServiceManager.DirectoryLocator.GetPath(Directories.App); } }
            public static string BinDir { get { return ServiceManager.DirectoryLocator.GetPath(Directories.Bin); } }
            public static string DocDir { get { return ServiceManager.DirectoryLocator.GetPath(Directories.Doc); } }
            public static string SetupDir { get { return ServiceManager.DirectoryLocator.GetPath(Directories.Setup); } }
        }

        private Directories _directories;
        private SettingsXml _xml;

        private ConfigPerformanceBase _configPerformance;
        private TriggeringConfig _triggeringConfig;
        private ResistanceCheckConfig _resistanceCheckingConfig;
        private CCCParameter _cccParameterSetting;
        private CCCParameter _cccCrdlParameterSetting;
        private HSTCCCCounter _hstCCCCounter;
        private HSTCCCCounter _crdlCCCCounter;
        private ANCSetting _ancsetting;

        public ConfigPerformanceBase getConfigPerformance()
        {
            return _configPerformance;
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public TriggeringConfig TriggeringSetting
        {
            get { return _triggeringConfig; }
        }

        public CCCParameter CccParameterSetting
        {
            get { return _cccParameterSetting; }
        }

        public CCCParameter CccCRDLParameterSetting
        {
            get { return _cccCrdlParameterSetting; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public ResistanceCheckConfig ResistanceCheckingConfig
        {
            get { return _resistanceCheckingConfig; }
        }

        private string _lastRunWorkOrderServerFileName = "";
        public string LastRunWorkOrderServerFileName
        {
            get { return _lastRunWorkOrderServerFileName; }
            set { _lastRunWorkOrderServerFileName = value; }
        }

        public bool iSTDFGlobalDriveConnected { get; set; }

        public HSTCCCCounter TicCCCCounter { get { return _hstCCCCounter; } set { _hstCCCCounter = value; } }

        public HSTCCCCounter CRDLCccCounter { get { return _crdlCCCCounter; } set { _crdlCCCCounter = value; } }
        #region Seatrack

        private string _equipType = "ZHS";//Display Worcell Information
        public string EquipmentType
        {
            get { return _equipType; }
            set { _equipType = value; }
        }


        private int _processID = 0;//Display Worcell Information
        public int ProcessID
        {
            get { return _processID; }
            set { _processID = value; }
        }

        private string _operatorGID = "";//Can keep Employee ID
        public string OperatorGID
        {
            get { return _operatorGID; }
            set { _operatorGID = value; }
        }

        public bool TurnOnTestRunWithoutData
        {
            get;
            set;
        }

        #endregion

        private JobStates _state = JobStates.OutJob;
        public JobStates State   //InJob-Machine still not fully flushed (lot not ended), OutJob-Can change work order and new lot
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    if (OnJobStateChanged != null)
                        OnJobStateChanged(_state);
                }
            }
        }
        public void LoadUsers()
        {
            _usersPath = this.UsersSettingsFilePath;

            _settingsDoc = new SettingsDocument();
            _settingsDoc.Load(_usersPath, SettingsFileOption.Encrypted);
            SettingsConverter.UpdateObjectFromNode(this.getUserAccessSettings(), _settingsDoc);
        }
        public SamplingData Sampledata
        {
            get { return _SamplingData; }
        }

        public ANCSetting ANCSettings
        {
            get { return _ancsetting; }
        }

        public void Load()
        {
            if (this.HSTSettingsFilePath == null)
            {
                return;
            }

            if (!File.Exists(this.HSTSettingsFilePath))
            {
                File.Create(this.HSTSettingsFilePath).Dispose();
                return;
            }

            LoadUsers();

            _xml = new SettingsXml(this.HSTSettingsFilePath);
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

            OperatorGID = _xml.Read("OperatorGID", CommonFunctions.UNKNOWN);
            _xml.CloseSection();

            _xml.OpenSection("Install");
            Install.EquipmentID = _xml.Read("EquipmentID", CommonFunctions.UNKNOWN);
            Install.LocationID = _xml.Read("LocationID", CommonFunctions.UNKNOWN);
            Install.CellID = _xml.Read("CellID", CommonFunctions.UNKNOWN);
            Install.Factory = (FactoryList)Enum.Parse(typeof(FactoryList), _xml.Read("Factory", FactoryList.Unknown.ToString()));
            Install.AudibleAlarmEnabled = _xml.Read("AudibleAlarmEnabled", false);
            Install.DataLoggingFileSavingEnabled = _xml.Read("DataLoggingFileSavingEnabled", true);
            Install.ProcessStepCheckingEnabled = _xml.Read("ProcessStepCheckingEnabled", false);
            Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled = _xml.Read("DataLoggingForRFIDAndSeatrackRecordUpdateEnabled", false);
            Install.SeatrackRecordUpdateEnabled = _xml.Read("SeatrackRecordUpdateEnabled", false);
            Install.ClearImproperShutDownMessage = _xml.Read("ClearImproperShutDownMessage", false);
            Install.EnableDebugLog = _xml.Read("EnableDebugLog", false);
            Install.EnableRunTestScriptButton = _xml.Read("EnableRunTestScriptButton", false);
            Install.TestScript = _xml.Read("TestScript", CommonFunctions.UNKNOWN);
            Install.LogIOChangeState = _xml.Read("LogIOChangeState", true);
            Install.HGADetectionUsingVision = _xml.Read("HGADetectionUsingVision", true);
            Install.EnableVision = _xml.Read("EnableVision", true);
            Install.OperationMode = (OperationMode)Enum.Parse(typeof(OperationMode), _xml.Read("OperationMode", OperationMode.Auto.ToString()));
            Install.EnabledUserAccessControl = _xml.Read("EnabledUserAccessControl", true);
            Install.DataRecordDisplayCounter = _xml.Read("DataRecordDisplayCounter", 500);
            Install.ANCGraphCounterMaximum = _xml.Read("ANCGraphCounterMaximum", 3000);
            // Disable Simulation if Vision is enabled
            if (Install.EnableVision == true)
            {
                if (Install.OperationMode == OperationMode.Simulation)
                {
                    Install.OperationMode = OperationMode.Auto;
                }
            }
            Install.ConveyorCongestionToleranceTimeLimit = _xml.Read("ConveyorCongestionToleranceTimeLimit", 0);
            Install.MeasurementTestTimeOutLimit = _xml.Read("MeasurementTestTimeOutLimit", 0);
            Install.HGAPurgingDurationInms = _xml.Read("HGAPurgingDurationInms", 0);
            Install.TICFailHGAsInBoatLimit = _xml.Read("TICFailHGAsInBoatLimit", 0);
            Install.TICConsecutiveFailBoatsLimit = _xml.Read("TICConsecutiveFailBoatsLimit", 0);
            Install.TICFailHGAsTotalLimit = _xml.Read("TICFailHGAsTotalLimit", 0);
            Install.TICErrorCountingTimeInterval = _xml.Read("TICErrorCountingTimeInterval", 0);
            Install.ConsecutiveFailBoatsFailPickupByInputEE = _xml.Read("ConsecutiveFailBoatsFailPickupByInputEE", 0);
            Install.FlattenerDeployBeforePrecisorVaccumON = _xml.Read("FlattenerDeployBeforePrecisorVaccumON", true);
            Install.FlattenerDeployDuration = _xml.Read("FlattenerDeployDuration", 50);
            Install.EnableFlattenerAsPrecisor = _xml.Read("EnableFlattener", true);
            Install.EnableFlattenerAsInput = _xml.Read("EnableFlattenerAsInput", true);
            Install.EnableMaintenanceSpeedForManualMove = _xml.Read("EnableMaintenanceSpeedForManualMove", true);
            Install.RFIDUpdateOption = (RFIDUpdateOption)Enum.Parse(typeof(RFIDUpdateOption), _xml.Read("RFIDUpdateOption", RFIDUpdateOption.UpdateALL.ToString()));
            Install.InputEETouchingOnDycemDuration = _xml.Read("InputEETouchingOnDycemDuration", 1);
            Install.OutputEETouchingOnDycemDuration = _xml.Read("OutputEETouchingOnDycemDuration", 1);
            Install.TotalNumberOfInputEETouchingOnDycem = _xml.Read("TotalNumberOfInputEETouchingOnDycem", 1);
            Install.TotalNumberOfOutputEETouchingOnDycem = _xml.Read("TotalNumberOfOutputEETouchingOnDycem", 1);
            Install.CapacitanceTestSamplingSize = _xml.Read("CapacitanceTestSamplingSize", 0);
            Install.EnabledTDFFileSystem = _xml.Read("EnabledTDFSystem", false);
            Install.EnabledSaveTDFFileOnly = _xml.Read("EnabledSaveTDFFileOnly", false);
            Install.EnabledSaveTDFBackupFile = _xml.Read("EnabledSaveTDFBackupFile", false);
            _xml.CloseSection();

            _xml.OpenSection("Directories");
            Directory.WorkorderLocalPath = _xml.Read("LocalRecipePath", CommonFunctions.UNKNOWN);
            Directory.WorkorderGlobalPath = _xml.Read("GlobalRecipePath", CommonFunctions.UNKNOWN);
            Directory.LogFilePath = _xml.Read("LogFilePath", "C:\\Seagate\\HGA.HST\\Log");
            Directory.ErrorHandlingPath = _xml.Read("ErrorHandlingPath", CommonFunctions.UNKNOWN);
            Directory.TSRRecipLocalPath = _xml.Read("WorkOrderPath", CommonFunctions.UNKNOWN);
            Directory.DataPath = _xml.Read("DataPath", "C:\\Seagate\\HGA.HST\\Data");
            Directory.TSRRecipeGlobalPath = _xml.Read("ServerDirectoryForWorkOrder", CommonFunctions.UNKNOWN);
            Directory.IOChangeStatePath = _xml.Read("IOChangeStatePath", CommonFunctions.UNKNOWN);
            Directory.McConfigGlobalPath = _xml.Read("McConfigGlobalPath", CommonFunctions.UNKNOWN);
            Directory.MachineRobotPointPath = _xml.Read("MachineRobotPointPath", CommonFunctions.UNKNOWN);
            if (Directory.McConfigGlobalPath != CommonFunctions.UNKNOWN)
            {
                var split = Directory.McConfigGlobalPath.Split('\\');
                string product = string.Empty;
                string globalPath = string.Empty;

                if (split.Length > 1)
                {
                    globalPath = System.IO.Path.Combine(@"" + split[0] + "\\" + split[1] + "\\" + split[2] + "\\");
                }
                Directory.SpacialUserGlobalPath = System.IO.Path.Combine(@"" + globalPath + "Spacial_User" + "\\");
            }

            if (Directory.WorkorderLocalPath == CommonFunctions.UNKNOWN) Directory.WorkorderLocalPath = "C:\\Seagate\\HGA.HST\\Recipes";
            if (Directory.WorkorderGlobalPath == CommonFunctions.UNKNOWN) Directory.WorkorderGlobalPath = "N:\\HGAWorkOrder\\Released";
            if (Directory.LogFilePath == CommonFunctions.UNKNOWN) Directory.LogFilePath = "C:\\Seagate\\HGA.HST\\Log";
            if (Directory.DataPath == CommonFunctions.UNKNOWN) Directory.DataPath = "C:\\Seagate\\HGA.HST\\Data";
            if (Directory.SpacialUserGlobalPath == CommonFunctions.UNKNOWN) Directory.SpacialUserGlobalPath = "C:\\Seagate\\HGA.HST\\";

            _xml.CloseSection();

            _xml.OpenSection("SimulatedPart");
            SimulatedPart.Carriers.Clear();

            for (int i = 0; ; i++)
            {
                string CurrentCarrierLabel = "Carrier" + i;
                CarrierSettings CurrentCarrier = new CarrierSettings();
                CurrentCarrier.CarrierID = _xml.Read(CurrentCarrierLabel + "CarrierID", CommonFunctions.UNKNOWN);
                CurrentCarrier.Name = _xml.Read(CurrentCarrierLabel + "Name", CommonFunctions.UNKNOWN);
                CurrentCarrier.ImageFileName = _xml.Read(CurrentCarrierLabel + "ImageFileName", CommonFunctions.UNKNOWN);
                CurrentCarrier.RFIDFileName = _xml.Read(CurrentCarrierLabel + "RFIDFileName", CommonFunctions.UNKNOWN);
                CurrentCarrier.IsPassThroughMode = _xml.Read(CurrentCarrierLabel + "IsPassThroughMode", false);

                CurrentCarrier.Hga1.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA1Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga2.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA2Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga3.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA3Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga4.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA4Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga5.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA5Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga6.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA6Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga7.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA7Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga8.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA8Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga9.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA9Status", CommonFunctions.UNKNOWN));
                CurrentCarrier.Hga10.Hga_Status = (HGAStatus)Enum.Parse(typeof(HGAStatus), _xml.Read(CurrentCarrierLabel + "HGA10Status", CommonFunctions.UNKNOWN));

                if (String.Compare(CurrentCarrier.Name, CommonFunctions.UNKNOWN, true) != 0)
                {
                    SimulatedPart.Carriers.Add(CurrentCarrier);
                }
                else
                {
                    break;
                }
            }

            _xml.CloseSection();

            _xml.OpenSection("Bypass");
            Install.BypassInputAndOutputEEsPickAndPlace = _xml.Read("BypassInputAndOutputEEsPickAndPlace", false);
            Install.BypassMeasurementTestAtTestProbe = _xml.Read("BypassMeasurementTestAtTestProbe", false);
            Install.BypassRFIDReadAtInput = _xml.Read("BypassRFIDReadAtInput", false);
            Install.BypassRFIDAndSeatrackWriteAtOutput = _xml.Read("BypassRFIDAndSeatrackWriteAtOutput", false);
            Install.BypassVisionAtInputTurnStation = _xml.Read("BypassVisionAtInputTurnStation", false);
            Install.BypassVisionAtOutput = _xml.Read("BypassVisionAtOutput", false);
            Install.WorkOrderFilePath = _xml.Read("WorkOrderFilePath", CommonFunctions.UNKNOWN);
            _xml.CloseSection();

            _xml.OpenSection("DryRun");
            Install.DryRunWithoutBoat = _xml.Read("DryRunWithoutBoat", false);
            _xml.CloseSection();

            _configPerformance.Load("PerformanceConfig", _xml);

            _SamplingData.Load("SamplingData", _xml);
            _ancsetting.Load("Ancsetting", _xml);
            _triggeringConfig.Load("TriggeringConfig", _xml);
            _resistanceCheckingConfig.Load("ResistanceChecking", _xml);
            _cccParameterSetting.Load("CCCParameterSetting", _xml);
            _cccCrdlParameterSetting.Load("CccCrdlParameterSetting", _xml);
            _hstCCCCounter.Load("TicCounter", _xml);
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.uTICMachineName = _ancsetting.uTICMachineName1;
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.uTICMachineName = _ancsetting.uTICMachineName2;


            //End Bypass hstCCCCounter
            //---------------------------------------------------------------------
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.TicFailCounter = int.Parse(_ancsetting.All_TicFailCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.HstFailCounter = int.Parse(_ancsetting.All_HstFailCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.TicGoodPartCounter = int.Parse(_ancsetting.All_TicGoodPartCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.HstGoodPartCounter = int.Parse(_ancsetting.All_HstGoodPartCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.AdaptivePartRunCounter = int.Parse(_ancsetting.All_AdaptivePartRunCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.AdaptiveDefectCounter = int.Parse(_ancsetting.All_AdaptiveDefectCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.LastSaveLogTime = DateTime.ParseExact(_ancsetting.All_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.MCDownTriggering = int.Parse(_ancsetting.All_MCDownTriggering.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.TicHighPercentTriggeringCounter = int.Parse(_ancsetting.All_TicHighPercentTriggeringCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.HstHighPercentTriggeringCounter = int.Parse(_ancsetting.All_HstHighPercentTriggeringCounter.ToString());
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.InternalTriggerCounter = int.Parse(_ancsetting.All_InternalTriggerCounter.ToString());

            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.TicFailCounter = int.Parse(_ancsetting.MC1_TicFailCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.HstFailCounter = int.Parse(_ancsetting.MC1_HstFailCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.TicGoodPartCounter = int.Parse(_ancsetting.MC1_TicGoodPartCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.HstGoodPartCounter = int.Parse(_ancsetting.MC1_HstGoodPartCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.AdaptivePartRunCounter = int.Parse(_ancsetting.MC1_AdaptivePartRunCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.AdaptiveDefectCounter = int.Parse(_ancsetting.MC1_AdaptiveDefectCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.LastSaveLogTime = DateTime.ParseExact(_ancsetting.MC1_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.MCDownTriggering = int.Parse(_ancsetting.MC1_MCDownTriggering.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.TicHighPercentTriggeringCounter = int.Parse(_ancsetting.MC1_TicHighPercentTriggeringCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.HstHighPercentTriggeringCounter = int.Parse(_ancsetting.MC1_HstHighPercentTriggeringCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.InternalTriggerCounter = int.Parse(_ancsetting.MC1_InternalTriggerCounter.ToString());

            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.TicFailCounter = int.Parse(_ancsetting.MC2_TicFailCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.HstFailCounter = int.Parse(_ancsetting.MC2_HstFailCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.TicGoodPartCounter = int.Parse(_ancsetting.MC2_TicGoodPartCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.HstGoodPartCounter = int.Parse(_ancsetting.MC2_HstGoodPartCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.AdaptivePartRunCounter = int.Parse(_ancsetting.MC2_AdaptivePartRunCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.AdaptiveDefectCounter = int.Parse(_ancsetting.MC2_AdaptiveDefectCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.LastSaveLogTime = DateTime.ParseExact(_ancsetting.MC2_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.MCDownTriggering = int.Parse(_ancsetting.MC2_MCDownTriggering.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.TicHighPercentTriggeringCounter = int.Parse(_ancsetting.MC2_TicHighPercentTriggeringCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.HstHighPercentTriggeringCounter = int.Parse(_ancsetting.MC2_HstHighPercentTriggeringCounter.ToString());
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.InternalTriggerCounter = int.Parse(_ancsetting.MC2_InternalTriggerCounter.ToString());

            //---------------------------------------------------------------------
            //End of Bypass hstCCCCounter
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
        }
        public void saveCCC()
        {
            _xml = new SettingsXml(this.HSTSettingsFilePath);
            _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst = HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter;
            _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter;
            _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst = HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter;

            _ancsetting.All_TicFailCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.TicFailCounter.ToString());
            _ancsetting.All_HstFailCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.HstFailCounter.ToString());
            _ancsetting.All_TicGoodPartCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.TicGoodPartCounter.ToString());
            _ancsetting.All_HstGoodPartCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.HstGoodPartCounter.ToString());
            _ancsetting.All_AdaptivePartRunCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.AdaptivePartRunCounter.ToString());
            _ancsetting.All_AdaptiveDefectCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.AdaptiveDefectCounter.ToString());
            _ancsetting.All_LastSaveLogTime = DateTime.ParseExact(System.DateTime.Now.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            _ancsetting.All_MCDownTriggering = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.MCDownTriggering.ToString()); int.Parse(_ancsetting.All_MCDownTriggering.ToString());
            _ancsetting.All_TicHighPercentTriggeringCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.TicHighPercentTriggeringCounter.ToString());
            _ancsetting.All_HstHighPercentTriggeringCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.HstHighPercentTriggeringCounter.ToString());
            _ancsetting.All_InternalTriggerCounter = double.Parse(_hstCCCCounter.CCCCounterAllMc.CCCCounterForHst.InternalTriggerCounter.ToString());

            _ancsetting.MC1_TicFailCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.TicFailCounter.ToString());
            _ancsetting.MC1_HstFailCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.HstFailCounter.ToString());
            _ancsetting.MC1_TicGoodPartCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.TicGoodPartCounter.ToString());
            _ancsetting.MC1_HstGoodPartCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.HstGoodPartCounter.ToString());
            _ancsetting.MC1_AdaptivePartRunCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.AdaptivePartRunCounter.ToString());
            _ancsetting.MC1_AdaptiveDefectCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.AdaptiveDefectCounter.ToString());
            _ancsetting.MC1_LastSaveLogTime = DateTime.ParseExact(System.DateTime.Now.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            _ancsetting.MC1_MCDownTriggering = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.MCDownTriggering.ToString()); int.Parse(_ancsetting.MC1_MCDownTriggering.ToString());
            _ancsetting.MC1_TicHighPercentTriggeringCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.TicHighPercentTriggeringCounter.ToString());
            _ancsetting.MC1_HstHighPercentTriggeringCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.HstHighPercentTriggeringCounter.ToString());
            _ancsetting.MC1_InternalTriggerCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst.InternalTriggerCounter.ToString());

            _ancsetting.MC2_TicFailCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.TicFailCounter.ToString());
            _ancsetting.MC2_HstFailCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.HstFailCounter.ToString());
            _ancsetting.MC2_TicGoodPartCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.TicGoodPartCounter.ToString());
            _ancsetting.MC2_HstGoodPartCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.HstGoodPartCounter.ToString());
            _ancsetting.MC2_AdaptivePartRunCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.AdaptivePartRunCounter.ToString());
            _ancsetting.MC2_AdaptiveDefectCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.AdaptiveDefectCounter.ToString());
            _ancsetting.MC2_LastSaveLogTime = DateTime.ParseExact(System.DateTime.Now.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            _ancsetting.MC2_MCDownTriggering = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.MCDownTriggering.ToString()); int.Parse(_ancsetting.MC2_MCDownTriggering.ToString());
            _ancsetting.MC2_TicHighPercentTriggeringCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.TicHighPercentTriggeringCounter.ToString());
            _ancsetting.MC2_HstHighPercentTriggeringCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.HstHighPercentTriggeringCounter.ToString());
            _ancsetting.MC2_InternalTriggerCounter = double.Parse(_hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst.InternalTriggerCounter.ToString());


            _hstCCCCounter.Save("TicCounter", _xml);
            _ancsetting.Save();
            _xml.Save();

        }
        public void Save()
        {
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
            if (string.Compare(Directory.TSRRecipeGlobalPath, Directory.TSRRecipLocalPath) == 0)
            {
                Notify.PopUp("WorkOrder Error", "WorkOrder global path and local path should not be the same location, please check directory setting!", "", "OK");
                return;
            }
            File.Create(this.HSTSettingsFilePath).Dispose();

            _xml = new SettingsXml(this.HSTSettingsFilePath);
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

            _xml.Write("OperatorGID", OperatorGID);
            _xml.CloseSection();
            // Any other types goes here.

            _xml.OpenSection("Install");
            _xml.Write("EquipmentID", Install.EquipmentID);
            _xml.Write("LocationID", Install.LocationID);
            _xml.Write("CellID", Install.CellID);
            _xml.Write("Factory", Install.Factory.ToString());

            if (Install.LocationID.Length > 7) Install.LocationID = Install.LocationID.Substring(0, 7);
            if (Install.CellID.Length > 7) Install.CellID = Install.CellID.Substring(0, 7);
            // Disable simulation if vision is enabled
            if (Install.EnableVision == true)
            {
                if (Install.OperationMode == OperationMode.Simulation)
                {
                    Install.OperationMode = OperationMode.Auto;
                    HSTMachine.Workcell.OperationMode = OperationMode.Auto;
                }
            }

            _xml.Write("OperationMode", HSTMachine.Workcell.OperationMode.ToString());
            _xml.Write("EnableDebugLog", Install.EnableDebugLog);
            _xml.Write("AudibleAlarmEnabled", Install.AudibleAlarmEnabled);
            _xml.Write("DataLoggingFileSavingEnabled", Install.DataLoggingFileSavingEnabled);
            _xml.Write("ProcessStepCheckingEnabled", Install.ProcessStepCheckingEnabled);
            _xml.Write("DataLoggingForRFIDAndSeatrackRecordUpdateEnabled", Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled);
            _xml.Write("SeatrackRecordUpdateEnabled", Install.SeatrackRecordUpdateEnabled);
            _xml.Write("ClearImproperShutDownMessage", Install.ClearImproperShutDownMessage);
            _xml.Write("EnableRunTestScriptButton", Install.EnableRunTestScriptButton);
            _xml.Write("TestScript", Install.TestScript);
            _xml.Write("LogIOChangeState", Install.LogIOChangeState);
            _xml.Write("HGADetectionUsingVision", Install.HGADetectionUsingVision);
            _xml.Write("EnableVision", Install.EnableVision);
            _xml.Write("ConveyorCongestionToleranceTimeLimit", Install.ConveyorCongestionToleranceTimeLimit);
            _xml.Write("MeasurementTestTimeOutLimit", Install.MeasurementTestTimeOutLimit);
            _xml.Write("HGAPurgingDurationInms", Install.HGAPurgingDurationInms);
            _xml.Write("TICFailHGAsInBoatLimit", Install.TICFailHGAsInBoatLimit);
            _xml.Write("TICConsecutiveFailBoatsLimit", Install.TICConsecutiveFailBoatsLimit);
            _xml.Write("TICFailHGAsTotalLimit", Install.TICFailHGAsTotalLimit);
            _xml.Write("TICErrorCountingTimeInterval", Install.TICErrorCountingTimeInterval);
            _xml.Write("ConsecutiveFailBoatsFailPickupByInputEE", Install.ConsecutiveFailBoatsFailPickupByInputEE);
            _xml.Write("FlattenerDeployBeforePrecisorVaccumON", Install.FlattenerDeployBeforePrecisorVaccumON);
            _xml.Write("FlattenerDeployDuration", Install.FlattenerDeployDuration);
            _xml.Write("EnableFlattener", Install.EnableFlattenerAsPrecisor);
            _xml.Write("EnableFlattenerAsInput", Install.EnableFlattenerAsInput);
            _xml.Write("EnableMaintenanceSpeedForManualMove", Install.EnableMaintenanceSpeedForManualMove);
            _xml.Write("RFIDUpdateOption", Install.RFIDUpdateOption.ToString());
            _xml.Write("InputEETouchingOnDycemDuration", Install.InputEETouchingOnDycemDuration);
            _xml.Write("OutputEETouchingOnDycemDuration", Install.OutputEETouchingOnDycemDuration);
            _xml.Write("TotalNumberOfInputEETouchingOnDycem", Install.TotalNumberOfInputEETouchingOnDycem);
            _xml.Write("TotalNumberOfOutputEETouchingOnDycem", Install.TotalNumberOfOutputEETouchingOnDycem);
            _xml.Write("CapacitanceTestSamplingSize", Install.CapacitanceTestSamplingSize);
            _xml.Write("EnabledTDFSystem", Install.EnabledTDFFileSystem);
            _xml.Write("EnabledSaveTDFFileOnly", Install.EnabledSaveTDFFileOnly);
            _xml.Write("EnabledSaveTDFBackupFile", Install.EnabledSaveTDFBackupFile);
            _xml.Write("EnabledUserAccessControl", Install.EnabledUserAccessControl);
            _xml.Write("DataRecordDisplayCounter", Install.DataRecordDisplayCounter);
            _xml.Write("ANCGraphCounterMaximum", Install.ANCGraphCounterMaximum);

            _xml.CloseSection();


            _xml.OpenSection("Directories");
            _xml.Write("LocalRecipePath", Directory.WorkorderLocalPath);
            _xml.Write("GlobalRecipePath", Directory.WorkorderGlobalPath);
            _xml.Write("LogFilePath", Directory.LogFilePath);
            _xml.Write("WorkOrderPath", Directory.TSRRecipLocalPath);
            _xml.Write("DataPath", Directory.DataPath);
            _xml.Write("ErrorHandlingPath", Directory.ErrorHandlingPath);
            _xml.Write("ServerDirectoryForWorkOrder", Directory.TSRRecipeGlobalPath);
            _xml.Write("IOChangeStatePath", Directory.IOChangeStatePath);
            _xml.Write("McConfigGlobalPath", Directory.McConfigGlobalPath);
            _xml.Write("MachineRobotPointPath", Directory.MachineRobotPointPath);

            _xml.CloseSection();

            _xml.OpenSection("SimulatedPart");

            int i = 0;
            foreach (CarrierSettings CurrentCarrier in SimulatedPart.Carriers)
            {
                string CurrentCarrierLabel = "Carrier" + (i++);

                _xml.Write(CurrentCarrierLabel + "CarrierID", CurrentCarrier.CarrierID);
                _xml.Write(CurrentCarrierLabel + "Name", CurrentCarrier.Name);
                _xml.Write(CurrentCarrierLabel + "ImageFileName", CurrentCarrier.ImageFileName);
                _xml.Write(CurrentCarrierLabel + "RFIDFileName", CurrentCarrier.RFIDFileName);
                _xml.Write(CurrentCarrierLabel + "IsPassThroughMode", CurrentCarrier.IsPassThroughMode);
                _xml.Write(CurrentCarrierLabel + "HGA1Status", CurrentCarrier.Hga1.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA2Status", CurrentCarrier.Hga2.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA3Status", CurrentCarrier.Hga3.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA4Status", CurrentCarrier.Hga4.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA5Status", CurrentCarrier.Hga5.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA6Status", CurrentCarrier.Hga6.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA7Status", CurrentCarrier.Hga7.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA8Status", CurrentCarrier.Hga8.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA9Status", CurrentCarrier.Hga9.Hga_Status.ToString());
                _xml.Write(CurrentCarrierLabel + "HGA10Status", CurrentCarrier.Hga10.Hga_Status.ToString());
            }

            _xml.CloseSection();


            Install.BypassInputAndOutputEEsPickAndPlace = HSTMachine.Workcell.getPanelSetup().chkBypassInputAndOutputEEsPickAndPlace.Checked;
            Install.BypassMeasurementTestAtTestProbe = HSTMachine.Workcell.getPanelSetup().chkBypassMeasurementTestAtTestProbe.Checked;
            Install.BypassRFIDReadAtInput = HSTMachine.Workcell.getPanelSetup().chkBypassRFIDReadAtInputStation.Checked;
            Install.BypassRFIDAndSeatrackWriteAtOutput = HSTMachine.Workcell.getPanelSetup().chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked;
            Install.BypassVisionAtInputTurnStation = HSTMachine.Workcell.getPanelSetup().chkBypassVisionAtInputTurnStation.Checked;
            Install.BypassVisionAtOutput = HSTMachine.Workcell.getPanelSetup().chkBypassVisionAtOutputStation.Checked;
            Install.DryRunWithoutBoat = HSTMachine.Workcell.getPanelSetup().rdoWithoutBoat.Checked;

            _xml.OpenSection("Bypass");
            _xml.Write("BypassInputAndOutputEEsPickAndPlace", Install.BypassInputAndOutputEEsPickAndPlace);
            _xml.Write("BypassMeasurementTestAtTestProbe", Install.BypassMeasurementTestAtTestProbe);
            _xml.Write("BypassRFIDReadAtInput", Install.BypassRFIDReadAtInput);
            _xml.Write("BypassRFIDAndSeatrackWriteAtOutput", Install.BypassRFIDAndSeatrackWriteAtOutput);
            _xml.Write("BypassVisionAtInputTurnStation", Install.BypassVisionAtInputTurnStation);
            _xml.Write("BypassVisionAtOutput", Install.BypassVisionAtOutput);
            _xml.Write("WorkOrderFilePath", Install.WorkOrderFilePath);
            _xml.CloseSection();

            _xml.OpenSection("DryRun");
            _xml.Write("DryRunWithoutBoat", Install.DryRunWithoutBoat);
            _xml.CloseSection();

            _xml.CloseSection();

            _configPerformance.Save("PerformanceConfig", _xml);
            _triggeringConfig.Save("TriggeringConfig", _xml);
            _resistanceCheckingConfig.Save("ResistanceChecking", _xml);
            _cccParameterSetting.Save("CCCParameterSetting", _xml);
            _cccCrdlParameterSetting.Save("CccCrdlParameterSetting", _xml);

            if(HSTMachine.Workcell.TICCccControl != null)
            {
                _hstCCCCounter.CCCCounterAllMc.CCCCounterForHst = HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter;
                _hstCCCCounter.CCCCounterTicMc1.CCCCounterForHst = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter;
                _hstCCCCounter.CCCCounterTicMc2.CCCCounterForHst = HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter;
                _hstCCCCounter.Save("TicCounter", _xml);
            }
            _xml.Save();

            Install.OperationMode = HSTMachine.Workcell.OperationMode;
            HSTMachine.Workcell.RunMode = HSTMachine.Workcell.OperationMode;
            HSTMachine.Instance.MainForm.getPanelCommand().DebugButtonsVisibility();
        }

        public void SaveSettingsToFile(object sender, EventArgs e)
        {
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
