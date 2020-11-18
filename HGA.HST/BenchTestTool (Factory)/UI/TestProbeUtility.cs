using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using BenchTestsTool.Data;
using BenchTestsTool.Data.IncomingTestProbeData;
using BenchTestsTool.Data.OutgoingTestProbeData;
using BenchTestsTool.Utils;

namespace BenchTestsTool.UI
{
    public partial class frmMain
    {
        const int BUFFER_SIZE = 256;
        int numBytes = 0;
        private int DelayBeforeProcessReadBuffer = 300;
        private byte[] writeDataBuffer = new byte[BUFFER_SIZE];
        private byte[] readDataBuffer = new byte[BUFFER_SIZE];
        private bool IsConfigurationSetupTempered = false;
        private bool IsPCBACalibrationTempered = false;
        private bool IsCableCalibrationTempered = false;
        private bool IsPrecisorCompensationTempered = false;

        Dictionary<int, string> dictionary = new Dictionary<int, string>()
	    {
	        {1, "Host RS-232 communication timed-out."},
            {2, "Host command framing was out of synchronization."},
            {3, "No 'ETX' byte was detected in host command frame."},
            {4, "Host RS-232 command checksum error."},
            {5, "Wrong command parameter received from host."},
            {6, "Illegal command received from host."},
            {7, "Failed to write to ADC register."},
            {8, "Failed to read from EEPROM."},
            {9, "Failed to write to EEPROM."},
            {10, "Signature bytes were corrupted in EEPROM."},
            {11, "Wrong checksum of calibration data in EEPROM."},
            {12, "Communication time-out with LCR meter."},
            {13, "Measurement error from LCR meter."},
            {14, "HST calibration is disabled."},
	        {15, "ADC input is out of range."}
	    };            


        delegate void TestProbeGUIUpdateCallback(GUIPage page, byte MessageID, byte[] data);

        // OutgoingTestProbeData data structure
        private TestProbe2ConfigResistanceMeasurement TestProbe2ConfigResistanceMeasurement;
        private TestProbe3ConfigCapacitanceMeasurement TestProbe3ConfigCapacitanceMeasurement;
        private TestProbe4ConfigShortDetection TestProbe4ConfigShortDetection;
        private TestProbe5MeasurementChannelEnable TestProbe5MeasurementChannelEnable;
        private TestProbe6HGAEnable TestProbe6HGAEnable;
        private TestProbe9SetStartMeasurement TestProbe9SetStartMeasurement;
        private TestProbe14SetGetAllResultsByHGA TestProbe14SetGetAllResultsByHGA;
        private TestProbe15SetGetBiasByHGA TestProbe15SetGetBiasByHGA;
        private TestProbe16SetGetSensingByHGA TestProbe16SetGetSensingByHGA;
        private TestProbe17CalibrationEnable TestProbe17CalibrationEnable;
        private TestProbe21SetManualCalibration TestProbe21SetManualCalibration;
        private TestProbe22EEPROMWrite TestProbe22EEPROMWrite;
        private TestProbe23SetEEPROMRead TestProbe23SetEEPROMRead;
        private TestProbe24DACWrite TestProbe24DACWrite;
        private TestProbe25SetDACRead TestProbe25SetDACRead;
        private TestProbe26DACOutputEnable TestProbe26DACOutputEnable;  // By KA Gan: Not used?
        private TestProbe27ADCWrite TestProbe27ADCWrite;
        private TestProbe28SetADCRead TestProbe28SetADCRead;
        private TestProbe29SetADCVoltagesRead TestProbe29SetADCVoltagesRead;
        private TestProbe30SetMUX TestProbe30SetMUX;
        private TestProbe31SetTemperatureCalibration TestProbe31SetTemperatureCalibration;
        private TestProbe32ConfigTemperatureMeasurement TestProbe32ConfigTemperatureMeasurement;
        private TestProbe43SetFlexCableCalibration TestProbe43SetFlexCableCalibration;
        private TestProbe44SetGetCableCalibrationResistanceResults TestProbe44SetGetCableCalibrationResistanceResults;
        private TestProbe45SetCableCompensation TestProbe45SetCableCompensation;
        private TestProbe47SetShortDetectionThreshold TestProbe47SetShortDetectionThreshold;
        private TestProbe49SetTemp1Offset TestProbe49SetTemp1Offset;
        private TestProbe51SetGetCableCalibrationCapacitanceResults TestProbe51SetGetCableCalibrationCapacitanceResults;
        private TestProbe52SetPrecisorCapacitanceCompensation TestProbe52SetPrecisorCapacitanceCompensation;
        private TestProbe53SetGetPrecisorCapacitanceCompensation TestProbe53SetGetPrecisorCapacitanceCompensation;



        // IncomingTestProbeData data structure
        private TestProbeGetStatusAndErrorCode TestProbeGetStatusAndErrorCode;
        private TestProbe7GetProductID TestProbe7GetProductID;
        private TestProbe8GetOperationMode TestProbe8GetOperationMode;
        private TestProbe10GetAllHGAShortDetection TestProbe10GetAllHGAShortDetection;
        private TestProbe11GetAllHGAResistanceResults TestProbe11GetAllHGAResistanceResults;
        private TestProbe12GetAllHGACapacitanceResults TestProbe12GetAllHGACapacitanceResults;
        private TestProbe13GetAllHGABiasVoltages TestProbe13GetAllHGABiasVoltages;  // By KA Gan: Not used in this application
        private TestProbe14GetAllResultsByHGA TestProbe14GetAllResultsByHGA;        
        private TestProbe15GetBiasByHGA TestProbe15GetBiasByHGA;
        private TestProbe16GetSensingByHGA TestProbe16GetSensingByHGA;
        private TestProbe18StartAutoCalibration TestProbe18StartAutoCalibration;
        private TestProbe20GetCalibrationData TestProbe20GetCalibrationData;
        private TestProbe21GetManualCalibration TestProbe21GetManualCalibration;
        private TestProbe23GetEEPROMRead TestProbe23GetEEPROMRead;
        private TestProbe25GetDACRead TestProbe25GetDACRead;
        private TestProbe28GetADCRead TestProbe28GetADCRead;
        private TestProbe29GetADCVoltagesRead TestProbe29GetADCVoltagesRead;
        private TestProbe31GetTemperatureCalibration TestProbe31GetTemperatureCalibration;
        private TestProbe33GetTemperature TestProbe33GetTemperature;
        private TestProbe34GetAllHGACapacitanceSecondaryResults TestProbe34GetAllHGACapacitanceSecondaryResults;
        private TestProbe35GetCapacitanceReadingFromLCRMeter TestProbe35GetCapacitanceReadingFromLCRMeter;
        private TestProbe36StartSelfTest TestProbe36StartSelfTest;
        private TestProbe37GetFirmwareVersion TestProbe37GetFirmwareVersion;
        private TestProbe38GetCalibrateOffset TestProbe38GetCalibrateOffset;
        private TestProbe39GetCalibrationOffset TestProbe39GetCalibrationOffset;        
        private TestProbe44GetCableCalibrationResistanceResults TestProbe44GetCableCalibrationResistanceResults;
        private TestProbe48GetShortDetectionThreshold TestProbe48GetShortDetectionThreshold;
        private TestProbe50GetTemp1Offset TestProbe50GetTemp1Offset;
        private TestProbe51GetCableCalibrationCapacitanceResults TestProbe51GetCableCalibrationCapacitanceResults;
        private TestProbe53GetPrecisorCapacitanceCompensation TestProbe53GetPrecisorCapacitanceCompensation;


        private bool constructAndSendWriteDataBuffer()
        {
            if (CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Count == 0)
            {
                DataReceived = true;
                return false;
            }

            try
            {
                string CommandParameterInText = "null";
                TestProbeAPICommand APICommand = CommonFunctions.Instance.OutgoingTestProbeDataAPIs.First();

                writeDataBuffer = new byte[BUFFER_SIZE];
                int MessageCheckSum = 0;

                writeDataBuffer[0] = 0x02;  // Start of Message
                writeDataBuffer[1] = APICommand.CommandSize;  // Message Size
                writeDataBuffer[2] = (byte)MessageType.Command;  // Message Type (Checksum calculation begins here)
                writeDataBuffer[3] = APICommand.CommandID;  // Message ID

                MessageCheckSum += writeDataBuffer[2] + writeDataBuffer[3];

                if (APICommand.CommandParameter != null)
                {
                    CommandParameterInText = BitConverter.ToString(APICommand.CommandParameter, 0, APICommand.CommandParameter.Length);
                    CommandParameterInText = CommandParameterInText.Replace('-', ' ');

                    for (int i = 0; i < APICommand.CommandParameter.Length; i++)
                    {
                        writeDataBuffer[4 + i] = APICommand.CommandParameter[i];
                        MessageCheckSum += writeDataBuffer[4 + i];
                    }

                    writeDataBuffer[4 + APICommand.CommandParameter.Length] = (byte)MessageCheckSum; // Message Checksum
                    writeDataBuffer[5 + APICommand.CommandParameter.Length] = 0x03;  // End of Message
                    numBytes = 6 + APICommand.CommandParameter.Length;
                }
                else
                {                    
                    writeDataBuffer[4] = (byte)MessageCheckSum;
                    writeDataBuffer[5] = 0x03;  // End of Message
                    numBytes = 6;
                }

                Log.Info("OutgoingTestProbeData serial data", "Sending command: {0}, parameter: {1}, message size: {2} to uProcessor.", APICommand.CommandID, CommandParameterInText, APICommand.CommandSize);
                testProbeComPort.Write(writeDataBuffer, 0, numBytes);                
                return true;
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Serial Port Transmission Error", "Error in constructAndSendWriteDataBuffer() function: " + ex.Message + ".");                
                return false;
            }
        }


        private void testProbeComPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte CommandStatusFromMicroprocessor = 0;
            byte CommandErrorCodeFromMicroprocessor = 0;
            bool commandSentToMicroprocessor = false;
            string CommandParameterInText = "null";
            readDataBuffer = new byte[testProbeComPort.ReadBufferSize];

            Thread.Sleep(DelayBeforeProcessReadBuffer);
            numBytes = testProbeComPort.BytesToRead;
            testProbeComPort.Read(readDataBuffer, 0, numBytes);

            // readDataBuffer[0]  is the Start of Message byte
            byte CommandSize = readDataBuffer[1]; // Message Size
            byte CommandType = readDataBuffer[2]; // Message Type (Checksum calculation begins here)
            byte CommandID = readDataBuffer[3]; // Message ID
            CommandStatusFromMicroprocessor = readDataBuffer[4];
            CommandErrorCodeFromMicroprocessor = readDataBuffer[5];

            CableCalibrationDataReceived = true;

            if (CommandSize < 5 || CommandID < 1)
            {
                return;
            }

            int CommandParameterSize = 0;

            if (CommandStatusFromMicroprocessor == (byte)CommandStatus.Busy)
            {
                Notify.PopUpError("uProcessor Returned Busy Status", String.Format("uProcessor was too busy to execute command: {0}.", CommandID));
                Log.Info("uProcessor", "uProcessor was too busy to execute command: {0}.", CommandID);

                testProbeComPort.DiscardInBuffer();                
                // Resend the same commands
                commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                return;
            }
            else if (CommandStatusFromMicroprocessor == (byte)CommandStatus.Error)
            {
                bool foundErrorMessage = false;
                foreach (var KeyvaluePair in dictionary)
                {
                    if(KeyvaluePair.Key == CommandErrorCodeFromMicroprocessor)
                    {
                        Notify.PopUpError("uProcessor Returned Error Status", String.Format("uProcessor failed to execute command: {0} due to error code: {1}, error message: {2}.", CommandID, KeyvaluePair.Key, KeyvaluePair.Value));
                        Log.Info("uProcessor", "uProcessor failed to execute command: {0} due to error code: {1}, error message: {2}.", CommandID, KeyvaluePair.Key, KeyvaluePair.Value);
                        foundErrorMessage = true;
                        break;
                    }
                }

                if (foundErrorMessage == false)
                {
                    Notify.PopUpError("uProcessor Returned Error Status", String.Format("uProcessor failed to execute command: {0} due to error code: {1}.", CommandID, CommandErrorCodeFromMicroprocessor));
                    Log.Info("uProcessor", "uProcessor failed to execute command: {0} due to error code: {1}.", CommandID, CommandErrorCodeFromMicroprocessor);
                }

                testProbeComPort.DiscardInBuffer();      
                return;
            }            

            // Only data structures from IncomingTestProbeData directory are listed here
            switch (CommandID)
            {
                case (byte)MessageID.HST_config_res_meas: // 2
                case (byte)MessageID.HST_config_cap_meas: // 3
                case (byte)MessageID.HST_config_short_detection: // 4
                case (byte)MessageID.HST_meas_channel_enable: // 5
                case (byte)MessageID.HST_hga_enable: // 6
                case (byte)MessageID.HST_start_meas: // 9
                case (byte)MessageID.HST_calibration_enable: // 17
                case (byte)MessageID.HST_save_calibration_data: // 19
                case (byte)MessageID.HST_eeprom_write: // 22
                case (byte)MessageID.HST_dac_write: // 24
                case (byte)MessageID.HST_dac_output_enable: // 26
                case (byte)MessageID.HST_adc_write: // 27
                case (byte)MessageID.HST_set_mux: // 30
                case (byte)MessageID.HST_config_temp_meas: // 32  
                case (byte)MessageID.HST_set_cable_compensation: // 45  
                case (byte)MessageID.HST_clear_all_cable_compensation: // 46 
                case (byte)MessageID.HST_set_short_detection_threshold: // 47 
                case (byte)MessageID.HST_set_temp1_offset: // 49 
                case (byte)MessageID.HST_set_precisor_cap_compensation: // 52 
                case (byte)MessageID.HST_save_precisor_cap_compensation: // 54
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    break;
                case (byte)MessageID.HST_unsolicited_status: // 255
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    // To be implemented by KA Gan for HST_unsolicited_status                    

                    break;
                case (byte)MessageID.HST_get_status: // 1
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_product_id: // 7
                    TestProbe7GetProductID = TestProbe7GetProductID.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe7GetProductID));
                    UpdateGUIDisplay(GUIPage.CommonCaptionBanner, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_operation_mode: // 8
                    TestProbe8GetOperationMode = TestProbe8GetOperationMode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe8GetOperationMode));
                    UpdateGUIDisplay(GUIPage.CommonCaptionBanner, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_short_detection: // 10
                    TestProbe10GetAllHGAShortDetection = TestProbe10GetAllHGAShortDetection.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe10GetAllHGAShortDetection));
                    UpdateGUIDisplay(GUIPage.BenchTestsTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_res_results: // 11
                    TestProbe11GetAllHGAResistanceResults = TestProbe11GetAllHGAResistanceResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe11GetAllHGAResistanceResults));
                    UpdateGUIDisplay(GUIPage.BenchTestsTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_cap_results: // 12
                    TestProbe12GetAllHGACapacitanceResults = TestProbe12GetAllHGACapacitanceResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe12GetAllHGACapacitanceResults));
                    UpdateGUIDisplay(GUIPage.BenchTestsTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_results_by_hga: // 14
                    TestProbe14GetAllResultsByHGA = TestProbe14GetAllResultsByHGA.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe14GetAllResultsByHGA));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_bias_by_hga: // 15
                    TestProbe15GetBiasByHGA = TestProbe15GetBiasByHGA.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe15GetBiasByHGA));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_sensing_by_hga: // 16
                    TestProbe16GetSensingByHGA = TestProbe16GetSensingByHGA.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe16GetSensingByHGA));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_start_auto_calibration: // 18
                    TestProbe18StartAutoCalibration = TestProbe18StartAutoCalibration.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe18StartAutoCalibration));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_calibration_data: // 20
                    TestProbe20GetCalibrationData = TestProbe20GetCalibrationData.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe20GetCalibrationData));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_manual_set_calibration: // 21
                    TestProbe21GetManualCalibration = TestProbe21GetManualCalibration.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe21GetManualCalibration));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_eeprom_read: // 23
                    TestProbe23GetEEPROMRead = TestProbe23GetEEPROMRead.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe23GetEEPROMRead));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_dac_read: // 25
                    TestProbe25GetDACRead = TestProbe25GetDACRead.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe25GetDACRead));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_adc_read: // 28
                    TestProbe28GetADCRead = TestProbe28GetADCRead.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe28GetADCRead));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_adc_voltages: // 29
                    TestProbe29GetADCVoltagesRead = TestProbe29GetADCVoltagesRead.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe29GetADCVoltagesRead));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_set_temp_calibration: // 31
                    TestProbe31GetTemperatureCalibration = TestProbe31GetTemperatureCalibration.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe31GetTemperatureCalibration));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_temperature: // 33
                    TestProbe33GetTemperature = TestProbe33GetTemperature.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe33GetTemperature));
                    UpdateGUIDisplay(GUIPage.FunctionalTestsTabPage, CommandID, readDataBuffer);
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_cap_secondary_results: // 34
                    TestProbe34GetAllHGACapacitanceSecondaryResults = TestProbe34GetAllHGACapacitanceSecondaryResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe34GetAllHGACapacitanceSecondaryResults));
                    UpdateGUIDisplay(GUIPage.BenchTestsTabPage, CommandID, readDataBuffer);
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_cap_reading: // 35
                    TestProbe35GetCapacitanceReadingFromLCRMeter = TestProbe35GetCapacitanceReadingFromLCRMeter.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe35GetCapacitanceReadingFromLCRMeter));
                    UpdateGUIDisplay(GUIPage.DebugTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_start_self_test: // 36
                    TestProbe36StartSelfTest = TestProbe36StartSelfTest.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe36StartSelfTest));
                    UpdateGUIDisplay(GUIPage.FunctionalTestsTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_firmware_version: // 37
                    TestProbe37GetFirmwareVersion = TestProbe37GetFirmwareVersion.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe37GetFirmwareVersion));
                    UpdateGUIDisplay(GUIPage.CommonCaptionBanner, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_calibrate_offset: // 38
                    TestProbe38GetCalibrateOffset = TestProbe38GetCalibrateOffset.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe38GetCalibrateOffset));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_calibration_offset: // 39
                    TestProbe39GetCalibrationOffset = TestProbe39GetCalibrationOffset.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe39GetCalibrationOffset));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;                
                case (byte)MessageID.HST_get_cable_calibration_res_results: // 44
                    TestProbe44GetCableCalibrationResistanceResults = TestProbe44GetCableCalibrationResistanceResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe44GetCableCalibrationResistanceResults));
                    UpdateGUIDisplay(GUIPage.CableCalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_short_detection_threshold: // 48 
                    TestProbe48GetShortDetectionThreshold = TestProbe48GetShortDetectionThreshold.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe48GetShortDetectionThreshold));
                    UpdateGUIDisplay(GUIPage.CableCalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_temp1_offset: // 50
                    TestProbe50GetTemp1Offset = TestProbe50GetTemp1Offset.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe50GetTemp1Offset));
                    UpdateGUIDisplay(GUIPage.PCBACalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_cable_calibration_cap_results: // 51
                    TestProbe51GetCableCalibrationCapacitanceResults = TestProbe51GetCableCalibrationCapacitanceResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe51GetCableCalibrationCapacitanceResults));
                    UpdateGUIDisplay(GUIPage.CableCalibrationTabPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_precisor_cap_compensation: // 53
                    TestProbe53GetPrecisorCapacitanceCompensation = TestProbe53GetPrecisorCapacitanceCompensation.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe53GetPrecisorCapacitanceCompensation));
                    UpdateGUIDisplay(GUIPage.PrecisorCompensationTabPage, CommandID, readDataBuffer);
                    break;
            }

            uint MessageCheckSum = 0;
            for (int i = 4; i < CommandParameterSize; i++)
            {                
                MessageCheckSum += readDataBuffer[i];
            }


            uint CalculatedMessageCheckSum = (uint)CommandType + (uint)CommandID + MessageCheckSum;
            byte AdjustedCalculatedMessageCheckSum = (byte)(CalculatedMessageCheckSum >> 24);
            byte CommandCheckSum = readDataBuffer[CommandParameterSize];

            CommandParameterInText = BitConverter.ToString(readDataBuffer, 0, CommandSize + 3);
            CommandParameterInText = CommandParameterInText.Replace('-', ' ');

            Log.Info("IncomingTestProbeData serial data", "Receiving command: {0}, parameter: {1}, message size: {2} from uProcessor. Calculated CheckSum: {3}, Received CheckSum: {4}.", CommandID, CommandParameterInText, CommandSize, AdjustedCalculatedMessageCheckSum, CommandCheckSum);

            if (AdjustedCalculatedMessageCheckSum > 0 && CommandCheckSum > 0)
            {
                if (AdjustedCalculatedMessageCheckSum != CommandCheckSum)
                {
                    Notify.PopUpError("CheckSum Mismatch Error", String.Format("Calculated CheckSum: {0}, Received CheckSum: {1}.", AdjustedCalculatedMessageCheckSum, CommandCheckSum));
                    Log.Error("CheckSum Mismatch Error", "Calculated CheckSum: {0}, Received CheckSum: {1}.", AdjustedCalculatedMessageCheckSum, CommandCheckSum);
                    return;
                }
            }

            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Dequeue();

            Thread.Sleep(50);

            commandSentToMicroprocessor = constructAndSendWriteDataBuffer();                        
        }

        private void UpdateGUIDisplay(GUIPage page, byte CommandID, byte[] data)
        {           
            if (this.InvokeRequired)
            {
                TestProbeGUIUpdateCallback d = new TestProbeGUIUpdateCallback(UpdateGUIDisplay);
                this.Invoke(d, new object[] { page, CommandID, data });
            }
            else
            {
                switch (page)
                {
                    case GUIPage.CommonCaptionBanner:
                        if (CommandID == (byte)MessageID.HST_get_product_id)
                        {
                            lblCommonProductID.Text = TestProbe7GetProductID.ProductID.ToString();
                            CommonFunctions.Instance.strProductID = lblCommonProductID.Text;
                        }
                        if (CommandID == (byte)MessageID.HST_get_operation_mode)
                        {
                            lblCommonOperationMode.Text = TestProbe8GetOperationMode.OperatingMode.ToString();
                        }
                        if (CommandID == (byte)MessageID.HST_get_firmware_version)
                        {
                            lblCommonFirmwareVersion.Text = "v" + TestProbe37GetFirmwareVersion.MajorRevision.ToString() + "." + TestProbe37GetFirmwareVersion.MinorRevision.ToString();
                        }
                        break;
                    case GUIPage.BenchTestsTabPage:

                        ShortDetection HGA1Short = ShortDetection.NoTest;
                        ShortDetection HGA2Short = ShortDetection.NoTest;
                        ShortDetection HGA3Short = ShortDetection.NoTest;
                        ShortDetection HGA4Short = ShortDetection.NoTest;
                        ShortDetection HGA5Short = ShortDetection.NoTest;
                        ShortDetection HGA6Short = ShortDetection.NoTest;
                        ShortDetection HGA7Short = ShortDetection.NoTest;
                        ShortDetection HGA8Short = ShortDetection.NoTest;
                        ShortDetection HGA9Short = ShortDetection.NoTest;
                        ShortDetection HGA10Short = ShortDetection.NoTest;
                        
                        if (CommandID == (byte)MessageID.HST_get_res_results)
                        {
                            // HGA1 
                            double HGA1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA1WriterResistance() / 1000.0);
                            double HGA1TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA1TAResistance() / 1000.0);
                            double HGA1WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA1WHResistance() / 1000.0);
                            double HGA1RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA1RHResistance() / 1000.0);
                            double HGA1R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA1R1Resistance() / 1000.0);
                            double HGA1R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA1R2Resistance() / 1000.0);
                            txtBenchTestsHGA1Ch1Res.Text = HGA1WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA1Ch2Res.Text = HGA1TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA1Ch3Res.Text = HGA1WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA1Ch4Res.Text = HGA1RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA1Ch5Res.Text = HGA1R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA1Ch6Res.Text = HGA1R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA2
                            double HGA2WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA2WriterResistance() / 1000.0);
                            double HGA2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA2TAResistance() / 1000.0);
                            double HGA2WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA2WHResistance() / 1000.0);
                            double HGA2RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA2RHResistance() / 1000.0);
                            double HGA2R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA2R1Resistance() / 1000.0);
                            double HGA2R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA2R2Resistance() / 1000.0);
                            txtBenchTestsHGA2Ch1Res.Text = HGA2WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA2Ch2Res.Text = HGA2TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA2Ch3Res.Text = HGA2WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA2Ch4Res.Text = HGA2RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA2Ch5Res.Text = HGA2R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA2Ch6Res.Text = HGA2R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA3
                            double HGA3WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA3WriterResistance() / 1000.0);
                            double HGA3TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA3TAResistance() / 1000.0);
                            double HGA3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA3WHResistance() / 1000.0);
                            double HGA3RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA3RHResistance() / 1000.0);
                            double HGA3R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA3R1Resistance() / 1000.0);
                            double HGA3R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA3R2Resistance() / 1000.0);
                            txtBenchTestsHGA3Ch1Res.Text = HGA3WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA3Ch2Res.Text = HGA3TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA3Ch3Res.Text = HGA3WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA3Ch4Res.Text = HGA3RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA3Ch5Res.Text = HGA3R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA3Ch6Res.Text = HGA3R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA4
                            double HGA4WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA4WriterResistance() / 1000.0);
                            double HGA4TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA4TAResistance() / 1000.0);
                            double HGA4WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA4WHResistance() / 1000.0);
                            double HGA4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA4RHResistance() / 1000.0);
                            double HGA4R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA4R1Resistance() / 1000.0);
                            double HGA4R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA4R2Resistance() / 1000.0);
                            txtBenchTestsHGA4Ch1Res.Text = HGA4WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA4Ch2Res.Text = HGA4TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA4Ch3Res.Text = HGA4WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA4Ch4Res.Text = HGA4RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA4Ch5Res.Text = HGA4R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA4Ch6Res.Text = HGA4R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA5
                            double HGA5WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA5WriterResistance() / 1000.0);
                            double HGA5TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA5TAResistance() / 1000.0);
                            double HGA5WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA5WHResistance() / 1000.0);
                            double HGA5RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA5RHResistance() / 1000.0);
                            double HGA5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA5R1Resistance() / 1000.0);
                            double HGA5R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA5R2Resistance() / 1000.0);
                            txtBenchTestsHGA5Ch1Res.Text = HGA5WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA5Ch2Res.Text = HGA5TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA5Ch3Res.Text = HGA5WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA5Ch4Res.Text = HGA5RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA5Ch5Res.Text = HGA5R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA5Ch6Res.Text = HGA5R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA6
                            double HGA6WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA6WriterResistance() / 1000.0);
                            double HGA6TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA6TAResistance() / 1000.0);
                            double HGA6WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA6WHResistance() / 1000.0);
                            double HGA6RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA6RHResistance() / 1000.0);
                            double HGA6R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA6R1Resistance() / 1000.0);
                            double HGA6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA6R2Resistance() / 1000.0);
                            txtBenchTestsHGA6Ch1Res.Text = HGA6WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA6Ch2Res.Text = HGA6TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA6Ch3Res.Text = HGA6WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA6Ch4Res.Text = HGA6RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA6Ch5Res.Text = HGA6R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA6Ch6Res.Text = HGA6R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA7
                            double HGA7WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA7WriterResistance() / 1000.0);
                            double HGA7TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA7TAResistance() / 1000.0);
                            double HGA7WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA7WHResistance() / 1000.0);
                            double HGA7RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA7RHResistance() / 1000.0);
                            double HGA7R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA7R1Resistance() / 1000.0);
                            double HGA7R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA7R2Resistance() / 1000.0);
                            txtBenchTestsHGA7Ch1Res.Text = HGA7WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA7Ch2Res.Text = HGA7TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA7Ch3Res.Text = HGA7WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA7Ch4Res.Text = HGA7RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA7Ch5Res.Text = HGA7R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA7Ch6Res.Text = HGA7R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA8
                            double HGA8WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA8WriterResistance() / 1000.0);
                            double HGA8TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA8TAResistance() / 1000.0);
                            double HGA8WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA8WHResistance() / 1000.0);
                            double HGA8RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA8RHResistance() / 1000.0);
                            double HGA8R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA8R1Resistance() / 1000.0);
                            double HGA8R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA8R2Resistance() / 1000.0);
                            txtBenchTestsHGA8Ch1Res.Text = HGA8WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA8Ch2Res.Text = HGA8TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA8Ch3Res.Text = HGA8WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA8Ch4Res.Text = HGA8RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA8Ch5Res.Text = HGA8R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA8Ch6Res.Text = HGA8R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA9
                            double HGA9WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA9WriterResistance() / 1000.0);
                            double HGA9TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA9TAResistance() / 1000.0);
                            double HGA9WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA9WHResistance() / 1000.0);
                            double HGA9RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA9RHResistance() / 1000.0);
                            double HGA9R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA9R1Resistance() / 1000.0);
                            double HGA9R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA9R2Resistance() / 1000.0);
                            txtBenchTestsHGA9Ch1Res.Text = HGA9WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA9Ch2Res.Text = HGA9TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA9Ch3Res.Text = HGA9WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA9Ch4Res.Text = HGA9RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA9Ch5Res.Text = HGA9R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA9Ch6Res.Text = HGA9R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            // HGA10
                            double HGA10WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA10WriterResistance() / 1000.0);
                            double HGA10TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA10TAResistance() / 1000.0);
                            double HGA10WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA10WHResistance() / 1000.0);
                            double HGA10RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA10RHResistance() / 1000.0);
                            double HGA10R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA10R1Resistance() / 1000.0);
                            double HGA10R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA10R2Resistance() / 1000.0);
                            txtBenchTestsHGA10Ch1Res.Text = HGA10WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA10Ch2Res.Text = HGA10TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA10Ch3Res.Text = HGA10WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA10Ch4Res.Text = HGA10RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA10Ch5Res.Text = HGA10R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtBenchTestsHGA10Ch6Res.Text = HGA10R2Resistance.ToString("F3", CultureInfo.InvariantCulture);
                        }

                        if (CommandID == (byte)MessageID.HST_get_cap_results)
                        {
                            txtBenchTestsHGA1Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA1Capacitance1().ToString();
                            txtBenchTestsHGA1Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA1Capacitance2().ToString();

                            txtBenchTestsHGA2Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA2Capacitance1().ToString();
                            txtBenchTestsHGA2Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA2Capacitance2().ToString();

                            txtBenchTestsHGA3Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA3Capacitance1().ToString();
                            txtBenchTestsHGA3Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA3Capacitance2().ToString();

                            txtBenchTestsHGA4Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA4Capacitance1().ToString();
                            txtBenchTestsHGA4Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA4Capacitance2().ToString();

                            txtBenchTestsHGA5Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA5Capacitance1().ToString();
                            txtBenchTestsHGA5Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA5Capacitance2().ToString();

                            txtBenchTestsHGA6Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA6Capacitance1().ToString();
                            txtBenchTestsHGA6Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA6Capacitance2().ToString();

                            txtBenchTestsHGA7Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA7Capacitance1().ToString();
                            txtBenchTestsHGA7Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA7Capacitance2().ToString();

                            txtBenchTestsHGA8Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA8Capacitance1().ToString();
                            txtBenchTestsHGA8Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA8Capacitance2().ToString();

                            txtBenchTestsHGA9Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA9Capacitance1().ToString();
                            txtBenchTestsHGA9Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA9Capacitance2().ToString();

                            txtBenchTestsHGA10Ch1Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA10Capacitance1().ToString();
                            txtBenchTestsHGA10Ch2Capa.Text = TestProbe12GetAllHGACapacitanceResults.HGA10Capacitance2().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_cap_secondary_results)
                        {
                            txtBenchTestsHGA1Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA1ESR1().ToString();
                            txtBenchTestsHGA1Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA1ESR2().ToString();

                            txtBenchTestsHGA2Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA2ESR1().ToString();
                            txtBenchTestsHGA2Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA2ESR2().ToString();

                            txtBenchTestsHGA3Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA3ESR1().ToString();
                            txtBenchTestsHGA3Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA3ESR2().ToString();

                            txtBenchTestsHGA4Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA4ESR1().ToString();
                            txtBenchTestsHGA4Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA4ESR2().ToString();

                            txtBenchTestsHGA5Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA5ESR1().ToString();
                            txtBenchTestsHGA5Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA5ESR2().ToString();

                            txtBenchTestsHGA6Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA6ESR1().ToString();
                            txtBenchTestsHGA6Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA6ESR2().ToString();

                            txtBenchTestsHGA7Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA7ESR1().ToString();
                            txtBenchTestsHGA7Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA7ESR2().ToString();

                            txtBenchTestsHGA8Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA8ESR1().ToString();
                            txtBenchTestsHGA8Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA8ESR2().ToString();

                            txtBenchTestsHGA9Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA9ESR1().ToString();
                            txtBenchTestsHGA9Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA9ESR2().ToString();

                            txtBenchTestsHGA10Ch1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA10ESR1().ToString();
                            txtBenchTestsHGA10Ch2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA10ESR2().ToString();
                        }                        

                        if (CommandID == (byte)MessageID.HST_get_short_detection)
                        {
                            if (TestProbe10GetAllHGAShortDetection.HGA1WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA1R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA1Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA1Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA2WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA2R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA2Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA2Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA3WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA3R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA3Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA3Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA4WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA4R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA4Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA4Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA5WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA5R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA5Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA5Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA6WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA6R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA6Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA6Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA7WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA7R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA7Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA7Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA8WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA8R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA8Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA8Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA9WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA9R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA9Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA9Short = ShortDetection.Open;
                            }

                            if (TestProbe10GetAllHGAShortDetection.HGA10WPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10WMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10TAPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10TAMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10WHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10WHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10RHPlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10RHMinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10R1PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10R1MinusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10R2PlusPad == (int)ShortDetection.Short ||
                                TestProbe10GetAllHGAShortDetection.HGA10R2MinusPad == (int)ShortDetection.Short)
                            {
                                HGA10Short = ShortDetection.Short;
                            }
                            else
                            {
                                HGA10Short = ShortDetection.Open;
                            }

                            // HGA1
                            if (HGA1Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA1Short.Text = "Short";
                            }
                            else if (HGA1Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA1Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA1Short.Text = "No Test";
                            }

                            // HGA2
                            if (HGA2Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA2Short.Text = "Short";
                            }
                            else if (HGA2Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA2Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA2Short.Text = "No Test";
                            }

                            // HGA3
                            if (HGA3Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA3Short.Text = "Short";
                            }
                            else if (HGA3Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA3Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA3Short.Text = "No Test";
                            }

                            // HGA4
                            if (HGA4Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA4Short.Text = "Short";
                            }
                            else if (HGA4Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA4Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA4Short.Text = "No Test";
                            }

                            // HGA5
                            if (HGA5Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA5Short.Text = "Short";
                            }
                            else if (HGA5Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA5Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA5Short.Text = "No Test";
                            }

                            // HGA6
                            if (HGA6Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA6Short.Text = "Short";
                            }
                            else if (HGA6Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA6Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA6Short.Text = "No Test";
                            }

                            // HGA7
                            if (HGA7Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA7Short.Text = "Short";
                            }
                            else if (HGA7Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA7Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA7Short.Text = "No Test";
                            }

                            // HGA8
                            if (HGA8Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA8Short.Text = "Short";
                            }
                            else if (HGA8Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA8Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA8Short.Text = "No Test";
                            }

                            // HGA9
                            if (HGA9Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA9Short.Text = "Short";
                            }
                            else if (HGA9Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA9Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA9Short.Text = "No Test";
                            }

                            // HGA10
                            if (HGA10Short == ShortDetection.Short)
                            {
                                txtBenchTestsHGA10Short.Text = "Short";
                            }
                            else if (HGA10Short == ShortDetection.Open)
                            {
                                txtBenchTestsHGA10Short.Text = "Open";
                            }
                            else
                            {
                                txtBenchTestsHGA10Short.Text = "No Test";
                            }
                        }

                        break;
                    case GUIPage.FunctionalTestsTabPage: 
                        if (CommandID == (byte)MessageID.HST_start_self_test)
                        {                            
                            // 0Ohm
                            double Ch1WriterResistance0Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance0Ohm() / 1000.0);
                            double Ch2TAResistance0Ohm = (TestProbe36StartSelfTest.Ch2TAResistance0Ohm() / 1000.0);
                            double Ch3WHResistance0Ohm = (TestProbe36StartSelfTest.Ch3WHResistance0Ohm() / 1000.0);
                            double Ch4RHResistance0Ohm = (TestProbe36StartSelfTest.Ch4RHResistance0Ohm() / 1000.0);
                            double Ch5R1Resistance0Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance0Ohm() / 1000.0);
                            double Ch6R2Resistance0Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance0Ohm() / 1000.0);

                            txtFunctionalTests0CH1.Text = Ch1WriterResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests0CH2.Text = Ch2TAResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests0CH3.Text = Ch3WHResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests0CH4.Text = Ch4RHResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests0CH5.Text = Ch5R1Resistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests0CH6.Text = Ch6R2Resistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 10Ohm
                            double Ch1WriterResistance10Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance10Ohm() / 1000.0);
                            double Ch2TAResistance10Ohm = (TestProbe36StartSelfTest.Ch2TAResistance10Ohm() / 1000.0);
                            double Ch3WHResistance10Ohm = (TestProbe36StartSelfTest.Ch3WHResistance10Ohm() / 1000.0);
                            double Ch4RHResistance10Ohm = (TestProbe36StartSelfTest.Ch4RHResistance10Ohm() / 1000.0);
                            double Ch5R1Resistance10Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance10Ohm() / 1000.0);
                            double Ch6R2Resistance10Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance10Ohm() / 1000.0);

                            txtFunctionalTests10CH1.Text = Ch1WriterResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10CH2.Text = Ch2TAResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10CH3.Text = Ch3WHResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10CH4.Text = Ch4RHResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10CH5.Text = Ch5R1Resistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10CH6.Text = Ch6R2Resistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 100Ohm
                            double Ch1WriterResistance100Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance100Ohm() / 1000.0);
                            double Ch2TAResistance100Ohm = (TestProbe36StartSelfTest.Ch2TAResistance100Ohm() / 1000.0);
                            double Ch3WHResistance100Ohm = (TestProbe36StartSelfTest.Ch3WHResistance100Ohm() / 1000.0);
                            double Ch4RHResistance100Ohm = (TestProbe36StartSelfTest.Ch4RHResistance100Ohm() / 1000.0);
                            double Ch5R1Resistance100Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance100Ohm() / 1000.0);
                            double Ch6R2Resistance100Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance100Ohm() / 1000.0);

                            txtFunctionalTests100CH1.Text = Ch1WriterResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests100CH2.Text = Ch2TAResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests100CH3.Text = Ch3WHResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests100CH4.Text = Ch4RHResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests100CH5.Text = Ch5R1Resistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests100CH6.Text = Ch6R2Resistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 500Ohm
                            double Ch1WriterResistance500Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance500Ohm() / 1000.0);
                            double Ch2TAResistance500Ohm = (TestProbe36StartSelfTest.Ch2TAResistance500Ohm() / 1000.0);
                            double Ch3WHResistance500Ohm = (TestProbe36StartSelfTest.Ch3WHResistance500Ohm() / 1000.0);
                            double Ch4RHResistance500Ohm = (TestProbe36StartSelfTest.Ch4RHResistance500Ohm() / 1000.0);
                            double Ch5R1Resistance500Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance500Ohm() / 1000.0);
                            double Ch6R2Resistance500Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance500Ohm() / 1000.0);

                            txtFunctionalTests500CH1.Text = Ch1WriterResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests500CH2.Text = Ch2TAResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests500CH3.Text = Ch3WHResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests500CH4.Text = Ch4RHResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests500CH5.Text = Ch5R1Resistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests500CH6.Text = Ch6R2Resistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 1000Ohm
                            double Ch1WriterResistance1000Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance1000Ohm() / 1000.0);
                            double Ch2TAResistance1000Ohm = (TestProbe36StartSelfTest.Ch2TAResistance1000Ohm() / 1000.0);
                            double Ch3WHResistance1000Ohm = (TestProbe36StartSelfTest.Ch3WHResistance1000Ohm() / 1000.0);
                            double Ch4RHResistance1000Ohm = (TestProbe36StartSelfTest.Ch4RHResistance1000Ohm() / 1000.0);
                            double Ch5R1Resistance1000Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance1000Ohm() / 1000.0);
                            double Ch6R2Resistance1000Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance1000Ohm() / 1000.0);

                            txtFunctionalTests1000CH1.Text = Ch1WriterResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests1000CH2.Text = Ch2TAResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests1000CH3.Text = Ch3WHResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests1000CH4.Text = Ch4RHResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests1000CH5.Text = Ch5R1Resistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests1000CH6.Text = Ch6R2Resistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 10000Ohm
                            double Ch1WriterResistance10000Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance10000Ohm() / 1000.0);
                            double Ch2TAResistance10000Ohm = (TestProbe36StartSelfTest.Ch2TAResistance10000Ohm() / 1000.0);
                            double Ch3WHResistance10000Ohm = (TestProbe36StartSelfTest.Ch3WHResistance10000Ohm() / 1000.0);
                            double Ch4RHResistance10000Ohm = (TestProbe36StartSelfTest.Ch4RHResistance10000Ohm() / 1000.0);
                            double Ch5R1Resistance10000Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance10000Ohm() / 1000.0);
                            double Ch6R2Resistance10000Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance10000Ohm() / 1000.0);

                            txtFunctionalTests10000CH1.Text = Ch1WriterResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10000CH2.Text = Ch2TAResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10000CH3.Text = Ch3WHResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10000CH4.Text = Ch4RHResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10000CH5.Text = Ch5R1Resistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            txtFunctionalTests10000CH6.Text = Ch6R2Resistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // Capacitance
                            txtFunctionalTests100Capa.Text = TestProbe36StartSelfTest.Capacitance100pF().ToString();
                            txtFunctionalTests270Capa.Text = TestProbe36StartSelfTest.Capacitance270pF().ToString();
                            txtFunctionalTests470Capa.Text = TestProbe36StartSelfTest.Capacitance470pF().ToString();
                            txtFunctionalTests680Capa.Text = TestProbe36StartSelfTest.Capacitance680pF().ToString();
                            txtFunctionalTests820Capa.Text = TestProbe36StartSelfTest.Capacitance820pF().ToString();
                            txtFunctionalTests10000Capa.Text = TestProbe36StartSelfTest.Capacitance10nF().ToString();

                            // 0Ohm
                            double Ch1WriterResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm / 1000 * 0.5 / 100) + 0.25;

                            if( ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm / 1000) - Ch1WriterResistance0OhmDeviation <= Ch1WriterResistance0Ohm) && (Ch1WriterResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm / 1000) + Ch1WriterResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm / 1000) - Ch2TAResistance0OhmDeviation <= Ch2TAResistance0Ohm) && (Ch2TAResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm / 1000) + Ch2TAResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm / 1000) - Ch3WHResistance0OhmDeviation <= Ch3WHResistance0Ohm) && (Ch3WHResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm / 1000) + Ch3WHResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm / 1000) - Ch4RHResistance0OhmDeviation <= Ch4RHResistance0Ohm) && (Ch4RHResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm / 1000) + Ch4RHResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm / 1000) - Ch5R1Resistance0OhmDeviation <= Ch5R1Resistance0Ohm) && (Ch5R1Resistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm / 1000) + Ch5R1Resistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm / 1000) - Ch6R2Resistance0OhmDeviation <= Ch6R2Resistance0Ohm) && (Ch6R2Resistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm / 1000) + Ch6R2Resistance0OhmDeviation) )
                            {
                                txtFunctionalTests0RestResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests0RestResults.Text = "FAIL";
                            }

                            // 10Ohm
                            double Ch1WriterResistance10OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance10OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance10OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance10OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance10OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance10OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10Ohm / 1000 * 0.5 / 100) + 0.25;

                            if (((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10Ohm / 1000) - Ch1WriterResistance10OhmDeviation <= Ch1WriterResistance10Ohm) && (Ch1WriterResistance10Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10Ohm / 1000) + Ch1WriterResistance10OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10Ohm / 1000) - Ch2TAResistance10OhmDeviation <= Ch2TAResistance10Ohm) && (Ch2TAResistance10Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10Ohm / 1000) + Ch2TAResistance10OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10Ohm / 1000) - Ch3WHResistance10OhmDeviation <= Ch3WHResistance10Ohm) && (Ch3WHResistance10Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10Ohm / 1000) + Ch3WHResistance10OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10Ohm / 1000) - Ch4RHResistance10OhmDeviation <= Ch4RHResistance10Ohm) && (Ch4RHResistance10Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10Ohm / 1000) + Ch4RHResistance10OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10Ohm / 1000) - Ch5R1Resistance10OhmDeviation <= Ch5R1Resistance10Ohm) && (Ch5R1Resistance10Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10Ohm / 1000) + Ch5R1Resistance10OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10Ohm / 1000) - Ch6R2Resistance10OhmDeviation <= Ch6R2Resistance10Ohm) && (Ch6R2Resistance10Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10Ohm / 1000) + Ch6R2Resistance10OhmDeviation))
                            {
                                txtFunctionalTests10RestResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests10RestResults.Text = "FAIL";
                            }

                            // 100Ohm
                            double Ch1WriterResistance100OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance100Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance100OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance100Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance100OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance100Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance100OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance100Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance100OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance100Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance100OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance100Ohm / 1000 * 0.5 / 100) + 0.25;

                            if (((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance100Ohm / 1000) - Ch1WriterResistance100OhmDeviation <= Ch1WriterResistance100Ohm) && (Ch1WriterResistance100Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance100Ohm / 1000) + Ch1WriterResistance100OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance100Ohm / 1000) - Ch2TAResistance100OhmDeviation <= Ch2TAResistance100Ohm) && (Ch2TAResistance100Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance100Ohm / 1000) + Ch2TAResistance100OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance100Ohm / 1000) - Ch3WHResistance100OhmDeviation <= Ch3WHResistance100Ohm) && (Ch3WHResistance100Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance100Ohm / 1000) + Ch3WHResistance100OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance100Ohm / 1000) - Ch4RHResistance100OhmDeviation <= Ch4RHResistance100Ohm) && (Ch4RHResistance100Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance100Ohm / 1000) + Ch4RHResistance100OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance100Ohm / 1000) - Ch5R1Resistance100OhmDeviation <= Ch5R1Resistance100Ohm) && (Ch5R1Resistance100Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance100Ohm / 1000) + Ch5R1Resistance100OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance100Ohm / 1000) - Ch6R2Resistance100OhmDeviation <= Ch6R2Resistance100Ohm) && (Ch6R2Resistance100Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance100Ohm / 1000) + Ch6R2Resistance100OhmDeviation))
                            {
                                txtFunctionalTests100RestResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests100RestResults.Text = "FAIL";
                            }

                            // 500Ohm
                            double Ch1WriterResistance500OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance500Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance500OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance500Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance500OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance500Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance500OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance500Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance500OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance500Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance500OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance500Ohm / 1000 * 0.5 / 100) + 0.25;

                            if (((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance500Ohm / 1000) - Ch1WriterResistance500OhmDeviation <= Ch1WriterResistance500Ohm) && (Ch1WriterResistance500Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance500Ohm / 1000) + Ch1WriterResistance500OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance500Ohm / 1000) - Ch2TAResistance500OhmDeviation <= Ch2TAResistance500Ohm) && (Ch2TAResistance500Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance500Ohm / 1000) + Ch2TAResistance500OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance500Ohm / 1000) - Ch3WHResistance500OhmDeviation <= Ch3WHResistance500Ohm) && (Ch3WHResistance500Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance500Ohm / 1000) + Ch3WHResistance500OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance500Ohm / 1000) - Ch4RHResistance500OhmDeviation <= Ch4RHResistance500Ohm) && (Ch4RHResistance500Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance500Ohm / 1000) + Ch4RHResistance500OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance500Ohm / 1000) - Ch5R1Resistance500OhmDeviation <= Ch5R1Resistance500Ohm) && (Ch5R1Resistance500Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance500Ohm / 1000) + Ch5R1Resistance500OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance500Ohm / 1000) - Ch6R2Resistance500OhmDeviation <= Ch6R2Resistance500Ohm) && (Ch6R2Resistance500Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance500Ohm / 1000) + Ch6R2Resistance500OhmDeviation))
                            {
                                txtFunctionalTests500RestResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests500RestResults.Text = "FAIL";
                            }

                            // 1000Ohm
                            double Ch1WriterResistance1000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance1000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance1000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance1000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance1000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance1000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance1000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance1000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance1000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance1000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance1000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance1000Ohm / 1000 * 0.5 / 100) + 0.25;

                            if (((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance1000Ohm / 1000) - Ch1WriterResistance1000OhmDeviation <= Ch1WriterResistance1000Ohm) && (Ch1WriterResistance1000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance1000Ohm / 1000) + Ch1WriterResistance1000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance1000Ohm / 1000) - Ch2TAResistance1000OhmDeviation <= Ch2TAResistance1000Ohm) && (Ch2TAResistance1000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance1000Ohm / 1000) + Ch2TAResistance1000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance1000Ohm / 1000) - Ch3WHResistance1000OhmDeviation <= Ch3WHResistance1000Ohm) && (Ch3WHResistance1000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance1000Ohm / 1000) + Ch3WHResistance1000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance1000Ohm / 1000) - Ch4RHResistance1000OhmDeviation <= Ch4RHResistance1000Ohm) && (Ch4RHResistance1000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance1000Ohm / 1000) + Ch4RHResistance1000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance1000Ohm / 1000) - Ch5R1Resistance1000OhmDeviation <= Ch5R1Resistance1000Ohm) && (Ch5R1Resistance1000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance1000Ohm / 1000) + Ch5R1Resistance1000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance1000Ohm / 1000) - Ch6R2Resistance1000OhmDeviation <= Ch6R2Resistance1000Ohm) && (Ch6R2Resistance1000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance1000Ohm / 1000) + Ch6R2Resistance1000OhmDeviation))
                            {
                                txtFunctionalTests1000RestResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests1000RestResults.Text = "FAIL";
                            }

                            // 10000Ohm0
                            double Ch1WriterResistance10000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance10000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance10000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance10000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance10000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10000Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance10000OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10000Ohm / 1000 * 0.5 / 100) + 0.25;

                            if (((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10000Ohm / 1000) - Ch1WriterResistance10000OhmDeviation <= Ch1WriterResistance10000Ohm) && (Ch1WriterResistance10000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10000Ohm / 1000) + Ch1WriterResistance10000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10000Ohm / 1000) - Ch2TAResistance10000OhmDeviation <= Ch2TAResistance10000Ohm) && (Ch2TAResistance10000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10000Ohm / 1000) + Ch2TAResistance10000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10000Ohm / 1000) - Ch3WHResistance10000OhmDeviation <= Ch3WHResistance10000Ohm) && (Ch3WHResistance10000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10000Ohm / 1000) + Ch3WHResistance10000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10000Ohm / 1000) - Ch4RHResistance10000OhmDeviation <= Ch4RHResistance10000Ohm) && (Ch4RHResistance10000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10000Ohm / 1000) + Ch4RHResistance10000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10000Ohm / 1000) - Ch5R1Resistance10000OhmDeviation <= Ch5R1Resistance10000Ohm) && (Ch5R1Resistance10000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10000Ohm / 1000) + Ch5R1Resistance10000OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10000Ohm / 1000) - Ch6R2Resistance10000OhmDeviation <= Ch6R2Resistance10000Ohm) && (Ch6R2Resistance10000Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10000Ohm / 1000) + Ch6R2Resistance10000OhmDeviation))
                            {
                                txtFunctionalTests10000RestResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests10000RestResults.Text = "FAIL";
                            }

                            // Capacitance
                            double Capacitance100pFDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance100pF * 1.5 / 100) + 10;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance100pF - Capacitance100pFDeviation <= TestProbe36StartSelfTest.Capacitance100pF()) && (TestProbe36StartSelfTest.Capacitance100pF() <= CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance100pF + Capacitance100pFDeviation))
                            {
                                txtFunctionalTests100CapaResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests100CapaResults.Text = "FAIL";
                            }

                            double Capacitance270pFDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance270pF * 1.5 / 100) + 10;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance270pF - Capacitance270pFDeviation <= TestProbe36StartSelfTest.Capacitance270pF()) && (TestProbe36StartSelfTest.Capacitance270pF() <= CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance270pF + Capacitance270pFDeviation))
                            {
                                txtFunctionalTests270CapaResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests270CapaResults.Text = "FAIL";
                            }

                            double Capacitance470pFDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance470pF * 1.5 / 100) + 10;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance470pF - Capacitance470pFDeviation <= TestProbe36StartSelfTest.Capacitance470pF()) && (TestProbe36StartSelfTest.Capacitance470pF() <= CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance470pF + Capacitance470pFDeviation))
                            {
                                txtFunctionalTests470CapaResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests470CapaResults.Text = "FAIL";
                            }

                            double Capacitance680pFDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance680pF * 1.5 / 100) + 10;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance680pF - Capacitance680pFDeviation <= TestProbe36StartSelfTest.Capacitance680pF()) && (TestProbe36StartSelfTest.Capacitance680pF() <= CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance680pF + Capacitance680pFDeviation))
                            {
                                txtFunctionalTests680CapaResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests680CapaResults.Text = "FAIL";
                            }

                            double Capacitance820pFDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance820pF * 1.5 / 100) + 10;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance820pF - Capacitance820pFDeviation <= TestProbe36StartSelfTest.Capacitance820pF()) && (TestProbe36StartSelfTest.Capacitance820pF() <= CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance820pF + Capacitance820pFDeviation))
                            {
                                txtFunctionalTests820CapaResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests820CapaResults.Text = "FAIL";
                            }

                            double Capacitance10nFDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance10nF * 1.5 / 100) + 10;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance10nF - Capacitance10nFDeviation <= TestProbe36StartSelfTest.Capacitance10nF()) && (TestProbe36StartSelfTest.Capacitance10nF() <= CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance10nF + Capacitance10nFDeviation))
                            {
                                txtFunctionalTests10000CapaResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests10000CapaResults.Text = "FAIL";
                            }
                        }

                        if (CommandID == (byte)MessageID.HST_get_temperature)
                        {
                            double Ch1Temperature0 = (TestProbe33GetTemperature.Ch1Temperature() / 10.0);
                            txtFunctionalTests0Temp.Text = Ch1Temperature0.ToString("F1", CultureInfo.InvariantCulture);

                            //Ch1 Temperature
                            double Ch1TemperatureDeviation = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1Temperature / 10 * 0.5 / 100;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1Temperature / 10 - Ch1TemperatureDeviation <= Ch1Temperature0) && (Ch1Temperature0 <= CommonFunctions.Instance.FunctionalTestsRecipe.Ch1Temperature / 10 + Ch1TemperatureDeviation))
                            {
                                txtFunctionalTests0TempResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests0TempResults.Text = "FAIL";
                            }

                            double Ch2Temperature50 = (TestProbe33GetTemperature.Ch2Temperature() / 10.0);
                            txtFunctionalTests50Temp.Text = Ch2Temperature50.ToString("F1", CultureInfo.InvariantCulture);

                            //Ch2 Temperature
                            double Ch2TemperatureDeviation = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2Temperature / 10 * 0.5 / 100;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1Temperature / 10 - Ch2TemperatureDeviation <= Ch2Temperature50) && (Ch2Temperature50 <= CommonFunctions.Instance.FunctionalTestsRecipe.Ch2Temperature / 10 + Ch2TemperatureDeviation))
                            {
                                txtFunctionalTests50TempResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests50TempResults.Text = "FAIL";
                            }
                            double Ch3Temperature100 = (TestProbe33GetTemperature.Ch3Temperature() / 10.0);
                            txtFunctionalTests100Temp.Text = Ch3Temperature100.ToString("F1", CultureInfo.InvariantCulture);

                            //Ch3 Temperature
                            double Ch3TemperatureDeviation = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3Temperature / 10 * 0.5 / 100;
                            if ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3Temperature / 10 - Ch3TemperatureDeviation <= Ch3Temperature100) && (Ch3Temperature100 <= CommonFunctions.Instance.FunctionalTestsRecipe.Ch3Temperature / 10 + Ch3TemperatureDeviation))
                            {
                                txtFunctionalTests100TempResults.Text = "PASS";
                            }
                            else
                            {
                                txtFunctionalTests100TempResults.Text = "FAIL";
                            }                           
                        }

                        break;
                    case GUIPage.ConfigurationSetupTabPage:
                        // Nothing returned from the uProcessor
                        break;
                    case GUIPage.PCBACalibrationTabPage:
                        if (CommandID == (byte)MessageID.HST_start_auto_calibration || 
                            CommandID == (byte)MessageID.HST_manual_set_calibration ||
                            CommandID == (byte)MessageID.HST_get_calibration_data)
                        {
                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "0R", true) == 0)
                            {
                               double MeasuredValue = (TestProbe21GetManualCalibration.MeasuredValue() / 1000.0);
                               if(rdxPCBACalibrationManualCalibrationCh1.Checked)
                               {
                                   txtPCBACalibration0Ch1.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                               }
                               else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
                               {
                                   txtPCBACalibration0Ch2.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                               }
                               else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
                               {
                                   txtPCBACalibration0Ch3.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                               }
                               else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
                               {
                                   txtPCBACalibration0Ch4.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                               }
                               else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
                               {
                                   txtPCBACalibration0Ch5.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                               }
                               else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
                               {
                                   txtPCBACalibration0Ch6.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                               }
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //0Ohm
                                double Ch1Writer0Ohm = (TestProbe18StartAutoCalibration.Ch1Writer0Ohm() / 1000.0);
                                double Ch2TA0Ohm = (TestProbe18StartAutoCalibration.Ch2TA0Ohm() / 1000.0);
                                double Ch3WH0Ohm = (TestProbe18StartAutoCalibration.Ch3WH0Ohm() / 1000.0);
                                double Ch4RH0Ohm = (TestProbe18StartAutoCalibration.Ch4RH0Ohm() / 1000.0);
                                double Ch5R10Ohm = (TestProbe18StartAutoCalibration.Ch5R10Ohm() / 1000.0);
                                double Ch6R20Ohm = (TestProbe18StartAutoCalibration.Ch6R20Ohm() / 1000.0);
                                txtPCBACalibration0Ch1.Text = Ch1Writer0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch2.Text = Ch2TA0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch3.Text = Ch3WH0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch4.Text = Ch4RH0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch5.Text = Ch5R10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch6.Text = Ch6R20Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //0Ohm
                                double Ch1Writer0Ohm = (TestProbe20GetCalibrationData.Ch1Writer0Ohm() / 1000.0);
                                double Ch2TA0Ohm = (TestProbe20GetCalibrationData.Ch2TA0Ohm() / 1000.0);
                                double Ch3WH0Ohm = (TestProbe20GetCalibrationData.Ch3WH0Ohm() / 1000.0);
                                double Ch4RH0Ohm = (TestProbe20GetCalibrationData.Ch4RH0Ohm() / 1000.0);
                                double Ch5R10Ohm = (TestProbe20GetCalibrationData.Ch5R10Ohm() / 1000.0);
                                double Ch6R20Ohm = (TestProbe20GetCalibrationData.Ch6R20Ohm() / 1000.0);
                                txtPCBACalibration0Ch1.Text = Ch1Writer0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch2.Text = Ch2TA0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch3.Text = Ch3WH0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch4.Text = Ch4RH0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch5.Text = Ch5R10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration0Ch6.Text = Ch6R20Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "10R", true) == 0)
                            {
                                double MeasuredValue = (TestProbe21GetManualCalibration.MeasuredValue() / 1000.0);
                                if (rdxPCBACalibrationManualCalibrationCh1.Checked)
                                {
                                    txtPCBACalibration10Ch1.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
                                {
                                    txtPCBACalibration10Ch2.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
                                {
                                    txtPCBACalibration10Ch3.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
                                {
                                    txtPCBACalibration10Ch4.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
                                {
                                    txtPCBACalibration10Ch5.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
                                {
                                    txtPCBACalibration10Ch6.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //10Ohm
                                double Ch1Writer10Ohm = (TestProbe18StartAutoCalibration.Ch1Writer10Ohm() / 1000.0);
                                double Ch2TA10Ohm = (TestProbe18StartAutoCalibration.Ch2TA10Ohm() / 1000.0);
                                double Ch3WH10Ohm = (TestProbe18StartAutoCalibration.Ch3WH10Ohm() / 1000.0);
                                double Ch4RH10Ohm = (TestProbe18StartAutoCalibration.Ch4RH10Ohm() / 1000.0);
                                double Ch5R110Ohm = (TestProbe18StartAutoCalibration.Ch5R110Ohm() / 1000.0);
                                double Ch6R210Ohm = (TestProbe18StartAutoCalibration.Ch6R210Ohm() / 1000.0);
                                txtPCBACalibration10Ch1.Text = Ch1Writer10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch2.Text = Ch2TA10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch3.Text = Ch3WH10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch4.Text = Ch4RH10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch5.Text = Ch5R110Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch6.Text = Ch6R210Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //10Ohm
                                double Ch1Writer10Ohm = (TestProbe20GetCalibrationData.Ch1Writer10Ohm() / 1000.0);
                                double Ch2TA10Ohm = (TestProbe20GetCalibrationData.Ch2TA10Ohm() / 1000.0);
                                double Ch3WH10Ohm = (TestProbe20GetCalibrationData.Ch3WH10Ohm() / 1000.0);
                                double Ch4RH10Ohm = (TestProbe20GetCalibrationData.Ch4RH10Ohm() / 1000.0);
                                double Ch5R110Ohm = (TestProbe20GetCalibrationData.Ch5R110Ohm() / 1000.0);
                                double Ch6R210Ohm = (TestProbe20GetCalibrationData.Ch6R210Ohm() / 1000.0);
                                txtPCBACalibration10Ch1.Text = Ch1Writer10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch2.Text = Ch2TA10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch3.Text = Ch3WH10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch4.Text = Ch4RH10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch5.Text = Ch5R110Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10Ch6.Text = Ch6R210Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "100R", true) == 0)
                            {
                                double MeasuredValue = (TestProbe21GetManualCalibration.MeasuredValue() / 1000.0);
                                if (rdxPCBACalibrationManualCalibrationCh1.Checked)
                                {
                                    txtPCBACalibration100Ch1.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
                                {
                                    txtPCBACalibration100Ch2.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
                                {
                                    txtPCBACalibration100Ch3.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
                                {
                                    txtPCBACalibration100Ch4.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
                                {
                                    txtPCBACalibration100Ch5.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
                                {
                                    txtPCBACalibration100Ch6.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //100Ohm
                                double Ch1Writer100Ohm = (TestProbe18StartAutoCalibration.Ch1Writer100Ohm() / 1000.0);
                                double Ch2TA100Ohm = (TestProbe18StartAutoCalibration.Ch2TA100Ohm() / 1000.0);
                                double Ch3WH100Ohm = (TestProbe18StartAutoCalibration.Ch3WH100Ohm() / 1000.0);
                                double Ch4RH100Ohm = (TestProbe18StartAutoCalibration.Ch4RH100Ohm() / 1000.0);
                                double Ch5R1100Ohm = (TestProbe18StartAutoCalibration.Ch5R1100Ohm() / 1000.0);
                                double Ch6R2100Ohm = (TestProbe18StartAutoCalibration.Ch6R2100Ohm() / 1000.0);
                                txtPCBACalibration100Ch1.Text = Ch1Writer100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch2.Text = Ch2TA100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch3.Text = Ch3WH100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch4.Text = Ch4RH100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch5.Text = Ch5R1100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch6.Text = Ch6R2100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //100Ohm
                                double Ch1Writer100Ohm = (TestProbe20GetCalibrationData.Ch1Writer100Ohm() / 1000.0);
                                double Ch2TA100Ohm = (TestProbe20GetCalibrationData.Ch2TA100Ohm() / 1000.0);
                                double Ch3WH100Ohm = (TestProbe20GetCalibrationData.Ch3WH100Ohm() / 1000.0);
                                double Ch4RH100Ohm = (TestProbe20GetCalibrationData.Ch4RH100Ohm() / 1000.0);
                                double Ch5R1100Ohm = (TestProbe20GetCalibrationData.Ch5R1100Ohm() / 1000.0);
                                double Ch6R2100Ohm = (TestProbe20GetCalibrationData.Ch6R2100Ohm() / 1000.0);
                                txtPCBACalibration100Ch1.Text = Ch1Writer100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch2.Text = Ch2TA100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch3.Text = Ch3WH100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch4.Text = Ch4RH100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch5.Text = Ch5R1100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration100Ch6.Text = Ch6R2100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "500R", true) == 0)
                            {
                                double MeasuredValue = (TestProbe21GetManualCalibration.MeasuredValue() / 1000.0);
                                if (rdxPCBACalibrationManualCalibrationCh1.Checked)
                                {
                                    txtPCBACalibration500Ch1.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
                                {
                                    txtPCBACalibration500Ch2.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
                                {
                                    txtPCBACalibration500Ch3.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
                                {
                                    txtPCBACalibration500Ch4.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
                                {
                                    txtPCBACalibration500Ch5.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
                                {
                                    txtPCBACalibration500Ch6.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //500Ohm
                                double Ch1Writer500Ohm = (TestProbe18StartAutoCalibration.Ch1Writer500Ohm() / 1000.0);
                                double Ch2TA500Ohm = (TestProbe18StartAutoCalibration.Ch2TA500Ohm() / 1000.0);
                                double Ch3WH500Ohm = (TestProbe18StartAutoCalibration.Ch3WH500Ohm() / 1000.0);
                                double Ch4RH500Ohm = (TestProbe18StartAutoCalibration.Ch4RH500Ohm() / 1000.0);
                                double Ch5R1500Ohm = (TestProbe18StartAutoCalibration.Ch5R1500Ohm() / 1000.0);
                                double Ch6R2500Ohm = (TestProbe18StartAutoCalibration.Ch6R2500Ohm() / 1000.0);
                                txtPCBACalibration500Ch1.Text = Ch1Writer500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch2.Text = Ch2TA500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch3.Text = Ch3WH500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch4.Text = Ch4RH500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch5.Text = Ch5R1500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch6.Text = Ch6R2500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //500Ohm
                                double Ch1Writer500Ohm = (TestProbe20GetCalibrationData.Ch1Writer500Ohm() / 1000.0);
                                double Ch2TA500Ohm = (TestProbe20GetCalibrationData.Ch2TA500Ohm() / 1000.0);
                                double Ch3WH500Ohm = (TestProbe20GetCalibrationData.Ch3WH500Ohm() / 1000.0);
                                double Ch4RH500Ohm = (TestProbe20GetCalibrationData.Ch4RH500Ohm() / 1000.0);
                                double Ch5R1500Ohm = (TestProbe20GetCalibrationData.Ch5R1500Ohm() / 1000.0);
                                double Ch6R2500Ohm = (TestProbe20GetCalibrationData.Ch6R2500Ohm() / 1000.0);
                                txtPCBACalibration500Ch1.Text = Ch1Writer500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch2.Text = Ch2TA500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch3.Text = Ch3WH500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch4.Text = Ch4RH500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch5.Text = Ch5R1500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration500Ch6.Text = Ch6R2500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "1000R", true) == 0)
                            {
                                double MeasuredValue = (TestProbe21GetManualCalibration.MeasuredValue() / 1000.0);
                                if (rdxPCBACalibrationManualCalibrationCh1.Checked)
                                {
                                    txtPCBACalibration1000Ch1.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
                                {
                                    txtPCBACalibration1000Ch2.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
                                {
                                    txtPCBACalibration1000Ch3.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
                                {
                                    txtPCBACalibration1000Ch4.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
                                {
                                    txtPCBACalibration1000Ch5.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
                                {
                                    txtPCBACalibration1000Ch6.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //1kOhm
                                double Ch1Writer1kOhm = (TestProbe18StartAutoCalibration.Ch1Writer1kOhm() / 1000.0);
                                double Ch2TA1kOhm = (TestProbe18StartAutoCalibration.Ch2TA1kOhm() / 1000.0);
                                double Ch3WH1kOhm = (TestProbe18StartAutoCalibration.Ch3WH1kOhm() / 1000.0);
                                double Ch4RH1kOhm = (TestProbe18StartAutoCalibration.Ch4RH1kOhm() / 1000.0);
                                double Ch5R11kOhm = (TestProbe18StartAutoCalibration.Ch5R11kOhm() / 1000.0);
                                double Ch6R21kOhm = (TestProbe18StartAutoCalibration.Ch6R21kOhm() / 1000.0);
                                txtPCBACalibration1000Ch1.Text = Ch1Writer1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch2.Text = Ch2TA1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch3.Text = Ch3WH1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch4.Text = Ch4RH1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch5.Text = Ch5R11kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch6.Text = Ch6R21kOhm.ToString("F3", CultureInfo.InvariantCulture);
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //1kOhm
                                double Ch1Writer1kOhm = (TestProbe20GetCalibrationData.Ch1Writer1kOhm() / 1000.0);
                                double Ch2TA1kOhm = (TestProbe20GetCalibrationData.Ch2TA1kOhm() / 1000.0);
                                double Ch3WH1kOhm = (TestProbe20GetCalibrationData.Ch3WH1kOhm() / 1000.0);
                                double Ch4RH1kOhm = (TestProbe20GetCalibrationData.Ch4RH1kOhm() / 1000.0);
                                double Ch5R11kOhm = (TestProbe20GetCalibrationData.Ch5R11kOhm() / 1000.0);
                                double Ch6R21kOhm = (TestProbe20GetCalibrationData.Ch6R21kOhm() / 1000.0);
                                txtPCBACalibration1000Ch1.Text = Ch1Writer1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch2.Text = Ch2TA1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch3.Text = Ch3WH1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch4.Text = Ch4RH1kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch5.Text = Ch5R11kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration1000Ch6.Text = Ch6R21kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "10000R", true) == 0)
                            {
                                double MeasuredValue = (TestProbe21GetManualCalibration.MeasuredValue() / 1000.0);
                                if (rdxPCBACalibrationManualCalibrationCh1.Checked)
                                {
                                    txtPCBACalibration10000Ch1.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
                                {
                                    txtPCBACalibration10000Ch2.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
                                {
                                    txtPCBACalibration10000Ch3.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
                                {
                                    txtPCBACalibration10000Ch4.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
                                {
                                    txtPCBACalibration10000Ch5.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                                else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
                                {
                                    txtPCBACalibration10000Ch6.Text = MeasuredValue.ToString("F3", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //10kOhm
                                double Ch1Writer10kOhm = (TestProbe18StartAutoCalibration.Ch1Writer10kOhm() / 1000.0);
                                double Ch2TA10kOhm = (TestProbe18StartAutoCalibration.Ch2TA10kOhm() / 1000.0);
                                double Ch3WH10kOhm = (TestProbe18StartAutoCalibration.Ch3WH10kOhm() / 1000.0);
                                double Ch4RH10kOhm = (TestProbe18StartAutoCalibration.Ch4RH10kOhm() / 1000.0);
                                double Ch5R110kOhm = (TestProbe18StartAutoCalibration.Ch5R110kOhm() / 1000.0);
                                double Ch6R210kOhm = (TestProbe18StartAutoCalibration.Ch6R210kOhm() / 1000.0);
                                txtPCBACalibration10000Ch1.Text = Ch1Writer10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch2.Text = Ch2TA10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch3.Text = Ch3WH10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch4.Text = Ch4RH10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch5.Text = Ch5R110kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch6.Text = Ch6R210kOhm.ToString("F3", CultureInfo.InvariantCulture);
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //10kOhm
                                double Ch1Writer10kOhm = (TestProbe20GetCalibrationData.Ch1Writer10kOhm() / 1000.0);
                                double Ch2TA10kOhm = (TestProbe20GetCalibrationData.Ch2TA10kOhm() / 1000.0);
                                double Ch3WH10kOhm = (TestProbe20GetCalibrationData.Ch3WH10kOhm() / 1000.0);
                                double Ch4RH10kOhm = (TestProbe20GetCalibrationData.Ch4RH10kOhm() / 1000.0);
                                double Ch5R110kOhm = (TestProbe20GetCalibrationData.Ch5R110kOhm() / 1000.0);
                                double Ch6R210kOhm = (TestProbe20GetCalibrationData.Ch6R210kOhm() / 1000.0);
                                txtPCBACalibration10000Ch1.Text = Ch1Writer10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch2.Text = Ch2TA10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch3.Text = Ch3WH10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch4.Text = Ch4RH10kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch5.Text = Ch5R110kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                txtPCBACalibration10000Ch6.Text = Ch6R210kOhm.ToString("F3", CultureInfo.InvariantCulture);
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }


                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "100C", true) == 0)
                            {
                                double MeasuredValue = TestProbe21GetManualCalibration.MeasuredValue();                                
                                txtPCBACalibration100Capa.Text = MeasuredValue.ToString();                                    
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //100pF
                                double Capacitance100pF = TestProbe18StartAutoCalibration.Capacitance100pF();                                
                                txtPCBACalibration100Capa.Text = Capacitance100pF.ToString();          
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //100pF
                                double Capacitance100pF = TestProbe20GetCalibrationData.Capacitance100pF();                                
                                txtPCBACalibration100Capa.Text = Capacitance100pF.ToString();
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "270C", true) == 0)
                            {
                                double MeasuredValue = TestProbe21GetManualCalibration.MeasuredValue();
                                txtPCBACalibration270Capa.Text = MeasuredValue.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //270pF
                                double Capacitance270pF = TestProbe18StartAutoCalibration.Capacitance270pF();
                                txtPCBACalibration270Capa.Text = Capacitance270pF.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //270pF
                                double Capacitance270pF = TestProbe20GetCalibrationData.Capacitance270pF();
                                txtPCBACalibration270Capa.Text = Capacitance270pF.ToString();
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "470C", true) == 0)
                            {
                                double MeasuredValue = TestProbe21GetManualCalibration.MeasuredValue();
                                txtPCBACalibration470Capa.Text = MeasuredValue.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //470pF
                                double Capacitance470pF = TestProbe18StartAutoCalibration.Capacitance470pF();
                                txtPCBACalibration470Capa.Text = Capacitance470pF.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //470pF
                                double Capacitance470pF = TestProbe20GetCalibrationData.Capacitance470pF();
                                txtPCBACalibration470Capa.Text = Capacitance470pF.ToString();
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "680C", true) == 0)
                            {
                                double MeasuredValue = TestProbe21GetManualCalibration.MeasuredValue();
                                txtPCBACalibration680Capa.Text = MeasuredValue.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //680pF
                                double Capacitance680pF = TestProbe18StartAutoCalibration.Capacitance680pF();
                                txtPCBACalibration680Capa.Text = Capacitance680pF.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //680pF
                                double Capacitance680pF = TestProbe20GetCalibrationData.Capacitance680pF();
                                txtPCBACalibration680Capa.Text = Capacitance680pF.ToString();
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "820C", true) == 0)
                            {
                                double MeasuredValue = TestProbe21GetManualCalibration.MeasuredValue();
                                txtPCBACalibration820Capa.Text = MeasuredValue.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //820pF
                                double Capacitance820pF = TestProbe18StartAutoCalibration.Capacitance820pF();
                                txtPCBACalibration820Capa.Text = Capacitance820pF.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //820pF
                                double Capacitance820pF = TestProbe20GetCalibrationData.Capacitance820pF();
                                txtPCBACalibration820Capa.Text = Capacitance820pF.ToString();
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }

                            if (CommandID == (byte)MessageID.HST_manual_set_calibration && String.Compare(CommonFunctions.Instance.strManualCalibrationClickedButton, "10000C", true) == 0)
                            {
                                double MeasuredValue = TestProbe21GetManualCalibration.MeasuredValue();
                                txtPCBACalibration10000Capa.Text = MeasuredValue.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_start_auto_calibration)
                            {
                                //10nF
                                double Capacitance10nF = TestProbe18StartAutoCalibration.Capacitance10nF();
                                txtPCBACalibration10000Capa.Text = Capacitance10nF.ToString();
                            }
                            else if (CommandID == (byte)MessageID.HST_get_calibration_data)
                            {
                                //10nF
                                double Capacitance10nF = TestProbe20GetCalibrationData.Capacitance10nF();
                                txtPCBACalibration10000Capa.Text = Capacitance10nF.ToString();
                                IsPCBACalibrationTempered = false;
                                tabPagePCBACalibration.Refresh();
                            }
                        }

                        if (CommandID == (byte)MessageID.HST_set_temp_calibration)
                        {
                            txtPCBACalibrationTemperatureCalibrationVoltage.Text = TestProbe31GetTemperatureCalibration.Voltage().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_temp1_offset)
                        {
                            if (TestProbe50GetTemp1Offset.TemperatureOffsetValue < 0 || TestProbe50GetTemp1Offset.TemperatureOffsetValue > 127)
                            {
                                Notify.PopUpError("Command Execution Aborted", "The value of CH1 Temperature Offset is out of range.");
                                return;
                            }
                            txtPCBACalibrationCh1TemperatureOffset.Text = TestProbe50GetTemp1Offset.TemperatureOffsetValue.ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_calibrate_offset)
                        {
                            txtPCBACalibrationOffsetSensingCh1.Text = TestProbe38GetCalibrateOffset.WriterSensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh2.Text = TestProbe38GetCalibrateOffset.TASensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh3.Text = TestProbe38GetCalibrateOffset.WHSensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh4.Text = TestProbe38GetCalibrateOffset.RHSensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh5.Text = TestProbe38GetCalibrateOffset.R1SensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh6.Text = TestProbe38GetCalibrateOffset.R2SensingOffset().ToString();

                            txtPCBACalibrationOffsetDUTCh1.Text = TestProbe38GetCalibrateOffset.WriterDUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh2.Text = TestProbe38GetCalibrateOffset.TADUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh3.Text = TestProbe38GetCalibrateOffset.WHDUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh4.Text = TestProbe38GetCalibrateOffset.RHDUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh5.Text = TestProbe38GetCalibrateOffset.R1DUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh6.Text = TestProbe38GetCalibrateOffset.R2DUTOffset().ToString();

                            txtPCBACalibrationOffsetINARef.Text = TestProbe38GetCalibrateOffset.INARefVoltage().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_calibration_offset)
                        {
                            txtPCBACalibrationOffsetSensingCh1.Text = TestProbe39GetCalibrationOffset.WriterSensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh2.Text = TestProbe39GetCalibrationOffset.TASensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh3.Text = TestProbe39GetCalibrationOffset.WHSensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh4.Text = TestProbe39GetCalibrationOffset.RHSensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh5.Text = TestProbe39GetCalibrationOffset.R1SensingOffset().ToString();
                            txtPCBACalibrationOffsetSensingCh6.Text = TestProbe39GetCalibrationOffset.R2SensingOffset().ToString();

                            txtPCBACalibrationOffsetDUTCh1.Text = TestProbe39GetCalibrationOffset.WriterDUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh2.Text = TestProbe39GetCalibrationOffset.TADUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh3.Text = TestProbe39GetCalibrationOffset.WHDUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh4.Text = TestProbe39GetCalibrationOffset.RHDUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh5.Text = TestProbe39GetCalibrationOffset.R1DUTOffset().ToString();
                            txtPCBACalibrationOffsetDUTCh6.Text = TestProbe39GetCalibrationOffset.R2DUTOffset().ToString();

                            txtPCBACalibrationOffsetINARef.Text = TestProbe39GetCalibrationOffset.INARefVoltage().ToString();
                        }
                        break;
                    case GUIPage.DebugTabPage:
                        if (CommandID == (byte)MessageID.HST_eeprom_read)
                        {
                            txtDebugEEPROMReadData.Text = TestProbe23GetEEPROMRead.DataByte1.ToString("X");
                        }

                        if (CommandID == (byte)MessageID.HST_dac_read)
                        {                            
                            string strval = TestProbe25GetDACRead.Data().ToString("X");
                            txtDebugDACReadData.Text = strval;
                        }

                        if (CommandID == (byte)MessageID.HST_adc_read)
                        {
                            string strval = TestProbe28GetADCRead.Data().ToString("X");
                            txtDebugADCReadData.Text = strval;
                        }

                        if (CommandID == (byte)MessageID.HST_get_cap_reading)
                        {
                            txtDebugMUXCapacitance.Text = TestProbe35GetCapacitanceReadingFromLCRMeter.Capacitance().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_status)
                        {
                            txtDebugMUXProcessorStatus.Text = TestProbeGetStatusAndErrorCode.Status.ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_temperature)
                        {
                            double Ch1Temperature0 = (TestProbe33GetTemperature.Ch1Temperature() / 10.0);
                            txtDebugTemperatureT1.Text = Ch1Temperature0.ToString("F1", CultureInfo.InvariantCulture);

                            double Ch2Temperature0 = (TestProbe33GetTemperature.Ch2Temperature() / 10.0);
                            txtDebugTemperatureT2.Text = Ch2Temperature0.ToString("F1", CultureInfo.InvariantCulture);

                            double Ch3Temperature0 = (TestProbe33GetTemperature.Ch3Temperature() / 10.0);
                            txtDebugTemperatureT3.Text = Ch3Temperature0.ToString("F1", CultureInfo.InvariantCulture);   
                        }
                        
                        if (CommandID == (byte)MessageID.HST_get_adc_voltages)
                        {
                            txtDebugADCVoltageCh1.Text = TestProbe29GetADCVoltagesRead.Voltage1().ToString();
                            txtDebugADCVoltageCh2.Text = TestProbe29GetADCVoltagesRead.Voltage2().ToString();
                            txtDebugADCVoltageCh3.Text = TestProbe29GetADCVoltagesRead.Voltage3().ToString();
                            txtDebugADCVoltageCh4.Text = TestProbe29GetADCVoltagesRead.Voltage4().ToString();
                            txtDebugADCVoltageCh5.Text = TestProbe29GetADCVoltagesRead.Voltage5().ToString();
                            txtDebugADCVoltageCh6.Text = TestProbe29GetADCVoltagesRead.Voltage6().ToString();
                            txtDebugADCVoltageCh7.Text = TestProbe29GetADCVoltagesRead.Voltage7().ToString();
                            txtDebugADCVoltageCh8.Text = TestProbe29GetADCVoltagesRead.Voltage8().ToString();
                            txtDebugADCVoltageCh9.Text = TestProbe29GetADCVoltagesRead.Voltage9().ToString();
                            txtDebugADCVoltageCh10.Text = TestProbe29GetADCVoltagesRead.Voltage10().ToString();
                            txtDebugADCVoltageCh11.Text = TestProbe29GetADCVoltagesRead.Voltage11().ToString();
                            txtDebugADCVoltageCh12.Text = TestProbe29GetADCVoltagesRead.Voltage12().ToString();
                            txtDebugADCVoltageCh13.Text = TestProbe29GetADCVoltagesRead.Voltage13().ToString();
                            txtDebugADCVoltageCh14.Text = TestProbe29GetADCVoltagesRead.Voltage14().ToString();
                            txtDebugADCVoltageCh15.Text = TestProbe29GetADCVoltagesRead.Voltage15().ToString();
                            txtDebugADCVoltageCh16.Text = TestProbe29GetADCVoltagesRead.Voltage16().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_bias_by_hga)
                        {
                            txtDebugBiasSensingGetBiasVoltagesCh1.Text = TestProbe15GetBiasByHGA.WriterBias().ToString();
                            txtDebugBiasSensingGetBiasVoltagesCh2.Text = TestProbe15GetBiasByHGA.TABias().ToString();
                            txtDebugBiasSensingGetBiasVoltagesCh3.Text = TestProbe15GetBiasByHGA.WHBias().ToString();
                            txtDebugBiasSensingGetBiasVoltagesCh4.Text = TestProbe15GetBiasByHGA.RHBias().ToString();
                            txtDebugBiasSensingGetBiasVoltagesCh5.Text = TestProbe15GetBiasByHGA.R1Bias().ToString();
                            txtDebugBiasSensingGetBiasVoltagesCh6.Text = TestProbe15GetBiasByHGA.R2Bias().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_sensing_by_hga)
                        {
                            txtDebugBiasSensingGetSensingVoltagesCh1.Text = TestProbe16GetSensingByHGA.WriterSensing().ToString();
                            txtDebugBiasSensingGetSensingVoltagesCh2.Text = TestProbe16GetSensingByHGA.TASensing().ToString();
                            txtDebugBiasSensingGetSensingVoltagesCh3.Text = TestProbe16GetSensingByHGA.WHSensing().ToString();
                            txtDebugBiasSensingGetSensingVoltagesCh4.Text = TestProbe16GetSensingByHGA.RHSensing().ToString();
                            txtDebugBiasSensingGetSensingVoltagesCh5.Text = TestProbe16GetSensingByHGA.R1Sensing().ToString();
                            txtDebugBiasSensingGetSensingVoltagesCh6.Text = TestProbe16GetSensingByHGA.R2Sensing().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_results_by_hga)
                        {
                            txtDebugResistanceCapacitancePad1.Text = TestProbe14GetAllResultsByHGA.WPlusStatus.ToString();
                            txtDebugResistanceCapacitancePad2.Text = TestProbe14GetAllResultsByHGA.WMinusStatus.ToString();
                            txtDebugResistanceCapacitancePad3.Text = TestProbe14GetAllResultsByHGA.TAPlusStatus.ToString();
                            txtDebugResistanceCapacitancePad4.Text = TestProbe14GetAllResultsByHGA.TAMinusStatus.ToString();
                            txtDebugResistanceCapacitancePad5.Text = TestProbe14GetAllResultsByHGA.WHPlusStatus.ToString();
                            txtDebugResistanceCapacitancePad6.Text = TestProbe14GetAllResultsByHGA.WHMinusStatus.ToString();
                            txtDebugResistanceCapacitancePad7.Text = TestProbe14GetAllResultsByHGA.RHPlusStatus.ToString();
                            txtDebugResistanceCapacitancePad8.Text = TestProbe14GetAllResultsByHGA.RHMinusStatus.ToString();
                            txtDebugResistanceCapacitancePad9.Text = TestProbe14GetAllResultsByHGA.R1PlusStatus.ToString();
                            txtDebugResistanceCapacitancePad10.Text = TestProbe14GetAllResultsByHGA.R1MinusStatus.ToString();
                            txtDebugResistanceCapacitancePad11.Text = TestProbe14GetAllResultsByHGA.R2PlusStatus.ToString();
                            txtDebugResistanceCapacitancePad12.Text = TestProbe14GetAllResultsByHGA.R2MinusStatus.ToString();

                            double WriterResistance = (TestProbe14GetAllResultsByHGA.WriterResistance() / 1000.0);
                            double TAResistance = (TestProbe14GetAllResultsByHGA.TAResistance() / 1000.0);
                            double WHResistance = (TestProbe14GetAllResultsByHGA.WHResistance() / 1000.0);
                            double RHResistance = (TestProbe14GetAllResultsByHGA.RHResistance() / 1000.0);
                            double R1Resistance = (TestProbe14GetAllResultsByHGA.R1Resistance() / 1000.0);
                            double R2Resistance = (TestProbe14GetAllResultsByHGA.R2Resistance() / 1000.0);
                            txtDebugResistanceCapacitanceCh1.Text = WriterResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtDebugResistanceCapacitanceCh2.Text = TAResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtDebugResistanceCapacitanceCh3.Text = WHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtDebugResistanceCapacitanceCh4.Text = RHResistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtDebugResistanceCapacitanceCh5.Text = R1Resistance.ToString("F3", CultureInfo.InvariantCulture);
                            txtDebugResistanceCapacitanceCh6.Text = R2Resistance.ToString("F3", CultureInfo.InvariantCulture);

                            //Capacitance                            
                            txtDebugResistanceCapacitanceCapa1.Text = TestProbe14GetAllResultsByHGA.Capacitance1().ToString();
                            txtDebugResistanceCapacitanceCapa2.Text = TestProbe14GetAllResultsByHGA.Capacitance2().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_cap_secondary_results)
                        {
                            if(int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 1)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA1ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA1ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 2)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA2ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA2ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 3)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA3ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA3ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 4)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA4ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA4ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 5)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA5ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA5ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 6)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA6ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA6ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 7)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA7ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA7ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 8)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA8ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA8ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 9)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA9ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA9ESR2().ToString();
                            }
                            else if (int.Parse(cboDebugResistanceCapacitanceHGA.Text) == 10)
                            {
                                txtDebugResistanceCapacitanceCapa1ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA10ESR1().ToString();
                                txtDebugResistanceCapacitanceCapa2ESR.Text = TestProbe34GetAllHGACapacitanceSecondaryResults.HGA10ESR2().ToString();
                            }
                        }
                        
                        break;
                    
                    case GUIPage.CableCalibrationTabPage:                        

                        if (CommandID == (byte)MessageID.HST_get_cable_calibration_res_results)
                        {
                            // HGA1
                            double HGA1Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA1WriterResistance() / 1000.0);
                            double HGA1Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA1TAResistance() / 1000.0);
                            double HGA1Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA1WHResistance() / 1000.0);
                            double HGA1Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA1RHResistance() / 1000.0);
                            double HGA1Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA1R1Resistance() / 1000.0);
                            double HGA1Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA1R2Resistance() / 1000.0);
                            txtCableCalibrationHGA1Ch1.Text = HGA1Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA1Ch2.Text = HGA1Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA1Ch3.Text = HGA1Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA1Ch4.Text = HGA1Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA1Ch5.Text = HGA1Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA1Ch6.Text = HGA1Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA2
                            double HGA2Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA2WriterResistance() / 1000.0);
                            double HGA2Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA2TAResistance() / 1000.0);
                            double HGA2Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA2WHResistance() / 1000.0);
                            double HGA2Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA2RHResistance() / 1000.0);
                            double HGA2Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA2R1Resistance() / 1000.0);
                            double HGA2Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA2R2Resistance() / 1000.0);
                            txtCableCalibrationHGA2Ch1.Text = HGA2Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA2Ch2.Text = HGA2Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA2Ch3.Text = HGA2Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA2Ch4.Text = HGA2Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA2Ch5.Text = HGA2Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA2Ch6.Text = HGA2Ch6R2.ToString("F3", CultureInfo.InvariantCulture);                            

                            // HGA3
                            double HGA3Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA3WriterResistance() / 1000.0);
                            double HGA3Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA3TAResistance() / 1000.0);
                            double HGA3Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA3WHResistance() / 1000.0);
                            double HGA3Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA3RHResistance() / 1000.0);
                            double HGA3Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA3R1Resistance() / 1000.0);
                            double HGA3Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA3R2Resistance() / 1000.0);
                            txtCableCalibrationHGA3Ch1.Text = HGA3Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA3Ch2.Text = HGA3Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA3Ch3.Text = HGA3Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA3Ch4.Text = HGA3Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA3Ch5.Text = HGA3Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA3Ch6.Text = HGA3Ch6R2.ToString("F3", CultureInfo.InvariantCulture);                            

                            // HGA4
                            double HGA4Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA4WriterResistance() / 1000.0);
                            double HGA4Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA4TAResistance() / 1000.0);
                            double HGA4Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA4WHResistance() / 1000.0);
                            double HGA4Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA4RHResistance() / 1000.0);
                            double HGA4Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA4R1Resistance() / 1000.0);
                            double HGA4Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA4R2Resistance() / 1000.0);
                            txtCableCalibrationHGA4Ch1.Text = HGA4Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA4Ch2.Text = HGA4Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA4Ch3.Text = HGA4Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA4Ch4.Text = HGA4Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA4Ch5.Text = HGA4Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA4Ch6.Text = HGA4Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA5
                            double HGA5Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA5WriterResistance() / 1000.0);
                            double HGA5Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA5TAResistance() / 1000.0);
                            double HGA5Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA5WHResistance() / 1000.0);
                            double HGA5Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA5RHResistance() / 1000.0);
                            double HGA5Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA5R1Resistance() / 1000.0);
                            double HGA5Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA5R2Resistance() / 1000.0);
                            txtCableCalibrationHGA5Ch1.Text = HGA5Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA5Ch2.Text = HGA5Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA5Ch3.Text = HGA5Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA5Ch4.Text = HGA5Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA5Ch5.Text = HGA5Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA5Ch6.Text = HGA5Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA6
                            double HGA6Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA6WriterResistance() / 1000.0);
                            double HGA6Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA6TAResistance() / 1000.0);
                            double HGA6Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA6WHResistance() / 1000.0);
                            double HGA6Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA6RHResistance() / 1000.0);
                            double HGA6Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA6R1Resistance() / 1000.0);
                            double HGA6Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA6R2Resistance() / 1000.0);
                            txtCableCalibrationHGA6Ch1.Text = HGA6Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA6Ch2.Text = HGA6Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA6Ch3.Text = HGA6Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA6Ch4.Text = HGA6Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA6Ch5.Text = HGA6Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA6Ch6.Text = HGA6Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA7
                            double HGA7Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA7WriterResistance() / 1000.0);
                            double HGA7Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA7TAResistance() / 1000.0);
                            double HGA7Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA7WHResistance() / 1000.0);
                            double HGA7Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA7RHResistance() / 1000.0);
                            double HGA7Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA7R1Resistance() / 1000.0);
                            double HGA7Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA7R2Resistance() / 1000.0);
                            txtCableCalibrationHGA7Ch1.Text = HGA7Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA7Ch2.Text = HGA7Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA7Ch3.Text = HGA7Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA7Ch4.Text = HGA7Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA7Ch5.Text = HGA7Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA7Ch6.Text = HGA7Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA8
                            double HGA8Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA8WriterResistance() / 1000.0);
                            double HGA8Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA8TAResistance() / 1000.0);
                            double HGA8Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA8WHResistance() / 1000.0);
                            double HGA8Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA8RHResistance() / 1000.0);
                            double HGA8Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA8R1Resistance() / 1000.0);
                            double HGA8Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA8R2Resistance() / 1000.0);
                            txtCableCalibrationHGA8Ch1.Text = HGA8Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA8Ch2.Text = HGA8Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA8Ch3.Text = HGA8Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA8Ch4.Text = HGA8Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA8Ch5.Text = HGA8Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA8Ch6.Text = HGA8Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA9
                            double HGA9Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA9WriterResistance() / 1000.0);
                            double HGA9Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA9TAResistance() / 1000.0);
                            double HGA9Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA9WHResistance() / 1000.0);
                            double HGA9Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA9RHResistance() / 1000.0);
                            double HGA9Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA9R1Resistance() / 1000.0);
                            double HGA9Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA9R2Resistance() / 1000.0);
                            txtCableCalibrationHGA9Ch1.Text = HGA9Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA9Ch2.Text = HGA9Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA9Ch3.Text = HGA9Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA9Ch4.Text = HGA9Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA9Ch5.Text = HGA9Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA9Ch6.Text = HGA9Ch6R2.ToString("F3", CultureInfo.InvariantCulture);
                            
                            // HGA10
                            double HGA10Ch1Writer = (TestProbe44GetCableCalibrationResistanceResults.HGA10WriterResistance() / 1000.0);
                            double HGA10Ch2TA = (TestProbe44GetCableCalibrationResistanceResults.HGA10TAResistance() / 1000.0);
                            double HGA10Ch3WH = (TestProbe44GetCableCalibrationResistanceResults.HGA10WHResistance() / 1000.0);
                            double HGA10Ch4RH = (TestProbe44GetCableCalibrationResistanceResults.HGA10RHResistance() / 1000.0);
                            double HGA10Ch5R1 = (TestProbe44GetCableCalibrationResistanceResults.HGA10R1Resistance() / 1000.0);
                            double HGA10Ch6R2 = (TestProbe44GetCableCalibrationResistanceResults.HGA10R2Resistance() / 1000.0);
                            txtCableCalibrationHGA10Ch1.Text = HGA10Ch1Writer.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA10Ch2.Text = HGA10Ch2TA.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA10Ch3.Text = HGA10Ch3WH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA10Ch4.Text = HGA10Ch4RH.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA10Ch5.Text = HGA10Ch5R1.ToString("F3", CultureInfo.InvariantCulture);
                            txtCableCalibrationHGA10Ch6.Text = HGA10Ch6R2.ToString("F3", CultureInfo.InvariantCulture);                            
                        }

                        if (CommandID == (byte)MessageID.HST_get_cable_calibration_cap_results)
                        {
                            // HGA1                            
                            txtCableCalibrationHGA1Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA1Ch1Capacitance().ToString();
                            txtCableCalibrationHGA1Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA1Ch2Capacitance().ToString();

                            // HGA2
                            txtCableCalibrationHGA2Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA2Ch1Capacitance().ToString();
                            txtCableCalibrationHGA2Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA2Ch2Capacitance().ToString();

                            // HGA3
                            txtCableCalibrationHGA3Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA3Ch1Capacitance().ToString();
                            txtCableCalibrationHGA3Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA3Ch2Capacitance().ToString();

                            // HGA4
                            txtCableCalibrationHGA4Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA4Ch1Capacitance().ToString();
                            txtCableCalibrationHGA4Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA4Ch2Capacitance().ToString();

                            // HGA5
                            txtCableCalibrationHGA5Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA5Ch1Capacitance().ToString();
                            txtCableCalibrationHGA5Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA5Ch2Capacitance().ToString();

                            // HGA6
                            txtCableCalibrationHGA6Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA6Ch1Capacitance().ToString();
                            txtCableCalibrationHGA6Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA6Ch2Capacitance().ToString();

                            // HGA7
                            txtCableCalibrationHGA7Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA7Ch1Capacitance().ToString();
                            txtCableCalibrationHGA7Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA7Ch2Capacitance().ToString();

                            // HGA8
                            txtCableCalibrationHGA8Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA8Ch1Capacitance().ToString();
                            txtCableCalibrationHGA8Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA8Ch2Capacitance().ToString();

                            // HGA9
                            txtCableCalibrationHGA9Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA9Ch1Capacitance().ToString();
                            txtCableCalibrationHGA9Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA9Ch2Capacitance().ToString();

                            // HGA10
                            txtCableCalibrationHGA10Ch1Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA10Ch1Capacitance().ToString();
                            txtCableCalibrationHGA10Ch2Capa.Text = TestProbe51GetCableCalibrationCapacitanceResults.HGA10Ch2Capacitance().ToString();
                        }

                        if (CommandID == (byte)MessageID.HST_get_short_detection_threshold)
                        {
                            txtCableCalibrationShortDetectionThreshold.Text = TestProbe48GetShortDetectionThreshold.ThresholdVoltage().ToString();
                        }
                        break;

                    case GUIPage.PrecisorCompensationTabPage:

                        if (CommandID == (byte)MessageID.HST_get_precisor_cap_compensation)
                        {
                            // HGA1
                            txtPrecisorCompensationHGA1CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA1Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA1CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA1Ch2Compensation().ToString();

                            // HGA2
                            txtPrecisorCompensationHGA2CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA2Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA2CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA2Ch2Compensation().ToString();

                            // HGA3
                            txtPrecisorCompensationHGA3CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA3Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA3CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA3Ch2Compensation().ToString();

                            // HGA4
                            txtPrecisorCompensationHGA4CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA4Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA4CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA4Ch2Compensation().ToString();

                            // HGA5
                            txtPrecisorCompensationHGA5CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA5Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA5CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA5Ch2Compensation().ToString();

                            // HGA6
                            txtPrecisorCompensationHGA6CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA6Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA6CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA6Ch2Compensation().ToString();

                            // HGA7
                            txtPrecisorCompensationHGA7CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA7Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA7CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA7Ch2Compensation().ToString();

                            // HGA8
                            txtPrecisorCompensationHGA8CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA8Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA8CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA8Ch2Compensation().ToString();

                            // HGA9
                            txtPrecisorCompensationHGA9CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA9Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA9CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA9Ch2Compensation().ToString();

                            // HGA10
                            txtPrecisorCompensationHGA10CH1.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA10Ch1Compensation().ToString();
                            txtPrecisorCompensationHGA10CH2.Text = TestProbe53GetPrecisorCapacitanceCompensation.HGA10Ch2Compensation().ToString();

                            if(TestProbe53GetPrecisorCapacitanceCompensation.EnableFlag == 1)
                            {
                                cboPrecisorCompensationEnableFlag.Text = "TRUE";
                            }
                            else
                            {
                                cboPrecisorCompensationEnableFlag.Text = "FALSE";
                            }
                        }
                        break;
                }
                
            }
        }                
	}
}