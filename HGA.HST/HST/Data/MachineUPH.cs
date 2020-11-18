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
    public class MachineUPH
    {
        public string CarrierID
        {
            get;
            set;
        }

        public TimeSpan TestStationCycleTimeSpanUPH
        {
            get;
            set;
        }

        public TimeSpan TestStationCycleTimeSpanUPH2
        {
            get;
            set;
        }

        // to determine the throughput UPH 
        public DateTime PrecisorNestAtInputPositionTimeStamp
        {
            get;
            set;
        }

        // to determine the Machine UPH
        public DateTime PrecisorNestAtStandbyPositionTimeStamp
        {
            get;
            set;
        }

        // to determine the Machine UPH
        public DateTime PreviousPrecisorNestAtStandbyPositionTimeStamp
        {
            get;
            set;
        }

        public int TestedHGACount
        {
            get;
            set;
        }
        
        // Throughput UPH
        public int UPH
        {
            get;
            set;
        }

        // Machine UPH
        public int UPH2
        {
            get;
            set;
        }

        public void LogUPH()
        {
            string MachineUPHFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath + "\\MachineUPH.csv");
            try
            {
                using (StreamWriter HGACountFile = new StreamWriter(MachineUPHFilePath, true))
                {
                    string HGACountRecord = String.Join(",",
                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                       "Carrier ID: " + CarrierID,
                       "Tested HGA Count: " + TestedHGACount,
                       "Precisor Nest at Input Position Time Stamp: " + PrecisorNestAtInputPositionTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                       "Precisor Nest at Standby Position Time Stamp: " + PrecisorNestAtStandbyPositionTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                       "Previous Precisor Nest at Standby Position Time Stamp: " + PreviousPrecisorNestAtStandbyPositionTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                       "Cycle Time (seconds) for throughput UPH: " + ((TestStationCycleTimeSpanUPH.Hours * 3600) + (TestStationCycleTimeSpanUPH.Minutes * 60) + TestStationCycleTimeSpanUPH.Seconds + (TestStationCycleTimeSpanUPH.Milliseconds / 1000.0)),
                       "Cycle Time (seconds) for Machine UPH: " + ((TestStationCycleTimeSpanUPH2.Hours * 3600) + (TestStationCycleTimeSpanUPH2.Minutes * 60) + TestStationCycleTimeSpanUPH2.Seconds + (TestStationCycleTimeSpanUPH2.Milliseconds / 1000.0)),
                       "Throughput UPH: " + UPH,
                       "Machine UPH: " + UPH2
                       );

                    HGACountFile.WriteLine(HGACountRecord);
                }
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Error in logging machine UPH into '" + MachineUPHFilePath + "'", ex);
            }
        }
    }
}
