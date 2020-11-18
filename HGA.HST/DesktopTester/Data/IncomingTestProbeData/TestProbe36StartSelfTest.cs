using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe36StartSelfTest
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // Ch1WriterResistance0Ohm
        public byte Ch1WriterResistance0OhmLSBLow;
        public byte Ch1WriterResistance0OhmLSBMid;
        public byte Ch1WriterResistance0OhmMSBLow;
        public byte Ch1WriterResistance0OhmMSBHigh;

        // Ch2TAResistance0Ohm
        public byte Ch2TAResistance0OhmLSBLow;
        public byte Ch2TAResistance0OhmLSBMid;
        public byte Ch2TAResistance0OhmMSBLow;
        public byte Ch2TAResistance0OhmMSBHigh;

        // Ch3WHResistance0Ohm
        public byte Ch3WHResistance0OhmLSBLow;
        public byte Ch3WHResistance0OhmLSBMid;
        public byte Ch3WHResistance0OhmMSBLow;
        public byte Ch3WHResistance0OhmMSBHigh;

        // Ch4RHResistance0Ohm
        public byte Ch4RHResistance0OhmLSBLow;
        public byte Ch4RHResistance0OhmLSBMid;
        public byte Ch4RHResistance0OhmMSBLow;
        public byte Ch4RHResistance0OhmMSBHigh;

        // Ch5R1Resistance0Ohm
        public byte Ch5R1Resistance0OhmLSBLow;
        public byte Ch5R1Resistance0OhmLSBMid;
        public byte Ch5R1Resistance0OhmMSBLow;
        public byte Ch5R1Resistance0OhmMSBHigh;

        // Ch6R2Resistance0Ohm
        public byte Ch6R2Resistance0OhmLSBLow;
        public byte Ch6R2Resistance0OhmLSBMid;
        public byte Ch6R2Resistance0OhmMSBLow;
        public byte Ch6R2Resistance0OhmMSBHigh;

        // Ch1WriterResistance10Ohm
        public byte Ch1WriterResistance10OhmLSBLow;
        public byte Ch1WriterResistance10OhmLSBMid;
        public byte Ch1WriterResistance10OhmMSBLow;
        public byte Ch1WriterResistance10OhmMSBHigh;

        // Ch2TAResistance10Ohm
        public byte Ch2TAResistance10OhmLSBLow;
        public byte Ch2TAResistance10OhmLSBMid;
        public byte Ch2TAResistance10OhmMSBLow;
        public byte Ch2TAResistance10OhmMSBHigh;

        // Ch3WHResistance10Ohm
        public byte Ch3WHResistance10OhmLSBLow;
        public byte Ch3WHResistance10OhmLSBMid;
        public byte Ch3WHResistance10OhmMSBLow;
        public byte Ch3WHResistance10OhmMSBHigh;

        // Ch4RHResistance10Ohm
        public byte Ch4RHResistance10OhmLSBLow;
        public byte Ch4RHResistance10OhmLSBMid;
        public byte Ch4RHResistance10OhmMSBLow;
        public byte Ch4RHResistance10OhmMSBHigh;

        // Ch5R1Resistance10Ohm
        public byte Ch5R1Resistance10OhmLSBLow;
        public byte Ch5R1Resistance10OhmLSBMid;
        public byte Ch5R1Resistance10OhmMSBLow;
        public byte Ch5R1Resistance10OhmMSBHigh;

        // Ch6R2Resistance10Ohm
        public byte Ch6R2Resistance10OhmLSBLow;
        public byte Ch6R2Resistance10OhmLSBMid;
        public byte Ch6R2Resistance10OhmMSBLow;
        public byte Ch6R2Resistance10OhmMSBHigh;

        // Ch1WriterResistance100Ohm
        public byte Ch1WriterResistance100OhmLSBLow;
        public byte Ch1WriterResistance100OhmLSBMid;
        public byte Ch1WriterResistance100OhmMSBLow;
        public byte Ch1WriterResistance100OhmMSBHigh;

        // Ch2TAResistance100Ohm
        public byte Ch2TAResistance100OhmLSBLow;
        public byte Ch2TAResistance100OhmLSBMid;
        public byte Ch2TAResistance100OhmMSBLow;
        public byte Ch2TAResistance100OhmMSBHigh;

        // Ch3WHResistance100Ohm
        public byte Ch3WHResistance100OhmLSBLow;
        public byte Ch3WHResistance100OhmLSBMid;
        public byte Ch3WHResistance100OhmMSBLow;
        public byte Ch3WHResistance100OhmMSBHigh;

        // Ch4RHResistance100Ohm
        public byte Ch4RHResistance100OhmLSBLow;
        public byte Ch4RHResistance100OhmLSBMid;
        public byte Ch4RHResistance100OhmMSBLow;
        public byte Ch4RHResistance100OhmMSBHigh;

        // Ch5R1Resistance100Ohm
        public byte Ch5R1Resistance100OhmLSBLow;
        public byte Ch5R1Resistance100OhmLSBMid;
        public byte Ch5R1Resistance100OhmMSBLow;
        public byte Ch5R1Resistance100OhmMSBHigh;

        // Ch6R2Resistance100Ohm
        public byte Ch6R2Resistance100OhmLSBLow;
        public byte Ch6R2Resistance100OhmLSBMid;
        public byte Ch6R2Resistance100OhmMSBLow;
        public byte Ch6R2Resistance100OhmMSBHigh;

        // Ch1WriterResistance500Ohm
        public byte Ch1WriterResistance500OhmLSBLow;
        public byte Ch1WriterResistance500OhmLSBMid;
        public byte Ch1WriterResistance500OhmMSBLow;
        public byte Ch1WriterResistance500OhmMSBHigh;

        // Ch2TAResistance500Ohm
        public byte Ch2TAResistance500OhmLSBLow;
        public byte Ch2TAResistance500OhmLSBMid;
        public byte Ch2TAResistance500OhmMSBLow;
        public byte Ch2TAResistance500OhmMSBHigh;

        // Ch3WHResistance500Ohm
        public byte Ch3WHResistance500OhmLSBLow;
        public byte Ch3WHResistance500OhmLSBMid;
        public byte Ch3WHResistance500OhmMSBLow;
        public byte Ch3WHResistance500OhmMSBHigh;

        // Ch4RHResistance500Ohm
        public byte Ch4RHResistance500OhmLSBLow;
        public byte Ch4RHResistance500OhmLSBMid;
        public byte Ch4RHResistance500OhmMSBLow;
        public byte Ch4RHResistance500OhmMSBHigh;

        // Ch5R1Resistance500Ohm
        public byte Ch5R1Resistance500OhmLSBLow;
        public byte Ch5R1Resistance500OhmLSBMid;
        public byte Ch5R1Resistance500OhmMSBLow;
        public byte Ch5R1Resistance500OhmMSBHigh;

        // Ch6R2Resistance500Ohm
        public byte Ch6R2Resistance500OhmLSBLow;
        public byte Ch6R2Resistance500OhmLSBMid;
        public byte Ch6R2Resistance500OhmMSBLow;
        public byte Ch6R2Resistance500OhmMSBHigh;

        // Ch1WriterResistance1000Ohm
        public byte Ch1WriterResistance1000OhmLSBLow;
        public byte Ch1WriterResistance1000OhmLSBMid;
        public byte Ch1WriterResistance1000OhmMSBLow;
        public byte Ch1WriterResistance1000OhmMSBHigh;

        // Ch2TAResistance1000Ohm
        public byte Ch2TAResistance1000OhmLSBLow;
        public byte Ch2TAResistance1000OhmLSBMid;
        public byte Ch2TAResistance1000OhmMSBLow;
        public byte Ch2TAResistance1000OhmMSBHigh;

        // Ch3WHResistance1000Ohm
        public byte Ch3WHResistance1000OhmLSBLow;
        public byte Ch3WHResistance1000OhmLSBMid;
        public byte Ch3WHResistance1000OhmMSBLow;
        public byte Ch3WHResistance1000OhmMSBHigh;

        // Ch4RHResistance1000Ohm
        public byte Ch4RHResistance1000OhmLSBLow;
        public byte Ch4RHResistance1000OhmLSBMid;
        public byte Ch4RHResistance1000OhmMSBLow;
        public byte Ch4RHResistance1000OhmMSBHigh;

        // Ch5R1Resistance1000Ohm
        public byte Ch5R1Resistance1000OhmLSBLow;
        public byte Ch5R1Resistance1000OhmLSBMid;
        public byte Ch5R1Resistance1000OhmMSBLow;
        public byte Ch5R1Resistance1000OhmMSBHigh;

        // Ch6R2Resistance1000Ohm
        public byte Ch6R2Resistance1000OhmLSBLow;
        public byte Ch6R2Resistance1000OhmLSBMid;
        public byte Ch6R2Resistance1000OhmMSBLow;
        public byte Ch6R2Resistance1000OhmMSBHigh;

        // Ch1WriterResistance10000Ohm
        public byte Ch1WriterResistance10000OhmLSBLow;
        public byte Ch1WriterResistance10000OhmLSBMid;
        public byte Ch1WriterResistance10000OhmMSBLow;
        public byte Ch1WriterResistance10000OhmMSBHigh;

        // Ch2TAResistance10000Ohm
        public byte Ch2TAResistance10000OhmLSBLow;
        public byte Ch2TAResistance10000OhmLSBMid;
        public byte Ch2TAResistance10000OhmMSBLow;
        public byte Ch2TAResistance10000OhmMSBHigh;

        // Ch3WHResistance10000Ohm
        public byte Ch3WHResistance10000OhmLSBLow;
        public byte Ch3WHResistance10000OhmLSBMid;
        public byte Ch3WHResistance10000OhmMSBLow;
        public byte Ch3WHResistance10000OhmMSBHigh;

        // Ch4RHResistance10000Ohm
        public byte Ch4RHResistance10000OhmLSBLow;
        public byte Ch4RHResistance10000OhmLSBMid;
        public byte Ch4RHResistance10000OhmMSBLow;
        public byte Ch4RHResistance10000OhmMSBHigh;

        // Ch5R1Resistance10000Ohm
        public byte Ch5R1Resistance10000OhmLSBLow;
        public byte Ch5R1Resistance10000OhmLSBMid;
        public byte Ch5R1Resistance10000OhmMSBLow;
        public byte Ch5R1Resistance10000OhmMSBHigh;

        // Ch6R2Resistance10000Ohm
        public byte Ch6R2Resistance10000OhmLSBLow;
        public byte Ch6R2Resistance10000OhmLSBMid;
        public byte Ch6R2Resistance10000OhmMSBLow;
        public byte Ch6R2Resistance10000OhmMSBHigh;

        // Capacitance100pF
        public byte Capacitance100pFLSBLow;
        public byte Capacitance100pFLSBMid;
        public byte Capacitance100pFMSBLow;
        public byte Capacitance100pFMSBHigh;

        // Capacitance270pF
        public byte Capacitance270pFLSBLow;
        public byte Capacitance270pFLSBMid;
        public byte Capacitance270pFMSBLow;
        public byte Capacitance270pFMSBHigh;

        // Capacitance470pF
        public byte Capacitance470pFLSBLow;
        public byte Capacitance470pFLSBMid;
        public byte Capacitance470pFMSBLow;
        public byte Capacitance470pFMSBHigh;

        // Capacitance680pF
        public byte Capacitance680pFLSBLow;
        public byte Capacitance680pFLSBMid;
        public byte Capacitance680pFMSBLow;
        public byte Capacitance680pFMSBHigh;

        // Capacitance820pF
        public byte Capacitance820pFLSBLow;
        public byte Capacitance820pFLSBMid;
        public byte Capacitance820pFMSBLow;
        public byte Capacitance820pFMSBHigh;

        // Capacitance10nF
        public byte Capacitance10nFLSBLow;
        public byte Capacitance10nFLSBMid;
        public byte Capacitance10nFMSBLow;
        public byte Capacitance10nFMSBHigh;


        public static TestProbe36StartSelfTest ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe36StartSelfTest*)pb;
            }
        }

        // Ch1WriterResistance0Ohm
        public int Ch1WriterResistance0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1WriterResistance0OhmLSBLow;
            array[1] = Ch1WriterResistance0OhmLSBMid;
            array[2] = Ch1WriterResistance0OhmMSBLow;
            array[3] = Ch1WriterResistance0OhmMSBHigh;    
            return BitConverter.ToInt32(array, 0);
        }

        // Ch2TAResistance0Ohm
        public int Ch2TAResistance0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TAResistance0OhmLSBLow;
            array[1] = Ch2TAResistance0OhmLSBMid;
            array[2] = Ch2TAResistance0OhmMSBLow;
            array[3] = Ch2TAResistance0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch3WHResistance0Ohm
        public int Ch3WHResistance0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WHResistance0OhmLSBLow;
            array[1] = Ch3WHResistance0OhmLSBMid;
            array[2] = Ch3WHResistance0OhmMSBLow;
            array[3] = Ch3WHResistance0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch4RHResistance0Ohm
        public int Ch4RHResistance0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RHResistance0OhmLSBLow;
            array[1] = Ch4RHResistance0OhmLSBMid;
            array[2] = Ch4RHResistance0OhmMSBLow;
            array[3] = Ch4RHResistance0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch5R1Resistance0Ohm
        public int Ch5R1Resistance0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1Resistance0OhmLSBLow;
            array[1] = Ch5R1Resistance0OhmLSBMid;
            array[2] = Ch5R1Resistance0OhmMSBLow;
            array[3] = Ch5R1Resistance0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch6R2Resistance0Ohm
        public int Ch6R2Resistance0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2Resistance0OhmLSBLow;
            array[1] = Ch6R2Resistance0OhmLSBMid;
            array[2] = Ch6R2Resistance0OhmMSBLow;
            array[3] = Ch6R2Resistance0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch1WriterResistance10Ohm
        public int Ch1WriterResistance10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1WriterResistance10OhmLSBLow;
            array[1] = Ch1WriterResistance10OhmLSBMid;
            array[2] = Ch1WriterResistance10OhmMSBLow;
            array[3] = Ch1WriterResistance10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch2TAResistance10Ohm
        public int Ch2TAResistance10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TAResistance10OhmLSBLow;
            array[1] = Ch2TAResistance10OhmLSBMid;
            array[2] = Ch2TAResistance10OhmMSBLow;
            array[3] = Ch2TAResistance10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch3WHResistance10Ohm
        public int Ch3WHResistance10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WHResistance10OhmLSBLow;
            array[1] = Ch3WHResistance10OhmLSBMid;
            array[2] = Ch3WHResistance10OhmMSBLow;
            array[3] = Ch3WHResistance10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch4RHResistance10Ohm
        public int Ch4RHResistance10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RHResistance10OhmLSBLow;
            array[1] = Ch4RHResistance10OhmLSBMid;
            array[2] = Ch4RHResistance10OhmMSBLow;
            array[3] = Ch4RHResistance10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch5R1Resistance10Ohm
        public int Ch5R1Resistance10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1Resistance10OhmLSBLow;
            array[1] = Ch5R1Resistance10OhmLSBMid;
            array[2] = Ch5R1Resistance10OhmMSBLow;
            array[3] = Ch5R1Resistance10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch6R2Resistance10Ohm
        public int Ch6R2Resistance10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2Resistance10OhmLSBLow;
            array[1] = Ch6R2Resistance10OhmLSBMid;
            array[2] = Ch6R2Resistance10OhmMSBLow;
            array[3] = Ch6R2Resistance10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch1WriterResistance100Ohm
        public int Ch1WriterResistance100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1WriterResistance100OhmLSBLow;
            array[1] = Ch1WriterResistance100OhmLSBMid;
            array[2] = Ch1WriterResistance100OhmMSBLow;
            array[3] = Ch1WriterResistance100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch2TAResistance100Ohm
        public int Ch2TAResistance100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TAResistance100OhmLSBLow;
            array[1] = Ch2TAResistance100OhmLSBMid;
            array[2] = Ch2TAResistance100OhmMSBLow;
            array[3] = Ch2TAResistance100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch3WHResistance100Ohm
        public int Ch3WHResistance100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WHResistance100OhmLSBLow;
            array[1] = Ch3WHResistance100OhmLSBMid;
            array[2] = Ch3WHResistance100OhmMSBLow;
            array[3] = Ch3WHResistance100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch4RHResistance100Ohm
        public int Ch4RHResistance100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RHResistance100OhmLSBLow;
            array[1] = Ch4RHResistance100OhmLSBMid;
            array[2] = Ch4RHResistance100OhmMSBLow;
            array[3] = Ch4RHResistance100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch5R1Resistance100Ohm
        public int Ch5R1Resistance100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1Resistance100OhmLSBLow;
            array[1] = Ch5R1Resistance100OhmLSBMid;
            array[2] = Ch5R1Resistance100OhmMSBLow;
            array[3] = Ch5R1Resistance100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch6R2Resistance100Ohm
        public int Ch6R2Resistance100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2Resistance100OhmLSBLow;
            array[1] = Ch6R2Resistance100OhmLSBMid;
            array[2] = Ch6R2Resistance100OhmMSBLow;
            array[3] = Ch6R2Resistance100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch1WriterResistance500Ohm
        public int Ch1WriterResistance500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1WriterResistance500OhmLSBLow;
            array[1] = Ch1WriterResistance500OhmLSBMid;
            array[2] = Ch1WriterResistance500OhmMSBLow;
            array[3] = Ch1WriterResistance500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch2TAResistance500Ohm
        public int Ch2TAResistance500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TAResistance500OhmLSBLow;
            array[1] = Ch2TAResistance500OhmLSBMid;
            array[2] = Ch2TAResistance500OhmMSBLow;
            array[3] = Ch2TAResistance500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch3WHResistance500Ohm
        public int Ch3WHResistance500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WHResistance500OhmLSBLow;
            array[1] = Ch3WHResistance500OhmLSBMid;
            array[2] = Ch3WHResistance500OhmMSBLow;
            array[3] = Ch3WHResistance500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch4RHResistance500Ohm
        public int Ch4RHResistance500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RHResistance500OhmLSBLow;
            array[1] = Ch4RHResistance500OhmLSBMid;
            array[2] = Ch4RHResistance500OhmMSBLow;
            array[3] = Ch4RHResistance500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch5R1Resistance500Ohm
        public int Ch5R1Resistance500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1Resistance500OhmLSBLow;
            array[1] = Ch5R1Resistance500OhmLSBMid;
            array[2] = Ch5R1Resistance500OhmMSBLow;
            array[3] = Ch5R1Resistance500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch6R2Resistance500Ohm
        public int Ch6R2Resistance500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2Resistance500OhmLSBLow;
            array[1] = Ch6R2Resistance500OhmLSBMid;
            array[2] = Ch6R2Resistance500OhmMSBLow;
            array[3] = Ch6R2Resistance500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch1WriterResistance1000Ohm
        public int Ch1WriterResistance1000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1WriterResistance1000OhmLSBLow;
            array[1] = Ch1WriterResistance1000OhmLSBMid;
            array[2] = Ch1WriterResistance1000OhmMSBLow;
            array[3] = Ch1WriterResistance1000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch2TAResistance1000Ohm
        public int Ch2TAResistance1000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TAResistance1000OhmLSBLow;
            array[1] = Ch2TAResistance1000OhmLSBMid;
            array[2] = Ch2TAResistance1000OhmMSBLow;
            array[3] = Ch2TAResistance1000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch3WHResistance1000Ohm
        public int Ch3WHResistance1000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WHResistance1000OhmLSBLow;
            array[1] = Ch3WHResistance1000OhmLSBMid;
            array[2] = Ch3WHResistance1000OhmMSBLow;
            array[3] = Ch3WHResistance1000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch4RHResistance1000Ohm
        public int Ch4RHResistance1000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RHResistance1000OhmLSBLow;
            array[1] = Ch4RHResistance1000OhmLSBMid;
            array[2] = Ch4RHResistance1000OhmMSBLow;
            array[3] = Ch4RHResistance1000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch5R1Resistance1000Ohm
        public int Ch5R1Resistance1000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1Resistance1000OhmLSBLow;
            array[1] = Ch5R1Resistance1000OhmLSBMid;
            array[2] = Ch5R1Resistance1000OhmMSBLow;
            array[3] = Ch5R1Resistance1000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch6R2Resistance1000Ohm
        public int Ch6R2Resistance1000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2Resistance1000OhmLSBLow;
            array[1] = Ch6R2Resistance1000OhmLSBMid;
            array[2] = Ch6R2Resistance1000OhmMSBLow;
            array[3] = Ch6R2Resistance1000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch1WriterResistance10000Ohm
        public int Ch1WriterResistance10000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1WriterResistance10000OhmLSBLow;
            array[1] = Ch1WriterResistance10000OhmLSBMid;
            array[2] = Ch1WriterResistance10000OhmMSBLow;
            array[3] = Ch1WriterResistance10000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch2TAResistance10000Ohm
        public int Ch2TAResistance10000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TAResistance10000OhmLSBLow;
            array[1] = Ch2TAResistance10000OhmLSBMid;
            array[2] = Ch2TAResistance10000OhmMSBLow;
            array[3] = Ch2TAResistance10000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch3WHResistance10000Ohm
        public int Ch3WHResistance10000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WHResistance10000OhmLSBLow;
            array[1] = Ch3WHResistance10000OhmLSBMid;
            array[2] = Ch3WHResistance10000OhmMSBLow;
            array[3] = Ch3WHResistance10000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch4RHResistance10000Ohm
        public int Ch4RHResistance10000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RHResistance10000OhmLSBLow;
            array[1] = Ch4RHResistance10000OhmLSBMid;
            array[2] = Ch4RHResistance10000OhmMSBLow;
            array[3] = Ch4RHResistance10000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch5R1Resistance10000Ohm
        public int Ch5R1Resistance10000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1Resistance10000OhmLSBLow;
            array[1] = Ch5R1Resistance10000OhmLSBMid;
            array[2] = Ch5R1Resistance10000OhmMSBLow;
            array[3] = Ch5R1Resistance10000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Ch6R2Resistance10000Ohm
        public int Ch6R2Resistance10000Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2Resistance10000OhmLSBLow;
            array[1] = Ch6R2Resistance10000OhmLSBMid;
            array[2] = Ch6R2Resistance10000OhmMSBLow;
            array[3] = Ch6R2Resistance10000OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Capacitance100pF
        public int Capacitance100pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance100pFLSBLow;
            array[1] = Capacitance100pFLSBMid;
            array[2] = Capacitance100pFMSBLow;
            array[3] = Capacitance100pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Capacitance270pF
        public int Capacitance270pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance270pFLSBLow;
            array[1] = Capacitance270pFLSBMid;
            array[2] = Capacitance270pFMSBLow;
            array[3] = Capacitance270pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Capacitance470pF
        public int Capacitance470pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance470pFLSBLow;
            array[1] = Capacitance470pFLSBMid;
            array[2] = Capacitance470pFMSBLow;
            array[3] = Capacitance470pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Capacitance680pF
        public int Capacitance680pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance680pFLSBLow;
            array[1] = Capacitance680pFLSBMid;
            array[2] = Capacitance680pFMSBLow;
            array[3] = Capacitance680pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Capacitance820pF
        public int Capacitance820pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance820pFLSBLow;
            array[1] = Capacitance820pFLSBMid;
            array[2] = Capacitance820pFMSBLow;
            array[3] = Capacitance820pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Capacitance10nF
        public int Capacitance10nF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance10nFLSBLow;
            array[1] = Capacitance10nFLSBMid;
            array[2] = Capacitance10nFMSBLow;
            array[3] = Capacitance10nFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }  
    }
}
