using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    [Flags]
    public enum AdjacentPadTopic
    {
        [Description("Unknown")]
        Unknown,
        [Description("W+")]
        W_Plush,
        [Description("W-")]
        W_Minus,
        [Description("TA+")]
        TA_Plush,
        [Description("TA-")]
        TA_Minus,
        [Description("WH+")]
        WH_Plush,
        [Description("WH-")]
        WH_Minus,
        [Description("rH+")]
        RH_Plush,
        [Description("rH-")]
        RH_Minus,
        [Description("R1+")]
        R1_Plush,
        [Description("R1-")]
        R1_Minus,
        [Description("R2+")]
        R2_Plush,
        [Description("R2-")]
        R2_Minus
    }

    public unsafe struct MeasurementTestRecipe
    {
        public int ProductID;

        // Ch1WriterResistance       
        public double Ch1WriterResistanceMin;
        public double Ch1WriterResistanceMax;

        // Ch2TAResistance        
        public double Ch2TAResistanceMin;
        public double Ch2TAResistanceMax;

        // Ch3WHResistance       
        public double Ch3WHResistanceMin;
        public double Ch3WHResistanceMax;

        // Ch4RHResistance        
        public double Ch4RHResistanceMin;
        public double Ch4RHResistanceMax;

        // Ch5R1Resistance       
        public double Ch5R1ResistanceMin;
        public double Ch5R1ResistanceMax;

        // Ch6R2Resistance        
        public double Ch6R2ResistanceMin;
        public double Ch6R2ResistanceMax;

        // Ch6LDUResistance        
        public double Ch6LDUResistanceMin;
        public double Ch6LDUResistanceMax;

        // Ch1Capacitance        
        public double Ch1CapacitanceMin;
        public double Ch1CapacitanceMax;

        // Ch2Capacitance        
        public double Ch2CapacitanceMin;
        public double Ch2CapacitanceMax;


        // Ch1WriterOpen/Short Spec      
        public double Ch1WriterOpenShortMin;
        public double Ch1WriterOpenShortMax;

        // Ch2TAOpen/Short Spec     
        public double Ch2TAOpenShortMin;
        public double Ch2TAOpenShortMax;

        // Ch3WHOpen/Short Spec      
        public double Ch3WHOpenShortMin;
        public double Ch3WHOpenShortMax;

        // Ch4RHOpen/Short Spec      
        public double Ch4RHOpenShortMin;
        public double Ch4RHOpenShortMax;

        // Ch5R1ROpen/Short Spec    
        public double Ch5R1OpenShortMin;
        public double Ch5R1OpenShortMax;

        // Ch6R2Open/Short Spec     
        public double Ch6R2OpenShortMin;
        public double Ch6R2OpenShortMax;

        // Ch6R2Open/Short Spec     
        public double Ch6LDUOpenShortMin;
        public double Ch6LDUOpenShortMax;

        //Delta ISI spec
        public bool DeltaISI_Enable;
        public double DeltaISISpec1;
        public double DeltaISISpec2;
        public double DeltaISISpec1P;
        public double DeltaISISpec1N;
        public double DeltaISISpec2P;
        public double DeltaISISpec2N;

        public double MinDeltaWriter;
        public double MaxDeltaWriter;
        public double Percentile;
        public double DeltaWriterUp;
        public double DeltaWriterDn;
        public double DistributionSample;

        //Grading Sort
        public bool DisableAll;
        public bool EnableSortData;
        public bool EnableSortResistanceSpec;

        //SDET Writer spec
        public double OffsetR1HSTSDET;
        public double DeltaR1SpecMoreThan;
        public double DeltaR1SpecLessThan;
        public double OffsetR2HSTSDET;
        public double DeltaR2SpecMoreThan;
        public double DeltaR2SpecLessThan;

        public int SamplingETOnDisk;
        public double WriterETOnDisk;
        //ET Offset
        public double Ch1WriterResistanceOffset;
        public double Ch2TAResistanceOffset;
        public double Ch3WHResistanceOffset;
        public double Ch4RHResistanceOffset;
        public double Ch5R1ResistanceOffset;
        public double Ch6R2ResistanceOffset;
        public double Ch6LDUResistanceOffset;

        //ResistanceVoltRatio
        public double Ch1WriterResistanceVoltRatio;
        public double Ch2TAResistanceVoltRatio;
        public double Ch3WHResistanceVoltRatio;
        public double Ch4RHResistanceVoltRatio;
        public double Ch5R1ResistanceVoltRatio;
        public double Ch6R2ResistanceVoltRatio;
        public double Ch6LDUResistanceVoltRatio;

        //ToleranceSpecShortTest
        public double Ch1WriterResistanceToleranceUpperSpec;
        public double Ch2TAResistanceToleranceUpperSpec;
        public double Ch3WHResistanceToleranceUpperSpec;
        public double Ch4RHResistanceToleranceUpperSpec;
        public double Ch5R1ResistanceToleranceUpperSpec;
        public double Ch6R2ResistanceToleranceUpperSpec;
        public double Ch6LDUResistanceToleranceUpperSpec;
        public double Ch1WriterResistanceToleranceLowSpec;
        public double Ch2TAResistanceToleranceLowSpec;
        public double Ch3WHResistanceToleranceLowSpec;
        public double Ch4RHResistanceToleranceLowSpec;
        public double Ch5R1ResistanceToleranceLowSpec;
        public double Ch6R2ResistanceToleranceLowSpec;
        public double Ch6LDUResistanceToleranceLowSpec;

        //BiasCurrentSupply
        public double BiasCurrentCh1Writer;
        public double BiasCurrentCh2TA;
        public double BiasCurrentCh3WH;
        public double BiasCurrentCh4RH;
        public double BiasCurrentCh5R1;
        public double BiasCurrentCh6R2;
        public double BiasCurrentCh6LDU1stPoint;
        public double BiasCurrentCh6LDU2ndPoint;
        public double BiasCurrentCh6LDUStep;
        public double BiasCurrent3rdPointforIThreshold;
        public double BiasCurrent4thPointforIThreshold;

        //ykl
        public double BiasCurrentLEDCh6LDU1stPoint;
        public double BiasCurrentLEDCh6LDU2ndPoint;
        public double BiasCurrentLEDCh6LDUStep;
      

        //Vol Threshold
        public double VolThreshold1HiLimit;
        public double VolThreshold1LowLimit;
        public double VolThreshold2HiLimit;
        public double VolThreshold2LowLimit;

        // For ANC
        public double TestRunCount;
        public double YieldLimit;
        public double YieldTarget;
        public double CountLimit;
        public double GoodBetweenBad;

        //Product Information
        public string ProductName;
        public string SuspensionType;
        public string RecipeProbeType;
        public string RecipeTailType;
        public string SliderForm;
        public string TgaPnUp;
        public string TgaPnDn;
        public string InterPad;
        public string PadWidth;
        public string PadPitch;
        //*---------------------

        // Resistance report spec
        public double MinReportSpec;
        public double MaxReportSpec;

        //TSR Information
        //public string HgaPartNumber;
        public string TSRGroup;
        public string TSRName;
        public string TSRNumber;
        public double SpecNumber;
        public string SpecVersion;
        public string ParamID;
        public string ScriptName;
        public string ScriptDate;
        //public string SliderLotID;
        public double Radius;
        //public string RefRadius;
        public double RPM;
        //public string REF_RPM;
        public double SkewAngle;
        //public double LoadRadius;
        //*-----------------------

        public double WriterResistanceSpecUP;
        public double WriterResistanceSpecDN;
        public double ReaderImpedanceR1Spec;
        public double ReaderImpedanceR2Spec;
        public AdjacentPadList AdjacentPadsList;
        public SortGradingList SortGradingsList;

        public double PDVoltageMinSpec;
        public double PDVoltageMaxSpec;

        public double IThresholdSpecLower;
        public double IThresholdSpecUpper;
        
        public double DeltaIThresholdNegativeSpec;
        public double DeltaIThresholdPositiveSpec;
        public double LEDInterceptSpecLower;
        public double LEDInterceptSpecUpper;

        public double Gompertz_IThresholdSpecLower;
        public double Gompertz_IThresholdSpecUpper;
    }

    public class AdjacentPadList
    {
        int _topicListCount = 0;
        private AdjacentPad[] _adjacentPads;

        public AdjacentPadList()
        {
            _topicListCount = 0;
            initializeObj();
        }

        public AdjacentPadList(int count)
        {
            _topicListCount = count;
            initializeObj();
        }


        public int ListCount { get { return _topicListCount; } }
        public AdjacentPad[] DataPadList { get { return _adjacentPads; } }

        private void initializeObj()
        {
            _adjacentPads = new AdjacentPad[_topicListCount];

            for (int i = 0; i < _topicListCount; i++)
            {
                _adjacentPads[i] = new AdjacentPad((i + 1).ToString(), AdjacentPadTopic.Unknown, AdjacentPadTopic.Unknown);
            }
        }

        public void Save(string section, SettingsXml xml)
        {
            for (int i = 0; i < _topicListCount; i++)
            {
                _adjacentPads[i].Save(section, xml);
            }
        }

        public void Load(string section, SettingsXml xml)
        {
            bool endloop = false;
            int index = 0;
            int totalPadCount = 0;

            // Initial pad list count in-case first time recipe modify by manual
            if (_adjacentPads.Length == 0)
            {
                _adjacentPads = new AdjacentPad[999];
                for (int i = 0; i < 999; i++)
                {
                    _adjacentPads[i] = new AdjacentPad((i + 1).ToString(), AdjacentPadTopic.Unknown, AdjacentPadTopic.Unknown);
                }

                do
                {
                    totalPadCount++;
                    _adjacentPads[index].Load(section, xml);
                    if ((string.IsNullOrEmpty(_adjacentPads[index].ColTopic)) &&
                        (string.IsNullOrEmpty(_adjacentPads[index].RowTopic)))
                    {
                        totalPadCount--;
                        _topicListCount = totalPadCount;
                        endloop = true;
                    }
                    index++;
                } while (!endloop);

                initializeObj();
            }

            //Load normal
            for (int i = 0; i < _topicListCount; i++)
            {
                _adjacentPads[i].Load(section, xml);
            }
        }

    }
    public class AdjacentPad
    {
        public AdjacentPad()
        {
            Name = string.Empty;
            RowTopic = string.Empty;
            ColTopic = string.Empty;
        }

        public AdjacentPad(string name, AdjacentPadTopic rowTopic, AdjacentPadTopic colTopic)
        {
            Name = name;
            RowTopic = GetDescription(rowTopic);
            ColTopic = GetDescription(colTopic);
        }

        public string Name { get; set; }

        public string RowTopic { get; set; }

        public string ColTopic { get; set; }

        public string GetDescription(AdjacentPadTopic value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/DetectCondition" + Name);
            xml.Write("RowTopic", RowTopic);
            xml.Write("ColTopic", ColTopic);
            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/DetectCondition" + Name);
            RowTopic = xml.Read("RowTopic", string.Empty);
            ColTopic = xml.Read("ColTopic", string.Empty);
            xml.CloseSection();
        }
    }


    public class SortGradingList
    {
        int _resistanceTopicCount = 6;
        int _sortPerResistanceCount = 0;
        private SortGrading[] _writerSortGrading;
        private SortGrading[] _taSortGrading;
        private SortGrading[] _wHeaterSortGrading;
        private SortGrading[] _rHeaterSortGrading;
        private SortGrading[] _reader1SortGrading;
        private SortGrading[] _reader2SortGrading;

        public SortGradingList()
        {
            _sortPerResistanceCount = 0;
            initializeObj();
        }

        public SortGradingList(int sortPerResistanceCount)
        {
            _sortPerResistanceCount = sortPerResistanceCount;
            initializeObj();
        }

        public int SortCount { get { return _sortPerResistanceCount; } }
        public int ResistanceTopicCount { get { return _resistanceTopicCount; } }
        public SortGrading[] Reader1 { get { return _reader1SortGrading; } set { _reader1SortGrading = value; } }
        public SortGrading[] Reader2 { get { return _reader2SortGrading; } set { _reader2SortGrading = value; } }
        public SortGrading[] rHeater { get { return _rHeaterSortGrading; } set { _rHeaterSortGrading = value; } }
        public SortGrading[] TA { get { return _taSortGrading; } set { _taSortGrading = value; } }
        public SortGrading[] wHeater { get { return _wHeaterSortGrading; } set { _wHeaterSortGrading = value; } }
        public SortGrading[] Writer { get { return _writerSortGrading; } set { _writerSortGrading = value; } }

        private void initializeObj()
        {
            _reader1SortGrading = new SortGrading[_sortPerResistanceCount];
            _reader2SortGrading = new SortGrading[_sortPerResistanceCount];
            _rHeaterSortGrading = new SortGrading[_sortPerResistanceCount];
            _taSortGrading = new SortGrading[_sortPerResistanceCount];
            _wHeaterSortGrading = new SortGrading[_sortPerResistanceCount];
            _writerSortGrading = new SortGrading[_sortPerResistanceCount];

            for (int i = 0; i < _sortPerResistanceCount; i++)
            {
                _reader1SortGrading[i] = new SortGrading((i + 1).ToString());
                _reader2SortGrading[i] = new SortGrading((i + 1).ToString());
                _rHeaterSortGrading[i] = new SortGrading((i + 1).ToString());
                _taSortGrading[i] = new SortGrading((i + 1).ToString());
                _wHeaterSortGrading[i] = new SortGrading((i + 1).ToString());
                _writerSortGrading[i] = new SortGrading((i + 1).ToString());
            }
        }

        public void Save(string section, SettingsXml xml)
        {
            for (int i = 0; i < _sortPerResistanceCount; i++)
            {
                _reader1SortGrading[i].Save(section + "/Reader1", xml);
                _reader2SortGrading[i].Save(section + "/Reader2", xml);
                _rHeaterSortGrading[i].Save(section + "/RHeater", xml);
                _taSortGrading[i].Save(section + "/TA", xml);
                _wHeaterSortGrading[i].Save(section + "/WHeater", xml);
                _writerSortGrading[i].Save(section + "/Writer", xml);
            }

        }

        public void Load(string section, SettingsXml xml)
        {
            for (int i = 0; i < _sortPerResistanceCount; i++)
            {
                _reader1SortGrading[i].Load(section + "/Reader1", xml);
                _reader2SortGrading[i].Load(section + "/Reader2", xml);
                _rHeaterSortGrading[i].Load(section + "/RHeater", xml);
                _taSortGrading[i].Load(section + "/TA", xml);
                _wHeaterSortGrading[i].Load(section + "/WHeater", xml);
                _writerSortGrading[i].Load(section + "/Writer", xml);
            }

        }

    }
    public class SortGrading
    {
        public SortGrading()
        {
            SortName = string.Empty;
            MinSpec = 0.00;
            MaxSpec = 0.00;
        }
        public SortGrading(string sortName)
        {
            SortName = sortName;
            MinSpec = 0.00;
            MaxSpec = 0.00;
        }
        public string SortName { get; set; }

        public double MinSpec { get; set; }

        public double MaxSpec { get; set; }

        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/Sort" + SortName);
            xml.Write("MinSpec", MinSpec);
            xml.Write("MaxSpec", MaxSpec);
            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/Sort" + SortName);
            MinSpec = xml.Read("MinSpec", 0.00);
            MaxSpec = xml.Read("MaxSpec", 0.00);
            xml.CloseSection();
        }
    }


}
