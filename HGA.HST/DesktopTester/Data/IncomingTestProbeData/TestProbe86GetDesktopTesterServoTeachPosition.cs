using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe86GetDesktopTesterServoTeachPosition
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // XOrZ2 Position
        public byte XOrZ2PositionLSBLow;
        public byte XOrZ2PositionLSBMid;
        public byte XOrZ2PositionMSBLow;
        public byte XOrZ2PositionMSBHigh;

        // Y Position
        public byte YPositionLSBLow;
        public byte YPositionLSBMid;
        public byte YPositionMSBLow;
        public byte YPositionMSBHigh;

        // Theta Position
        public byte ThetaPositionLSBLow;
        public byte ThetaPositionLSBMid;
        public byte ThetaPositionMSBLow;
        public byte ThetaPositionMSBHigh;


        public static TestProbe86GetDesktopTesterServoTeachPosition ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe86GetDesktopTesterServoTeachPosition*)pb;
            }
        }

        // X OrZ2 Position
        public int XOrZ2Position()
        {
            byte[] array = new byte[4];
            array[0] = XOrZ2PositionLSBLow;
            array[1] = XOrZ2PositionLSBMid;
            array[2] = XOrZ2PositionMSBLow;
            array[3] = XOrZ2PositionMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Y Position
        public int YPosition()
        {
            byte[] array = new byte[4];
            array[0] = YPositionLSBLow;
            array[1] = YPositionLSBMid;
            array[2] = YPositionMSBLow;
            array[3] = YPositionMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Theta Position
        public int ThetaPosition()
        {
            byte[] array = new byte[4];
            array[0] = ThetaPositionLSBLow;
            array[1] = ThetaPositionLSBMid;
            array[2] = ThetaPositionMSBLow;
            array[3] = ThetaPositionMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
