//
//  ?Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Timers;

using qf4net;
using Seagate.AAS.Parsel;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Equipment.HGA.UI.Utils;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Exceptions;
using System.Windows.Forms;
//using Seagate.AAS.Parsel.Device.ADLink8164;
//using Seagate.AAS.Parsel.Device.LightTower;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Process
{
    /// <summary>
    /// Represent an error message that has internal self error monitoring feature.
    /// 
    /// If the OK button on the Error Prompt is pressed, depending on whether the ErrorResponded
    /// event is handled or not. If it is handled, the event is being sent. If not, it try to reset
    /// the error by calling the ClearErrorMessage.
    /// 
    /// </summary>
    /// 
    public delegate void MessageInputHandler();

    public class MonitorErrorMessage : ErrorMessage
    {
        // Nested declarations -------------------------------------------------    
        public event ErrorInputHandler ErrorResponded;

        // Member variables ----------------------------------------------------
        private HSTWorkcell _workcell;
        private DigitalIOState _workingState;
        private bool _isStopMachine;
        private static List<MonitorErrorMessage> _stopMachineErrors;
        private static object _locker = new object();
        private bool _isErrorAdded;
        private static List<MonitorErrorMessage> _list = new List<MonitorErrorMessage>();
        private static bool _disposed;
        private string _errorInfo = "";
        private string _workInstruction = "";
        // Constructors & Finalizers -------------------------------------------
        public MonitorErrorMessage(HSTWorkcell workcell, bool stopMacineOnError, string src, ButtonList lst)
            : base(src, lst)
        {
            _workcell = workcell;
            _isStopMachine = stopMacineOnError;
            _stopMachineErrors = new List<MonitorErrorMessage>();

            _list.Add(this);
            IsErrorCleared = true;
        }

        public static void Dispose()
        {
            _disposed = true;
        }

        static MonitorErrorMessage()
        {
        }

        public static void Checking()
        {
            try
            {
                foreach (MonitorErrorMessage mem in _list)
                {
                    if (_disposed)
                        return;
                    mem.CheckDigitalInputState(mem._workingState);
                }
            }
            catch (Exception ex)
            {
                Console.Beep();
            }
        }

        public void SetErrorInfo(string ErrorInfo)
        {
            _errorInfo = ErrorInfo;
            _fullDescription = ErrorInfo + Environment.NewLine + "Work Insruction: " + _workInstruction +
                Environment.NewLine + "Process: MonitorProcess" +
                Environment.NewLine + "State: Monitoring" +
                Environment.NewLine + "Timestamp: " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
        }

        public void SetWorkInstruction(string workInstruction)
        {
            _workInstruction = workInstruction;
            _fullDescription = _errorInfo + Environment.NewLine + "Work Insruction: " + workInstruction +
                Environment.NewLine + "Process: MonitorProcess" +
                Environment.NewLine + "State: Monitoring" +
                Environment.NewLine + "Timestamp: " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
        }

        // Properties ----------------------------------------------------------
        public bool IsStopMachine
        {
            get { return _isStopMachine; }
        }

        private IDigitalInput _digitalInput;
        /// <summary>
        /// Gets/sets digital input to be checked.
        /// </summary>
        public IDigitalInput DigitalInput
        {
            get { return _digitalInput; }
            set { _digitalInput = value; }
        }

        private bool _isAutoClearError = false;
        /// <summary>
        /// If true, error message will be cleared when the digital
        /// input is at same state as defined WorkigState property.
        /// </summary>
        public bool IsAutoClearError
        {
            get { return _isAutoClearError; }
            set { _isAutoClearError = value; }
        }

        /// <summary>
        /// Gets/sets working state that will be used to check against
        /// the defined DigitalInput property.
        /// </summary>
        public DigitalIOState WorkingState
        {
            get { return _workingState; }
            set { _workingState = value; }
        }

        public bool TriggerError { get; set; }

        public bool IsErrorCleared { get; set; }
        // Methods -------------------------------------------------------------
        public override void CreateErrorPanel()
        {
        }

        public override void OnInput()
        {
            // If someone handle the event themselve, they should clear the error by themselve.
            if (ErrorResponded != null)
            {
                ErrorResponded(this);
            }
            else
            {
                ClearErrorMessage();
            }
        }

        public void PostErrorMessage()
        {
            if (_isErrorAdded == false)
            {
                IsErrorCleared = false;
                if (ServiceManager.ErrorHandler.ErrorList.Contains(this) == false)
                {
                    ServiceManager.ErrorHandler.RegisterMessage(this as IErrorMessage);

                    // Log error message.                   
                    Log.Error(this, "{0}, UniqueID:{1} {2}", LoggerCategory.AlarmOccured,
                                            this.UniqueID.ToString(),
                                            this.Source + ":" + this.Text);
                    if (!IsErrorCleared)
                    {
                        Log.Maintenance(this, "{0}", this.Source + "," + this.ToString() + "," + this.Text + "," + LoggerCategory.AlarmOccured);
                        Log.Maintenance(this, "{0},{1},{2}", this.Source, this.Text, LoggerCategory.AlarmOccured);
                    }

                }
                _isErrorAdded = true;
            }
        }

        public void ClearErrorMessage()
        {
            IsErrorCleared = true;
            TriggerError = false;
            if (_stopMachineErrors.Contains(this) == true)
            {
                lock (_locker)
                    _stopMachineErrors.Remove(this);
            }
            if (_stopMachineErrors.Count == 0)
            {
                _workcell.Process.IsErrorState = false;
            }

            if (_digitalInput != null)
            {
                int test = 0;
                {
                    _isErrorAdded = false;
                    if (ServiceManager.ErrorHandler.ErrorList.Contains(this) == true)
                    {
                        ServiceManager.ErrorHandler.UnRegisterMessage(this as IErrorMessage);

                        // Log error message cleard.       
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "{0}, UniqueID:{1} {2}", LoggerCategory.AlarmCleared,
                                                this.UniqueID.ToString(),
                                                this.Source + ":" + this.Text);
                        }
                    }
                }
            }
            else
            {
                if (_isErrorAdded)
                {
                    _isErrorAdded = false;
                    ServiceManager.ErrorHandler.UnRegisterMessage(this as IErrorMessage);

                    // Log error message cleard.  
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, UniqueID:{1} {2}", LoggerCategory.AlarmCleared,
                                                this.UniqueID.ToString(),
                                                this.Source + ":" + this.Text);
                    }
                }
            }
        }

        // Internal methods ----------------------------------------------------
        private void CheckDigitalInputState(DigitalIOState workingState)
        {
            lock (ServiceManager.ErrorHandler)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    if (this.TriggerError)
                        PostErrorMessage();
                    else
                    {
                        if (_isAutoClearError)
                        {
                            ClearErrorMessage();
                        }
                    }
                    return;
                }

                if (_digitalInput == null)
                    return;

                _workingState = workingState;

                if (_digitalInput.Get() != workingState /*&& !isBypassedOn*/)
                {
                    PostErrorMessage();
                }
                else
                {
                    if (_isAutoClearError)
                    {
                        ClearErrorMessage();
                    }
                }
            }
        }

        #region IComparable Members

        public override int CompareTo(object obj)
        {
            if (obj is ErrorMessage)
            {
                ErrorMessage m = (ErrorMessage)obj;
                return this.Priority.CompareTo(m.Priority);
            }
            return 0;
        }

        #endregion
    }

    /// <summary>
    /// Monitor error messages.
    /// </summary>
    public class MonitorProcess : ActiveProcessHST
    {
        // Public event handler
        public event EventHandler TowerLightAndDoorStatusEvent;
        public event EventHandler EMOReset;

        // Member variables ----------------------------------------------------
        protected QState stateMonitoring;

        private HSTIOManifest ioManifest;
        private QTimer _stateTimer;
        private TimeSpan _loopTimeOut;
        private MonitorController _controller;
        //        private bool _byPassSwitchIsOn = false;

        //declare monitor process type
        private MonitorErrorMessage mainDoorIsOpen;
        private MonitorErrorMessage emoTriggered;
        private MonitorErrorMessage mainDoorIsNotLocked;
        private MonitorErrorMessage mainAirPressureError;
        private MonitorErrorMessage wristGroundError;
        private MonitorErrorMessage groundMasterError;
        private MonitorErrorMessage motionError;
        private MonitorErrorMessage safetyControllerPresentError;
        private MonitorErrorMessage ticTriggeringError;
        private MonitorErrorMessage performanceTriggeringError;
        private MonitorErrorMessage probeFunctionalTestTriggeringError;
        private MonitorErrorMessage samplingOverLimitTriggeringError;
        private MonitorErrorMessage cccMachineTriggeringError;
        //Have not implemeted yet
        private MonitorErrorMessage seaTrackCommunicationError;
        private MonitorErrorMessage conveyorNotTurnedON;

        //Warning
        private MonitorErrorMessage conveyorCongestion;

        private bool enableAmberTowerLight = false;
        private bool enableRedTowerLight = false;
        private bool enableSiren = false;
        private bool enablePowerLED = false;
        private bool enableAutomationRunLED = false;
        private bool enableAutomationStopLED = false;
        private bool blinkAutomationStopLED = false;
        private bool preparetoStopAutomation = false;
        private bool stopAutomation = false;
        private bool mainDoorIsNotLockedPrev = false;  // Lai: for maindoor not lock error checking purpose
        private bool isDoorLockPrev = false; // Lai: for deciding whether to disable or axes or not.
        private bool emoTriggeredPrev = false;
        private bool prepareEMOReset = false;
        private bool _isInMotionAlarmProcess = false;
        private int SirenCount = 0;

        private System.Timers.Timer conveyorCongestionTimer;
        private DateTime _lastConveyorCongestionTime = DateTime.Now;

        private IDigitalOutput _doSoftStartUp;
        private int _doorUnlockCounterCumulative = 0;

        // Constructors & Finalizers -------------------------------------------
        public MonitorProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this.ioManifest = (HSTIOManifest)workcell.IOManifest;
            _isNonIdleProcess = true;
            _stateTimer = new QTimer(this);

            conveyorCongestionTimer = new System.Timers.Timer(1000);
            conveyorCongestionTimer.Stop();

            // initialize HSM delegates
            stateMonitoring = new QState(this.Monitoring);
        }

        public MonitorController Controller
        {
            get { return _controller; }
        }

        public bool HasWarning
        {
            get
            {
                try
                {
                    bool foundWarning = false;
                    foreach (object item in ServiceManager.ErrorHandler.ErrorList)
                    {
                        if (item is MonitorErrorMessage)
                        {
                            MonitorErrorMessage errMsg = (MonitorErrorMessage)item;
                            if (!errMsg.IsStopMachine)
                                foundWarning = true;
                        }
                        if (item is MonitorErrorMessage)
                        {
                            MonitorErrorMessage errMsg = (MonitorErrorMessage)item;
                            if (errMsg.IsStopMachine)
                                foundWarning = false;
                        }
                    }
                    return foundWarning;
                }
                catch (Exception ex)
                {
                    Console.Beep();
                    return false;
                }
            }
        }

        public bool HasError
        {
            get
            {
                try
                {
                    bool foundMonErr = false;
                    foreach (object item in ServiceManager.ErrorHandler.ErrorList)
                    {
                        if (item is MonitorErrorMessage)
                        {
                            MonitorErrorMessage errMsg = (MonitorErrorMessage)item;
                            if (errMsg.IsStopMachine)
                            {
                                foundMonErr = true;
                                return foundMonErr;
                            }
                        }
                    }
                    foundMonErr = false;
                    return foundMonErr;
                }
                catch (Exception ex)
                {
                    Console.Beep();
                    return false;
                }
            }
        }

        public bool TestError1;
        public bool TestError2;

        // Methods -------------------------------------------------------------
        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()

            _controller = new MonitorController(_workcell, "MM", "Monitor Controller");
            _controller.InitializeController();
            _doSoftStartUp = _workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Soft_Start_Up);
            base.Start(priority);
        }

        public override void Dispose()
        {
            MonitorErrorMessage.Dispose();
            base.Dispose();
        }

        // Internal Methods -------------------------------------------------------------
        protected override void InitializeStateMachine()
        {
            base.InitializeStateMachine();
            TransitionTo(stateMonitoring);

            #region Error messages.
            ButtonList lst = new ButtonList(ErrorButton.NoButton, ErrorButton.OK, ErrorButton.NoButton);
            string src = "HST.Monitor";


            mainDoorIsNotLocked = new MonitorErrorMessage(_workcell, true, src, lst);
            mainDoorIsNotLocked.Format(HSTErrors.DoorNotLockError.ToString());
            mainDoorIsNotLocked.Priority = 1;
            mainDoorIsNotLocked.TriggerError = false;
            mainDoorIsNotLocked.IsAutoClearError = true;
            mainDoorIsNotLocked.Source = ((int)HSTErrors.DoorNotLockError).ToString("000000");
            mainDoorIsNotLocked.SetWorkInstruction("Work instruction for door not lock Error is TBD");

            groundMasterError = new MonitorErrorMessage(_workcell, true, src, lst);
            groundMasterError.Format(HSTErrors.GroundMasterError.ToString());
            groundMasterError.Priority = 14;
            groundMasterError.DigitalInput = ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Ground_Master);
            groundMasterError.WorkingState = DigitalIOState.Off;
            groundMasterError.IsAutoClearError = false;
            groundMasterError.Source = ((int)HSTErrors.GroundMasterError).ToString("000000");
            groundMasterError.SetWorkInstruction("Work instruction for ground Master Error is TBD");

            emoTriggered = new MonitorErrorMessage(_workcell, true, src, lst);
            emoTriggered.Format(HSTErrors.EMOTriggeredError.ToString());
            emoTriggered.Priority = 8;
            emoTriggered.TriggerError = false;
            emoTriggered.IsAutoClearError = false;
            emoTriggered.Source = ((int)HSTErrors.EMOTriggeredError).ToString("000000");
            emoTriggered.SetWorkInstruction("Work instruction for EMO tiggered error is TBD");

            safetyControllerPresentError = new MonitorErrorMessage(_workcell, true, src, lst);
            safetyControllerPresentError.Format(HSTErrors.SafetyControllerPresentError.ToString());
            safetyControllerPresentError.Priority = 15;
            safetyControllerPresentError.TriggerError = false;
            safetyControllerPresentError.IsAutoClearError = false;
            safetyControllerPresentError.Source = ((int)HSTErrors.SafetyControllerPresentError).ToString("000000");
            safetyControllerPresentError.SetWorkInstruction("Work instruction for Safety Controller Present Error is TBD");

            ticTriggeringError = new MonitorErrorMessage(_workcell, true, src, lst);
            ticTriggeringError.Format(HSTErrors.TICTriggeringError.ToString());
            ticTriggeringError.Priority = 15;
            ticTriggeringError.TriggerError = false;
            ticTriggeringError.IsAutoClearError = false;
            ticTriggeringError.Source = ((int)HSTErrors.TICTriggeringError).ToString("000000");

            performanceTriggeringError = new MonitorErrorMessage(_workcell, true, src, lst);
            performanceTriggeringError.Format(HSTErrors.PerformanceTriggeringError.ToString());
            performanceTriggeringError.Priority = 1;
            performanceTriggeringError.TriggerError = false;
            performanceTriggeringError.IsAutoClearError = false;
            performanceTriggeringError.Source = ((int)HSTErrors.PerformanceTriggeringError).ToString("000000");
            performanceTriggeringError.SetWorkInstruction("Please call technician to check tesing performance because running result hit triggering limitation percentage.");

            probeFunctionalTestTriggeringError = new MonitorErrorMessage(_workcell, true, src, lst);
            probeFunctionalTestTriggeringError.Format(HSTErrors.ProbeFunctionalTestTriggeringError.ToString());
            probeFunctionalTestTriggeringError.Priority = 1;
            probeFunctionalTestTriggeringError.TriggerError = false;
            probeFunctionalTestTriggeringError.IsAutoClearError = false;
            probeFunctionalTestTriggeringError.Source = ((int)HSTErrors.ProbeFunctionalTestTriggeringError).ToString("000000");
            probeFunctionalTestTriggeringError.SetWorkInstruction("Please call technician to check resistance measurement board because known resistance reading was failed.");

            samplingOverLimitTriggeringError = new MonitorErrorMessage(_workcell, true, src, lst);
            samplingOverLimitTriggeringError.Format(HSTErrors.ErrorCodeTriggeringError.ToString());
            samplingOverLimitTriggeringError.Priority = 1;
            samplingOverLimitTriggeringError.TriggerError = false;
            samplingOverLimitTriggeringError.IsAutoClearError = false;
            samplingOverLimitTriggeringError.Source = ((int)HSTErrors.SamplingTriggeringError).ToString("000000");
            samplingOverLimitTriggeringError.SetWorkInstruction("Part status {A} for on disk sampling over target, Please call technician to verify!");

            cccMachineTriggeringError = new MonitorErrorMessage(_workcell, true, src, lst);
            cccMachineTriggeringError.Format(HSTErrors.ANCTriggeringError.ToString());
            cccMachineTriggeringError.Priority = 1;
            cccMachineTriggeringError.TriggerError = false;
            cccMachineTriggeringError.IsAutoClearError = false;
            cccMachineTriggeringError.Source = ((int)HSTErrors.ANCTriggeringError).ToString("000000");
            cccMachineTriggeringError.SetWorkInstruction("ANC Triggering");

            motionError = new MonitorErrorMessage(_workcell, true, src, lst);
            motionError.Format(HSTErrors.MotionAxitError.ToString());
            motionError.Priority = 1;
            motionError.TriggerError = false;
            motionError.IsAutoClearError = false;
            motionError.Source = ((int)HSTErrors.MotionAxitError).ToString("000000");
            motionError.SetWorkInstruction("Aerotech motion error");


            #endregion

            // WARNING MEESSAGE
            #region Warnning messages
            mainDoorIsNotLocked.TriggerError = false;
            mainDoorIsNotLocked.IsAutoClearError = false;
            mainDoorIsNotLocked.Source = ((int)HSTErrors.DoorNotLockError).ToString("000000");
            mainDoorIsNotLocked.SetWorkInstruction("Work instruction for door not lock Error is TBD");

            conveyorCongestion = new MonitorErrorMessage(_workcell, false, src, lst);
            conveyorCongestion.Format(HSTErrors.OutputConveyorCongestionError.ToString());
            conveyorCongestion.Priority = 2;
            conveyorCongestion.TriggerError = false;
            conveyorCongestion.IsAutoClearError = false;
            conveyorCongestion.SetWorkInstruction("Work instruction for Conveyor Congestion is TBD");

            #endregion


        }

        public void HandleExceptionRaisedByTestProbeUtility(Exception ex)
        {
            ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
            TransitionToErrorState(btnlst, ex);
        }

        protected QState Monitoring(IQEvent qEvent)
        {
            var groundMonitorStatusWorking = true;
            //LogMessage("Monitoring", qEvent);
            if (qEvent.IsSignal(SigStateJob))
            {
                if (HSTWorkcell.terminatingHSTApps)
                {
                    return null;
                }

                //Show alert message grounding
                try
                {
                    groundMonitorStatusWorking = Controller.isGroundMasterWorking();
                    _workcell.GroundMonitoringStatus = groundMonitorStatusWorking;
                }
                catch (Exception)
                {
                }

                if (!groundMonitorStatusWorking)
                {
                    _workcell.SendGroundMasterStatusMessage();
                    groundMasterError.PostErrorMessage();

                    if (!_workcell.GroundMonitoringErrActivated) _workcell.GroundMonitoringErrActivated = true;
                }
                else
                {
                    groundMasterError.ClearErrorMessage();
                    _workcell.SendGroundMasterStatusMessage();
                }

                if (Controller.IsCriticalTriggeringActivated && !_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted)
                {
                    _workcell.SendMachinePerformanceStatusMessage();
                    performanceTriggeringError.PostErrorMessage();
                }
                else
                {
                    _workcell.SendMachinePerformanceStatusMessage();
                    performanceTriggeringError.ClearErrorMessage();
                }

                if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated && !HSTMachine.Workcell.OffAlertMessage)
                {
                    _workcell.SendFunctionalTestStatusMessage();
                    probeFunctionalTestTriggeringError.PostErrorMessage();
                }
                else
                {
                    _workcell.SendFunctionalTestStatusMessage();
                    probeFunctionalTestTriggeringError.ClearErrorMessage();
                }

                if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.SamplingTriggeringOverPercentageActivate && !HSTMachine.Workcell.OffAlertMessage)
                {
                    _workcell.SendSamplingOverTargetTriggeringStatusMessage();
                    samplingOverLimitTriggeringError.PostErrorMessage();
                }
                else
                {
                    _workcell.SendSamplingOverTargetTriggeringStatusMessage();
                    samplingOverLimitTriggeringError.ClearErrorMessage();
                }

                if (TowerLightAndDoorStatusEvent != null)
                    TowerLightAndDoorStatusEvent(this, new TowerLightAndDoorLockStatusEventArgs(enableAmberTowerLight, enableRedTowerLight, _controller.isDoorLocked()));

                if (_controller.isAutomationStopButtonPressed() || ((_controller.isAutomationPrepareToStop()) && _controller.isAutomationEnabled()))
                {
                    HSTWorkcell.disableBoundaryCheck = false;
                    if (HSTMachine.Workcell.Process.IsRunState)
                    {
                        PublishSignal(new QEvent(HSTWorkcell.SigStopMachineRun));
                    }
                    preparetoStopAutomation = true;
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "preparetoStopAutomation. isAutomationStopButtonPressed: {0}, isAutomationPrepareToStop: {1}", _controller.isAutomationStopButtonPressed(), _controller.isAutomationPrepareToStop());
                    }
                }

                if (HSTMachine.Workcell.Process.IsRunState)
                {
                    conveyorCongestionTimer.Start();
                    conveyorCongestionTimer.Elapsed -= conveyorCongestionTimer_Elapsed;
                    conveyorCongestionTimer.Elapsed += conveyorCongestionTimer_Elapsed;
                }
                else
                {
                    conveyorCongestionTimer.Stop();
                    conveyorCongestionTimer.Elapsed -= conveyorCongestionTimer_Elapsed;
                }

                DetermineButtonsLEDState();
                DetermineTowerLightState();

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    mainDoorIsNotLocked.TriggerError = false;
                }

                if (_controller.isEMOPressed())
                {
                    emoTriggered.TriggerError = true;
                    CommonFunctions.Instance.powerOnConveyor = false;
                }
                else
                {
                    emoTriggered.TriggerError = false;
                }

                if (prepareEMOReset && _controller.isPowerOn())
                {
                    prepareEMOReset = false;
                    if (EMOReset != null)
                        EMOReset(this, new EventArgs());
                }

                if (!emoTriggered.TriggerError && emoTriggeredPrev)
                {
                    prepareEMOReset = true;
                    CommonFunctions.Instance.powerOnConveyor = true;
                    CommonFunctions.Instance.powerOffConveyor = false;
                }

                if (preparetoStopAutomation && !HSTMachine.Workcell.Process.IsRunState)
                {
                    preparetoStopAutomation = false;
                    stopAutomation = true;
                }

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    if ((Controller.isAutomationStopButtonPressed()) || stopAutomation)
                    {
                        _controller.EnableAllEndEffectorAxes(false);
                    }
                }

                DateTime currentTime = DateTime.Now;
                TimeSpan diffTime = currentTime.Subtract(_lastConveyorCongestionTime);

                if (HSTMachine.Workcell.HSTSettings.Install.ConveyorCongestionToleranceTimeLimit > 0)
                {
                    conveyorCongestion.TriggerError = ((int)diffTime.TotalSeconds >= HSTMachine.Workcell.HSTSettings.Install.ConveyorCongestionToleranceTimeLimit) ? true : false;
                }
                else
                {
                    conveyorCongestion.TriggerError = false;
                }

                List<string> errorList = _controller.getErrorList();

                if (errorList.Count > 0)
                {
                    // KA Gan: shouldn't line below be safetyControllerPresentError.TriggerError = true;  ?
                    safetyControllerPresentError.TriggerError = false;

                    string errorInfo = "";
                    for (int i = 0; i < errorList.Count; i++)
                        errorInfo = errorInfo + errorList[i] + Environment.NewLine;

                    safetyControllerPresentError.SetErrorInfo(errorInfo);
                }
                else
                {
                    safetyControllerPresentError.TriggerError = false;
                }

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    MonitorErrorMessage.Checking();
                }

                if ((mainDoorIsNotLocked.TriggerError && !mainDoorIsNotLockedPrev) ||
                    (emoTriggered.TriggerError && !emoTriggeredPrev) ||
                    HSTWorkcell.stopSystemDueToAxisError)
                {
                    // KA Gan: Change system state to Stopped if the EMO button has been pressed.
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {

                        Log.Info(this, "stopSystemProcess. mainDoorIsNotLocked.TriggerError: {0}, emoTriggered.TriggerError: {1}, stopSystemDueToAxisError: {2}, ", mainDoorIsNotLocked.TriggerError, emoTriggered.TriggerError, HSTWorkcell.stopSystemDueToAxisError);
                    }
                    if (HSTMachine.Instance.MainForm != null)
                    {
                        HSTMachine.Instance.MainForm.getPanelCommand().stopSystemProcess();
                        try
                        {

                            if (emoTriggered.TriggerError || mainDoorIsNotLocked.TriggerError)
                                _doSoftStartUp.Set(DigitalIOState.Off);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    HSTWorkcell.stopSystemDueToAxisError = false;
                }
                mainDoorIsNotLockedPrev = mainDoorIsNotLocked.TriggerError;
                emoTriggeredPrev = emoTriggered.TriggerError;

                if (enableSiren)
                    SirenCount++;

                if (SirenCount == 2)
                {
                    enableSiren = false;
                    SirenCount = 0;
                }

                isDoorLockPrev = _controller.isDoorLocked();

                try
                {
                    _controller.SendRequestToSafetyController(enableAmberTowerLight, enableRedTowerLight, enableSiren,
                    enablePowerLED, enableAutomationRunLED, enableAutomationStopLED, stopAutomation,
                    CommonFunctions.Instance.powerOnConveyor, CommonFunctions.Instance.powerOffConveyor);

                    if (stopAutomation)
                    {
                        stopAutomation = false;
                    }
                }
                catch (Exception ex)
                {
                }

                ////Get axis status
                try
                {
                    bool erroractive = false;
                    string erroraxis = string.Empty;
                    string errormsg = string.Empty;
                    var thetaStatus = _workcell.Process.PrecisorStationProcess.Controller.GetThetaAxisStatus(HSTIOManifest.Axes.Theta);
                    var xStatus = _workcell.Process.PrecisorStationProcess.Controller.GetThetaAxisStatus(HSTIOManifest.Axes.X);
                    var yStatus = _workcell.Process.PrecisorStationProcess.Controller.GetThetaAxisStatus(HSTIOManifest.Axes.Y);
                    var z1Status = _workcell.Process.PrecisorStationProcess.Controller.GetThetaAxisStatus(HSTIOManifest.Axes.Z1);
                    var z2Status = _workcell.Process.PrecisorStationProcess.Controller.GetThetaAxisStatus(HSTIOManifest.Axes.Z2);
                    var z3Status = _workcell.Process.PrecisorStationProcess.Controller.GetThetaAxisStatus(HSTIOManifest.Axes.Z3);

                    # region Get axis status
                    if (!thetaStatus.None)
                    {
                        erroractive = true;
                        erroraxis = String.Format("[{0}] Axis ", HSTIOManifest.Axes.Theta.ToString().ToUpper());
                        errormsg = thetaStatus.ToString();
                    }

                    if (!xStatus.None)
                    {
                        erroractive = true;
                        erroraxis = String.Format("[{0}] Axis ", HSTIOManifest.Axes.X.ToString().ToUpper());
                        errormsg = xStatus.ToString();
                    }

                    if (!yStatus.None)
                    {
                        erroractive = true;
                        erroraxis = String.Format("[{0}] Axis ", HSTIOManifest.Axes.Y.ToString().ToUpper());
                        errormsg = yStatus.ToString();
                    }

                    if (!z1Status.None)
                    {
                        erroractive = true;
                        erroraxis = String.Format("[{0}] Axis ", HSTIOManifest.Axes.Z1.ToString().ToUpper());
                        errormsg = z1Status.ToString();
                    }

                    if (!z2Status.None)
                    {
                        erroractive = true;
                        erroraxis = String.Format("[{0}] Axis ", HSTIOManifest.Axes.Z2.ToString().ToUpper());
                        errormsg = z2Status.ToString();
                    }

                    if (!z3Status.None)
                    {
                        erroractive = true;
                        erroraxis = String.Format("[{0}] Axis ", HSTIOManifest.Axes.Z3.ToString().ToUpper());
                        errormsg = z3Status.ToString();
                    }

                    #endregion

                    if (erroractive && !motionError.TriggerError && !_isInMotionAlarmProcess)
                    {
                        _isInMotionAlarmProcess = true;
                        ButtonList lst = new ButtonList(ErrorButton.NoButton, ErrorButton.NoButton, ErrorButton.NoButton);
                        motionError = new MonitorErrorMessage(_workcell, true, String.Format("Aerotech {0} error", erroraxis), lst);
                        motionError.Format(errormsg);
                        motionError.SetWorkInstruction("Please verify motion axis status by using <A3200 Motion Composer>, reset the error and continue run");
                        motionError.IsAutoClearError = true;
                        motionError.TriggerError = true;
                        motionError.PostErrorMessage();
                    }
                    else
                    {
                        if (!erroractive && motionError.TriggerError)
                        {
                            motionError.ClearErrorMessage();
                            _isInMotionAlarmProcess = false;
                            motionError.IsErrorCleared = true;
                        }
                    }
                }
                catch (Exception)
                {
                }


                _loopTimeOut = new TimeSpan(0, 0, 0, 0, 500); // 300 msec
                _stateTimer.FireIn(_loopTimeOut, new QEvent(SigStateJob));
                conveyorCongestionTimer.Enabled = true;
                return null;
            }
            return base.TopState;
        }

        private void DetermineButtonsLEDState()
        {
            enablePowerLED = (_controller.isPowerButtonPressed() || _controller.isPowerOn()) ? true : false;
            enableAutomationRunLED = (_controller.isAutomationEnabledButtonPressed() || _controller.isAutomationEnabled()) ? true : false;

            if (_controller.isAutomationStopButtonPressed())
            {
                enableAutomationStopLED = true;
                blinkAutomationStopLED = true;
            }
            else if (blinkAutomationStopLED && _controller.isAutomationEnabled())
                enableAutomationStopLED = !enableAutomationStopLED;
            else
            {
                enableAutomationStopLED = false;
                blinkAutomationStopLED = false;
            }
        }

        private void DetermineTowerLightState()
        {
            bool processesHasError = false;
            foreach (Active item in HSTMachine.Workcell.Process.ActiveProcess.Values)
            {
                if (((ActiveProcessHST)item).IsErrorState)
                    processesHasError = true;
            }

            if (processesHasError)
            {
                enableAmberTowerLight = true;
                enableRedTowerLight = (enableRedTowerLight) ? false : true;
                enableSiren = true;
            }
            else if (_workcell.Process.IsErrorState)
            {
                enableAmberTowerLight = false;
                enableRedTowerLight = (enableRedTowerLight) ? false : true;
                enableSiren = true;

            }
            else if (this.HasError)
            {
                enableAmberTowerLight = false;
                enableRedTowerLight = (enableRedTowerLight) ? false : true;
                enableSiren = true;
            }
            else if (this.HasWarning)
            {
                enableAmberTowerLight = (enableAmberTowerLight) ? false : true;
                enableRedTowerLight = false;
                enableSiren = false;

            }
            else if (_workcell.Process.IsIdleState && !_workcell.Process.IsErrorState)
            {
                enableAmberTowerLight = true;
                enableRedTowerLight = false;
                enableSiren = false;

            }
            else if (_workcell.Process.IsRunState)
            {
                if (_workcell.CCCMachineTriggeringActivated && HSTMachine.Workcell.Process.IsRunState)
                {
                    if (_workcell.HSTSettings.CccParameterSetting.EnableAlertMsg)
                    {
                        enableAmberTowerLight = false;
                        enableRedTowerLight = (enableRedTowerLight) ? false : true;
                        enableSiren = true;
                    }
                }
                else
                {
                    enableAmberTowerLight = false;
                    enableRedTowerLight = false;
                    enableSiren = false;
                }
            }

            if (_workcell.OffAlarm)
            {
                enableSiren = false;
            }
        }

        private void conveyorCongestionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if ((HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation) ||
            (ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_Exit_Clear).Get().Equals(DigitalIOState.Off)))
                {
                    _lastConveyorCongestionTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                // Lai: Do nothing for now
            }
        }

        public void disableTowerLightAndDoorEvent()
        {
            TowerLightAndDoorStatusEvent = null;
            Thread.Sleep(1000);
        }

        public void ClearAllForAutoStartRun()
        {
            performanceTriggeringError.ClearErrorMessage();
            probeFunctionalTestTriggeringError.ClearErrorMessage();
            samplingOverLimitTriggeringError.ClearErrorMessage();
            cccMachineTriggeringError.ClearErrorMessage();
            _workcell.CCCMachineTriggeringActivated = false;
        }

        protected override void AddProcessError()
        {
        }

    }
}
