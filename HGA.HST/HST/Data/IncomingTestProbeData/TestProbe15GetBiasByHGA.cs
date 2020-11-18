using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe15GetBiasByHGA
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        public byte WriterBiasLSBLow;
        public byte WriterBiasLSBMid;
        public byte WriterBiasMSBLow;
        public byte WriterBiasMSBHigh;

        public byte TABiasLSBLow;
        public byte TABiasLSBMid;
        public byte TABiasMSBLow;
        public byte TABiasMSBHigh;

        public byte WHBiasLSBLow;
        public byte WHBiasLSBMid;
        public byte WHBiasMSBLow;
        public byte WHBiasMSBHigh;

        public byte RHBiasLSBLow;
        public byte RHBiasLSBMid;
        public byte RHBiasMSBLow;
        public byte RHBiasMSBHigh;

        public byte R1BiasLSBLow;
        public byte R1BiasLSBMid;
        public byte R1BiasMSBLow;
        public byte R1BiasMSBHigh;

        public byte R2BiasLSBLow;
        public byte R2BiasLSBMid;
        public byte R2BiasMSBLow;
        public byte R2BiasMSBHigh;        

        
        public static TestProbe15GetBiasByHGA ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe15GetBiasByHGA*)pb;
            }
        }

        public int WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = WriterBiasLSBLow;
            array[1] = WriterBiasLSBMid;
            array[2] = WriterBiasMSBLow;
            array[3] = WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int TABias()
        {
            byte[] array = new byte[4];
            array[0] = TABiasLSBLow;
            array[1] = TABiasLSBMid;
            array[2] = TABiasMSBLow;
            array[3] = TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int WHBias()
        {
            byte[] array = new byte[4];
            array[0] = WHBiasLSBLow;
            array[1] = WHBiasLSBMid;
            array[2] = WHBiasMSBLow;
            array[3] = WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int RHBias()
        {
            byte[] array = new byte[4];
            array[0] = RHBiasLSBLow;
            array[1] = RHBiasLSBMid;
            array[2] = RHBiasMSBLow;
            array[3] = RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = R1BiasLSBLow;
            array[1] = R1BiasLSBMid;
            array[2] = R1BiasMSBLow;
            array[3] = R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = R2BiasLSBLow;
            array[1] = R2BiasLSBMid;
            array[2] = R2BiasMSBLow;
            array[3] = R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }        
    }
}
