using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe37GetFirmwareVersion
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
                
        public byte MajorRevision;
        public byte MinorRevision;


        public static TestProbe37GetFirmwareVersion ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe37GetFirmwareVersion*)pb;
            }
        }        
    }
}
