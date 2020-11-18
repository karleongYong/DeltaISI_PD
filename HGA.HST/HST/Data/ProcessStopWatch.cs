using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    public class ProcessStopWatch
    {
        private String StopWatchStartTime;
        private String StopWatchStopTime;

        public String CarrierID
        {
            get;
            set;
        }

        public Stopwatch StopWatchObject
        {
            get;
            set;
        }        

        public ProcessStopWatch(String carrierID, Stopwatch stopWatchObject)
        {
            CarrierID = carrierID;
            StopWatchObject = stopWatchObject;
            StopWatchObject.Start();
            StopWatchStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public void Stop()
        {
            StopWatchObject.Stop();
            StopWatchStopTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public TimeSpan GetElapsedTime()
        {
            return StopWatchObject.ElapsedTime;
        }

        public double GetElapsedTimeInSeconds()
        {
            return StopWatchObject.ElapsedTime_sec;
        }

        public String GetStartTime()
        {
            return StopWatchStartTime;
        }

        public String GetStopTime()
        {
            return StopWatchStopTime;
        }
    }
}
