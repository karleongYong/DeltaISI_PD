using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.Parsel.Device.SafetyController;
using Seagate.AAS.IO.Serial;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class OmronSafetyController : ControllerHST
    {
        // Member variables ----------------------------------------------------

        private SafetyController _safetyController;

        public bool EMOPressed = false;
        public bool DoorLocked = false;
        public bool PowerButtonPressed = false;
        public bool AutomationEnabledButtonPressed = false;
        public bool AutomationStopButtonPressed = false;
        public bool PowerOn = false;
        public bool AutomationEnabled = false;
        public bool ConveyorEnabled = false;
        public bool AutomationPrepareToStop = false;
        public List<string> errorList = new List<string>();

               // Constructors & Finalizers -------------------------------------------
        public OmronSafetyController(HSTWorkcell workcell, string controllerID, string controllerName)
            : base(workcell, controllerID, controllerName)
        {
            _workcell = workcell;               
            this._controllerID = controllerID;

            try
            {
                _safetyController = HSTMachine.Workcell.OmronSafetyController;
            }
            catch (Exception ex)
            {
                ExceptionSafetyController _exceptionSafetyController = new ExceptionSafetyController();
            }
        }

        public void SendRequestToSafetyController(bool towerlightAmberOn, bool towerLightRedOn, bool sirenOn,
            bool powerLEDOn, bool AutomationRunLEDOn, bool AutomationStopLEDOn, bool StopAutomation, bool PowerOnConveyor, bool PowerOffConveyor)
        {
            byte[] receptionData = new byte[6];
            byte firstByte;
            byte secondByte;

            firstByte = 0x00;
            secondByte = 0x00;

            SetBit(ref firstByte, 0, towerlightAmberOn);
            SetBit(ref firstByte, 1, towerLightRedOn);
            if (HSTMachine.Workcell.HSTSettings.Install.AudibleAlarmEnabled)
                SetBit(ref firstByte, 2, sirenOn);
            else
                SetBit(ref firstByte, 2, false);

            SetBit(ref firstByte, 3, powerLEDOn);
            SetBit(ref firstByte, 4, AutomationRunLEDOn);
            SetBit(ref firstByte, 5, AutomationStopLEDOn);
            SetBit(ref firstByte, 6, StopAutomation);
            SetBit(ref firstByte, 7, PowerOffConveyor);
            SetBit(ref secondByte, 0, PowerOnConveyor);
            
            receptionData[0] = firstByte;
            receptionData[1] = secondByte;
            receptionData[2] = 0x00;
            receptionData[3] = 0x00;
            receptionData[4] = 0x00;
            receptionData[5] = 0x00;

            _safetyController.constructAndSendWriteDataBuffer(receptionData);

            _safetyController.DataReceivedFromSafetyController -= ReceivedDataFromSafetyController;
            _safetyController.DataReceivedFromSafetyController += ReceivedDataFromSafetyController;
        }

        private void ReceivedDataFromSafetyController(object sender, EventArgs e)
        {
            getResponseFromSafetyController();
        }


        public void getResponseFromSafetyController()
        {
            if (!GetBit(_safetyController.inputTerminalDataFlags[0], 0) || !GetBit(_safetyController.inputTerminalDataFlags[0], 1))
                EMOPressed = true;
            else
                EMOPressed = false;

            if (GetBit(_safetyController.inputTerminalDataFlags[0], 2) && GetBit(_safetyController.inputTerminalDataFlags[0], 3))
                DoorLocked = true;
            else
                DoorLocked = false;


            if (GetBit(_safetyController.inputTerminalDataFlags[1], 0))
                PowerButtonPressed = true;
            else
                PowerButtonPressed = false;

            if (GetBit(_safetyController.inputTerminalDataFlags[1], 1))
                AutomationEnabledButtonPressed = true;
            else
                AutomationEnabledButtonPressed = false;

            if (GetBit(_safetyController.inputTerminalDataFlags[1], 4))
                AutomationStopButtonPressed = true;
            else
                AutomationStopButtonPressed = false;

            if (GetBit(_safetyController.outputTerminalDataFlags[0], 0))
                PowerOn = true;
            else
                PowerOn = false;

            if (GetBit(_safetyController.outputTerminalDataFlags[0], 2))
                AutomationEnabled = true;
            else
                AutomationEnabled = false;

            if (GetBit(_safetyController.outputTerminalDataFlags[0], 6)) // k6 from schematic
                ConveyorEnabled = true;
            else
                ConveyorEnabled = false;

            if (GetBit(_safetyController.OptionalCommTransmittionData[0], 0))
                AutomationPrepareToStop = true;
            else
                AutomationPrepareToStop = false;

            errorList = _safetyController.errorList;
        }

        private void SetBit(ref byte aByte, int pos, bool value)
        {
            if (value)
            {
                //left-shift 1, then bitwise OR
                aByte = (byte)(aByte | (1 << pos));
            }
            else
            {
                //left-shift 1, then take complement, then bitwise AND
                aByte = (byte)(aByte & ~(1 << pos));
            }
        }

        public static bool GetBit(byte aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        public void Dispose()
        {
            try
            {

            }
            catch
            {
            }
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

    }
}
