using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe90GetDesktopTesterAllInputs
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte EStopInputStatus;
        public byte FlattenerRetract1SensorInput;
        public byte FlattenerExtend1SensorInput;
        public byte Reserved1;
        public byte Reserved2;
        public byte Reserved3;
        public byte Reserved4;
        public byte Reserved5;

        public static TestProbe90GetDesktopTesterAllInputs ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe90GetDesktopTesterAllInputs*)pb;
            }
        }        
    }
}
