using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe60GetShortDetectionThreshold
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // ThresholdVoltageHighLimit
        public byte ThresholdVoltageHighLimitLSBLowCH1;
        public byte ThresholdVoltageHighLimitLSBMidCH1;
        public byte ThresholdVoltageHighLimitMSBLowCH1;
        public byte ThresholdVoltageHighLimitMSBHighCH1;

        public byte ThresholdVoltageHighLimitLSBLowCH2;
        public byte ThresholdVoltageHighLimitLSBMidCH2;
        public byte ThresholdVoltageHighLimitMSBLowCH2;
        public byte ThresholdVoltageHighLimitMSBHighCH2;

        public byte ThresholdVoltageHighLimitLSBLowCH3;
        public byte ThresholdVoltageHighLimitLSBMidCH3;
        public byte ThresholdVoltageHighLimitMSBLowCH3;
        public byte ThresholdVoltageHighLimitMSBHighCH3;

        public byte ThresholdVoltageHighLimitLSBLowCH4;
        public byte ThresholdVoltageHighLimitLSBMidCH4;
        public byte ThresholdVoltageHighLimitMSBLowCH4;
        public byte ThresholdVoltageHighLimitMSBHighCH4;

        public byte ThresholdVoltageHighLimitLSBLowCH5;
        public byte ThresholdVoltageHighLimitLSBMidCH5;
        public byte ThresholdVoltageHighLimitMSBLowCH5;
        public byte ThresholdVoltageHighLimitMSBHighCH5;

        public byte ThresholdVoltageHighLimitLSBLowCH6;
        public byte ThresholdVoltageHighLimitLSBMidCH6;
        public byte ThresholdVoltageHighLimitMSBLowCH6;
        public byte ThresholdVoltageHighLimitMSBHighCH6;

        public byte ThresholdVoltageLowLimitLSBLowCH1;
        public byte ThresholdVoltageLowLimitLSBMidCH1;
        public byte ThresholdVoltageLowLimitMSBLowCH1;
        public byte ThresholdVoltageLowLimitMSBHighCH1;

        public byte ThresholdVoltageLowLimitLSBLowCH2;
        public byte ThresholdVoltageLowLimitLSBMidCH2;
        public byte ThresholdVoltageLowLimitMSBLowCH2;
        public byte ThresholdVoltageLowLimitMSBHighCH2;

        public byte ThresholdVoltageLowLimitLSBLowCH3;
        public byte ThresholdVoltageLowLimitLSBMidCH3;
        public byte ThresholdVoltageLowLimitMSBLowCH3;
        public byte ThresholdVoltageLowLimitMSBHighCH3;

        public byte ThresholdVoltageLowLimitLSBLowCH4;
        public byte ThresholdVoltageLowLimitLSBMidCH4;
        public byte ThresholdVoltageLowLimitMSBLowCH4;
        public byte ThresholdVoltageLowLimitMSBHighCH4;

        public byte ThresholdVoltageLowLimitLSBLowCH5;
        public byte ThresholdVoltageLowLimitLSBMidCH5;
        public byte ThresholdVoltageLowLimitMSBLowCH5;
        public byte ThresholdVoltageLowLimitMSBHighCH5;

        public byte ThresholdVoltageLowLimitLSBLowCH6;
        public byte ThresholdVoltageLowLimitLSBMidCH6;
        public byte ThresholdVoltageLowLimitMSBLowCH6;
        public byte ThresholdVoltageLowLimitMSBHighCH6;

        // public byte SetLimits;

        public static TestProbe60GetShortDetectionThreshold ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe60GetShortDetectionThreshold*)pb;
            }
        }

        // ThresholdVoltageHighLimitCH1
        public int ThresholdVoltageHighLimitCH1()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageHighLimitLSBLowCH1;
            array[1] = ThresholdVoltageHighLimitLSBMidCH1;
            array[2] = ThresholdVoltageHighLimitMSBLowCH1;
            array[3] = ThresholdVoltageHighLimitMSBHighCH1;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageHighLimitCH2
        public int ThresholdVoltageHighLimitCH2()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageHighLimitLSBLowCH2;
            array[1] = ThresholdVoltageHighLimitLSBMidCH2;
            array[2] = ThresholdVoltageHighLimitMSBLowCH2;
            array[3] = ThresholdVoltageHighLimitMSBHighCH2;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageHighLimitCH3
        public int ThresholdVoltageHighLimitCH3()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageHighLimitLSBLowCH3;
            array[1] = ThresholdVoltageHighLimitLSBMidCH3;
            array[2] = ThresholdVoltageHighLimitMSBLowCH3;
            array[3] = ThresholdVoltageHighLimitMSBHighCH3;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageHighLimitCH4
        public int ThresholdVoltageHighLimitCH4()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageHighLimitLSBLowCH4;
            array[1] = ThresholdVoltageHighLimitLSBMidCH4;
            array[2] = ThresholdVoltageHighLimitMSBLowCH4;
            array[3] = ThresholdVoltageHighLimitMSBHighCH4;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageHighLimitCH5
        public int ThresholdVoltageHighLimitCH5()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageHighLimitLSBLowCH5;
            array[1] = ThresholdVoltageHighLimitLSBMidCH5;
            array[2] = ThresholdVoltageHighLimitMSBLowCH5;
            array[3] = ThresholdVoltageHighLimitMSBHighCH5;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageHighLimitCH6
        public int ThresholdVoltageHighLimitCH6()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageHighLimitLSBLowCH6;
            array[1] = ThresholdVoltageHighLimitLSBMidCH6;
            array[2] = ThresholdVoltageHighLimitMSBLowCH6;
            array[3] = ThresholdVoltageHighLimitMSBHighCH6;
            return BitConverter.ToInt32(array, 0);
        }


        public int ThresholdVoltageLowLimitCH1()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLowLimitLSBLowCH1;
            array[1] = ThresholdVoltageLowLimitLSBMidCH1;
            array[2] = ThresholdVoltageLowLimitMSBLowCH1;
            array[3] = ThresholdVoltageLowLimitMSBHighCH1;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageLowLimitCH2
        public int ThresholdVoltageLowLimitCH2()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLowLimitLSBLowCH2;
            array[1] = ThresholdVoltageLowLimitLSBMidCH2;
            array[2] = ThresholdVoltageLowLimitMSBLowCH2;
            array[3] = ThresholdVoltageLowLimitMSBHighCH2;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageLowLimitCH3
        public int ThresholdVoltageLowLimitCH3()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLowLimitLSBLowCH3;
            array[1] = ThresholdVoltageLowLimitLSBMidCH3;
            array[2] = ThresholdVoltageLowLimitMSBLowCH3;
            array[3] = ThresholdVoltageLowLimitMSBHighCH3;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageLowLimitCH4
        public int ThresholdVoltageLowLimitCH4()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLowLimitLSBLowCH4;
            array[1] = ThresholdVoltageLowLimitLSBMidCH4;
            array[2] = ThresholdVoltageLowLimitMSBLowCH4;
            array[3] = ThresholdVoltageLowLimitMSBHighCH4;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageLowLimitCH5
        public int ThresholdVoltageLowLimitCH5()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLowLimitLSBLowCH5;
            array[1] = ThresholdVoltageLowLimitLSBMidCH5;
            array[2] = ThresholdVoltageLowLimitMSBLowCH5;
            array[3] = ThresholdVoltageLowLimitMSBHighCH5;
            return BitConverter.ToInt32(array, 0);
        }

        // ThresholdVoltageLowLimitCH6
        public int ThresholdVoltageLowLimitCH6()
        {
            byte[] array = new byte[4];
            array[0] = ThresholdVoltageLowLimitLSBLowCH6;
            array[1] = ThresholdVoltageLowLimitLSBMidCH6;
            array[2] = ThresholdVoltageLowLimitMSBLowCH6;
            array[3] = ThresholdVoltageLowLimitMSBHighCH6;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
