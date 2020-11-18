using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.HGA.HST.Machine;
using qf4net;
using System.IO;
using System.Linq;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Controllers;
using System.Threading;
using XyratexOSC.Logging;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.Settings;
using AutoMapper;
using XyratexOSC.Utilities;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Process
{
    public class InputStationProcess : ActiveProcessHST
    {
        private bool InputEEAxisBoundaryCheckCompleteSignalReceived = false;
        private bool TestProbeAxisBoundaryCheckCompleteSignalReceived = false;
        private bool OutputEEAxisBoundaryCheckCompleteSignalReceived = false;
        private InputStationController _controller;
        protected HSTWorkcell _workcell;
        private Carrier _currentInputCarrier = null;
        private CarrierSettings _currentInputCarrierSettings = null;
        private Stopwatch HGALoadingAtInputStationProcessCycleTimeStopWatch;
        private Stopwatch InputStationStopperDeploymentProcessCycleTimeStopWatch;
        private Stopwatch InputStationRFIDReadProcessCycleTimeStopWatch;
        private Stopwatch BoatLiftedAtInputStationProcessCycleTimeStopWatch;
        private Stopwatch InputStationStopperRetractionProcessCycleTimeStopWatch;
        private Stopwatch BoatLeavesInputStationProcessCycleTimeStopWatch;
        private Stopwatch InputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch;
        private Stopwatch InputStationCarrierUnlockProcessCycleTimeStopWatch;
        private Stopwatch InputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch;
        private Stopwatch InputStationInputLifterLoweredProcessCycleTimeStopWatch;

        private uint _timeUsed = 0;

        #region HSM declaration

        //RunInit mode
        private QState stateInputLifterMoveToHomePosition;
        private QState statePublishInputLifterHomed;
        private QState stateWaitForZAxisBoundaryCheckCompleteSignals;
        private QState stateBoundaryCheck;
        private QState stateRunInitCompleteMoveToHomePosition;
        private QState stateStandbyInputStation;

        //Run mode
        private QState stateRunStart;
        private QState stateDoAutoClearCarrier;
        private QState stateRaiseInputStopper;
        private QState statePublishSigInputStationReady;
        private QState stateWaitForSigCarrierInInputStation;
        private QState stateCheckCarrierPresentInInputStation;
        private QState stateReleaseCarrier;
        private QState stateRfidRead;
        private QState stateRfidResultVerify;
        private QState stateIsCarrierEmptyOrAllHGAsFailedVerify;
        private QState stateIsCarrierHGAsMissingSerialNo;
        private QState stateInputLifterUp;
        private QState stateForwardClampAndRotateOpen;
        private QState stateBackwardClampAndRotateClose;
        private QState stateGetSLDRParamBinData;
        private QState stateWaitForInputEECompleted;
        private QState stateInputLifterDown;
        private QState stateWaitForBufferStationReady;
        private QState stateWaitForGetISIDataCompleted;
        private QState stateWaitForDycemCleaningComplete;
        private QState stateRecipeChangedError;
        private QState stateAllOutputProcssDone;

        private Signal _sigTimeout = new Signal("SigTimeout");
        private QTimer _qTimer;
        private bool _releaseError = false;
        private int _retryCount = 0;


        #endregion HSM declaration


        // Constructors ------------------------------------------------------------
        public InputStationProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //RunInit mode
            stateInputLifterMoveToHomePosition = new QState(this.StateInputLifterMoveToHomePosition);
            statePublishInputLifterHomed = new QState(this.StatePublishInputLifterHomed);
            stateWaitForZAxisBoundaryCheckCompleteSignals = new QState(this.StateWaitForZAxisBoundaryCheckCompleteSignals);
            stateRunInitCompleteMoveToHomePosition = new QState(this.StateRunInitCompleteMoveToHomePosition);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateDoAutoClearCarrier = new QState(this.StateDoAutoClearCarrier);
            stateRaiseInputStopper = new QState(this.StateRaiseInputStopper);
            stateWaitForSigCarrierInInputStation = new QState(this.StateWaitForSigCarrierInInputStation);
            stateCheckCarrierPresentInInputStation = new QState(this.StateCheckCarrierPresentInInputStation);

            stateRfidRead = new QState(this.StateRfidRead);
            stateRfidResultVerify = new QState(this.StateRfidResultVerify);
            stateIsCarrierEmptyOrAllHGAsFailedVerify = new QState(this.StateIsCarrierEmptyOrAllHGAsFailedVerify);
            stateIsCarrierHGAsMissingSerialNo = new QState(this.StateIsCarrierHGAsMissingSerialNo);
            stateInputLifterUp = new QState(this.StateInputLifterUp);
            stateForwardClampAndRotateOpen = new QState(this.StateForwardClampAndRotateOpen);
            stateBackwardClampAndRotateClose = new QState(this.StateBackwardClampAndRotateClose);

            stateWaitForInputEECompleted = new QState(this.StateWaitForInputEECompleted);
            stateWaitForGetISIDataCompleted = new QState(this.StateWaitForGetISIDataCompleted);
            stateInputLifterDown = new QState(this.StateInputLifterDown);
            stateReleaseCarrier = new QState(this.StateReleaseCarrier);
            stateWaitForBufferStationReady = new QState(this.StateWaitForBufferStationReady);
            stateWaitForDycemCleaningComplete = new QState(this.StateWaitForDycemCleaningComplete);
            stateRecipeChangedError = new QState(this.StateReportRecipeNameError);
            stateAllOutputProcssDone = new QState(this.StateAllOutputProcessDone);
        }
        // Properties ----------------------------------------------------------

        public InputStationController Controller
        { get { return _controller; } }


        // Internal methods ----------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigCarrierPresentInInputStation);
            AddAndSubscribeSignal(HSTWorkcell.SigInputLifterDownCompleted);
            AddAndSubscribeSignal(HSTWorkcell.SigHGAsInputEECompletePick);
            AddAndSubscribeSignal(HSTWorkcell.SigStart);
            AddAndSubscribeSignal(HSTWorkcell.SigInputEEDycemCleaningComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigInputGetISIDataComplete);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new InputStationController(HSTMachine.Workcell, "B", "InputStation Controller");
            base.Start(priority);
        }

        #region StateRunInit

        protected override QState StateRunInit(IQEvent qEvent)
        {
            HGALoadingAtInputStationProcessCycleTimeStopWatch = new Stopwatch();
            InputStationStopperDeploymentProcessCycleTimeStopWatch = new Stopwatch();
            InputStationRFIDReadProcessCycleTimeStopWatch = new Stopwatch();
            BoatLiftedAtInputStationProcessCycleTimeStopWatch = new Stopwatch();
            InputStationStopperRetractionProcessCycleTimeStopWatch = new Stopwatch();
            BoatLeavesInputStationProcessCycleTimeStopWatch = new Stopwatch();
            InputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch = new Stopwatch();
            InputStationCarrierUnlockProcessCycleTimeStopWatch = new Stopwatch();
            InputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch = new Stopwatch();
            InputStationInputLifterLoweredProcessCycleTimeStopWatch = new Stopwatch();
            HSTMachine.Workcell.InputStationBoatPositionError = false;
            HSTMachine.Workcell.BufferStationBoatPositionError = false;

            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    InputEEAxisBoundaryCheckCompleteSignalReceived = false;
                    TestProbeAxisBoundaryCheckCompleteSignalReceived = false;
                    OutputEEAxisBoundaryCheckCompleteSignalReceived = false;
                    _controller.RaiseInputStationStopper(true);
                    _controller.RaiseInputLifter(false);
                    _controller.InputStationForwardClamp(false);
                    _controller.InputStationClampRotaryOpenCover(false);
                    _controller.InhibitBufferStation(true);
                    _controller.InhibitInputStation(true);
                    if (HSTWorkcell.disableBoundaryCheck)
                        TransitionTo(stateWaitForZAxisBoundaryCheckCompleteSignals);
                    else
                        TransitionTo(stateInputLifterMoveToHomePosition);
                    _retryCount = 0;
                    _currentInputCarrier = null;
                    _currentInputCarrierSettings = null;
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to standby Input Station Station. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
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
            return base.StateRunInit(qEvent);

        }

        private QState StateInputLifterMoveToHomePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.InputLifterMoveDown();
                    _retryCount = 0;
                    TransitionTo(statePublishInputLifterHomed);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to move input lifter down. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
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

        private QState StatePublishInputLifterHomed(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigInputLifterHomed));
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
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForZAxisBoundaryCheckCompleteSignals, Received QSignal:{2} and transition to new State:StateBoundaryCheck because Z1AxisBoundaryCheckCompleteSignalReceived is true AND Z2AxisBoundaryCheckCompleteSignalReceived is true AND Z3AxisBoundaryCheckCompleteSignalReceived", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                InputEEAxisBoundaryCheckCompleteSignalReceived = false;
                TestProbeAxisBoundaryCheckCompleteSignalReceived = false;
                OutputEEAxisBoundaryCheckCompleteSignalReceived = false;

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
                TransitionTo(stateRunInitCompleteMoveToHomePosition);
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
                    _controller.InputLifterMoveDown();
                    _retryCount = 0;
                    TransitionTo(stateRunStart);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to move input lifter down. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
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
        #endregion

        #region StateRun
        private QState StateRunStart(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (HSTMachine.Workcell.IsMachineHomed)
                    TransitionTo(stateDoAutoClearCarrier);
                return null;
            }
            return stateRun;
        }

        private QState StateDoAutoClearCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (_controller.IsInputStationHoldCarrier())
                    TransitionTo(stateCheckCarrierPresentInInputStation);
                else
                    TransitionTo(stateRaiseInputStopper);
                return null;
            }
            return stateRun;
        }

        private QState StateRaiseInputStopper(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    uint timeUsed = 0;
                    InputStationStopperDeploymentProcessCycleTimeStopWatch.Start();
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass &&
                        HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true &&
                        HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true)
                        _controller.RaiseInputStationStopper(false, out timeUsed);
                    else
                        _controller.RaiseInputStationStopper(true, out timeUsed);
                    InputStationStopperDeploymentProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Station Stopper Deployment Process Cycle Time.csv", InputStationStopperDeploymentProcessCycleTimeStopWatch.ElapsedTime);
                    }
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigInputStationReady));
                    TransitionTo(stateWaitForSigCarrierInInputStation);
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

        private QState StateWaitForSigCarrierInInputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigCarrierPresentInInputStation))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigCarrierPresentInInputStation);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigCarrierPresentInInputStation))
            {
                try
                {
                    TransitionTo(stateCheckCarrierPresentInInputStation);
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

        private QState StateCheckCarrierPresentInInputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        _controller.WaitInputStationPartPresent();
                    }
                    HSTMachine.Workcell.SwapTimePerCarrier.Stop();
                    HSTMachine.Workcell.SwapTimeCarrier = HSTMachine.Workcell.SwapTimePerCarrier.ElapsedTime_sec;
                    HSTMachine.Workcell.SwapTimePerCarrier.Reset();
                    HSTMachine.Workcell.SwapTimePerCarrier.Start();
                    HGALoadingAtInputStationProcessCycleTimeStopWatch.Start();

                    _currentInputCarrier = _controller.IncomingCarrier.DeepCopy();
                    _currentInputCarrierSettings = _controller.IncomingCarrierSettings;

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, lai ProcessName:{1}, In InputStationProcess' StateWaitForSigCarrierInInputStation', _currentInputCarrier: Carrier ID:{2}, " +
                        "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                        "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID,
                        _currentInputCarrier.Hga1.Hga_Status, _currentInputCarrier.Hga2.Hga_Status, _currentInputCarrier.Hga3.Hga_Status, _currentInputCarrier.Hga4.Hga_Status, _currentInputCarrier.Hga5.Hga_Status,
                        _currentInputCarrier.Hga6.Hga_Status, _currentInputCarrier.Hga7.Hga_Status, _currentInputCarrier.Hga8.Hga_Status, _currentInputCarrier.Hga9.Hga_Status, _currentInputCarrier.Hga10.Hga_Status);
                    }

                    if (_currentInputCarrier == null)
                        throw new Exception("input carrier object is null");

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                        HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                    {
                        if (_currentInputCarrierSettings == null)
                            throw new Exception("input carrier settings object is null");
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass &&
                        HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true &&
                        HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true)
                        TransitionTo(stateWaitForBufferStationReady);
                    else if (_currentInputCarrier.IsRejectedCarrier)
                    {
                        _controller.RaiseInputStationStopper(false);
                        TransitionTo(stateWaitForBufferStationReady);
                    }
                    else
                        TransitionTo(stateRfidRead);

                    if (CheckDeferredSignal(HSTWorkcell.SigHGAsInputEECompletePick))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigHGAsInputEECompletePick);
                    }

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
                        case ErrorButton.Reject: // Reject, treat as bad carrier
                            {
                                _currentInputCarrier.IsRejectedCarrier = true;
                                Log.Info(this, "{0}, ProcessName:{1}, StateRfidRead, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                _retryCount = 0;
                                TransitionTo(stateInputLifterDown);
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
                    string WorkOrderFilePath = "";
                    InputStationRFIDReadProcessCycleTimeStopWatch.Start();

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                    {
                        if (_currentInputCarrier == null)
                            throw new Exception("Failed to find valid input carrier object to be assigned with the read RFID data.");

                        _currentInputCarrier.RFIDData = new RFIDInfo();
                        _currentInputCarrier.RFIDData.RFIDFileName = _currentInputCarrierSettings.RFIDFileName;
                        _currentInputCarrier.RFIDData.LoadRFIDFile(_currentInputCarrierSettings.RFIDFileName);

                    }
                    else
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true)
                        {
                            _currentInputCarrier.RFIDData = new RFIDInfo();
                            _currentInputCarrier.RFIDData.RFIDTagData.WorkOrder = "TE04323D";
                            _currentInputCarrier.RFIDData.RFIDTagData.WorkOrderVersion = '0';
                        }
                        else
                        {
                            _controller.ReadRfid();

                            //load RFID data to carrier object
                            lock (CommonFunctions.Instance.InputCarriersLock)
                            {
                                if (_currentInputCarrier == null)
                                    throw new Exception("Failed to find valid input carrier object to be assigned with the read RFID data.");

                                _currentInputCarrier.CarrierID = _controller.RfidController.FolaTagDataReadInfor.CarrierID;
                                _currentInputCarrier.RFIDData = new RFIDInfo();
                                _currentInputCarrier.RFIDData.RFIDTagData = _controller.RfidController.FolaTagDataReadInfor;
                                // rename file
                                try
                                {
                                    string newfilename = _currentInputCarrier.ImageFileName.Replace("NoCarrierInfo", _currentInputCarrier.CarrierID);
                                    if (System.IO.File.Exists(_currentInputCarrier.ImageFileName))
                                        System.IO.File.Move(@_currentInputCarrier.ImageFileName, newfilename);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(this, "Exception: {0} Process Moving bmp file: {1}", ex.Message, _currentInputCarrier.ImageFileName);
                                }
                                //
                            }
                        }
                    }

                    WorkOrderFilePath = HSTMachine.Workcell.HSTSettings.Directory.WorkorderLocalPath + "\\" + _currentInputCarrier.RFIDData.RFIDTagData.WorkOrder + "-" + _currentInputCarrier.RFIDData.RFIDTagData.WorkOrderVersion + ".wo";
                    HSTMachine.Workcell.WorkOrder.LoadNewLoadingWO(WorkOrderFilePath);
                    _currentInputCarrier.WorkOrderData = HSTMachine.Workcell._workOrder.Loading;
                    HSTMachine.Workcell.HSTSettings.LastRunWorkOrderServerFileName = _currentInputCarrier.RFIDData.RFIDTagData.WorkOrder + "-" + _currentInputCarrier.RFIDData.RFIDTagData.WorkOrderVersion;

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Carrier ID:{0}, IsPassThrough:{1}, Loaded work order file:{2}.", _currentInputCarrier.CarrierID, _currentInputCarrier.IsPassThroughMode.ToString(), WorkOrderFilePath);
                    }
                    string workOrderNo = _currentInputCarrier.WorkOrderData.WorkOrderNo;
                    char lastChar = workOrderNo[workOrderNo.Length - 1];

                    if (lastChar.Equals('U'))
                    {
                        _currentInputCarrier.HGATabType = HGAProductTabType.Up;
                        UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Text = HGAProductTabType.Up.ToString();
                        });
                    }
                    else if (lastChar.Equals('D'))
                    {
                        _currentInputCarrier.HGATabType = HGAProductTabType.Down;
                        UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Text = HGAProductTabType.Down.ToString();
                        });
                    }
                    else
                        throw new Exception(string.Format("Unable to determine HGA Tab Type from Work Order No:{0} Carrier ID:{1}", workOrderNo, _currentInputCarrier.CarrierID));
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "In InputStationProcess's StateRfidRead, Carrier ID = {0}, Work Order No = {1} and Tab Type = {2}.", _currentInputCarrier.CarrierID, workOrderNo, _currentInputCarrier.HGATabType.ToString());
                    }
                    _controller.CheckWorkOrder(_currentInputCarrier.CarrierID);
                    _retryCount = 0;

                    TransitionTo(stateRfidResultVerify);
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
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.Stop);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }

            return stateRun;
        }

        private QState StateRfidResultVerify(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            int errorId = 0;

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Reject: // Reject, treat as bad carrier
                            {
                                _currentInputCarrier.IsRejectedCarrier = true;
                                Log.Info(this, "{0}, ProcessName:{1}, StateRfidResultVerify, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                _retryCount = 0;
                                TransitionTo(stateInputLifterDown);
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
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                    HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                    (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true))
                    {
                        Carrier[] Carriers = CommonFunctions.Instance.InputCarriers.ToArray();

                        if (_controller.CheckCarrierInformation(_currentInputCarrier))
                        {
                            _currentInputCarrier.RFIDInfoVerify = RFIDInfoVerify.Matched;
                            string temp = string.Format("{0}-{1}", Carriers[0].CarrierID, Carriers[0].ImageFileName);
                            _currentInputCarrier.ImageFileName = temp;
                        }
                        else
                        {
                            _currentInputCarrier.RFIDInfoVerify = RFIDInfoVerify.NotMatched;
                            _currentInputCarrier.IsPassThroughMode = true;

                            TransitionTo(stateInputLifterDown);
                            return null;
                        }

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

                        TransitionTo(stateIsCarrierEmptyOrAllHGAsFailedVerify);
                    }
                    else
                    {
                        lock (CommonFunctions.Instance.InputCarriersLock)
                        {
                            Carrier[] Carriers = CommonFunctions.Instance.InputCarriers.ToArray();

                            if (!_controller.CheckCompareWorkOrderName(_currentInputCarrier))
                            {
                                var selectedName = string.Format("{0}-{1}", HSTMachine.Workcell.WorkOrder.Loading.WorkOrderNo, HSTMachine.Workcell.WorkOrder.Loading.Version);
                                var rfidName = string.Format("{0}-{1}", _currentInputCarrier.RFIDData.RFIDTagData.WorkOrder, _currentInputCarrier.RFIDData.RFIDTagData.WorkOrderVersion);
                                _controller.SaveWoOrRecipeChangeToLog("WORKORDER", String.Format("Selected workorder name and RFID workorder name is not the same, Selected Workorder =>'{0}', RFID Workorder => {1} please check", selectedName, rfidName));

                                HSTException.Throw(HSTErrors.InputRFIDReadWorkorderFailed, new Exception(String.Format("Selected workorder name and RFID workorder name is not the same, Selected Workorder =>'{0}', RFID Workorder => {1} please check", selectedName, rfidName)));
                            }

                            //Check compare recipe name between RFID and Operator selected
                            if (!_controller.CheckCompareHSTRecipeName(_currentInputCarrier))
                            {
                                TransitionTo(stateRecipeChangedError);
                                return null;
                            }

                            if (CommonFunctions.Instance.ProductRecipeName != "")
                            {
                                string recipename = CommonFunctions.Instance.ProductRecipeName + CommonFunctions.RecipeExt;
                                if (CommonFunctions.Instance.CheckCompareRecipeFileBetweenGlobalAndLocal(recipename))
                                {
                                    CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage = true;
                                    CommonFunctions.Instance.RecipeChangeErrorMessage = "Global recipe setting have been changed, please re-load file again!";
                                    _controller.SaveWoOrRecipeChangeToLog("RECIPE", "Recipe setting has been changed");
                                }
                                else
                                    CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage = false;
                            }

                            if (_controller.CheckCarrierInformation(_currentInputCarrier))
                            {
                                //update carrier object on rfidInfoVerify status
                                _currentInputCarrier.RFIDInfoVerify = RFIDInfoVerify.Matched;
                                string temp = string.Format("{0}-{1}", Carriers[0].CarrierID, Carriers[0].ImageFileName);
                                _currentInputCarrier.ImageFileName = temp;

                                TransitionTo(stateIsCarrierEmptyOrAllHGAsFailedVerify);
                            }
                            else
                            {
                                errorId = 1;
                                HSTException.Throw(HSTErrors.InputRFIDReadFailed, new Exception(String.Format("Last step complete not correct, reading last step =>'{0}', please check", _currentInputCarrier.RFIDData.RFIDTag.LastStep)));
                            }
                        }
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, HGA detection result from camera at Input Turn Table: Carrier ID:{2}, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}, IsPassThrough:{13}", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID,
                            _currentInputCarrier.Hga1.Hga_Status, _currentInputCarrier.Hga2.Hga_Status, _currentInputCarrier.Hga3.Hga_Status, _currentInputCarrier.Hga4.Hga_Status, _currentInputCarrier.Hga5.Hga_Status,
                            _currentInputCarrier.Hga6.Hga_Status, _currentInputCarrier.Hga7.Hga_Status, _currentInputCarrier.Hga8.Hga_Status, _currentInputCarrier.Hga9.Hga_Status, _currentInputCarrier.Hga10.Hga_Status,
                            _currentInputCarrier.IsPassThroughMode.ToString());
                    }
                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = null;
                    if (errorId > 0)
                        btnlst = new ButtonList(ErrorButton.NoButton, ErrorButton.Reject, ErrorButton.Stop);
                    else
                        btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.Stop);

                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateIsCarrierEmptyOrAllHGAsFailedVerify(IQEvent qEvent)
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
                                TransitionTo(stateInputLifterDown);
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
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true ||
                    (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true))
                {
                    _currentInputCarrier.IsPassThroughMode = true;
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StateIsCarrierEmptyOrAllHGAsFailedVerify, Current Input Carrier '{2}' is empty and therefore will be passed through.", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID);
                    }
                    TransitionTo(stateInputLifterDown);
                }
                else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                {
                    //skip the carrier if it is empty or all incoming HGAs status is failed
                    if (_currentInputCarrier.IsCarrierEmpty == IsCarrierEmpty.NotEmpty)
                    {
                        if (!_controller.AllHGAsFailed(_currentInputCarrier))
                        {
                            _controller.LoadRecipeInfo(_currentInputCarrier);

                            InputStationRFIDReadProcessCycleTimeStopWatch.Stop();
                            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                            {
                                CommonFunctions.Instance.LogProcessCycleTime("Input Station RFID Read Process Cycle Time.csv", InputStationRFIDReadProcessCycleTimeStopWatch.ElapsedTime);
                            }
                            TransitionTo(stateIsCarrierHGAsMissingSerialNo);
                        }
                        else
                        {
                            _currentInputCarrier.IsPassThroughMode = true;
                            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                            {
                                Log.Info(this, "{0}, ProcessName:{1}, StateName:StateIsCarrierEmptyOrAllHGAsFailedVerify, All HGAs Status in Current Input Carrier '{2}' is failed and therefore will be passed through.", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID);
                            }
                            TransitionTo(stateInputLifterDown);
                        }
                    }
                    else
                    {
                        _currentInputCarrier.IsPassThroughMode = true;
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "{0}, ProcessName:{1}, StateName:StateIsCarrierEmptyOrAllHGAsFailedVerify, Current Input Carrier '{2}' is empty and therefore will be passed through.", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID);
                        }
                        TransitionTo(stateInputLifterDown);
                    }
                }
                else
                {
                    try
                    {
                        lock (CommonFunctions.Instance.InputCarriersLock)
                        {
                            if (_currentInputCarrier.IsCarrierEmpty == IsCarrierEmpty.NotEmpty)
                            {
                                if (!_controller.AllHGAsFailed(_currentInputCarrier))
                                {
                                    _controller.LoadRecipeInfo(_currentInputCarrier);
                                    TransitionTo(stateIsCarrierHGAsMissingSerialNo);
                                }
                                else
                                {
                                    _currentInputCarrier.IsPassThroughMode = true;
                                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                                    {
                                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StateIsCarrierEmptyOrAllHGAsFailedVerify, All HGAs Status in Current Input Carrier '{2}' is failed and therefore will be passed through.", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID);
                                    }
                                    try
                                    {
                                        HSTException.Throw(HSTErrors.InputRFIDReadFoundAllHGAsFailed, new Exception("RFID read found all HGAs failed by previous systems."));
                                    }
                                    catch (Exception ex)
                                    {
                                        ButtonList btnlst = new ButtonList(ErrorButton.Reject, ErrorButton.NoButton, ErrorButton.Stop);
                                        TransitionToErrorState(btnlst, ex);
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                _currentInputCarrier.IsPassThroughMode = true;
                                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                                {
                                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateIsCarrierEmptyOrAllHGAsFailedVerify, Current Input Carrier '{2}' is empty and therefore will be passed through.", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID);
                                }
                                TransitionTo(stateInputLifterDown);
                            }
                        }

                        _retryCount = 0;
                    }
                    catch (Exception ex)
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.Stop);
                        TransitionToErrorState(btnlst, ex);
                        return null;
                    }
                }
                return null;
            }
            return stateRun;
        }

        private QState StateIsCarrierHGAsMissingSerialNo(IQEvent qEvent)
        {

            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Yes: // Continue to test HGAs
                            {
                                TransitionTo(stateForwardClampAndRotateOpen);
                            }
                            break;
                        case ErrorButton.No: // Skip testing HGAs
                            {
                                _currentInputCarrier.IsPassThroughMode = true;
                                //Mar 2020 Add is rejected Carrier
                                _currentInputCarrier.IsRejectedCarrier = true;
                                TransitionTo(stateInputLifterDown);
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
                    int numberOfHGAWithoutSerialNo = _controller.NumberOfHGAWithoutSerialNo(_currentInputCarrier);

                    if (numberOfHGAWithoutSerialNo > 0)
                        HSTException.Throw(HSTErrors.InputRFIDReadFoundAHGAsMissingSerialNo, new Exception(String.Format("Found {0} HGAs without HGA Serial No. \nPress 'Yes' to continue HGA testing, and 'No'to skip the testing.", numberOfHGAWithoutSerialNo)));
                    else
                        TransitionTo(stateInputLifterUp);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Yes, ErrorButton.No, ErrorButton.Stop);
                    TransitionToErrorState(btnlst, ex);
                    return null;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateInputLifterUp(IQEvent qEvent)
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
                                _currentInputCarrier.IsRejectedCarrier = true;
                                Log.Info(this, "{0}, ProcessName:{1}, StateInputLifterUp, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                _retryCount = 0;
                                TransitionTo(stateInputLifterDown);
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
                    BoatLiftedAtInputStationProcessCycleTimeStopWatch.Start();

                    var inputISIRequestEvent = new QEvent(HSTWorkcell.SigInputRFIDReadComplete);
                    inputISIRequestEvent.EventObject = _currentInputCarrier;
                    QF.Instance.Publish(inputISIRequestEvent);

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass &&
                    HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true &&
                    HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true)
                    {
                        // do nothing
                    }
                    else
                        _controller.RaiseInputLifter(true, out _timeUsed);
                    Thread.Sleep(200);

                    BoatLiftedAtInputStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Boat Lifted At Input Station Process Cycle Time.csv", BoatLiftedAtInputStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    InputStationStopperRetractionProcessCycleTimeStopWatch.Start();
                    InputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch.Start();

                    if (_currentInputCarrier == null)
                        throw new Exception("Failed to find valid input carrier object to be assigned with the read RFID data.");
                    else
                        _controller.RfidController.CarrierCount++;

                    _workcell.Process.InputEEProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();

                    if (_currentInputCarrier.IsDycemBoat)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StateInputLifterUp, Current Input Carrier is a Dycem Boat.", LoggerCategory.StateTransition, _processName);
                        QF.Instance.Publish(new QEvent(HSTWorkcell.SigInputEEStartDycemCleaning));
                        TransitionTo(stateWaitForDycemCleaningComplete);
                    }
                    else
                        TransitionTo(stateForwardClampAndRotateOpen);

                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to move input lifter up or move stopper down. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.Stop);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }

            return stateRun;
        }

        private QState StateForwardClampAndRotateOpen(IQEvent qEvent)
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
                                _currentInputCarrier.IsRejectedCarrier = true;
                                _controller.InputStationForwardClamp(false);
                                Thread.Sleep(250);
                                _controller.InputStationClampRotaryOpenCover(false);

                                Log.Info(this, "{0}, ProcessName:{1}, StateForwardClampAndRotateOpen, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                _retryCount = 0;
                                TransitionTo(stateInputLifterDown);
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
                    _controller.InputStationForwardClamp(true);
                    InputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Station Carrier Screwdriver Deployment Process Cycle Time.csv", InputStationCarrierScrewdriverDeploymentProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    InputStationCarrierUnlockProcessCycleTimeStopWatch.Start();
                    _controller.InputStationClampRotaryOpenCover(true);

                    Thread.Sleep(200);

                    //Check and re-open clamp again
                    if (!Controller.IsInputStationCarrierClampOpen())
                    {
                        _controller.InputStationForwardClamp(false);
                        Thread.Sleep(250);
                        _controller.InputStationClampRotaryOpenCover(false);
                        Thread.Sleep(250);
                        _controller.InputStationClampRotaryOpenCover(true);
                        Thread.Sleep(200);
                        if (!Controller.IsInputStationCarrierClampOpen())
                        {
                            throw new Exception(String.Format("Input carrier clamp is not opened, please check!!"));
                        }
                    }

                    InputStationCarrierUnlockProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Station Carrier Unlock Process Cycle Time.csv", InputStationCarrierUnlockProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    TransitionTo(stateBackwardClampAndRotateClose);

                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Reject, ErrorButton.Stop);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateBackwardClampAndRotateClose(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    InputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch.Start();
                    _controller.InputStationForwardClamp(false);
                    Thread.Sleep(250);
                    _controller.InputStationClampRotaryOpenCover(false);
                    InputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Station Carrier Screwdriver Retraction Process Cycle Time.csv", InputStationCarrierScrewdriverRetractionProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    _retryCount = 0;

                    if (_currentInputCarrier.IsRejectedCarrier)
                        TransitionTo(stateInputLifterDown);
                    else
                    {
                        TransitionTo(stateWaitForGetISIDataCompleted);
                    }
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to backward clamp and rotate close. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        _retryCount++;
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
            return stateRun;
        }

        private QState StateWaitForGetISIDataCompleted(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigInputGetISIDataComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputGetISIDataComplete);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputGetISIDataComplete))
            {
                QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGAsReadyToPickAtInputLifter));
                _workcell.Process.InputEEProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();
                TransitionTo(stateWaitForInputEECompleted);

                return null;
            }
            return stateRun;
        }

        private QState StateWaitForInputEECompleted(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigHGAsInputEECompletePick))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigHGAsInputEECompletePick);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigHGAsInputEECompletePick))
            {
                TransitionTo(stateInputLifterDown);
                return null;
            }
            return stateRun;
        }

        private QState StateInputLifterDown(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    lock (CommonFunctions.Instance.OutputCarriersQueue)
                    {
                        _workcell.Process.InputEEProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();
                        CommonFunctions.Instance.OutputCarriersQueue.Enqueue(_workcell.Process.InputEEProcess.Controller.IncomingCarrier);
                    }

                    InputStationInputLifterLoweredProcessCycleTimeStopWatch.Start();

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass &&
                    HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true &&
                    HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true)
                    {
                        // do nothing
                    }
                    else
                        _controller.RaiseInputLifter(false, out _timeUsed);

                    Thread.Sleep(1000);
                    _controller.RfidController.CarrierCount++;

                    InputStationInputLifterLoweredProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Station Input Lifter Lowered Process Cycle Time.csv", InputStationInputLifterLoweredProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    InputStationStopperRetractionProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input Station Stopper Retraction Process Cycle Time.csv", InputStationStopperRetractionProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    BoatLeavesInputStationProcessCycleTimeStopWatch.Start();

                    TransitionTo(stateWaitForBufferStationReady);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to move lifter down. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
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
            return stateRun;
        }

        private QState StateWaitForBufferStationReady(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                if (_controller.IsBufferStationHoldCarrier())
                {
                    _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                }
                else
                {
                    HGALoadingAtInputStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("HGA Loading At Input Station Process Cycle Time.csv", HGALoadingAtInputStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    TransitionTo(StateReleaseCarrier);
                }
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
                        try
                        {
                            _controller.RaiseInputStationStopper(false, out _timeUsed);
                        }
                        catch (Exception ex)
                        {
                            ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                            TransitionToErrorState(btnlst, ex);
                        }

                        _controller.InhibitBufferStation(true);
                        _controller.InhibitInputStation(false);

                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            _controller.WaitInputStationPartCleared();
                        }

                        _controller.InhibitInputStation(true);
                        if (_currentInputCarrier.IsRejectedCarrier)
                        {

                            HSTMachine.Workcell.Process.OutputStationProcess._currentOutputCarrier = _currentInputCarrier.DeepCopy();
                        }

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            //lai
                            Log.Info(this, "{0}, lai ProcessName:{1}, In InputStationProcess' StateReleaseCarrier', _currentInputCarrier: Carrier ID:{2}, " +
                            "HGA1:{3}, HGA2:{4}, HGA3:{5}, HGA4:{6}, HGA5:{7}, " +
                            "HGA6:{8},  HGA7:{9}, HGA8:{10}, HGA9:{11}, HGA10:{12}", LoggerCategory.StateTransition, _processName, _currentInputCarrier.CarrierID,
                            _currentInputCarrier.Hga1.Hga_Status, _currentInputCarrier.Hga2.Hga_Status, _currentInputCarrier.Hga3.Hga_Status, _currentInputCarrier.Hga4.Hga_Status, _currentInputCarrier.Hga5.Hga_Status,
                            _currentInputCarrier.Hga6.Hga_Status, _currentInputCarrier.Hga7.Hga_Status, _currentInputCarrier.Hga8.Hga_Status, _currentInputCarrier.Hga9.Hga_Status, _currentInputCarrier.Hga10.Hga_Status);
                        }
                        BoatLeavesInputStationProcessCycleTimeStopWatch.Stop();
                        ProcessStopWatch PSW1 = new ProcessStopWatch("", new Stopwatch());
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("Boat Leaves Input Station Process Cycle Time.csv", BoatLeavesInputStationProcessCycleTimeStopWatch.ElapsedTime);
                            CommonFunctions.Instance.BoatArrivesAtOutputStationFromInputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                        }

                        //for simulation mode
                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                            (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
                        {
                            _workcell.IsCarrierInBufferZone = true;
                        }
                        TransitionTo(stateRaiseInputStopper);
                    }
                    catch (Exception ex)
                    {
                        _releaseError = true;//No matter this is a local zone or next zone error
                        throw new Exception("Input Station release carrier failed.", ex);
                    }
                }
                catch (Exception ex2)
                {
                    if (_retryCount < 3)
                    {
                        Log.Error(this, "Failed to release carrier. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex2.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex2);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForDycemCleaningComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigInputEEDycemCleaningComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputEEDycemCleaningComplete);
                    return null;
                }
                return null;

            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputEEDycemCleaningComplete))
            {
                TransitionTo(stateInputLifterDown);
                return null;
            }

            return stateRun;
        }

        private QState StateReportRecipeNameError(IQEvent qEvent)
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
                                TransitionTo(stateAllOutputProcssDone);
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
                    CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage = true;
                    HSTException.Throw(HSTErrors.TestElectronicsRecipeChangedErrorDetection, new Exception(CommonFunctions.Instance.AlertRecipeChangeErrorMessage));
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

        private QState StateAllOutputProcessDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (!_workcell.Process.InputStationProcess.Controller.IsBufferStationHoldCarrier() &&
                        !CommonFunctions.Instance.IsOutputWorkingProcess &&
                        !_workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier() &&
                        _workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationTurn0Deg())
                    {
                        _workcell.Process.Stop();
                    }
                    else
                    {
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 500), new QEvent(_sigTimeout));
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

        #endregion
    }
}
