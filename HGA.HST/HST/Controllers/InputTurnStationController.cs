using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.Parsel.Device.TurnSection;
using Seagate.AAS.HGA.HST.Exceptions;
using System.Threading;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class InputTurnStationController : ControllerHST
    {

        private SeaveyorZoneIO _seaveyorInputConveyorIO;
        private SeaveyorZoneIO _seaveyorInputStationIO;
        private ISeaveyorZone _seaveyorInputConveyor;
        private ISeaveyorZone _seaveyorInputStation;
        private string name = "InputTurnStationController";

        internal TurnSectionIO inputTurnSectionIO;
        internal ITurnSection inputTurnSection;

        private IDigitalOutput _doInputTurnTableTurnTo0Deg;
        private IDigitalOutput _doInputTurnTableTurnTo90Deg;
        private IDigitalOutput _doInputLifterUp;
        private IDigitalOutput _doInputLifterDown;

        private IDigitalOutput _doInputStopper;

        private const uint _carrierInPositionTimeout = 3000;
        private const uint _zoneClearTimeout = 3000;
        private const uint nCylinderTimeout = 5000;	//ms
        private const uint nSensorTimeout = 100;	//ms
        #region Simulation time
        private const int nTurningTime = 300;	//ms
        private const int _boatExchangeTime = 1200;	//ms
        private const int nGetCarrierTime = 800;	//ms
        private const int nReleaseCarrierTime = 1200;	//ms
        #endregion


        // Constructors ------------------------------------------------------------
        public InputTurnStationController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            this._workcell = workcell;
            this._ioManifest = (HSTIOManifest)workcell.IOManifest;
            _seaveyorInputConveyorIO = new SeaveyorZoneIO();
            _seaveyorInputStationIO = new SeaveyorZoneIO();
            inputTurnSectionIO = new TurnSectionIO();


            ///// Input Turn Station/////////////

            //digital input
            inputTurnSectionIO.inPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Turn_Station_In_Position);
            inputTurnSectionIO.At90DegSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_90_Deg);
            inputTurnSectionIO.At0DegSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_0_Deg);

            //digital output
            inputTurnSectionIO.turnTo90Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Turn_Station_Turn_To_90_Deg);
            inputTurnSectionIO.turnTo0Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Turn_Station_Turn_To_0_Deg);
            inputTurnSectionIO.inhibitRelay = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Turn_Station_Inhibit);

            _doInputTurnTableTurnTo0Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Turn_Station_Turn_To_0_Deg);
            _doInputTurnTableTurnTo90Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Turn_Station_Turn_To_90_Deg);

            _doInputLifterUp = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Up);
            _doInputLifterDown = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Down);

            _doInputStopper = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Stopper);
    
            TurnSection TS = new TurnSection(inputTurnSectionIO);
            inputTurnSection = TS as Seagate.AAS.Parsel.Device.ITurnSection;
            inputTurnSection.Simulation = (workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            inputTurnSection.Name = "InputTurnSection";


            ///// Input Conveyor Seaveyor/////////////

            //digital input
            _seaveyorInputConveyorIO.inPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Conveyor_Position_On);

            //digital output
            _seaveyorInputConveyorIO.inhibit = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Conveyor_Inhibit);

            SeaveyorZone _seaveyorZone = new SeaveyorZone(_seaveyorInputConveyorIO);
            _seaveyorInputConveyor = _seaveyorZone as ISeaveyorZone;
            _seaveyorInputConveyor.Simulation = (workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            _seaveyorInputConveyor.Name = "InputConveyor";



            ///// Input Station Seaveyor/////////////
            //digital input
            _seaveyorInputStationIO.inPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.BIS_Position_On);

            //digital output
            _seaveyorInputStationIO.inhibit = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.CIS_Inhibit);


            _seaveyorZone = new SeaveyorZone(_seaveyorInputStationIO);
            _seaveyorInputStation = _seaveyorZone as ISeaveyorZone;
            _seaveyorInputStation.Simulation = (workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            _seaveyorInputStation.Name = "InputStation";
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
            }
            catch (Exception ex)
            {
                throw CreateControllerException(1, ex);
            }
        }

        public void WaitInputTurnStationPartCleared()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(10 * CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                int autoRetry = 2;

                do
                {
                    try
                    {
                        inputTurnSectionIO.inPosition.WaitForState(DigitalIOState.Off, 5000);
                        HSTMachine.Workcell.InputTurnStationBoatPositionError = false;
                        autoRetry = 0;
                    }
                    catch (Exception ex)
                    {
                        if(autoRetry <= 0)
                        {
                            HSTMachine.Workcell.InputTurnStationBoatPositionError = true;
                            HSTException.Throw(HSTErrors.InputTurnStationInPositionNotOffError, ex);
                        }
                        else
                        {
                            autoRetry--;
                        }
                    }
                } while (autoRetry > 0);

            }
        }

        public bool IsInputLineReady()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return true;
            }
            else
            {
                return (_seaveyorInputConveyor.IsInpositionSensorOn() == true);
            }
        }

        public void CheckCarrierPresentAtInputTurnStation()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(10 * CommonFunctions.SIMULATION_DELAY);
                return;
            }

            try
            {
                inputTurnSectionIO.inPosition.WaitForState(DigitalIOState.On, 200);
                HSTMachine.Workcell.InputTurnStationBoatPositionError = false;
            }
            catch (Exception ex)
            {
                HSTMachine.Workcell.InputTurnStationBoatPositionError = true;
                HSTException.Throw(HSTErrors.InputTurnStationInPositionNotOnError, ex);
            }

        }

        public bool IsInputTurnStationHoldCarrier()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return false;
            }
            return (inputTurnSectionIO.inPosition.Get() == DigitalIOState.On ? true : false);
        }

        public void InhibitInputTurnStation(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                inputTurnSection.InhibitRelay = 1;
            }
            else
            {
                inputTurnSection.InhibitRelay = 0;
            }
        }

        public void InhibitInputConveyor(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                _seaveyorInputConveyor.Inhibit(true);
            }
            else
            {
                _seaveyorInputConveyor.Inhibit(false);
            }
        }

        public void InputTurnSectionTurnTo90Deg(out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                inputTurnSection.RotaryCylinder.TurnCCW(nCylinderTimeout, out timeUsed);
                if (inputTurnSectionIO.At90DegSensor.Get() == DigitalIOState.Off)
                {
                    throw new ControllerException("Input TurnSection no in 90 degree position.");
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputTurnStationRotaryRotateClockwiseError, ex);
            }
            return;
        }

        public void InputTurnSectionTurnTo0Deg(out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                inputTurnSection.RotaryCylinder.TurnCW(nCylinderTimeout, out timeUsed);//chanfg mask LinearActuator call and replace with direct IO call
                if (inputTurnSectionIO.At0DegSensor.Get() == DigitalIOState.Off)
                {
                    throw new ControllerException("Input TurnSection no in 0 degree position.");
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputTurnStationRotaryRotateCounterClockwiseError, ex);
            }
            return;
        }

        public void InputTurnTableFree()
        {
            _doInputTurnTableTurnTo0Deg.Set(DigitalIOState.Off);
            _doInputTurnTableTurnTo90Deg.Set(DigitalIOState.Off);
        }

        public void InputTurnSectionTurnTo0Deg()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                inputTurnSection.RotaryCylinder.TurnCW(nCylinderTimeout);

                if (inputTurnSectionIO.At0DegSensor.Get() == DigitalIOState.Off)
                {
                    throw new ControllerException("Input TurnSection no in 0 degree position.");
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputTurnStationRotaryRotateCounterClockwiseError, ex);
            }
            return;
        }

        public void InputTurnSectionTurnTo90Deg()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                inputTurnSection.RotaryCylinder.TurnCCW(nCylinderTimeout);
                if (inputTurnSectionIO.At90DegSensor.Get() == DigitalIOState.Off)
                {
                    throw new ControllerException("Input TurnSection no in 90 degree position.");
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputTurnStationRotaryRotateClockwiseError, ex);
            }
            return;
        }
    }
}
