using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe33GetTemperature
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // Ch1 Temperature
        public byte Ch1TemperatureLSB;
        public byte Ch1TemperatureMSB;

        // Ch2 Temperature
        public byte Ch2TemperatureLSB;
        public byte Ch2TemperatureMSB;

        // Ch3 Temperature
        public byte Ch3TemperatureLSB;
        public byte Ch3TemperatureMSB; 

        public static TestProbe33GetTemperature ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe33GetTemperature*)pb;
            }
        }

        // Ch1 Temperature
        public int Ch1Temperature()
        {
            byte[] array = new byte[2];
            array[0] = Ch1TemperatureLSB;
            array[1] = Ch1TemperatureMSB;                
            return BitConverter.ToInt16(array, 0);
        }

        // Ch2 Temperature
        public int Ch2Temperature()
        {
            byte[] array = new byte[2];
            array[0] = Ch2TemperatureLSB;
            array[1] = Ch2TemperatureMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // Ch3 Temperature
        public int Ch3Temperature()
        {
            byte[] array = new byte[2];
            array[0] = Ch3TemperatureLSB;
            array[1] = Ch3TemperatureMSB;
            return BitConverter.ToInt16(array, 0);
        }      
    }
}
