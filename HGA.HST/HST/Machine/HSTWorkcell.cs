using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.UI.Main;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using System.Windows.Forms;
using System.Reflection;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Recipe;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.Utils;
using System.Threading;
using qf4net;
using Seagate.AAS.Parsel.Hw.Aerotech.A3200;
using Seagate.AAS.Parsel.Device.SafetyController;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using System.Xml;

namespace Seagate.AAS.HGA.HST.Machine
{
    public class HSTWorkcell : Workcell, IWorkcell
    {
        PanelDiagnostics _panelDiagnostics;
        PanelData _panelData;
        PanelOperation _panelOperation;
        PanelSetup _panelSetup;
        PanelRecipe _panelRecipe;
        PanelHelp _panelHelp;

        HSTSettings _HSTSettings;
        CalibrationSettings _CalibrationSettings;
        AutomationConfigSettings _SetupSettings;
        GompertzSettings gompertzSettings;
        public OperationMode _runMode;
        protected ProcessHST _process;
        public event EventHandler Aborted;
        public WorkOrderInfo _workOrder;
        private ProductionCounterData loadCounter;
        private DycemCleaningCounterData dycemCleaningCounter;
        private ARAMSStateHST _aramsState;
        private TeachPointRecipe _teachPointRecipe;
        public CSVFileOutput csvFileOutput;
        private DataLog _dataLog;
        public HSTIOManifest _ioManifest;
        protected Hashtable activeProcessCollector;
        protected Hashtable activeProcesses = new Hashtable();
        protected bool _machineHomed;
        protected bool _inputEEHomed;
        protected bool _outputEEHomed;
        protected bool _testProbeHomed;
        protected bool _precisorNestInInputStation;
        protected bool _precisorNestInWorkStation;
        protected bool _precisorNestInOutputStation;
        protected bool _inputEEIsMovingDown;
        protected bool _outputEEIsMovingDown;
        protected bool _testProbeEEIsMovingDown;
        protected bool _precisorNestIsMovingLeft;
        protected bool _precisorNestIsMovingRight;
        protected bool _carrierInBufferZone = false;
        protected bool _carrierInOutputTurnTable = false;
        private int _outputLeftOverNestError = 0;
        private int _goodRunningCounter = 0;
        private HGAProductTailType _hgaTailType = HGAProductTailType.Unknown;
        private MessageChannel _messageChannel;
        private bool flagNeedToReadBarcodeAfterWorkOrderChanged = false;
        private bool seaveyorAutoClearFlag = false;
        private Utils.Utils _utils = new Utils.Utils();
        private SetupConfigHST _setupConfig;
        private static object _locker = new object();
        private TDFOutput _tdfOutputData;
        private HSTCCCControl _hstCCCControl;
        private HSTCCCControl _crdlCCCControl;

        public CCCFileLoger _cccFileLoger;
        private MachineFailure _ticMCfailureCounter;
        private CCCAlertInformation _cccFailureInfo;
        private bool _isOutputReadStationFailed = false;
        private ImageManaging _inputImageDeletedList;
        private ImageManaging _outputImageDeletedList;
        private Dictionary<string, List<ISI_Data_Map>> _isiDataListIn;
        private Dictionary<string, List<TESTED_DATA_MAP>> _testedDataMaps;
        private List<ISI_Data_Map> _isiDataListOut;
        private double[] _wrBridgeSpecList;
        public EventHandler RunModeChanged;
        public EventHandler GroundMasterStatusChanged;
        public EventHandler MachinePerformanceStatusChanged;
        public EventHandler FunctionalTestStatusChanged;
        public EventHandler SamplingOverTargetTriggeringStatusChaned;
        public EventHandler LoadDefaultRecipeEventHandler;
        public EventHandler<LoadRecipeEventArgs> RecipeLoadedCompletedHandler;
        public EventHandler CCCVerifyEventRaised;
        public EventHandler<CCCDefectSelection> CCCDefectSelectedChanged;
        public EventHandler<CCCFinalRunResult> OnFinalCCCRunPart;
        public ARAMSStateHST ARAMSState { get { return _aramsState; } }

        
        public override IProcess MainProcess
        { get { return (IProcess)_process; } }

        public ProcessHST Process
        { get { return _process; } }

        public bool IsActive
        {
            get { return _process.IsActive; }
        }

        public string SwVersion
        {
            get { return Version; }
        }

        public Hashtable ActiveProcess
        { get { return activeProcesses; } }

        public bool IsMachineHomed { get { return _machineHomed; } set { _machineHomed = value; } }

        public InputEEZAxis InputEEZAxisPosition;
        public OutputEEZAxis OutputEEZAxisPosition;
        public TestProbeZAxis TestProbeZAxisPosition;
        public PrecisorNestXAxis PrecisorNestXAxisPosition;

        public bool IsCarrierInBufferZone { get { return _carrierInBufferZone; } set { _carrierInBufferZone = value; } }

        public bool IsCarrierInOutputTurnTable { get { return _carrierInOutputTurnTable; } set { _carrierInOutputTurnTable = value; } }

        public HGAProductTailType HGATailType { get { return _hgaTailType; } set { _hgaTailType = value; } }

        public bool IsOutputStationRFIDReadFailed { get { return _isOutputReadStationFailed; } set { _isOutputReadStationFailed = value; } }
        public TDFOutput TDFOutputObj { get { return _tdfOutputData; } set { _tdfOutputData = value; } }
        public HSTCCCControl TICCccControl { get { return _hstCCCControl; } set { _hstCCCControl = value; } }
        public HSTCCCControl CRDLCccControl { get { return _crdlCCCControl; } set { _crdlCCCControl = value; } }
        public CCCFileLoger CCCFileLoger { get { return _cccFileLoger; } set { _cccFileLoger = value; } }
        public MachineFailure TicMcFailureCounter { get { return _ticMCfailureCounter; } set { _ticMCfailureCounter = value; } }
        public CCCAlertInformation CCCFailureInfo { get { return _cccFailureInfo; } set { _cccFailureInfo = value; } }
        public void DisplayTitleMessage(string message)
        {
            _messageChannel.SendMessage(this, new Seagate.AAS.Parsel.Services.Message(message));
        }

        public bool AllProcessesIdle
        {
            get
            {
                foreach (ActiveProcessHST proc in activeProcesses.Values)
                {
                    if (proc.CurrentStateName != "StateIdle")
                        return false;
                }
                return true;
            }
        }

        public bool FlagNeedToReadBarcodeAfterWorkOrderChanged { get { return flagNeedToReadBarcodeAfterWorkOrderChanged; } set { flagNeedToReadBarcodeAfterWorkOrderChanged = value; } }
        public bool SeaveyorAutoClearFlag { get { return seaveyorAutoClearFlag; } set { seaveyorAutoClearFlag = value; } }

        public WorkOrderInfo WorkOrder { get { return _workOrder; } }
        public ProductionCounterData LoadCounter { get { return loadCounter; } }
        public DycemCleaningCounterData DycemCleaningCounter { get { return dycemCleaningCounter; } }
        public ImageManaging InputImageDeletedList { get { return _inputImageDeletedList; } set { _inputImageDeletedList = value; } }
        public ImageManaging OutputImageDeletedList { get { return _outputImageDeletedList; } set { _outputImageDeletedList = value; } }
        public Dictionary<string, List<ISI_Data_Map>> ISIDataListIn { get { return _isiDataListIn; } set { _isiDataListIn = value; } }

        public Dictionary<string, List<TESTED_DATA_MAP>> TestedDataMaps
        {
            get { return _testedDataMaps; }
            set
            {
                _testedDataMaps = value;
            }
        }

        public A3200HC _a3200HC;
        public static bool enableErrorLifter = false;
        public static bool enableErrorEE = false;
        public static bool stopRunScript = false;
        public static bool terminatingHSTApps = false;

        public static bool disableBoundaryCheck = false;

        public static bool outputEEPickDone = false;

        public static bool outputEEHoldingHGAs = false;

        public static bool outputCarrierIsUnlocked = false;

        public static bool stopSystemDueToAxisError = false;

        public static bool inputEEPickDone = false;


        public Utils.Utils Utils { get { return _utils; } }
        public SetupConfigHST SetupConfig { get { return _setupConfig; } }

        public TeachPointRecipe TeachPointRecipe
        { get { return _teachPointRecipe; } }

        public bool OffAlarm { get; set; }
        public OperationMode OperationMode { get; set; }

        public bool InputTurnStationBoatPositionError { get; set; }
        public bool InputStationBoatPositionError { get; set; }
        public bool BufferStationBoatPositionError { get; set; }
        public bool OutputStationBoatPositionError { get; set; }
        public bool OutputTurnStationBoatPositionError { get; set; }
        public bool GroundMonitoringStatus { get; set; }
        public bool GroundMonitoringErrActivated { get; set; }
        public bool CCCMachineTriggeringActivated { get; set; }
        public bool CCCMachineTriggeringDown { get; set; }
        public bool IsFirmwareGetDone { get; set; }
        public bool IsInPurgingProcess { get; set; }
        public bool IsIgnoreHomeAxisForByPass { get { return (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass) && HSTMachine.Workcell.HSTSettings.Install.DisabledHomeAxis; } }
        public bool OffAlertMessage { get; set; }
        public bool IsMainFormloaded { get; set; }
        public bool IsUnderWriterBridgeFailureAlert { get; set; }
        public Stopwatch TestTimePerCarrier { get; set; }
        public Stopwatch SwapTimePerCarrier { get; set; }
        public double TestTimePerHead { get; set; }
        public double SwapTimeCarrier { get; set; }
        public double[] WRBridgeDecreaseStepList { get { return _wrBridgeSpecList; } set { _wrBridgeSpecList = value; } }
        public int LastSamplingSlotNumber { get; set; }
        public string LastRunningRecipeName { get; set; }
        public string WRBridgeFailueCarrierId { get; set; }
        public CCCAlertActiveStatus CurretCCCActiveStatus { get; set; }
        public GraphWithMcMapping GraphAndMcMapping { get; set; }
        public bool IsBaselineRatioSpecCompared { get; set; }               
        public bool IsToleranceSpecShortTestCompared { get; set; }          
        public bool IsVolThresholdSpecCompared { get; set; }                
        public int AncUticMcCounter { get; set; }
        public bool IsRecipeLoadedDone { get; set; }
        public bool IsAllMeasurementFailed { get; set; }

        #region Public Signals
        //Signal to Communicate between Seaveyor
        public static readonly Signal SigSeaveyorAutoClearDone = new Signal("SigSeaveyorAutoClearDone");
        public static readonly Signal SigAutoClearAllDone = new Signal("SigAutoClearAllDone");
        public static readonly Signal SigInputStationReady = new Signal("SigInputStationReady");//Publish from InputStation to InputTurnStation
        public static readonly Signal SigPartPresentInInputStation = new Signal("SigPartPresentInInputStation");// from InputTurnStation to InputStation
        public static readonly Signal SigInputLifterCarrierReadyForPick = new Signal("SigInputLifterCarrierReadyForPick");
        public static readonly Signal SigInputLifterCarrierPickDone = new Signal("SigInputLifterCarrierPickDone");

        public static readonly Signal SigInputLifterDownCompleted = new Signal("SigInputLifterDownCompleted");
        public static readonly Signal SigHGAsReadyToPickAtInputLifter = new Signal("SigHGAsReadyToPickAtInputLifter");
        public static readonly Signal SigHGAsPickDoneAtInputLifter = new Signal("SigHGAsPickDoneAtInputLifter");
        public static readonly Signal SigHGAsInputEECompletePick = new Signal("SigHGAsInputEECompletePick");

        public static readonly Signal SigStart = new Signal("SigStart");
        public static readonly Signal SigCarrierPresentInInputStation = new Signal("SigCarrierPresentInInputStation");

        public static readonly Signal SigCarrierPresentInZone3 = new Signal("SigCarrierPresentInZone3");
        public static readonly Signal SigCarrierPresentInBufferStation = new Signal("SigCarrierPresentInBufferStation");
        public static readonly Signal SigCarrierPresentInOutputStation = new Signal("SigCarrierPresentInOutputStation");
        public static readonly Signal SigOutputStationReady = new Signal("SigOutputStationReady");

        public static readonly Signal SigVisionDetectionFoundMissingHGAs = new Signal("SigVisionDetectionFoundMissingHGAs");
        public static readonly Signal SigVisionDetectionFoundNoMissingHGAs = new Signal("SigVisionDetectionFoundNoMissingHGAs");
        public static readonly Signal SigPrecisorReadyForPlace = new Signal("SigPrecisorReadyForPlace");
        public static readonly Signal SigPrecisorSetVacuumOn = new Signal("SigPrecisorSetVacuumOn");
        public static readonly Signal SigPrecisorVacuumOn = new Signal("SigPrecisorVacuumOn");
        public static readonly Signal SigPrecisorPlaceDone = new Signal("SigPrecisorPlaceDone");
        public static readonly Signal SigPrecisorReadyForPick = new Signal("SigPrecisorReadyForPick");
        public static readonly Signal SigPrecisorPickDone = new Signal("SigPrecisorPickDone");
        public static readonly Signal SigReadyForProbe = new Signal("SigReadyForProbe");
        public static readonly Signal SigHGATestingDone = new Signal("SigHGATestingDone");
        public static readonly Signal SigProbeDone = new Signal("SigProbeDone");
        public static readonly Signal SigOutputCarrierReadyForPlace = new Signal("SigOutputCarrierReadyForPlace");
        public static readonly Signal SigOutputCarrierPlaceDone = new Signal("SigOutputCarrierPlaceDone");
        public static readonly Signal SigOutputPickAllHGAs = new Signal("SigOutputPickAllHGAs");
        public static readonly Signal SigTestProbeGetResultDone = new Signal("SigTestProbeGetResultDone");

        public static readonly Signal SigCarrierPresentInOutputTurnStation = new Signal("SigCarrierPresentInOutputTurnStation");
        public static readonly Signal SigCarrierIsInOutputTurnStation = new Signal("SigCarrierIsInOutputTurnStation");

        public static readonly Signal SigInputEEHomed = new Signal("SigInputEEHomed");
        public static readonly Signal SigOutputEEHomed = new Signal("SigOutputEEHomed");
        public static readonly Signal SigTestProbeHomed = new Signal("SigTestProbeHomed");
        public static readonly Signal SigInputLifterHomed = new Signal("SigInputLifterHomed");
        public static readonly Signal SigOutputLifterHomed = new Signal("SigOutputLifterHomed");
        public static readonly Signal SigStartBoundaryCheckInputEEAxis = new Signal("SigStartBoundaryCheckInputEEAxis");
        public static readonly Signal SigStartBoundaryCheckTestProbeAxis = new Signal("SigStartBoundaryCheckTestProbeAxis");
        public static readonly Signal SigStartBoundaryCheckOutputEEAxis = new Signal("SigStartBoundaryCheckOutputEEAxis");
        public static readonly Signal SigInputEEBoundaryCheckComplete = new Signal("SigInputEEBoundaryCheckComplete");
        public static readonly Signal SigOutputEEBoundaryCheckComplete = new Signal("SigOutputEEBoundaryCheckComplete");
        public static readonly Signal SigTestProbeBoundaryCheckComplete = new Signal("SigTestProbeBoundaryCheckComplete");
        public static readonly Signal SigStopMachineRun = new Signal("SigStopMachineRun");
        public static readonly Signal SigEndRunProcess = new Signal("SigEndRunProcess");
        public static readonly Signal SigRunTestScript = new Signal("SigRunTestScript");
        public static readonly Signal SigOutputEEAxisSafeToPlacePrecisorNestAtInputStation = new Signal("SigOutputEEAxisSafeToPlacePrecisorNestAtInputStation");
        public static readonly Signal SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition = new Signal("SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition");
        public static readonly Signal SigInputEEStartDycemCleaning = new Signal("SigInputEEStartDycemCleaning");
        public static readonly Signal SigInputEEDycemCleaningComplete = new Signal("SigInputEEDycemCleaningComplete");
        public static readonly Signal SigOutputEEStartDycemCleaning = new Signal("SigOutputEEStartDycemCleaning");
        public static readonly Signal SigOutputEEDycemCleaningComplete = new Signal("SigOutputEEDycemCleaningComplete");

        public static readonly Signal SigInputRFIDReadComplete = new Signal("SigInputRFIDReadComplete");
        public static readonly Signal SigOutputProcessDataComplete = new Signal("SigOutputProcessDataComplete");
        public static readonly Signal SigInputGetISIDataComplete = new Signal("SigInputGetISIDataComplete");

        public static readonly Signal SigOutputStartRFIDProcess = new Signal("SigOutputStartRFIDProcess");
        public static readonly Signal SigOutputClampRotateProcessComplete = new Signal("SigOutputClampRotateProcessComplete");

        public static readonly Signal SigOverallMeasurementDone = new Signal("SigOverallMeasurementDone");
        public static readonly Signal SigAllMeasurementDataDone = new Signal("SigAllMeasurementDataDone");
        #endregion

        public HSTWorkcell()
        {
            Console.WriteLine("Workcell Constructor");
            this.Name = "HST";
            this.SlotName = "HST";
            string temp = Assembly.GetExecutingAssembly().FullName;
            temp = temp.Split(',')[1];
            temp = temp.Split('=')[1];
            Version = temp;
            activeProcessCollector = new Hashtable();

            _messageChannel = ServiceManager.Messaging.CreateMessageChannel("TitleMessage");

            _a3200HC = new A3200HC();

            _ioManifest = new HSTIOManifest(this);

            _process = new ProcessHST(this);

            _setupConfig = new SetupConfigHST(this);

            _teachPointRecipe = new TeachPointRecipe(); // Lai: Do we need to load any recipe file here?

            TestTimePerCarrier = new Stopwatch();
            SwapTimePerCarrier = new Stopwatch();
            //Load default recipe for all required teach position for boundary check
            string TeachPointRecipeFilePath = "C:\\Seagate\\HGA.HST\\Recipes\\default.teachpointrcp";

            XmlDocument doc = new XmlDocument();
            doc.Load(TeachPointRecipeFilePath);
            _inputImageDeletedList = new ImageManaging();
            _outputImageDeletedList = new ImageManaging();
            _isiDataListIn = new Dictionary<string, List<ISI_Data_Map>>();
            _testedDataMaps = new Dictionary<string, List<TESTED_DATA_MAP>>();
            _isiDataListOut = new List<ISI_Data_Map>();
            _ticMCfailureCounter = new MachineFailure();
            _cccFailureInfo = new CCCAlertInformation();
            CurretCCCActiveStatus = new CCCAlertActiveStatus();
            GraphAndMcMapping = new GraphWithMcMapping();
            _wrBridgeSpecList = new double[41];
            TeachPointRecipe.Load(doc);
            IsBaselineRatioSpecCompared = true;                                     
            IsToleranceSpecShortTestCompared = true;                                
            IsVolThresholdSpecCompared = true;                                      
        }

        public void instantiateWorkOrder()
        {
            _aramsState = new ARAMSStateHST(this);
            if (String.IsNullOrEmpty(HSTSettings.Directory.LogFilePath))
            {
                HSTSettings.Directory.LogFilePath = "C:\\Seagate\\HGA.HST\\Log";
            }
            if (String.IsNullOrEmpty(HSTSettings.Directory.TSRRecipLocalPath))
            {
                HSTSettings.Directory.TSRRecipLocalPath = "C:\\Seagate\\HGA.HST\\WorkOrder";
            }
            if (String.IsNullOrEmpty(HSTSettings.Directory.TDFLocalDataPath))
            {
                HSTSettings.Directory.TDFLocalDataPath = "C:\\Seagate\\HGA.HST\\TDFData";
            }
            if (String.IsNullOrEmpty(HSTSettings.Directory.TDFDataBackPath))
            {
                HSTSettings.Directory.TDFDataBackPath = "C:\\Seagate\\HGA.HST\\TDFDataBackup";
            }
            //Global
            if (String.IsNullOrEmpty(HSTSettings.Directory.TDFGlobalDataPath))
            {
                HSTSettings.Directory.TDFGlobalDataPath = "F:\\";
            }

            if (String.IsNullOrEmpty(HSTSettings.Directory.ANCDataLogPath))
            {
                HSTSettings.Directory.ANCDataLogPath = "C:\\Seagate\\HGA.HST\\Data\\CCCData";
            }

            _workOrder = new WorkOrderInfo(HSTSettings.Directory.TSRRecipLocalPath, HSTSettings.Directory.LogFilePath, CommonFunctions.HST_STATION_CODE);
            loadCounter = new ProductionCounterData(HSTSettings.Directory.DataPath + @"\LoadCounter.info");
            dycemCleaningCounter = new DycemCleaningCounterData(HSTSettings.Directory.DataPath + @"\DycemCleaningCounter.info");
            _tdfOutputData = new TDFOutput();
        }

        #region public properties

        public FolaReader RFIDScanner { get; private set; }
        public SafetyController OmronSafetyController { get; private set; }

        public OperationMode RunMode
        {
            get { return _runMode; }
            set
            {
                _runMode = value;
                if (RunModeChanged != null)
                    RunModeChanged(this, EventArgs.Empty);
            }
        }

        public string HSTSettingsFilePath
        {
            get
            {
                return ServiceManager.DirectoryLocator.GetPath(Folders.Setup) + @"\HGAHST.config";
            }
        }

        public string CalibrationSettingsFilePath
        {
            get
            {
                return ServiceManager.DirectoryLocator.GetPath(Folders.Setup) + @"\Calibration.config";
            }
        }

        public string UsersSettingsFilePath
        {
            get
            {
                return ServiceManager.DirectoryLocator.GetPath(Folders.Setup) + @"\Users.config";
            }
        }

        public string SetupSettingsFilePath
        {
            get
            {
                return ServiceManager.DirectoryLocator.GetPath(Folders.Setup) + @"\Automation.config";
            }
        }

        public HSTSettings HSTSettings
        {
            get
            {
                if (_HSTSettings == null)
                {
                    _HSTSettings = HSTSettings.Instance;
                    _HSTSettings.HSTSettingsFilePath = this.HSTSettingsFilePath;
                    _HSTSettings.CalibrationSettingsFilePath = this.CalibrationSettingsFilePath;
                    _HSTSettings.UsersSettingsFilePath = this.UsersSettingsFilePath;
                    _HSTSettings.SetupSettingsFilePath = this.SetupSettingsFilePath;
                    _HSTSettings.Load();
                    this.RunMode = _HSTSettings.Install.OperationMode;
                    this.OperationMode = _HSTSettings.Install.OperationMode;

                    if (File.Exists(CommonFunctions.TempFileWhenRunningHSTApplication))
                    {
                        _HSTSettings.Install.ClearImproperShutDownMessage = false;
                    }
                    else
                    {
                        _HSTSettings.Install.ClearImproperShutDownMessage = true;
                        File.WriteAllText(CommonFunctions.TempFileWhenRunningHSTApplication, "This file is generated by HST application to detect improper close of HST application.\n\nDo not delete this file while HST application is running.");
                    }
                }

                return _HSTSettings;
            }
        }

        public CalibrationSettings CalibrationSettings
        {
            get
            {
                if (_CalibrationSettings == null)
                {
                    _CalibrationSettings = CalibrationSettings.Instance;
                    _CalibrationSettings.Load();
                }

                return _CalibrationSettings;
            }
        }

        public AutomationConfigSettings SetupSettings
        {
            get
            {
                if (_SetupSettings == null)
                {
                    _SetupSettings = AutomationConfigSettings.Instance;
                    _SetupSettings.Load();
                }

                return _SetupSettings;
            }
        }

        public void UpdateUsersSettings(object sender, EventArgs e)
        {
            try
            {
                UserAccessSettings UserAccessSettings = sender as UserAccessSettings;

                if (UserAccessSettings != null)
                {
                    HSTMachine.Workcell.HSTSettings.LoadUsers();
                    HSTMachine.Instance.MainForm.getPanelTitle().AssignWorkcell(HSTMachine.Workcell);
                    HSTMachine.Instance.MainForm.getPanelNavigation().SetPanel("Operation");
                    UserControl activePanel = Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.GetActivePanel();
                    TabControl tabControl = activePanel.Controls[0] as TabControl;
                    tabControl.SelectedIndex = 0;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Called during app shut down to close out all connections and release resources
        /// </summary>
        public void Shutdown()
        {
            // TODO:
            if (HSTMachine.Workcell.Process.MonitorIOState != null)
                HSTMachine.Workcell.Process.MonitorIOState.StopLogIOState();
            HSTMachine.HwSystem.GetHwComponent((int)HST.Machine.HSTHwSystem.HardwareComponent.VisionSystem).ShutDown();

            System.Threading.Thread.Sleep(1000);
        }

        #endregion

        #region methods

        public override void RegisterPanels()
        {
            ServiceManager.MenuNavigator.RegisterPanel("Operation", SlotName, OperationPanel);
            ServiceManager.MenuNavigator.RegisterPanel("Recipe", SlotName, SetupRecipe);
            ServiceManager.MenuNavigator.RegisterPanel("Setup", SlotName, SetupPanel);
            ServiceManager.MenuNavigator.RegisterPanel("Diagnostic", SlotName, DiagnosticPanel);
            ServiceManager.MenuNavigator.RegisterPanel("Data", SlotName, DataPanel);
            ServiceManager.MenuNavigator.RegisterPanel("Help", SlotName, HelpPanel);
        }

        private void OnRunModeChanged(object sender, EventArgs e)
        {
            Seagate.AAS.HGA.HST.UI.FormMainE95 form = HSTMachine.Instance.MainForm;
            if (form == null)
                return;

            // Disable simulation if vision is enabled
            if (HSTMachine.Workcell.HSTSettings.Install.EnableVision == true)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    _runMode = OperationMode.Auto;
                }
            }

            if (_runMode == OperationMode.Auto)
            {
                form.ModeCaption = "Auto";
            }
            else if (_runMode == OperationMode.DryRun)
            {
                form.ModeCaption = (HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat) ? "Dry Run (No Boats)" : "Dry Run (With Boats)";
            }
            else if (_runMode == OperationMode.Bypass)
            {
                form.ModeCaption = "Bypass";
            }
            else if (_runMode == OperationMode.Simulation)
            {
                form.ModeCaption = "Simulation";
            }
        }

        #endregion

        #region overrides

        public override void Initialize()
        {
            // Log startup.
            Log.Info(this, "{0}, Workcell Initialize happening just before active processes are added in call with their constructors", LoggerCategory.ApplicationLaunch);

            ServiceManager.Tracing.Trace("Initializing HST Workcell");

            try
            {
                InitializeRFIDDataBlock();
                InitializeSafetyController();
                _process.Initialize();
            }
            catch (System.Exception ex)
            {
                string msg = ServiceManager.ErrorHandler.GetExceptionMessages(ex);
                Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(msg);
                ServiceManager.Tracing.Trace(msg);
            }

            RunModeChanged += new EventHandler(OnRunModeChanged);
        }

        public override void Startup()
        {
            _process.Startup();


            int priority = 0;
            foreach (ActiveProcess ap in activeProcesses.Values)
            {
                System.Diagnostics.Trace.WriteLine("starting active process: " + ap.GetType().ToString());
                ap.Start(10 * Slot + priority++);
            }
            try
            {
                _a3200HC.Initialize(false);
            }
            catch (Exception ex)
            {
                string msg = ServiceManager.ErrorHandler.GetExceptionMessages(ex);
                Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(msg);
                ServiceManager.Tracing.Trace(msg);
            }

            csvFileOutput = new CSVFileOutput(this.HSTSettings.Directory.DataPath);
            _dataLog = new DataLog();
            // csvFileOutput.GenerateNewCSVFile(/*"DataLog.csv",*/ "EVENT_DATE", "CARRIER_ID", "CARRIER_SLOT", "SERIAL_NO", "STATUS",/* "CYCLE_TIME", */"WORK_ORDER",
            //     "ERROR_MSG_CODE", "SETUP_FILE", "READER1_RES", "READER2_RES", "DeltaISI_Reader1", "DeltaISI_Reader2", "WRITER_RES", "rHEATER_RES", "wHEATER_RES", "TA_RES", "SHORT_TEST", "SHORT_TESTPOS", "VOLTAGE_DELTA1", "VOLTAGE_DELTA2", "TEMP_Board",
            //     "SDET_READER1", "SDET_READER2", "DELTA_POLARITY1", "DELTA_POLARITY2", "UTH_EQUIP_ID", "UTH_TIME","Location", "Equip_ID", "Equip_Type"/*, "UPH"*/, "SDET_WRITER", "HST_SDET_DELTA_WRITER", "WRBRIDGE_PCT", "WRBRIDGE_ADAP_SPEC", "STATUS_SW", "LOGIN_USER_GID", "OPERATION_MODE", "WORKORDER VERSION", "ISI_READER1", "ISI_READER2", "LDU_RES", "LDU_SPEC");

            csvFileOutput.GenerateNewCSVFile(/*"DataLog.csv",*/ "EVENT_DATE", "CARRIER_ID", "CARRIER_SLOT", "SERIAL_NO", "STATUS",/* "CYCLE_TIME", */"WORK_ORDER", //6
                "ERROR_MSG_CODE", "SETUP_FILE", "READER1_RES", "READER2_RES", "DeltaISI_Reader1", "DeltaISI_Reader2", "WRITER_RES", "rHEATER_RES", "wHEATER_RES", "TA_RES", "SHORT_TEST", "SHORT_TESTPOS", "VOLTAGE_DELTA1", "VOLTAGE_DELTA2", "TEMP_Board", //15
                "SDET_READER1", "SDET_READER2", "DELTA_POLARITY1", "DELTA_POLARITY2", "UTH_EQUIP_ID", "UTH_TIME", "Location", "Equip_ID", "Equip_Type"/*, "UPH"*/, "SDET_WRITER", "HST_SDET_DELTA_WRITER", "WRBRIDGE_PCT", "WRBRIDGE_ADAP_SPEC", "STATUS_SW",//14
                "LOGIN_USER_GID", "OPERATION_MODE", "WORKORDER VERSION", "ISI_READER1", "ISI_READER2", "VOL_RATIO_CH1", "VOL_RATIO_CH2", "VOL_RATIO_CH3", "VOL_RATIO_CH4", "VOL_RATIO_CH5", "VOL_RATIO_CH6", "LDU_RES", "LDU_SPEC", //7
                "LED_INTERCEPT", "I_THRESHOLD", "MAX_V_PD", "LDU_TURN_ON_VOLTAGE", "SDET_I_THRESHOLD", "DELTA_I_THRESHOLD"); //4

            _dataLog.ResetAllData();
            _process.MonitorIOState.GetIOState();
        }

        public void AbortMachine()
        {
            this.Process.Stop();
            this.IsMachineHomed = false;
            this.OnAbort();
        }


        public void OnAbort()
        {
            if (Aborted != null)
                Aborted(this, new EventArgs());
        }

        public System.Windows.Forms.UserControl OperationPanel
        {
            get
            {
                if (_panelOperation == null)
                    _panelOperation = new PanelOperation(this);

                return _panelOperation;
            }
        }

        public PanelOperation getPanelOperation()
        {
            return _panelOperation;
        }

        public PanelDiagnostics getPanelDiagnostics()
        {
            return _panelDiagnostics;
        }

        public System.Windows.Forms.UserControl DiagnosticPanel
        {
            get
            {
                if (_panelDiagnostics == null)
                    _panelDiagnostics = new PanelDiagnostics(this);

                return _panelDiagnostics;
            }
        }

        public PanelData getPanelData()
        {
            return _panelData;
        }
        public System.Windows.Forms.UserControl DataPanel
        {
            get
            {
                if (_panelData == null)
                    _panelData = new PanelData(this);

                return _panelData;
            }
        }


        public System.Windows.Forms.UserControl HelpPanel
        {
            get
            {
                if (_panelHelp == null)
                    _panelHelp = new PanelHelp(this);

                return _panelHelp;
            }
        }
        public PanelSetup getPanelSetup()
        {
            return _panelSetup;
        }

        public System.Windows.Forms.UserControl SetupPanel
        {
            get
            {
                if (_panelSetup == null)
                    _panelSetup = new PanelSetup(this);

                return _panelSetup;
            }
        }

        public PanelRecipe getPanelRecipe()
        {
            return _panelRecipe;
        }

        public System.Windows.Forms.UserControl SetupRecipe
        {
            get
            {
                if (_panelRecipe == null)
                    _panelRecipe = new PanelRecipe(this);

                return _panelRecipe;
            }
        }

        public override AAS.Parsel.Hw.IOManifest IOManifest
        {
            get
            {
                Console.WriteLine("Workcell IO Manifest Get");
                return (Seagate.AAS.Parsel.Hw.IOManifest)_ioManifest;
            }
        }

        public override string ToString()
        {
            return string.Format("HSTWorkcell-{0}", this.Slot);
        }

        public override void Dispose()
        {
            LoadCounter.Save();
            _HSTSettings.Save();

            if (this.RFIDScanner != null)
            {
                this.RFIDScanner.ShutDown();
            }

            if (this._a3200HC != null)
            {
                this._a3200HC.ShutDown();
            }
            _process.Dispose();

            activeProcesses.Clear();
        }

        public void RaiseLoadDefaultRecipeEvent()
        {
            if (LoadDefaultRecipeEventHandler != null)
                LoadDefaultRecipeEventHandler(this, EventArgs.Empty);
        }

        public void RaiseLoadRecipeStatus(LoadRecipeEventArgs status)
        {
            if (RecipeLoadedCompletedHandler != null)
                RecipeLoadedCompletedHandler(this, status);
        }

        public void SendGroundMasterStatusMessage()
        {
            if (GroundMasterStatusChanged != null)
                GroundMasterStatusChanged(this, EventArgs.Empty);
        }

        public void SendMachinePerformanceStatusMessage()
        {
            if (MachinePerformanceStatusChanged != null)
                MachinePerformanceStatusChanged(this, EventArgs.Empty);
        }

        public void SendFunctionalTestStatusMessage()
        {
            if (FunctionalTestStatusChanged != null)
                FunctionalTestStatusChanged(this, EventArgs.Empty);
        }

        public void SendSamplingOverTargetTriggeringStatusMessage()
        {
            if (SamplingOverTargetTriggeringStatusChaned != null)
                SamplingOverTargetTriggeringStatusChaned(this, EventArgs.Empty);
        }

        public void RaiseCCCRunPartUpdated(CCCRunResult unitData, CCCDataLogger runResult, CCCOutput.Trigger_Type triggerType, TIC_BIN_DATA uticData)
        {
            if (OnFinalCCCRunPart != null)
            {
                OnFinalCCCRunPart(this, new CCCFinalRunResult(unitData, runResult, triggerType, uticData));
            }
        }

        public void RaiseCCCDefectUpdated(CCCDefectSelection defect)
        {
            if (CCCDefectSelectedChanged != null)
            {
                CCCDefectSelectedChanged(this, defect);
            }
        }

        public void RaiseCCCVerifyEventStatus()
        {
            if (CCCVerifyEventRaised != null)
                CCCVerifyEventRaised(this, EventArgs.Empty);
        }

        #endregion

        public void LoadProductRecipe()
        {
            try
            {
                var productName = CommonFunctions.Instance.ProductRecipeName;

                // Get product name from recipe
                if (productName != null)
                {
                    CommonFunctions.Instance.ProductRecipeName = productName;
                    SetupConfig.LastRunRecipeName = productName;
                    HSTMachine.Workcell.getPanelSetup().btnConfigurationLoadConfiguration_Click(this, new EventArgs());

                    //remark for one recipe
                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtRecipeName.Text = productName;
                    Log.Info(this, "In SaveConfigToProcessor, found a match in the HGA Product Type database in which the Product ID = {0} and Product Name = {1}.", productName, productName);

                    //Turn off to ignore now after one recipe has been implemented
                    //This still get config from INI file, make sure you have those file before turning on this
                    CommonFunctions.Instance.CompensationInfo = new CompensationInfo(HSTMachine.Workcell.HSTSettings.Directory.WorkorderLocalPath + "\\" + productName + ".ini", false);

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "For '{10}' Up Tab, CH 1 Capacitance Compensation values - HGA1:{0}, HGA2:{1}, HGA3:{2}, HGA4:{3}, HGA5:{4}, HGA6:{5}, HGA7:{6}, HGA8:{7}, HGA9:{8}, HGA10:{9}",
                            CommonFunctions.Instance.CompensationInfo.HGA1UpTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA2UpTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA3UpTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA4UpTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA5UpTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA6UpTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA7UpTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA8UpTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA9UpTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA10UpTabCapacitance1Compensation,
                            productName);

                        Log.Info(this, "For '{10}' Down Tab, CH 1 Capacitance Compensation values - HGA1:{0}, HGA2:{1}, HGA3:{2}, HGA4:{3}, HGA5:{4}, HGA6:{5}, HGA7:{6}, HGA8:{7}, HGA9:{8}, HGA10:{9}",
                            CommonFunctions.Instance.CompensationInfo.HGA1DownTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA2DownTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA3DownTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA4DownTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA5DownTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA6DownTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA7DownTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA8DownTabCapacitance1Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA9DownTabCapacitance1Compensation, CommonFunctions.Instance.CompensationInfo.HGA10DownTabCapacitance1Compensation,
                            productName);

                        Log.Info(this, "For '{10}' Up Tab, CH 2 Capacitance Compensation values - HGA1:{0}, HGA2:{1}, HGA3:{2}, HGA4:{3}, HGA5:{4}, HGA6:{5}, HGA7:{6}, HGA8:{7}, HGA9:{8}, HGA10:{9}",
                            CommonFunctions.Instance.CompensationInfo.HGA1UpTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA2UpTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA3UpTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA4UpTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA5UpTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA6UpTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA7UpTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA8UpTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA9UpTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA10UpTabCapacitance2Compensation,
                            productName);

                        Log.Info(this, "For '{10}' Down Tab, CH 2 Capacitance Compensation values - HGA1:{0}, HGA2:{1}, HGA3:{2}, HGA4:{3}, HGA5:{4}, HGA6:{5}, HGA7:{6}, HGA8:{7}, HGA9:{8}, HGA10:{9}",
                            CommonFunctions.Instance.CompensationInfo.HGA1DownTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA2DownTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA3DownTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA4DownTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA5DownTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA6DownTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA7DownTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA8DownTabCapacitance2Compensation,
                            CommonFunctions.Instance.CompensationInfo.HGA9DownTabCapacitance2Compensation, CommonFunctions.Instance.CompensationInfo.HGA10DownTabCapacitance2Compensation,
                            productName);

                        Log.Info(this, "For '{0}' LDU enable is {1}", productName, (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable));
                    }

                    //break;
                    // }
                    //}

                    File.WriteAllText(CommonFunctions.TempFileToIndicateDownloadConfigToProcessorClicked, "This file is generated by HST application to detect if operator has clicked the 'Save Config to Processor' button prior to running the machine.\n\nDo not delete this file while HST application is running.");

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation &&
                        (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.DryRun))
                    {
                        HSTMachine.Workcell.getPanelSetup().DownloadConfigurationToMicroProcessor();
                        HSTMachine.Workcell.getPanelRecipe().UpdateLDUSettingPanel();
                    }
                    HSTMachine.Instance.MainForm.getPanelNavigation().SetPanel("Operation");
                    UserControl activePanel = Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.GetActivePanel();
                    TabControl tabControl = activePanel.Controls[0] as TabControl;
                    tabControl.SelectedIndex = 0;

                    RaiseLoadRecipeStatus(new LoadRecipeEventArgs(true));


                    //Assign ANC parameter from TSR
                    if (HSTMachine.Workcell.TICCccControl == null || IsAncSettingChanged())
                    {
                        bool isResetNeed = false;

                        if (IsAncSettingChanged())
                            isResetNeed = true;
                        InitialANCobject(isResetNeed);
                    }
                }
            }
            catch (Exception)
            {
                RaiseLoadRecipeStatus(new LoadRecipeEventArgs(false));
            }
        }


        /// <summary>
        /// Compare ratio spec
        /// </summary>
        /// <param name="fromController"></param>
        /// <returns></returns>
        public bool CompareBaselineRatioSpec(BaselineVoltageRatio fromController)
        {
            bool returnResult = false;

            if (CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceVoltRatio == 0 && CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceVoltRatio == 0 && CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceVoltRatio == 0 &&
                CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceVoltRatio == 0 && CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceVoltRatio == 0 && CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceVoltRatio == 0)
                return true;

            returnResult = ((fromController.CurrentRatio_CH1 == CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceVoltRatio) &&
                (fromController.CurrentRatio_CH2 == CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceVoltRatio) &&
                (fromController.CurrentRatio_CH3 == CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceVoltRatio) &&
                (fromController.CurrentRatio_CH4 == CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceVoltRatio) &&
                (fromController.CurrentRatio_CH5 == CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceVoltRatio) &&
                (fromController.CurrentRatio_CH6 == CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceVoltRatio));

            return returnResult;
        }


        public bool CompareToleranceSpec(TestProbe60GetShortDetectionThreshold fromController)
        {
            bool returnResult = false;

            var voltageHighLimitCH1 = fromController.ThresholdVoltageHighLimitCH1();
            var voltageHighLimitCH2 = fromController.ThresholdVoltageHighLimitCH2();
            var voltageHighLimitCH3 = fromController.ThresholdVoltageHighLimitCH3();
            var voltageHighLimitCH4 = fromController.ThresholdVoltageHighLimitCH4();
            var voltageHighLimitCH5 = fromController.ThresholdVoltageHighLimitCH5();
            var voltageHighLimitCH6 = fromController.ThresholdVoltageHighLimitCH6();
            var voltageLowLimitCH1 = fromController.ThresholdVoltageLowLimitCH1();
            var voltageLowLimitCH2 = fromController.ThresholdVoltageLowLimitCH2();
            var voltageLowLimitCH3 = fromController.ThresholdVoltageLowLimitCH3();
            var voltageLowLimitCH4 = fromController.ThresholdVoltageLowLimitCH4();
            var voltageLowLimitCH5 = fromController.ThresholdVoltageLowLimitCH5();
            var voltageLowLimitCH6 = fromController.ThresholdVoltageLowLimitCH6();

            returnResult = ((voltageHighLimitCH1 == CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceToleranceUpperSpec) &&
                (voltageHighLimitCH2 == CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceToleranceUpperSpec) &&
                (voltageHighLimitCH3 == CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceToleranceUpperSpec) &&
                (voltageHighLimitCH4 == CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceToleranceUpperSpec) &&
                (voltageHighLimitCH5 == CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceUpperSpec) &&
                (voltageHighLimitCH6 == CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceToleranceUpperSpec) &&
                (voltageLowLimitCH1 == CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceToleranceLowSpec) &&
                (voltageLowLimitCH2 == CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceToleranceLowSpec) &&
                (voltageLowLimitCH3 == CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceToleranceLowSpec) &&
                (voltageLowLimitCH4 == CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceToleranceLowSpec) &&
                (voltageLowLimitCH5 == CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec) &&
                (voltageLowLimitCH6 == CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceToleranceLowSpec));

            return returnResult;
        }

        public bool CompareVolThresholdSpec(TestProbe71GetShortDetectionVolThreshold fromController)
        {
            bool returnResult = false;

            var volThreshold1Hi = fromController.GetVolThreshold1HiLimit();
            var volThreshold1Low = fromController.GetVolThreshold1LowLimit();
            var volThreshold2Hi = fromController.GetVolThreshold2HiLimit();
            var volThreshold2Low = fromController.GetVolThreshold2LowLimit();

            returnResult = ((volThreshold1Hi == CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold1HiLimit) &&
                (volThreshold1Low == CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold1LowLimit) &&
                (volThreshold2Hi == CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold2HiLimit) &&
                (volThreshold2Low == CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold2LowLimit));

            return returnResult;
        }

        public void InitialANCobject(bool isResetNeeded)
        {
            ////Assign ANC parameter from TSR
            CCCParameter ancSetting = new CCCParameter();
            if (CommonFunctions.Instance.IsRunningWithNewTSR)
            {
                ancSetting.TestRunGroup = Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.TestRunCount);
                ancSetting.YeildLimit = CommonFunctions.Instance.MeasurementTestRecipe.YieldLimit;
                ancSetting.YeildTarget =
                    CommonFunctions.Instance.MeasurementTestRecipe.YieldTarget;
                ancSetting.DefectCounterLimit = Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.CountLimit);
                ancSetting.Alpha = CommonFunctions.Instance.MeasurementTestRecipe.GoodBetweenBad;

            }
            else
            {
                ancSetting = HSTMachine.Workcell.HSTSettings.CccParameterSetting;
            }

            HSTMachine.Workcell.TICCccControl = new Data.CumulativeCountofConforming.HSTCCCControl(ancSetting, HSTMachine.Workcell.HSTSettings.TicCCCCounter, true, false);

            if(isResetNeeded)
                HSTMachine.Workcell.getPanelSetup().ANCReset();

            //End Bypass hstCCCCounter
            //---------------------------------------------------------------------
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicDefactCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_TicFailCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstFailCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_HstFailCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicGoodPartCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_TicGoodPartCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstGoodPartCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_HstGoodPartCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.AdaptivePartRunCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_AdaptivePartRunCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.AdaptiveDefectCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_AdaptiveDefectCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.LastSaveLogTime = DateTime.ParseExact(HSTMachine.Workcell.HSTSettings.ANCSettings.All_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.MCDownTriggering = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_MCDownTriggering.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicHighPercentTriggeringCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_TicHighPercentTriggeringCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstHighPercentTriggeringCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_HstHighPercentTriggeringCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.InternalTriggerCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.All_InternalTriggerCounter.ToString());

            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicFailCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_TicFailCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstFailCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_HstFailCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicGoodPartCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_TicGoodPartCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstGoodPartCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_HstGoodPartCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_AdaptivePartRunCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_AdaptiveDefectCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastSaveLogTime = DateTime.ParseExact(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_MCDownTriggering.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicHighPercentTriggeringCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_TicHighPercentTriggeringCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstHighPercentTriggeringCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_HstHighPercentTriggeringCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.InternalTriggerCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC1_InternalTriggerCounter.ToString());

            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicFailCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_TicFailCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstFailCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_HstFailCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicGoodPartCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_TicGoodPartCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstGoodPartCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_HstGoodPartCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_AdaptivePartRunCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_AdaptiveDefectCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastSaveLogTime = DateTime.ParseExact(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_MCDownTriggering.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicHighPercentTriggeringCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_TicHighPercentTriggeringCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstHighPercentTriggeringCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_HstHighPercentTriggeringCounter.ToString());
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.InternalTriggerCounter = int.Parse(HSTMachine.Workcell.HSTSettings.ANCSettings.MC2_InternalTriggerCounter.ToString());

            HSTMachine.Workcell.HSTSettings.Save();

        }

        public bool IsAncSettingChanged()
        {
            bool isChanged = false;

            if (HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup != Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.TestRunCount) ||
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit != CommonFunctions.Instance.MeasurementTestRecipe.YieldLimit ||
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget != CommonFunctions.Instance.MeasurementTestRecipe.YieldTarget ||
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit != Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.CountLimit) ||
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha != CommonFunctions.Instance.MeasurementTestRecipe.GoodBetweenBad)
                isChanged = true;
            return isChanged;
        }

        // Internal methods ----------------------------------------------------
        private void InitializeRFIDDataBlock()
        {
            try
            {
                this.RFIDScanner = new FolaReader();
                this.RFIDScanner.PortSettings = this.CalibrationSettings.getRfidPortSettings();
                this.RFIDScanner.Initialize(this.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            }
            catch (Exception ex)
            {
                string msg = ServiceManager.ErrorHandler.GetExceptionMessages(ex);
                msg = msg.TrimEnd('\r', '\n');
                Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(msg);
                ServiceManager.Tracing.Trace(msg);
            }
        }

        private void InitializeSafetyController()
        {
            try
            {
                this.OmronSafetyController = new SafetyController();
                this.OmronSafetyController.PortSettings = this.CalibrationSettings.getSafetyControllerPortSettings();
                this.OmronSafetyController.Initialize(this.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            }
            catch (Exception ex)
            {
                string msg = ServiceManager.ErrorHandler.GetExceptionMessages(ex);
                Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(msg);
                ServiceManager.Tracing.Trace(msg);
            }
        }

        public GompertzSettings GompertzSettings
        {
            get
            {
                if (gompertzSettings == null)
                {
                    gompertzSettings = GompertzSettings.Instance;
                    gompertzSettings.Load();
                }

                return gompertzSettings;
            }
        }
    }
}
