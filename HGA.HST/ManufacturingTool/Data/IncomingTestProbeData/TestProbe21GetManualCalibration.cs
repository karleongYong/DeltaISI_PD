using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe21GetManualCalibration
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        public byte MeasuredValueLSBLow;
        public byte MeasuredValueLSBMid;
        public byte MeasuredValueMSBLow;
        public byte MeasuredValueMSBHigh;        

        public static TestProbe21GetManualCalibration ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe21GetManualCalibration*)pb;
            }
        }

        public int MeasuredValue()
        {
            byte[] array = new byte[4];
            array[0] = MeasuredValueLSBLow;
            array[1] = MeasuredValueLSBMid;
            array[2] = MeasuredValueMSBLow;
            array[3] = MeasuredValueMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }        
    }
}
