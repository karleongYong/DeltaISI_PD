using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe7GetProductID
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte ProductID;


        public static TestProbe7GetProductID ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe7GetProductID*)pb;
            }
        } 
    }
}
