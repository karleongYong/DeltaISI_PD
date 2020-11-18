using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe71GetShortDetectionVolThreshold
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        //VolThreshold
         public byte VolThreshold1LowLimitLSBLow;
        public byte VolThreshold1LowLimitLSBMid;

        public byte VolThreshold1HiLimitLSBLow;
        public byte VolThreshold1HiLimitLSBMid;

        public byte VolThreshold2LowLimitLSBLow;
        public byte VolThreshold2LowLimitLSBMid;

        public byte VolThreshold2HiLimitLSBLow;
        public byte VolThreshold2HiLimitLSBMid;
        //  public byte VolThresholdMSBLow;
        //   public byte VolThresholdMSBHigh;


        public static TestProbe71GetShortDetectionVolThreshold ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe71GetShortDetectionVolThreshold*)pb;
            }
        }

        //HGA1
        public int GetVolThreshold1LowLimit()
        {
            byte[] array = new byte[4];
            array[0] = VolThreshold1LowLimitLSBLow;
            array[1] = VolThreshold1LowLimitLSBMid;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolThreshold1HiLimit()
        {
            byte[] array = new byte[4];
            array[0] = VolThreshold1HiLimitLSBLow;
            array[1] = VolThreshold1HiLimitLSBMid;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolThreshold2LowLimit()
        {
            byte[] array = new byte[4];
            array[0] = VolThreshold2LowLimitLSBLow;
            array[1] = VolThreshold2LowLimitLSBMid;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolThreshold2HiLimit()
        {
            byte[] array = new byte[4];
            array[0] = VolThreshold2HiLimitLSBLow;
            array[1] = VolThreshold2HiLimitLSBMid;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }



    }
}