using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Settings;
using System.Threading;
using qf4net;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Logging;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Process
{

    public class OutputTurnStationProcess : ActiveProcessHST
    {

        private OutputTurnStationController _controller;
        protected HSTWorkcell _workcell;
        private Stopwatch BoatLeavesOutputTurnStationProcessCycleTimeStopWatch;
        private IDigitalOutput _doSoftStartUp;

        //RunInit mode
        private QState stateRunInitStandbyOutputSection;

        //Run mode
        private QState stateRunStart;
        private QState stateWaitForSigCarrierPresentInOutputTurnStation;
        private QState stateCheckCarrierPresentInOutputTurnStation;
        private QState stateOutputTurnSectionTurnForward;
        private QState stateCheckNextZoneReady;
        private QState stateOutputTurnSectionReleaseCarrier;

        private bool _releaseError = false;
        private bool machineStop = true;
        private int _retryCount = 0;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");

        private uint _timeUsed = 0;
        // Constructors ------------------------------------------------------------
        public OutputTurnStationProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //RunInit mode
            stateRunInitStandbyOutputSection = new QState(this.StateRunInitStandbyOutputSection);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateWaitForSigCarrierPresentInOutputTurnStation = new QState(this.StateWaitForSigCarrierPresentInOutputTurnStation);
            stateCheckCarrierPresentInOutputTurnStation = new QState(this.StateCheckCarrierPresentInOutputTurnStation);
            stateOutputTurnSectionTurnForward = new QState(this.StateOutputTurnSectionTurnForward);
            stateCheckNextZoneReady = new QState(this.StateCheckNextZoneReady);
            stateOutputTurnSectionReleaseCarrier = new QState(this.StateOutputTurnSectionReleaseCarrier);

        }

        // Properties ----------------------------------------------------------

        public OutputTurnStationController Controller
        { get { return _controller; } }

        // Methods -------------------------------------------------------------

        // Internal methods ----------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigCarrierPresentInOutputTurnStation);
            AddAndSubscribeSignal(HSTWorkcell.SigEndRunProcess);

            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new OutputTurnStationController(_workcell, "OutputTurnStation", "Output Turn Station");
            try
            {
                _controller.SetProcessCode(this, 2);
                _controller.InitializeController();
                _doSoftStartUp = _workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Soft_Start_Up);
            }
            catch (Exception ex)
            {
            }
            base.Start(priority);
        }

        #region StateRunInit
        protected override QState StateRunInit(IQEvent qEvent)
        {
            BoatLeavesOutputTurnStationProcessCycleTimeStopWatch = new Stopwatch();
            HSTMachine.Workcell.OutputTurnStationBoatPositionError = false;

            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                TransitionTo(stateRunInitStandbyOutputSection);
                return null;
            }
            return base.StateRunInit(qEvent);

        }

        private QState StateRunInitStandbyOutputSection(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.OutputTurnSectionTurnTo0Deg(out _timeUsed);
                    _controller.InhibitOutputTurnStation(false);
                    Thread.Sleep(1000);
                    _controller.InhibitOutputTurnStation(true);
                    TransitionTo(stateRunStart);
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

        #endregion

        #region StateRun
        private QState StateRunStart(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            machineStop = false;

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    TransitionTo(stateWaitForSigCarrierPresentInOutputTurnStation);
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

        private QState StateWaitForSigCarrierPresentInOutputTurnStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                    if (RecallDeferredSignal(HSTWorkcell.SigCarrierPresentInOutputTurnStation))
                    {
                        UpdateSignalRecipient(HSTWorkcell.SigCarrierPresentInOutputTurnStation);
                        return null;
                    }

                    if (RecallDeferredSignal(HSTWorkcell.SigEndRunProcess))
                    {
                        UpdateSignalRecipient(HSTWorkcell.SigEndRunProcess);
                        return null;
                    }
            }

            if (qEvent.IsSignal(HSTWorkcell.SigEndRunProcess))
            {
                machineStop = true;
            }

            if (machineStop)
            {
                if (CommonFunctions.Instance.InputCarriers.Count == 0)
                {
                    _workcell.Process.Stop();
                }
                else
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "machineStop is true. InputCarriers.Count is {0}", CommonFunctions.Instance.InputCarriers.Count);
                    }
                }
            }

            if (qEvent.IsSignal(HSTWorkcell.SigCarrierPresentInOutputTurnStation))
            {
                TransitionTo(stateCheckCarrierPresentInOutputTurnStation);
                return null;
            }
            return stateRun;
        }

        private QState StateCheckCarrierPresentInOutputTurnStation(IQEvent qEvent)
        {
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun  && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        _controller.WaitOutputTurnStationPartReady();
                        PublishSignal(new QEvent(HSTWorkcell.SigCarrierIsInOutputTurnStation));
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        if (CommonFunctions.Instance.BoatArrivesAtOutputTurnStationFromOutputStationProcessCycleTimeStopWatch.Count > 0)
                        {
                            ProcessStopWatch PSW = CommonFunctions.Instance.BoatArrivesAtOutputTurnStationFromOutputStationProcessCycleTimeStopWatch.First();
                            PSW.Stop();
                            CommonFunctions.Instance.LogProcessCycleTime("Boat Arrives At Output Turn Station From Output Station Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                            CommonFunctions.Instance.BoatArrivesAtOutputTurnStationFromOutputStationProcessCycleTimeStopWatch.Dequeue();
                        }
                    }

                    BoatLeavesOutputTurnStationProcessCycleTimeStopWatch.Start();

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForCarrierPresentInOutputTurnStation, Received QSignal:{2} and transition to new State:StateOutputTurnSectionTurnForward", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                    }
                    TransitionTo(stateOutputTurnSectionTurnForward);
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

        private QState StateOutputTurnSectionTurnForward(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    //lai: Add delay march12-2016
                    Thread.Sleep(500);
                    if (!_controller.IsOutputTurnStationHoldCarrier())
                    {
                        int retrycount = 0;
                        while (!_controller.IsOutputTurnStationHoldCarrier())
                        {
                            if (retrycount < 4)
                                Thread.Sleep(500);
                            else
                                HSTException.Throw(HSTErrors.OutputTurnStationInPositionNotOnError, new Exception("Inposition on timeout"));
                            retrycount++;
                        }
                    }

                    _controller.OutputTurnSectionTurnTo90Deg(out _timeUsed);
                    TransitionTo(stateCheckNextZoneReady);
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

        private QState StateCheckNextZoneReady(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (_controller.NextZoneReady())
                    {
                        TransitionTo(stateOutputTurnSectionReleaseCarrier);
                    }
                    else
                    {
                        //lai: reduce from 200 march12-2016
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 150), new QEvent(_sigTimeout));
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

        private QState StateOutputTurnSectionReleaseCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {

                    // allow turn table to settle before release to avoid boat hit the corner of the output conveyor.
                    Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.OutputTurnTableFullyStopDelay);
                    _controller.InhibitOutputTurnStation(false);
                    _controller.WaitOutputTurnStationPartCleared();
                    
                    Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.OutputTurnTableReleaseBoatDelay );//required to ensure boat is cleared from turn table 
                    _controller.InhibitOutputTurnStation(true);

                    BoatLeavesOutputTurnStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Boat Leaves Output Turn Station Process Cycle Time.csv", BoatLeavesOutputTurnStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    Carrier currentCarrier;

                    //remove the carrier from list (can remove at later stage if required)
                    lock (CommonFunctions.Instance.InputCarriersLock)
                    {
                        currentCarrier = CommonFunctions.Instance.InputCarriers.Dequeue();
                    }


                    if (CommonFunctions.Instance.OverallHGATestProcessCycleTimeStopWatch.Count > 0)
                    {
                        ProcessStopWatch PSW = CommonFunctions.Instance.OverallHGATestProcessCycleTimeStopWatch.First();
                        PSW.Stop();

                        CommonFunctions.Instance.LogProcessCycleTime("Overall HGA Test Process Cycle Time.csv", PSW.GetElapsedTime(), currentCarrier.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());

                        if (!currentCarrier.IsPassThroughMode)
                        {
                            if (CommonFunctions.Instance.CarrierCycleTime != null)
                            {
                                CommonFunctions.Instance.CarrierCycleTime.TimeStampAtOutputTurnTable = DateTime.Now;

                                if (CommonFunctions.Instance.CarrierCycleTimeQueue.Count > 0)
                                {
                                    CarrierCycleTime FirstCarrierCycleTime = CommonFunctions.Instance.CarrierCycleTimeQueue.First();
                                    CommonFunctions.Instance.CarrierCycleTime.OverallProcessCycleTimeSpan = CommonFunctions.Instance.CarrierCycleTime.TimeStampAtOutputTurnTable.Subtract(FirstCarrierCycleTime.TimeStampAtOutputTurnTable);
                                    CommonFunctions.Instance.CarrierCycleTimeQueue.Enqueue(CommonFunctions.Instance.CarrierCycleTime);

                                    CommonFunctions.Instance.CarrierCycleTimeQueue.Dequeue();
                                }
                                else
                                {
                                    CommonFunctions.Instance.CarrierCycleTime.OverallProcessCycleTimeSpan = new TimeSpan(0);
                                    CommonFunctions.Instance.CarrierCycleTimeQueue.Enqueue(CommonFunctions.Instance.CarrierCycleTime);
                                }

                                CommonFunctions.Instance.CarrierCycleTime.LogHGACount();
                                double CycleTimeElapsed = ((CommonFunctions.Instance.CarrierCycleTime.OverallProcessCycleTimeSpan.Hours * 3600) + (CommonFunctions.Instance.CarrierCycleTime.OverallProcessCycleTimeSpan.Minutes * 60) + CommonFunctions.Instance.CarrierCycleTime.OverallProcessCycleTimeSpan.Seconds + (CommonFunctions.Instance.CarrierCycleTime.OverallProcessCycleTimeSpan.Milliseconds / 1000.0));
                                if (CycleTimeElapsed > 0.0)
                                {
                                    HSTMachine.Workcell.LoadCounter.CycleTime = CycleTimeElapsed;
                                }
                            }
                        }

                        CommonFunctions.Instance.OverallHGATestProcessCycleTimeStopWatch.Dequeue();

                        if (HSTMachine.Instance.MainForm.getPanelCommand().buttonRun.Enabled == false)
                        {
                            if (HSTMachine.Workcell != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation() != null)
                                {
                                    if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                    {
                                        UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel(), () =>
                                        {
                                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getProductionCounterUserControl().txtSystemUPH.Text = HSTMachine.Workcell.LoadCounter.UPHString;
                                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getProductionCounterUserControl().txtSystemUPH2.Text = HSTMachine.Workcell.LoadCounter.UPH2String;
                                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getProductionCounterUserControl().txtSystemCycleTime.Text = HSTMachine.Workcell.LoadCounter.CycleTimeString;
                                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getProductionCounterUserControl().txtProcessedHGACount.Text = HSTMachine.Workcell.LoadCounter.ProcessedHGACountString;
                                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getProductionCounterUserControl().txtSamplingCounter.Text = HSTMachine.Workcell.LoadCounter.GetCurrentSamplingNumber.ToString("F2");
                                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getProductionCounterUserControl().textBoxWRBrigeFailure.Text = HSTMachine.Workcell.LoadCounter.LastWRBridgePercentage.ToString("F2");
                                            HSTMachine.Workcell.LoadCounter.Save();
                                        });
                                    }
                                }
                            }
                        }
                    }

                    TransitionTo(StateOutputTurnSectionTurnBackward);
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

        private QState StateOutputTurnSectionTurnBackward(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.OutputTurnSectionTurnTo0Deg(out _timeUsed);
                    _workcell.IsCarrierInOutputTurnTable = false;
                    TransitionTo(stateWaitForSigCarrierPresentInOutputTurnStation);
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
        #endregion
    }
}
