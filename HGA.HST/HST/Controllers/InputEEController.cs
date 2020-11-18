using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.HGA.HST.Recipe;
using System.Threading;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.UI;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class InputEEController : ControllerHST
    {
        IAxis _InputEndEffectorZAxis;

        private IDigitalInput _diInputEndEffectorPnpPresent1;
        private IDigitalInput _diInputEndEffectorPnpPresent2;

        private IDigitalOutput _doInputEndEffectorPnpVacuumOn;
        private IDigitalOutput _doInputEndEffectorPnpPurgeOn;

        private IDigitalOutput _doInputEndEffectorFlattener;

        private VisionHardware HSTVision;
        private Camera _inputcamera;
        private VisionApp _inputcameraVisionApp;

        private AxesProfile _axesProfile;
        private TeachPointRecipe _teachPointRecipe;
        public event EventHandler OnSettingChanged;

        private const int inputEEMaxAcce = 3000;
        private const int inputEEMaxDece = 3000;
        private const int inputEEMaxVel = 250;
        private const double maintenanceSpeedRatio = 0.1;

        // Motor locations
        public enum Motor_Position
        {
            ZPosPickPos,
            ZPosPlacePos_UpTab,
            ZPosPlacePos_DownTab,
            ZPosParkPos,
            ZPosDycem
        }

        // Local Variables
        private const uint _sensorTimeout = 3000;           
        private const int _InputEEVacuumOnTiming = 3000;
        private const uint _actuatorHomeTimeout = 10000;
        private const uint _moveTimeout = 50000;

        // Constructors ------------------------------------------------------------
        public InputEEController(HSTWorkcell workcell, string controllerID, string controllerName)
            : base(workcell, controllerID, controllerName)
        {            
            this._ioManifest = (HSTIOManifest)workcell.IOManifest;

            _axesProfile = workcell.SetupConfig.AxesProfile;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Acceleration > inputEEMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Acceleration = inputEEMaxAcce;
            _axesProfile.InputEEZ.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Deceleration > inputEEMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Deceleration = inputEEMaxDece;
            _axesProfile.InputEEZ.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Velocity > inputEEMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Velocity = inputEEMaxVel;
            _axesProfile.InputEEZ.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Velocity;

            _teachPointRecipe = workcell.TeachPointRecipe;

            _diInputEndEffectorPnpPresent1 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_EE_VS1);
            _diInputEndEffectorPnpPresent2 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_EE_VS2);

            _doInputEndEffectorPnpVacuumOn = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_VCH);
            _doInputEndEffectorPnpPurgeOn = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_PCH);

            _doInputEndEffectorFlattener = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_Flattener);

            _InputEndEffectorZAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z1);

            Machine.HSTMachine.Workcell.CalibrationSettings.OnSettingsChanged += VisionSettingsChanged;
            Machine.HSTMachine.Workcell.SetupSettings.OnSettingsChanged += AutomationConfigChanged;
        }

        public Carrier IncomingCarrier { get; set; }
        public bool StartWorkingOnPlacingToPrecisor { get; set; }
        public override void InitializeController()
        {
            if (!Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
                return;
            HSTVision = HSTMachine.HwSystem.GetHwComponent((int)HSTHwSystem.HardwareComponent.VisionSystem) as VisionHardware;

            if (!HSTVision.Simulation)
            {
                _inputcamera = new Camera(HSTVision.GetCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.CameraSerialNumber));
                _inputcameraVisionApp = new VisionApp();

                ConfigureInputCamera(false, false);    
                for (int count = 0; count < 3; count++)
                {
                    
                    _inputcamera.GrabManual(true);
                    Thread.Sleep(500);
                }
                ConfigureInputCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveImagesLessThanTenHGAs,
                                      Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveAllImages);
                
            }
 
        }


        #region Motion

        public void HomeZAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);                
                return;
            }
            try
            {
                if(!_workcell.IsIgnoreHomeAxisForByPass)
                {
                    _workcell._a3200HC.FaultAcknowlegde(_InputEndEffectorZAxis);
                    _InputEndEffectorZAxis.Enable(true);
                    Thread.Sleep(500);
                    _InputEndEffectorZAxis.Home(_actuatorHomeTimeout);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.InputHandlerZAxisHomeError, ex);
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
                double dMoveDownPos = _teachPointRecipe.InputEEPickHeight;//lowest position in operation range

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "InputEE Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _InputEndEffectorZAxis.GetActualPosition(), dMoveDownPos, _axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Deceleration, _axesProfile.InputEEZ.Velocity);
                }

                if(!_workcell.IsIgnoreHomeAxisForByPass)
                {
                    _InputEndEffectorZAxis.MoveAbsolute(_axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Velocity, dMoveDownPos, _moveTimeout);

                    while (_InputEndEffectorZAxis.GetActualPosition() < dMoveDownPos - 10)
                    {
                        MessageBox.Show(string.Format("{0}: Actual Position not tally with target position after move done", this));
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "InputEEMoveZAxis. _InputEndEffectorZAxis.GetActualPosition():{0} targetposition:{1}", _InputEndEffectorZAxis.GetActualPosition(), dMoveDownPos);
                        }
                        Thread.Sleep(100);
                    }
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "InputEE Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _InputEndEffectorZAxis.GetActualPosition(), dMoveDownPos, _axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Deceleration, _axesProfile.InputEEZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.InputHandlerZAxisMoveDownError, ex);
            }

            try
            {
                double dMoveUpPos = _teachPointRecipe.InputEESafeHeight;//highest position in operation range

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "InputEE Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _InputEndEffectorZAxis.GetActualPosition(), dMoveUpPos, _axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Deceleration, _axesProfile.InputEEZ.Velocity);
                }

                if(!_workcell.IsIgnoreHomeAxisForByPass)
                {
                    _InputEndEffectorZAxis.MoveAbsolute(_axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Velocity, dMoveUpPos, _moveTimeout);

                    while (_InputEndEffectorZAxis.GetActualPosition() > dMoveUpPos + 10)
                    {
                        MessageBox.Show(string.Format("{0}: Actual Position not tally with target position after move done", this));
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "InputEE MoveZAxis. _InputEndEffectorZAxis.GetActualPosition():{0} targetposition:{1}", _InputEndEffectorZAxis.GetActualPosition(), dMoveUpPos);
                        }
                        Thread.Sleep(100);
                    }
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "InputEE Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _InputEndEffectorZAxis.GetActualPosition(), dMoveUpPos, _axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Deceleration, _axesProfile.InputEEZ.Velocity);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputHandlerZAxisMoveUpError, ex);
            }
        }

        public bool IsSafeToMoveDown(bool pickHga)
        {
            bool flagValue = false;

            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "HSTMachine.Workcell.PrecisorNestXAxisPosition: {0}", HSTMachine.Workcell.PrecisorNestXAxisPosition.ToString());
            }
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                flagValue = true;
            }            
            else if (pickHga && HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtInputStation)
            {
                flagValue = true;
            }
            else if (!pickHga && (HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtInputStation))
            {
                flagValue = true;
            }
            return flagValue;
        }

        public void DoJobMoveZToPick(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Pick;
                return;
            }

            MoveZAxis(Motor_Position.ZPosPickPos, maintenanceSpeed);
            HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Pick;
        }

        public void DoJobMoveZToPark(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Parked;
                return;
            }

            MoveZAxis(Motor_Position.ZPosParkPos, maintenanceSpeed);
            HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Parked;
        }

        public void DoJobMoveZToPlace(bool isUp, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Place;
                return;
            }

            if (isUp)
                MoveZAxis(Motor_Position.ZPosPlacePos_UpTab, maintenanceSpeed);
            else
                MoveZAxis(Motor_Position.ZPosPlacePos_DownTab, maintenanceSpeed);
            HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Place;
        }

        public void DoJobMoveZToDycem(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.DycemCleaning;
                return;
            }

            MoveZAxis(Motor_Position.ZPosDycem, maintenanceSpeed);
            HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.DycemCleaning;
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
                        dMovePos = _teachPointRecipe.InputEESafeHeight;
                        moveDown = false;

                    }
                    break;
                case Motor_Position.ZPosPickPos:
                    {
                        dMovePos = _teachPointRecipe.InputEEPickHeight;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZPosPlacePos_UpTab:
                    {
                        dMovePos = _teachPointRecipe.InputEEPlaceHeight_UpTab;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZPosPlacePos_DownTab:
                    {
                        dMovePos = _teachPointRecipe.InputEEPlaceHeight_DownTab;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZPosDycem:
                    {
                        dMovePos = _teachPointRecipe.InputEEDycemHeight;
                        moveDown = true;
                    }
                    break;
            }

            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "InputEE Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _InputEndEffectorZAxis.GetActualPosition(), dMovePos, _axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Deceleration, _axesProfile.InputEEZ.Velocity);
                }

                if (maintenanceSpeed && HSTMachine.Workcell.HSTSettings.Install.EnableMaintenanceSpeedForManualMove)
                    _InputEndEffectorZAxis.MoveAbsolute(inputEEMaxAcce * maintenanceSpeedRatio, inputEEMaxVel * maintenanceSpeedRatio, dMovePos, _moveTimeout);
                else
                    _InputEndEffectorZAxis.MoveAbsolute(_axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Velocity, dMovePos, _moveTimeout);

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "InputEE Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _InputEndEffectorZAxis.GetActualPosition(), dMovePos, _axesProfile.InputEEZ.Acceleration, _axesProfile.InputEEZ.Deceleration, _axesProfile.InputEEZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw((moveDown) ? HSTErrors.InputHandlerZAxisMoveDownError : HSTErrors.InputHandlerZAxisMoveUpError, ex);
            }
        }

        public double GetInputEEPositionZ()
        {

            double position = 0;

            try
            {
                position = _InputEndEffectorZAxis.GetActualPosition();
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputHandlerZAxisReadPositionError, ex);
            }

            return position;
        }

        #endregion


        #region IO Functions
        public void CheckPnpPartPresent(bool bOn)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
            {
                return;
            }
            else
            {
                if (bOn)
                {
                    // Input EE Pnp Present 1
                    try
                    {
                        _diInputEndEffectorPnpPresent1.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.InputEndEffectorVacuumPressureSensor1NotOnError, ex);
                    }
                }
                else
                    try
                    {
                        _diInputEndEffectorPnpPresent1.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.InputEndEffectorVacuumPressureSensor1NotOffError, ex);
                    }
            }
            if (bOn)
            {
                try
                {
                    _diInputEndEffectorPnpPresent2.WaitForState(DigitalIOState.On, _sensorTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.InputEndEffectorVacuumPressureSensor2NotOnError, ex);
                }
            }
            else

                try
                {
                    _diInputEndEffectorPnpPresent2.WaitForState(DigitalIOState.Off, _sensorTimeout);

                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.InputEndEffectorVacuumPressureSensor2NotOffError, ex);
                }

            //Reset seaveyor
            CommonFunctions.Instance.powerOnConveyor = false;
            CommonFunctions.Instance.powerOffConveyor = true;
            Thread.Sleep(1000);
            CommonFunctions.Instance.powerOnConveyor = true;
            CommonFunctions.Instance.powerOffConveyor = false;
            Thread.Sleep(1000);

        }

        public void DoJobVacuum(bool bOn)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation /*|| HSTMachine.Workcell.Config.DryRunWithoutPart*/)
                return;

            if (bOn)
            {
                _doInputEndEffectorPnpPurgeOn.Set(DigitalIOState.Off);
                _doInputEndEffectorPnpVacuumOn.Set(DigitalIOState.On);
                Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.VacuumOnDelay);
            }
            else
            {
                _doInputEndEffectorPnpVacuumOn.Set(DigitalIOState.Off);
                _doInputEndEffectorPnpPurgeOn.Set(DigitalIOState.On);
            }
        }

        public void DoOffPurge()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation /*|| HSTMachine.Workcell.Config.DryRunWithoutPart*/)
                return;

            _doInputEndEffectorPnpPurgeOn.Set(DigitalIOState.Off);

        }

        public void DeployInputEEFlattener()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation /*|| HSTMachine.Workcell.Config.DryRunWithoutPart*/)
                return;

            _doInputEndEffectorFlattener.Set(DigitalIOState.On);
            Thread.Sleep(HSTMachine.Workcell.HSTSettings.Install.FlattenerDeployDuration);
            _doInputEndEffectorFlattener.Set(DigitalIOState.Off);
        }

        public void ExtendInputEEFlattener()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation /*|| HSTMachine.Workcell.Config.DryRunWithoutPart*/)
                return;

            _doInputEndEffectorFlattener.Set(DigitalIOState.On);
        }


        public void RetractInputEEFlattener()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation /*|| HSTMachine.Workcell.Config.DryRunWithoutPart*/)
                return;

            _doInputEndEffectorFlattener.Set(DigitalIOState.Off);
        }

        #endregion


        #region Vision

        public Camera InputCamera
        {
            
            get
            {
                return _inputcamera;
            }
        }

        public Carrier VisionInspect()
        {
            
                
            Carrier _carrier = new Carrier();
            if (Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
            {
                if (!HSTVision.Simulation)
                {
                    if (/*Machine.HSTMachine.Workcell.HSTSettings.Install.HGADetectionUsingVision &&*/ Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
                    {
                        if (_inputcamera != null)
                        {
                            if (_inputcamera.GrabManual(true))
                            {
                                if (_inputcameraVisionApp.RunToolBlock(_inputcamera.grabImage, "NoCarrierInfo"))
                                {
                                    _carrier.ImageFileName = _inputcameraVisionApp.ImageFileName(); // 

                                    _carrier.Hga1.Hga_Status = _inputcameraVisionApp.GetResult(0);
                                    _carrier.Hga2.Hga_Status = _inputcameraVisionApp.GetResult(1);
                                    _carrier.Hga3.Hga_Status = _inputcameraVisionApp.GetResult(2);
                                    _carrier.Hga4.Hga_Status = _inputcameraVisionApp.GetResult(3);
                                    _carrier.Hga5.Hga_Status = _inputcameraVisionApp.GetResult(4);
                                    _carrier.Hga6.Hga_Status = _inputcameraVisionApp.GetResult(5);
                                    _carrier.Hga7.Hga_Status = _inputcameraVisionApp.GetResult(6);
                                    _carrier.Hga8.Hga_Status = _inputcameraVisionApp.GetResult(7);
                                    _carrier.Hga9.Hga_Status = _inputcameraVisionApp.GetResult(8);
                                    _carrier.Hga10.Hga_Status = _inputcameraVisionApp.GetResult(9);
                                    _carrier.IsLoadedInWrongDirection = _inputcameraVisionApp.IsBoatReverse();
                                    _carrier.IsDycemBoat = _inputcameraVisionApp.IsDycemBoat();
                                    CommonFunctions.Instance.visionError = "NoError";
                                }
                                else
                                {
                                    CommonFunctions.Instance.visionError = _inputcameraVisionApp.ErrorMessage();
                                }
                            }else
                            {
                                MessageBox.Show("Fail to acquire image");
                            }
                        }
                        else
                        {
                            MessageBox.Show("InputCamera not initialize...");

                        }
                    }
                    else
                    {
                        _carrier.Hga1.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga2.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga3.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga4.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga5.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga6.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga7.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga8.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga9.Hga_Status = HGAStatus.HGAPresent;
                        _carrier.Hga10.Hga_Status = HGAStatus.HGAPresent;
                    }
                }
            }
            else
            {
                _carrier.Hga1.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga2.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga3.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga4.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga5.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga6.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga7.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga8.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga9.Hga_Status = HGAStatus.HGAPresent;
                _carrier.Hga10.Hga_Status = HGAStatus.HGAPresent;
            }
            return _carrier;
        }

        private void VisionSettingsChanged(object sender, EventArgs e)
        {
            if (_inputcameraVisionApp != null)
            {
                ConfigureInputCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveImagesLessThanTenHGAs,
                                     Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveAllImages);
            }
        }

        private void AutomationConfigChanged(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Acceleration > inputEEMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Acceleration = inputEEMaxAcce;
            _axesProfile.InputEEZ.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Deceleration > inputEEMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Deceleration = inputEEMaxDece;
            _axesProfile.InputEEZ.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Velocity > inputEEMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Velocity = inputEEMaxVel;
            _axesProfile.InputEEZ.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.InputEEZ.Velocity;
        }

        private void ConfigureInputCamera( bool SaveImagesIfLessHGAs, bool SaveAllImages)
        {
            if (!HSTVision.Simulation)
            {
                try
                {
                    _inputcameraVisionApp.LoadRecipe(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.Recipe, CameraLocation.InputStation);
                    _inputcameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.ImagesOutputPath,
                                                        SaveImagesIfLessHGAs,
                                                        SaveAllImages);
                }
                catch (Exception ex)
                {
                    Notify.PopUpError("Input Camera Configuration Error.", ex);
                }
            }
        }

        #endregion
    }
}
