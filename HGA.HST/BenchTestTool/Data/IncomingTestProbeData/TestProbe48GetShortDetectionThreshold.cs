using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe48GetShortDetectionThreshold
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // ThresholdVoltage
        public byte ThresholdVoltageLSBLow;
        public byte ThresholdVoltageLSBMid;
        public byte ThresholdVoltageMSBLow;
        public byte ThresholdVoltageMSBHigh;


        public static TestProbe48GetShortDetectionThreshold ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe48GetShortDetectionThreshold*)pb;
            }
        }

        // ThresholdVoltage
        public int ThresholdVoltage()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLSBLow;
            array[1] = ThresholdVoltageLSBMid;
            array[2] = ThresholdVoltageMSBLow;
            array[3] = ThresholdVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }        
    }
}
