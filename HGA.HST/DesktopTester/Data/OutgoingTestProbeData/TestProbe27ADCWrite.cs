using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe27ADCWrite
    {
        public byte ADCNumber;
        public byte RegisterAddress;

        // Data
        public byte DataLSB;
        public byte DataMid;
        public byte DataMSB;
    }
}
