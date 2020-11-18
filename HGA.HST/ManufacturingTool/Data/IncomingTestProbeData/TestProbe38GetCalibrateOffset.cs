using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe38GetCalibrateOffset
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // WriterSensingOffset
        public byte WriterSensingOffsetLSBLow;
        public byte WriterSensingOffsetLSBMid;
        public byte WriterSensingOffsetMSBLow;
        public byte WriterSensingOffsetMSBHigh;

        // TASensingOffset
        public byte TASensingOffsetLSBLow;
        public byte TASensingOffsetLSBMid;
        public byte TASensingOffsetMSBLow;
        public byte TASensingOffsetMSBHigh;

        // WHSensingOffset
        public byte WHSensingOffsetLSBLow;
        public byte WHSensingOffsetLSBMid;
        public byte WHSensingOffsetMSBLow;
        public byte WHSensingOffsetMSBHigh;

        // RHSensingOffset
        public byte RHSensingOffsetLSBLow;
        public byte RHSensingOffsetLSBMid;
        public byte RHSensingOffsetMSBLow;
        public byte RHSensingOffsetMSBHigh;

        // R1SensingOffset
        public byte R1SensingOffsetLSBLow;
        public byte R1SensingOffsetLSBMid;
        public byte R1SensingOffsetMSBLow;
        public byte R1SensingOffsetMSBHigh;

        // R2SensingOffset
        public byte R2SensingOffsetLSBLow;
        public byte R2SensingOffsetLSBMid;
        public byte R2SensingOffsetMSBLow;
        public byte R2SensingOffsetMSBHigh;

        // WriterDUTOffset
        public byte WriterDUTOffsetLSBLow;
        public byte WriterDUTOffsetLSBMid;
        public byte WriterDUTOffsetMSBLow;
        public byte WriterDUTOffsetMSBHigh;

        // TADUTOffset
        public byte TADUTOffsetLSBLow;
        public byte TADUTOffsetLSBMid;
        public byte TADUTOffsetMSBLow;
        public byte TADUTOffsetMSBHigh;

        // WHDUTOffset
        public byte WHDUTOffsetLSBLow;
        public byte WHDUTOffsetLSBMid;
        public byte WHDUTOffsetMSBLow;
        public byte WHDUTOffsetMSBHigh;

        // RHDUTOffset
        public byte RHDUTOffsetLSBLow;
        public byte RHDUTOffsetLSBMid;
        public byte RHDUTOffsetMSBLow;
        public byte RHDUTOffsetMSBHigh;

        // R1DUTOffset
        public byte R1DUTOffsetLSBLow;
        public byte R1DUTOffsetLSBMid;
        public byte R1DUTOffsetMSBLow;
        public byte R1DUTOffsetMSBHigh;

        // R2DUTOffset
        public byte R2DUTOffsetLSBLow;
        public byte R2DUTOffsetLSBMid;
        public byte R2DUTOffsetMSBLow;
        public byte R2DUTOffsetMSBHigh;

        // INARefVoltage
        public byte INARefVoltageLSBLow;
        public byte INARefVoltageLSBMid;
        public byte INARefVoltageMSBLow;
        public byte INARefVoltageMSBHigh;

        public static TestProbe38GetCalibrateOffset ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe38GetCalibrateOffset*)pb;
            }
        }

        // WriterSensingOffset
        public int WriterSensingOffset()
        {
            byte[] array = new byte[4];
            array[0] = WriterSensingOffsetLSBLow;
            array[1] = WriterSensingOffsetLSBMid;
            array[2] = WriterSensingOffsetMSBLow;
            array[3] = WriterSensingOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // TASensingOffset
        public int TASensingOffset()
        {
            byte[] array = new byte[4];
            array[0] = TASensingOffsetLSBLow;
            array[1] = TASensingOffsetLSBMid;
            array[2] = TASensingOffsetMSBLow;
            array[3] = TASensingOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // WHSensingOffset
        public int WHSensingOffset()
        {
            byte[] array = new byte[4];
            array[0] = WHSensingOffsetLSBLow;
            array[1] = WHSensingOffsetLSBMid;
            array[2] = WHSensingOffsetMSBLow;
            array[3] = WHSensingOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // RHSensingOffset
        public int RHSensingOffset()
        {
            byte[] array = new byte[4];
            array[0] = RHSensingOffsetLSBLow;
            array[1] = RHSensingOffsetLSBMid;
            array[2] = RHSensingOffsetMSBLow;
            array[3] = RHSensingOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // R1SensingOffset
        public int R1SensingOffset()
        {
            byte[] array = new byte[4];
            array[0] = R1SensingOffsetLSBLow;
            array[1] = R1SensingOffsetLSBMid;
            array[2] = R1SensingOffsetMSBLow;
            array[3] = R1SensingOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // R2SensingOffset
        public int R2SensingOffset()
        {
            byte[] array = new byte[4];
            array[0] = R2SensingOffsetLSBLow;
            array[1] = R2SensingOffsetLSBMid;
            array[2] = R2SensingOffsetMSBLow;
            array[3] = R2SensingOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // WriterDUTOffset
        public int WriterDUTOffset()
        {
            byte[] array = new byte[4];
            array[0] = WriterDUTOffsetLSBLow;
            array[1] = WriterDUTOffsetLSBMid;
            array[2] = WriterDUTOffsetMSBLow;
            array[3] = WriterDUTOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // TADUTOffset
        public int TADUTOffset()
        {
            byte[] array = new byte[4];
            array[0] = TADUTOffsetLSBLow;
            array[1] = TADUTOffsetLSBMid;
            array[2] = TADUTOffsetMSBLow;
            array[3] = TADUTOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // WHDUTOffset
        public int WHDUTOffset()
        {
            byte[] array = new byte[4];
            array[0] = WHDUTOffsetLSBLow;
            array[1] = WHDUTOffsetLSBMid;
            array[2] = WHDUTOffsetMSBLow;
            array[3] = WHDUTOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // RHDUTOffset
        public int RHDUTOffset()
        {
            byte[] array = new byte[4];
            array[0] = RHDUTOffsetLSBLow;
            array[1] = RHDUTOffsetLSBMid;
            array[2] = RHDUTOffsetMSBLow;
            array[3] = RHDUTOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // R1DUTOffset
        public int R1DUTOffset()
        {
            byte[] array = new byte[4];
            array[0] = R1DUTOffsetLSBLow;
            array[1] = R1DUTOffsetLSBMid;
            array[2] = R1DUTOffsetMSBLow;
            array[3] = R1DUTOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // R2DUTOffset
        public int R2DUTOffset()
        {
            byte[] array = new byte[4];
            array[0] = R2DUTOffsetLSBLow;
            array[1] = R2DUTOffsetLSBMid;
            array[2] = R2DUTOffsetMSBLow;
            array[3] = R2DUTOffsetMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // INARefVoltage
        public int INARefVoltage()
        {
            byte[] array = new byte[4];
            array[0] = INARefVoltageLSBLow;
            array[1] = INARefVoltageLSBMid;
            array[2] = INARefVoltageMSBLow;
            array[3] = INARefVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
