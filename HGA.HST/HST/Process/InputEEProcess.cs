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
using XyratexOSC.UI;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Process
{
    public class InputEEProcess : ActiveProcessHST
    {
        // Member variables ----------------------------------------------------
        private InputEEController _controller;
        private bool _isUnloadEE1;
        private Carrier _currentInputCarrier = null;
        private Stopwatch InputEEMovesDownToInputStationPickPositionProcessCycleTimeStopWatch;
        private Stopwatch InputEEPicksUpHGAsFromInputStationProcessCycleTimeStopWatch;
        private Stopwatch InputEEPlacesDownHGAOntoPrecisorNestProcessCycleTimeStopWatch;
        private Stopwatch InputEEMovesDownToPrecisorNestProcessCycleTimeStopWatch;
        private Stopwatch InputEELeavesPrecisorNestAfterHGAPlaceProcessCycleTimeStopWatch;

        //Time Monitoring
        private Stopwatch InputEEDownToLifterPositionTravellingTimeStopWatch;
        private Stopwatch InputEELifterVacuumProcessTimeStopWatch;
        private Stopwatch InputEEUpFromLifterPositionTravellingTimeStopWatch;
        private Stopwatch InputEEWaitPrecisorComeToInputStationTimeStopWatch;
        private Stopwatch InputEEDownToPrecisorNestPositionTravellingTimeStopWatch;
        private Stopwatch InputEEPrecisorVacuumProcessTimeStopWatch;
        private Stopwatch InputEEUpFromPrecisorNestPositionTravellingTimeStopWatch;
        
        #region HSM declaration
        // Delegates
//        private QState stateRunInitCheckSafeToInit;
//        private QState stateRunInitHomeZ;  

        //RunInit mode
        private QState stateRunInitMoveToHome;
        private QState statePublishInputEEHomed;
        private QState stateWaitForSigStartBoundaryCheckInputEEAxis;
        private QState stateBoundaryCheck;
        private QState statePublishInputEEBoundaryCheckComplete;
        private QState stateCheckVaccuum;
        private QState stateRunInitCompleteMoveToParkPosition;

        //Run mode
        private QState stateRunStart;
        private QState stateRunCheckHGAReadyForPick;
        private QState statePickHgaVacuumOn;
        private QState statePickHgaZDown;
        private QState statePickHgaZUpToParkPosition;
        private QState statePickHgaCheckVacuum;
        private QState statePickHgaPublishPickDone;

 //       private QState statePickHgaStart;
 //       private QState stateCheckStopped;
//        private QState statePickHgaEnd;

        private QState stateRunCheckHGAReadyForPlace;
//        private QState stateWaitForVisionInspectionOnMissingHGAs;
//        private QState statePlaceHgaStart;
        private QState statePlaceHgaZDown;
        private QState statePlaceHgaVacuumOff;
//        private QState stateActivateFlattener;
//        private QState stateRequestPrecisorToTurnOnVaccum;
        private QState stateWaitPrecisorTurnOnAllVacuums;
//        private QState stateDeActivateFlattener;
        private QState statePlaceHgaZUpToParkPosition;
        private QState statePlaceHgaPublishPlaceDone;
//        private QState statePlaceHgaEnd;
        private QState statePerformCleaningWithDycem;

        #endregion        

        private int _retryCount = 0;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");

        // Constructors ------------------------------------------------------------
        public InputEEProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //RunInit mode     
            stateRunInitMoveToHome = new QState(this.StateRunInitMoveToHome);
            statePublishInputEEHomed = new QState(this.StatePublishInputEEHomed);
            stateWaitForSigStartBoundaryCheckInputEEAxis = new QState(this.StateWaitForSigStartBoundaryCheckInputEEAxis);
            stateBoundaryCheck = new QState(this.StateBoundaryCheck);
            statePublishInputEEBoundaryCheckComplete = new QState(this.StatePublishInputEEBoundaryCheckComplete);
            stateCheckVaccuum = new QState(this.StateCheckVaccuum);
            stateRunInitCompleteMoveToParkPosition = new QState(this.StateRunInitCompleteMoveToParkPosition);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateRunCheckHGAReadyForPick = new QState(this.StateRunCheckHGAReadyForPick);
            statePickHgaVacuumOn = new QState(this.StatePickHgaVacuumOn);
            statePickHgaZDown = new QState(this.StatePickHgaZDown);
            statePickHgaZUpToParkPosition = new QState(this.StatePickHgaZUpToParkPosition);
            statePickHgaCheckVacuum = new QState(this.StatePickHgaCheckVacuum);
            statePickHgaPublishPickDone = new QState(this.StatePickHgaPublishPickDone);
            stateRunCheckHGAReadyForPlace = new QState(this.StateRunCheckHGAReadyForPlace);
            statePlaceHgaZDown = new QState(this.StatePlaceHgaZDown);
            statePlaceHgaVacuumOff = new QState(this.StatePlaceHgaVacuumOff);
            stateWaitPrecisorTurnOnAllVacuums = new QState(this.StateWaitPrecisorTurnOnAllVacuums);
            statePlaceHgaZUpToParkPosition = new QState(this.StatePlaceHgaZUpToParkPosition);
            statePlaceHgaPublishPlaceDone = new QState(this.StatePlaceHgaPublishPlaceDone);
            statePerformCleaningWithDycem = new QState(this.StatePerformCleaningWithDycem);
        }

        // Internal methods ---------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigStartBoundaryCheckInputEEAxis);  
            AddAndSubscribeSignal(HSTWorkcell.SigHGAsReadyToPickAtInputLifter);
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorReadyForPlace);
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorVacuumOn);
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorPlaceDone);
            AddAndSubscribeSignal(HSTWorkcell.SigInputEEStartDycemCleaning);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new InputEEController(_workcell, "P15", "InputEE Controller");
            _controller.SetProcessCode(this, 1);
            _controller.InitializeController();

            base.Start(priority);
        }

        // Properties ----------------------------------------------------------
        public InputEEController Controller
        { get { return _controller; } }
        public bool PurgeEndingInProgress { get; set; }

        #region StateRunInit
        protected override QState StateRunInit(IQEvent qEvent)
        {
            InputEEMovesDownToInputStationPickPositionProcessCycleTimeStopWatch = new Stopwatch();
            InputEEPicksUpHGAsFromInputStationProcessCycleTimeStopWatch = new Stopwatch();
            InputEEPlacesDownHGAOntoPrecisorNestProcessCycleTimeStopWatch = new Stopwatch();            
            InputEEMovesDownToPrecisorNestProcessCycleTimeStopWatch = new Stopwatch();
            InputEELeavesPrecisorNestAfterHGAPlaceProcessCycleTimeStopWatch = new Stopwatch();            
            
            //Time monitoring
            InputEEDownToLifterPositionTravellingTimeStopWatch = new Stopwatch();
            InputEELifterVacuumProcessTimeStopWatch = new Stopwatch();
            InputEEUpFromLifterPositionTravellingTimeStopWatch = new Stopwatch();
            InputEEWaitPrecisorComeToInputStationTimeStopWatch = new Stopwatch();
            InputEEDownToPrecisorNestPositionTravellingTimeStopWatch = new Stopwatch();
            InputEEPrecisorVacuumProcessTimeStopWatch = new Stopwatch();
            InputEEUpFromPrecisorNestPositionTravellingTimeStopWatch = new Stopwatch();
            
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
//            init = true;
     

            if (qEvent.IsSignal(SigStateJob))
            {
                _currentInputCarrier = null;
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_VCH).Set(DigitalIOState.Off);
                }
                HSTWorkcell.inputEEPickDone = false;
                if (HSTWorkcell.disableBoundaryCheck)
                    TransitionTo(stateRunInitCompleteMoveToParkPosition);
                else
                    TransitionTo(stateRunInitMoveToHome);
                _retryCount = 0;
                return null;
            }
            return base.StateRunInit(qEvent);
        }
        
        private QState StateRunInitMoveToHome(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if(!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.HomeZAxis();     
               
                    TransitionTo(statePublishInputEEHomed);
                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Failed to home Input EE. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                    if (_retryCount < 1)
                    {
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishInputEEHomed(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigInputEEHomed));
                    TransitionTo(stateWaitForSigStartBoundaryCheckInputEEAxis);
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

        private QState StateWaitForSigStartBoundaryCheckInputEEAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigStartBoundaryCheckInputEEAxis))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigStartBoundaryCheckInputEEAxis);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigStartBoundaryCheckInputEEAxis))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:SigStartBoundaryCheckInputEEAxis, Received QSignal:{2} and transition to new State:StateBoundaryCheck", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
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
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.BoundaryCheck();
                    TransitionTo(statePublishInputEEBoundaryCheckComplete);
                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to perform boundary check on Input EE. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishInputEEBoundaryCheckComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigInputEEBoundaryCheckComplete));
                    TransitionTo(stateCheckVaccuum);
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

        private QState StateCheckVaccuum(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.CheckPnpPartPresent(false);
                    TransitionTo(stateRunInitCompleteMoveToParkPosition);
                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to perform Input EE Vacuum check. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
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
                        _controller.DoJobMoveZToPark(false);
                    if (HSTWorkcell.disableBoundaryCheck)
                        PublishSignal(new QEvent(HSTWorkcell.SigInputEEBoundaryCheckComplete));
                    TransitionTo(stateRunStart);
                    _retryCount = 0;
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to home Input EE. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
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
                if (_workcell.IsMachineHomed)
                {
                    _isUnloadEE1 = true;
                    TransitionTo(stateRunCheckHGAReadyForPick);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateRunCheckHGAReadyForPick(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);           
            
            if (qEvent.IsSignal(SigStateJob))
            {
                Controller.StartWorkingOnPlacingToPrecisor = false;
                if (RecallDeferredSignal(HSTWorkcell.SigHGAsReadyToPickAtInputLifter))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigHGAsReadyToPickAtInputLifter);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigInputEEStartDycemCleaning))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputEEStartDycemCleaning);
                }

                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputEEStartDycemCleaning))
            {
                TransitionTo(statePerformCleaningWithDycem);
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigHGAsReadyToPickAtInputLifter))
            {
                InputEEMovesDownToInputStationPickPositionProcessCycleTimeStopWatch.Start();                
                TransitionTo(statePickHgaZDown);
                return null;
            }
            return stateRun;
        }        

        private QState StatePickHgaZDown(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (_controller.IsSafeToMoveDown(true))
                    {
                        InputEEDownToLifterPositionTravellingTimeStopWatch.Start();
                        _controller.DoJobMoveZToPick(false);

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableFlattenerAsInput)
                        {
                            _controller.ExtendInputEEFlattener();
                            Thread.Sleep(HSTMachine.Workcell.HSTSettings.Install.FlattenerDeployDuration);
                        }

                        InputEEDownToLifterPositionTravellingTimeStopWatch.Stop();

                        if (HSTMachine.Workcell != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation() != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                {
                                    UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                    {
                                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelInputDownToLifterPositionTravellingTime.Text = InputEEDownToLifterPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                    });
                                }
                            }
                        }

                        InputEEMovesDownToInputStationPickPositionProcessCycleTimeStopWatch.Stop();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("Input EE Moves Down To Input Station Pick Position Process Cycle Time.csv", InputEEMovesDownToInputStationPickPositionProcessCycleTimeStopWatch.ElapsedTime);
                        }

                        InputEEPicksUpHGAsFromInputStationProcessCycleTimeStopWatch.Start();
                        TransitionTo(statePickHgaVacuumOn);
                    }
                    else
                    {
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to move Input EE down to pick HGAs. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }

        private QState StatePickHgaVacuumOn(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.DoJobVacuum(true);
                    TransitionTo(statePickHgaZUpToParkPosition);
                }
                catch (Exception ex)
                {
                    // Pick fail also Z Up First. 
                    // Check again at Up Height
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to turn on vacuum channel on Input EE during pick HGAs. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }

        private QState StatePickHgaZUpToParkPosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {                    
                    InputEEUpFromLifterPositionTravellingTimeStopWatch.Start();
                    _controller.DoJobMoveZToPark(false);
                    HSTWorkcell.inputEEPickDone = true;
                    InputEEUpFromLifterPositionTravellingTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelInputUpFromLifterPositionTravellingTime.Text = InputEEUpFromLifterPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }
                    TransitionTo(statePickHgaCheckVacuum);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to move Input EE up after picked HGAs. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }

        private QState StatePickHgaCheckVacuum(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    InputEELifterVacuumProcessTimeStopWatch.Start();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableFlattenerAsInput)
                    {
                        _controller.RetractInputEEFlattener();
                        Thread.Sleep(HSTMachine.Workcell.HSTSettings.Install.FlattenerDeployDuration);
                    }
                    InputEELifterVacuumProcessTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelInputLifterVacuumProcessTime.Text = InputEELifterVacuumProcessTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    TransitionTo(statePickHgaPublishPickDone);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to perform vaccuum check on Input EE after picked HGAs. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }
        
        private QState StatePickHgaPublishPickDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);                        

            if (qEvent.IsSignal(SigStateJob))
            {

                InputEEPicksUpHGAsFromInputStationProcessCycleTimeStopWatch.Stop();
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    CommonFunctions.Instance.LogProcessCycleTime("Input EE Picks Up HGAs From Input Station Process Cycle Time.csv", InputEEPicksUpHGAsFromInputStationProcessCycleTimeStopWatch.ElapsedTime);
                }

                _currentInputCarrier = _controller.IncomingCarrier.DeepCopy();
                if (_currentInputCarrier == null)
                    throw new Exception("input carrier object is null");

                _workcell.Process.PrecisorStationProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();
                QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGAsPickDoneAtInputLifter));
                QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGAsInputEECompletePick));

                InputEEPlacesDownHGAOntoPrecisorNestProcessCycleTimeStopWatch.Start();

                TransitionTo(stateRunCheckHGAReadyForPlace);
                return null;
            }
            return stateRun;
        }

        private QState StateRunCheckHGAReadyForPlace(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            InputEEWaitPrecisorComeToInputStationTimeStopWatch.Start();
                        
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigPrecisorReadyForPlace))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigPrecisorReadyForPlace);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigPrecisorReadyForPlace))
            {
                InputEEWaitPrecisorComeToInputStationTimeStopWatch.Stop();

                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelWaitPrecisorEnterInputStation.Text = InputEEWaitPrecisorComeToInputStationTimeStopWatch.ElapsedTime.ToString();
                            });
                        }
                    }
                }
                TransitionTo(statePlaceHgaZDown);
                return null;
            }
            return stateRun;
        }

        private QState StatePlaceHgaZDown(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob)  || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if (_controller.IsSafeToMoveDown(false))
                    {
                        InputEEMovesDownToPrecisorNestProcessCycleTimeStopWatch.Start();

                        InputEEDownToPrecisorNestPositionTravellingTimeStopWatch.Start();
                        HSTWorkcell.inputEEPickDone = false;
                        bool isUp = (_currentInputCarrier.HGATabType == HGAProductTabType.Up);
                        _controller.DoJobMoveZToPlace(isUp, false);
                        InputEEDownToPrecisorNestPositionTravellingTimeStopWatch.Stop();

                        if (HSTMachine.Workcell != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation() != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                {
                                    UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                    {
                                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelInputDownToPrecisorNestPositionTravellingTime.Text = InputEEDownToPrecisorNestPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                    });
                                }
                            }
                        }
                                                                        
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            if (CommonFunctions.Instance.PrecisorNestStabilityAtInputStationProcessCycleTimeStopWatch.Count > 0)
                            {
                                ProcessStopWatch PSW = CommonFunctions.Instance.PrecisorNestStabilityAtInputStationProcessCycleTimeStopWatch.First();
                                PSW.Stop();
                                CommonFunctions.Instance.LogProcessCycleTime("Precisor Nest Stability At Input Station Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                                CommonFunctions.Instance.PrecisorNestStabilityAtInputStationProcessCycleTimeStopWatch.Dequeue();
                            }
                        }

                        InputEEMovesDownToPrecisorNestProcessCycleTimeStopWatch.Stop();
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            CommonFunctions.Instance.LogProcessCycleTime("Input EE Moves Down To Precisor Nest Process Cycle Time.csv", InputEEMovesDownToPrecisorNestProcessCycleTimeStopWatch.ElapsedTime);
                        }

                        TransitionTo(statePlaceHgaVacuumOff);
                    }
                    else
                    {
                        _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to move Input EE down to place HGAs. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePlaceHgaVacuumOff(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    InputEEPrecisorVacuumProcessTimeStopWatch.Start();
                    _controller.DoJobVacuum(false);
                    
                    InputEEPlacesDownHGAOntoPrecisorNestProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input EE Places Down HGAs Onto Precisor Nest Process Cycle Time.csv", InputEEPlacesDownHGAOntoPrecisorNestProcessCycleTimeStopWatch.ElapsedTime);
                    }
                    Thread.Sleep(200);
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableFlattenerAsPrecisor)
                    {
                        _controller.ExtendInputEEFlattener();
                        Thread.Sleep(HSTMachine.Workcell.HSTSettings.Install.FlattenerDeployDuration);
                    }
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigPrecisorSetVacuumOn));
                    Thread.Sleep(200);
                    TransitionTo(stateWaitPrecisorTurnOnAllVacuums);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to turn off Vaccum channel on Input EE. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitPrecisorTurnOnAllVacuums(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigPrecisorVacuumOn))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigPrecisorVacuumOn);
                    return null;
                }

                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigPrecisorVacuumOn))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableFlattenerAsPrecisor)
                {
                    _controller.RetractInputEEFlattener();
                }
                TransitionTo(statePlaceHgaZUpToParkPosition);
                return null;
            }

            return stateRun;
        }

        private QState StatePlaceHgaZUpToParkPosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    InputEELeavesPrecisorNestAfterHGAPlaceProcessCycleTimeStopWatch.Start();

                    InputEEUpFromPrecisorNestPositionTravellingTimeStopWatch.Start();
                    _controller.DoJobMoveZToPark(false);
                    InputEEUpFromPrecisorNestPositionTravellingTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelInputUpFromPrecisorNestPositionTravellingTime.Text = InputEEUpFromPrecisorNestPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }
                   
                    InputEELeavesPrecisorNestAfterHGAPlaceProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Input EE Leaves Precisor Nest After HGA Place Process Cycle Time.csv", InputEELeavesPrecisorNestAfterHGAPlaceProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    ProcessStopWatch PSW1 = new ProcessStopWatch(_controller.IncomingCarrier.CarrierID, new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingInputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    TransitionTo(statePlaceHgaPublishPlaceDone);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 1)
                    {
                        Log.Error(this, "Failed to home Input EE. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }

        private QState StatePlaceHgaPublishPlaceDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);    
            
            if (qEvent.IsSignal(SigStateJob))
            {
                QF.Instance.Publish(new QEvent(HSTWorkcell.SigPrecisorPlaceDone));
                _controller.DoOffPurge();
                TransitionTo(stateRunCheckHGAReadyForPick);
                return null;
            }
            return stateRun;
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
                                QF.Instance.Publish(new QEvent(HSTWorkcell.SigInputEEDycemCleaningComplete));
                                TransitionTo(stateRunCheckHGAReadyForPick);
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
                    for (int i = 0; i < HSTMachine.Workcell.HSTSettings.Install.TotalNumberOfInputEETouchingOnDycem; i++)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StatePerformCleaningWithDycem, Move Input EE to touch on Dycem. Cycle {2}", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString(), i + 1);
                        // move to touch on dycem
                        _controller.DoJobMoveZToDycem(false);
                        Thread.Sleep(HSTMachine.Workcell.HSTSettings.Install.InputEETouchingOnDycemDuration * 1000);
                        // move to EE place height
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StatePerformCleaningWithDycem, Move Input EE up to place height. Cycle {2}", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString(), i + 1);
                        _controller.DoJobMoveZToPlace(true, false);
                        HSTMachine.Workcell.DycemCleaningCounter.InputEEDycemCleaningCount += 1;
                    }
                    _controller.DoJobMoveZToPark(false);

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel().getDycemCleaningCounterUserControl().txtInputEECleanCount.Text = HSTMachine.Workcell.DycemCleaningCounter.InputEEDycemCleaningCountString;
                                    HSTMachine.Workcell.DycemCleaningCounter.Save();
                                });
                            }
                        }
                    }
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigInputEEDycemCleaningComplete));
                    TransitionTo(stateRunCheckHGAReadyForPick);
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

        #endregion
    }
}
