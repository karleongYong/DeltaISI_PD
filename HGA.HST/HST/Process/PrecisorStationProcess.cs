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
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Logging;
using XyratexOSC.XMath;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Data;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Process
{

    public class PrecisorStationProcess : ActiveProcessHST
    {
        private bool InputEEAxisHomedSignalReceived = false;
        private bool OutputEEAxisHomedSignalReceived = false;
        private bool InputLifterHomedSignalReceived = false;
        private bool OutputLifterHomedSignalReceived = false;
        private bool InputEEBoundaryCheckCompleteSignalReceived = false;
        private bool TestProbeBoundaryCheckCompleteSignalReceived = false;
        private bool OutputEEBoundaryCheckCompleteSignalReceived = false;
        private bool TriggeringForSecondProcessActivated = false;

        private PrecisorStationController _controller;
        private TestProbeController _testProbeController;
        protected HSTWorkcell _workcell;

        private Stopwatch PrecisorNestMovesFromOutputStationtoInputStationProcessCycleTimeStopWatch;
        private Stopwatch PrecisorNestMovesFromInputStationtoPrecisorStationProcessCycleTimeStopWatch;
        private Stopwatch HGAMeasurementTestAtPrecisorNestProcessCycleTimeStopWatch;

        //Time Monitoring
        private Stopwatch WaitToEnterInputStationTimeStopWatch;
        private Stopwatch PrecisorNestTravelFromParkToInputStationTimeStopWatch;
        private Stopwatch PrecisorNestTravelFromInputStationToTestStationTimeStopWatch;
        private Stopwatch PrecisorNestTravelFromTestStationToOutputStationStopWatch;
        private Stopwatch PrecisorNestTravelFromOutputStationToParkPositionStopWatch;

        //RunInit mode
        private QState stateRunInitInitialization;
        private QState stateHomeXAxisAndBoundaryCheckXAxis;
        private QState stateHomeYAxis;
        private QState stateHomeThetaAxis;
        private QState statePublishSigStartBoundaryCheckInputEEAndTestProbeAxis;
        private QState statePublishSigStartBoundaryCheckOutputEEAxis;
        private QState stateWaitForInputEEAndTestProbeAxisBoundaryCheckCompleteSignals;
        private QState stateParkPrecisorNestBetweenInputStationAndPrecisorStation;
        private QState stateParkPrecisorNestForOutputEEBoundaryCheck;
        private QState stateWaitForOutputEEAxisBoundaryCheckCompleteSignals;
        private QState stateWaitForAllZAxesParkComplete;
        private QState stateBoundaryCheckYAxisAndThetaAxis;        
        private QState stateWaitForTestProbeZAxisHomedSignal;

        //Run mode
        private QState stateMoveToParkPosition;
        private QState stateWaitForZAxisHomedSignals;
        private QState stateWaitForVisionInspectionOnMissingHGAs;
        private QState stateWaitForSignals;
        private QState stateAlertOperatorToRemoveCarrier;        
        private QState stateCheckMoveToInputStationSafely;
        private QState stateMoveToInputStation;        
        private QState statePublishSigPrecisorReadyForPlace;
        private QState stateWaitForSigPrecisorVacuumOn;
        private QState stateSetValvePosition;
        private QState stateTurnOnVacuumChannelsOneByOne;        
        private QState stateWaitForSigPrecisorPlaceDone;
        private QState stateCheckMoveToPrecisorStationSafely;
        private QState stateMoveToPrecisorStation;
        private QState statePerformAlignmentUsingVision;
        private QState statePublishSigHGAReadyForProbe;
        private QState stateWaitForSigProbeDone;
        private QState stateCheckMoveToOutputStationSafely;
        private QState stateMoveToOutputStation;
        private QState stateTurnOffVacuumChannels;        
        private QState stateWaitForSigPrecisorPickDone;
        private QState statePlaceBackLeftOverHGAsToOutputCarrier;
        private int _retryCount = 0;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private Carrier _currentInputCarrier = null;
        private bool init = true;


        // Constructors ------------------------------------------------------------
        public PrecisorStationProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //RunInit mode
            stateRunInitInitialization = new QState(this.StateRunInitInitialization);
            stateHomeXAxisAndBoundaryCheckXAxis = new QState(this.StateHomeXAxisAndBoundaryCheckXAxis);
            stateHomeYAxis = new QState(this.StateHomeYAxis);
            stateHomeThetaAxis = new QState(this.StateHomeThetaAxis);
            statePublishSigStartBoundaryCheckInputEEAndTestProbeAxis = new QState(this.StatePublishSigStartBoundaryCheckInputEEAndTestProbeAxis);
            stateWaitForOutputEEAxisBoundaryCheckCompleteSignals = new QState(this.StateWaitForOutputEEAxisBoundaryCheckCompleteSignals);
            statePublishSigStartBoundaryCheckOutputEEAxis = new QState(this.StatePublishSigStartBoundaryCheckOutputEEAxis);
            stateWaitForTestProbeZAxisHomedSignal = new QState(this.StateWaitForTestProbeZAxisHomedSignal);
            stateParkPrecisorNestForOutputEEBoundaryCheck = new QState(this.StateParkPrecisorNestForOutputEEBoundaryCheck);
            stateBoundaryCheckYAxisAndThetaAxis = new QState(this.StateBoundaryCheckYAxisAndThetaAxis);
            stateWaitForAllZAxesParkComplete = new QState(this.StateWaitForAllZAxesParkComplete);

            //Run mode
            stateMoveToParkPosition = new QState(this.StateMoveToParkPosition);
            stateWaitForZAxisHomedSignals = new QState(this.StateWaitForZAxisHomedSignals);
            stateWaitForVisionInspectionOnMissingHGAs = new QState(this.StateWaitForVisionInspectionOnMissingHGAs);
            stateWaitForSignals = new QState(this.StateWaitForSignals);
            stateAlertOperatorToRemoveCarrier = new QState(this.StateAlertOperatorToRemoveCarrier);
            stateCheckMoveToInputStationSafely = new QState(this.StateCheckMoveToInputStationSafely);
            stateMoveToInputStation = new QState(this.StateMoveToInputStation);            
            statePublishSigPrecisorReadyForPlace = new QState(this.StatePublishSigPrecisorReadyForPlace);
            stateWaitForSigPrecisorVacuumOn = new QState(this.StateWaitForSigPrecisorVacuumOn);
            stateSetValvePosition = new QState(this.StateSetValvePosition);
            stateTurnOnVacuumChannelsOneByOne = new QState(this.StateTurnOnVacuumChannelsOneByOne);            
            stateWaitForSigPrecisorPlaceDone = new QState(this.StateWaitForSigPrecisorPlaceDone);
            stateCheckMoveToPrecisorStationSafely = new QState(this.StateCheckMoveToPrecisorStationSafely);
            stateMoveToPrecisorStation = new QState(this.StateMoveToPrecisorStation);
            statePublishSigHGAReadyForProbe = new QState(this.StatePublishSigHGAReadyForProbe);
            stateWaitForSigProbeDone = new QState(this.StateWaitForSigProbeDone);
            stateCheckMoveToOutputStationSafely = new QState(this.StateCheckMoveToOutputStationSafely);
            stateMoveToOutputStation = new QState(this.StateMoveToOutputStation);
            stateTurnOffVacuumChannels = new QState(this.StateTurnOffVacuumChannels);            
            stateWaitForSigPrecisorPickDone = new QState(this.StateWaitForSigPrecisorPickDone);
            statePlaceBackLeftOverHGAsToOutputCarrier = new QState(this.StatePlaceBackLeftOverHGAsToOutputCarrier);
        }

        // Properties ----------------------------------------------------------

        public PrecisorStationController Controller
        { get { return _controller; } }

        // Methods -------------------------------------------------------------

        protected override void InitializeStateMachine()
        {                        
            AddAndSubscribeSignal(HSTWorkcell.SigInputEEHomed);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEHomed);
            AddAndSubscribeSignal(HSTWorkcell.SigTestProbeHomed);
            AddAndSubscribeSignal(HSTWorkcell.SigInputLifterHomed);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputLifterHomed);
            AddAndSubscribeSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigHGAsPickDoneAtInputLifter);            
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorSetVacuumOn);
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorPlaceDone);
            AddAndSubscribeSignal(HSTWorkcell.SigProbeDone);
            AddAndSubscribeSignal(HSTWorkcell.SigPrecisorPickDone);
            AddAndSubscribeSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs);
            AddAndSubscribeSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs);
            AddAndSubscribeSignal(HSTWorkcell.SigInputGetISIDataComplete);
            AddAndSubscribeSignal(HSTWorkcell.SigOutputPickAllHGAs);
            AddAndSubscribeSignal(HSTWorkcell.SigOverallMeasurementDone);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new PrecisorStationController(_workcell, "PrecisorStation", "Precisor Station");
            
            try
            {
                _currentInputCarrier = null;
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
            PrecisorNestMovesFromOutputStationtoInputStationProcessCycleTimeStopWatch = new Stopwatch();
            PrecisorNestMovesFromInputStationtoPrecisorStationProcessCycleTimeStopWatch = new Stopwatch();
            HGAMeasurementTestAtPrecisorNestProcessCycleTimeStopWatch = new Stopwatch();                                    

            //Time Monitoring
            WaitToEnterInputStationTimeStopWatch = new Stopwatch();
            PrecisorNestTravelFromInputStationToTestStationTimeStopWatch = new Stopwatch();
            PrecisorNestTravelFromParkToInputStationTimeStopWatch = new Stopwatch();
            PrecisorNestTravelFromTestStationToOutputStationStopWatch = new Stopwatch();
            PrecisorNestTravelFromOutputStationToParkPositionStopWatch = new Stopwatch();

            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                _currentInputCarrier = null;
                InputEEAxisHomedSignalReceived = false;
                OutputEEAxisHomedSignalReceived = false;
                InputLifterHomedSignalReceived = false;
                OutputLifterHomedSignalReceived = false;
                InputEEBoundaryCheckCompleteSignalReceived = false;
                TestProbeBoundaryCheckCompleteSignalReceived = false;
                OutputEEBoundaryCheckCompleteSignalReceived = false;

                _controller.TurnOffVaccuumChannels();

                if (HSTWorkcell.disableBoundaryCheck)
                {
                    TransitionTo(stateWaitForAllZAxesParkComplete);
                }
                else
                    TransitionTo(stateWaitForTestProbeZAxisHomedSignal);
                return null;
            }
            return base.StateRunInit(qEvent);
        }

        private QState StateWaitForTestProbeZAxisHomedSignal(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigTestProbeHomed))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigTestProbeHomed);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigTestProbeHomed))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForTestProbeZAxisHomedSignal, Received QSignal:{2} and transition to new State:StateHomeYAxis because TestProbeZAxisHomedSignal", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                
                TransitionTo(stateWaitForZAxisHomedSignals);
                return null;
            }
            return stateRunInit;
        }

        private QState StateWaitForZAxisHomedSignals(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigInputEEHomed))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputEEHomed);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigOutputEEHomed))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputEEHomed);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigInputLifterHomed))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputLifterHomed);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigOutputLifterHomed))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputLifterHomed);
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputEEHomed))
                InputEEAxisHomedSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEHomed))
                OutputEEAxisHomedSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigInputLifterHomed))
                InputLifterHomedSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigOutputLifterHomed))
                OutputLifterHomedSignalReceived = true;

            if (InputEEAxisHomedSignalReceived &&
                OutputEEAxisHomedSignalReceived &&
                InputLifterHomedSignalReceived &&
                OutputLifterHomedSignalReceived)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForZAxisHomedSignals, Received QSignal:{2} and transition to new State:StateHomeYAxis because InputEE, OutputEE, InputLifter and OutputLifter HomedSignalReceived is true", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                InputEEAxisHomedSignalReceived = false;
                OutputEEAxisHomedSignalReceived = false;
                InputLifterHomedSignalReceived = false;
                OutputLifterHomedSignalReceived = false;

                TransitionTo(stateHomeYAxis);
                return null;
            }
            return stateRunInit;
        }

        protected QState StateHomeYAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);  

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.HomeYAxis();
                    TransitionTo(stateHomeThetaAxis);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        protected QState StateHomeThetaAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.HomeThetaAxis();

                    TransitionTo(stateHomeXAxisAndBoundaryCheckXAxis);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateHomeXAxisAndBoundaryCheckXAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);  

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                    {
                        _controller.HomeXAxis();
                        _controller.BoundaryCheckXAxis();
                    }
                    TransitionTo(statePublishSigStartBoundaryCheckInputEEAndTestProbeAxis);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishSigStartBoundaryCheckInputEEAndTestProbeAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigStartBoundaryCheckInputEEAxis));
                    PublishSignal(new QEvent(HSTWorkcell.SigStartBoundaryCheckTestProbeAxis));
                    TransitionTo(StateWaitForInputEEAndTestProbeAxisBoundaryCheckCompleteSignals);
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

        private QState StateWaitForInputEEAndTestProbeAxisBoundaryCheckCompleteSignals(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if(RecallDeferredSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete))
                { 
                    UpdateSignalRecipient(HSTWorkcell.SigInputEEBoundaryCheckComplete);
                }

                if (RecallDeferredSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
                }

                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputEEBoundaryCheckComplete))
                InputEEBoundaryCheckCompleteSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                TestProbeBoundaryCheckCompleteSignalReceived = true;

            if (InputEEBoundaryCheckCompleteSignalReceived && TestProbeBoundaryCheckCompleteSignalReceived)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForInputEEAndTestProbeAxisBoundaryCheckCompleteSignals, Received QSignal:{2} and transition to new State:stateParkPrecisorNestForOutputEEBoundaryCheck because InputEEBoundaryCheckCompleteSignalReceived is true AND TestProbeBoundaryCheckCompleteSignalReceived is true", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                InputEEBoundaryCheckCompleteSignalReceived = false;
                TestProbeBoundaryCheckCompleteSignalReceived = false;
                TransitionTo(stateParkPrecisorNestForOutputEEBoundaryCheck);
                return null;
            }
            return stateRunInit;
        }



        private QState StateParkPrecisorNestForOutputEEBoundaryCheck(IQEvent qEvent)
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

                    if (CheckDeferredSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete);
                    }

                    if(!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.ParkPrecisorNestBetweenInputStationAndPrecisorStation(false);
                    TransitionTo(statePublishSigStartBoundaryCheckOutputEEAxis);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StatePublishSigStartBoundaryCheckOutputEEAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigStartBoundaryCheckOutputEEAxis));
                    TransitionTo(StateWaitForOutputEEAxisBoundaryCheckCompleteSignals);
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

        private QState StateWaitForOutputEEAxisBoundaryCheckCompleteSignals(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputEEBoundaryCheckComplete);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForOutputEEAxisBoundaryCheckCompleteSignals, Received QSignal:{2} and transition to new State:stateBoundaryCheckYAxisAndThetaAxis", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }

                TransitionTo(stateBoundaryCheckYAxisAndThetaAxis);
                return null;
            }
            return stateRunInit;
        }

        private QState StateBoundaryCheckYAxisAndThetaAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);  

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if(!_workcell.IsIgnoreHomeAxisForByPass)
                    {
                        _controller.BoundaryCheckYAxis();
                        _controller.BoundaryCheckThetaAxis();
                    }
                    HSTWorkcell.disableBoundaryCheck = true;
                    TransitionTo(stateRunInitInitialization);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRunInit;
        }

        private QState StateWaitForAllZAxesParkComplete(IQEvent qEvent)
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
                InputEEBoundaryCheckCompleteSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigOutputEEBoundaryCheckComplete))
                OutputEEBoundaryCheckCompleteSignalReceived = true;

            if (qEvent.IsSignal(HSTWorkcell.SigTestProbeBoundaryCheckComplete))
                TestProbeBoundaryCheckCompleteSignalReceived = true;

            if (InputEEBoundaryCheckCompleteSignalReceived && TestProbeBoundaryCheckCompleteSignalReceived && OutputEEBoundaryCheckCompleteSignalReceived)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForAllZAxesParkComplete, Received QSignal:{2} and transition to new State:StateBoundaryCheck because Z1AxisBoundaryCheckCompleteSignalReceived is true AND Z2AxisBoundaryCheckCompleteSignalReceived is true AND Z3AxisBoundaryCheckCompleteSignalReceived", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                InputEEBoundaryCheckCompleteSignalReceived = false;
                TestProbeBoundaryCheckCompleteSignalReceived = false;
                OutputEEBoundaryCheckCompleteSignalReceived = false;

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
                TransitionTo(stateRunInitInitialization);
                return null;
            }
            return stateRunInit;
        }

        private QState StateRunInitInitialization(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.CheckPrecisorVacuum();
                    init = true;
                    TransitionTo(stateMoveToParkPosition);
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
        private QState StateMoveToParkPosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {                
                try
                {
                    if (CheckDeferredSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs);
                    }

                    if (CheckDeferredSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs);
                    }

                    PrecisorNestTravelFromOutputStationToParkPositionStopWatch.Start();
                    if(!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.ParkPrecisorNestBetweenInputStationAndPrecisorStation(false);
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Move to Park Station");
                    }
                    PrecisorNestTravelFromOutputStationToParkPositionStopWatch.Stop();

                    if (HSTWorkcell.outputEEPickDone)
                    {
                        PublishSignal(new QEvent(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtParkedPosition));
                        HSTWorkcell.outputEEPickDone = false;
                    }

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelPrecisorNestTravelFromOutputStationToParkPosition.Text = PrecisorNestTravelFromOutputStationToParkPositionStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    TransitionTo(stateWaitForSignals);
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

        private QState StateWaitForSignals(IQEvent qEvent)
        {
            WaitToEnterInputStationTimeStopWatch.Start();
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigHGAsPickDoneAtInputLifter))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigHGAsPickDoneAtInputLifter);
                }                
                return null;
            }

            if(qEvent.IsSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs))
            {
                TransitionTo(statePlaceBackLeftOverHGAsToOutputCarrier);
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigHGAsPickDoneAtInputLifter))
            {                
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSignals, Received QSignal:{2} and transition to new State:StateCheckMoveToInputStationSafely", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }

                WaitToEnterInputStationTimeStopWatch.Stop();

                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelWaitToEnterInputStationTime.Text = WaitToEnterInputStationTimeStopWatch.ElapsedTime.ToString();
                            });
                        }
                    }
                }

                TransitionTo(stateCheckMoveToInputStationSafely);
                return null;
            }            
            return stateRun;
        }

        private QState StateCheckMoveToInputStationSafely(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                if (_controller.IsSafeToMoveToInputStation())
                {
                    TransitionTo(stateMoveToInputStation);
                    return null;
                }
                else
                {
                    _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                    return null;
                }
            }
            return stateRun;
        }

        private QState StateMoveToInputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PrecisorNestTravelFromParkToInputStationTimeStopWatch.Start();

                    //if precisor is already at input station
                    if (HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtInputStation)
                    {
                        bool IsUp = (_controller.IncomingCarrier.HGATabType == HGAProductTabType.Up);
                        _controller.MoveToInputStation(IsUp, false);

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "Move Precisor Nest From Park to Input Station");
                        }
                    }
                    else
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "Precisor Nest already at Input Station");
                        }
                    }
                    if (HSTWorkcell.outputEEPickDone)
                    {
                        PublishSignal(new QEvent(HSTWorkcell.SigOutputEEAxisSafeToPlacePrecisorNestAtInputStation));
                        HSTWorkcell.outputEEPickDone = false;
                    }
                    PrecisorNestTravelFromParkToInputStationTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelPrecisorNestTravelFromParkToInputStationTime.Text = PrecisorNestTravelFromParkToInputStationTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    PrecisorNestMovesFromOutputStationtoInputStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Precisor Nest Moves From  Output Station To Input Station Process Cycle Time.csv", PrecisorNestMovesFromOutputStationtoInputStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    PrecisorNestMovesFromInputStationtoPrecisorStationProcessCycleTimeStopWatch.Start();
                    
                    ProcessStopWatch PSW1 = new ProcessStopWatch("", new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.PrecisorNestStabilityAtInputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    if (init || (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassVisionAtOutput == true))
                        TransitionTo(statePublishSigPrecisorReadyForPlace);
                    else
                        TransitionTo(StateWaitForVisionInspectionOnMissingHGAs);
                    init = false;
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

        private QState StateWaitForVisionInspectionOnMissingHGAs(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigVisionDetectionFoundMissingHGAs);                    
                }

                if (RecallDeferredSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs);                    
                }
                if (RecallDeferredSignal(HSTWorkcell.SigOutputPickAllHGAs))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputPickAllHGAs);        
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForVisionInspectionOnMissingHGAs, Received QSignal:{2} and transition to new State:StatePlaceBackLeftOverHGAsToOutputCarrier", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                //Lai: Allow precisor nest to come back to input station again.
                if (!CheckDeferredSignal(HSTWorkcell.SigOutputCarrierReadyForPlace))
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigHGAsPickDoneAtInputLifter));
                }
                TransitionTo(statePlaceBackLeftOverHGAsToOutputCarrier);
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForVisionInspectionOnMissingHGAs, Received QSignal:{2} and transition to new State:StatePublishSigPrecisorReadyForPlace", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }

                TransitionTo(statePublishSigPrecisorReadyForPlace);
                return null;
            }

            return stateRun;
        }

        private QState StatePublishSigPrecisorReadyForPlace(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);            

            if (qEvent.IsSignal(SigStateJob))
            {
                PublishSignal(new QEvent(HSTWorkcell.SigPrecisorReadyForPlace));
                CommonFunctions.Instance.PreviousPrecisorNestAtStandbyPositionTimeStamp = DateTime.Now;

               TransitionTo(stateSetValvePosition);
            }
            return stateRun;
        }

        private QState StateSetValvePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _currentInputCarrier = _controller.IncomingCarrier.DeepCopy();

                    if (_currentInputCarrier == null)
                        throw new Exception("input carrier object is null");

                    bool IsUp = (_currentInputCarrier.HGATabType == HGAProductTabType.Up);
                    _controller.SetValvePosition(IsUp);
                    TransitionTo(stateWaitForSigPrecisorVacuumOn);
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

        private QState StateWaitForSigPrecisorVacuumOn(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Retry:
                            {
                                _workcell.HGATailType = HGAProductTailType.ShortTail;
                                TransitionTo(stateTurnOnVacuumChannelsOneByOne);
                                return null;
                            }
                            break;
                    }
                }
            }

            if (qEvent.IsSignal(SigStateJob))
            {
                RecallDeferredSignal(HSTWorkcell.SigPrecisorSetVacuumOn);
                UpdateSignalRecipient(HSTWorkcell.SigPrecisorSetVacuumOn);
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigPrecisorSetVacuumOn))
            {
                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigPrecisorVacuumOn, Received QSignal:{2} and transition to new State:StateSetValvePosition", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                    }

                    _workcell.HGATailType = HGAProductTailType.Unknown;
                    var tailtype = string.Empty;
                    if (CommonFunctions.Instance.MeasurementTestRecipe.RecipeTailType.ToLower().Contains("long"))
                        _workcell.HGATailType = HGAProductTailType.LongTail;
                    else if (CommonFunctions.Instance.MeasurementTestRecipe.RecipeTailType.ToLower().Contains("short"))
                        _workcell.HGATailType = HGAProductTailType.ShortTail;

                    if (_workcell.HGATailType == HGAProductTailType.Unknown)
                        throw new Exception(String.Format("Unknown HGA tail type for input Carrier '{0}'!", _currentInputCarrier.CarrierID));

                    TransitionTo(stateTurnOnVacuumChannelsOneByOne);
                    return null;
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Stop, ErrorButton.NoButton, ErrorButton.Retry);
                    TransitionToErrorState(btnlst, ex);
                }

            }
            return stateRun;
        }

        private QState StateTurnOnVacuumChannelsOneByOne(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.TurnOnVacuumChannelsOneByOne(_workcell.HGATailType);

                    PublishSignal(new QEvent(HSTWorkcell.SigPrecisorVacuumOn));
                    TransitionTo(stateWaitForSigPrecisorPlaceDone);
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
       
        private QState StateWaitForSigPrecisorPlaceDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);      
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigPrecisorPlaceDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigPrecisorPlaceDone);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigPrecisorPlaceDone))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigPrecisorPlaceDone, Received QSignal:{2} and transition to new State:StateCheckMoveToPrecisorStationSafely", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }

                TransitionTo(stateCheckMoveToPrecisorStationSafely);
                
                return null;
            }
            return stateRun;
        }

        private QState StateCheckMoveToPrecisorStationSafely(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                if (_controller.IsSafeToMoveToPrecisorStation())
                {
                    TransitionTo(stateMoveToPrecisorStation);

                    PrecisorNestMovesFromInputStationtoPrecisorStationProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Precisor Nest Moves From Input Station To Precisor Station Process Cycle Time.csv", PrecisorNestMovesFromInputStationtoPrecisorStationProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    return null;
                }
                else
                {
                    _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 200), new QEvent(_sigTimeout));
                    return null;
                }
            }
            return stateRun;
        }

        private QState StateMoveToPrecisorStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (CheckDeferredSignal(HSTWorkcell.SigHGAsPickDoneAtInputLifter))
                {
                    RemoveDeferredSignal(HSTWorkcell.SigHGAsPickDoneAtInputLifter);
                }

                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        if (CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingInputStationProcessCycleTimeStopWatch.Count > 0)
                        {
                            ProcessStopWatch PSW = CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingInputStationProcessCycleTimeStopWatch.First();
                            PSW.Stop();
                            CommonFunctions.Instance.LogProcessCycleTime("Collision Avoidance Between Input EE And Precisor Nest Leaving Input Station Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                            CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingInputStationProcessCycleTimeStopWatch.Dequeue();
                        }
                    }

                    PrecisorNestTravelFromInputStationToTestStationTimeStopWatch.Start();
                    bool IsUp = (_currentInputCarrier.HGATabType == HGAProductTabType.Up);
                    _controller.MoveToPrecisorStation(IsUp, false);
                    PrecisorNestTravelFromInputStationToTestStationTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelPrecisorNestTravelFromInputStationToTestStation.Text = PrecisorNestTravelFromInputStationToTestStationTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    HGAMeasurementTestAtPrecisorNestProcessCycleTimeStopWatch.Start();
                    ProcessStopWatch PSW1 = new ProcessStopWatch(_controller.IncomingCarrier.CarrierID, new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.TestProbeHandlerMovesDownToPrecisorNestProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    TriggeringForSecondProcessActivated = false;

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true)
                    {
                        TransitionTo(stateCheckMoveToOutputStationSafely);
                    }
                    else
                    {
                        TransitionTo(statePublishSigHGAReadyForProbe);
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

        private QState StatePublishSigHGAReadyForProbe(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);               

            if (qEvent.IsSignal(SigStateJob))
            {
                if (!TriggeringForSecondProcessActivated)
                {
                    _currentInputCarrier = _controller.IncomingCarrier.DeepCopy();

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                    {
                        _currentInputCarrier.setPrecisorNestPositionX(_controller.GetPrecisorNestPositionX());
                        _currentInputCarrier.setPrecisorNestPositionY(_controller.GetPrecisorNestPositionY());
                        _currentInputCarrier.setPrecisorNestPositionTheta(_controller.GetPrecisorNestPositionTheta());
                    }

                    _workcell.Process.TestProbeProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();
                    CommonFunctions.Instance.TestedCarrierQueue.Enqueue(_workcell.Process.TestProbeProcess.Controller.IncomingCarrier);
                }

                PublishSignal(new QEvent(HSTWorkcell.SigReadyForProbe));
                TransitionTo(stateWaitForSigProbeDone);
            }
            return stateRun;
        }

        private QState StateWaitForSigProbeDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);    
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigProbeDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigProbeDone);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigProbeDone))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigProbeDone, Received QSignal:{2} and transition to new State:StateCheckMoveToOutputStationSafely", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }

                TransitionTo(stateCheckMoveToOutputStationSafely);
                return null;
            }
            return stateRun;
        }

        private QState StateCheckMoveToOutputStationSafely(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                if (_controller.IsSafeToMoveToOutputStation() && !HSTWorkcell.outputEEHoldingHGAs)
                {
                    RemoveDeferredSignal(HSTWorkcell.SigOverallMeasurementDone);
                    TransitionTo(stateMoveToOutputStation);
                    
                    return null;
                }
                else
                {
                    _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 100), new QEvent(_sigTimeout));
                    return null;
                }
            }
            return stateRun;
        }

        private QState StateMoveToOutputStation(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
           
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {

                    if (CheckDeferredSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigVisionDetectionFoundMissingHGAs);
                    }

                    if (CheckDeferredSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigVisionDetectionFoundNoMissingHGAs);
                    }

                    if (CheckDeferredSignal(HSTWorkcell.SigOverallMeasurementDone))
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigOverallMeasurementDone);
                    }

                    HGAMeasurementTestAtPrecisorNestProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("HGA Measurement Test At Precisor Station Process Cycle Time.csv", HGAMeasurementTestAtPrecisorNestProcessCycleTimeStopWatch.ElapsedTime);

                        if (CommonFunctions.Instance.CollisionAvoidanceBetweenTestProbeHandlerAndPrecisorNestProcessCycleTimeStopWatch.Count > 0)
                        {
                            ProcessStopWatch PSW = CommonFunctions.Instance.CollisionAvoidanceBetweenTestProbeHandlerAndPrecisorNestProcessCycleTimeStopWatch.First();
                            PSW.Stop();
                            CommonFunctions.Instance.LogProcessCycleTime("Collision Avoidance Between Test Probe And Precisor Nest Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                            CommonFunctions.Instance.CollisionAvoidanceBetweenTestProbeHandlerAndPrecisorNestProcessCycleTimeStopWatch.Dequeue();
                        }
                    }
                    
                    ProcessStopWatch PSW1 = new ProcessStopWatch(_currentInputCarrier.CarrierID, new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.PrecisorNestMovesFromPrecisorStationToOutputStationProcessCycleTimeStopWatch.Enqueue(PSW1);
                    }

                    PrecisorNestTravelFromTestStationToOutputStationStopWatch.Start();
                    bool IsUp = (_currentInputCarrier.HGATabType == HGAProductTabType.Up);
                    _controller.MoveToOutputStation(IsUp, false);
                    PrecisorNestTravelFromTestStationToOutputStationStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelPrecisorNestTravelFromTestStationToOutputStation.Text = PrecisorNestTravelFromTestStationToOutputStationStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        if (CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.Count > 0)
                        {
                            ProcessStopWatch PSW = CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.First();
                            PSW.Stop();
                            CommonFunctions.Instance.LogProcessCycleTime("Collision Avoidance Between Input EE And Precisor Nest Leaving Output Station Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                            CommonFunctions.Instance.CollisionAvoidanceBetweenInputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.Dequeue();
                        }

                    }

                    PrecisorNestMovesFromOutputStationtoInputStationProcessCycleTimeStopWatch.Start();

                    PublishSignal(new QEvent(HSTWorkcell.SigPrecisorReadyForPick));
                    TransitionTo(stateTurnOffVacuumChannels);
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

        private QState StateTurnOffVacuumChannels(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _workcell.Process.OutputEEProcess.Controller.IncomingCarrier = _currentInputCarrier.DeepCopy();
                    TransitionTo(stateWaitForSigPrecisorPickDone);
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
        
        private QState StateWaitForSigPrecisorPickDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);    
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigPrecisorPickDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigPrecisorPickDone);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigPrecisorPickDone))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigPrecisorPickDone, Received QSignal:{2} and transition to new State:StateMoveToHomePosition", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                
                if(RecallDeferredSignal(HSTWorkcell.SigAllMeasurementDataDone))
                {
                    RemoveDeferredSignal(HSTWorkcell.SigAllMeasurementDataDone);
                }

                TransitionTo(stateMoveToParkPosition);


                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    if (CommonFunctions.Instance.CollisionAvoidanceBetweenOutputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.Count > 0)
                    {
                        ProcessStopWatch PSW = CommonFunctions.Instance.CollisionAvoidanceBetweenOutputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.First();
                        PSW.Stop();
                        CommonFunctions.Instance.LogProcessCycleTime("Collision Avoidance Between Output EE And Precisor Nest Leaving Output Station Process Cycle Time.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                        CommonFunctions.Instance.CollisionAvoidanceBetweenOutputEEAndPrecisorNestLeavingOutputStationProcessCycleTimeStopWatch.Dequeue();
                    }
                }

                ProcessStopWatch PSW1 = new ProcessStopWatch(_controller.IncomingCarrier.CarrierID, new Stopwatch());
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    CommonFunctions.Instance.CollisionAvoidanceBetweenPrecisorNestAndOutputEEMovingDownToOutputCarrierProcessCycleTimeStopWatch.Enqueue(PSW1);
                }

                return null;
            }
            return stateRun;
        }

        private QState StatePlaceBackLeftOverHGAsToOutputCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);    
            
            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    Controller.TurnOnVacuumChannelsOneByOne(_workcell.HGATailType);
                    TransitionTo(stateCheckMoveToOutputStationSafely);
                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;
        }

        #endregion
    }
}
