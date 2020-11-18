using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe87GetLDUConfiguration_2
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte enable_ldu;
        //LED
        public byte LEDStartCurrentLSB;
        public byte LEDStartCurrentMSB;

        public byte LEDStopCurrentLSB;
        public byte LEDStopCurrentMSB;

        public byte LEDSteppingSizeLSB;
        public byte LEDSteppingSizeMSB;

        public byte LEDVoltageSampleSize;
        public byte LEDNumberOfSteps;
        //9
        //LDU

        public byte LDU4PtMode;

        public byte LDUStartCurrentLSB;
        public byte LDUStartCurrentMSB;

        public byte LDUStopCurrentLSB;
        public byte LDUStopCurrentMSB;

        public byte LDUSteppingSizeLSB;
        public byte LDUSteppingSizeMSB;

        public byte LDUVoltageSampleSize;
        public byte LDUNumberOfSteps;

        public byte LDUISweep1LSB;
        public byte LDUISweep1MSB;

        public byte LDUISweep2LSB;
        public byte LDUISweep2MSB;

        public byte LDUISweep3LSB;
        public byte LDUISweep3MSB;

        public byte LDUISweep4LSB;
        public byte LDUISweep4MSB;

        public byte timeIntervalLSB;
        public byte timeIntervalMSB;
        //19

        public static TestProbe87GetLDUConfiguration_2 ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe87GetLDUConfiguration_2*)pb;
            }
        }


      /*  public int GetLDUEnable()
        {
            byte[] array = new byte[4];
            array[0] = enable_ldu;
            array[1] = 0;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);


        }
        */
        public int GetLEDStartCurrent()
        {
            byte[] array = new byte[4];
            array[0] = LEDStartCurrentLSB;
            array[1] = LEDStartCurrentMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLEDStopCurrent()
        {
            byte[] array = new byte[4];
            array[0] = LEDStopCurrentLSB;
            array[1] = LEDStopCurrentMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLEDSteppingSize()
        {
            byte[] array = new byte[4];
            array[0] = LEDSteppingSizeLSB;
            array[1] = LEDSteppingSizeMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUStartCurrent()
        {
            byte[] array = new byte[4];
            array[0] = LDUStartCurrentLSB;
            array[1] = LDUStartCurrentMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUStopCurrent()
        {
            byte[] array = new byte[4];
            array[0] = LDUStopCurrentLSB;
            array[1] = LDUStopCurrentMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUSteppingSize()
        {
            byte[] array = new byte[4];
            array[0] = LDUSteppingSizeLSB;
            array[1] = LDUSteppingSizeMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUISweep1()
        {
            byte[] array = new byte[4];
            array[0] = LDUISweep1LSB;
            array[1] = LDUISweep1MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUISweep2()
        {
            byte[] array = new byte[4];
            array[0] = LDUISweep2LSB;
            array[1] = LDUISweep2MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUISweep3()
        {
            byte[] array = new byte[4];
            array[0] = LDUISweep3LSB;
            array[1] = LDUISweep3MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        //-----------------------------------------------

        public int GetLDUISweep4()
        {
            byte[] array = new byte[4];
            array[0] = LDUISweep4LSB;
            array[1] = LDUISweep4MSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetLDUtimeInterval()
        {
            byte[] array = new byte[4];
            array[0] = timeIntervalLSB;
            array[1] = timeIntervalMSB;
            array[2] = 0;
            array[3] = 0;
            return BitConverter.ToInt32(array, 0);

        }

    }

}
