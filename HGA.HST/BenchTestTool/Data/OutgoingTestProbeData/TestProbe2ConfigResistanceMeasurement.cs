using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe2ConfigResistanceMeasurement
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

        // Ch6        
        public byte Ch6BiasCurrentLSB;
        public byte Ch6BiasCurrentMSB;

        public byte SampleCountForAverage;        
    }
}
