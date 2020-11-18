using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe85GetDesktopTesterServoPositions
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // X Position
        public byte XPositionLSBLow;
        public byte XPositionLSBMid;
        public byte XPositionMSBLow;
        public byte XPositionMSBHigh;

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

        // Z2 Position
        public byte Z2PositionLSBLow;
        public byte Z2PositionLSBMid;
        public byte Z2PositionMSBLow;
        public byte Z2PositionMSBHigh;


        public static TestProbe85GetDesktopTesterServoPositions ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe85GetDesktopTesterServoPositions*)pb;
            }
        }

        // X Position
        public int XPosition()
        {
            byte[] array = new byte[4];
            array[0] = XPositionLSBLow;
            array[1] = XPositionLSBMid;
            array[2] = XPositionMSBLow;
            array[3] = XPositionMSBHigh;
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

        // Z2 Position
        public int Z2Position()
        {
            byte[] array = new byte[4];
            array[0] = Z2PositionLSBLow;
            array[1] = Z2PositionLSBMid;
            array[2] = Z2PositionMSBLow;
            array[3] = Z2PositionMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
