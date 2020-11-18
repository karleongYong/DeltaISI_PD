using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe3ConfigCapacitanceMeasurement
    {
        // Frequency        
        public byte FrequencyLSB;
        public byte FrequencyMSB;

        // Bias Voltage    
        public byte BiasVoltageLSB;
        public byte BiasVoltageMSB;

        // Peak Voltage       
        public byte PeakVoltageLSB;
        public byte PeakVoltageMSB;

        public byte MeasurementMode;
        public byte SampleCountForAverage;
    }
}
