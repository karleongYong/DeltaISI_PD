using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe28GetADCRead
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        // Data
        public byte DataLSB;
        public byte DataMid;
        public byte DataMSB;

        public static TestProbe28GetADCRead ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe28GetADCRead*)pb;
            }
        }

        // Data
        public int Data()
        {
            byte[] array = new byte[4];
            array[0] = DataLSB;
            array[1] = DataMid;
            array[2] = DataMSB;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }          
    }
}
