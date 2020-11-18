using System;
using System.Collections.Generic;
using System.Text;
using qf4net;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Services;
using System.Diagnostics;
using System.Threading;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Process
{
    public class ProcessHST : Seagate.AAS.Parsel.Equipment.Process
    {

        public event EventHandler OnInit;
        public event EventHandler OnInitStart;

        // Nested declarations -------------------------------------------------

        public enum AP
        {              
            InputTurnStationProcess,
            InputStationProcess,
            BufferStationProcess,
            TestProbeProcess,
            PrecisorStationProcess,
            InputEEProcess,
            OutputEEProcess,
            OutputStation,
            OutputTurnStation,
            MonitorProcess,
            RunTestScriptProcess,
            FileManageProcess,
            DataManagingProcess,
            TDFDataRunProcess,
            ClampRotateProcess,
            ImageFileManageProcess,
            FISGetputMonitoringProcess
        }

        // Member variables ----------------------------------------------------

        private HSTWorkcell _workcell;

        private bool _isErrorState;
        private bool _isIdleState;
        private bool _isRunState;
        private bool _isPauseState;
        private bool _isStepMode;
        private bool _isCarrierCyclestop;
        private bool _isUnload;
        private bool _isInit;
        private bool _autoRunAfterInit;
        private Thread _checkHomeDoneThread;
        private OperationMode _lastActiveRunMode = OperationMode.Auto;        

        private InputTurnStationProcess _InputTurnStationProcess;
        private InputStationProcess _InputStationProcess;
        private InputEEProcess _inputEEprocess;
        private TestProbeProcess _testProbeProcess;
        private PrecisorStationProcess _precisorStationProcess;
        private OutputEEProcess _outputEEProcess;
        private OutputStationProcess _outputStationProcess;
        private OutputTurnStationProcess _outputTurnStationProcess;
        private MonitorProcess _monitorProcess;
        private RunTestScriptProcess _runTestScriptProcess;
        private FileManageProcess _fileManageProcess;
        private DataManagingProcess _dataManagingProcess;
        private TDFDataRunProcess _tdfDataRunProcess;
        private OutputClampRotateProcess _clampAndRotateProcess;
        private ImageFileManageProcess _imageFileManageProcess;
        private GetputMonitoringProcess _fisGetputMonitoringProcess;

        private bool _disposed;

        private MonitorIOState _monitorIOState;

        public ProcessHST(HSTWorkcell Workcell)
            : base((Workcell)Workcell)
        {
            _workcell = Workcell;
            base.workcell = Workcell;            
            _workcell.Aborted += new EventHandler(_workcell_Aborted);            

            _InputTurnStationProcess = new InputTurnStationProcess(_workcell, "InputTurnStation", "InputTurnStation");
            _InputStationProcess = new InputStationProcess(_workcell, "InputStation", "InputStation");
            _testProbeProcess = new TestProbeProcess(_workcell, "TestProbe", "TestProbe");
            _precisorStationProcess = new PrecisorStationProcess(_workcell, "PrecisorStation", "PrecisorStation");
            _inputEEprocess = new InputEEProcess(_workcell, "InputEE", "InputEE");
            _outputEEProcess = new OutputEEProcess(_workcell, "OutputEE", "OutputEE");
            _outputStationProcess = new OutputStationProcess(_workcell, "OutputStation", "OutputStation");
            _outputTurnStationProcess = new OutputTurnStationProcess(_workcell, "OutputTurnStation", "OutputTurnStation");
            _monitorProcess = new MonitorProcess(_workcell, "MonitorProcess", "MonitorProcess");
            _runTestScriptProcess = new RunTestScriptProcess(_workcell, "RunTestScriptProcess", "RunTestScriptProcess");
            _fileManageProcess = new FileManageProcess(_workcell, "FileManageProcess", "FileManageProcess");
            _dataManagingProcess = new DataManagingProcess(_workcell, "DataManagingProcess", "DataManagingProcess");
            _tdfDataRunProcess = new TDFDataRunProcess(_workcell, "TDFDataRunProcess", "TDFDataRunProcess");
            _clampAndRotateProcess = new OutputClampRotateProcess(_workcell, "ClampAndRotateProcess", "ClampAndRotateProcess");
            _imageFileManageProcess = new ImageFileManageProcess(_workcell, "ImageFileManageProcess", "ImageFileManageProcess");
            _fisGetputMonitoringProcess = new GetputMonitoringProcess(_workcell, "GetputMonitoringProcess", "GetputMonitoringProcess");
            _monitorIOState = new MonitorIOState(_workcell.HSTSettings);
        }

        // Properties ----------------------------------------------------------
        public bool IsErrorState
        {
            get { return _isErrorState; }
            set { _isErrorState = value; }
        }

        public bool IsInit
        {
            get { return _isInit; }
            set { _isInit = value; }
        }

        public bool IsIdleState
        {
            get { return _isIdleState; }
            set { _isIdleState = value; }
        }

        public bool IsRunState
        {
            get { return _isRunState; }
            set { _isRunState = value; }
        }

        public bool IsPauseState
        {
            get { return _isPauseState; }
            set { _isPauseState = value; }
        }

        public bool IsStepMode
        {
            get { return _isStepMode; }
            set { _isStepMode = value; }
        }

        public bool IsCarrierCycleStop
        {
            get { return _isCarrierCyclestop; }
            set { _isCarrierCyclestop = value; }
        }

        public bool IsUnload
        {
            get { return _isUnload; }
            set { _isUnload = value; }
        }

        public override void Initialize()
        {
            // Mask here and thread will not run

            RegisterActiveProcess((int)AP.MonitorProcess, _monitorProcess);
            RegisterActiveProcess((int)AP.InputTurnStationProcess, _InputTurnStationProcess);
            RegisterActiveProcess((int)AP.InputStationProcess, _InputStationProcess);
            RegisterActiveProcess((int)AP.InputEEProcess, _inputEEprocess);
            RegisterActiveProcess((int)AP.TestProbeProcess, _testProbeProcess);
            RegisterActiveProcess((int)AP.PrecisorStationProcess, _precisorStationProcess);
            RegisterActiveProcess((int)AP.OutputEEProcess, _outputEEProcess);
            RegisterActiveProcess((int)AP.OutputStation, _outputStationProcess);
            RegisterActiveProcess((int)AP.OutputTurnStation, _outputTurnStationProcess);
            RegisterActiveProcess((int)AP.RunTestScriptProcess, _runTestScriptProcess);
            RegisterActiveProcess((int)AP.FileManageProcess, _fileManageProcess);
            RegisterActiveProcess((int)AP.DataManagingProcess, _dataManagingProcess);
            RegisterActiveProcess((int)AP.TDFDataRunProcess, _tdfDataRunProcess);
            RegisterActiveProcess((int)AP.ClampRotateProcess, _clampAndRotateProcess);
            RegisterActiveProcess((int)AP.ImageFileManageProcess, _imageFileManageProcess);
            RegisterActiveProcess((int)AP.FISGetputMonitoringProcess, _fisGetputMonitoringProcess);
            base.Initialize();
        }

        public override void Startup()
        {
            try
            {
            }
            catch
            {
            }

            base.Startup();
        }

        public override void Dispose()
        {
            _disposed = true;
            if (_checkHomeDoneThread != null)
                _checkHomeDoneThread.Join();
            base.Dispose();
        }

        public InputTurnStationProcess InputTurnStationProcess
        { get { return _InputTurnStationProcess; } }

        public InputStationProcess InputStationProcess
        { get { return _InputStationProcess; } }

        public InputEEProcess InputEEProcess
        { get { return _inputEEprocess; } }

        public TestProbeProcess TestProbeProcess
        { get { return _testProbeProcess; } }

        public PrecisorStationProcess PrecisorStationProcess
        { get { return _precisorStationProcess; } }

        public OutputEEProcess OutputEEProcess
        { get { return _outputEEProcess; } }

        public OutputStationProcess OutputStationProcess
        { get { return _outputStationProcess; } }

        public OutputTurnStationProcess OutputTurnStationProcess
        { get { return _outputTurnStationProcess; } }

        public MonitorProcess MonitorProcess
        { get { return _monitorProcess; } }

        public RunTestScriptProcess RuntestScriptProcess
        { get { return _runTestScriptProcess; } }

        public MonitorIOState MonitorIOState
        { get { return _monitorIOState; }
          set { _monitorIOState = value; }
        }
       
        public FileManageProcess FileManageProcess
        {   get { return _fileManageProcess; }
            set { _fileManageProcess = value; }
        }
        public DataManagingProcess DataManagingProcess
        {
            get { return _dataManagingProcess; }
            set { _dataManagingProcess = value; }
        }
        public TDFDataRunProcess TDFDataRunProcess
        {
            get { return _tdfDataRunProcess; }
            set { _tdfDataRunProcess = value; }
        }

        public OutputClampRotateProcess ClampAndRotateProcess
        {
            get { return _clampAndRotateProcess; }
            set { _clampAndRotateProcess = value; }
        }

        public ImageFileManageProcess ImageManageProcess
        {
            get { return _imageFileManageProcess; }
            set { _imageFileManageProcess = value; }
        }

        public GetputMonitoringProcess FisGetputMonitoringProcess
        { 
            get { return _fisGetputMonitoringProcess; }
            set { _fisGetputMonitoringProcess = value; }
        }

        public void PauseMachine()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    if (_checkHomeDoneThread != null && _checkHomeDoneThread.IsAlive)
                    {
                        _checkHomeDoneThread.Abort();
                    }
                    Pause();
                }
                catch //(Exception ex)
                { }
            });
        }        

        /// <summary>
        /// Trigger initialization process for the machine.
        /// </summary>
        public void InitializeMachine(bool isAutoRun)
        {
            if (_monitorProcess.HasError && !HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated &&
                !HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated)
                throw new Exception("Please clear monitor error first");
            ThreadPool.QueueUserWorkItem(delegate
               {
                   Thread.Sleep(1000);
                   //lock (ServiceManager.ErrorHandler)
                   {
                       try
                       {
                           ServiceManager.ErrorHandler.AutoClearMessages();
                       }
                       catch (Exception ex)
                       {
                           Console.Beep();
                       }
                   }
                   if (OnInitStart != null)
                       OnInitStart(this, new EventArgs());
                   _autoRunAfterInit = isAutoRun;
                   _isInit = true;
                   _workcell.IsMachineHomed = false;
                   if (_checkHomeDoneThread != null && _checkHomeDoneThread.IsAlive)
                   {
                       _checkHomeDoneThread.Abort();
                   }

                   // Make all not init done.
                   foreach (ActiveProcess p in this.ActiveProcess.Values)
                   {
                       ActiveProcessHST clp = (ActiveProcessHST)p;
                       clp.IsStateInitialized = false;
                   }
                   Stop();
                   while (true)
                   {
                       Thread.Sleep(10);
                       bool isBreak = true;
                       foreach (ActiveProcessHST activeHST in this.activeObjects.Values)
                       {
                           if (activeHST.IsNonIdleProcess == false && activeHST.CurrentStateName != "StateIdle"
                               && activeHST.IsNonIdleProcess == false && activeHST.CurrentStateName != "StateError")
                               isBreak = false;
                       }
                       if (isBreak)
                           break;
                   }
                   Thread.Sleep(200);

                   Start();
                   Thread.Sleep(200);

                   _checkHomeDoneThread = new Thread(DoCheckInitCompleted);
                   _checkHomeDoneThread.Name = "ProcessInit";
                   _checkHomeDoneThread.IsBackground = true;
                   _checkHomeDoneThread.Start();

                   Thread.Sleep(200);
               });             
        }

        private void DoCheckInitCompleted()
        {
            try
            {
                _workcell.DisplayTitleMessage("Initializing system...");                
                Log.Info(this, "{0}, system initialization started", LoggerCategory.SystemInitializationStarted);

                while (true)
                {
                    if (_disposed)
                        return;
                    try
                    {                                                                    
                        bool exit = true;
                        // If all processes are initialized, then exit and stop machine. (Except Non-idle process such MonitorProcess).
                        foreach (ActiveProcess p in this.ActiveProcess.Values)
                        {
                            ActiveProcessHST clp = (ActiveProcessHST)p;
                            if (clp.IsStateInitialized == false && !clp.IsNonIdleProcess)
                            {
                                exit = false;
                            }
                        }
                        if (_isInit & exit)
                            break;
                    }
                    catch(Exception ex)
                    {
                        // Do nothing
                    }
                    
                }
                Stop();                
                Log.Info(this, "{0}, System initialization ended", LoggerCategory.SystemInitializationEnded);
                
                _lastActiveRunMode = OperationMode.Auto;     // Reset last active mode to Auto
                _isInit = false;               
                _workcell.IsMachineHomed = true;
                _workcell.DisplayTitleMessage("System Initialized");
                if (OnInit != null)
                    OnInit(this, null);

                // Auto run
                if (_autoRunAfterInit)
                    Start();
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Exception in DoCheckInitCompleted", ex);
            }
        }

        /// <summary>
        ///     Initializes the system and perform homing
        /// </summary>
        public void InitializeSystem()
        {
            Start();
        }

        /// <summary>
        ///     Reset system initialization flag
        /// </summary>
        public void ResetSystemInit()
        {
            foreach (var ap in activeObjects.Values)
            {
                var activeProcess = ap as ActiveProcessHST;
                if (activeProcess != null)
                {
                    activeProcess.ResetInit();
                }
            }
        }


        // Internal methods ----------------------------------------------------
        protected override QState StateProcess(IQEvent qEvent)
        {
            if (qEvent.IsSignal(Active.SigPause))
            {
                _isPauseState = true;
            }

            return base.StateProcess(qEvent);
        }

        protected override QState StateStartup(IQEvent qEvent)
        {
            if (qEvent.IsSignal(SigInitialized))
            {
            }
            return base.StateStartup(qEvent);
        }

        protected override QState StatePause(IQEvent qEvent)
        {
            if (qEvent.IsSignal(SigResume))
            {
                _isPauseState = false;
            }
            return base.StatePause(qEvent);
        }

        protected override QState StateActive(IQEvent qEvent)
        {
            if (qEvent.IsSignal(QSignals.Entry))
            {
            }
            if (qEvent.IsSignal(QSignals.Exit))
            {
                if (_isInit)
                    _lastActiveRunMode = OperationMode.Auto;
                else
                    _lastActiveRunMode = HSTMachine.Workcell.HSTSettings.Install.OperationMode;  // Update last active run mode
            }
            if (qEvent.IsSignal(Active.SigStop))
            {
                Trace.WriteLine(string.Format("Process {0} STOP", this.workcell.Name));
                foreach (Active active in this.activeObjects.Values)
                {
                    active.PostFIFO(new QEvent(Active.SigStop));
                }
                // This loop wait for all active process to be in idle state before proceed. 
                while (true)
                {
                    if (_disposed)
                        return null;
                    Thread.Sleep(10);
                    bool isBreak = true;                    
                    foreach (ActiveProcessHST activeHST in this.activeObjects.Values)
                    {
                        if (activeHST.IsNonIdleProcess == false && activeHST.CurrentStateName != "StateIdle")
                            isBreak = false;
                    }
                    if (isBreak)
                        break;
                }
            }
            return base.StateActive(qEvent); // pass the event to base for further processing
        }

        protected override QState StateIdle(IQEvent qEvent)
        {
            if (qEvent.IsSignal(QSignals.Entry))
            {
                _isRunState = false;
                _isIdleState = true;
                _isPauseState = false;
                _workcell.DisplayTitleMessage("System Idle");

                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelOriginatorSignalName.Text = "";
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelOriginatorSignalSource.Text = "";
                            });
                        }
                    }
                }
            }
            if (qEvent.IsSignal(QSignals.Exit))
            {
                _isRunState = true;
                _isIdleState = false;
                _isPauseState = false;
            }
            if (qEvent.IsSignal(Active.SigStart))
            {
                if (CommonFunctions.Instance.SystemInitializationCompleted)
                {
                    _workcell.DisplayTitleMessage("System Run");
                }

                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = false;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = false;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = false;

                                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().Enabled = false;
                                HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel().Enabled = false;

                                HSTMachine.Instance.MainForm.getPanelNavigation().btnSetup.Enabled = false;
                                HSTMachine.Instance.MainForm.getPanelNavigation().btnRecipe.Enabled = false;
                                HSTMachine.Instance.MainForm.getPanelNavigation().btnDiagnostic.Enabled = false;
                            });
                        }
                    }
                }
                
                //lock (ServiceManager.ErrorHandler)
                {
                    try
                    {
                        ServiceManager.ErrorHandler.AutoClearMessages();
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        Console.Beep();
                    }
                }
            }            
            return base.StateIdle(qEvent);            
        }
        
        void _workcell_Aborted(object sender, EventArgs e)
        {
            if (_checkHomeDoneThread != null && _checkHomeDoneThread.IsAlive)
            {
                _checkHomeDoneThread.Abort();
            }
        }     
        
        
        
           
    }
}
