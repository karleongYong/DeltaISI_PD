using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe25GetDACRead
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        public byte DataLSB;
        public byte DataMSB;


        public static TestProbe25GetDACRead ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe25GetDACRead*)pb;
            }
        }

        // Data
        public int Data()
        {
            byte[] array = new byte[4];
            array[0] = DataLSB;
            array[1] = DataMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }           
    }
}
