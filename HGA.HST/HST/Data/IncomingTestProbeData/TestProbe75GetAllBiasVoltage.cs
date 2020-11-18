using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe75GetAllBiasVoltage
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        //Hga1
        public byte Hga1WriterBiasLSBLow;
        public byte Hga1WriterBiasLSBMid;
        public byte Hga1WriterBiasMSBLow;
        public byte Hga1WriterBiasMSBHigh;

        public byte Hga1TABiasLSBLow;
        public byte Hga1TABiasLSBMid;
        public byte Hga1TABiasMSBLow;
        public byte Hga1TABiasMSBHigh;

        public byte Hga1WHBiasLSBLow;
        public byte Hga1WHBiasLSBMid;
        public byte Hga1WHBiasMSBLow;
        public byte Hga1WHBiasMSBHigh;

        public byte Hga1RHBiasLSBLow;
        public byte Hga1RHBiasLSBMid;
        public byte Hga1RHBiasMSBLow;
        public byte Hga1RHBiasMSBHigh;

        public byte Hga1R1BiasLSBLow;
        public byte Hga1R1BiasLSBMid;
        public byte Hga1R1BiasMSBLow;
        public byte Hga1R1BiasMSBHigh;

        public byte Hga1R2BiasLSBLow;
        public byte Hga1R2BiasLSBMid;
        public byte Hga1R2BiasMSBLow;
        public byte Hga1R2BiasMSBHigh;

        //

        //Hga2
        public byte Hga2WriterBiasLSBLow;
        public byte Hga2WriterBiasLSBMid;
        public byte Hga2WriterBiasMSBLow;
        public byte Hga2WriterBiasMSBHigh;

        public byte Hga2TABiasLSBLow;
        public byte Hga2TABiasLSBMid;
        public byte Hga2TABiasMSBLow;
        public byte Hga2TABiasMSBHigh;

        public byte Hga2WHBiasLSBLow;
        public byte Hga2WHBiasLSBMid;
        public byte Hga2WHBiasMSBLow;
        public byte Hga2WHBiasMSBHigh;

        public byte Hga2RHBiasLSBLow;
        public byte Hga2RHBiasLSBMid;
        public byte Hga2RHBiasMSBLow;
        public byte Hga2RHBiasMSBHigh;

        public byte Hga2R1BiasLSBLow;
        public byte Hga2R1BiasLSBMid;
        public byte Hga2R1BiasMSBLow;
        public byte Hga2R1BiasMSBHigh;

        public byte Hga2R2BiasLSBLow;
        public byte Hga2R2BiasLSBMid;
        public byte Hga2R2BiasMSBLow;
        public byte Hga2R2BiasMSBHigh;

        //

        //Hga3
        public byte Hga3WriterBiasLSBLow;
        public byte Hga3WriterBiasLSBMid;
        public byte Hga3WriterBiasMSBLow;
        public byte Hga3WriterBiasMSBHigh;

        public byte Hga3TABiasLSBLow;
        public byte Hga3TABiasLSBMid;
        public byte Hga3TABiasMSBLow;
        public byte Hga3TABiasMSBHigh;

        public byte Hga3WHBiasLSBLow;
        public byte Hga3WHBiasLSBMid;
        public byte Hga3WHBiasMSBLow;
        public byte Hga3WHBiasMSBHigh;

        public byte Hga3RHBiasLSBLow;
        public byte Hga3RHBiasLSBMid;
        public byte Hga3RHBiasMSBLow;
        public byte Hga3RHBiasMSBHigh;

        public byte Hga3R1BiasLSBLow;
        public byte Hga3R1BiasLSBMid;
        public byte Hga3R1BiasMSBLow;
        public byte Hga3R1BiasMSBHigh;

        public byte Hga3R2BiasLSBLow;
        public byte Hga3R2BiasLSBMid;
        public byte Hga3R2BiasMSBLow;
        public byte Hga3R2BiasMSBHigh;

        //

        //Hga4
        public byte Hga4WriterBiasLSBLow;
        public byte Hga4WriterBiasLSBMid;
        public byte Hga4WriterBiasMSBLow;
        public byte Hga4WriterBiasMSBHigh;

        public byte Hga4TABiasLSBLow;
        public byte Hga4TABiasLSBMid;
        public byte Hga4TABiasMSBLow;
        public byte Hga4TABiasMSBHigh;

        public byte Hga4WHBiasLSBLow;
        public byte Hga4WHBiasLSBMid;
        public byte Hga4WHBiasMSBLow;
        public byte Hga4WHBiasMSBHigh;

        public byte Hga4RHBiasLSBLow;
        public byte Hga4RHBiasLSBMid;
        public byte Hga4RHBiasMSBLow;
        public byte Hga4RHBiasMSBHigh;

        public byte Hga4R1BiasLSBLow;
        public byte Hga4R1BiasLSBMid;
        public byte Hga4R1BiasMSBLow;
        public byte Hga4R1BiasMSBHigh;

        public byte Hga4R2BiasLSBLow;
        public byte Hga4R2BiasLSBMid;
        public byte Hga4R2BiasMSBLow;
        public byte Hga4R2BiasMSBHigh;

        //

        //Hga5
        public byte Hga5WriterBiasLSBLow;
        public byte Hga5WriterBiasLSBMid;
        public byte Hga5WriterBiasMSBLow;
        public byte Hga5WriterBiasMSBHigh;

        public byte Hga5TABiasLSBLow;
        public byte Hga5TABiasLSBMid;
        public byte Hga5TABiasMSBLow;
        public byte Hga5TABiasMSBHigh;

        public byte Hga5WHBiasLSBLow;
        public byte Hga5WHBiasLSBMid;
        public byte Hga5WHBiasMSBLow;
        public byte Hga5WHBiasMSBHigh;

        public byte Hga5RHBiasLSBLow;
        public byte Hga5RHBiasLSBMid;
        public byte Hga5RHBiasMSBLow;
        public byte Hga5RHBiasMSBHigh;

        public byte Hga5R1BiasLSBLow;
        public byte Hga5R1BiasLSBMid;
        public byte Hga5R1BiasMSBLow;
        public byte Hga5R1BiasMSBHigh;

        public byte Hga5R2BiasLSBLow;
        public byte Hga5R2BiasLSBMid;
        public byte Hga5R2BiasMSBLow;
        public byte Hga5R2BiasMSBHigh;

        //

        //Hga6
        public byte Hga6WriterBiasLSBLow;
        public byte Hga6WriterBiasLSBMid;
        public byte Hga6WriterBiasMSBLow;
        public byte Hga6WriterBiasMSBHigh;

        public byte Hga6TABiasLSBLow;
        public byte Hga6TABiasLSBMid;
        public byte Hga6TABiasMSBLow;
        public byte Hga6TABiasMSBHigh;

        public byte Hga6WHBiasLSBLow;
        public byte Hga6WHBiasLSBMid;
        public byte Hga6WHBiasMSBLow;
        public byte Hga6WHBiasMSBHigh;

        public byte Hga6RHBiasLSBLow;
        public byte Hga6RHBiasLSBMid;
        public byte Hga6RHBiasMSBLow;
        public byte Hga6RHBiasMSBHigh;

        public byte Hga6R1BiasLSBLow;
        public byte Hga6R1BiasLSBMid;
        public byte Hga6R1BiasMSBLow;
        public byte Hga6R1BiasMSBHigh;

        public byte Hga6R2BiasLSBLow;
        public byte Hga6R2BiasLSBMid;
        public byte Hga6R2BiasMSBLow;
        public byte Hga6R2BiasMSBHigh;

        //

        //Hga7
        public byte Hga7WriterBiasLSBLow;
        public byte Hga7WriterBiasLSBMid;
        public byte Hga7WriterBiasMSBLow;
        public byte Hga7WriterBiasMSBHigh;

        public byte Hga7TABiasLSBLow;
        public byte Hga7TABiasLSBMid;
        public byte Hga7TABiasMSBLow;
        public byte Hga7TABiasMSBHigh;

        public byte Hga7WHBiasLSBLow;
        public byte Hga7WHBiasLSBMid;
        public byte Hga7WHBiasMSBLow;
        public byte Hga7WHBiasMSBHigh;

        public byte Hga7RHBiasLSBLow;
        public byte Hga7RHBiasLSBMid;
        public byte Hga7RHBiasMSBLow;
        public byte Hga7RHBiasMSBHigh;

        public byte Hga7R1BiasLSBLow;
        public byte Hga7R1BiasLSBMid;
        public byte Hga7R1BiasMSBLow;
        public byte Hga7R1BiasMSBHigh;

        public byte Hga7R2BiasLSBLow;
        public byte Hga7R2BiasLSBMid;
        public byte Hga7R2BiasMSBLow;
        public byte Hga7R2BiasMSBHigh;

        //

        //Hga8
        public byte Hga8WriterBiasLSBLow;
        public byte Hga8WriterBiasLSBMid;
        public byte Hga8WriterBiasMSBLow;
        public byte Hga8WriterBiasMSBHigh;

        public byte Hga8TABiasLSBLow;
        public byte Hga8TABiasLSBMid;
        public byte Hga8TABiasMSBLow;
        public byte Hga8TABiasMSBHigh;

        public byte Hga8WHBiasLSBLow;
        public byte Hga8WHBiasLSBMid;
        public byte Hga8WHBiasMSBLow;
        public byte Hga8WHBiasMSBHigh;

        public byte Hga8RHBiasLSBLow;
        public byte Hga8RHBiasLSBMid;
        public byte Hga8RHBiasMSBLow;
        public byte Hga8RHBiasMSBHigh;

        public byte Hga8R1BiasLSBLow;
        public byte Hga8R1BiasLSBMid;
        public byte Hga8R1BiasMSBLow;
        public byte Hga8R1BiasMSBHigh;

        public byte Hga8R2BiasLSBLow;
        public byte Hga8R2BiasLSBMid;
        public byte Hga8R2BiasMSBLow;
        public byte Hga8R2BiasMSBHigh;

        //

        //Hga9
        public byte Hga9WriterBiasLSBLow;
        public byte Hga9WriterBiasLSBMid;
        public byte Hga9WriterBiasMSBLow;
        public byte Hga9WriterBiasMSBHigh;

        public byte Hga9TABiasLSBLow;
        public byte Hga9TABiasLSBMid;
        public byte Hga9TABiasMSBLow;
        public byte Hga9TABiasMSBHigh;

        public byte Hga9WHBiasLSBLow;
        public byte Hga9WHBiasLSBMid;
        public byte Hga9WHBiasMSBLow;
        public byte Hga9WHBiasMSBHigh;

        public byte Hga9RHBiasLSBLow;
        public byte Hga9RHBiasLSBMid;
        public byte Hga9RHBiasMSBLow;
        public byte Hga9RHBiasMSBHigh;

        public byte Hga9R1BiasLSBLow;
        public byte Hga9R1BiasLSBMid;
        public byte Hga9R1BiasMSBLow;
        public byte Hga9R1BiasMSBHigh;

        public byte Hga9R2BiasLSBLow;
        public byte Hga9R2BiasLSBMid;
        public byte Hga9R2BiasMSBLow;
        public byte Hga9R2BiasMSBHigh;

        //

        //Hga10
        public byte Hga10WriterBiasLSBLow;
        public byte Hga10WriterBiasLSBMid;
        public byte Hga10WriterBiasMSBLow;
        public byte Hga10WriterBiasMSBHigh;

        public byte Hga10TABiasLSBLow;
        public byte Hga10TABiasLSBMid;
        public byte Hga10TABiasMSBLow;
        public byte Hga10TABiasMSBHigh;

        public byte Hga10WHBiasLSBLow;
        public byte Hga10WHBiasLSBMid;
        public byte Hga10WHBiasMSBLow;
        public byte Hga10WHBiasMSBHigh;

        public byte Hga10RHBiasLSBLow;
        public byte Hga10RHBiasLSBMid;
        public byte Hga10RHBiasMSBLow;
        public byte Hga10RHBiasMSBHigh;

        public byte Hga10R1BiasLSBLow;
        public byte Hga10R1BiasLSBMid;
        public byte Hga10R1BiasMSBLow;
        public byte Hga10R1BiasMSBHigh;

        public byte Hga10R2BiasLSBLow;
        public byte Hga10R2BiasLSBMid;
        public byte Hga10R2BiasMSBLow;
        public byte Hga10R2BiasMSBHigh;

        //

        public static TestProbe75GetAllBiasVoltage ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe75GetAllBiasVoltage*)pb;
            }
        }

        //Hga1
        public int Hga1WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga1WriterBiasLSBLow;
            array[1] = Hga1WriterBiasLSBMid;
            array[2] = Hga1WriterBiasMSBLow;
            array[3] = Hga1WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga1TABiasLSBLow;
            array[1] = Hga1TABiasLSBMid;
            array[2] = Hga1TABiasMSBLow;
            array[3] = Hga1TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga1WHBiasLSBLow;
            array[1] = Hga1WHBiasLSBMid;
            array[2] = Hga1WHBiasMSBLow;
            array[3] = Hga1WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga1RHBiasLSBLow;
            array[1] = Hga1RHBiasLSBMid;
            array[2] = Hga1RHBiasMSBLow;
            array[3] = Hga1RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga1R1BiasLSBLow;
            array[1] = Hga1R1BiasLSBMid;
            array[2] = Hga1R1BiasMSBLow;
            array[3] = Hga1R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga1R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga1R2BiasLSBLow;
            array[1] = Hga1R2BiasLSBMid;
            array[2] = Hga1R2BiasMSBLow;
            array[3] = Hga1R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga2
        public int Hga2WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga2WriterBiasLSBLow;
            array[1] = Hga2WriterBiasLSBMid;
            array[2] = Hga2WriterBiasMSBLow;
            array[3] = Hga2WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga2TABiasLSBLow;
            array[1] = Hga2TABiasLSBMid;
            array[2] = Hga2TABiasMSBLow;
            array[3] = Hga2TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga2WHBiasLSBLow;
            array[1] = Hga2WHBiasLSBMid;
            array[2] = Hga2WHBiasMSBLow;
            array[3] = Hga2WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga2RHBiasLSBLow;
            array[1] = Hga2RHBiasLSBMid;
            array[2] = Hga2RHBiasMSBLow;
            array[3] = Hga2RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga2R1BiasLSBLow;
            array[1] = Hga2R1BiasLSBMid;
            array[2] = Hga2R1BiasMSBLow;
            array[3] = Hga2R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga2R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga2R2BiasLSBLow;
            array[1] = Hga2R2BiasLSBMid;
            array[2] = Hga2R2BiasMSBLow;
            array[3] = Hga2R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //Hga3
        public int Hga3WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga3WriterBiasLSBLow;
            array[1] = Hga3WriterBiasLSBMid;
            array[2] = Hga3WriterBiasMSBLow;
            array[3] = Hga3WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga3TABiasLSBLow;
            array[1] = Hga3TABiasLSBMid;
            array[2] = Hga3TABiasMSBLow;
            array[3] = Hga3TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga3WHBiasLSBLow;
            array[1] = Hga3WHBiasLSBMid;
            array[2] = Hga3WHBiasMSBLow;
            array[3] = Hga3WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga3RHBiasLSBLow;
            array[1] = Hga3RHBiasLSBMid;
            array[2] = Hga3RHBiasMSBLow;
            array[3] = Hga3RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga3R1BiasLSBLow;
            array[1] = Hga3R1BiasLSBMid;
            array[2] = Hga3R1BiasMSBLow;
            array[3] = Hga3R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga3R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga3R2BiasLSBLow;
            array[1] = Hga3R2BiasLSBMid;
            array[2] = Hga3R2BiasMSBLow;
            array[3] = Hga3R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //Hga4
        public int Hga4WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga4WriterBiasLSBLow;
            array[1] = Hga4WriterBiasLSBMid;
            array[2] = Hga4WriterBiasMSBLow;
            array[3] = Hga4WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga4TABiasLSBLow;
            array[1] = Hga4TABiasLSBMid;
            array[2] = Hga4TABiasMSBLow;
            array[3] = Hga4TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga4WHBiasLSBLow;
            array[1] = Hga4WHBiasLSBMid;
            array[2] = Hga4WHBiasMSBLow;
            array[3] = Hga4WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga4RHBiasLSBLow;
            array[1] = Hga4RHBiasLSBMid;
            array[2] = Hga4RHBiasMSBLow;
            array[3] = Hga4RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga4R1BiasLSBLow;
            array[1] = Hga4R1BiasLSBMid;
            array[2] = Hga4R1BiasMSBLow;
            array[3] = Hga4R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga4R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga4R2BiasLSBLow;
            array[1] = Hga4R2BiasLSBMid;
            array[2] = Hga4R2BiasMSBLow;
            array[3] = Hga4R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga5
        public int Hga5WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga5WriterBiasLSBLow;
            array[1] = Hga5WriterBiasLSBMid;
            array[2] = Hga5WriterBiasMSBLow;
            array[3] = Hga5WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga5TABiasLSBLow;
            array[1] = Hga5TABiasLSBMid;
            array[2] = Hga5TABiasMSBLow;
            array[3] = Hga5TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga5WHBiasLSBLow;
            array[1] = Hga5WHBiasLSBMid;
            array[2] = Hga5WHBiasMSBLow;
            array[3] = Hga5WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga5RHBiasLSBLow;
            array[1] = Hga5RHBiasLSBMid;
            array[2] = Hga5RHBiasMSBLow;
            array[3] = Hga5RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga5R1BiasLSBLow;
            array[1] = Hga5R1BiasLSBMid;
            array[2] = Hga5R1BiasMSBLow;
            array[3] = Hga5R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga5R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga5R2BiasLSBLow;
            array[1] = Hga5R2BiasLSBMid;
            array[2] = Hga5R2BiasMSBLow;
            array[3] = Hga5R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga6
        public int Hga6WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga6WriterBiasLSBLow;
            array[1] = Hga6WriterBiasLSBMid;
            array[2] = Hga6WriterBiasMSBLow;
            array[3] = Hga6WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga6TABiasLSBLow;
            array[1] = Hga6TABiasLSBMid;
            array[2] = Hga6TABiasMSBLow;
            array[3] = Hga6TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga6WHBiasLSBLow;
            array[1] = Hga6WHBiasLSBMid;
            array[2] = Hga6WHBiasMSBLow;
            array[3] = Hga6WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga6RHBiasLSBLow;
            array[1] = Hga6RHBiasLSBMid;
            array[2] = Hga6RHBiasMSBLow;
            array[3] = Hga6RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga6R1BiasLSBLow;
            array[1] = Hga6R1BiasLSBMid;
            array[2] = Hga6R1BiasMSBLow;
            array[3] = Hga6R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga6R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga6R2BiasLSBLow;
            array[1] = Hga6R2BiasLSBMid;
            array[2] = Hga6R2BiasMSBLow;
            array[3] = Hga6R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga7
        public int Hga7WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga7WriterBiasLSBLow;
            array[1] = Hga7WriterBiasLSBMid;
            array[2] = Hga7WriterBiasMSBLow;
            array[3] = Hga7WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga7TABiasLSBLow;
            array[1] = Hga7TABiasLSBMid;
            array[2] = Hga7TABiasMSBLow;
            array[3] = Hga7TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga7WHBiasLSBLow;
            array[1] = Hga7WHBiasLSBMid;
            array[2] = Hga7WHBiasMSBLow;
            array[3] = Hga7WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga7RHBiasLSBLow;
            array[1] = Hga7RHBiasLSBMid;
            array[2] = Hga7RHBiasMSBLow;
            array[3] = Hga7RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga7R1BiasLSBLow;
            array[1] = Hga7R1BiasLSBMid;
            array[2] = Hga7R1BiasMSBLow;
            array[3] = Hga7R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga7R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga7R2BiasLSBLow;
            array[1] = Hga7R2BiasLSBMid;
            array[2] = Hga7R2BiasMSBLow;
            array[3] = Hga7R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga8
        public int Hga8WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga8WriterBiasLSBLow;
            array[1] = Hga8WriterBiasLSBMid;
            array[2] = Hga8WriterBiasMSBLow;
            array[3] = Hga8WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga8TABiasLSBLow;
            array[1] = Hga8TABiasLSBMid;
            array[2] = Hga8TABiasMSBLow;
            array[3] = Hga8TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga8WHBiasLSBLow;
            array[1] = Hga8WHBiasLSBMid;
            array[2] = Hga8WHBiasMSBLow;
            array[3] = Hga8WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga8RHBiasLSBLow;
            array[1] = Hga8RHBiasLSBMid;
            array[2] = Hga8RHBiasMSBLow;
            array[3] = Hga8RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga8R1BiasLSBLow;
            array[1] = Hga8R1BiasLSBMid;
            array[2] = Hga8R1BiasMSBLow;
            array[3] = Hga8R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga8R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga8R2BiasLSBLow;
            array[1] = Hga8R2BiasLSBMid;
            array[2] = Hga8R2BiasMSBLow;
            array[3] = Hga8R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga9
        public int Hga9WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga9WriterBiasLSBLow;
            array[1] = Hga9WriterBiasLSBMid;
            array[2] = Hga9WriterBiasMSBLow;
            array[3] = Hga9WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga9TABiasLSBLow;
            array[1] = Hga9TABiasLSBMid;
            array[2] = Hga9TABiasMSBLow;
            array[3] = Hga9TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga9WHBiasLSBLow;
            array[1] = Hga9WHBiasLSBMid;
            array[2] = Hga9WHBiasMSBLow;
            array[3] = Hga9WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga9RHBiasLSBLow;
            array[1] = Hga9RHBiasLSBMid;
            array[2] = Hga9RHBiasMSBLow;
            array[3] = Hga9RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga9R1BiasLSBLow;
            array[1] = Hga9R1BiasLSBMid;
            array[2] = Hga9R1BiasMSBLow;
            array[3] = Hga9R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga9R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga9R2BiasLSBLow;
            array[1] = Hga9R2BiasLSBMid;
            array[2] = Hga9R2BiasMSBLow;
            array[3] = Hga9R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }


        //Hga10
        public int Hga10WriterBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga10WriterBiasLSBLow;
            array[1] = Hga10WriterBiasLSBMid;
            array[2] = Hga10WriterBiasMSBLow;
            array[3] = Hga10WriterBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10TABias()
        {
            byte[] array = new byte[4];
            array[0] = Hga10TABiasLSBLow;
            array[1] = Hga10TABiasLSBMid;
            array[2] = Hga10TABiasMSBLow;
            array[3] = Hga10TABiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10WHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga10WHBiasLSBLow;
            array[1] = Hga10WHBiasLSBMid;
            array[2] = Hga10WHBiasMSBLow;
            array[3] = Hga10WHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10RHBias()
        {
            byte[] array = new byte[4];
            array[0] = Hga10RHBiasLSBLow;
            array[1] = Hga10RHBiasLSBMid;
            array[2] = Hga10RHBiasMSBLow;
            array[3] = Hga10RHBiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10R1Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga10R1BiasLSBLow;
            array[1] = Hga10R1BiasLSBMid;
            array[2] = Hga10R1BiasMSBLow;
            array[3] = Hga10R1BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Hga10R2Bias()
        {
            byte[] array = new byte[4];
            array[0] = Hga10R2BiasLSBLow;
            array[1] = Hga10R2BiasLSBMid;
            array[2] = Hga10R2BiasMSBLow;
            array[3] = Hga10R2BiasMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}

