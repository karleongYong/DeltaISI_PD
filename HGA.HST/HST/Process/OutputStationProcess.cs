using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using qf4net;
using System.Linq;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Logging;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Data;


namespace Seagate.AAS.HGA.HST.Process
{

    public class OutputStationProcess : ActiveProcessHST
    {
        private bool InputEEAxisBoundaryCheckCompleteSignalReceived = false;
        private bool TestProbeAxisBoundaryCheckCompleteSignalReceived = false;
        private bool OutputEEAxisBoundaryCheckCompleteSignalReceived = false;
        private OutputStationController _controller;
        protected HSTWorkcell _workcell;
        private Thread _lockCarrier;

        private Stopwatch HGAUnloadingAtOutputStationProcessCycleTimeStopWatch;
        private Stopwatch OutputStationStopperDeploymentProcessCycleTimeStopWatch;
        private Stopwatch BoatLiftedAtOutputStationProcessCycleTimeStopWatch;
        private Stopwatch OutputStationStopperRetractionProcessCycleTimeStopWatch;
        private Stopwatch OutputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch;
        private Stopwatch OutputCameraHGADetectionAtOutputStationProcessCycleTimeStopWatch;
        private Stopwatch OutputStationCarrierLockProcessCycleTimeStopWatch;
        private Stopwatch OutputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch;
        private Stopwatch OutputStationOutputLifterLoweredProcessCycleTimeStopWatch;
        private Stopwatch OutputStationRFIDWriteProcessCycleTimeStopWatch;
        private Stopwatch BoatLeavesOutputStationProcessCycleTimeStopWatch;

        private uint _timeUsed = 0;

        //RunInit mode
        private QState stateOutputLifterMoveToHomePosition;
        private QState statePublishOutputLifterHomed;
        private QState stateWaitForZAxisBoundaryCheckCompleteSignals;
        private QState stateBoundaryCheck;
        private QState stateRunInitCompleteMoveToHomePosition;
        private QState stateRunInitStandbyConveyorOutputStation;

        //Run mode
        private QState stateRunStart;
        private QState stateCheckForSafeToReleaseToOutputTurnStation;
        private QState stateWaitForCarrierPresentInOutputStation;
        private QState stateRfidRead;
        private QState stateOutputLifterMoveUp;
        private QState statePublishSigOutputCarrierReadyForPlace;
        private QState stateWaitForSigOutputCarrierPlaceDone;
        private QState stateVisionInspectOnHGAPresent;
        private QState stateWaitForAllMeasurementDataDone;
        private QState stateRfidWrite;
        private QState stateSendDataToSeatrack;
        private QState statePublishClampAndRotateStart;
        private QState stateWaitForClampAndRotateComplete;
        private QState stateOutputLifterMoveDown;
        private QState stateLowerOutputStationStopper;
        private QState stateReleaseCarrierToOutputTurnStation;
        private QState statePublishSigCarrierPresentInOutputTurnStation;
        private QState stateWaitForDycemCleaningComplete;
        private QState stateWaitForCarrierReleaseComplete;

        private int _retryCount = 0;
        private int _repickRetryCount = 0;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private bool leftOverHGAsOnPrecisorNest = false;
        private bool failToExtendAndCounterRotateCarrierScrewDriver = false;
        private bool failToRetractAndRotateCarrierScrewDriver = false;
        private Carrier _OutputCarrierWithOutputCameraInspectionResult;
        private Carrier _OutputCarrierWithInputCameraInspectionResult;
        public Carrier _currentOutputCarrier; // this carrier object is from the buffer station
        private Carrier _currentTestedCarrier;
        private QTimer _stateTimer;
        private TimeSpan _loopTimeOut;
        private bool _ignoreandcontinue;
        private bool isRepickProcess = false;

        private Stopwatch _closeClampTime = new Stopwatch();
        // Constructors ------------------------------------------------------------
        public OutputStationProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            this._workcell = workcell;
            _qTimer = new QTimer(this);
            _stateTimer = new QTimer(this);

            //RunInit mode
            stateOutputLifterMoveToHomePosition = new QState(this.StateOutputLifterMoveToHomePosition);
            statePublishOutputLifterHomed = new QState(this.StatePublishOutputLifterHomed);
            stateWaitForZAxisBoundaryCheckCompleteSignals = new QState(this.StateWaitForZAxisBoundaryCheckCompleteSignals);
            stateBoundaryCheck = new QState(this.StateBoundaryCheck);
            stateRunInitCompleteMoveToHomePosition = new QState(this.StateRunInitCompleteMoveToHomePosition);
            stateRunInitStandbyConveyorOutputStation = new QState(this.StateRunInitStandbyConveyorOutputStation);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateCheckForSafeToReleaseToOutputTurnStation = new QState(this.StateCheckForSafeToReleaseToOutputTurnStation);
            stateWaitForCarrierPresentInOutputStation = new QState(this.StateWaitForCarrierPresentInOutputStation);
            stateRfidRead = new QState(this.StateRfidRead);
            stateOutputLifterMoveUp = new QState(this.StateOutputLifterMoveUp);
            statePublishSigOutputCarrierReadyForPlace = new QState(this.StatePublishSigOutputCarrierReadyForPlace);
            stateWaitForSigOutputCarrierPlaceDone = new QState(this.StateWaitForSigOutputCarrierPlaceDone);
            stateVisionInspectOnHGAPresent = new QState(this.StateVisionInspectOnHGAPresent);
            stateWaitForAllMeasurementDataDone = new QState(this.StateWaitForAllMeasurementDataDone);
            stateRfidWrite = new QState(this.StateRfidWrite);
            stateSendDataToSeatrack = new QState(this.StateSendDataToSeatrack);
            statePublishClampAndRotateStart = new QState(this.StatePublishClampAndRotateStart);
            stateWaitForClampAndRotateComplete = new QState(this.StateWaitForClampAndRotateComplete);
            stateOutputLifterMoveDown = new QState(this.StateOutputLifterMoveDown);
            stateLowerOutputStationStopper = new QState(this.StateLowerOutputStationStopper);
            stateReleaseCarrierToOutputTurnStation = new QState(this.StateReleaseCarrierToOutputTurnStation);
            statePublishSigCarrierPresentInOutputTurnStation = new QState(this.StatePublishSigCarrierPresentInOutputTurnStation);
            stateWaitForDycemCleaningComplete = new QState(this.StateWaitForDycemCleaningComplete);
            stateWaitForCarrierReleaseComplete = new QState(this.StateWaitForSigCarrierReleaseComplete);

        }
        // Properties ----------------------------------------------------------

        public OutputStationController Controller
        { get { return _controller; } }


        // Internal methods ----------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigCarrierPresentInOutputStation);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputCarrierPlaceDone);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEDycemCleaningComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigCarrierIsInOutputTurnStation);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputClampRotateProcessComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigAllMeasurementDataDone);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new OutputStationController(_workcell, "OutputStation", "Output Station");
            try
            {
                _controller.SetProcessCode(this, 2);
                _controller.InitializeController();
            }
            catch (Exception ex)
            {
            }
            base.Start(priority);
        }

        protected override QState StateActive(IQEvent qEvent)
        {
            if (qEvent.IsSignal(QSignals.Entry))
            {
                ClearDeferredSignals();
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputClampRotateProcessComplete) ||
                qEvent.IsSignal(HSTWorkcell.SigOutputCarrierPlaceDone) ||
                qEvent.IsSignal(HSTWorkcell.SigCarrierIsInOutputTurnStation))
            {
                AddDeferredSignal(qEvent);
                return null;
            }
            return base.StateActive(qEvent);
        }

        #region StateRunInit
        protected override QState StateRunInit(IQEvent qEvent)
        {
            HGAUnloadingAtOutputStationProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationStopperDeploymentProcessCycleTimeStopWatch = new Stopwatch();
            BoatLiftedAtOutputStationProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationStopperRetractionProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch = new Stopwatch();
            OutputCameraHGADetectionAtOutputStationProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationCarrierLockProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationOutputLifterLoweredProcessCycleTimeStopWatch = new Stopwatch();
            OutputStationRFIDWriteProcessCycleTimeStopWatch = new Stopwatch();
            BoatLeavesOutputStationProcessCycleTimeStopWatch = new Stopwatch();
            HSTMachine.Workcell.OutputStationBoatPositionError = false;
            CommonFunctions.Instance.OutputCarriersQueue.Clear();
            CommonFunctions.Instance.TestedCarrierQueue.Clear();


            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    InputEEAxisBoundaryCheckCompleteSignalReceived = false;
                    TestProbeAxisBoundaryCheckCompleteSignalReceived = false;
                    OutputEEAxisBoundaryCheckCompleteSignalReceived = false;
                    leftOverHGAsOnPrecisorNest = false;
                    failToExtendAndCounterRotateCarrierScrewDriver = false;
                    failToRetractAndRotateCarrierScrewDriver = false;
                    _currentTestedCarrier = null;
                    _currentOutputCarrier = null;
                    _controller.RaiseOutputStationStopper(false);
                    _controller.RaiseOutputLifter(false);
                    _controller.OutputStationForwardClamp(false);
                    _controller.OutputStationClampRotaryOpenCover(true);
                    _controller.InhibitOutputStation(false);
                    Thread.Sleep(2000);
                    _controller.InhibitOutputStation(true);
                    _retryCount = 0;
                    HSTWorkcell.outputCarrierIsUnlocked = false;
                    _ignoreandcontinue = false;             
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }

                if (HSTWorkcell.disableBoundaryCheck)
                    TransitionTo(stateWaitForZAxisBoundaryCheckCompleteSignals);
                else
                    TransitionTo(stateOutputLifterMoveToHomePosition);
                return null;
            }
            return base.StateRunInit(qEvent);

        }

        private QState StateOutputLifterMoveToHomePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.RaiseOutputLifter(false, out _timeUsed);
                    TransitionTo(statePublishOutputLifterHomed);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishOutputLifterHomed(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigOutputLifterHomed));
                    TransitionTo(stateWaitForZAxisBoundaryCheckCompleteSignals);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateWaitForZAxisBoundaryCheckCompleteSignals(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputEEBoundaryCheckComplete);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputEEBoundaryCheckComplete);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete))
                InputEEAxisBoundaryCheckCompleteSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete))
                OutputEEAxisBoundaryCheckCompleteSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                TestProbeAxisBoundaryCheckCompleteSignalReceived = true;

            if (InputEEAxisBoundaryCheckCompleteSignalReceived && TestProbeAxisBoundaryCheckCompleteSignalReceived && OutputEEAxisBoundaryCheckCompleteSignalReceived)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForZAxisBoundaryCheckCompleteSignals, Received QSignal:{2} and transition to new State:StateBoundaryCheck because Z1AxisBoundaryCheckCompleteSignalReceived is true AND Z2AxisBoundaryCheckCompleteSignalReceived is true AND Z3AxisBoundaryCheckCompleteSignalReceived is true", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                InputEEAxisBoundaryCheckCompleteSignalReceived = false;
                TestProbeAxisBoundaryCheckCompleteSignalReceived = false;
                OutputEEAxisBoundaryCheckCompleteSignalReceived = false;

                TransitionTo(StateBoundaryCheck);
                return null;
            }
            return stateRunInit;
        }

        private QState StateBoundaryCheck(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (CheckDeferredSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete);
                    }

                    if (CheckDeferredSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete);
                    }

                    if (CheckDeferredSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
                    }
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.BoundaryCheck();
                    TransitionTo(stateRunInitCompleteMoveToHomePosition);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateRunInitCompleteMoveToHomePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.RaiseOutputLifter(false, out _timeUsed);
                    TransitionTo(stateRunInitStandbyConveyorOutputStation);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateRunInitStandbyConveyorOutputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.InhibitOutputStation(true);

                    HGAUnloadingAtOutputStationProcessCycleTimeStopWatch.Start();

                    TransitionTo(stateRunStart);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        #endregion

        #region StateRun
        private QState StateRunStart(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (_controller.IsOutputStationHoldCarrier())
                {
                    TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);
                }
                else
                {
                    TransitionTo(stateWaitForCarrierPresentInOutputStation);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForCarrierPresentInOutputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Reject:
                            {
                                TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);
                            }
                            break;
                    }
                    return null;
                }
            }


            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                if (_controller.IsBufferStationHoldCarrier())
                {
                    try
                    {
                        CommonFunctions.Instance.IsOutputWorkingProcess = true;
                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass &&
                            HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true &&
                            HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true)
                        {
                            _controller.RaiseOutputStationStopper(false);
                        }
                        else
                            _controller.RaiseOutputStationStopper(true);

                        _currentTestedCarrier = null;
                        _controller.InhibitOutputStation(true);
                        _controller.InhibitBufferStation(false);
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "Releasing carrier from buffer zone.");
                        }

                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            _controller.WaitBufferStationPartClear();
                        }
                    }
                    catch (Exception ex)
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                }

                if (_controller.IsOutputStationHoldCarrier())
                {
                    OutputStationStopperDeploymentProcessCycleTimeStopWatch.Start();

                    //lai
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        if (_currentOutputCarrier != null)
                        {
                            Log.Info(this, "{0}, lai ProcessName:{1}, In OutputStationProcess' StateWaitForCarrierPresentInOutputStation, _currentOutputCarrier: Carrier ID:{2}, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _currentOutputCarrier.CarrierID,
                            _currentOutputCarrier.Hga1.Hga_Status, _currentOutputCarrier.Hga2.Hga_Status, _currentOutputCarrier.Hga3.Hga_Status, _currentOutputCarrier.Hga4.Hga_Status, _currentOutputCarrier.Hga5.Hga_Status,
                            _currentOutputCarrier.Hga6.Hga_Status, _currentOutputCarrier.Hga7.Hga_Status, _currentOutputCarrier.Hga8.Hga_Status, _currentOutputCarrier.Hga9.Hga_Status, _currentOutputCarrier.Hga10.Hga_Status);
                        }
                    }
                    ProcessStopWatch PSW1;
                    if (_currentOutputCarrier != null)
                    {
                        PSW1 = new ProcessStopWatch(_currentOutputCarrier.CarrierID, new Stopwatch());

                    }
                    else
                    {
                        PSW1 = new ProcessStopWatch("", new Stopwatch());
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.PrecisorNestStabilityAtOutputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    try
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            if (CommonFunctions.Instance.BoatArrivesAtOutputStationFromInputStationProcessCycleTimeStopWatch.Count > 0)
                            {
                                ProcessStopWatch PSW = CommonFunctions.Instance.BoatArrivesAtOutputStationFromInputStationProcessCycleTimeStopWatch.First();
                                PSW.Stop();
                                CommonFunctions.Instance.LogProcessCycleTime("Boat Arrives At Output Station From Input Station Process Cycle Time.csv", PSW.GetElapsedTime(), _currentOutputCarrier.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                                CommonFunctions.Instance.BoatArrivesAtOutputStationFromInputStationProcessCycleTimeStopWatch.Dequeue();
                            }

                            if (CommonFunctions.Instance.PrecisorNestMovesFromPrecisorStationToOutputStationProcessCycleTimeStopWatch.Count > 0)
                            {
                                ProcessStopWatch PSW = CommonFunctions.Instance.PrecisorNestMovesFromPrecisorStationToOutputStationProcessCycleTimeStopWatch.First();
                                PSW.Stop();
                                CommonFunctions.Instance.LogProcessCycleTime("Precisor Nest Moves From Precisor Station To Output Station Process Cycle Time.csv", PSW.GetElapsedTime(), _currentOutputCarrier.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                                CommonFunctions.Instance.PrecisorNestMovesFromPrecisorStationToOutputStationProcessCycleTimeStopWatch.Dequeue();
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true &&
                        HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true)
                    {
                        TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);
                        return null;
                    }
                    else
                    {
                        Thread.Sleep(200);
                        if (_currentOutputCarrier != null)
                        {
                            if (_currentOutputCarrier.IsRejectedCarrier)
                            {
                                TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);
                                _currentOutputCarrier = null;
                            }
                            else
                            {
                                TransitionTo(stateOutputLifterMoveUp);
                            }
                        }
                        else
                        {
                            TransitionTo(stateOutputLifterMoveUp);
                        }
                    }
                }
                else
                {
                    _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                }
                return null;
            }
            return stateRun;
        }

        private QState StateLowerOutputStationStopper(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {

                    _controller.RaiseOutputStationStopper(false);
                    TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateOutputLifterMoveUp(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    OutputStationStopperDeploymentProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Output Station Stopper Deployment Process Cycle Time.csv", OutputStationStopperDeploymentProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    _controller.StartMoveLifter(true);
                    _controller.WaitForLifterMoveDone(true);

                    BoatLiftedAtOutputStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Boat Lifted At Output Station Process Cycle Time.csv", BoatLiftedAtOutputStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    _workcell.IsOutputStationRFIDReadFailed = false;
                    OutputStationStopperRetractionProcessCycleTimeStopWatch.Start();
                    OutputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch.Start();
                    OutputCameraHGADetectionAtOutputStationProcessCycleTimeStopWatch.Start();
                    TransitionTo(stateRfidRead);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateRfidRead(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Retry: // Reject, treat as bad carrier
                            {
                                TransitionTo(stateRfidRead);
                            }
                            break;
                        case ErrorButton.Reject:
                            {
                                _workcell.IsOutputStationRFIDReadFailed = true;
                                if (_workcell.Process.ClampAndRotateProcess.Controller.IsOutputClampOpenDetect())
                                    TransitionTo(statePublishSigOutputCarrierReadyForPlace);
                                else
                                    TransitionTo(stateOutputLifterMoveDown);
                            }
                            break;
                    }
                    return null;
                }
            }


            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.ReadRfid();

                    //load RFID data to carrier object
                    Carrier outputCarrier = null;
                    bool isCarrierDataLoaded = false;
                    string carrierID = _controller.RfidController.FolaTagDataReadInfor.CarrierID;
                    
  
                    
                    if (CommonFunctions.Instance.OutputCarriersQueue.Count > 0)
                    {
                        try
                        {
                            var getCarrierData = CommonFunctions.Instance.OutputCarriersQueue.Where(C => C.CarrierID == carrierID).First();
                            if (!getCarrierData.IsRejectedCarrier)
                                isCarrierDataLoaded = true;
                        }
                        catch (Exception)
                        {
                            isCarrierDataLoaded = false;
                        }

                        if (isCarrierDataLoaded)
                        {
                            Carrier IncomingOutputCarrier;
                            lock (CommonFunctions.Instance.OutputCarriersQueue)
                            {
                                IncomingOutputCarrier = CommonFunctions.Instance.OutputCarriersQueue.Where(C => C.CarrierID == carrierID).First();
                                CommonFunctions.Instance.OutputCarriersQueue.Dequeue();
                            }
                            _currentOutputCarrier = IncomingOutputCarrier;

                            if (_currentOutputCarrier.CarrierID == carrierID ||
                                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true))
                            {
                                outputCarrier = _currentOutputCarrier.DeepCopy();
                                outputCarrier.CarrierCurrentLocation = CarrierLocation.OutputStation;
                            }

                            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                            {
                                Log.Info(this, "In OutputStation's StateRFIDRead, Carrier ID:{0}, IsPassThrough:{1}, IsMeasurementTestDone:{2}.", _currentOutputCarrier.CarrierID, _currentOutputCarrier.IsPassThroughMode.ToString(), _currentOutputCarrier.IsMeasurementTestDone.ToString());
                            }

                            if (_currentOutputCarrier.RFIDInfoVerify == RFIDInfoVerify.NotMatched)
                            {
                                TransitionTo(stateOutputLifterMoveDown);
                            }
                            else if (outputCarrier.IsPassThroughMode)
                            {
                                CommonFunctions.Instance.CarrierCycleTime = new CarrierCycleTime("", 0, 0, 0);
                                TransitionTo(stateOutputLifterMoveDown);
                            }
                            // 26-Mar-2020 Bypass Old Data due RFID Mode
                            else if (HSTMachine.Workcell.HSTSettings.Install.RFIDUpdateOption == RFIDUpdateOption.UpdateOnlyHGAStatus)
                            {
                                _currentOutputCarrier.RFIDData.RFIDTagData = _controller.RfidController.FolaTagDataReadInfor;
                                TransitionTo(statePublishSigOutputCarrierReadyForPlace);
                            }
                            else
                            {
                                TransitionTo(statePublishSigOutputCarrierReadyForPlace);
                            }
                        }
                        else
                        {
                            TransitionTo(stateOutputLifterMoveDown);
                        }
                    }
                    else
                    {
                        TransitionTo(stateOutputLifterMoveDown);
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to read RFID. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Reject, ErrorButton.Retry, ErrorButton.Stop);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }

            return stateRun;
        }

        private QState StatePublishSigOutputCarrierReadyForPlace(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (CheckDeferredSignal(HSTWorkcell.SigOutputCarrierPlaceDone))
                {
                    RemoveDeferredSignal(HSTWorkcell.SigOutputCarrierPlaceDone);
                }

                //Re-check carrier clamp and open
                if (!Controller.IsOutputStationCarrierClampOpen())
                {
                    //If not open, re-open again 1 time.
                    _controller.OutputStationClampRotaryOpenCover(false);
                    Thread.Sleep(200);
                    _controller.OutputStationForwardClamp(true);
                    Thread.Sleep(200);
                    _controller.OutputStationClampRotaryOpenCover(true);
                    Thread.Sleep(200);
                    if (!Controller.IsOutputStationCarrierClampOpen())
                    {
                        throw new Exception(String.Format("Output carrier clamp is not opened, please check!!"));
                    }
                    else
                    {
                        _controller.OutputStationForwardClamp(false);
                        Thread.Sleep(200);
                        _controller.OutputStationClampRotaryOpenCover(true);
                        Thread.Sleep(200);
                    }
                }

                isRepickProcess = false;
                _repickRetryCount = 0;
                _ignoreandcontinue = false;
                PublishSignal(new QEvent(HSTWorkcell.SigOutputCarrierReadyForPlace));
                TransitionTo(stateWaitForSigOutputCarrierPlaceDone);
            }
            return stateRun;
        }

        private QState StateWaitForSigOutputCarrierPlaceDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.OK:
                            {
                                Notify.PopUp("Info", "Press OK to stop the machine run.", "", "OK");
                                _workcell.Process.Stop();
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
                if (RecallDeferredSignal(HSTWorkcell.SigOutputCarrierPlaceDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputCarrierPlaceDone);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigOutputCarrierPlaceDone))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true)
                {
                    // Do nothing
                }
                else
                {
                    if (!_workcell.IsOutputStationRFIDReadFailed)
                    {
                        _currentTestedCarrier = CommonFunctions.Instance.TestedCarrierQueue.First();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "{0}, lai ProcessName:{1}, In OutputStationProcess' StateWaitForSigOutputCarrierPlaceDone, Status of tested HGAs: Carrier ID:{2}, " +
                                    "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                                    "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _currentTestedCarrier.CarrierID,
                                    _currentTestedCarrier.Hga1.Hga_Status, _currentTestedCarrier.Hga2.Hga_Status, _currentTestedCarrier.Hga3.Hga_Status, _currentTestedCarrier.Hga4.Hga_Status, _currentTestedCarrier.Hga5.Hga_Status,
                                    _currentTestedCarrier.Hga6.Hga_Status, _currentTestedCarrier.Hga7.Hga_Status, _currentTestedCarrier.Hga8.Hga_Status, _currentTestedCarrier.Hga9.Hga_Status, _currentTestedCarrier.Hga10.Hga_Status);
                        }
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigOutputCarrierPlaceDone, Received QSignal:{2} and transition to new State:StateVisionInspectOnHGAPresent", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                        }

                        try
                        {
                            if (_currentTestedCarrier.CarrierID != _currentOutputCarrier.CarrierID)
                            {
                                throw new Exception(String.Format("Current Tested HGAs is from Carrier(ID:{0}) which doesn't match the lifted output carrier({1})! ", _currentTestedCarrier.CarrierID, _currentOutputCarrier.CarrierID));
                            }
                            else
                            {
                                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                                {
                                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigOutputCarrierPlaceDone, Current Tested Carrier:{2}, Current Output Carrier:{3}", LoggerCategory.StateTransition, _processName, _currentTestedCarrier.CarrierID, _currentOutputCarrier.CarrierID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.Stop, ErrorButton.NoButton);
                            TransitionToErrorState(btnlst, ex);
                        }

                        TransitionTo(stateVisionInspectOnHGAPresent);
                        return null;

                    }
                    else
                    {
                        TransitionTo(statePublishClampAndRotateStart);
                        return null;
                    }
                }

            }
            return stateRun;
        }

        private QState StateVisionInspectOnHGAPresent(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Yes: // Try to repick the hgas that were left out
                            {
                                PublishSignal(new QEvent(HSTWorkcell.SigVisionDetectionFoundMissingHGAs));
                                PublishSignal(new QEvent(HSTWorkcell.SigOutputCarrierReadyForPlace));
                                TransitionTo(stateWaitForSigOutputCarrierPlaceDone);
                            }
                            break;
                        case ErrorButton.Stop: // Update RFID, lock carrier and then stop run
                            {
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga1.Hga_Status == HGAStatus.NoHGAPresent) // if camera say no hga
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga1.Hga_Status != HGAStatus.NoHGAPresent) // if the output camera result different from input camera throw exception
                                    {
                                        _currentTestedCarrier.Hga1.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga1.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga2.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga2.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga2.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga2.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga3.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga3.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga3.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga3.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga4.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga4.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga4.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga4.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga5.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga5.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga5.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga5.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga6.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga6.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga6.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga6.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga7.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga7.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga7.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga7.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga8.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga8.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga8.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga8.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga9.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga9.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga9.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga9.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                if (_OutputCarrierWithOutputCameraInspectionResult.Hga10.Hga_Status == HGAStatus.NoHGAPresent)
                                {
                                    if (this._OutputCarrierWithInputCameraInspectionResult.Hga10.Hga_Status != HGAStatus.NoHGAPresent)
                                    {
                                        _currentTestedCarrier.Hga10.Hga_Status = HGAStatus.NoHGAPresent;
                                        _currentOutputCarrier.Hga10.Hga_Status = HGAStatus.NoHGAPresent;
                                    }
                                }
                                PublishSignal(new QEvent(HSTWorkcell.SigStopMachineRun));
                            }
                            break;
                        case ErrorButton.Continue:
                            {
                                //Continue to process HGAs
                                _ignoreandcontinue = true;
                                _loopTimeOut = new TimeSpan(0, 0, 0, 0, 100); // 300 msec
                                _stateTimer.FireIn(_loopTimeOut, new QEvent(SigStateJob));
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
                    int testedHGACount = 0;
                    int untestedHGACount = 0;
                    int missingOrUnknownHGACount = 0;

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true)
                    {
                        this._currentTestedCarrier = _currentOutputCarrier.DeepCopy();

                        if (_currentTestedCarrier.Hga1.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga1.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga2.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga2.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga3.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga3.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga4.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga4.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga5.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga5.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga6.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga6.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga7.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga7.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga8.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga8.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga9.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga9.Hga_Status = HGAStatus.Untested;

                        if (_currentTestedCarrier.Hga10.Hga_Status != HGAStatus.NoHGAPresent)
                            this._currentTestedCarrier.Hga10.Hga_Status = HGAStatus.Untested;

                    }

                    leftOverHGAsOnPrecisorNest = false;

                    // HGA1
                    if (this._currentTestedCarrier.Hga1.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga1.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga1.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga1.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA2
                    if (this._currentTestedCarrier.Hga2.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga2.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga2.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga2.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA3
                    if (this._currentTestedCarrier.Hga3.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga3.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga3.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga3.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA4
                    if (this._currentTestedCarrier.Hga4.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga4.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga4.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga4.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA5
                    if (this._currentTestedCarrier.Hga5.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga5.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga5.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga5.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA6
                    if (this._currentTestedCarrier.Hga6.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga6.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga6.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga6.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA7
                    if (this._currentTestedCarrier.Hga7.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga7.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga7.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga7.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA8
                    if (this._currentTestedCarrier.Hga8.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga8.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga8.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga8.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA9
                    if (this._currentTestedCarrier.Hga9.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga9.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga9.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga9.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    // HGA10
                    if (this._currentTestedCarrier.Hga10.Hga_Status == HGAStatus.TestedPass || this._currentTestedCarrier.Hga10.Hga_Status == HGAStatus.TestedFail)
                    {
                        testedHGACount++;
                    }
                    else if (this._currentTestedCarrier.Hga10.Hga_Status == HGAStatus.NoHGAPresent || this._currentTestedCarrier.Hga10.Hga_Status == HGAStatus.Unknown)
                    {
                        missingOrUnknownHGACount++;
                    }
                    else
                    {
                        untestedHGACount++;
                    }

                    CommonFunctions.Instance.CarrierCycleTime = new CarrierCycleTime(_currentTestedCarrier.CarrierID, testedHGACount, untestedHGACount, missingOrUnknownHGACount);
                    if (!isRepickProcess) HSTMachine.Workcell.LoadCounter.ProcessedHGACount += testedHGACount;
                    if (HSTMachine.Workcell.LoadCounter.WriterBridgePartRunCounter > 5000)
                        HSTMachine.Workcell.LoadCounter.ResetWriterBridgCounter();

                    CommonFunctions.Instance.MachineUPH.TestedHGACount = /*testedHGACount*/ 10;
                    CommonFunctions.Instance.MachineUPH.CarrierID = _currentTestedCarrier.CarrierID;

                    CommonFunctions.Instance.MachineUPH.LogUPH();

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1},In OutputStationProcess' StateVisionInspectOnHGAPresent,  Carrier ID:{2}, TestedHGACount:{3}, UntestedHGACount:{4}, MissingOrUnknownHGACount:{5}", LoggerCategory.StateTransition, _processName, _currentTestedCarrier.CarrierID, testedHGACount, untestedHGACount, missingOrUnknownHGACount);

                        Log.Info(this, "{0}, ProcessName:{1},In OutputStationProcess' StateVisionInspectOnHGAPresent,  Status of tested HGAs after place: Carrier ID:{2}, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _currentTestedCarrier.CarrierID,
                            _currentTestedCarrier.Hga1.Hga_Status, _currentTestedCarrier.Hga2.Hga_Status, _currentTestedCarrier.Hga3.Hga_Status, _currentTestedCarrier.Hga4.Hga_Status, _currentTestedCarrier.Hga5.Hga_Status,
                            _currentTestedCarrier.Hga6.Hga_Status, _currentTestedCarrier.Hga7.Hga_Status, _currentTestedCarrier.Hga8.Hga_Status, _currentTestedCarrier.Hga9.Hga_Status, _currentTestedCarrier.Hga10.Hga_Status);
                    }

                    this._OutputCarrierWithInputCameraInspectionResult = new Carrier();

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                        HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                        (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassVisionAtOutput == true))
                    {
                        this._OutputCarrierWithInputCameraInspectionResult = _currentOutputCarrier.DeepCopy();
                    }
                    else
                    {
                        this._OutputCarrierWithInputCameraInspectionResult = _currentOutputCarrier.DeepCopy();

                        // to check the present of HGA at output station after place

                        _OutputCarrierWithOutputCameraInspectionResult = _workcell.Process.OutputEEProcess.Controller.VisionInspect(HGAStatus.HGAPresent, this._OutputCarrierWithInputCameraInspectionResult.CarrierID);

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "{0}, ProcessName:{1}, In OutputStationProcess' StateVisionInspectOnHGAPresent, HGA detection result from output camera after place on to carrier: Carrier ID:{2}, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _OutputCarrierWithOutputCameraInspectionResult.CarrierID,
                            _OutputCarrierWithOutputCameraInspectionResult.Hga1.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga2.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga3.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga4.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga5.Hga_Status,
                            _OutputCarrierWithOutputCameraInspectionResult.Hga6.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga7.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga8.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga9.Hga_Status, _OutputCarrierWithOutputCameraInspectionResult.Hga10.Hga_Status);

                            Log.Info(this, "{0}, ProcessName:{1}, In OutputStationProcess' StateVisionInspectOnHGAPresent, HGA detection result from input camera when carrier: Carrier ID:{2} was at input turn table, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _OutputCarrierWithInputCameraInspectionResult.CarrierID,
                            _OutputCarrierWithInputCameraInspectionResult.Hga1.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga2.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga3.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga4.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga5.Hga_Status,
                            _OutputCarrierWithInputCameraInspectionResult.Hga6.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga7.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga8.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga9.Hga_Status, _OutputCarrierWithInputCameraInspectionResult.Hga10.Hga_Status);

                        }

                        if (!_ignoreandcontinue)
                        {

                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga1.Hga_Status == HGAStatus.NoHGAPresent) // if camera say no hga
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga1.Hga_Status != HGAStatus.NoHGAPresent) // if the output camera result different from input camera throw exception
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga2.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga2.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga3.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga3.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga4.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga4.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga5.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga5.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga6.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga6.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga7.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga7.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga8.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga8.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga9.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga9.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }
                            if (_OutputCarrierWithOutputCameraInspectionResult.Hga10.Hga_Status == HGAStatus.NoHGAPresent)
                            {
                                if (this._OutputCarrierWithInputCameraInspectionResult.Hga10.Hga_Status != HGAStatus.NoHGAPresent)
                                {
                                    leftOverHGAsOnPrecisorNest = true;
                                }
                            }

                            try
                            {
                                if (leftOverHGAsOnPrecisorNest)
                                {
                                    if (!_ignoreandcontinue)
                                        HSTException.Throw(HSTErrors.OutputDetectionCameraFailedToPlaceBackAllHGAsAfterMeasurementError, new Exception("Missing HGA after place. \nPress 'Yes' to repick left over HGA on precisor nest if any.\n'No'to stop the run. \n 'Continue' ignore and continue to process HGAs"));
                                    else
                                        leftOverHGAsOnPrecisorNest = false;
                                }

                            }
                            catch (Exception ex)
                            {
                                if (leftOverHGAsOnPrecisorNest && _repickRetryCount == 0)
                                {
                                    isRepickProcess = true;
                                    _repickRetryCount++;
                                    Thread.Sleep(200);
                                    PublishSignal(new QEvent(HSTWorkcell.SigVisionDetectionFoundMissingHGAs));
                                    PublishSignal(new QEvent(HSTWorkcell.SigOutputCarrierReadyForPlace));
                                    TransitionTo(stateWaitForSigOutputCarrierPlaceDone);
                                    return null;
                                }
                                else
                                {
                                    ButtonList btnlst = new ButtonList(ErrorButton.Yes, ErrorButton.Stop, ErrorButton.Continue);
                                    TransitionToErrorState(btnlst, ex);
                                    return null;
                                }
                            }
                        }
                    }

                    CommonFunctions.Instance.MachineUPH.PrecisorNestAtInputPositionTimeStamp = DateTime.Now;

                    CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH = CommonFunctions.Instance.MachineUPH.PrecisorNestAtInputPositionTimeStamp.Subtract(CommonFunctions.Instance.PreviousPrecisorNestAtInputPositionTimeStamp);
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Received SigHGAsPickDoneAtInputLifter in PrecisorStationProcess, PrecisorNestAtInputPositionTimeStamp : {0}, PreviousPrecisorNestAtInputPositionTimeStamp : {1}, TestStationCycleTimeSpan : {2}",
                            CommonFunctions.Instance.MachineUPH.PrecisorNestAtInputPositionTimeStamp, CommonFunctions.Instance.PreviousPrecisorNestAtInputPositionTimeStamp, ((CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Hours * 3600) + (CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Minutes * 60) + CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Seconds + CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Milliseconds / 1000.0));
                    }

                    HSTMachine.Workcell.LoadCounter.UPH = Convert.ToInt32(3600 * CommonFunctions.Instance.MachineUPH.TestedHGACount / ((CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Hours * 3600) + (CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Minutes * 60) + CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Seconds + (CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH.Milliseconds / 1000.0)));
                    CommonFunctions.Instance.MachineUPH.UPH = HSTMachine.Workcell.LoadCounter.UPH;
                    CommonFunctions.Instance.PreviousPrecisorNestAtInputPositionTimeStamp = CommonFunctions.Instance.MachineUPH.PrecisorNestAtInputPositionTimeStamp;

                    // Lai: Calculate Machine UPH
                    CommonFunctions.Instance.MachineUPH.PrecisorNestAtStandbyPositionTimeStamp = DateTime.Now;

                    CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2 = CommonFunctions.Instance.MachineUPH.PrecisorNestAtStandbyPositionTimeStamp.Subtract(CommonFunctions.Instance.PreviousPrecisorNestAtStandbyPositionTimeStamp);
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Received SigHGAsPickDoneAtInputLifter in PrecisorStationProcess, PrecisorNestAtStandbyPositionTimeStamp : {0}, PreviousPrecisorNestAtStandbyPositionTimeStamp : {1}, TestStationCycleTimeSpan : {2}",
                            CommonFunctions.Instance.MachineUPH.PrecisorNestAtStandbyPositionTimeStamp, CommonFunctions.Instance.PreviousPrecisorNestAtStandbyPositionTimeStamp, ((CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Hours * 3600) + (CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Minutes * 60) + CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Seconds + CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Milliseconds / 1000.0));
                    }
                    HSTMachine.Workcell.LoadCounter.UPH2 = Convert.ToInt32(3600 * CommonFunctions.Instance.MachineUPH.TestedHGACount / ((CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Hours * 3600) + (CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Minutes * 60) + CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Seconds + (CommonFunctions.Instance.MachineUPH.TestStationCycleTimeSpanUPH2.Milliseconds / 1000.0)));
                    CommonFunctions.Instance.MachineUPH.UPH2 = HSTMachine.Workcell.LoadCounter.UPH2;
                    CommonFunctions.Instance.MachineUPH.PreviousPrecisorNestAtStandbyPositionTimeStamp = CommonFunctions.Instance.PreviousPrecisorNestAtStandbyPositionTimeStamp;

                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs));
                    CommonFunctions.Instance.TestedCarrierQueue.Dequeue();
                    TransitionTo(statePublishClampAndRotateStart);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.Continue);
                    TransitionToErrorState(btnlst, ex);
                }

                return null;
            }
            return stateRun;
        }

        private void LockCarrier()
        {
            try
            {
                OutputStationCarrierLockProcessCycleTimeStopWatch.Start();

                _controller.OutputStationForwardClamp(true);
                Thread.Sleep(200);
                _controller.OutputStationClampRotaryOpenCover(false);
                Thread.Sleep(200);
                HSTWorkcell.outputCarrierIsUnlocked = false;

                OutputStationCarrierLockProcessCycleTimeStopWatch.Stop();
                OutputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch.Stop();
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    CommonFunctions.Instance.LogProcessCycleTime("Output Station Carrier Lock Process Cycle Time.csv", OutputStationCarrierLockProcessCycleTimeStopWatch.ElapsedTime);
                    CommonFunctions.Instance.LogProcessCycleTime("Output Station Carrier Screwdriver Deployment Process Cycle Time.csv", OutputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch.ElapsedTime);
                }
            }
            catch (Exception ex)
            {
                failToExtendAndCounterRotateCarrierScrewDriver = true;
            }

            try
            {
                OutputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch.Start();

                _controller.OutputStationForwardClamp(false);
                _controller.OutputStationClampRotaryOpenCover(true);

                OutputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch.Stop();
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    CommonFunctions.Instance.LogProcessCycleTime("Output Station Carrier Screwdriver Retraction Process Cycle Time.csv", OutputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch.ElapsedTime);
                }
            }
            catch (Exception ex)
            {
                failToRetractAndRotateCarrierScrewDriver = true;
            }
        }

        private QState StatePublishClampAndRotateStart(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (CheckDeferredSignal(HSTWorkcell.SigOutputStartRFIDProcess))
                {
                    RemoveDeferredSignal(HSTWorkcell.SigOutputStartRFIDProcess);
                }

                PublishSignal(new QEvent(HSTWorkcell.SigOutputStartRFIDProcess));

                //Remove running ISI from list
                try
                {
                    _workcell.ISIDataListIn.Remove(_currentOutputCarrier.CarrierID);
                }
                catch (Exception)
                {
                }

                TransitionTo(stateWaitForAllMeasurementDataDone);
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForAllMeasurementDataDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 4000), new QEvent(_sigTimeout));

                if (RecallDeferredSignal(HSTWorkcell.SigAllMeasurementDataDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigAllMeasurementDataDone);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigAllMeasurementDataDone))
            {
                TransitionTo(stateRfidWrite);
                return null;
            }
            if (qEvent.IsSignal(_sigTimeout))
            {
                RemoveDeferredSignal(HSTWorkcell.SigAllMeasurementDataDone);
                TransitionTo(stateRfidWrite);
                return null;
            }
            return stateRun;
        }

        private QState StateRfidWrite(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Reject: // Reject, treat as bad carrier
                            {
                                Log.Info(this, "{0}, ProcessName:{1}, StateRfidWrite, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                _retryCount = 0;
                                TransitionTo(stateWaitForClampAndRotateComplete);
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
                    OutputStationRFIDWriteProcessCycleTimeStopWatch.Start();

                    if (_currentOutputCarrier != null)
                    {
                        if (_currentOutputCarrier.IsPassThroughMode || HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true)
                        {
                            _controller.WriteRfid(_controller.RfHead, _currentOutputCarrier);
                            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                            {
                                Log.Info(this, "_currentOutputCarrier Carrier ID:{0}, IsPassThrough:{1}, IsMeasurementTestDone:{2}.", _currentOutputCarrier.CarrierID, _currentOutputCarrier.IsPassThroughMode.ToString(), _currentOutputCarrier.IsMeasurementTestDone.ToString());
                            }
                        }
                        else
                        {
                            if (!_workcell.IsOutputStationRFIDReadFailed && !_currentOutputCarrier.IsRejectedCarrier)
                            {
                                _controller.WriteRfid(_controller.RfHead, _currentTestedCarrier);
                                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                                {
                                    Log.Info(this, "_currentTestedCarrier Carrier ID:{0}, IsPassThrough:{1}, IsMeasurementTestDone:{2}.", _currentTestedCarrier.CarrierID, _currentTestedCarrier.IsPassThroughMode.ToString(), _currentTestedCarrier.IsMeasurementTestDone.ToString());
                                }
                            }
                        }
                    }
                    OutputStationRFIDWriteProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Output Station RFID Write Deployment Process Cycle Time.csv", OutputStationRFIDWriteProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    BoatLeavesOutputStationProcessCycleTimeStopWatch.Start();
                    TransitionTo(stateWaitForClampAndRotateComplete);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForClampAndRotateComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigOutputClampRotateProcessComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputClampRotateProcessComplete);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigOutputClampRotateProcessComplete))
            {
                TransitionTo(stateOutputLifterMoveDown);
                return null;
            }
            return stateRun;
        }

        private QState StateOutputLifterMoveDown(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    OutputStationOutputLifterLoweredProcessCycleTimeStopWatch.Start();

                    _controller.StartMoveLifter(false);
                    _controller.WaitForLifterMoveDone(false);

                    OutputStationOutputLifterLoweredProcessCycleTimeStopWatch.Stop();
                    OutputStationStopperRetractionProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Output Station Output Lifter Lowered Process Cycle Time.csv", OutputStationOutputLifterLoweredProcessCycleTimeStopWatch.ElapsedTime);
                        CommonFunctions.Instance.LogProcessCycleTime("Output Station Stopper Retraction Process Cycle Time.csv", OutputStationStopperRetractionProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    if (_currentOutputCarrier != null)
                        TransitionTo(stateSendDataToSeatrack);
                    else
                        TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);

                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateSendDataToSeatrack(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Reject: // Reject, treat as bad carrier
                            {
                                Log.Info(this, "{0}, ProcessName:{1}, StateRfidWrite, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                _retryCount = 0;
                                TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);
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
                    Carrier outputCarrier = new Carrier();
                    bool _isSendDataSkip = false;
                    if (_currentTestedCarrier != null)
                    {
                        outputCarrier = _currentTestedCarrier;
                    }
                    else
                    {
                        if (_currentOutputCarrier != null)
                        {
                            outputCarrier = _currentOutputCarrier;
                        }
                    }

                    if (outputCarrier == null)
                        _isSendDataSkip = true;

                    if (!_isSendDataSkip)
                    {
                        // Reset error message code
                        CommonFunctions.Instance.strErrorMessageCode = "0";

                        var outputTagInfo = _controller.RfidController.FolaTagDataWriteInfor;
                        object[] outputObj = new object[2];
                        outputObj[0] = outputTagInfo;
                        outputObj[1] = outputCarrier;
                        var outputRequestEvent = new QEvent(HSTWorkcell.SigOutputProcessDataComplete);
                        outputRequestEvent.EventObject = outputObj;

                        QF.Instance.Publish(outputRequestEvent);
                    }

                    if (_controller.IsCCCFunctional())
                    {
                        if (_workcell.HSTSettings.CccParameterSetting.EnableAlertMsg)
                        {
                            _workcell.RaiseCCCVerifyEventStatus();
                        }
                        else
                        {
                            HSTMachine.Workcell.CurretCCCActiveStatus.ChangeActiveStatus(false);
                            HSTMachine.Workcell.CCCMachineTriggeringActivated = false;
                            HSTMachine.Workcell.TicMcFailureCounter.ResetByMC(HSTMachine.Workcell.CCCFailureInfo.FailedMc);
                            
                            if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.IsTriggering)
                            {
                                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.Default();
                            }
                            if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.IsTriggering)
                            {
                                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.Default();
                            }

                            HSTMachine.Workcell.CCCMachineTriggeringDown = true;
                            HSTMachine.Workcell.HSTSettings.Save();
                        }
                    }

                    TransitionTo(stateCheckForSafeToReleaseToOutputTurnStation);

                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateCheckForSafeToReleaseToOutputTurnStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Reject: // Reject, treat as bad carrier
                            {
                                Log.Info(this, "{0}, ProcessName:{1}, StateRfidWrite, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                TransitionTo(stateReleaseCarrierToOutputTurnStation);
                            }
                            break;
                    }
                    return null;
                }
            }

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (!HSTMachine.Workcell.IsUnderWriterBridgeFailureAlert && _controller.IsOutputTurnStationInPositionSensorOff() && _controller.IsOutputTurnStationAt0Degree() && !_controller.IsCCCFunctional())
                    {
                        TransitionTo(stateReleaseCarrierToOutputTurnStation);
                    }
                    else
                    {
                        //lai: reduce from 200 march12-2016
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 150), new QEvent(_sigTimeout));
                    }
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Reject, ErrorButton.NoButton, ErrorButton.Abort);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateReleaseCarrierToOutputTurnStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    uint timeUsed = 0;
                    Stopwatch timeToWaitStationClear = new Stopwatch();

                    _controller.RaiseOutputStationStopper(false);

                    _controller.InhibitOutputStation(false);
                    timeToWaitStationClear.Reset();
                    timeToWaitStationClear.Start();
                    _controller.WaitOutputStationPartClear();
                    timeToWaitStationClear.Stop();
                    if (timeToWaitStationClear.ElapsedTime_sec > 20)
                        Thread.Sleep(3000);

                    BoatLeavesOutputStationProcessCycleTimeStopWatch.Stop();
                    HGAUnloadingAtOutputStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Boat Leaves Output Station Process Cycle Time.csv", BoatLeavesOutputStationProcessCycleTimeStopWatch.ElapsedTime);
                        CommonFunctions.Instance.LogProcessCycleTime("HGA Unloading At Output Station Process Cycle Time.csv", HGAUnloadingAtOutputStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    string CarrierID = "";
                    if (_currentTestedCarrier != null)
                    {
                        CarrierID = _currentTestedCarrier.CarrierID;
                    }
                    else if (_currentOutputCarrier != null)
                    {
                        CarrierID = _currentOutputCarrier.CarrierID;
                    }

                    _workcell.IsCarrierInOutputTurnTable = true;

                    ProcessStopWatch PSW1 = new ProcessStopWatch(CarrierID, new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.BoatArrivesAtOutputTurnStationFromOutputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    TransitionTo(statePublishSigCarrierPresentInOutputTurnStation);
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

        private QState StatePublishSigCarrierPresentInOutputTurnStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                PublishSignal(new QEvent(HSTWorkcell.SigCarrierPresentInOutputTurnStation));
                TransitionTo(stateWaitForCarrierReleaseComplete);
            }
            return stateRun;
        }

        private QState StateWaitForSigCarrierReleaseComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigCarrierIsInOutputTurnStation))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigCarrierIsInOutputTurnStation);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigCarrierIsInOutputTurnStation))
            {
                UIUtility.Invoke(HSTMachine.Workcell.getPanelData(), () =>
                {
                    _workcell.csvFileOutput.DataUpdateCompleted();
                });

                CommonFunctions.Instance.IsOutputWorkingProcess = false;
                TransitionTo(stateWaitForCarrierPresentInOutputStation);
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForDycemCleaningComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigOutputEEDycemCleaningComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputEEDycemCleaningComplete);
                    return null;
                }
                return null;

            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEDycemCleaningComplete))
            {
                TransitionTo(stateOutputLifterMoveDown);
                return null;
            }

            return stateRun;
        }
        #endregion
    }
}
