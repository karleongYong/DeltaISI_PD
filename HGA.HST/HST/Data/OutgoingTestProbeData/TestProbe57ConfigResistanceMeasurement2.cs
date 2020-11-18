using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe57ConfigResistanceMeasurement2
    {
        // Ch1        
        public byte Ch1BiasCurrentLSB;
        public byte Ch1BiasCurrentMSB;

        // Ch2        
        public byte Ch2BiasCurrentLSB;
        public byte Ch2BiasCurrentMSB;

        // Ch3        
        public byte Ch3BiasCurrentLSB;
        public byte Ch3BiasCurrentMSB;

        // Ch4        
        public byte Ch4BiasCurrentLSB;
        public byte Ch4BiasCurrentMSB;

        // Ch5        
        public byte Ch5BiasCurrentLSB;
        public byte Ch5BiasCurrentMSB;

        // Ch6  Ia
        public byte Ch6IaBiasCurrentLSB;
        public byte Ch6IaBiasCurrentMSB;

        // Ch6  Ib
        public byte Ch6IbBiasCurrentLSB;
        public byte Ch6IbBiasCurrentMSB;

        public byte SampleCountForAverage;        
    }
}
