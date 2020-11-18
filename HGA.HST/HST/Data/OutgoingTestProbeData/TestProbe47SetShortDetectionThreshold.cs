using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe47SetShortDetectionThreshold
    {        
        // Threshold Voltage
        public byte ThresholdVoltageLSBLow;
        public byte ThresholdVoltageLSBMid;
        public byte ThresholdVoltageMSBLow;
        public byte ThresholdVoltageMSBHigh;
    }
}
