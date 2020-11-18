using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel;
using System.IO;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Exceptions
{
    public class HSTException : ParselException
    {
        // Nested declarations -------------------------------------------------
        private static object _locker = new object();
        

        //Lai: To define error message and work instruction for each error code.
        static Dictionary<HSTErrors, string[]> errorInfo = new Dictionary<HSTErrors, string[]>
        {
            {HSTErrors.InputTurnStationRotaryRotateClockwiseError , new string[] {"Input turn station rotary actuation error.", "Work Instruction TBD"}},
            {HSTErrors.InputTurnStationRotaryRotateCounterClockwiseError , new string[] {"Input turn station rotary actuation error.", "Work Instruction TBD"}},
            {HSTErrors.InputTurnStationEntranceClearNotOnError , new string[] {"Input Turn Station Entrance Clear Not On Error", "Work Instruction TBD"}},
            {HSTErrors.InputTurnStationInPositionNotOnError , new string[] {"Input Turn Station In Position Not On Error", "Work Instruction TBD"}},
            {HSTErrors.InputTurnStationInPositionNotOffError , new string[] {"Input Turn Station In Position Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.InputTurnStationExitClearNotOnError , new string[] {"Input Turn Station Exit Clear Not On Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationLifterExtendError , new string[] {"Input Station Lifter Failed to Extend Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationLifterRetractError , new string[] {"Input Station Lifter Failed to Retract Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationStopperExtendError , new string[] {"Input Station Stopper Failed to Extend Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationStopperRetractError , new string[] {"Input Station Stopper Failed to Retract Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationCarrierScrewDriverExtendError , new string[] {"Input Station Carrier Screw Driver Extend Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationCarrierScrewDriverRetractError , new string[] {"Input Station Carrier Screw Driver Retract Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationCarrierScrewDriverRotateClockwiseError , new string[] {"Input Station Carrier Screw Driver Rotate Clockwise Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationCarrierScrewDriverRotateCounterClockwiseError , new string[] {"Input Station Carrier Screw Driver Rotate Counter Clockwise Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationInPositionNotOnError , new string[] {"Input Station In Position Not On Error", "Work Instruction TBD"}},
            {HSTErrors.InputStationInPositionNotOffError , new string[] {"Input Station In Position Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationLifterExtendError , new string[] {"Output Station Lifter Extend Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationLifterRetractError , new string[] {"Output Station Lifter Retract Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationStopperExtendError , new string[] {"Output Station Stopper Extend Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationStopperRetractError , new string[] {"Output Station Stopper Retract Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationCarrierScrewDriverExtendError , new string[] {"Output Station Carrier Screw Driver Extend Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationCarrierScrewDriverRetractError , new string[] {"Output Station Carrier Screw Driver Retract Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationCarrierScrewDriverRotateClockwiseError , new string[] {"Output Station Carrier Screw Driver Rotate Clockwise Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationCarrierScrewDriverRotateCounterClockwiseError , new string[] {"Output Station Carrier Screw Driver Rotate Counter Clockwise Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationInPositionNotOnError , new string[] {"Output Station In Position Not On Error", "Work Instruction TBD"}},
            {HSTErrors.OutputStationInPositionNotOffError , new string[] {"Output Station In Position Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.OutputTurnStationRotaryRotateClockwiseError , new string[] {"Output Turn Station Rotary Rotate Actuation Error", "Work Instruction TBD"}},
            {HSTErrors.OutputTurnStationRotaryRotateCounterClockwiseError , new string[] {"Output Turn Station Rotary Rotate Actuation Error", "Work Instruction TBD"}},
            {HSTErrors.OutputTurnStationEntranceClearNotOnError , new string[] {"Output Turn Station Entrance Clear Not On Error", "Work Instruction TBD"}},
            {HSTErrors.OutputTurnStationInPositionNotOnError , new string[] {"Output Turn Station In Position Not On Error", "Work Instruction TBD"}},
            {HSTErrors.OutputTurnStationInPositionNotOffError , new string[] {"Output Turn Station In Position Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.OutputTurnStationExitClearNotOnError , new string[] {"Output Turn Station Exit Clear Not On Error", "Work Instruction TBD"}},
            {HSTErrors.OutputConveyorCongestionError , new string[] {"Output Conveyor Congestion Error", "Work Instruction TBD"}},
            {HSTErrors.InputHandlerZAxisHomeError, new string[] {"Input Handler Z Axis Home Error", "Work Instruction TBD"}},
            {HSTErrors.InputHandlerZAxisMoveDownError , new string[] {"Input Handler Z Axis Move Down Error", "Work Instruction TBD"}},
            {HSTErrors.InputHandlerZAxisMoveUpError , new string[] {"Input Handler Z Axis Move Up Error", "Work Instruction TBD"}},
            {HSTErrors.InputHandlerZAxisReadPositionError , new string[] {"Input Handler Z Axis Read Position Error", "Work Instruction TBD"}},
            {HSTErrors.OutputHandlerZAxisHomeError, new string[] {"Output Handler Z Axis Home Error", "Work Instruction TBD"}},
            {HSTErrors.OutputHandlerZAxisMoveDownError , new string[] {"Output Handler Z Axis Move Down Error", "Work Instruction TBD"}},
            {HSTErrors.OutputHandlerZAxisMoveUpError , new string[] {"Output Handler Z Axis Move Up Error", "Work Instruction TBD"}},
            {HSTErrors.OutputHandlerZAxisReadPositionError , new string[] {"Output Handler Z Axis Read Position Error", "Work Instruction TBD"}},
            {HSTErrors.InputEndEffectorVacuumPressureSensor1NotOnError , new string[] {"Input EE Vaccum turn on error on channel 1", "Work Instruction TBD"}},
            {HSTErrors.InputEndEffectorVacuumPressureSensor1NotOffError , new string[] {"Input EE Vaccum turn off error on channel 1", "Work Instruction TBD"}},
            {HSTErrors.InputEndEffectorVacuumPressureSensor2NotOnError , new string[] {"Input EE Vaccum turn on error on channel 2", "Work Instruction TBD"}},
            {HSTErrors.InputEndEffectorVacuumPressureSensor2NotOffError , new string[] {"Input EE Vaccum turn on error on channel 2", "Work Instruction TBD"}},
            {HSTErrors.OutputEndEffectorVacuumPressureSensor1NotOnError , new string[] {"Output EE Vaccum turn on error on channel 1", "Work Instruction TBD"}},
            {HSTErrors.OutputEndEffectorVacuumPressureSensor1NotOffError , new string[] {"Output EE Vaccum turn on error on channel 1", "Work Instruction TBD"}},
            {HSTErrors.OutputEndEffectorVacuumPressureSensor2NotOnError , new string[] {"Output EE Vaccum turn on error on channel 2", "Work Instruction TBD"}},
            {HSTErrors.OutputEndEffectorVacuumPressureSensor2NotOffError , new string[] {"Output EE Vaccum turn on error on channel 2", "Work Instruction TBD"}},
            {HSTErrors.BufferStationInPositionNotOnError , new string[] {"Buffer Zone In Position Not On Error", "Work Instruction TBD"}},
            {HSTErrors.BufferStationInPositionNotOffError , new string[] {"Buffer Zone In Position Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestXAxisHomeError, new string[] {"Precisor Nest X Axis Home Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestXAxisMoveLeftError , new string[] {"Precisor Nest X Axis Move Left Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestXAxisMoveRightError , new string[] {"Precisor Nest X Axis Move Right Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestXAxisReadPositionError , new string[] {"Precisor Nest X Axis Read Position Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestYAxisHomeError, new string[] {"Precisor Nest Y Axis Home Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestYAxisMoveBackError , new string[] {"Precisor Nest Y Axis Move Back Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestYAxisMoveFrontError , new string[] {"Precisor Nest Y Axis Move Front Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestYAxisReadPositionError , new string[] {"Precisor Nest Y Axis Read Position Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestThetaAxisHomeError, new string[] {"Precisor Nest Theta Axis Home Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestThetaAxisRotateClockwiseError , new string[] {"Precisor Nest Theta Axis Rotate Clockwise Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestThetaAxisRotateCounterClockwiseError , new string[] {"Precisor Nest Theta Rotate Counter Clockwise Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestThetaAxisReadPositionError , new string[] {"Precisor Nest Theta Axis Read Position Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor1NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 1 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor1NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 1 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor2NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 2 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor2NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 2 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor3NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 3 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor3NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 3 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor4NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 4 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor4NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 4 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor5NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 5 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor5NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 5 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor6NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 6 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor6NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 6 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor7NotOffError , new string[] {"Precisor Nest Vacuum Pressure Sensor 7 Not Off Error", "Work Instruction TBD"}},
            {HSTErrors.PrecisorNestVacuumPressureSensor7NotOnError , new string[] {"Precisor Nest Vacuum Pressure Sensor 7 Not On Error", "Work Instruction TBD"}},
            {HSTErrors.TestProbeHandlerZAxisHomeError, new string[] {"Test Probe Handler Z Axis Home Error", "Work Instruction TBD"}},
            {HSTErrors.TestProbeHandlerZAxisMoveDownError , new string[] {"Test Probe Handler Z Axis Move Down Error", "Work Instruction TBD"}},
            {HSTErrors.TestProbeHandlerZAxisMoveUpError , new string[] {"Test Probe Handler Z Axis Move Up Error", "Work Instruction TBD"}},
            {HSTErrors.TestProbeHandlerZAxisReadPositionError , new string[] {"Test Probe Handler Z Axis Read Position Error", "Work Instruction TBD"}},
            {HSTErrors.TestProbeFixtureCalibrationError , new string[] {"Test Probe Fixture Calibration Error", "Work Instruction TBD"}},
            {HSTErrors.AlignmentCameraError , new string[] {"Alignment Camera Error", "Work Instruction TBD"}},
            {HSTErrors.InputDetectionCameraError , new string[] {"Input Detection Camera Error", "Work Instruction TBD"}},
            {HSTErrors.InputDetectionCameraCarrierLoadedInWrongDirection , new string[] {"Input Detection Camera Detected Carrier Loaded in Wrong Direction Error", "Work Instruction TBD"}},
            {HSTErrors.OutputDetectionCameraError , new string[] {"Output Detection Camera Error", "Work Instruction TBD"}},
            {HSTErrors.OutputDetectionCameraHGANotPickedUpAtInputStationError , new string[] {"Output Detection Camera Not All HGAs Picked Up At Input Station Error", "Work Instruction TBD"}},
            {HSTErrors.OutputDetectionCameraFailedToPlaceBackAllHGAsAfterMeasurementError , new string[] {"Output Detection Camera Failed To Place Back All HGAs After Measurement Error", "Work Instruction TBD"}},
            {HSTErrors.GroundMasterError , new string[] {"Ground Master error", "Work Instruction TBD"}},
            {HSTErrors.DoorNotLockError , new string[] {"Door is not lock error", "Work Instruction TBD"}},
            {HSTErrors.EMOTriggeredError , new string[] {"", "Work Instruction TBD"}},
            {HSTErrors.InputRFIDReadFailed , new string[] {"Last step before not complete", "Last step not update before run HST, please check"}},
            {HSTErrors.InputRFIDReadWorkorderFailed , new string[] {"Workorder not macth", "Workorder has been changed in RFID, please check"}},
            {HSTErrors.InputRFIDReadFoundAllHGAsFailed , new string[] {"Input RFID read found all HGAs already failed by previous systems", "Work Instruction TBD"}},
            {HSTErrors.InputRFIDReadFoundAHGAsMissingSerialNo , new string[] {"Input RFID read found HGAs Missing HGA Serial Number", "Work Instruction TBD"}},
            {HSTErrors.InputRFIDReadFoundProductTypeMismatch , new string[] {"Input RFID read found Product Type mismatch with user selected product type", "If this carrier need to be processed, please select the same product type via the 'Save Config to Processor' button, and then press 'Retry' button again."}},
            {HSTErrors.OutputRFIDReadError , new string[] {"Data retrieved from Output RFID incomplete or no response", "Work Instruction TBD"}},
            {HSTErrors.OutputRFIDWriteError , new string[] {"Output RFID Write Failed", "Work Instruction TBD"}},
            {HSTErrors.SafetyControllerPresentError , new string[] {"Safety Controller Present Error", "Work Instruction TBD"}},
            {HSTErrors.TestElectronicsMeasurementError , new string[] {"Aborting subsequent measurement test owing to error.", "Work Instruction TBD."}},
            {HSTErrors.TestElectronicsMeasurementTimeout , new string[] {"Measurement test has time out.", "Work Instruction TBD."}},
            {HSTErrors.TestElectronicsWrongProductType , new string[] {"The product type of the loaded work order is NOT supported by the test electronics board.", "Please ensure the correct work order or test electronics board is used."}},
            {HSTErrors.TestElectronicsOutdatedMeasurementTestConfiguration , new string[] {"The product type of the loaded work order is missing from the configuration of the Measurement Test component.", "Please add the new product type to the configuration of the Measurement Test component."}},
            {HSTErrors.TestElectronicsSerialPortCommunicationError , new string[] {"Unable to communicate with the test electronics board via serial port.", "Please ensure correct serial port settings with the test eletronics board have been configured."}},
            {HSTErrors.TestElectronicsTICErrorDetection , new string[] {"Test Electronics TIC Error Detection.", "Please check previous systems on why many HGAs has TIC errors."}},
            {HSTErrors.TestElectronecsGetputErrorDetection , new string[] {"Test Electronics HGA Sort Error Detection.", "Please contact FIS (ext.7510) or try to re-open FISGetputServer software again."}},
            {HSTErrors.TestElectronecsGetputErrorDetection2 , new string[] {"Test Electronics HGA Sort Error Detection.", "Please contact Process team or Production team to verify, slider lot number."}},

       };



        public static void Throw(HSTErrors error, Exception exception)
        {
            string errMsg = "";
            string workInstruction = "";

            lock (_locker)
            {
                errMsg = errorInfo.ContainsKey(error)? errorInfo[error][0] : error.ToString();
                workInstruction = errorInfo.ContainsKey(error)? errorInfo[error][1] : error.ToString();

                string errCode;
                errCode = ((int)error).ToString("000000");
                throw new HSTException(errCode, errMsg, exception, workInstruction);
            }
        }

        // Member variables ----------------------------------------------------

        // Constructors & Finalizers -------------------------------------------
        public HSTException(string errorCode, string message, Exception innerException, string workInstruction)
            : base(message, innerException)
        {
            CommonFunctions.Instance.strErrorMessageCode = errorCode;
            this.ErrorCode = errorCode;
            this.WorkInstruction = workInstruction;
            this.TimeStamp = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
        }


        // Properties ----------------------------------------------------------
        public string ErrorCode { get; private set; }
        public string WorkInstruction { get; private set; }
        public string TimeStamp { get; private set; }

        // Methods -------------------------------------------------------------
    }
}
