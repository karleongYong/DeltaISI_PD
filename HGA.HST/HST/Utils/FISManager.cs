using System;
using System.Collections.Generic;
using System.Text;
using FisApi;
using System.Threading;
using Seagate.AAS.Parsel;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Machine;
using XyratexOSC.Logging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.VisualBasic;
using System.IO;

namespace Seagate.AAS.HGA.HST.Utils
{
    public class FISManager : IDisposable
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        private static FISManager _instance = null;
        private readonly object _lockObject = new object();
        private const int TIC_EQUIP_ID = 57;
        FisApiServer _server = null;

        //private int _dictionaryRev = 0;
        private bool _disposed = false;
        private bool _GPSStarted = false;
        private bool _isConnection = true;
        private HSTWorkcell _workcell;        

        // Constructors & Finalizers -------------------------------------------
        protected FISManager()
        {
            _instance = this;
            _workcell = HSTMachine.Workcell;            
        }

        public static FISManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FISManager();                  
                }
                return _instance;
            }

            set { _instance = value; }
        }

        public bool IsFISConnected {get { return _isConnection; }
            private set { ; }
        }
        ~FISManager()
        {
            Dispose(false);
        }

        // Properties ----------------------------------------------------------

        // Methods -------------------------------------------------------------

        public void Launch()
        {
            LaunchServer();
        }


        private void LaunchServer()
        {
            if (_server == null)
                _server = FisApiServer.Instance();
            try
            {
                _server.EquipmentType = "ZHS"; // equipmenttype = ZCU
                _server.EquipmentId = "001"; // From Setup page /Equipemnt Name
                _server.LocationId = ""; // From Setup page/Line Location
                _server.UnitType = FisApiServer.UNIT_HGA;
                _server.EmployeeNumber = ""; // Optional, input before start
            }
            catch (FisException)
            {
                _server = null;

                String str = "Can not launch FISGetPutServer can not be started.  No data will be sent to SeaTrack.";
                str += "  Check that it is installed correctly.";
                throw new ParselException(str);
            }
        }

        public void TestConnection()
        {
            bool connection = false;
            try
            {
                if (_server != null)
                {
                    _server.TestConnectionToFisAgent();
                    connection = true;
                }
            }
            catch (FisException fe)
            {
            }
            _isConnection = connection;
        }

        public void StartGetPutServer()
        {
            var l = System.Diagnostics.Process.GetProcessesByName("FisGetPutServer").Length;
            if (System.Diagnostics.Process.GetProcessesByName("FisGetPutServer").Length == 0)
            {
                // Try to start getput server
                string fileName = @"C:\FisGetputServer\FisGetPutServer.exe";
                string batfile = @"C:\Seagate\HGA.HST\Setup\rungetput.bat";
                if (System.IO.File.Exists(fileName))
                {
                    if(!System.IO.File.Exists(batfile))
                        CreateBatFile();
                    System.Diagnostics.Process proc = null;
                    string targetDir = string.Format(@"C:\Seagate\HGA.HST\Setup\");   //this is where mybatch.bat lies
                    proc = new System.Diagnostics.Process();
                    proc.StartInfo.WorkingDirectory = targetDir;
                    proc.StartInfo.FileName = "rungetput.bat";
                    proc.StartInfo.Arguments = string.Format("10");  //this is argument
                    proc.StartInfo.CreateNoWindow = false;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;  //this is for hiding the cmd window...so execution will happen in back ground.
                    proc.Start();
                    proc.WaitForExit();

                }
                else throw new Exception("Couldn't start FIS Getput Server");
            }
        }

        public void SendSeatrackLoadData(SeatrackLoadData loadData)//this will be send 10 times
        {

             if (_server == null)
                throw new ParselException("SeaTrack error:  FISGetPutServer has not been started.");

            FisSession ses = _server.FisSessions();                       
            try
            {
                #region Assign FisSession Data.
                // Event   
                if (String.IsNullOrEmpty(loadData.SerialNo) == true)
                {
                    loadData.SerialNo = "NULL";
                }

                ses.Start(loadData.SerialNo, loadData.EquipmentID.Remove(3));
                ses.StartTime = DateTime.Now;           
                ses.EventStatus = loadData.Status;
                ses.LocationId = loadData.Location;
                ses.EquipmentId = loadData.EquipmentID;
                ses.EquipmentType = loadData.EquipmentType;
                ses.EmployeeNumber = loadData.LoginUser;
                ses.PartNumber = loadData.HGAPartNumber;
                //ses.PartNumber = loadData.FSAPartNumber;
                ses.Attribute("CARRIER_ID", loadData.CarrierID);
                ses.Attribute("CARRIER_SLOT", loadData.CarrierSlot);
                ses.Attribute("WORK_ORDER", loadData.WorkOrder);
                ses.Attribute("WORK_ORDER_VER", loadData.WorkOrderVersion);
                ses.Attribute("SOFT_REV", _workcell.Version);
                ses.Attribute("SETUP_FILE", loadData.SetupFile);
                ses.Attribute("STATUS_SW", loadData.SoftwareStatus.ToString());
                if(loadData.ErrorMessageCode != "" && loadData.ErrorMessageCode != string.Empty)
                    ses.Attribute("ERROR_MSG_CODE", loadData.ErrorMessageCode);
                ses.Attribute("OPERATION_MODE", loadData.OperationMode.ToString());
                ses.Attribute("SHORT_TEST_POS", loadData.ShortTestPosition);
                ses.Attribute("TEMP_BOARD", loadData.TemperatureBoard.ToString());

                ses.Attribute("STATUS_CODE", loadData.StatusCode.ToString());
                ses.Attribute("HST_SDET_STATUS", loadData.SDET_TST_STATUS.ToString());
                ses.Attribute("UTH_EQUIP_ID", loadData.TicEquipID.ToString());
                ses.Attribute("UTH_EVENT_DATE", loadData.TicTime);
                ses.Attribute("UTH_DOCKSIDE", loadData.TicDockSide);
                ses.Attribute("TGA_PART_NUM", loadData.FSAPartNumber.ToString());
                ses.StartParametricCollection("ZHS_PARAM", FisSession.PARAM_COL_SCALAR);
                ses.Parametric("ANC_YIELD", loadData.anc_yield);
                ses.Parametric("ANC_HGA_COUNT", loadData.anc_hga_count);
                ses.Parametric("DELTA_POLARITY_R2", loadData.DeltaISI_R2_SDET_Tolerance);
                ses.Parametric("DELTA_POLARITY_R1", loadData.DeltaISI_R1_SDET_Tolerance);

                if(!string.IsNullOrEmpty(loadData.ReaderResistance))
                    ses.Parametric("READER1_RES", loadData.ReaderResistance);

                if (!string.IsNullOrEmpty(loadData.Reader2Resistance))
                    ses.Parametric("READER2_RES", loadData.Reader2Resistance);

                if (!string.IsNullOrEmpty(loadData.WriterResistance))
                    ses.Parametric("WRITER_RES", loadData.WriterResistance);

                if (!string.IsNullOrEmpty(loadData.RHeaterResistance))
                    ses.Parametric("RHEATER_RES", loadData.RHeaterResistance);

                if (!string.IsNullOrEmpty(loadData.WHeaterResistance))
                    ses.Parametric("WHEATER_RES", loadData.WHeaterResistance);

                if (!string.IsNullOrEmpty(loadData.TAResistance))
                    ses.Parametric("TA_RES", loadData.TAResistance);

                ses.Parametric("SHORT_TEST", loadData.ShortTest);
                ses.Parametric("UACT_CAPA_1", loadData.UACTCapacitance1);
                ses.Parametric("UACT_CAPA_2", loadData.UACTCapacitance2);
                ses.Parametric("BIAS_VOL_MEAS", loadData.BiasVoltageMeasure);
                ses.Parametric("BIAS_CUR_SOUR", loadData.BiasCurrentSource);
                ses.Parametric("WRITER_RES_SPEC_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin);
                ses.Parametric("WRITER_RES_SPEC_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax);
                ses.Parametric("TA_RES_SPEC_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin);
                ses.Parametric("TA_RES_SPEC_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax);
                ses.Parametric("WHEATER_RES_SPEC_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin);
                ses.Parametric("WHEATER_RES_SPEC_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax);
                ses.Parametric("RHEATER_RES_SPEC_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin);
                ses.Parametric("RHEATER_RES_SPEC_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax);
                ses.Parametric("READER_RES_SPEC_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin);
                ses.Parametric("READER_RES_SPEC_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax);
                ses.Parametric("CAPA_SPEC_CH1_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch1CapacitanceMin);
                ses.Parametric("CAPA_SPEC_CH1_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch1CapacitanceMax);
                ses.Parametric("CAPA_SPEC_CH2_MIN", CommonFunctions.Instance.MeasurementTestRecipe.Ch2CapacitanceMin);
                ses.Parametric("CAPA_SPEC_CH2_MAX", CommonFunctions.Instance.MeasurementTestRecipe.Ch2CapacitanceMax);
                ses.Parametric("X_AXIS", loadData.XAxis);
                ses.Parametric("Y_AXIS", loadData.YAxis);
                ses.Parametric("Z_AXIS", loadData.ZAxis);
                ses.Parametric("UPH", loadData.UPH);
                ses.Parametric("ISI_READER1", loadData.ISI_Reader1);
                ses.Parametric("ISI_READER2", CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable == false?loadData.ISI_Reader2:0);
                ses.Parametric("DELTA_R1", loadData.Delta_R1);
                ses.Parametric("DELTA_R2", CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable == false?loadData.Delta_R2:0);

                //For HAMR *-------------------------------
                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                {
                    ses.Parametric("LDU_RES", loadData.LDU_Res);
                    ses.Parametric("LDU_RES_SPEC_MIN", loadData.LDU_Res_Spec_Min);
                    ses.Parametric("LDU_RES_SPEC_MAX", loadData.LDU_Res_Spec_Max);
                    ses.Parametric("LDU_THRESHOLD_MA", loadData.LDU_Threshold_Ma);
                    ses.Parametric("LDU_THRESHOLD_SPEC_MIN", loadData.LDU_Threshold_Spec_Min);
                    ses.Parametric("LDU_THRESHOLD_SPEC_MAX", loadData.LDU_Threshold_Spec_Max);
                    ses.Parametric("LDU_VOLT_INTERCEPT", loadData.LDU_Volt_Intercept);
                    ses.Parametric("LDU_SWEEP_SPEC_MIN", loadData.LDU_Sweep_Spec_Min);
                    ses.Parametric("LDU_SWEEP_SPEC_MAX", loadData.LDU_Sweep_Spec_Max);
                    ses.Parametric("LED_INTERCEPT", loadData.Led_Intercept);
                    ses.Parametric("LED_INTERCEPT_SPEC", loadData.Led_Intercept_Spec);
                    ses.Parametric("LED_INTERCEPT_SPEC_MAX", loadData.Led_Intercept_Spec_Max);
                    ses.Parametric("PD_VOLTAGE", loadData.Pd_Voltage);
                    ses.Parametric("PD_VOLTAGE_SPEC_MIN", loadData.Pd_Voltage_Spec_Min);
                    ses.Parametric("PD_VOLTAGE_SPEC_MAX", loadData.Pd_Voltage_Spec_Max);
                    ses.Parametric("SDET_ITHRESHOLD", loadData.Sdet_iThreshold);
                    ses.Parametric("DELTA_ITHRESHOLD", loadData.Delta_iThreshold);
                }
                //For HAMR *-------------------------------

                ses.Parametric("GET_BIAS_CH1", loadData.Get_Bias_Ch1);
                ses.Parametric("GET_BIAS_CH2", loadData.Get_Bias_Ch2);
                ses.Parametric("GET_BIAS_CH3", loadData.Get_Bias_Ch3);
                ses.Parametric("GET_BIAS_CH4", loadData.Get_Bias_Ch4);
                ses.Parametric("GET_BIAS_CH5", loadData.Get_Bias_Ch5);
                ses.Parametric("GET_BIAS_CH6", loadData.Get_Bias_Ch6);
                ses.Parametric("GET_SENSING_CH1", loadData.Get_Sensing_Ch1);
                ses.Parametric("GET_SENSING_CH2", loadData.Get_Sensing_Ch2);
                ses.Parametric("GET_SENSING_CH3", loadData.Get_Sensing_Ch3);
                ses.Parametric("GET_SENSING_CH4", loadData.Get_Sensing_Ch4);
                ses.Parametric("GET_SENSING_CH5", loadData.Get_Sensing_Ch5);
                ses.Parametric("GET_SENSING_CH6", loadData.Get_Sensing_Ch6);
                ses.Parametric("VOLT_RATIO_CH1", loadData.Volt_Ratio_Ch1);
                ses.Parametric("VOLT_RATIO_CH2", loadData.Volt_Ratio_Ch2);
                ses.Parametric("VOLT_RATIO_CH3", loadData.Volt_Ratio_Ch3);
                ses.Parametric("VOLT_RATIO_CH4", loadData.Volt_Ratio_Ch4);
                ses.Parametric("VOLT_RATIO_CH5", loadData.Volt_Ratio_Ch5);
                ses.Parametric("VOLT_RATIO_CH6", loadData.Volt_Ratio_Ch6);
                ses.Parametric("VOLTAGE_DELTA1", loadData.Voltage_Delta1);
                ses.Parametric("VOLTAGE_DELTA2", loadData.Voltage_Delta2);
                ses.Parametric("IBS_CHECK", loadData.IBSCheck.ToString());
                if(!string.IsNullOrEmpty(loadData.RDIbsPattern))
                    ses.Parametric("HST_RD_IBS_PATTERN", loadData.RDIbsPattern.ToString());
                if(!string.IsNullOrEmpty(loadData.WRIbsPattern))
                    ses.Parametric("HST_WR_IBS_PATTERN", loadData.WRIbsPattern.ToString());

                ses.Parametric("SDET_WRITER", loadData.Delta_Writer_SDET);
                ses.Parametric("HST_SDET_DELTA_WRITER", loadData.Delta_Writer_HST_SDET);
                ses.Parametric("WRBRIDGE_PCT", loadData.WRBridge_Percentage);
                ses.Parametric("WRBRIDGE_ADAP_SPEC", loadData.wrbridge_adap_spec);
                ses.Parametric("SDET_READER1", loadData.sdet_reader1);
                ses.Parametric("SDET_READER2", loadData.sdet_reader2);
                ses.Parametric("GAUSS_METER1", loadData.gauss_meter1);
                ses.Parametric("GAUSS_METER2", loadData.gauss_meter2);
                ses.StartParametricCollection("CYCLE_TIME_PARAM", FisSession.PARAM_COL_SCALAR);
                ses.Parametric("CYCLE_TIME", loadData.CycleTime);
                ses.StopParametricCollection();

                #endregion
                if (!HSTMachine.Workcell.HSTSettings.TurnOnTestRunWithoutData)
                    ses.Send();
                ses.CloseSession();

                if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        XyratexOSC.Logging.Log.Info(this, "Serial Number, {0}, Operation, {1}", loadData.SerialNo, loadData.OperationMode);
                        XyratexOSC.Logging.Log.Info(this, "Start Time, {0}", ses.StartTime);
                        XyratexOSC.Logging.Log.Info(this, "Unit Type, {0}", ses.UnitType);
                        XyratexOSC.Logging.Log.Info(this, "Event Status, {0}", loadData.Status);
                        XyratexOSC.Logging.Log.Info(this, "Location ID, {0}", loadData.Location);
                        XyratexOSC.Logging.Log.Info(this, "Equipment ID, {0}", loadData.EquipmentID);

                        XyratexOSC.Logging.Log.Info(this, "Equipment Type, {0}", loadData.EquipmentType);
                        XyratexOSC.Logging.Log.Info(this, "Login User, {0}", loadData.LoginUser);
                        XyratexOSC.Logging.Log.Info(this, "Carrier ID, {0}", loadData.CarrierID);
                        XyratexOSC.Logging.Log.Info(this, "Carrier Slot, {0}", loadData.CarrierSlot);

                        XyratexOSC.Logging.Log.Info(this, "Work Order, {0}", loadData.WorkOrder);
                        XyratexOSC.Logging.Log.Info(this, "Work Order Version, {0}", loadData.WorkOrderVersion);
                        XyratexOSC.Logging.Log.Info(this, "Setup File, {0}", loadData.SetupFile);
                        XyratexOSC.Logging.Log.Info(this, "Software Status, {0}", loadData.SoftwareStatus);

                        XyratexOSC.Logging.Log.Info(this, "Reader1 Resistance, {0}", loadData.ReaderResistance);
                        XyratexOSC.Logging.Log.Info(this, "Reader2 Resistance, {0}", loadData.Reader2Resistance);
                        XyratexOSC.Logging.Log.Info(this, "Writer Resistance, {0}", loadData.WriterResistance);
                        XyratexOSC.Logging.Log.Info(this, "rHeater Resistance, {0}", loadData.RHeaterResistance);
                        XyratexOSC.Logging.Log.Info(this, "wHeater Resistance, {0}", loadData.WHeaterResistance);
                        XyratexOSC.Logging.Log.Info(this, "TA Resistance, {0}", loadData.TAResistance);
                        XyratexOSC.Logging.Log.Info(this, "Short Test, {0}", loadData.ShortTest);
                        XyratexOSC.Logging.Log.Info(this, "Short Test Position, {0}", loadData.ShortTestPosition);
                        XyratexOSC.Logging.Log.Info(this, "UACT Capacitance1, {0}", loadData.UACTCapacitance1);
                        XyratexOSC.Logging.Log.Info(this, "UACT Capacitance2, {0}", loadData.UACTCapacitance2);
                        XyratexOSC.Logging.Log.Info(this, "Temperature Board, {0}", loadData.TemperatureBoard);
                        XyratexOSC.Logging.Log.Info(this, "Bias Voltage Measure, {0}", loadData.BiasVoltageMeasure);
                        XyratexOSC.Logging.Log.Info(this, "Bias Current Source, {0}", loadData.BiasCurrentSource);
                        XyratexOSC.Logging.Log.Info(this, "Resistance Specification, {0}", loadData.ResistanceSpecification);
                        XyratexOSC.Logging.Log.Info(this, "X Axis, {0}", loadData.XAxis);
                        XyratexOSC.Logging.Log.Info(this, "Y Axis, {0}", loadData.YAxis);
                        XyratexOSC.Logging.Log.Info(this, "Z Axis, {0}", loadData.ZAxis);
                    }
                }
            }

            catch (FisException ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new ParselException("Seatrack error:  " + ex.description());
            }
        }

        public bool GetSLDR_PARAM_BIN_Data(SLDR_PARAM_BIN_DATA sldr_param_bin, string serialNumber)
        {
            lock (_lockObject)
            {
                bool returnGet = false;
                int autoretry = 2;

                while (autoretry > 0)
                {
                    try
                    {
                        FisSession ses = _server.FisSessions();
                        ses.UnitType = FisApiServer.UNIT_DRIVE;

                        if (serialNumber == string.Empty)
                            serialNumber = "";

                        ses.Request(FisSession.REQUEST_HGA_COHRES, "ATTR", serialNumber, "HEDNUM");
                        ses.SendRequests();

                        RecordsetCollection rsc = ses.RecordsetCollections();

                        if (rsc.RecordsetCount() > 0)
                        {
                            Recordset rec = rsc.Recordsets("ATTR");

                            if (rec.RecordCount() > 0)
                            {
                                autoretry = 0;

                                sldr_param_bin.DATE_TIME = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.DATE_TIME).Value;
                                sldr_param_bin.SLDR_LOT_ID = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_LOT_ID).Value;
                                sldr_param_bin.SLDR_MTF_NUM = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_MTF_NUM).Value;
                                sldr_param_bin.SLDR_BIN = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_BIN).Value;
                                sldr_param_bin.SLDR_DATA_SOURCE = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_DATA_SOURCE).Value;
                                sldr_param_bin.SLDR_F1 = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_F1).Value;
                                sldr_param_bin.SLDR_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_RES).Value;
                                sldr_param_bin.SLDR_ASYM = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_ASYM).Value;
                                sldr_param_bin.SLDR_ADC = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_ADC).Value;
                                sldr_param_bin.SLDR_BP = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_BP).Value;
                                sldr_param_bin.SLDR_WPE = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_WPE).Value;
                                sldr_param_bin.PRB_TP_WDTH = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.PRB_TP_WDTH).Value;
                                sldr_param_bin.SLDR_PART_NUM = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_PART_NUM).Value;
                                sldr_param_bin.SLDR_TAB = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_TAB).Value;
                                sldr_param_bin.SLDR_AKL_BP = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_AKL_BP).Value;
                                sldr_param_bin.SLDR_TAD_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_TAD_RES).Value;
                                sldr_param_bin.SLDR_RES_RD2 = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_RES_RD2).Value;
                                sldr_param_bin.ET_DISPOSITION = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_DISPOSITION).Value;
                                sldr_param_bin.TST_STATUS = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_TST_STATUS).Value;
                                sldr_param_bin.WAF_RDR_HTR_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.WAF_RDR_HTR_RES).Value;
                                sldr_param_bin.WAF_TAD_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.WAF_TAD_RES).Value;
                                sldr_param_bin.WAF_WTR_HTR_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.WAF_WTR_HTR_RES).Value;
                                sldr_param_bin.WAF_WTR_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.WAF_WTR_RES).Value;
                                sldr_param_bin.ISI_AMP_AT_ET = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_F1).Value;
                                sldr_param_bin.ISI_ASYM_AT_ET = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_ASYM).Value;
                                sldr_param_bin.ISI_RES_AT_ET = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_RES).Value;
                                sldr_param_bin.ET_WRT1_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_WRT1_RES).Value;
                                sldr_param_bin.ET_WRT2_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_WRT2_RES).Value;
                                sldr_param_bin.ET_RD1_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_RD1_RES).Value;
                                sldr_param_bin.ET_RD2_RES = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_RD2_RES).Value;
                                sldr_param_bin.SLDR_F1_RD2 = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_F1_RD2).Value;
                                sldr_param_bin.SLDR_ASYM_RD2 = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SLDR_ASYM_RD2).Value;
                                sldr_param_bin.ISI_RD_IBS_PATTERN = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.RD_IBS_PATTERN).Value;
                                sldr_param_bin.ISI_WR_IBS_PATTERN = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.WR_IBS_PATTERN).Value;
                                sldr_param_bin.ET_LAS_THRESHOLD = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.ET_LAS_THRESHOLD).Value;
                                if (sldr_param_bin.ISI_RD_IBS_PATTERN == "?") sldr_param_bin.ISI_RD_IBS_PATTERN = "A";
                                if (sldr_param_bin.ISI_WR_IBS_PATTERN == "?") sldr_param_bin.ISI_WR_IBS_PATTERN = "1";
                                if(string.IsNullOrEmpty(sldr_param_bin.ET_RD1_RES)) sldr_param_bin.ET_RD1_RES = "0";
                                if(string.IsNullOrEmpty(sldr_param_bin.ET_RD2_RES)) sldr_param_bin.ET_RD2_RES = "0";
                                var testget = rec.Fields((int)SLDR_PARAM_BIN_DATA.SLDR_PARAM_BIN.SETS_PRFL_RECORD).Value;
                                CheckSLDRDataFormater(sldr_param_bin);

                                if (sldr_param_bin.TST_STATUS == string.Empty)
                                    Log.Info("Get sort number error SN={0}", serialNumber);
                                returnGet = true;
                            }
                            else
                            {
                                autoretry--;
                                Thread.Sleep(300);
                            }
                        }
                        else
                        {
                            autoretry--;
                            Thread.Sleep(300);
                        }
                        ses.CloseSession();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                return returnGet;
            }
        }

        public TIC_BIN_DATA GetUTICMachineNumber(string Sn)
        {
            //"TIC_EQUIP_ID" = 58
            //"TIC_EVENT_DATE" = 59
            TIC_BIN_DATA ticdata = new TIC_BIN_DATA();

            lock (_lockObject)
            {
                string returnMcNo = string.Empty;
                int autoretry = 1;

                while (autoretry > 0)
                {
                    try
                    {
                        FisSession ses = _server.FisSessions();
                        ses.UnitType = FisApiServer.UNIT_HGA;

                        if (Sn == string.Empty)
                            Sn = "";

                        Recordset rec = ses.RequestRecordset(FisSession.REQUEST_ATTRIBUTE, Sn, "NA");
                        if(rec.RecordCount() > 0)
                        {
                            bool end = false;
                            bool isEquipfound = false;
                            bool isEventDataFound = false;
                            bool isDockSideDataFound = false;

                            rec.MoveTo(15);

                            while (!rec.Eof() && !end)
                            {
                                    if (rec.Fields(rec.GetColumnName(0)).Value.ToUpper().Equals(TIC_BIN_DATA.TIC_PARAM_BIN.TIC_EQUIP_ID.ToString()))
                                    {
                                        ticdata.EQUIP_ID = rec.Fields(rec.GetColumnName(1)).Value;
                                        isEquipfound = true;
                                    }
                                    else if (rec.Fields(rec.GetColumnName(0)).Value.ToUpper().Equals(TIC_BIN_DATA.TIC_PARAM_BIN.TIC_EVENT_DATE.ToString()))
                                    {
                                        ticdata.EVENT_DATE = rec.Fields(rec.GetColumnName(1)).Value;
                                        isEventDataFound = true;
                                    }
                                    else if (rec.Fields(rec.GetColumnName(0)).Value.ToUpper().Equals(TIC_BIN_DATA.TIC_PARAM_BIN.DOCKSIDE.ToString()))
                                    {
                                        ticdata.DOCK_SIDE = rec.Fields(rec.GetColumnName(1)).Value;
                                        isDockSideDataFound = true;
                                        rec.MoveTo(50);
                                    }
                                if (isEquipfound && isEventDataFound && isDockSideDataFound)
                                    {
                                        rec.MoveLast();
                                        rec.Eof();
                                        end = true;
                                        autoretry = 0;
                                    }
                                    else
                                        rec.MoveNext();
                                    Thread.Sleep(10);
                            }
                        }
                        ses.CloseSession();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    autoretry--;
                }
                return ticdata;
            }
        }

        private void CheckSLDRDataFormater(SLDR_PARAM_BIN_DATA sldr_param_bin)
        {
            if (sldr_param_bin.SLDR_F1 == string.Empty) sldr_param_bin.SLDR_F1 = "0";
            if (sldr_param_bin.SLDR_RES == string.Empty) sldr_param_bin.SLDR_RES = "0";
            if (sldr_param_bin.SLDR_ASYM == string.Empty) sldr_param_bin.SLDR_ASYM = "0";
            if (sldr_param_bin.SLDR_ADC == string.Empty) sldr_param_bin.SLDR_ADC = "0";
            if (sldr_param_bin.SLDR_BP == string.Empty) sldr_param_bin.SLDR_BP = "0";
            if (sldr_param_bin.SLDR_WPE == string.Empty) sldr_param_bin.SLDR_WPE = "0";
            if (sldr_param_bin.SLDR_AKL_BP == string.Empty) sldr_param_bin.SLDR_AKL_BP = "0";
            if (sldr_param_bin.SLDR_TAD_RES == string.Empty) sldr_param_bin.SLDR_TAD_RES = "0";
            if (sldr_param_bin.SLDR_RES_RD2 == string.Empty) sldr_param_bin.SLDR_RES_RD2 = "0";
            if (sldr_param_bin.TST_STATUS == string.Empty) sldr_param_bin.TST_STATUS = "0";
            if (sldr_param_bin.WAF_RDR_HTR_RES == string.Empty) sldr_param_bin.WAF_RDR_HTR_RES = "0";
            if (sldr_param_bin.WAF_TAD_RES == string.Empty) sldr_param_bin.WAF_TAD_RES = "0";
            if (sldr_param_bin.WAF_WTR_HTR_RES == string.Empty) sldr_param_bin.WAF_WTR_HTR_RES = "0";
            if (sldr_param_bin.WAF_WTR_RES == string.Empty) sldr_param_bin.WAF_WTR_RES = "0";
            if (sldr_param_bin.ISI_AMP_AT_ET == string.Empty) sldr_param_bin.ISI_AMP_AT_ET = "0";
            if (sldr_param_bin.ISI_ASYM_AT_ET == string.Empty) sldr_param_bin.ISI_ASYM_AT_ET = "0";
            if (sldr_param_bin.ISI_RES_AT_ET == string.Empty) sldr_param_bin.ISI_RES_AT_ET = "0";
            if (sldr_param_bin.ET_WRT1_RES == string.Empty) sldr_param_bin.ET_WRT1_RES = "0";
            if (sldr_param_bin.SLDR_F1_RD2 == string.Empty) sldr_param_bin.SLDR_F1_RD2 = "0";
            if (sldr_param_bin.SLDR_ASYM_RD2 == string.Empty) sldr_param_bin.SLDR_ASYM_RD2 = "0";
            if (sldr_param_bin.ET_WRT1_RES == string.Empty) sldr_param_bin.ET_WRT1_RES = "0";
        }

        public void Shutdown()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    //_service.Stop();
                    if (_server != null)
                        _server.ShutDownFisApiServer();

                    _server = null;
                }
                catch (FisException)
                {
                }

                _disposed = true;
            }
        }

        private void StartService()
        {
            if (_GPSStarted)
                return;

            try
            {
                _GPSStarted = true;
            }

            catch (FisException)
            {
                String str = "FIS GPS Service can not be started.  No data will be sent to Seatrack.";
                str += "  Check that it is installed and you have permission to use the service.";
                throw new ParselException(str);
            }
        }

        private void CreateBatFile()
        {
            string path = @"C:\Seagate\HGA.HST\Setup\";
            using (StreamWriter writetext = new StreamWriter(path + "rungetput.bat"))
            {
                writetext.WriteLine("start /d " + '"' + @"C:\FISGetPutServer\" + '"' + " FISGetPutServer.exe");
            }
        }
    }
}
