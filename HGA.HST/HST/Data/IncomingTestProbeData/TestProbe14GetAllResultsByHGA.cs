using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe14GetAllResultsByHGA
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        public byte WPlusStatus;
        public byte WMinusStatus;
        public byte TAPlusStatus;
        public byte TAMinusStatus;
        public byte WHPlusStatus;
        public byte WHMinusStatus;
        public byte RHPlusStatus;
        public byte RHMinusStatus;
        public byte R1PlusStatus;
        public byte R1MinusStatus;
        public byte R2PlusStatus;
        public byte R2MinusStatus;

        public byte WriterResistanceLSBLow;
        public byte WriterResistanceLSBMid;
        public byte WriterResistanceMSBLow;
        public byte WriterResistanceMSBHigh;

        public byte TAResistanceLSBLow;
        public byte TAResistanceLSBMid;
        public byte TAResistanceMSBLow;
        public byte TAResistanceMSBHigh;

        public byte WHResistanceLSBLow;
        public byte WHResistanceLSBMid;
        public byte WHResistanceMSBLow;
        public byte WHResistanceMSBHigh;

        public byte RHResistanceLSBLow;
        public byte RHResistanceLSBMid;
        public byte RHResistanceMSBLow;
        public byte RHResistanceMSBHigh;

        public byte R1ResistanceLSBLow;
        public byte R1ResistanceLSBMid;
        public byte R1ResistanceMSBLow;
        public byte R1ResistanceMSBHigh;

        public byte R2ResistanceLSBLow;
        public byte R2ResistanceLSBMid;
        public byte R2ResistanceMSBLow;
        public byte R2ResistanceMSBHigh;

        public byte Capacitance1LSBLow;
        public byte Capacitance1LSBMid;
        public byte Capacitance1MSBLow;
        public byte Capacitance1MSBHigh;

        public byte Capacitance2LSBLow;
        public byte Capacitance2LSBMid;
        public byte Capacitance2MSBLow;
        public byte Capacitance2MSBHigh;

        public static TestProbe14GetAllResultsByHGA ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe14GetAllResultsByHGA*)pb;
            }
        }

        public int WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = WriterResistanceLSBLow;
            array[1] = WriterResistanceLSBMid;
            array[2] = WriterResistanceMSBLow;
            array[3] = WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = TAResistanceLSBLow;
            array[1] = TAResistanceLSBMid;
            array[2] = TAResistanceMSBLow;
            array[3] = TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = WHResistanceLSBLow;
            array[1] = WHResistanceLSBMid;
            array[2] = WHResistanceMSBLow;
            array[3] = WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = RHResistanceLSBLow;
            array[1] = RHResistanceLSBMid;
            array[2] = RHResistanceMSBLow;
            array[3] = RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = R1ResistanceLSBLow;
            array[1] = R1ResistanceLSBMid;
            array[2] = R1ResistanceMSBLow;
            array[3] = R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = R2ResistanceLSBLow;
            array[1] = R2ResistanceLSBMid;
            array[2] = R2ResistanceMSBLow;
            array[3] = R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance1LSBLow;
            array[1] = Capacitance1LSBMid;
            array[2] = Capacitance1MSBLow;
            array[3] = Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance2LSBLow;
            array[1] = Capacitance2LSBMid;
            array[2] = Capacitance2MSBLow;
            array[3] = Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
