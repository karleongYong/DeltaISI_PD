using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe67GetCurrentRatio
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;


        public byte CurrentRatioCH1LSB;
        public byte CurrentRatioCH1MSB;

        public byte CurrentRatioCH2LSB;
        public byte CurrentRatioCH2MSB;

        public byte CurrentRatioCH3LSB;
        public byte CurrentRatioCH3MSB;

        public byte CurrentRatioCH4LSB;
        public byte CurrentRatioCH4MSB;

        public byte CurrentRatioCH5LSB;
        public byte CurrentRatioCH5MSB;

        public byte CurrentRatioCH6LSB;
        public byte CurrentRatioCH6MSB;



        public static TestProbe67GetCurrentRatio ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe67GetCurrentRatio*)pb;
            }
        }

        
        public int GetCurrentRatio_CH1()
        {
            byte[] array = new byte[4];
            array[0] = CurrentRatioCH1LSB;
            array[1] = CurrentRatioCH1MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }


        public int GetCurrentRatio_CH2()
        {
            byte[] array = new byte[4];
            array[0] = CurrentRatioCH2LSB;
            array[1] = CurrentRatioCH2MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }


        public int GetCurrentRatio_CH3()
        {
            byte[] array = new byte[4];
            array[0] = CurrentRatioCH3LSB;
            array[1] = CurrentRatioCH3MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }


        public int GetCurrentRatio_CH4()
        {
            byte[] array = new byte[4];
            array[0] = CurrentRatioCH4LSB;
            array[1] = CurrentRatioCH4MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }


        public int GetCurrentRatio_CH5()
        {
            byte[] array = new byte[4];
            array[0] = CurrentRatioCH5LSB;
            array[1] = CurrentRatioCH5MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }


        public int GetCurrentRatio_CH6()
        {
            byte[] array = new byte[4];
            array[0] = CurrentRatioCH6LSB;
            array[1] = CurrentRatioCH6MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }
    }
}