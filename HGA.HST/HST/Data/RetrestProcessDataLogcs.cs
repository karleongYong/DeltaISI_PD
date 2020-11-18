using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.EFI.Log;
using System.IO;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    public class RetrestProcessDataLogcs
    {
        public class RetestProcessFilelogger : EfiLogger
        {
            public const string PreTDFName = "ReTestProcessLog";
            public const string PosTDFName = ".CSV";

            public enum Category
            {
                DATE_TIME,
                CARRIER_ID,
                TEST_INDEX,
                TEST_SLOT,
                HGA_STATUS,
                RD1_RES,
                RD2_RES,
                RD1_ISI_RES,
                RD2_ISI_RES,
                WR_RES,
                RH_RES,
                WH_RES,
                TA_RES,
                SHORT_TEST,
                SHORT_PAD,
                BIAS_VOL,
                BIAS_CURR,
                ERR_PER_PERIOD,
                ERR_CUM,
                ERR_CODE,
                BUYOFF_RESULT
            }

            private DirectoryInfo _dirInfo;
            private string _logPath = string.Empty;
            private string _logName = string.Empty;
            public RetestProcessFilelogger(string loggerPath, string fileName)
                : base(loggerPath, fileName)
            {
                _dirInfo = new DirectoryInfo(loggerPath);
                _logName = fileName;
                _logPath = loggerPath;

                SaveHeaderInfo();
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
                    header.Append("\"" + category.ToString() + "\"");
                    loopIndex++;
                }

                string headerFileName = _logPath + "\\" + _logName;

                if (!File.Exists(headerFileName))
                {
                    header.Append("\r\n");
                    // Create a file called test.txt in the current directory:d
                    using (Stream fs = new FileStream(headerFileName, FileMode.Create))
                    {
                        using (TextWriter writer = new StreamWriter(fs))
                        {
                            writer.Write(header.ToString());
                        }
                    }
                }
            }
        }
    }
}
