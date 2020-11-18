using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.HGA.HST.Recipe;
using System.Threading;
using FisApi;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.Utils;
using System.IO;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class TDFDataRunController : ControllerHST
    {
        private OutputStationController _outpuController;
        private SeatrackLoadData _loadData = new SeatrackLoadData();
        private Models.DataLog _datalog = new Models.DataLog();
        private readonly object _lockObject = new object();
        // Constructors ------------------------------------------------------------
        public TDFDataRunController(HSTWorkcell workcell, string controllerID, string controllerName)
            : base(workcell, controllerID, controllerName)
        {
            _workcell = workcell;
            this._ioManifest = (HSTIOManifest)workcell.IOManifest;
        }

        public override void InitializeController()
        {
            _outpuController = _workcell.Process.OutputStationProcess.Controller;
        }

        public void PrepareCommonSeatrackLoadData(FolaTagData folaTagData, Carrier outputCarrier)
        {
            _loadData.Clear();
            _loadData.EventDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
            _loadData.Location = HSTMachine.Workcell.HSTSettings.Install.LocationID;
            _loadData.EquipmentID = HSTMachine.Workcell.HSTSettings.Install.EquipmentID;
            _loadData.EquipmentType = HSTMachine.Workcell.HSTSettings.EquipmentType;
            _loadData.LoginUser = HSTMachine.Workcell.HSTSettings.OperatorGID;
            _loadData.CarrierID = folaTagData.CarrierID;
            _loadData.WorkOrder = folaTagData.WorkOrder; 
            _loadData.WorkOrderVersion = folaTagData.WorkOrderVersion.ToString();            
            _loadData.SetupFile = HSTMachine.Workcell.WorkOrder.Loading.LoadingProgramName;
            _loadData.SoftwareStatus = 1; 
            _loadData.OperationMode = (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Auto) ? 0 : 1;    // Operation Mode
            _loadData.XAxis = outputCarrier.getPrecisorNestPositionX();
            _loadData.YAxis = outputCarrier.getPrecisorNestPositionY();
            _loadData.ZAxis = outputCarrier.getPrecisorNestPositionZ();
            _loadData.ThetaAxis = outputCarrier.getPrecisorNestPositionTheta();
            _loadData.UPH = HSTMachine.Workcell.LoadCounter.UPH;
            _loadData.CycleTime = HSTMachine.Workcell.LoadCounter.CycleTime;
            _loadData.HGAPartNumber = outputCarrier.WorkOrderData.HgaPartNo;
            _loadData.FSAPartNumber = outputCarrier.WorkOrderData.FsaPartNo;
       
        }

        public void SendLoadDataToSeaTrack(FolaTagData folaTagData, Carrier outputCarrier)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true))
            {
                return;
            }

            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        XyratexOSC.Logging.Log.Info(this, "Seatrack EventDate, {0}", _loadData.EventDate);
                        XyratexOSC.Logging.Log.Info(this, "Location ID, {0}", _loadData.Location);
                        XyratexOSC.Logging.Log.Info(this, "Equipment ID, {0}", _loadData.EquipmentID);
                        XyratexOSC.Logging.Log.Info(this, "Equipment Type, {0}", _loadData.EquipmentType);
                        XyratexOSC.Logging.Log.Info(this, "Login User, {0}", _loadData.LoginUser);
                        XyratexOSC.Logging.Log.Info(this, "Carrier ID, {0}", _loadData.CarrierID);
                        XyratexOSC.Logging.Log.Info(this, "Work Order Number, {0}", _loadData.WorkOrder);
                        XyratexOSC.Logging.Log.Info(this, "Work Order Version, {0}", _loadData.WorkOrderVersion);
                        XyratexOSC.Logging.Log.Info(this, "Setup File, {0}", _loadData.SetupFile);
                        XyratexOSC.Logging.Log.Info(this, "X Axis, {0}", _loadData.XAxis);
                        XyratexOSC.Logging.Log.Info(this, "Y Axis, {0}", _loadData.YAxis);
                        XyratexOSC.Logging.Log.Info(this, "Z Axis, {0}", _loadData.ThetaAxis);
                        XyratexOSC.Logging.Log.Info(this, "Software Status, {0}", _loadData.SoftwareStatus);
                        XyratexOSC.Logging.Log.Info(this, "Operation Mode, {0}", _loadData.OperationMode);
                        XyratexOSC.Logging.Log.Info(this, "Status, {0}", _loadData.Status);
                        XyratexOSC.Logging.Log.Info(this, "Temperature Board, {0}", _loadData.TemperatureBoard);
                        XyratexOSC.Logging.Log.Info(this, "Bias Current Source, {0}", _loadData.BiasCurrentSource);
                        XyratexOSC.Logging.Log.Info(this, "Resistance Specification, {0}", _loadData.ResistanceSpecification);
                    }
                }

                bool[] UntestedHGA = new bool[10];
                UntestedHGA[0] = ((outputCarrier.Hga1.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga1.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[1] = ((outputCarrier.Hga2.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga2.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[2] = ((outputCarrier.Hga3.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga3.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[3] = ((outputCarrier.Hga4.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga4.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[4] = ((outputCarrier.Hga5.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga5.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[5] = ((outputCarrier.Hga6.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga6.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[6] = ((outputCarrier.Hga7.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga7.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[7] = ((outputCarrier.Hga8.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga8.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[8] = ((outputCarrier.Hga9.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga9.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[9] = ((outputCarrier.Hga10.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga10.Hga_Status == HGAStatus.TestedFail)) ? false : true;

                for (int i = 0; i < folaTagData.CarrierSize; i++)
                {
                    bool sendHGADataToSeatrack = true;

                    if (String.IsNullOrEmpty(folaTagData.HGAData[i].HgaSN) == true ||
                        (folaTagData.HGAData[i].HgaSN) == "\0")
                    {
                        _loadData.SerialNo = "NULL";
                        sendHGADataToSeatrack = false;
                    }
                    else
                    {
                        _loadData.SerialNo = folaTagData.HGAData[i].HgaSN;
                    }

                    _loadData.Status = (GetHGAPartStatus(i, outputCarrier)) ? FisSession.EVENT_PASS : FisSession.EVENT_FAIL;
                    _loadData.HGASerialNumber = folaTagData.HGAData[i].HgaSN;
                    _loadData.StatusCode = folaTagData.HGAData[i].Status.ToString();
                    _loadData.IBSCheck = outputCarrier.WorkOrderData.IBSCheck.ToString().ToUpper();
                    _loadData.CarrierSlot = (i + 1).ToString();
                    string ErrorMessageCode = "";

                    if (UntestedHGA[i])
                    {
                        _loadData.ReaderResistance = "";
                        _loadData.WriterResistance = "";
                        _loadData.RHeaterResistance = "";
                        _loadData.WHeaterResistance = "";
                        _loadData.TAResistance = "";
                        _loadData.ShortTest = 0;
                        _loadData.ShortTestPosition = "0";
                        _loadData.UACTCapacitance1 = 0;
                        _loadData.UACTCapacitance2 = 0;
                        _loadData.BiasVoltageMeasure = 0;
                        _loadData.TemperatureBoard = "";
                        _loadData.BiasCurrentSource = 0;
                        _loadData.ResistanceSpecification = "";
                        _loadData.ErrorMessageCode = "";
                        _loadData.ISI_Reader1 = 0;
                        _loadData.ISI_Reader2 = 0;
                        _loadData.Delta_R1 = 0;
                        _loadData.Delta_R2 = 0;
                        _loadData.LDU_Res = 0;
                        _loadData.LDU_Res_Spec_Min = 0;
                        _loadData.LDU_Res_Spec_Max = 0;
                        _loadData.StatusCode = "";
                        _loadData.IBSCheck = "";
                        _loadData.RDIbsPattern = "";
                        _loadData.WRIbsPattern = "";
                        _loadData.TicEquipID = "";

                        //add 5-feb-2020
                        _loadData.sdet_writer = 0;
                        _loadData.hst_sdet_delta_writer = 0;
                        _loadData.wrbridge_pct = 0;
                        _loadData.sdet_reader1 = 0;
                        _loadData.sdet_reader2 = 0;

                        _loadData.gauss_meter1 = 0;
                        _loadData.gauss_meter2 = 0;
                    }
                    #region HGA1
                    else if (i == 0)  // HGA1
                    {

                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga1.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga1.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga1.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga1.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga1.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga1.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga1.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga1.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga1.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga1.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga1.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga1.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga1.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga1.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga1.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga1.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga1.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga1.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;     
                        _loadData.Reader2Resistance = ctq_rd_res2;     
                        _loadData.WriterResistance = ctp_wrt_res;      
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;     
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga1.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga1.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga1.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga1.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga1.getCh1Temperature() + "_" + outputCarrier.Hga1.getCh2Temperature() + "_" + outputCarrier.Hga1.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga1.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga1.getResistanceSpec();
                        _loadData.ISI_Reader1 = outputCarrier.Hga1.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga1.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga1.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga1.getDeltaISIReader2() : 0;
                        
                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga1.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga1.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga1.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga1.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga1.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga1.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga1.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga1.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga1.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga1.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga1.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga1.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga1.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga1.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga1.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga1.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga1.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga1.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga1.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga1.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga1.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga1.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga1.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga1.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch1()) / outputCarrier.Hga1.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch2()) / outputCarrier.Hga1.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch3()) / outputCarrier.Hga1.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch4()) / outputCarrier.Hga1.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch5()) / outputCarrier.Hga1.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch6()) / outputCarrier.Hga1.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga1.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga1.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga1.UTIC_DATA.EQUIP_ID;

                        _loadData.TicTime = outputCarrier.Hga1.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga1.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga1.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga1.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga1.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga1.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga1.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga1.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga1.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga1.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga1.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga1.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga1.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga1.get_DeltaISI_R2_SDET_Tolerance();

                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (outputCarrier.Hga1.TST_STATUS.Equals('\0') || outputCarrier.Hga1.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga1.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga1.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga1.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga1.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA1
                    #region HGA2
                    else if (i == 1)  // HGA2
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga2.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga2.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga2.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga2.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga2.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga2.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga2.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga2.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga2.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga2.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga2.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga2.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga2.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga2.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga2.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga2.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga2.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga2.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;                 
                                         
                        _loadData.ISI_Reader1 = outputCarrier.Hga2.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga2.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga2.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga2.getDeltaISIReader2() : 0;


                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga2.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga2.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga2.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga2.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga2.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga2.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga2.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga2.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga2.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga2.Delta_IThreshold;
                        //For HAMR *-------------------------------


                        if (outputCarrier.Hga2.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga2.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga2.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga2.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga2.getCh1Temperature() + "_" + outputCarrier.Hga2.getCh2Temperature() + "_" + outputCarrier.Hga2.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga2.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga2.getResistanceSpec();
                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga2.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga2.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga2.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga2.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga2.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga2.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga2.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga2.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga2.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga2.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga2.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga2.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga2.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga2.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch1()) / outputCarrier.Hga2.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch2()) / outputCarrier.Hga2.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch3()) / outputCarrier.Hga2.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch4()) / outputCarrier.Hga2.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch5()) / outputCarrier.Hga2.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch6()) / outputCarrier.Hga2.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga2.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga2.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga2.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga2.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga2.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga2.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga2.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga2.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga2.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga2.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga2.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga2.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga2.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga2.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga2.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga2.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga2.get_DeltaISI_R2_SDET_Tolerance();

                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (outputCarrier.Hga2.TST_STATUS.Equals('\0') || outputCarrier.Hga2.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga2.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga2.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga2.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga2.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA2
                    #region HGA3
                    else if (i == 2)  // HGA3
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga3.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga3.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga3.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga3.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga3.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga3.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga3.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga3.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga3.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga3.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga3.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga3.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga3.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga3.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga3.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga3.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga3.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga3.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga3.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga3.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga3.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga3.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga3.getCh1Temperature() + "_" + outputCarrier.Hga3.getCh2Temperature() + "_" + outputCarrier.Hga3.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga3.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga3.getResistanceSpec();
                        _loadData.ISI_Reader1 = outputCarrier.Hga3.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga3.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga3.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga3.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga3.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga3.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga3.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga3.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga3.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga3.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga3.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga3.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga3.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga3.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga3.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga3.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga3.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga3.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga3.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga3.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga3.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga3.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga3.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga3.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga3.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga3.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga3.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga3.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch1()) / outputCarrier.Hga3.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch2()) / outputCarrier.Hga3.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch3()) / outputCarrier.Hga3.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch4()) / outputCarrier.Hga3.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch5()) / outputCarrier.Hga3.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch6()) / outputCarrier.Hga3.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga3.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga3.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga3.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga3.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga3.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga3.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga3.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga3.Get_WRBridge_Percentage();
                        _loadData.wrbridge_adap_spec = outputCarrier.Hga3.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga3.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga3.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga3.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga3.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga3.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga3.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga3.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga3.get_DeltaISI_R2_SDET_Tolerance();
                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {

                            if (outputCarrier.Hga3.TST_STATUS.Equals('\0') || outputCarrier.Hga3.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga3.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga3.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga3.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga3.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA3
                    #region HGA4
                    else if (i == 3)  // HGA4
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga4.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga4.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga4.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga4.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga4.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga4.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga4.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga4.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga4.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga4.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga4.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga4.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga4.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga4.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga4.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga4.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga4.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga4.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;    
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga4.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga4.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga4.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga4.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga4.getCh1Temperature() + "_" + outputCarrier.Hga4.getCh2Temperature() + "_" + outputCarrier.Hga4.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga4.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga4.getResistanceSpec();
                        _loadData.ISI_Reader1 = outputCarrier.Hga4.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga4.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga4.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga4.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga4.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga4.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga4.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga4.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga4.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga4.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga4.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga4.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga4.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga4.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga4.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga4.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga4.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga4.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga4.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga4.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga4.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga4.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga4.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga4.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga4.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga4.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga4.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga4.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch1()) / outputCarrier.Hga4.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch2()) / outputCarrier.Hga4.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch3()) / outputCarrier.Hga4.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch4()) / outputCarrier.Hga4.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch5()) / outputCarrier.Hga4.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch6()) / outputCarrier.Hga4.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga4.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga4.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga4.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga4.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga4.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga4.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga4.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga4.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga4.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga4.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga4.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga4.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga4.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga4.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga4.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga4.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga4.get_DeltaISI_R2_SDET_Tolerance();


                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {

                            if (outputCarrier.Hga4.TST_STATUS.Equals('\0') || outputCarrier.Hga4.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga4.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga4.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga4.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga4.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA4
                    #region HGA5
                    else if (i == 4)  // HGA5
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga5.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga5.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga5.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga5.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga5.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga5.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga5.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga5.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga5.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga5.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga5.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga5.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga5.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga5.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga5.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga5.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga5.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga5.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;     
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;             


                        if (outputCarrier.Hga5.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga5.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga5.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga5.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga5.getCh1Temperature() + "_" + outputCarrier.Hga5.getCh2Temperature() + "_" + outputCarrier.Hga5.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga5.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga5.getResistanceSpec();

                        _loadData.ISI_Reader1 = outputCarrier.Hga5.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga5.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga5.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga5.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga5.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga5.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga5.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga5.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga5.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga5.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga5.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga5.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga5.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga5.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga5.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga5.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga5.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga5.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga5.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga5.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga5.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga5.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga5.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga5.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga5.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga5.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga5.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga5.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch1()) / outputCarrier.Hga5.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch2()) / outputCarrier.Hga5.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch3()) / outputCarrier.Hga5.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch4()) / outputCarrier.Hga5.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch5()) / outputCarrier.Hga5.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch6()) / outputCarrier.Hga5.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga5.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga5.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga5.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga5.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga5.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga5.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga5.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga5.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga5.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga5.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga5.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga5.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga5.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga5.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga5.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga5.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga5.get_DeltaISI_R2_SDET_Tolerance();

                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (outputCarrier.Hga5.TST_STATUS.Equals('\0') || outputCarrier.Hga5.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga5.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga5.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga5.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga5.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA5
                    #region HGA6
                    else if (i == 5)  // HGA6
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga6.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga6.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga6.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga6.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga6.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga6.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga6.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga6.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga6.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga6.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga6.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga6.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga6.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga6.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga6.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga6.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga6.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga6.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;    
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga6.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga6.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga6.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga6.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga6.getCh1Temperature() + "_" + outputCarrier.Hga6.getCh2Temperature() + "_" + outputCarrier.Hga6.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga6.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga6.getResistanceSpec();

                        _loadData.ISI_Reader1 = outputCarrier.Hga6.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga6.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga6.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga6.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga6.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga6.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga6.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga6.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga6.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga6.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga6.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga6.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga6.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga6.Delta_IThreshold;
                        //For HAMR *-------------------------------
                        
                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga6.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga6.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga6.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga6.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga6.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga6.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga6.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga6.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga6.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga6.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga6.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga6.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga6.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga6.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch1()) / outputCarrier.Hga6.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch2()) / outputCarrier.Hga6.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch3()) / outputCarrier.Hga6.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch4()) / outputCarrier.Hga6.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch5()) / outputCarrier.Hga6.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch6()) / outputCarrier.Hga6.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga6.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga6.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga6.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga6.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga6.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga6.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga6.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga6.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga6.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga6.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga6.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga6.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga6.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga6.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga6.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga6.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga6.get_DeltaISI_R2_SDET_Tolerance();
                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (outputCarrier.Hga6.TST_STATUS.Equals('\0') || outputCarrier.Hga6.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga6.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga6.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga6.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga6.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA6
                    #region HGA7
                    else if (i == 6)  // HGA7
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga7.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga7.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga7.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga7.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga7.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga7.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga7.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga7.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga7.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga7.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga7.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga7.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga7.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga7.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga7.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga7.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga7.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga7.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;           

                        if (outputCarrier.Hga7.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga7.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga7.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga7.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga7.getCh1Temperature() + "_" + outputCarrier.Hga7.getCh2Temperature() + "_" + outputCarrier.Hga7.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga7.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga7.getResistanceSpec();
                        _loadData.ISI_Reader1 = outputCarrier.Hga7.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga7.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga7.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga7.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga7.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga7.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga7.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga7.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga7.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga7.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga7.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga7.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga7.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga7.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga7.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga7.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga7.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga7.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga7.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga7.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga7.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga7.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga7.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga7.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga7.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga7.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga7.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga7.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch1()) / outputCarrier.Hga7.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch2()) / outputCarrier.Hga7.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch3()) / outputCarrier.Hga7.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch4()) / outputCarrier.Hga7.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch5()) / outputCarrier.Hga7.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch6()) / outputCarrier.Hga7.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga7.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga7.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga7.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga7.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga7.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga7.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga7.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga7.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga7.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga7.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga7.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga7.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga7.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga7.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga7.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga7.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga7.get_DeltaISI_R2_SDET_Tolerance();

                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (outputCarrier.Hga7.TST_STATUS.Equals('\0') || outputCarrier.Hga7.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga7.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga7.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga7.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga7.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA7
                    #region HGA8
                    else if (i == 7)  // HGA8
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga8.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga8.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga8.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga8.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga8.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga8.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga8.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga8.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga8.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga8.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga8.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga8.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga8.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga8.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga8.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga8.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga8.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga8.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;      
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga8.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga8.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga8.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga8.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga8.getCh1Temperature() + "_" + outputCarrier.Hga8.getCh2Temperature() + "_" + outputCarrier.Hga8.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga8.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga8.getResistanceSpec();
                        _loadData.ISI_Reader1 = outputCarrier.Hga8.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga8.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga8.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga8.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga8.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga8.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga8.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga8.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga8.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga8.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga8.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga8.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga8.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga8.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga8.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga8.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga8.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga8.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga8.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga8.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga8.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga8.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga8.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga8.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga8.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga8.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga8.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga8.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch1()) / outputCarrier.Hga8.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch2()) / outputCarrier.Hga8.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch3()) / outputCarrier.Hga8.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch4()) / outputCarrier.Hga8.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch5()) / outputCarrier.Hga8.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch6()) / outputCarrier.Hga8.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga8.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga8.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga8.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga8.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga8.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga8.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga8.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga8.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga8.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga8.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga8.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga8.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga8.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga8.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga8.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga8.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga8.get_DeltaISI_R2_SDET_Tolerance();
                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (outputCarrier.Hga8.TST_STATUS.Equals('\0') || outputCarrier.Hga8.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga8.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga8.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga8.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga8.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA8
                    #region HGA9
                    else if (i == 8)  // HGA9
                    {
                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga9.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga9.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga9.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga9.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga9.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga9.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga9.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga9.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga9.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga9.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga9.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga9.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga9.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga9.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga9.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga9.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga9.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga9.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga9.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga9.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga9.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga9.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga9.getCh1Temperature() + "_" + outputCarrier.Hga9.getCh2Temperature() + "_" + outputCarrier.Hga9.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga9.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga9.getResistanceSpec();
                        _loadData.ISI_Reader1 = outputCarrier.Hga9.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga9.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga9.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga9.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga9.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga9.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga9.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga9.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga9.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga9.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga9.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga9.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga9.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga9.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga9.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga9.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga9.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga9.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga9.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga9.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga9.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga9.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga9.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga9.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga9.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga9.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga9.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga9.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch1()) / outputCarrier.Hga9.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch2()) / outputCarrier.Hga9.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch3()) / outputCarrier.Hga9.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch4()) / outputCarrier.Hga9.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch5()) / outputCarrier.Hga9.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch6()) / outputCarrier.Hga9.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga9.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga9.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga9.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga9.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga9.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga9.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga9.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga9.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga9.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga9.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga9.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga9.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga9.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga9.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga9.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga9.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga9.get_DeltaISI_R2_SDET_Tolerance();
                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {

                            if (outputCarrier.Hga9.TST_STATUS.Equals('\0') || outputCarrier.Hga9.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga9.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga9.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga9.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga9.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA9
                    #region HGA10
                    else if (i == 9)  // HGA10
                    {

                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((outputCarrier.Hga10.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            outputCarrier.Hga10.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = outputCarrier.Hga10.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((outputCarrier.Hga10.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga10.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = outputCarrier.Hga10.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((outputCarrier.Hga10.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga10.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = outputCarrier.Hga10.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((outputCarrier.Hga10.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga10.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = outputCarrier.Hga10.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((outputCarrier.Hga10.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga10.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = outputCarrier.Hga10.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((outputCarrier.Hga10.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                outputCarrier.Hga10.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = outputCarrier.Hga10.getWHeaterResistance().ToString("F3");

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (outputCarrier.Hga10.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (outputCarrier.Hga10.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = outputCarrier.Hga10.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = outputCarrier.Hga10.getBiasVoltage();
                        _loadData.TemperatureBoard = outputCarrier.Hga10.getCh1Temperature() + "_" + outputCarrier.Hga10.getCh2Temperature() + "_" + outputCarrier.Hga10.getCh3Temperature();
                        _loadData.BiasCurrentSource = outputCarrier.Hga10.getBiasCurrent();
                        _loadData.ResistanceSpecification = outputCarrier.Hga10.getResistanceSpec();

                        _loadData.ISI_Reader1 = outputCarrier.Hga10.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = outputCarrier.Hga10.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = outputCarrier.Hga10.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga10.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga10.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = outputCarrier.Hga10.get_ldu_Threshold_Ma();
                        if (_workcell.getPanelRecipe().GetGomperztCalculation().UseGompertzCalculation())
                        {
                            if (outputCarrier.Hga10.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                            }
                            else
                            {
                                _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                                _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                            }
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = outputCarrier.Hga10.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = outputCarrier.Hga10.get_ldu_Sweep_Spec_Min();
                        _loadData.LDU_Sweep_Spec_Max = outputCarrier.Hga10.get_ldu_Sweep_Spec_Max();
                        _loadData.Led_Intercept = outputCarrier.Hga10.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = outputCarrier.Hga10.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;
                        _loadData.Sdet_iThreshold = outputCarrier.Hga10.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = outputCarrier.Hga10.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = outputCarrier.Hga10.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = outputCarrier.Hga10.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = outputCarrier.Hga10.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = outputCarrier.Hga10.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = outputCarrier.Hga10.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = outputCarrier.Hga10.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = outputCarrier.Hga10.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = outputCarrier.Hga10.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = outputCarrier.Hga10.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = outputCarrier.Hga10.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = outputCarrier.Hga10.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = outputCarrier.Hga10.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = outputCarrier.Hga10.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = outputCarrier.Hga10.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch1()) / outputCarrier.Hga10.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch2()) / outputCarrier.Hga10.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch3()) / outputCarrier.Hga10.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch4()) / outputCarrier.Hga10.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch5()) / outputCarrier.Hga10.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch6()) / outputCarrier.Hga10.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = outputCarrier.Hga10.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = outputCarrier.Hga10.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = outputCarrier.Hga10.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = outputCarrier.Hga10.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = outputCarrier.Hga10.UTIC_DATA.DOCK_SIDE;

                        _loadData.Delta_Writer_SDET = outputCarrier.Hga10.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = outputCarrier.Hga10.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = outputCarrier.Hga10.Get_WRBridge_Percentage();

                        _loadData.wrbridge_adap_spec = outputCarrier.Hga10.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = outputCarrier.Hga10.get_sdet_reader1();
                        _loadData.sdet_reader2 = outputCarrier.Hga10.get_sdet_reader2();
                        _loadData.gauss_meter1 = outputCarrier.Hga10.get_gauss_meter1();
                        _loadData.gauss_meter2 = outputCarrier.Hga10.get_gauss_meter2();
                        _loadData.anc_yield = outputCarrier.Hga10.get_anc_yield();
                        _loadData.anc_hga_count = outputCarrier.Hga10.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = outputCarrier.Hga10.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = outputCarrier.Hga10.get_DeltaISI_R2_SDET_Tolerance();

                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {

                            if (outputCarrier.Hga10.TST_STATUS.Equals('\0') || outputCarrier.Hga10.TST_STATUS.Equals(string.Empty) || outputCarrier.Hga10.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                outputCarrier.Hga10.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = outputCarrier.Hga10.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[i].Status;
                        }

                        ErrorMessageCode = outputCarrier.Hga10.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }
                    #endregion HGA10

                    if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog && !UntestedHGA[i])
                        {
                            XyratexOSC.Logging.Log.Info(this,
                                "For HGA {0}, Serial Number: {1}, Status: {2},  Reader1 Resist: {3},  Reader2 Resist: {4},Writer Resist: {5}, RHeater Resist: {6}, WHeater Resist: {7}, TA Resist: {8}, Short Test: {9}, Short Test Position: {10}, Bias Voltage: {11}, Bias Current: {12}, Temperature: {13}, Error_Msg_Code: {14}, Resist Spec: {15}",
                                _loadData.CarrierSlot, _loadData.SerialNo, _loadData.Status, _loadData.ReaderResistance, _loadData.WriterResistance, _loadData.RHeaterResistance, _loadData.WHeaterResistance, _loadData.TAResistance, _loadData.ShortTest,
                                _loadData.ShortTestPosition, _loadData.UACTCapacitance1, _loadData.UACTCapacitance2, _loadData.BiasVoltageMeasure, _loadData.BiasCurrentSource, _loadData.TemperatureBoard, _loadData.ErrorMessageCode, _loadData.Reader2Resistance);
                        }
                    }

                    if (String.IsNullOrEmpty(folaTagData.HGAData[i].HgaSN) == true || folaTagData.HGAData[i].HgaSN == "\0")
                    {
                        _loadData.SerialNo = "NULL";
                        sendHGADataToSeatrack = false;
                    }

                    if (outputCarrier.RFIDData.RFIDTag.HGAData[i].Status != CommonFunctions.SKIP_TEST && !UntestedHGA[i] && sendHGADataToSeatrack && HSTMachine.Workcell.HSTSettings.Install.SeatrackRecordUpdateEnabled)
                        FISManager.Instance.SendSeatrackLoadData(_loadData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Seatrack Send error from Output Station processs", ex);
            }
        }

        public void SendLoadDataToSeaTrack_new(FolaTagData folaTagData, Carrier outputCarrier)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true))
            {
                return;
            }

            try
            {

                if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        XyratexOSC.Logging.Log.Info(this, "Seatrack EventDate, {0}", _loadData.EventDate);
                        XyratexOSC.Logging.Log.Info(this, "Location ID, {0}", _loadData.Location);
                        XyratexOSC.Logging.Log.Info(this, "Equipment ID, {0}", _loadData.EquipmentID);
                        XyratexOSC.Logging.Log.Info(this, "Equipment Type, {0}", _loadData.EquipmentType);
                        XyratexOSC.Logging.Log.Info(this, "Login User, {0}", _loadData.LoginUser);
                        XyratexOSC.Logging.Log.Info(this, "Carrier ID, {0}", _loadData.CarrierID);
                        XyratexOSC.Logging.Log.Info(this, "Work Order Number, {0}", _loadData.WorkOrder);
                        XyratexOSC.Logging.Log.Info(this, "Work Order Version, {0}", _loadData.WorkOrderVersion);
                        XyratexOSC.Logging.Log.Info(this, "Setup File, {0}", _loadData.SetupFile);
                        XyratexOSC.Logging.Log.Info(this, "X Axis, {0}", _loadData.XAxis);
                        XyratexOSC.Logging.Log.Info(this, "Y Axis, {0}", _loadData.YAxis);
                        XyratexOSC.Logging.Log.Info(this, "Z Axis, {0}", _loadData.ThetaAxis);
                        XyratexOSC.Logging.Log.Info(this, "Software Status, {0}", _loadData.SoftwareStatus);
                        XyratexOSC.Logging.Log.Info(this, "Operation Mode, {0}", _loadData.OperationMode);
                        XyratexOSC.Logging.Log.Info(this, "Status, {0}", _loadData.Status);
                        XyratexOSC.Logging.Log.Info(this, "Temperature Board, {0}", _loadData.TemperatureBoard);
                        XyratexOSC.Logging.Log.Info(this, "Bias Current Source, {0}", _loadData.BiasCurrentSource);
                        XyratexOSC.Logging.Log.Info(this, "Resistance Specification, {0}", _loadData.ResistanceSpecification);
                    }
                }

                var hgaData = new Hga(1, HGAStatus.Unknown);
                bool[] UntestedHGA = new bool[10];
                UntestedHGA[0] = ((outputCarrier.Hga1.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga1.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[1] = ((outputCarrier.Hga2.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga2.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[2] = ((outputCarrier.Hga3.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga3.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[3] = ((outputCarrier.Hga4.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga4.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[4] = ((outputCarrier.Hga5.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga5.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[5] = ((outputCarrier.Hga6.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga6.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[6] = ((outputCarrier.Hga7.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga7.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[7] = ((outputCarrier.Hga8.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga8.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[8] = ((outputCarrier.Hga9.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga9.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[9] = ((outputCarrier.Hga10.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga10.Hga_Status == HGAStatus.TestedFail)) ? false : true;

                for (int slot = 0; slot < outputCarrier.RFIDData.RFIDTagData.CarrierSize; slot++)
                {

                    switch (slot)
                    {
                        case 0:
                            hgaData = outputCarrier.Hga1;
                            break;
                        case 1:
                            hgaData = outputCarrier.Hga2;
                            break;
                        case 2:
                            hgaData = outputCarrier.Hga3;
                            break;
                        case 3:
                            hgaData = outputCarrier.Hga4;
                            break;
                        case 4:
                            hgaData = outputCarrier.Hga5;
                            break;
                        case 5:
                            hgaData = outputCarrier.Hga6;
                            break;
                        case 6:
                            hgaData = outputCarrier.Hga7;
                            break;
                        case 7:
                            hgaData = outputCarrier.Hga8;
                            break;
                        case 8:
                            hgaData = outputCarrier.Hga9;
                            break;
                        case 9:
                            hgaData = outputCarrier.Hga10;
                            break;
                    }

                    bool sendHGADataToSeatrack = true;

                    if (String.IsNullOrEmpty(folaTagData.HGAData[slot].HgaSN) == true ||
                        (folaTagData.HGAData[slot].HgaSN) == "\0")
                    {
                        _loadData.SerialNo = "NULL";
                        sendHGADataToSeatrack = false;
                    }
                    else
                    {
                        _loadData.SerialNo = folaTagData.HGAData[slot].HgaSN;
                    }

                    _loadData.Status = (GetHGAPartStatus(slot, outputCarrier)) ? FisSession.EVENT_PASS : FisSession.EVENT_FAIL;
                    _loadData.HGASerialNumber = folaTagData.HGAData[slot].HgaSN;
                    _loadData.StatusCode = folaTagData.HGAData[slot].Status.ToString();
                    _loadData.IBSCheck = outputCarrier.WorkOrderData.IBSCheck.ToString().ToUpper();
                    _loadData.CarrierSlot = (slot + 1).ToString();
                    string ErrorMessageCode = "";

                    if (UntestedHGA[slot])
                    {
                        _loadData.ReaderResistance = "";
                        _loadData.WriterResistance = "";
                        _loadData.RHeaterResistance = "";
                        _loadData.WHeaterResistance = "";
                        _loadData.TAResistance = "";
                        _loadData.ShortTest = 0;
                        _loadData.ShortTestPosition = "0";
                        _loadData.UACTCapacitance1 = 0;
                        _loadData.UACTCapacitance2 = 0;
                        _loadData.BiasVoltageMeasure = 0;
                        _loadData.TemperatureBoard = "";
                        _loadData.BiasCurrentSource = 0;
                        _loadData.ResistanceSpecification = "";
                        _loadData.ErrorMessageCode = "";
                        _loadData.ISI_Reader1 = 0;
                        _loadData.ISI_Reader2 = 0;
                        _loadData.Delta_R1 = 0;
                        _loadData.Delta_R2 = 0;
                        _loadData.LDU_Res = 0;
                        _loadData.LDU_Res_Spec_Min = 0;
                        _loadData.LDU_Res_Spec_Max = 0;
                        _loadData.StatusCode = "";
                        _loadData.IBSCheck = "";
                        _loadData.RDIbsPattern = "";
                        _loadData.WRIbsPattern = "";
                        _loadData.TicEquipID = "";

                        //add 5-feb-2020
                        _loadData.Led_Intercept = 0.0;
                        _loadData.Led_Intercept_Spec = 0.0;
                        _loadData.Led_Intercept_Spec_Max = 0.0;
                        _loadData.Pd_Voltage = 0.0;
                        _loadData.Pd_Voltage_Spec_Max = 0.0;
                        _loadData.Sdet_iThreshold = 0.0;
                        _loadData.LDU_Res = 0.0;
                        _loadData.sdet_writer = 0.0;
                        _loadData.hst_sdet_delta_writer = 0.0;
                        _loadData.wrbridge_pct = 0.0;
                        _loadData.sdet_reader1 = 0.0;
                        _loadData.sdet_reader2 = 0.0;

                        _loadData.gauss_meter1 = 0.0;
                        _loadData.gauss_meter2 = 0.0;
                    }
                    else
                    {

                        //CTQ_WRT_RES
                        string ctp_wrt_res = String.Empty;
                        if ((hgaData.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                            hgaData.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_wrt_res = hgaData.getWriterResistance().ToString("F3");

                        //CTQ_RD_RES1
                        string ctq_rd_res1 = String.Empty;
                        if ((hgaData.getReader1Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                hgaData.getReader1Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res1 = hgaData.getReader1Resistance().ToString("F3");

                        //CTQ_RD_RES2
                        string ctq_rd_res2 = String.Empty;
                        if ((hgaData.getReader2Resistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                hgaData.getReader2Resistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_rd_res2 = hgaData.getReader2Resistance().ToString("F3");

                        //CTQ_HTR2_RES
                        string ctp_htr2_res = String.Empty;
                        if ((hgaData.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                hgaData.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctp_htr2_res = hgaData.getRHeaterResistance().ToString("F3");

                        //CTQ_TA_RES
                        string ctq_ta_res = String.Empty;
                        if ((hgaData.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                hgaData.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_ta_res = hgaData.getTAResistance().ToString("F3");

                        //CTQ_HTR_RES
                        string ctq_htr_res = String.Empty;
                        if ((hgaData.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                hgaData.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                            ctq_htr_res = hgaData.getWHeaterResistance().ToString("F3");

                        double last_delta_threshold = 0.0;
                        last_delta_threshold = hgaData.getIThreshold() - hgaData.Last_ET_Threshold;

                        _loadData.ReaderResistance = ctq_rd_res1;       
                        _loadData.Reader2Resistance = ctq_rd_res2;      
                        _loadData.WriterResistance = ctp_wrt_res;       
                        _loadData.RHeaterResistance = ctp_htr2_res;     
                        _loadData.WHeaterResistance = ctq_htr_res;      
                        _loadData.TAResistance = ctq_ta_res;            

                        if (hgaData.getShortTest() == ShortDetection.Short)
                        {
                            _loadData.ShortTest = 1;
                        }
                        else if (hgaData.getShortTest() == ShortDetection.NoTest)
                        {
                            _loadData.ShortTest = 2;
                        }
                        else
                        {
                            _loadData.ShortTest = 0;
                        }
                        _loadData.ShortTestPosition = hgaData.getShortPadPosition();
                        _loadData.BiasVoltageMeasure = hgaData.getBiasVoltage();
                        _loadData.TemperatureBoard = hgaData.getCh1Temperature() + "_" + hgaData.getCh2Temperature() + "_" + hgaData.getCh3Temperature();
                        _loadData.BiasCurrentSource = hgaData.getBiasCurrent();
                        _loadData.ResistanceSpecification = hgaData.getResistanceSpec();
                        _loadData.ISI_Reader1 = hgaData.DeltaISIResistanceRD1;
                        _loadData.ISI_Reader2 = hgaData.DeltaISIResistanceRD2;
                        _loadData.Delta_R1 = hgaData.getDeltaISIReader1();
                        _loadData.Delta_R2 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? hgaData.getDeltaISIReader2() : 0;

                        //For HAMR *-------------------------------
                        _loadData.LDU_Res = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? hgaData.getReader2Resistance() : 0; //HAMR
                        _loadData.LDU_Res_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin;    //HAMR
                        _loadData.LDU_Res_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax;    //HAMR

                        _loadData.LDU_Threshold_Ma = hgaData.get_ldu_Threshold_Ma();
                        if (hgaData.get_IthresholdCalculationMethod() == I_ThresholdCalculationMethod.Gompertz)
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper;
                        }
                        else
                        {
                            _loadData.LDU_Threshold_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower;
                            _loadData.LDU_Threshold_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        }
                        _loadData.LDU_Volt_Intercept = hgaData.getLDUTurnOnVoltage();
                        _loadData.LDU_Sweep_Spec_Min = hgaData.get_ldu_Sweep_Spec_Min();    
                        _loadData.LDU_Sweep_Spec_Max = hgaData.get_ldu_Sweep_Spec_Max();    
                        _loadData.Led_Intercept = hgaData.getLEDIntercept();
                        _loadData.Led_Intercept_Spec =
                            CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower;
                        _loadData.Led_Intercept_Spec_Max =
                            CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper;
                        _loadData.Pd_Voltage = hgaData.getMaxVPD();

                        _loadData.Pd_Voltage_Spec_Min = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec;  //*
                        _loadData.Pd_Voltage_Spec_Max = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec;  //*
                        _loadData.Sdet_iThreshold = hgaData.Last_ET_Threshold;
                        _loadData.Delta_iThreshold = last_delta_threshold;    //*
                        //For HAMR *-------------------------------

                        _loadData.Get_Bias_Ch1 = hgaData.Get_Bias_Ch1();
                        _loadData.Get_Bias_Ch2 = hgaData.Get_Bias_Ch2();
                        _loadData.Get_Bias_Ch3 = hgaData.Get_Bias_Ch3();
                        _loadData.Get_Bias_Ch4 = hgaData.Get_Bias_Ch4();
                        _loadData.Get_Bias_Ch5 = hgaData.Get_Bias_Ch5();
                        _loadData.Get_Bias_Ch6 = hgaData.Get_Bias_Ch6();
                        _loadData.Get_Sensing_Ch1 = hgaData.Get_Sensing_Ch1();
                        _loadData.Get_Sensing_Ch2 = hgaData.Get_Sensing_Ch2();
                        _loadData.Get_Sensing_Ch3 = hgaData.Get_Sensing_Ch3();
                        _loadData.Get_Sensing_Ch4 = hgaData.Get_Sensing_Ch4();
                        _loadData.Get_Sensing_Ch5 = hgaData.Get_Sensing_Ch5();
                        _loadData.Get_Sensing_Ch6 = hgaData.Get_Sensing_Ch6();
                        _loadData.Voltage_Delta1 = hgaData.Get_Voltage_Delta1();
                        _loadData.Voltage_Delta2 = hgaData.Get_Voltage_Delta2();
                        _loadData.Volt_Ratio_Ch1 = (Decimal.ToDouble(hgaData.Get_Bias_Ch1()) / hgaData.Get_Sensing_Ch1());
                        _loadData.Volt_Ratio_Ch2 = (Decimal.ToDouble(hgaData.Get_Bias_Ch2()) / hgaData.Get_Sensing_Ch2());
                        _loadData.Volt_Ratio_Ch3 = (Decimal.ToDouble(hgaData.Get_Bias_Ch3()) / hgaData.Get_Sensing_Ch3());
                        _loadData.Volt_Ratio_Ch4 = (Decimal.ToDouble(hgaData.Get_Bias_Ch4()) / hgaData.Get_Sensing_Ch4());
                        _loadData.Volt_Ratio_Ch5 = (Decimal.ToDouble(hgaData.Get_Bias_Ch5()) / hgaData.Get_Sensing_Ch5());
                        _loadData.Volt_Ratio_Ch6 = (Decimal.ToDouble(hgaData.Get_Bias_Ch6()) / hgaData.Get_Sensing_Ch6());
                        _loadData.RDIbsPattern = hgaData.IBS_Data.Current_RD_Pattern;
                        _loadData.WRIbsPattern = hgaData.IBS_Data.Current_WR_Pattern;
                        _loadData.TicEquipID = hgaData.UTIC_DATA.EQUIP_ID;
                        _loadData.TicTime = hgaData.UTIC_DATA.EVENT_DATE;
                        _loadData.TicDockSide = hgaData.UTIC_DATA.DOCK_SIDE;
                        _loadData.Delta_Writer_SDET = hgaData.get_sdet_writer();
                        _loadData.Delta_Writer_HST_SDET = hgaData.Get_Delta_WR_Hst_Sdet();
                        _loadData.WRBridge_Percentage = hgaData.Get_WRBridge_Percentage();
                        _loadData.wrbridge_adap_spec = hgaData.get_wrbridge_adap_spec();
                        _loadData.sdet_reader1 = hgaData.get_sdet_reader1();
                        _loadData.sdet_reader2 = hgaData.get_sdet_reader2();
                        _loadData.gauss_meter1 = hgaData.get_gauss_meter1();
                        _loadData.gauss_meter2 = hgaData.get_gauss_meter2();
                        _loadData.anc_yield = hgaData.get_anc_yield();
                        _loadData.anc_hga_count = hgaData.get_anc_hga_count();
                        _loadData.DeltaISI_R1_SDET_Tolerance = hgaData.get_DeltaISI_R1_SDET_Tolerance();
                        _loadData.DeltaISI_R2_SDET_Tolerance = hgaData.get_DeltaISI_R2_SDET_Tolerance();

                        if (CommonFunctions.Instance.MeasurementTestRecipe.EnableSortData || CommonFunctions.Instance.MeasurementTestRecipe.EnableSortResistanceSpec)
                        {
                            if (hgaData.TST_STATUS.Equals('\0') || hgaData.TST_STATUS.Equals(string.Empty) || hgaData.TST_STATUS.Equals('0'))
                            {
                                _loadData.Status = FisSession.EVENT_FAIL;
                                _loadData.SDET_TST_STATUS = CommonFunctions.TEST_FAIL_CODE;
                                hgaData.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                _loadData.SDET_TST_STATUS = hgaData.TST_STATUS;
                            }
                        }
                        else
                        {
                            _loadData.SDET_TST_STATUS = _workcell.Process.OutputStationProcess.Controller.RfidController.FolaTagDataWriteInfor.HGAData[slot].Status;
                        }

                        ErrorMessageCode = hgaData.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _loadData.ErrorMessageCode = ErrorMessageCode.Replace("; ", "_");
                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog && !UntestedHGA[slot])
                        {
                            XyratexOSC.Logging.Log.Info(this,
                                "For HGA {0}, Serial Number: {1}, Status: {2},  Reader1 Resist: {3},  Reader2 Resist: {19},Writer Resist: {4}, RHeater Resist: {5}, WHeater Resist: {6}, TA Resist: {7}, Short Test: {8}, Short Test Position: {9}, Bias Voltage: {10}, Bias Current: {11}, Temperature: {12}, Resist Spec: {13}, Capa1 Spec: {14}, Capa2 Spec: {15},Error_Msg_Code: {16}",
                                _loadData.CarrierSlot, _loadData.SerialNo, _loadData.Status, _loadData.ReaderResistance, _loadData.WriterResistance, _loadData.RHeaterResistance, _loadData.WHeaterResistance, _loadData.TAResistance, _loadData.ShortTest,
                                _loadData.ShortTestPosition, _loadData.UACTCapacitance1, _loadData.UACTCapacitance2, _loadData.BiasVoltageMeasure, _loadData.BiasCurrentSource, _loadData.TemperatureBoard, _loadData.ResistanceSpecification, _loadData.ErrorMessageCode, _loadData.Reader2Resistance);
                        }
                    }

                    if (String.IsNullOrEmpty(folaTagData.HGAData[slot].HgaSN) == true || folaTagData.HGAData[slot].HgaSN == "\0")
                    {
                        _loadData.SerialNo = "NULL";
                        sendHGADataToSeatrack = false;
                    }

                    if (outputCarrier.RFIDData.RFIDTag.HGAData[slot].Status != CommonFunctions.SKIP_TEST && !UntestedHGA[slot] && sendHGADataToSeatrack && HSTMachine.Workcell.HSTSettings.Install.SeatrackRecordUpdateEnabled)
                        FISManager.Instance.SendSeatrackLoadData(_loadData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Seatrack Send error from Output Station processs", ex);
            }
        }

        public void UpdateDataLog(FolaTagData folaTagData, Carrier outputCarrier)
        {
            _datalog = new Models.DataLog();

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true))
            {
                return;
            }

            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "UpdateDataLog at OutputStation for Carrier ID:{0}.", outputCarrier.CarrierID);
                }
                _datalog.MachineLocation = HSTMachine.Workcell.HSTSettings.Install.LocationID;
                _datalog.EquipmentID = HSTMachine.Workcell.HSTSettings.Install.EquipmentID;
                _datalog.EquipmentType = HSTMachine.Workcell.HSTSettings.EquipmentType;
                _datalog.LoginUser = HSTMachine.Workcell.HSTSettings.OperatorGID;
                _datalog.WorkOrder = folaTagData.WorkOrder;
                _datalog.WorkOrderVersion = folaTagData.WorkOrderVersion.ToString();
                _datalog.SetupFileName = HSTMachine.Workcell.WorkOrder.Loading.LoadingProgramName;

                if (!outputCarrier.IsPassThroughMode)
                {
                    _datalog.XAxis = outputCarrier.getPrecisorNestPositionX();
                    _datalog.YAxis = outputCarrier.getPrecisorNestPositionY();
                    _datalog.ZAxis = outputCarrier.getPrecisorNestPositionZ();
                    _datalog.theta = outputCarrier.getPrecisorNestPositionTheta();
                }
                else
                {
                    _datalog.XAxis = 0.00;
                    _datalog.YAxis = 0.00;
                    _datalog.ZAxis = 0.00;
                    _datalog.theta = 0.00;
                }

                bool[] UntestedHGA = new bool[10];
                UntestedHGA[0] = ((outputCarrier.Hga1.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga1.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[1] = ((outputCarrier.Hga2.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga2.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[2] = ((outputCarrier.Hga3.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga3.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[3] = ((outputCarrier.Hga4.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga4.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[4] = ((outputCarrier.Hga5.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga5.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[5] = ((outputCarrier.Hga6.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga6.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[6] = ((outputCarrier.Hga7.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga7.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[7] = ((outputCarrier.Hga8.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga8.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[8] = ((outputCarrier.Hga9.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga9.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[9] = ((outputCarrier.Hga10.Hga_Status == HGAStatus.TestedPass) ||
                                    (outputCarrier.Hga10.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                Log.Info(this, "Carrier Size = {0}", folaTagData.CarrierSize);
                for (int i = 0; i < folaTagData.CarrierSize; i++)
                {

                    _datalog.CarrierID = folaTagData.CarrierID;
                    _datalog.CarrierSlot = i + 1;
                    _datalog.HGASerialNumber = folaTagData.HGAData[i].HgaSN;
                    _datalog.CycleTime = HSTMachine.Workcell.LoadCounter.CycleTime;
                    _datalog.UPH = HSTMachine.Workcell.LoadCounter.UPH;
                    _datalog.UPH2 = HSTMachine.Workcell.LoadCounter.UPH2;
                    _datalog.LDUSpec = String.Format("Min={0}-Max={1}", CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin.ToString(),
                            CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax.ToString());

                    string ErrorMessageCode = "";
                    //_datalog.HGAStatus = _rfidController.FolaTagDataWriteInfor[i].Status == CommonFunctions.TEST_PASS_CODE ? HGAStatus.TestedPass : HGAStatus.TestedFail;
                    if (i == 0)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga1.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga1.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 1)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga2.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga2.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 2)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga3.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga3.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 3)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga4.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga4.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 4)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga5.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga5.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 5)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga6.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga6.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 6)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga7.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga7.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 7)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga8.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga8.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else if (i == 8)
                    {
                        _datalog.HGAStatus = outputCarrier.Hga9.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga9.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }
                    else
                    {
                        _datalog.HGAStatus = outputCarrier.Hga10.Hga_Status;
                        ErrorMessageCode = outputCarrier.Hga10.Error_Msg_Code;
                        if (ErrorMessageCode.EndsWith("; "))
                        {
                            ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                        }
                        _datalog.ErrorMessageCode = ErrorMessageCode;
                    }

                    if (UntestedHGA[i])
                    {
                        _datalog.ReaderResistance = 0;
                        _datalog.Reader2Resistance = 0;
                        _datalog.WriterResistance = 0;
                        _datalog.rHeaterResistance = 0;
                        _datalog.wHeaterResistance = 0;
                        _datalog.TAResistance = 0;
                        _datalog.ShortTest = ShortTest.NoShort;
                        _datalog.ShortTestPosition = "0";
                        _datalog.MicroActuatorCapacitance1 = 0;
                        _datalog.MicroActuatorCapacitance2 = 0;
                        _datalog.BiasVoltage = 0;
                        _datalog.BiasCurrent = 0;
                        _datalog.ResistanceSpec = "";
                        _datalog.CapacitanceSpec1 = "";
                        _datalog.CapacitanceSpec2 = "";
                        _datalog.ThermisterTemperature = "";
                        _datalog.ErrorMessageCode = "";
                        _datalog.LDURes = "";
                        _datalog.LDUSpec = "";
                    }
                    else if (i == 0)  // HAG1
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga1.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga1.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga1.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga1.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga1.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga1.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga1.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga1.getTAResistance();

                        if (outputCarrier.Hga1.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga1.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga1.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga1.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga1.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga1.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga1.getCh1Temperature() + "_" + outputCarrier.Hga1.getCh2Temperature() + "_" + outputCarrier.Hga1.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga1.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga1.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga1.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga1.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga1.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga1.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga1.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga1.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga1.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga1.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga1.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch1()) / outputCarrier.Hga1.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch2()) / outputCarrier.Hga1.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch3()) / outputCarrier.Hga1.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch4()) / outputCarrier.Hga1.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch5()) / outputCarrier.Hga1.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga1.Get_Bias_Ch6()) / outputCarrier.Hga1.Get_Sensing_Ch6());

                        _datalog.Sdet_Writer = outputCarrier.Hga1.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga1.Get_Delta_WR_Hst_Sdet();

                        _datalog.Wrbridge_Pct = outputCarrier.Hga1.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga1.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga1.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga1.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga1.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga1.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga1.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga1.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 1)  // HGA2
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga2.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga2.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga2.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga2.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga2.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga2.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga2.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga2.getTAResistance();

                        if (outputCarrier.Hga2.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga2.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga2.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga2.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga2.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga2.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga2.getCh1Temperature() + "_" + outputCarrier.Hga2.getCh2Temperature() + "_" + outputCarrier.Hga2.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga2.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga2.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga2.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga2.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga2.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga2.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga2.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga2.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga2.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga2.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga2.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch1()) / outputCarrier.Hga2.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch2()) / outputCarrier.Hga2.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch3()) / outputCarrier.Hga2.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch4()) / outputCarrier.Hga2.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch5()) / outputCarrier.Hga2.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga2.Get_Bias_Ch6()) / outputCarrier.Hga2.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga2.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga2.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga2.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga2.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga2.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga2.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga2.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga2.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga2.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga2.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 2)  // HGA3
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga3.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga3.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga3.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga3.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga3.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga3.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga3.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga3.getTAResistance();

                        if (outputCarrier.Hga3.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga3.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga3.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga3.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga3.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga3.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga3.getCh1Temperature() + "_" + outputCarrier.Hga3.getCh2Temperature() + "_" + outputCarrier.Hga3.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga3.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga3.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga3.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga3.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga3.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga3.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga3.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga3.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga3.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga3.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga3.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch1()) / outputCarrier.Hga3.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch2()) / outputCarrier.Hga3.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch3()) / outputCarrier.Hga3.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch4()) / outputCarrier.Hga3.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch5()) / outputCarrier.Hga3.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga3.Get_Bias_Ch6()) / outputCarrier.Hga3.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga3.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga3.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga3.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga3.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga3.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga3.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga3.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga3.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga3.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga3.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 3)  // HGA4
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga4.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga4.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga4.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga4.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga4.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga4.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga4.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga4.getTAResistance();

                        if (outputCarrier.Hga4.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga4.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga4.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga4.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga4.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga4.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga4.getCh1Temperature() + "_" + outputCarrier.Hga4.getCh2Temperature() + "_" + outputCarrier.Hga4.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga4.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga4.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga4.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga4.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga4.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga4.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga4.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga4.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga4.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga4.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga4.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch1()) / outputCarrier.Hga4.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch2()) / outputCarrier.Hga4.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch3()) / outputCarrier.Hga4.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch4()) / outputCarrier.Hga4.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch5()) / outputCarrier.Hga4.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga4.Get_Bias_Ch6()) / outputCarrier.Hga4.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga4.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga4.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga4.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga4.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga4.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga4.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga4.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga4.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga4.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga4.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 4)  // HGA5
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga5.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga5.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga5.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga5.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga5.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga5.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga5.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga5.getTAResistance();

                        if (outputCarrier.Hga5.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga5.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga5.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga5.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga5.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga5.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga5.getCh1Temperature() + "_" + outputCarrier.Hga5.getCh2Temperature() + "_" + outputCarrier.Hga5.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga5.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga5.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga5.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga5.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga5.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga5.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga5.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga5.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga5.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga5.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga5.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch1()) / outputCarrier.Hga5.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch2()) / outputCarrier.Hga5.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch3()) / outputCarrier.Hga5.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch4()) / outputCarrier.Hga5.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch5()) / outputCarrier.Hga5.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga5.Get_Bias_Ch6()) / outputCarrier.Hga5.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga5.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga5.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga5.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga5.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga5.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga5.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga5.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga5.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga5.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga5.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 5)  // HGA6
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga6.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga6.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga6.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga6.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga6.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga6.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga6.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga6.getTAResistance();

                        if (outputCarrier.Hga6.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga6.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga6.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga6.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga6.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga6.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga6.getCh1Temperature() + "_" + outputCarrier.Hga6.getCh2Temperature() + "_" + outputCarrier.Hga6.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga6.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga6.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga6.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga6.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga6.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga6.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga6.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga6.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga6.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga6.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga6.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch1()) / outputCarrier.Hga6.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch2()) / outputCarrier.Hga6.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch3()) / outputCarrier.Hga6.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch4()) / outputCarrier.Hga6.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch5()) / outputCarrier.Hga6.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga6.Get_Bias_Ch6()) / outputCarrier.Hga6.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga6.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga6.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga6.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga6.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga6.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga6.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga6.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga6.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga6.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga6.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 6)  // HGA7
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga7.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga7.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga7.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga7.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga7.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga7.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga7.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga7.getTAResistance();

                        if (outputCarrier.Hga7.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga7.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga7.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga7.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga7.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga7.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga7.getCh1Temperature() + "_" + outputCarrier.Hga7.getCh2Temperature() + "_" + outputCarrier.Hga7.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga7.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga7.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga7.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga7.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga7.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga7.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga7.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga7.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga7.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga7.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga7.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch1()) / outputCarrier.Hga7.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch2()) / outputCarrier.Hga7.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch3()) / outputCarrier.Hga7.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch4()) / outputCarrier.Hga7.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch5()) / outputCarrier.Hga7.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga7.Get_Bias_Ch6()) / outputCarrier.Hga7.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga7.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga7.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga7.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga7.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga7.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga7.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga7.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga7.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga7.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga7.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 7)  // HGA8
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga8.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga8.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga8.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga8.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga8.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga8.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga8.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga8.getTAResistance();

                        if (outputCarrier.Hga8.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga8.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga8.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga8.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga8.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga8.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga8.getCh1Temperature() + "_" + outputCarrier.Hga8.getCh2Temperature() + "_" + outputCarrier.Hga8.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga8.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga8.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga8.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga8.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga8.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga8.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga8.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga8.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga8.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga8.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga8.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch1()) / outputCarrier.Hga8.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch2()) / outputCarrier.Hga8.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch3()) / outputCarrier.Hga8.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch4()) / outputCarrier.Hga8.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch5()) / outputCarrier.Hga8.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga8.Get_Bias_Ch6()) / outputCarrier.Hga8.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga8.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga8.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga8.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga8.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga8.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga8.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga8.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga8.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga8.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga8.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 8)  // HGA9
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga9.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga9.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga9.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga9.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga9.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga9.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga9.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga9.getTAResistance();

                        if (outputCarrier.Hga9.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga9.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga9.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga9.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga9.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga9.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga9.getCh1Temperature() + "_" + outputCarrier.Hga9.getCh2Temperature() + "_" + outputCarrier.Hga9.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga9.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga9.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga9.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga9.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga9.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga9.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga9.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga9.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga9.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga9.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga9.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch1()) / outputCarrier.Hga9.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch2()) / outputCarrier.Hga9.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch3()) / outputCarrier.Hga9.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch4()) / outputCarrier.Hga9.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch5()) / outputCarrier.Hga9.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga9.Get_Bias_Ch6()) / outputCarrier.Hga9.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga9.get_sdet_writer();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga9.Get_Delta_WR_Hst_Sdet();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga9.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga9.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga9.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga9.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga9.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga9.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga9.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga9.UTIC_DATA.EVENT_DATE;

                    }
                    else if (i == 9)  // HGA10
                    {
                        _datalog.ReaderResistance = outputCarrier.Hga10.getReader1Resistance();
                        _datalog.Reader2Resistance = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga10.getReader2Resistance() : 0;
                        _datalog.DeltaISIReader1 = outputCarrier.Hga10.getDeltaISIReader1();
                        _datalog.DeltaISIReader2 = outputCarrier.Hga10.getDeltaISIReader2();

                        _datalog.WriterResistance = outputCarrier.Hga10.getWriterResistance();
                        _datalog.rHeaterResistance = outputCarrier.Hga10.getRHeaterResistance();
                        _datalog.wHeaterResistance = outputCarrier.Hga10.getWHeaterResistance();
                        _datalog.TAResistance = outputCarrier.Hga10.getTAResistance();

                        if (outputCarrier.Hga10.getShortTest() == ShortDetection.Short)
                        {
                            _datalog.ShortTest = ShortTest.Short;

                        }
                        else if (outputCarrier.Hga10.getShortTest() == ShortDetection.NoTest)
                        {
                            _datalog.ShortTest = ShortTest.NoTest;
                        }
                        else
                        {
                            _datalog.ShortTest = ShortTest.NoShort;

                        }
                        _datalog.ShortTestPosition = outputCarrier.Hga10.getShortPadPosition();
                        _datalog.BiasVoltage = outputCarrier.Hga10.getBiasVoltage();
                        _datalog.BiasCurrent = outputCarrier.Hga10.getBiasCurrent();
                        _datalog.ResistanceSpec = outputCarrier.Hga10.getResistanceSpec();
                        _datalog.ThermisterTemperature = outputCarrier.Hga10.getCh1Temperature() + "_" + outputCarrier.Hga10.getCh2Temperature() + "_" + outputCarrier.Hga10.getCh3Temperature();
                        _datalog.DeltaISIReader1FromTable = outputCarrier.Hga10.DeltaISIResistanceRD1.ToString();
                        _datalog.DeltaISIReader2FromTable = outputCarrier.Hga10.DeltaISIResistanceRD2.ToString();

                        //For new GUI data
                        _datalog.Voltage_Delta1 = outputCarrier.Hga10.Get_Voltage_Delta1();
                        _datalog.Voltage_Delta2 = outputCarrier.Hga10.Get_Voltage_Delta2();

                        //For HAMR *-------------------------------
                        _datalog.LDURes = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? outputCarrier.Hga10.getReader2Resistance().ToString() : "0";
                        _datalog.led_intercept = outputCarrier.Hga10.getLEDIntercept();
                        _datalog.i_threshold = outputCarrier.Hga10.getIThreshold();
                        _datalog.max_v_pd = outputCarrier.Hga10.getMaxVPD();
                        _datalog.ldu_turn_on_voltage = outputCarrier.Hga10.getLDUTurnOnVoltage();
                        _datalog.Sdet_i_threshold = outputCarrier.Hga10.Last_ET_Threshold;
                        _datalog.Delta_i_threshold = outputCarrier.Hga10.Delta_IThreshold;
                        //For HAMR *-------------------------------

                        _datalog.Volt_Ratio_Ch1 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch1()) / outputCarrier.Hga10.Get_Sensing_Ch1());
                        _datalog.Volt_Ratio_Ch2 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch2()) / outputCarrier.Hga10.Get_Sensing_Ch2());
                        _datalog.Volt_Ratio_Ch3 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch3()) / outputCarrier.Hga10.Get_Sensing_Ch3());
                        _datalog.Volt_Ratio_Ch4 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch4()) / outputCarrier.Hga10.Get_Sensing_Ch4());
                        _datalog.Volt_Ratio_Ch5 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch5()) / outputCarrier.Hga10.Get_Sensing_Ch5());
                        _datalog.Volt_Ratio_Ch6 = (Decimal.ToDouble(outputCarrier.Hga10.Get_Bias_Ch6()) / outputCarrier.Hga10.Get_Sensing_Ch6());
                        _datalog.Sdet_Writer = outputCarrier.Hga10.Get_Delta_Writer_Sdet();
                        _datalog.Hst_Sdet_Delta_Writer = outputCarrier.Hga10.get_sdet_writer();
                        _datalog.Wrbridge_Pct = outputCarrier.Hga10.get_wrbridge_pct();
                        _datalog.Wrbridge_Adap_Spec = outputCarrier.Hga10.get_wrbridge_adap_spec();
                        _datalog.Sdet_Reader1 = outputCarrier.Hga10.get_sdet_reader1();
                        _datalog.Sdet_Reader2 = outputCarrier.Hga10.get_sdet_reader2();
                        _datalog.Delta_Polarity_R1 = outputCarrier.Hga10.get_DeltaISI_R1_SDET_Tolerance();
                        _datalog.Delta_Polarity_R2 = outputCarrier.Hga10.get_DeltaISI_R2_SDET_Tolerance();
                        _datalog.Uth_Equip_Id = outputCarrier.Hga10.UTIC_DATA.EQUIP_ID;
                        _datalog.Uth_Time = outputCarrier.Hga10.UTIC_DATA.EVENT_DATE;

                    }

                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog && !UntestedHGA[i])
                    {
                        Log.Info(this, "In UpdateDataLog, Carrier ID: {0}, Slot: {1}, Serial Number: {2}, , HGA Status: {3}, Reader1 Resist: {4}, Reader2 Resist: {18}, DeltaISI Reader1: {19}, DeltaISI Reader2: {20}, Writer Resist: {5}, rHeater Resist: {6}, wHeater Resist: {7}, TA Resist: {8}, Short: {9}, Capa1: {10}, Capa2: {11},Bias Voltage: {12}, Bias Current: {13}, Temperaturee: {14}, Resist Spec: {15}, Capa1 Spec: {16}, Capa2 Spec: {17}, LDU_RES: {21}, LDU_SPEC: {22}",
                            _datalog.CarrierID, _datalog.CarrierSlot, _datalog.HGASerialNumber, _datalog.HGAStatus, _datalog.ReaderResistance, _datalog.WriterResistance,
                            _datalog.rHeaterResistance, _datalog.wHeaterResistance, _datalog.TAResistance, _datalog.ShortTest, _datalog.MicroActuatorCapacitance1, _datalog.MicroActuatorCapacitance2, _datalog.BiasVoltage,
                            _datalog.BiasCurrent, _datalog.ThermisterTemperature, _datalog.ResistanceSpec, _datalog.CapacitanceSpec1, _datalog.CapacitanceSpec2, _datalog.Reader2Resistance, _datalog.DeltaISIReader1, _datalog.DeltaISIReader2, _datalog.LDURes, _datalog.LDUSpec);
                        
                        
                        Log.Info(this, "LED_intercept: {0}, I_Threshold: {1}, Max_V_PD: {2}", _datalog.led_intercept, _datalog.i_threshold, _datalog.max_v_pd);
                    
                    }

                    if (!UntestedHGA[i])
                    {
                        UIUtility.Invoke(HSTMachine.Workcell.getPanelData(), () =>
                        {
                            _workcell.csvFileOutput.AppendNewCSVRecord(_datalog);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Update data error from Output Station processs", ex);
            }
            return;
        }

        public void SaveTDFDataToLocalFile(FolaTagData folaTagData, Carrier carrierData)
        {
            try
            {
                bool[] UntestedHGA = new bool[10];
                bool isIgnoreThisPart = false;
                string ErrorMessageCode = "";

                UntestedHGA[0] = ((carrierData.Hga1.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga1.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[1] = ((carrierData.Hga2.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga2.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[2] = ((carrierData.Hga3.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga3.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[3] = ((carrierData.Hga4.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga4.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[4] = ((carrierData.Hga5.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga5.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[5] = ((carrierData.Hga6.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga6.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[6] = ((carrierData.Hga7.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga7.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[7] = ((carrierData.Hga8.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga8.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[8] = ((carrierData.Hga9.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga9.Hga_Status == HGAStatus.TestedFail)) ? false : true;
                UntestedHGA[9] = ((carrierData.Hga10.Hga_Status == HGAStatus.TestedPass) ||
                                    (carrierData.Hga10.Hga_Status == HGAStatus.TestedFail)) ? false : true;

                //Not save untested part
                isIgnoreThisPart = UntestedHGA[0] && UntestedHGA[1] && UntestedHGA[2] && UntestedHGA[3] && UntestedHGA[4] &&
                            UntestedHGA[5] && UntestedHGA[6] && UntestedHGA[7] && UntestedHGA[8] && UntestedHGA[9];

                if (!isIgnoreThisPart)
                {
                    string tdfFileName = string.Empty;
                    tdfFileName = TDFOutput.PreTDFName + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2") + TDFOutput.PosTDFName;
                    if (IsFileNameExisting(tdfFileName))
                        tdfFileName = GetBackupFileName();

                    _workcell.TDFOutputObj.IsInProcessing = false;
                    _workcell.TDFOutputObj = new TDFOutput(HSTSettings.Instance.Directory.TDFLocalDataPath, tdfFileName);
                    var hgaData = new Hga(1, HGAStatus.Unknown);
                    
                    //Start date for reader1 and reader2 have to difference for FIS condition
                    var start_date_reader1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    Stopwatch timeout = new Stopwatch();
                    bool endloop = false;
                    timeout.Reset();
                    timeout.Start();
                    while (start_date_reader1.Second == DateTime.Now.Second && !endloop)
                    {
                        Thread.Sleep(100);
                        if (timeout.ElapsedTime_sec > 10)
                            endloop = true;
                    }
                    var start_date_reader2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    var save_date = DateTime.Now.ToString("dd-MMM-yy:HH:mm:ss").ToUpper();


                    for (int slot = 0; slot < carrierData.RFIDData.RFIDTagData.CarrierSize; slot++)
                    {
                        switch (slot)
                        {
                            case 0:
                                hgaData = carrierData.Hga1;
                                break;
                            case 1:
                                hgaData = carrierData.Hga2;
                                break;
                            case 2:
                                hgaData = carrierData.Hga3;
                                break;
                            case 3:
                                hgaData = carrierData.Hga4;
                                break;
                            case 4:
                                hgaData = carrierData.Hga5;
                                break;
                            case 5:
                                hgaData = carrierData.Hga6;
                                break;
                            case 6:
                                hgaData = carrierData.Hga7;
                                break;
                            case 7:
                                hgaData = carrierData.Hga8;
                                break;
                            case 8:
                                hgaData = carrierData.Hga9;
                                break;
                            case 9:
                                hgaData = carrierData.Hga10;
                                break;
                        }

                        string readerIndex = "";
                        double readerReading = 0;
                        DateTime assignedDate = new DateTime();
                        string isi_res_at_et = string.Empty;
                        string isi_asym_at_et = string.Empty;
                        string isi_amp_at_et = string.Empty;
                        string rd_res_sdet = string.Empty;
                        string delta_isi_res = string.Empty;

                        //There is no way that 2 chanel have been diabled the same time
                        if (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 0 &&
                            CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 0)
                        {
                            CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 = 1;
                            CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 = 1;
                        }

                        for (int sent = 0; sent < 2; sent++)
                        {
                            if(sent == 0)
                            {
                                assignedDate = start_date_reader1;
                                rd_res_sdet = hgaData.get_sdet_reader1().ToString("F3");
                                delta_isi_res = hgaData.getDeltaISIReader1().ToString("F3");
                            }else
                            {
                                assignedDate = start_date_reader2;
                                rd_res_sdet = hgaData.get_sdet_reader2().ToString("F3");
                                delta_isi_res = hgaData.getDeltaISIReader2().ToString("F3");
                            }

                            if (sent == 0 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                readerReading = GetReaderValue(carrierData.CarrierID,
                                    carrierData.RFIDData.RFIDTagData.HGAData[slot].HgaSN, hgaData, 1);
                                readerIndex = "1";
                                isi_res_at_et = hgaData.ISI_RES_AT_ET;
                                isi_asym_at_et = hgaData.ISI_ASYM_AT_ET;
                                isi_amp_at_et = hgaData.ISI_AMP_AT_ET;
                            }
                            if (sent == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                if(!CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                {
                                    readerReading = GetReaderValue(carrierData.CarrierID,
                                        carrierData.RFIDData.RFIDTagData.HGAData[slot].HgaSN, hgaData, 2);
                                    isi_res_at_et = hgaData.ISI_RES_AT_ET_RD2;
                                    isi_asym_at_et = hgaData.ISI_ASYM_AT_ET_RD2;
                                    isi_amp_at_et = hgaData.ISI_AMP_AT_ET_RD2;
                                }
                                else
                                {
                                    readerReading = 0.0;
                                    isi_res_at_et = "0";
                                    isi_asym_at_et = "0";
                                    isi_amp_at_et = "0";
                                }
                                readerIndex = "2";
                            }

                            //Re-check date time format
                            var currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            var diffTime = currentTime - assignedDate;
                            if(diffTime.Minutes > 10)
                            {
                                assignedDate = currentTime;
                            }

                            //TDF HST Data

                            //Get TST-STATUS
                            var get_tst_status = string.Empty;
                            if (hgaData.TST_STATUS.Equals('\0') || hgaData.TST_STATUS.Equals(string.Empty) || hgaData.TST_STATUS.Equals('0'))
                            {
                                get_tst_status = string.Empty;
                                hgaData.Hga_Status = HGAStatus.TestedFail;
                                hgaData.Error_Msg_Code = ERROR_MESSAGE_CODE.FAILGETSORT.ToString();
                            }
                            else
                            {
                                if (GetHGAPartStatus(slot, carrierData))
                                    get_tst_status = hgaData.TST_STATUS.Equals('\0') ? string.Empty : hgaData.TST_STATUS.ToString();
                                else
                                    get_tst_status = "F";
                            }

                            //Get VALUE86
                            var get_value_86 = string.Empty;
                            if (hgaData.TST_STATUS.Equals('\0') || hgaData.TST_STATUS.ToString() == string.Empty)
                            {
                                get_value_86 = "-1";
                            }
                            else
                            {
                                if (hgaData.TST_STATUS == CommonFunctions.TEST_FAIL_CODE)
                                    get_value_86 = "-2";
                                else
                                    get_value_86 = hgaData.TST_STATUS.Equals('\0') ? string.Empty : hgaData.TST_STATUS.ToString();
                            }

                            string errorcode = string.Empty;
                            if (!GetHGAPartStatus(slot, carrierData))
                            {
                                var splitErrCode = hgaData.getShortPadPosition();
                                errorcode = string.Join("", splitErrCode);
                            }

                            //Get VALUE11
                            var get_value_11 = string.Empty;
                            var get_isi_data = 0.00;
                            if (sent == 0 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1)
                            {
                                get_isi_data = hgaData.DeltaISIResistanceRD1;
                            }
                            if (sent == 1 && CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1)
                            {
                                get_isi_data = hgaData.DeltaISIResistanceRD2;
                            }

                            if (hgaData.Error_Msg_Code == ERROR_MESSAGE_CODE.CRDL.ToString())
                            {
                                if (get_isi_data == null || get_isi_data == 0.0)
                                {
                                    get_value_11 = "-1";
                                }
                                else
                                {
                                    if (get_isi_data == -1)
                                        get_value_11 = "-2";
                                    else if (get_isi_data < -1)
                                        get_value_11 = "-4";

                                    var get_tab = _loadData.WorkOrder.Substring(_loadData.WorkOrder.Length - 1, 1);
                                    if (hgaData.ISI_TAB != "" && (get_tab != hgaData.ISI_TAB))
                                        get_value_11 = "-3";
                                }
                            }
                            else
                            {
                                if (get_isi_data == null || get_isi_data == 0.0)
                                    get_value_11 = "-1";
                                else
                                    get_value_11 = "1";
                            }

                            ////Get CTQ_DELTA_ISI
                            var get_ctq_delta_isi = string.Empty;
                            if (hgaData.Error_Msg_Code != ERROR_MESSAGE_CODE.CRDL.ToString())
                            {
                                if (get_isi_data > 1)
                                    get_ctq_delta_isi = "1";
                                else
                                    get_ctq_delta_isi = "-1";
                            }
                            else
                            {
                                if (get_isi_data > 1)
                                    get_ctq_delta_isi = "2";
                                else if(get_isi_data == -1)
                                    get_ctq_delta_isi = "-2";
                                else if (get_isi_data < -1)
                                    get_ctq_delta_isi = "-4";
                                else if (get_isi_data == 0.0)
                                    get_ctq_delta_isi = "-1";
                                else
                                {
                                    if(get_value_11 == "-3")
                                        get_ctq_delta_isi = "-3";
                                }
                            }

                            //Get name24 for LDU information
                            var get_name_24 = "LAS_IV_RES_OHM";
                            var get_value_24 = string.Empty;
                            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                                get_value_24 = hgaData.getReader2Resistance().ToString("F3");

                            var get_name_25 = "LED_INTCP";
                            var get_value_25 = string.Empty;
                            get_value_25 = hgaData.getLEDIntercept().ToString("F3");

                            var get_name_26 = "LAS_ITHRESHOLD_MA";
                            var get_value_26 = string.Empty;
                                get_value_26 = hgaData.get_ldu_Threshold_Ma().ToString("F3");

                            var get_name_27 = "LAS_PD_VOLT";
                            var get_value_27 = string.Empty;
                                get_value_27 = hgaData.getMaxVPD().ToString("F3");

                            var get_name_28 = "DELTA_ITHRESHOLD";
                            var get_value_28 = string.Empty;
                                get_value_28 = hgaData.Delta_IThreshold.ToString("F3");

                            var get_name_29 = "SDET_ITHRESHOLD";
                            var get_value_29 = string.Empty;
                            get_value_29 = hgaData.Last_ET_Threshold.ToString();

                            var get_name_30 = "LAS_VOLTINTCP";
                            var get_value_30 = string.Empty;
                                get_value_30 = hgaData.getLDUTurnOnVoltage().ToString("F3");

                            var get_name_31 = "VOLT_RATIO_CH1_WRITER";
                            var get_value_31 = string.Empty;
                                get_value_31 =(Decimal.ToDouble(hgaData.Get_Bias_Ch1())/hgaData.Get_Sensing_Ch1()).ToString("F3");

                            var get_name_32 = "VOLT_RATIO_CH2_TA";
                            var get_value_32 = string.Empty;
                            get_value_32 = (Decimal.ToDouble(hgaData.Get_Bias_Ch2())/hgaData.Get_Sensing_Ch2()).ToString("F3");

                            var get_name_33 = "VOLT_RATIO_CH3_WHEATER";
                            var get_value_33 = string.Empty;
                            get_value_33 = (Decimal.ToDouble(hgaData.Get_Bias_Ch3())/hgaData.Get_Sensing_Ch3()).ToString("F3");

                            var get_name_34 = "VOLT_RATIO_CH4_RHEATER";
                            var get_value_34 = string.Empty;
                            get_value_34 = (Decimal.ToDouble(hgaData.Get_Bias_Ch4())/hgaData.Get_Sensing_Ch4()).ToString("F3");

                            var get_name_35 = "VOLT_RATIO_CH5_READER1";
                            var get_value_35 = string.Empty;
                            get_value_35 = (Decimal.ToDouble(hgaData.Get_Bias_Ch5())/hgaData.Get_Sensing_Ch5()).ToString("F3");

                            var get_name_36 = "VOLT_RATIO_CH6_READER2";
                            var get_value_36 = string.Empty;
                            get_value_36 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? (Decimal.ToDouble(hgaData.Get_Bias_Ch6())/hgaData.Get_Sensing_Ch6()).ToString("F3"):"0";

                            var get_name_37 = "VOLT_DELTA_1";
                            var get_value_37 = string.Empty;
                            get_value_37 = hgaData.Get_Voltage_Delta1().ToString();

                            var get_name_38 = "VOLT_DELTA_2";
                            var get_value_38 = string.Empty;
                            get_value_38 = hgaData.Get_Voltage_Delta2().ToString();

                            var get_name_39 = "WRITER_RES_BEFORE_OFFSET";
                            var get_value_39 = string.Empty;
                                get_value_39 = (hgaData.getWRResBeforeOffset() / 1000).ToString("F5");

                            var get_name_40 = "TA_RES_BEFORE_OFFSET";
                            var get_value_40 = string.Empty;
                            get_value_40 = (hgaData.getTAResBeforeOffset() / 1000).ToString("F5");

                            var get_name_41 = "WHEATER_RES_BEFORE_OFFSET";
                            var get_value_41 = string.Empty;
                                get_value_41 = (hgaData.getWHResBeforeOffset() / 1000).ToString("F5");

                            var get_name_42 = "RHEATER_RES_BEFORE_OFFSET";
                            var get_value_42 = string.Empty;
                                get_value_42 = (hgaData.getRHResBeforeOffset() / 1000).ToString("F5");

                            var get_name_43 = "READER1_RES_BEFORE_OFFSET";
                            var get_value_43 = string.Empty;
                                get_value_43 = (hgaData.getR1ResBeforeOffset() / 1000).ToString("F5");

                            var get_name_44 = "READER2_RES_BEFORE_OFFSET";
                            var get_value_44 = string.Empty;
                                get_value_44 = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable ? (hgaData.getR2ResBeforeOffset() / 1000).ToString() : "0";

                            var get_name_93 = "WRITER_SDET";
                            var get_value_93 = string.Empty;
                                get_value_93 = hgaData.get_sdet_writer().ToString("F3");

                            var get_name_94 = "DEL_WRT_HST_SDET";
                            var get_value_94 = string.Empty;
                                get_value_94 = hgaData.Get_Delta_WR_Hst_Sdet().ToString("F3");

                            var get_name_95 = "WRBRIDGE_PCT";
                            var get_value_95 = string.Empty;
                                get_value_95 = hgaData.Get_WRBridge_Percentage().ToString("F3");

                            var get_name_96 = "RD_RES_SDET";
                            var get_value_96 = string.Empty;
                                get_value_96 = rd_res_sdet;

                            var get_name_97 = "WRBRIDGE_ADAP_SPEC";
                            var get_value_97 = string.Empty;
                                get_value_97 = hgaData.get_wrbridge_adap_spec().ToString("F3");

                            var get_name_98 = "DEL_POLARITY_R1";
                            var get_value_98 = string.Empty;
                            get_value_98 = hgaData.get_DeltaISI_R1_SDET_Tolerance().ToString("F3");

                            var get_name_99 = "DEL_POLARITY_R2";
                            var get_value_99 = string.Empty;
                            get_value_99 = hgaData.get_DeltaISI_R2_SDET_Tolerance().ToString("F3");

                            var get_name_100 = "ANC_YIELD";
                            var get_value_100 = string.Empty;
                            get_value_100 = hgaData.get_anc_yield().ToString("F3");

                            var get_name_101 = "ANC_HGA_COUNT";
                            var get_value_101 = string.Empty;
                            get_value_101 = hgaData.get_anc_hga_count().ToString("F3");

                            var get_name_102 = "UTH_EVENT_DATE";
                            var get_value_102 = string.Empty;
                            {
                                var convertToDate = Convert.ToDateTime(hgaData.UTIC_DATA.EVENT_DATE);
                                get_value_102 = string.Format("{0}{1}{2}{3}{4}{5}", convertToDate.Year,
                                    convertToDate.Month, convertToDate.Day,
                                    convertToDate.Hour, convertToDate.Minute, convertToDate.Second);
                            }

                            var get_name_103 = "UTH_DOCKSIDE";
                            var get_value_103 = string.Empty;
                            get_value_103 = hgaData.UTIC_DATA.DOCK_SIDE;

                            var get_name_104 = "TGA_PART_NUM";
                            var get_value_104 = string.Empty;
                            get_value_104 = carrierData.WorkOrderData.FsaPartNo;

                            var get_name_105 = "GUASS_METER1";
                            var get_value_105 = string.Empty;
                            get_value_105 = hgaData.get_gauss_meter1().ToString("F3");

                            var get_name_106 = "GUASS_METER2";
                            var get_value_106 = string.Empty;
                            get_value_106 = hgaData.get_gauss_meter2().ToString("F3");

                            var get_name_107 = "MAGNET_CURRENT";
                            var get_value_107 = string.Empty;
                            get_value_107 = "0.00";

                            var get_name_12 = "DEL_ISI_RES_HST";
                            var get_value_12 = string.Empty;
                                get_value_12 = delta_isi_res;

                            var err_cd = string.Empty;
                            ErrorMessageCode = hgaData.Error_Msg_Code;
                            if (ErrorMessageCode.EndsWith("; "))
                            {
                                ErrorMessageCode.Substring(0, ErrorMessageCode.Length - 2);
                            }
                            err_cd = ErrorMessageCode.Replace("; ", "_");

                            //CTQ_WRT_RES
                            string ctp_wrt_res = String.Empty;
                            if ((hgaData.getWriterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                hgaData.getWriterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                                ctp_wrt_res = hgaData.getWriterResistance().ToString("F3");

                            //CTQ_RD_RES
                            string ctq_rd_res = String.Empty;
                            if ((readerReading > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                    readerReading < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                                ctq_rd_res = readerReading.ToString("F3");

                            //CTQ_HTR2_RES
                            string ctp_htr2_res = String.Empty;
                            if ((hgaData.getRHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                    hgaData.getRHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                                ctp_htr2_res = hgaData.getRHeaterResistance().ToString("F3");

                            //CTQ_TA_RES
                            string ctq_ta_res = String.Empty;
                            if ((hgaData.getTAResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                    hgaData.getTAResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                                ctq_ta_res = hgaData.getTAResistance().ToString("F3");

                            //CTQ_HTR_RES
                            string ctq_htr_res = String.Empty;
                            if ((hgaData.getWHeaterResistance() > CommonFunctions.Instance.MeasurementTestRecipe.MinReportSpec) &&
                                    hgaData.getWHeaterResistance() < CommonFunctions.Instance.MeasurementTestRecipe.MaxReportSpec)
                                ctq_htr_res = hgaData.getWHeaterResistance().ToString("F3");

                            var hstTdfData = new TDFOutput.HSTTDFDATA
                            {
                                HD_NUM = folaTagData.HGAData[slot].HgaSN,
                                TST_DATE = _workcell.TDFOutputObj.GetCurrentDateTimeByFormat(assignedDate),
                                LOCATION = "BKK",
                                TSTR_PART_NUM = _loadData.EquipmentID,
                                HGA_PART_NUM = _loadData.HGAPartNumber,
                                FSA_PART_NUM = _loadData.FSAPartNumber,
                                TMWI = HSTMachine.Workcell._workOrder.Loading.TMWIP,
                                COMMENTS = "HST",
                                TST_STATUS = get_tst_status,
                                RSTAT = folaTagData.HGAData[slot].Status.ToString(),
                                PART_DISPOSITION_CODE = GetHGAPartStatus(slot, carrierData) == true ? "SHIP" : "FAIL",
                                SPC_STD = "R",
                                DISK_NUM = "NO_DISK",
                                OPER_ID = _loadData.LoginUser,
                                MR_RES = ctq_rd_res,
                                CTQ_ISLTN = GetHGAPartStatus(slot, carrierData) == true ? "0" : "1",
                                CTQ_ISLTN_FLAG = hgaData.getShortTest() == ShortDetection.Short ? "0" : "1",

                                CTQ_WRT_RES = ctp_wrt_res,
                                CTQ_RD_RES = ctq_rd_res,
                                CTQ_HTR2_RES = ctp_htr2_res,
                                CTQ_TA_RES = ctq_ta_res,
                                CTQ_HTR_RES = ctq_htr_res,
                                CTQ_DELTA_ISI = get_ctq_delta_isi,
                                NAME86 = "DISPOS_FLG",
                                VALUE86 = get_value_86,
                                NAME24 = get_name_24,
                                VALUE24 = get_value_24,
                                NAME25 = get_name_25,
                                VALUE25 = get_value_25,
                                NAME26 = get_name_26,
                                VALUE26 = get_value_26,
                                NAME27 = get_name_27,
                                VALUE27 = get_value_27,
                                NAME28 = get_name_28,
                                VALUE28 = get_value_28,
                                NAME29 = get_name_29,
                                VALUE29 = get_value_29,
                                NAME30 = get_name_30,
                                VALUE30 = get_value_30,
                                NAME31 = get_name_31,
                                VALUE31 = get_value_31,
                                NAME32 = get_name_32,
                                VALUE32 = get_value_32,
                                NAME33 = get_name_33,
                                VALUE33 = get_value_33,
                                NAME34 = get_name_34,
                                VALUE34 = get_value_34,
                                NAME35 = get_name_35,
                                VALUE35 = get_value_35,
                                NAME36 = get_name_36,
                                VALUE36 = get_value_36,
                                NAME37 = get_name_37,
                                VALUE37 = get_value_37,
                                NAME38 = get_name_38,
                                VALUE38 = get_value_38,
                                NAME39 = get_name_39,
                                VALUE39 = get_value_39,
                                NAME40 = get_name_40,
                                VALUE40 = get_value_40,
                                NAME41 = get_name_41,
                                VALUE41 = get_value_41,
                                NAME42 = get_name_42,
                                VALUE42 = get_value_42,
                                NAME43 = get_name_43,
                                VALUE43 = get_value_43,
                                NAME44 = get_name_44,
                                VALUE44 = get_value_44,
                                NAME12 = get_name_12,
                                VALUE12 = get_value_12,
                                NAME93 = get_name_93,
                                VALUE93 = get_value_93,
                                NAME94 = get_name_94,
                                VALUE94 = get_value_94,
                                NAME95 = get_name_95,
                                VALUE95 = get_value_95,
                                NAME96 = get_name_96,
                                VALUE96 = get_value_96,
                                NAME97 = get_name_97,
                                VALUE97 = get_value_97,
                                NAME98 = get_name_98,
                                VALUE98 = get_value_98,
                                NAME99 = get_name_99,
                                VALUE99 = get_value_99,
                                NAME100 = get_name_100,
                                VALUE100 = get_value_100,
                                NAME101 = get_name_101,
                                VALUE101 = get_value_101,
                                NAME102 = get_name_102,
                                VALUE102 = string.IsNullOrEmpty(get_value_102)?string.Empty: get_value_102,
                                NAME103 = get_name_103,
                                VALUE103 = string.IsNullOrEmpty(get_value_103)?string.Empty: get_value_103,
                                NAME104 = get_name_104,
                                VALUE104 = get_value_104,
                                NAME105 = get_name_105,
                                VALUE105 = get_value_105,
                                NAME106 = get_name_106,
                                VALUE106 = get_value_106,
                                NAME107 = get_name_107,
                                VALUE107 = get_value_107,
                                POS_ID = string.Format("C{0}P{1}", folaTagData.HGAData.Length.ToString(), hgaData.Index.ToString()),
                                ERR_CDS1 = "",
                                ERR_CDS2 = "",
                                ERR_CDS3 = "",
                                ERR_CDS4 = "",
                                ERR_CDS5 = "",
                                ERR_CDS6 = "",
                                ERR_CDS7 = "",
                                ERR_CDS8 = "",

                                //Temperary send blank data first
                                //ERR_CDS1 = hgaData.TST_STATUS.Equals('1') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS2 = hgaData.TST_STATUS.Equals('2') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS3 = hgaData.TST_STATUS.Equals('3') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS4 = hgaData.TST_STATUS.Equals('4') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS5 = hgaData.TST_STATUS.Equals('5') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS6 = hgaData.TST_STATUS.Equals('6') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS7 = hgaData.TST_STATUS.Equals('7') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //ERR_CDS8 = hgaData.TST_STATUS.Equals('8') == true && GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode,
                                //***************************

                                CELL_NUM = _loadData.Location,
                                BUILD_NUM = _loadData.WorkOrder,
                                WORK_ORDER = _loadData.WorkOrder,
                                WORK_ORDER_VER = _loadData.WorkOrderVersion,
                                TRAY_ID = _loadData.CarrierID,
                                TEST_TYPE = "PRODUCTION",
                                START_DATE = _workcell.TDFOutputObj.GetCurrentDateTimeByFormat(assignedDate),
                                TIME = DisplayFormatter.Round(_workcell.TestTimePerHead, DisplayFormatter.DecimalDigits.Three),
                                SWAP = DisplayFormatter.Round(_workcell.SwapTimeCarrier, DisplayFormatter.DecimalDigits.Three),
                                SOFT_REV = _workcell.SwVersion,
                                READER_INDEX = readerIndex,
                                ERR_CD = err_cd,
                                ERR_CDH = err_cd,
                                WRITER_CHOICE_FLAG = hgaData.IBS_Data.CurentChoiceFlag,                                
                            };

                            //TDF FIS Data
                            var fisTDFData = new TDFOutput.FISTDFDATA
                            {
                                SPEC_NO = DisplayFormatter.Round(CommonFunctions.Instance.MeasurementTestRecipe.SpecNumber, DisplayFormatter.DecimalDigits.Zero),
                                SPEC_VER = CommonFunctions.Instance.MeasurementTestRecipe.SpecVersion,
                                PARM_ID = CommonFunctions.Instance.MeasurementTestRecipe.ParamID,
                                SCRIPT_NAME = CommonFunctions.Instance.MeasurementTestRecipe.ScriptName,
                                SCRIPT_DATE = CommonFunctions.Instance.MeasurementTestRecipe.ScriptDate.ToUpper(),
                                TSR_NUM = CommonFunctions.Instance.MeasurementTestRecipe.TSRNumber,
                                TSR_GROUP = CommonFunctions.Instance.MeasurementTestRecipe.TSRGroup,
                                SLIDER_LOT_ID = hgaData.Slider_Lot_Number != null ? hgaData.Slider_Lot_Number : string.Empty,
                                RADIUS = CommonFunctions.Instance.MeasurementTestRecipe.Radius,
                                REF_RADIUS = Convert.ToDouble(CommonFunctions.Instance.MeasurementTestRecipe.Radius),
                                RPM = Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.RPM),
                                REF_RPM = Convert.ToDouble(CommonFunctions.Instance.MeasurementTestRecipe.RPM),
                                SKEW_ANGLE = CommonFunctions.Instance.MeasurementTestRecipe.SkewAngle,
                                LOAD_RADIUS = DisplayFormatter.Round(CommonFunctions.Instance.MeasurementTestRecipe.Radius, DisplayFormatter.DecimalDigits.Three),
                                WAF_TAD_RES = hgaData.ISI_WAF_TAD_RES != null ? hgaData.ISI_WAF_TAD_RES : string.Empty,
                                WAF_RDR_HTR_RES = hgaData.ISI_WAF_RDR_HTR_RES != null ? hgaData.ISI_WAF_RDR_HTR_RES : string.Empty,
                                WAF_WTR_RES = hgaData.ISI_WAF_WTR_RES != null ? hgaData.ISI_WAF_WTR_RES : string.Empty,
                                WAF_WTR_HTR_RES = hgaData.ISI_WAF_WTR_HTR_RES != null ? hgaData.ISI_WAF_WTR_HTR_RES : string.Empty,
                                ISI_RES_AT_ET = isi_res_at_et != null ? isi_res_at_et : string.Empty,
                                ISI_AMP_AT_ET = isi_amp_at_et != null ? isi_amp_at_et : string.Empty,
                                ISI_ASYM_AT_ET = isi_asym_at_et != null ? isi_asym_at_et : string.Empty
                            };

                            if (!UntestedHGA[slot])
                            {
                                _workcell.TDFOutputObj.IsInProcessing = true;
                                _workcell.TDFOutputObj.UpdateHstTdfData(hstTdfData);
                                _workcell.TDFOutputObj.UpdateFisTdfData(fisTDFData);
                                _workcell.TDFOutputObj.SaveTDFOutputToFile();
                                _workcell.TDFOutputObj.IsInProcessing = false;
                            }

                            Log.Info(this, "ERR_CDS={0},ERR_CDH={1}", GetHGAPartStatus(slot, carrierData) == true ? "PASS" : errorcode, hgaData.Error_Msg_Code);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Save TDF file error", ex);
            }
        }

        public bool IsFileNameExisting(string filename)
        {
            var totalName = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFLocalDataPath + "\\" + filename + TDFOutput.PosTDFName);
            return File.Exists(totalName);
        }

        public bool GetHGAPartStatus(int hgaslot, Carrier outputcarrier)
        {
            bool returnStatus = false;
            string sn = outputcarrier.RFIDData.RFIDTagData.HGAData[hgaslot].HgaSN;
            HGAStatus status = HGAStatus.Unknown;
            switch (hgaslot)
            {
                case 0:
                    if (outputcarrier.Hga1.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga1.Hga_Status;
                    break;
                case 1:
                    if (outputcarrier.Hga2.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga2.Hga_Status;
                    break;
                case 2:
                    if (outputcarrier.Hga3.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga3.Hga_Status;
                    break;
                case 3:
                    if (outputcarrier.Hga4.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga4.Hga_Status;
                    break;
                case 4:
                    if (outputcarrier.Hga5.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga5.Hga_Status;
                    break;
                case 5:
                    if (outputcarrier.Hga6.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga6.Hga_Status;
                    break;
                case 6:
                    if (outputcarrier.Hga7.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga7.Hga_Status;
                    break;
                case 7:
                    if (outputcarrier.Hga8.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga8.Hga_Status;
                    break;
                case 8:
                    if (outputcarrier.Hga9.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga9.Hga_Status;
                    break;
                case 9:
                    if (outputcarrier.Hga10.Hga_Status == HGAStatus.TestedPass) returnStatus = true;
                    status = outputcarrier.Hga10.Hga_Status;
                    break;
                default:
                    break;
            }

            Log.Info(this, "Try to log why status fail, Serialnumber: {0}, HGA Status: {1}", sn, status.ToString());

            return returnStatus;
        }

        public double GetReaderValue(string carrierID,string sn, Hga hgaData, int number)
        {
            double getReading = 0.0;
            try
            {
                var getbyCarrier = _workcell.TestedDataMaps.Where(P => P.Key == carrierID).FirstOrDefault().Value;
                var getbySlot = getbyCarrier.Where(S => S.SN == sn).FirstOrDefault();

                if (number == 1)
                {
                    getReading = hgaData.getReader1Resistance();

                    if (getReading == 0 || getReading == 0.0)
                    {
                        if (getbySlot.Reading_RD1 != 0)
                            getReading = getbySlot.Reading_RD1;
                    }
                }
                else if (number == 2)
                {
                    getReading = hgaData.getReader2Resistance();
                    if (getReading == 0 || getReading == 0.0)
                    {
                        if (getbySlot.Reading_RD2 != 0)
                            getReading = getbySlot.Reading_RD2;
                    }
                }
            }
            catch (Exception)
            {
                if (number == 1)
                {
                    getReading = hgaData.getReader1Resistance();
                }
                else if (number == 2)
                {
                    getReading = hgaData.getReader2Resistance();
                }
            }

            return getReading;
        }

        public string GetBackupFileName()
        {
            string returnFileName = string.Empty;
            bool goodFileName = false;

            while (!goodFileName)
            {
                _workcell.TDFOutputObj.BackupNameCounter++;
                if (_workcell.TDFOutputObj.BackupNameCounter > 999999)
                    _workcell.TDFOutputObj.BackupNameCounter = 1;

                returnFileName = TDFOutput.PreTDFName + _workcell.TDFOutputObj.BackupNameCounter.ToString("D6");
                if (IsFileNameExisting(returnFileName))
                    Thread.Sleep(100);
                else
                    goodFileName = true;
            }

            return returnFileName;
        }


    }
}
