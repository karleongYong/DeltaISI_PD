using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe7GetConversionBoardID
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte ConversionBoardID;


        public static TestProbe7GetConversionBoardID ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe7GetConversionBoardID*)pb;
            }
        } 
    }
}
