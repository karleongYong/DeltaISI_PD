using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe21SetManualCalibration
    {                            
        public byte CalibrationType;
        public byte HGAIndex;
        public byte ChannelNumber;
        
        // External Value     
        public byte ExternalValueLSBLow;
        public byte ExternalValueLSBMid;
        public byte ExternalValueMSBLow;
        public byte ExternalValueMSBHigh;
    }
}
