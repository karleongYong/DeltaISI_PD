using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe63GetLDUAndLEDData
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte HGAIndex;


        // TA LED points
        public byte Point0LEDLSBLow;
        public byte Point0LEDLSBMid;
        public byte Point0LEDMSBLow;
        public byte Point0LEDMSBHigh;

        public byte Point1LEDLSBLow;
        public byte Point1LEDLSBMid;
        public byte Point1LEDMSBLow;
        public byte Point1LEDMSBHigh;

        public byte Point2LEDLSBLow;
        public byte Point2LEDLSBMid;
        public byte Point2LEDMSBLow;
        public byte Point2LEDMSBHigh;


        public byte Point3LEDLSBLow;
        public byte Point3LEDLSBMid;
        public byte Point3LEDMSBLow;
        public byte Point3LEDMSBHigh;


        public byte Point4LEDLSBLow;
        public byte Point4LEDLSBMid;
        public byte Point4LEDMSBLow;
        public byte Point4LEDMSBHigh;


        public byte Point5LEDLSBLow;
        public byte Point5LEDLSBMid;
        public byte Point5LEDMSBLow;
        public byte Point5LEDMSBHigh;


        public byte Point6LEDLSBLow;
        public byte Point6LEDLSBMid;
        public byte Point6LEDMSBLow;
        public byte Point6LEDMSBHigh;

        public byte Point7LEDLSBLow;
        public byte Point7LEDLSBMid;
        public byte Point7LEDMSBLow;
        public byte Point7LEDMSBHigh;

        public byte Point8LEDLSBLow;
        public byte Point8LEDLSBMid;
        public byte Point8LEDMSBLow;
        public byte Point8LEDMSBHigh;


        public byte Point9LEDLSBLow;
        public byte Point9LEDLSBMid;
        public byte Point9LEDMSBLow;
        public byte Point9LEDMSBHigh;


        public byte Point10LEDLSBLow;
        public byte Point10LEDLSBMid;
        public byte Point10LEDMSBLow;
        public byte Point10LEDMSBHigh;

        public byte Point11LEDLSBLow;
        public byte Point11LEDLSBMid;
        public byte Point11LEDMSBLow;
        public byte Point11LEDMSBHigh;

        public byte Point12LEDLSBLow;
        public byte Point12LEDLSBMid;
        public byte Point12LEDMSBLow;
        public byte Point12LEDMSBHigh;


        public byte Point13LEDLSBLow;
        public byte Point13LEDLSBMid;
        public byte Point13LEDMSBLow;
        public byte Point13LEDMSBHigh;


        public byte Point14LEDLSBLow;
        public byte Point14LEDLSBMid;
        public byte Point14LEDMSBLow;
        public byte Point14LEDMSBHigh;


        public byte Point15LEDLSBLow;
        public byte Point15LEDLSBMid;
        public byte Point15LEDMSBLow;
        public byte Point15LEDMSBHigh;


        public byte Point16LEDLSBLow;
        public byte Point16LEDLSBMid;
        public byte Point16LEDMSBLow;
        public byte Point16LEDMSBHigh;

        public byte Point17LEDLSBLow;
        public byte Point17LEDLSBMid;
        public byte Point17LEDMSBLow;
        public byte Point17LEDMSBHigh;

        public byte Point18LEDLSBLow;
        public byte Point18LEDLSBMid;
        public byte Point18LEDMSBLow;
        public byte Point18LEDMSBHigh;


        public byte Point19LEDLSBLow;
        public byte Point19LEDLSBMid;
        public byte Point19LEDMSBLow;
        public byte Point19LEDMSBHigh;

        public byte Point20LEDLSBLow;
        public byte Point20LEDLSBMid;
        public byte Point20LEDMSBLow;
        public byte Point20LEDMSBHigh;

        // LDU Bias Voltage 21 points
        public byte Point0LDULSBLow;
        public byte Point0LDULSBMid;
        public byte Point0LDUMSBLow;
        public byte Point0LDUMSBHigh;


        public byte Point1LDULSBLow;
        public byte Point1LDULSBMid;
        public byte Point1LDUMSBLow;
        public byte Point1LDUMSBHigh;

        public byte Point2LDULSBLow;
        public byte Point2LDULSBMid;
        public byte Point2LDUMSBLow;
        public byte Point2LDUMSBHigh;


        public byte Point3LDULSBLow;
        public byte Point3LDULSBMid;
        public byte Point3LDUMSBLow;
        public byte Point3LDUMSBHigh;

        public byte Point4LDULSBLow;
        public byte Point4LDULSBMid;
        public byte Point4LDUMSBLow;
        public byte Point4LDUMSBHigh;


        public byte Point5LDULSBLow;
        public byte Point5LDULSBMid;
        public byte Point5LDUMSBLow;
        public byte Point5LDUMSBHigh;


        public byte Point6LDULSBLow;
        public byte Point6LDULSBMid;
        public byte Point6LDUMSBLow;
        public byte Point6LDUMSBHigh;

        public byte Point7LDULSBLow;
        public byte Point7LDULSBMid;
        public byte Point7LDUMSBLow;
        public byte Point7LDUMSBHigh;

        public byte Point8LDULSBLow;
        public byte Point8LDULSBMid;
        public byte Point8LDUMSBLow;
        public byte Point8LDUMSBHigh;


        public byte Point9LDULSBLow;
        public byte Point9LDULSBMid;
        public byte Point9LDUMSBLow;
        public byte Point9LDUMSBHigh;


        public byte Point10LDULSBLow;
        public byte Point10LDULSBMid;
        public byte Point10LDUMSBLow;
        public byte Point10LDUMSBHigh;

        public byte Point11LDULSBLow;
        public byte Point11LDULSBMid;
        public byte Point11LDUMSBLow;
        public byte Point11LDUMSBHigh;

        public byte Point12LDULSBLow;
        public byte Point12LDULSBMid;
        public byte Point12LDUMSBLow;
        public byte Point12LDUMSBHigh;


        public byte Point13LDULSBLow;
        public byte Point13LDULSBMid;
        public byte Point13LDUMSBLow;
        public byte Point13LDUMSBHigh;


        public byte Point14LDULSBLow;
        public byte Point14LDULSBMid;
        public byte Point14LDUMSBLow;
        public byte Point14LDUMSBHigh;


        public byte Point15LDULSBLow;
        public byte Point15LDULSBMid;
        public byte Point15LDUMSBLow;
        public byte Point15LDUMSBHigh;


        public byte Point16LDULSBLow;
        public byte Point16LDULSBMid;
        public byte Point16LDUMSBLow;
        public byte Point16LDUMSBHigh;

        public byte Point17LDULSBLow;
        public byte Point17LDULSBMid;
        public byte Point17LDUMSBLow;
        public byte Point17LDUMSBHigh;

        public byte Point18LDULSBLow;
        public byte Point18LDULSBMid;
        public byte Point18LDUMSBLow;
        public byte Point18LDUMSBHigh;


        public byte Point19LDULSBLow;
        public byte Point19LDULSBMid;
        public byte Point19LDUMSBLow;
        public byte Point19LDUMSBHigh;

        public byte Point20LDULSBLow;
        public byte Point20LDULSBMid;
        public byte Point20LDUMSBLow;
        public byte Point20LDUMSBHigh;


        // TA DeltaResistance
       /* public byte DeltaRes0LSBLow;
        public byte DeltaRes0LSBMid;
        public byte DeltaRes0MSBLow;
        public byte DeltaRes0MSBHigh;

        public byte DeltaRes1LSBLow;
        public byte DeltaRes1LSBMid;
        public byte DeltaRes1MSBLow;
        public byte DeltaRes1MSBHigh;*/

        public static TestProbe63GetLDUAndLEDData ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe63GetLDUAndLEDData*)pb;
            }
        }

        // Point1
        public int Point0LED()
        {
            byte[] array = new byte[4];
            array[0] = Point0LEDLSBLow;
            array[1] = Point0LEDLSBMid;
            array[2] = Point0LEDMSBLow;
            array[3] = Point0LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point1
        public int Point1LED()
        {
            byte[] array = new byte[4];
            array[0] = Point1LEDLSBLow;
            array[1] = Point1LEDLSBMid;
            array[2] = Point1LEDMSBLow;
            array[3] = Point1LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point2
        public int Point2LED()
        {
            byte[] array = new byte[4];
            array[0] = Point2LEDLSBLow;
            array[1] = Point2LEDLSBMid;
            array[2] = Point2LEDMSBLow;
            array[3] = Point2LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point3
        public int Point3LED()
        {
            byte[] array = new byte[4];
            array[0] = Point3LEDLSBLow;
            array[1] = Point3LEDLSBMid;
            array[2] = Point3LEDMSBLow;
            array[3] = Point3LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point4
        public int Point4LED()
        {
            byte[] array = new byte[4];
            array[0] = Point4LEDLSBLow;
            array[1] = Point4LEDLSBMid;
            array[2] = Point4LEDMSBLow;
            array[3] = Point4LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point5
        public int Point5LED()
        {
            byte[] array = new byte[4];
            array[0] = Point5LEDLSBLow;
            array[1] = Point5LEDLSBMid;
            array[2] = Point5LEDMSBLow;
            array[3] = Point5LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point6
        public int Point6LED()
        {
            byte[] array = new byte[4];
            array[0] = Point6LEDLSBLow;
            array[1] = Point6LEDLSBMid;
            array[2] = Point6LEDMSBLow;
            array[3] = Point6LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point7
        public int Point7LED()
        {
            byte[] array = new byte[4];
            array[0] = Point7LEDLSBLow;
            array[1] = Point7LEDLSBMid;
            array[2] = Point7LEDMSBLow;
            array[3] = Point7LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point8
        public int Point8LED()
        {
            byte[] array = new byte[4];
            array[0] = Point8LEDLSBLow;
            array[1] = Point8LEDLSBMid;
            array[2] = Point8LEDMSBLow;
            array[3] = Point8LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point9
        public int Point9LED()
        {
            byte[] array = new byte[4];
            array[0] = Point9LEDLSBLow;
            array[1] = Point9LEDLSBMid;
            array[2] = Point9LEDMSBLow;
            array[3] = Point9LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point10
        public int Point10LED()
        {
            byte[] array = new byte[4];
            array[0] = Point10LEDLSBLow;
            array[1] = Point10LEDLSBMid;
            array[2] = Point10LEDMSBLow;
            array[3] = Point10LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        // Point11
        public int Point11LED()
        {
            byte[] array = new byte[4];
            array[0] = Point11LEDLSBLow;
            array[1] = Point11LEDLSBMid;
            array[2] = Point11LEDMSBLow;
            array[3] = Point11LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point12
        public int Point12LED()
        {
            byte[] array = new byte[4];
            array[0] = Point12LEDLSBLow;
            array[1] = Point12LEDLSBMid;
            array[2] = Point12LEDMSBLow;
            array[3] = Point12LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point13
        public int Point13LED()
        {
            byte[] array = new byte[4];
            array[0] = Point13LEDLSBLow;
            array[1] = Point13LEDLSBMid;
            array[2] = Point13LEDMSBLow;
            array[3] = Point13LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point14
        public int Point14LED()
        {
            byte[] array = new byte[4];
            array[0] = Point14LEDLSBLow;
            array[1] = Point14LEDLSBMid;
            array[2] = Point14LEDMSBLow;
            array[3] = Point14LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point15
        public int Point15LED()
        {
            byte[] array = new byte[4];
            array[0] = Point15LEDLSBLow;
            array[1] = Point15LEDLSBMid;
            array[2] = Point15LEDMSBLow;
            array[3] = Point15LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point16
        public int Point16LED()
        {
            byte[] array = new byte[4];
            array[0] = Point16LEDLSBLow;
            array[1] = Point16LEDLSBMid;
            array[2] = Point16LEDMSBLow;
            array[3] = Point16LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point17
        public int Point17LED()
        {
            byte[] array = new byte[4];
            array[0] = Point17LEDLSBLow;
            array[1] = Point17LEDLSBMid;
            array[2] = Point17LEDMSBLow;
            array[3] = Point17LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point18
        public int Point18LED()
        {
            byte[] array = new byte[4];
            array[0] = Point18LEDLSBLow;
            array[1] = Point18LEDLSBMid;
            array[2] = Point18LEDMSBLow;
            array[3] = Point18LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Point19
        public int Point19LED()
        {
            byte[] array = new byte[4];
            array[0] = Point19LEDLSBLow;
            array[1] = Point19LEDLSBMid;
            array[2] = Point19LEDMSBLow;
            array[3] = Point19LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        // Point19
        public int Point20LED()
        {
            byte[] array = new byte[4];
            array[0] = Point20LEDLSBLow;
            array[1] = Point20LEDLSBMid;
            array[2] = Point20LEDMSBLow;
            array[3] = Point20LEDMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        // LDU voltage point
        public int Point0LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point0LDULSBLow;
            array[1] = Point0LDULSBMid;
            array[2] = Point0LDUMSBLow;
            array[3] = Point0LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point1LDULSBLow;
            array[1] = Point1LDULSBMid;
            array[2] = Point1LDUMSBLow;
            array[3] = Point1LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point2LDULSBLow;
            array[1] = Point2LDULSBMid;
            array[2] = Point2LDUMSBLow;
            array[3] = Point2LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point3LDULSBLow;
            array[1] = Point3LDULSBMid;
            array[2] = Point3LDUMSBLow;
            array[3] = Point3LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point4LDULSBLow;
            array[1] = Point4LDULSBMid;
            array[2] = Point4LDUMSBLow;
            array[3] = Point4LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point5LDULSBLow;
            array[1] = Point5LDULSBMid;
            array[2] = Point5LDUMSBLow;
            array[3] = Point5LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point6LDULSBLow;
            array[1] = Point6LDULSBMid;
            array[2] = Point6LDUMSBLow;
            array[3] = Point6LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point7LDULSBLow;
            array[1] = Point7LDULSBMid;
            array[2] = Point7LDUMSBLow;
            array[3] = Point7LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point8LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point8LDULSBLow;
            array[1] = Point8LDULSBMid;
            array[2] = Point8LDUMSBLow;
            array[3] = Point8LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point9LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point9LDULSBLow;
            array[1] = Point9LDULSBMid;
            array[2] = Point9LDUMSBLow;
            array[3] = Point9LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point10LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point10LDULSBLow;
            array[1] = Point10LDULSBMid;
            array[2] = Point10LDUMSBLow;
            array[3] = Point10LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        public int Point11LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point11LDULSBLow;
            array[1] = Point11LDULSBMid;
            array[2] = Point11LDUMSBLow;
            array[3] = Point11LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point12LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point12LDULSBLow;
            array[1] = Point12LDULSBMid;
            array[2] = Point12LDUMSBLow;
            array[3] = Point12LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point13LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point13LDULSBLow;
            array[1] = Point13LDULSBMid;
            array[2] = Point13LDUMSBLow;
            array[3] = Point13LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point14LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point14LDULSBLow;
            array[1] = Point14LDULSBMid;
            array[2] = Point14LDUMSBLow;
            array[3] = Point14LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point15LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point15LDULSBLow;
            array[1] = Point15LDULSBMid;
            array[2] = Point15LDUMSBLow;
            array[3] = Point15LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point16LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point16LDULSBLow;
            array[1] = Point16LDULSBMid;
            array[2] = Point16LDUMSBLow;
            array[3] = Point16LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point17LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point17LDULSBLow;
            array[1] = Point17LDULSBMid;
            array[2] = Point17LDUMSBLow;
            array[3] = Point17LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point18LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point18LDULSBLow;
            array[1] = Point18LDULSBMid;
            array[2] = Point18LDUMSBLow;
            array[3] = Point18LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point19LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point19LDULSBLow;
            array[1] = Point19LDULSBMid;
            array[2] = Point19LDUMSBLow;
            array[3] = Point19LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point20LDU()
        {
            byte[] array = new byte[4];
            array[0] = Point20LDULSBLow;
            array[1] = Point20LDULSBMid;
            array[2] = Point20LDUMSBLow;
            array[3] = Point20LDUMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
/*
        public int TAResistance_0()
        {
            byte[] array = new byte[4];
            array[0] = DeltaRes0LSBLow;
            array[1] = DeltaRes0LSBMid;
            array[2] = DeltaRes0MSBLow;
            array[3] = DeltaRes0MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int TAResistance_1()
        {
            byte[] array = new byte[4];
            array[0] = DeltaRes1LSBLow;
            array[1] = DeltaRes1LSBMid;
            array[2] = DeltaRes1MSBLow;
            array[3] = DeltaRes1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
        */
    }
}
