using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Recipe;
using System.Threading;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Vision;
using XyratexOSC.XMath;
using Seagate.AAS.Parsel.Hw.Aerotech.A3200;
using Seagate.AAS.Utils;
using XyratexOSC.Logging;
using System.Threading.Tasks;
using Aerotech.A3200;
using Aerotech.A3200.Status;
namespace Seagate.AAS.HGA.HST.Controllers
{
    public class PrecisorStationController : ControllerHST
    {
        private HSTWorkcell _workcell;
        private HSTIOManifest _ioManifest;
        private string name = "PrecisorStationController";

        IAxis _precisorNestXAxis;
        IAxis _precisorNestYAxis;
        IAxis _precisorNestThetaAxis;
        AxisGroup _precisorNest;

        private IDigitalInput _diPrecisorNestPnpPresent1;
        private IDigitalInput _diPrecisorNestPnpPresent2;
        private IDigitalInput _diPrecisorNestPnpPresent3;
        private IDigitalInput _diPrecisorNestPnpPresent4;
        private IDigitalInput _diPrecisorNestPnpPresent5;
        private IDigitalInput _diPrecisorNestPnpPresent6;
        private IDigitalInput _diPrecisorNestPnpPresent7;

        private IDigitalOutput _doPrecisorNestVacuumTabSelector1;
        private IDigitalOutput _doPrecisorNestVacuumTabSelector2;
        private IDigitalOutput _doPrecisorNestVacuumTabSelector3;
        private IDigitalOutput _doPrecisorNestVacuumTabSelector4;
        private IDigitalOutput _doPrecisorNestVacuumTabSelector5;
        private IDigitalOutput _doPrecisorNestVacuumTabSelector6;
        private IDigitalOutput _doPrecisorNestVacuumTabSelector7;

        private IDigitalOutput _doPrecisorNestPnpVacuumOn1;
        private IDigitalOutput _doPrecisorNestPnpVacuumOn2;
        private IDigitalOutput _doPrecisorNestPnpVacuumOn3;
        private IDigitalOutput _doPrecisorNestPnpVacuumOn4;
        private IDigitalOutput _doPrecisorNestPnpVacuumOn5;
        private IDigitalOutput _doPrecisorNestPnpVacuumOn6;
        private IDigitalOutput _doPrecisorNestPnpVacuumOn7;

        private IDigitalOutput _doPrecisorNestPnpPurgeOn1;
        private IDigitalOutput _doPrecisorNestPnpPurgeOn2;
        private IDigitalOutput _doPrecisorNestPnpPurgeOn3;
        private IDigitalOutput _doPrecisorNestPnpPurgeOn4;
        private IDigitalOutput _doPrecisorNestPnpPurgeOn5;
        private IDigitalOutput _doPrecisorNestPnpPurgeOn6;
        private IDigitalOutput _doPrecisorNestPnpPurgeOn7;

        private AxesProfile _axesProfile;
        private TeachPointRecipe _teachPointRecipe;

        private VisionHardware HSTVision;
        private Camera _fiducialcamera;
        private VisionApp _fiducialcameraVisionApp;
        public event EventHandler OnSettingChanged;

        private const int precisorNestXMaxAcce = 3500;
        private const int precisorNestXMaxDece = 3500;
        private const int precisorNestXMaxVel = 1000;

        private const int precisorNestYMaxAcce = 1000;
        private const int precisorNestYMaxDece = 1000;
        private const int precisorNestYMaxVel = 50;

        private const int precisorNestThetaMaxAcce = 100;
        private const int precisorNestThetaMaxDece = 100;
        private const int precisorNestThetaMaxVel = 10;

        private const uint _moveTimeout = 50000;

        private const double maintenanceSpeedRatio = 0.1;

        // Motor locations
        public enum Motor_Position
        {
            PosInputStation_UpTab,
            PosInputStation_DownTab,
            PosOutputStation_UpTab,
            PosOutputStation_DownTab,
            PosPrecisorStation_UpTab,
            PosPrecisorStation_DownTab,
            PosParkLocation
        }

        // Local Variables
        private const uint _actuatorHomeXTimeout = 50000;
        private const uint _actuatorHomeYTimeout = 10000;
        private const uint _actuatorHomeThetaTimeout = 10000;
        private const uint _sensorTimeout = 3000;
        private HGAProductTailType _currentHGATailType;

        public PrecisorStationController()
        {

        }

        // Constructors ------------------------------------------------------------
        public PrecisorStationController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            this._ioManifest = (HSTIOManifest)HSTMachine.Workcell.IOManifest;

            _axesProfile = workcell.SetupConfig.AxesProfile;

            //x
            if (_axesProfile.PrecisorX.Acceleration > precisorNestXMaxAcce)
                _axesProfile.PrecisorX.Acceleration = precisorNestXMaxAcce;
            _axesProfile.PrecisorX.Acceleration = _axesProfile.PrecisorX.Acceleration;

            if (_axesProfile.PrecisorX.Deceleration > precisorNestXMaxDece)
                _axesProfile.PrecisorX.Deceleration = precisorNestXMaxDece;
            _axesProfile.PrecisorX.Deceleration = _axesProfile.PrecisorX.Deceleration;

            if (_axesProfile.PrecisorX.Velocity > precisorNestXMaxVel)
                _axesProfile.PrecisorX.Velocity = precisorNestXMaxVel;
            _axesProfile.PrecisorX.Velocity = _axesProfile.PrecisorX.Velocity;

            //y
            if (_axesProfile.PrecisorY.Acceleration > precisorNestYMaxAcce)
                _axesProfile.PrecisorY.Acceleration = precisorNestYMaxAcce;
            _axesProfile.PrecisorY.Acceleration = _axesProfile.PrecisorY.Acceleration;

            if (_axesProfile.PrecisorY.Deceleration > precisorNestYMaxDece)
                _axesProfile.PrecisorY.Deceleration = precisorNestYMaxDece;
            _axesProfile.PrecisorY.Deceleration = _axesProfile.PrecisorY.Deceleration;

            if (_axesProfile.PrecisorY.Velocity > precisorNestYMaxVel)
                _axesProfile.PrecisorY.Velocity = precisorNestYMaxVel;
            _axesProfile.PrecisorY.Velocity = _axesProfile.PrecisorY.Velocity;

            //theta
            if (_axesProfile.PrecisorTheta.Acceleration > precisorNestThetaMaxAcce)
                _axesProfile.PrecisorTheta.Acceleration = precisorNestThetaMaxAcce;
            _axesProfile.PrecisorTheta.Acceleration = _axesProfile.PrecisorTheta.Acceleration;

            if (_axesProfile.PrecisorTheta.Deceleration > precisorNestThetaMaxDece)
                _axesProfile.PrecisorTheta.Deceleration = precisorNestThetaMaxDece;
            _axesProfile.PrecisorTheta.Deceleration = _axesProfile.PrecisorTheta.Deceleration;

            if (_axesProfile.PrecisorTheta.Velocity > precisorNestThetaMaxVel)
                _axesProfile.PrecisorTheta.Velocity = precisorNestThetaMaxVel;
            _axesProfile.PrecisorTheta.Velocity = _axesProfile.PrecisorTheta.Velocity;

            _teachPointRecipe = workcell.TeachPointRecipe;

            _precisorNestXAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.X);
            _precisorNestYAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Y);
            _precisorNestThetaAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Theta);
            _precisorNest = new AxisGroup(_workcell._a3200HC, new[] { _precisorNestXAxis, _precisorNestYAxis, _precisorNestThetaAxis }, TaskId.T01);


            _diPrecisorNestPnpPresent1 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_1);
            _diPrecisorNestPnpPresent2 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_2);
            _diPrecisorNestPnpPresent3 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_3);
            _diPrecisorNestPnpPresent4 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_4);
            _diPrecisorNestPnpPresent5 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_5);
            _diPrecisorNestPnpPresent6 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_6);
            _diPrecisorNestPnpPresent7 = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.PN_VS_7);

            _doPrecisorNestVacuumTabSelector1 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_1);
            _doPrecisorNestVacuumTabSelector2 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_2);
            _doPrecisorNestVacuumTabSelector3 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_3);
            _doPrecisorNestVacuumTabSelector4 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_4);
            _doPrecisorNestVacuumTabSelector5 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_5);
            _doPrecisorNestVacuumTabSelector6 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_6);
            _doPrecisorNestVacuumTabSelector7 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VTS_7);

            _doPrecisorNestPnpVacuumOn1 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_1);
            _doPrecisorNestPnpVacuumOn2 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_2);
            _doPrecisorNestPnpVacuumOn3 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_3);
            _doPrecisorNestPnpVacuumOn4 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_4);
            _doPrecisorNestPnpVacuumOn5 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_5);
            _doPrecisorNestPnpVacuumOn6 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_6);
            _doPrecisorNestPnpVacuumOn7 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_7);

            _doPrecisorNestPnpPurgeOn1 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_1);
            _doPrecisorNestPnpPurgeOn2 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_2);
            _doPrecisorNestPnpPurgeOn3 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_3);
            _doPrecisorNestPnpPurgeOn4 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_4);
            _doPrecisorNestPnpPurgeOn5 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_5);
            _doPrecisorNestPnpPurgeOn6 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_6);
            _doPrecisorNestPnpPurgeOn7 = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_PCH_7);
            Machine.HSTMachine.Workcell.CalibrationSettings.OnSettingsChanged += VisionSettingsChanged;
            Machine.HSTMachine.Workcell.SetupSettings.OnSettingsChanged += AutomationConfigChanged;

        }


        // Properties ----------------------------------------------------------
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Carrier IncomingCarrier { get; set; }


        // Methods -------------------------------------------------------------
        public override void InitializeController()
        {
            SetAction(1);
        }

        public void CheckPrecisorVacuum()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }
        }

        public void BoundaryCheckAllAxes()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            BoundaryCheckXAxis();
            BoundaryCheckYAxis();
            BoundaryCheckThetaAxis();
        }

        public void BoundaryCheckXAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            try
            {
                double dMoveXPos = _teachPointRecipe.PrecisorInputStationPositionX_DownTab;//Left most position in operation range

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest X Axis before move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                }
                _precisorNestXAxis.MoveAbsolute(_axesProfile.PrecisorX.Acceleration, _axesProfile.PrecisorX.Velocity, dMoveXPos, _moveTimeout);

                while (_precisorNestXAxis.GetActualPosition() < dMoveXPos - 10 || _precisorNestXAxis.GetActualPosition() > dMoveXPos + 10)
                {
                    MessageBox.Show("PrecisorNest X Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveXAxis. _precisorNestXAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                    }
                    Thread.Sleep(100);
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest X Axis after move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveLeftError, ex);
            }

            try
            {
                double dMoveXPos = _teachPointRecipe.PrecisorOutputStationPositionX_UpTab;//Right most position in operation range

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest X Axis before move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                }
                _precisorNestXAxis.MoveAbsolute(_axesProfile.PrecisorX.Acceleration, _axesProfile.PrecisorX.Velocity, dMoveXPos, _moveTimeout);

                while (_precisorNestXAxis.GetActualPosition() < dMoveXPos - 10 || _precisorNestXAxis.GetActualPosition() > dMoveXPos + 10)
                {
                    MessageBox.Show("PrecisorNest X Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveXAxis. _precisorNestXAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest X Axis after move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveRightError, ex);
            }
        }

        public void BoundaryCheckYAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            try
            {
                double dMoveYPos = _teachPointRecipe.PrecisorInputStationPositionY_DownTab + 2;//take input position plus 10mm as the back most position in operation range 

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Y Axis before move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                }
                _precisorNestYAxis.MoveAbsolute(_axesProfile.PrecisorY.Acceleration, _axesProfile.PrecisorY.Velocity, dMoveYPos, _moveTimeout);

                while (_precisorNestYAxis.GetActualPosition() < dMoveYPos - 10 || _precisorNestYAxis.GetActualPosition() > dMoveYPos + 10)
                {
                    MessageBox.Show("PrecisorNest Y Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveYAxis. _precisorNestYAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Y Axis after move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveFrontError, ex);
            }

            try
            {
                double dMoveYPos = _teachPointRecipe.PrecisorInputStationPositionY_DownTab - 2;//take input position plus 10mm as the front most position in operation range 

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Y Axis before move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                }
                _precisorNestYAxis.MoveAbsolute(_axesProfile.PrecisorY.Acceleration, _axesProfile.PrecisorY.Velocity, dMoveYPos, _moveTimeout);

                while (_precisorNestYAxis.GetActualPosition() < dMoveYPos - 10 || _precisorNestYAxis.GetActualPosition() > dMoveYPos + 10)
                {
                    MessageBox.Show("PrecisorNest Y Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveYAxis. _precisorNestYAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Y Axis after move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveBackError, ex);
            }
        }

        public void BoundaryCheckThetaAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            try
            {
                double dMoveThetaPos = _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab + 1;//take input position plus 5 degree as the CW most position in operation range 
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Theta Axis before move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveThetaPos);
                }
                _precisorNestThetaAxis.MoveAbsolute(_axesProfile.PrecisorTheta.Acceleration, _axesProfile.PrecisorTheta.Velocity, dMoveThetaPos, _moveTimeout);

                while (_precisorNestThetaAxis.GetActualPosition() < dMoveThetaPos - 10 || _precisorNestThetaAxis.GetActualPosition() > dMoveThetaPos + 10)
                {
                    MessageBox.Show("PrecisorNest Theta Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveThetaAxis. _precisorNestThetaAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveThetaPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Theta Axis after move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveThetaPos);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateClockwiseError, ex);
            }

            try
            {
                double dMoveThetaPos = _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab - 1;//take input position plus 5 degree as the CCW most position in operation range 

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Theta Axis before move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveThetaPos);
                }
                _precisorNestThetaAxis.MoveAbsolute(_axesProfile.PrecisorTheta.Acceleration, _axesProfile.PrecisorTheta.Velocity, dMoveThetaPos, _moveTimeout);

                while (_precisorNestThetaAxis.GetActualPosition() < dMoveThetaPos - 10 || _precisorNestThetaAxis.GetActualPosition() > dMoveThetaPos + 10)
                {
                    MessageBox.Show("PrecisorNest Theta Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveThetaAxis. _precisorNestThetaAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveThetaPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Theta Axis after move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveThetaPos);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateCounterClockwiseError, ex);
            }
        }

        public void GoToHomeAllAxes()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            HomeXAxis();
            HomeYAxis();
            HomeThetaAxis();
        }

        public void HomeXAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _workcell._a3200HC.FaultAcknowlegde(_precisorNestXAxis);

                _precisorNestXAxis.Enable(true);
                Thread.Sleep(400);
                _precisorNestXAxis.Home(_actuatorHomeXTimeout);

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisHomeError, ex);
            }
        }

        public void HomeYAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            bool Status = false;
            int retry = 0;

            while (retry < 3)// if something wrong during home retry 2 times;
            {
                try
                {
                    _workcell._a3200HC.FaultAcknowlegde(_precisorNestYAxis);

                    _precisorNestYAxis.Enable(true);
                    Thread.Sleep(400);
                    _precisorNestYAxis.Home(_actuatorHomeYTimeout);
                    Thread.Sleep(1000);

                    Status = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(200);
                    Log.Warn(this, "Home Y axis error: {0}, Retry:{1}", ex.Message, retry);
                    retry++;
                    _workcell._a3200HC.FaultAcknowlegde(_precisorNestYAxis);
                    Thread.Sleep(200);

                    if (retry == 2)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestYAxisHomeError, ex);
                        break;
                    }
                }

                if (Status)
                    break;
            }
        }

        public void HomeThetaAxis()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _workcell._a3200HC.FaultAcknowlegde(_precisorNestThetaAxis);

                _precisorNestThetaAxis.Enable(true);
                Thread.Sleep(400);
                _precisorNestThetaAxis.Home(_actuatorHomeThetaTimeout);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestThetaAxisHomeError, ex);
            }

        }

        public void ParkPrecisorNestBetweenInputStationAndPrecisorStation(bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.Parked;
                return;
            }
            try
            {
                MovePrecisorNestXYTheta(Motor_Position.PosParkLocation, maintenanceSpeed);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.Parked;
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveToParkPositionError, ex);
            }
        }

        public bool IsSafeToMoveToInputStation()
        {
            bool flagValue = false;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                flagValue = true;
            }
            else if (HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked)
            {
                flagValue = true;
            }
            return flagValue;
        }

        public void MoveToInputStation(bool IsUp, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtInputStation;
                return;
            }
            try
            {
                if (IsUp)
                    MovePrecisorNestXYTheta(Motor_Position.PosInputStation_UpTab, maintenanceSpeed);
                else
                    MovePrecisorNestXYTheta(Motor_Position.PosInputStation_DownTab, maintenanceSpeed);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtInputStation;
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveLeftError, ex);
            }

        }

        public void SetValvePosition(bool IsUp)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (IsUp)
            {
                _doPrecisorNestVacuumTabSelector1.Set(DigitalIOState.On);
                _doPrecisorNestVacuumTabSelector2.Set(DigitalIOState.On);
                _doPrecisorNestVacuumTabSelector3.Set(DigitalIOState.On);
                _doPrecisorNestVacuumTabSelector4.Set(DigitalIOState.On);
                _doPrecisorNestVacuumTabSelector5.Set(DigitalIOState.On);
                _doPrecisorNestVacuumTabSelector6.Set(DigitalIOState.On);
                _doPrecisorNestVacuumTabSelector7.Set(DigitalIOState.On);
            }
            else
            {
                _doPrecisorNestVacuumTabSelector1.Set(DigitalIOState.Off);
                _doPrecisorNestVacuumTabSelector2.Set(DigitalIOState.Off);
                _doPrecisorNestVacuumTabSelector3.Set(DigitalIOState.Off);
                _doPrecisorNestVacuumTabSelector4.Set(DigitalIOState.Off);
                _doPrecisorNestVacuumTabSelector5.Set(DigitalIOState.Off);
                _doPrecisorNestVacuumTabSelector6.Set(DigitalIOState.Off);
                _doPrecisorNestVacuumTabSelector7.Set(DigitalIOState.Off);
            }
        }

        public void TurnOnVacuumChannelsOneByOne(HGAProductTailType HGATailType)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            int delay = 100;

            if (HGATailType == HGAProductTailType.LongTail)
            {
                _doPrecisorNestPnpPurgeOn4.Set(DigitalIOState.Off);
                _doPrecisorNestPnpVacuumOn4.Set(DigitalIOState.On);
                Thread.Sleep(delay + 100);
                _doPrecisorNestPnpPurgeOn3.Set(DigitalIOState.Off);
                _doPrecisorNestPnpVacuumOn3.Set(DigitalIOState.On);
                Thread.Sleep(delay);
                _doPrecisorNestPnpPurgeOn2.Set(DigitalIOState.Off);
                _doPrecisorNestPnpVacuumOn2.Set(DigitalIOState.On);
                Thread.Sleep(delay);
            }
            else
            {
                _doPrecisorNestPnpPurgeOn2.Set(DigitalIOState.Off);
                _doPrecisorNestPnpVacuumOn2.Set(DigitalIOState.On);
                Thread.Sleep(delay);
                _doPrecisorNestPnpPurgeOn3.Set(DigitalIOState.Off);
                _doPrecisorNestPnpVacuumOn3.Set(DigitalIOState.On);
                Thread.Sleep(delay);
                _doPrecisorNestPnpPurgeOn4.Set(DigitalIOState.Off);
                _doPrecisorNestPnpVacuumOn4.Set(DigitalIOState.On);
                Thread.Sleep(delay);
            }
            _doPrecisorNestPnpPurgeOn1.Set(DigitalIOState.Off);
            _doPrecisorNestPnpVacuumOn1.Set(DigitalIOState.On);
        }

        public void TurnOffVaccuumChannels()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            _doPrecisorNestPnpVacuumOn1.Set(DigitalIOState.Off);
            _doPrecisorNestPnpVacuumOn2.Set(DigitalIOState.Off);
            _doPrecisorNestPnpVacuumOn3.Set(DigitalIOState.Off);
            _doPrecisorNestPnpVacuumOn4.Set(DigitalIOState.Off);

            // Lai: Attempt to resolve left over HGAs at precisor nest after pick by output EE
            _doPrecisorNestPnpPurgeOn1.Set(DigitalIOState.On);
            _doPrecisorNestPnpPurgeOn2.Set(DigitalIOState.On);
            _doPrecisorNestPnpPurgeOn3.Set(DigitalIOState.On);
            _doPrecisorNestPnpPurgeOn4.Set(DigitalIOState.On);
            if (_workcell.HSTSettings.Install.HGAPurgingDurationInms > 0)
            {
                Thread.Sleep(_workcell.HSTSettings.Install.HGAPurgingDurationInms);
                Log.Info(this, "HGA Purging duartion:{0} ms", _workcell.HSTSettings.Install.HGAPurgingDurationInms);
            }
            else
            {

                Thread.Sleep(200);
                Log.Info(this, "HGA Purging duartion:200 ms");
            }
            _doPrecisorNestPnpPurgeOn1.Set(DigitalIOState.Off);
            _doPrecisorNestPnpPurgeOn2.Set(DigitalIOState.Off);
            _doPrecisorNestPnpPurgeOn3.Set(DigitalIOState.Off);
            _doPrecisorNestPnpPurgeOn4.Set(DigitalIOState.Off);
        }

        public void TurnOnPurgeChannels()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            _doPrecisorNestPnpPurgeOn1.Set(DigitalIOState.On);
            _doPrecisorNestPnpPurgeOn2.Set(DigitalIOState.On);
            _doPrecisorNestPnpPurgeOn3.Set(DigitalIOState.On);
            _doPrecisorNestPnpPurgeOn4.Set(DigitalIOState.On);
        }

        public void TurnOffPurgeChannels()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            _doPrecisorNestPnpPurgeOn1.Set(DigitalIOState.Off);
            _doPrecisorNestPnpPurgeOn2.Set(DigitalIOState.Off);
            _doPrecisorNestPnpPurgeOn3.Set(DigitalIOState.Off);
            _doPrecisorNestPnpPurgeOn4.Set(DigitalIOState.Off);
        }
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
                    try
                    {
                        _diPrecisorNestPnpPresent1.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor1NotOnError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent2.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor2NotOnError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent3.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor3NotOnError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent4.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor4NotOnError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent5.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor5NotOnError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent6.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor6NotOnError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent7.WaitForState(DigitalIOState.On, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor7NotOnError, ex);
                    }
                }
                else
                {
                    try
                    {
                        _diPrecisorNestPnpPresent1.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor1NotOffError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent2.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor2NotOffError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent3.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor3NotOffError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent4.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor4NotOffError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent5.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor5NotOffError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent6.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor6NotOffError, ex);
                    }

                    try
                    {
                        _diPrecisorNestPnpPresent7.WaitForState(DigitalIOState.Off, _sensorTimeout);
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.PrecisorNestVacuumPressureSensor7NotOffError, ex);
                    }
                }
            }
        }

        public bool IsSafeToMoveToPrecisorStation()
        {
            bool flagValue = false;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                flagValue = true;
            }
            else if (HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked)
            {
                flagValue = true;
            }
            return flagValue;
        }

        public void MoveToPrecisorStation(bool IsUp, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtPrecisorStation;
                return;
            }

            try
            {
                if (IsUp)
                    MovePrecisorNestXYTheta(Motor_Position.PosPrecisorStation_UpTab, maintenanceSpeed);
                else
                    MovePrecisorNestXYTheta(Motor_Position.PosPrecisorStation_DownTab, maintenanceSpeed);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtPrecisorStation;
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveToTestStationError, ex);
            }
        }

        public bool IsSafeToMoveToOutputStation()
        {
            bool flagValue = false;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                flagValue = true;
            }
            else if (HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked)
            {
                flagValue = true;
            }
            return flagValue;
        }

        public void MoveToOutputStation(bool IsUp, bool maintenanceSpeed)
        {
            Log.Info(this, "PrecisorNest going to move");
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtOutputStation;
                return;
            }
            try
            {
                if (IsUp)
                    MovePrecisorNestXYTheta(Motor_Position.PosOutputStation_UpTab, maintenanceSpeed);
                else
                    MovePrecisorNestXYTheta(Motor_Position.PosOutputStation_DownTab, maintenanceSpeed);
                HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtOutputStation;
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveRightError, ex);
            }
        }

        public void MoveTAxis(Motor_Position pos)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            double dMoveTPos = 0;
            switch (pos)
            {
                case Motor_Position.PosInputStation_UpTab:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorInputStationPositionTheta_UpTab;
                    }
                    break;
                case Motor_Position.PosInputStation_DownTab:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_UpTab:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorOutputStationPositionTheta_UpTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_DownTab:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorOutputStationPositionTheta_DownTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_UpTab:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorTestStationPositionTheta_UpTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_DownTab:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorTestStationPositionTheta_DownTab;
                    }
                    break;
                case Motor_Position.PosParkLocation:
                    {
                        dMoveTPos = _teachPointRecipe.PrecisorSafePositionTheta;
                    }
                    break;
            }

            bool moveClockWise = (dMoveTPos > _precisorNestThetaAxis.GetActualPosition()) ? true : false;

            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Theta Axis before move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveTPos);
                }
                _precisorNestThetaAxis.MoveAbsolute(_axesProfile.PrecisorTheta.Acceleration, _axesProfile.PrecisorTheta.Velocity, dMoveTPos, _moveTimeout);

                while (_precisorNestThetaAxis.GetActualPosition() < dMoveTPos - 10 || _precisorNestThetaAxis.GetActualPosition() > dMoveTPos + 10)
                {
                    MessageBox.Show("PrecisorNest Theta Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveThetaAxis. _precisorNestThetaAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveTPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Theta Axis after move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveTPos);
                }
            }
            catch (Exception ex)
            {
                if (moveClockWise)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateClockwiseError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateCounterClockwiseError, ex);
                }
            }

        }

        public void MoveXAxis(Motor_Position location)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            double dMoveXPos = 0;

            switch (location)
            {
                case Motor_Position.PosInputStation_UpTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorInputStationPositionX_UpTab;
                    }
                    break;
                case Motor_Position.PosInputStation_DownTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorInputStationPositionX_DownTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_UpTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorOutputStationPositionX_UpTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_DownTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorOutputStationPositionX_DownTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_UpTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorTestStationPositionX_UpTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_DownTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorTestStationPositionX_DownTab;
                    }
                    break;
                case Motor_Position.PosParkLocation:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorSafePositionX;
                    }
                    break;
            }

            bool moveRight = false;

            try
            {
                moveRight = (dMoveXPos > _precisorNestXAxis.GetActualPosition()) ? true : false;
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest X Axis before move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                }
                _precisorNestXAxis.MoveAbsolute(_axesProfile.PrecisorX.Acceleration, _axesProfile.PrecisorX.Velocity, dMoveXPos, _moveTimeout);

                while (_precisorNestXAxis.GetActualPosition() < dMoveXPos - 10 || _precisorNestXAxis.GetActualPosition() > dMoveXPos + 10)
                {
                    MessageBox.Show("PrecisorNest X Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveXAxis. _precisorNestXAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                    }
                    Thread.Sleep(100);
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest X Axis after move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                }

            }
            catch (Exception ex)
            {
                if (moveRight)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveRightError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveLeftError, ex);
                }
            }
        }


        public void MoveYAxis(Motor_Position location)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            double dMoveYPos = 0;

            switch (location)
            {
                case Motor_Position.PosInputStation_UpTab:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorInputStationPositionY_UpTab;
                    }
                    break;
                case Motor_Position.PosInputStation_DownTab:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorInputStationPositionY_DownTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_UpTab:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorOutputStationPositionY_UpTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_DownTab:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorOutputStationPositionY_DownTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_UpTab:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorTestStationPositionY_UpTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_DownTab:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorTestStationPositionY_DownTab;
                    }
                    break;
                case Motor_Position.PosParkLocation:
                    {
                        dMoveYPos = _teachPointRecipe.PrecisorSafePositionY;
                    }
                    break;
            }

            bool moveFront = false;

            try
            {
                moveFront = (dMoveYPos > _precisorNestYAxis.GetActualPosition()) ? true : false;
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Y Axis before move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                }

                _precisorNestYAxis.MoveAbsolute(_axesProfile.PrecisorY.Acceleration, _axesProfile.PrecisorY.Velocity, dMoveYPos, _moveTimeout);

                while (_precisorNestYAxis.GetActualPosition() < dMoveYPos - 10 || _precisorNestYAxis.GetActualPosition() > dMoveYPos + 10)
                {
                    MessageBox.Show("PrecisorNest Y Axis Actual Position not tally with target position after move done");
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Precisor Nest MoveYAxis. _precisorNestYAxis.GetActualPosition():{0} targetposition:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                    }
                    Thread.Sleep(100);
                }
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Precisor Nest Y Axis after move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                }
            }
            catch (Exception ex)
            {
                if (moveFront)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveFrontError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveBackError, ex);
                }
            }
        }

        public void MoveXAxisRelatively(double dRelativePos)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _precisorNestXAxis.MoveRelative(_axesProfile.PrecisorX.Acceleration, _axesProfile.PrecisorX.Velocity, dRelativePos, _moveTimeout);
            }
            catch (Exception ex)
            {
                if (dRelativePos > 0)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveRightError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveLeftError, ex);
                }
            }
        }

        public void MoveYAxisRelatively(double dRelativePos)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _precisorNestYAxis.MoveRelative(_axesProfile.PrecisorY.Acceleration, _axesProfile.PrecisorY.Velocity, dRelativePos, _moveTimeout);
            }
            catch (Exception ex)
            {
                if (dRelativePos > 0)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveBackError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveFrontError, ex);
                }
            }
        }

        public void MoveTAxisRelatively(double dRelativePos)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                _precisorNestThetaAxis.MoveRelative(_axesProfile.PrecisorTheta.Acceleration, _axesProfile.PrecisorTheta.Velocity, dRelativePos, _moveTimeout);
            }
            catch (Exception ex)
            {
                if (dRelativePos > 0)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateClockwiseError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateCounterClockwiseError, ex);
                }
            }

        }

        public void MovePrecisorNestXYTheta(Motor_Position location, bool maintenanceSpeed)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            double dMoveXPos = 0;
            double dMoveYPos = 0;
            double dMoveTPos = 0;

            switch (location)
            {
                case Motor_Position.PosInputStation_UpTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorInputStationPositionX_UpTab;
                        dMoveYPos = _teachPointRecipe.PrecisorInputStationPositionY_UpTab;
                        dMoveTPos = _teachPointRecipe.PrecisorInputStationPositionTheta_UpTab;
                    }
                    break;
                case Motor_Position.PosInputStation_DownTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorInputStationPositionX_DownTab;
                        dMoveYPos = _teachPointRecipe.PrecisorInputStationPositionY_DownTab;
                        dMoveTPos = _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_UpTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorOutputStationPositionX_UpTab;
                        dMoveYPos = _teachPointRecipe.PrecisorOutputStationPositionY_UpTab;
                        dMoveTPos = _teachPointRecipe.PrecisorOutputStationPositionTheta_UpTab;
                    }
                    break;
                case Motor_Position.PosOutputStation_DownTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorOutputStationPositionX_DownTab;
                        dMoveYPos = _teachPointRecipe.PrecisorOutputStationPositionY_DownTab;
                        dMoveTPos = _teachPointRecipe.PrecisorOutputStationPositionTheta_DownTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_UpTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorTestStationPositionX_UpTab;
                        dMoveYPos = _teachPointRecipe.PrecisorTestStationPositionY_UpTab;
                        dMoveTPos = _teachPointRecipe.PrecisorTestStationPositionTheta_UpTab;
                    }
                    break;
                case Motor_Position.PosPrecisorStation_DownTab:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorTestStationPositionX_DownTab;
                        dMoveYPos = _teachPointRecipe.PrecisorTestStationPositionY_DownTab;
                        dMoveTPos = _teachPointRecipe.PrecisorTestStationPositionTheta_DownTab;
                    }
                    break;
                case Motor_Position.PosParkLocation:
                    {
                        dMoveXPos = _teachPointRecipe.PrecisorSafePositionX;
                        dMoveYPos = _teachPointRecipe.PrecisorSafePositionY;
                        dMoveTPos = _teachPointRecipe.PrecisorSafePositionTheta;
                    }
                    break;
            }

            bool moveRight = false;
            bool moveFront = false;
            bool moveClockWise = false;

            try
            {
                moveRight = (dMoveXPos > _precisorNestXAxis.GetActualPosition()) ? true : false;
                moveFront = (dMoveYPos > _precisorNestYAxis.GetActualPosition()) ? true : false;
                moveClockWise = (dMoveTPos > _precisorNestThetaAxis.GetActualPosition()) ? true : false;

                if (location != Motor_Position.PosParkLocation)
                {
                    double[] PrecisorNestAbsolutePosition = new double[] { dMoveXPos, dMoveYPos, dMoveTPos };
                    if (maintenanceSpeed && HSTMachine.Workcell.HSTSettings.Install.EnableMaintenanceSpeedForManualMove)
                        _precisorNest.SetLinearMoveProfile(new Seagate.AAS.Parsel.Hw.Aerotech.A3200.MoveProfile(precisorNestXMaxAcce * maintenanceSpeedRatio, precisorNestXMaxDece * maintenanceSpeedRatio, precisorNestXMaxVel * maintenanceSpeedRatio));
                    else
                        _precisorNest.SetLinearMoveProfile(new Seagate.AAS.Parsel.Hw.Aerotech.A3200.MoveProfile(_axesProfile.PrecisorX.Acceleration, _axesProfile.PrecisorX.Deceleration, _axesProfile.PrecisorX.Velocity));
                    _precisorNest.LinearMoveAbsolute(PrecisorNestAbsolutePosition, _moveTimeout);
                }
                else
                {
                    if (maintenanceSpeed && HSTMachine.Workcell.HSTSettings.Install.EnableMaintenanceSpeedForManualMove)
                        _precisorNestXAxis.MoveAbsolute(precisorNestXMaxAcce * maintenanceSpeedRatio, precisorNestXMaxVel * maintenanceSpeedRatio, dMoveXPos, _moveTimeout);
                    else
                        _precisorNestXAxis.MoveAbsolute(_axesProfile.PrecisorX.Acceleration, _axesProfile.PrecisorX.Velocity, dMoveXPos, _moveTimeout);
                }

                Log.Info(this, "Precisor Nest X Axis after move position:{0}, Target position:{1}", _precisorNestXAxis.GetActualPosition(), dMoveXPos);
                Log.Info(this, "Precisor Nest Y Axis after move position:{0}, Target position:{1}", _precisorNestYAxis.GetActualPosition(), dMoveYPos);
                Log.Info(this, "Precisor Nest Theta Axis after move position:{0}, Target position:{1}", _precisorNestThetaAxis.GetActualPosition(), dMoveTPos);
            }
            catch (Exception ex)
            {
                if (moveRight)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveRightError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestXAxisMoveLeftError, ex);
                }

                if (moveFront)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveFrontError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestYAxisMoveBackError, ex);
                }

                if (moveClockWise)
                {
                    HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateClockwiseError, ex);
                }
                else
                {
                    HSTException.Throw(HSTErrors.PrecisorNestThetaAxisRotateCounterClockwiseError, ex);
                }
            }
        }

        public double GetPrecisorNestPositionX()
        {
            double position = 0;

            try
            {
                position = _precisorNestXAxis.GetActualPosition();
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestXAxisReadPositionError, ex);
            }

            return position;
        }

        public double GetPrecisorNestPositionY()
        {
            double position = 0;

            try
            {
                position = _precisorNestYAxis.GetActualPosition();
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestYAxisReadPositionError, ex);
            }

            return position;
        }

        public double GetPrecisorNestPositionTheta()
        {
            double position = 0;

            try
            {
                position = _precisorNestThetaAxis.GetActualPosition();
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.PrecisorNestThetaAxisReadPositionError, ex);
            }

            return position;
        }

        public AxisFault GetThetaAxisStatus(HSTIOManifest.Axes axis)
        {

            IAxis assAxis = null;
            AxisFault status = null;

            switch (axis)
            {
                case HSTIOManifest.Axes.X:
                    assAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.X);
                    break;
                case HSTIOManifest.Axes.Y:
                    assAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Y);
                    break;
                case HSTIOManifest.Axes.Theta:
                    assAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Theta);
                    break;
                case HSTIOManifest.Axes.Z1:
                    assAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z1);
                    break;
                case HSTIOManifest.Axes.Z2:
                    assAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z2);
                    break;
                case HSTIOManifest.Axes.Z3:
                    assAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z3);
                    break;
                default:
                    break;
            }

            status = _workcell._a3200HC.GetAxisFault((Axis)assAxis);
            return status;
        }


        #region Vision
        public Camera FiducialCamera
        {
            get
            {
                return _fiducialcamera;
            }

        }
        public void VisionInspect(/*out double offset_x, out double offset_y, out double offset_theta*/)
        {
            double offset_x = 0;
            double offset_y = 0;
            double offset_theta = 0;

            if (!HSTVision.Simulation)
            {
                if (Machine.HSTMachine.Workcell.HSTSettings.Install.EnableVision)
                {
                    if (_fiducialcamera != null)
                    {
                        if (_fiducialcamera.GrabManual(true))
                        {
                            _fiducialcameraVisionApp.FiducialToolBlock(_fiducialcamera.grabImage, out offset_x, out offset_y, out offset_theta); // return true if vision tool success                                        
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fiducial Camera not initialize...");
                    }
                }
            }
        }
        private void VisionSettingsChanged(object sender, EventArgs e)
        {
            /*  if (_fiducialcameraVisionApp != null)
              {
                  _fiducialcameraVisionApp.LoadRecipe(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.Recipe, CameraLocation.PrecisorStation);
                  _fiducialcameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.ImagesOutputPath,
                                                      Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.SaveFailImages,
                                                      Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.SaveAllImages);

                  HSTVision.SetCameraExpoure(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.ExposureTime, Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.CameraSerialNumber);
        
              }*/
        }

        private void ConfigureAlignmentCamera(bool SaveImagesIfLessHGAs, bool SaveAllImages)
        {
            /* if (!HSTVision.Simulation)
             {
                 try
                 {
                     _fiducialcameraVisionApp.LoadRecipe(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.Recipe, CameraLocation.PrecisorStation);
                     _fiducialcameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.ImagesOutputPath,
                                                         SaveImagesIfLessHGAs,
                                                         SaveAllImages);
                     //   HSTVision.SetCameraExpoure(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.ExposureTime, Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.CameraSerialNumber);


                 }
                 catch (Exception ex)
                 {
                    // Notify.PopUpError("Input Camera Configuration Error.", ex);
                 }
             }*/
        }

        #endregion

        public void PrecisorAlignment(Point3D Offset)
        {
            Point2D CenterPoint = new Point2D(-10000, -10000);//in pixel//-10000 is temp, it should be replace when we have calculate the center point separately

            //Hardcode center point value
            if (_workcell.HGATailType == HGAProductTailType.LongTail)
                CenterPoint = new Point2D(-10000, -10000);//in pixel//-10000 is temp, it should be replace when we have calculate the center point separately
            else if (_workcell.HGATailType == HGAProductTailType.LongTail)
                CenterPoint = new Point2D(-10000, -10000);//in pixel//-10000 is temp, it should be replace when we have calculate the center point separately

            //rotate precisor station base of vision return theta
            MoveTAxisRelatively(Offset.Z);//used Z as theta

            //check point: the fiducial angle should be correct now.

            //Calculate point after rotate
            double ThetaInRad = ((Math.PI / 2) + Offset.Z);
            Point2D afterTurn = Offset.RotateAround(ThetaInRad, CenterPoint);

            //get the after rotate x and y offset
            double rotatedOffsetX = afterTurn.X - Offset.X;
            double rotatedOffsetY = afterTurn.Y - Offset.Y;

            //compensate back the rotatedOffsetX and rotatedOffsetY
            //convert from pixel value to world measurement
            double rotatedOffsetXInMM = 0;//rotatedOffsetX * Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.PixelSize;//temp put 0.001 as convertion 
            double rotatedOffsetYInMM = 0;//rotatedOffsetY * Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.PixelSize;

            MoveXAxisRelatively(rotatedOffsetXInMM);
            MoveYAxisRelatively(rotatedOffsetYInMM);

            //check point: the fiducial should be at the same position if compare to alignment position (except x and y offset referring to reference position).

            //compensate back the visionReturnOffsetX and visionReturnOffsetY
            double visionReturnOffsetXInMM = 0;// Offset.X * Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.PixelSize;//temp put 0.001 as convertion 
            double visionReturnOffsetYInMM = 0;//Offset.Y * Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.PixelSize;

            MoveXAxisRelatively(visionReturnOffsetXInMM);
            MoveYAxisRelatively(visionReturnOffsetYInMM);

            //presicor station should now at same position compare to reference position.
        }

        private void AutomationConfigChanged(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Acceleration > precisorNestXMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Acceleration = precisorNestXMaxAcce;
            _axesProfile.PrecisorX.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Deceleration > precisorNestXMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Deceleration = precisorNestXMaxDece;
            _axesProfile.PrecisorX.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Velocity > precisorNestXMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Velocity = precisorNestXMaxVel;
            _axesProfile.PrecisorX.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorX.Velocity;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Acceleration > precisorNestYMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Acceleration = precisorNestYMaxAcce;
            _axesProfile.PrecisorY.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Deceleration > precisorNestYMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Deceleration = precisorNestYMaxDece;
            _axesProfile.PrecisorY.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Velocity > precisorNestYMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Velocity = precisorNestYMaxVel;
            _axesProfile.PrecisorY.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorY.Velocity;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Acceleration > precisorNestThetaMaxAcce)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Acceleration = precisorNestThetaMaxAcce;
            _axesProfile.PrecisorTheta.Acceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Acceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Deceleration > precisorNestThetaMaxDece)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Deceleration = precisorNestThetaMaxDece;
            _axesProfile.PrecisorTheta.Deceleration = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Deceleration;

            if (HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Velocity > precisorNestThetaMaxVel)
                HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Velocity = precisorNestThetaMaxVel;
            _axesProfile.PrecisorTheta.Velocity = HSTMachine.Workcell.SetupSettings.MoveProfile.PrecisorTheta.Velocity;

        }

    }
}
