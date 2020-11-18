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
    public class OutputTurnStationController : ControllerHST
    {        
        private HSTWorkcell _workcell;
        private HSTIOManifest _ioManifest;        
        private string name = "OutputTurnStationController";

        internal TurnSectionIO outputTurnSectionIO;
        internal ITurnSection outputTurnSection;

        private IDigitalOutput _doOutputTurnTableTurnTo0Deg;
        private IDigitalOutput _doOutputTurnTableTurnTo90Deg;

        private IDigitalOutput _doOutputStopper;

        private const uint nCylinderTimeout = 5000;	//ms
        private const int nTurningTime = 300;	//ms
        private const int nReleaseCarrierTime = 1200;	//ms

        // Constructors ------------------------------------------------------------
        public OutputTurnStationController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {

            this._workcell = workcell;            
            this._ioManifest = (HSTIOManifest)HSTMachine.Workcell.IOManifest;
            outputTurnSectionIO = new TurnSectionIO();


            ///// Input Turn Station/////////////

            //digital input
            outputTurnSectionIO.exitClearSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_Exit_Clear);
            outputTurnSectionIO.inPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_In_Position);
            outputTurnSectionIO.At90DegSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_90_Deg);
            outputTurnSectionIO.At0DegSensor = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_0_Deg);

            //digital output
            outputTurnSectionIO.turnTo90Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Turn_Station_Turn_To_90_Deg);
            outputTurnSectionIO.turnTo0Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Turn_Station_Turn_To_0_Deg);
            outputTurnSectionIO.inhibitRelay = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Turn_Station_Inhibit);

            _doOutputTurnTableTurnTo0Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Turn_Station_Turn_To_0_Deg);
            _doOutputTurnTableTurnTo90Deg = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Turn_Station_Turn_To_90_Deg);

            _doOutputStopper = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Stopper);

            TurnSection TS = new TurnSection(outputTurnSectionIO);
            outputTurnSection = TS as Seagate.AAS.Parsel.Device.ITurnSection;
            outputTurnSection.Simulation = (workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            outputTurnSection.Name = "OutputTurnSection";
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

        public bool NextZoneReady()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
            {                
                return true;
            }
            // Lai: We use exit clear signal as it's sensed from the next zone conveyor "In Position" sensor
            return (outputTurnSectionIO.exitClearSensor.Get() == DigitalIOState.Off ? true : false);
        }

        public void WaitOutputTurnStationPartReady()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(10 * CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    outputTurnSectionIO.inPosition.WaitForState(DigitalIOState.On, 5000);
                    HSTMachine.Workcell.OutputTurnStationBoatPositionError = false;
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.OutputTurnStationBoatPositionError = true;
                    HSTException.Throw(HSTErrors.OutputTurnStationInPositionNotOnError, ex);
                }
            }
        }

        public void WaitOutputTurnStationPartCleared()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(10 * CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    outputTurnSectionIO.inPosition.WaitForState(DigitalIOState.Off, 5000);
                    HSTMachine.Workcell.OutputTurnStationBoatPositionError = false;
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.OutputTurnStationBoatPositionError = true;
                    HSTException.Throw(HSTErrors.InputTurnStationInPositionNotOffError, ex);
                }
            }
        }


        public void InhibitOutputTurnStation(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                outputTurnSection.InhibitRelay = 1;
            }
            else
            {
                outputTurnSection.InhibitRelay = 0;
            }
        }

        public void OutputTurnSectionReleaseCarrier(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                if (on)
                {
                    if (outputTurnSectionIO.At90DegSensor.Get() == DigitalIOState.Off)
                    {
                        throw new ControllerException("Output TurnSection no in 90 degree position.");
                    }
                    outputTurnSectionIO.exitRelay.Set(DigitalIOState.On);
                }
                else
                {
                    outputTurnSectionIO.exitRelay.Set(DigitalIOState.Off);
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.OutputTurnStationExitClearNotOnError, ex);
            }
            return;
        }

        public void OutputTurnSectionTurnTo90Deg(out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                outputTurnSection.RotaryCylinder.TurnCCW(nCylinderTimeout, out timeUsed);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.OutputTurnStationRotaryRotateClockwiseError, ex);
            }
            return;
        }

        public void OutputTurnSectionTurnTo0Deg(out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                outputTurnSection.RotaryCylinder.TurnCW(nCylinderTimeout, out timeUsed);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.OutputTurnStationRotaryRotateCounterClockwiseError, ex);
            }
            return;
        }

        public bool IsOutputTurnStationHoldCarrier()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return false;
            }
            return (outputTurnSectionIO.inPosition.Get() == DigitalIOState.On ? true : false);
        }

        public bool IsOutputTurnStationTurn0Deg()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return false;
            }

            return (outputTurnSection.RotaryCylinder.State == RotaryActuatorState.Clockwise ? true : false);
        }

        public void OutputTurnTableFree()
        {
            _doOutputTurnTableTurnTo0Deg.Set(DigitalIOState.Off);
            _doOutputTurnTableTurnTo90Deg.Set(DigitalIOState.Off);
        }

        public void OutputTurnSectionTurnTo0Deg()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                outputTurnSection.RotaryCylinder.TurnCW(nCylinderTimeout);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.OutputTurnStationRotaryRotateCounterClockwiseError, ex);
            }
            return;
        }

        public void OutputTurnSectionTurnTo90Deg()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            try
            {
                outputTurnSection.RotaryCylinder.TurnCCW(nCylinderTimeout);
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.OutputTurnStationRotaryRotateClockwiseError, ex);
            }
            return;
        }
    }
}
