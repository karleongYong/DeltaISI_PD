using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Settings;
using XyratexOSC.UI;
using XyratexOSC.XMath;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    [TypeConverter(typeof(HgaConverter))]
    public class Hga : ISettings
    {
        private double _reader1Resistance = 0.0;
        private double _reader2Resistance = 0.0;
        private double _deltaISIReader1 = 0.0;
        private double _deltaISIReader2 = 0.0;
        private double _writerResistance = 0.0;
        private double _rHeaterResistance = 0.0;
        private double _wHeaterResistance = 0.0;
        private double _tAResistance = 0.0;
        private ShortDetection _shortTest = ShortDetection.NoTest;
        private string _shortPadPosition = "0";
        private double _capacitance1 = 0.0;
        private double _capacitance2 = 0.0;
        private double _biasVoltage = 0.0;
        private double _Ch1temperature = 0.0;
        private double _Ch2temperature = 0.0;
        private double _Ch3temperature = 0.0;
        private double _biasCurrent = 0.0;
        private string _resistanceSpec = "Empty";
        private string _capacitanceSpec1 = "Empty";
        private string _capacitanceSpec2 = "Empty";
        private string _errorMessageCode = string.Empty;

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
        private double _get_voltage_delta1 = 0.0;
        private double _get_voltage_delta2 = 0.0;
        private double _delta_writer_sdet = 0.0;
        private double _delta_writer_hst_sdet = 0.0;
        private double _wrbridge_percentage = 0.0;

        private double _ldu_Threshold_Ma = 0.0;
        private int _ldu_Sweep_Spec_Min = 0;
        private int _ldu_Sweep_Spec_Max = 0;
        //private double _pd_Voltage_Spec_Min = 0.0;
        //private double _pd_Voltage_Spec_Max = 0.0;
        private double _sdet_iThreshold = 0.0;
        //private double _delta_iThreshold = 0.0;
        //private double _last_ithreshold_ma = 0.0;
        //private double _last_pd_volt = 0.0;


        // added 3-Feb-2020
        private double _led_intercept = 0.0;
        private double _led_intercept_spec = 0.0;
        private double _ldu_delta_ta = 0.0;
        private double _ldu_delta_ta_spec = 0.0;

        private double _sdet_writer = 0.0;
        private double _wrbridge_pct = 0.0;
        private double _wrbridge_adap_spec = 0.0;
        private double _sdet_reader1 = 0.0;
        private double _sdet_reader2 = 0.0;
        private double _gauss_meter1 = 0.0;
        private double _gauss_meter2 = 0.0;
        private double _delta_isi_reader1 = 0.0;
        private double _delta_isi_reader2 = 0.0;

        private LDUFailureType _lduFailure = LDUFailureType.Unknown;
        private IBSObj _ibsObj;
        private double _anc_yield = 0;
        private double _anc_hga_count = 0;
        private double _DeltaISI_R2_SDET_Tolerance = 0;
        private double _DeltaISI_R1_SDET_Tolerance = 0;
        //ldu
        private double[] LEDPoint = new double[21];
        private double[] LDUPoint = new double[21];
        private double _LEDThreshold = 0.0;

        private double _LDUResistance = 0.0;
        private double _LDUTurnOnVoltage = 0.0;
        private double _ledIntercept = 0.0;

        private double _Volt_DelTA1 = 0.0;
        private double _Volt_DelTA2 = 0.0;
        private double _WrRes_Before_Offset = 0.0;
        private double _TaRes_Before_Offset = 0.0;
        private double _WhRes_Before_Offset = 0.0;
        private double _RhRes_Before_Offset = 0.0;
        private double _R1Res_Before_Offset = 0.0;
        private double _R2Res_Before_Offset = 0.0;

        private string _LDUSpec = "Empty";
        private double _LEDSpec = 0.0;
        private double _TADeltaResistanceSpec = 0.0;
        private string _hgaSerialNumber = "Unknown";

        private double _IThreshold = 0.0;
        private double _MaxVPD = 0.0;

        private I_ThresholdCalculationMethod _IthresholdCalculationMethod = I_ThresholdCalculationMethod.Unknown;
        //ldu
        public Hga(int index, HGAStatus hgaStatus)
        {
            Index = index;
            Hga_Status = hgaStatus;
            _ibsObj = new IBSObj();
            UTIC_DATA = new TIC_BIN_DATA();
        }


        public void setCh1Temperature(double value)
        {
            _Ch1temperature = value;
        }

        public double getCh1Temperature()
        {
            return _Ch1temperature;
        }

        public void setCh2Temperature(double value)
        {
            _Ch2temperature = value;
        }

        public double getCh2Temperature()
        {
            return _Ch2temperature;
        }

        public void setCh3Temperature(double value)
        {
            _Ch3temperature = value;
        }

        public double getCh3Temperature()
        {
            return _Ch3temperature;
        }

        public void setBiasCurrent(double value)
        {
            _biasCurrent = value;
        }

        public double getBiasCurrent()
        {
            return _biasCurrent;
        }

        public void setResistanceSpec(string value)
        {
            _resistanceSpec = value;
        }

        public string getResistanceSpec()
        {
            return _resistanceSpec;
        }
        //// C1
        //public void setCapacitance1Spec(string value)
        //{
        //    _capacitanceSpec1 = value;
        //}

        //public string getCapacitance1Spec()
        //{
        //    return _capacitanceSpec1;
        //}
        //// C1
        //// C2
        //public void setCapacitance2Spec(string value)
        //{
        //    _capacitanceSpec2 = value;
        //}

        //public string getCapacitance2Spec()
        //{
        //    return _capacitanceSpec2;
        //}
        //// C2     

        public void setReader1Resistance(double value)
        {
            _reader1Resistance = value;
        }       

        public double getReader1Resistance()
        {
            return _reader1Resistance;
        }

        public double getReader2Resistance()
        {
            return _reader2Resistance;
        }

        public void setReader2Resistance(double value)
        {
            _reader2Resistance = value;
        }
        //DeltaISI
        public double getDeltaISIReader1()
        {
            return _deltaISIReader1; 
        }

        public void setDeltaISIReader1(double value)
        {
            _deltaISIReader1 = Math.Round(value, 2);
        }

        public double getDeltaISIReader2()
        {
            return _deltaISIReader2;
        }

        public void setDeltaISIReader2(double value)
        {
            _deltaISIReader2 = Math.Round(value, 2);
        }

        public void setWriterResistance(double value)
        {
            _writerResistance = value;
        }

        public double getWriterResistance()
        {
            return _writerResistance;
        }

        public void setRHeaterResistance(double value)
        {
            _rHeaterResistance = value;
        }

        public double getRHeaterResistance()
        {
            return _rHeaterResistance;
        }

        public void setWHeaterResistance(double value)
        {
            _wHeaterResistance = value;
        }

        public double getWHeaterResistance()
        {
            return _wHeaterResistance;
        }

        public void setTAResistance(double value)
        {
            _tAResistance = value;
        }

        public double getTAResistance()
        {
            return _tAResistance;
        }

        public void setShortTest(ShortDetection value)
        {
            _shortTest = value;
        }

        public ShortDetection getShortTest()
        {
            return _shortTest;
        }

        public void setShortPadPosition(string value)
        {
            _shortPadPosition = value;
        }

        public string getShortPadPosition()
        {
            return _shortPadPosition;
        }

        //public void setCapacitance1(double value)
        //{
        //    _capacitance1 = value;
        //}

        //public double getCapacitance1()
        //{
        //    return _capacitance1;
        //}

        //public void setCapacitance2(double value)
        //{
        //    _capacitance2 = value;
        //}

        //public double getCapacitance2()
        //{
        //    return _capacitance2;
        //}
        public void setBiasVoltage(double value)
        {
            _biasVoltage = value;
        }

        public double getBiasVoltage()
        {
            return _biasVoltage;
        }

        //Bias
        public void Set_Bias_Ch1(int value)
        {
            _get_bias_ch1 = value;
        }

        public int Get_Bias_Ch1()
        {
            return _get_bias_ch1;
        }

        public void Set_Bias_Ch2(int value)
        {
            _get_bias_ch2 = value;
        }

        public int Get_Bias_Ch2()
        {
            return _get_bias_ch2;
        }

        public void Set_Bias_Ch3(int value)
        {
            _get_bias_ch3 = value;
        }

        public int Get_Bias_Ch3()
        {
            return _get_bias_ch3; 
        }

        public void Set_Bias_Ch4(int value)
        {
            _get_bias_ch4 = value;
        }
        public int Get_Bias_Ch4()
        {
            return _get_bias_ch4;
        }
        public void Set_Bias_Ch5(int value)
        {
            _get_bias_ch5 = value;
        }
        public int Get_Bias_Ch5()
        {
            return _get_bias_ch5;
        }
        public void Set_Bias_Ch6(int value)
        {
            _get_bias_ch6 = value;
        }

        public int Get_Bias_Ch6()
        {
            return _get_bias_ch6;
        }

        //Sensing
        public void Set_Sensing_Ch1(int value)
        {
            _get_sensing_ch1 = value;
        }
        public int Get_Sensing_Ch1()
        {
            return _get_sensing_ch1;
        }
        public void Set_Sensing_Ch2(int value)
        {
            _get_sensing_ch2 = value;
        }
        public int Get_Sensing_Ch2()
        {
            return _get_sensing_ch2;
        }
        public void Set_Sensing_Ch3(int value)
        {
            _get_sensing_ch3 = value;
        }
        public int Get_Sensing_Ch3()
        {
            return _get_sensing_ch3;
        }
        public void Set_Sensing_Ch4(int value)
        {
            _get_sensing_ch4 = value;
        }
        public int Get_Sensing_Ch4()
        {
            return _get_sensing_ch4;
        }
        public void Set_Sensing_Ch5(int value)
        {
            _get_sensing_ch5 = value;
        }
        public int Get_Sensing_Ch5()
        {
            return _get_sensing_ch5;
        }
        public void Set_Sensing_Ch6(int value)
        {
            _get_sensing_ch6 = value;
        }
        public int Get_Sensing_Ch6()
        {
            return _get_sensing_ch6;
        }

        public void Set_Voltage_Delta1(double value)
        {
            _get_voltage_delta1 = value;
        }
        public double Get_Voltage_Delta1()
        {
            return _get_voltage_delta1;
        }

        public void Set_Voltage_Delta2(double value)
        {
            _get_voltage_delta2 = value;
        }
        public double Get_Voltage_Delta2()
        {
            return _get_voltage_delta2;
        }

        public double Get_Delta_WR_Hst_Sdet()
        {
            return _delta_writer_hst_sdet;        
        }

        public void Set_Delta_WR_Hst_Sdet(double value)
        {
            _delta_writer_hst_sdet = value;
        }

        public double Get_WRBridge_Percentage()
        {
            return _wrbridge_percentage;
        }

        public void Set_WRBridge_Percentage(double Value)
        {
            _wrbridge_percentage = Value;
        }

        public double Get_Delta_Writer_Sdet()
        {
            return _delta_writer_sdet;
        }

        public void Set_Delta_Writer_Sdet(double Value)
        {
            _delta_writer_sdet = Value;
        }

        public void set_ldu_Threshold_Ma(double value)
        {
            _ldu_Threshold_Ma = value;
        }

        public double get_ldu_Threshold_Ma()
        {
            return _ldu_Threshold_Ma;
        }

        public void set_ldu_Sweep_Spec_Min(int value)
        {
            _ldu_Sweep_Spec_Min = value;
        }

        public int get_ldu_Sweep_Spec_Min()
        {
            return _ldu_Sweep_Spec_Min;
        }

        public void set_ldu_Sweep_Spec_Max(int value)
        {
            _ldu_Sweep_Spec_Max = value;
        }

        public int get_ldu_Sweep_Spec_Max()
        {
            return _ldu_Sweep_Spec_Max;
        }

        public void set_sdet_iThreshold(double value)
        {
            _sdet_iThreshold = value;
        }

        public double get_sdet_iThreshold()
        {
            return _sdet_iThreshold;
        }

        public double get_led_intercept_spec() { return  _led_intercept_spec ; }
        public double get_sdet_writer() { return _sdet_writer ; }
        public double get_wrbridge_pct() { return _wrbridge_pct ; }

        public double get_wrbridge_adap_spec() { return _wrbridge_adap_spec ; }
        public double get_sdet_reader1() { return _sdet_reader1 ; }
        public double get_sdet_reader2() { return _sdet_reader2 ; }
        public double get_gauss_meter1() { return _gauss_meter1 ; }
        public double get_gauss_meter2() { return _gauss_meter2 ; }

        public double get_anc_yield() { return _anc_yield; }
        public double get_anc_hga_count() { return _anc_hga_count; }
        public double get_DeltaISI_R2_SDET_Tolerance() { return _DeltaISI_R2_SDET_Tolerance; }

        public double get_DeltaISI_R1_SDET_Tolerance() { return _DeltaISI_R1_SDET_Tolerance; }

        public void Set_sdet_writer(double Value) {  _sdet_writer = Value; }

        public void Set_wrbridge_pct(double Value) {  _wrbridge_pct = Value; }

        public void Set_wrbridge_adap_spec(double Value) {  _wrbridge_adap_spec = Value; }
        public void Set_sdet_reader1(double Value) {  _sdet_reader1 = Value; }
        public void Set_sdet_reader2(double Value) {  _sdet_reader2 = Value; }
        public void Set_gauss_meter1(double Value) {  _gauss_meter1 = Value; }
        public void Set_gauss_meter2(double Value) {  _gauss_meter2 = Value; }

        public void Set_delta_isi_reader1(double Value) { _delta_isi_reader1 = Value; }
        public void Set_delta_isi_reader2(double Value) { _delta_isi_reader2 = Value; }

        public void set_ANC_YIELD(double Value) { _anc_yield = Value; }
        public void set_ANC_HGA_Count(double Value) { _anc_hga_count = Value; }

        public void set_DeltaISI_R2_SDET_Tolerance(double Value) { _DeltaISI_R2_SDET_Tolerance = Value; }
        public void set_DeltaISI_R1_SDET_Tolerance(double Value) { _DeltaISI_R1_SDET_Tolerance = Value; }

        [Browsable(false)]
        public int Index
        {
            get;
            set;
        }

        public HGAStatus Hga_Status
        {
            get;
            set;
        }

        public bool ForceToSampling
        {
            get;
            set;
        }

        public bool ForceToRiskCode
        {
            get;
            set;
        }

        public bool IsPassLDUSpec { get; set; }
        public bool ForceToPolarityRiskSamplingDeltaR1 { get; set; }
        public bool ForceToPolarityRiskSamplingDeltaR2 { get; set; }

        public string Error_Msg_Code
        {
            get { return _errorMessageCode; }
            set { _errorMessageCode = value; }
        }

        public bool Error_Msg_Code_Set_Flag
        {
            get;
            set;
        }

        public bool OverallMeasurementTestPass
        {
            get;
            set;
        }

        public double DeltaISIResistanceRD1
        {
            get;
            set;
        }

        public double DeltaISIResistanceRD2
        {
            get;
            set;
        }

        public double DeltaISIResistanceWriter
        {
            get;
            set;
        }

        public char TST_STATUS
        {
            get;
            set;
        }

        public bool IsGetISIPassed
        {
            get;
            set;
        }

        public string Slider_Lot_Number
        {
            get;
            set;
        }

        public string ISI_WAF_TAD_RES
        {
            get;
            set;
        }
        public string ISI_WAF_RDR_HTR_RES
        {
            get;
            set;
        }
        public string ISI_WAF_WTR_RES
        {
            get;
            set;
        }
        public string ISI_WAF_WTR_HTR_RES
        {
            get;
            set;
        }
        public string ISI_RES_AT_ET
        {
            get;
            set;
        }

        public string ISI_RES_AT_ET_RD2
        {
            get;
            set;
        }

        public string ISI_AMP_AT_ET
        {
            get;
            set;
        }

        public string ISI_AMP_AT_ET_RD2
        {
            get;
            set;
        }

        public string ISI_ASYM_AT_ET
        {
            get;
            set;
        }

        public string ISI_ASYM_AT_ET_RD2
        {
            get;
            set;
        }
        public string ISI_TAB
        {
            get;
            set;
        }

        public double ISI_ET_RD2_RES
        {
            get;
            set;
        }
        public double ISI_ET_RD1_RES
        {
            get;
            set;
        }

        public double Last_ET_Threshold
        {
            get;
            set;
        }

        public double Delta_IThreshold
        {
            get;
            set;
        }

        public TIC_BIN_DATA UTIC_DATA
        {
            get;
            set;
        }

        public IBSObj IBS_Data
        {
            get { return _ibsObj; }
            set { _ibsObj = value; }
        }


        public void setHgaSerialNumber(string value)
        {
            _hgaSerialNumber = value;
        }

        public string getHgaSerialNumber()
        {
            return _hgaSerialNumber;
        }

        public double getLEDIntercept()
        {
            return _ledIntercept;
        }
        public void setLedIntercept(double value)
        {
            _ledIntercept = value;
        }

        public double getLDUTurnOnVoltage()
        {
            return _LDUTurnOnVoltage;
        }
        public void setLDUTurnOnVoltage(double value)
        {
            _LDUTurnOnVoltage = value;
        }


        public double getIThreshold()
        {
            return _IThreshold;
        }
        public void setIThreshold(double value)
        {
            _IThreshold = value;
        }


        public void set_IthresholdCalculationMethod(I_ThresholdCalculationMethod value)
        {
            _IthresholdCalculationMethod = value;
        }

        public I_ThresholdCalculationMethod get_IthresholdCalculationMethod()
        {
            return _IthresholdCalculationMethod;
        }



        public double getMaxVPD()
        {
            return _MaxVPD;
        }
        public void setMaxVPD(double value)
        {
            _MaxVPD = value;
        }

        public void setWRResBeforeOffset(double value)
        {
            _WrRes_Before_Offset = value;
        }

        public double getWRResBeforeOffset()
        {
            return _WrRes_Before_Offset;
        }

        public void setTAResBeforeOffset(double value)
        {
            _TaRes_Before_Offset = value;
        }

        public double getTAResBeforeOffset()
        {
            return _TaRes_Before_Offset;
        }

        public void setWHResBeforeOffset(double value)
        {
            _WhRes_Before_Offset = value;
        }

        public double getWHResBeforeOffset()
        {
            return _WhRes_Before_Offset;
        }

        public void setRHResBeforeOffset(double value)
        {
            _RhRes_Before_Offset = value;
        }

        public double getRHResBeforeOffset()
        {
            return _RhRes_Before_Offset;
        }

        public void setR1ResBeforeOffset(double value)
        {
            _R1Res_Before_Offset = value;
        }

        public double getR1ResBeforeOffset()
        {
            return _R1Res_Before_Offset;
        }

        public void setR2ResBeforeOffset(double value)
        {
            _R2Res_Before_Offset = value;
        }

        public double getR2ResBeforeOffset()
        {
            return _R2Res_Before_Offset;
        }

        public void setVoltDeltaTA1(double value)
        {
            _Volt_DelTA1 = value;
        }

        public double getVoltDeltaTA1()
        {
            return _Volt_DelTA1;
        }

        public void setVoltDeltaTA2(double value)
        {
            _Volt_DelTA2 = value;
        }

        public double getVoltDeltaTA2()
        {
            return _Volt_DelTA2;
        }
        public override string ToString()
        {
            return String.Format("Index,{0},HGA Status,{1}", Index, Hga_Status);
        }

        public static Hga FromString(string hgaString)
        {
            string[] hgaInfo = hgaString.Trim('(', ')').Split(',');

            if (hgaInfo.Length != 2)
                throw new FormatException(String.Format("Cannot create HGA object from '{0}'.", hgaInfo));

            int index = int.Parse(hgaInfo[0]);
            HGAStatus hgaStatus = (HGAStatus)Enum.Parse(typeof(HGAStatus), hgaInfo[1]);

            return new Hga(index, hgaStatus);
        }

        public void UpdateFromSettingsNode(SettingsNode node)
        {
            if (node.ExistsAndHasAValue<int>("Index"))
                Index = node["Index"].GetValueAs<int>();
            if (node.ExistsAndHasAValue<HGAStatus>("Hga_Status"))
                Hga_Status = node["Hga_Status"].GetValueAs<HGAStatus>();
        }
        
        public SettingsNode ConvertToSettingsNode()
        {
            SettingsNode node = new SettingsNode("Hga");
            node.AddChild("Index", "", typeof(int), Index);
            node.AddChild("Hga_Status", "", typeof(HGAStatus), Hga_Status);

            return node;
        }
    }//class Hga

    
    public class HgaConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pds = base.GetProperties(context, value, attributes);

            PropertyDescriptor[] props = new PropertyDescriptor[pds.Count];

            for (int i = 0; i < pds.Count; i++)
            {
                PropertyInfo pi = pds[i].ComponentType.GetProperty(pds[i].Name);
                props[i] = new HgaPropertyDescriptor(pi, pds[i], pds[i].DisplayName);
            }

            // Organize into X,Y,Z,Theta
            return new HgaPropertyDescriptorCollection(props);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(Hga))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(string) && value is Hga)
            {
                Hga hga = (Hga)value;
                return hga.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    return Hga.FromString((string)value);
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to HGA object");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Provides an abstraction of a RobotPoint property.
    /// </summary>
    public class HgaPropertyDescriptor : PropertyDescriptor
    {
        private PropertyInfo _parentPI;
        private PropertyDescriptor _childPD;

        public HgaPropertyDescriptor(PropertyInfo parentPI, PropertyDescriptor childPD, string propertyDescriptorName)
            : base(propertyDescriptorName, null)
        {
            _childPD = childPD;
            _parentPI = parentPI;
        }

        public override bool IsReadOnly { get { return false; } }
        public override void ResetValue(object component) { }
        public override bool CanResetValue(object component) { return false; }
        public override bool ShouldSerializeValue(object component) { return true; }
        public override Type ComponentType { get { return _childPD.ComponentType; } }
        public override Type PropertyType { get { return _childPD.PropertyType; } }

        public override object GetValue(object component)
        {
            return _childPD.GetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _parentPI.SetValue(component, value, null);
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="HgaPropertyDescriptor"/> objects, which does not allow alphabetical sorts.
    /// </summary>
    public class HgaPropertyDescriptorCollection : PropertyDescriptorCollection
    {
        public HgaPropertyDescriptorCollection(PropertyDescriptor[] propertyDescriptors)
            : base(propertyDescriptors)
        {
        }

        public override PropertyDescriptorCollection Sort()
        {
            return this;
        }
        public override PropertyDescriptorCollection Sort(string[] names)
        {
            return this;
        }

        public override PropertyDescriptorCollection Sort(string[] names, System.Collections.IComparer comparer)
        {
            return this;
        }

        public override PropertyDescriptorCollection Sort(System.Collections.IComparer comparer)
        {
            return this;
        }
    }
}
