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
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Data;

namespace Seagate.AAS.HGA.HST.Process
{

    public class TestProbeProcess : ActiveProcessHST
    {
        public int retestcounter = 0;
        private TestProbeController _controller;
        private PrecisorStationController _precisorStationController;
        protected HSTWorkcell _workcell;

        private Stopwatch TestProbeMovesDownToPrecisorNestProbePositionProcessCycleTimeStopWatch;
        private Stopwatch HGAMeasurementTestAtTestProbeProcessCycleTimeStopWatch;
        private Stopwatch TestProbeHandlerMovesUpAfterMeasurementCompleteProcessCycleTimeStopWatch;
        private Stopwatch TestProbeStabilityProcessCycleTimeStopWatch;

        //Time Monitoring
        private Stopwatch TestProbeDownToTestPositionTravellingTimeStopWatch;
        private Stopwatch TestTimeStopWatch;
        private Stopwatch TestProbeUpFromTestPositionTravellingTimeStopWatch;

        //RunInit mode
        private QState stateMoveToHomePosition;
        private QState statePublishSigTestProbeHomed;
        private QState stateWaitForSigStartBoundaryCheckTestProbeAxis;
        private QState stateBoundaryCheck;
        private QState stateRunInitCompleteMoveToParkPosition;
        private QState statePublishSigTestProbeBoundaryCheckComplete;

        //Run mode
        private QState stateCheckProbeFunctionalTest;
        private QState stateWaitForSigReadyForProbe;
        private QState stateMoveTestProbeToProbePosition;
        private QState statePublishSigRequestForHGATesting;
        private QState stateWaitForSigHGATestingDone;        
        private QState stateMoveTestProbeToParkPosition;
        private QState stateCheckDoubleTestPeriod;
        private QState stateStateWaitForSigGetMeasurementDone;
        private QState statePublishSigProbeDone;

        private int _retryCount = 0;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private Carrier _currentInputCarrier = null;
        private DateTime BeforeProbeTimeStamp;
        private DateTime AfterProbeTimeStamp;
        private DateTime _lastRequestTestTime = DateTime.Now;

        // Constructors ------------------------------------------------------------
        public TestProbeProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //RunInit mode
            stateMoveToHomePosition = new QState(this.StateMoveToHomePosition);
            statePublishSigTestProbeHomed = new QState(this.StatePublishSigTestProbeHomed);
            stateWaitForSigStartBoundaryCheckTestProbeAxis = new QState(this.StateWaitForSigStartBoundaryCheckTestProbeAxis);
            stateBoundaryCheck = new QState(this.StateBoundaryCheck);
            stateRunInitCompleteMoveToParkPosition = new QState(this.StateRunInitCompleteMoveToParkPosition);
            statePublishSigTestProbeBoundaryCheckComplete = new QState(this.StatePublishSigTestProbeBoundaryCheckComplete);

            //Run mode
            stateCheckProbeFunctionalTest = new QState(this.StateCheckProbeFunctionTest);
            stateWaitForSigReadyForProbe = new QState(this.StateWaitForSigReadyForProbe);
            stateMoveTestProbeToProbePosition = new QState(this.StateMoveTestProbeToProbePosition);
            statePublishSigRequestForHGATesting = new QState(this.StatePublishSigRequestForHGATesting);
            stateWaitForSigHGATestingDone = new QState(this.StateWaitForSigHGATestingDone);           
            stateMoveTestProbeToParkPosition = new QState(this.StateMoveTestProbeToParkPosition);
            stateCheckDoubleTestPeriod = new QState(this.StateCheckDoubleTestPeriod);
            stateStateWaitForSigGetMeasurementDone = new QState(this.StateWaitForSigGetMeasurementDone);
            statePublishSigProbeDone = new QState(this.StatePublishSigProbeDone);
        }

        // Properties ----------------------------------------------------------

        public TestProbeController Controller
        { get { return _controller; } }
        

        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigStartBoundaryCheckTestProbeAxis);  
            AddAndSubscribeSignal(HSTWorkcell.SigReadyForProbe);
            AddAndSubscribeSignal(HSTWorkcell.SigHGATestingDone);
            AddAndSubscribeSignal(HSTWorkcell.SigTestProbeGetResultDone);      
            base.InitializeStateMachine();
        }

        protected override QState StateActive(IQEvent qEvent)
        {
            if (qEvent.IsSignal(QSignals.Entry))
            {
                ClearDeferredSignals();
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigHGATestingDone) ||
                qEvent.IsSignal(HSTWorkcell.SigReadyForProbe) ||
                qEvent.IsSignal(HSTWorkcell.SigHGATestingDone))
            {
                AddDeferredSignal(qEvent);
                return null;
            }
            return base.StateActive(qEvent);
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new TestProbeController(_workcell, "TestProbe", "Test Probe");
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
            TestProbeMovesDownToPrecisorNestProbePositionProcessCycleTimeStopWatch = new Stopwatch();
            HGAMeasurementTestAtTestProbeProcessCycleTimeStopWatch = new Stopwatch();
            TestProbeHandlerMovesUpAfterMeasurementCompleteProcessCycleTimeStopWatch = new Stopwatch();
            TestProbeStabilityProcessCycleTimeStopWatch = new Stopwatch();            

            //Time Monitoring
            TestProbeDownToTestPositionTravellingTimeStopWatch = new Stopwatch();
            TestTimeStopWatch = new Stopwatch();
            TestProbeUpFromTestPositionTravellingTimeStopWatch = new Stopwatch();

            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);                

            if (qEvent.IsSignal(SigStateJob))
            {
                if (HSTWorkcell.disableBoundaryCheck)
                {
                    TransitionTo(stateRunInitCompleteMoveToParkPosition);
                }
                else
                    TransitionTo(stateMoveToHomePosition);
                return null;
            }
            return base.StateRunInit(qEvent);
        }        

        private QState StateMoveToHomePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Stop:
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
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.HomeZAxis();
                    
                    TransitionTo(statePublishSigTestProbeHomed);
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

        private QState StatePublishSigTestProbeHomed(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigTestProbeHomed));
                    TransitionTo(stateWaitForSigStartBoundaryCheckTestProbeAxis);
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

        private QState StateWaitForSigStartBoundaryCheckTestProbeAxis(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigStartBoundaryCheckTestProbeAxis))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigStartBoundaryCheckTestProbeAxis);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigStartBoundaryCheckTestProbeAxis))
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigStartBoundaryCheckTestProbeAxis, Received QSignal:{2} and transition to new State:StateBoundaryCheck", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }
                TransitionTo(stateBoundaryCheck);
                return null;
            }
            return stateRunInit;
        }

        private QState StateBoundaryCheck(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Stop:
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
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.BoundaryCheck();
                    TransitionTo(statePublishSigTestProbeBoundaryCheckComplete);
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

        private QState StatePublishSigTestProbeBoundaryCheckComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigTestProbeBoundaryCheckComplete));
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

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Stop:
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
                    if (!_workcell.IsIgnoreHomeAxisForByPass)
                        _controller.GoToParkPosition(false);

                    if (HSTWorkcell.disableBoundaryCheck)
                        PublishSignal(new QEvent(HSTWorkcell.SigTestProbeBoundaryCheckComplete));
                    TransitionTo(stateCheckProbeFunctionalTest);
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
        private QState StateCheckProbeFunctionTest(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            if (qEvent.IsSignal(SigStateJob))
            {
                TransitionTo(stateWaitForSigReadyForProbe);
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForSigReadyForProbe(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);  
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigReadyForProbe))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigReadyForProbe);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigReadyForProbe))
            {

                var isProbeFunctionalTestNeed = Controller.IsProbFunctionalTestNeeded();
                if (isProbeFunctionalTestNeed)
                {
                    UIUtility.Invoke(_workcell.getPanelData().FunctionalTestPanel, () =>
                    {
                        _workcell.getPanelData().FunctionalTestPanel.btnFunctionalTestsClearDisplay_Click(this, null);
                        _workcell.getPanelData().FunctionalTestPanel.btnFunctionalTestsStartSelfTests_Click(this, null);
                    });

                    Controller.UpdateProbFunctionalCheck();
                }

                _currentInputCarrier = CommonFunctions.Instance.TestedCarrierQueue.First();
                Controller.IsDoubleTestActive = false;
                Controller.ClearTriggeringRunResult();
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigReadyForProbe, Received QSignal:{2} and transition to new State:StateMoveTestProbeToProbePosition", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                }                

                TransitionTo(stateMoveTestProbeToProbePosition);
                
                return null;
            }
            return stateRun;
        }

        private QState StateMoveTestProbeToProbePosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Stop:
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

            if (qEvent.IsSignal(SigStateJob)/* || qEvent.IsSignal(_sigTimeout)*/)
            {
                try
                {
                    if (_controller.IsSafeToMoveDown())
                    {
                        if ((HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true))
                        {
                            // Do nothing
                        }
                        else
                        {
                            bool foundMatchingProductType = false;
                            if (String.Compare(CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType, CommonFunctions.Instance.MeasurementTestRecipe.RecipeProbeType, true) == 0)
                            {
                                foundMatchingProductType = true;
                                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                                {
                                    Log.Info(this, "In TestProbeProcess' StateMoveTestProbeToProbePosition , found a match in the HGA Product Type database in which the Product ID = {0} and Product Name = {1}.", 
                                        CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType, CommonFunctions.Instance.MeasurementTestRecipe.RecipeProbeType);
                                }
                            }

                            if (foundMatchingProductType == false)
                            {
                                try
                                {
                                    try
                                    {
                                        throw new Exception(String.Format("Unrecognized Product Name of '{0}'.", _currentInputCarrier.WorkOrderData.ProductName));
                                    }
                                    catch (Exception ex)
                                    {
                                        HSTException.Throw(HSTErrors.TestElectronicsOutdatedMeasurementTestConfiguration, new Exception(String.Format("Unrecognized Product Name of '{0}'.", _currentInputCarrier.WorkOrderData.ProductName)));
                                    }                                    
                                }
                                catch (Exception ex)
                                {                                    
                                    ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
                                    TransitionToErrorState(btnlst, ex);
                                }
                            }
                        }                            

                        TestProbeMovesDownToPrecisorNestProbePositionProcessCycleTimeStopWatch.Start();

                        TestProbeDownToTestPositionTravellingTimeStopWatch.Start();
                        bool isUp = (_currentInputCarrier.HGATabType == HGAProductTabType.Up);
                        _controller.GoToProbePosition(isUp, false);

                        TestProbeDownToTestPositionTravellingTimeStopWatch.Stop();

                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                        {
                            _currentInputCarrier.setPrecisorNestPositionZ(_controller.GetTestProbePositionZ());
                        }

                        if (HSTMachine.Workcell != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation() != null)
                            {
                                if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                                {
                                    UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                    {
                                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelDownToTestPositionTravellingTime.Text = TestProbeDownToTestPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                                    });
                                }
                            }
                        }
                        
                        TestProbeStabilityProcessCycleTimeStopWatch.Start();               
                        HGAMeasurementTestAtTestProbeProcessCycleTimeStopWatch.Start();

                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            if (CommonFunctions.Instance.TestProbeHandlerMovesDownToPrecisorNestProcessCycleTimeStopWatch.Count > 0)
                            {
                                ProcessStopWatch PSW = CommonFunctions.Instance.TestProbeHandlerMovesDownToPrecisorNestProcessCycleTimeStopWatch.First();
                                PSW.Stop();
                                CommonFunctions.Instance.LogProcessCycleTime("Test Probe Handler Moves Down To Precisor Nest Process Cycle Time.csv.csv", PSW.GetElapsedTime(), PSW.CarrierID, PSW.GetStartTime(), PSW.GetStopTime());
                                CommonFunctions.Instance.TestProbeHandlerMovesDownToPrecisorNestProcessCycleTimeStopWatch.Dequeue();
                            }
                        }
                            
                        TransitionTo(statePublishSigRequestForHGATesting);
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

        private QState StatePublishSigRequestForHGATesting(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);  
            
            if (qEvent.IsSignal(SigStateJob))
            {
                if (CheckDeferredSignal(HSTWorkcell.SigHGATestingDone))
                    RemoveDeferredSignal(HSTWorkcell.SigHGATestingDone);

                TestTimeStopWatch.Start();
                HSTMachine.Workcell.TestTimePerCarrier.Reset();
                HSTMachine.Workcell.TestTimePerCarrier.Start();

                TestProbeMovesDownToPrecisorNestProbePositionProcessCycleTimeStopWatch.Stop();
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    CommonFunctions.Instance.LogProcessCycleTime("Test Probe Moves Down To Precisor Nest Probe Position Process Cycle Time.csv", TestProbeMovesDownToPrecisorNestProbePositionProcessCycleTimeStopWatch.ElapsedTime);
                }

                //Lai: model the measurement test duration
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                    Thread.Sleep(5500);

                if ((HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                    HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun) ||
                    (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true))
                {
                    // HGA1
                    if (_currentInputCarrier.Hga1.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga1.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga1.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA2
                    if (_currentInputCarrier.Hga2.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga2.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga2.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA3
                    if (_currentInputCarrier.Hga3.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga3.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga3.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA4
                    if (_currentInputCarrier.Hga4.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga4.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga4.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA5
                    if (_currentInputCarrier.Hga5.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga5.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga5.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA6
                    if (_currentInputCarrier.Hga6.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga6.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga6.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA7
                    if (_currentInputCarrier.Hga7.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga7.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga7.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA8
                    if (_currentInputCarrier.Hga8.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga8.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga8.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA9
                    if (_currentInputCarrier.Hga9.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga9.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga9.Hga_Status = HGAStatus.TestedPass;
                    }
                    // HGA10
                    if (_currentInputCarrier.Hga10.Hga_Status == HGAStatus.HGAPresent || _currentInputCarrier.Hga10.Hga_Status == HGAStatus.Untested)
                    {
                        _currentInputCarrier.Hga10.Hga_Status = HGAStatus.TestedPass;
                    }

                    _currentInputCarrier.IsMeasurementTestDone = true;
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGATestingDone));  
                }
                else
                {                                                  
#if TestOpt2HardwareTrigger                    
                    _controller.SetStartMeasurementTabType(_currentInputCarrier.HGATabType);                  
                    _controller.TurnOnStartMeasurementOuput();   
 
                    //sleep for led busy signal to  turn ON before we wait for it to turn off.
                    Thread.Sleep(1000);
                    _controller.WaitForMeasurementCompleted();                  
#endif
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        BeforeProbeTimeStamp = DateTime.Now;
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "StatePublishSigRequestForHGATesting in TestProbeProcess, BeforeProbeTimeStamp : {0}", BeforeProbeTimeStamp);
                        }
                    }

                    UIUtility.Invoke(_workcell.getPanelOperation().getOperationMainPanel(), () =>
                    {
                        _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl()._manualTest = false;
                        _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest_Click(this, null);   
                   
                    });     
                   
                }
                _workcell.IsAllMeasurementFailed = false;

                TestProbeStabilityProcessCycleTimeStopWatch.Stop();
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    CommonFunctions.Instance.LogProcessCycleTime("Test Probe Stability Process Cycle Time.csv", TestProbeStabilityProcessCycleTimeStopWatch.ElapsedTime);
                }

                _lastRequestTestTime = DateTime.Now;
                TransitionTo(stateWaitForSigHGATestingDone);
            }
            return stateRun;
        }

        private QState StateWaitForSigHGATestingDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Yes: // Retry testing
                            {
                                TransitionTo(statePublishSigRequestForHGATesting);
                            }
                            break;
                        case ErrorButton.No: // Skip testing
                            {
                                Log.Info(this, "{0}, ProcessName:{1}, StateWaitForSigHGATestingDone, Carrier rejected by user", LoggerCategory.StateTransition, _processName);
                                if (_currentInputCarrier.Hga1.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga1.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga2.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga2.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga3.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga3.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga4.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga4.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga5.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga5.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga6.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga6.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga7.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga7.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga8.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga8.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga9.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga9.Hga_Status = HGAStatus.Untested;
                                if (_currentInputCarrier.Hga10.Hga_Status == HGAStatus.HGAPresent)
                                    _currentInputCarrier.Hga10.Hga_Status = HGAStatus.Untested;
                                TransitionTo(stateMoveTestProbeToParkPosition);
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
                _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 1000), new QEvent(_sigTimeout));

                if (RecallDeferredSignal(HSTWorkcell.SigHGATestingDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigHGATestingDone);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigHGATestingDone))
            {
                HSTMachine.Workcell.TestTimePerCarrier.Stop();
                HSTMachine.Workcell.TestTimePerHead = HSTMachine.Workcell.TestTimePerCarrier.ElapsedTime_sec / 10;
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    AfterProbeTimeStamp = DateTime.Now;
                    TimeSpan ProbeTimeSpan = AfterProbeTimeStamp.Subtract(BeforeProbeTimeStamp);
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Received SigHGATestingDone in TestProbeProcess, AfterProbeTimeStamp : {0}, ProbeTimeSpan : {1}", AfterProbeTimeStamp, (ProbeTimeSpan.Seconds + ProbeTimeSpan.Milliseconds / 1000.0));
                    }
                }

                TransitionTo(stateMoveTestProbeToParkPosition);
                return null;
            }

            if (qEvent.IsSignal(_sigTimeout))
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan diffTime = currentTime.Subtract(_lastRequestTestTime);
                int totalTimeout = 0;
                if (!Controller.IsDoubleTestActive)
                    totalTimeout = HSTMachine.Workcell.HSTSettings.Install.MeasurementTestTimeOutLimit;
                else
                    totalTimeout = HSTMachine.Workcell.HSTSettings.Install.MeasurementTestTimeOutLimit * 2;

                if (HSTMachine.Workcell.HSTSettings.Install.MeasurementTestTimeOutLimit > 0)
                {
                    if ((int)diffTime.TotalSeconds >= totalTimeout)
                    {
                        try
                        {
                            HSTException.Throw(HSTErrors.TestElectronicsMeasurementTimeout, new Exception(String.Format("Test duration exceed {0}s timeout Limit. \nPress 'Yes' to retry HGA testing, and 'No'to skip the testing.", HSTMachine.Workcell.HSTSettings.Install.MeasurementTestTimeOutLimit)));
                        }
                        catch (Exception ex)
                        {
                            ButtonList btnlst = new ButtonList(ErrorButton.Yes, ErrorButton.No, ErrorButton.NoButton);
                            TransitionToErrorState(btnlst, ex);
                            return null;
                        }

                    }
                }

                _qTimer.FireIn(new TimeSpan(0, 0, 0, 0, 1000), new QEvent(_sigTimeout));
                return null;
            }

            return stateRun;
        }

        private QState StateMoveTestProbeToParkPosition(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Stop:
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
                TestProbeUpFromTestPositionTravellingTimeStopWatch.Start();

                //Delay probe before moved up
                Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.ProbeMoveUpDelay);

                _controller.GoToParkPosition(false);
                TestProbeUpFromTestPositionTravellingTimeStopWatch.Stop();
                
                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelUpFromTestPositionTravellingTime.Text = TestProbeUpFromTestPositionTravellingTimeStopWatch.ElapsedTime.ToString();
                            });
                        }
                    }
                }

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                    HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                    (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true))
                {
                    simulation = true;
                }
                else
                {
                    TestTimeStopWatch.Stop();

                    if (HSTMachine.Workcell != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation() != null)
                        {
                            if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                            {
                                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelTestTime.Text = TestTimeStopWatch.ElapsedTime.ToString();
                                });
                            }
                        }
                    }

                    _retryCount = 0;
                }

                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:StateWaitForSigHGATestingDone, Received QSignal:{2} and transition to new State:StatePublishSigProbeDone", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString());
                    }

                    HGAMeasurementTestAtTestProbeProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("HGA Measurement Test At Test Probe Process Cycle Time.csv", HGAMeasurementTestAtTestProbeProcessCycleTimeStopWatch.ElapsedTime);
                    }

                    TestProbeHandlerMovesUpAfterMeasurementCompleteProcessCycleTimeStopWatch.Start();

                    ProcessStopWatch PSW = new ProcessStopWatch(_currentInputCarrier.CarrierID, new Stopwatch());
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.CollisionAvoidanceBetweenTestProbeHandlerAndPrecisorNestProcessCycleTimeStopWatch.Enqueue(PSW);
                    }


                    TransitionTo(stateCheckDoubleTestPeriod);
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

        private QState StateCheckDoubleTestPeriod(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    var isRetestProcess = Controller.IsTriggeringNeeded();
                    if ((isRetestProcess) && (retestcounter > 0))
                    {
                        var hgastatus = _currentInputCarrier.Hga1.Hga_Status;
                        var reading = _currentInputCarrier.Hga1.getReader1Resistance();

                        Controller.UpdateTriggeringRunResult(_currentInputCarrier);
                        if (CheckDeferredSignal(HSTWorkcell.SigTestProbeGetResultDone))
                        {
                            Log.Info(this, "Perform double test process, SigHGATestProbeGetResultDone Exists", DateTime.Now);

                            if (Controller.IsDoubleTestActive)
                            {
                                RemoveDeferredSignal(HSTWorkcell.SigTestProbeGetResultDone);
                                Log.Info(this, "Perform double test process, SigHGATestProbeGetResultDone and Double Test Exists,Remove Signal", DateTime.Now);
                            }
                        }
                        if (!Controller.IsDoubleTestActive)
                        {
                            Log.Info(this, "Perform double test process",DateTime.Now);
                           Thread.Sleep(HSTSettings.Instance.TriggeringSetting.DoubleTestDelayMillisec);
                            TransitionTo(StateWaitForSigGetMeasurementDone);
                            return null;
                        }
                        else
                        {
                            bool buyOffResult = false;
                            //Check result after tringgering period
                            if (!HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated)
                                Controller.CheckCarrierTriggeringRunResult();
                            else
                                //Check result after got result triggering period failed and need to buyoff process
                                if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted)
                                    buyOffResult = Controller.CheckBuyOffAfterTriggeringResult();

                            Controller.SaveLogFileReTestProcess(_currentInputCarrier, buyOffResult);
                            TransitionTo(statePublishSigProbeDone);
                        }
                    }
                    else
                    {
                        RemoveDeferredSignal(HSTWorkcell.SigTestProbeGetResultDone);
                        TransitionTo(statePublishSigProbeDone);
                    }
                    if (retestcounter == 0)
                    {

                        retestcounter++;
                    }
                }
                catch (Exception ex)
                {
                    TransitionTo(statePublishSigProbeDone);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForSigGetMeasurementDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigTestProbeGetResultDone))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigTestProbeGetResultDone);
                    return null;
                }
                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigTestProbeGetResultDone))
            {
                Controller.IsDoubleTestActive = true;
                TransitionTo(stateMoveTestProbeToProbePosition);
                return null;
            }
            return stateRun;
        }

        private QState StatePublishSigProbeDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigRecover))
            {
                if (errorMessage != null)
                {
                    ErrorButton response = (ErrorButton)(((QEvent)qEvent).EventObject);
                    switch (response)
                    {
                        case ErrorButton.Continue:
                            {
                                PublishSignal(new QEvent(HSTWorkcell.SigProbeDone));
                                TransitionTo(stateCheckProbeFunctionalTest);
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
                    TestProbeHandlerMovesUpAfterMeasurementCompleteProcessCycleTimeStopWatch.Stop();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        CommonFunctions.Instance.LogProcessCycleTime("Test Probe Handler Moves Up After Measurement Complete Process Cycle Time.csv", TestProbeHandlerMovesUpAfterMeasurementCompleteProcessCycleTimeStopWatch.ElapsedTime);
                    }
                    
                    if(_workcell.IsAllMeasurementFailed)
                    {
                        throw new Exception("All measurement results failed, Please verify the communications with test electronics work properly");
                    }
                    else
                    {
                        PublishSignal(new QEvent(HSTWorkcell.SigProbeDone));
                        TransitionTo(stateCheckProbeFunctionalTest);
                    }

                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Continue, ErrorButton.NoButton, ErrorButton.Stop);
                    TransitionToErrorState(btnlst, ex);
                }


            }
            return stateRun;
        }
        
        #endregion
    }
}
