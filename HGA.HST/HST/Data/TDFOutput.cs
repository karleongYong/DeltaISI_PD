using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.EFI.Log;
using System.IO;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.Utils;
using System.Threading;

namespace Seagate.AAS.HGA.HST.Data
{
    public class TDFOutput
    {
        public const string PreTDFName = "HE";
        public const string PosTDFName = ".BCK";
        public const string NoData = "NO_DATA";
        public class TDFFileLoger : EfiLogger
        {
            public enum Category
            {
                HD_NUM,
                TST_DATE,
                LOCATION,
                TSTR_PART_NUM,
                HGA_PART_NUM,
                TMWI,
                COMMENTS,
                TST_STATUS,
                RSTAT,
                PART_DISPOSITION_CODE,
                SPC_STD,
                DISK_NUM,
                OPER_ID,
                MR_RES,
                CTQ_ISLTN,
                CTQ_ISLTN_FLAG,
                CTQ_WRT_RES,
                CTQ_RD_RES,
                CTQ_HTR_RES,
                CTQ_TA_RES,
                CTQ_HTR_2_RES,
                CTQ_DELTA_ISI,
                NAME86,
                VALUE86,
                NAME24,
                VALUE24,
                NAME25,
                VALUE25,
                NAME26,
                VALUE26,
                NAME27,
                VALUE27,
                NAME28,
                VALUE28,
                NAME29,
                VALUE29,
                NAME30,
                VALUE30,
                NAME31,
                VALUE31,
                NAME32,
                VALUE32,
                NAME33,
                VALUE33,
                NAME34,
                VALUE34,
                NAME35,
                VALUE35,
                NAME36,
                VALUE36,
                NAME37,
                VALUE37,
                NAME38,
                VALUE38,
                NAME39,
                VALUE39,
                NAME40,
                VALUE40,
                NAME41,
                VALUE41,
                NAME42,
                VALUE42,
                NAME43,
                VALUE43,
                NAME44,
                VALUE44,
                NAME12,
                VALUE12,
                NAME93,
                VALUE93,
                NAME94,
                VALUE94,
                NAME95,
                VALUE95,
                NAME96,
                VALUE96,
                NAME97,
                VALUE97,
                NAME98,
                VALUE98,
                NAME99,
                VALUE99,
                NAME100,
                VALUE100,
                NAME101,
                VALUE101,
                NAME102,
                VALUE102,
                NAME103,
                VALUE103,
                NAME104,
                VALUE104,
                NAME105,
                VALUE105,
                NAME106,
                VALUE106,
                NAME107,
                VALUE107,
                POS_ID,
                ERR_CDS1,
                ERR_CDS2,
                ERR_CDS3,
                ERR_CDS4,
                ERR_CDS5,
                ERR_CDS6,
                ERR_CDS7,
                ERR_CDS8,
                CELL_NUM,
                BUILD_NUM,
                WORK_ORDER,
                WORK_ORDER_VER,
                TRAY_ID,
                TEST_TYPE,
                START_DATE,
                TIME,
                SWAP,
                SOFT_REV,
                READER_INDEX,
                ERR_CD,
                ERR_CDH,
                WRITER_CHOICE_FLAG,
                SPEC_NO,
                SPEC_VER,
                PARM_ID,
                SCRIPT_NAME,
                SCRIPT_DATE,
                TSR_NUM,
                TSR_GROUP,
                SLIDER_LOT_ID,
                RADIUS,
                REF_RADIUS,
                RPM,
                REF_RPM,
                SKEW_ANGLE,
                LOAD_RADIUS,
                WAF_TAD_RES,
                WAF_RDR_HTR_RES,
                WAF_WTR_RES,
                WAF_WTR_HTR_RES,
                ISI_RES_AT_ET,
                ISI_AMP_AT_ET,
                ISI_ASYM_AT_ET
                //RD_IBS_PATTERN,
                //WR_IBS_PATTERN
            }

            private DirectoryInfo _dirInfo;
            private string _logPath = string.Empty;
            private string _logName = string.Empty;
            public TDFFileLoger(string loggerPath, string fileName)
                : base(loggerPath, fileName)
            {
                _dirInfo = new DirectoryInfo(loggerPath);
                _logName = fileName;
                _logPath = loggerPath;

                SaveHeaderInfo();
                GenerateTDFPath();
            }

            public override void Dispose()
            {
                base.Dispose();
            }

            // Properties ----------------------------------------------------------
            // Methods -------------------------------------------------------------


            public void LogLine(string data)
            {
                base.Log(data);
            }

            public void LogLine(Category category)
            {
                base.Log((int)category);
            }

            public void LogLine(Category category, string logline)
            {
                string fileName = _logPath + "\\" + _logName;

                base.Log((int)category, logline);

            }

            public string GetStandardTimeStamp()
            {
                return base.GetTimeStamp();
            }

            // Event handlers ------------------------------------------------------

            // Internal methods ----------------------------------------------------


            private void SaveHeaderInfo()
            {
                var header = new StringBuilder();
                int loopIndex = 0;
                foreach (Category category in Enum.GetValues(typeof(Category)))
                {
                    if (loopIndex != 0)
                        header.Append(",");
                    header.Append("\"" +category.ToString() + "\"");
                    loopIndex++;
                }

                string headerFileName = _logPath + "\\" + _logName;

                while (!File.Exists(headerFileName))
                {
                    header.Append("\r\n");
                    using (Stream fs = new FileStream(headerFileName, FileMode.Create))
                    {
                        using (TextWriter writer = new StreamWriter(fs))
                        {
                            writer.Write(header.ToString());
                        }
                    }
                    Thread.Sleep(200);
                }
            }

            public void GenerateTDFPath()
            {
                if (!Directory.Exists(HSTSettings.Instance.Directory.TDFLocalDataPath))
                {
                    Directory.CreateDirectory(HSTSettings.Instance.Directory.TDFLocalDataPath);
                }
            }
        }

        private HSTTDFDATA _hstTdfData;
        private FISTDFDATA _fisTdfData;
        private TDFOutput.TDFFileLoger _tdfLogger;
        public TDFOutput()
        {
            _hstTdfData = new HSTTDFDATA();
            _fisTdfData = new FISTDFDATA();
        }
        public TDFOutput(string path, string filename)
        {
            _hstTdfData = new HSTTDFDATA();
            _fisTdfData = new FISTDFDATA();
            _tdfLogger = new TDFOutput.TDFFileLoger(path, filename);
        }

        public bool IsInProcessing { get; set; }
        public int BackupNameCounter { get; set; }

        public void UpdateHstTdfData(HSTTDFDATA hstData)
        {
            _hstTdfData = hstData;
        }

        public void UpdateFisTdfData(FISTDFDATA fisData)
        {
            _fisTdfData = fisData;
        }

        public void SetDefault()
        {
            _hstTdfData.Default();
            _fisTdfData.Default();
        }

        public void Clear()
        {
            _hstTdfData.Clear();
            _fisTdfData.Clear();

        }

        public string GetCurrentDateTimeByFormat(DateTime datetime)
        {
            return datetime.ToString("dd-MMM-yy:HH:mm:ss").ToUpper();
        }

        public void SaveTDFOutputToFile()
        {
            //HST Data
            var strHstTdfData = string.Empty;
            if(_hstTdfData.WRITER_CHOICE_FLAG == null || _hstTdfData.WRITER_CHOICE_FLAG == string.Empty)
            {
                strHstTdfData = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}," +
                                    "{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}," +
                                    "{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}," +
                                    "{31},{32},{33},{34},{35},{36},{37},{38},{39},{40}," +
                                    "{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51}," +
                                    "{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62}," +
                                    "{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73}," +
                                    "{74},{75},{76},{77},{78},{79},{80},{81},{82},{83},{84}," +
                                    "{85},{86},{87},{88},{89},{90},{91},{92},{93},{94},{95}," +
                                    "{96},{97},{98},{99},{100},{101},{102},{103},{104},{105}," +
                                    "{106},{107},{108},{109},{110},{111},{112},{113},{114}," +
                                    "{115},{116},{117},{118},{119},{120}",
                                    _hstTdfData.HD_NUM,
                                    _hstTdfData.TST_DATE,
                                    _hstTdfData.LOCATION,
                                    _hstTdfData.TSTR_PART_NUM,
                                    _hstTdfData.HGA_PART_NUM,
                                    _hstTdfData.TMWI,
                                    _hstTdfData.COMMENTS,
                                    _hstTdfData.TST_STATUS,
                                    _hstTdfData.RSTAT,
                                    _hstTdfData.PART_DISPOSITION_CODE,
                                    _hstTdfData.SPC_STD,
                                    _hstTdfData.DISK_NUM,
                                    _hstTdfData.OPER_ID,
                                    _hstTdfData.MR_RES,
                                    _hstTdfData.CTQ_ISLTN,
                                    _hstTdfData.CTQ_ISLTN_FLAG,
                                    _hstTdfData.CTQ_WRT_RES,
                                    _hstTdfData.CTQ_RD_RES,
                                    _hstTdfData.CTQ_HTR_RES,
                                    _hstTdfData.CTQ_TA_RES,
                                    _hstTdfData.CTQ_HTR2_RES,
                                    _hstTdfData.CTQ_DELTA_ISI,
                                    _hstTdfData.NAME86,
                                    _hstTdfData.VALUE86,
                                    _hstTdfData.NAME24,
                                    _hstTdfData.VALUE24,
                                    _hstTdfData.NAME25,
                                    _hstTdfData.VALUE25,
                                    _hstTdfData.NAME26,
                                    _hstTdfData.VALUE26,
                                    _hstTdfData.NAME27,
                                    _hstTdfData.VALUE27,
                                    _hstTdfData.NAME28,
                                    _hstTdfData.VALUE28,
                                    _hstTdfData.NAME29,
                                    _hstTdfData.VALUE29,
                                    _hstTdfData.NAME30,
                                    _hstTdfData.VALUE30,
                                    _hstTdfData.NAME31,
                                    _hstTdfData.VALUE31,
                                    _hstTdfData.NAME32,
                                    _hstTdfData.VALUE32,
                                    _hstTdfData.NAME33,
                                    _hstTdfData.VALUE33,
                                    _hstTdfData.NAME34,
                                    _hstTdfData.VALUE34,
                                    _hstTdfData.NAME35,
                                    _hstTdfData.VALUE35,
                                    _hstTdfData.NAME36,
                                    _hstTdfData.VALUE36,
                                    _hstTdfData.NAME37,
                                    _hstTdfData.VALUE37,
                                    _hstTdfData.NAME38,
                                    _hstTdfData.VALUE38,
                                    _hstTdfData.NAME39,
                                    _hstTdfData.VALUE39,
                                    _hstTdfData.NAME40,
                                    _hstTdfData.VALUE40,
                                    _hstTdfData.NAME41,
                                    _hstTdfData.VALUE41,
                                    _hstTdfData.NAME42,
                                    _hstTdfData.VALUE42,
                                    _hstTdfData.NAME43,
                                    _hstTdfData.VALUE43,
                                    _hstTdfData.NAME44,
                                    _hstTdfData.VALUE44,
                                    _hstTdfData.NAME12,
                                    _hstTdfData.VALUE12,
                                    _hstTdfData.NAME93,
                                    _hstTdfData.VALUE93,
                                    _hstTdfData.NAME94,
                                    _hstTdfData.VALUE94,
                                    _hstTdfData.NAME95,
                                    _hstTdfData.VALUE95,
                                    _hstTdfData.NAME96,
                                    _hstTdfData.VALUE96,
                                    _hstTdfData.NAME97,
                                    _hstTdfData.VALUE97,
                                    _hstTdfData.NAME98,
                                    _hstTdfData.VALUE98,
                                    _hstTdfData.NAME99,
                                    _hstTdfData.VALUE99,
                                    _hstTdfData.NAME100,
                                    _hstTdfData.VALUE100,
                                    _hstTdfData.NAME101,
                                    _hstTdfData.VALUE101,
                                    _hstTdfData.NAME102,
                                    _hstTdfData.VALUE102,
                                    _hstTdfData.NAME103,
                                    _hstTdfData.VALUE103,
                                    _hstTdfData.NAME104,
                                    _hstTdfData.VALUE104,
                                    _hstTdfData.NAME105,
                                    _hstTdfData.VALUE105,
                                    _hstTdfData.NAME106,
                                    _hstTdfData.VALUE106,
                                    _hstTdfData.NAME107,
                                    _hstTdfData.VALUE107,
                                    _hstTdfData.POS_ID,
                                    _hstTdfData.ERR_CDS1,
                                    _hstTdfData.ERR_CDS2,
                                    _hstTdfData.ERR_CDS3,
                                    _hstTdfData.ERR_CDS4,
                                    _hstTdfData.ERR_CDS5,
                                    _hstTdfData.ERR_CDS6,
                                    _hstTdfData.ERR_CDS7,
                                    _hstTdfData.ERR_CDS8,
                                    _hstTdfData.CELL_NUM,
                                    _hstTdfData.BUILD_NUM,
                                    _hstTdfData.WORK_ORDER,
                                    _hstTdfData.WORK_ORDER_VER,
                                    _hstTdfData.TRAY_ID,
                                    _hstTdfData.TEST_TYPE,
                                    _hstTdfData.START_DATE,
                                    _hstTdfData.TIME,
                                    _hstTdfData.SWAP,
                                    _hstTdfData.SOFT_REV,
                                    _hstTdfData.READER_INDEX,
                                    _hstTdfData.ERR_CD,
                                    _hstTdfData.ERR_CDH,
                                    _hstTdfData.WRITER_CHOICE_FLAG);
            }
            else
            {
                strHstTdfData = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}," +
                    "{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}," +
                    "{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}," +
                    "{31},{32},{33},{34},{35},{36},{37},{38},{39},{40}," +
                    "{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51}," +
                    "{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62}," +
                    "{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73}," +
                    "{74},{75},{76},{77},{78},{79},{80},{81},{82},{83},{84}," +
                    "{85},{86},{87},{88},{89},{90},{91},{92},{93},{94},{95}," +
                    "{96},{97},{98},{99},{100},{101},{102},{103},{104},{105}," +
                    "{106},{107},{108},{109},{110},{111},{112},{113},{114}," +
                    "{115},{116},{117},{118},{119},{120}",
                    _hstTdfData.HD_NUM,
                    _hstTdfData.TST_DATE,
                    _hstTdfData.LOCATION,
                    _hstTdfData.TSTR_PART_NUM,
                    _hstTdfData.HGA_PART_NUM,
                    _hstTdfData.TMWI,
                    _hstTdfData.COMMENTS,
                    _hstTdfData.TST_STATUS,
                    _hstTdfData.RSTAT,
                    _hstTdfData.PART_DISPOSITION_CODE,
                    _hstTdfData.SPC_STD,
                    _hstTdfData.DISK_NUM,
                    _hstTdfData.OPER_ID,
                    _hstTdfData.MR_RES,
                    _hstTdfData.CTQ_ISLTN,
                    _hstTdfData.CTQ_ISLTN_FLAG,
                    _hstTdfData.CTQ_WRT_RES,
                    _hstTdfData.CTQ_RD_RES,
                    _hstTdfData.CTQ_HTR_RES,
                    _hstTdfData.CTQ_TA_RES,
                    _hstTdfData.CTQ_HTR2_RES,
                    _hstTdfData.CTQ_DELTA_ISI,
                    _hstTdfData.NAME86,
                    _hstTdfData.VALUE86,
                    _hstTdfData.NAME24,
                    _hstTdfData.VALUE24,
                    _hstTdfData.NAME25,
                    _hstTdfData.VALUE25,
                    _hstTdfData.NAME26,
                    _hstTdfData.VALUE26,
                    _hstTdfData.NAME27,
                    _hstTdfData.VALUE27,
                    _hstTdfData.NAME28,
                    _hstTdfData.VALUE28,
                    _hstTdfData.NAME29,
                    _hstTdfData.VALUE29,
                    _hstTdfData.NAME30,
                    _hstTdfData.VALUE30,
                    _hstTdfData.NAME31,
                    _hstTdfData.VALUE31,
                    _hstTdfData.NAME32,
                    _hstTdfData.VALUE32,
                    _hstTdfData.NAME33,
                    _hstTdfData.VALUE33,
                    _hstTdfData.NAME34,
                    _hstTdfData.VALUE34,
                    _hstTdfData.NAME35,
                    _hstTdfData.VALUE35,
                    _hstTdfData.NAME36,
                    _hstTdfData.VALUE36,
                    _hstTdfData.NAME37,
                    _hstTdfData.VALUE37,
                    _hstTdfData.NAME38,
                    _hstTdfData.VALUE38,
                    _hstTdfData.NAME39,
                    _hstTdfData.VALUE39,
                    _hstTdfData.NAME40,
                    _hstTdfData.VALUE40,
                    _hstTdfData.NAME41,
                    _hstTdfData.VALUE41,
                    _hstTdfData.NAME42,
                    _hstTdfData.VALUE42,
                    _hstTdfData.NAME43,
                    _hstTdfData.VALUE43,
                    _hstTdfData.NAME44,
                    _hstTdfData.VALUE44,
                    _hstTdfData.NAME12,
                    _hstTdfData.VALUE12,
                    _hstTdfData.NAME93,
                    _hstTdfData.VALUE93,
                    _hstTdfData.NAME94,
                    _hstTdfData.VALUE94,
                    _hstTdfData.NAME95,
                    _hstTdfData.VALUE95,
                    _hstTdfData.NAME96,
                    _hstTdfData.VALUE96,
                    _hstTdfData.NAME97,
                    _hstTdfData.VALUE97,
                    _hstTdfData.NAME98,
                    _hstTdfData.VALUE98,
                    _hstTdfData.NAME99,
                    _hstTdfData.VALUE99,
                    _hstTdfData.NAME100,
                    _hstTdfData.VALUE100,
                    _hstTdfData.NAME101,
                    _hstTdfData.VALUE101,
                    _hstTdfData.NAME102,
                    _hstTdfData.VALUE102,
                    _hstTdfData.NAME103,
                    _hstTdfData.VALUE103,
                    _hstTdfData.NAME104,
                    _hstTdfData.VALUE104,
                    _hstTdfData.NAME105,
                    _hstTdfData.VALUE105,
                    _hstTdfData.NAME106,
                    _hstTdfData.VALUE106,
                    _hstTdfData.NAME107,
                    _hstTdfData.VALUE107,
                    _hstTdfData.POS_ID,
                    _hstTdfData.ERR_CDS1,
                    _hstTdfData.ERR_CDS2,
                    _hstTdfData.ERR_CDS3,
                    _hstTdfData.ERR_CDS4,
                    _hstTdfData.ERR_CDS5,
                    _hstTdfData.ERR_CDS6,
                    _hstTdfData.ERR_CDS7,
                    _hstTdfData.ERR_CDS8,
                    _hstTdfData.CELL_NUM,
                    _hstTdfData.BUILD_NUM,
                    _hstTdfData.WORK_ORDER,
                    _hstTdfData.WORK_ORDER_VER,
                    _hstTdfData.TRAY_ID,
                    _hstTdfData.TEST_TYPE,
                    _hstTdfData.START_DATE,
                    _hstTdfData.TIME,
                    _hstTdfData.SWAP,
                    _hstTdfData.SOFT_REV,
                    _hstTdfData.READER_INDEX,
                    _hstTdfData.ERR_CD,
                    _hstTdfData.ERR_CDH,
                    _hstTdfData.WRITER_CHOICE_FLAG);
            }


            //FIS Data
            var strFisTdfData = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}," +
                                                "{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                                                _fisTdfData.SPEC_NO,
                                                _fisTdfData.SPEC_VER,
                                                _fisTdfData.PARM_ID,
                                                _fisTdfData.SCRIPT_NAME,
                                                _fisTdfData.SCRIPT_DATE,
                                                _fisTdfData.TSR_NUM,
                                                _fisTdfData.TSR_GROUP,
                                                _fisTdfData.SLIDER_LOT_ID,
                                                _fisTdfData.RADIUS,
                                                _fisTdfData.REF_RADIUS,
                                                _fisTdfData.RPM,
                                                _fisTdfData.REF_RPM,
                                                DisplayFormatter.Round(_fisTdfData.SKEW_ANGLE,DisplayFormatter.DecimalDigits.Four),
                                                _fisTdfData.LOAD_RADIUS,
                                                _fisTdfData.WAF_TAD_RES,
                                                _fisTdfData.WAF_RDR_HTR_RES,
                                                _fisTdfData.WAF_WTR_RES,
                                                _fisTdfData.WAF_WTR_HTR_RES,
                                                _fisTdfData.ISI_RES_AT_ET,
                                                _fisTdfData.ISI_AMP_AT_ET,
                                                _fisTdfData.ISI_ASYM_AT_ET);

            var strTdfData = strHstTdfData + "," + strFisTdfData;

            _tdfLogger.LogLine(strTdfData);
        }

        public class HSTTDFDATA
        {

                string _hd_num = string.Empty;
                string _tst_date = string.Empty;
                string _location = string.Empty;
                string _tstr_partnumber = string.Empty;
                string _hga_partnumber = string.Empty;
                string _fsa_partnumber = string.Empty;
                string _tmwi = string.Empty;
                string _comments = string.Empty;
                string _tst_status = string.Empty;
                string _rstat = string.Empty;
                string _part_disposition_code = string.Empty;
                string _spc_std = string.Empty;
                string _disk_num = string.Empty;
                string _opertor_id = string.Empty;
                string _mr_res = string.Empty;
                string _ctq_isltn = string.Empty;
                string _ctq_isltn_flag = string.Empty;
                string _ctq_wrt_res = string.Empty;
                string _ctq_rd_res = string.Empty;
                string _ctq_htr_res = string.Empty;
                string _ctq_ta_res = string.Empty;
                string _ctq_htr2_res = string.Empty;
                string _ctq_delta_isi = string.Empty;
                string _name9 = string.Empty;
                string _value9 = string.Empty;
                string _name24 = string.Empty;
                string _value24 = string.Empty;
                string _name25 = string.Empty;
                string _value25 = string.Empty;
                string _name26 = string.Empty;
                string _value26 = string.Empty;
                string _name27 = string.Empty;
                string _value27 = string.Empty;
                string _name28 = string.Empty;
                string _value28 = string.Empty;
                string _name29 = string.Empty;
                string _value29 = string.Empty;
                string _name30 = string.Empty;
                string _value30 = string.Empty;
                string _name31 = string.Empty;
                string _value31 = string.Empty;
                string _name32 = string.Empty;
                string _value32 = string.Empty;
                string _name33 = string.Empty;
                string _value33 = string.Empty;
                string _name34 = string.Empty;
                string _value34 = string.Empty;
                string _name35 = string.Empty;
                string _value35 = string.Empty;
                string _name36 = string.Empty;
                string _value36 = string.Empty;
                string _name37 = string.Empty;
                string _value37 = string.Empty;
                string _name38 = string.Empty;
                string _value38 = string.Empty;
                string _name39 = string.Empty;
                string _value39 = string.Empty;
                string _name40 = string.Empty;
                string _value40 = string.Empty;
                string _name41 = string.Empty;
                string _value41 = string.Empty;
                string _name42 = string.Empty;
                string _value42 = string.Empty;
                string _name43 = string.Empty;
                string _value43 = string.Empty;
                string _name44 = string.Empty;
                string _value44 = string.Empty;
                string _name12 = string.Empty;
                string _value12 = string.Empty;
                string _name93 = string.Empty;
                string _value93 = string.Empty;
                string _name94 = string.Empty;
                string _value94 = string.Empty;
                string _name95 = string.Empty;
                string _value95 = string.Empty;
                string _name96 = string.Empty;
                string _value96 = string.Empty;
                string _name97 = string.Empty;
                string _value97 = string.Empty;
                string _name98 = string.Empty;
                string _value98 = string.Empty;
                string _name99 = string.Empty;
                string _value99 = string.Empty;
                string _name100 = string.Empty;
                string _value100 = string.Empty;
                string _name101 = string.Empty;
                string _value101 = string.Empty;
                string _name102 = string.Empty;
                string _value102 = string.Empty;
                string _name103 = string.Empty;
                string _value103 = string.Empty;
                string _name104 = string.Empty;
                string _value104 = string.Empty;
                string _name105 = string.Empty;
                string _value105 = string.Empty;
                string _name106 = string.Empty;
                string _value106 = string.Empty;
                string _name107 = string.Empty;
                string _value107 = string.Empty;
                string _pos_id = string.Empty;
                string _err_cds1 = string.Empty;
                string _err_cds2 = string.Empty;
                string _err_cds3 = string.Empty;
                string _err_cds4 = string.Empty;
                string _err_cds5 = string.Empty;
                string _err_cds6 = string.Empty;
                string _err_cds7 = string.Empty;
                string _err_cds8 = string.Empty;
                string _cel_num = string.Empty;
                string _build_num = string.Empty;
                string _work_order = string.Empty;
                string _work_order_ver = string.Empty;
                string _tray_id = string.Empty;
                string _test_type = string.Empty;
                string _start_date = string.Empty;
                string _time = string.Empty;
                string _swap = string.Empty;
                string _soft_rev = string.Empty;
                string _reader_index = string.Empty;
                string _err_cd = string.Empty;
                string _err_cdh = string.Empty;
                string _writer_choice_flag = string.Empty;
                string _rd_ibs_pattern = string.Empty;
                string _wr_ibs_pattern = string.Empty;
            /// <summary>
            /// HD num from RFID
            /// </summary>
            public string HD_NUM { get { return "\"" + _hd_num.Trim() + "\""; } set { _hd_num = value; } }

            /// <summary>
            /// Stamp date/time
            /// </summary>
                public string TST_DATE { get { return "\"" + _tst_date.Trim() + "\""; } set {_tst_date = value;} }  //HST /D /09-JAN-18:13:16:55

            /// <summary>
            /// Stamp "BKK"
            /// </summary>
                public string LOCATION { get { return "\"" + _location.Trim() + "\""; } set { _location = value; } }    

            /// <summary>
            /// HST Machine number
            /// </summary>
                public string TSTR_PART_NUM { get { return "\"" + _tstr_partnumber.Trim() + "\""; } set {_tstr_partnumber = value;} }   

            /// <summary>
            /// Part number HGA
            /// TSR Saleforce
            /// </summary>
                public string HGA_PART_NUM { get { return "\"" + _hga_partnumber.Trim() + "\""; } set { _hga_partnumber = value; } }    
                public string FSA_PART_NUM { get { return "\"" + _fsa_partnumber.Trim() + "\""; } set { _fsa_partnumber = value; } }    

            /// <summary>
            /// Workorder Name
            /// </summary>
                public string TMWI { get { return "\"" +_tmwi.Trim() + "\""; } set { _tmwi = value; } }        

            /// <summary>
            /// Write "HST" to this
            /// </summary>
                public string COMMENTS { get { return "\"" + _comments.Trim() + "\""; } set { _comments = value; } }    

            /// <summary>
            /// Sort number
            /// 1.1 Read sort from ET-SDET
            /// 1.2 Grading sort by self
            /// </summary>
                public string TST_STATUS { get { return "\"" + _tst_status.Trim() + "\""; } set { _tst_status = value; } }      

            /// <summary>
            /// Same as TST_Status
            /// </summary>
                public string RSTAT { get { return "\"" + _rstat.Trim() + "\""; } set { _rstat = value; } }       

            /// <summary>
            /// 1.1 Write "SHIP" for Good part
            /// 1.2 Write "HOLD" for fail part
            /// </summary>
                public string PART_DISPOSITION_CODE { get { return "\"" + _part_disposition_code.Trim() + "\""; } set { _part_disposition_code = value; } }   

            /// <summary>
            /// Write R 
            /// </summary>
                public string SPC_STD { get { return "\"" + _spc_std.Trim() + "\""; } set { _spc_std = value; } }     

            /// <summary>
            /// Write "NO_DISK" 
            /// </summary>
                public string DISK_NUM { get { return "\"" + _disk_num.Trim() + "\""; } set { _disk_num = value; } }    

            /// <summary>
            /// Write GID access
            /// </summary>
                public string OPER_ID { get { return "\"" + _opertor_id.Trim() + "\""; } set { _opertor_id = value; } }     

            /// <summary>
            /// Writer to heater short 
            /// 1.1 Pass stamp "0"
            /// 1.2 Fail stamp "1" 
            /// </summary>
                public string MR_RES { get { return "\"" + _mr_res.Trim() + "\""; } set { _mr_res = value; } }       

            /// <summary>
            /// Writer to heater short 
            /// 1.1 Pass stamp "0"
            /// 1.2 Fail stamp "1" 
            /// </summary>
                public string CTQ_ISLTN { get { return "\"" + _ctq_isltn.Trim() + "\""; } set { _ctq_isltn = value; } }       

            /// <summary>
            /// Writer short to ground 
            /// 1.1 Pass stamp "0"
            /// 1.2 Fail stamp "1" 
            /// </summary>
                public string CTQ_ISLTN_FLAG { get { return "\"" +_ctq_isltn_flag.Trim() + "\""; } set { _ctq_isltn_flag = value; } }      

            /// <summary>
            /// Data writer resistance
            /// </summary>
                public string CTQ_WRT_RES { get { return "\"" + _ctq_wrt_res.Trim() + "\""; } set { _ctq_wrt_res = value; } }     

            /// <summary>
            /// Data reader resistance
            /// </summary>
                public string CTQ_RD_RES { get { return "\"" + _ctq_rd_res.Trim() + "\""; } set { _ctq_rd_res = value; } }      

            /// <summary>
            /// Data Heater read resistance
            /// </summary>
                public string CTQ_HTR_RES { get { return "\"" + _ctq_htr_res.Trim() + "\""; } set { _ctq_htr_res = value; } }     

            /// <summary>
            /// Data TA resistance
            /// </summary>
                public string CTQ_TA_RES { get { return "\"" + _ctq_ta_res.Trim() + "\""; } set { _ctq_ta_res = value; } }      

            /// <summary>
            /// Data Heater write resistance
            /// </summary>
                public string CTQ_HTR2_RES { get { return "\"" + _ctq_htr2_res.Trim() + "\""; } set { _ctq_htr2_res = value; } }    

            /// <summary>
            /// As same as Value12
            /// </summary>
            public string CTQ_DELTA_ISI { get { return "\"" + _ctq_delta_isi.ToString() + "\""; } set { _ctq_delta_isi = value; } }    

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
            public string NAME86 { get { return "\"" + _name9.Trim() + "\""; } set { _name9 = value; } }       

            /// <summary>
            /// Data sort 
            /// </summary>
                public string VALUE86 { get { return "\"" +_value9.Trim() + "\""; } set { _value9 = value; } }      

            /// <summary>
            /// Sort value 
            /// 1.1 Read sort from ET-SDET
            /// 1.2 Grading sort by self
            /// </summary>
                public string NAME11 { get { return "\"" + _name27.Trim() + "\""; } set { _name27 = value; } }      

            /// <summary>
            /// ISI Resistance check 
            /// </summary>
                public string VALUE11 { get { return "\"" + _value27.Trim() + "\""; } set { _value27 = value; } }     

            /// <summary>
            /// Name for LDU resistance test data
            /// </summary>
            public string NAME24 { get { return "\"" + _name24.Trim() + "\""; } set { _name24 = value; } }      

            /// <summary>
            /// Value for LDU resistance test data
            /// </summary>
            public string VALUE24 { get { return "\"" + _value24.Trim() + "\""; } set { _value24 = value; } }     

            public string NAME25 { get { return "\"" + _name25.Trim() + "\""; } set { _name25 = value; } }       
            public string VALUE25 { get { return "\"" + _value25.Trim() + "\""; } set { _value25 = value; } }      

            public string NAME26 { get { return "\"" + _name26.Trim() + "\""; } set { _name26 = value; } }       
            public string VALUE26 { get { return "\"" + _value26.Trim() + "\""; } set { _value26 = value; } }      

            public string NAME27 { get { return "\"" + _name27.Trim() + "\""; } set { _name27 = value; } }       
            public string VALUE27 { get { return "\"" + _value27.Trim() + "\""; } set { _value27 = value; } }

            public string NAME28 { get { return "\"" + _name28.Trim() + "\""; } set { _name28 = value; } }
            public string VALUE28 { get { return "\"" + _value28.Trim() + "\""; } set { _value28 = value; } }

            public string NAME29 { get { return "\"" + _name29.Trim() + "\""; } set { _name29 = value; } }
            public string VALUE29 { get { return "\"" + _value29.Trim() + "\""; } set { _value29 = value; } }

            public string NAME30 { get { return "\"" + _name30.Trim() + "\""; } set { _name30 = value; } }
            public string VALUE30 { get { return "\"" + _value30.Trim() + "\""; } set { _value30 = value; } }

            public string NAME31 { get { return "\"" + _name31.Trim() + "\""; } set { _name31 = value; } }
            public string VALUE31 { get { return "\"" + _value31.Trim() + "\""; } set { _value31 = value; } }

            public string NAME32 { get { return "\"" + _name32.Trim() + "\""; } set { _name32 = value; } }
            public string VALUE32 { get { return "\"" + _value32.Trim() + "\""; } set { _value32 = value; } }

            public string NAME33 { get { return "\"" + _name33.Trim() + "\""; } set { _name33 = value; } }
            public string VALUE33 { get { return "\"" + _value33.Trim() + "\""; } set { _value33 = value; } }

            public string NAME34 { get { return "\"" + _name34.Trim() + "\""; } set { _name34 = value; } }
            public string VALUE34 { get { return "\"" + _value34.Trim() + "\""; } set { _value34 = value; } }

            public string NAME35 { get { return "\"" + _name35.Trim() + "\""; } set { _name35 = value; } }
            public string VALUE35 { get { return "\"" + _value35.Trim() + "\""; } set { _value35 = value; } }

            public string NAME36 { get { return "\"" + _name36.Trim() + "\""; } set { _name36 = value; } }
            public string VALUE36 { get { return "\"" + _value36.Trim() + "\""; } set { _value36 = value; } }

            public string NAME37 { get { return "\"" + _name37.Trim() + "\""; } set { _name37 = value; } }
            public string VALUE37 { get { return "\"" + _value37.Trim() + "\""; } set { _value37 = value; } }

            public string NAME38 { get { return "\"" + _name38.Trim() + "\""; } set { _name38 = value; } }
            public string VALUE38 { get { return "\"" + _value38.Trim() + "\""; } set { _value38 = value; } }

            public string NAME39 { get { return "\"" + _name39.Trim() + "\""; } set { _name39 = value; } }
            public string VALUE39 { get { return "\"" + _value39.Trim() + "\""; } set { _value39 = value; } }

            public string NAME40 { get { return "\"" + _name40.Trim() + "\""; } set { _name40 = value; } }
            public string VALUE40 { get { return "\"" + _value40.Trim() + "\""; } set { _value40 = value; } }

            public string NAME41 { get { return "\"" + _name41.Trim() + "\""; } set { _name41 = value; } }
            public string VALUE41 { get { return "\"" + _value41.Trim() + "\""; } set { _value41 = value; } }

            public string NAME42 { get { return "\"" + _name42.Trim() + "\""; } set { _name42 = value; } }
            public string VALUE42 { get { return "\"" + _value42.Trim() + "\""; } set { _value42 = value; } }

            public string NAME43 { get { return "\"" + _name43.Trim() + "\""; } set { _name43 = value; } }
            public string VALUE43 { get { return "\"" + _value43.Trim() + "\""; } set { _value43 = value; } }

            public string NAME44 { get { return "\"" + _name44.Trim() + "\""; } set { _name44 = value; } }
            public string VALUE44 { get { return "\"" + _value44.Trim() + "\""; } set { _value44 = value; } }


            /// <summary>
            /// Delta ISI calculation
            /// </summary>
            public string NAME12 { get { return "\"" + _name12.Trim() + "\""; } set { _name12 = value; } }     
            public string VALUE12 { get { return "\"" + _value12.Trim() + "\""; } set { _value12 = value; } }     

            /// <summary>
            /// Writer resistance from SDET
            /// </summary>
            public string NAME93 { get { return "\"" + _name93.Trim() + "\""; } set { _name93 = value; } }      
            public string VALUE93 { get { return "\"" + _value93.Trim() + "\""; } set { _value93 = value; } }     

            /// <summary>
            /// Delta writer resistance of HST and SDET
            /// </summary>
            public string NAME94 { get { return "\"" + _name94.Trim() + "\""; } set { _name94 = value; } }      
            public string VALUE94 { get { return "\"" + _value94.Trim() + "\""; } set { _value94 = value; } }     

            /// <summary>
            /// % Code E
            /// </summary>
            public string NAME95 { get { return "\"" + _name95.Trim() + "\""; } set { _name95 = value; } }      
            public string VALUE95 { get { return "\"" + _value95.Trim() + "\""; } set { _value95 = value; } }     

            /// <summary>
            /// Reader resistance from SDET separate by reader index
            /// </summary>
            public string NAME96 { get { return "\"" + _name96.Trim() + "\""; } set { _name96 = value; } }     
            public string VALUE96 { get { return "\"" + _value96.Trim() + "\""; } set { _value96 = value; } }     

            /// <summary>
            /// Current writer bridgeing adaptive spec
            /// </summary>
            public string NAME97 { get { return "\"" + _name97.Trim() + "\""; } set { _name97 = value; } }      
            public string VALUE97 { get { return "\"" + _value97.Trim() + "\""; } set { _value97 = value; } }

            public string NAME98 { get { return "\"" + _name98.Trim() + "\""; } set { _name98 = value; } }
            public string VALUE98 { get { return "\"" + _value98.Trim() + "\""; } set { _value98 = value; } }

            public string NAME99 { get { return "\"" + _name99.Trim() + "\""; } set { _name99 = value; } }
            public string VALUE99 { get { return "\"" + _value99.Trim() + "\""; } set { _value99 = value; } }

            public string NAME100 { get { return "\"" + _name100.Trim() + "\""; } set { _name100 = value; } }
            public string VALUE100 { get { return "\"" + _value100.Trim() + "\""; } set { _value100 = value; } }

            public string NAME101 { get { return "\"" + _name101.Trim() + "\""; } set { _name101 = value; } }
            public string VALUE101 { get { return "\"" + _value101.Trim() + "\""; } set { _value101 = value; } }

            public string NAME102 { get { return "\"" + _name102.Trim() + "\""; } set { _name102 = value; } }
            public string VALUE102 { get { return "\"" + _value102.Trim() + "\""; } set { _value102 = value; } }

            public string NAME103 { get { return "\"" + _name103.Trim() + "\""; } set { _name103 = value; } }
            public string VALUE103 { get { return "\"" + _value103.Trim() + "\""; } set { _value103 = value; } }

            public string NAME104 { get { return "\"" + _name104.Trim() + "\""; } set { _name104 = value; } }
            public string VALUE104 { get { return "\"" + _value104.Trim() + "\""; } set { _value104 = value; } }

            public string NAME105 { get { return "\"" + _name105.Trim() + "\""; } set { _name105 = value; } }
            public string VALUE105 { get { return "\"" + _value105.Trim() + "\""; } set { _value105 = value; } }

            public string NAME106 { get { return "\"" + _name106.Trim() + "\""; } set { _name106 = value; } }
            public string VALUE106 { get { return "\"" + _value106.Trim() + "\""; } set { _value106 = value; } }

            public string NAME107 { get { return "\"" + _name107.Trim() + "\""; } set { _name107 = value; } }
            public string VALUE107 { get { return "\"" + _value107.Trim() + "\""; } set { _value107 = value; } }


            /// <summary>
            /// Resistance ISI fail or pass
            /// 1.1 Pass stamp "0"
            /// 1.2 Fail stamp "1" 
            /// </summary>
            public string POS_ID { get { return "\"" + _pos_id.Trim() + "\""; } set { _pos_id = value; } }      

            /// <summary>
            /// Carrier slot 
            /// Stamp" C0P01" is Carrier 0 Position 1 
            /// </summary>
                public string ERR_CDS1 { get { return "\"" + _err_cds1.Trim() + "\""; } set { _err_cds1 = value; } }       

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS2 { get { return "\"" +_err_cds2.Trim() + "\""; } set { _err_cds2 = value; } }      

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS3 { get { return "\"" + _err_cds3.Trim() + "\""; } set { _err_cds3 = value; } }      

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS4 { get { return "\"" + _err_cds4.Trim() + "\""; } set { _err_cds4 = value; } }      

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS5 { get { return "\"" + _err_cds5.Trim() + "\""; } set { _err_cds5 = value; } }      

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS6 { get { return "\"" + _err_cds6.Trim() + "\""; } set { _err_cds6 = value; } }      

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS7 { get { return "\"" + _err_cds7.Trim()  + "\""; } set { _err_cds7 = value; } }       

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string ERR_CDS8 { get { return "\"" + _err_cds8.Trim() + "\""; } set { _err_cds8 = value; } }       

            /// <summary>
            /// Fail sort 1
            /// Check "VALUE9"
            /// </summary>
                public string CELL_NUM { get { return "\"" + _cel_num.Trim() + "\""; } set { _cel_num = value; } }      

            /// <summary>
            /// Machine location 
            /// </summary>
                public string BUILD_NUM { get { return "\"" + _build_num.Trim() + "\""; } set { _build_num = value; } }      

            /// <summary>
            /// Work order
            /// </summary>
                public string WORK_ORDER { get { return "\"" + _work_order.Trim() + "\""; } set { _work_order = value; } }    

            /// <summary>
            /// Work order
            /// </summary>
                public string WORK_ORDER_VER { get { return "\"" +_work_order_ver.Trim() + "\""; } set { _work_order_ver = value; } }     

            /// <summary>
            /// Work order no,
            /// </summary>
                public string TRAY_ID { get { return "\"" + _tray_id.Trim() + "\""; } set { _tray_id = value; } }   

            /// <summary>
            /// Carrier ID
            /// </summary>
                public string TEST_TYPE { get { return "\"" + _test_type.Trim() + "\""; } set { _test_type = value; } }      

            /// <summary>
            /// Mode of machine 
            /// 1.1 PRODUCTION
            /// 1.2 ENGINEERING
            /// 1.3 PROCESS DEV
            /// </summary>
                public string START_DATE { get { return "\"" + _start_date.Trim() + "\""; } set { _start_date = value; } }  

            /// <summary>
            /// Stamp date/time
            /// </summary>
                public string TIME { get { return "\"" + _time.Trim() + "\""; } set { _time = value; } }       

            /// <summary>
            /// Test time/boat
            /// </summary>
                public string SWAP { get { return "\"" + _swap.Trim() + "\""; } set { _swap = value; } }      

            /// <summary>
            /// Time swap/boat 
            /// </summary>
                public string SOFT_REV { get { return "\"" + _soft_rev.Trim() + "\""; } set { _soft_rev = value; } }      

            /// <summary>
            /// Software version
            /// </summary>
                public string READER_INDEX { get { return "\"" + _reader_index.Trim() + "\""; } set { _reader_index = value; } }     

            /// <summary>
            /// Reader index test
            /// 1.1 Reader 1 ==> "1" 
            /// 1.1 Reader 2==> "2" 
            /// </summary>
                public string ERR_CD { get { return "\"" + _err_cd + "\""; } set { _err_cd = value; } }    

            /// <summary>
            /// 1..1 Good part ==> "blank"
            /// 1.2 Fail part ==> stamp ERR code 
            /// </summary>
                public string ERR_CDH { get { return "\"" + _err_cdh + "\""; } set { _err_cdh = value; } }   

            public string WRITER_CHOICE_FLAG { get { return "\"" + _writer_choice_flag + "\""; } set { _writer_choice_flag = value; } }

            public string RD_IBS_PATTERN { get { return "\"" + _rd_ibs_pattern + "\""; } set { _rd_ibs_pattern = value; } }

            public string WR_IBS_PATTERN { get { return "\"" + _wr_ibs_pattern + "\""; } set { _wr_ibs_pattern = value; } }

            public void Default()
            {
                var dateTime = DateTime.Now;
                HD_NUM = "RJCMD5K7O0";
                TST_DATE = GetCurrentDateTimeByFormat(DateTime.Now);
                LOCATION = "BKK";
                TSTR_PART_NUM = "HST00001";
                HGA_PART_NUM = "100815213";
                FSA_PART_NUM = "736477201";
                TMWI = "PESSN003";
                COMMENTS = "HST";
                TST_STATUS = "8";
                RSTAT = "8";
                PART_DISPOSITION_CODE = "HSIP";
                SPC_STD = "R";
                DISK_NUM = "NO_DISK";
                OPER_ID = "24135";
                MR_RES = string.Empty;
                CTQ_ISLTN = "1";
                CTQ_ISLTN_FLAG = "0";
                CTQ_WRT_RES = "5.8442";
                CTQ_RD_RES = "445.2958";
                CTQ_HTR_RES = "55.4509";
                CTQ_TA_RES = "78.1427";
                CTQ_HTR2_RES = "56.2897";
                CTQ_DELTA_ISI = "1";
                NAME86 = "DISPOS_FLG";
                VALUE86 = "8";
                NAME11 = "HGA_ISI_RES_FLG";
                VALUE11 = "1";
                NAME24 = "LAS_IV_RES_OHM";
                VALUE24 = "30.04";
                NAME25 = "LED_INTCP";
                VALUE25 = "1.22";
                NAME26 = "LAS_ITHRESHOLD_MA";
                VALUE26 = "12.32";
                NAME27 = "LAS_PD_VOLT";
                VALUE27 = "1.22";
                POS_ID = "T0P05";
                ERR_CDS1 = "V09+";
                ERR_CDS2 = "V09+";
                ERR_CDS3 = "V09+";
                ERR_CDS4 = "V09+";
                ERR_CDS5 = "V09+";
                ERR_CDS6 = "V09+";
                ERR_CDS7 = "V09+";
                ERR_CDS8 = "PASS";
                CELL_NUM = "A46-11";
                BUILD_NUM = "PESSN003";
                WORK_ORDER = "P0SN003D";
                WORK_ORDER_VER = "1";
                TRAY_ID = "X75";
                TEST_TYPE = "PRODUCTION";
                START_DATE = GetCurrentDateTimeByFormat(DateTime.Now);
                TIME = "29.438";
                SWAP = "3.641";
                SOFT_REV = "W32.00B";
                READER_INDEX = "2";
                ERR_CD = string.Empty;
                ERR_CDH = string.Empty;
                WRITER_CHOICE_FLAG = string.Empty;
                RD_IBS_PATTERN = string.Empty;
                WR_IBS_PATTERN = string.Empty;
            }

            public string GetCurrentDateTimeByFormat(DateTime datetime)
            {
                return datetime.ToString("dd-MMM-yy:HH:mm:ss").ToUpper();
            }

            public void Clear()
            {
                HD_NUM = string.Empty;
                TST_DATE = string.Empty;
                LOCATION = "BKK";
                TSTR_PART_NUM = string.Empty;
                HGA_PART_NUM = string.Empty;
                FSA_PART_NUM = string.Empty;
                TMWI = string.Empty;
                COMMENTS = "HST";
                TST_STATUS = string.Empty;
                RSTAT = string.Empty;
                PART_DISPOSITION_CODE = string.Empty;
                SPC_STD = "R";
                DISK_NUM = "NO_DISK";
                OPER_ID = string.Empty;
                MR_RES = string.Empty;
                CTQ_ISLTN = string.Empty;
                CTQ_ISLTN_FLAG = string.Empty;
                CTQ_WRT_RES = string.Empty;
                CTQ_RD_RES = string.Empty;
                CTQ_HTR_RES = string.Empty;
                CTQ_TA_RES = string.Empty;
                CTQ_HTR2_RES = string.Empty;
                CTQ_DELTA_ISI = string.Empty;
                NAME86 = string.Empty;
                VALUE86 = string.Empty;
                NAME11 = string.Empty;
                VALUE11 = string.Empty;
                NAME24 = string.Empty;
                VALUE24 = string.Empty;
                NAME25 = String.Empty;
                VALUE25 = String.Empty;
                NAME26 = String.Empty;
                VALUE26 = String.Empty;
                NAME27 = String.Empty;
                VALUE27 = String.Empty;
                NAME12 = string.Empty;
                VALUE12 = string.Empty;
                NAME93 = string.Empty;
                VALUE93 = string.Empty;
                NAME94 = string.Empty;
                VALUE94 = string.Empty;
                NAME95 = string.Empty;
                VALUE95 = string.Empty;
                NAME96 = string.Empty;
                VALUE96 = string.Empty;
                NAME97 = string.Empty;
                VALUE97 = string.Empty;
                POS_ID = string.Empty;
                ERR_CDS1 = string.Empty;
                ERR_CDS2 = string.Empty;
                ERR_CDS3 = string.Empty;
                ERR_CDS4 = string.Empty;
                ERR_CDS5 = string.Empty;
                ERR_CDS6 = string.Empty;
                ERR_CDS7 = string.Empty;
                ERR_CDS8 = string.Empty;
                CELL_NUM = string.Empty;
                BUILD_NUM = string.Empty;
                WORK_ORDER = string.Empty;
                WORK_ORDER_VER = string.Empty;
                TRAY_ID = string.Empty;
                TEST_TYPE = string.Empty;
                START_DATE = string.Empty;
                TIME = string.Empty;
                SWAP = string.Empty;
                SOFT_REV = string.Empty;
                READER_INDEX = string.Empty;
                ERR_CD = string.Empty;
                ERR_CDH = string.Empty;
                WRITER_CHOICE_FLAG = string.Empty;
                RD_IBS_PATTERN = string.Empty;
                WR_IBS_PATTERN = string.Empty;
            }
        }


        public class FISTDFDATA
        {
                string _spec_no = string.Empty;
                string _spec_ver = string.Empty;
                string _param_id = string.Empty;
                string _script_name = string.Empty;
                string _script_date = string.Empty;
                string _tsr_num = string.Empty;
                string _tsr_group = string.Empty;
                string _slider_lot_id = string.Empty;
                double _radius = 0;
                double _ref_radius = 0;
                double _rpm = 0;
                double _ref_rpm = 0;
                double _skew_angle = 0;
                string _load_radius = string.Empty;
                string _isi_res_at_et = string.Empty;
                string _isi_amp_at_et = string.Empty;
                string _isi_asym_at_et = string.Empty;
                string _waf_tad_res = string.Empty;
                string _waf_rdr_htr_res = string.Empty;
                string _waf_wtr_res = string.Empty;
                string _waf_wtr_htr_res = string.Empty;


            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string SPEC_NO { get { return "\"" + _spec_no.Trim() + "\""; } set { _spec_no = value; } }     

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string SPEC_VER { get { return "\"" + _spec_ver.Trim() + "\""; } set { _spec_ver = value; } }   

            /// <summary>
            /// TSR Saleforce 
            /// </summary>
                public string PARM_ID { get { return "\"" + _param_id.Trim() + "\""; } set { _param_id = value; } }   

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string SCRIPT_NAME { get { return "\"" + _script_name.Trim() + "\""; } set { _script_name = value; } }   

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string SCRIPT_DATE { get { return "\"" + _script_date.Trim() + "\""; } set { _script_date = value; } }   

            /// <summary>
            /// TSR Saleforce 
            /// TSR group 
            /// </summary>
                public string TSR_NUM { get { return "\"" + _tsr_num.Trim() + "\""; } set { _tsr_num = value; } }    

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string TSR_GROUP { get { return "\"" + _tsr_group.Trim() + "\""; } set { _tsr_group = value; } }  

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string SLIDER_LOT_ID { get { return "\"" +_slider_lot_id.Trim() + "\""; } set { _slider_lot_id = value; } }  

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public double RADIUS { get { return _radius; } set { _radius = value; } } 

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public double REF_RADIUS { get { return _ref_radius; } set { _ref_radius = value; } }    

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public double RPM { get { return _rpm; } set { _rpm = value; } }   

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public double REF_RPM { get { return _ref_rpm; } set { _ref_rpm = value; } }   

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public double SKEW_ANGLE { get { return _skew_angle; } set { _skew_angle = value; } } 

            /// <summary>
            /// ET SDET output via FIS
            /// </summary>
                public string LOAD_RADIUS { get { return "\"" +  _load_radius.Trim() + "\""; } set { _load_radius = value; } }   

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
            public string ISI_RES_AT_ET { get { return "\"" + _isi_res_at_et + "\""; } set { _isi_res_at_et = value; } } 

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
            public string ISI_AMP_AT_ET { get { return "\"" + _isi_amp_at_et + "\""; } set { _isi_amp_at_et = value; } } 

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
            public string ISI_ASYM_AT_ET { get { return "\"" + _isi_asym_at_et + "\""; } set { _isi_asym_at_et = value; } } 

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
            public string WAF_TAD_RES { get { return "\"" + _waf_tad_res + "\""; } set { _waf_tad_res = value; } }  

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
                public string WAF_RDR_HTR_RES { get { return "\"" + _waf_rdr_htr_res + "\""; } set { _waf_rdr_htr_res = value; } }   

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
                public string WAF_WTR_RES { get { return "\"" + _waf_wtr_res + "\""; } set { _waf_wtr_res = value; } }   

            /// <summary>
            /// FFW output via FIS
            /// Resistance from ISI via Feed forward
            /// </summary>
                public string WAF_WTR_HTR_RES { get { return "\"" +  _waf_wtr_htr_res + "\""; } set { _waf_wtr_htr_res = value; } }   

            public void Default()
            {
                SPEC_NO = "P-QJC";
                SPEC_VER = "R";
                PARM_ID = "RSH24JCP";
                SCRIPT_NAME = "RSH24JCP";
                SCRIPT_DATE = GetCurrentDateTimeByFormat(DateTime.Now);
                TSR_NUM = "12002";
                TSR_GROUP = "SDET_HGA";
                SLIDER_LOT_ID = "RJCNZ6N1";
                RADIUS = 0.825;
                REF_RADIUS = 0.825;
                RPM = 4325;
                REF_RPM = 4325;
                SKEW_ANGLE = -14.99;
                LOAD_RADIUS = "0.825";
                ISI_RES_AT_ET = "442.272";
                ISI_AMP_AT_ET = "18859.59";
                ISI_ASYM_AT_ET = "-6.818";
                WAF_TAD_RES = "26.6425";
                WAF_RDR_HTR_RES = "50.4123";
                WAF_WTR_RES = "2.882";
                WAF_WTR_HTR_RES = "42.386";
            }

            public void Clear()
            {
                SPEC_NO = string.Empty;
                SPEC_VER = string.Empty;
                PARM_ID = string.Empty;
                SCRIPT_NAME = string.Empty;
                SCRIPT_DATE = GetCurrentDateTimeByFormat(DateTime.Now);
                TSR_NUM = string.Empty;
                TSR_GROUP = string.Empty;
                SLIDER_LOT_ID = string.Empty;
                RADIUS = 0;
                REF_RADIUS = 0;
                RPM = 0;
                REF_RPM = 0;
                SKEW_ANGLE = 0;
                LOAD_RADIUS = string.Empty;
                ISI_RES_AT_ET = string.Empty;
                ISI_AMP_AT_ET = string.Empty;
                ISI_ASYM_AT_ET = string.Empty;
                WAF_TAD_RES = string.Empty;
                WAF_RDR_HTR_RES = string.Empty;
                WAF_WTR_RES = string.Empty;
                WAF_WTR_HTR_RES = string.Empty;
            }

            public string GetCurrentDateTimeByFormat(DateTime datetime)
            {
                return datetime.ToString("dd-MMM-yy:HH:mm:ss").ToUpper();
            }
        }
    }
}
