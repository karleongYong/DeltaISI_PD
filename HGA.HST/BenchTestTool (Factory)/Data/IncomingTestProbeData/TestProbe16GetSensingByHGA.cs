using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe16GetSensingByHGA
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        public byte WriterSensingLSBLow;
        public byte WriterSensingLSBMid;
        public byte WriterSensingMSBLow;
        public byte WriterSensingMSBHigh;

        public byte TASensingLSBLow;
        public byte TASensingLSBMid;
        public byte TASensingMSBLow;
        public byte TASensingMSBHigh;

        public byte WHSensingLSBLow;
        public byte WHSensingLSBMid;
        public byte WHSensingMSBLow;
        public byte WHSensingMSBHigh;

        public byte RHSensingLSBLow;
        public byte RHSensingLSBMid;
        public byte RHSensingMSBLow;
        public byte RHSensingMSBHigh;

        public byte R1SensingLSBLow;
        public byte R1SensingLSBMid;
        public byte R1SensingMSBLow;
        public byte R1SensingMSBHigh;

        public byte R2SensingLSBLow;
        public byte R2SensingLSBMid;
        public byte R2SensingMSBLow;
        public byte R2SensingMSBHigh;        
        
        public static TestProbe16GetSensingByHGA ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe16GetSensingByHGA*)pb;
            }
        }

        public int WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = WriterSensingLSBLow;
            array[1] = WriterSensingLSBMid;
            array[2] = WriterSensingMSBLow;
            array[3] = WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int TASensing()
        {
            byte[] array = new byte[4];
            array[0] = TASensingLSBLow;
            array[1] = TASensingLSBMid;
            array[2] = TASensingMSBLow;
            array[3] = TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = WHSensingLSBLow;
            array[1] = WHSensingLSBMid;
            array[2] = WHSensingMSBLow;
            array[3] = WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = RHSensingLSBLow;
            array[1] = RHSensingLSBMid;
            array[2] = RHSensingMSBLow;
            array[3] = RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = R1SensingLSBLow;
            array[1] = R1SensingLSBMid;
            array[2] = R1SensingMSBLow;
            array[3] = R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = R2SensingLSBLow;
            array[1] = R2SensingLSBMid;
            array[2] = R2SensingMSBLow;
            array[3] = R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }        
    }
}
