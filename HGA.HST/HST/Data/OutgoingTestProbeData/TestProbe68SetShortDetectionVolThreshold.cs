using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe68SetShortDetectionVolThreshold
    {
        public byte VolThresholdLSBLow1;
        public byte VolThresholdLSBMid1;

        public byte VolThresholdLSBLow2;
        public byte VolThresholdLSBMid2;

        public byte HiLowLimit;
        //  public byte VolThresholdMSBLow;
        // public byte VolThresholdMSBHigh;
    }
}
