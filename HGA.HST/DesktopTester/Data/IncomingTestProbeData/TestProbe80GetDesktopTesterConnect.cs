using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe80GetDesktopTesterConnect
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // Password
        public byte PasswordLSB;
        public byte PasswordMSB;


        public static TestProbe80GetDesktopTesterConnect ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe80GetDesktopTesterConnect*)pb;
            }
        }

        // Password
        public ushort Password()
        {
            byte[] array = new byte[4];
            array[0] = PasswordLSB;
            array[1] = PasswordMSB;            
            return BitConverter.ToUInt16(array, 0);
        }
    }
}
