using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe31GetTemperatureCalibration
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        // Voltage
        public byte VoltageLSBLow;
        public byte VoltageLSBMid;
        public byte VoltageMSBLow;
        public byte VoltageMSBHigh;        

        public static TestProbe31GetTemperatureCalibration ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe31GetTemperatureCalibration*)pb;
            }
        }

        // Voltage
        public int Voltage()
        {
            byte[] array = new byte[4];
            array[0] = VoltageLSBLow;
            array[1] = VoltageLSBMid;
            array[2] = VoltageMSBLow;
            array[3] = VoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }        
    }
}
