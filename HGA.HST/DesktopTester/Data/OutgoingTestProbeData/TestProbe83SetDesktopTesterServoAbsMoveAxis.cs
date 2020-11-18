using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe83SetDesktopTesterServoAbsMoveAxis
    {                                    
        public byte ServoAxis;

        // X Abs Position
        public byte XAbsPositionLSBLow;
        public byte XAbsPositionLSBMid;
        public byte XAbsPositionMSBLow;
        public byte XAbsPositionMSBHigh;

        // Y Abs Position
        public byte YAbsPositionLSBLow;
        public byte YAbsPositionLSBMid;
        public byte YAbsPositionMSBLow;
        public byte YAbsPositionMSBHigh;

        // Theta Abs Position
        public byte ThetaAbsPositionLSBLow;
        public byte ThetaAbsPositionLSBMid;
        public byte ThetaAbsPositionMSBLow;
        public byte ThetaAbsPositionMSBHigh;

        // Z2 Abs Position
        public byte Z2AbsPositionLSBLow;
        public byte Z2AbsPositionLSBMid;
        public byte Z2AbsPositionMSBLow;
        public byte Z2AbsPositionMSBHigh;
    }
}
