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
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.UI;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Exceptions;
using qf4net;
using AutoMapper;



namespace Seagate.AAS.HGA.HST.UI
{

    public partial class FormMainE95 : FormMainLite
    {

        // Public event handler
        public event EventHandler TemperaturesEvent;
        public event EventHandler TestElectronicsStatusEvent;
        public event EventHandler TestFirmwareVersionEvent;

        const int BUFFER_SIZE = 256;
        int numBytes = 0;
        private int DelayBeforeProcessReadBuffer = 300;
        private byte[] writeDataBuffer = new byte[BUFFER_SIZE];
        private byte[] readDataBuffer = new byte[BUFFER_SIZE];
        private Carrier _carrierUnderTest;

        public bool IsConfigurationSetupTempered = false;
        public bool IsPCBACalibrationTempered = false;
        public bool IsCableCalibrationTempered = false;

        public bool HGA1ResistanceTestPASS = true;
        public bool HGA1CapacitanceTestPASS = true;
        public bool HGA1ShortTestPASS = true;

        public bool HGA2ResistanceTestPASS = true;
        public bool HGA2CapacitanceTestPASS = true;
        public bool HGA2ShortTestPASS = true;

        public bool HGA3ResistanceTestPASS = true;
        public bool HGA3CapacitanceTestPASS = true;
        public bool HGA3ShortTestPASS = true;

        public bool HGA4ResistanceTestPASS = true;
        public bool HGA4CapacitanceTestPASS = true;
        public bool HGA4ShortTestPASS = true;

        public bool HGA5ResistanceTestPASS = true;
        public bool HGA5CapacitanceTestPASS = true;
        public bool HGA5ShortTestPASS = true;

        public bool HGA6ResistanceTestPASS = true;
        public bool HGA6CapacitanceTestPASS = true;
        public bool HGA6ShortTestPASS = true;

        public bool HGA7ResistanceTestPASS = true;
        public bool HGA7CapacitanceTestPASS = true;
        public bool HGA7ShortTestPASS = true;

        public bool HGA8ResistanceTestPASS = true;
        public bool HGA8CapacitanceTestPASS = true;
        public bool HGA8ShortTestPASS = true;

        public bool HGA9ResistanceTestPASS = true;
        public bool HGA9CapacitanceTestPASS = true;
        public bool HGA9ShortTestPASS = true;

        public bool HGA10ResistanceTestPASS = true;
        public bool HGA10CapacitanceTestPASS = true;
        public bool HGA10ShortTestPASS = true;

        bool HGA1Present = true;
        bool HGA2Present = true;
        bool HGA3Present = true;
        bool HGA4Present = true;
        bool HGA5Present = true;
        bool HGA6Present = true;
        bool HGA7Present = true;
        bool HGA8Present = true;
        bool HGA9Present = true;
        bool HGA10Present = true;

        bool HGA1PassInPreviousSystem = true;
        bool HGA2PassInPreviousSystem = true;
        bool HGA3PassInPreviousSystem = true;
        bool HGA4PassInPreviousSystem = true;
        bool HGA5PassInPreviousSystem = true;
        bool HGA6PassInPreviousSystem = true;
        bool HGA7PassInPreviousSystem = true;
        bool HGA8PassInPreviousSystem = true;
        bool HGA9PassInPreviousSystem = true;
        bool HGA10PassInPreviousSystem = true;

        double DeltaISI_Tolerance1 = 0;
        double DeltaISI_Tolerance2 = 0;
        double DeltaISI_R1_SDET_Tolerance = 0;
        double DeltaISI_R2_SDET_Tolerance = 0;

        double[] LDU_M = new double[10];
        double[] LDU_C = new double[10];
        double[] LED_M = new double[10];
        double[] LED_C = new double[10];
        double[] TADeltaResistance = new double[10];
        double[] DeltaTAArray0 = new double[10];
        double[] DeltaTAArray1 = new double[10];
        double[,] LDUVoltagePoint = new double[10, 21];
        double[,] LEDVoltagePoint = new double[10, 21];
        double[,] shortRatio = new double[10, 6];


        private Thread LDUAndLEDCalculationThread;
        private Thread VPDCalculationThread;

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
        public TestProbe2ConfigResistanceMeasurement TestProbe2ConfigResistanceMeasurement;
        public TestProbe3ConfigCapacitanceMeasurement TestProbe3ConfigCapacitanceMeasurement;
        public TestProbe4ConfigShortDetection TestProbe4ConfigShortDetection;
        public TestProbe5MeasurementChannelEnable TestProbe5MeasurementChannelEnable;
        public TestProbe6HGAEnable TestProbe6HGAEnable;
        public TestProbe9SetStartMeasurement TestProbe9SetStartMeasurement;
        public TestProbe14SetGetAllResultsByHGA TestProbe14SetGetAllResultsByHGA;
        public TestProbe15SetGetBiasByHGA TestProbe15SetGetBiasByHGA;
        public TestProbe16SetGetSensingByHGA TestProbe16SetGetSensingByHGA;
        public TestProbe17CalibrationEnable TestProbe17CalibrationEnable;
        public TestProbe21SetManualCalibration TestProbe21SetManualCalibration;
        public TestProbe22EEPROMWrite TestProbe22EEPROMWrite;
        public TestProbe23SetEEPROMRead TestProbe23SetEEPROMRead;
        public TestProbe24DACWrite TestProbe24DACWrite;
        public TestProbe25SetDACRead TestProbe25SetDACRead;
        public TestProbe26DACOutputEnable TestProbe26DACOutputEnable;  // By KA Gan: Not used?
        public TestProbe27ADCWrite TestProbe27ADCWrite;
        public TestProbe28SetADCRead TestProbe28SetADCRead;
        public TestProbe29SetADCVoltagesRead TestProbe29SetADCVoltagesRead;
        public TestProbe30SetMUX TestProbe30SetMUX;
        public TestProbe31SetTemperatureCalibration TestProbe31SetTemperatureCalibration;
        public TestProbe32ConfigTemperatureMeasurement TestProbe32ConfigTemperatureMeasurement;
        public TestProbe43SetFlexCableCalibration TestProbe43SetFlexCableCalibration;
        public TestProbe44SetGetCableCalibrationResistanceResults TestProbe44SetGetCableCalibrationResistanceResults;
        public TestProbe45SetCableCompensation TestProbe45SetCableCompensation;
        public TestProbe47SetShortDetectionThreshold TestProbe47SetShortDetectionThreshold;
        public TestProbe49SetTemp1Offset TestProbe49SetTemp1Offset;
        public TestProbe51SetGetCableCalibrationCapacitanceResults TestProbe51SetGetCableCalibrationCapacitanceResults;
        public TestProbe57ConfigResistanceMeasurement2 TestProbe57ConfigResistanceMeasurement2;
        public TestProbe81SetSwapCH3AndCH4 TestProbe81SetSwapCH3AndCH4;
        public TestProbe66SetCurrentRatio TestProbe66SetCurrentRatio;                               
        public TestProbe59SetShortDetectionThreshold TestProbe59SetShortDetectionThreshold;         
        public TestProbe68SetShortDetectionVolThreshold TestProbe68SetShortDetectionVolThreshold;   

        // IncomingTestProbeData data structure
        public TestProbeGetStatusAndErrorCode TestProbeGetStatusAndErrorCode;
        public TestProbe7GetConversionBoardID TestProbe7GetConversionBoardID;
        public TestProbe8GetOperationMode TestProbe8GetOperationMode;
        public TestProbe10GetAllHGAShortDetection TestProbe10GetAllHGAShortDetection;
        public TestProbe11GetAllHGAResistanceResults TestProbe11GetAllHGAResistanceResults;
        public TestProbe12GetAllHGACapacitanceResults TestProbe12GetAllHGACapacitanceResults;
        public TestProbe13GetAllHGABiasVoltages TestProbe13GetAllHGABiasVoltages;  // By KA Gan: Not used in this application
        public TestProbe14GetAllResultsByHGA TestProbe14GetAllResultsByHGA;
        public TestProbe15GetBiasByHGA TestProbe15GetBiasByHGA;
        public TestProbe16GetSensingByHGA TestProbe16GetSensingByHGA;
        public TestProbe18StartAutoCalibration TestProbe18StartAutoCalibration;
        public TestProbe20GetCalibrationData TestProbe20GetCalibrationData;
        public TestProbe21GetManualCalibration TestProbe21GetManualCalibration;
        public TestProbe23GetEEPROMRead TestProbe23GetEEPROMRead;
        public TestProbe25GetDACRead TestProbe25GetDACRead;
        public TestProbe28GetADCRead TestProbe28GetADCRead;
        public TestProbe29GetADCVoltagesRead TestProbe29GetADCVoltagesRead;
        public TestProbe31GetTemperatureCalibration TestProbe31GetTemperatureCalibration;
        public TestProbe33GetTemperature TestProbe33GetTemperature;
        public TestProbe34GetAllHGACapacitanceSecondaryResults TestProbe34GetAllHGACapacitanceSecondaryResults;
        public TestProbe35GetCapacitanceReadingFromLCRMeter TestProbe35GetCapacitanceReadingFromLCRMeter;
        public TestProbe36StartSelfTest TestProbe36StartSelfTest;
        public TestProbe37GetFirmwareVersion TestProbe37GetFirmwareVersion;
        public TestProbe38GetCalibrateOffset TestProbe38GetCalibrateOffset;
        public TestProbe39GetCalibrationOffset TestProbe39GetCalibrationOffset;
        public TestProbe43StartFlexCableCalibration TestProbe43StartFlexCableCalibration;
        public TestProbe44GetCableCalibrationResistanceResults TestProbe44GetCableCalibrationResistanceResults;
        public TestProbe48GetShortDetectionThreshold TestProbe48GetShortDetectionThreshold;
        public TestProbe50GetTemp1Offset TestProbe50GetTemp1Offset;
        public TestProbe51GetCableCalibrationCapacitanceResults TestProbe51GetCableCalibrationCapacitanceResults;
        public TestProbe55GetResMeasConfiguration TestProbe55GetResMeasConfiguration;
        public TestProbe58GetResMeas2Configuration TestProbe58GetResMeas2Configuration;
        public TestProbe72GetVolDelta TestProbe72GetVolDelta;
        public TestProbe75GetAllBiasVoltage TestProbe75GetAllBiasVoltage;
        public TestProbe76GetAllSensingVoltage TestProbe76GetAllSensingVoltage;

        public TestProbe60GetShortDetectionThreshold TestProbe60GetShortDetectionThreshold;             
        public TestProbe71GetShortDetectionVolThreshold TestProbe71GetShortDetectionVolThreshold;       
        public TestProbe67GetCurrentRatio TestProbe67GetCurrentRatio;                                   
        public TestProbe79GetAllBiasVoltage TestProbe79GetAllBiasVoltage;                               
        public TestProbe80GetAllSensingVoltage TestProbe80GetAllSensingVoltage;                         

        public TestProbe85SetLDUConfiguration_2 TestProbe85SetLDUConfiguration_2;
        public TestProbe86RequestPhotoDiodeVoltageByHga TestProbe86RequestPhotoDiodeVoltageByHga;
        public TestProbe86GetPhotodiodeDataByHGA TestProbe86GetPhotodiodeDataByHGA;
        public TestProbe63GetLDUAndLEDData TestProbe63GetLDUAndLEDData;
        public TestProbe63SetGetLDUDataByHGA TestProbe63SetGetLDUDataByHGA;
        public TestProbe87GetLDUConfiguration_2 TestProbe87GetLDUConfiguration_2;

        public TestProbe88GetHalfLDUAndLEDData TestProbe88GetHalfLDUAndLEDData;
        public TestProbe89GetHalfPhotoDiodeData TestProbe89GetHalfPhotoDiodeData;
        public TestProbe88Request_5_LDUAndLEDData TestProbe88Request_5_LDUAndLEDData;
        public TestProbe89RequestPDVoltage TestProbe89RequestPDVoltage;


        private Dictionary<DateTime, int> DictTICError = new Dictionary<DateTime, int>();

        private void OverallMeasurementTestResults(Carrier _carrierUnderTest)
        {
            if (_carrierUnderTest != null)
            {
                /* Overall Measurement Test Results */

                // Default value of OverallMeasurementTestPass is set to "false"
                _carrierUnderTest.Hga1.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga2.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga3.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga4.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga5.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga6.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga7.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga8.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga9.OverallMeasurementTestPass = false;
                _carrierUnderTest.Hga10.OverallMeasurementTestPass = false;

                //                            if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled == false)
                {
                    int numberOfTestedFailHGAs = 0;
                    // Update HGA status
                    // HGA1
                    if (_carrierUnderTest.Hga1.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga1.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga1.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga1.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga1.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga1.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga1.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga1.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga1.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga1.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga1.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga1.OverallMeasurementTestPass = true;

                                }
                                else
                                {
                                    _carrierUnderTest.Hga1.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA1 Test Status = '{0}', HGA1ResistanceTestPASS : {1}, HGA1CapacitanceTestPASS : {2}, HGA1ShortTestPASS : {3}.", _carrierUnderTest.Hga1.Hga_Status, HGA1ResistanceTestPASS, HGA1CapacitanceTestPASS, HGA1ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA2
                    if (_carrierUnderTest.Hga2.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga2.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga2.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga2.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga2.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga2.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga2.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga2.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga2.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga2.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga2.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga2.OverallMeasurementTestPass = true;

                                }
                                else
                                {
                                    _carrierUnderTest.Hga2.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA2 Test Status = '{0}', HGA2ResistanceTestPASS : {1}, HGA2CapacitanceTestPASS : {2}, HGA2ShortTestPASS : {3}.", _carrierUnderTest.Hga2.Hga_Status, HGA2ResistanceTestPASS, HGA2CapacitanceTestPASS, HGA2ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA3
                    if (_carrierUnderTest.Hga3.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga3.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga3.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga3.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga3.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga3.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga3.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga3.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga3.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga3.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga3.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga3.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga3.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA3 Test Status = '{0}', HGA3ResistanceTestPASS : {1}, HGA3CapacitanceTestPASS : {2}, HGA3ShortTestPASS : {3}.", _carrierUnderTest.Hga3.Hga_Status, HGA3ResistanceTestPASS, HGA3CapacitanceTestPASS, HGA3ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA4
                    if (_carrierUnderTest.Hga4.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga4.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga4.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga4.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga4.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga4.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga4.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga4.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga4.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga4.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga4.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga4.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga4.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA4 Test Status = '{0}', HGA4ResistanceTestPASS : {1}, HGA4CapacitanceTestPASS : {2}, HGA4ShortTestPASS : {3}.", _carrierUnderTest.Hga4.Hga_Status, HGA4ResistanceTestPASS, HGA4CapacitanceTestPASS, HGA4ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA5
                    if (_carrierUnderTest.Hga5.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga5.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga5.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga5.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga5.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga5.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga5.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga5.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga5.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga5.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga5.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga5.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga5.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA5 Test Status = '{0}', HGA5ResistanceTestPASS : {1}, HGA5CapacitanceTestPASS : {2}, HGA5ShortTestPASS : {3}.", _carrierUnderTest.Hga5.Hga_Status, HGA5ResistanceTestPASS, HGA5CapacitanceTestPASS, HGA5ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA6
                    if (_carrierUnderTest.Hga6.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga6.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga6.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga6.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga6.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga6.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga6.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga6.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga6.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga6.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga6.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga6.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga6.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA6 Test Status = '{0}', HGA6ResistanceTestPASS : {1}, HGA6CapacitanceTestPASS : {2}, HGA6ShortTestPASS : {3}.", _carrierUnderTest.Hga6.Hga_Status, HGA6ResistanceTestPASS, HGA6CapacitanceTestPASS, HGA6ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA7
                    if (_carrierUnderTest.Hga7.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga7.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga7.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga7.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga7.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga7.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga7.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga7.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga7.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga7.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga7.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga7.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga7.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA7 Test Status = '{0}', HGA7ResistanceTestPASS : {1}, HGA7CapacitanceTestPASS : {2}, HGA7ShortTestPASS : {3}.", _carrierUnderTest.Hga7.Hga_Status, HGA7ResistanceTestPASS, HGA7CapacitanceTestPASS, HGA7ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA8
                    if (_carrierUnderTest.Hga8.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga8.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga8.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga8.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga8.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga8.TST_STATUS.Equals('0'));
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga8.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga8.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga8.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga8.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga8.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga8.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga8.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA8 Test Status = '{0}', HGA8ResistanceTestPASS : {1}, HGA8CapacitanceTestPASS : {2}, HGA8ShortTestPASS : {3}.", _carrierUnderTest.Hga8.Hga_Status, HGA8ResistanceTestPASS, HGA8CapacitanceTestPASS, HGA8ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA9
                    if (_carrierUnderTest.Hga9.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga9.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga9.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga9.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga9.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga9.TST_STATUS.Equals('0'));

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {

                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga9.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga9.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga9.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga9.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga9.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga9.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga9.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA9 Test Status = '{0}', HGA9ResistanceTestPASS : {1}, HGA9CapacitanceTestPASS : {2}, HGA9ShortTestPASS : {3}.", _carrierUnderTest.Hga9.Hga_Status, HGA9ResistanceTestPASS, HGA9CapacitanceTestPASS, HGA9ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    // HGA10
                    if (_carrierUnderTest.Hga10.Hga_Status == HGAStatus.NoHGAPresent || _carrierUnderTest.Hga10.Hga_Status == HGAStatus.Unknown)
                    {
                        // Do not update the status since HGA is missing or unknown
                    }
                    else
                    {
                        if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text, CommonFunctions.NOT_AVAILABLE, true) == 0)
                        {
                            _carrierUnderTest.Hga10.Hga_Status = HGAStatus.Untested;
                        }
                        else
                        {
                            var isFailGetSort = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable && (_carrierUnderTest.Hga10.TST_STATUS.Equals('\0') || _carrierUnderTest.Hga10.TST_STATUS.Equals(string.Empty) || _carrierUnderTest.Hga10.TST_STATUS.Equals('0'));

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10C1Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            //String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10C2Capa.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text, CommonFunctions.FAIL, true) == 0 ||
                            String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                if (!isFailGetSort && _workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga10.Error_Msg_Code))
                                {
                                    _carrierUnderTest.Hga10.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga10.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga10.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                            else
                            {
                                if (!isFailGetSort)
                                {
                                    _carrierUnderTest.Hga10.Hga_Status = HGAStatus.TestedPass;
                                    _carrierUnderTest.Hga10.OverallMeasurementTestPass = true;
                                }
                                else
                                {
                                    _carrierUnderTest.Hga10.Hga_Status = HGAStatus.TestedFail;
                                    numberOfTestedFailHGAs++;
                                }
                            }
                        }
                        Log.Info(this, "Carrier ID : {4}, Overall HGA10 Test Status = '{0}', HGA10ResistanceTestPASS : {1}, HGA10CapacitanceTestPASS : {2}, HGA10ShortTestPASS : {3}.", _carrierUnderTest.Hga10.Hga_Status, HGA10ResistanceTestPASS, HGA10CapacitanceTestPASS, HGA10ShortTestPASS, _carrierUnderTest.CarrierID);
                    }

                    //TIC Error check feature 1
                    if ((HSTMachine.Workcell.HSTSettings.Install.TICFailHGAsInBoatLimit > 0) &&
                        (HSTMachine.Workcell.HSTSettings.Install.TICConsecutiveFailBoatsLimit > 0))
                    {
                        if (numberOfTestedFailHGAs >= HSTMachine.Workcell.HSTSettings.Install.TICFailHGAsInBoatLimit)
                            CommonFunctions.Instance.ConsecutiveBoatsWithTICError++;
                        else
                            CommonFunctions.Instance.ConsecutiveBoatsWithTICError = 0;

                        if (CommonFunctions.Instance.ConsecutiveBoatsWithTICError >= HSTMachine.Workcell.HSTSettings.Install.TICConsecutiveFailBoatsLimit)
                        {
                            CommonFunctions.Instance.StopMachineRunDueToTICError = true;
                            CommonFunctions.Instance.TICErrorMessage = String.Format("Total number of consecutive boat with TIC errors exceeed user defined limit({0})",
                                HSTMachine.Workcell.HSTSettings.Install.TICConsecutiveFailBoatsLimit);
                            QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));
                        }

                    }

                    //TIC Error check feature 2
                    if ((HSTMachine.Workcell.HSTSettings.Install.TICErrorCountingTimeInterval > 0) &&
                        (HSTMachine.Workcell.HSTSettings.Install.TICFailHGAsTotalLimit > 0))
                    {
                        if (numberOfTestedFailHGAs > 0)
                        {
                            DictTICError.Add(DateTime.Now, numberOfTestedFailHGAs);
                        }

                        foreach (KeyValuePair<DateTime, int> pair in DictTICError)
                        {
                            DateTime currentTime = DateTime.Now;
                            TimeSpan timeDiff = currentTime.Subtract(pair.Key);

                            if (timeDiff.TotalMinutes > HSTMachine.Workcell.HSTSettings.Install.TICErrorCountingTimeInterval)
                            {
                                DictTICError.Remove(pair.Key);
                            }
                        }

                        int totalFailedHGAs = 0;
                        foreach (KeyValuePair<DateTime, int> pair in DictTICError)
                        {
                            totalFailedHGAs += pair.Value;
                        }

                        if (totalFailedHGAs >= HSTMachine.Workcell.HSTSettings.Install.TICFailHGAsTotalLimit)
                        {
                            CommonFunctions.Instance.StopMachineRunDueToTICError = true;
                            CommonFunctions.Instance.TICErrorMessage = String.Format("Total Failed HGAs exceeed user defined limit({0}) within last {1} minutes",
                                HSTMachine.Workcell.HSTSettings.Install.TICFailHGAsTotalLimit, HSTMachine.Workcell.HSTSettings.Install.TICErrorCountingTimeInterval);
                            QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));
                        }
                    }

                    // Overall Test Result of HGA1
                    if (_carrierUnderTest.Hga1.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga1.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA2
                    if (_carrierUnderTest.Hga2.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga2.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA3
                    if (_carrierUnderTest.Hga3.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga3.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA4
                    if (_carrierUnderTest.Hga4.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga4.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA5
                    if (_carrierUnderTest.Hga5.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga5.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA6
                    if (_carrierUnderTest.Hga6.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga6.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA7
                    if (_carrierUnderTest.Hga7.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga7.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA8
                    if (_carrierUnderTest.Hga8.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga8.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA9
                    if (_carrierUnderTest.Hga9.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga9.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.ForeColor = Color.Black;
                    }

                    // Overall Test Result of HGA10
                    if (_carrierUnderTest.Hga10.Hga_Status == HGAStatus.TestedPass)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.Text = CommonFunctions.PASS;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.ForeColor = Color.Green;
                    }
                    else if (_carrierUnderTest.Hga10.Hga_Status == HGAStatus.TestedFail)
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.Text = CommonFunctions.FAIL;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.ForeColor = Color.Red;
                    }
                    else
                    {
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.Text = CommonFunctions.NOT_AVAILABLE;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.ForeColor = Color.Black;
                    }


                    Log.Info(this, "In TestProbeUtility's UpdateGUIDisplay, Carrier ID:{0}, Overall test status of HGA1:{1}, HGA2:{2}, HGA3:{3}, HGA4:{4}, HGA5:{5}, HGA6:{6}, HGA7:{7}, HGA8:{8}, HGA9:{9}, HGA10:{10}.",
                            _carrierUnderTest.CarrierID, _carrierUnderTest.Hga1.Hga_Status, _carrierUnderTest.Hga2.Hga_Status, _carrierUnderTest.Hga3.Hga_Status, _carrierUnderTest.Hga4.Hga_Status,
                            _carrierUnderTest.Hga5.Hga_Status, _carrierUnderTest.Hga6.Hga_Status, _carrierUnderTest.Hga7.Hga_Status, _carrierUnderTest.Hga8.Hga_Status, _carrierUnderTest.Hga9.Hga_Status, _carrierUnderTest.Hga10.Hga_Status);

                    {
                        if (_carrierUnderTest.Hga1.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga2.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga3.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga4.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga5.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga6.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga7.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga8.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga9.Hga_Status == HGAStatus.TestedFail &&
                            _carrierUnderTest.Hga10.Hga_Status == HGAStatus.TestedFail)
                        {
                            _workcell.IsAllMeasurementFailed = true;
                        }
                    }

                    //Send signal for output process to make sure that all data were completed.
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigAllMeasurementDataDone));

                }
            }
        }

        private string DetermineHGA1ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA1ShortPadPositions(), HGA1WPlusPad:{0}, HGA1WMinusPad:{1}, HGA1TAPlusPad:{2}, HGA1TAMinusPad:{3}, HGA1WHPlusPad:{4}, HGA1WHMinusPad:{5}, HGA1RHPlusPad:{6}, HGA1RHMinusPad:{7}, HGA1R1PlusPad:{8}, HGA1R1MinusPad:{9}, HGA1R2PlusPad:{10}, HGA1R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA1WPlusPad, TestProbe10GetAllHGAShortDetection.HGA1WMinusPad, TestProbe10GetAllHGAShortDetection.HGA1TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA1TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA1WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA1WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA1RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA1RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA1R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA1R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA1R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA1R2MinusPad);


            if (TestProbe10GetAllHGAShortDetection.HGA1WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 

            if (TestProbe10GetAllHGAShortDetection.HGA1R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA1R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga1.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga1.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA2ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA1ShortPadPositions(), HGA2WPlusPad:{0}, HGA2WMinusPad:{1}, HGA2TAPlusPad:{2}, HGA2TAMinusPad:{3}, HGA2WHPlusPad:{4}, HGA2WHMinusPad:{5}, HGA2RHPlusPad:{6}, HGA2RHMinusPad:{7}, HGA2R1PlusPad:{8}, HGA2R1MinusPad:{9}, HGA2R2PlusPad:{10}, HGA2R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA2WPlusPad, TestProbe10GetAllHGAShortDetection.HGA2WMinusPad, TestProbe10GetAllHGAShortDetection.HGA2TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA2TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA2WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA2WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA2RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA2RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA2R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA2R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA2R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA2R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA2WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 

            if (TestProbe10GetAllHGAShortDetection.HGA2R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA2R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga2.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga2.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA3ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA3ShortPadPositions(), HGA3WPlusPad:{0}, HGA3WMinusPad:{1}, HGA3TAPlusPad:{2}, HGA3TAMinusPad:{3}, HGA3WHPlusPad:{4}, HGA3WHMinusPad:{5}, HGA3RHPlusPad:{6}, HGA3RHMinusPad:{7}, HGA3R1PlusPad:{8}, HGA3R1MinusPad:{9}, HGA3R2PlusPad:{10}, HGA3R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA3WPlusPad, TestProbe10GetAllHGAShortDetection.HGA3WMinusPad, TestProbe10GetAllHGAShortDetection.HGA3TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA3TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA3WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA3WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA3RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA3RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA3R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA3R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA3R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA3R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA3WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 

            if (TestProbe10GetAllHGAShortDetection.HGA3R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA3R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga3.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga3.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA4ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA4ShortPadPositions(), HGA4WPlusPad:{0}, HGA4WMinusPad:{1}, HGA4TAPlusPad:{2}, HGA4TAMinusPad:{3}, HGA4WHPlusPad:{4}, HGA4WHMinusPad:{5}, HGA4RHPlusPad:{6}, HGA4RHMinusPad:{7}, HGA4R1PlusPad:{8}, HGA4R1MinusPad:{9}, HGA4R2PlusPad:{10}, HGA4R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA4WPlusPad, TestProbe10GetAllHGAShortDetection.HGA4WMinusPad, TestProbe10GetAllHGAShortDetection.HGA4TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA4TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA4WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA4WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA4RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA4RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA4R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA4R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA4R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA4R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA4WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+ ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W- ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+ ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA- ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+ ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH- ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+ ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH- ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+ ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1- ";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA4R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+ ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA4R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga4.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga4.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA5ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA5ShortPadPositions(), HGA5WPlusPad:{0}, HGA5WMinusPad:{1}, HGA5TAPlusPad:{2}, HGA5TAMinusPad:{3}, HGA5WHPlusPad:{4}, HGA5WHMinusPad:{5}, HGA5RHPlusPad:{6}, HGA5RHMinusPad:{7}, HGA5R1PlusPad:{8}, HGA5R1MinusPad:{9}, HGA5R2PlusPad:{10}, HGA5R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA5WPlusPad, TestProbe10GetAllHGAShortDetection.HGA5WMinusPad, TestProbe10GetAllHGAShortDetection.HGA5TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA5TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA5WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA5WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA5RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA5RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA5R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA5R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA5R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA5R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA5WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA5R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA5R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga5.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga5.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA6ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA6ShortPadPositions(), HGA6WPlusPad:{0}, HGA6WMinusPad:{1}, HGA6TAPlusPad:{2}, HGA6TAMinusPad:{3}, HGA6WHPlusPad:{4}, HGA6WHMinusPad:{5}, HGA6RHPlusPad:{6}, HGA6RHMinusPad:{7}, HGA6R1PlusPad:{8}, HGA6R1MinusPad:{9}, HGA6R2PlusPad:{10}, HGA6R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA6WPlusPad, TestProbe10GetAllHGAShortDetection.HGA6WMinusPad, TestProbe10GetAllHGAShortDetection.HGA6TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA6TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA6WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA6WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA6RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA6RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA6R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA6R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA6R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA6R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA6WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA6R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA6R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga6.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga6.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA7ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA7ShortPadPositions(), HGA7WPlusPad:{0}, HGA7WMinusPad:{1}, HGA7TAPlusPad:{2}, HGA7TAMinusPad:{3}, HGAA7WHPlusPad:{4}, HGA7WHMinusPad:{5}, HGA7RHPlusPad:{6}, HGA7RHMinusPad:{7}, HGA7R1PlusPad:{8}, HGA7R1MinusPad:{9}, HGA7R2PlusPad:{10}, HGA7R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA7WPlusPad, TestProbe10GetAllHGAShortDetection.HGA7WMinusPad, TestProbe10GetAllHGAShortDetection.HGA7TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA7TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA7WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA7WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA7RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA7RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA7R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA7R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA7R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA7R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA7WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA7R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA7R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga7.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga7.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA8ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA8ShortPadPositions(), HGA8WPlusPad:{0}, HGA8WMinusPad:{1}, HGA8TAPlusPad:{2}, HGA8TAMinusPad:{3}, HGA8WHPlusPad:{4}, HGA8WHMinusPad:{5}, HGA8RHPlusPad:{6}, HGA8RHMinusPad:{7}, HGA8R1PlusPad:{8}, HGA8R1MinusPad:{9}, HGA8R2PlusPad:{10}, HGA8R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA8WPlusPad, TestProbe10GetAllHGAShortDetection.HGA8WMinusPad, TestProbe10GetAllHGAShortDetection.HGA8TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA8TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA8WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA8WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA8RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA8RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA8R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA8R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA8R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA8R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA8WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA8R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA8R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga8.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga8.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA9ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA9ShortPadPositions(), HGA9WPlusPad:{0}, HGA9WMinusPad:{1}, HGA9TAPlusPad:{2}, HGA9TAMinusPad:{3}, HGA9WHPlusPad:{4}, HGA9WHMinusPad:{5}, HGA9RHPlusPad:{6}, HGA9RHMinusPad:{7}, HGA9R1PlusPad:{8}, HGA9R1MinusPad:{9}, HGA9R2PlusPad:{10}, HGA9R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA9WPlusPad, TestProbe10GetAllHGAShortDetection.HGA9WMinusPad, TestProbe10GetAllHGAShortDetection.HGA9TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA9TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA9WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA9WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA9RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA9RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA9R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA9R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA9R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA9R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA9WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA9R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA9R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+_";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga9.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga9.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        private string DetermineHGA10ShortPadPositions()
        {
            string shortPadPosition = "";

            Log.Info(this, "In DetermineHGA10ShortPadPositions(), HGA10WPlusPad:{0}, HGA10WMinusPad:{1}, HGA10TAPlusPad:{2}, HGA10TAMinusPad:{3}, HGA10WHPlusPad:{4}, HGA10WHMinusPad:{5}, HGA10RHPlusPad:{6}, HGA10RHMinusPad:{7}, HGA10R1PlusPad:{8}, HGA10R1MinusPad:{9}, HGA10R2PlusPad:{10}, HGA10R2MinusPad:{11}",
                TestProbe10GetAllHGAShortDetection.HGA10WPlusPad, TestProbe10GetAllHGAShortDetection.HGA10WMinusPad, TestProbe10GetAllHGAShortDetection.HGA10TAPlusPad, TestProbe10GetAllHGAShortDetection.HGA10TAMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA10WHPlusPad, TestProbe10GetAllHGAShortDetection.HGA10WHMinusPad, TestProbe10GetAllHGAShortDetection.HGA10RHPlusPad, TestProbe10GetAllHGAShortDetection.HGA10RHMinusPad,
                TestProbe10GetAllHGAShortDetection.HGA10R1PlusPad, TestProbe10GetAllHGAShortDetection.HGA10R1MinusPad, TestProbe10GetAllHGAShortDetection.HGA10R2PlusPad, TestProbe10GetAllHGAShortDetection.HGA10R2MinusPad);
            if (TestProbe10GetAllHGAShortDetection.HGA10WPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10WMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "W-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10TAPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10TAMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "TA-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10WHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10WHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "wH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10RHPlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10RHMinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "rH-_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10R1PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1+_";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10R1MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R1-_";
            }

            // R2+ and R2- are not applicable for Rosewood and V11 product 
            if (TestProbe10GetAllHGAShortDetection.HGA10R2PlusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }
            if (TestProbe10GetAllHGAShortDetection.HGA10R2MinusPad == (int)ShortDetection.Short)
            {
                shortPadPosition += "R2+; ";
            }

            if (String.IsNullOrEmpty(shortPadPosition))
            {
                _carrierUnderTest.Hga10.setShortPadPosition("0");
            }
            else
            {
                _carrierUnderTest.Hga10.setShortPadPosition(shortPadPosition.Remove(shortPadPosition.Length - 1));
            }

            return shortPadPosition;
        }

        public void ResetMeasurementTestResult()
        {
            HGA1ResistanceTestPASS = true;
            HGA1CapacitanceTestPASS = true;
            HGA1ShortTestPASS = true;

            HGA2ResistanceTestPASS = true;
            HGA2CapacitanceTestPASS = true;
            HGA2ShortTestPASS = true;

            HGA3ResistanceTestPASS = true;
            HGA3CapacitanceTestPASS = true;
            HGA3ShortTestPASS = true;

            HGA4ResistanceTestPASS = true;
            HGA4CapacitanceTestPASS = true;
            HGA4ShortTestPASS = true;

            HGA5ResistanceTestPASS = true;
            HGA5CapacitanceTestPASS = true;
            HGA5ShortTestPASS = true;

            HGA6ResistanceTestPASS = true;
            HGA6CapacitanceTestPASS = true;
            HGA6ShortTestPASS = true;

            HGA7ResistanceTestPASS = true;
            HGA7CapacitanceTestPASS = true;
            HGA7ShortTestPASS = true;

            HGA8ResistanceTestPASS = true;
            HGA8CapacitanceTestPASS = true;
            HGA8ShortTestPASS = true;

            HGA9ResistanceTestPASS = true;
            HGA9CapacitanceTestPASS = true;
            HGA9ShortTestPASS = true;

            HGA10ResistanceTestPASS = true;
            HGA10CapacitanceTestPASS = true;
            HGA10ShortTestPASS = true;
            _carrierUnderTest = null;
        }

        public bool constructAndSendWriteDataBuffer(bool popUpErrorMessage)
        {
            if (CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Count == 0)
            {
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

                if (CommonFunctions.Instance.TestedCarrierQueue.Count > 0 && (!HSTMachine.Workcell.Process.IsIdleState))
                {
                    _carrierUnderTest = CommonFunctions.Instance.TestedCarrierQueue.First();
                }

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info("OutgoingTestProbeData serial data", "Sending command ID: {0}, Name: {1}, parameter: {2}, message size: {3} to uProcessor.", APICommand.CommandID, APICommand.CommandName, CommandParameterInText, APICommand.CommandSize);
                }
                testProbeComPort.Write(writeDataBuffer, 0, numBytes);
                return true;
            }
            catch (Exception ex)
            {
                if (popUpErrorMessage)
                {
                    Notify.PopUpError("Serial Port Transmission Error", "Error in constructAndSendWriteDataBuffer() function: " + ex.Message + ".");
                }
                else
                {
                    Log.Error("OutgoingTestProbeData serial data", "Error in constructAndSendWriteDataBuffer() function: " + ex.Message + ".");
                }
                HSTException.Throw(HSTErrors.TestElectronicsSerialPortCommunicationError, ex);
                return false;
            }
        }

        private void testProbeComPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte CommandStatusFromMicroprocessor = 0;
            byte CommandErrorCodeFromMicroprocessor = 0;
            bool commandSentToMicroprocessor = false;
            string CommandParameterInText = "null";
            string CommandName = "null";
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

            if (CommandSize < 5 || CommandID < 1)
            {
                return;
            }

            if (TestElectronicsStatusEvent != null)
            {
                CommandStatus commandStatus = (CommandStatus)CommandStatusFromMicroprocessor;
                TestElectronicsStatusEvent(this, new TestElectronicsStatusEventArgs(String.Format("Status: {0}", commandStatus.ToString())));
            }

            int CommandParameterSize = 0;

            if (CommandStatusFromMicroprocessor == (byte)CommandStatus.Busy)
            {
                Notify.PopUpError("uProcessor Returned Busy Status", String.Format("uProcessor was too busy to execute command: {0}.", CommandID));
                Log.Warn("uProcessor", "uProcessor was too busy to execute command: {0}.", CommandID);

                testProbeComPort.DiscardInBuffer();
                // Resend the same commands
                commandSentToMicroprocessor = constructAndSendWriteDataBuffer(false);
                return;
            }
            else if (CommandStatusFromMicroprocessor == (byte)CommandStatus.Error)
            {
                bool foundErrorMessage = false;
                foreach (var KeyvaluePair in dictionary)
                {
                    if (KeyvaluePair.Key == CommandErrorCodeFromMicroprocessor)
                    {
                        Notify.PopUpError("uProcessor Returned Error Status", String.Format("uProcessor failed to execute command: {0} due to error code: {1}, error message: {2}.", CommandID, KeyvaluePair.Key, KeyvaluePair.Value));
                        Log.Error("uProcessor", "uProcessor failed to execute command: {0} due to error code: {1}, error message: {2}.", CommandID, KeyvaluePair.Key, KeyvaluePair.Value);
                        foundErrorMessage = true;
                        break;
                    }
                }

                if (foundErrorMessage == false)
                {
                    Notify.PopUpError("uProcessor Returned Error Status", String.Format("uProcessor failed to execute command: {0} due to error code: {1}.", CommandID, CommandErrorCodeFromMicroprocessor));
                    Log.Error("uProcessor", "uProcessor failed to execute command: {0} due to error code: {1}.", CommandID, CommandErrorCodeFromMicroprocessor);
                }

                try
                {
                    try
                    {
                        throw new Exception("Test Eletronics returned command status error.");
                    }
                    catch (Exception ex)
                    {
                        HSTException.Throw(HSTErrors.TestElectronicsMeasurementError, new Exception("Test Eletronics returned command status error."));
                    }
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.Process.MonitorProcess.HandleExceptionRaisedByTestProbeUtility(ex);
                }

                testProbeComPort.DiscardInBuffer();
                return;
            }

            if (_carrierUnderTest == null && HSTMachine.Workcell.Process.IsIdleState)
            {
                // Lai: get one simulated carrier for manual measurement testing
                CarrierSettings carrierSettings = HSTMachine.Workcell.HSTSettings.SimulatedPart.Carriers.First();
                Mapper.CreateMap<CarrierSettings, Carrier>();
                _carrierUnderTest = Mapper.Map<Carrier>(carrierSettings);
                _carrierUnderTest.RFIDData = new RFIDInfo();
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[0].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[1].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[2].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[3].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[4].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[5].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[6].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[7].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[8].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.RFIDData.RFIDTagData.HGAData[9].Status = CommonFunctions.TEST_PASS_CODE;
                _carrierUnderTest.Hga1.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga2.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga3.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga4.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga5.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga6.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga7.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga8.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga9.Hga_Status = HGAStatus.HGAPresent;
                _carrierUnderTest.Hga10.Hga_Status = HGAStatus.HGAPresent;

            }

            // Only data structures from IncomingTestProbeData directory are listed here
            switch (CommandID)
            {
                case (byte)MessageID.HST_calibration_enable: // 17
                case (byte)MessageID.HST_save_calibration_data: // 19
                case (byte)MessageID.HST_eeprom_write: // 22
                case (byte)MessageID.HST_dac_write: // 24
                case (byte)MessageID.HST_dac_output_enable: // 26
                case (byte)MessageID.HST_adc_write: // 27
                case (byte)MessageID.HST_set_mux: // 30                    
                case (byte)MessageID.HST_set_cable_compensation: // 45  
                case (byte)MessageID.HST_clear_all_cable_compensation: // 46  
                case (byte)MessageID.HST_set_short_detection_threshold: // 47 
                case (byte)MessageID.HST_set_temp1_offset: // 49 
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    break;
                case (byte)MessageID.HST_config_res_meas: // 2
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_config_res_meas";
                    break;
                case (byte)MessageID.HST_config_cap_meas: // 3
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_config_cap_meas";
                    break;
                case (byte)MessageID.HST_config_short_detection: // 4
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_config_short_detection";
                    break;
                case (byte)MessageID.HST_meas_channel_enable: // 5
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_config_short_detection";
                    break;
                case (byte)MessageID.HST_hga_enable: // 6
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_hga_enable";
                    break;
                case (byte)MessageID.HST_start_meas: // 9 //First received command
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_start_meas";
                    //move here so that the TestPRobe EE can be lifted and the precisor nest can start to move before any data processing
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "testProbeComPort_DataReceived in TestProbeUtility, Publish SigHGATestingDone TimeStamp : {0}", DateTime.Now);
                    }

                    if (!_workcell.Process.TestProbeProcess.Controller.IsTriggeringNeeded())
                        QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGATestingDone));

                    _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().IssueGetMeasurementResults();

                    return;
                    break;
                case (byte)MessageID.HST_config_temp_meas: // 32            
                    // No GUI update required
                    TestProbeGetStatusAndErrorCode = TestProbeGetStatusAndErrorCode.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbeGetStatusAndErrorCode));
                    CommandName = "HST_config_temp_meas";
                    break;
                case (byte)MessageID.HST_unsolicited_status: // 255                    
                    // Issue API commands to get the measurement results because the uProcessor has completed the eletronics test
                    _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().IssueGetMeasurementResults();
                    CommandName = "HST_unsolicited_status";
                    return;
                    break;
                case (byte)MessageID.HST_get_conversion_board_id: // 7
                    TestProbe7GetConversionBoardID = TestProbe7GetConversionBoardID.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe7GetConversionBoardID));
                    CommandName = "HST_get_conversion_board_id";
                    UpdateGUIDisplay(GUIPage.MeasurementTestMainOperationPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_short_detection: // 10
                    TestProbe10GetAllHGAShortDetection = TestProbe10GetAllHGAShortDetection.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe10GetAllHGAShortDetection));
                    CommandName = "HST_get_short_detection";
                    UpdateGUIDisplay(GUIPage.MeasurementTestMainOperationPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_res_results: // 11
                    TestProbe11GetAllHGAResistanceResults = TestProbe11GetAllHGAResistanceResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe11GetAllHGAResistanceResults));
                    CommandName = "HST_get_res_results";
                    UpdateGUIDisplay(GUIPage.MeasurementTestMainOperationPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_cap_results: // 12
                    TestProbe12GetAllHGACapacitanceResults = TestProbe12GetAllHGACapacitanceResults.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe12GetAllHGACapacitanceResults));
                    CommandName = "HST_get_cap_results";
                    break;
                case (byte)MessageID.HST_get_bias_voltages: // 13
                    TestProbe13GetAllHGABiasVoltages = TestProbe13GetAllHGABiasVoltages.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe13GetAllHGABiasVoltages));
                    CommandName = "HST_get_bias_voltages";
                    UpdateGUIDisplay(GUIPage.MeasurementTestMainOperationPage, CommandID, readDataBuffer);

                    if (HSTMachine.Workcell.Process.IsIdleState)
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.UpdateDataLogForManualMeasurementTest(_carrierUnderTest);

            //        if (!_workcell.Process.TestProbeProcess.Controller.IsTriggeringNeeded())
            //            QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGATestingDone));

                    break;
                case (byte)MessageID.HST_get_temperature: // 33
                    TestProbe33GetTemperature = TestProbe33GetTemperature.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe33GetTemperature));
                    CommandName = "HST_get_temperature";
                    UpdateGUIDisplay(GUIPage.MeasurementTestMainOperationPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_start_self_test: // 36
                    TestProbe36StartSelfTest = TestProbe36StartSelfTest.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe36StartSelfTest));
                    UpdateGUIDisplay(GUIPage.FunctionalTest, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_firmware_version: // 37
                    TestProbe37GetFirmwareVersion = TestProbe37GetFirmwareVersion.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe37GetFirmwareVersion));
                    UpdateGUIDisplay(GUIPage.FunctionalTest, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_res_meas_configuration: // 55
                    TestProbe55GetResMeasConfiguration = TestProbe55GetResMeasConfiguration.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe55GetResMeasConfiguration));
                    CommandName = "HST_get_res_meas_configuration";
                    UpdateGUIDisplay(GUIPage.MeasurementTestConfigurationSetupPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_res_meas2_configuration: // 58
                    TestProbe58GetResMeas2Configuration = TestProbe58GetResMeas2Configuration.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe58GetResMeas2Configuration));
                    CommandName = "HST_get_res_meas2_configuration";
                    UpdateGUIDisplay(GUIPage.MeasurementTestConfigurationSetupPage, CommandID, readDataBuffer);
                    break;
                case (byte)MessageID.HST_get_short_detection_threshold2: // 60                                                          
                    TestProbe60GetShortDetectionThreshold = TestProbe60GetShortDetectionThreshold.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe60GetShortDetectionThreshold));
                    CommandName = "HST_get_short_detection_vol_threshold";
                    _workcell.IsToleranceSpecShortTestCompared = _workcell.CompareToleranceSpec(TestProbe60GetShortDetectionThreshold);
                    break;
                case (byte)MessageID.HST_get_ch_short_det_cur_ratio: // 67                                                             
                    TestProbe67GetCurrentRatio = TestProbe67GetCurrentRatio.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe67GetCurrentRatio));
                    CommandName = "HST_get_ch_short_det_cur_ratio";
                    try
                    {
                        BaselineVoltageRatio baselineVoltageRatio = new BaselineVoltageRatio();
                        baselineVoltageRatio.CurrentRatio_CH1 = TestProbe67GetCurrentRatio.GetCurrentRatio_CH1();
                        baselineVoltageRatio.CurrentRatio_CH2 = TestProbe67GetCurrentRatio.GetCurrentRatio_CH2();
                        baselineVoltageRatio.CurrentRatio_CH3 = TestProbe67GetCurrentRatio.GetCurrentRatio_CH3();
                        baselineVoltageRatio.CurrentRatio_CH4 = TestProbe67GetCurrentRatio.GetCurrentRatio_CH4();
                        baselineVoltageRatio.CurrentRatio_CH5 = TestProbe67GetCurrentRatio.GetCurrentRatio_CH5();
                        baselineVoltageRatio.CurrentRatio_CH6 = TestProbe67GetCurrentRatio.GetCurrentRatio_CH6();

                        _workcell.IsBaselineRatioSpecCompared = _workcell.CompareBaselineRatioSpec(baselineVoltageRatio);
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case (byte)MessageID.HST_get_short_detection_vol_threshold: // 71                                                       
                    TestProbe71GetShortDetectionVolThreshold = TestProbe71GetShortDetectionVolThreshold.ReadStruct(readDataBuffer);

                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe71GetShortDetectionVolThreshold));
                    CommandName = "HST_get_short_detection_vol_threshold";

                    _workcell.IsVolThresholdSpecCompared = _workcell.CompareVolThresholdSpec(TestProbe71GetShortDetectionVolThreshold);

                    if (!HSTMachine.Workcell.Process.IsRunState)
                    {
                        if (!_workcell.IsBaselineRatioSpecCompared || !_workcell.IsToleranceSpecShortTestCompared || !_workcell.IsVolThresholdSpecCompared)
                        {
                            Log.Info(this, "Re-download firmware for setting changed", DateTime.Now);
                            HSTMachine.Workcell.getPanelSetup().DownloadConfigurationToMicroProcessor();
                        }
                    }
                    break;
                case (byte)MessageID.HST_get_vol_delta: // 72 //Last received command
                    TestProbe72GetVolDelta = TestProbe72GetVolDelta.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe72GetVolDelta));
                    CommandName = "HST_get_vol_delta";
                    LogGetVolDelta();

                    if (_workcell.Process.TestProbeProcess.Controller.IsTriggeringNeeded())
                        QF.Instance.Publish(new QEvent(HSTWorkcell.SigTestProbeGetResultDone)); // this signal for double test only
                        QF.Instance.Publish(new QEvent(HSTWorkcell.SigHGATestingDone));

              // Thre are data loss happening .. Im not sure what causes it.. It maybe related to the thread.
             //       if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation() == true)
             //       {
             //           //make sure the thread is ended 
             //           VPDCalculationThread.Join();
             //           Log.Info(this, "Thread VPDCalculationThread end.");
             //           ApplyAllLDUAndLEDData();
             //       }
             //       else
             //       {
                        ApplyAllLDUAndLEDData();
             //       }

                    break;
                case (byte)MessageID.HST_get_all_bias_voltage: // 79 
                    TestProbe79GetAllBiasVoltage = TestProbe79GetAllBiasVoltage.ReadStruct(readDataBuffer);
                    CommandParameterSize =
                        System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe79GetAllBiasVoltage));
                    CommandName = "HST_get_all_bias_voltage";
                    LogGetAllBiasVoltage();
                    break;
                case (byte)MessageID.HST_get_all_sensing_voltage: // 80 
                    TestProbe80GetAllSensingVoltage = TestProbe80GetAllSensingVoltage.ReadStruct(readDataBuffer);
                    CommandParameterSize =
                        System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe80GetAllSensingVoltage));
                    CommandName = "HST_get_all_sensing_voltage";
                    LogGetAllSensingVoltage();
                    break;
                case (byte)MessageID.HST_get_ldu_data_by_hga:   //63
                    TestProbe63GetLDUAndLEDData = TestProbe63GetLDUAndLEDData.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe63GetLDUAndLEDData));
                    CommandName = "HST_get_ldu_data_by_hga";
                    CollectLDUAndLEDData();
                    break;
                case (byte)MessageID.HST_get_photodiode_data_by_hga: // 86
                    TestProbe86GetPhotodiodeDataByHGA = TestProbe86GetPhotodiodeDataByHGA.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe86GetPhotodiodeDataByHGA));
                    CommandName = "HST_get_photodiode_data_by_hga";
                    LogPhotoDiodeVoltageData();
                    break;
                case (byte)MessageID.HST_get_ldu_configuration_2: // 87
                    TestProbe87GetLDUConfiguration_2 = TestProbe87GetLDUConfiguration_2.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe87GetLDUConfiguration_2));
                    CommandName = "HST_get_ldu_configuration_2";
                    RestoreLDUConfiguration2();
                    break;
                case (byte)MessageID.HST_get_half_LDU_Data: // 88
                    TestProbe88GetHalfLDUAndLEDData = TestProbe88GetHalfLDUAndLEDData.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe88GetHalfLDUAndLEDData));
                    CommandName = "HST_get_half_LDU_Data";
                    GetHalfLDUAndLEDData();
                    break;
                case (byte)MessageID.HST_get_half_photodiode_data: // 89
                    TestProbe89GetHalfPhotoDiodeData = TestProbe89GetHalfPhotoDiodeData.ReadStruct(readDataBuffer);
                    CommandParameterSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TestProbe89GetHalfPhotoDiodeData));
                    CommandName = "HST_get_half_photodiode_data";
                    GetHalfPhotoDiodeData();
                    
                    if (TestProbe89GetHalfPhotoDiodeData.first_5_Hga != 1 && HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation() == true)
                    {
                        // Data is collected now.. 
                        // Time to spawn thread to calculate the y=mx+c

                        // ThreadStart ThreadGompertzCalculation = GompertzCalculation;
                        // VPDCalculationThread = new Thread(ThreadGompertzCalculation);
                        // Log.Info(this, "Thread VPDCalculationThread start.");
                        // VPDCalculationThread.Start();
                        GompertzCalculation();
                    }
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

            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info("IncomingTestProbeData serial data", "Receiving command ID: {0}, Name: {1}, parameter: {2}, message size: {3} from uProcessor. Calculated CheckSum: {4}, Adjusted CheckSum: {5}, Received CheckSum: {6}.", CommandID, CommandName, CommandParameterInText, CommandSize, CalculatedMessageCheckSum, AdjustedCalculatedMessageCheckSum, CommandCheckSum);
            }

            if (AdjustedCalculatedMessageCheckSum > 0 && CommandCheckSum > 0)
            {
                if (AdjustedCalculatedMessageCheckSum != CommandCheckSum)
                {
                    Notify.PopUpError("CheckSum Mismatch Error", String.Format("Calculated CheckSum: {0}, Adjusted CheckSum: {1}, Received CheckSum: {2}.", CalculatedMessageCheckSum, AdjustedCalculatedMessageCheckSum, CommandCheckSum));
                    Log.Error("CheckSum Mismatch Error", "Calculated CheckSum: {0}, Adjusted CheckSum: {1}, Received CheckSum: {2}.", CalculatedMessageCheckSum, AdjustedCalculatedMessageCheckSum, CommandCheckSum);
                    return;
                }
            }

            if (CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Count > 0)
            {
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Dequeue();
            }

            Thread.Sleep(50);

            commandSentToMicroprocessor = constructAndSendWriteDataBuffer(false);
        }

        public bool DeltaISIResistanceComparision(string carrierID, int hga_slot, double Reader1Resistance, double Reader2Resistance, double SLDR_RES_RD1, double SLDR_RES_RD2)
        {
            bool isDeltaPass = false;
            bool isCh5R1Pass = false;
            bool isCh6R2Pass = false;
            bool applyDataFromList = false;

            DeltaISI_Tolerance1 = 0;
            DeltaISI_Tolerance2 = 0;

            try
            {
                if (SLDR_RES_RD1 == 0 || SLDR_RES_RD2 == 0)
                {
                    var getISIbyCarrier = _workcell.ISIDataListIn.Where(P => P.Key == carrierID).ToList();
                    var getISIbySlot = getISIbyCarrier.FirstOrDefault().Value.ElementAt(hga_slot - 1);

                    if (getISIbySlot != null && getISIbySlot.HgaSN != string.Empty)
                    {
                        if (!Math.Equals(SLDR_RES_RD1, getISIbySlot.ISIReader1Data))
                            applyDataFromList = true;
                        if (!Math.Equals(SLDR_RES_RD2, getISIbySlot.ISIReader2Data))
                            applyDataFromList = true;
                    }

                    if (applyDataFromList)
                    {
                        SLDR_RES_RD1 = getISIbySlot.ISIReader1Data;
                        SLDR_RES_RD2 = getISIbySlot.ISIReader2Data;
                    }
                }
            }
            catch (Exception)
            {
            }

            //Disable until this project is ready
            //if (_carrierUnderTest.Hga1.IBS_Data.Current_RD_Pattern.Contains("1"))
            //    CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 = 0;

            //if (_carrierUnderTest.Hga1.IBS_Data.Current_RD_Pattern.Contains("2"))
            //    CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 = 0;

            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
            {
                if (SLDR_RES_RD1 != 0)
                {
                    
                    if ((CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1) != 0)
                    {
                        DeltaISI_Tolerance1 = Math.Abs(((Reader1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.ReaderImpedanceR1Spec) -  SLDR_RES_RD1)
                                                                    / SLDR_RES_RD1) * 100;
                        if (DeltaISI_Tolerance1 < CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1)
                            isCh5R1Pass = true;
                    }
                    else // if new spec
                    {
                        DeltaISI_Tolerance1 = (((Reader1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.ReaderImpedanceR1Spec) - SLDR_RES_RD1) / SLDR_RES_RD1) * 100;
                       if ((DeltaISI_Tolerance1 > CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1N) && (DeltaISI_Tolerance1 < CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1P))
                            isCh5R1Pass = true;                                                                                    
                    }
                }
            }
            else
            {
                isCh5R1Pass = true;
            }

            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 &&
                !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
            {
                if (SLDR_RES_RD2 != 0)
                {
                    if ((CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2) != 0)
                    {
                        DeltaISI_Tolerance2 = Math.Abs(((Reader2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.ReaderImpedanceR2Spec) - SLDR_RES_RD2)
                                                       / SLDR_RES_RD2) * 100;
                        if (DeltaISI_Tolerance2 < CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2)
                            isCh6R2Pass = true;
                    }
                    else // if new version of spec
                    {
                        DeltaISI_Tolerance2 = (((Reader2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.ReaderImpedanceR2Spec) - SLDR_RES_RD2)
                                                                                   / SLDR_RES_RD2) * 100;
                        if ((DeltaISI_Tolerance2 > CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2N) && (DeltaISI_Tolerance2 < CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2P))
                            isCh6R2Pass = true;             
                    }
                }
            }
            else
            {
                isCh6R2Pass = true;
            }

            if ((isCh5R1Pass) && (isCh6R2Pass))
            {
                isDeltaPass = true;
            }

            return isDeltaPass;
        }

        public bool DeltaR2ISIWriterSDETComparision(string carrierID, int hga_slot, double Reader2Resistance, double SDET_RES_RD2)
        {
            bool isComparePass = false;

            DeltaISI_R2_SDET_Tolerance = 0;

            if (((CommonFunctions.Instance.MeasurementTestRecipe.DeltaR2SpecMoreThan - CommonFunctions.Instance.MeasurementTestRecipe.DeltaR2SpecLessThan) !=0) &&
                !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
            {
                if (SDET_RES_RD2 != 0)
                {
                    DeltaISI_R2_SDET_Tolerance = (((Reader2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - SDET_RES_RD2)
                                                    / SDET_RES_RD2) * 100;

                    if ((DeltaISI_R2_SDET_Tolerance > CommonFunctions.Instance.MeasurementTestRecipe.DeltaR2SpecLessThan) &&
                        (DeltaISI_R2_SDET_Tolerance < CommonFunctions.Instance.MeasurementTestRecipe.DeltaR2SpecMoreThan))
                        isComparePass = true;
                }
            }
            else
            {
                isComparePass = true;
            }

            return isComparePass;
        }

        public bool DeltaR1ISIWriterSDETComparision(string carrierID, int hga_slot, double Reader1Resistance, double SDET_RES_RD1)
        {
            bool isComparePass = false;

            DeltaISI_R1_SDET_Tolerance = 0;

            if (((CommonFunctions.Instance.MeasurementTestRecipe.DeltaR1SpecMoreThan - CommonFunctions.Instance.MeasurementTestRecipe.DeltaR1SpecLessThan) != 0) &&
                !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
            {
                if (SDET_RES_RD1 != 0)
                {
                    //       DeltaISI_R2_SDET_Tolerance = Math.Abs(((Reader2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - SDET_RES_RD2)
                    //                                       / SDET_RES_RD2) * 100;
                    DeltaISI_R1_SDET_Tolerance = (((Reader1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - SDET_RES_RD1)
                                                    / SDET_RES_RD1) * 100;

                    if ((DeltaISI_R1_SDET_Tolerance > CommonFunctions.Instance.MeasurementTestRecipe.DeltaR1SpecLessThan) &&
                        (DeltaISI_R1_SDET_Tolerance < CommonFunctions.Instance.MeasurementTestRecipe.DeltaR1SpecMoreThan))
                        isComparePass = true;
                }
            }
            else
            {
                isComparePass = true;
            }

            return isComparePass;
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
                List<TESTED_DATA_MAP> testedDataMap = new List<TESTED_DATA_MAP>();
                switch (page)
                {
                    case GUIPage.MeasurementTestMainOperationPage:
                        #region HST_get_temperature
                        // HST_get_temperature                
                        if (CommandID == (byte)MessageID.HST_get_temperature)
                        {
                            /* Ch1Temperature0: Measurement Board Temperature 
                               Ch2Temperature50: Working Zone Temperature 
                               Ch3Temperature100: Exhaust Temperature */
                            double Ch1Temperature0 = (TestProbe33GetTemperature.Ch1Temperature() / 10.0);
                            double Ch2Temperature50 = (TestProbe33GetTemperature.Ch2Temperature() / 10.0);
                            double Ch3Temperature100 = (TestProbe33GetTemperature.Ch3Temperature() / 10.0);

                            if (TemperaturesEvent != null)
                                TemperaturesEvent(this, new TemperaturesEventArgs(Ch1Temperature0.ToString(), Ch2Temperature50.ToString(), Ch3Temperature100.ToString()));

                            {
                                Log.Info(this, "Carrier ID : {0}, Measurement results returned by Command Name: HST_get_temperature", _carrierUnderTest.CarrierID);
                                Log.Info(this, "Measurement Board Temperature: {0} degrees, Working Zone Temperature: {1} degrees, Exhaust Temperature: {2} degrees", Ch1Temperature0, Ch2Temperature50, Ch3Temperature100);

                                // Ch1Temperature0: Measurement Board Temperature 
                                _carrierUnderTest.Hga1.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga2.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga3.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga4.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga5.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga6.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga7.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga8.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga9.setCh1Temperature(Ch1Temperature0);
                                _carrierUnderTest.Hga10.setCh1Temperature(Ch1Temperature0);

                                // Ch2Temperature50: Working Zone Temperature 
                                _carrierUnderTest.Hga1.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga2.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga3.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga4.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga5.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga6.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga7.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga8.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga9.setCh2Temperature(Ch2Temperature50);
                                _carrierUnderTest.Hga10.setCh2Temperature(Ch2Temperature50);

                                // Ch3Temperature100: Exhaust Temperature
                                _carrierUnderTest.Hga1.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga2.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga3.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga4.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga5.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga6.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga7.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga8.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga9.setCh3Temperature(Ch3Temperature100);
                                _carrierUnderTest.Hga10.setCh3Temperature(Ch3Temperature100);
                            }
                        }
                        #endregion

                        #region HST_get_bias_voltages
                        // HST_get_bias_voltages
                        if (CommandID == (byte)MessageID.HST_get_bias_voltages)
                        {
                            if (HSTMachine.Workcell.Process.IsIdleState)
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = true;
                            }
                            double HGA1Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA1WriterBias() / 1000.0);
                            double HGA1Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA1TABias() / 1000.0);
                            double HGA1Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA1WHBias() / 1000.0);
                            double HGA1Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA1RHBias() / 1000.0);
                            double HGA1Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA1R1Bias() / 1000.0);
                            double HGA1Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA1R2Bias() / 1000.0);

                            double HGA2Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA2WriterBias() / 1000.0);
                            double HGA2Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA2TABias() / 1000.0);
                            double HGA2Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA2WHBias() / 1000.0);
                            double HGA2Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA2RHBias() / 1000.0);
                            double HGA2Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA2R1Bias() / 1000.0);
                            double HGA2Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA2R2Bias() / 1000.0);

                            double HGA3Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA3WriterBias() / 1000.0);
                            double HGA3Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA3TABias() / 1000.0);
                            double HGA3Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA3WHBias() / 1000.0);
                            double HGA3Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA3RHBias() / 1000.0);
                            double HGA3Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA3R1Bias() / 1000.0);
                            double HGA3Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA3R2Bias() / 1000.0);

                            double HGA4Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA4WriterBias() / 1000.0);
                            double HGA4Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA4TABias() / 1000.0);
                            double HGA4Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA4WHBias() / 1000.0);
                            double HGA4Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA4RHBias() / 1000.0);
                            double HGA4Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA4R1Bias() / 1000.0);
                            double HGA4Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA4R2Bias() / 1000.0);

                            double HGA5Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA5WriterBias() / 1000.0);
                            double HGA5Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA5TABias() / 1000.0);
                            double HGA5Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA5WHBias() / 1000.0);
                            double HGA5Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA5RHBias() / 1000.0);
                            double HGA5Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA5R1Bias() / 1000.0);
                            double HGA5Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA5R2Bias() / 1000.0);

                            double HGA6Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA6WriterBias() / 1000.0);
                            double HGA6Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA6TABias() / 1000.0);
                            double HGA6Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA6WHBias() / 1000.0);
                            double HGA6Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA6RHBias() / 1000.0);
                            double HGA6Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA6R1Bias() / 1000.0);
                            double HGA6Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA6R2Bias() / 1000.0);

                            double HGA7Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA7WriterBias() / 1000.0);
                            double HGA7Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA7TABias() / 1000.0);
                            double HGA7Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA7WHBias() / 1000.0);
                            double HGA7Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA7RHBias() / 1000.0);
                            double HGA7Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA7R1Bias() / 1000.0);
                            double HGA7Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA7R2Bias() / 1000.0);

                            double HGA8Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA8WriterBias() / 1000.0);
                            double HGA8Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA8TABias() / 1000.0);
                            double HGA8Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA8WHBias() / 1000.0);
                            double HGA8Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA8RHBias() / 1000.0);
                            double HGA8Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA8R1Bias() / 1000.0);
                            double HGA8Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA8R2Bias() / 1000.0);

                            double HGA9Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA9WriterBias() / 1000.0);
                            double HGA9Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA9TABias() / 1000.0);
                            double HGA9Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA9WHBias() / 1000.0);
                            double HGA9Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA9RHBias() / 1000.0);
                            double HGA9Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA9R1Bias() / 1000.0);
                            double HGA9Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA9R2Bias() / 1000.0);

                            double HGA10Ch1WriterBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA10WriterBias() / 1000.0);
                            double HGA10Ch2TABiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA10TABias() / 1000.0);
                            double HGA10Ch3WHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA10WHBias() / 1000.0);
                            double HGA10Ch4RHBiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA10RHBias() / 1000.0);
                            double HGA10Ch5R1BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA10R1Bias() / 1000.0);
                            double HGA10Ch6R2BiasVoltage = (TestProbe13GetAllHGABiasVoltages.HGA10R2Bias() / 1000.0);

                            {
                                Log.Info(this, "Carrier ID : {0}, Measurement results returned by Command Name: HST_get_bias_voltages", _carrierUnderTest.CarrierID);
                                _carrierUnderTest.Hga1.setBiasVoltage(HGA1Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga2.setBiasVoltage(HGA2Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga3.setBiasVoltage(HGA3Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga4.setBiasVoltage(HGA4Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga5.setBiasVoltage(HGA5Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga6.setBiasVoltage(HGA6Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga7.setBiasVoltage(HGA7Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga8.setBiasVoltage(HGA8Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga9.setBiasVoltage(HGA9Ch5R1BiasVoltage);
                                _carrierUnderTest.Hga10.setBiasVoltage(HGA10Ch5R1BiasVoltage);
                            }

                            // HGA1
                            if (HGA1Present == false)
                            {
                                HGA1Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga1.setBiasVoltage(HGA1Ch5R1BiasVoltage);
                            }

                            // HGA2
                            if (HGA2Present == false)
                            {
                                HGA2Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga2.setBiasVoltage(HGA2Ch5R1BiasVoltage);
                            }

                            // HGA3
                            if (HGA3Present == false)
                            {
                                HGA3Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga3.setBiasVoltage(HGA3Ch5R1BiasVoltage);
                            }

                            // HGA4
                            if (HGA4Present == false)
                            {
                                HGA4Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga4.setBiasVoltage(HGA4Ch5R1BiasVoltage);
                            }

                            // HGA5
                            if (HGA5Present == false)
                            {
                                HGA5Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga5.setBiasVoltage(HGA5Ch5R1BiasVoltage);
                            }

                            // HGA6
                            if (HGA6Present == false)
                            {
                                HGA6Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga6.setBiasVoltage(HGA6Ch5R1BiasVoltage);
                            }

                            // HGA7
                            if (HGA7Present == false)
                            {
                                HGA7Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga7.setBiasVoltage(HGA7Ch5R1BiasVoltage);
                            }

                            // HGA8
                            if (HGA8Present == false)
                            {
                                HGA8Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga8.setBiasVoltage(HGA8Ch5R1BiasVoltage);
                            }

                            // HGA9
                            if (HGA9Present == false)
                            {
                                HGA9Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga9.setBiasVoltage(HGA9Ch5R1BiasVoltage);
                            }

                            // HGA10
                            if (HGA10Present == false)
                            {
                                HGA10Ch5R1BiasVoltage = 0;
                                _carrierUnderTest.Hga10.setBiasVoltage(HGA10Ch5R1BiasVoltage);
                            }

                            var dataTestedRequestEvent = new QEvent(HSTWorkcell.SigOverallMeasurementDone);
                            dataTestedRequestEvent.EventObject = _carrierUnderTest;
                            QF.Instance.Publish(dataTestedRequestEvent);

                        }
                        #endregion

                        #region HST_get_res_results
                        // HST_get_res_results
                        if (CommandID == (byte)MessageID.HST_get_res_results)
                        {
                            double ch1WrOffset = 1.0;
                            double ch2TaOffset = 1.0;
                            double ch3WhOffset = 1.0;
                            double ch4RhOffset = 1.0;
                            double ch5Rd1Offset = 1.0;
                            double ch6Rd2Offset = 1.0;

                            _carrierUnderTest.CapacitanceCH1_Enable = false;        //Fix status cap test to disable
                             
                        //    if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation() == true)
                        //    {
                        //        //make sure the tread is ended 
                        //        VPDCalculationThread.Join();
                        //        Log.Info(this, "Thread VPDCalculationThread end.");
                        //    }
                            if (CommonFunctions.Instance.IsRunningWithNewTSR)
                            {
                                ch1WrOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceOffset;
                                ch2TaOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceOffset;
                                ch3WhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceOffset;
                                ch4RhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceOffset;
                                ch5Rd1Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceOffset;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceOffset;
                                else
                                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceOffset;
                            }
                            else
                            {
                                ch1WrOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceOffset / 1000;
                                ch2TaOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceOffset / 1000;
                                ch3WhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceOffset / 1000;
                                ch4RhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceOffset / 1000;
                                ch5Rd1Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceOffset / 1000;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceOffset / 1000;
                                else
                                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceOffset / 1000;
                            }

                            double HGA1Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA1Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA1Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA1Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA1Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA1Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA1LEDInterceptVoltage = 0;
                            double HGA1LDU_IThreshold = 0;
                            double HGA1VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA1Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[0] * 1000;
                                HGA1LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[0];
                                if(_carrierUnderTest.Hga1.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA1LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[0];
                                }
                                else
                                {
                                    HGA1LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[0];
                                }
                               
                                HGA1VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[0];
                                _carrierUnderTest.Hga1.Delta_IThreshold = (HGA1LDU_IThreshold -
                                                                           _carrierUnderTest.Hga1.Last_ET_Threshold);
                            }

                            double HGA2Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA2Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA2Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA2Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA2Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA2Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA2LEDInterceptVoltage = 0;
                            double HGA2LDU_IThreshold = 0;
                            double HGA2VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA2Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[1] * 1000;
                                HGA2LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[1];
                                // HGA2LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[1];
                                if (_carrierUnderTest.Hga2.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA2LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[1];
                                }
                                else
                                {
                                    HGA2LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[1];
                                }
                                HGA2VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[1];
                                _carrierUnderTest.Hga2.Delta_IThreshold = (HGA2LDU_IThreshold -
                                                                           _carrierUnderTest.Hga2.Last_ET_Threshold);
                            }

                            double HGA3Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA3Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA3Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA3Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA3Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA3Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA3LEDInterceptVoltage = 0;
                            double HGA3LDU_IThreshold = 0;
                            double HGA3VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA3Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[2] * 1000;
                                HGA3LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[2];
                                //    HGA3LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[2];
                                if (_carrierUnderTest.Hga3.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA3LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[2];
                                }
                                else
                                {
                                    HGA3LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[2];
                                }
                                HGA3VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[2];
                                _carrierUnderTest.Hga3.Delta_IThreshold = (HGA3LDU_IThreshold -
                                           _carrierUnderTest.Hga3.Last_ET_Threshold);
                            }

                            double HGA4Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA4Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA4Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA4Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA4Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA4Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA4LEDInterceptVoltage = 0;
                            double HGA4LDU_IThreshold = 0;
                            double HGA4VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA4Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[3] * 1000;
                                HGA4LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[3];
                                //    HGA4LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[3];
                                if (_carrierUnderTest.Hga4.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA4LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[3];
                                }
                                else
                                {
                                    HGA4LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[3];
                                }
                                HGA4VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[3];
                                _carrierUnderTest.Hga4.Delta_IThreshold = (HGA4LDU_IThreshold -
                                           _carrierUnderTest.Hga4.Last_ET_Threshold);
                            }

                            double HGA5Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA5Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA5Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA5Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA5Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA5Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA5LEDInterceptVoltage = 0;
                            double HGA5LDU_IThreshold = 0;
                            double HGA5VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA5Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[4] * 1000;
                                HGA5LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[4];
                                //    HGA5LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[4];
                                if (_carrierUnderTest.Hga5.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA5LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[4];
                                }
                                else
                                {
                                    HGA5LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[4];
                                }
                                HGA5VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[4];
                                _carrierUnderTest.Hga5.Delta_IThreshold = (HGA5LDU_IThreshold -
                                           _carrierUnderTest.Hga5.Last_ET_Threshold);
                            }

                            double HGA6Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA6Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA6Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA6Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA6Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA6Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA6LEDInterceptVoltage = 0;
                            double HGA6LDU_IThreshold = 0;
                            double HGA6VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA6Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[5] * 1000;
                                HGA6LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[5];
                                //    HGA6LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[5];
                                if (_carrierUnderTest.Hga6.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA6LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[5];
                                }
                                else
                                {
                                    HGA6LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[5];
                                }
                                HGA6VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[5];
                                _carrierUnderTest.Hga6.Delta_IThreshold = (HGA6LDU_IThreshold -
                                           _carrierUnderTest.Hga6.Last_ET_Threshold);
                            }

                            double HGA7Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA7Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA7Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA7Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA7Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA7Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA7LEDInterceptVoltage = 0;
                            double HGA7LDU_IThreshold = 0;
                            double HGA7VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA7Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[6] * 1000;
                                HGA7LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[6];
                                //    HGA7LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[6];
                                if (_carrierUnderTest.Hga7.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA7LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[6];
                                }
                                else
                                {
                                    HGA7LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[6];
                                }
                                HGA7VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[6];
                                _carrierUnderTest.Hga7.Delta_IThreshold = (HGA7LDU_IThreshold -
                                           _carrierUnderTest.Hga7.Last_ET_Threshold);
                            }

                            double HGA8Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA8Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA8Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA8Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA8Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA8Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA8LEDInterceptVoltage = 0;
                            double HGA8LDU_IThreshold = 0;
                            double HGA8VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA8Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[7] * 1000;
                                HGA8LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[7];
                                //   HGA8LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[7];
                                if (_carrierUnderTest.Hga8.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA8LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[7];
                                }
                                else
                                {
                                    HGA8LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[7];
                                }
                                HGA8VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[7];
                                _carrierUnderTest.Hga8.Delta_IThreshold = (HGA8LDU_IThreshold -
                                           _carrierUnderTest.Hga8.Last_ET_Threshold);
                            }

                            double HGA9Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA9Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA9Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA9Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA9Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA9Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA9LEDInterceptVoltage = 0;
                            double HGA9LDU_IThreshold = 0;
                            double HGA9VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA9Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[8] * 1000;
                                HGA9LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[8];
                                //    HGA9LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[8];
                                if (_carrierUnderTest.Hga9.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA9LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[8];
                                }
                                else
                                {
                                    HGA9LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[8];
                                }
                                HGA9VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[8];
                                _carrierUnderTest.Hga9.Delta_IThreshold = (HGA9LDU_IThreshold -
                                           _carrierUnderTest.Hga9.Last_ET_Threshold);
                            }

                            double HGA10Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                            double HGA10Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                            double HGA10Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                            double HGA10Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                            double HGA10Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                            double HGA10Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);

                            //ldu
                            double HGA10LEDInterceptVoltage = 0;
                            double HGA10LDU_IThreshold = 0;
                            double HGA10VPDMax = 0;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                HGA10Ch6R2Resistance = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[9] * 1000;
                                HGA10LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[9];
                                //    HGA10LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[9];
                                if (_carrierUnderTest.Hga10.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                                {
                                    HGA10LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[9];
                                }
                                else
                                {
                                    HGA10LDU_IThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[9];
                                }
                                HGA10VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[9];
                                _carrierUnderTest.Hga10.Delta_IThreshold = (HGA10LDU_IThreshold -
                                           _carrierUnderTest.Hga10.Last_ET_Threshold);
                            }

                            //double AvgResistanceSpec = (CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) / 2;
                            //double AvgCapacitanceSpec = (CommonFunctions.Instance.MeasurementTestRecipe.Ch1CapacitanceMin + CommonFunctions.Instance.MeasurementTestRecipe.Ch1CapacitanceMax) / 2;


                            {
                                Log.Info(this, "Carrier ID : {0}, Measurement results returned by Command Name: HST_get_res_results", _carrierUnderTest.CarrierID);

                                //Before Offset
                                _carrierUnderTest.Hga1.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA1Ch1WriterResistance());
                                _carrierUnderTest.Hga1.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA1Ch2TAResistance());
                                _carrierUnderTest.Hga1.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA1Ch3WHResistance());
                                _carrierUnderTest.Hga1.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA1Ch4RHResistance());
                                _carrierUnderTest.Hga1.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA1Ch5R1Resistance());
                                _carrierUnderTest.Hga1.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA1Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga1.setWriterResistance(HGA1Ch1WriterResistance);
                                _carrierUnderTest.Hga1.setTAResistance(HGA1Ch2TAResistance);
                                _carrierUnderTest.Hga1.setWHeaterResistance(HGA1Ch3WHResistance);
                                _carrierUnderTest.Hga1.setRHeaterResistance(HGA1Ch4RHResistance);
                                _carrierUnderTest.Hga1.setReader1Resistance(HGA1Ch5R1Resistance);
                                _carrierUnderTest.Hga1.setReader2Resistance(HGA1Ch6R2Resistance);

                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga1.set_ldu_Threshold_Ma(HGA1LDU_IThreshold);
                                    _carrierUnderTest.Hga1.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {
                                    _carrierUnderTest.Hga1.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga1.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga2.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA2Ch1WriterResistance());
                                _carrierUnderTest.Hga2.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA2Ch2TAResistance());
                                _carrierUnderTest.Hga2.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA2Ch3WHResistance());
                                _carrierUnderTest.Hga2.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA2Ch4RHResistance());
                                _carrierUnderTest.Hga2.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA2Ch5R1Resistance());
                                _carrierUnderTest.Hga2.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA2Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga2.setWriterResistance(HGA2Ch1WriterResistance);
                                _carrierUnderTest.Hga2.setTAResistance(HGA2Ch2TAResistance);
                                _carrierUnderTest.Hga2.setWHeaterResistance(HGA2Ch3WHResistance);
                                _carrierUnderTest.Hga2.setRHeaterResistance(HGA2Ch4RHResistance);
                                _carrierUnderTest.Hga2.setReader1Resistance(HGA2Ch5R1Resistance);
                                _carrierUnderTest.Hga2.setReader2Resistance(HGA2Ch6R2Resistance);

                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga2.set_ldu_Threshold_Ma(HGA2LDU_IThreshold);
                                    _carrierUnderTest.Hga2.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {
                                _carrierUnderTest.Hga2.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga2.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga3.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA3Ch1WriterResistance());
                                _carrierUnderTest.Hga3.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA3Ch2TAResistance());
                                _carrierUnderTest.Hga3.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA3Ch3WHResistance());
                                _carrierUnderTest.Hga3.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA3Ch4RHResistance());
                                _carrierUnderTest.Hga3.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA3Ch5R1Resistance());
                                _carrierUnderTest.Hga3.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA3Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga3.setWriterResistance(HGA3Ch1WriterResistance);
                                _carrierUnderTest.Hga3.setTAResistance(HGA3Ch2TAResistance);
                                _carrierUnderTest.Hga3.setWHeaterResistance(HGA3Ch3WHResistance);
                                _carrierUnderTest.Hga3.setRHeaterResistance(HGA3Ch4RHResistance);
                                _carrierUnderTest.Hga3.setReader1Resistance(HGA3Ch5R1Resistance);
                                _carrierUnderTest.Hga3.setReader2Resistance(HGA3Ch6R2Resistance);

                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga3.set_ldu_Threshold_Ma(HGA3LDU_IThreshold);
                                    _carrierUnderTest.Hga3.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga3.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga3.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga4.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA4Ch1WriterResistance());
                                _carrierUnderTest.Hga4.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA4Ch2TAResistance());
                                _carrierUnderTest.Hga4.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA4Ch3WHResistance());
                                _carrierUnderTest.Hga4.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA4Ch4RHResistance());
                                _carrierUnderTest.Hga4.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA4Ch5R1Resistance());
                                _carrierUnderTest.Hga4.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA4Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga4.setWriterResistance(HGA4Ch1WriterResistance);
                                _carrierUnderTest.Hga4.setTAResistance(HGA4Ch2TAResistance);
                                _carrierUnderTest.Hga4.setWHeaterResistance(HGA4Ch3WHResistance);
                                _carrierUnderTest.Hga4.setRHeaterResistance(HGA4Ch4RHResistance);
                                _carrierUnderTest.Hga4.setReader1Resistance(HGA4Ch5R1Resistance);
                                _carrierUnderTest.Hga4.setReader2Resistance(HGA4Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga4.set_ldu_Threshold_Ma(HGA4LDU_IThreshold);
                                    _carrierUnderTest.Hga4.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga4.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga4.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga5.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA5Ch1WriterResistance());
                                _carrierUnderTest.Hga5.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA5Ch2TAResistance());
                                _carrierUnderTest.Hga5.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA5Ch3WHResistance());
                                _carrierUnderTest.Hga5.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA5Ch4RHResistance());
                                _carrierUnderTest.Hga5.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA5Ch5R1Resistance());
                                _carrierUnderTest.Hga5.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA5Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga5.setWriterResistance(HGA5Ch1WriterResistance);
                                _carrierUnderTest.Hga5.setTAResistance(HGA5Ch2TAResistance);
                                _carrierUnderTest.Hga5.setWHeaterResistance(HGA5Ch3WHResistance);
                                _carrierUnderTest.Hga5.setRHeaterResistance(HGA5Ch4RHResistance);
                                _carrierUnderTest.Hga5.setReader1Resistance(HGA5Ch5R1Resistance);
                                _carrierUnderTest.Hga5.setReader2Resistance(HGA5Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga5.set_ldu_Threshold_Ma(HGA5LDU_IThreshold);
                                    _carrierUnderTest.Hga5.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga5.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga5.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga6.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA6Ch1WriterResistance());
                                _carrierUnderTest.Hga6.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA6Ch2TAResistance());
                                _carrierUnderTest.Hga6.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA6Ch3WHResistance());
                                _carrierUnderTest.Hga6.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA6Ch4RHResistance());
                                _carrierUnderTest.Hga6.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA6Ch5R1Resistance());
                                _carrierUnderTest.Hga6.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA6Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga6.setWriterResistance(HGA6Ch1WriterResistance);
                                _carrierUnderTest.Hga6.setTAResistance(HGA6Ch2TAResistance);
                                _carrierUnderTest.Hga6.setWHeaterResistance(HGA6Ch3WHResistance);
                                _carrierUnderTest.Hga6.setRHeaterResistance(HGA6Ch4RHResistance);
                                _carrierUnderTest.Hga6.setReader1Resistance(HGA6Ch5R1Resistance);
                                _carrierUnderTest.Hga6.setReader2Resistance(HGA6Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga6.set_ldu_Threshold_Ma(HGA6LDU_IThreshold);
                                    _carrierUnderTest.Hga6.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga6.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga6.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga7.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA7Ch1WriterResistance());
                                _carrierUnderTest.Hga7.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA7Ch2TAResistance());
                                _carrierUnderTest.Hga7.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA7Ch3WHResistance());
                                _carrierUnderTest.Hga7.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA7Ch4RHResistance());
                                _carrierUnderTest.Hga7.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA7Ch5R1Resistance());
                                _carrierUnderTest.Hga7.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA7Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga7.setWriterResistance(HGA7Ch1WriterResistance);
                                _carrierUnderTest.Hga7.setTAResistance(HGA7Ch2TAResistance);
                                _carrierUnderTest.Hga7.setWHeaterResistance(HGA7Ch3WHResistance);
                                _carrierUnderTest.Hga7.setRHeaterResistance(HGA7Ch4RHResistance);
                                _carrierUnderTest.Hga7.setReader1Resistance(HGA7Ch5R1Resistance);
                                _carrierUnderTest.Hga7.setReader2Resistance(HGA7Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga7.set_ldu_Threshold_Ma(HGA7LDU_IThreshold);
                                    _carrierUnderTest.Hga7.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga7.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga7.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga8.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA8Ch1WriterResistance());
                                _carrierUnderTest.Hga8.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA8Ch2TAResistance());
                                _carrierUnderTest.Hga8.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA8Ch3WHResistance());
                                _carrierUnderTest.Hga8.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA8Ch4RHResistance());
                                _carrierUnderTest.Hga8.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA8Ch5R1Resistance());
                                _carrierUnderTest.Hga8.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA8Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga8.setWriterResistance(HGA8Ch1WriterResistance);
                                _carrierUnderTest.Hga8.setTAResistance(HGA8Ch2TAResistance);
                                _carrierUnderTest.Hga8.setWHeaterResistance(HGA8Ch3WHResistance);
                                _carrierUnderTest.Hga8.setRHeaterResistance(HGA8Ch4RHResistance);
                                _carrierUnderTest.Hga8.setReader1Resistance(HGA8Ch5R1Resistance);
                                _carrierUnderTest.Hga8.setReader2Resistance(HGA8Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga8.set_ldu_Threshold_Ma(HGA8LDU_IThreshold);
                                    _carrierUnderTest.Hga8.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga8.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga8.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga9.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA9Ch1WriterResistance());
                                _carrierUnderTest.Hga9.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA9Ch2TAResistance());
                                _carrierUnderTest.Hga9.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA9Ch3WHResistance());
                                _carrierUnderTest.Hga9.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA9Ch4RHResistance());
                                _carrierUnderTest.Hga9.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA9Ch5R1Resistance());
                                _carrierUnderTest.Hga9.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA9Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga9.setWriterResistance(HGA9Ch1WriterResistance);
                                _carrierUnderTest.Hga9.setTAResistance(HGA9Ch2TAResistance);
                                _carrierUnderTest.Hga9.setWHeaterResistance(HGA9Ch3WHResistance);
                                _carrierUnderTest.Hga9.setRHeaterResistance(HGA9Ch4RHResistance);
                                _carrierUnderTest.Hga9.setReader1Resistance(HGA9Ch5R1Resistance);
                                _carrierUnderTest.Hga9.setReader2Resistance(HGA9Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga9.set_ldu_Threshold_Ma(HGA9LDU_IThreshold);
                                    _carrierUnderTest.Hga9.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga9.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga9.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());

                                //Before Offset
                                _carrierUnderTest.Hga10.setWRResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA10Ch1WriterResistance());
                                _carrierUnderTest.Hga10.setTAResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA10Ch2TAResistance());
                                _carrierUnderTest.Hga10.setWHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA10Ch3WHResistance());
                                _carrierUnderTest.Hga10.setRHResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA10Ch4RHResistance());
                                _carrierUnderTest.Hga10.setR1ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA10Ch5R1Resistance());
                                _carrierUnderTest.Hga10.setR2ResBeforeOffset(TestProbe11GetAllHGAResistanceResults.HGA10Ch6R2Resistance());

                                //After Offset
                                _carrierUnderTest.Hga10.setWriterResistance(HGA10Ch1WriterResistance);
                                _carrierUnderTest.Hga10.setTAResistance(HGA10Ch2TAResistance);
                                _carrierUnderTest.Hga10.setWHeaterResistance(HGA10Ch3WHResistance);
                                _carrierUnderTest.Hga10.setRHeaterResistance(HGA10Ch4RHResistance);
                                _carrierUnderTest.Hga10.setReader1Resistance(HGA10Ch5R1Resistance);
                                _carrierUnderTest.Hga10.setReader2Resistance(HGA10Ch6R2Resistance);
                                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    _carrierUnderTest.Hga10.set_ldu_Threshold_Ma(HGA10LDU_IThreshold);
                                    _carrierUnderTest.Hga10.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                        CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax);
                                }
                                else
                                {

                                _carrierUnderTest.Hga10.setResistanceSpec(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax + "; " +
                                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin + " - " + CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax);
                                }
                                _carrierUnderTest.Hga10.setBiasCurrent(CommonFunctions.Instance.ConfigurationSetupRecipe.getCh1BiasCurrent());
                                //update 13-feb-2020
                                if (HSTMachine.Workcell.HSTSettings._SamplingData.getCount >= 100)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtAdaptiveDelta.Text = HSTMachine.Workcell.HSTSettings._SamplingData.getCurrentZvalue.ToString();

                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtAdaptiveDelta.Text = " (Count : " + HSTMachine.Workcell.HSTSettings._SamplingData.getCount.ToString() + ")";
                                }
                                double pctBridge = 0;
                                if (HSTMachine.Workcell.HSTSettings._SamplingData.getTotalTest > 0)
                                {
                                    pctBridge   = (HSTMachine.Workcell.HSTSettings._SamplingData.getTotalWRBridge/HSTMachine.Workcell.HSTSettings._SamplingData.getTotalTest) * 100;
                                
                                }
                                else
                                {
                                    pctBridge = 0;
                                }
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtPctBridge.Text = pctBridge.ToString() + " %";
                                HSTMachine.Workcell.getPanelData().UpdateSampleData();
                            }
                            try
                            {
                                _workcell.TestedDataMaps.Remove(_carrierUnderTest.CarrierID);
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[0].HgaSN, Reading_RD1 = HGA1Ch5R1Resistance, Reading_RD2 = HGA1Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[1].HgaSN, Reading_RD1 = HGA2Ch5R1Resistance, Reading_RD2 = HGA2Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[2].HgaSN, Reading_RD1 = HGA3Ch5R1Resistance, Reading_RD2 = HGA3Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[3].HgaSN, Reading_RD1 = HGA4Ch5R1Resistance, Reading_RD2 = HGA4Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[4].HgaSN, Reading_RD1 = HGA5Ch5R1Resistance, Reading_RD2 = HGA5Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[5].HgaSN, Reading_RD1 = HGA6Ch5R1Resistance, Reading_RD2 = HGA6Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[6].HgaSN, Reading_RD1 = HGA7Ch5R1Resistance, Reading_RD2 = HGA7Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[7].HgaSN, Reading_RD1 = HGA8Ch5R1Resistance, Reading_RD2 = HGA8Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[8].HgaSN, Reading_RD1 = HGA9Ch5R1Resistance, Reading_RD2 = HGA9Ch6R2Resistance });
                                testedDataMap.Add(new TESTED_DATA_MAP { SN = _carrierUnderTest.RFIDData.RFIDTagData.HGAData[9].HgaSN, Reading_RD1 = HGA10Ch5R1Resistance, Reading_RD2 = HGA10Ch6R2Resistance });
                                _workcell.TestedDataMaps.Add(_carrierUnderTest.CarrierID, testedDataMap);
                            }
                            catch (Exception)
                            {
                            }
                           
                            // HGA1
                            if (HGA1Present == false || HGA1PassInPreviousSystem == false)
                            {
                                HGA1Ch1WriterResistance = 0;
                                HGA1Ch2TAResistance = 0;
                                HGA1Ch3WHResistance = 0;
                                HGA1Ch4RHResistance = 0;
                                HGA1Ch5R1Resistance = 0;
                                HGA1Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga1.setWriterResistance(HGA1Ch1WriterResistance);
                                _carrierUnderTest.Hga1.setTAResistance(HGA1Ch2TAResistance);
                                _carrierUnderTest.Hga1.setWHeaterResistance(HGA1Ch3WHResistance);
                                _carrierUnderTest.Hga1.setRHeaterResistance(HGA1Ch4RHResistance);
                                _carrierUnderTest.Hga1.setReader1Resistance(HGA1Ch5R1Resistance);
                                _carrierUnderTest.Hga1.setReader2Resistance(HGA1Ch6R2Resistance);

                            }

                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().textBoxCarrierId.Text = _carrierUnderTest.CarrierID;

                            Log.Info(this, "HGA1 Enable = {3}, Channel 1 Enable = {4}, Measured HGA1Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA1Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());

                            var totalpad = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 ? 11 : 9;
                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open
                                bool HGA1CH1ShortTestStatus = true;
                                if ((HGA1Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA1Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA1CH1ShortTestStatus = false;
                                }


                                if ((HGA1Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA1Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA1CH1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA1Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.Text);

                            Log.Info(this, "HGA1 Enable = {3}, Channel 2 Enable = {4}, Measured HGA1Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA1Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {

                                //Short/Open
                                bool HGA1CH2ShortTestStatus = true;
                                if ((HGA1Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA1Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA1CH2ShortTestStatus = false;
                                }

                                //Resistance
                                if ((HGA1Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA1Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA1CH2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA1Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.Text);

                            Log.Info(this, "HGA1 Enable = {3}, Channel 3 Enable = {4}, Measured HGA1Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA1Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA1CH3ShortTestStatus = true;
                                if ((HGA1Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA1Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA1CH3ShortTestStatus = false;
                                }

                                if ((HGA1Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA1Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA1CH3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA1Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.Text);

                            Log.Info(this, "HGA1 Enable = {3}, Channel 4 Enable = {4}, Measured HGA1Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA1Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA1CH4ShortTestStatus = true;
                                if ((HGA1Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA1Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA1CH4ShortTestStatus = false;
                                }

                                if ((HGA1Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA1Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA1CH4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA1Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.Text);

                            Log.Info(this, "HGA1 Enable = {3}, Channel 5 Enable = {4}, Measured HGA1Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA1Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA1CH5ShortTestStatus = true;
                                if ((HGA1Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA1Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA1CH5ShortTestStatus = false;
                                }

                                if ((HGA1Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA1Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA1CH5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA1Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.Text);

                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                            {
                                Log.Info(this, "HGA1 Enable = {3}, Channel 6 Enable = {4}, Measured HGA1Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                    HGA1Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax,
                                    (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            }
                            else
                            {
                            Log.Info(this, "HGA1 Enable = {3}, Channel 6 Enable = {4}, Measured HGA1Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA1Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            }
                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short
                                bool HGA1CH6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA1Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA1Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA1CH6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA1Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA1Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA1CH6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA1 resistance spec
                                var isHGA1Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA1Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA1Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA1Ch6R2PassSpec = true;
                                    }
                                }
                                ////With LDU
                                else
                                {
                                    isHGA1Ch6R2PassSpec = true;

                                    if ((HGA1LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA1LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA1Ch6R2PassSpec = false;
                                    }

                                    if ((HGA1Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA1Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA1Ch6R2PassSpec = false;
                                    }

                                    if ((HGA1VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA1VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA1Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga1.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA1LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA1LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA1Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA1LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA1LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA1Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA1LDU_IThreshold - _carrierUnderTest.Hga1.Last_ET_Threshold);
                                    if((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA1Ch6R2PassSpec = false;
                                    }

                                }
                                #endregion

                                if (isHGA1Ch6R2PassSpec && HGA1CH6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.ForeColor = Color.Red;
                                }

                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA1Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text);

                            if (HGA1Present && HGA1PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 1, HGA1Ch5R1Resistance, HGA1Ch6R2Resistance, _carrierUnderTest.Hga1.DeltaISIResistanceRD1, _carrierUnderTest.Hga1.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA1Ch6R2Resistance,
                                        _carrierUnderTest.Hga1.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga1.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga1.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA1Ch5R1Resistance,
                                       _carrierUnderTest.Hga1.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga1.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga1.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga1.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga1.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.ForeColor = Color.Red;
                                }

                                if (_carrierUnderTest.Hga1.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga1.set_DeltaISI_R2_SDET_Tolerance(((((HGA1Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga1.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga1.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga1.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga1.set_DeltaISI_R1_SDET_Tolerance(((((HGA1Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga1.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga1.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga1.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga1.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                            }
                            else
                            {
                                Log.Info(this, "HGA1,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.ForeColor;

                            Log.Info(this, "HGA1DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga1.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga1.setDeltaISIReader2(DeltaISI_Tolerance2);
                            // HGA2
                            if (HGA2Present == false || HGA2PassInPreviousSystem == false)
                            {
                                HGA2Ch1WriterResistance = 0;
                                HGA2Ch2TAResistance = 0;
                                HGA2Ch3WHResistance = 0;
                                HGA2Ch4RHResistance = 0;
                                HGA2Ch5R1Resistance = 0;
                                HGA2Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga2.setWriterResistance(HGA2Ch1WriterResistance);
                                _carrierUnderTest.Hga2.setTAResistance(HGA2Ch2TAResistance);
                                _carrierUnderTest.Hga2.setWHeaterResistance(HGA2Ch3WHResistance);
                                _carrierUnderTest.Hga2.setRHeaterResistance(HGA2Ch4RHResistance);
                                _carrierUnderTest.Hga2.setReader1Resistance(HGA2Ch5R1Resistance);
                                _carrierUnderTest.Hga2.setReader2Resistance(HGA2Ch6R2Resistance);
                            }


                            Log.Info(this, "HGA2 Enable = {3}, Channel 1 Enable = {4}, Measured HGA2Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA2Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA2Ch1ShortTestStatus = true;
                                if ((HGA2Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA2Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA2Ch1ShortTestStatus = false;
                                }

                                if ((HGA2Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA2Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA2Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.Text);

                            Log.Info(this, "HGA2 Enable = {3}, Channel 2 Enable = {4}, Measured HGA2Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA2Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {

                                //Short/Open CH2
                                bool HGA2Ch2ShortTestStatus = true;
                                if ((HGA2Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA2Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA2Ch2ShortTestStatus = false;
                                }

                                if ((HGA2Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA2Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA2Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.Text);

                            Log.Info(this, "HGA2 Enable = {3}, Channel 3 Enable = {4}, Measured HGA2Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA2Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA2Ch3ShortTestStatus = true;
                                if ((HGA2Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA2Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA2Ch3ShortTestStatus = false;
                                }

                                if ((HGA2Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA2Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA2Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.Text);

                            Log.Info(this, "HGA2 Enable = {3}, Channel 4 Enable = {4}, Measured HGA2Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA2Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA2Ch4ShortTestStatus = true;
                                if ((HGA2Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA2Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA2Ch4ShortTestStatus = false;
                                }

                                if ((HGA2Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA2Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA2Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.Text);

                            Log.Info(this, "HGA2 Enable = {3}, Channel 5 Enable = {4}, Measured HGA2Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA2Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA2Ch5ShortTestStatus = true;
                                if ((HGA2Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA2Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA2Ch5ShortTestStatus = false;
                                }

                                if ((HGA2Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA2Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA2Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.Text);

                            Log.Info(this, "HGA2 Enable = {3}, Channel 6 Enable = {4}, Measured HGA2Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA2Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {

                                //Open/short CH6
                                bool HGA2Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA2Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA2Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA2Ch6ShortTestStatus = false;
                                    }

                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA2Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA2Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA2Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA2 resistance spec
                                var isHGA2Ch6R2PassSpec = false;
                                // With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA2Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA2Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA2Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA2Ch6R2PassSpec = true;
                                    if ((HGA2Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA2Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA2Ch6R2PassSpec = false;
                                    }

                                    if ((HGA2VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA2VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA2Ch6R2PassSpec = false;
                                    }

                                    if ((HGA2LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA2LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA2Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga2.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA2LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA2LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA2Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA2LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA2LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA2Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA2LDU_IThreshold - _carrierUnderTest.Hga2.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA2Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA2Ch6R2PassSpec && HGA2Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text);

                            if (HGA2Present && HGA2PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 2, HGA2Ch5R1Resistance, HGA2Ch6R2Resistance, _carrierUnderTest.Hga2.DeltaISIResistanceRD1, _carrierUnderTest.Hga2.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA2Ch6R2Resistance,
                                        _carrierUnderTest.Hga2.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga2.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga2.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA2Ch5R1Resistance,
                                        _carrierUnderTest.Hga2.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga2.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga2.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga2.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga2.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.ForeColor = Color.Red;
                                }

                                if (_carrierUnderTest.Hga2.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga2.set_DeltaISI_R2_SDET_Tolerance(((((HGA2Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga2.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga2.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga2.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga2.set_DeltaISI_R1_SDET_Tolerance(((((HGA2Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga2.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga2.ISI_ET_RD1_RES) * 100);


                                if (_carrierUnderTest.Hga2.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga2.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }

                            }
                            else
                            {
                                Log.Info(this, "HGA2,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.ForeColor;

                            Log.Info(this, "HGA2DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text);
                            ////19-Feb-2020
                            _carrierUnderTest.Hga2.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga2.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA3
                            if (HGA3Present == false || HGA3PassInPreviousSystem == false)
                            {
                                HGA3Ch1WriterResistance = 0;
                                HGA3Ch2TAResistance = 0;
                                HGA3Ch3WHResistance = 0;
                                HGA3Ch4RHResistance = 0;
                                HGA3Ch5R1Resistance = 0;
                                HGA3Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga3.setWriterResistance(HGA3Ch1WriterResistance);
                                _carrierUnderTest.Hga3.setTAResistance(HGA3Ch2TAResistance);
                                _carrierUnderTest.Hga3.setWHeaterResistance(HGA3Ch3WHResistance);
                                _carrierUnderTest.Hga3.setRHeaterResistance(HGA3Ch4RHResistance);
                                _carrierUnderTest.Hga3.setReader1Resistance(HGA3Ch5R1Resistance);
                                _carrierUnderTest.Hga3.setReader2Resistance(HGA3Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA3 Enable = {3}, Channel 1 Enable = {4}, Measured HGA3Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA3Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());

                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA3Ch1ShortTestStatus = true;
                                if ((HGA3Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA3Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA3Ch1ShortTestStatus = false;
                                }

                                if ((HGA3Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA3Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA3Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA3Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.Text);

                            Log.Info(this, "HGA3 Enable = {3}, Channel 2 Enable = {4}, Measured HGA3Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA3Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {
                                //Short/Open CH2
                                bool HGA3Ch2ShortTestStatus = true;
                                if ((HGA3Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA3Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA3Ch2ShortTestStatus = false;
                                }

                                if ((HGA3Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA3Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA3Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA3Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.Text);

                            Log.Info(this, "HGA3 Enable = {3}, Channel 3 Enable = {4}, Measured HGA3Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA3Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA3Ch3ShortTestStatus = true;
                                if ((HGA3Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA3Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA3Ch3ShortTestStatus = false;
                                }

                                if ((HGA3Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA3Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA3Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA3Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.Text);

                            Log.Info(this, "HGA3 Enable = {3}, Channel 4 Enable = {4}, Measured HGA3Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA3Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA3Ch4ShortTestStatus = true;
                                if ((HGA3Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA3Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA3Ch4ShortTestStatus = false;
                                }

                                if ((HGA3Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA3Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA3Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA3Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.Text);

                            Log.Info(this, "HGA3 Enable = {3}, Channel 5 Enable = {4}, Measured HGA3Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA3Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA3Ch5ShortTestStatus = true;
                                if ((HGA3Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA3Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA3Ch5ShortTestStatus = false;
                                }

                                if ((HGA3Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA3Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA3Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA3Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.Text);

                            Log.Info(this, "HGA3 Enable = {3}, Channel 6 Enable = {4}, Measured HGA3Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA3Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA3Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA3Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA3Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA3Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA3Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA3Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA3Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA3 resistance spec
                                var isHGA3Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA3Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA3Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA3Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA3Ch6R2PassSpec = true;
                                    if ((HGA3Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA3Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA3Ch6R2PassSpec = false;
                                    }

                                    if ((HGA3VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA3VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA3Ch6R2PassSpec = false;
                                    }

                                    if ((HGA3LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA3LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA3Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga3.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA3LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA3LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA3Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA3LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA3LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA3Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA3LDU_IThreshold - _carrierUnderTest.Hga3.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA3Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA3Ch6R2PassSpec && HGA3Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA3Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text);

                            //DeltaISI
                            if (HGA3Present && HGA3PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable == true)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 3, HGA3Ch5R1Resistance, HGA3Ch6R2Resistance, _carrierUnderTest.Hga3.DeltaISIResistanceRD1, _carrierUnderTest.Hga3.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA3Ch6R2Resistance,
                                        _carrierUnderTest.Hga3.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga3.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga3.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA3Ch5R1Resistance,
                                       _carrierUnderTest.Hga3.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga3.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga3.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga3.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga3.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.ForeColor = Color.Red;
                                }
                                if (_carrierUnderTest.Hga3.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga3.set_DeltaISI_R2_SDET_Tolerance(((((HGA3Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga3.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga3.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga3.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga3.set_DeltaISI_R1_SDET_Tolerance(((((HGA3Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga3.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga3.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga3.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga3.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                            }
                            else
                            {
                                Log.Info(this, "HGA3,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.ForeColor;

                            Log.Info(this, "HGA3DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text);
                            _carrierUnderTest.Hga3.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga3.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA4
                            if (HGA4Present == false || HGA4PassInPreviousSystem == false)
                            {
                                HGA4Ch1WriterResistance = 0;
                                HGA4Ch2TAResistance = 0;
                                HGA4Ch3WHResistance = 0;
                                HGA4Ch4RHResistance = 0;
                                HGA4Ch5R1Resistance = 0;
                                HGA4Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga4.setWriterResistance(HGA4Ch1WriterResistance);
                                _carrierUnderTest.Hga4.setTAResistance(HGA4Ch2TAResistance);
                                _carrierUnderTest.Hga4.setWHeaterResistance(HGA4Ch3WHResistance);
                                _carrierUnderTest.Hga4.setRHeaterResistance(HGA4Ch4RHResistance);
                                _carrierUnderTest.Hga4.setReader1Resistance(HGA4Ch5R1Resistance);
                                _carrierUnderTest.Hga4.setReader2Resistance(HGA4Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA4 Enable = {3}, Channel 1 Enable = {4}, Measured HGA4Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA4Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA4Ch1ShortTestStatus = true;
                                if ((HGA4Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA4Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA4Ch1ShortTestStatus = false;
                                }

                                if ((HGA4Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA4Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA4Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA4Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.Text);

                            Log.Info(this, "HGA4 Enable = {3}, Channel 2 Enable = {4}, Measured HGA4Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA4Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {
                                //Short/Open CH2
                                bool HGA4Ch2ShortTestStatus = true;
                                if ((HGA4Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA4Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA4Ch2ShortTestStatus = false;
                                }

                                if ((HGA4Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA4Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA4Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA4Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.Text);

                            Log.Info(this, "HGA4 Enable = {3}, Channel 3 Enable = {4}, Measured HGA4Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA4Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA4Ch3ShortTestStatus = true;
                                if ((HGA4Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA4Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA4Ch3ShortTestStatus = false;
                                }

                                if ((HGA4Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA4Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA4Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA4Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.Text);

                            Log.Info(this, "HGA4 Enable = {3}, Channel 4 Enable = {4}, Measured HGA4Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA4Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA4Ch4ShortTestStatus = true;
                                if ((HGA4Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA4Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA4Ch4ShortTestStatus = false;
                                }

                                if ((HGA4Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA4Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA4Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA4Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.Text);

                            Log.Info(this, "HGA4 Enable = {3}, Channel 5 Enable = {4}, Measured HGA4Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA4Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA4Ch5ShortTestStatus = true;
                                if ((HGA4Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA4Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA4Ch5ShortTestStatus = false;
                                }

                                if ((HGA4Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA4Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA4Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA4Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.Text);

                            Log.Info(this, "HGA4 Enable = {3}, Channel 6 Enable = {4}, Measured HGA4Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA4Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA4Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA4Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA4Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA4Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA4Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA4Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA4Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA4 resistance spec
                                var isHGA4Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA4Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA4Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA4Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA4Ch6R2PassSpec = true;
                                    if ((HGA4Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA4Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA4Ch6R2PassSpec = false;
                                    }

                                    if ((HGA4VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA4VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA4Ch6R2PassSpec = false;
                                    }

                                    if ((HGA4LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA4LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA4Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga4.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA4LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA4LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA4Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA4LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA4LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA4Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA4LDU_IThreshold - _carrierUnderTest.Hga4.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA4Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA4Ch6R2PassSpec && HGA4Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA4Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text);

                            //Delta ISI
                            if (HGA4Present && HGA4PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable == true)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 4, HGA4Ch5R1Resistance, HGA4Ch6R2Resistance, _carrierUnderTest.Hga4.DeltaISIResistanceRD1, _carrierUnderTest.Hga4.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA4Ch6R2Resistance, _carrierUnderTest.Hga4.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga4.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga4.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA4Ch5R1Resistance,
                                       _carrierUnderTest.Hga4.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga4.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga4.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga4.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga4.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.ForeColor = Color.Red;
                                }
                                if (_carrierUnderTest.Hga4.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga4.set_DeltaISI_R2_SDET_Tolerance(((((HGA4Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga4.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga4.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga4.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga4.set_DeltaISI_R1_SDET_Tolerance(((((HGA4Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga4.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga4.ISI_ET_RD1_RES) * 100);


                                if (_carrierUnderTest.Hga4.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga4.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                            }
                            else
                            {
                                Log.Info(this, "HGA4,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.ForeColor;

                            Log.Info(this, "HGA4DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text);
                            _carrierUnderTest.Hga4.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga4.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA5
                            if (HGA5Present == false || HGA5PassInPreviousSystem == false)
                            {
                                HGA5Ch1WriterResistance = 0;
                                HGA5Ch2TAResistance = 0;
                                HGA5Ch3WHResistance = 0;
                                HGA5Ch4RHResistance = 0;
                                HGA5Ch5R1Resistance = 0;
                                HGA5Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga5.setWriterResistance(HGA5Ch1WriterResistance);
                                _carrierUnderTest.Hga5.setTAResistance(HGA5Ch2TAResistance);
                                _carrierUnderTest.Hga5.setWHeaterResistance(HGA5Ch3WHResistance);
                                _carrierUnderTest.Hga5.setRHeaterResistance(HGA5Ch4RHResistance);
                                _carrierUnderTest.Hga5.setReader1Resistance(HGA5Ch5R1Resistance);
                                _carrierUnderTest.Hga5.setReader2Resistance(HGA5Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA5 Enable = {3}, Channel 1 Enable = {4}, Measured HGA5Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA5Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA5Ch1ShortTestStatus = true;
                                if ((HGA5Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA5Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA5Ch1ShortTestStatus = false;
                                }

                                if ((HGA5Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA5Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA5Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA5Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.Text);

                            Log.Info(this, "HGA5 Enable = {3}, Channel 2 Enable = {4}, Measured HGA5Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA5Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {

                                //Short/Open CH2
                                bool HGA5Ch2ShortTestStatus = true;
                                if ((HGA5Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA5Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA5Ch2ShortTestStatus = false;
                                }

                                if ((HGA5Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA5Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA5Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA5Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.Text);

                            Log.Info(this, "HGA5 Enable = {3}, Channel 3 Enable = {4}, Measured HGA5Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA5Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA5Ch3ShortTestStatus = true;
                                if ((HGA5Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA5Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA5Ch3ShortTestStatus = false;
                                }

                                if ((HGA5Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA5Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA5Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA5Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.Text);

                            Log.Info(this, "HGA5 Enable = {3}, Channel 4 Enable = {4}, Measured HGA5Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA5Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {

                                //Open/short CH4
                                bool HGA5Ch4ShortTestStatus = true;
                                if ((HGA5Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA5Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA5Ch4ShortTestStatus = false;
                                }

                                if ((HGA5Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA5Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA5Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA5Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.Text);

                            Log.Info(this, "HGA5 Enable = {3}, Channel 5 Enable = {4}, Measured HGA5Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA5Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {

                                //Open/Short CH5
                                bool HGA5Ch5ShortTestStatus = true;
                                if ((HGA5Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA5Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA5Ch5ShortTestStatus = false;
                                }

                                if ((HGA5Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA5Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA5Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA5Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.Text);

                            Log.Info(this, "HGA5 Enable = {3}, Channel 6 Enable = {4}, Measured HGA5Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA5Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA5Ch5ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA5Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA5Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA5Ch5ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA5Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA5Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA5Ch5ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA5 resistance spec
                                var isHGA5Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA5Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA5Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA5Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA5Ch6R2PassSpec = true;

                                    if ((HGA5Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA5Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA5Ch6R2PassSpec = false;
                                    }

                                    if ((HGA5VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA5VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA5Ch6R2PassSpec = false;
                                    }

                                    if ((HGA5LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA5LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA5Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga5.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA5LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA5LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA5Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA5LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA5LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA5Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA5LDU_IThreshold - _carrierUnderTest.Hga5.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA5Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA5Ch6R2PassSpec && HGA5Ch5ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA5Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text);

                            //Delta ISI
                            if (HGA5Present && HGA5PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 5, HGA5Ch5R1Resistance, HGA5Ch6R2Resistance, _carrierUnderTest.Hga5.DeltaISIResistanceRD1, _carrierUnderTest.Hga5.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA5Ch6R2Resistance, _carrierUnderTest.Hga5.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga5.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga5.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA2Ch5R1Resistance,
                                       _carrierUnderTest.Hga5.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga5.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga5.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga5.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga5.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.ForeColor = Color.Red;
                                }
                                if (_carrierUnderTest.Hga5.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga5.set_DeltaISI_R2_SDET_Tolerance(((((HGA5Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga5.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga5.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga5.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga5.set_DeltaISI_R1_SDET_Tolerance(((((HGA5Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga5.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga5.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga5.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga5.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }

                            }
                            else
                            {
                                Log.Info(this, "HGA5,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.ForeColor;

                            Log.Info(this, "HGA5DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga5.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga5.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA6
                            if (HGA6Present == false || HGA6PassInPreviousSystem == false)
                            {
                                HGA6Ch1WriterResistance = 0;
                                HGA6Ch2TAResistance = 0;
                                HGA6Ch3WHResistance = 0;
                                HGA6Ch4RHResistance = 0;
                                HGA6Ch5R1Resistance = 0;
                                HGA6Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga6.setWriterResistance(HGA6Ch1WriterResistance);
                                _carrierUnderTest.Hga6.setTAResistance(HGA6Ch2TAResistance);
                                _carrierUnderTest.Hga6.setWHeaterResistance(HGA6Ch3WHResistance);
                                _carrierUnderTest.Hga6.setRHeaterResistance(HGA6Ch4RHResistance);
                                _carrierUnderTest.Hga6.setReader1Resistance(HGA6Ch5R1Resistance);
                                _carrierUnderTest.Hga6.setReader2Resistance(HGA6Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA6 Enable = {3}, Channel 1 Enable = {4}, Measured HGA6Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA6Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA6Ch1ShortTestStatus = true;
                                if ((HGA6Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA6Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA6Ch1ShortTestStatus = false;
                                }

                                if ((HGA6Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA6Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA6Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA6Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.Text);

                            Log.Info(this, "HGA6 Enable = {3}, Channel 2 Enable = {4}, Measured HGA6Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA6Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {
                                //Short/Open CH2
                                bool HGA6Ch2ShortTestStatus = true;
                                if ((HGA6Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA6Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA6Ch2ShortTestStatus = false;
                                }

                                if ((HGA6Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA6Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA6Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA6Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.Text);

                            Log.Info(this, "HGA6 Enable = {3}, Channel 3 Enable = {4}, Measured HGA6Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA6Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {

                                //Open/short CH3
                                bool HGA6Ch3ShortTestStatus = true;
                                if ((HGA6Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA6Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA6Ch3ShortTestStatus = false;
                                }

                                if ((HGA6Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA6Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA6Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA6Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.Text);

                            Log.Info(this, "HGA6 Enable = {3}, Channel 4 Enable = {4}, Measured HGA6Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA6Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {

                                //Open/short CH4
                                bool HGA6Ch4ShortTestStatus = true;
                                if ((HGA6Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA6Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA6Ch4ShortTestStatus = false;
                                }

                                if ((HGA6Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA6Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA6Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA6Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.Text);

                            Log.Info(this, "HGA6 Enable = {3}, Channel 5 Enable = {4}, Measured HGA6Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA6Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA6Ch5ShortTestStatus = true;
                                if ((HGA6Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA6Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA6Ch5ShortTestStatus = false;
                                }

                                if ((HGA6Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA6Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA6Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA6Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.Text);

                            Log.Info(this, "HGA6 Enable = {3}, Channel 6 Enable = {4}, Measured HGA6Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA6Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA6Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA6Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA6Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA6Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA6Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA6Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA6Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA6 resistance spec
                                var isHGA6Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA6Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA6Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA6Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA6Ch6R2PassSpec = true;

                                    if ((HGA6Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA6Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA6Ch6R2PassSpec = false;
                                    }

                                    if ((HGA6VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA6VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA6Ch6R2PassSpec = false;
                                    }

                                    if ((HGA6LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA6LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA6Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga6.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA6LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA6LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA6Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA6LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA6LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA6Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA6LDU_IThreshold - _carrierUnderTest.Hga6.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA6Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA6Ch6R2PassSpec && HGA6Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA6Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text);

                            //Delta ISI
                            if (HGA6Present && HGA6PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 6, HGA6Ch5R1Resistance, HGA6Ch6R2Resistance, _carrierUnderTest.Hga6.DeltaISIResistanceRD1, _carrierUnderTest.Hga6.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA6Ch6R2Resistance,
                                        _carrierUnderTest.Hga6.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga6.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga6.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA6Ch5R1Resistance,
                                       _carrierUnderTest.Hga6.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga6.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga6.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga6.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga6.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.ForeColor = Color.Red;
                                }
                                if (_carrierUnderTest.Hga6.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga6.set_DeltaISI_R2_SDET_Tolerance(((((HGA6Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga6.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga6.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga6.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga6.set_DeltaISI_R1_SDET_Tolerance(((((HGA6Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga6.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga6.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga6.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga6.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }

                            }
                            else
                            {
                                Log.Info(this, "HGA6,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;

                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.ForeColor;

                            Log.Info(this, "HGA6DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga6.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga6.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA7
                            if (HGA7Present == false || HGA7PassInPreviousSystem == false)
                            {
                                HGA7Ch1WriterResistance = 0;
                                HGA7Ch2TAResistance = 0;
                                HGA7Ch3WHResistance = 0;
                                HGA7Ch4RHResistance = 0;
                                HGA7Ch5R1Resistance = 0;
                                HGA7Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga7.setWriterResistance(HGA7Ch1WriterResistance);
                                _carrierUnderTest.Hga7.setTAResistance(HGA7Ch2TAResistance);
                                _carrierUnderTest.Hga7.setWHeaterResistance(HGA7Ch3WHResistance);
                                _carrierUnderTest.Hga7.setRHeaterResistance(HGA7Ch4RHResistance);
                                _carrierUnderTest.Hga7.setReader1Resistance(HGA7Ch5R1Resistance);
                                _carrierUnderTest.Hga7.setReader2Resistance(HGA7Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA7 Enable = {3}, Channel 1 Enable = {4}, Measured HGA7Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA7Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA7Ch1ShortTestStatus = true;
                                if ((HGA7Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA7Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA7Ch1ShortTestStatus = false;
                                }

                                if ((HGA7Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA7Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA7Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA7Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.Text);

                            Log.Info(this, "HGA7 Enable = {3}, Channel 2 Enable = {4}, Measured HGA7Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA7Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {
                                //Short/Open CH2
                                bool HGA7Ch2ShortTestStatus = true;
                                if ((HGA7Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA7Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA7Ch2ShortTestStatus = false;
                                }

                                if ((HGA7Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA7Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA7Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA7Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.Text);

                            Log.Info(this, "HGA7 Enable = {3}, Channel 3 Enable = {4}, Measured HGA7Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA7Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA7Ch3ShortTestStatus = true;
                                if ((HGA7Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA7Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA7Ch3ShortTestStatus = false;
                                }

                                if ((HGA7Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA7Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA7Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA7Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.Text);

                            Log.Info(this, "HGA7 Enable = {3}, Channel 4 Enable = {4}, Measured HGA7Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA7Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA7Ch4ShortTestStatus = true;
                                if ((HGA7Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA7Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA7Ch4ShortTestStatus = false;
                                }

                                if ((HGA7Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA7Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA7Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA7Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.Text);

                            Log.Info(this, "HGA7 Enable = {3}, Channel 5 Enable = {4}, Measured HGA7Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA7Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA7Ch5ShortTestStatus = true;
                                if ((HGA7Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA7Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA7Ch5ShortTestStatus = false;
                                }

                                if ((HGA7Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA7Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA7Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA7Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.Text);

                            Log.Info(this, "HGA7 Enable = {3}, Channel 6 Enable = {4}, Measured HGA7Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA7Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {

                                //Open/short CH6
                                bool HGA7Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA7Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA7Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA7Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA7Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) &&
                                        (HGA7Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA7Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA7 resistance spec
                                var isHGA7Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA7Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA7Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA7Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA7Ch6R2PassSpec = true;

                                    if ((HGA7Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA7Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA7Ch6R2PassSpec = false;
                                    }

                                    if ((HGA7VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA7VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA7Ch6R2PassSpec = false;
                                    }

                                    if ((HGA7LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA7LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA7Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga7.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA7LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA7LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA7Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA7LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA7LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA7Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA7LDU_IThreshold - _carrierUnderTest.Hga7.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA7Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA7Ch6R2PassSpec && HGA7Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA7Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text);

                            //DeltaISI
                            if (HGA7Present && HGA7PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 7, HGA7Ch5R1Resistance, HGA7Ch6R2Resistance, _carrierUnderTest.Hga7.DeltaISIResistanceRD1, _carrierUnderTest.Hga7.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA7Ch6R2Resistance,
                                        _carrierUnderTest.Hga7.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga7.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga7.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA7Ch5R1Resistance,
                                       _carrierUnderTest.Hga7.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga7.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga7.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga7.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga7.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.ForeColor = Color.Red;
                                }
                                if (_carrierUnderTest.Hga7.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga7.set_DeltaISI_R2_SDET_Tolerance(((((HGA7Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga7.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga7.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga7.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga7.set_DeltaISI_R1_SDET_Tolerance(((((HGA7Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga7.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga7.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga7.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga7.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }

                            }
                            else
                            {
                                Log.Info(this, "HGA7,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;

                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.ForeColor;

                            Log.Info(this, "HGA7DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga7.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga7.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA8
                            if (HGA8Present == false || HGA8PassInPreviousSystem == false)
                            {
                                HGA8Ch1WriterResistance = 0;
                                HGA8Ch2TAResistance = 0;
                                HGA8Ch3WHResistance = 0;
                                HGA8Ch4RHResistance = 0;
                                HGA8Ch5R1Resistance = 0;
                                HGA8Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga8.setWriterResistance(HGA8Ch1WriterResistance);
                                _carrierUnderTest.Hga8.setTAResistance(HGA8Ch2TAResistance);
                                _carrierUnderTest.Hga8.setWHeaterResistance(HGA8Ch3WHResistance);
                                _carrierUnderTest.Hga8.setRHeaterResistance(HGA8Ch4RHResistance);
                                _carrierUnderTest.Hga8.setReader1Resistance(HGA8Ch5R1Resistance);
                                _carrierUnderTest.Hga8.setReader2Resistance(HGA8Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA8 Enable = {3}, Channel 1 Enable = {4}, Measured HGA8Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA8Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA8Ch1ShortTestStatus = true;
                                if ((HGA8Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA8Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA8Ch1ShortTestStatus = false;
                                }

                                if ((HGA8Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA8Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA8Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA8Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.Text);

                            Log.Info(this, "HGA8 Enable = {3}, Channel 2 Enable = {4}, Measured HGA8Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA8Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {

                                //Short/Open CH2
                                bool HGA8Ch2ShortTestStatus = true;
                                if ((HGA8Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA8Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA8Ch2ShortTestStatus = false;
                                }

                                if ((HGA8Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA8Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA8Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA8Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.Text);

                            Log.Info(this, "HGA8 Enable = {3}, Channel 3 Enable = {4}, Measured HGA8Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA8Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA8Ch3ShortTestStatus = true;
                                if ((HGA8Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA8Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA8Ch3ShortTestStatus = false;
                                }

                                if ((HGA8Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA8Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA8Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA8Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.Text);

                            Log.Info(this, "HGA8 Enable = {3}, Channel 4 Enable = {4}, Measured HGA8Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA8Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA8Ch4ShortTestStatus = true;
                                if ((HGA8Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA8Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA8Ch4ShortTestStatus = false;
                                }

                                if ((HGA8Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA8Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA8Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA8Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.Text);

                            Log.Info(this, "HGA8 Enable = {3}, Channel 5 Enable = {4}, Measured HGA8Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA8Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA8Ch5ShortTestStatus = true;
                                if ((HGA8Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA8Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA8Ch5ShortTestStatus = false;
                                }

                                if ((HGA8Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA8Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA8Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA8Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.Text);

                            Log.Info(this, "HGA8 Enable = {3}, Channel 6 Enable = {4}, Measured HGA8Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA8Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA8Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA8Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA8Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA8Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA8Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA8Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA8Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA8 resistance spec
                                var isHGA8Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA8Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA8Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA8Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA8Ch6R2PassSpec = true;
                                    if ((HGA8Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA8Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA8Ch6R2PassSpec = false;
                                    }

                                    if ((HGA8VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA8VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA8Ch6R2PassSpec = false;
                                    }

                                    if ((HGA8LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA8LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA8Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga8.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA8LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA8LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA8Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA8LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA8LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA8Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA8LDU_IThreshold - _carrierUnderTest.Hga8.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA8Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA8Ch6R2PassSpec && HGA8Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA8Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text);

                            //Delta ISI
                            if (HGA8Present && HGA8PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 8, HGA8Ch5R1Resistance, HGA8Ch6R2Resistance, _carrierUnderTest.Hga8.DeltaISIResistanceRD1, _carrierUnderTest.Hga8.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA8Ch6R2Resistance,
                                        _carrierUnderTest.Hga8.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga8.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga8.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA8Ch5R1Resistance,
                                       _carrierUnderTest.Hga8.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga8.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga8.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga8.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga8.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.ForeColor = Color.Red;
                                }
                                if (_carrierUnderTest.Hga8.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga8.set_DeltaISI_R2_SDET_Tolerance(((((HGA8Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga8.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga8.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga8.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga8.set_DeltaISI_R1_SDET_Tolerance(((((HGA8Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga8.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga8.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga8.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga8.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }

                            }
                            else
                            {
                                Log.Info(this, "HGA8,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.ForeColor;

                            Log.Info(this, "HGA8DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga8.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga8.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA9
                            if (HGA9Present == false || HGA9PassInPreviousSystem == false)
                            {
                                HGA9Ch1WriterResistance = 0;
                                HGA9Ch2TAResistance = 0;
                                HGA9Ch3WHResistance = 0;
                                HGA9Ch4RHResistance = 0;
                                HGA9Ch5R1Resistance = 0;
                                HGA9Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga9.setWriterResistance(HGA9Ch1WriterResistance);
                                _carrierUnderTest.Hga9.setTAResistance(HGA9Ch2TAResistance);
                                _carrierUnderTest.Hga9.setWHeaterResistance(HGA9Ch3WHResistance);
                                _carrierUnderTest.Hga9.setRHeaterResistance(HGA9Ch4RHResistance);
                                _carrierUnderTest.Hga9.setReader1Resistance(HGA9Ch5R1Resistance);
                                _carrierUnderTest.Hga9.setReader2Resistance(HGA9Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA9 Enable = {3}, Channel 1 Enable = {4}, Measured HGA9Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA9Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA9Ch1ShortTestStatus = true;
                                if ((HGA9Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA9Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA9Ch1ShortTestStatus = false;
                                }

                                if ((HGA9Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA9Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA9Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA9Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.Text);

                            Log.Info(this, "HGA9 Enable = {3}, Channel 2 Enable = {4}, Measured HGA9Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA9Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {
                                //Short/Open CH2
                                bool HGA9Ch2ShortTestStatus = true;
                                if ((HGA9Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA9Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA9Ch2ShortTestStatus = false;
                                }

                                if ((HGA9Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA9Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA9Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA9Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.Text);

                            Log.Info(this, "HGA9 Enable = {3}, Channel 3 Enable = {4}, Measured HGA9Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA9Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA9Ch2ShortTestStatus = true;
                                if ((HGA9Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA9Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA9Ch2ShortTestStatus = false;
                                }

                                if ((HGA9Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA9Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA9Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA9Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.Text);

                            Log.Info(this, "HGA9 Enable = {3}, Channel 4 Enable = {4}, Measured HGA9Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA9Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH4
                                bool HGA9Ch4ShortTestStatus = true;
                                if ((HGA9Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA9Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA9Ch4ShortTestStatus = false;
                                }

                                if ((HGA9Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA9Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA9Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA9Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.Text);

                            Log.Info(this, "HGA9 Enable = {3}, Channel 5 Enable = {4}, Measured HGA9Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA9Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                //Open/Short CH5
                                bool HGA9Ch5ShortTestStatus = true;
                                if ((HGA9Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA9Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA9Ch5ShortTestStatus = false;
                                }

                                if ((HGA9Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA9Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA9Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA9Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.Text);

                            Log.Info(this, "HGA9 Enable = {3}, Channel 6 Enable = {4}, Measured HGA9Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA9Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA9Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA9Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA9Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA9Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA9Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA9Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA9Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA9 resistance spec
                                var isHGA9Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA9Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA9Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA9Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA9Ch6R2PassSpec = true;

                                    if ((HGA9Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA9Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA9Ch6R2PassSpec = false;
                                    }

                                    if ((HGA9VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA9VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA9Ch6R2PassSpec = false;
                                    }

                                    if ((HGA9LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA9LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA9Ch6R2PassSpec = false;
                                    }
                                    if (_carrierUnderTest.Hga9.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA9LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA9LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA9Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA9LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA9LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA9Ch6R2PassSpec = false;
                                        }
                                    }
                                    var deltaI_Threshold = (HGA9LDU_IThreshold - _carrierUnderTest.Hga9.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA9Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA9Ch6R2PassSpec && HGA9Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA9Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text);

                            //Delta ISI
                            if (HGA9Present && HGA9PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 9, HGA9Ch5R1Resistance, HGA9Ch6R2Resistance, _carrierUnderTest.Hga9.DeltaISIResistanceRD1, _carrierUnderTest.Hga9.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA9Ch6R2Resistance,
                                        _carrierUnderTest.Hga9.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga9.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga9.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA9Ch5R1Resistance,
                                       _carrierUnderTest.Hga9.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga9.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga9.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga9.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga9.Error_Msg_Code_Set_Flag = true;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.ForeColor = Color.Red;
                                }

                                var T9 = (((((HGA9Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga9.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga9.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga9.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga9.set_DeltaISI_R2_SDET_Tolerance(((((HGA9Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga9.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga9.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga9.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga9.set_DeltaISI_R1_SDET_Tolerance(((((HGA9Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga9.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga9.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga9.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga9.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                            }
                            else
                            {
                                Log.Info(this, "HGA9,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }

                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.ForeColor;

                            Log.Info(this, "HGA9DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga9.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga9.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // HGA10
                            if (HGA10Present == false || HGA10PassInPreviousSystem == false)
                            {
                                HGA10Ch1WriterResistance = 0;
                                HGA10Ch2TAResistance = 0;
                                HGA10Ch3WHResistance = 0;
                                HGA10Ch4RHResistance = 0;
                                HGA10Ch5R1Resistance = 0;
                                HGA10Ch6R2Resistance = 0;
                                _carrierUnderTest.Hga10.setWriterResistance(HGA10Ch1WriterResistance);
                                _carrierUnderTest.Hga10.setTAResistance(HGA10Ch2TAResistance);
                                _carrierUnderTest.Hga10.setWHeaterResistance(HGA10Ch3WHResistance);
                                _carrierUnderTest.Hga10.setRHeaterResistance(HGA10Ch4RHResistance);
                                _carrierUnderTest.Hga10.setReader1Resistance(HGA10Ch5R1Resistance);
                                _carrierUnderTest.Hga10.setReader2Resistance(HGA10Ch6R2Resistance);
                            }

                            Log.Info(this, "HGA10 Enable = {3}, Channel 1 Enable = {4}, Measured HGA10Ch1WriterResistance = {0}. Range specified in recipe: Ch1WriterResistanceMin = {1}, Ch1WriterResistanceMax = {2}.",
                                HGA10Ch1WriterResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1).ToString());
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                            {
                                //Short/Open CH1
                                bool HGA10Ch1ShortTestStatus = true;
                                if ((HGA10Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin) ||
                                    (HGA10Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax))
                                {
                                    HGA10Ch1ShortTestStatus = false;
                                }

                                if ((HGA10Ch1WriterResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin &&
                                    HGA10Ch1WriterResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) && (HGA10Ch1ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA10Ch1WriterResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.Text);

                            Log.Info(this, "HGA10 Enable = {3}, Channel 2 Enable = {4}, Measured HGA10Ch2TAResistance = {0}. Range specified in recipe: Ch2TAResistanceMin = {1}, Ch2TAResistanceMax = {2}.",
                                HGA10Ch2TAResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1).ToString());
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                            {
                                //Short/Open CH2
                                bool HGA10Ch2ShortTestStatus = true;
                                if ((HGA10Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax) ||
                                     (HGA10Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin))
                                {
                                    HGA10Ch2ShortTestStatus = false;
                                }

                                if ((HGA10Ch2TAResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin &&
                                    HGA10Ch2TAResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) && (HGA10Ch2ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA10Ch2TAResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.Text);

                            Log.Info(this, "HGA10 Enable = {3}, Channel 3 Enable = {4}, Measured HGA10Ch3WHResistance = {0}. Range specified in recipe: Ch3WHResistanceMin = {1}, Ch3WHResistanceMax = {2}.",
                                HGA10Ch3WHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1).ToString());
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                            {

                                //Open/short CH3
                                bool HGA10Ch3ShortTestStatus = true;
                                if ((HGA10Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax) ||
                                    (HGA10Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin))
                                {
                                    HGA10Ch3ShortTestStatus = false;
                                }

                                if ((HGA10Ch3WHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin &&
                                    HGA10Ch3WHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) && (HGA10Ch3ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA10Ch3WHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.Text);

                            Log.Info(this, "HGA10 Enable = {3}, Channel 4 Enable = {4}, Measured HGA10Ch4RHResistance = {0}. Range specified in recipe: Ch4RHResistanceMin = {1}, Ch4RHResistanceMax = {2}.",
                                HGA10Ch4RHResistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1).ToString());
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                            {
                                //Open/short CH3
                                bool HGA10Ch4ShortTestStatus = true;
                                if ((HGA10Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax) ||
                                    (HGA10Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin))
                                {
                                    HGA10Ch4ShortTestStatus = false;
                                }

                                if ((HGA10Ch4RHResistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin &&
                                    HGA10Ch4RHResistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) && (HGA10Ch4ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA10Ch4RHResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.Text);

                            Log.Info(this, "HGA10 Enable = {3}, Channel 5 Enable = {4}, Measured HGA10Ch5R1Resistance = {0}. Range specified in recipe: Ch5R1ResistanceMin = {1}, Ch5R1ResistanceMax = {2}.",
                                HGA10Ch5R1Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1).ToString());
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {

                                //Open/Short CH5
                                bool HGA10Ch5ShortTestStatus = true;
                                if ((HGA10Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax) ||
                                    (HGA10Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin))
                                {
                                    HGA10Ch5ShortTestStatus = false;
                                }

                                if ((HGA10Ch5R1Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin &&
                                    HGA10Ch5R1Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) && (HGA10Ch5ShortTestStatus))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA10Ch5R1Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.Text);

                            Log.Info(this, "HGA10 Enable = {3}, Channel 6 Enable = {4}, Measured HGA10Ch6R2Resistance = {0}. Range specified in recipe: Ch6R2ResistanceMin = {1}, Ch6R2ResistanceMax = {2}.",
                                HGA10Ch6R2Resistance, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin, CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax,
                                (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1).ToString(), (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1).ToString());
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                //Open/short CH6
                                bool HGA10Ch6ShortTestStatus = true;
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    //CH6 RD2
                                    if ((HGA10Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax) ||
                                        (HGA10Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin))
                                    {
                                        HGA10Ch6ShortTestStatus = false;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    //CH6 RD2
                                    if ((HGA10Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax) ||
                                        (HGA10Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin))
                                    {
                                        HGA10Ch6ShortTestStatus = false;
                                    }
                                }

                                #region Check HGA10 resistance spec
                                var isHGA10Ch6R2PassSpec = false;
                                //With Normal
                                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    if (HGA10Ch6R2Resistance >= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin && HGA10Ch6R2Resistance <= CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax)
                                    {
                                        isHGA10Ch6R2PassSpec = true;
                                    }
                                }
                                //With LDU
                                else
                                {
                                    isHGA10Ch6R2PassSpec = true;
                                    if ((HGA10Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax) ||
                                        (HGA10Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin))
                                    {
                                        isHGA10Ch6R2PassSpec = false;
                                    }

                                    if ((HGA10VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                                        (HGA10VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                                    {
                                        isHGA10Ch6R2PassSpec = false;
                                    }

                                    if ((HGA10LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                                        (HGA10LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                                    {
                                        isHGA10Ch6R2PassSpec = false;
                                    }

                                    if (_carrierUnderTest.Hga10.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Ymxc)
                                    {
                                        if ((HGA10LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                                            (HGA10LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                                        {
                                            isHGA10Ch6R2PassSpec = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((HGA10LDU_IThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper) ||
                                            (HGA10LDU_IThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower))
                                        {
                                            isHGA10Ch6R2PassSpec = false;
                                        }
                                    }

                                    var deltaI_Threshold = (HGA10LDU_IThreshold - _carrierUnderTest.Hga10.Last_ET_Threshold);
                                    if ((deltaI_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                                        (deltaI_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                                    {
                                        isHGA10Ch6R2PassSpec = false;
                                    }
                                }
                                #endregion

                                if (isHGA10Ch6R2PassSpec && HGA10Ch6ShortTestStatus)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.ForeColor = Color.Black;
                            }

                            Log.Info(this, "HGA10Ch6R2Resistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text);

                            //Delta ISI
                            if (HGA10Present && HGA10PassInPreviousSystem && CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1 && CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                            {
                                if (DeltaISIResistanceComparision(_carrierUnderTest.CarrierID, 10, HGA10Ch5R1Resistance, HGA10Ch6R2Resistance, _carrierUnderTest.Hga10.DeltaISIResistanceRD1, _carrierUnderTest.Hga10.DeltaISIResistanceRD2))
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.ForeColor = Color.Green;

                                    if (!DeltaR2ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA10Ch6R2Resistance,
                                        _carrierUnderTest.Hga10.ISI_ET_RD2_RES))
                                    {
                                        _carrierUnderTest.Hga10.ForceToPolarityRiskSamplingDeltaR2 = true;
                                        _carrierUnderTest.Hga10.Error_Msg_Code_Set_Flag = true;
                                    }
                                    if (!DeltaR1ISIWriterSDETComparision(_carrierUnderTest.CarrierID, 1, HGA10Ch5R1Resistance,
                                       _carrierUnderTest.Hga10.ISI_ET_RD1_RES))
                                    {
                                        _carrierUnderTest.Hga10.ForceToPolarityRiskSamplingDeltaR1 = true;
                                        _carrierUnderTest.Hga10.Error_Msg_Code_Set_Flag = true;
                                    }
                                }
                                else
                                {
                                    _carrierUnderTest.Hga10.Error_Msg_Code = ERROR_MESSAGE_CODE.CRDL.ToString();
                                    _carrierUnderTest.Hga10.Error_Msg_Code_Set_Flag = true;

                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.ForeColor = Color.Red;
                                }

                                if (_carrierUnderTest.Hga10.ISI_ET_RD2_RES != 0)
                                    _carrierUnderTest.Hga10.set_DeltaISI_R2_SDET_Tolerance(((((HGA10Ch6R2Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET) - _carrierUnderTest.Hga10.ISI_ET_RD2_RES)) / _carrierUnderTest.Hga10.ISI_ET_RD2_RES) * 100);
                                if (_carrierUnderTest.Hga10.ISI_ET_RD1_RES != 0)
                                    _carrierUnderTest.Hga10.set_DeltaISI_R1_SDET_Tolerance(((((HGA10Ch5R1Resistance - CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET) - _carrierUnderTest.Hga10.ISI_ET_RD1_RES)) / _carrierUnderTest.Hga10.ISI_ET_RD1_RES) * 100);

                                if (_carrierUnderTest.Hga10.DeltaISIResistanceRD1 != 0 || _carrierUnderTest.Hga10.DeltaISIResistanceRD2 != 0)
                                {
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI_Diff.Text = DeltaISI_Tolerance1.ToString("F2");
                                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1 && !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISIR2_Diff.Text = DeltaISI_Tolerance2.ToString("F2");
                                    else
                                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                }

                            }
                            else
                            {
                                Log.Info(this, "HGA10,ISI NA Tracking => HGAPresent='{0}', HGAPassInPreviousSystem='{1}',ConfigurationSetupRecipeNo='{2}'", HGA1Present, HGA1PassInPreviousSystem, CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10);
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.ForeColor = Color.Black;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI_Diff.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISIR2_Diff.Text = CommonFunctions.NOT_AVAILABLE;

                                DeltaISI_Tolerance1 = 0;
                                DeltaISI_Tolerance2 = 0;
                            }
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.ForeColor;
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISIR2_Diff.ForeColor =
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.ForeColor;

                            Log.Info(this, "HGA10DeltaISIResistance Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text);
                            //19-Feb-2020
                            _carrierUnderTest.Hga10.setDeltaISIReader1(DeltaISI_Tolerance1);
                            _carrierUnderTest.Hga10.setDeltaISIReader2(DeltaISI_Tolerance2);

                            // Overall resistance test result
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA1ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA2ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA3ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA4ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA5ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA6ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA7ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA8ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA9ResistanceTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch1WriterResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch2TAResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch3WHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch4RHResistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch5R1Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10Ch6R2Resistance.Text, CommonFunctions.FAIL, true) == 0 ||
                                String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA10ResistanceTestPASS = false;
                            }

                            OverallMeasurementTestResults(_carrierUnderTest);

                            if (HGA1Present && HGA1PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga1.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga1.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(1, totalpad, _carrierUnderTest.Hga1.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga1.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga1.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga1.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }

                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga1.Error_Msg_Code))
                                    {
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text = CommonFunctions.PASS;
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA1 Short Test Result to PASS");
                                    }

                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);

                                }
                            }


                            if (HGA2Present && HGA2PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter(); 
                                if (_carrierUnderTest.Hga2.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga2.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(2, totalpad, _carrierUnderTest.Hga2.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga2.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga2.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga2.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }

                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga2.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text = CommonFunctions.PASS;
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA2 Short Test Result to PASS");
                                    }

                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }


                            if (HGA3Present && HGA3PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter(); 
                                if (_carrierUnderTest.Hga3.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga3.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(3, totalpad, _carrierUnderTest.Hga3.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga3.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga3.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga3.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga3.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text = CommonFunctions.PASS;
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA3 Short Test Result to PASS");
                                    }

                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }


                            if (HGA4Present && HGA4PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga4.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga4.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(4, totalpad, _carrierUnderTest.Hga4.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga4.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga4.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga4.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga4.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text = CommonFunctions.PASS;
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA4 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }

                            if (HGA5Present && HGA5PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga5.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga5.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(5, totalpad, _carrierUnderTest.Hga5.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga5.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga5.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga5.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga5.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text = CommonFunctions.PASS;
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA5 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }

                            if (HGA6Present && HGA6PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter(); 
                                if (_carrierUnderTest.Hga6.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga6.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(6, totalpad, _carrierUnderTest.Hga6.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga6.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga6.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga6.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga6.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text = CommonFunctions.PASS;
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA6 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }


                            if (HGA7Present && HGA7PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga7.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga7.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(7, totalpad, _carrierUnderTest.Hga7.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga7.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga7.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga7.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga7.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text = CommonFunctions.PASS;
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA7 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }

                            if (HGA8Present && HGA8PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga8.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga8.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(8, totalpad, _carrierUnderTest.Hga8.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga8.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga8.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga8.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga8.Error_Msg_Code))
                                    {
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text = CommonFunctions.PASS;
                                     //   HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA8 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }

                            if (HGA9Present && HGA9PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga9.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga9.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(9, totalpad, _carrierUnderTest.Hga9.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga9.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga9.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga9.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga9.Error_Msg_Code))
                                    {
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text = CommonFunctions.PASS;
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA9 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }


                            if (HGA10Present && HGA10PassInPreviousSystem)
                            {
                                IncreaseCRDLRunPartCounter();
                                if (_carrierUnderTest.Hga10.OverallMeasurementTestPass)
                                {
                                    _carrierUnderTest.Hga10.Error_Msg_Code = string.Empty;
                                }
                                else
                                {
                                    var errlist = GetErrorCode(10, totalpad, _carrierUnderTest.Hga10.get_IthresholdCalculationMethod());
                                    if (_carrierUnderTest.Hga10.Error_Msg_Code == string.Empty)
                                    {
                                        if (errlist.Count > 0)
                                        {
                                            _carrierUnderTest.Hga10.Error_Msg_Code = GetErrorCodePriority(errlist);
                                            _carrierUnderTest.Hga10.Error_Msg_Code_Set_Flag = true;
                                        }
                                    }
                                    if (_workcell.Process.OutputStationProcess.Controller.CheckAssignedToSampling(_carrierUnderTest.Hga10.Error_Msg_Code))
                                    {
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text = CommonFunctions.PASS;
                                    //    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.ForeColor = Color.Green;
                                        Log.Info(this, "Changing HGA10 Short Test Result to PASS");
                                    }
                                    if (checkSTFamilyAndCRDL(errlist) != "") _carrierUnderTest.Hga1.Error_Msg_Code = checkSTFamilyAndCRDL(errlist);
                                }
                            }

                            bool isAssignedToSamplingCode = (_carrierUnderTest.Hga1.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga1.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga2.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga2.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga3.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga3.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga4.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga4.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga5.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga5.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga6.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga6.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga7.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga7.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga8.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga8.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga9.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga9.Error_Msg_Code.Contains("BRIDGE") ||
                                _carrierUnderTest.Hga10.Error_Msg_Code.Contains("ST") || _carrierUnderTest.Hga10.Error_Msg_Code.Contains("BRIDGE"));

                            if (isAssignedToSamplingCode)
                                OverallMeasurementTestResults(_carrierUnderTest);

                            //Check CRDL error failure percentage
                            CheckErrorCodeTriggering();
                        }

                        #endregion

                        #region HST_get_cap_results
                        // HST_get_cap_results
                        if (CommandID == (byte)MessageID.HST_get_cap_results)
                        {
                        }
                        #endregion

                        #region HST_get_short_detection
                        // HST_get_short_detection
                        if (CommandID == (byte)MessageID.HST_get_short_detection)
                        {
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

                            Log.Info(this, "Carrier ID : {0}, Measurement results returned by Command Name: HST_get_short_detection", _carrierUnderTest.CarrierID);

                            if (HGA1Present == true && HGA1PassInPreviousSystem)
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
                                    string ShortPadPositions = DetermineHGA1ShortPadPositions();
                                    _carrierUnderTest.Hga1.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA1 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA1Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga1.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga1.setShortPadPosition("0");
                                HGA1Short = ShortDetection.NoTest;
                            }

                            if (HGA2Present == true && HGA2PassInPreviousSystem)
                            {
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

                                    {
                                        string ShortPadPositions = DetermineHGA2ShortPadPositions();
                                        _carrierUnderTest.Hga2.setShortPadPosition(ShortPadPositions);
                                        Log.Info(this, "HGA2 Short Pad Positions = '{0}'.", ShortPadPositions);
                                    }
                                }
                                else
                                {
                                    HGA2Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga2.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga2.setShortPadPosition("0");
                                HGA2Short = ShortDetection.NoTest;
                            }

                            if (HGA3Present == true && HGA3PassInPreviousSystem)
                            {
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
                                    string ShortPadPositions = DetermineHGA3ShortPadPositions();
                                    _carrierUnderTest.Hga3.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA3 Short Pad Positions = '{0}'.", ShortPadPositions);

                                }
                                else
                                {
                                    HGA3Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga3.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga3.setShortPadPosition("0");
                                HGA3Short = ShortDetection.NoTest;
                            }

                            if (HGA4Present == true && HGA4PassInPreviousSystem)
                            {
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
                                    string ShortPadPositions = DetermineHGA4ShortPadPositions();
                                    _carrierUnderTest.Hga4.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA4 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA4Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga4.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga4.setShortPadPosition("0");
                                HGA4Short = ShortDetection.NoTest;
                            }

                            if (HGA5Present == true && HGA5PassInPreviousSystem)
                            {
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

                                    string ShortPadPositions = DetermineHGA5ShortPadPositions();
                                    _carrierUnderTest.Hga5.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA5 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA5Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga5.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga5.setShortPadPosition("0");
                                HGA5Short = ShortDetection.NoTest;
                            }

                            if (HGA6Present == true && HGA6PassInPreviousSystem)
                            {
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

                                    string ShortPadPositions = DetermineHGA6ShortPadPositions();
                                    _carrierUnderTest.Hga6.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA6 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA6Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga6.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga6.setShortPadPosition("0");
                                HGA6Short = ShortDetection.NoTest;
                            }

                            if (HGA7Present == true && HGA7PassInPreviousSystem)
                            {
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

                                    string ShortPadPositions = DetermineHGA7ShortPadPositions();
                                    _carrierUnderTest.Hga7.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA7 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA7Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga7.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga7.setShortPadPosition("0");
                                HGA7Short = ShortDetection.NoTest;
                            }

                            if (HGA8Present == true && HGA8PassInPreviousSystem)
                            {
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

                                    string ShortPadPositions = DetermineHGA8ShortPadPositions();
                                    _carrierUnderTest.Hga8.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA8 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA8Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga8.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga8.setShortPadPosition("0");
                                HGA8Short = ShortDetection.NoTest;
                            }

                            if (HGA9Present == true && HGA9PassInPreviousSystem)
                            {
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

                                    string ShortPadPositions = DetermineHGA9ShortPadPositions();
                                    _carrierUnderTest.Hga9.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA9 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA9Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga9.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga9.setShortPadPosition("0");
                                HGA9Short = ShortDetection.NoTest;
                            }

                            if (HGA10Present == true && HGA10PassInPreviousSystem)
                            {
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

                                    string ShortPadPositions = DetermineHGA10ShortPadPositions();
                                    _carrierUnderTest.Hga10.setShortPadPosition(ShortPadPositions);
                                    Log.Info(this, "HGA10 Short Pad Positions = '{0}'.", ShortPadPositions);
                                }
                                else
                                {
                                    HGA10Short = ShortDetection.Open;
                                    _carrierUnderTest.Hga10.setShortPadPosition("0");
                                }
                            }
                            else
                            {
                                _carrierUnderTest.Hga10.setShortPadPosition("0");
                                HGA10Short = ShortDetection.NoTest;
                            }

                            {
                                Log.Info(this, "Carrier ID : {0}, Measurement results returned by Command Name: HST_get_short_detection", _carrierUnderTest.CarrierID);
                                _carrierUnderTest.Hga1.setShortTest(HGA1Short);
                                _carrierUnderTest.Hga2.setShortTest(HGA2Short);
                                _carrierUnderTest.Hga3.setShortTest(HGA3Short);
                                _carrierUnderTest.Hga4.setShortTest(HGA4Short);
                                _carrierUnderTest.Hga5.setShortTest(HGA5Short);
                                _carrierUnderTest.Hga6.setShortTest(HGA6Short);
                                _carrierUnderTest.Hga7.setShortTest(HGA7Short);
                                _carrierUnderTest.Hga8.setShortTest(HGA8Short);
                                _carrierUnderTest.Hga9.setShortTest(HGA9Short);
                                _carrierUnderTest.Hga10.setShortTest(HGA10Short);
                            }


                            // HGA1
                            if (HGA1Present)
                            {
                                if (HGA1Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.ForeColor = Color.Red;

                                }
                                else if (HGA1Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA1 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text);

                            // HGA2
                            if (HGA2Present)
                            {
                                if (HGA2Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA2Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA2 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text);

                            // HGA3
                            if (HGA3Present)
                            {
                                if (HGA3Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA3Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA3 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text);

                            // HGA4
                            if (HGA4Present)
                            {
                                if (HGA4Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA4Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA4 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text);

                            // HGA5
                            if (HGA5Present)
                            {
                                if (HGA5Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA5Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA5 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text);

                            // HGA6
                            if (HGA6Present)
                            {
                                if (HGA6Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA6Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA6 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text);

                            // HGA7
                            if (HGA7Present)
                            {
                                if (HGA7Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA7Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA7 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text);

                            // HGA8
                            if (HGA8Present)
                            {
                                if (HGA8Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA8Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA8 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text);

                            // HGA9
                            if (HGA9Present)
                            {
                                if (HGA9Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA9Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA9 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text);

                            // HGA10
                            if (HGA10Present)
                            {
                                if (HGA10Short == ShortDetection.Short)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text = CommonFunctions.FAIL;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.ForeColor = Color.Red;
                                }
                                else if (HGA10Short == ShortDetection.Open)
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text = CommonFunctions.PASS;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.ForeColor = Color.Green;
                                }
                                else
                                {
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text = CommonFunctions.NOT_AVAILABLE;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.ForeColor = Color.Black;
                            }
                            Log.Info(this, "HGA10 Short Result = '{0}'.", HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text);

                            // Overall short test result
                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA1ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA2ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA3ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA4ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA5ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA6ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA7ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA8ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA9ShortTestPASS = false;
                            }

                            if (String.Compare(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10ShortTest.Text, CommonFunctions.FAIL, true) == 0)
                            {
                                HGA10ShortTestPASS = false;
                            }

                        }

                        #endregion

                        #region HST_get_conversion_board_id
                        // HST_get_conversion_board_id
                        if (CommandID == (byte)MessageID.HST_get_conversion_board_id)
                        {
                            CommonFunctions.Instance.ConversionBoardID = TestProbe7GetConversionBoardID.ConversionBoardID;
                            CommonFunctions.Instance.LoadConfigurationRecipe();
                        }
                        #endregion

                        break;
                    case GUIPage.MeasurementTestConfigurationSetupPage:
                        if (CommandID == (byte)MessageID.HST_get_res_meas_configuration)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {

                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH1Current.Text = TestProbe55GetResMeasConfiguration.Ch1BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH2Current.Text = TestProbe55GetResMeasConfiguration.Ch2BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH3Current.Text = TestProbe55GetResMeasConfiguration.Ch3BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH4Current.Text = TestProbe55GetResMeasConfiguration.Ch4BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH5Current.Text = TestProbe55GetResMeasConfiguration.Ch5BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH6Current.Text = TestProbe55GetResMeasConfiguration.Ch6BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupAvgCurrentSampleCount.Text = TestProbe55GetResMeasConfiguration.AverageSampling().ToString("D", CultureInfo.InstalledUICulture);
                            });
                        }
                        else if (CommandID == (byte)MessageID.HST_get_res_meas2_configuration)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH1Current.Text = TestProbe58GetResMeas2Configuration.Ch1BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH2Current.Text = TestProbe58GetResMeas2Configuration.Ch2BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH3Current.Text = TestProbe58GetResMeas2Configuration.Ch3BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH4Current.Text = TestProbe58GetResMeas2Configuration.Ch4BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH5Current.Text = TestProbe58GetResMeas2Configuration.Ch5BiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH6Current.Text = TestProbe58GetResMeas2Configuration.Ch6IaBiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupCH6IbCurrent.Text = TestProbe58GetResMeas2Configuration.Ch6IbBiasCurrent().ToString("D", CultureInfo.InstalledUICulture);
                                HSTMachine.Workcell.getPanelSetup().txtConfigurationSetupAvgCurrentSampleCount.Text = TestProbe58GetResMeas2Configuration.AverageSampling().ToString("D", CultureInfo.InstalledUICulture);
                            });
                        }
                        break;
                    case GUIPage.FunctionalTest:
                        #region Start_self_test
                        if (CommandID == (byte)MessageID.HST_start_self_test)
                        {
                            var _tests0RestResultPass = false;
                            var _tests10RestResultPass = false;
                            var _tests100RestResultPass = false;
                            var _tests500RestResultPass = false;
                            var _tests1000RestResultPass = false;
                            var _tests10000RestResultPass = false;

                            // 0Ohm
                            double Ch1WriterResistance0Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance0Ohm() / 1000.0);
                            double Ch2TAResistance0Ohm = (TestProbe36StartSelfTest.Ch2TAResistance0Ohm() / 1000.0);
                            double Ch3WHResistance0Ohm = (TestProbe36StartSelfTest.Ch3WHResistance0Ohm() / 1000.0);
                            double Ch4RHResistance0Ohm = (TestProbe36StartSelfTest.Ch4RHResistance0Ohm() / 1000.0);
                            double Ch5R1Resistance0Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance0Ohm() / 1000.0);
                            double Ch6R2Resistance0Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance0Ohm() / 1000.0);
                            //Updata display 0Ohm result
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0CH1.Text = Ch1WriterResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0CH2.Text = Ch2TAResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0CH3.Text = Ch3WHResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0CH4.Text = Ch4RHResistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0CH5.Text = Ch5R1Resistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0CH6.Text = Ch6R2Resistance0Ohm.ToString("F3", CultureInfo.InvariantCulture);


                            // 10Ohm
                            double Ch1WriterResistance10Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance10Ohm() / 1000.0);
                            double Ch2TAResistance10Ohm = (TestProbe36StartSelfTest.Ch2TAResistance10Ohm() / 1000.0);
                            double Ch3WHResistance10Ohm = (TestProbe36StartSelfTest.Ch3WHResistance10Ohm() / 1000.0);
                            double Ch4RHResistance10Ohm = (TestProbe36StartSelfTest.Ch4RHResistance10Ohm() / 1000.0);
                            double Ch5R1Resistance10Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance10Ohm() / 1000.0);
                            double Ch6R2Resistance10Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance10Ohm() / 1000.0);
                            //Updata display 10Ohm result
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10CH1.Text = Ch1WriterResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10CH2.Text = Ch2TAResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10CH3.Text = Ch3WHResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10CH4.Text = Ch4RHResistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10CH5.Text = Ch5R1Resistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10CH6.Text = Ch6R2Resistance10Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 100Ohm
                            double Ch1WriterResistance100Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance100Ohm() / 1000.0);
                            double Ch2TAResistance100Ohm = (TestProbe36StartSelfTest.Ch2TAResistance100Ohm() / 1000.0);
                            double Ch3WHResistance100Ohm = (TestProbe36StartSelfTest.Ch3WHResistance100Ohm() / 1000.0);
                            double Ch4RHResistance100Ohm = (TestProbe36StartSelfTest.Ch4RHResistance100Ohm() / 1000.0);
                            double Ch5R1Resistance100Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance100Ohm() / 1000.0);
                            double Ch6R2Resistance100Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance100Ohm() / 1000.0);
                            //Updata display 100Ohm result
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100CH1.Text = Ch1WriterResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100CH2.Text = Ch2TAResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100CH3.Text = Ch3WHResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100CH4.Text = Ch4RHResistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100CH5.Text = Ch5R1Resistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100CH6.Text = Ch6R2Resistance100Ohm.ToString("F3", CultureInfo.InvariantCulture);


                            // 500Ohm
                            double Ch1WriterResistance500Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance500Ohm() / 1000.0);
                            double Ch2TAResistance500Ohm = (TestProbe36StartSelfTest.Ch2TAResistance500Ohm() / 1000.0);
                            double Ch3WHResistance500Ohm = (TestProbe36StartSelfTest.Ch3WHResistance500Ohm() / 1000.0);
                            double Ch4RHResistance500Ohm = (TestProbe36StartSelfTest.Ch4RHResistance500Ohm() / 1000.0);
                            double Ch5R1Resistance500Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance500Ohm() / 1000.0);
                            double Ch6R2Resistance500Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance500Ohm() / 1000.0);
                            //Updata display 500Ohm result
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500CH1.Text = Ch1WriterResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500CH2.Text = Ch2TAResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500CH3.Text = Ch3WHResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500CH4.Text = Ch4RHResistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500CH5.Text = Ch5R1Resistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500CH6.Text = Ch6R2Resistance500Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 1000Ohm
                            double Ch1WriterResistance1000Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance1000Ohm() / 1000.0);
                            double Ch2TAResistance1000Ohm = (TestProbe36StartSelfTest.Ch2TAResistance1000Ohm() / 1000.0);
                            double Ch3WHResistance1000Ohm = (TestProbe36StartSelfTest.Ch3WHResistance1000Ohm() / 1000.0);
                            double Ch4RHResistance1000Ohm = (TestProbe36StartSelfTest.Ch4RHResistance1000Ohm() / 1000.0);
                            double Ch5R1Resistance1000Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance1000Ohm() / 1000.0);
                            double Ch6R2Resistance1000Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance1000Ohm() / 1000.0);
                            //Updata display 1000Ohm result
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000CH1.Text = Ch1WriterResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000CH2.Text = Ch2TAResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000CH3.Text = Ch3WHResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000CH4.Text = Ch4RHResistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000CH5.Text = Ch5R1Resistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000CH6.Text = Ch6R2Resistance1000Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 10000Ohm
                            double Ch1WriterResistance10000Ohm = (TestProbe36StartSelfTest.Ch1WriterResistance10000Ohm() / 1000.0);
                            double Ch2TAResistance10000Ohm = (TestProbe36StartSelfTest.Ch2TAResistance10000Ohm() / 1000.0);
                            double Ch3WHResistance10000Ohm = (TestProbe36StartSelfTest.Ch3WHResistance10000Ohm() / 1000.0);
                            double Ch4RHResistance10000Ohm = (TestProbe36StartSelfTest.Ch4RHResistance10000Ohm() / 1000.0);
                            double Ch5R1Resistance10000Ohm = (TestProbe36StartSelfTest.Ch5R1Resistance10000Ohm() / 1000.0);
                            double Ch6R2Resistance10000Ohm = (TestProbe36StartSelfTest.Ch6R2Resistance10000Ohm() / 1000.0);
                            //Updata display 10000Ohm result
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000CH1.Text = Ch1WriterResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000CH2.Text = Ch2TAResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000CH3.Text = Ch3WHResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000CH4.Text = Ch4RHResistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000CH5.Text = Ch5R1Resistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);
                            HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000CH6.Text = Ch6R2Resistance10000Ohm.ToString("F3", CultureInfo.InvariantCulture);

                            // 0Ohm
                            double Ch1WriterResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch2TAResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch3WHResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch4RHResistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch5R1Resistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm / 1000 * 0.5 / 100) + 0.25;
                            double Ch6R2Resistance0OhmDeviation = (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm / 1000 * 0.5 / 100) + 0.25;

                            if (((CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm / 1000) - Ch1WriterResistance0OhmDeviation <= Ch1WriterResistance0Ohm) && (Ch1WriterResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm / 1000) + Ch1WriterResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm / 1000) - Ch2TAResistance0OhmDeviation <= Ch2TAResistance0Ohm) && (Ch2TAResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm / 1000) + Ch2TAResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm / 1000) - Ch3WHResistance0OhmDeviation <= Ch3WHResistance0Ohm) && (Ch3WHResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm / 1000) + Ch3WHResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm / 1000) - Ch4RHResistance0OhmDeviation <= Ch4RHResistance0Ohm) && (Ch4RHResistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm / 1000) + Ch4RHResistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm / 1000) - Ch5R1Resistance0OhmDeviation <= Ch5R1Resistance0Ohm) && (Ch5R1Resistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm / 1000) + Ch5R1Resistance0OhmDeviation) &&
                                ((CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm / 1000) - Ch6R2Resistance0OhmDeviation <= Ch6R2Resistance0Ohm) && (Ch6R2Resistance0Ohm <= (CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm / 1000) + Ch6R2Resistance0OhmDeviation))
                            {
                                _tests0RestResultPass = true;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0RestResults.Text = "PASS";
                            }
                            else
                            {
                                _tests0RestResultPass = false;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests0RestResults.Text = "FAIL";
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
                                _tests10RestResultPass = true;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10RestResults.Text = "PASS";
                            }
                            else
                            {
                                _tests10RestResultPass = false;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10RestResults.Text = "FAIL";
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
                                _tests100RestResultPass = true;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100RestResults.Text = "PASS";
                            }
                            else
                            {
                                _tests100RestResultPass = false;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests100RestResults.Text = "FAIL";
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
                                _tests500RestResultPass = true;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500RestResults.Text = "PASS";
                            }
                            else
                            {
                                _tests500RestResultPass = false;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests500RestResults.Text = "FAIL";
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
                                _tests1000RestResultPass = true;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000RestResults.Text = "PASS";
                            }
                            else
                            {
                                _tests1000RestResultPass = false;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests1000RestResults.Text = "FAIL";
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
                                _tests10000RestResultPass = true;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000RestResults.Text = "PASS";
                            }
                            else
                            {
                                _tests10000RestResultPass = false;
                                HSTMachine.Workcell.getPanelData().FunctionalTestPanel.txtFunctionalTests10000RestResults.Text = "FAIL";
                            }

                        }
                        #endregion

                        if (CommandID == (byte)MessageID.HST_get_firmware_version)
                        {
                            HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion = TestProbe37GetFirmwareVersion;
                            var MjRev = TestProbe37GetFirmwareVersion.MajorRevision;
                            var MiRev = TestProbe37GetFirmwareVersion.MinorRevision;
                            var SubMiRev = TestProbe37GetFirmwareVersion.SubMinorRevision;
                            HSTMachine.Instance.MainForm.getPanelTitle().FirmwareVersion = string.Format("{0}.{1}.{2}", MjRev.ToString(), MiRev.ToString(), SubMiRev.ToString());
                            HSTMachine.Workcell.IsFirmwareGetDone = true;
                            if (TestFirmwareVersionEvent != null)
                            {
                                TestFirmwareVersionEvent(this, new TestFirmwareEventArgs(String.Format("Major:{0}, Minor:{1}.{2}", MjRev.ToString(), MiRev.ToString(), SubMiRev.ToString())));
                            }
                        }
                        break;
                }
            }
        }

        private List<string> GetErrorCode(int hgaSlot, int totalPads, I_ThresholdCalculationMethod i_ThresholdCalculationMethod)
        {
            double Ch1WriterResistance = 0;
            double Ch2TAResistance = 0;
            double Ch3WHResistance = 0;
            double Ch4RHResistance = 0;
            double Ch5R1Resistance = 0;
            double Ch6R2Resistance = 0;
            byte WPlusPad = new byte();
            byte WMinusPad = new byte();
            byte TAPlusPad = new byte();
            byte TAMinusPad = new byte();
            byte WHPlusPad = new byte();
            byte WHMinusPad = new byte();
            byte RHPlusPad = new byte();
            byte RHMinusPad = new byte();
            byte R1PlusPad = new byte();
            byte R1MinusPad = new byte();
            byte R2PlusPad = new byte();
            byte R2MinusPad = new byte();
            Hga currentHga = null;

            double AvgResistanceSpec = (CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin + CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) / 2;
            string currentErrCode = string.Empty;
            bool errorSetFlag = false;
            bool iSISIDataFail = false;
            List<string> errorCodeList = new List<string>();

            double ch1WrOffset = 1.0;
            double ch2TaOffset = 1.0;
            double ch3WhOffset = 1.0;
            double ch4RhOffset = 1.0;
            double ch5Rd1Offset = 1.0;
            double ch6Rd2Offset = 1.0;

            double LEDInterceptVoltage = 0.0;            
            double LDUIThreshold = 0.0;
            double DeltaITheshold = 0.0;
            double VPDMax = 0.0;
            double delta_Threshold = 0.0;

            if (CommonFunctions.Instance.IsRunningWithNewTSR)
            {
                ch1WrOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceOffset;
                ch2TaOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceOffset;
                ch3WhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceOffset;
                ch4RhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceOffset;
                ch5Rd1Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceOffset;
                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceOffset;
                else
                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceOffset;
            }
            else
            {
                ch1WrOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceOffset / 1000;
                ch2TaOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceOffset / 1000;
                ch3WhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceOffset / 1000;
                ch4RhOffset = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceOffset / 1000;
                ch5Rd1Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceOffset / 1000;
                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceOffset / 1000;
                else
                    ch6Rd2Offset = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceOffset / 1000;
            }

            #region GetMeasurement
            switch (hgaSlot)
            {
                #region HGA1
                case 1:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA1Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {                     

                        Ch6R2Resistance = (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[0] * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[0];                          
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[0];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[0];
                        DeltaITheshold = _carrierUnderTest.Hga1.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga1.Last_ET_Threshold);

                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[0];
                            }                           
                        }
                        
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA1WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA1WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA1TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA1TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA1WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA1WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA1RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA1RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA1R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA1R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA1R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA1R2MinusPad;
                    currentHga = _carrierUnderTest.Hga1;

                    ////If HGA1 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA1OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA1DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }

                    break;
                #endregion
                #region HGA2
                case 2:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA2Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                        
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[1])* 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[1];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[1];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[1];
                        DeltaITheshold = _carrierUnderTest.Hga2.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga2.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[1];
                            }                        
                        }
                      
                    }
                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA2WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA2WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA2TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA2TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA2WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA2WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA2RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA2RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA2R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA2R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA2R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA2R2MinusPad;
                    currentHga = _carrierUnderTest.Hga2;

                    ////If HGA2 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA2OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA2DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }

                    }
                    break;
                #endregion
                #region HGA3
                case 3:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA3Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                        
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[2]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[2];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[2];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[2];
                        DeltaITheshold = _carrierUnderTest.Hga3.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga3.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[2];
                            }
                     
                        }
                       
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA3WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA3WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA3TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA3TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA3WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA3WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA3RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA3RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA3R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA3R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA3R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA3R2MinusPad;
                    currentHga = _carrierUnderTest.Hga3;

                    ////If HGA3 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA3OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA3DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }

                    }
                    break;
                #endregion
                #region HGA4
                case 4:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA4Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                        
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[3]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[3];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[3];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[3];
                        DeltaITheshold = _carrierUnderTest.Hga4.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga4.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[3];
                            }                          
                        }
                      
                    }
                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA4WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA4WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA4TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA4TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA4WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA4WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA4RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA4RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA4R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA4R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA4R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA4R2MinusPad;
                    currentHga = _carrierUnderTest.Hga4;

                    ////If HGA4 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA4OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA4DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }
                    break;
                #endregion
                #region  HGA5
                case 5:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA5Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                       
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[4]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[4];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[4];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[4];
                        DeltaITheshold = _carrierUnderTest.Hga5.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga5.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[4];
                            }                          
                        }
                      
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA5WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA5WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA5TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA5TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA5WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA5WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA5RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA5RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA5R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA5R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA5R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA5R2MinusPad;
                    currentHga = _carrierUnderTest.Hga5;
                    ////If HGA5 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA5OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA5DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }

                    }
                    break;
                #endregion
                #region HGA6
                case 6:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA6Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                       
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[5]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[5];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[5];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[5];
                        DeltaITheshold = _carrierUnderTest.Hga6.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga6.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[5];
                            }
                           
                        }
                        
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA6WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA6WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA6TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA6TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA6WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA6WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA6RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA6RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA6R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA6R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA6R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA6R2MinusPad;
                    currentHga = _carrierUnderTest.Hga6;

                    ////If HGA6 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA6OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA6DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }
                    break;
                #endregion
                #region HGA7
                case 7:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA7Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                       
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[6]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[6];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[6];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[6];
                        DeltaITheshold = _carrierUnderTest.Hga7.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga7.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[6];
                            }
                           
                        }
                        
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA7WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA7WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA7TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA7TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA7WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA7WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA7RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA7RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA7R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA7R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA7R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA7R2MinusPad;
                    currentHga = _carrierUnderTest.Hga7;

                    ////If HGA7 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA7OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA7DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }

                    break;
                #endregion
                #region HGA8
                case 8:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA8Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                        
                       
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[7]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[7];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[7];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[7];
                        DeltaITheshold = _carrierUnderTest.Hga8.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga8.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[7];
                            }
                           
                        }
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA8WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA8WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA8TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA8TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA8WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA8WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA8RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA8RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA8R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA8R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA8R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA8R2MinusPad;
                    currentHga = _carrierUnderTest.Hga8;

                    ////If HGA8 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA8OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA8DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }
                    break;
                #endregion
                #region HGA9
                case 9:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA9Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[8]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[8];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[8];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[8];
                        DeltaITheshold = _carrierUnderTest.Hga8.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga9.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[8];
                            }                       
                        }
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA9WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA9WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA9TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA9TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA9WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA9WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA9RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA9RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA9R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA9R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA9R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA9R2MinusPad;
                    currentHga = _carrierUnderTest.Hga9;

                    ////If HGA9 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA9OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA9DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }
                    break;
                #endregion
                #region HGA10
                case 10:
                    Ch1WriterResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch1WriterResistance() / 1000.0) - (ch1WrOffset);
                    Ch2TAResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch2TAResistance() / 1000.0) - (ch2TaOffset);
                    Ch3WHResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch3WHResistance() / 1000.0) - (ch3WhOffset);
                    Ch4RHResistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch4RHResistance() / 1000.0) - (ch4RhOffset);
                    Ch5R1Resistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch5R1Resistance() / 1000.0) - (ch5Rd1Offset);
                    if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                    Ch6R2Resistance = (TestProbe11GetAllHGAResistanceResults.HGA10Ch6R2Resistance() / 1000.0) - (ch6Rd2Offset);
                    else
                    {
                        Ch6R2Resistance = ((HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[9]) * 1000) - (ch6Rd2Offset);
                        LEDInterceptVoltage = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[9];
                        LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[9];
                        VPDMax = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[9];
                        DeltaITheshold = _carrierUnderTest.Hga10.Delta_IThreshold;
                        delta_Threshold = (LDUIThreshold - _carrierUnderTest.Hga10.Last_ET_Threshold);
                        if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                            {
                                LDUIThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[9];
                            }
                        }
                    }

                    WPlusPad = TestProbe10GetAllHGAShortDetection.HGA10WHPlusPad;
                    WMinusPad = TestProbe10GetAllHGAShortDetection.HGA10WMinusPad;
                    TAPlusPad = TestProbe10GetAllHGAShortDetection.HGA10TAPlusPad;
                    TAMinusPad = TestProbe10GetAllHGAShortDetection.HGA10TAMinusPad;
                    WHPlusPad = TestProbe10GetAllHGAShortDetection.HGA10WHPlusPad;
                    WHMinusPad = TestProbe10GetAllHGAShortDetection.HGA10WHMinusPad;
                    RHPlusPad = TestProbe10GetAllHGAShortDetection.HGA10RHPlusPad;
                    RHMinusPad = TestProbe10GetAllHGAShortDetection.HGA10RHMinusPad;
                    R1PlusPad = TestProbe10GetAllHGAShortDetection.HGA10R1PlusPad;
                    R1MinusPad = TestProbe10GetAllHGAShortDetection.HGA10R1MinusPad;
                    R2PlusPad = TestProbe10GetAllHGAShortDetection.HGA10R2PlusPad;
                    R2MinusPad = TestProbe10GetAllHGAShortDetection.HGA10R2MinusPad;
                    currentHga = _carrierUnderTest.Hga10;

                    ////If HGA10 overall failed
                    if (HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtHGA10OverallResult.Text == CommonFunctions.FAIL.ToString())
                    {
                        //Fail delta ISI
                        if (!errorSetFlag && HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().txtbox_HGA10DeltaISI.Text == CommonFunctions.FAIL.ToString())
                        {
                            iSISIDataFail = true;
                        }
                    }
                    break;
                #endregion
            }
            #endregion

            #region Detect Resistance

            #region Priority1 (Open and short by spec)
            //CH5 RD1
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                if (Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax)
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.RCO.ToString());
                    currentErrCode = ERROR_MESSAGE_CODE.RCO.ToString();
                    errorSetFlag = true;
                }
            if (Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.RCS.ToString());

                if (!errorSetFlag)
                    currentErrCode = ERROR_MESSAGE_CODE.RCS.ToString();

                errorSetFlag = true;
            }

            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                {
                    //CH6 RD2
                    if (Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax)
                    {
                        errorCodeList.Add(ERROR_MESSAGE_CODE.CO2.ToString());
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.CO2.ToString();
                        errorSetFlag = true;
                    }

                    if (Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin)
                    {
                        errorCodeList.Add(ERROR_MESSAGE_CODE.CS2.ToString());
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.CS2.ToString();
                        errorSetFlag = true;
                    }
                }
                //With LDU
                else
                {
                    //CH6 RD2
                    if (Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax)
                    {
                        errorCodeList.Add(ERROR_MESSAGE_CODE.LDURESFAIL.ToString());
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.LDURESFAIL.ToString();
                        errorSetFlag = true;
                    }

                    if (Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin)
                    {
                        errorCodeList.Add(ERROR_MESSAGE_CODE.LDURESFAIL.ToString());
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.LDURESFAIL.ToString();
                        errorSetFlag = true;
                    }
  

                }

            //CH1 W
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                if (Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax)
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.WRO.ToString());
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.WRO.ToString();
                    errorSetFlag = true;
                }
            if (Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.WRS.ToString());
                if (!errorSetFlag)
                    currentErrCode = ERROR_MESSAGE_CODE.WRS.ToString();
                errorSetFlag = true;
            }

            //CH3 WH
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                if (Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax)
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.HRO.ToString());
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.HRO.ToString();
                    errorSetFlag = true;
                }
            if (Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.HRS.ToString());
                if (!errorSetFlag)
                    currentErrCode = ERROR_MESSAGE_CODE.HRS.ToString();
                errorSetFlag = true;
            }
            //CH4 RH
            if (Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.H2O.ToString());
                if (!errorSetFlag)
                    currentErrCode = ERROR_MESSAGE_CODE.H2O.ToString();
                errorSetFlag = true;
            }
            if (Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.H2S.ToString());
                if (!errorSetFlag)
                    currentErrCode = ERROR_MESSAGE_CODE.H2S.ToString();
                errorSetFlag = true;
            }

            //CH2
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                if (Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax)
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.TAO.ToString());
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.TAO.ToString();
                    errorSetFlag = true;
                }
            if (Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.TAS.ToString());
                if (!errorSetFlag)
                    currentErrCode = ERROR_MESSAGE_CODE.TAS.ToString();
                errorSetFlag = true;
            }

            #endregion

            #region Priority2 (Resistance spec)

            if (!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
            {
                //CH5 RD1, CH6 RD2
                if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                    if ((Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) ||
                        (Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin) ||
                        (Ch6R2Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax) ||
                        (Ch6R2Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin))
                    {
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.CRDR.ToString();
                        errorCodeList.Add(ERROR_MESSAGE_CODE.CRDR.ToString());
                        errorSetFlag = true;
                    }
            }
            else
            {
                //CH5 RD1
                if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                    if ((Ch5R1Resistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax) ||
                        (Ch5R1Resistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin))
                    {
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.CRDR.ToString();
                        errorCodeList.Add(ERROR_MESSAGE_CODE.CRDR.ToString());
                        errorSetFlag = true;
                    }

                if ((VPDMax > CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec) ||
                    (VPDMax < CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec))
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.VPDFAIL.ToString());
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.VPDFAIL.ToString();
                    errorSetFlag = true;
                }

                if ((LEDInterceptVoltage < CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower) ||
                    (LEDInterceptVoltage > CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper))
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.LEDINTCFAIL.ToString());
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.LEDINTCFAIL.ToString();
                    errorSetFlag = true;
                }

                /*       if ((LDUIThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper) ||
                           (LDUIThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower))
                       {
                           errorCodeList.Add(ERROR_MESSAGE_CODE.ITHRESFAIL.ToString());
                           if (!errorSetFlag)
                               currentErrCode = ERROR_MESSAGE_CODE.ITHRESFAIL.ToString();
                           errorSetFlag = true;
                       }
       */

                if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Gompertz)
                {
                    if (LDUIThreshold > CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper ||
                       LDUIThreshold < CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower)
                    {
                        errorCodeList.Add(ERROR_MESSAGE_CODE.ITHRESFAIL.ToString());
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.ITHRESFAIL.ToString();
                        errorSetFlag = true;
                    }
                }                
                if (i_ThresholdCalculationMethod == I_ThresholdCalculationMethod.Ymxc)
                {
                    if (LDUIThreshold > CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper ||
                       LDUIThreshold < CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower)
                    {
                        errorCodeList.Add(ERROR_MESSAGE_CODE.ITHRESFAIL.ToString());
                        if (!errorSetFlag)
                            currentErrCode = ERROR_MESSAGE_CODE.ITHRESFAIL.ToString();
                        errorSetFlag = true;
                    }
                }
               
                if ((delta_Threshold > CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec) ||
                    (delta_Threshold < CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec))
                {
                    errorCodeList.Add(ERROR_MESSAGE_CODE.DELTAITHRESFAIL.ToString());
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.DELTAITHRESFAIL.ToString();
                    errorSetFlag = true;
                }
            }



            //CH1
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1)
                if ((Ch1WriterResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax) ||
                        (Ch1WriterResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin))
                {
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.CWRR.ToString();
                    errorCodeList.Add(ERROR_MESSAGE_CODE.CWRR.ToString());
                    errorSetFlag = true;
                }

            //CH3
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1)
                if ((Ch3WHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax) ||
                        (Ch3WHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin))
                {
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.CHTR.ToString();
                    errorCodeList.Add(ERROR_MESSAGE_CODE.CHTR.ToString());
                    errorSetFlag = true;
                }

            //CH4
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1)
                if ((Ch4RHResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax) ||
                        (Ch4RHResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin))
                {
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.CHT2.ToString();
                    errorCodeList.Add(ERROR_MESSAGE_CODE.CHT2.ToString());
                    errorSetFlag = true;
                }

            //CH2
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1)
                if ((Ch2TAResistance > CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax) ||
                        (Ch2TAResistance < CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin))
                {
                    if (!errorSetFlag)
                        currentErrCode = ERROR_MESSAGE_CODE.CTAR.ToString();
                    errorCodeList.Add(ERROR_MESSAGE_CODE.CTAR.ToString());
                    errorSetFlag = true;
                }

            //LDU



            #endregion

            #region Priority3 (ISI Data)
            //Fail delta ISI
            if (iSISIDataFail)
            {
                if (HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodeEnabled &&
                    HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePercent > 0)
                {
                    if (!errorSetFlag)
                        IncreaseCRDLFailureCounter();
                }

                if (!errorSetFlag)
                currentErrCode = ERROR_MESSAGE_CODE.CRDL.ToString();
                errorCodeList.Add(ERROR_MESSAGE_CODE.CRDL.ToString());
                errorSetFlag = true;
            }
            #endregion

            #region Priority4 (Short)
            bool isCH1WriterShort = false;
            bool isCH2TAShort = false;
            bool isCH3WHeaterShort = false;
            bool isCH4RHeaterShort = false;
            bool isCH5Reader1Short = false;
            bool isCH6Reader2Short = false;
            int shortDetectCounter = 0;

            if ((WPlusPad == (int)ShortDetection.Short) || (WMinusPad == (int)ShortDetection.Short))
            {
                isCH1WriterShort = true;
            }
            if ((TAPlusPad == (int)ShortDetection.Short) || (TAMinusPad == (int)ShortDetection.Short))
            {
                isCH2TAShort = true;
            }

            if ((WHPlusPad == (int)ShortDetection.Short) || (WHMinusPad == (int)ShortDetection.Short))
            {
                isCH3WHeaterShort = true;
            }
            if ((RHPlusPad == (int)ShortDetection.Short) || (RHMinusPad == (int)ShortDetection.Short))
            {
                isCH4RHeaterShort = true;
            }

            if ((R1PlusPad == (int)ShortDetection.Short) || (R1MinusPad == (int)ShortDetection.Short))
            {
                isCH5Reader1Short = true;
            }
            if ((R2PlusPad == (int)ShortDetection.Short) || (R2MinusPad == (int)ShortDetection.Short))
            {
                isCH6Reader2Short = true;
            }

            //CH5 && CH6 Short
            if (isCH5Reader1Short && isCH6Reader2Short)
            {
                shortDetectCounter++;
                errorCodeList.Add(ERROR_MESSAGE_CODE.R2R1ST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.R2R1ST.ToString();
                    errorSetFlag = true;
                }
            }

            //CH5 && CH3 Short
            if (isCH5Reader1Short && isCH3WHeaterShort)
            {
                shortDetectCounter++;
                errorCodeList.Add(ERROR_MESSAGE_CODE.R1HRST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.R1HRST.ToString();
                    errorSetFlag = true;
                }
            }

            //CH3 && CH4 Short
            if (isCH3WHeaterShort && isCH4RHeaterShort)
            {
                shortDetectCounter++;
                errorCodeList.Add(ERROR_MESSAGE_CODE.HRH2ST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.HRH2ST.ToString();
                    errorSetFlag = true;
                }

            }

            //CH1 && CH3 && CH4 Short
            if ((isCH1WriterShort && isCH3WHeaterShort && isCH4RHeaterShort) && (!errorSetFlag))
            {
                shortDetectCounter++;
                errorCodeList.Add(ERROR_MESSAGE_CODE.CISL.ToString());
                currentErrCode = ERROR_MESSAGE_CODE.CISL.ToString();
                errorSetFlag = true;
            }

            //CH2 && CH6 Short
            if (isCH2TAShort && isCH6Reader2Short)
            {
                shortDetectCounter++;
                errorCodeList.Add(ERROR_MESSAGE_CODE.TAR2ST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.TAR2ST.ToString();
                    errorSetFlag = true;
                }
            }

            //Short detection more than 3 pad
            if (shortDetectCounter > 2)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.ADJST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.ADJST.ToString();
                    errorSetFlag = true;
                }
            }

            if (isCH5Reader1Short)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.R1ST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.R1ST.ToString();
                    errorSetFlag = true;
                }
            }
            if (isCH6Reader2Short)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.R2ST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.R2ST.ToString();
                    errorSetFlag = true;
                }
            }
            if (isCH1WriterShort)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.WST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.WST.ToString();
                    errorSetFlag = true;
                }
            }
            if (isCH3WHeaterShort)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.WHST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.WHST.ToString();
                    errorSetFlag = true;
                }
            }
            if (isCH4RHeaterShort)
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.RHST.ToString());

                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.RHST.ToString();
                    errorSetFlag = true;
                }
            }
            if ((isCH2TAShort) && (!errorSetFlag))
            {
                errorCodeList.Add(ERROR_MESSAGE_CODE.TAST.ToString());
                if (!errorSetFlag)
                {
                    currentErrCode = ERROR_MESSAGE_CODE.TAST.ToString();
                    errorSetFlag = true;
                }
            }
            #endregion

            #endregion

            #region Assign error message
            switch (hgaSlot)
            {
                case 1:
                    _carrierUnderTest.Hga1.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga1.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga1.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 2:
                    _carrierUnderTest.Hga2.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga2.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga2.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 3:
                    _carrierUnderTest.Hga3.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga3.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga3.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 4:
                    _carrierUnderTest.Hga4.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga4.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga4.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 5:
                    _carrierUnderTest.Hga5.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga5.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga5.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 6:
                    _carrierUnderTest.Hga6.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga6.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga6.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 7:
                    _carrierUnderTest.Hga7.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga7.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga7.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 8:
                    _carrierUnderTest.Hga8.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga8.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga8.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 9:
                    _carrierUnderTest.Hga9.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga9.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga9.IsPassLDUSpec = !errorSetFlag;
                    break;
                case 10:
                    _carrierUnderTest.Hga10.Error_Msg_Code = GetErrorCodePriority(errorCodeList);
                    _carrierUnderTest.Hga10.Error_Msg_Code_Set_Flag = errorSetFlag;
                    if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                        _carrierUnderTest.Hga10.IsPassLDUSpec = !errorSetFlag;
                    break;
            }
            #endregion

            #region Monitor error log
            var strErrCode = string.Empty;
            foreach (var item in errorCodeList)
            {
                var data = item + ",";
                strErrCode += data;
            }

            if (errorCodeList.Count > 0)
                Log.Info(this, "ErrorCodeLog===>SN={0},Error code={1},Error code list={2}", _carrierUnderTest.RFIDData.RFIDTagData.HGAData[hgaSlot - 1].HgaSN, GetErrorCodePriority(errorCodeList), strErrCode);

            #endregion

            return errorCodeList;
        }

        public string GetErrorCodePriority(List<string> errList)
        {
            string returnCode = string.Empty;
            int lastPriority = 99;
            string assignErrCode = string.Empty;
            int currentPriority = 0;
            foreach (var item in errList)
            {
                foreach (var errcode in Enum.GetValues(typeof(ERROR_MESSAGE_CODE)))
                {
                    if (item == errcode.ToString())
                    {
                        assignErrCode = errcode.ToString();
                        currentPriority = (int)errcode;
                    }
                }

                if (currentPriority < lastPriority)
                {
                    returnCode = assignErrCode;
                    lastPriority = currentPriority;
                }
            }

            return returnCode;
        }

        public void IncreaseCRDLFailureCounter()
        {
            HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodeFailureCounter++;
        }

        public void IncreaseCRDLRunPartCounter()
        {
            HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePartRunCounter++;
        }

        //CRDL failure cumulative checking 
        public void CheckErrorCodeTriggering()
        {
            if (HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePartRunCounter >= 10)
            {
                double totalPercentage = (((double)HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodeFailureCounter /
                        (double)HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePartPerPeriod) * 100);

                if (double.IsNaN(totalPercentage) || double.IsInfinity(totalPercentage)) totalPercentage = 0;
                if (totalPercentage > HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePercent)
                {
                    CommonFunctions.Instance.CRDLErrorMessage = String.Format("Total number of consecutive CRDL failure exceeed user defined limit, current percentage =({0}%)", totalPercentage.ToString("F3"));
                    HSTSettings.Instance.TriggeringSetting.ErrorCodeTriggeringActivate = true;
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));
                }
                else
                {
                    if (HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePartRunCounter >=
                        HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePartPerPeriod)
                    {
                        HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodeFailureCounter = 0;
                        HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodePartRunCounter = 0;
                    }
                    HSTSettings.Instance.TriggeringSetting.ErrorCodeTriggeringActivate = false;

                }
            }
        }

        public void LogGetAllBiasVoltage()
        {
            //hga1
            Log.Info(this, "HGA1 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga1WriterBias().ToString());
            Log.Info(this, "HGA1 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga1TABias().ToString());
            Log.Info(this, "HGA1 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga1WHBias().ToString());
            Log.Info(this, "HGA1 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga1RHBias().ToString());
            Log.Info(this, "HGA1 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga1R1Bias().ToString());
            Log.Info(this, "HGA1 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga1R2Bias().ToString());
            _carrierUnderTest.Hga1.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga1WriterBias());
            _carrierUnderTest.Hga1.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga1TABias());
            _carrierUnderTest.Hga1.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga1WHBias());
            _carrierUnderTest.Hga1.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga1RHBias());
            _carrierUnderTest.Hga1.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga1R1Bias());
            _carrierUnderTest.Hga1.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga1R2Bias());

            //hga2
            Log.Info(this, "HGA2 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga2WriterBias().ToString());
            Log.Info(this, "HGA2 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga2TABias().ToString());
            Log.Info(this, "HGA2 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga2WHBias().ToString());
            Log.Info(this, "HGA2 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga2RHBias().ToString());
            Log.Info(this, "HGA2 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga2R1Bias().ToString());
            Log.Info(this, "HGA2 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga2R2Bias().ToString());
            _carrierUnderTest.Hga2.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga2WriterBias());
            _carrierUnderTest.Hga2.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga2TABias());
            _carrierUnderTest.Hga2.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga2WHBias());
            _carrierUnderTest.Hga2.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga2RHBias());
            _carrierUnderTest.Hga2.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga2R1Bias());
            _carrierUnderTest.Hga2.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga2R2Bias());

            //hga3
            Log.Info(this, "HGA3 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga3WriterBias().ToString());
            Log.Info(this, "HGA3 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga3TABias().ToString());
            Log.Info(this, "HGA3 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga3WHBias().ToString());
            Log.Info(this, "HGA3 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga3RHBias().ToString());
            Log.Info(this, "HGA3 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga3R1Bias().ToString());
            Log.Info(this, "HGA3 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga3R2Bias().ToString());
            _carrierUnderTest.Hga3.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga3WriterBias());
            _carrierUnderTest.Hga3.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga3TABias());
            _carrierUnderTest.Hga3.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga3WHBias());
            _carrierUnderTest.Hga3.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga3RHBias());
            _carrierUnderTest.Hga3.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga3R1Bias());
            _carrierUnderTest.Hga3.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga3R2Bias());

            //hga4
            Log.Info(this, "HGA4 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga4WriterBias().ToString());
            Log.Info(this, "HGA4 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga4TABias().ToString());
            Log.Info(this, "HGA4 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga4WHBias().ToString());
            Log.Info(this, "HGA4 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga4RHBias().ToString());
            Log.Info(this, "HGA4 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga4R1Bias().ToString());
            Log.Info(this, "HGA4 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga4R2Bias().ToString());
            _carrierUnderTest.Hga4.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga4WriterBias());
            _carrierUnderTest.Hga4.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga4TABias());
            _carrierUnderTest.Hga4.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga4WHBias());
            _carrierUnderTest.Hga4.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga4RHBias());
            _carrierUnderTest.Hga4.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga4R1Bias());
            _carrierUnderTest.Hga4.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga4R2Bias());

            //hga5
            Log.Info(this, "HGA5 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga5WriterBias().ToString());
            Log.Info(this, "HGA5 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga5TABias().ToString());
            Log.Info(this, "HGA5 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga5WHBias().ToString());
            Log.Info(this, "HGA5 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga5RHBias().ToString());
            Log.Info(this, "HGA5 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga5R1Bias().ToString());
            Log.Info(this, "HGA5 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga5R2Bias().ToString());
            _carrierUnderTest.Hga5.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga5WriterBias());
            _carrierUnderTest.Hga5.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga5TABias());
            _carrierUnderTest.Hga5.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga5WHBias());
            _carrierUnderTest.Hga5.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga5RHBias());
            _carrierUnderTest.Hga5.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga5R1Bias());
            _carrierUnderTest.Hga5.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga5R2Bias());

            //hga6
            Log.Info(this, "HGA6 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga6WriterBias().ToString());
            Log.Info(this, "HGA6 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga6TABias().ToString());
            Log.Info(this, "HGA6 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga6WHBias().ToString());
            Log.Info(this, "HGA6 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga6RHBias().ToString());
            Log.Info(this, "HGA6 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga6R1Bias().ToString());
            Log.Info(this, "HGA6 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga6R2Bias().ToString());
            _carrierUnderTest.Hga6.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga6WriterBias());
            _carrierUnderTest.Hga6.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga6TABias());
            _carrierUnderTest.Hga6.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga6WHBias());
            _carrierUnderTest.Hga6.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga6RHBias());
            _carrierUnderTest.Hga6.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga6R1Bias());
            _carrierUnderTest.Hga6.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga6R2Bias());

            //hga7
            Log.Info(this, "HGA7 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga7WriterBias().ToString());
            Log.Info(this, "HGA7 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga7TABias().ToString());
            Log.Info(this, "HGA7 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga7WHBias().ToString());
            Log.Info(this, "HGA7 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga7RHBias().ToString());
            Log.Info(this, "HGA7 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga7R1Bias().ToString());
            Log.Info(this, "HGA7 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga7R2Bias().ToString());
            _carrierUnderTest.Hga7.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga7WriterBias());
            _carrierUnderTest.Hga7.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga7TABias());
            _carrierUnderTest.Hga7.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga7WHBias());
            _carrierUnderTest.Hga7.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga7RHBias());
            _carrierUnderTest.Hga7.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga7R1Bias());
            _carrierUnderTest.Hga7.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga7R2Bias());

            //hga8
            Log.Info(this, "HGA8 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga8WriterBias().ToString());
            Log.Info(this, "HGA8 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga8TABias().ToString());
            Log.Info(this, "HGA8 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga8WHBias().ToString());
            Log.Info(this, "HGA8 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga8RHBias().ToString());
            Log.Info(this, "HGA8 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga8R1Bias().ToString());
            Log.Info(this, "HGA8 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga8R2Bias().ToString());
            _carrierUnderTest.Hga8.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga8WriterBias());
            _carrierUnderTest.Hga8.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga8TABias());
            _carrierUnderTest.Hga8.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga8WHBias());
            _carrierUnderTest.Hga8.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga8RHBias());
            _carrierUnderTest.Hga8.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga8R1Bias());
            _carrierUnderTest.Hga8.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga8R2Bias());

            //hga9
            Log.Info(this, "HGA9 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga9WriterBias().ToString());
            Log.Info(this, "HGA9 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga9TABias().ToString());
            Log.Info(this, "HGA9 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga9WHBias().ToString());
            Log.Info(this, "HGA9 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga9RHBias().ToString());
            Log.Info(this, "HGA9 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga9R1Bias().ToString());
            Log.Info(this, "HGA9 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga9R2Bias().ToString());
            _carrierUnderTest.Hga9.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga9WriterBias());
            _carrierUnderTest.Hga9.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga9TABias());
            _carrierUnderTest.Hga9.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga9WHBias());
            _carrierUnderTest.Hga9.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga9RHBias());
            _carrierUnderTest.Hga9.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga9R1Bias());
            _carrierUnderTest.Hga9.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga9R2Bias());


            //hga10
            Log.Info(this, "HGA10 bias voltage Writer: " + this.TestProbe79GetAllBiasVoltage.Hga10WriterBias().ToString());
            Log.Info(this, "HGA10 bias voltage TA: " + this.TestProbe79GetAllBiasVoltage.Hga10TABias().ToString());
            Log.Info(this, "HGA10 bias voltage WH: " + this.TestProbe79GetAllBiasVoltage.Hga10WHBias().ToString());
            Log.Info(this, "HGA10 bias voltage RH: " + this.TestProbe79GetAllBiasVoltage.Hga10RHBias().ToString());
            Log.Info(this, "HGA10 bias voltage R1: " + this.TestProbe79GetAllBiasVoltage.Hga10R1Bias().ToString());
            Log.Info(this, "HGA10 bias voltage R2: " + this.TestProbe79GetAllBiasVoltage.Hga10R2Bias().ToString());
            _carrierUnderTest.Hga10.Set_Bias_Ch1(this.TestProbe79GetAllBiasVoltage.Hga10WriterBias());
            _carrierUnderTest.Hga10.Set_Bias_Ch2(this.TestProbe79GetAllBiasVoltage.Hga10TABias());
            _carrierUnderTest.Hga10.Set_Bias_Ch3(this.TestProbe79GetAllBiasVoltage.Hga10WHBias());
            _carrierUnderTest.Hga10.Set_Bias_Ch4(this.TestProbe79GetAllBiasVoltage.Hga10RHBias());
            _carrierUnderTest.Hga10.Set_Bias_Ch5(this.TestProbe79GetAllBiasVoltage.Hga10R1Bias());
            _carrierUnderTest.Hga10.Set_Bias_Ch6(this.TestProbe79GetAllBiasVoltage.Hga10R2Bias());

        }

        public void LogGetAllSensingVoltage()
        {
            Log.Info(this, "HGA1 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga1WriterSensing().ToString());
            Log.Info(this, "HGA1 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga1TASensing().ToString());
            Log.Info(this, "HGA1 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga1WHSensing().ToString());
            Log.Info(this, "HGA1 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga1RHSensing().ToString());
            Log.Info(this, "HGA1 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga1R1Sensing().ToString());
            Log.Info(this, "HGA1 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga1R2Sensing().ToString());
            _carrierUnderTest.Hga1.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga1WriterSensing());
            _carrierUnderTest.Hga1.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga1TASensing());
            _carrierUnderTest.Hga1.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga1WHSensing());
            _carrierUnderTest.Hga1.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga1RHSensing());
            _carrierUnderTest.Hga1.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga1R1Sensing());
            _carrierUnderTest.Hga1.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga1R2Sensing());

            Log.Info(this, "HGA2 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga2WriterSensing().ToString());
            Log.Info(this, "HGA2 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga2TASensing().ToString());
            Log.Info(this, "HGA2 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga2WHSensing().ToString());
            Log.Info(this, "HGA2 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga2RHSensing().ToString());
            Log.Info(this, "HGA2 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga2R1Sensing().ToString());
            Log.Info(this, "HGA2 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga2R2Sensing().ToString());
            _carrierUnderTest.Hga2.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga2WriterSensing());
            _carrierUnderTest.Hga2.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga2TASensing());
            _carrierUnderTest.Hga2.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga2WHSensing());
            _carrierUnderTest.Hga2.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga2RHSensing());
            _carrierUnderTest.Hga2.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga2R1Sensing());
            _carrierUnderTest.Hga2.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga2R2Sensing());

            Log.Info(this, "HGA3 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga3WriterSensing().ToString());
            Log.Info(this, "HGA3 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga3TASensing().ToString());
            Log.Info(this, "HGA3 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga3WHSensing().ToString());
            Log.Info(this, "HGA3 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga3RHSensing().ToString());
            Log.Info(this, "HGA3 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga3R1Sensing().ToString());
            Log.Info(this, "HGA3 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga3R2Sensing().ToString());
            _carrierUnderTest.Hga3.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga3WriterSensing());
            _carrierUnderTest.Hga3.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga3TASensing());
            _carrierUnderTest.Hga3.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga3WHSensing());
            _carrierUnderTest.Hga3.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga3RHSensing());
            _carrierUnderTest.Hga3.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga3R1Sensing());
            _carrierUnderTest.Hga3.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga3R2Sensing());

            Log.Info(this, "HGA4 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga4WriterSensing().ToString());
            Log.Info(this, "HGA4 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga4TASensing().ToString());
            Log.Info(this, "HGA4 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga4WHSensing().ToString());
            Log.Info(this, "HGA4 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga4RHSensing().ToString());
            Log.Info(this, "HGA4 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga4R1Sensing().ToString());
            Log.Info(this, "HGA4 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga4R2Sensing().ToString());
            _carrierUnderTest.Hga4.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga4WriterSensing());
            _carrierUnderTest.Hga4.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga4TASensing());
            _carrierUnderTest.Hga4.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga4WHSensing());
            _carrierUnderTest.Hga4.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga4RHSensing());
            _carrierUnderTest.Hga4.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga4R1Sensing());
            _carrierUnderTest.Hga4.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga4R2Sensing());

            Log.Info(this, "HGA5 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga5WriterSensing().ToString());
            Log.Info(this, "HGA5 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga5TASensing().ToString());
            Log.Info(this, "HGA5 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga5WHSensing().ToString());
            Log.Info(this, "HGA5 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga5RHSensing().ToString());
            Log.Info(this, "HGA5 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga5R1Sensing().ToString());
            Log.Info(this, "HGA5 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga5R2Sensing().ToString());
            _carrierUnderTest.Hga5.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga5WriterSensing());
            _carrierUnderTest.Hga5.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga5TASensing());
            _carrierUnderTest.Hga5.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga5WHSensing());
            _carrierUnderTest.Hga5.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga5RHSensing());
            _carrierUnderTest.Hga5.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga5R1Sensing());
            _carrierUnderTest.Hga5.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga5R2Sensing());

            Log.Info(this, "HGA6 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga6WriterSensing().ToString());
            Log.Info(this, "HGA6 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga6TASensing().ToString());
            Log.Info(this, "HGA6 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga6WHSensing().ToString());
            Log.Info(this, "HGA6 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga6RHSensing().ToString());
            Log.Info(this, "HGA6 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga6R1Sensing().ToString());
            Log.Info(this, "HGA6 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga6R2Sensing().ToString());
            _carrierUnderTest.Hga6.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga6WriterSensing());
            _carrierUnderTest.Hga6.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga6TASensing());
            _carrierUnderTest.Hga6.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga6WHSensing());
            _carrierUnderTest.Hga6.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga6RHSensing());
            _carrierUnderTest.Hga6.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga6R1Sensing());
            _carrierUnderTest.Hga6.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga6R2Sensing());

            Log.Info(this, "HGA7 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga7WriterSensing().ToString());
            Log.Info(this, "HGA7 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga7TASensing().ToString());
            Log.Info(this, "HGA7 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga7WHSensing().ToString());
            Log.Info(this, "HGA7 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga7RHSensing().ToString());
            Log.Info(this, "HGA7 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga7R1Sensing().ToString());
            Log.Info(this, "HGA7 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga7R2Sensing().ToString());
            _carrierUnderTest.Hga7.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga7WriterSensing());
            _carrierUnderTest.Hga7.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga7TASensing());
            _carrierUnderTest.Hga7.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga7WHSensing());
            _carrierUnderTest.Hga7.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga7RHSensing());
            _carrierUnderTest.Hga7.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga7R1Sensing());
            _carrierUnderTest.Hga7.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga7R2Sensing());

            Log.Info(this, "HGA8 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga8WriterSensing().ToString());
            Log.Info(this, "HGA8 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga8TASensing().ToString());
            Log.Info(this, "HGA8 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga8WHSensing().ToString());
            Log.Info(this, "HGA8 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga8RHSensing().ToString());
            Log.Info(this, "HGA8 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga8R1Sensing().ToString());
            Log.Info(this, "HGA8 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga8R2Sensing().ToString());
            _carrierUnderTest.Hga8.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga8WriterSensing());
            _carrierUnderTest.Hga8.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga8TASensing());
            _carrierUnderTest.Hga8.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga8WHSensing());
            _carrierUnderTest.Hga8.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga8RHSensing());
            _carrierUnderTest.Hga8.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga8R1Sensing());
            _carrierUnderTest.Hga8.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga8R2Sensing());

            Log.Info(this, "HGA9 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga9WriterSensing().ToString());
            Log.Info(this, "HGA9 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga9TASensing().ToString());
            Log.Info(this, "HGA9 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga9WHSensing().ToString());
            Log.Info(this, "HGA9 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga9RHSensing().ToString());
            Log.Info(this, "HGA9 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga9R1Sensing().ToString());
            Log.Info(this, "HGA9 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga9R2Sensing().ToString());
            _carrierUnderTest.Hga9.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga9WriterSensing());
            _carrierUnderTest.Hga9.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga9TASensing());
            _carrierUnderTest.Hga9.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga9WHSensing());
            _carrierUnderTest.Hga9.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga9RHSensing());
            _carrierUnderTest.Hga9.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga9R1Sensing());
            _carrierUnderTest.Hga9.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga9R2Sensing());

            Log.Info(this, "HGA10 sensing voltage Writer: " + this.TestProbe80GetAllSensingVoltage.Hga10WriterSensing().ToString());
            Log.Info(this, "HGA10 sensing voltage TA: " + this.TestProbe80GetAllSensingVoltage.Hga10TASensing().ToString());
            Log.Info(this, "HGA10 sensing voltage WH: " + this.TestProbe80GetAllSensingVoltage.Hga10WHSensing().ToString());
            Log.Info(this, "HGA10 sensing voltage RH: " + this.TestProbe80GetAllSensingVoltage.Hga10RHSensing().ToString());
            Log.Info(this, "HGA10 sensing voltage R1: " + this.TestProbe80GetAllSensingVoltage.Hga10R1Sensing().ToString());
            Log.Info(this, "HGA10 sensing voltage R2: " + this.TestProbe80GetAllSensingVoltage.Hga10R2Sensing().ToString());
            _carrierUnderTest.Hga10.Set_Sensing_Ch1(this.TestProbe80GetAllSensingVoltage.Hga10WriterSensing());
            _carrierUnderTest.Hga10.Set_Sensing_Ch2(this.TestProbe80GetAllSensingVoltage.Hga10TASensing());
            _carrierUnderTest.Hga10.Set_Sensing_Ch3(this.TestProbe80GetAllSensingVoltage.Hga10WHSensing());
            _carrierUnderTest.Hga10.Set_Sensing_Ch4(this.TestProbe80GetAllSensingVoltage.Hga10RHSensing());
            _carrierUnderTest.Hga10.Set_Sensing_Ch5(this.TestProbe80GetAllSensingVoltage.Hga10R1Sensing());
            _carrierUnderTest.Hga10.Set_Sensing_Ch6(this.TestProbe80GetAllSensingVoltage.Hga10R2Sensing());

        }

        public void LogGetVolDelta()
        {

            Log.Info(this, "HGA1 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA1() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA1().ToString());
            Log.Info(this, "HGA2 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA2() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA2().ToString());
            Log.Info(this, "HGA3 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA3() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA3().ToString());
            Log.Info(this, "HGA4 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA4() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA4().ToString());
            Log.Info(this, "HGA5 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA5() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA5().ToString());
            Log.Info(this, "HGA6 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA6() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA6().ToString());
            Log.Info(this, "HGA7 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA7() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA7().ToString());
            Log.Info(this, "HGA8 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA8() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA8().ToString());
            Log.Info(this, "HGA9 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA9() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA9().ToString());
            Log.Info(this, "HGA10 voltage delta 1: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA10() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta1HGA10().ToString());

            _carrierUnderTest.Hga1.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA1() * 0.149 / 1000));
            _carrierUnderTest.Hga2.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA2() * 0.149 / 1000));
            _carrierUnderTest.Hga3.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA3() * 0.149 / 1000));
            _carrierUnderTest.Hga4.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA4() * 0.149 / 1000));
            _carrierUnderTest.Hga5.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA5() * 0.149 / 1000));
            _carrierUnderTest.Hga6.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA6() * 0.149 / 1000));
            _carrierUnderTest.Hga7.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA7() * 0.149 / 1000));
            _carrierUnderTest.Hga8.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA8() * 0.149 / 1000));
            _carrierUnderTest.Hga9.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA9() * 0.149 / 1000));
            _carrierUnderTest.Hga10.Set_Voltage_Delta1(((double)this.TestProbe72GetVolDelta.GetVolDelta1HGA10() * 0.149 / 1000));

            Log.Info(this, "HGA1 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA1() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA1().ToString());
            Log.Info(this, "HGA2 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA2() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA2().ToString());
            Log.Info(this, "HGA3 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA3() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA3().ToString());
            Log.Info(this, "HGA4 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA4() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA4().ToString());
            Log.Info(this, "HGA5 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA5() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA5().ToString());
            Log.Info(this, "HGA6 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA6() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA6().ToString());
            Log.Info(this, "HGA7 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA7() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA7().ToString());
            Log.Info(this, "HGA8 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA8() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA8().ToString());
            Log.Info(this, "HGA9 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA9() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA9().ToString());
            Log.Info(this, "HGA10 voltage delta 2: " + ((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA10() * 0.149 / 1000).ToString("F3", CultureInfo.InstalledUICulture) + "mV" + " " + TestProbe72GetVolDelta.GetVolDelta2HGA10().ToString());

            _carrierUnderTest.Hga1.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA1() * 0.149 / 1000));
            _carrierUnderTest.Hga2.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA2() * 0.149 / 1000));
            _carrierUnderTest.Hga3.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA3() * 0.149 / 1000));
            _carrierUnderTest.Hga4.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA4() * 0.149 / 1000));
            _carrierUnderTest.Hga5.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA5() * 0.149 / 1000));
            _carrierUnderTest.Hga6.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA6() * 0.149 / 1000));
            _carrierUnderTest.Hga7.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA7() * 0.149 / 1000));
            _carrierUnderTest.Hga8.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA8() * 0.149 / 1000));
            _carrierUnderTest.Hga9.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA9() * 0.149 / 1000));
            _carrierUnderTest.Hga10.Set_Voltage_Delta2(((double)this.TestProbe72GetVolDelta.GetVolDelta2HGA10() * 0.149 / 1000));
        }

        public void CollectLDUAndLEDData()
        {
            int HgaIndex = TestProbe63GetLDUAndLEDData.HGAIndex - 1;


            LDUVoltagePoint[HgaIndex, 0] = (double)TestProbe63GetLDUAndLEDData.Point0LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 1] = (double)TestProbe63GetLDUAndLEDData.Point1LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 2] = (double)TestProbe63GetLDUAndLEDData.Point2LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 3] = (double)TestProbe63GetLDUAndLEDData.Point3LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 4] = (double)TestProbe63GetLDUAndLEDData.Point4LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 5] = (double)TestProbe63GetLDUAndLEDData.Point5LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 6] = (double)TestProbe63GetLDUAndLEDData.Point6LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 7] = (double)TestProbe63GetLDUAndLEDData.Point7LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 8] = (double)TestProbe63GetLDUAndLEDData.Point8LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 9] = (double)TestProbe63GetLDUAndLEDData.Point9LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 10] = (double)TestProbe63GetLDUAndLEDData.Point10LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 11] = (double)TestProbe63GetLDUAndLEDData.Point11LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 12] = (double)TestProbe63GetLDUAndLEDData.Point12LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 13] = (double)TestProbe63GetLDUAndLEDData.Point13LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 14] = (double)TestProbe63GetLDUAndLEDData.Point14LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 15] = (double)TestProbe63GetLDUAndLEDData.Point15LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 16] = (double)TestProbe63GetLDUAndLEDData.Point16LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 17] = (double)TestProbe63GetLDUAndLEDData.Point17LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 18] = (double)TestProbe63GetLDUAndLEDData.Point18LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 19] = (double)TestProbe63GetLDUAndLEDData.Point19LDU() / 1000000;
            LDUVoltagePoint[HgaIndex, 20] = (double)TestProbe63GetLDUAndLEDData.Point20LDU() / 1000000;

            LEDVoltagePoint[HgaIndex, 0] = (double)TestProbe63GetLDUAndLEDData.Point0LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 1] = (double)TestProbe63GetLDUAndLEDData.Point1LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 2] = (double)TestProbe63GetLDUAndLEDData.Point2LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 3] = (double)TestProbe63GetLDUAndLEDData.Point3LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 4] = (double)TestProbe63GetLDUAndLEDData.Point4LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 5] = (double)TestProbe63GetLDUAndLEDData.Point5LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 6] = (double)TestProbe63GetLDUAndLEDData.Point6LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 7] = (double)TestProbe63GetLDUAndLEDData.Point7LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 8] = (double)TestProbe63GetLDUAndLEDData.Point8LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 9] = (double)TestProbe63GetLDUAndLEDData.Point9LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 10] = (double)TestProbe63GetLDUAndLEDData.Point10LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 11] = (double)TestProbe63GetLDUAndLEDData.Point11LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 12] = (double)TestProbe63GetLDUAndLEDData.Point12LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 13] = (double)TestProbe63GetLDUAndLEDData.Point13LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 14] = (double)TestProbe63GetLDUAndLEDData.Point14LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 15] = (double)TestProbe63GetLDUAndLEDData.Point15LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 16] = (double)TestProbe63GetLDUAndLEDData.Point16LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 17] = (double)TestProbe63GetLDUAndLEDData.Point17LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 18] = (double)TestProbe63GetLDUAndLEDData.Point18LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 19] = (double)TestProbe63GetLDUAndLEDData.Point19LED() / 1000000;
            LEDVoltagePoint[HgaIndex, 20] = (double)TestProbe63GetLDUAndLEDData.Point20LED() / 1000000;

            // CalculateLDUResistance(HgaIndex);
            // CalculateLEDResistance(HgaIndex);
           

            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint = LDUVoltagePoint;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint = LEDVoltagePoint;

            //HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().SetUpdateLDUVoltage = LDUVoltagePoint;


            Log.Info(this, "CollectLDUAndLEDData");

            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                for (int x = 0; x < 21; x++)
                {
                    Log.Info(this, "HGA{0}: LDU Point{1}:{2}", HgaIndex + 1, x, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, x]);
                }
                for (int x = 0; x < 21; x++)
                {
                    Log.Info(this, "HGA{0}: LED Point{1}:{2}", HgaIndex + 1, x, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[HgaIndex, x]);
                }
            }
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateCalculationParameter();
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.SweepEnable)
            {
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateFullSweepPhotoDiodeGraph_line1(HgaIndex, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().T1StopPoint, false);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateFullSweepPhotoDiodeGraph_line2(HgaIndex, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().T2StartPoint, false);
                
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateLEDGraph(HgaIndex, false);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateLDUGraph(HgaIndex, false);
            }
            else
            {
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateLEDGraph(HgaIndex, false);
                
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().Update4PtSweepPhotoDiodeGraph_line1(HgaIndex, false);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().Update4PtSweepPhotoDiodeGraph_line2(HgaIndex, false);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().Update4PtLDUGraph(HgaIndex, false);
            }

      
        }

        public void ApplyAllLDUAndLEDData()
        {
            bool useGompertz;
            for (int HgaIndex = 0; HgaIndex < 10; HgaIndex++)
            {
                useGompertz = false;
                double IThreshold = (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend2[HgaIndex] - HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend1[HgaIndex]) /
                    (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend1[HgaIndex] - HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend2[HgaIndex]);
                double MaxVPD = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[HgaIndex];

                if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation() )
                {
                    IThreshold = HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[HgaIndex];
                    Log.Info(this, "Gompertz Calculation HGA{0}, Serial Number = {1}, IThreshold = {2},", HgaIndex + 1, HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataReadInfor[HgaIndex].HgaSN, IThreshold);
                    useGompertz = true;
                }
                else
                {
                    Log.Info(this, "Y=MX+C Calculation HGA{0}, Serial Number = {1}, IThreshold = {2},", HgaIndex + 1, HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataReadInfor[HgaIndex].HgaSN, IThreshold);
                }

                Log.Info(this, "--------------- ISweep_C_Trend2:{0},ISweep_C_Trend1:{1},ISweep_M_Trend1:{2},ISweep_M_Trend2:{3}",
                            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend2[HgaIndex],
                            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend1[HgaIndex],
                            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend1[HgaIndex],
                            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend2[HgaIndex]);

                LDU_C[HgaIndex] = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_C[HgaIndex];
                LDU_M[HgaIndex] = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[HgaIndex];
                LED_C[HgaIndex] = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[HgaIndex];
                LED_M[HgaIndex] = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_M[HgaIndex];

                string hga_serial = "?";
                hga_serial = HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataReadInfor[HgaIndex].HgaSN;
                switch (HgaIndex)
                {
                    case 0:
                        _carrierUnderTest.Hga1.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga1.setLDUTurnOnVoltage(LDU_C[HgaIndex]); // turn On Voltage
                        _carrierUnderTest.Hga1.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga1.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga1.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga1.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga1.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga1.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        Log.Info(this, "LED_C:{0},LDU_C:{1},I_Threshold:{2},MaxVPD:{3}",
                            LED_C[HgaIndex],
                            LDU_C[HgaIndex],
                            IThreshold,
                            MaxVPD);
                        break;
                    case 1:
                        _carrierUnderTest.Hga2.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga2.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga2.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga2.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga2.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga2.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga2.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga2.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 2:
                        _carrierUnderTest.Hga3.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga3.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga3.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga3.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga3.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga3.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga3.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga3.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 3:
                        _carrierUnderTest.Hga4.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga4.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga4.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga4.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga4.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga4.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga4.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga4.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 4:
                        _carrierUnderTest.Hga5.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga5.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga5.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga5.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga5.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga5.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga5.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga5.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 5:
                        _carrierUnderTest.Hga6.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga6.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga6.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga6.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga6.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga6.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga6.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga6.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 6:
                        _carrierUnderTest.Hga7.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga7.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga7.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga7.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga7.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga7.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga7.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga7.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 7:
                        _carrierUnderTest.Hga8.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga8.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga8.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga8.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga8.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga8.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga8.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga8.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 8:
                        _carrierUnderTest.Hga9.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga9.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga9.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga9.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga9.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga9.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga9.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga9.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                    case 9:
                        _carrierUnderTest.Hga10.setLedIntercept(LED_C[HgaIndex]);
                        _carrierUnderTest.Hga10.setLDUTurnOnVoltage(LDU_C[HgaIndex]);
                        _carrierUnderTest.Hga10.setIThreshold(IThreshold);
                        _carrierUnderTest.Hga10.setMaxVPD(MaxVPD);
                        _carrierUnderTest.Hga10.set_ldu_Sweep_Spec_Min(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1);
                        _carrierUnderTest.Hga10.set_ldu_Sweep_Spec_Max(HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                        if (useGompertz)
                            _carrierUnderTest.Hga10.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Gompertz);
                        else
                            _carrierUnderTest.Hga10.set_IthresholdCalculationMethod(I_ThresholdCalculationMethod.Ymxc);
                        break;
                }

                SaveLEDResults_New(HgaIndex, hga_serial);
                if(HSTMachine.Workcell.OperationMode == OperationMode.Simulation)
                {
                    CommonFunctions.Instance.ConfigurationSetupRecipe.SweepEnable = true;
                }
                if (CommonFunctions.Instance.ConfigurationSetupRecipe.SweepEnable)
                {
                    if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                    {

                        
                        SaveLDUResults_New(HgaIndex, hga_serial, IThreshold, I_ThresholdCalculationMethod.Gompertz, I_ThresholdCalculationMethod.Gompertz);
                        Log.Info(this, "Use Gompertz Calculation");
                        
                       
                    }
                    else
                    {
                        //not using gompertz
                        SaveLDUResults_New(HgaIndex, hga_serial, IThreshold, I_ThresholdCalculationMethod.Ymxc, I_ThresholdCalculationMethod.Ymxc);
                    }

                }
                else
                    Save4ptLDUResults_New(HgaIndex, hga_serial);
            }
        }


        public void CalculateLDUResistance(int HGAIndex)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 21;


            // double[] data = { 1, 2, 2, 2, 2, 2, 7, 5, 6, 4 };


            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;


            // Double.TryParse(textBoxLDUStep.Text, out result);
            LDUXPointSize = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep / 1000; //result / 1000;
            //Double.TryParse(textBoxLDUStart.Text, out result);
            lduStartCurrentLimit = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint / 1000;//result / 1000;
                                                                                                                 // Double.TryParse(textBoxLDUStop.Text, out result);
            lduStopCurrentLimit = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint / 1000;//result / 1000;


            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            //double[] LDUVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };// new double[totalPoints];
            //double[] LEDVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };


            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];


            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUVoltagePoint[HGAIndex, i];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);
                        LDUpoints++;
                    }
                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUVoltagePoint[HGAIndex, i] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2

                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }

                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);
                //  labelymxcLDU.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F3", CultureInfo.InvariantCulture), b_ldu.ToString("F6", CultureInfo.InvariantCulture));

                LDU_M[HGAIndex] = Math.Round(LDUSlope * 1000, 3);
                LDU_C[HGAIndex] = b_ldu;

                temp_3 = 100000;
                temp_1 = 0;
                /*   for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                   {

                       if (k == LDUpoints

                           )
                           break;

                       DataPoint d_LDU = new DataPoint(i, LDUVoltagePoint[HGAIndex, k]);
                       d_LDU.ToolTip = "#VALY";

                       LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                       DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                       d_LDULFit.ToolTip = "#VALY";

                       chartLDU.Series[0].Points.Add(d_LDU);
                       chartLDU.Series[1].Points.Add(d_LDULFit);

                       if (LDUVoltagePoint[HGAIndex, k] > temp_1)
                           temp_1 = LDUVoltagePoint[HGAIndex, k];

                       if (temp_3 > LDUVoltagePoint[HGAIndex, k])
                           temp_3 = LDUVoltagePoint[HGAIndex, k];

                       k++;
                   }

                   this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                   this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                   this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                   this.chartLDU.ChartAreas[0].AxisY.Interval = (double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                   this.chartLDU.ChartAreas[0].AxisY.Minimum = temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                   this.chartLDU.ChartAreas[0].AxisY.Maximum = temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;
   */
            }

            catch
            {

            }
        }

        public void CalculateLEDResistance(int HGAIndex)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LEDVoltageAverage = 0;
            double XBar_LEDCurrentAverage = 0;


            int LEDpoints = 0;


            double LEDSlope = 0;

            double b_led = 0;


            int LEDtotalPoints = 21;
            
            double LEDXPointSize;//size;
            double ledStartCurrentLimit; //start; // starting point
            double ledStopCurrentLimit;// stop; //end point



            double result; //=1;//size;



            LEDXPointSize = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDUStep / 1000;
           
            ledStartCurrentLimit = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU1stPoint / 1000; //result / 1000;
            
            ledStopCurrentLimit = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU2ndPoint / 1000;//result / 1000;


            double[] XBarLED = new double[LEDtotalPoints];
            double[] YBarLED = new double[LEDtotalPoints];
            double[] LEDVoltageLinearFitPoint = new double[LEDtotalPoints];// new double[totalPoints];

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LEDtotalPoints; i++)
                {
                    if (LEDVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LEDVoltagePoint[HGAIndex, i];
                        temp_3 += ledStartCurrentLimit + (i * LEDXPointSize);


                        LEDpoints++;
                    }


                }
                YBar_LEDVoltageAverage = temp_1 / LEDpoints;
                XBar_LEDCurrentAverage = temp_3 / LEDpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LEDVoltageAverage = 0;
                    XBar_LEDCurrentAverage = 0;

                }
                for (int i = 0; i < LEDpoints; i++)
                {
                    YBarLED[i] = LEDVoltagePoint[HGAIndex, i] - YBar_LEDVoltageAverage;//(Y-YBar)
                    XBarLED[i] = (ledStartCurrentLimit + (i * LEDXPointSize)) - XBar_LEDCurrentAverage; // (X -XBar)

                    temp_4 += XBarLED[i] * YBarLED[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLED[i] * XBarLED[i]; // (X -XBar)^2
                }
                //calculate M
                LEDSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LEDSlope = 0;
                }

                b_led = YBar_LEDVoltageAverage - (LEDSlope * XBar_LEDCurrentAverage);

                LED_M[HGAIndex] = LEDSlope;
                LED_C[HGAIndex] = Math.Round(b_led, 3);

            }

            catch
            {

            }
        }

        private void SaveLEDResults(int HgaIndex, string hga_serial)
        {
            if(HSTSettings.Instance.Directory.LDUDirectoryPath == null) HSTSettings.Instance.Directory.LDUDirectoryPath = "C:\\Seagate\\HGA.HST\\Data\\LDU";
            if (!Directory.Exists(HSTSettings.Instance.Directory.LDUDirectoryPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.LDUDirectoryPath);
            }
            string LEDDirectoryPath = string.Format("{0}\\LED_{1}.csv", HSTSettings.Instance.Directory.LDUDirectoryPath, DateTime.Now.ToString("yyyy-MM-dd"));//string.Format("{ 0}{1}.xls", CommonFunctions.Instance.MeasurementTestFileDirectory, "BiasAndSensingVoltageData");

            if (!File.Exists(LEDDirectoryPath))
            {
                double startCurrent;
                double stopCurrent;
                double size;
                size = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDUStep / 1000;

                startCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU1stPoint / 1000; //result / 1000;
                stopCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU2ndPoint / 1000;//result / 1000;

                if (startCurrent == 0 && stopCurrent == 0 && size == 0)
                {
                    return;
                }

                using (StreamWriter WriteToFile = new StreamWriter(LEDDirectoryPath))
                {

                    WriteToFile.Write("HGA,LED_THRESHOLD, LED_SLOPE");//, DELTA_TA_RES, TA_RES_LOWCURRENT, TA_RES_HIGHCURRENT");//,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",textBoxLDUStart.Text);

                    for (int i = 0; i < 21; i++)
                    {
                        if (LEDVoltagePoint[HgaIndex, i] != 0)
                            WriteToFile.Write(",{0}mA", (double)(startCurrent + i * size));
                    }
                    WriteToFile.WriteLine();

                }
            }

            using (FileStream fs = new FileStream(LEDDirectoryPath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(String.Format("{0},{1},{2},", hga_serial,
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[HgaIndex].ToString("F3", CultureInfo.InvariantCulture),
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_M[HgaIndex].ToString("F3", CultureInfo.InvariantCulture)));

                for (int i = 0; i < 21; i++)
                {
                    if (LEDVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", LEDVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }
                sw.WriteLine();

            }

        }

        private void SaveLDUResults(int HgaIndex, string hga_serial)
        {
            if(HSTSettings.Instance.Directory.LDUDirectoryPath == null) HSTSettings.Instance.Directory.LDUDirectoryPath = "C:\\Seagate\\HGA.HST\\Data\\LDU";
            if (!Directory.Exists(HSTSettings.Instance.Directory.LDUDirectoryPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.LDUDirectoryPath);
            }
            //rawinun
            string LDUDirectoryPath = string.Format("{0}\\LDU_{1}.csv", HSTSettings.Instance.Directory.LDUDirectoryPath, DateTime.Now.ToString("yyyy-MM-dd"));//string.Format("{ 0}{1}.xls", CommonFunctions.Instance.MeasurementTestFileDirectory, "BiasAndSensingVoltageData");

            if (!File.Exists(LDUDirectoryPath))
            {
                double startCurrent;
                double stopCurrent;
                double size;
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    size = 0.7;

                    startCurrent = 10.1;
                                                                                                                            //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = 15.0;
                }
                else
                {
                    size = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep / 1000;

                    startCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint / 1000; //result / 1000;
                                                                                                                            //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint / 1000;//result / 1000;
                }

                if (startCurrent == 0 && stopCurrent == 0 && size == 0)
                {
                    MessageBox.Show("Goto LDU Graph Tab to reload the configuration");
                    return;
                }

                using (StreamWriter WriteToFile = new StreamWriter(LDUDirectoryPath))
                {

                    WriteToFile.Write("HGA,LDU_SLOPE, LDU_Y-INTERCEPT");

                    for (int i = 0; i < 21; i++)
                    {
                        if (LDUVoltagePoint[HgaIndex, i] != 0)
                            WriteToFile.Write(",{0}mA", (double)(startCurrent + i * size));
                    }
                    WriteToFile.WriteLine();

                }
            }

            using (FileStream fs = new FileStream(LDUDirectoryPath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {/*HgaIndex + 1*/
                sw.Write(String.Format("{0},{1},{2},", hga_serial,
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[HgaIndex].ToString("F3", CultureInfo.InvariantCulture),
                  HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_C[HgaIndex].ToString("F3", CultureInfo.InvariantCulture)));

                for (int i = 0; i < 21; i++)
                {
                    if (LDUVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", LDUVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }

                sw.WriteLine();

            }

        }

        private void LogPhotoDiodeVoltageData()
        {
            int HgaIndex = TestProbe86GetPhotodiodeDataByHGA.HGAIndex - 1;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 0] = (double)TestProbe86GetPhotodiodeDataByHGA.Point0PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 1] = (double)TestProbe86GetPhotodiodeDataByHGA.Point1PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 2] = (double)TestProbe86GetPhotodiodeDataByHGA.Point2PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 3] = (double)TestProbe86GetPhotodiodeDataByHGA.Point3PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 4] = (double)TestProbe86GetPhotodiodeDataByHGA.Point4PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 5] = (double)TestProbe86GetPhotodiodeDataByHGA.Point5PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 6] = (double)TestProbe86GetPhotodiodeDataByHGA.Point6PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 7] = (double)TestProbe86GetPhotodiodeDataByHGA.Point7PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 8] = (double)TestProbe86GetPhotodiodeDataByHGA.Point8PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 9] = (double)TestProbe86GetPhotodiodeDataByHGA.Point9PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 10] = (double)TestProbe86GetPhotodiodeDataByHGA.Point10PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 11] = (double)TestProbe86GetPhotodiodeDataByHGA.Point11PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 12] = (double)TestProbe86GetPhotodiodeDataByHGA.Point12PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 13] = (double)TestProbe86GetPhotodiodeDataByHGA.Point13PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 14] = (double)TestProbe86GetPhotodiodeDataByHGA.Point14PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 15] = (double)TestProbe86GetPhotodiodeDataByHGA.Point15PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 16] = (double)TestProbe86GetPhotodiodeDataByHGA.Point16PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 17] = (double)TestProbe86GetPhotodiodeDataByHGA.Point17PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 18] = (double)TestProbe86GetPhotodiodeDataByHGA.Point18PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 19] = (double)TestProbe86GetPhotodiodeDataByHGA.Point19PhotoDiodeVoltage() / 1000000;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, 20] = ((double)TestProbe86GetPhotodiodeDataByHGA.Point20PhotoDiodeVoltage() / 1000000);

            for (int x = 0; x < 21; x++)
            {
                Log.Info(this, "HGA = {0}, point {1}, {2}", TestProbe86GetPhotodiodeDataByHGA.HGAIndex, x, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, x]);
            }

            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateData(HgaIndex);
        }

        private void RestoreLDUConfiguration2()
        {
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUEnable = Convert.ToBoolean(TestProbe87GetLDUConfiguration_2.enable_ldu);//CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().Point_4Mode = Convert.ToBoolean(TestProbe87GetLDUConfiguration_2.LDU4PtMode);//!CommonFunctions.Instance.ConfigurationSetupRecipe.SweepEnable;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUSweepMode = !Convert.ToBoolean(TestProbe87GetLDUConfiguration_2.LDU4PtMode);//CommonFunctions.Instance.ConfigurationSetupRecipe.SweepEnable;

            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDStartCurrent = TestProbe87GetLDUConfiguration_2.GetLEDStartCurrent();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU1stPoint);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDStopCurrent = TestProbe87GetLDUConfiguration_2.GetLEDStopCurrent();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU2ndPoint);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDStepSize =TestProbe87GetLDUConfiguration_2.GetLEDSteppingSize();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDUStep);

            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUStartCurrent = TestProbe87GetLDUConfiguration_2.GetLDUStartCurrent();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUStopCurrent = TestProbe87GetLDUConfiguration_2.GetLDUStopCurrent();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUStepSize = TestProbe87GetLDUConfiguration_2.GetLDUSteppingSize();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep);

            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1 = TestProbe87GetLDUConfiguration_2.GetLDUISweep1();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep2 = TestProbe87GetLDUConfiguration_2.GetLDUISweep2();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep3 = TestProbe87GetLDUConfiguration_2.GetLDUISweep3();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrent3rdPointforIThreshold);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4 = TestProbe87GetLDUConfiguration_2.GetLDUISweep4();//Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrent4thPointforIThreshold);
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUTimeInterval = TestProbe87GetLDUConfiguration_2.GetLDUtimeInterval();
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().T1StopPoint = CommonFunctions.Instance.ConfigurationSetupRecipe.Trend1StopPoint;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().T2StartPoint = CommonFunctions.Instance.ConfigurationSetupRecipe.Trend2StartPoint;
            HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateSetting();
        }

        private void SaveLDUResults_New(int HgaIndex, string hdnum, double Ithreshold, I_ThresholdCalculationMethod Defaultcalculationmethod, I_ThresholdCalculationMethod calculationmethod)
        {
            //rawinun
            if (HSTSettings.Instance.Directory.LDUDirectoryPath == null) HSTSettings.Instance.Directory.LDUDirectoryPath = "C:\\Seagate\\HGA.HST\\Data\\LDU";
            if (!Directory.Exists(HSTSettings.Instance.Directory.LDUDirectoryPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.LDUDirectoryPath);
            }
            //rawinun
            string LDUDirectoryPath = string.Format("{0}\\LDU_{1}.csv", HSTSettings.Instance.Directory.LDUDirectoryPath, DateTime.Now.ToString("yyyy-MM-dd"));//string.Format("{ 0}{1}.xls", CommonFunctions.Instance.MeasurementTestFileDirectory, "BiasAndSensingVoltageData");
            if (!File.Exists(LDUDirectoryPath))
            {
                double startCurrent;
                double stopCurrent;
                double size;
                //startCurrent = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUStartCurrent;
                //stopCurrent = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUStopCurrent;
                //size = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUStepSize;


                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    size = 0.7;

                    startCurrent = 10.1;
                    //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = 15.0;
                }
                else
                {
                    size = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep / 1000;

                    startCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint / 1000; //result / 1000;
                                                                                                                            //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint / 1000;//result / 1000;
                }
            //    size = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep / 1000;

            //    startCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint / 1000; //result / 1000;
                //Double.TryParse(textBoxLEDStop.Text, out result);
            //    stopCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint / 1000;//result / 1000;


                if (startCurrent == 0 && stopCurrent == 0 && size == 0)
                {
                    MessageBox.Show("Goto LDU Graph Tab to reload the configuration");
                    return;
                }

                using (StreamWriter WriteToFile = new StreamWriter(LDUDirectoryPath))
                {
                    // WriteToFile.Write("HGA,LDU_THRESHOLD,pt1,pt2,pt3,pt4,pt5,pt6,pt7,pt8,pt9,pt10,pt11,pt12,");
                    //  WriteToFile.WriteLine("pt13,pt14,pt15,pt16,pt17,pt18,pt19,pt20,pt21");

                    WriteToFile.Write("HGA,LDU_RESISTANCE,LDU_INTERCEPT");//,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",textBoxLDUStart.Text);

                    for (int i = 0; i < 8; i++)
                    {
                    //    if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, i] != 0)
                            WriteToFile.Write(",LDU @ {0}mA", (double)(startCurrent + i * size));
                    }
                    WriteToFile.Write(",VPD-Max, I_Threshold, V-TurnOnVoltage, CalculationSetting, CalculationMethod");//,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",textBoxLDUStart.Text);
                    for (int i = 0; i < 8; i++)
                    {
                     //   if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, i] != 0)
                            WriteToFile.Write(",VPD @ {0}mA", (double)(startCurrent + i * size));
                    }
                    WriteToFile.Write(",Gompertz_IThreshold,Y=mx+c_Ithreshold");

                    WriteToFile.WriteLine();
                }

            }

            using (FileStream fs = new FileStream(LDUDirectoryPath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(String.Format("{0},{1},{2}", hdnum, (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[HgaIndex] * 1000).ToString("F3", CultureInfo.InvariantCulture), (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_C[HgaIndex]).ToString("F3", CultureInfo.InvariantCulture)));
                for (int i = 0; i < 21; i++)
                {
                 //   if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }

                sw.Write(String.Format(",{0},{1},{2}, {3}, {4}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                                                 //   HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                                                    Ithreshold.ToString("F5", CultureInfo.InvariantCulture),
                                                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_C[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                                                    Defaultcalculationmethod.ToString(),
                                                    calculationmethod.ToString()));

                for (int i = 0; i < 21; i++)
                {
                //    if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }

                sw.Write(",{0},{1}", HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().I_threshold[HgaIndex].ToString("F5", CultureInfo.InvariantCulture), HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[HgaIndex].ToString("F5", CultureInfo.InvariantCulture));
                sw.WriteLine();

            }

        }

        private void Save4ptLDUResults_New(int HgaIndex, string hdnum)
        {
            //rawinun
            if (HSTSettings.Instance.Directory.LDUDirectoryPath == null) HSTSettings.Instance.Directory.LDUDirectoryPath = "C:\\Seagate\\HGA.HST\\Data\\LDU";
            if (!Directory.Exists(HSTSettings.Instance.Directory.LDUDirectoryPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.LDUDirectoryPath);
            }
            //rawinun
            string LDUDirectoryPath = string.Format("{0}\\LDU_{1}.csv", HSTSettings.Instance.Directory.LDUDirectoryPath, DateTime.Now.ToString("yyyy-MM-dd"));//string.Format("{ 0}{1}.xls", CommonFunctions.Instance.MeasurementTestFileDirectory, "BiasAndSensingVoltageData");
            if (!File.Exists(LDUDirectoryPath))
            {
                double startCurrent;
                double stopCurrent;
                double size;
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    size = 0.7;

                    startCurrent = 10.1;
                    //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = 15.0;
                }
                else
                {
                    size = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep / 1000;

                    startCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint / 1000; //result / 1000;
                                                                                                                            //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint / 1000;//result / 1000;
                }

                if (startCurrent == 0 && stopCurrent == 0 && size == 0)
                {
                    MessageBox.Show("Goto LDU Graph Tab to reload the configuration");
                    return;
                }

                using (StreamWriter WriteToFile = new StreamWriter(LDUDirectoryPath))
                {
                    // WriteToFile.Write("HGA,LDU_THRESHOLD,pt1,pt2,pt3,pt4,pt5,pt6,pt7,pt8,pt9,pt10,pt11,pt12,");
                    //  WriteToFile.WriteLine("pt13,pt14,pt15,pt16,pt17,pt18,pt19,pt20,pt21");

                    WriteToFile.Write("HGA,LDU_RESISTANCE,LDU_INTERCEPT");//,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",textBoxLDUStart.Text);                                          
                    WriteToFile.Write(",LDU @ {0}mA,LDU @ {1}mA, LDU @ {2}mA, LDU @ {3}mA ", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep2, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep3, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                    WriteToFile.Write(",VPD-Max, I_Threshold, V-TurnOnVoltage");
                    //    WriteToFile.Write(",Delta TA Resistance (T2 -T1), TA1, TA2");
                    WriteToFile.Write(",VPD @ {0}mA, VPD @ {1}mA, VPD @ {2}mA, VPD @ {3}mA ", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep1, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep2, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep3, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUISweep4);
                    WriteToFile.WriteLine();
                }

            }

            using (FileStream fs = new FileStream(LDUDirectoryPath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(String.Format("{0},{1},{2}", hdnum, (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[HgaIndex] * 1000).ToString("F3", CultureInfo.InvariantCulture), (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_C[HgaIndex]).ToString("F3", CultureInfo.InvariantCulture)));
                for (int i = 0; i < 4; i++)
                {
                 //   if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }

                sw.Write(String.Format(",{0},{1},{2}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                                                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                                                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_C[HgaIndex].ToString("F5", CultureInfo.InvariantCulture)));
                for (int i = 0; i < 4; i++)
                {
                //    if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }
                //    sw.Write(String.Format(",{0},{1},{2}", DeltaTAResistance[HgaIndex].ToString("F3", CultureInfo.InvariantCulture), deltaTA1.ToString("F3", CultureInfo.InvariantCulture), deltaTA2.ToString("F3", CultureInfo.InvariantCulture)));

                sw.WriteLine();

            }

        }

        private void SaveLEDResults_New(int HgaIndex,string hdnum)
        {
            //rawinun
            if (HSTSettings.Instance.Directory.LDUDirectoryPath == null) HSTSettings.Instance.Directory.LDUDirectoryPath = "C:\\Seagate\\HGA.HST\\Data\\LDU";
            if (!Directory.Exists(HSTSettings.Instance.Directory.LDUDirectoryPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.LDUDirectoryPath);
            }
            string LEDDirectoryPath = string.Format("{0}\\LED_{1}.csv", HSTSettings.Instance.Directory.LDUDirectoryPath, DateTime.Now.ToString("yyyy-MM-dd"));//string.Format("{ 0}{1}.xls", CommonFunctions.Instance.MeasurementTestFileDirectory, "BiasAndSensingVoltageData");

            if (!File.Exists(LEDDirectoryPath))
            {
                double startCurrent;
                double stopCurrent;
                double size;

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    size = 0.7;

                    startCurrent = 10.1;
                    //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = 15.0;
                }
                else
                {
                    size = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep / 1000;

                    startCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint / 1000; //result / 1000;
                                                                                                                            //Double.TryParse(textBoxLEDStop.Text, out result);
                    stopCurrent = (double)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint / 1000;//result / 1000;
                }

                if (startCurrent == 0 && stopCurrent == 0 && size == 0)
                {
                    MessageBox.Show("Goto LDU Graph Tab to reload the configuration");
                    return;
                }

                using (StreamWriter WriteToFile = new StreamWriter(LEDDirectoryPath))
                {
                    //WriteToFile.Write("HGA,LED_THRESHOLD,pt1,pt2,pt3,pt4,pt5,pt6,pt7,pt8,pt9,pt10,pt11,pt12,");
                    //WriteToFile.WriteLine("pt13,pt14,pt15,pt16,pt17,pt18,pt19,pt20,pt21");

                    WriteToFile.Write("HGA,LED_THRESHOLD, LED_SLOPE");//,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",textBoxLDUStart.Text);

                    for (int i = 0; i < 21; i++)
                    {
                        if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[HgaIndex, i] != 0)
                            WriteToFile.Write(",LED @ {0}mA", (double)(startCurrent + i * size));
                    }
                    WriteToFile.WriteLine();

                }
            }

            using (FileStream fs = new FileStream(LEDDirectoryPath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(String.Format("{0},{1},{2}", hdnum, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[HgaIndex].ToString("F3", CultureInfo.InvariantCulture), (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_M[HgaIndex]).ToString("F3", CultureInfo.InvariantCulture)));
                for (int i = 0; i < 21; i++)
                {
                    if (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[HgaIndex, i] != 0)
                        sw.Write(String.Format(",{0}", HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[HgaIndex, i].ToString("F3", CultureInfo.InvariantCulture)));
                }
                sw.WriteLine();
                /*sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}", HgaIndex + 1, LED_C[HgaIndex].ToString("F3", CultureInfo.InvariantCulture),
                                                                                                                                        //test_no,                                                                                                           ));*/
            }

        }

        private void GetHalfLDUAndLEDData()
        {
            int first_5_hga = TestProbe88GetHalfLDUAndLEDData.first_5_Hga;

            if (first_5_hga == 1)
            {
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[0, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUFirstHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUSecondHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUThirdHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[3, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUFourthHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[4, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUFifthHga() / 1000000, 3);

                //LED
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[0, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[0, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[0, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[0, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDFirstHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[1, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[1, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[1, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[1, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDSecondHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[2, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[2, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[2, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[2, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDThirdHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[3, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[3, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[3, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[3, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDFourthHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[4, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[4, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[4, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[4, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDFifthHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateCalculationParameter();// to update the calculation parameter

                if (HSTMachine.Workcell.OperationMode == OperationMode.Simulation)
                {
                   // ,x,1.891,1.908,1.926,1.943,1.960,1.977
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 0] = 1.854362;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 1] = 1.872554;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 2] = 1.890627;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 3] = 1.908;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 4] = 1.926;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 5] = 1.943;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 6] = 1.960;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[1, 7] = 1.977;
                                                                                                         
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 0] = 1.858;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 1] = 1.876;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 2] = 1.894;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 3] = 1.911;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 4] = 1.928;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 5] = 1.945;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 6] = 1.962;
                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[2, 7] = 1.979;
                }
                for (int HgaIndex = 0; HgaIndex < 5; HgaIndex++)
                {
                    for (int point = 0; point < 4; point++)
                    {
                        Log.Info(this, "HGA = {0}, LED Voltage point {1}, {2}", HgaIndex + 1, point, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[HgaIndex, point].ToString("F3", CultureInfo.InstalledUICulture));
                    }
                    for (int point = 0; point < 8; point++)
                    {
                        Log.Info(this, "HGA = {0}, LDU Voltage point {1}, {2}", HgaIndex + 1, point, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, point].ToString("F3", CultureInfo.InstalledUICulture));
                    }
                    Log.Info(this, "");
                }
            }

            else
            {
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[5, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUFirstHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[6, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUSecondHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[7, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUThirdHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[8, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUFourthHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 4] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point4LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 5] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point5LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 6] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point6LDUFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[9, 7] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point7LDUFifthHga() / 1000000, 3);

                //LED
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[5, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[5, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[5, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDFirstHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[5, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDFirstHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[6, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[6, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[6, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDSecondHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[6, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDSecondHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[7, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[7, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[7, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDThirdHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[7, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDThirdHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[8, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[8, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[8, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDFourthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[8, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDFourthHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[9, 0] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point0LEDFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[9, 1] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point1LEDFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[9, 2] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point2LEDFifthHga() / 1000000, 3);
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[9, 3] = Math.Round((double)TestProbe88GetHalfLDUAndLEDData.Point3LEDFifthHga() / 1000000, 3);

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateCalculationParameter();// to update the calculation parameter
               
                for (int HgaIndex = 5; HgaIndex < 10; HgaIndex++)
                {
                    for (int point = 0; point < 4; point++)
                    {
                        Log.Info(this, "HGA = {0}, LED Voltage point {1}, {2}", HgaIndex + 1, point, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LEDVoltagePoint[HgaIndex, point].ToString("F3", CultureInfo.InstalledUICulture));
                    }
                    for (int point = 0; point < 8; point++)
                    {
                        Log.Info(this, "HGA = {0}, LDU Voltage point {1}, {2}", HgaIndex + 1, point, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUVoltagePoint[HgaIndex, point].ToString("F3", CultureInfo.InstalledUICulture));
                    }
                    Log.Info(this, "");
                    
                }
            }
        }

        private void GetHalfPhotoDiodeData()
        {
            int first_5_hga = TestProbe89GetHalfPhotoDiodeData.first_5_Hga;

            if (first_5_hga == 1)
            {

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[0, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFirstHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[1, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUSecondHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[2, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUThirdHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[3, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFourthHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[4, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFifthHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[0, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFirstHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[1, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUSecondHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[2, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUThirdHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[3, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFourthHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[4, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFifthHga() / 1000000;
            }
            else
            {

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[5, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFirstHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[6, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUSecondHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[7, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUThirdHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[8, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFourthHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[9, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFifthHga() / 1000000;

                // Gompertz
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFirstHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[5, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFirstHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUSecondHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[6, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUSecondHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUThirdHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[7, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUThirdHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFourthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[8, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFourthHga() / 1000000;

                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 0] = (double)TestProbe89GetHalfPhotoDiodeData.Point0LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 1] = (double)TestProbe89GetHalfPhotoDiodeData.Point1LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 2] = (double)TestProbe89GetHalfPhotoDiodeData.Point2LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 3] = (double)TestProbe89GetHalfPhotoDiodeData.Point3LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 4] = (double)TestProbe89GetHalfPhotoDiodeData.Point4LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 5] = (double)TestProbe89GetHalfPhotoDiodeData.Point5LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 6] = (double)TestProbe89GetHalfPhotoDiodeData.Point6LDUFifthHga() / 1000000;
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().InputVpd[9, 7] = (double)TestProbe89GetHalfPhotoDiodeData.Point7LDUFifthHga() / 1000000;

            }

            if (first_5_hga != 1)
            {
                for (int HgaIndex = 0; HgaIndex < 10; HgaIndex++)
                {
                    for (int point = 0; point < 8; point++)
                    {
                        Log.Info(this, "HGA = {0}, Photodiode Voltage point {1}, {2}", HgaIndex + 1, point, HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUPhotodiodeVoltagePoint[HgaIndex, point].ToString("F3", CultureInfo.InstalledUICulture));
                    }




                    HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().UpdateData(HgaIndex);
                 //   Log.Info(this, "LDU PASS/FAIL -- HgaIndex: {0}, LDU_IThreshold: {1}, LDU_Resistance: {2}, LED_Inercept: {3}, LDUMAxVPD: {4}", HgaIndex,
                 //       HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_IThreshold[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                 //       (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDU_M[HgaIndex] * 1000).ToString("F5", CultureInfo.InvariantCulture),
                 //       HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LED_C[HgaIndex].ToString("F5", CultureInfo.InvariantCulture),
                 //       HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[HgaIndex].ToString("F5", CultureInfo.InvariantCulture));

                }
            }

        }

        private string checkSTFamilyAndCRDL(List<string> errlist)
        {
            string result = "";
            bool isSTFamily = false;
            bool isCRDL = false;
            var highestPriortyErrorCode = "";
            highestPriortyErrorCode = GetErrorCodePriority(errlist);
            if (highestPriortyErrorCode == ERROR_MESSAGE_CODE.CRDL.ToString())
            {
                foreach (var item in errlist)
                {
                    if (item.Contains("ST")) isSTFamily = true;
                    if (item.Contains("CRDL")) isCRDL = true;

                }
                if (isSTFamily && isCRDL && CommonFunctions.Instance.IsRunningWithNewTSR && CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec != 40)
                {
                    result = ERROR_MESSAGE_CODE.STFAMILY.ToString();
                }
            }
            else
            {
                return "";
            }

            return result;
        }


        private void GompertzCalculation()
        {
            int LDUStepSize = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUStepSize;
            int LDUStartCurrent = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUStartCurrent;
            int LDUStopCurrent = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUStopCurrent;
            Log.Info(this, "Method: GompertzCalculation Thread Start");
            //HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().Initialize_Gompertz_Variables();//Initialize_All_Variable(0, true);
            if (HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
            {
                //Gompertz
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().SetCurrent(LDUStartCurrent, LDUStepSize, LDUStopCurrent);              
                HSTMachine.Workcell.getPanelRecipe().GetGomperztCalculation().StartCalculate();
            }
            Log.Info(this, "Method: GompertzCalculation Thread End");
        }
    }
}