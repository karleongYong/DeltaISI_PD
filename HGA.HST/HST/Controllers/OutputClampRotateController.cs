using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.Utils;
using System.IO;
using System.Threading;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.Parsel.Device.PneumaticControl;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class OutputClampRotateController : ControllerHST
    {
        private ILinearActuator _clamp;
        private ILinearActuator _clampRotary;
        private HSTWorkcell _workcell;
        private HSTIOManifest _ioManifest;
        private const uint nCylinderTimeout = 5000;	//ms

        private IDigitalOutput _doOutputStationClampDeploy;
        private IDigitalOutput _doOutputStationClampRotate;

        private IDigitalInput _diOutputStationClampForward;
        private IDigitalInput _diOutputStationClampBackward;
        private IDigitalInput _diOutputStationClampRotateCwOpen;
        private IDigitalInput _diOutputStationClampRotateCcwClose;

        private IDigitalInput _diOutputStationClampOpenDetect;

        // Constructors ------------------------------------------------------------
        public OutputClampRotateController(HSTWorkcell workcell, string controllerID, string controllerName)
            : base(workcell, controllerID, controllerName)
        {
            _workcell = workcell;
            this._ioManifest = (HSTIOManifest) workcell.IOManifest;

            _doOutputStationClampDeploy =
                _ioManifest.GetDigitalOutput((int) HSTIOManifest.DigitalOutputs.Output_CS_Deploy);
            _doOutputStationClampRotate =
                _ioManifest.GetDigitalOutput((int) HSTIOManifest.DigitalOutputs.Output_CS_Rotate);

            _diOutputStationClampForward =
                _ioManifest.GetDigitalInput((int) HSTIOManifest.DigitalInputs.Output_CS_Deploy);
            _diOutputStationClampBackward =
                _ioManifest.GetDigitalInput((int) HSTIOManifest.DigitalInputs.Output_CS_Retract);
            _diOutputStationClampRotateCwOpen =
                _ioManifest.GetDigitalInput((int) HSTIOManifest.DigitalInputs.Output_CS_Lock);
            _diOutputStationClampRotateCcwClose =
                _ioManifest.GetDigitalInput((int) HSTIOManifest.DigitalInputs.Output_CS_Unlock);

            _diOutputStationClampOpenDetect = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Carrier_Clamp_Sensor);

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
            {
                //3. Clamp
                LinearActuator ClampActuator = new LinearActuator(_doOutputStationClampDeploy,
                    _diOutputStationClampForward, _diOutputStationClampBackward, DigitalIOState.On);
                _clamp = ClampActuator as ILinearActuator;
                _clamp.Name = "Zone 5 clamp actuator";
                _clamp.ExtendedDirectionName = "Forward";
                _clamp.ExtendedStateName = "Forward";
                _clamp.RetractedDirectionName = "Backward";
                _clamp.RetractedStateName = "Backward";

                //4. ClampRotary
                LinearActuator ClampRotaryActuator = new LinearActuator(_doOutputStationClampRotate,
                    _diOutputStationClampRotateCcwClose, _diOutputStationClampRotateCwOpen, DigitalIOState.On);
                _clampRotary = ClampRotaryActuator as ILinearActuator;
                _clampRotary.Name = "Zone 5 clamp rotary actuator";
                _clampRotary.ExtendedDirectionName = "Cw Open";
                _clampRotary.ExtendedStateName = "Cw Open";
                _clampRotary.RetractedDirectionName = "Ccw Close";
                _clampRotary.RetractedStateName = "Ccw Close";

            }
        }

        public override void InitializeController()
        {

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

        public bool IsOutputClampOpenDetect()
        {
            return (_diOutputStationClampOpenDetect.Get() == DigitalIOState.On ? true : false);
        }
    }
}
