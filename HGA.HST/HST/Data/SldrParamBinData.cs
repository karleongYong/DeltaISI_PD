using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data
{
    public class SLDR_PARAM_BIN_DATA
    {
        public enum SLDR_PARAM_BIN
        {
            DATE_TIME = 0,          //0
            SLDR_LOT_ID,            //1
            SLDR_MTF_NUM,           //2
            SLDR_BIN,               //3
            SLDR_DATA_SOURCE,       //4
            SLDR_F1,                //5
            SLDR_RES,               //6
            SLDR_ASYM,              //7
            SLDR_ADC,               //8
            SLDR_BP,                //9
            SLDR_WPE,               //10
            PRB_TP_WDTH,            //11
            SLDR_PART_NUM,          //12
            SLDR_TAB,               //13
            SLDR_AKL_BP,            //14
            SLDR_TAD_RES,           //15
            SETS_ID,                //16
            SETS_NULL_RECORD,       //17
            SETS_STATUS,            //18
            SLDR_AIR_BEARING_DESIGN,//19
            SLDR_DLC_RUN_NUM,       //20
            SLDR_CHUCK_ID,          //21
            WAF_RDR_HTR_RES,        //22
            WAF_TAD_RES,            //23
            WAF_WTR_RES,            //24
            WAF_WTR_HTR_RES,        //25
            SLDR_F1_RD2,            //26
            SLDR_RES_RD2,           //27
            SLDR_ASYM_RD2,          //28
            ET_TST_STATUS,          //29
            ET_DISPOSITION,         //30
            ET_WORK_ORDER,          //31
            SETS_PRFL_RECORD,       //32
            RD_IBS_PATTERN,         //33
            WR_IBS_PATTERN,         //34
            ET_RD1_RES,             //35
            ET_RD2_RES,             //36
            ET_WRT1_RES,            //37
            ET_WRT2_RES,            //38
            ET_WRT1_HTR_RES,        //39    
            ET_WRT2_HTR_RES,        //40
            ET_RD_HTR_RES,          //41
            ET_TA_RES,              //42
            ET_LAS_THRESHOLD = 45
        }

        public SLDR_PARAM_BIN_DATA()
        {
            Clear();
        }
        #region --- declared ---
        private string _DATE_TIME;
        private string _SLDR_LOT_ID;
        private string _SLDR_MTF_NUM;
        private string _HGA_SITE;
        private string _SLDR_BIN;
        private string _SLDR_DATA_SOURCE;
        private string _SLDR_F1;
        private string _SLDR_RES;
        private string _SLDR_ASYM;
        private string _SLDR_ADC;
        private string _SLDR_BP;
        private string _SLDR_WPE;
        private string _PRB_TP_WDTH;
        private string _STATUS;
        private string _LOAD_DATE;
        private string _SLDR_PART_NUM;
        private string _STATUS_DATE;
        private string _SLDR_TAB;
        private string _SLDR_AKL_BP;
        private string _SLDR_TAD_RES;
        private string _SLDR_AIR_BEARING_DESIGN;
        private string _SLDR_DLC_RUN_NUM;
        private string _SLDR_CHUNK_ID;
        private string _SLDR_RES_ORG;
        private string _WAF_RDR_HTR_RES;
        private string _WAF_TAD_RES;
        private string _WAF_WTR_RES;
        private string _WAF_WTR_HTR_RES;
        private string _SLDR_F1_RD2;
        private string _SLDR_RES_RD2;
        private string _SLDR_ASYM_RD2;
        private string _ET_DISPOSITION;
        private string _AAB_DESIGN;
        private string _TST_STATUS;
        private string _ISI_RES_AT_ET;
        private string _ISI_AMP_AT_ET;
        private string _ISI_ASYM_AT_ET;
        private string _ET_WRT1_RES;
        private string _ET_WRT2_RES;
        private string _ET_RD1_RES;
        private string _ET_RD2_RES;
        private string _RD_IBS_PATTERN;
        private string _WR_IBS_PATTERN;
        private string _ET_LAS_THRESHOLD;
        private string _UTIC_NO;
        #endregion

        #region --- Properties ---
        public string DATE_TIME
        {
            get { return _DATE_TIME; }
            set { _DATE_TIME = value; }
        }
        public string SLDR_LOT_ID
        {
            get { return _SLDR_LOT_ID; }
            set { _SLDR_LOT_ID = value; }
        }

        public string SLDR_MTF_NUM
        {
            get { return _SLDR_MTF_NUM; }
            set { _SLDR_MTF_NUM = value; }
        }

        public string HGA_SITE
        {
            get { return _HGA_SITE; }
            set { _HGA_SITE = value; }
        }

        public string SLDR_BIN
        {
            get { return _SLDR_BIN; }
            set { _SLDR_BIN = value; }
        }

        public string SLDR_DATA_SOURCE
        {
            get { return _SLDR_DATA_SOURCE; }
            set { _SLDR_DATA_SOURCE = value; }
        }

        public string SLDR_F1
        {
            get { return _SLDR_F1; }
            set { _SLDR_F1 = value; }
        }

        public string SLDR_RES
        {
            get { return _SLDR_RES; }
            set { _SLDR_RES = value; }
        }

        public string SLDR_ASYM
        {
            get { return _SLDR_ASYM; }
            set { _SLDR_ASYM = value; }
        }

        public string SLDR_ADC
        {
            get { return _SLDR_ADC; }
            set { _SLDR_ADC = value; }
        }

        public string SLDR_BP
        {
            get { return _SLDR_BP; }
            set { _SLDR_BP = value; }
        }

        public string SLDR_WPE
        {
            get { return _SLDR_WPE; }
            set { _SLDR_WPE = value; }
        }

        public string PRB_TP_WDTH
        {
            get { return _PRB_TP_WDTH; }
            set { _PRB_TP_WDTH = value; }
        }

        public string STATUS
        {
            get { return _STATUS; }
            set { _STATUS = value; }
        }

        public string LOAD_DATE
        {
            get { return _LOAD_DATE; }
            set { _LOAD_DATE = value; }
        }

        public string SLDR_PART_NUM
        {
            get { return _SLDR_PART_NUM; }
            set { _SLDR_PART_NUM = value; }
        }

        public string STATUS_DATE
        {
            get { return _STATUS_DATE; }
            set { _STATUS_DATE = value; }
        }

        public string SLDR_TAB
        {
            get { return _SLDR_TAB; }
            set { _SLDR_TAB = value; }
        }

        public string SLDR_AKL_BP
        {
            get { return _SLDR_AKL_BP; }
            set { _SLDR_AKL_BP = value; }
        }

        public string SLDR_TAD_RES
        {
            get { return _SLDR_TAD_RES; }
            set { _SLDR_TAD_RES = value; }
        }

        public string SLDR_AIR_BEARING_DESIGN
        {
            get { return _SLDR_AIR_BEARING_DESIGN; }
            set { _SLDR_AIR_BEARING_DESIGN = value; }
        }

        public string SLDR_DLC_RUN_NUM
        {
            get { return _SLDR_DLC_RUN_NUM; }
            set { _SLDR_DLC_RUN_NUM = value; }
        }

        public string SLDR_CHUNK_ID
        {
            get { return _SLDR_CHUNK_ID; }
            set { _SLDR_CHUNK_ID = value; }
        }

        public string SLDR_RES_ORG
        {
            get { return _SLDR_RES_ORG; }
            set { _SLDR_RES_ORG = value; }
        }

        public string WAF_RDR_HTR_RES
        {
            get { return _WAF_RDR_HTR_RES; }
            set { _WAF_RDR_HTR_RES = value; }
        }

        public string WAF_TAD_RES
        {
            get { return _WAF_TAD_RES; }
            set { _WAF_TAD_RES = value; }
        }

        public string WAF_WTR_RES
        {
            get { return _WAF_WTR_RES; }
            set { _WAF_WTR_RES = value; }
        }

        public string WAF_WTR_HTR_RES
        {
            get { return _WAF_WTR_HTR_RES; }
            set { _WAF_WTR_HTR_RES = value; }
        }

        public string SLDR_F1_RD2
        {
            get { return _SLDR_F1_RD2; }
            set { _SLDR_F1_RD2 = value; }
        }

        public string SLDR_RES_RD2
        {
            get { return _SLDR_RES_RD2; }
            set { _SLDR_RES_RD2 = value; }
        }

        public string SLDR_ASYM_RD2
        {
            get { return _SLDR_ASYM_RD2; }
            set { _SLDR_ASYM_RD2 = value; }
        }

        public string ET_DISPOSITION
        {
            get { return _ET_DISPOSITION; }
            set { _ET_DISPOSITION = value; }
        }

        public string AAB_DESIGN
        {
            get { return _AAB_DESIGN; }
            set { _AAB_DESIGN = value; }
        }

        public string TST_STATUS
        {
            get { return _TST_STATUS; }
            set { _TST_STATUS = value; }
        }

        public string ISI_RES_AT_ET
        {
            get{return _ISI_RES_AT_ET;}
            set{_ISI_RES_AT_ET = value;}
        }
        public string ISI_AMP_AT_ET
        {
            get{return _ISI_AMP_AT_ET;}
            set{_ISI_AMP_AT_ET = value;}
        }
        public string ISI_ASYM_AT_ET
        {
            get { return _ISI_ASYM_AT_ET; }
            set { _ISI_ASYM_AT_ET = value; }
        }

        public string ET_WRT1_RES
        {
            get { return _ET_WRT1_RES; }
            set { _ET_WRT1_RES = value; }
        }

        public string ET_WRT2_RES
        {
            get { return _ET_WRT2_RES; }
            set { _ET_WRT2_RES = value; }
        }

        public string ET_RD1_RES
        {
            get { return _ET_RD1_RES; }
            set { _ET_RD1_RES = value; }
        }

        public string ET_RD2_RES
        {
            get { return _ET_RD2_RES; }
            set { _ET_RD2_RES = value; }
        }

        public string ISI_RD_IBS_PATTERN
        {
            get { return _RD_IBS_PATTERN; }
            set { _RD_IBS_PATTERN = value; }
        }

        public string ISI_WR_IBS_PATTERN
        {
            get { return _WR_IBS_PATTERN; }
            set { _WR_IBS_PATTERN = value; }
        }

        public string UTIC_MC_NO
        {
            get { return _UTIC_NO; }
            set { _UTIC_NO = value; }
        }

        public string ET_LAS_THRESHOLD
        {
            get { return _ET_LAS_THRESHOLD; }
            set { _ET_LAS_THRESHOLD = value; }
        }
        #endregion

        public void Clear()
        {
            _DATE_TIME = "";
            _SLDR_LOT_ID = "";
            _SLDR_MTF_NUM = "";
            _HGA_SITE = "";
            _SLDR_BIN = "";
            _SLDR_DATA_SOURCE = "";
            _SLDR_F1 = "";
            _SLDR_RES = "0";
            _SLDR_ASYM = "";
            _SLDR_ADC = "";
            _SLDR_BP = "";
            _SLDR_WPE = "";
            _PRB_TP_WDTH = "";
            _STATUS = "";
            _LOAD_DATE = "";
            _SLDR_PART_NUM = "";
            _STATUS_DATE = "";
            _SLDR_TAB = "";
            _SLDR_AKL_BP = "";
            _SLDR_TAD_RES = "";
            _SLDR_AIR_BEARING_DESIGN = "";
            _SLDR_DLC_RUN_NUM = "";
            _SLDR_CHUNK_ID = "";
            _SLDR_RES_ORG = "";
            _WAF_RDR_HTR_RES = "";
            _WAF_TAD_RES = "";
            _WAF_WTR_RES = "";
            _WAF_WTR_HTR_RES = "";
            _SLDR_F1_RD2 = "";
            _SLDR_RES_RD2 = "0";
            _SLDR_ASYM_RD2 = "";
            _AAB_DESIGN = "";
            _TST_STATUS = "";
            _ET_WRT1_RES = "0";
            _ISI_AMP_AT_ET = "";
            _ISI_ASYM_AT_ET = "";
            _ISI_RES_AT_ET = "";
            _RD_IBS_PATTERN = "";
            _WR_IBS_PATTERN = "";
            _UTIC_NO = "";
        }
    }

    public class ISI_Data_Map
    {
        private double _isiReader1;
        private double _isiReader2;
        private string _hga_SN;
        private int _slot;
        public ISI_Data_Map()
        {
            _hga_SN = string.Empty;
            _isiReader1 = 0;
            _isiReader2 = 0;
        }
        public int slot
        {
            get { return _slot; }
            set { _slot = value; }
        }

        public string HgaSN
        {
            get { return _hga_SN; }
            set { _hga_SN = value; }
        }

        public double ISIReader1Data
        {
            get { return _isiReader1; }
            set { _isiReader1 = value; }
        }

        public double ISIReader2Data
        {
            get { return _isiReader2; }
            set { _isiReader2 = value; }
        }
    }

    public class TESTED_DATA_MAP
    {
        public string SN { get; set; }
        public double Reading_RD1 { get; set; }
        public double Reading_RD2 { get; set; }
    }

   [Serializable]
    public class TIC_BIN_DATA
    {
        public enum TIC_PARAM_BIN
        {
            TIC_EQUIP_ID = 0,          //0
            TIC_EVENT_DATE,            //1
            DOCKSIDE
        }

        public TIC_BIN_DATA()
        {
            EQUIP_ID = string.Empty;
            EVENT_DATE = string.Empty;
            ERROR_CODE = String.Empty;
        }
        public string EQUIP_ID { get; set; }
        public string EVENT_DATE { get; set; }
        public string DOCK_SIDE { get; set; }
        public string ERROR_CODE { get; set; }
    }
}
