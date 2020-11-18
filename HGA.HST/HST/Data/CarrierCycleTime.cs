using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Settings;
using XyratexOSC.UI;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Data
{
    public class CarrierCycleTime
    {   
        public string CarrierID
        {
            get;
            set;
        }

        public int TestedHGACount
        {
            get;
            set;
        }

        public int UntestedHGACount
        {
            get;
            set;
        }

        public int MissingOrUnknownHGACount
        {
            get;
            set;
        }

        public TimeSpan OverallProcessCycleTimeSpan
        {
            get;
            set;
        }

        public DateTime TimeStampAtOutputTurnTable
        {
            get;
            set;
        }       

        public CarrierCycleTime(string carrierID, int testedHGACount, int untestedHGACount, int missingOrUnknownHGACount)
        {
            CarrierID = carrierID;
            TestedHGACount = testedHGACount;
            UntestedHGACount = untestedHGACount;
            MissingOrUnknownHGACount = missingOrUnknownHGACount; 
        }

        public void LogHGACount()
        {
            string HGACountFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath + "\\HGACount.csv");
            try
            {
                using (StreamWriter HGACountFile = new StreamWriter(HGACountFilePath, true))
                {
                    string HGACountRecord = String.Join(",",
                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                       "Carrier ID: " + CarrierID,
                       "Cycle Time (seconds): " + ((OverallProcessCycleTimeSpan.Hours * 3600) + (OverallProcessCycleTimeSpan.Minutes * 60) + OverallProcessCycleTimeSpan.Seconds + (OverallProcessCycleTimeSpan.Milliseconds / 1000.0)),
                       "Tested: " + TestedHGACount,
                       "Untested: " + UntestedHGACount,
                       "MissinbgOrUnknown: " + MissingOrUnknownHGACount
                       );

                    HGACountFile.WriteLine(HGACountRecord);
                }
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Error in logging HGA counts into '" + HGACountFilePath + "'", ex);
            }
        }
    }
}
