using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe86GetPhotodiodeDataByHGA
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte HGAIndex;

        // LDU Bias Voltage 21 points
        public byte Point0PhotoDiodeVoltageLSBLow;
        public byte Point0PhotoDiodeVoltageLSBMid;
        public byte Point0PhotoDiodeVoltageMSBLow;
        public byte Point0PhotoDiodeVoltageMSBHigh;


        public byte Point1PhotoDiodeVoltageLSBLow;
        public byte Point1PhotoDiodeVoltageLSBMid;
        public byte Point1PhotoDiodeVoltageMSBLow;
        public byte Point1PhotoDiodeVoltageMSBHigh;

        public byte Point2PhotoDiodeVoltageLSBLow;
        public byte Point2PhotoDiodeVoltageLSBMid;
        public byte Point2PhotoDiodeVoltageMSBLow;
        public byte Point2PhotoDiodeVoltageMSBHigh;


        public byte Point3PhotoDiodeVoltageLSBLow;
        public byte Point3PhotoDiodeVoltageLSBMid;
        public byte Point3PhotoDiodeVoltageMSBLow;
        public byte Point3PhotoDiodeVoltageMSBHigh;

        public byte Point4PhotoDiodeVoltageLSBLow;
        public byte Point4PhotoDiodeVoltageLSBMid;
        public byte Point4PhotoDiodeVoltageMSBLow;
        public byte Point4PhotoDiodeVoltageMSBHigh;


        public byte Point5PhotoDiodeVoltageLSBLow;
        public byte Point5PhotoDiodeVoltageLSBMid;
        public byte Point5PhotoDiodeVoltageMSBLow;
        public byte Point5PhotoDiodeVoltageMSBHigh;


        public byte Point6PhotoDiodeVoltageLSBLow;
        public byte Point6PhotoDiodeVoltageLSBMid;
        public byte Point6PhotoDiodeVoltageMSBLow;
        public byte Point6PhotoDiodeVoltageMSBHigh;

        public byte Point7PhotoDiodeVoltageLSBLow;
        public byte Point7PhotoDiodeVoltageLSBMid;
        public byte Point7PhotoDiodeVoltageMSBLow;
        public byte Point7PhotoDiodeVoltageMSBHigh;

        public byte Point8PhotoDiodeVoltageLSBLow;
        public byte Point8PhotoDiodeVoltageLSBMid;
        public byte Point8PhotoDiodeVoltageMSBLow;
        public byte Point8PhotoDiodeVoltageMSBHigh;


        public byte Point9PhotoDiodeVoltageLSBLow;
        public byte Point9PhotoDiodeVoltageLSBMid;
        public byte Point9PhotoDiodeVoltageMSBLow;
        public byte Point9PhotoDiodeVoltageMSBHigh;


        public byte Point10PhotoDiodeVoltageLSBLow;
        public byte Point10PhotoDiodeVoltageLSBMid;
        public byte Point10PhotoDiodeVoltageMSBLow;
        public byte Point10PhotoDiodeVoltageMSBHigh;

        public byte Point11PhotoDiodeVoltageLSBLow;
        public byte Point11PhotoDiodeVoltageLSBMid;
        public byte Point11PhotoDiodeVoltageMSBLow;
        public byte Point11PhotoDiodeVoltageMSBHigh;

        public byte Point12PhotoDiodeVoltageLSBLow;
        public byte Point12PhotoDiodeVoltageLSBMid;
        public byte Point12PhotoDiodeVoltageMSBLow;
        public byte Point12PhotoDiodeVoltageMSBHigh;


        public byte Point13PhotoDiodeVoltageLSBLow;
        public byte Point13PhotoDiodeVoltageLSBMid;
        public byte Point13PhotoDiodeVoltageMSBLow;
        public byte Point13PhotoDiodeVoltageMSBHigh;


        public byte Point14PhotoDiodeVoltageLSBLow;
        public byte Point14PhotoDiodeVoltageLSBMid;
        public byte Point14PhotoDiodeVoltageMSBLow;
        public byte Point14PhotoDiodeVoltageMSBHigh;


        public byte Point15PhotoDiodeVoltageLSBLow;
        public byte Point15PhotoDiodeVoltageLSBMid;
        public byte Point15PhotoDiodeVoltageMSBLow;
        public byte Point15PhotoDiodeVoltageMSBHigh;


        public byte Point16PhotoDiodeVoltageLSBLow;
        public byte Point16PhotoDiodeVoltageLSBMid;
        public byte Point16PhotoDiodeVoltageMSBLow;
        public byte Point16PhotoDiodeVoltageMSBHigh;

        public byte Point17PhotoDiodeVoltageLSBLow;
        public byte Point17PhotoDiodeVoltageLSBMid;
        public byte Point17PhotoDiodeVoltageMSBLow;
        public byte Point17PhotoDiodeVoltageMSBHigh;

        public byte Point18PhotoDiodeVoltageLSBLow;
        public byte Point18PhotoDiodeVoltageLSBMid;
        public byte Point18PhotoDiodeVoltageMSBLow;
        public byte Point18PhotoDiodeVoltageMSBHigh;


        public byte Point19PhotoDiodeVoltageLSBLow;
        public byte Point19PhotoDiodeVoltageLSBMid;
        public byte Point19PhotoDiodeVoltageMSBLow;
        public byte Point19PhotoDiodeVoltageMSBHigh;

        public byte Point20PhotoDiodeVoltageLSBLow;
        public byte Point20PhotoDiodeVoltageLSBMid;
        public byte Point20PhotoDiodeVoltageMSBLow;
        public byte Point20PhotoDiodeVoltageMSBHigh;

        public static TestProbe86GetPhotodiodeDataByHGA ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe86GetPhotodiodeDataByHGA*)pb;
            }
        }


        // LDU voltage point
        public int Point0PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point0PhotoDiodeVoltageLSBLow;
            array[1] = Point0PhotoDiodeVoltageLSBMid;
            array[2] = Point0PhotoDiodeVoltageMSBLow;
            array[3] = Point0PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point1PhotoDiodeVoltageLSBLow;
            array[1] = Point1PhotoDiodeVoltageLSBMid;
            array[2] = Point1PhotoDiodeVoltageMSBLow;
            array[3] = Point1PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point2PhotoDiodeVoltageLSBLow;
            array[1] = Point2PhotoDiodeVoltageLSBMid;
            array[2] = Point2PhotoDiodeVoltageMSBLow;
            array[3] = Point2PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point3PhotoDiodeVoltageLSBLow;
            array[1] = Point3PhotoDiodeVoltageLSBMid;
            array[2] = Point3PhotoDiodeVoltageMSBLow;
            array[3] = Point3PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point4PhotoDiodeVoltageLSBLow;
            array[1] = Point4PhotoDiodeVoltageLSBMid;
            array[2] = Point4PhotoDiodeVoltageMSBLow;
            array[3] = Point4PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point5PhotoDiodeVoltageLSBLow;
            array[1] = Point5PhotoDiodeVoltageLSBMid;
            array[2] = Point5PhotoDiodeVoltageMSBLow;
            array[3] = Point5PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point6PhotoDiodeVoltageLSBLow;
            array[1] = Point6PhotoDiodeVoltageLSBMid;
            array[2] = Point6PhotoDiodeVoltageMSBLow;
            array[3] = Point6PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point7PhotoDiodeVoltageLSBLow;
            array[1] = Point7PhotoDiodeVoltageLSBMid;
            array[2] = Point7PhotoDiodeVoltageMSBLow;
            array[3] = Point7PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point8PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point8PhotoDiodeVoltageLSBLow;
            array[1] = Point8PhotoDiodeVoltageLSBMid;
            array[2] = Point8PhotoDiodeVoltageMSBLow;
            array[3] = Point8PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point9PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point9PhotoDiodeVoltageLSBLow;
            array[1] = Point9PhotoDiodeVoltageLSBMid;
            array[2] = Point9PhotoDiodeVoltageMSBLow;
            array[3] = Point9PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point10PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point10PhotoDiodeVoltageLSBLow;
            array[1] = Point10PhotoDiodeVoltageLSBMid;
            array[2] = Point10PhotoDiodeVoltageMSBLow;
            array[3] = Point10PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        public int Point11PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point11PhotoDiodeVoltageLSBLow;
            array[1] = Point11PhotoDiodeVoltageLSBMid;
            array[2] = Point11PhotoDiodeVoltageMSBLow;
            array[3] = Point11PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point12PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point12PhotoDiodeVoltageLSBLow;
            array[1] = Point12PhotoDiodeVoltageLSBMid;
            array[2] = Point12PhotoDiodeVoltageMSBLow;
            array[3] = Point12PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point13PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point13PhotoDiodeVoltageLSBLow;
            array[1] = Point13PhotoDiodeVoltageLSBMid;
            array[2] = Point13PhotoDiodeVoltageMSBLow;
            array[3] = Point13PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point14PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point14PhotoDiodeVoltageLSBLow;
            array[1] = Point14PhotoDiodeVoltageLSBMid;
            array[2] = Point14PhotoDiodeVoltageMSBLow;
            array[3] = Point14PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point15PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point15PhotoDiodeVoltageLSBLow;
            array[1] = Point15PhotoDiodeVoltageLSBMid;
            array[2] = Point15PhotoDiodeVoltageMSBLow;
            array[3] = Point15PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point16PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point16PhotoDiodeVoltageLSBLow;
            array[1] = Point16PhotoDiodeVoltageLSBMid;
            array[2] = Point16PhotoDiodeVoltageMSBLow;
            array[3] = Point16PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point17PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point17PhotoDiodeVoltageLSBLow;
            array[1] = Point17PhotoDiodeVoltageLSBMid;
            array[2] = Point17PhotoDiodeVoltageMSBLow;
            array[3] = Point17PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point18PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point18PhotoDiodeVoltageLSBLow;
            array[1] = Point18PhotoDiodeVoltageLSBMid;
            array[2] = Point18PhotoDiodeVoltageMSBLow;
            array[3] = Point18PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point19PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point19PhotoDiodeVoltageLSBLow;
            array[1] = Point19PhotoDiodeVoltageLSBMid;
            array[2] = Point19PhotoDiodeVoltageMSBLow;
            array[3] = Point19PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point20PhotoDiodeVoltage()
        {
            byte[] array = new byte[4];
            array[0] = Point20PhotoDiodeVoltageLSBLow;
            array[1] = Point20PhotoDiodeVoltageLSBMid;
            array[2] = Point20PhotoDiodeVoltageMSBLow;
            array[3] = Point20PhotoDiodeVoltageMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
