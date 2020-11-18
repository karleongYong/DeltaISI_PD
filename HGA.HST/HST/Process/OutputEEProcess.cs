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
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Logging;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Process
{

    public class OutputEEProcess : ActiveProcessHST
    {
        private OutputEEController _controller;
        protected HSTWorkcell _workcell;
        private Carrier _currentTestedCarrier = null;
        private Stopwatch OutputEEMovesDownToPrecisorNestPickPositionProcessCycleTimeStopWatch;
        private Stopwatch OutputEEPicksUpHGAFromPrecisorNestProcessCycleTimeStopWatch;
        private Stopwatch OutputEELeavesPrecisorNestAfterHGAPickProcessCycleTimeStopWatch;
        private Stopwatch OutputEEPlacesDownHGAOntoOutputCarrierProcessCycleTimeStopWatch;
        private Stopwatch OutputEELeavesOutputCarrierAfterHGAPlaceProcessCycleTimeStopWatch;

        //Time Monitoring
        private Stopwatch OutputEEDownToPrecisorNestPositionTravellingTimeStopWatch;
        private Stopwatch OutputEEUpFromPrecisorNestPositionTravellingTimeStopWatch;
        private Stopwatch OutputEEDownToLifterPositionTravellingTimeStopWatch;
        private Stopwatch OutputEEUpFromLifterPositionTravellingTimeStopWatch;
        private Stopwatch WaitOutputCarrierReadyForPlaceTimeStopWatch;
        private Stopwatch OutputLifterPurgeProcessTime;
        
        //RunInit mode
        private QState stateMoveToHomePosition;
        private QState statePublishOutputEEHomed;
        private QState stateWaitForSigStartBoundaryCheckOutputEEAxis;
        private QState stateBoundaryCheck;
        private QState statePublishOutputEEBoundaryCheckComplete;
        private QState stateRunInitCompleteMoveToParkPosition;

        //Run mode
        private QState stateWaitForPrecisorReadyForPick;
        private QState statePickHGAVacuumOn;
        private QState statePickHGAZDown;
        private QState statePickHGACheckVacuumAndMoveUp;
        private QState stateAlertOperatorHGADrop;
        private QState statePublishSigPrecisorPickDone;
        private QState stateWaitForSigOutputCarrierReadyForPlace;
        private QState statePlaceHGAZDown;
        private QState statePlaceHGAVacuumOff;
        private QState statePlaceHGAZUp;
        private QState statePublishSigOutputCarrierPlaceDone;
        private QState statePerformCleaningWithDycem;

        private int _retryCount = 0;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private bool OutputCarrierReadyForPlaceSignalReceived = false;
        private bool PrecisorNestIsAtParkPositionSignalReceived = false;

        // Constructors ------------------------------------------------------------
        public OutputEEProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //RunInit mode
            stateMoveToHomePosition = new QState(this.StateMoveToHomePosition);
            statePublishOutputEEHomed = new QState(this.StatePublishOutputEEHomed);
            stateWaitForSigStartBoundaryCheckOutputEEAxis = new QState(this.StateWaitForSigStartBoundaryCheckOutputEEAxis);
            stateBoundaryCheck = new QState(this.StateBoundaryCheck);
            statePublishOutputEEBoundaryCheckComplete = new QState(this.StatePublishOutputEEBoundaryCheckComplete);
            stateRunInitCompleteMoveToParkPosition = new QState(this.StateRunInitCompleteMoveToParkPosition);

            //Run mode
            stateWaitForPrecisorReadyForPick = new QState(this.StateWaitForPrecisorReadyForPick);
            statePickHGAVacuumOn = new QState(this.StatePickHGAVacuumOn);
            statePickHGAZDown = new QState(this.StatePickHGAZDown);            
            statePickHGACheckVacuumAndMoveUp = new QState(this.StatePickHGACheckVacuumAndMoveUp);
            stateAlertOperatorHGADrop = new QState(this.StateAlertOperatorHGADrop);
            statePublishSigPrecisorPickDone = new QState(this.StatePublishSigPrecisorPickDone);
            stateWaitForSigOutputCarrierReadyForPlace = new QState(this.StateWaitForSigOutputCarrierReadyForPlace);
            statePlaceHGAZDown = new QState(this.StatePlaceHGAZDown);
            statePlaceHGAVacuumOff = new QState(this.StatePlaceHGAVacuumOff);
            statePlaceHGAZUp = new QState(this.StatePlaceHGAZUp);
            statePublishSigOutputCarrierPlaceDone = new QState(this.StatePublishSigOutputCarrierPlaceDone);
            statePerformCleaningWithDycem = new QState(this.StatePerformCleaningWithDycem);
        }

        // Properties ----------------------------------------------------------

        public OutputEEController Controller
        { get { return _controller; } }

        // Methods -------------------------------------------------------------

        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigStartBoundaryCheckOutputEEAxis);  
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorReadyForPick);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputCarrierReadyForPlace);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtInputStation);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEStartDycemCleaning);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new OutputEEController(_workcell, "OutputEE", "Output EE");
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

        #region StateRunInit
        protected override QState StateRunInit(IQEvent qEvent)
        {
            OutputEEMovesDownToPrecisorNestPickPositionProcessCycleTimeStopWatch = new Stopwatch();
            OutputEEPicksUpHGAFromPrecisorNestProcessCycleTimeStopWatch = new Stopwatch();
            OutputEELeavesPrecisorNestAfterHGAPickProcessCycleTimeStopWatch = new Stopwatch();            
            OutputEEPlacesDownHGAOntoOutputCarrierProcessCycleTimeStopWatch = new Stopwatch();
            OutputEELeavesOutputCarrierAfterHGAPlaceProcessCycleTimeStopWatch = new Stopwatch();            

            //Time Monitoring
            OutputEEDownToPrecisorNestPositionTravellingTimeStopWatch = new Stopwatch();
            OutputEEUpFromPrecisorNestPositionTravellingTimeStopWatch = new Stopwatch();
            OutputEEDownToLifterPositionTravellingTimeStopWatch = new Stopwatch();
            OutputEEUpFromLifterPositionTravellingTimeStopWatch = new Stopwatch();
            WaitOutputCarrierReadyForPlaceTimeStopWatch = new Stopwatch();
            OutputLifterPurgeProcessTime = new Stopwatch();
            
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);             

            if (qEvent.IsSignal(SigStateJob))
            {
                OutputCarrierReadyForPlaceSignalReceived = false;
                PrecisorNestIsAtParkPositionSignalReceived = false;
                HSTWorkcell.outputEEPickDone = false;
                                
                HSTWorkcell.outputEEHoldingHGAs = false;
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_VCH).Set(DigitalIOState.Off);
                }
                
                if (HSTWorkcell.disableBoundaryCheck)
                    TransitionTo(stateRunInitCompleteMoveToParkPosition);
                else
                    TransitionTo(stateMoveToHomePosition);
                return null;
            }
            return base.StateRunInit(qEvent);
        }        

        private QState StateMoveToHomePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {                
                try
                {
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.HomeZAxis();
                    
                    TransitionTo(statePublishOutputEEHomed);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishOutputEEHomed(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigOutputEEHomed));
                    TransitionTo(stateWaitForSigStartBoundaryCheckOutputEEAxis);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateWaitForSigStartBoundaryCheckOutputEEAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigStartBoundaryCheckOutputEEAxis))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigStartBoundaryCheckOutputEEAxis);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigStartBoundaryCheckOutputEEAxis))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:SigStartBoundaryCheckOutputEEAxis, Received QSignal:{2} and transition to new State:StateBoundaryCheck", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                TransitionTo(stateBoundaryCheck);
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
                    if(!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.BoundaryCheck();
                    TransitionTo(statePublishOutputEEBoundaryCheckComplete);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishOutputEEBoundaryCheckComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigOutputEEBoundaryCheckComplete));
                    TransitionTo(stateRunInitCompleteMoveToParkPosition);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateRunInitCompleteMoveToParkPosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if(!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.DoJobMoveZToPark(false, false);

                    if (HSTWorkcell.disableBoundaryCheck)
                        PublishSignal(new QEvent(HSTWorkcell.SigOutputEEBoundaryCheckComplete));
                    TransitionTo(stateWaitForPrecisorReadyForPick);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        #endregion

        #region StateRun
        private QState StateWaitForPrecisorReadyForPick(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);                  

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigPrecisorReadyForPick))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigPrecisorReadyForPick);
                    return null;
                }

                if (RecallDeferredSignal(HSTWorkcell.SigOutputEEStartDycemCleaning))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputEEStartDycemCleaning);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEStartDycemCleaning))
            {
                TransitionTo(statePerformCleaningWithDycem);
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigPrecisorReadyForPick))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForPrecisorReadyForPick, Received QSignal:{2} and transition to new State:StatePickHGAVacuumOn", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }

                HSTMachine.Workcell.Process.OutputEEProcess.Controller.All10PartsPicked = false;
                TransitionTo(statePickHGAVacuumOn);
                return null;
            }
            return stateRun;
        }

        private QState StatePickHGAVacuumOn(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    bool IsOn = true;
                    _controller.SetVacuum(IsOn);
                    TransitionTo(statePickHGAZDown);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePickHGAZDown(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (_controller.IsSafeToMoveDown(false))
                    {
                        OutputEEMovesDownToPrecisorNestPickPositionProcessCycleTimeStopWatch.Start();

                        OutputEEDownToPrecisorNestPositionTravellingTimeStopWatch.Start();

                        bool isUp = (_controller.IncomingCarrier.HGATabType == HGAProductTabType.Up);
                        _controller.PickHGAMoveDown(isUp);
                        HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOffVaccuumChannels();
                        Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.VacuumOffAtPrecisorBeforeOutputEEPick);//original value 250                        
                        OutputEEDownToPrecisorNestPositionTravellingTimeStopWatch.Stop();

                        if (HSTMachine.Workcell != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation() != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                {
                                    UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                    {
                                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelOutputDownToPrecisorNestPositionTravellingTime.Text = OutputEEDownToPrecisorNestPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                    });
                                }
                            }
                        }

                        OutputEEMovesDownToPrecisorNestPickPositionProcessCycleTimeStopWatch.Stop();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("Output EE Moves Down To Precisor Nest Pick Position Process Cycle Time.csv", OutputEEMovesDownToPrecisorNestPickPositionProcessCycleTimeStopWatch.ElapsedTime);

                            if (CommonFunctions.Instance.PrecisorNestStabilityAtOutputStationProcessCycleTimeStopWatch.Count > 0)
                            {
                                ProcessStopWatch PSW = CommonFunctions.Instance.PrecisorNestStabilityAtOutputStationProcessCycleTimeStopWatch.First();
                                PSW.Stop();
                                CommonFunctions.Instance.LogProcessCycleTime("Precisor Nest Stability At Output Station Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                                CommonFunctions.Instance.PrecisorNestStabilityAtOutputStationProcessCycleTimeStopWatch.Dequeue();
                            }
                        }

                        OutputEEPicksUpHGAFromPrecisorNestProcessCycleTimeStopWatch.Start();

                        TransitionTo(statePickHGACheckVacuumAndMoveUp);
                    }
                    else
                    {
                        //lai: reduce from 200 march12-2016
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 150), new QEvent(_sigTimeout));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }
        
        private QState StatePickHGACheckVacuumAndMoveUp(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (_controller.IsVacuumOK())
                    {
                        OutputEEPicksUpHGAFromPrecisorNestProcessCycleTimeStopWatch.Stop();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("Output EE Picks Up HGAs From Precisor Nest Process Cycle Time.csv", OutputEEPicksUpHGAFromPrecisorNestProcessCycleTimeStopWatch.ElapsedTime);
                        }

                        OutputEELeavesPrecisorNestAfterHGAPickProcessCycleTimeStopWatch.Start();

                        OutputEEUpFromPrecisorNestPositionTravellingTimeStopWatch.Start();
                        _controller.PickHGAMoveUp();
                        
                        HSTWorkcell.outputEEHoldingHGAs = true;
                        OutputEEUpFromPrecisorNestPositionTravellingTimeStopWatch.Stop();

                        if (HSTMachine.Workcell != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation() != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                {
                                    UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                    {
                                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelOutputUpFromPrecisorNestPositionTravellingTime.Text = OutputEEUpFromPrecisorNestPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                    });
                                }
                            }
                        }

                        OutputEELeavesPrecisorNestAfterHGAPickProcessCycleTimeStopWatch.Stop();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("Output EE Leaves Precisor Nest After HGA Pick Process Cycle Time.csv", OutputEELeavesPrecisorNestAfterHGAPickProcessCycleTimeStopWatch.ElapsedTime);
                        }

                        ProcessStopWatch PSW1 = new ProcessStopWatch(_controller.IncomingCarrier.CarrierID, new Stopwatch());
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.CollisionAvoidanceBetweenOutputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                        }

                        TransitionTo(statePublishSigPrecisorPickDone);
                    }
                    else
                    {
                        TransitionTo(stateAlertOperatorHGADrop);                        
                    }
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePublishSigPrecisorPickDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);          
            
            if (qEvent.IsSignal(SigStateJob))
            {
                HSTWorkcell.outputEEPickDone = true;
                if (CheckDeferredSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition))
                {
                    RemoveDeferredSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition);
                }

                PublishSignal(new QEvent(HSTWorkcell.SigPrecisorPickDone));
                TransitionTo(stateWaitForSigOutputCarrierReadyForPlace);
            }
            return stateRun;
        }

        private QState StateWaitForSigOutputCarrierReadyForPlace(IQEvent qEvent)
        {
            WaitOutputCarrierReadyForPlaceTimeStopWatch.Start();
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);          
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigOutputCarrierReadyForPlace))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputCarrierReadyForPlace);
                }
                if (RecallDeferredSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition);
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputCarrierReadyForPlace))
                OutputCarrierReadyForPlaceSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition))
                PrecisorNestIsAtParkPositionSignalReceived = true;

            if (OutputCarrierReadyForPlaceSignalReceived && PrecisorNestIsAtParkPositionSignalReceived)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigOutputCarrierReadyForPlace, Received QSignal:{2} and transition to new State:StatePlaceHGAZDown because OutputCarrierReadyForPlaceSignalReceived is true && PrecisorNestIsAtParkPositionSignalReceived is true", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                OutputCarrierReadyForPlaceSignalReceived = false;
                PrecisorNestIsAtParkPositionSignalReceived = false;

                TransitionTo(statePlaceHGAZDown);
                WaitOutputCarrierReadyForPlaceTimeStopWatch.Stop();

                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelWaitCarrierReadyForPlaceTime.Text = WaitOutputCarrierReadyForPlaceTimeStopWatch.ElapsedTime.ToString();
                            });
                        }
                    }
                }
                return null;
            }
            
            return stateRun;
        }

        private QState StatePlaceHGAZDown(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (_controller.IsSafeToMoveDown(true))
                    {
                        OutputEEDownToLifterPositionTravellingTimeStopWatch.Start();
                        
                        _controller.PlaceHGAMoveDown();
                        OutputEEDownToLifterPositionTravellingTimeStopWatch.Stop();

                        if (HSTMachine.Workcell != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation() != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                {
                                    UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                    {
                                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelOutputDownToLifterPositionTravellingTime.Text = OutputEEDownToLifterPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                    });
                                }
                            }
                        }

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            if (CommonFunctions.Instance.CollisionAvoidanceBetweenPrecisorNestAndOutputEEMovingDownToOutputCarrierProcessCycleTimeStopWatch.Count > 0)
                            {
                                ProcessStopWatch PSW = CommonFunctions.Instance.CollisionAvoidanceBetweenPrecisorNestAndOutputEEMovingDownToOutputCarrierProcessCycleTimeStopWatch.First();
                                PSW.Stop();
                                CommonFunctions.Instance.LogProcessCycleTime("Collision Avoidance Between Precisor Nest And Output EE Moving Down To Output Carrier Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                                CommonFunctions.Instance.CollisionAvoidanceBetweenPrecisorNestAndOutputEEMovingDownToOutputCarrierProcessCycleTimeStopWatch.Dequeue();
                            }
                        }

                        OutputEEPlacesDownHGAOntoOutputCarrierProcessCycleTimeStopWatch.Start();

                        TransitionTo(statePlaceHGAVacuumOff);
                    }
                    else
                    {
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePlaceHGAVacuumOff(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    bool IsOn = false;

                    OutputLifterPurgeProcessTime.Start();
                    _controller.SetVacuum(IsOn);
                    OutputLifterPurgeProcessTime.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelOutputLifterPurgeProcessTime.Text = OutputLifterPurgeProcessTime.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    OutputEEPlacesDownHGAOntoOutputCarrierProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Output EE Places Down HGAs Onto Output Carrier Process Cycle Time.csv", OutputEEPlacesDownHGAOntoOutputCarrierProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    OutputEELeavesOutputCarrierAfterHGAPlaceProcessCycleTimeStopWatch.Start();

                    TransitionTo(statePlaceHGAZUp);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePlaceHGAZUp(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    OutputEEUpFromLifterPositionTravellingTimeStopWatch.Start();
                                        
                    HSTWorkcell.outputEEHoldingHGAs = false;
                    OutputEEUpFromLifterPositionTravellingTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelOutputUpFromLifterPositionTravellingTime.Text = OutputEEUpFromLifterPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    OutputEELeavesOutputCarrierAfterHGAPlaceProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Output EE Leaves Output Carrier After HGA Place Process Cycle Time.csv", OutputEELeavesOutputCarrierAfterHGAPlaceProcessCycleTimeStopWatch.ElapsedTime);
                    }
                    
                    ProcessStopWatch PSW1 = new ProcessStopWatch(_controller.IncomingCarrier.CarrierID, new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.OutputStationHGADetectionStabilityProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    TransitionTo(statePublishSigOutputCarrierPlaceDone);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePublishSigOutputCarrierPlaceDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);   
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true)
                {
                    // Do nothing
                }
                else
                {
                    _workcell.Process.OutputStationProcess.Controller.IncomingTestedCarrier = CommonFunctions.Instance.TestedCarrierQueue.First();
                }

                if (CheckDeferredSignal(HSTWorkcell.SigOutputCarrierReadyForPlace))
                {
                    RemoveDeferredSignal(HSTWorkcell.SigOutputCarrierReadyForPlace);
                }

                _controller.PlaceHGAMoveUp();
                PublishSignal(new QEvent(HSTWorkcell.SigOutputCarrierPlaceDone));

                TransitionTo(stateWaitForPrecisorReadyForPick);                
            }
            return stateRun;
        }

        #endregion

        private QState StateAlertOperatorHGADrop(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);   
            
            if (qEvent.IsSignal(SigStateJob))
            {
                Notify.PopUp("User Action Required", "Detected low Output End Effector's vacuum reading. Please check if all HGAs are still attached to the Output End Effector.", "", "OK");
                HSTMachine.Instance.MainForm.getPanelCommand().stopSystemProcess();
                return stateStopping;
            }
            return stateStopping;
        }

        private QState StatePerformCleaningWithDycem(IQEvent qEvent)
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
                                Log.Info(this, "{0}, ProcessName:{1}, StatePerformCleaningWithDycem, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                PublishSignal(new QEvent(HSTWorkcell.SigOutputEEDycemCleaningComplete));
                                TransitionTo(stateWaitForPrecisorReadyForPick);
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
                    for (int i = 0; i < HSTMachine.Workcell.HSTSettings.Install.TotalNumberOfOutputEETouchingOnDycem; i++)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StatePerformCleaningWithDycem, Move Output EE to touch on Dycem. Cycle {2}", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString(), i + 1);
                        // move to touch on dycem
                        _controller.DoJobMoveZToDycem(false);
                        Thread.Sleep(HSTMachine.Workcell.HSTSettings.Install.OutputEETouchingOnDycemDuration * 1000);
                        // move to EE pick height
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StatePerformCleaningWithDycem, Move Output EE up to pick height. Cycle {2}", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString(), i + 1);
                        _controller.DoJobMoveZToPick(true, false);
                        HSTMachine.Workcell.DycemCleaningCounter.OutputEEDycemCleaningCount += 1;
                    }
                    _controller.DoJobMoveZToPark(false, false);

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel().getDycemCleaningCounterUserControl().txtOutputEECleanCount.Text = HSTMachine.Workcell.DycemCleaningCounter.OutputEEDycemCleaningCountString;
                                    HSTMachine.Workcell.DycemCleaningCounter.Save();
                                });
                            }
                        }
                    }

                    PublishSignal(new QEvent(HSTWorkcell.SigOutputEEDycemCleaningComplete));
                    TransitionTo(stateWaitForPrecisorReadyForPick);
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

    }
}
