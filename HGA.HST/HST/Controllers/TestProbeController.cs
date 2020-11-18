using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using System.Diagnostics;
//using Seagate.AAS.Parsel.Device.PneumaticControl;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Recipe;
using System.Threading;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;
using System.Windows.Forms;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class TestProbeController : ControllerHST
    {        
        private HSTWorkcell _workcell;
        private HSTIOManifest _ioManifest;        
        private string name = "TestProbeController";
        IAxis _TestProbeZAxis;
        IAxis _XAxis;
        IAxis _YAxis;
        IAxis _Z1Axis;
        IAxis _Z3Axis;
        IAxis _ThetaAxis;
        private AxesProfile _axesProfile;
        private TeachPointRecipe _teachPointRecipe;
        private List<Carrier> _triggeringRunningResult;

        // Local Variables
        private const uint _sensorTimeout = 3000;           // timeouts if a tray is not in position within this time
        private const int _InputEEVacuumOnTiming = 3000;
        private const uint _actuatorHomeTimeout = 10000;
        private const uint _moveTimeout = 50000;

        private const int testProbeEEMaxAcce = 3000;
        private const int testProbeEEMaxDece = 3000;
        private const int testProbeEEMaxVel = 250;
        private const double maintenanceSpeedRatio = 0.1;

#if TestOpt2HardwareTrigger
        private const uint _MeasurementTimeout = 10000;

        private IDigitalOutput _doStartMeasurement;
        private IDigitalOutput _doDownTab;

        private IDigitalInput _diMeasurementIsBusy;
        private IDigitalInput _diMeasurementError;
#endif
        public event EventHandler OnSettingChanged;
        // Motor locations
        public enum Motor_Position
        {
            ZPosTestPos_UpTab,
            ZPosTestPos_DownTab,
            ZPosParkPos
        }

        public Carrier IncomingCarrier { get; set; }

        // Constructors ------------------------------------------------------------
        public TestProbeController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            _workcell = workcell;            
            this._ioManifest = (HSTIOManifest)HSTMachine.Workcell.IOManifest;

            _axesProfile = workcell.SetupConfig.AxesProfile;
            if (HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Acceleration > testProbeEEMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Acceleration = testProbeEEMaxAcce;
            _axesProfile.TestProbeZ.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Deceleration > testProbeEEMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Deceleration = testProbeEEMaxDece;
            _axesProfile.TestProbeZ.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Velocity > testProbeEEMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Velocity = testProbeEEMaxVel;
            _axesProfile.TestProbeZ.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Velocity;

            _teachPointRecipe = workcell.TeachPointRecipe;

            _XAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.X);
            _YAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Y);
            _ThetaAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Theta);
            _Z1Axis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z1);
            _TestProbeZAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z2);
            _Z3Axis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z3);

#if TestOpt2HardwareTrigger
            _doStartMeasurement = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Electronics_Input_1);//Measurement start
            _doDownTab = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Electronics_Input_2);//On = down tab, off = up tab

            _diMeasurementIsBusy = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Electronics_Output_1);//Measurement is in busy state (means measurement not complete)
            _diMeasurementError = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Electronics_Output_2);//Measurement status
#endif
            Machine.HSTMachine.Workcell.SetupSettings.OnSettingsChanged += AutomationConfigChanged;
        }

        // Properties ----------------------------------------------------------
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsDoubleTestActive { get; set; }

        // Methods -------------------------------------------------------------
        public override void InitializeController()
        {            
            SetAction(1);
            try
            {
            }
            catch (Exception ex)
            {
                throw CreateControllerException(1, ex);
            }
        }

        public void BoundaryCheck()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            try
            {
                double dMoveDownPos = (_teachPointRecipe.TestProbeTestHeight_UpTab > _teachPointRecipe.TestProbeTestHeight_DownTab) ? _teachPointRecipe.TestProbeTestHeight_UpTab : _teachPointRecipe.TestProbeTestHeight_DownTab;//lowest position in operation range
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Test Probe Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _TestProbeZAxis.GetActualPosition(), dMoveDownPos, _axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Deceleration, _axesProfile.TestProbeZ.Velocity);
                }
                _TestProbeZAxis.MoveAbsolute(_axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Velocity, dMoveDownPos, _moveTimeout);

                while (_TestProbeZAxis.GetActualPosition() < dMoveDownPos - 10)
                {
                    MessageBox.Show(string.Format("{0}: Actual Position not tally with target position after move done", this));
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Test Probe MoveZAxis. _TestProbeZAxis.GetActualPosition():{0} targetposition:{1}", _TestProbeZAxis.GetActualPosition(), dMoveDownPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Test Probe Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _TestProbeZAxis.GetActualPosition(), dMoveDownPos, _axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Deceleration, _axesProfile.TestProbeZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.TestProbeHandlerZAxisMoveDownError, ex);
            }

            try
            {
                double dMoveUpPos = _teachPointRecipe.TestProbeSafeHeight;//highest position in operation range
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Test Probe Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _TestProbeZAxis.GetActualPosition(), dMoveUpPos, _axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Deceleration, _axesProfile.TestProbeZ.Velocity);
                }
                _TestProbeZAxis.MoveAbsolute(_axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Velocity, dMoveUpPos, _moveTimeout);

                while (_TestProbeZAxis.GetActualPosition() > dMoveUpPos + 10)
                {
                    MessageBox.Show(string.Format("{0}: Actual Position not tally with target position after move done", this));
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Test Probe MoveZAxis. _TestProbeZAxis.GetActualPosition():{0} targetposition:{1}", _TestProbeZAxis.GetActualPosition(), dMoveUpPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Test Probe Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _TestProbeZAxis.GetActualPosition(), dMoveUpPos, _axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Deceleration, _axesProfile.TestProbeZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.TestProbeHandlerZAxisMoveUpError, ex);
            }
        }

        public void HomeZAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);                
                return;
            }

            try
            {
                _workcell._a3200HC.FaultAcknowlegde(_TestProbeZAxis);

                _TestProbeZAxis.Enable(true);
                Thread.Sleep(500);
                _TestProbeZAxis.Home(_actuatorHomeTimeout);
                Thread.Sleep(1000);                
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.TestProbeHandlerZAxisHomeError, ex);
            }
        }

        public void InitialGentry()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _workcell._a3200HC.FaultAcknowlegde(_XAxis);
                _workcell._a3200HC.FaultAcknowlegde(_YAxis);
                _workcell._a3200HC.FaultAcknowlegde(_ThetaAxis);
                _workcell._a3200HC.FaultAcknowlegde(_Z1Axis);
                _workcell._a3200HC.FaultAcknowlegde(_TestProbeZAxis);
                _workcell._a3200HC.FaultAcknowlegde(_Z3Axis);

                _XAxis.Enable(true);
                _YAxis.Enable(true);
                _ThetaAxis.Enable(true);
                _Z1Axis.Enable(true);
                _TestProbeZAxis.Enable(true);
                _Z3Axis.Enable(true);

                Thread.Sleep(500);
                _Z1Axis.Home(_actuatorHomeTimeout);
                _TestProbeZAxis.Home(_actuatorHomeTimeout);
                _Z3Axis.Home(_actuatorHomeTimeout);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.TestProbeHandlerZAxisHomeError, ex);
            }
        }

        public bool IsSafeToMoveDown()
        {
            bool flagValue = false;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true))
            {
                flagValue = true;
            }            
            else if (HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtPrecisorStation)
            {
                flagValue = true;
            }
            return flagValue;
        }

        public void GoToProbePosition(bool isUp, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
               (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Test;
                return;
            }

            if(isUp)
                MoveZAxis(Motor_Position.ZPosTestPos_UpTab, maintenanceSpeed);
            else
                MoveZAxis(Motor_Position.ZPosTestPos_DownTab, maintenanceSpeed);
            HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Test;
        }

        public void GoToParkPosition(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe == true))
            {
                HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Parked;
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            MoveZAxis(Motor_Position.ZPosParkPos, maintenanceSpeed);
            HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Parked;
        }

        public void MoveZAxis(Motor_Position pos, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            double y = 0;
            double dMovePos = 0;
            //double pc = 100.0;
            bool moveDown = false;

            switch (pos)
            {
                case Motor_Position.ZPosParkPos:
                {
                    dMovePos = _teachPointRecipe.TestProbeSafeHeight;
                    moveDown = false;
                }
                break;
                case Motor_Position.ZPosTestPos_UpTab:
                {
                    dMovePos = _teachPointRecipe.TestProbeTestHeight_UpTab;

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                        dMovePos = dMovePos - 5;
                    
                    moveDown = true;
                }
                break;
                case Motor_Position.ZPosTestPos_DownTab:
                {
                    dMovePos = _teachPointRecipe.TestProbeTestHeight_DownTab;

                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                        dMovePos = dMovePos - 5;

                    moveDown = true;
                }
                break;
            }

            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Test Probe Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _TestProbeZAxis.GetActualPosition(), dMovePos, _axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Deceleration, _axesProfile.TestProbeZ.Velocity);
                }

                if (maintenanceSpeed && HSTMachine.Workcell.HSTSettings.Install.EnableMaintenanceSpeedForManualMove)
                    _TestProbeZAxis.MoveAbsolute(testProbeEEMaxAcce * maintenanceSpeedRatio, testProbeEEMaxVel * maintenanceSpeedRatio, dMovePos, _moveTimeout);
                else
                    _TestProbeZAxis.MoveAbsolute(_axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Velocity, dMovePos, _moveTimeout);

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Test Probe Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _TestProbeZAxis.GetActualPosition(), dMovePos, _axesProfile.TestProbeZ.Acceleration, _axesProfile.TestProbeZ.Deceleration, _axesProfile.TestProbeZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw((moveDown) ? HSTErrors.TestProbeHandlerZAxisMoveDownError : HSTErrors.TestProbeHandlerZAxisMoveUpError, ex);
            }

        }

#if TestOpt2HardwareTrigger
        public void TurnOnStartMeasurementOuput()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _doStartMeasurement.Set(DigitalIOState.On);//set back to off after recieved Meas_completed and Meas_status
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.TestElectronicsMeasurementError, ex);
            } 
        }

        public void WaitForMeasurementCompleted()
        {
            try
            {
                _diMeasurementIsBusy.WaitForState(DigitalIOState.Off, _MeasurementTimeout);

                if (_diMeasurementError.Get() == DigitalIOState.On ? true : false)
                {
                    try
                    {
                        throw new Exception("Measurement Error Signal Triggered");
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.TestElectronicsMeasurementError, new Exception("Measurement Error Signal Triggered"));
                    }  
                    
                }

                //reset start measurement output
                _doStartMeasurement.Set(DigitalIOState.Off);//set back to off after recieved Meas_completed and Meas_status
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.TestElectronicsMeasurementError, ex);
            }
        }

        public void SetStartMeasurementTabType(HGAProductTabType HGATabType)
        {
            if (HGATabType == HGAProductTabType.Down)
                _doDownTab.Set(DigitalIOState.On);
            else
                _doDownTab.Set(DigitalIOState.Off);
        }
#endif

        public double GetTestProbePositionZ()
        {

            double position = 0;

            try
            {
                position = _TestProbeZAxis.GetActualPosition();
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.TestProbeHandlerZAxisReadPositionError, ex);
            }

            return position;
        }

        private void AutomationConfigChanged(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Acceleration > testProbeEEMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Acceleration = testProbeEEMaxAcce;
            _axesProfile.TestProbeZ.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Deceleration > testProbeEEMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Deceleration = testProbeEEMaxDece;
            _axesProfile.TestProbeZ.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Velocity > testProbeEEMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Velocity = testProbeEEMaxVel;
            _axesProfile.TestProbeZ.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.TestProbeZ.Velocity;

        }

        public bool IsProbFunctionalTestNeeded()
        {
            bool returnRet = false;
            bool _isForceToDo = false;

            if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.EnabledResistanceCheck)
            {
                if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.IsRetestProcessRequired)
                {
                    returnRet = true;
                }
                else
                {
                    if (CommonFunctions.Instance.IsDoubleTestSpecLoaded)
                    {
                        ///Check by hour
                        if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckType ==
                            ResistanceCheckConfig.ResistanceCheckType.CheckByHour)
                        {
                            if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByHourActive == null ||
                                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByHourActive ==
                                string.Empty)
                            {
                                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByHourActive =
                                    DateTime.Now.ToString();
                                _isForceToDo = true;
                            }

                            var lastActivateTime =
                                DateTime.Parse(
                                    HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByHourActive);
                            var currentTimeStamp = DateTime.Now;
                            var intervalTime = currentTimeStamp - lastActivateTime;
                            int configTime = 0;
                            if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByHourCounter < 1)
                                configTime = 1;
                            else
                                configTime =
                                    Convert.ToInt16(
                                        HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByHourCounter);
                            if (intervalTime.Days > 1 || (intervalTime.Hours > configTime) || _isForceToDo)
                            {
                                returnRet = true;
                            }
                        }
                        ///Check by part counter
                        else if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckType ==
                                 ResistanceCheckConfig.ResistanceCheckType.CheckByPartCounter)
                        {
                            var totalPartRunCounter = HSTMachine.Workcell.LoadCounter.ProcessedHGACount -
                                                      HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig
                                                          .LastCheckByPartCountActive;

                            if (totalPartRunCounter >=
                                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByPartCounter)
                            {
                                returnRet = true;
                            }
                        }
                    }
                }
            }

            return returnRet;
        }

        public void UpdateProbFunctionalCheck()
        {
            Stopwatch stopWatch = new Stopwatch();
            bool endloop = false;
            stopWatch.Reset();
            stopWatch.Start();

            while (_workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000RestResults.Text == string.Empty && !endloop)
            {
                Thread.Sleep(200);

                if (stopWatch.Elapsed.Minutes > 2)
                    endloop = true;
            }

            var getFunctionalResult = _workcell.getPanelData().FunctionalTestPanel.GetFunctionalTestResult();
            if(getFunctionalResult && !endloop)
            {
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByHourActive = DateTime.Now.ToString();
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByPartCountActive = HSTMachine.Workcell.LoadCounter.ProcessedHGACount;
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated = false;
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.IsRetestProcessRequired = false;
            }
            else
            {
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated = true;
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.IsRetestProcessRequired = false;
            }
            HSTMachine.Workcell.HSTSettings.Save();
        }


        #region Double test method
        public bool IsTriggeringNeeded()
        {
            bool _returnResult = false;
            bool _isForceToDo = false;
            if (HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierEnabled)
            {
                /// Triggering by carrier per hour
                if (!HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted)
                {
                    if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate == null ||
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate == string.Empty)
                    {
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate = DateTime.Now.ToString();
                        _isForceToDo = true;
                    }

                    var lastActivateTime = DateTime.Parse(HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate);
                    var currentTimeStamp = DateTime.Now;
                    var intervalTime = currentTimeStamp - lastActivateTime;

                    if (intervalTime.Days >= 1 || (intervalTime.Hours >= HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierHour) || _isForceToDo)
                    {
                        if (intervalTime.Days >= 1)
                        {
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative = 0;
                        }
                        _returnResult = true;
                    }
                    else
                    {
                        if (HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated)
                            if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CounterAfterTriggering < HSTMachine.Workcell.HSTSettings.TriggeringSetting.TrigerByCarrierCount)
                            {
                                _triggeringRunningResult = new List<Carrier>();
                                _returnResult = true;
                            }
                    }
                }
                /// Triggering by buyoff process
                else
                {
                    if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffCarrierCounter <= HSTMachine.Workcell.HSTSettings.TriggeringSetting.TotalCarrierForBuyOff)
                        _returnResult = true;
                }

            }

            return _returnResult;
        }

        public void UpdateTriggeringRunResult(Carrier carrierInfo)
        {
            if (_triggeringRunningResult == null) _triggeringRunningResult = new List<Carrier>();
            Carrier currentRunningInfo = new Carrier();
            currentRunningInfo = carrierInfo.DeepCopy();
            _triggeringRunningResult.Add(currentRunningInfo);
        }

        public void ClearTriggeringRunResult()
        {
            if (_triggeringRunningResult != null &&_triggeringRunningResult.Count > 0)
            _triggeringRunningResult.Clear();
        }

        public void CheckCarrierTriggeringRunResult()
        {
            try
            {
                bool returnResult = false;
                int totalPart = 0;
                int totalPass = 0;
                int totalFail = 0;
                Carrier carrierFirstTest = new Carrier();
                Carrier carrierSecondTest = new Carrier();

                if (HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierEnabled)
                {
                    if (_triggeringRunningResult.Count > 1)
                    {

                        carrierFirstTest = _triggeringRunningResult[0];
                        carrierSecondTest = _triggeringRunningResult[1];
                        for (int slot = 0; slot < IncomingCarrier.RFIDData.RFIDTagData.CarrierSize; slot++)
                        {
                            switch (slot)
                            {
                                case 0:
                                    if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 1:
                                    if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 2:
                                    if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 3:
                                    if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 4:
                                    if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 5:
                                    if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 6:
                                    if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 7:
                                    if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 8:
                                    if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 9:
                                    if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                            }
                        }

                        totalPart = totalPass + totalFail;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CounterAfterTriggering++;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TestFailCounter += totalFail;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TestPassCounter += totalPass;

                    }

                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TotalPartRunCounter += (totalPass + totalFail);

                    if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TotalPartRunCounter != 0)
                    {
                        // Percent = TotalResultFail / TotalTestPart * 100
                        var failurePercentage = (((double)HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TestFailCounter /
                            (double)HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TotalPartRunCounter) * 100);

                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorPerPeriod = failurePercentage;

                        //Check phase-1 spec
                        if (failurePercentage > HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase1Min)
                        {
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                            returnResult = true;
                        }
                        else
                        {
                            if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CounterAfterTriggering >
                               HSTMachine.Workcell.HSTSettings.TriggeringSetting.TrigerByCarrierCount)
                            {
                                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                                HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = false;
                                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate = DateTime.Now.ToString();
                                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
                            }
                        }

                    }

                    //Check phase-2 spec
                    var failureCumulative = (((double)HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TestFailCounter /
                                            (double)HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.TotalPartRunCounter) * 100);
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative = failureCumulative;
                    if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative > HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase2Min)
                    {
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                        returnResult = true;
                    }

                    //Update result to monitor process
                    HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = returnResult;
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CriticalTriggeringActivated =
                        HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated;
                    HSTMachine.Workcell.LoadCounter.Save();
                }
            }
            catch (Exception)
            {
                Log.Trace("Check summary carrier triggering result error!");
            }
        }

        public bool CheckBuyOffAfterTriggeringResult()
        {
            bool returnResult = false;
            try
            {
                int totalPart = 0;
                int totalPass = 0;
                int totalFail = 0;
                Carrier carrierFirstTest = new Carrier();
                Carrier carrierSecondTest = new Carrier();

                if (HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierEnabled)
                {
                    if (_triggeringRunningResult.Count > 1)
                    {

                        carrierFirstTest = _triggeringRunningResult[0];
                        carrierSecondTest = _triggeringRunningResult[1];
                        for (int slot = 0; slot < IncomingCarrier.RFIDData.RFIDTagData.CarrierSize; slot++)
                        {
                            switch (slot)
                            {
                                case 0:
                                    if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga1.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga1.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 1:
                                    if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga2.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga2.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 2:
                                    if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga3.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga3.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 3:
                                    if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga4.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga4.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 4:
                                    if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga5.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga5.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 5:
                                    if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga6.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga6.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 6:
                                    if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga7.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga7.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 7:
                                    if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga8.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga8.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 8:
                                    if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga9.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga9.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                                case 9:
                                    if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedPass))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedFail))
                                        totalPass++;
                                    else if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedFail) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedPass))
                                        totalFail++;
                                    else if ((carrierFirstTest.Hga10.Hga_Status == HGAStatus.TestedPass) && (carrierSecondTest.Hga10.Hga_Status == HGAStatus.TestedFail))
                                        totalFail++;
                                    break;
                            }
                        }

                        totalPart = totalPass + totalFail;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffCarrierCounter++;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffPartLoadedCounter += totalPart;
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffPartFailStatusCounter += totalFail;
                    }

                    if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffCarrierCounter >= HSTMachine.Workcell.HSTSettings.TriggeringSetting.TotalCarrierForBuyOff)
                    {
                        // Percent = TotalResultFail / TotalTestPart * 100
                        var failurePercentage = (((double)HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffPartFailStatusCounter /
                            (double)HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffPartLoadedCounter) * 100);

                        if (failurePercentage < HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase1Min)
                        {
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                            HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = false;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate = DateTime.Now.ToString();
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
                            returnResult = true;
                        }
                        else
                        {
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                            HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = true;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
                            returnResult = false;
                        }

                        if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative >
                            HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase2Min)
                        {
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative = 0;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                            HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = false;
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate = DateTime.Now.ToString();
                            HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
                            returnResult = true;

                        }
                    }

                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CriticalTriggeringActivated =
                        HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated;
                    HSTMachine.Workcell.LoadCounter.Save();
                }
            }
            catch (Exception)
            {
                returnResult = false;
                Log.Trace("Check summary carrier triggering result error!");
            }

            return returnResult;
        }

        public void SaveLogFileReTestProcess(Carrier runningCarrier, bool buyoffresult)
        {
            RetrestProcessDataLogcs.RetestProcessFilelogger filelogger;
            string fileName = string.Empty;
            fileName = RetrestProcessDataLogcs.RetestProcessFilelogger.PreTDFName + "_" + DateTime.Now.Year.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + RetrestProcessDataLogcs.RetestProcessFilelogger.PosTDFName;
            filelogger = new RetrestProcessDataLogcs.RetestProcessFilelogger(Settings.HSTSettings.Instance.Directory.DataPath, fileName);

            try
            {
                int index = 0;
                foreach (var item in _triggeringRunningResult)
                {
                    index++;
                    for (int slot = 0; slot < IncomingCarrier.RFIDData.RFIDTagData.CarrierSize; slot++)
                    {
                        var hgaData = new Hga(1, HGAStatus.Unknown);
                        switch (slot)
                        {
                            case 0:
                                hgaData = item.Hga1;
                                break;
                            case 1:
                                hgaData = item.Hga2;
                                break;
                            case 2:
                                hgaData = item.Hga3;
                                break;
                            case 3:
                                hgaData = item.Hga4;
                                break;
                            case 4:
                                hgaData = item.Hga5;
                                break;
                            case 5:
                                hgaData = item.Hga6;
                                break;
                            case 6:
                                hgaData = item.Hga7;
                                break;
                            case 7:
                                hgaData = item.Hga8;
                                break;
                            case 8:
                                hgaData = item.Hga9;
                                break;
                            case 9:
                                hgaData = item.Hga10;
                                break;
                        }

                        string CSVRecord = String.Join(",",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                        runningCarrier.CarrierID,
                        "Test-" + index,
                        (slot + 1).ToString(),
                        hgaData.Hga_Status.ToString(),
                        hgaData.getReader1Resistance().ToString(),
                        hgaData.getReader2Resistance().ToString(),
                        hgaData.getDeltaISIReader1().ToString(),
                        hgaData.getDeltaISIReader2().ToString(),
                        hgaData.getWriterResistance().ToString(),
                        hgaData.getRHeaterResistance().ToString(),
                        hgaData.getWHeaterResistance().ToString(),
                        hgaData.getTAResistance().ToString(),
                        hgaData.getShortTest().ToString(),
                        hgaData.getShortPadPosition().ToString(),
                        hgaData.getBiasVoltage().ToString(),
                        hgaData.getBiasCurrent().ToString(),
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorPerPeriod.ToString(),
                        HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative.ToString(),
                        hgaData.Error_Msg_Code.ToString(),
                        buyoffresult.ToString()
                        );

                        if (filelogger != null)
                        {
                            filelogger.LogLine(CSVRecord);
                        }

                    }
                }
            }
            catch (Exception)
            {
                Log.Trace("Log data carrier triggering result to file error!");
            }

        }

        #endregion
    }
}
