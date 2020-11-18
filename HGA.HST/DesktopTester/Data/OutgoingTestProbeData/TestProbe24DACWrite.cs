using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe24DACWrite
    {
        public byte RegisterAddress;
        
        public byte DataLSB;
        public byte DataMSB;              
    }
}
