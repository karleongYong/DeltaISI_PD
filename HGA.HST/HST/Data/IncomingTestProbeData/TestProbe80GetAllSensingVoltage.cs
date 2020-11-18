using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe80GetAllSensingVoltage
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        //Hga1
        public byte Hga1WriterSensingLSBLow;
        public byte Hga1WriterSensingLSBMid;
        public byte Hga1WriterSensingMSBLow;
        public byte Hga1WriterSensingMSBHigh;

        public byte Hga1TASensingLSBLow;
        public byte Hga1TASensingLSBMid;
        public byte Hga1TASensingMSBLow;
        public byte Hga1TASensingMSBHigh;

        public byte Hga1WHSensingLSBLow;
        public byte Hga1WHSensingLSBMid;
        public byte Hga1WHSensingMSBLow;
        public byte Hga1WHSensingMSBHigh;

        public byte Hga1RHSensingLSBLow;
        public byte Hga1RHSensingLSBMid;
        public byte Hga1RHSensingMSBLow;
        public byte Hga1RHSensingMSBHigh;

        public byte Hga1R1SensingLSBLow;
        public byte Hga1R1SensingLSBMid;
        public byte Hga1R1SensingMSBLow;
        public byte Hga1R1SensingMSBHigh;

        public byte Hga1R2SensingLSBLow;
        public byte Hga1R2SensingLSBMid;
        public byte Hga1R2SensingMSBLow;
        public byte Hga1R2SensingMSBHigh;

        //

        //Hga2
        public byte Hga2WriterSensingLSBLow;
        public byte Hga2WriterSensingLSBMid;
        public byte Hga2WriterSensingMSBLow;
        public byte Hga2WriterSensingMSBHigh;

        public byte Hga2TASensingLSBLow;
        public byte Hga2TASensingLSBMid;
        public byte Hga2TASensingMSBLow;
        public byte Hga2TASensingMSBHigh;

        public byte Hga2WHSensingLSBLow;
        public byte Hga2WHSensingLSBMid;
        public byte Hga2WHSensingMSBLow;
        public byte Hga2WHSensingMSBHigh;

        public byte Hga2RHSensingLSBLow;
        public byte Hga2RHSensingLSBMid;
        public byte Hga2RHSensingMSBLow;
        public byte Hga2RHSensingMSBHigh;

        public byte Hga2R1SensingLSBLow;
        public byte Hga2R1SensingLSBMid;
        public byte Hga2R1SensingMSBLow;
        public byte Hga2R1SensingMSBHigh;

        public byte Hga2R2SensingLSBLow;
        public byte Hga2R2SensingLSBMid;
        public byte Hga2R2SensingMSBLow;
        public byte Hga2R2SensingMSBHigh;

        //

        //Hga3
        public byte Hga3WriterSensingLSBLow;
        public byte Hga3WriterSensingLSBMid;
        public byte Hga3WriterSensingMSBLow;
        public byte Hga3WriterSensingMSBHigh;

        public byte Hga3TASensingLSBLow;
        public byte Hga3TASensingLSBMid;
        public byte Hga3TASensingMSBLow;
        public byte Hga3TASensingMSBHigh;

        public byte Hga3WHSensingLSBLow;
        public byte Hga3WHSensingLSBMid;
        public byte Hga3WHSensingMSBLow;
        public byte Hga3WHSensingMSBHigh;

        public byte Hga3RHSensingLSBLow;
        public byte Hga3RHSensingLSBMid;
        public byte Hga3RHSensingMSBLow;
        public byte Hga3RHSensingMSBHigh;

        public byte Hga3R1SensingLSBLow;
        public byte Hga3R1SensingLSBMid;
        public byte Hga3R1SensingMSBLow;
        public byte Hga3R1SensingMSBHigh;

        public byte Hga3R2SensingLSBLow;
        public byte Hga3R2SensingLSBMid;
        public byte Hga3R2SensingMSBLow;
        public byte Hga3R2SensingMSBHigh;

        //

        //Hga4
        public byte Hga4WriterSensingLSBLow;
        public byte Hga4WriterSensingLSBMid;
        public byte Hga4WriterSensingMSBLow;
        public byte Hga4WriterSensingMSBHigh;

        public byte Hga4TASensingLSBLow;
        public byte Hga4TASensingLSBMid;
        public byte Hga4TASensingMSBLow;
        public byte Hga4TASensingMSBHigh;

        public byte Hga4WHSensingLSBLow;
        public byte Hga4WHSensingLSBMid;
        public byte Hga4WHSensingMSBLow;
        public byte Hga4WHSensingMSBHigh;

        public byte Hga4RHSensingLSBLow;
        public byte Hga4RHSensingLSBMid;
        public byte Hga4RHSensingMSBLow;
        public byte Hga4RHSensingMSBHigh;

        public byte Hga4R1SensingLSBLow;
        public byte Hga4R1SensingLSBMid;
        public byte Hga4R1SensingMSBLow;
        public byte Hga4R1SensingMSBHigh;

        public byte Hga4R2SensingLSBLow;
        public byte Hga4R2SensingLSBMid;
        public byte Hga4R2SensingMSBLow;
        public byte Hga4R2SensingMSBHigh;

        //

        //Hga5
        public byte Hga5WriterSensingLSBLow;
        public byte Hga5WriterSensingLSBMid;
        public byte Hga5WriterSensingMSBLow;
        public byte Hga5WriterSensingMSBHigh;

        public byte Hga5TASensingLSBLow;
        public byte Hga5TASensingLSBMid;
        public byte Hga5TASensingMSBLow;
        public byte Hga5TASensingMSBHigh;

        public byte Hga5WHSensingLSBLow;
        public byte Hga5WHSensingLSBMid;
        public byte Hga5WHSensingMSBLow;
        public byte Hga5WHSensingMSBHigh;

        public byte Hga5RHSensingLSBLow;
        public byte Hga5RHSensingLSBMid;
        public byte Hga5RHSensingMSBLow;
        public byte Hga5RHSensingMSBHigh;

        public byte Hga5R1SensingLSBLow;
        public byte Hga5R1SensingLSBMid;
        public byte Hga5R1SensingMSBLow;
        public byte Hga5R1SensingMSBHigh;

        public byte Hga5R2SensingLSBLow;
        public byte Hga5R2SensingLSBMid;
        public byte Hga5R2SensingMSBLow;
        public byte Hga5R2SensingMSBHigh;

        //

        //Hga6
        public byte Hga6WriterSensingLSBLow;
        public byte Hga6WriterSensingLSBMid;
        public byte Hga6WriterSensingMSBLow;
        public byte Hga6WriterSensingMSBHigh;

        public byte Hga6TASensingLSBLow;
        public byte Hga6TASensingLSBMid;
        public byte Hga6TASensingMSBLow;
        public byte Hga6TASensingMSBHigh;

        public byte Hga6WHSensingLSBLow;
        public byte Hga6WHSensingLSBMid;
        public byte Hga6WHSensingMSBLow;
        public byte Hga6WHSensingMSBHigh;

        public byte Hga6RHSensingLSBLow;
        public byte Hga6RHSensingLSBMid;
        public byte Hga6RHSensingMSBLow;
        public byte Hga6RHSensingMSBHigh;

        public byte Hga6R1SensingLSBLow;
        public byte Hga6R1SensingLSBMid;
        public byte Hga6R1SensingMSBLow;
        public byte Hga6R1SensingMSBHigh;

        public byte Hga6R2SensingLSBLow;
        public byte Hga6R2SensingLSBMid;
        public byte Hga6R2SensingMSBLow;
        public byte Hga6R2SensingMSBHigh;

        //

        //Hga7
        public byte Hga7WriterSensingLSBLow;
        public byte Hga7WriterSensingLSBMid;
        public byte Hga7WriterSensingMSBLow;
        public byte Hga7WriterSensingMSBHigh;

        public byte Hga7TASensingLSBLow;
        public byte Hga7TASensingLSBMid;
        public byte Hga7TASensingMSBLow;
        public byte Hga7TASensingMSBHigh;

        public byte Hga7WHSensingLSBLow;
        public byte Hga7WHSensingLSBMid;
        public byte Hga7WHSensingMSBLow;
        public byte Hga7WHSensingMSBHigh;

        public byte Hga7RHSensingLSBLow;
        public byte Hga7RHSensingLSBMid;
        public byte Hga7RHSensingMSBLow;
        public byte Hga7RHSensingMSBHigh;

        public byte Hga7R1SensingLSBLow;
        public byte Hga7R1SensingLSBMid;
        public byte Hga7R1SensingMSBLow;
        public byte Hga7R1SensingMSBHigh;

        public byte Hga7R2SensingLSBLow;
        public byte Hga7R2SensingLSBMid;
        public byte Hga7R2SensingMSBLow;
        public byte Hga7R2SensingMSBHigh;

        //

        //Hga8
        public byte Hga8WriterSensingLSBLow;
        public byte Hga8WriterSensingLSBMid;
        public byte Hga8WriterSensingMSBLow;
        public byte Hga8WriterSensingMSBHigh;

        public byte Hga8TASensingLSBLow;
        public byte Hga8TASensingLSBMid;
        public byte Hga8TASensingMSBLow;
        public byte Hga8TASensingMSBHigh;

        public byte Hga8WHSensingLSBLow;
        public byte Hga8WHSensingLSBMid;
        public byte Hga8WHSensingMSBLow;
        public byte Hga8WHSensingMSBHigh;

        public byte Hga8RHSensingLSBLow;
        public byte Hga8RHSensingLSBMid;
        public byte Hga8RHSensingMSBLow;
        public byte Hga8RHSensingMSBHigh;

        public byte Hga8R1SensingLSBLow;
        public byte Hga8R1SensingLSBMid;
        public byte Hga8R1SensingMSBLow;
        public byte Hga8R1SensingMSBHigh;

        public byte Hga8R2SensingLSBLow;
        public byte Hga8R2SensingLSBMid;
        public byte Hga8R2SensingMSBLow;
        public byte Hga8R2SensingMSBHigh;

        //

        //Hga9
        public byte Hga9WriterSensingLSBLow;
        public byte Hga9WriterSensingLSBMid;
        public byte Hga9WriterSensingMSBLow;
        public byte Hga9WriterSensingMSBHigh;

        public byte Hga9TASensingLSBLow;
        public byte Hga9TASensingLSBMid;
        public byte Hga9TASensingMSBLow;
        public byte Hga9TASensingMSBHigh;

        public byte Hga9WHSensingLSBLow;
        public byte Hga9WHSensingLSBMid;
        public byte Hga9WHSensingMSBLow;
        public byte Hga9WHSensingMSBHigh;

        public byte Hga9RHSensingLSBLow;
        public byte Hga9RHSensingLSBMid;
        public byte Hga9RHSensingMSBLow;
        public byte Hga9RHSensingMSBHigh;

        public byte Hga9R1SensingLSBLow;
        public byte Hga9R1SensingLSBMid;
        public byte Hga9R1SensingMSBLow;
        public byte Hga9R1SensingMSBHigh;

        public byte Hga9R2SensingLSBLow;
        public byte Hga9R2SensingLSBMid;
        public byte Hga9R2SensingMSBLow;
        public byte Hga9R2SensingMSBHigh;

        //

        //Hga10
        public byte Hga10WriterSensingLSBLow;
        public byte Hga10WriterSensingLSBMid;
        public byte Hga10WriterSensingMSBLow;
        public byte Hga10WriterSensingMSBHigh;

        public byte Hga10TASensingLSBLow;
        public byte Hga10TASensingLSBMid;
        public byte Hga10TASensingMSBLow;
        public byte Hga10TASensingMSBHigh;

        public byte Hga10WHSensingLSBLow;
        public byte Hga10WHSensingLSBMid;
        public byte Hga10WHSensingMSBLow;
        public byte Hga10WHSensingMSBHigh;

        public byte Hga10RHSensingLSBLow;
        public byte Hga10RHSensingLSBMid;
        public byte Hga10RHSensingMSBLow;
        public byte Hga10RHSensingMSBHigh;

        public byte Hga10R1SensingLSBLow;
        public byte Hga10R1SensingLSBMid;
        public byte Hga10R1SensingMSBLow;
        public byte Hga10R1SensingMSBHigh;

        public byte Hga10R2SensingLSBLow;
        public byte Hga10R2SensingLSBMid;
        public byte Hga10R2SensingMSBLow;
        public byte Hga10R2SensingMSBHigh;

        //

        public static TestProbe80GetAllSensingVoltage ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe80GetAllSensingVoltage*)pb;
            }
        }

        //Hga1
        public int Hga1WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga1WriterSensingLSBLow;
            array[1] = Hga1WriterSensingLSBMid;
            array[2] = Hga1WriterSensingMSBLow;
            array[3] = Hga1WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga1TASensingLSBLow;
            array[1] = Hga1TASensingLSBMid;
            array[2] = Hga1TASensingMSBLow;
            array[3] = Hga1TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga1WHSensingLSBLow;
            array[1] = Hga1WHSensingLSBMid;
            array[2] = Hga1WHSensingMSBLow;
            array[3] = Hga1WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga1RHSensingLSBLow;
            array[1] = Hga1RHSensingLSBMid;
            array[2] = Hga1RHSensingMSBLow;
            array[3] = Hga1RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga1R1SensingLSBLow;
            array[1] = Hga1R1SensingLSBMid;
            array[2] = Hga1R1SensingMSBLow;
            array[3] = Hga1R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga1R2SensingLSBLow;
            array[1] = Hga1R2SensingLSBMid;
            array[2] = Hga1R2SensingMSBLow;
            array[3] = Hga1R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga2
        public int Hga2WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga2WriterSensingLSBLow;
            array[1] = Hga2WriterSensingLSBMid;
            array[2] = Hga2WriterSensingMSBLow;
            array[3] = Hga2WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga2TASensingLSBLow;
            array[1] = Hga2TASensingLSBMid;
            array[2] = Hga2TASensingMSBLow;
            array[3] = Hga2TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga2WHSensingLSBLow;
            array[1] = Hga2WHSensingLSBMid;
            array[2] = Hga2WHSensingMSBLow;
            array[3] = Hga2WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga2RHSensingLSBLow;
            array[1] = Hga2RHSensingLSBMid;
            array[2] = Hga2RHSensingMSBLow;
            array[3] = Hga2RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga2R1SensingLSBLow;
            array[1] = Hga2R1SensingLSBMid;
            array[2] = Hga2R1SensingMSBLow;
            array[3] = Hga2R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga2R2SensingLSBLow;
            array[1] = Hga2R2SensingLSBMid;
            array[2] = Hga2R2SensingMSBLow;
            array[3] = Hga2R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //Hga3
        public int Hga3WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga3WriterSensingLSBLow;
            array[1] = Hga3WriterSensingLSBMid;
            array[2] = Hga3WriterSensingMSBLow;
            array[3] = Hga3WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga3TASensingLSBLow;
            array[1] = Hga3TASensingLSBMid;
            array[2] = Hga3TASensingMSBLow;
            array[3] = Hga3TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga3WHSensingLSBLow;
            array[1] = Hga3WHSensingLSBMid;
            array[2] = Hga3WHSensingMSBLow;
            array[3] = Hga3WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga3RHSensingLSBLow;
            array[1] = Hga3RHSensingLSBMid;
            array[2] = Hga3RHSensingMSBLow;
            array[3] = Hga3RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga3R1SensingLSBLow;
            array[1] = Hga3R1SensingLSBMid;
            array[2] = Hga3R1SensingMSBLow;
            array[3] = Hga3R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga3R2SensingLSBLow;
            array[1] = Hga3R2SensingLSBMid;
            array[2] = Hga3R2SensingMSBLow;
            array[3] = Hga3R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //Hga4
        public int Hga4WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga4WriterSensingLSBLow;
            array[1] = Hga4WriterSensingLSBMid;
            array[2] = Hga4WriterSensingMSBLow;
            array[3] = Hga4WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga4TASensingLSBLow;
            array[1] = Hga4TASensingLSBMid;
            array[2] = Hga4TASensingMSBLow;
            array[3] = Hga4TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga4WHSensingLSBLow;
            array[1] = Hga4WHSensingLSBMid;
            array[2] = Hga4WHSensingMSBLow;
            array[3] = Hga4WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga4RHSensingLSBLow;
            array[1] = Hga4RHSensingLSBMid;
            array[2] = Hga4RHSensingMSBLow;
            array[3] = Hga4RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga4R1SensingLSBLow;
            array[1] = Hga4R1SensingLSBMid;
            array[2] = Hga4R1SensingMSBLow;
            array[3] = Hga4R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga4R2SensingLSBLow;
            array[1] = Hga4R2SensingLSBMid;
            array[2] = Hga4R2SensingMSBLow;
            array[3] = Hga4R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga5
        public int Hga5WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga5WriterSensingLSBLow;
            array[1] = Hga5WriterSensingLSBMid;
            array[2] = Hga5WriterSensingMSBLow;
            array[3] = Hga5WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga5TASensingLSBLow;
            array[1] = Hga5TASensingLSBMid;
            array[2] = Hga5TASensingMSBLow;
            array[3] = Hga5TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga5WHSensingLSBLow;
            array[1] = Hga5WHSensingLSBMid;
            array[2] = Hga5WHSensingMSBLow;
            array[3] = Hga5WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga5RHSensingLSBLow;
            array[1] = Hga5RHSensingLSBMid;
            array[2] = Hga5RHSensingMSBLow;
            array[3] = Hga5RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga5R1SensingLSBLow;
            array[1] = Hga5R1SensingLSBMid;
            array[2] = Hga5R1SensingMSBLow;
            array[3] = Hga5R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga5R2SensingLSBLow;
            array[1] = Hga5R2SensingLSBMid;
            array[2] = Hga5R2SensingMSBLow;
            array[3] = Hga5R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga6
        public int Hga6WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga6WriterSensingLSBLow;
            array[1] = Hga6WriterSensingLSBMid;
            array[2] = Hga6WriterSensingMSBLow;
            array[3] = Hga6WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga6TASensingLSBLow;
            array[1] = Hga6TASensingLSBMid;
            array[2] = Hga6TASensingMSBLow;
            array[3] = Hga6TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga6WHSensingLSBLow;
            array[1] = Hga6WHSensingLSBMid;
            array[2] = Hga6WHSensingMSBLow;
            array[3] = Hga6WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga6RHSensingLSBLow;
            array[1] = Hga6RHSensingLSBMid;
            array[2] = Hga6RHSensingMSBLow;
            array[3] = Hga6RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga6R1SensingLSBLow;
            array[1] = Hga6R1SensingLSBMid;
            array[2] = Hga6R1SensingMSBLow;
            array[3] = Hga6R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga6R2SensingLSBLow;
            array[1] = Hga6R2SensingLSBMid;
            array[2] = Hga6R2SensingMSBLow;
            array[3] = Hga6R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga7
        public int Hga7WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga7WriterSensingLSBLow;
            array[1] = Hga7WriterSensingLSBMid;
            array[2] = Hga7WriterSensingMSBLow;
            array[3] = Hga7WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga7TASensingLSBLow;
            array[1] = Hga7TASensingLSBMid;
            array[2] = Hga7TASensingMSBLow;
            array[3] = Hga7TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga7WHSensingLSBLow;
            array[1] = Hga7WHSensingLSBMid;
            array[2] = Hga7WHSensingMSBLow;
            array[3] = Hga7WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga7RHSensingLSBLow;
            array[1] = Hga7RHSensingLSBMid;
            array[2] = Hga7RHSensingMSBLow;
            array[3] = Hga7RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga7R1SensingLSBLow;
            array[1] = Hga7R1SensingLSBMid;
            array[2] = Hga7R1SensingMSBLow;
            array[3] = Hga7R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga7R2SensingLSBLow;
            array[1] = Hga7R2SensingLSBMid;
            array[2] = Hga7R2SensingMSBLow;
            array[3] = Hga7R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga8
        public int Hga8WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga8WriterSensingLSBLow;
            array[1] = Hga8WriterSensingLSBMid;
            array[2] = Hga8WriterSensingMSBLow;
            array[3] = Hga8WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga8TASensingLSBLow;
            array[1] = Hga8TASensingLSBMid;
            array[2] = Hga8TASensingMSBLow;
            array[3] = Hga8TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga8WHSensingLSBLow;
            array[1] = Hga8WHSensingLSBMid;
            array[2] = Hga8WHSensingMSBLow;
            array[3] = Hga8WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga8RHSensingLSBLow;
            array[1] = Hga8RHSensingLSBMid;
            array[2] = Hga8RHSensingMSBLow;
            array[3] = Hga8RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga8R1SensingLSBLow;
            array[1] = Hga8R1SensingLSBMid;
            array[2] = Hga8R1SensingMSBLow;
            array[3] = Hga8R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga8R2SensingLSBLow;
            array[1] = Hga8R2SensingLSBMid;
            array[2] = Hga8R2SensingMSBLow;
            array[3] = Hga8R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga9
        public int Hga9WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga9WriterSensingLSBLow;
            array[1] = Hga9WriterSensingLSBMid;
            array[2] = Hga9WriterSensingMSBLow;
            array[3] = Hga9WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga9TASensingLSBLow;
            array[1] = Hga9TASensingLSBMid;
            array[2] = Hga9TASensingMSBLow;
            array[3] = Hga9TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga9WHSensingLSBLow;
            array[1] = Hga9WHSensingLSBMid;
            array[2] = Hga9WHSensingMSBLow;
            array[3] = Hga9WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga9RHSensingLSBLow;
            array[1] = Hga9RHSensingLSBMid;
            array[2] = Hga9RHSensingMSBLow;
            array[3] = Hga9RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga9R1SensingLSBLow;
            array[1] = Hga9R1SensingLSBMid;
            array[2] = Hga9R1SensingMSBLow;
            array[3] = Hga9R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga9R2SensingLSBLow;
            array[1] = Hga9R2SensingLSBMid;
            array[2] = Hga9R2SensingMSBLow;
            array[3] = Hga9R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga10
        public int Hga10WriterSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga10WriterSensingLSBLow;
            array[1] = Hga10WriterSensingLSBMid;
            array[2] = Hga10WriterSensingMSBLow;
            array[3] = Hga10WriterSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10TASensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga10TASensingLSBLow;
            array[1] = Hga10TASensingLSBMid;
            array[2] = Hga10TASensingMSBLow;
            array[3] = Hga10TASensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10WHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga10WHSensingLSBLow;
            array[1] = Hga10WHSensingLSBMid;
            array[2] = Hga10WHSensingMSBLow;
            array[3] = Hga10WHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10RHSensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga10RHSensingLSBLow;
            array[1] = Hga10RHSensingLSBMid;
            array[2] = Hga10RHSensingMSBLow;
            array[3] = Hga10RHSensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10R1Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga10R1SensingLSBLow;
            array[1] = Hga10R1SensingLSBMid;
            array[2] = Hga10R1SensingMSBLow;
            array[3] = Hga10R1SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10R2Sensing()
        {
            byte[] array = new byte[4];
            array[0] = Hga10R2SensingLSBLow;
            array[1] = Hga10R2SensingLSBMid;
            array[2] = Hga10R2SensingMSBLow;
            array[3] = Hga10R2SensingMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}

