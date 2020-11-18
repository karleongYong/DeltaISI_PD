using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe85SetLDUConfiguration_2
    {
        public byte enable_ldu;
        //LED
        public byte LEDStartCurrentLSB;
        public byte LEDStartCurrentMSB;

        public byte LEDStopCurrentLSB;
        public byte LEDStopCurrentMSB;

        public byte LEDSteppingSizeLSB;
        public byte LEDSteppingSizeMSB;

        public byte LEDVoltageSampleSize;
        public byte LEDNumberOfSteps;
        //9
        //LDU

        public byte LDU4PtMode;

        public byte LDUStartCurrentLSB;
        public byte LDUStartCurrentMSB;

        public byte LDUStopCurrentLSB;
        public byte LDUStopCurrentMSB;

        public byte LDUSteppingSizeLSB;
        public byte LDUSteppingSizeMSB;

        public byte LDUVoltageSampleSize;
        public byte LDUNumberOfSteps;

        public byte LDUISweep1LSB;
        public byte LDUISweep1MSB;

        public byte LDUISweep2LSB;
        public byte LDUISweep2MSB;

        public byte LDUISweep3LSB;
        public byte LDUISweep3MSB;

        public byte LDUISweep4LSB;
        public byte LDUISweep4MSB;

        public byte timeIntervalLSB;
        public byte timeIntervalMSB;
        //19
    }
}

