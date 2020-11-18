using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe84SetDesktopTesterServoRelMoveAxis
    {                                    
        public byte ServoAxis;

        // Rel Position
        public byte RelPositionLSBLow;
        public byte RelPositionLSBMid;
        public byte RelPositionMSBLow;
        public byte RelPositionMSBHigh;        
    }
}
