using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe53GetPrecisorCapacitanceCompensation
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
                        
        public byte EnableFlag;   

        // HGA1 CH1
        public byte HGA1Ch1CompensationLSBLow;
        public byte HGA1Ch1CompensationLSBMid;
        public byte HGA1Ch1CompensationMSBLow;
        public byte HGA1Ch1CompensationMSBHigh;

        // HGA1 CH2
        public byte HGA1Ch2CompensationLSBLow;
        public byte HGA1Ch2CompensationLSBMid;
        public byte HGA1Ch2CompensationMSBLow;
        public byte HGA1Ch2CompensationMSBHigh;

        // HGA2 CH1
        public byte HGA2Ch1CompensationLSBLow;
        public byte HGA2Ch1CompensationLSBMid;
        public byte HGA2Ch1CompensationMSBLow;
        public byte HGA2Ch1CompensationMSBHigh;

        // HGA2 CH2
        public byte HGA2Ch2CompensationLSBLow;
        public byte HGA2Ch2CompensationLSBMid;
        public byte HGA2Ch2CompensationMSBLow;
        public byte HGA2Ch2CompensationMSBHigh;

        // HGA3 CH1
        public byte HGA3Ch1CompensationLSBLow;
        public byte HGA3Ch1CompensationLSBMid;
        public byte HGA3Ch1CompensationMSBLow;
        public byte HGA3Ch1CompensationMSBHigh;

        // HGA3 CH2
        public byte HGA3Ch2CompensationLSBLow;
        public byte HGA3Ch2CompensationLSBMid;
        public byte HGA3Ch2CompensationMSBLow;
        public byte HGA3Ch2CompensationMSBHigh;

        // HGA4 CH1
        public byte HGA4Ch1CompensationLSBLow;
        public byte HGA4Ch1CompensationLSBMid;
        public byte HGA4Ch1CompensationMSBLow;
        public byte HGA4Ch1CompensationMSBHigh;

        // HGA4 CH2
        public byte HGA4Ch2CompensationLSBLow;
        public byte HGA4Ch2CompensationLSBMid;
        public byte HGA4Ch2CompensationMSBLow;
        public byte HGA4Ch2CompensationMSBHigh;

        // HGA5 CH1
        public byte HGA5Ch1CompensationLSBLow;
        public byte HGA5Ch1CompensationLSBMid;
        public byte HGA5Ch1CompensationMSBLow;
        public byte HGA5Ch1CompensationMSBHigh;

        // HGA5 CH2
        public byte HGA5Ch2CompensationLSBLow;
        public byte HGA5Ch2CompensationLSBMid;
        public byte HGA5Ch2CompensationMSBLow;
        public byte HGA5Ch2CompensationMSBHigh;

        // HGA6 CH1
        public byte HGA6Ch1CompensationLSBLow;
        public byte HGA6Ch1CompensationLSBMid;
        public byte HGA6Ch1CompensationMSBLow;
        public byte HGA6Ch1CompensationMSBHigh;

        // HGA6 CH2
        public byte HGA6Ch2CompensationLSBLow;
        public byte HGA6Ch2CompensationLSBMid;
        public byte HGA6Ch2CompensationMSBLow;
        public byte HGA6Ch2CompensationMSBHigh;

        // HGA7 CH1
        public byte HGA7Ch1CompensationLSBLow;
        public byte HGA7Ch1CompensationLSBMid;
        public byte HGA7Ch1CompensationMSBLow;
        public byte HGA7Ch1CompensationMSBHigh;

        // HGA7 CH2
        public byte HGA7Ch2CompensationLSBLow;
        public byte HGA7Ch2CompensationLSBMid;
        public byte HGA7Ch2CompensationMSBLow;
        public byte HGA7Ch2CompensationMSBHigh;

        // HGA8 CH1
        public byte HGA8Ch1CompensationLSBLow;
        public byte HGA8Ch1CompensationLSBMid;
        public byte HGA8Ch1CompensationMSBLow;
        public byte HGA8Ch1CompensationMSBHigh;

        // HGA8 CH2
        public byte HGA8Ch2CompensationLSBLow;
        public byte HGA8Ch2CompensationLSBMid;
        public byte HGA8Ch2CompensationMSBLow;
        public byte HGA8Ch2CompensationMSBHigh;

        // HGA9 CH1
        public byte HGA9Ch1CompensationLSBLow;
        public byte HGA9Ch1CompensationLSBMid;
        public byte HGA9Ch1CompensationMSBLow;
        public byte HGA9Ch1CompensationMSBHigh;

        // HGA9 CH2
        public byte HGA9Ch2CompensationLSBLow;
        public byte HGA9Ch2CompensationLSBMid;
        public byte HGA9Ch2CompensationMSBLow;
        public byte HGA9Ch2CompensationMSBHigh;

        // HGA10 CH1
        public byte HGA10Ch1CompensationLSBLow;
        public byte HGA10Ch1CompensationLSBMid;
        public byte HGA10Ch1CompensationMSBLow;
        public byte HGA10Ch1CompensationMSBHigh;

        // HGA10 CH2
        public byte HGA10Ch2CompensationLSBLow;
        public byte HGA10Ch2CompensationLSBMid;
        public byte HGA10Ch2CompensationMSBLow;
        public byte HGA10Ch2CompensationMSBHigh;

        public static TestProbe53GetPrecisorCapacitanceCompensation ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe53GetPrecisorCapacitanceCompensation*)pb;
            }
        }

        // HGA1 CH1
        public int HGA1Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch1CompensationLSBLow;
            array[1] = HGA1Ch1CompensationLSBMid;
            array[2] = HGA1Ch1CompensationMSBLow;
            array[3] = HGA1Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA1 CH2
        public int HGA1Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch2CompensationLSBLow;
            array[1] = HGA1Ch2CompensationLSBMid;
            array[2] = HGA1Ch2CompensationMSBLow;
            array[3] = HGA1Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2 CH1
        public int HGA2Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch1CompensationLSBLow;
            array[1] = HGA2Ch1CompensationLSBMid;
            array[2] = HGA2Ch1CompensationMSBLow;
            array[3] = HGA2Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2 CH2
        public int HGA2Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch2CompensationLSBLow;
            array[1] = HGA2Ch2CompensationLSBMid;
            array[2] = HGA2Ch2CompensationMSBLow;
            array[3] = HGA2Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3 CH1
        public int HGA3Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch1CompensationLSBLow;
            array[1] = HGA3Ch1CompensationLSBMid;
            array[2] = HGA3Ch1CompensationMSBLow;
            array[3] = HGA3Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3 CH2
        public int HGA3Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch2CompensationLSBLow;
            array[1] = HGA3Ch2CompensationLSBMid;
            array[2] = HGA3Ch2CompensationMSBLow;
            array[3] = HGA3Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4 CH1
        public int HGA4Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch1CompensationLSBLow;
            array[1] = HGA4Ch1CompensationLSBMid;
            array[2] = HGA4Ch1CompensationMSBLow;
            array[3] = HGA4Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4 CH2
        public int HGA4Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch2CompensationLSBLow;
            array[1] = HGA4Ch2CompensationLSBMid;
            array[2] = HGA4Ch2CompensationMSBLow;
            array[3] = HGA4Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5 CH1
        public int HGA5Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch1CompensationLSBLow;
            array[1] = HGA5Ch1CompensationLSBMid;
            array[2] = HGA5Ch1CompensationMSBLow;
            array[3] = HGA5Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5 CH2
        public int HGA5Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch2CompensationLSBLow;
            array[1] = HGA5Ch2CompensationLSBMid;
            array[2] = HGA5Ch2CompensationMSBLow;
            array[3] = HGA5Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6 CH1
        public int HGA6Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch1CompensationLSBLow;
            array[1] = HGA6Ch1CompensationLSBMid;
            array[2] = HGA6Ch1CompensationMSBLow;
            array[3] = HGA6Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6 CH2
        public int HGA6Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch2CompensationLSBLow;
            array[1] = HGA6Ch2CompensationLSBMid;
            array[2] = HGA6Ch2CompensationMSBLow;
            array[3] = HGA6Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7 CH1
        public int HGA7Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch1CompensationLSBLow;
            array[1] = HGA7Ch1CompensationLSBMid;
            array[2] = HGA7Ch1CompensationMSBLow;
            array[3] = HGA7Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7 CH2
        public int HGA7Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch2CompensationLSBLow;
            array[1] = HGA7Ch2CompensationLSBMid;
            array[2] = HGA7Ch2CompensationMSBLow;
            array[3] = HGA7Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8 CH1
        public int HGA8Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch1CompensationLSBLow;
            array[1] = HGA8Ch1CompensationLSBMid;
            array[2] = HGA8Ch1CompensationMSBLow;
            array[3] = HGA8Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8 CH2
        public int HGA8Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch2CompensationLSBLow;
            array[1] = HGA8Ch2CompensationLSBMid;
            array[2] = HGA8Ch2CompensationMSBLow;
            array[3] = HGA8Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9 CH1
        public int HGA9Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch1CompensationLSBLow;
            array[1] = HGA9Ch1CompensationLSBMid;
            array[2] = HGA9Ch1CompensationMSBLow;
            array[3] = HGA9Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9 CH2
        public int HGA9Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch2CompensationLSBLow;
            array[1] = HGA9Ch2CompensationLSBMid;
            array[2] = HGA9Ch2CompensationMSBLow;
            array[3] = HGA9Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10 CH1
        public int HGA10Ch1Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch1CompensationLSBLow;
            array[1] = HGA10Ch1CompensationLSBMid;
            array[2] = HGA10Ch1CompensationMSBLow;
            array[3] = HGA10Ch1CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10 CH2
        public int HGA10Ch2Compensation()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch2CompensationLSBLow;
            array[1] = HGA10Ch2CompensationLSBMid;
            array[2] = HGA10Ch2CompensationMSBLow;
            array[3] = HGA10Ch2CompensationMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
