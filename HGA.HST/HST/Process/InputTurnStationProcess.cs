using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using qf4net;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Settings;
using System.Windows.Forms;
using AutoMapper;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Process
{

    public class InputTurnStationProcess : ActiveProcessHST
    {

        private InputTurnStationController _controller;
        protected HSTWorkcell _workcell;
        private Carrier _currentInputCarrier = null;
        private CarrierSettings _currentInputCarrierSettings = null;

        #region HSM declaration
        //RunInit
        private QState stateStandbyInputTurnSection;

        //Run mode
        private QState stateRunStart;
        private QState stateDoAutoClearCarrier;
        private QState stateCheckForNextCarrier;
        private QState stateReleaseCarrierToInputTurnStation;
        private QState stateVisionInspectCarrierAfterLoad;
        private QState stateWaitForSigInputStationReady;
        private QState stateReleaseCarrier;
        private QState statePublishSigCarrierInInputStation;
        private QState stateInputTurnSectionTurnForward;
        private QState stateInputTurnSectionTurnBackward;
        private QState stateReportTICError;
        private QState stateReportCRDLError;
        private QState stateReportGetputError;
        #endregion HSM declaration

        // Resolution for Stopwatch is 100 nanoseconds
        private Stopwatch HGADetectionAtInputTurnStationProcessCycleTimeStopWatch;
        private Stopwatch InputCameraHGADetectionAtInputTurnStationProcessCycleTimeStopWatch;
        private Stopwatch BoatLeavesInputTurnStationProcessCycleTimeStopWatch;

        private Signal _sigTimeout = new Signal("SigTimeout");
        private QTimer _qTimer;
        private bool _releaseError = false;
        private int _retryCount = 0;
        private uint _timeUsed = 0;
        private bool machineStop = true;
        private uint timeUsed = 0;
        private bool _lastSeaveyorIDInposStatus = false;
        private Queue<CarrierSettings> SimulatedInputCarriersSettingsQueue;       

        // Constructors ------------------------------------------------------------
        public InputTurnStationProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            
            this._workcell = workcell;

            // HSM delegates
            _qTimer = new QTimer(this);

            //RunInit mode
            stateStandbyInputTurnSection = new QState(this.StateStandbyInputTurnSection);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateDoAutoClearCarrier = new QState(this.StateDoAutoClearCarrier);
            stateCheckForNextCarrier = new QState(this.StateCheckForNextCarrier);
            stateReleaseCarrierToInputTurnStation = new QState(this.StateReleaseCarrierToInputTurnStation);
            stateInputTurnSectionTurnForward = new QState(this.StateInputTurnSectionTurnForward);
            stateVisionInspectCarrierAfterLoad  = new QState(this.StateVisionInspectCarrierAfterLoad);
            stateWaitForSigInputStationReady = new QState(this.StateWaitForSigInputStationReady);
            statePublishSigCarrierInInputStation = new QState(this.StatePublishSigCarrierInInputStation);
            stateReleaseCarrier = new QState(this.StateReleaseCarrier);
            stateInputTurnSectionTurnBackward = new QState(this.StateInputTurnSectionTurnBackward);
            stateReportTICError = new QState(this.StateReportTICError);
            stateReportCRDLError = new QState(this.StateReportCRDLError);
            stateReportGetputError = new QState(this.StateReportGetputError);
        }

        // Properties ----------------------------------------------------------

        public InputTurnStationController Controller
        { get { return _controller; } }

        // Methods -------------------------------------------------------------

        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigInputStationReady);
            AddAndSubscribeSignal(HSTWorkcell.SigStopMachineRun); 
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new InputTurnStationController(_workcell, "ITS", "InputTurnStation Controller");
            base.Start(priority);
        }

        #region StateRunInit


        protected override QState StateRunInit(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            _retryCount = 0;
            _currentInputCarrier = null;
            _currentInputCarrierSettings = null;
            HGADetectionAtInputTurnStationProcessCycleTimeStopWatch = new Stopwatch();
            InputCameraHGADetectionAtInputTurnStationProcessCycleTimeStopWatch = new Stopwatch();
            BoatLeavesInputTurnStationProcessCycleTimeStopWatch = new Stopwatch();            
            HSTMachine.Workcell.InputTurnStationBoatPositionError = false;
            CommonFunctions.Instance.StopMachineRunDueToTICError = false;
            if (qEvent.IsSignal(SigStateJob))
            {
                TransitionTo(stateStandbyInputTurnSection);
                return null;
            }
            return base.StateRunInit(qEvent);
        }

        private QState StateStandbyInputTurnSection(IQEvent qEvent)
        {
            
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.InputTurnSectionTurnTo0Deg(out _timeUsed);
                    _controller.InhibitInputConveyor(true);
                    _controller.InhibitInputTurnStation(false);
                    Thread.Sleep(1000);
                    _controller.InhibitInputTurnStation(true);
                    _lastSeaveyorIDInposStatus = _controller.IsInputLineReady();
                    TransitionTo(stateRunStart);
                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to standby Input Turn Station. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRunInit;
        }

        #endregion StateHome
        
        #region StateRun

        private QState StateRunStart(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            machineStop = false;

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
            {
                SimulatedInputCarriersSettingsQueue = new Queue<CarrierSettings>();

                foreach (CarrierSettings carrier in HSTMachine.Workcell.HSTSettings.SimulatedPart.Carriers)
                {
                    SimulatedInputCarriersSettingsQueue.Enqueue(carrier);
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                if (_workcell.IsMachineHomed)
                {                    
                    TransitionTo(stateDoAutoClearCarrier);                    
                }
                return null;
            }
            return stateRun;
        }


        private QState StateDoAutoClearCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent); 
            
            if (qEvent.IsSignal(SigStateJob))
            {                
                _workcell.SeaveyorAutoClearFlag = true;
                if (_controller.IsInputTurnStationHoldCarrier() || (HSTMachine.Workcell.HSTSettings.Install.OperationMode == Utils.OperationMode.Simulation ||
                    HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
                    TransitionTo(stateInputTurnSectionTurnForward);
                else
                {
                    TransitionTo(stateCheckForNextCarrier);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateCheckForNextCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent); 
            
            if (qEvent.IsSignal(SigStateJob))
            {
                //lai: reduce from 200 march12-2016
                _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 150), new QEvent(_sigTimeout));
                if (RecallDeferredSignal(HSTWorkcell.SigStopMachineRun))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigStopMachineRun);                   
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigStopMachineRun))
            {
                machineStop = true;

                if (CommonFunctions.Instance.StopMachineRunDueToTICError)
                {
                    CommonFunctions.Instance.StopMachineRunDueToTICError = false;
                    CommonFunctions.Instance.ConsecutiveBoatsWithTICError = 0;
                    TransitionTo(stateReportTICError);
                    return null;
                }else if (HSTMachine.Workcell.HSTSettings.TriggeringSetting.ErrorCodeTriggeringActivate)
                {
                    TransitionTo(stateReportCRDLError);
                    return null;
                }else if(CommonFunctions.Instance.StopMachineRunDueToGetputError)
                {
                    CommonFunctions.Instance.StopMachineRunDueToGetputError = false;
                    TransitionTo(stateReportGetputError);
                    return null;

                }
                else
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigEndRunProcess));
                }
            }

            if (qEvent.IsSignal(_sigTimeout) && !machineStop)
            {
                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                        (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
                    {
                        if (SimulatedInputCarriersSettingsQueue.Count == 0)
                        {
                            foreach (CarrierSettings carrier in HSTMachine.Workcell.HSTSettings.SimulatedPart.Carriers)
                            {
                                SimulatedInputCarriersSettingsQueue.Enqueue(carrier);
                            }
                        }

                        if (SimulatedInputCarriersSettingsQueue.Count > 0)
                        {
                            TransitionTo(stateReleaseCarrierToInputTurnStation);
                        }

                    }
                    else
                    {
                        if (_lastSeaveyorIDInposStatus || _controller.IsInputLineReady())
                        {
                            if(HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                            {
                                if (SimulatedInputCarriersSettingsQueue.Count == 0)
                                {
                                    foreach (CarrierSettings carrier in HSTMachine.Workcell.HSTSettings.SimulatedPart.Carriers)
                                    {
                                        SimulatedInputCarriersSettingsQueue.Enqueue(carrier);
                                    }
                                }
                            }

                            if (_lastSeaveyorIDInposStatus)
                            {
                                _lastSeaveyorIDInposStatus = false;
                                Thread.Sleep(1000); 
                            }
                            TransitionTo(stateReleaseCarrierToInputTurnStation);
                        }
                        else if (_controller.IsInputTurnStationHoldCarrier())
                        {
                            TransitionTo(stateInputTurnSectionTurnForward);
                        }
                        else
                        {
                            _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 150), new QEvent(_sigTimeout));
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
            }
            return stateRun;
        }

        private QState StateReleaseCarrierToInputTurnStation(IQEvent qEvent)
        {            
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent); 
            
            if (qEvent.IsSignal(SigStateJob))
            {                
                try
                {
                    
                    HGADetectionAtInputTurnStationProcessCycleTimeStopWatch.Start();
                    if(_lastSeaveyorIDInposStatus)_lastSeaveyorIDInposStatus = false;

                    ProcessStopWatch PSW = new ProcessStopWatch("", new Stopwatch());
                    CommonFunctions.Instance.OverallHGATestProcessCycleTimeStopWatch.Enqueue(PSW);

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun  && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat)
                    {
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        _controller.InhibitInputTurnStation(true);
                        _controller.InhibitInputConveyor(false);
                        _controller.CheckCarrierPresentAtInputTurnStation();
                    }
                    TransitionTo(stateInputTurnSectionTurnForward);
                }
                catch (Exception ex)
                {
                    if (!qEvent.IsSignal(SigStop))
                    {
                        TransitionTo(stateReleaseCarrierToInputTurnStation);
                    }

                    return null;
                }
                return null;
            }
            return stateRun;
        }
 
        private QState StateInputTurnSectionTurnForward(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (!_controller.IsInputTurnStationHoldCarrier())
                    {
                        int retrycount = 0;
                        while (!_controller.IsInputTurnStationHoldCarrier())
                        {
                            if(retrycount < 4)
                            Thread.Sleep(500);
                            else
                                HSTException.Throw(HSTErrors.InputTurnStationInPositionNotOnError, new Exception("Inpositon on timeout"));
                            retrycount++;
                        }
                    }

                    _controller.InhibitInputConveyor(true);
                    _controller.InputTurnSectionTurnTo90Deg(out _timeUsed);
                    TransitionTo(stateVisionInspectCarrierAfterLoad);
                    _retryCount = 0;
                }
                catch (Exception ex)
                {

                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to turn Input Turn Station to 90 degrees. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateVisionInspectCarrierAfterLoad(IQEvent qEvent)
        {
            bool carrierLoadedInWrongDirection = false;
 
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            InputCameraHGADetectionAtInputTurnStationProcessCycleTimeStopWatch.Start();

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Reject: 
                            {
                                BoatLeavesInputTurnStationProcessCycleTimeStopWatch.Start();
                                _currentInputCarrier.IsRejectedCarrier = true;
                                Log.Info(this, "{0}, ProcessName:{1}, StateVisionInspectCarrierAfterLoad, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                CommonFunctions.Instance.InputCarriers.Enqueue(_currentInputCarrier);
                                TransitionTo(stateWaitForSigInputStationReady);
                            }
                            break;
                        case ErrorButton.OK:
                            {
                                _retryCount = 0;
                            }
                            break;
                        default:
                            errorMessage = null;
                            System.Windows.Forms.MessageBox.Show(string.Format("Unhandled Button: {0}", response.ToString()), response.ToString());
                            return null;
                    }
                    return null;
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                    {
                        CarrierSettings carrierSettings = SimulatedInputCarriersSettingsQueue.Dequeue();
                        Mapper.CreateMap<CarrierSettings, Carrier>();
                        _currentInputCarrier = Mapper.Map<Carrier>(carrierSettings);
                        _currentInputCarrierSettings = carrierSettings;


                        if (_currentInputCarrier == null)
                            throw new Exception("Failed to find valid input carrier object to be assigned with the read RFID data.");                                               
                    }  
                    else if(HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassVisionAtInputTurnStation == true)
                    {
                        _currentInputCarrier = new Carrier();
                        _currentInputCarrier.Hga1.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga2.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga3.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga4.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga5.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga6.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga7.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga8.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga9.Hga_Status = HGAStatus.HGAPresent;
                        _currentInputCarrier.Hga10.Hga_Status = HGAStatus.HGAPresent;

                    }
                    else
                    {
                        Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.InputTurnTableFullyStopDelay);
                        _currentInputCarrier = _workcell.Process.InputEEProcess.Controller.VisionInspect();

                        if (CommonFunctions.Instance.visionError != "NoError")
                            HSTException.Throw(HSTErrors.InputDetectionCameraError, new Exception("Failed to run tool block: " + CommonFunctions.Instance.visionError));

                        if (_currentInputCarrier.IsLoadedInWrongDirection)
                        {
                            carrierLoadedInWrongDirection = true;
                            Log.Info(this, "{0}, ProcessName:{1}, StateVisionInspectCarrierAfterLoad, Input HGADetection camera detected carrier loaded in wrong direction. Reject this carrier.");

                            HSTException.Throw(HSTErrors.InputDetectionCameraCarrierLoadedInWrongDirection, new Exception("Input Camera detected current carrier was loaded in wrong direction."));
                        }

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "{0}, ProcessName:{1}, StateVisionInspectCarrierAfterLoad, Input HGADetection camera result Carrier ID:{2}, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, "Unknown",
                            _currentInputCarrier.Hga1.Hga_Status, _currentInputCarrier.Hga2.Hga_Status, _currentInputCarrier.Hga3.Hga_Status, _currentInputCarrier.Hga4.Hga_Status, _currentInputCarrier.Hga5.Hga_Status,
                            _currentInputCarrier.Hga6.Hga_Status, _currentInputCarrier.Hga7.Hga_Status, _currentInputCarrier.Hga8.Hga_Status, _currentInputCarrier.Hga9.Hga_Status, _currentInputCarrier.Hga10.Hga_Status);
                        }
                    }
                    _currentInputCarrier.CarrierCurrentLocation = CarrierLocation.InputTurnStation;                   

                    lock (CommonFunctions.Instance.InputCarriersLock)
                    {
                        CommonFunctions.Instance.InputCarriers.Enqueue(_currentInputCarrier);
                    }

                    _currentInputCarrier.IsCarrierEmpty = IsCarrierEmpty.Empty;

                    if (_currentInputCarrier.Hga1.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga2.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga3.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga4.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga5.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga6.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga7.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga8.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga9.Hga_Status >= HGAStatus.HGAPresent ||
                        _currentInputCarrier.Hga10.Hga_Status >= HGAStatus.HGAPresent)
                    {
                        _currentInputCarrier.IsCarrierEmpty = IsCarrierEmpty.NotEmpty;
                    }

                    InputCameraHGADetectionAtInputTurnStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Camera HGA Detection At Input Turn Station Process Cycle Time.csv", InputCameraHGADetectionAtInputTurnStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    BoatLeavesInputTurnStationProcessCycleTimeStopWatch.Start();
                    TransitionTo(stateWaitForSigInputStationReady);
                }
                catch (Exception ex)
                {
                    bool skipRetry = carrierLoadedInWrongDirection ? true : false;

                    if (_retryCount < 3 && !skipRetry)
                    {
                        Log.Error(this, "Failed to perform vision inspection. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst;
                        if (carrierLoadedInWrongDirection)
                        {
                            btnlst = new ButtonList(ErrorButton.Reject, ErrorButton.Stop, ErrorButton.NoButton);
                        }
                        else
                        {
                            btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.Stop);
                        }

                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForSigInputStationReady(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent); 
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigInputStationReady))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputStationReady);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigInputStationReady))
            {
                _releaseError = false;
                TransitionTo(stateReleaseCarrier);
                return null;
            }
            return stateRun;
        }

        private QState StateReleaseCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {                    
                    try
                    {
                        _controller.InhibitInputTurnStation(false);
                        _controller.WaitInputTurnStationPartCleared();
                        _controller.InhibitInputTurnStation(true);                        

                        string ProcessCycleTimeDirectory = HSTSettings.Instance.Directory.DataPath + "\\ProcessCycleTime";
                        if (!Directory.Exists(ProcessCycleTimeDirectory))
                        {
                            Directory.CreateDirectory(ProcessCycleTimeDirectory);
                        }

                        HGADetectionAtInputTurnStationProcessCycleTimeStopWatch.Stop();
                        BoatLeavesInputTurnStationProcessCycleTimeStopWatch.Stop();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("HGA Detection At Input Turn Station Process Cycle Time.csv", HGADetectionAtInputTurnStationProcessCycleTimeStopWatch.ElapsedTime);
                            CommonFunctions.Instance.LogProcessCycleTime("Boat Leaves Input Turn Station Process Cycle Time.csv", BoatLeavesInputTurnStationProcessCycleTimeStopWatch.ElapsedTime);
                        }
                      
                        TransitionTo(statePublishSigCarrierInInputStation);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("InputTurnStation release part failed.", ex);
                    }
                }
                catch (Exception ex2)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex2);
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePublishSigCarrierInInputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);   
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _workcell.Process.InputStationProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();
                    _workcell.Process.InputStationProcess.Controller.IncomingCarrierSettings = _currentInputCarrierSettings;

                    //lai
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, In InputTurnStationProcess' StatePublishSigCarrierInInputStation', _currentInputCarrier: Carrier ID:{2}, " +
                        "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                        "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID,
                        _currentInputCarrier.Hga1.Hga_Status, _currentInputCarrier.Hga2.Hga_Status, _currentInputCarrier.Hga3.Hga_Status, _currentInputCarrier.Hga4.Hga_Status, _currentInputCarrier.Hga5.Hga_Status,
                        _currentInputCarrier.Hga6.Hga_Status, _currentInputCarrier.Hga7.Hga_Status, _currentInputCarrier.Hga8.Hga_Status, _currentInputCarrier.Hga9.Hga_Status, _currentInputCarrier.Hga10.Hga_Status);
                    }

                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigCarrierPresentInInputStation));
                    TransitionTo(stateInputTurnSectionTurnBackward);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateInputTurnSectionTurnBackward(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent); 
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    Thread.Sleep(2000);
                    _controller.InputTurnSectionTurnTo0Deg(out _timeUsed);
                    TransitionTo(stateCheckForNextCarrier);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to turn Input Turn Station to 90 degrees. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateReportTICError(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.OK: // Reject, treat as bad carrier
                            {
                                PublishSignal(new QEvent(HSTWorkcell.SigEndRunProcess));
                            }
                            break;
                        default:
                            errorMessage = null;
                            System.Windows.Forms.MessageBox.Show(string.Format("Unhandled Button: {0}", response.ToString()), response.ToString());
                            return null;
                    }
                    return null;
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    HSTException.Throw(HSTErrors.TestElectronicsTICErrorDetection, new Exception(CommonFunctions.Instance.TICErrorMessage));
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
            }
            return stateRun;
        }

        private QState StateReportCRDLError(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.OK: // Reject, treat as bad carrier
                            {
                                PublishSignal(new QEvent(HSTWorkcell.SigEndRunProcess));
                            }
                            break;
                        default:
                            errorMessage = null;
                            System.Windows.Forms.MessageBox.Show(string.Format("Unhandled Button: {0}", response.ToString()), response.ToString());
                            return null;
                    }
                    return null;
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    CommonFunctions.Instance.ActivePopupCRDLerrorMessage = true;
                    HSTException.Throw(HSTErrors.TestElectronicsCRDLErrorDetection, new Exception(CommonFunctions.Instance.CRDLErrorMessage));
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
            }
            return stateRun;
        }

        private QState StateReportGetputError(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.OK: // Reject, treat as bad carrier
                            {
                                PublishSignal(new QEvent(HSTWorkcell.SigEndRunProcess));
                            }
                            break;
                        default:
                            errorMessage = null;
                            System.Windows.Forms.MessageBox.Show(string.Format("Unhandled Button: {0}", response.ToString()), response.ToString());
                            return null;
                    }
                    return null;
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    CommonFunctions.Instance.ActivePopupGetputErrorMessage = true;
                    if(FISManager.Instance.IsFISConnected)
                        HSTException.Throw(HSTErrors.TestElectronecsGetputErrorDetection2, new Exception(CommonFunctions.Instance.GetputErrorMessage));
                    else
                        HSTException.Throw(HSTErrors.TestElectronecsGetputErrorDetection, new Exception(CommonFunctions.Instance.GetputErrorMessage));

                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
            }
            return stateRun;
        }

        private QState StateReportWriterBrigeError(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.OK: // Reject, treat as bad carrier
                            {
                                HSTMachine.Workcell.IsUnderWriterBridgeFailureAlert = false;
                                PublishSignal(new QEvent(HSTWorkcell.SigEndRunProcess));
                                HSTMachine.Instance.MainForm.getPanelCommand().stopSystemProcess();
                            }
                            break;
                        default:
                            errorMessage = null;
                            System.Windows.Forms.MessageBox.Show(string.Format("Unhandled Button: {0}", response.ToString()), response.ToString());
                            return null;
                    }
                    return null;
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    HSTException.Throw(HSTErrors.TestElectronicsWriterBridgeDetection, 
                        new Exception(string.Format("{0},{1}Carrier ID={2}", "High percentage of writer bridging was detected, please confirm UTIC machine!", 
                            Environment.NewLine, HSTMachine.Workcell.WRBridgeFailueCarrierId)));
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
            }
            return stateRun;
        }

        #endregion StateRun      
    }

}
