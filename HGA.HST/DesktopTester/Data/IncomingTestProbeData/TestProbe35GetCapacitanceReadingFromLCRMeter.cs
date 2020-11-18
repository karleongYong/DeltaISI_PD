using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe35GetCapacitanceReadingFromLCRMeter
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        // Capacitance
        public byte CapacitanceLSBLow;
        public byte CapacitanceLSBMid;
        public byte CapacitanceMSBLow;
        public byte CapacitanceMSBHigh;

        public static TestProbe35GetCapacitanceReadingFromLCRMeter ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe35GetCapacitanceReadingFromLCRMeter*)pb;
            }
        }

        // Temperature
        public int Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = CapacitanceLSBLow;
            array[1] = CapacitanceLSBMid;
            array[2] = CapacitanceMSBLow;
            array[3] = CapacitanceMSBHigh;    
            return BitConverter.ToInt32(array, 0);
        }        
    }
}
