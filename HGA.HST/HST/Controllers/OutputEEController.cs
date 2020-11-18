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
using Seagate.AAS.Utils;
using XyratexOSC.UI;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class OutputEEController : ControllerHST
    {        
        private string name = "OutputEEController";

        IAxis _OutputEndEffectorZAxis;

        private IDigitalInput _diOutputEndEffectorPnpPresent1;
        private IDigitalInput _diOutputEndEffectorPnpPresent2;

        private IDigitalOutput _doOutputEndEffectorPnpVacuumOn;
        private IDigitalOutput _doOutputEndEffectorPnpPurgeOn;

        private VisionHardware HSTVision;       
        private Camera _outputcamera;
        private VisionApp _outputcameraVisionApp;


        private AxesProfile _axesProfile;
        private TeachPointRecipe _teachPointRecipe;
        private Stopwatch OutputEEPrecisorVacuumProcessTimeStopWatch;

        private const int outputEEMaxAcce = 3000;
        private const int outputEEMaxDece = 3000;
        private const int outputEEMaxVel = 250;

        private const double maintenanceSpeedRatio = 0.1;

        // Motor locations
        public enum Motor_Position
        {
            ZPosPickPos_UpTab,
            ZPosPickPos_DownTab,
            ZPosPlacePos,
            ZPosParkPos,
            ZPosVisionClearPos,
            ZposDycemPos,
            ZMoveUpPartiallyPos_UpTab,
            ZMoveUpPartiallyPos_DownTab,
        }

        // Local Variables
        private const uint _sensorTimeout = 3000;           // timeouts if a tray is not in position within this time
        private const int _OutputEEVacuumOnTiming = 3000;
        private const uint _actuatorHomeTimeout = 10000;
        private const uint _moveTimeout = 50000;

        public Carrier IncomingCarrier { get; set; }
        public bool All10PartsPicked { get; set; }

        // Constructors ------------------------------------------------------------
        public OutputEEController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            this._workcell = workcell;            
            this._ioManifest = (HSTIOManifest)workcell.IOManifest;

            _axesProfile = workcell.SetupConfig.AxesProfile;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Acceleration > outputEEMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Acceleration = outputEEMaxAcce;
            _axesProfile.OutputEEZ.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Deceleration > outputEEMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Deceleration = outputEEMaxDece;
            _axesProfile.OutputEEZ.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Velocity > outputEEMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Velocity = outputEEMaxVel;
            _axesProfile.OutputEEZ.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Velocity;


            _teachPointRecipe = workcell.TeachPointRecipe;

            _diOutputEndEffectorPnpPresent1 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_EE_VS1);
            _diOutputEndEffectorPnpPresent2 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_EE_VS2);

            _doOutputEndEffectorPnpVacuumOn = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_VCH);
            _doOutputEndEffectorPnpPurgeOn = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_PCH);

            _OutputEndEffectorZAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z3);

            OutputEEPrecisorVacuumProcessTimeStopWatch = new Stopwatch();
            Machine.HSTMachine.Workcell.CalibrationSettings.OnSettingsChanged += VisionSettingsChanged;
            Machine.HSTMachine.Workcell.SetupSettings.OnSettingsChanged += AutomationConfigChanged;
            
        }

        // Properties ----------------------------------------------------------
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        

        // Methods -------------------------------------------------------------
        public override void InitializeController()
        {            
            SetAction(1);
            try
            {
                if (!Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
                    return;
                HSTVision = HSTMachine.HwSystem.GetHwComponent((int)HSTHwSystem.HardwareComponent.VisionSystem) as VisionHardware;
                if (!HSTVision.Simulation)
                {
                    _outputcamera = new Camera(HSTVision.GetCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.CameraSerialNumber));
                    _outputcameraVisionApp = new VisionApp();
                    
                    //---------------- Dont save the images during start up ------------------------
                    ConfigureOutputCamera(false, false);
                    //----------------
                    for (int count = 0; count < 3; count++)
                    {
                        _outputcamera.GrabManual(true);
                        Thread.Sleep(500);
                    }
                    ConfigureOutputCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveImagesLessThanTenHGAs,
                                          Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveAllImages);
                }
               
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
                double dMoveDownPos = _teachPointRecipe.OutputEEPlaceHeight;
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "OutputEE Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _OutputEndEffectorZAxis.GetActualPosition(), dMoveDownPos, _axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Deceleration, _axesProfile.OutputEEZ.Velocity);
                }
                _OutputEndEffectorZAxis.MoveAbsolute(_axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Velocity, dMoveDownPos, _moveTimeout);

                while (_OutputEndEffectorZAxis.GetActualPosition() < dMoveDownPos - 10)
                {
                    MessageBox.Show(string.Format("{0}: Actual Position not tally with target position after move done", this));
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "OutputEE MoveZAxis. _OutputEndEffectorZAxis.GetActualPosition():{0} targetposition:{1}", _OutputEndEffectorZAxis.GetActualPosition(), dMoveDownPos);
                    }
                    Thread.Sleep(100);
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "OutputEE Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _OutputEndEffectorZAxis.GetActualPosition(), dMoveDownPos, _axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Deceleration, _axesProfile.OutputEEZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.OutputHandlerZAxisMoveDownError, ex);
            }

            try
            {
                double dMoveUpPos = _teachPointRecipe.InputEESafeHeight;

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "OutputEE Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _OutputEndEffectorZAxis.GetActualPosition(), dMoveUpPos, _axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Deceleration, _axesProfile.OutputEEZ.Velocity);
                }
                _OutputEndEffectorZAxis.MoveAbsolute(_axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Velocity, dMoveUpPos, _moveTimeout);

                while (_OutputEndEffectorZAxis.GetActualPosition() > dMoveUpPos + 10)
                {
                    MessageBox.Show(string.Format("{0}: Actual Position not tally with target position after move done", this));
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "OutputEE MoveZAxis. _OutputEndEffectorZAxis.GetActualPosition():{0} targetposition:{1}", _OutputEndEffectorZAxis.GetActualPosition(), dMoveUpPos);
                    }
                    Thread.Sleep(100);
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "OutputEE Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _OutputEndEffectorZAxis.GetActualPosition(), dMoveUpPos, _axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Deceleration, _axesProfile.OutputEEZ.Velocity);
                }
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.OutputHandlerZAxisMoveUpError, ex);
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
                _workcell._a3200HC.FaultAcknowlegde(_OutputEndEffectorZAxis);

                _OutputEndEffectorZAxis.Enable(true);
                Thread.Sleep(500);
                _OutputEndEffectorZAxis.Home(_actuatorHomeTimeout);
              
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {                
                HSTException.Throw(HSTErrors.OutputHandlerZAxisHomeError, ex);
            }
        }

        public void SetVacuum(bool IsOn)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {                
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            
            if(IsOn)
            {
                // Turn on vacuum
                DoJobVacuum(true);
            }
            else
            {
                // Turn off vacuum
                DoJobVacuum(false);
            }
        }

        public bool IsSafeToMoveDown(bool isPlace)
        {
            bool flagValue = false;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                flagValue = true;
            }            
            else if (isPlace && /*HSTWorkcell.outputCarrierIsUnlocked &&*/ HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtOutputStation)
            {
                flagValue = true;
            }
            else if (!isPlace && HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtOutputStation)
            {
                flagValue = true;
            }
            return flagValue;
        }

        public void PickHGAMoveDown(bool isUp)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Pick;
                return;
            }

            DoJobMoveZToPick(isUp, false);

            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Pick;
        }

        public void PickHGAMoveUp()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
                return;
            }
            DoJobMoveZToPark(false, true);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
        }

        public void PlaceHGAMoveDown()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Place;
                return;
            }

            DoJobMoveZToPlace(false);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Place;

        }

        public void PlaceHGAMoveUp()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
                return;
            }

            DoJobMoveZToPark(false, false);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
        }

        public void PlaceHGAMoveUpVisionClear()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
                return;
            }

            DoJobMoveZToVisionClear(false);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
        }

        public bool IsVacuumOK()
        {
            OutputEEPrecisorVacuumProcessTimeStopWatch.Start();
            bool VacuumOK = true;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return VacuumOK;
            }

            try
            {
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

            OutputEEPrecisorVacuumProcessTimeStopWatch.Stop();

            if (HSTMachine.Workcell != null)
            {
                if (HSTMachine.Workcell.getPanelOperation() != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                    {
                        UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().labelOutputPrecisorVacuumProcessTime.Text = OutputEEPrecisorVacuumProcessTimeStopWatch.ElapsedTime.ToString();
                        }); 
                    }
                }
            }

            return VacuumOK;
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
            bool moveDown = false;

            switch (pos)
            {
                case Motor_Position.ZPosParkPos:
                    {
                        dMovePos = _teachPointRecipe.OutputEESafeHeight;
                        moveDown = false;
                    }
                    break;
                case Motor_Position.ZPosPickPos_UpTab:
                    {
                        dMovePos = _teachPointRecipe.OutputEEPickHeight_UpTab;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZPosPickPos_DownTab:
                    {
                        dMovePos = _teachPointRecipe.OutputEEPickHeight_DownTab;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZPosPlacePos:
                    {
                        dMovePos = _teachPointRecipe.OutputEEPlaceHeight;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZPosVisionClearPos:
                    {
                        dMovePos = 79.00;
                        moveDown = false;
                    }
                    break;
                case Motor_Position.ZposDycemPos:
                    {
                        dMovePos = _teachPointRecipe.OutputEEDycemHeight;
                        moveDown = true;
                    }
                    break;
                case Motor_Position.ZMoveUpPartiallyPos_UpTab:
                    {
                        dMovePos = _teachPointRecipe.OutputEEPickHeight_UpTab - 0.5;
                        moveDown = false;
                    }
                    break;
                case Motor_Position.ZMoveUpPartiallyPos_DownTab:
                    {
                        dMovePos = _teachPointRecipe.OutputEEPickHeight_DownTab - 0.5;
                        moveDown = false;
                    }
                    break;
            }
            try
            {
                Log.Info(this, "OutputEE Axis before move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _OutputEndEffectorZAxis.GetActualPosition(), dMovePos, _axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Deceleration, _axesProfile.OutputEEZ.Velocity);

                if (maintenanceSpeed && HSTMachine.Workcell.HSTSettings.Install.EnableMaintenanceSpeedForManualMove)
                    _OutputEndEffectorZAxis.MoveAbsolute(outputEEMaxAcce * maintenanceSpeedRatio, outputEEMaxVel * maintenanceSpeedRatio, dMovePos, _moveTimeout);
                else
                    _OutputEndEffectorZAxis.MoveAbsolute(_axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Velocity, dMovePos, _moveTimeout);

                Log.Info(this, "OutputEE Axis after move position:{0}, Target position:{1}, Acce:{2}, Dece:{3}, Vel:{4}", _OutputEndEffectorZAxis.GetActualPosition(), dMovePos, _axesProfile.OutputEEZ.Acceleration, _axesProfile.OutputEEZ.Deceleration, _axesProfile.OutputEEZ.Velocity);
                
            }
            catch (Exception ex)
            {                
                HSTException.Throw((moveDown) ? HSTErrors.OutputHandlerZAxisMoveDownError : HSTErrors.OutputHandlerZAxisMoveUpError, ex);
            }
        }

        public void DoJobMoveZToPick(bool isUp, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Pick;
                return;
            }

            if(isUp)
                MoveZAxis(Motor_Position.ZPosPickPos_UpTab, maintenanceSpeed);
            else
                MoveZAxis(Motor_Position.ZPosPickPos_DownTab, maintenanceSpeed);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Pick;
        }

        public void DoJobMoveZToPark(bool maintenanceSpeed, bool moveZUpPositionPartially)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
                return;
            }
            if (moveZUpPositionPartially)
            {
                if(IncomingCarrier.HGATabType == HGAProductTabType.Up)
                    MoveZAxis(Motor_Position.ZMoveUpPartiallyPos_UpTab, maintenanceSpeed);
                else
                    MoveZAxis(Motor_Position.ZMoveUpPartiallyPos_DownTab, maintenanceSpeed);

                Thread.Sleep(300);
            }
            MoveZAxis(Motor_Position.ZPosParkPos, maintenanceSpeed);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
        }

        public void DoJobMoveZToVisionClear(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
                return;
            }

            MoveZAxis(Motor_Position.ZPosVisionClearPos, maintenanceSpeed);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
        }

        public void DoJobMoveZToPlace(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Place;
                return;
            }

            MoveZAxis(Motor_Position.ZPosPlacePos, maintenanceSpeed);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Place;
        }

        public void DoJobMoveZToDycem(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.DycemCleaning;
                return;
            }

            MoveZAxis(Motor_Position.ZposDycemPos, maintenanceSpeed);
            HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.DycemCleaning;
        }

        public double GetOutputEEPositionZ()
        {

            double position = 0;

            try
            {
                position = _OutputEndEffectorZAxis.GetActualPosition();
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.OutputHandlerZAxisReadPositionError, ex);
            }

            return position;
        }

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
                        _diOutputEndEffectorPnpPresent1.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.OutputEndEffectorVacuumPressureSensor1NotOnError, ex);
                        //End Effector1 Pnp Present1 Not On Error	
                    }
                }
                else
                    try
                    {
                        _diOutputEndEffectorPnpPresent1.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.OutputEndEffectorVacuumPressureSensor1NotOffError, ex);
                        //End Effector1 Pnp Present1 Not Off Error
                    }
            }
            if (bOn)
            {
                try
                {
                    _diOutputEndEffectorPnpPresent2.WaitForState(DigitalIOState.On, _sensorTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputEndEffectorVacuumPressureSensor2NotOnError, ex);
                    //End Effector1 Pnp Present2 Not On Error	
                }
            }
            else

                try
                {
                    _diOutputEndEffectorPnpPresent2.WaitForState(DigitalIOState.Off, _sensorTimeout);

                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputEndEffectorVacuumPressureSensor2NotOffError, ex);
                    //End Effector1 Pnp Present2 Not Off Error			

                }
        }

        public void DoJobVacuum(bool bOn)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation /*|| HSTMachine.Workcell.Config.DryRunWithoutPart*/)
                return;

            if (bOn)
            {
                _doOutputEndEffectorPnpPurgeOn.Set(DigitalIOState.Off);
                _doOutputEndEffectorPnpVacuumOn.Set(DigitalIOState.On);
            }
            else
            {
                _doOutputEndEffectorPnpVacuumOn.Set(DigitalIOState.Off);
                _doOutputEndEffectorPnpPurgeOn.Set(DigitalIOState.On);
                Thread.Sleep(HSTMachine.Workcell.SetupSettings.Delay.VacuumOffDelay);
                
                _doOutputEndEffectorPnpPurgeOn.Set(DigitalIOState.Off);

            }
        }


        #endregion

        #region Vision
        public Camera OutputCamera
        {
            get
            {
                return _outputcamera;
            }
            
        }
        public Carrier VisionInspect(HGAStatus hgaStatus, string CarrierID)
        {
            Carrier _carrier = new Carrier();
            if (Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
            {
                if (!HSTVision.Simulation)
                {
                    if (/*HSTMachine.Workcell.HSTSettings.Install.HGADetectionUsingVision && */Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
                    {
                        if (_outputcamera != null)
                        {
                            if (_outputcamera.GrabManual(true))
                            {
                                if (_outputcameraVisionApp.RunToolBlock(_outputcamera.grabImage, CarrierID)) // return true if vision tool success
                                {
                                    _carrier.ImageFileName = _outputcameraVisionApp.ImageFileName(); // 

                                    _carrier.Hga1.Hga_Status = _outputcameraVisionApp.GetResult(0);
                                    _carrier.Hga2.Hga_Status = _outputcameraVisionApp.GetResult(1);
                                    _carrier.Hga3.Hga_Status = _outputcameraVisionApp.GetResult(2);
                                    _carrier.Hga4.Hga_Status = _outputcameraVisionApp.GetResult(3);
                                    _carrier.Hga5.Hga_Status = _outputcameraVisionApp.GetResult(4);
                                    _carrier.Hga6.Hga_Status = _outputcameraVisionApp.GetResult(5);
                                    _carrier.Hga7.Hga_Status = _outputcameraVisionApp.GetResult(6);
                                    _carrier.Hga8.Hga_Status = _outputcameraVisionApp.GetResult(7);
                                    _carrier.Hga9.Hga_Status = _outputcameraVisionApp.GetResult(8);
                                    _carrier.Hga10.Hga_Status = _outputcameraVisionApp.GetResult(9);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Fail to acquire image");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Output Camera not initialize...");
                        }
                    }
                    else
                    {
                        _carrier.Hga1.Hga_Status = hgaStatus;
                        _carrier.Hga2.Hga_Status = hgaStatus;
                        _carrier.Hga3.Hga_Status = hgaStatus;
                        _carrier.Hga4.Hga_Status = hgaStatus;
                        _carrier.Hga5.Hga_Status = hgaStatus;
                        _carrier.Hga6.Hga_Status = hgaStatus;
                        _carrier.Hga7.Hga_Status = hgaStatus;
                        _carrier.Hga8.Hga_Status = hgaStatus;
                        _carrier.Hga9.Hga_Status = hgaStatus;
                        _carrier.Hga10.Hga_Status = hgaStatus;
                    }
                }
            }
            else 
            {
                _carrier.Hga1.Hga_Status = hgaStatus;
                _carrier.Hga2.Hga_Status = hgaStatus;
                _carrier.Hga3.Hga_Status = hgaStatus;
                _carrier.Hga4.Hga_Status = hgaStatus;
                _carrier.Hga5.Hga_Status = hgaStatus;
                _carrier.Hga6.Hga_Status = hgaStatus;
                _carrier.Hga7.Hga_Status = hgaStatus;
                _carrier.Hga8.Hga_Status = hgaStatus;
                _carrier.Hga9.Hga_Status = hgaStatus;
                _carrier.Hga10.Hga_Status = hgaStatus;
            }

            return _carrier;
        }

        public bool IsVacumnSensor10PartsActive()
        {
            All10PartsPicked = _diOutputEndEffectorPnpPresent1.Get() == DigitalIOState.On &&
                                _diOutputEndEffectorPnpPresent2.Get() == DigitalIOState.On;

            return All10PartsPicked;
        }

        private void VisionSettingsChanged(object sender, EventArgs e)
        {
            if (_outputcameraVisionApp != null)
            {
                ConfigureOutputCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveImagesLessThanTenHGAs,
                                      Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveAllImages);
            }
        }

        private void ConfigureOutputCamera(bool SaveImagesIfLessHGAs, bool SaveAllImages)
        {
            if (!HSTVision.Simulation)
            {
                try
                {
                    _outputcameraVisionApp.LoadRecipe(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.Recipe, CameraLocation.OutputStation);
                    _outputcameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.ImagesOutputPath,
                                                        SaveImagesIfLessHGAs,
                                                        SaveAllImages);
                }
                catch (Exception ex)
                {
                    Notify.PopUpError("Output Camera Configuration Error.", ex);
                }
            }
        }

        private void AutomationConfigChanged(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Acceleration > outputEEMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Acceleration = outputEEMaxAcce;
            _axesProfile.OutputEEZ.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Deceleration > outputEEMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Deceleration = outputEEMaxDece;
            _axesProfile.OutputEEZ.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Velocity > outputEEMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Velocity = outputEEMaxVel;
            _axesProfile.OutputEEZ.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.OutputEEZ.Velocity;
        }
        #endregion
    }
}
