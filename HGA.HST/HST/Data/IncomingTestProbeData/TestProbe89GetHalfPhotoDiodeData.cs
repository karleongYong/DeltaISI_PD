using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    
    public unsafe struct TestProbe89GetHalfPhotoDiodeData
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte first_5_Hga;

        // FirstHga LDU Bias Voltage 8 points
        public byte Point0LDULSBLowFirstHga;
        public byte Point0LDULSBMidFirstHga;
        public byte Point0LDUMSBLowFirstHga;
        public byte Point0LDUMSBHighFirstHga;


        public byte Point1LDULSBLowFirstHga;
        public byte Point1LDULSBMidFirstHga;
        public byte Point1LDUMSBLowFirstHga;
        public byte Point1LDUMSBHighFirstHga;

        public byte Point2LDULSBLowFirstHga;
        public byte Point2LDULSBMidFirstHga;
        public byte Point2LDUMSBLowFirstHga;
        public byte Point2LDUMSBHighFirstHga;


        public byte Point3LDULSBLowFirstHga;
        public byte Point3LDULSBMidFirstHga;
        public byte Point3LDUMSBLowFirstHga;
        public byte Point3LDUMSBHighFirstHga;

        public byte Point4LDULSBLowFirstHga;
        public byte Point4LDULSBMidFirstHga;
        public byte Point4LDUMSBLowFirstHga;
        public byte Point4LDUMSBHighFirstHga;

        public byte Point5LDULSBLowFirstHga;
        public byte Point5LDULSBMidFirstHga;
        public byte Point5LDUMSBLowFirstHga;
        public byte Point5LDUMSBHighFirstHga;

        public byte Point6LDULSBLowFirstHga;
        public byte Point6LDULSBMidFirstHga;
        public byte Point6LDUMSBLowFirstHga;
        public byte Point6LDUMSBHighFirstHga;


        public byte Point7LDULSBLowFirstHga;
        public byte Point7LDULSBMidFirstHga;
        public byte Point7LDUMSBLowFirstHga;
        public byte Point7LDUMSBHighFirstHga;

        // SecondHga LDU Bias Voltage 8 points
        public byte Point0LDULSBLowSecondHga;
        public byte Point0LDULSBMidSecondHga;
        public byte Point0LDUMSBLowSecondHga;
        public byte Point0LDUMSBHighSecondHga;


        public byte Point1LDULSBLowSecondHga;
        public byte Point1LDULSBMidSecondHga;
        public byte Point1LDUMSBLowSecondHga;
        public byte Point1LDUMSBHighSecondHga;

        public byte Point2LDULSBLowSecondHga;
        public byte Point2LDULSBMidSecondHga;
        public byte Point2LDUMSBLowSecondHga;
        public byte Point2LDUMSBHighSecondHga;


        public byte Point3LDULSBLowSecondHga;
        public byte Point3LDULSBMidSecondHga;
        public byte Point3LDUMSBLowSecondHga;
        public byte Point3LDUMSBHighSecondHga;

        public byte Point4LDULSBLowSecondHga;
        public byte Point4LDULSBMidSecondHga;
        public byte Point4LDUMSBLowSecondHga;
        public byte Point4LDUMSBHighSecondHga;

        public byte Point5LDULSBLowSecondHga;
        public byte Point5LDULSBMidSecondHga;
        public byte Point5LDUMSBLowSecondHga;
        public byte Point5LDUMSBHighSecondHga;

        public byte Point6LDULSBLowSecondHga;
        public byte Point6LDULSBMidSecondHga;
        public byte Point6LDUMSBLowSecondHga;
        public byte Point6LDUMSBHighSecondHga;


        public byte Point7LDULSBLowSecondHga;
        public byte Point7LDULSBMidSecondHga;
        public byte Point7LDUMSBLowSecondHga;
        public byte Point7LDUMSBHighSecondHga;

        // ThirdHga LDU Bias Voltage 8 points
        public byte Point0LDULSBLowThirdHga;
        public byte Point0LDULSBMidThirdHga;
        public byte Point0LDUMSBLowThirdHga;
        public byte Point0LDUMSBHighThirdHga;


        public byte Point1LDULSBLowThirdHga;
        public byte Point1LDULSBMidThirdHga;
        public byte Point1LDUMSBLowThirdHga;
        public byte Point1LDUMSBHighThirdHga;

        public byte Point2LDULSBLowThirdHga;
        public byte Point2LDULSBMidThirdHga;
        public byte Point2LDUMSBLowThirdHga;
        public byte Point2LDUMSBHighThirdHga;


        public byte Point3LDULSBLowThirdHga;
        public byte Point3LDULSBMidThirdHga;
        public byte Point3LDUMSBLowThirdHga;
        public byte Point3LDUMSBHighThirdHga;

        public byte Point4LDULSBLowThirdHga;
        public byte Point4LDULSBMidThirdHga;
        public byte Point4LDUMSBLowThirdHga;
        public byte Point4LDUMSBHighThirdHga;

        public byte Point5LDULSBLowThirdHga;
        public byte Point5LDULSBMidThirdHga;
        public byte Point5LDUMSBLowThirdHga;
        public byte Point5LDUMSBHighThirdHga;

        public byte Point6LDULSBLowThirdHga;
        public byte Point6LDULSBMidThirdHga;
        public byte Point6LDUMSBLowThirdHga;
        public byte Point6LDUMSBHighThirdHga;


        public byte Point7LDULSBLowThirdHga;
        public byte Point7LDULSBMidThirdHga;
        public byte Point7LDUMSBLowThirdHga;
        public byte Point7LDUMSBHighThirdHga;

        // FourthHga LDU Bias Voltage 8 points
        public byte Point0LDULSBLowFourthHga;
        public byte Point0LDULSBMidFourthHga;
        public byte Point0LDUMSBLowFourthHga;
        public byte Point0LDUMSBHighFourthHga;


        public byte Point1LDULSBLowFourthHga;
        public byte Point1LDULSBMidFourthHga;
        public byte Point1LDUMSBLowFourthHga;
        public byte Point1LDUMSBHighFourthHga;

        public byte Point2LDULSBLowFourthHga;
        public byte Point2LDULSBMidFourthHga;
        public byte Point2LDUMSBLowFourthHga;
        public byte Point2LDUMSBHighFourthHga;


        public byte Point3LDULSBLowFourthHga;
        public byte Point3LDULSBMidFourthHga;
        public byte Point3LDUMSBLowFourthHga;
        public byte Point3LDUMSBHighFourthHga;

        public byte Point4LDULSBLowFourthHga;
        public byte Point4LDULSBMidFourthHga;
        public byte Point4LDUMSBLowFourthHga;
        public byte Point4LDUMSBHighFourthHga;

        public byte Point5LDULSBLowFourthHga;
        public byte Point5LDULSBMidFourthHga;
        public byte Point5LDUMSBLowFourthHga;
        public byte Point5LDUMSBHighFourthHga;

        public byte Point6LDULSBLowFourthHga;
        public byte Point6LDULSBMidFourthHga;
        public byte Point6LDUMSBLowFourthHga;
        public byte Point6LDUMSBHighFourthHga;


        public byte Point7LDULSBLowFourthHga;
        public byte Point7LDULSBMidFourthHga;
        public byte Point7LDUMSBLowFourthHga;
        public byte Point7LDUMSBHighFourthHga;

        // FifthHga LDU Bias Voltage 8 points
        public byte Point0LDULSBLowFifthHga;
        public byte Point0LDULSBMidFifthHga;
        public byte Point0LDUMSBLowFifthHga;
        public byte Point0LDUMSBHighFifthHga;


        public byte Point1LDULSBLowFifthHga;
        public byte Point1LDULSBMidFifthHga;
        public byte Point1LDUMSBLowFifthHga;
        public byte Point1LDUMSBHighFifthHga;

        public byte Point2LDULSBLowFifthHga;
        public byte Point2LDULSBMidFifthHga;
        public byte Point2LDUMSBLowFifthHga;
        public byte Point2LDUMSBHighFifthHga;


        public byte Point3LDULSBLowFifthHga;
        public byte Point3LDULSBMidFifthHga;
        public byte Point3LDUMSBLowFifthHga;
        public byte Point3LDUMSBHighFifthHga;

        public byte Point4LDULSBLowFifthHga;
        public byte Point4LDULSBMidFifthHga;
        public byte Point4LDUMSBLowFifthHga;
        public byte Point4LDUMSBHighFifthHga;

        public byte Point5LDULSBLowFifthHga;
        public byte Point5LDULSBMidFifthHga;
        public byte Point5LDUMSBLowFifthHga;
        public byte Point5LDUMSBHighFifthHga;

        public byte Point6LDULSBLowFifthHga;
        public byte Point6LDULSBMidFifthHga;
        public byte Point6LDUMSBLowFifthHga;
        public byte Point6LDUMSBHighFifthHga;


        public byte Point7LDULSBLowFifthHga;
        public byte Point7LDULSBMidFifthHga;
        public byte Point7LDUMSBLowFifthHga;
        public byte Point7LDUMSBHighFifthHga;


        public static TestProbe89GetHalfPhotoDiodeData ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe89GetHalfPhotoDiodeData*)pb;
            }
        }


        // First Hga LDU voltage point
        public int Point0LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point0LDULSBLowFirstHga;
            array[1] = Point0LDULSBMidFirstHga;
            array[2] = Point0LDUMSBLowFirstHga;
            array[3] = Point0LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point1LDULSBLowFirstHga;
            array[1] = Point1LDULSBMidFirstHga;
            array[2] = Point1LDUMSBLowFirstHga;
            array[3] = Point1LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point2LDULSBLowFirstHga;
            array[1] = Point2LDULSBMidFirstHga;
            array[2] = Point2LDUMSBLowFirstHga;
            array[3] = Point2LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point3LDULSBLowFirstHga;
            array[1] = Point3LDULSBMidFirstHga;
            array[2] = Point3LDUMSBLowFirstHga;
            array[3] = Point3LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point4LDULSBLowFirstHga;
            array[1] = Point4LDULSBMidFirstHga;
            array[2] = Point4LDUMSBLowFirstHga;
            array[3] = Point4LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point5LDULSBLowFirstHga;
            array[1] = Point5LDULSBMidFirstHga;
            array[2] = Point5LDUMSBLowFirstHga;
            array[3] = Point5LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point6LDULSBLowFirstHga;
            array[1] = Point6LDULSBMidFirstHga;
            array[2] = Point6LDUMSBLowFirstHga;
            array[3] = Point6LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7LDUFirstHga()
        {
            byte[] array = new byte[4];
            array[0] = Point7LDULSBLowFirstHga;
            array[1] = Point7LDULSBMidFirstHga;
            array[2] = Point7LDUMSBLowFirstHga;
            array[3] = Point7LDUMSBHighFirstHga;
            return BitConverter.ToInt32(array, 0);
        }


        // Second Hga LDU voltage point
        public int Point0LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point0LDULSBLowSecondHga;
            array[1] = Point0LDULSBMidSecondHga;
            array[2] = Point0LDUMSBLowSecondHga;
            array[3] = Point0LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point1LDULSBLowSecondHga;
            array[1] = Point1LDULSBMidSecondHga;
            array[2] = Point1LDUMSBLowSecondHga;
            array[3] = Point1LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point2LDULSBLowSecondHga;
            array[1] = Point2LDULSBMidSecondHga;
            array[2] = Point2LDUMSBLowSecondHga;
            array[3] = Point2LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point3LDULSBLowSecondHga;
            array[1] = Point3LDULSBMidSecondHga;
            array[2] = Point3LDUMSBLowSecondHga;
            array[3] = Point3LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point4LDULSBLowSecondHga;
            array[1] = Point4LDULSBMidSecondHga;
            array[2] = Point4LDUMSBLowSecondHga;
            array[3] = Point4LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point5LDULSBLowSecondHga;
            array[1] = Point5LDULSBMidSecondHga;
            array[2] = Point5LDUMSBLowSecondHga;
            array[3] = Point5LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point6LDULSBLowSecondHga;
            array[1] = Point6LDULSBMidSecondHga;
            array[2] = Point6LDUMSBLowSecondHga;
            array[3] = Point6LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7LDUSecondHga()
        {
            byte[] array = new byte[4];
            array[0] = Point7LDULSBLowSecondHga;
            array[1] = Point7LDULSBMidSecondHga;
            array[2] = Point7LDUMSBLowSecondHga;
            array[3] = Point7LDUMSBHighSecondHga;
            return BitConverter.ToInt32(array, 0);
        }


        // Third Hga LDU voltage point
        public int Point0LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point0LDULSBLowThirdHga;
            array[1] = Point0LDULSBMidThirdHga;
            array[2] = Point0LDUMSBLowThirdHga;
            array[3] = Point0LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point1LDULSBLowThirdHga;
            array[1] = Point1LDULSBMidThirdHga;
            array[2] = Point1LDUMSBLowThirdHga;
            array[3] = Point1LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point2LDULSBLowThirdHga;
            array[1] = Point2LDULSBMidThirdHga;
            array[2] = Point2LDUMSBLowThirdHga;
            array[3] = Point2LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point3LDULSBLowThirdHga;
            array[1] = Point3LDULSBMidThirdHga;
            array[2] = Point3LDUMSBLowThirdHga;
            array[3] = Point3LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point4LDULSBLowThirdHga;
            array[1] = Point4LDULSBMidThirdHga;
            array[2] = Point4LDUMSBLowThirdHga;
            array[3] = Point4LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point5LDULSBLowThirdHga;
            array[1] = Point5LDULSBMidThirdHga;
            array[2] = Point5LDUMSBLowThirdHga;
            array[3] = Point5LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point6LDULSBLowThirdHga;
            array[1] = Point6LDULSBMidThirdHga;
            array[2] = Point6LDUMSBLowThirdHga;
            array[3] = Point6LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7LDUThirdHga()
        {
            byte[] array = new byte[4];
            array[0] = Point7LDULSBLowThirdHga;
            array[1] = Point7LDULSBMidThirdHga;
            array[2] = Point7LDUMSBLowThirdHga;
            array[3] = Point7LDUMSBHighThirdHga;
            return BitConverter.ToInt32(array, 0);
        }


        // Fourth Hga LDU voltage point
        public int Point0LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point0LDULSBLowFourthHga;
            array[1] = Point0LDULSBMidFourthHga;
            array[2] = Point0LDUMSBLowFourthHga;
            array[3] = Point0LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point1LDULSBLowFourthHga;
            array[1] = Point1LDULSBMidFourthHga;
            array[2] = Point1LDUMSBLowFourthHga;
            array[3] = Point1LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point2LDULSBLowFourthHga;
            array[1] = Point2LDULSBMidFourthHga;
            array[2] = Point2LDUMSBLowFourthHga;
            array[3] = Point2LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point3LDULSBLowFourthHga;
            array[1] = Point3LDULSBMidFourthHga;
            array[2] = Point3LDUMSBLowFourthHga;
            array[3] = Point3LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point4LDULSBLowFourthHga;
            array[1] = Point4LDULSBMidFourthHga;
            array[2] = Point4LDUMSBLowFourthHga;
            array[3] = Point4LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point5LDULSBLowFourthHga;
            array[1] = Point5LDULSBMidFourthHga;
            array[2] = Point5LDUMSBLowFourthHga;
            array[3] = Point5LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point6LDULSBLowFourthHga;
            array[1] = Point6LDULSBMidFourthHga;
            array[2] = Point6LDUMSBLowFourthHga;
            array[3] = Point6LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7LDUFourthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point7LDULSBLowFourthHga;
            array[1] = Point7LDULSBMidFourthHga;
            array[2] = Point7LDUMSBLowFourthHga;
            array[3] = Point7LDUMSBHighFourthHga;
            return BitConverter.ToInt32(array, 0);
        }

        // Fifth Hga LDU voltage point
        public int Point0LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point0LDULSBLowFifthHga;
            array[1] = Point0LDULSBMidFifthHga;
            array[2] = Point0LDUMSBLowFifthHga;
            array[3] = Point0LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point1LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point1LDULSBLowFifthHga;
            array[1] = Point1LDULSBMidFifthHga;
            array[2] = Point1LDUMSBLowFifthHga;
            array[3] = Point1LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point2LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point2LDULSBLowFifthHga;
            array[1] = Point2LDULSBMidFifthHga;
            array[2] = Point2LDUMSBLowFifthHga;
            array[3] = Point2LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point3LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point3LDULSBLowFifthHga;
            array[1] = Point3LDULSBMidFifthHga;
            array[2] = Point3LDUMSBLowFifthHga;
            array[3] = Point3LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point4LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point4LDULSBLowFifthHga;
            array[1] = Point4LDULSBMidFifthHga;
            array[2] = Point4LDUMSBLowFifthHga;
            array[3] = Point4LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point5LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point5LDULSBLowFifthHga;
            array[1] = Point5LDULSBMidFifthHga;
            array[2] = Point5LDUMSBLowFifthHga;
            array[3] = Point5LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point6LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point6LDULSBLowFifthHga;
            array[1] = Point6LDULSBMidFifthHga;
            array[2] = Point6LDUMSBLowFifthHga;
            array[3] = Point6LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

        public int Point7LDUFifthHga()
        {
            byte[] array = new byte[4];
            array[0] = Point7LDULSBLowFifthHga;
            array[1] = Point7LDULSBMidFifthHga;
            array[2] = Point7LDUMSBLowFifthHga;
            array[3] = Point7LDUMSBHighFifthHga;
            return BitConverter.ToInt32(array, 0);
        }

    }
}
