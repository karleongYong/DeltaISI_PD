using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.Parsel.Device.PneumaticControl;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Settings;
using System.Threading;
using System.IO;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using FisApi;
using qf4net;
using Seagate.AAS.HGA.HST.Process;
using System.Linq;
using XyratexOSC.Logging;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class OutputStationController : ControllerHST
    {
        public enum WriteSortOption
        {
            Unknown,
            DisableAll,
            SortByData,
            SortByResistanceSpec

        }

        private HSTWorkcell _workcell;
        private HSTIOManifest _ioManifest;
        private string name = "OutputStationController";
        private IDigitalInput _diOutputStationInPositionSensor;
        private IDigitalInput _diOutputTurnStationInPositionSensor;
        private IDigitalInput _diOutputTurnStationAt0DegreeSensor;

        private ReadWriteRFIDController _rfidController;
        private RFHead _rfHead;
        private SeatrackLoadData loadData = new SeatrackLoadData();
        private Models.DataLog _datalog = new Models.DataLog();

        private ReadWriteRFIDController.RFIDState _rfidOutputStationState = ReadWriteRFIDController.RFIDState.Idle;

        private ISeaveyorZone _seaveyorOutputStation;
        private SeaveyorZoneIO _seaveyorOutputStationZoneIO;

        private ISeaveyorZone _seaveyorBufferStation;
        private SeaveyorZoneIO _seaveyorBufferStationZoneIO;

        private ILinearActuator _stopper;
        private ILinearActuator _lifter;
        private ILinearActuator _clamp;
        private ILinearActuator _clampRotary;

        private IDigitalInput _diOutputStationStopperUp;
        private IDigitalInput _diOutputStationStopperDown;
        private IDigitalInput _diOutputStationClampPresent;
        private IDigitalInput _diOutputStationLifterUp;
        private IDigitalInput _diOutputStationLifterDown;
        private IDigitalInput _diOutputStationLifterBoatSense;
        private IDigitalInput _diOutputStationClampForward;
        private IDigitalInput _diOutputStationClampBackward;
        private IDigitalInput _diOutputStationClampRotateCwOpen;
        private IDigitalInput _diOutputStationClampRotateCcwClose;
        private IDigitalInput _diOutputCarrierClampOpen;

        private IDigitalOutput _doOutputStationEntranceLinkRelay;
        private IDigitalOutput _doOutputStationStopperExtendUp;
        private IDigitalOutput _doOutputStationLifterExtendUp;
        private IDigitalOutput _doOutputStationLifterRetractDown;
        private IDigitalOutput _doOutputStationClampDeploy;
        private IDigitalOutput _doOutputStationClampRotate;

        private const uint nCylinderTimeout = 5000;	//ms

        public Carrier IncomingTestedCarrier { get; set; }
        public int CarrierRunningCounter { get; set; }
        public bool SameCarrierSampling { get; set; }
        // Constructors ------------------------------------------------------------

        public OutputStationController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            _workcell = workcell;
            this._ioManifest = (HSTIOManifest)HSTMachine.Workcell.IOManifest;

            _seaveyorOutputStationZoneIO = new SeaveyorZoneIO();
            _seaveyorOutputStationZoneIO.inhibit = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.COS_Inhibit);

            _seaveyorBufferStationZoneIO = new SeaveyorZoneIO();
            _seaveyorBufferStationZoneIO.inhibit = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.BBZ_Inhibit);
            _seaveyorBufferStationZoneIO.inPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.BBZ_Position_On);

            _rfidController = new ReadWriteRFIDController(workcell, "RfidOutput", "RfidOutput", ReadWriteRFIDController.ReaderType.Fola, RFHead.Head2);
            _rfHead = Seagate.AAS.Parsel.Device.RFID.RFHead.Head2;

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
            {
                //Conveyor
                SeaveyorZone _seaveyorZone = new SeaveyorZone(_seaveyorOutputStationZoneIO);
                _seaveyorOutputStation = _seaveyorZone as ISeaveyorZone;
                _seaveyorOutputStation.Simulation = (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
                _seaveyorOutputStation.Name = "Output Station Zone";

                SeaveyorZone _bufferSeaveyorZone = new SeaveyorZone(_seaveyorBufferStationZoneIO);
                _seaveyorBufferStation = _bufferSeaveyorZone as ISeaveyorZone;
                _seaveyorBufferStation.Simulation = (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
                _seaveyorBufferStation.Name = "Buffer Station Zone";


                //digital input
                _seaveyorOutputStationZoneIO.inPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.BOS_Position_On);

                _diOutputStationInPositionSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.BOS_Position_On);
                _diOutputTurnStationInPositionSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_In_Position);
                _diOutputTurnStationAt0DegreeSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_0_Deg);
                _diOutputStationStopperUp = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Stopper_Up);
                _diOutputStationStopperDown = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Stopper_Down);
                _diOutputStationLifterUp = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Up);
                _diOutputStationLifterDown = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Down);
                _diOutputStationLifterBoatSense = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Carrier_Sense);
                _diOutputStationClampForward = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Deploy);
                _diOutputStationClampBackward = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Retract);
                _diOutputStationClampRotateCwOpen = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Lock);
                _diOutputStationClampRotateCcwClose = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Unlock);
                _diOutputCarrierClampOpen = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Carrier_Clamp_Sensor);

                //Digital Output
                _doOutputStationStopperExtendUp = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Stopper);


                _doOutputStationLifterExtendUp = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Up);
                _doOutputStationLifterRetractDown = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Down);
                _doOutputStationClampDeploy = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_CS_Deploy);
                _doOutputStationClampRotate = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_CS_Rotate);

                //1. Stopper
                LinearActuator StopperActuator = new LinearActuator(_doOutputStationStopperExtendUp, _diOutputStationStopperUp, _diOutputStationStopperDown, DigitalIOState.On);
                _stopper = StopperActuator as ILinearActuator;
                _stopper.Name = "Zone 5 stopper actuator";
                _stopper.ExtendedDirectionName = "Up";
                _stopper.ExtendedStateName = "Up";
                _stopper.RetractedDirectionName = "Down";
                _stopper.RetractedStateName = "Down";


                //2. Lifter
                LinearActuator lifterActuator = new LinearActuator(_doOutputStationLifterExtendUp, _doOutputStationLifterRetractDown, _diOutputStationLifterUp, _diOutputStationLifterDown);
                _lifter = lifterActuator as ILinearActuator;
                _lifter.Name = "Zone 5 lifter actuator";
                _lifter.ExtendedDirectionName = "Up";
                _lifter.ExtendedStateName = "Up";
                _lifter.RetractedDirectionName = "Down";
                _lifter.RetractedStateName = "Down";

                //3. Clamp
                LinearActuator ClampActuator = new LinearActuator(_doOutputStationClampDeploy, _diOutputStationClampForward, _diOutputStationClampBackward, DigitalIOState.On);
                _clamp = ClampActuator as ILinearActuator;
                _clamp.Name = "Zone 5 clamp actuator";
                _clamp.ExtendedDirectionName = "Forward";
                _clamp.ExtendedStateName = "Forward";
                _clamp.RetractedDirectionName = "Backward";
                _clamp.RetractedStateName = "Backward";

                //4. ClampRotary
                LinearActuator ClampRotaryActuator = new LinearActuator(_doOutputStationClampRotate, _diOutputStationClampRotateCcwClose, _diOutputStationClampRotateCwOpen, DigitalIOState.On);
                _clampRotary = ClampRotaryActuator as ILinearActuator;
                _clampRotary.Name = "Zone 5 clamp rotary actuator";
                _clampRotary.ExtendedDirectionName = "Cw Open";
                _clampRotary.ExtendedStateName = "Cw Open";
                _clampRotary.RetractedDirectionName = "Ccw Close";
                _clampRotary.RetractedStateName = "Ccw Close";

            }
        }

        // Properties ----------------------------------------------------------
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public RFHead RfHead
        {
            get { return _rfHead; }
        }

        public ReadWriteRFIDController RfidController
        {
            get { return _rfidController; }
            set { _rfidController = value; }
        }

        public ReadWriteRFIDController.RFIDState RfidOutputStationState
        {
            get
            {
                if (_rfidController == null)
                    return ReadWriteRFIDController.RFIDState.Idle;
                return _rfidController.State;
            }
        }

        public SeatrackLoadData GetCurrentloadData
        {
            get { return loadData; }
        }

        public ISeaveyorZone OutputSeaveyorBufferStation
        {
            get { return _seaveyorBufferStation; }
        }

        public ISeaveyorZone SeaveyorOutputStation
        {
            get { return _seaveyorOutputStation; }
        }

        public bool IsOutputStationCarrierClampOpen()
        {
            return (_diOutputCarrierClampOpen.Get() == DigitalIOState.On ? true : false);
        }

        public Carrier CurrentOutputHoldCarrier { get; set; }
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
        }

        public void InhibitOutputStation(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                _seaveyorOutputStation.Inhibit(true);
            }
            else
            {
                _seaveyorOutputStation.Inhibit(false);
            }

        }

        public void InhibitBufferStation(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                _seaveyorBufferStation.Inhibit(true);
            }
            else
            {
                _seaveyorBufferStation.Inhibit(false);
            }

        }

        public bool IsOutputStationHoldCarrier()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                if (_workcell.IsCarrierInBufferZone)
                {
                    _workcell.IsCarrierInBufferZone = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool isPosition = false;

            try
            {
                isPosition = (_diOutputStationInPositionSensor.Get() == DigitalIOState.On) ? true : false;
            }
            catch (Exception ex)
            {
                //Lai: Do nothing for now
            }
            return isPosition;
        }

        public bool IsBufferStationHoldCarrier()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                if (_workcell.IsCarrierInBufferZone)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool isPosition = false;

            try
            {
                isPosition = (_seaveyorBufferStationZoneIO.inPosition.Get() == DigitalIOState.On) ? true : false;
            }
            catch (Exception ex)
            {
                //Lai: Do nothing for now
            }
            return isPosition;
        }

        public bool IsOutputTurnStationInPositionSensorOff()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);

                if (_workcell.IsCarrierInOutputTurnTable)
                    return false;
                else
                    return true;
            }
            return (_diOutputTurnStationInPositionSensor.Get() == DigitalIOState.Off ? true : false);
        }

        public bool IsOutputTurnStationAt0Degree()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return true;
            }
            return (_diOutputTurnStationAt0DegreeSensor.Get() == DigitalIOState.Off ? false : true);
        }

        public bool IsLifterUp()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return true;
            }
            return (_lifter.State == LinearActuatorState.Extended ? false : true);

        }

        public bool IsCCCFunctional()
        {
            return _workcell.HSTSettings.CccParameterSetting.EnableAlertMsg &
                _workcell.HSTSettings.CccParameterSetting.Enabled &
                _workcell.CCCMachineTriggeringActivated;
        }

        public void WaitBufferStationPartClear()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    _diOutputStationInPositionSensor.WaitForState(DigitalIOState.On, 5000);
                    HSTMachine.Workcell.OutputStationBoatPositionError = false;
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.OutputStationBoatPositionError = true;
                    HSTException.Throw(HSTErrors.OutputStationInPositionNotOnError, ex);
                }
            }
        }

        public void WaitOutputStationPartClear()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    _diOutputStationInPositionSensor.WaitForState(DigitalIOState.Off, 5000);
                    HSTMachine.Workcell.OutputStationBoatPositionError = false;
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.OutputStationBoatPositionError = true;
                    HSTException.Throw(HSTErrors.OutputStationInPositionNotOnError, ex);
                }
            }
        }

        public void RaiseOutputLifter(bool on, out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }


            if (on)
            {
                try
                {
                    _lifter.Extend(nCylinderTimeout, out timeUsed);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationLifterExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _lifter.Retract(nCylinderTimeout, out timeUsed);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationLifterRetractError, ex);
                }
            }
            return;

        }

        public void StartMoveLifter(bool extend)
        {
            if (extend)
                _lifter.StartExtend();
            else
                _lifter.StartRetract();
        }

        public void WaitForLifterMoveDone(bool extend)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }


            if (extend)
            {
                try
                {
                    if (_lifter.RetractedSensorState == DigitalIOState.On)
                        _lifter.StartExtend();
                    _lifter.WaitForExtend(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationLifterExtendError, ex);
                }
            }
            else
            {
                try
                {
                    if (_lifter.ExtendedSensorState == DigitalIOState.On)
                        _lifter.StartRetract();
                    _lifter.WaitForRetract(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationLifterRetractError, ex);
                }
            }
        }

        public void OutputStationForwardClamp(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _clamp.Extend(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationCarrierScrewDriverExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _clamp.Retract(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationCarrierScrewDriverRetractError, ex);
                }
            }
            return;
        }

        public void OutputStationClampRotaryOpenCover(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                try
                {
                    _clampRotary.Extend(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationCarrierScrewDriverRotateClockwiseError, ex);
                }
            }
            else
            {
                try
                {
                    _clampRotary.Retract(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationCarrierScrewDriverRotateCounterClockwiseError, ex);
                }
            }

        }

        public bool IsOutputStationClampForward()
        {
            return (_diOutputStationClampForward.Get() == DigitalIOState.On ? true : false);
        }

        public bool IsOutputStationClampRotateCwOpen()
        {
            return (_diOutputStationClampRotateCwOpen.Get() == DigitalIOState.On ? true : false);
        }

        public void FreeOutputLifter()
        {
            _doOutputStationLifterExtendUp.Set(DigitalIOState.Off);
            _doOutputStationLifterRetractDown.Set(DigitalIOState.Off);
        }

        public void RaiseOutputStationStopper(bool on, out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _stopper.Extend(nCylinderTimeout, out timeUsed);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.InputStationStopperExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _stopper.Retract(nCylinderTimeout, out timeUsed);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.InputStationStopperRetractError, ex);
                }
            }
        }

        public void RaiseOutputLifter(bool on)
        {

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }


            if (on)
            {
                try
                {
                    _lifter.Extend(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationLifterExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _lifter.Retract(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationLifterRetractError, ex);
                }
            }
            return;

        }

        public void RaiseOutputStationStopper(bool on)
        {

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _stopper.Extend(nCylinderTimeout);
                    _stopper.WaitForExtend(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.InputStationStopperExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _stopper.Retract(nCylinderTimeout);
                    _stopper.WaitForRetract(nCylinderTimeout);
                }
                catch (Exception ex)
                {
                    HSTException.Throw(HSTErrors.OutputStationStopperRetractError, ex);
                }
            }
        }

        public void TurnOnWriteZoneSeaveyor()
        {
            CommonFunctions.Instance.powerOnConveyor = true;
            CommonFunctions.Instance.powerOffConveyor = false;
        }

        public void TurnOffWriteZoneSeaveyor()
        {
            CommonFunctions.Instance.powerOnConveyor = false;
            CommonFunctions.Instance.powerOffConveyor = true;
        }

        #region data handling

        /// <summary>
        /// Write data to RFID as option 1
        /// </summary>
        /// <param name="carrier"></param>
        public void PackageRfidOutputStationData(Carrier carrier, WriteSortOption sortOption)
        {
            int trckingstep = 0;

            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "In OutputStationController's PackageRfidOutputStationData, Carrier ID:{0}, " +
                                "HGA1:{1}, HGA2:{2}, HGA3:{3}, HGA4:{4}, HGA5:{5}, " +
                                "HGA6:{6},  HGA7:{7}, HGA8:{8}, HGA9:{9}, HGA10:{10}", carrier.CarrierID,
                                carrier.Hga1.Hga_Status, carrier.Hga2.Hga_Status, carrier.Hga3.Hga_Status, carrier.Hga4.Hga_Status, carrier.Hga5.Hga_Status,
                                carrier.Hga6.Hga_Status, carrier.Hga7.Hga_Status, carrier.Hga8.Hga_Status, carrier.Hga9.Hga_Status, carrier.Hga10.Hga_Status);
                }

                //FIFO, always get the oldest object in the carrier list
                lock (CommonFunctions.Instance.InputCarriersLock)
                {
                    _rfidController.FolaTagDataWriteInfor = carrier.RFIDData.RFIDTagData;
                    _rfidController.FolaTagDataWriteInfor.CarrierID = carrier.RFIDData.RFIDTagData.CarrierID;
                    _rfidController.FolaTagDataWriteInfor.WorkOrder = carrier.RFIDData.RFIDTagData.WorkOrder;
                    _rfidController.FolaTagDataWriteInfor.WorkOrderVersion = carrier.RFIDData.RFIDTagData.WorkOrderVersion;
                    _rfidController.FolaTagDataWriteInfor.WriteCount = carrier.RFIDData.RFIDTagData.WriteCount; // will be plus 1 in library

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "In OutputStationController's PackageRfidOutputStationData the RFID tag to be written: Carrier:{0}", _rfidController.FolaTagDataWriteInfor.CarrierID);
                        Log.Info(this, "Current Process Recipe: {0}", _rfidController.FolaTagDataWriteInfor.CurrentProcessStep.ProcessRecipe);
                        Log.Info(this, "WorkOrder: {0}", _rfidController.FolaTagDataWriteInfor.WorkOrder);
                        Log.Info(this, "Work Order Version: {0}", _rfidController.FolaTagDataWriteInfor.WorkOrderVersion.ToString());
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.RFIDUpdateOption == RFIDUpdateOption.UpdateOnlyHGAStatus)
                    {
                        _rfidController.FolaTagDataWriteInfor.LastStep = carrier.RFIDData.RFIDTagData.LastStep;
                    }
                    else
                    {
                        _rfidController.FolaTagDataWriteInfor.LastStep = carrier.RFIDData.RFIDTagData.LastStep + 1;
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Last Step: {0}", _rfidController.FolaTagDataWriteInfor.LastStep);
                    }

                    trckingstep++;
                    //Check to assign sampling case running in option 2 and 3
                    //UpdateOnly process step 

                    SameCarrierSampling = false;
                    //Check sampling condition with part spec
                    CheckToSamplingCondition(carrier);

                    //Check sampling condition with assinging period
                    CheckToSamplingByPeriod(carrier);

                    trckingstep++;

                    for (int slot = 0; slot < carrier.RFIDData.RFIDTagData.CarrierSize; slot++)
                    {
                        //Assigned hga running slot
                        var hga = new Hga(0, HGAStatus.Unknown);
                        switch (slot)
                        {
                            case 0:
                                hga = carrier.Hga1;
                                break;
                            case 1:
                                hga = carrier.Hga2;
                                break;
                            case 2:
                                hga = carrier.Hga3;
                                break;
                            case 3:
                                hga = carrier.Hga4;
                                break;
                            case 4:
                                hga = carrier.Hga5;
                                break;
                            case 5:
                                hga = carrier.Hga6;
                                break;
                            case 6:
                                hga = carrier.Hga7;
                                break;
                            case 7:
                                hga = carrier.Hga8;
                                break;
                            case 8:
                                hga = carrier.Hga9;
                                break;
                            case 9:
                                hga = carrier.Hga10;
                                break;
                        }
                        //6-apr if update only processstep
                        if (HSTMachine.Workcell.HSTSettings.Install.RFIDUpdateOption != RFIDUpdateOption.UpdateOnlyProcessStep)
                        {
                            if (_rfidController.FolaTagDataWriteInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                            {
                                ///// CCC Check if flag has been enabled
                                //if (_workcell.HSTSettings.CccParameterSetting.Enabled)
                                //    CheckCCCResult(hga);

                                trckingstep++;
                                //Assign hga status by sort option
                                //Option1
                                if (sortOption == WriteSortOption.DisableAll || sortOption == WriteSortOption.Unknown)
                                {
                                    if (hga.Hga_Status == HGAStatus.TestedPass)
                                    {
                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.TEST_PASS_CODE;
                                    }
                                    else
                                    {
                                        if (HSTMachine.Workcell.HSTSettings.Install.RFIDUpdateOption == RFIDUpdateOption.UpdateALL)
                                        {
                                            if (hga.Hga_Status != HGAStatus.NoHGAPresent)
                                            {
                                                if (CheckAssignedToSampling(hga.Error_Msg_Code))
                                                {
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.TEST_PASS_CODE;
                                                    if (hga.Hga_Status != HGAStatus.TestedPass) hga.Hga_Status = HGAStatus.TestedPass;
                                                }
                                                else
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                            }
                                            else
                                            {
                                                if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.SKIP_TEST;
                                                else
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Update RFID with replace HGA status by sort
                                //Option2
                                else if (sortOption == WriteSortOption.SortByData)
                                {
                                    trckingstep++;
                                    if (hga.Hga_Status == HGAStatus.TestedPass)
                                    {
                                        if (hga.Hga_Status != HGAStatus.NoHGAPresent)
                                        {
                                            if (hga.TST_STATUS.Equals('\0') || hga.TST_STATUS.Equals(string.Empty) || hga.TST_STATUS.Equals('0'))
                                            {
                                                var reloadSort = ReloadTSTstatus(_rfidController.FolaTagDataReadInfor.HGAData[slot].HgaSN);
                                                if (reloadSort != null)
                                                {
                                                    if (reloadSort.TST_STATUS.Equals('\0') || reloadSort.TST_STATUS.Equals(string.Empty) || !reloadSort.TST_STATUS.Equals('0'))
                                                        hga.TST_STATUS = Convert.ToChar(reloadSort.TST_STATUS);
                                                }
                                            }

                                            if (hga.ForceToSampling)
                                            {
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.TEST_PASS_CODE;
                                                hga.Hga_Status = HGAStatus.TestedPass;
                                                Log.Info(this, "Assigned to sampling: Date=,{0}, SN=,{1}, Status=,{2}, Code=,{3}, Total Sampling=,{4}, Part Counter=,{5}", DateTime.Now.ToString(),
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].HgaSN, CommonFunctions.TEST_PASS_CODE.ToString(),
                                                    hga.Error_Msg_Code, HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter.ToString(),
                                                    HSTMachine.Workcell.LoadCounter.ProcessedHGACount.ToString());
                                            }
                                            else if (hga.ForceToRiskCode)
                                            {
                                                //3-Sep-2020 change STFamily from 'E' RISK_OF_BRIDGE_CODE to 'V'  
                                                if ((CommonFunctions.Instance.IsRunningWithNewTSR) && 
                                                    (hga.Error_Msg_Code == ERROR_MESSAGE_CODE.STFAMILY.ToString()) && 
                                                    (CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec != 40))
                                                {
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                                    hga.Hga_Status = HGAStatus.TestedFail;
                                                }
                                                else
                                                {
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.RISK_OF_BRIDGE_CODE;
                                                    hga.Hga_Status = HGAStatus.TestedPass;
                                                }
                                                
                                            }
                                            else if (hga.ForceToPolarityRiskSamplingDeltaR2)
                                            {
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.RISK_OF_BRIDGE_CODE;
                                                hga.Hga_Status = HGAStatus.TestedPass;
                                                hga.Error_Msg_Code = ERROR_MESSAGE_CODE.R2DELTA.ToString();
                                            }
                                            else if (hga.ForceToPolarityRiskSamplingDeltaR1)
                                            {
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.RISK_OF_BRIDGE_CODE;
                                                hga.Hga_Status = HGAStatus.TestedPass;
                                                hga.Error_Msg_Code = ERROR_MESSAGE_CODE.R1DELTA.ToString();
                                            }
                                            else
                                            {
                                                if (!hga.TST_STATUS.Equals('\0') && !hga.TST_STATUS.Equals(string.Empty) && !hga.TST_STATUS.Equals('0'))
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = hga.TST_STATUS;
                                                else
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                            }
                                        }
                                        else
                                        {
                                            if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.SKIP_TEST;
                                            else
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                        }
                                    }
                                    else
                                    {
                                        if (hga.Hga_Status != HGAStatus.NoHGAPresent)
                                        {
                                            if (IsLDUFailureCondition(hga.Error_Msg_Code))
                                            {
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.LDU_FAIL_SHORT;
                                            }
                                            else
                                            {
                                                if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                            }

                                        }
                                        else
                                        {
                                            if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.SKIP_TEST;
                                        }
                                    }

                                }
                                #endregion
                                #region Update RFID with replace HGA status by sort and order by resistance spec
                                //Option3
                                else if (sortOption == WriteSortOption.SortByResistanceSpec)
                                {
                                    trckingstep++;
                                    if (hga.Hga_Status == HGAStatus.TestedPass)
                                    {
                                        if (hga.Hga_Status != HGAStatus.NoHGAPresent)
                                        {

                                            if (!hga.TST_STATUS.Equals('\0') && !hga.TST_STATUS.Equals(string.Empty) && !hga.TST_STATUS.Equals('0'))
                                            {
                                                if (hga.ForceToSampling)
                                                {
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.TEST_PASS_CODE;
                                                    hga.Hga_Status = HGAStatus.TestedPass;
                                                    Log.Info(this, "Assigned to sampling: Date,{0}, SN,{1}, Status,{2}", DateTime.Now.ToString(),
                                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].HgaSN, CommonFunctions.TEST_PASS_CODE.ToString());
                                                }
                                                else if (hga.ForceToRiskCode)
                                                {
                                                    if ((CommonFunctions.Instance.IsRunningWithNewTSR) &&
                                                        (hga.Error_Msg_Code == ERROR_MESSAGE_CODE.STFAMILY.ToString()) &&
                                                        (CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec != 40))
                                                    {
                                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                                        hga.Hga_Status = HGAStatus.TestedFail;
                                                    }
                                                    else
                                                    {
                                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.RISK_OF_BRIDGE_CODE;
                                                        hga.Hga_Status = HGAStatus.TestedPass;

                                                    }
                                                }
                                                else if (hga.ForceToPolarityRiskSamplingDeltaR2)
                                                {
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.RISK_OF_BRIDGE_CODE;
                                                    hga.Hga_Status = HGAStatus.TestedPass;
                                                    hga.Error_Msg_Code = ERROR_MESSAGE_CODE.R2DELTA.ToString();
                                                    HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter++;
                                                }
                                                else if (hga.ForceToPolarityRiskSamplingDeltaR1)
                                                {
                                                    _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.RISK_OF_BRIDGE_CODE;
                                                    hga.Hga_Status = HGAStatus.TestedPass;
                                                    hga.Error_Msg_Code = ERROR_MESSAGE_CODE.R1DELTA.ToString();
                                                    HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter++;
                                                }
                                                else
                                                {
                                                    if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.SKIP_TEST;
                                                    else
                                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                                }
                                            }
                                            else
                                            {
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                            }
                                        }
                                        else
                                            _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;
                                    }
                                    else
                                    {
                                        if (hga.Hga_Status != HGAStatus.NoHGAPresent)
                                        {
                                            if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.HST_STATION_CODE;

                                            if (hga.Error_Msg_Code == LDUFailureType.Spec.ToString())
                                                hga.TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                            else if (hga.Error_Msg_Code == LDUFailureType.OpenShort.ToString())
                                                hga.TST_STATUS = CommonFunctions.LDU_FAIL_SHORT;
                                        }
                                        else
                                        {
                                            if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                                                _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = CommonFunctions.SKIP_TEST;
                                        }
                                    }
                                }
                                #endregion

                                //Mar-2020 override ERROR_CODE by FISManager data from SLDR_BIN
                                var getSDLR_BIN = ReloadTSTstatus(_rfidController.FolaTagDataReadInfor.HGAData[slot].HgaSN).SLDR_BIN.ToString();
                                switch (getSDLR_BIN)
                                {
                                    case "1":
                                        //hga.TST_STATUS = 'A';
                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = 'A';
                                        hga.Error_Msg_Code = "FORCE_A";
                                        break;
                                    case "2":
                                        // hga.TST_STATUS = 'E';
                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = 'E';
                                        hga.Error_Msg_Code = "FORCE_E";
                                        break;
                                    case "3":
                                        //   hga.TST_STATUS = 'V';
                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = 'V';
                                        hga.Error_Msg_Code = "FORCE_V";
                                        break;
                                    case "4":
                                        //   hga.TST_STATUS = '0';
                                        _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status = '0';
                                        hga.Error_Msg_Code = "FORCE_0";
                                        break;
                                }
                            }

                        }
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            Log.Info(this, "HGA1 Serial Number: {0}, Status: {1}", _rfidController.FolaTagDataWriteInfor.HGAData[slot].HgaSN, _rfidController.FolaTagDataWriteInfor.HGAData[slot].Status.ToString());
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Registed value"))
                    Log.Info(this, "Tracking error <Registed value was not found>: {0}", trckingstep.ToString());
                HSTException.Throw(HSTErrors.OutputRFIDWriteError, ex);
            }
        }

        public Boolean AllHGAsFailed(Carrier outputCarrier)
        {
            for (int i = 0; i < 10; i++)
            {
                if (outputCarrier.RFIDData.RFIDTagData.HGAData[i].Status == CommonFunctions.TEST_PASS_CODE)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Write data to RFID as option 1
        /// </summary>
        /// <param name="carrier"></param>
        public void DoCheckCCCResult(Carrier carrier)
        {
            int trckingstep = 0;

            try
            {
                //FIFO, always get the oldest object in the carrier list
                lock (CommonFunctions.Instance.InputCarriersLock)
                {
                    for (int slot = 0; slot < carrier.RFIDData.RFIDTagData.CarrierSize; slot++)
                    {
                        //Assigned hga running slot
                        var hga = new Hga(0, HGAStatus.Unknown);
                        switch (slot)
                        {
                            case 0:
                                hga = carrier.Hga1;
                                break;
                            case 1:
                                hga = carrier.Hga2;
                                break;
                            case 2:
                                hga = carrier.Hga3;
                                break;
                            case 3:
                                hga = carrier.Hga4;
                                break;
                            case 4:
                                hga = carrier.Hga5;
                                break;
                            case 5:
                                hga = carrier.Hga6;
                                break;
                            case 6:
                                hga = carrier.Hga7;
                                break;
                            case 7:
                                hga = carrier.Hga8;
                                break;
                            case 8:
                                hga = carrier.Hga9;
                                break;
                            case 9:
                                hga = carrier.Hga10;
                                break;
                        }

                        //ANC should be used only part that incomming status pass
                        if (_rfidController.FolaTagDataReadInfor.HGAData[slot].Status == CommonFunctions.TEST_PASS_CODE)
                        {
                            CheckCCCResult(hga);
                            if (hga.UTIC_DATA.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1)
                            {
                                decimal PartCounter = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
                                decimal totalParts = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.AdaptivePartCounter.ToString());

                                decimal currentYield = 0;
                                var totalcount = (totalParts - PartCounter) * 100;
                                try
                                {
                                    if ((totalcount != 0 || totalParts != 0) && (totalcount > 100))
                                        currentYield = decimal.Divide(totalcount, totalParts);
                                }
                                catch (Exception)
                                {
                                }
                                hga.set_ANC_YIELD(Math.Round(double.Parse(currentYield.ToString()), 2));
                                hga.set_ANC_HGA_Count(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter);
                                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield = double.Parse(currentYield.ToString());
                            }
                            else if (hga.UTIC_DATA.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2)
                            {
                                decimal PartCounter = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
                                decimal totalParts = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.AdaptivePartCounter.ToString());

                                decimal currentYield = 0;
                                var totalcount = (totalParts - PartCounter) * 100;
                                try
                                {
                                    if ((totalcount != 0 || totalParts != 0) && (totalcount > 100))
                                        currentYield = decimal.Divide(totalcount, totalParts);
                                }
                                catch (Exception)
                                {
                                }
                                hga.set_ANC_YIELD(Math.Round(double.Parse(currentYield.ToString()), 2));
                                hga.set_ANC_HGA_Count(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter);
                                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.current_yield = double.Parse(currentYield.ToString());
                            }
                            else
                            {
                                hga.set_ANC_YIELD(0);
                                hga.set_ANC_HGA_Count(0);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Registed value"))
                    Log.Info(this, "Tracking error <Registed value was not found>: {0}", trckingstep.ToString());
            }

        }

        /// <summary>
        /// Check CCC result base on hga status
        /// </summary>
        /// <param name="currentHga"></param>
        public void CheckCCCResult(Hga currentHga)
        {
            _workcell.GraphAndMcMapping.Graph1McName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
            _workcell.GraphAndMcMapping.Graph2McName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;
            //for test 
            if ((_workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter + _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter) == 99)
            {

            }
            //25-Mar-2020 
            if (HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.LastSaveLogTime < System.DateTime.Today)
            {
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering = 0;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering = 0;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicDefactCounter = 0;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicDefactCounter = 0;
                ////   HSTMachine.Workcell.HSTSettings

                HSTMachine.Workcell.getPanelSetup().ANCReset();
                HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.LastSaveLogTime = System.DateTime.Today;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastSaveLogTime = System.DateTime.Today;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastSaveLogTime = System.DateTime.Today;

            }



            // Check condition before update CCC
            bool isIgnoreThisPart = false;
            var currentErrCode = currentHga.Error_Msg_Code;

            DateTime triggerTime;
            DateTime getTicTime;

            currentHga.UTIC_DATA.ERROR_CODE = currentHga.Error_Msg_Code;
            //For run

            getTicTime = Convert.ToDateTime(currentHga.UTIC_DATA.EVENT_DATE);
            if (currentHga.UTIC_DATA.EVENT_DATE == null)
            {
                getTicTime = System.DateTime.Now;
               
            }

           // getTicTime = DateTime.Now; // for test only

            triggerTime = DateTime.Now;

             var totalTime = (getTicTime - HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertActiveTime).TotalMinutes;


            if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.IsUnderCCCAlert || HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.IsUnderCCCAlert)
            {
                if ((totalTime > 1))
                {

                    HSTMachine.Workcell.CurretCCCActiveStatus.ClearActiveStatus();
                        if (currentHga.UTIC_DATA.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1)
                            {
                                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.IsUnderCCCAlert = false;
                            }
                        if (currentHga.UTIC_DATA.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2)
                            {
                                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.IsUnderCCCAlert = false;
                            }

                }
                else
                {
                        if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.IsUnderCCCAlert)
                            {
                                isIgnoreThisPart = true;
                            }
                        else if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.IsUnderCCCAlert)
                            {
                                isIgnoreThisPart = true;
                            }

                }
                    
            }

            if (isIgnoreThisPart)
            {
                if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.uTICMachineName == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1)
                {
                    HSTMachine.Workcell.getPanelData().SaveLogDataIgnorePartToLogfile("PartIgnored (after CCCAlert)", currentHga.UTIC_DATA.ERROR_CODE, currentHga.UTIC_DATA.EQUIP_ID, getTicTime);

                }
                else if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.uTICMachineName == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2)
                {
                    HSTMachine.Workcell.getPanelData().SaveLogDataIgnorePartToLogfile("PartIgnored (after CCCAlert)", currentHga.UTIC_DATA.ERROR_CODE, currentHga.UTIC_DATA.EQUIP_ID, getTicTime);

                }
                else
                {
                    isIgnoreThisPart = true;
                    HSTMachine.Workcell.getPanelData().SaveLogDataIgnorePartToLogfile("PartIgnored (not in CCCAlert) ", currentHga.UTIC_DATA.ERROR_CODE, currentHga.UTIC_DATA.EQUIP_ID, getTicTime);
                }


            }
            else
            {

                _workcell.TICCccControl.CCCControlAllMc.UpdatePartRunCounter();
                

                if (currentHga.Hga_Status != HGAStatus.TestedPass)
                {
                    if ((currentHga.Error_Msg_Code != string.Empty) && (currentHga.Error_Msg_Code != "ERROR_MSG_CODE"))
                    {
                        if (IsTICDefectDetected(currentHga.Error_Msg_Code))
                        {
                            if (!string.IsNullOrEmpty(currentHga.UTIC_DATA.EQUIP_ID))
                            {
                                if (!isIgnoreThisPart)
                                {

                                    if (string.IsNullOrEmpty(_workcell.GraphAndMcMapping.Graph1McName) && (currentHga.UTIC_DATA.EQUIP_ID != _workcell.GraphAndMcMapping.Graph2McName))
                                    {
                                        _workcell.AncUticMcCounter++;
                                        _workcell.TICCccControl.CCCControlTicMc1.CccResult.LastUTICDate = getTicTime;
                                        _workcell.GraphAndMcMapping.Graph1McName = currentHga.UTIC_DATA.EQUIP_ID;
                                        _workcell.TicMcFailureCounter.UpdateFailure(currentHga.UTIC_DATA.EQUIP_ID, currentHga.Error_Msg_Code);
                                        _workcell.CCCFailureInfo.FailDockNumber = currentHga.UTIC_DATA.DOCK_SIDE;
                                    }
                                    if (string.IsNullOrEmpty(_workcell.GraphAndMcMapping.Graph2McName) && (currentHga.UTIC_DATA.EQUIP_ID != _workcell.GraphAndMcMapping.Graph1McName))
                                    {
                                        _workcell.AncUticMcCounter++;
                                        _workcell.TICCccControl.CCCControlTicMc2.CccResult.LastUTICDate = getTicTime;
                                        _workcell.GraphAndMcMapping.Graph2McName = currentHga.UTIC_DATA.EQUIP_ID;
                                        _workcell.TicMcFailureCounter.UpdateFailure(currentHga.UTIC_DATA.EQUIP_ID, currentHga.Error_Msg_Code);
                                        _workcell.CCCFailureInfo.FailDockNumber = currentHga.UTIC_DATA.DOCK_SIDE;
                                    }

                                    if ((!string.IsNullOrEmpty(_workcell.GraphAndMcMapping.Graph1McName) && (currentHga.UTIC_DATA.EQUIP_ID != _workcell.GraphAndMcMapping.Graph2McName)) &&
                                        (!string.IsNullOrEmpty(_workcell.GraphAndMcMapping.Graph2McName) && (currentHga.UTIC_DATA.EQUIP_ID != _workcell.GraphAndMcMapping.Graph1McName)))
                                    {

                                        _workcell.AncUticMcCounter++;
                                    }

                                }

                                if (_workcell.AncUticMcCounter > 2)
                                {
                                }

                                if (string.Equals(currentHga.UTIC_DATA.EQUIP_ID, _workcell.GraphAndMcMapping.Graph1McName))
                                {

                                    _workcell.CCCFailureInfo.FailDockNumber = currentHga.UTIC_DATA.DOCK_SIDE;

                                    _workcell.TICCccControl.CCCControlTicMc1.UpdatePartRunCounter();
                                    _workcell.TICCccControl.CCCControlTicMc1.UpdateGoodCounter();
                                    if (currentHga.UTIC_DATA.EVENT_DATE != null)
                                    {
                                        _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastTICAlertActiveTime = getTicTime;
                                    }
                                    else
                                    {
                                        _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastTICAlertActiveTime = System.DateTime.Now;
                                    }                                   
                                }

                                if (string.Equals(currentHga.UTIC_DATA.EQUIP_ID, _workcell.GraphAndMcMapping.Graph2McName))
                                {
                                    _workcell.CCCFailureInfo.FailDockNumber = currentHga.UTIC_DATA.DOCK_SIDE;

                                    _workcell.TICCccControl.CCCControlTicMc2.UpdatePartRunCounter();
                                    _workcell.TICCccControl.CCCControlTicMc2.UpdateGoodCounter();
                                    if (currentHga.UTIC_DATA.EVENT_DATE != null)
                                    {
                                        _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastTICAlertActiveTime = getTicTime;
                                    }
                                    else
                                    {
                                        _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastTICAlertActiveTime = System.DateTime.Now;
                                    }
                                }
                            }
                            if (IncomingTestedCarrier == null)  // for test function (because CCC has null carrier)
                            {
                                IncomingTestedCarrier = new Carrier();
                                IncomingTestedCarrier.CarrierID = "AAA";
                                _workcell.CCCFailureInfo.CarrierId = IncomingTestedCarrier.CarrierID;
                            }
                            else
                            {
                                _workcell.CCCFailureInfo.CarrierId = IncomingTestedCarrier.CarrierID;
                                
                            }

                            if (!isIgnoreThisPart)
                            {
                                var runResult = new CCCRunResult
                                {
                                    AdaptiveNumber = _workcell.TICCccControl.CCCControlAllMc.CccResult.AdaptiveNumber,
                                    HighTICPercentTriggeringCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter
                                            .TicHighPercentTriggeringCounter,
                                    MCDownTriggering =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.MCDownTriggering,
                                    StdLcl = _workcell.TICCccControl.CCCControlAllMc.CccResult.StdLcl,
                                    AdaptiveDefectCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.LastAdaptiveDefectCounter,
                                    GoodCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.LastRunGoodCounter,
                                    DefactCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicFailCounter,
                                    InternalDefectCounter = _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.InternalTriggerCounter,
                                    CrdlTriggeringCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstDefactCounter,
                                    TICTriggeringCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicDefactCounter,
                                    HighCrdlPercentTriggeringCounter =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter
                                            .HstHighPercentTriggeringCounter,
                                    YieldTest = _workcell.TICCccControl.CCCControlAllMc.CccResult.YieldTest,
                                    CurrentTriggerMc =
                                        _workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.McTriggerType.ToString(),
                                    CurrentUTICTime = Convert.ToDateTime(currentHga.UTIC_DATA.EVENT_DATE)
                                };

                                _workcell.RaiseCCCRunPartUpdated(runResult, _workcell.TICCccControl.CCCControlAllMc.CccResult.CCCDataLogger, CCCOutput.Trigger_Type.TIC, currentHga.UTIC_DATA);
                            }
                        }
                    }
                }
                else //if no TIC or CRDL
                {
                    _workcell.TICCccControl.CCCControlAllMc.UpdateGoodCounter();

                    if (!string.IsNullOrEmpty(currentHga.UTIC_DATA.EQUIP_ID))
                    {
                        if (string.IsNullOrEmpty(_workcell.GraphAndMcMapping.Graph1McName) && (currentHga.UTIC_DATA.EQUIP_ID != _workcell.GraphAndMcMapping.Graph2McName))
                            _workcell.GraphAndMcMapping.Graph1McName = currentHga.UTIC_DATA.EQUIP_ID;
                        if (string.IsNullOrEmpty(_workcell.GraphAndMcMapping.Graph2McName) && (currentHga.UTIC_DATA.EQUIP_ID != _workcell.GraphAndMcMapping.Graph1McName))
                            _workcell.GraphAndMcMapping.Graph2McName = currentHga.UTIC_DATA.EQUIP_ID;
                    }

                    if (string.Equals(currentHga.UTIC_DATA.EQUIP_ID, _workcell.GraphAndMcMapping.Graph1McName))
                    {
                        _workcell.TICCccControl.CCCControlTicMc1.UpdatePartRunCounter();
                        _workcell.TICCccControl.CCCControlTicMc1.UpdateGoodCounter();
                    }

                    if (string.Equals(currentHga.UTIC_DATA.EQUIP_ID, _workcell.GraphAndMcMapping.Graph2McName))
                    {
                        _workcell.TICCccControl.CCCControlTicMc2.UpdatePartRunCounter();
                        _workcell.TICCccControl.CCCControlTicMc2.UpdateGoodCounter();
                    }
                }

            } // Loop IsIgnorePart underCCCAlert
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "DoCCCCheck : EQUIPID {0}, AdaptivePartRun : MC1 = {1}, MC2 ={2}", currentHga.UTIC_DATA.EQUIP_ID, _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter, _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter);
            }
            _workcell.HSTSettings.saveCCC();
        }

        /// <summary>
        /// Calculate and assign sampling ET on disk
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        private bool[] AssignedSamplingSlot(Carrier carrier)
        {
            bool[] assignSamplingSlot = new bool[10];
            for (int i = 0; i < 10; i++)
            {
                assignSamplingSlot[i] = false;
            }

            for (int i = 0; i < CommonFunctions.Instance.ConfigurationSetupRecipe.CounterSamplingPerCarrier; i++)
            {
                if (carrier.Hga1.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[0] = true;
                else if (carrier.Hga2.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[1] = true;
                else if (carrier.Hga3.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[2] = true;
                else if (carrier.Hga4.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[3] = true;
                else if (carrier.Hga5.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[4] = true;
                else if (carrier.Hga6.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[5] = true;
                else if (carrier.Hga7.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[6] = true;
                else if (carrier.Hga8.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[7] = true;
                else if (carrier.Hga9.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[8] = true;
                else if (carrier.Hga10.Hga_Status == HGAStatus.TestedPass && assignSamplingSlot[i] == false)
                    assignSamplingSlot[9] = true;

            }

            return assignSamplingSlot;
        }

        /// <summary>
        /// To get current running sort that got from SDET by ISI
        /// </summary>
        /// <param name="currentCarrier"></param>
        /// <param name="hgaNumber"></param>
        /// <returns></returns>
        public char GetHGATSRGradingSort(Carrier currentCarrier, int hgaNumber)
        {
            char assignSort = CommonFunctions.TEST_PASS_CODE;
            var currentTSRSortGrading = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList;
            var hgaData = new Hga(1, HGAStatus.Unknown);

            switch (hgaNumber)
            {
                case 1:
                    hgaData = currentCarrier.Hga1;
                    break;
                case 2:
                    hgaData = currentCarrier.Hga2;
                    break;
                case 3:
                    hgaData = currentCarrier.Hga3;
                    break;
                case 4:
                    hgaData = currentCarrier.Hga4;
                    break;
                case 5:
                    hgaData = currentCarrier.Hga5;
                    break;
                case 6:
                    hgaData = currentCarrier.Hga6;
                    break;
                case 7:
                    hgaData = currentCarrier.Hga7;
                    break;
                case 8:
                    hgaData = currentCarrier.Hga8;
                    break;
                case 9:
                    hgaData = currentCarrier.Hga9;
                    break;
                case 10:
                    hgaData = currentCarrier.Hga10;
                    break;
            }


            SortGrading[] sortGradingItem = new SortGrading[CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.SortCount];
            double getReading = 0;
            int currentSortNo = 0;

            for (int i = 0; i < CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.ResistanceTopicCount; i++)
            {
                switch (i)
                {
                    case 0:
                        sortGradingItem = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.Writer;
                        getReading = hgaData.getWriterResistance();
                        break;
                    case 1:
                        sortGradingItem = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.TA;
                        getReading = hgaData.getTAResistance();
                        break;
                    case 2:
                        sortGradingItem = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.wHeater;
                        getReading = hgaData.getWHeaterResistance();
                        break;
                    case 3:
                        sortGradingItem = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.rHeater;
                        getReading = hgaData.getRHeaterResistance();
                        break;
                    case 4:
                        sortGradingItem = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.Reader1;
                        getReading = hgaData.getReader1Resistance();
                        break;
                    case 5:
                        sortGradingItem = CommonFunctions.Instance.MeasurementTestRecipe.SortGradingsList.Reader2;
                        getReading = hgaData.getReader2Resistance();
                        break;

                }


                for (int iSort = 0; iSort < currentTSRSortGrading.SortCount; iSort++)
                {
                    var getMinTsrSortSpect = sortGradingItem[iSort].MinSpec;
                    var getMaxTsrSortSpect = sortGradingItem[iSort].MaxSpec;

                    if (getMinTsrSortSpect != 0 && getMaxTsrSortSpect != 0)
                        if ((getReading > getMinTsrSortSpect) && (getReading < getMaxTsrSortSpect))
                        {
                            if ((iSort + 1) > currentSortNo)
                            {
                                var sort = (iSort + 1).ToString();
                                assignSort = char.Parse(sort);
                                currentSortNo = iSort + 1;
                            }
                        }
                }

            }

            return assignSort;
        }

        public void ClearRFReadInfo()
        {
            _rfidController.FolaTagDataReadInfor.Clear();
        }

        public void ReadRfid()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true) ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                return;
            }
            else
            {
                int retryCnt = 1;
                while (retryCnt > 0)
                {
                    try
                    {
                        retryCnt--;
                        _rfidController.ReadFolaRFID(RFHead.Head2);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (retryCnt > 0)
                            Thread.Sleep(100);
                        else
                            throw ex;
                    }
                }
            }
        }

        public void WriteRfid(RFHead hd, Carrier carrier)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true) ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(0);
                return;
            }
            else
            {

                CarrierRunningCounter++;

                var runningSortOption = WriteSortOption.Unknown;
                //Check sort option
                if (CommonFunctions.Instance.MeasurementTestRecipe.DisableAll)
                    runningSortOption = WriteSortOption.DisableAll;
                else if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData)
                    runningSortOption = WriteSortOption.SortByData;
                else if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                    runningSortOption = WriteSortOption.SortByResistanceSpec;

                //Update RFID data
                PackageRfidOutputStationData(carrier, runningSortOption);

                if (_workcell.HSTSettings.CccParameterSetting.Enabled)
                {
                    CurrentOutputHoldCarrier = carrier;
                    DoCheckCCCResult(carrier);
                }

                //Write RFID to TAG
                if (HSTMachine.Workcell.HSTSettings.Install.RFIDUpdateOption != RFIDUpdateOption.DoNotUpdate)
                    _rfidController.WriteFolaRFID();
            }
        }

        public void UpdateDataLogForManualMeasurementTest(Carrier manuallyLoadedCarrier)
        {
            _datalog = new Models.DataLog();

            if (manuallyLoadedCarrier == null)
            {
                return;
            }
            try
            {
                _datalog.MachineLocation = HSTMachine.Workcell.HSTSettings.Install.LocationID;
                _datalog.EquipmentID = HSTMachine.Workcell.HSTSettings.Install.EquipmentID;
                _datalog.EquipmentType = HSTMachine.Workcell.HSTSettings.EquipmentType;
                _datalog.LoginUser = HSTMachine.Workcell.HSTSettings.OperatorGID;
                _datalog.WorkOrder = "Manual";
                _datalog.WorkOrderVersion = HSTMachine.Workcell.WorkOrder.Loading.Version;
                _datalog.SetupFileName = HSTMachine.Workcell.WorkOrder.Loading.LoadingProgramName;
                _datalog.SoftwareStatus = SoftwareStatus.Stop;
                //ykl
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    _datalog.XAxis = HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX();
                    _datalog.YAxis = HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionY();
                    _datalog.ZAxis = HSTMachine.Workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ();
                    _datalog.theta = HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionTheta();
                }
                else
                {
                    _datalog.XAxis = 100;// HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX();
                    _datalog.YAxis = 100; //HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionY();
                    _datalog.ZAxis = 100;// HSTMachine.Workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ();
                    _datalog.theta = 100;// HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionTheta();
                }
                //ykl
                for (int i = 0; i < 10; i++)
                {

                    _datalog.CarrierID = "Manual";
                    _datalog.CarrierSlot = i + 1;
                    _datalog.HGASerialNumber = "Manual";
                    _datalog.CycleTime = 0;
                    _datalog.UPH = 0;
                    string ErrorMessageCode = "";

                    if (i == 0)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga1.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga1.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;

                    }
                    else if (i == 1)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga2.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga2.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 2)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga3.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga3.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 3)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga4.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga4.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 4)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga5.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga5.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 5)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga6.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga6.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 6)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga7.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga7.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 7)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga8.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga8.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 8)
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga9.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga9.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else
                    {
                        _datalog.HGAStatus = manuallyLoadedCarrier.Hga10.Hga_Status;
                        ErrorMessageCode = manuallyLoadedCarrier.Hga10.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }

                    if (i == 0)  // HAG1
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga1.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga1.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga1.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga1.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga1.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga1.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga1.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga1.getTAResistance();

                        if (manuallyLoadedCarrier.Hga1.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga1.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga1.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga1.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga1.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga1.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga1.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga1.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga1.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga1.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga1.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga1.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga1.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga1.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga1.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga1.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga1.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga1.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga1.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga1.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga1.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga1.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga1.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga1.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga1.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga1.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga1.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga1.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga1.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga1.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga1.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga1.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga1.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga1.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga1.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga1.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga1.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga1.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga1.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga1.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga1.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga1.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 1)  // HGA2
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga2.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga2.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga2.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga2.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga2.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga2.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga2.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga2.getTAResistance();

                        if (manuallyLoadedCarrier.Hga2.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga2.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga2.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga2.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga2.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga2.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga2.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga2.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga2.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga2.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga2.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga2.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga2.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga2.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga2.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga2.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga2.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga2.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga2.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga2.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga2.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga2.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga2.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga2.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga2.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga2.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga2.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga2.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga2.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga2.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga2.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga2.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga2.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga2.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga2.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga2.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga2.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga2.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga2.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga2.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga2.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga2.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 2)  // HGA3
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga3.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga3.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga3.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga3.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga3.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga3.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga3.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga3.getTAResistance();

                        if (manuallyLoadedCarrier.Hga3.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga3.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga3.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga3.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga3.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga3.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga3.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga3.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga3.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga3.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga3.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga3.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga3.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga3.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga3.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga3.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga3.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga3.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga3.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga3.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga3.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga3.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga3.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga3.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga3.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga3.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga3.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga3.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga3.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga3.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga3.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga3.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga3.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga3.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga3.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga3.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga3.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga3.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga3.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga3.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga3.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga3.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 3)  // HGA4
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga4.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga4.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga4.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga4.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga4.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga4.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga4.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga4.getTAResistance();

                        if (manuallyLoadedCarrier.Hga4.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga4.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga4.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga4.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga4.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga4.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga4.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga4.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga4.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga4.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga4.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga4.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga4.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga4.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga4.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga4.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga4.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga4.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga4.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga4.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga4.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga4.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga4.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga4.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga4.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga4.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga4.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga4.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga4.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga4.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga4.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga4.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga4.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga4.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga4.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga4.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga4.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga4.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga4.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga4.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga4.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga4.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 4)  // HGA5
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga5.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga5.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga5.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga5.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga5.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga5.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga5.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga5.getTAResistance();

                        if (manuallyLoadedCarrier.Hga5.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga5.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga5.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga5.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga5.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga5.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga5.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga5.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga5.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga5.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga5.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga5.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga5.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga5.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga5.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga5.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga5.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga5.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga5.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga5.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga5.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga5.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga5.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga5.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga5.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga5.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga5.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga5.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga5.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga5.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga5.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga5.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga5.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga5.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga5.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga5.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga5.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga5.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga5.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga5.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga5.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga5.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 5)  // HGA6
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga6.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga6.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga6.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga6.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga6.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga6.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga6.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga6.getTAResistance();

                        if (manuallyLoadedCarrier.Hga6.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga6.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga6.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga6.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga6.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga6.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga6.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga6.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga6.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga6.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga6.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga6.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga6.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga6.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga6.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga6.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga6.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga6.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga6.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga6.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga6.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga6.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga6.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga6.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga6.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga6.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga6.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga6.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga6.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga6.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga6.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga6.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga6.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga6.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga6.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga6.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga6.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga6.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga6.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga6.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga6.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga6.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 6)  // HGA7
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga7.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga7.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga7.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga7.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga7.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga7.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga7.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga7.getTAResistance();

                        if (manuallyLoadedCarrier.Hga7.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga7.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga7.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga7.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga7.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga7.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga7.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga7.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga7.getCh3Temperature();


                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga7.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga7.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga7.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga7.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga7.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga7.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga7.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga7.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga7.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga7.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga7.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga7.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga7.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga7.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga7.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga7.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga7.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga7.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga7.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga7.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga7.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga7.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga7.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga7.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga7.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga7.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga7.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga7.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga7.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga7.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga7.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga7.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga7.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 7)  // HGA8
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga8.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga8.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga8.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga8.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga8.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga8.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga8.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga8.getTAResistance();

                        if (manuallyLoadedCarrier.Hga8.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga8.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga8.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga8.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga8.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga8.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga8.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga8.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga8.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga8.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga8.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga8.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga8.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga8.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga8.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga8.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga8.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga8.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga8.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga8.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga8.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga8.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga8.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga8.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga8.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga8.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga8.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga8.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga8.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga8.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga8.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga8.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga8.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga8.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga8.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga8.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga8.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga8.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga8.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga8.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga8.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga8.UTIC_DATA.EVENT_DATE;
                    }
                    else if (i == 8)  // HGA9
                    {
                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga9.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga9.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga9.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga9.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga9.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga9.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga9.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga9.getTAResistance();

                        if (manuallyLoadedCarrier.Hga9.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga9.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga9.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga9.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga9.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga9.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga9.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga9.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga9.getCh3Temperature();

                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga9.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga9.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga9.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga9.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga9.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga9.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga9.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga9.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga9.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga9.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga9.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga9.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga9.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga9.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga9.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga9.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga9.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga9.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga9.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga9.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga9.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga9.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga9.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga9.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga9.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga9.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga9.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga9.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga9.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga9.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga9.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga9.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga9.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 9)  // HGA10
                    {



                        _datalog.ReaderResistance = manuallyLoadedCarrier.Hga10.getReader1Resistance();
                        _datalog.Reader2Resistance = manuallyLoadedCarrier.Hga10.getReader2Resistance();
                        _datalog.DeltaISIReader1 = manuallyLoadedCarrier.Hga10.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = manuallyLoadedCarrier.Hga10.getDeltaISIReader2();
                        _datalog.WriterResistance = manuallyLoadedCarrier.Hga10.getWriterResistance();
                        _datalog.rHeaterResistance = manuallyLoadedCarrier.Hga10.getRHeaterResistance();
                        _datalog.wHeaterResistance = manuallyLoadedCarrier.Hga10.getWHeaterResistance();
                        _datalog.TAResistance = manuallyLoadedCarrier.Hga10.getTAResistance();

                        if (manuallyLoadedCarrier.Hga10.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (manuallyLoadedCarrier.Hga10.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = manuallyLoadedCarrier.Hga10.getShortPadPosition();
                        _datalog.BiasVoltage = manuallyLoadedCarrier.Hga10.getBiasVoltage();
                        _datalog.BiasCurrent = manuallyLoadedCarrier.Hga10.getBiasCurrent();
                        _datalog.ResistanceSpec = manuallyLoadedCarrier.Hga10.getResistanceSpec();
                        _datalog.ThermisterTemperature = manuallyLoadedCarrier.Hga10.getCh1Temperature() + "_" + manuallyLoadedCarrier.Hga10.getCh2Temperature() + "_" + manuallyLoadedCarrier.Hga10.getCh3Temperature();


                        _datalog.DeltaISIReader1FromTable = manuallyLoadedCarrier.Hga10.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = manuallyLoadedCarrier.Hga10.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = manuallyLoadedCarrier.Hga10.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = manuallyLoadedCarrier.Hga10.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? manuallyLoadedCarrier.Hga10.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = manuallyLoadedCarrier.Hga10.getLEDIntercept();
                        _datalog.i_threshold = manuallyLoadedCarrier.Hga10.getIThreshold();
                        _datalog.max_v_pd = manuallyLoadedCarrier.Hga10.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = manuallyLoadedCarrier.Hga10.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = manuallyLoadedCarrier.Hga10.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = manuallyLoadedCarrier.Hga10.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga10.Get_Bias_Ch1()) / manuallyLoadedCarrier.Hga10.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga10.Get_Bias_Ch2()) / manuallyLoadedCarrier.Hga10.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga10.Get_Bias_Ch3()) / manuallyLoadedCarrier.Hga10.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga10.Get_Bias_Ch4()) / manuallyLoadedCarrier.Hga10.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga10.Get_Bias_Ch5()) / manuallyLoadedCarrier.Hga10.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(manuallyLoadedCarrier.Hga10.Get_Bias_Ch6()) / manuallyLoadedCarrier.Hga10.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = manuallyLoadedCarrier.Hga10.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = manuallyLoadedCarrier.Hga10.get_sdet_writer();
                        _datalog.Wrbridge_Pct = manuallyLoadedCarrier.Hga10.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = manuallyLoadedCarrier.Hga10.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = manuallyLoadedCarrier.Hga10.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = manuallyLoadedCarrier.Hga10.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = manuallyLoadedCarrier.Hga10.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = manuallyLoadedCarrier.Hga10.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = manuallyLoadedCarrier.Hga10.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = manuallyLoadedCarrier.Hga10.UTIC_DATA.EVENT_DATE;

                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                     /*   Log.Info(this, "In UpdateDataLog, Carrier ID: {0}, Slot: {1}, Serial Number: {2}, , HGA Status: {3}, Reader1 Resist: {4}, Reader2 Resist: {18}, DeltaISI Reader1: {19}, DeltaISI Reader2: {20}, Writer Resist: {5}, rHeater Resist: {6}, wHeater Resist: {7}, TA Resist: {8}, Short: {9}, Capa1: {10}, Capa2: {11},Bias Voltage: {12}, Bias Current: {13}, Temperaturee: {14}, Resist Spec: {15}, Capa1 Spec: {16}, Capa2 Spec: {17}",
                            _datalog.CarrierID, _datalog.CarrierSlot, _datalog.HGASerialNumber, _datalog.HGAStatus, _datalog.ReaderResistance, _datalog.WriterResistance,
                            _datalog.rHeaterResistance, _datalog.wHeaterResistance, _datalog.TAResistance, _datalog.ShortTest, _datalog.MicroActuatorCapacitance1, _datalog.MicroActuatorCapacitance2, _datalog.BiasVoltage,
                            _datalog.BiasCurrent, _datalog.ThermisterTemperature, _datalog.ResistanceSpec, _datalog.CapacitanceSpec1, _datalog.CapacitanceSpec2, _datalog.Reader2Resistance, _datalog.DeltaISIReader1, _datalog.DeltaISIReader2);
*/

                        Log.Info(this, "Output Station In UpdateDataLog, Carrier ID: {0}, Slot: {1}, Serial Number: {2}, , HGA Status: {3}, Reader1 Resist: {4}, Reader2 Resist: {18}, DeltaISI Reader1: {19}, DeltaISI Reader2: {20}, Writer Resist: {5}, rHeater Resist: {6}, wHeater Resist: {7}, TA Resist: {8}, Short: {9}, Capa1: {10}, Capa2: {11},Bias Voltage: {12}, Bias Current: {13}, Temperaturee: {14}, Resist Spec: {15}, Capa1 Spec: {16}, Capa2 Spec: {17}, LDU_RES: {21}, LDU_SPEC: {22}",
                           _datalog.CarrierID, _datalog.CarrierSlot, _datalog.HGASerialNumber, _datalog.HGAStatus, _datalog.ReaderResistance, _datalog.WriterResistance,
                           _datalog.rHeaterResistance, _datalog.wHeaterResistance, _datalog.TAResistance, _datalog.ShortTest, _datalog.MicroActuatorCapacitance1, _datalog.MicroActuatorCapacitance2, _datalog.BiasVoltage,
                           _datalog.BiasCurrent, _datalog.ThermisterTemperature, _datalog.ResistanceSpec, _datalog.CapacitanceSpec1, _datalog.CapacitanceSpec2, _datalog.Reader2Resistance, _datalog.DeltaISIReader1, _datalog.DeltaISIReader2, _datalog.LDURes, _datalog.LDUSpec);


                        Log.Info(this, "LED_intercept: {0}, I_Threshold: {1}, Max_V_PD: {2}", _datalog.led_intercept, _datalog.i_threshold, _datalog.max_v_pd);
                    }

                    UIUtility.Invoke(HSTMachine.Workcell.getPanelData(), () =>
                    {
                        _workcell.csvFileOutput.AppendNewCSVRecord(_datalog);
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Update data error from Output Station processs", ex);
            }
            return;
        }

        /// <summary>
        /// Check all conditions to assign part status to sampling
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public Carrier CheckToSamplingCondition(Carrier carrier)
        {
            Carrier returnResult = new Carrier();
            Hga hga = null;
            bool assignedToRiskCode = false;

            //Check to reset counter without carrier every day
            var diffDay = HSTMachine.Workcell.LoadCounter.LastSamplingResetDate - DateTime.Now;
            if (Math.Abs(diffDay.Days) > 0)
            {
                HSTMachine.Workcell.LoadCounter.Reset();
            }

            string workOrderNo = carrier.WorkOrderData.WorkOrderNo;
            char lastChar = workOrderNo[workOrderNo.Length - 1];
            double writerImpledanceSpec = 0.00;

            if (lastChar.Equals('U'))
            {
                writerImpledanceSpec = CommonFunctions.Instance.MeasurementTestRecipe.WriterResistanceSpecUP;
            }
            else if (lastChar.Equals('D'))
            {
                writerImpledanceSpec = CommonFunctions.Instance.MeasurementTestRecipe.WriterResistanceSpecDN;
            }

            //Writer Check
            for (int i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 0:
                        hga = carrier.Hga1;
                        break;
                    case 1:
                        hga = carrier.Hga2;
                        break;
                    case 2:
                        hga = carrier.Hga3;
                        break;
                    case 3:
                        hga = carrier.Hga4;
                        break;
                    case 4:
                        hga = carrier.Hga5;
                        break;
                    case 5:
                        hga = carrier.Hga6;
                        break;
                    case 6:
                        hga = carrier.Hga7;
                        break;
                    case 7:
                        hga = carrier.Hga8;
                        break;
                    case 8:
                        hga = carrier.Hga9;
                        break;
                    case 9:
                        hga = carrier.Hga10;
                        break;
                }

                //4-Feb-2020  adaptive spec impledatance Spec
                if (HSTMachine.Workcell.HSTSettings.Sampledata.getLastestWoName != carrier.WorkOrderData.FsaPartNo)
                {
                    HSTMachine.Workcell.HSTSettings.Sampledata.setWOName(carrier.WorkOrderData.FsaPartNo);
                    HSTMachine.Workcell.HSTSettings.Sampledata.reset();
                }

                //Increase WRBridge part run counter
                if ((hga.Hga_Status == HGAStatus.TestedPass) || (hga.Hga_Status == HGAStatus.TestedFail))
                    HSTMachine.Workcell.LoadCounter.WriterBridgePartRunCounter++;
                HSTMachine.Workcell.HSTSettings.Sampledata.getTotalTest = HSTMachine.Workcell.HSTSettings.Sampledata.getTotalTest + 1;

                hga.Set_Delta_Writer_Sdet(hga.DeltaISIResistanceWriter);

                //Priority-1: Assign writer bridging to sampling
                if (writerImpledanceSpec > 0)
                {
                    var getImpledantSpec = GetWRBridgeImpledantSpec(false, writerImpledanceSpec);
                    var diffCompared = (hga.getWriterResistance() - hga.DeltaISIResistanceWriter);

                    hga.Set_Delta_WR_Hst_Sdet(diffCompared);

                    if (HSTMachine.Workcell.HSTSettings.Sampledata.getCount > 100)
                    {
                        HSTMachine.Workcell.HSTSettings.Sampledata.Append(diffCompared);
                        HSTMachine.Workcell.HSTSettings.Sampledata.Calculate();
                        writerImpledanceSpec = HSTMachine.Workcell.HSTSettings.Sampledata.getCurrentZvalue;
                        getImpledantSpec = HSTMachine.Workcell.HSTSettings.Sampledata.getCurrentZvalue;
                    }
                    else
                    {
                        HSTMachine.Workcell.HSTSettings.Sampledata.Append(diffCompared);
                    }
                    HSTMachine.Workcell.HSTSettings.Sampledata.Save();
                    // end of adaptive spec

                    if (diffCompared < getImpledantSpec)
                    {
                        if (diffCompared <= 0.5)
                        {
                            if (hga.Hga_Status == HGAStatus.TestedPass)
                            {
                                hga.Error_Msg_Code = ERROR_MESSAGE_CODE.WRFAIL.ToString();
                                HSTMachine.Workcell.LoadCounter.WRBridgeFailureCounter++;
                                HSTMachine.Workcell.HSTSettings.Sampledata.getTotalWRBridge++;
                                assignedToRiskCode = true;
                            }
                        }
                        else
                        {
                            if (hga.Hga_Status == HGAStatus.TestedPass)
                            {
                                HSTMachine.Workcell.LoadCounter.WRBridgeFailureCounter++;
                                HSTMachine.Workcell.HSTSettings.Sampledata.getTotalWRBridge++;
                                assignedToRiskCode = true;
                                hga.Error_Msg_Code = ERROR_MESSAGE_CODE.WRBRIDGE.ToString();
                            }
                        }
                    }

                    hga.Set_Delta_WR_Hst_Sdet(diffCompared);
                }

                if (hga.Error_Msg_Code == "Empty") hga.Error_Msg_Code = string.Empty;
                //Priority-2: Assign spacific error code to sampling
                var getBoardShortTest = hga.getShortTest();
                if (CheckErrorCodeForSampling(hga.Error_Msg_Code, getBoardShortTest))
                {
                    HSTMachine.Workcell.LoadCounter.WRBridgeFailureCounter++;
                    assignedToRiskCode = true;
                    hga.Set_WRBridge_Percentage(GetWRBridgeFailurePercentage());
                    hga.Error_Msg_Code = ERROR_MESSAGE_CODE.STFAMILY.ToString();
                }
                hga.Set_WRBridge_Percentage(GetWRBridgeWriterFailurePercentage());
                hga.Set_wrbridge_adap_spec(writerImpledanceSpec);

                var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (hga.TST_STATUS.Equals('\0') || hga.TST_STATUS.Equals(string.Empty) || hga.TST_STATUS.Equals('0'));

                if (assignedToRiskCode && !isFailGetSort)
                {
                    hga.Hga_Status = HGAStatus.TestedPass;
                    hga.ForceToRiskCode = true;
                    assignedToRiskCode = false;
                }

                SameCarrierSampling = true;
            }

            return returnResult;
        }


        private double GetWRBridgeFailurePercentage()
        {
            double returnVal = 0.0;
            double totalPercentage = 0.0;

            if (HSTMachine.Workcell.LoadCounter.WriterBridgePartRunCounter >= ProductionCounterData.WRBridgeMinimumPartCounter)
            {
                totalPercentage = (((double)HSTMachine.Workcell.LoadCounter.WRBridgeFailureCounter /
                    (double)HSTMachine.Workcell.LoadCounter.WriterBridgePartRunCounter) * 100);

                if (double.IsNaN(totalPercentage) || double.IsInfinity(totalPercentage)) totalPercentage = 0;
                HSTMachine.Workcell.LoadCounter.LastWRBridgePercentage = totalPercentage;
                returnVal = totalPercentage;
            }

            return returnVal;
        }

        private double GetWRBridgeWriterFailurePercentage()
        {
            double returnVal = 0.0;
            double totalPercentage = 0.0;

            if ((HSTMachine.Workcell.HSTSettings.Sampledata.getTotalTest > 0) && (HSTMachine.Workcell.HSTSettings.Sampledata.getCount > 100))
            {
                totalPercentage = (HSTMachine.Workcell.HSTSettings.Sampledata.getTotalWRBridge / HSTMachine.Workcell.HSTSettings.Sampledata.getTotalTest) * 100;
            }
            else
            {
                totalPercentage = 0;
            }
            returnVal = totalPercentage;
            return returnVal;
        }

        public double GetWRBridgeImpledantSpec(bool isEnableAdaptive, double recipeSpec)
        {
            double impledantSpec = 0.0;
            double totalPercentage = GetWRBridgeFailurePercentage();

            if (isEnableAdaptive)
            {
                if (_workcell.WRBridgeDecreaseStepList[1] == 0)
                    CalculateWRBridgeDecreaseStep(recipeSpec);

                var getAssignSpecIndex = GetAssignSpecIndex(totalPercentage);
                impledantSpec = _workcell.WRBridgeDecreaseStepList[getAssignSpecIndex];

                if (impledantSpec > HSTMachine.Workcell.LoadCounter.GetWRBridgeMaxSpect)
                    impledantSpec = HSTMachine.Workcell.LoadCounter.GetWRBridgeMaxSpect;
                if (impledantSpec < HSTMachine.Workcell.LoadCounter.GetWRBridgeLowwerLimitSpec)
                    impledantSpec = HSTMachine.Workcell.LoadCounter.GetWRBridgeLowwerLimitSpec;
            }
            else
            {
                impledantSpec = recipeSpec;
            }

            return impledantSpec;
        }

        public void CalculateWRBridgeDecreaseStep(double recipeSpec)
        {
            double sumStep = 0.0;
            int maxlimitpercent = (int)(HSTMachine.Workcell.LoadCounter.GetWRBridgeMaxSpect * 10);
            _workcell.WRBridgeDecreaseStepList = new double[maxlimitpercent + 1];
            var specLength = recipeSpec - HSTMachine.Workcell.LoadCounter.GetWRBridgeLowwerLimitSpec;

            var increasestep = specLength / maxlimitpercent;

            for (int i = 0; i < _workcell.WRBridgeDecreaseStepList.Length; i++)
            {
                if (i == 0)
                {
                    _workcell.WRBridgeDecreaseStepList[i] = recipeSpec;
                }
                else
                {
                    sumStep += increasestep;
                    _workcell.WRBridgeDecreaseStepList[i] = recipeSpec - sumStep;
                }
            }
        }

        public int GetAssignSpecIndex(double runningSampling)
        {
            int returnIndex = 0;
            double firstSpec = 0.0;
            double secondSpec = 0.0;
            if (runningSampling > _workcell.LoadCounter.GetWRBridgeMaxSpect)
                runningSampling = _workcell.LoadCounter.GetWRBridgeMaxSpect;
            for (int i = 0; i < _workcell.WRBridgeDecreaseStepList.Length; i++)
            {
                if (i == 0)
                {
                    firstSpec = 0;
                    secondSpec = 0.1;
                }
                else
                {
                    firstSpec = i * 0.1;
                    secondSpec = (i + 1) * 0.1;
                }

                if ((runningSampling >= firstSpec) && (runningSampling < secondSpec))
                {
                    returnIndex = i;
                    break;
                }
            }

            return returnIndex;
        }

        /// <summary>
        /// Check to assign sampling by carrier period
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public Carrier CheckToSamplingByPeriod(Carrier carrier)
        {
            Carrier returnResult = new Carrier();
            Hga hga = null;
            bool isAlreadyToSampling = false;

            for (int i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 0:
                        hga = carrier.Hga1;
                        break;
                    case 1:
                        hga = carrier.Hga2;
                        break;
                    case 2:
                        hga = carrier.Hga3;
                        break;
                    case 3:
                        hga = carrier.Hga4;
                        break;
                    case 4:
                        hga = carrier.Hga5;
                        break;
                    case 5:
                        hga = carrier.Hga6;
                        break;
                    case 6:
                        hga = carrier.Hga7;
                        break;
                    case 7:
                        hga = carrier.Hga8;
                        break;
                    case 8:
                        hga = carrier.Hga9;
                        break;
                    case 9:
                        hga = carrier.Hga10;
                        break;
                }

                if (hga.ForceToSampling || hga.ForceToRiskCode || !(string.IsNullOrEmpty(hga.Error_Msg_Code)) || hga.Error_Msg_Code_Set_Flag)
                    isAlreadyToSampling = true;
            }

            ////Priority-3: Assign carrier counter period time to sampling
            if (!isAlreadyToSampling)
            {
                var partCountPerSampling = CommonFunctions.Instance.ConfigurationSetupRecipe.CarrierCounterToSampling * 10;
                if (partCountPerSampling != 0)
                    if ((HSTMachine.Workcell.LoadCounter.ProcessedHGACount - HSTMachine.Workcell.LoadCounter.LastActiveSamplingPartCount) >=
                        partCountPerSampling)
                    {
                        if (HSTMachine.Workcell.LoadCounter.GetCurrentSamplingNumber <= CommonFunctions.Instance.MeasurementTestRecipe.SamplingETOnDisk)
                        {
                            if (_workcell.LastSamplingSlotNumber > 9)
                                _workcell.LastSamplingSlotNumber = 0;

                            for (int i = 0; i < 10; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        hga = carrier.Hga1;
                                        break;
                                    case 1:
                                        hga = carrier.Hga2;
                                        break;
                                    case 2:
                                        hga = carrier.Hga3;
                                        break;
                                    case 3:
                                        hga = carrier.Hga4;
                                        break;
                                    case 4:
                                        hga = carrier.Hga5;
                                        break;
                                    case 5:
                                        hga = carrier.Hga6;
                                        break;
                                    case 6:
                                        hga = carrier.Hga7;
                                        break;
                                    case 7:
                                        hga = carrier.Hga8;
                                        break;
                                    case 8:
                                        hga = carrier.Hga9;
                                        break;
                                    case 9:
                                        hga = carrier.Hga10;
                                        break;
                                }

                                if ((hga.Hga_Status != HGAStatus.TestedPass) && (i == _workcell.LastSamplingSlotNumber))
                                {
                                    _workcell.LastSamplingSlotNumber++;
                                }

                                var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (hga.TST_STATUS.Equals('\0') || hga.TST_STATUS.Equals(string.Empty) || hga.TST_STATUS.Equals('0'));
                                if (i == _workcell.LastSamplingSlotNumber && !isFailGetSort)
                                {
                                    if ((HSTMachine.Workcell.LoadCounter.ProcessedHGACount - HSTMachine.Workcell.LoadCounter.LastActiveSamplingPartCount) !=
                                            partCountPerSampling)
                                    {
                                        var diffCounter = (HSTMachine.Workcell.LoadCounter.ProcessedHGACount -
                                            HSTMachine.Workcell.LoadCounter.LastActiveSamplingPartCount) - partCountPerSampling;

                                        HSTMachine.Workcell.LoadCounter.LastActiveSamplingPartCount = (HSTMachine.Workcell.LoadCounter.ProcessedHGACount -
                                            diffCounter);
                                    }
                                    else
                                    {
                                        HSTMachine.Workcell.LoadCounter.LastActiveSamplingPartCount = HSTMachine.Workcell.LoadCounter.ProcessedHGACount;
                                    }

                                    hga.Error_Msg_Code = ERROR_MESSAGE_CODE.ONDISK.ToString();
                                    hga.Hga_Status = HGAStatus.TestedPass;
                                    hga.ForceToSampling = true;
                                    HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter++;

                                    HSTMachine.Workcell.LoadCounter.SetCurrentSamplingNumber();
                                    if (HSTMachine.Workcell.LoadCounter.GetCurrentSamplingNumber > CommonFunctions.Instance.MeasurementTestRecipe.SamplingETOnDisk + 1)
                                    {
                                        Log.Info(this, "Warning sampling counter number out of limit");
                                    }
                                }
                            }
                            _workcell.LastSamplingSlotNumber++;
                        }
                        else
                        {
                            //Re-calculate sampling % again
                            HSTMachine.Workcell.LoadCounter.SetCurrentSamplingNumber();
                        }

                    }

            }

            return returnResult;
        }


        /// <summary>
        /// Check to assign error code list to sampling
        /// </summary>
        /// <param name="errcode"></param>
        /// <returns></returns>
        public bool CheckErrorCodeForSampling(string errcode, ShortDetection BoardShortTestDetection)
        {
            bool returnResult = false;
            var currentErrCode = ERROR_MESSAGE_CODE.Unknown;
            try
            {
                if (errcode != string.Empty)
                    currentErrCode = (ERROR_MESSAGE_CODE)Enum.Parse(typeof(ERROR_MESSAGE_CODE), errcode);
                if (currentErrCode == ERROR_MESSAGE_CODE.R1ST ||
                    currentErrCode == ERROR_MESSAGE_CODE.R2ST ||
                    currentErrCode == ERROR_MESSAGE_CODE.R2R1ST ||
                    currentErrCode == ERROR_MESSAGE_CODE.ADJST ||
                    currentErrCode == ERROR_MESSAGE_CODE.R1HRST ||
                    currentErrCode == ERROR_MESSAGE_CODE.HRH2ST ||
                    currentErrCode == ERROR_MESSAGE_CODE.CISL ||
                    currentErrCode == ERROR_MESSAGE_CODE.TAR2ST ||
                    currentErrCode == ERROR_MESSAGE_CODE.WST ||
                    currentErrCode == ERROR_MESSAGE_CODE.WHST ||
                    currentErrCode == ERROR_MESSAGE_CODE.RHST ||
                    currentErrCode == ERROR_MESSAGE_CODE.TAST)
                {
                    returnResult = true;
                }
                else
                {
                    if (currentErrCode == ERROR_MESSAGE_CODE.Unknown)
                        if (BoardShortTestDetection == ShortDetection.Short)
                            returnResult = true;
                }

            }
            catch (Exception)
            {
            }
            return returnResult;
        }

        public bool CheckAssignedToSampling(string errCode)
        {
            bool returnResult = false;
            try
            {
                if (errCode != string.Empty)
                {
                    var currentErrCode = (ERROR_MESSAGE_CODE)Enum.Parse(typeof(ERROR_MESSAGE_CODE), errCode);
                    if (//currentErrCode == ERROR_MESSAGE_CODE.R1ST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.R2ST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.R2R1ST ||    //Move STFAMILY to be fail
                        //currentErrCode == ERROR_MESSAGE_CODE.ADJST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.R1HRST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.HRH2ST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.CISL ||
                        //currentErrCode == ERROR_MESSAGE_CODE.TAR2ST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.WST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.WHST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.RHST ||
                        //currentErrCode == ERROR_MESSAGE_CODE.TAST ||
                        currentErrCode == ERROR_MESSAGE_CODE.WRBRIDGE ||
                        //currentErrCode == ERROR_MESSAGE_CODE.STFAMILY ||
                        currentErrCode == ERROR_MESSAGE_CODE.R2DELTA)
                    {
                        Log.Info(this, "Changing Short Test Result");
                        returnResult = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return returnResult;
        }

        public bool IsTICDefectDetected(string errcode)
        {
            bool returnResult = false;
            try
            {
                if (errcode != string.Empty)
                {
                    var currentErrCode = (ERROR_MESSAGE_CODE)Enum.Parse(typeof(ERROR_MESSAGE_CODE), errcode);
                    if (currentErrCode == ERROR_MESSAGE_CODE.RCS ||
                        currentErrCode == ERROR_MESSAGE_CODE.RCO ||
                        currentErrCode == ERROR_MESSAGE_CODE.CS2 ||
                        currentErrCode == ERROR_MESSAGE_CODE.CO2 ||
                        currentErrCode == ERROR_MESSAGE_CODE.WRS ||
                        currentErrCode == ERROR_MESSAGE_CODE.WRO ||
                        currentErrCode == ERROR_MESSAGE_CODE.HRS ||
                        currentErrCode == ERROR_MESSAGE_CODE.HRO ||
                        currentErrCode == ERROR_MESSAGE_CODE.H2S ||
                        currentErrCode == ERROR_MESSAGE_CODE.H2O ||
                        currentErrCode == ERROR_MESSAGE_CODE.TAS ||
                        currentErrCode == ERROR_MESSAGE_CODE.TAO ||
                        currentErrCode == ERROR_MESSAGE_CODE.CRDR ||
                        currentErrCode == ERROR_MESSAGE_CODE.CWRR ||
                        currentErrCode == ERROR_MESSAGE_CODE.CHTR ||
                        currentErrCode == ERROR_MESSAGE_CODE.CHT2 ||
                        currentErrCode == ERROR_MESSAGE_CODE.CTAR)
                    {
                        returnResult = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return returnResult;
        }

        private SLDR_PARAM_BIN_DATA ReloadTSTstatus(string hga_sn)
        {
            var sldr_param_bin = new SLDR_PARAM_BIN_DATA();
            try
            {
                var getData = FISManager.Instance.GetSLDR_PARAM_BIN_Data(sldr_param_bin, hga_sn);
                if (sldr_param_bin.TST_STATUS == string.Empty)
                    sldr_param_bin = null;
            }
            catch (Exception ex)
            {
            }
            return sldr_param_bin;
        }

        private bool IsLDUFailureCondition(string err)
        {
            bool result = false;
            if (err == ERROR_MESSAGE_CODE.ITHRESFAIL.ToString() || err == ERROR_MESSAGE_CODE.LDURESFAIL.ToString() || err == ERROR_MESSAGE_CODE.LEDINTCFAIL.ToString() ||
                err == ERROR_MESSAGE_CODE.VPDFAIL.ToString() || err == ERROR_MESSAGE_CODE.LDURESFAIL.ToString() || err == ERROR_MESSAGE_CODE.DELTAITHRESFAIL.ToString())
                result = true;

            return result;
        }
    }
}
