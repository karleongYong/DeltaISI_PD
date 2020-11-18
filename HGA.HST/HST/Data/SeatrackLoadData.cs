using System;
using System.Collections.Generic;
using System.Text;
using FisApi;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    public class SeatrackLoadData
    {
        private string eventDate = "";
        private string carrierID = "";
        private string carrierSlot = "";
        private string serialNo = "";
        private string status = "";
        private string workOrder = "";
        private string errorMessageCode = "0";
        private string setupFile = "";
        private string readerResistance = "";
        private string reader2Resistance = "";
        private string writerResistance = "";
        private string rHeaterResistance = "";
        private string wHeaterResistance = "";
        private string tAResistance = "";
        private int shortTest = 0;
        private string shortTestPosition = "";
        private double uACTCapacitance1 = 0.0;
        private double uACTCapacitance2 = 0.0;
        private string temperatureBoard = "";
        private double biasVoltageMeasure = 0.0;
        private double biasCurrentSource = 0.0;
        private string resistanceSpecification = "";
        private string capacitanceSpecification1 = "";
        private string capacitanceSpecification2 = "";
        private string location = "";
        private string equipmentID = "";
        private string equipmentType = "";
        private double xAxis = 0.0;
        private double yAxis = 0.0;
        private double zAxis = 0.0;
        private double thetaAxis = 0.0;
        private int softwareStatus = 0;        
        private int operationMode = 0;
        private string workOrderVersion = "";
        private string hgaSerialNumber = "";
        private string hgapartNumber = "";

        private string fsapartNumber = "";
        private string loginUser = "";
        private int uph = 0;
        private double cycleTime = 0.0;

        private string _status_code = String.Empty;
        private string _ibs_check = String.Empty;
        private string _rd_ibs_pattern = String.Empty;
        private string _wr_ibs_pattern = String.Empty;
        private string _tic_equip_id = String.Empty;
        private string _tic_time = string.Empty;
        private string _tic_dockside = string.Empty;
        private double _isi_reader1 = 0.0;
        private double _isi_reader2 = 0.0;
        private double _delta_r1 = 0.0;
        private double _delta_r2 = 0.0;
        private double _ldu_res = 0.0;
        private double _ldu_res_spec_min = 0.0;
        private double _ldu_res_spec_max = 0.0;
        private double _ldu_threshold_ma = 0.0;
        private double _ldu_volt_intercept = 0.0;
        private double _ldu_threshold_spec_min = 0.0;
        private double _ldu_threshold_spec_max = 0.0;
        private double _ldu_sweep_spec_min = 0.0;
        private double _ldu_sweep_spec_max = 0.0;

        private int _get_bias_ch1 = 0;
        private int _get_bias_ch2 = 0;
        private int _get_bias_ch3 = 0;
        private int _get_bias_ch4 = 0;
        private int _get_bias_ch5 = 0;
        private int _get_bias_ch6 = 0;
        private int _get_sensing_ch1 = 0;
        private int _get_sensing_ch2 = 0;
        private int _get_sensing_ch3 = 0;
        private int _get_sensing_ch4 = 0;
        private int _get_sensing_ch5 = 0;
        private int _get_sensing_ch6 = 0;
        private double _volt_ratio_ch1 = 0.0;
        private double _volt_ratio_ch2 = 0.0;
        private double _volt_ratio_ch3 = 0.0;
        private double _volt_ratio_ch4 = 0.0;
        private double _volt_ratio_ch5 = 0.0;
        private double _volt_ratio_ch6 = 0.0;
        private double _voltage_delta1 = 0.0;
        private double _voltage_delta2 = 0.0;

        private char _sdet_tst_status = Char.MaxValue;

        private double _delta_writer_sdet = 0.0;
        private double _delta_writer_hst_sdet = 0.0;
        private double _wrbridge_percentage = 0.0;

        // added 3-Feb-2020
        private double _led_intercept = 0.0;
        private double _led_intercept_spec = 0.0;
        private double _led_intercept_spec_max = 0.0;
        private double _pd_voltage = 0.0;
        private double _pd_voltage_spec_min = 0.0;
        private double _pd_voltage_spec_max = 0.0;
        private double _sdet_ithreshold = 0.0;
        private double _delta_ithreshold = 0.0;

        private double _sdet_writer = 0.0;
        private double _hst_sdet_delta_writer = 0.0;

        private double _wrbridge_pct = 0.0;

        private double _wrbridge_adap_spec = 0.0;
        private double _sdet_reader1 = 0.0;
        private double _sdet_reader2 = 0.0;
        private double _gauss_meter1 = 0.0;
        private double _gauss_meter2 = 0.0;
        private double _anc_yield = 0.0;
        private double _anc_hga_count  = 0.0;
        private double _DeltaISI_R2_SDET_Tolerance = 0;
        private double _DeltaISI_R1_SDET_Tolerance = 0;


        #region Properties
        public string EventDate
        {
            get { return eventDate; }
            set { eventDate = value; }
        }

        public string CarrierID
        {
            get { return carrierID; }
            set { carrierID = value; }
        }

        public string CarrierSlot
        {
            get { return carrierSlot; }
            set { carrierSlot = value; }
        }

        public string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string WorkOrder
        {
            get { return workOrder; }
            set { workOrder = value; }
        }

        public string ErrorMessageCode
        {
            get { return errorMessageCode; }
            set { errorMessageCode = value; }
        }

        public string SetupFile
        {
            get { return setupFile; }
            set { setupFile = value; }
        }

        public string ReaderResistance
        {
            get { return readerResistance; }
            set { readerResistance = value; }
        }

        public string Reader2Resistance
        {
            get { return reader2Resistance; }
            set { reader2Resistance = value; }
        }
        public string WriterResistance
        {
            get { return writerResistance; }
            set { writerResistance = value; }
        }

        public string RHeaterResistance
        {
            get { return rHeaterResistance; }
            set { rHeaterResistance = value; }
        }

        public string WHeaterResistance
        {
            get { return wHeaterResistance; }
            set { wHeaterResistance = value; }
        }

        public string TAResistance
        {
            get { return tAResistance; }
            set { tAResistance = value; }
        }

        public int ShortTest
        {
            get { return shortTest; }
            set { shortTest = value; }
        }

        public string ShortTestPosition
        {
            get { return shortTestPosition; }
            set { shortTestPosition = value; }
        }

        public double UACTCapacitance1
        {
            get { return uACTCapacitance1; }
            set { uACTCapacitance1 = value; }
        }

        public double UACTCapacitance2
        {
            get { return uACTCapacitance2; }
            set { uACTCapacitance2 = value; }
        }
        public string TemperatureBoard
        {
            get { return temperatureBoard; }
            set { temperatureBoard = value; }
        }

        public double BiasVoltageMeasure
        {
            get { return biasVoltageMeasure; }
            set { biasVoltageMeasure = value; }
        }

        public double BiasCurrentSource
        {
            get { return biasCurrentSource; }
            set { biasCurrentSource = value; }
        }

        public string ResistanceSpecification
        {
            get { return resistanceSpecification; }
            set { resistanceSpecification = value; }
        }

        //public string CapacitanceSpecification1
        //{
        //    get { return capacitanceSpecification1; }
        //    set { capacitanceSpecification1 = value; }
        //}

        //public string CapacitanceSpecification2
        //{
        //    get { return capacitanceSpecification2; }
        //    set { capacitanceSpecification2 = value; }
        //}
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public string EquipmentID
        {
            get { return equipmentID; }
            set { equipmentID = value; }
        }

        public string EquipmentType
        {
            get { return equipmentType; }
            set { equipmentType = value; }
        }

        public double XAxis
        {
            get { return xAxis; }
            set { xAxis = value; }
        }

        public double YAxis
        {
            get { return yAxis; }
            set { yAxis = value; }
        }

        public double ZAxis
        {
            get { return zAxis; }
            set { zAxis = value; }
        }

        public double ThetaAxis
        {
            get { return thetaAxis; }
            set { thetaAxis = value; }
        }

        public int SoftwareStatus
        {
            get { return softwareStatus; }
            set { softwareStatus = value; }
        }

        public string LoginUser
        {
            get { return loginUser; }
            set { loginUser = value; }
        }

        public int OperationMode
        {
            get { return operationMode; }
            set { operationMode = value; }
        }

        public string WorkOrderVersion
        {
            get { return workOrderVersion; }
            set { workOrderVersion = value; }
        }

        public string HGASerialNumber
        {
            get { return hgaSerialNumber; }
            set { hgaSerialNumber = value; }
        }

        public string HGAPartNumber
        {
            get { return hgapartNumber; }
            set { hgapartNumber = value; }
        }

        public string FSAPartNumber
        {
            get { return fsapartNumber; }
            set { fsapartNumber = value; }
        }


        public int UPH
        {
            get { return uph; }
            set { uph = value; }
        }

        public double CycleTime
        {
            get { return cycleTime; }
            set { cycleTime = value; }
        }

        public string StatusCode
        {
            get { return _status_code; }
            set { _status_code = value; }
        }

        public string IBSCheck
        {
            get { return _ibs_check; }
            set { _ibs_check = value; }
        }

        public string RDIbsPattern
        {
            get { return _rd_ibs_pattern; }
            set { _rd_ibs_pattern = value; }
        }

        public string WRIbsPattern
        {
            get { return _wr_ibs_pattern; }
            set { _wr_ibs_pattern = value; }
        }

        public string TicEquipID
        {
            get { return _tic_equip_id; }
            set { _tic_equip_id = value; }
        }

        public string TicTime
        {
            get { return _tic_time; }
            set { _tic_time = value; }
        }

        public string TicDockSide
        {
            get { return _tic_dockside; }
            set { _tic_dockside = value; }
        }
        public double ISI_Reader1
        {
            get { return _isi_reader1; }
            set { _isi_reader1 = value; }
        }

        public double ISI_Reader2
        {
            get { return _isi_reader2; }
            set { _isi_reader2 = value; }
        }

        public double Delta_R1
        {
            get { return _delta_r1; }
            set { _delta_r1 = value; }
        }

        public double Delta_R2
        {
            get { return _delta_r2; }
            set { _delta_r2 = value; }
        }

        public double LDU_Res
        {
            get { return _ldu_res; }
            set { _ldu_res = value; }
        }

        public double LDU_Res_Spec_Min
        {
            get { return _ldu_res_spec_min; }
            set { _ldu_res_spec_min = value; }
        }

        public double LDU_Res_Spec_Max
        {
            get { return _ldu_res_spec_max; }
            set { _ldu_res_spec_max = value; }
        }

        public double LDU_Threshold_Ma
        {
            get { return _ldu_threshold_ma; }
            set { _ldu_threshold_ma = value; }
        }

        public double LDU_Volt_Intercept
        {
            get { return _ldu_volt_intercept; }
            set { _ldu_volt_intercept = value; }
        }

        public double LDU_Threshold_Spec_Min
        {
            get { return _ldu_threshold_spec_min; }
            set { _ldu_threshold_spec_min = value; }
        }

        public double LDU_Threshold_Spec_Max
        {
            get { return _ldu_threshold_spec_max; }
            set { _ldu_threshold_spec_max = value; }
        }

        public double LDU_Sweep_Spec_Min
        {
            get { return _ldu_sweep_spec_min; }
            set { _ldu_sweep_spec_min = value; }
        }

        public double LDU_Sweep_Spec_Max
        {
            get { return _ldu_sweep_spec_max; }
            set { _ldu_sweep_spec_max = value; }
        }

        public int Get_Bias_Ch1
        {
            get { return _get_bias_ch1; }
            set { _get_bias_ch1 = value; }

        }

        public int Get_Bias_Ch2
        {
            get { return _get_bias_ch2; }
            set { _get_bias_ch2 = value; }

        }
        public int Get_Bias_Ch3
        {
            get { return _get_bias_ch3; }
            set { _get_bias_ch3 = value; }

        }
        public int Get_Bias_Ch4
        {
            get { return _get_bias_ch4; }
            set { _get_bias_ch4 = value; }

        }
        public int Get_Bias_Ch5
        {
            get { return _get_bias_ch5; }
            set { _get_bias_ch5 = value; }

        }
        public int Get_Bias_Ch6
        {
            get { return _get_bias_ch6; }
            set { _get_bias_ch6 = value; }

        }

        public int Get_Sensing_Ch1
        {
            get { return _get_sensing_ch1; }
            set { _get_sensing_ch1 = value; }
        }
        public int Get_Sensing_Ch2
        {
            get { return _get_sensing_ch2; }
            set { _get_sensing_ch2 = value; }
        }
        public int Get_Sensing_Ch3
        {
            get { return _get_sensing_ch3; }
            set { _get_sensing_ch3 = value; }
        }
        public int Get_Sensing_Ch4
        {
            get { return _get_sensing_ch4; }
            set { _get_sensing_ch4 = value; }
        }
        public int Get_Sensing_Ch5
        {
            get { return _get_sensing_ch5; }
            set { _get_sensing_ch5 = value; }
        }
        public int Get_Sensing_Ch6
        {
            get { return _get_sensing_ch6; }
            set { _get_sensing_ch6 = value; }
        }

        public double Volt_Ratio_Ch1
        {
            get { return _volt_ratio_ch1; }
            set { _volt_ratio_ch1 = value; }
        }
        public double Volt_Ratio_Ch2
        {
            get { return _volt_ratio_ch2; }
            set { _volt_ratio_ch2 = value; }
        }
        public double Volt_Ratio_Ch3
        {
            get { return _volt_ratio_ch3; }
            set { _volt_ratio_ch3 = value; }
        }
        public double Volt_Ratio_Ch4
        {
            get { return _volt_ratio_ch4; }
            set { _volt_ratio_ch4 = value; }
        }
        public double Volt_Ratio_Ch5
        {
            get { return _volt_ratio_ch5; }
            set { _volt_ratio_ch5 = value; }
        }
        public double Volt_Ratio_Ch6
        {
            get { return _volt_ratio_ch6; }
            set { _volt_ratio_ch6 = value; }
        }

        public char SDET_TST_STATUS
        {
            get { return _sdet_tst_status; }
            set { _sdet_tst_status = value; }
        }

        public double Voltage_Delta1
        {
            get { return _voltage_delta1; }
            set { _voltage_delta1 = value; }
        }

        public double Voltage_Delta2
        {
            get { return _voltage_delta2; }
            set { _voltage_delta2 = value; }
        }

        public double Delta_Writer_SDET
        {
            get { return _delta_writer_sdet; }
            set { _delta_writer_sdet = value; }
        }

        public double Delta_Writer_HST_SDET
        {
            get { return _delta_writer_hst_sdet; }
            set { _delta_writer_hst_sdet = value; }
        }

        public double WRBridge_Percentage
        {
            get { return _wrbridge_percentage; }
            set { _wrbridge_percentage = value; }
        }

        public double Led_Intercept
        {
            get { return _led_intercept; }
            set { _led_intercept = value; }
        }

        public double Led_Intercept_Spec
        {
            get { return _led_intercept_spec; }
            set { _led_intercept_spec = value; }
        }

        public double Led_Intercept_Spec_Max
        {
            get { return _led_intercept_spec_max; }
            set { _led_intercept_spec_max = value; }
        }

        public double Pd_Voltage
        {
            get { return _pd_voltage; }
            set { _pd_voltage = value; }
        }

        public double Pd_Voltage_Spec_Min
        {
            get { return _pd_voltage_spec_min; }
            set { _pd_voltage_spec_min = value; }
        }

        public double Pd_Voltage_Spec_Max
        {
            get { return _pd_voltage_spec_max; }
            set { _pd_voltage_spec_max = value; }
        }

        public double Sdet_iThreshold
        {
            get { return _sdet_ithreshold; }
            set { _sdet_ithreshold = value; }
        }

        public double Delta_iThreshold
        {
            get { return _delta_ithreshold; }
            set { _delta_ithreshold = value; }
        }
        public double sdet_writer
        {
            get { return _sdet_writer; }
            set { _sdet_writer = value; }
        }

        public double hst_sdet_delta_writer
        {
            get { return _hst_sdet_delta_writer; }
            set { _hst_sdet_delta_writer = value; }
        }
        public double wrbridge_pct
        {
            get { return _wrbridge_pct; }
            set { _wrbridge_pct = value; }
        }
        public double wrbridge_adap_spec
        {
            get { return _wrbridge_adap_spec; }
            set { _wrbridge_adap_spec = value; }
        }
        public double sdet_reader1
        {
            get { return _sdet_reader1; }
            set { _sdet_reader1 = value; }
        }
        public double sdet_reader2
        {
            get { return _sdet_reader2; }
            set { _sdet_reader2 = value; }
        }
        public double gauss_meter1
        {
            get { return _gauss_meter1; }
            set { _gauss_meter1 = value; }
        }
        public double gauss_meter2
        {
            get { return _gauss_meter2; }
            set { _gauss_meter2 = value; }
        }

        public double anc_yield
        {
            get { return _anc_yield; }
            set { _anc_yield = value; }
        }

        public double anc_hga_count
        {
            get { return _anc_hga_count; }
            set { _anc_hga_count = value; }
        }

        public double DeltaISI_R1_SDET_Tolerance
        {
            get { return _DeltaISI_R1_SDET_Tolerance; }
            set { _DeltaISI_R1_SDET_Tolerance = value; }
        }

        public double DeltaISI_R2_SDET_Tolerance
        {
            get { return _DeltaISI_R2_SDET_Tolerance; }
            set { _DeltaISI_R2_SDET_Tolerance = value; }
        }
        #endregion

        public SeatrackLoadData()
        {

        }

        public void Clear()
        {
            eventDate = "";
            carrierID = "";
            carrierSlot = "";
            serialNo = "";
            status = "";
            workOrder = "";
            errorMessageCode = "0";
            setupFile = "";
            readerResistance = "";
            writerResistance = "";
            rHeaterResistance = "";
            wHeaterResistance = "";
            tAResistance = "";
            shortTest = 0;
            shortTestPosition = "";
            uACTCapacitance1 = 0.0;
            uACTCapacitance2 = 0.0;
            temperatureBoard = "";
            biasVoltageMeasure = 0.0;
            biasCurrentSource = 0.0;
            resistanceSpecification = "";
            capacitanceSpecification1 = "";
            capacitanceSpecification2 = "";
            location = "";
            equipmentID = "";
            equipmentType = "";
            xAxis = 0.0;
            yAxis = 0.0;
            zAxis = 0.0;
            thetaAxis = 0.0;
            softwareStatus = 0;
            loginUser = "";
            operationMode = 0;
            workOrderVersion = "";
            hgapartNumber = "";
            _status_code = String.Empty;
            _ibs_check = String.Empty;
            _rd_ibs_pattern = String.Empty;
            _wr_ibs_pattern = String.Empty;
            _tic_equip_id = String.Empty;
            _isi_reader1 = 0.0;
            _isi_reader2 = 0.0;
            _delta_r1 = 0.0;
            _delta_r2 = 0.0;
            _ldu_res = 0.0;
            _ldu_res_spec_min = 0.0;
            _ldu_res_spec_max = 0.0;
            _ldu_threshold_ma = 0.0;
            _ldu_volt_intercept = 0.0;
            _ldu_threshold_spec_max = 0.0;
            _ldu_threshold_spec_max = 0.0;
            _ldu_sweep_spec_min = 0.0;
            _ldu_sweep_spec_max = 0.0;
            _get_bias_ch1 = 0;
            _get_bias_ch2 = 0;
            _get_bias_ch3 = 0;
            _get_bias_ch4 = 0;
            _get_bias_ch5 = 0;
            _get_bias_ch6 = 0;
            _get_sensing_ch1 = 0;
            _get_sensing_ch2 = 0;
            _get_sensing_ch3 = 0;
            _get_sensing_ch4 = 0;
            _get_sensing_ch5 = 0;
            _get_sensing_ch6 = 0;
            _volt_ratio_ch1 = 0.0;
            _volt_ratio_ch2 = 0.0;
            _volt_ratio_ch3 = 0.0;
            _volt_ratio_ch4 = 0.0;
            _volt_ratio_ch5 = 0.0;
            _volt_ratio_ch6 = 0.0;
            _voltage_delta1 = 0.0;
            _voltage_delta2 = 0.0;
            _anc_yield = 0.0;
            _anc_hga_count = 0.0;
        }
    }

    public class BaselineVoltageRatio
    {
        public BaselineVoltageRatio()
        {
            CurrentRatio_CH1 = 0;
            CurrentRatio_CH2 = 0;
            CurrentRatio_CH3 = 0;
            CurrentRatio_CH4 = 0;
            CurrentRatio_CH5 = 0;
            CurrentRatio_CH6 = 0;
        }

        public int CurrentRatio_CH1 { get; set; }
        public int CurrentRatio_CH2 { get; set; }
        public int CurrentRatio_CH3 { get; set; }
        public int CurrentRatio_CH4 { get; set; }
        public int CurrentRatio_CH5 { get; set; }
        public int CurrentRatio_CH6 { get; set; }

    }
}
