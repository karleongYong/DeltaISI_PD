using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe51GetCableCalibrationCapacitanceResults
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;


        // HGA1
        public byte HGA1Ch1CapacitanceLSBLow;
        public byte HGA1Ch1CapacitanceLSBMid;
        public byte HGA1Ch1CapacitanceMSBLow;
        public byte HGA1Ch1CapacitanceMSBHigh;

        public byte HGA1Ch2CapacitanceLSBLow;
        public byte HGA1Ch2CapacitanceLSBMid;
        public byte HGA1Ch2CapacitanceMSBLow;
        public byte HGA1Ch2CapacitanceMSBHigh;

        // HGA2
        public byte HGA2Ch1CapacitanceLSBLow;
        public byte HGA2Ch1CapacitanceLSBMid;
        public byte HGA2Ch1CapacitanceMSBLow;
        public byte HGA2Ch1CapacitanceMSBHigh;

        public byte HGA2Ch2CapacitanceLSBLow;
        public byte HGA2Ch2CapacitanceLSBMid;
        public byte HGA2Ch2CapacitanceMSBLow;
        public byte HGA2Ch2CapacitanceMSBHigh;

        // HGA3
        public byte HGA3Ch1CapacitanceLSBLow;
        public byte HGA3Ch1CapacitanceLSBMid;
        public byte HGA3Ch1CapacitanceMSBLow;
        public byte HGA3Ch1CapacitanceMSBHigh;

        public byte HGA3Ch2CapacitanceLSBLow;
        public byte HGA3Ch2CapacitanceLSBMid;
        public byte HGA3Ch2CapacitanceMSBLow;
        public byte HGA3Ch2CapacitanceMSBHigh;

        // HGA4
        public byte HGA4Ch1CapacitanceLSBLow;
        public byte HGA4Ch1CapacitanceLSBMid;
        public byte HGA4Ch1CapacitanceMSBLow;
        public byte HGA4Ch1CapacitanceMSBHigh;

        public byte HGA4Ch2CapacitanceLSBLow;
        public byte HGA4Ch2CapacitanceLSBMid;
        public byte HGA4Ch2CapacitanceMSBLow;
        public byte HGA4Ch2CapacitanceMSBHigh;

        // HGA5
        public byte HGA5Ch1CapacitanceLSBLow;
        public byte HGA5Ch1CapacitanceLSBMid;
        public byte HGA5Ch1CapacitanceMSBLow;
        public byte HGA5Ch1CapacitanceMSBHigh;

        public byte HGA5Ch2CapacitanceLSBLow;
        public byte HGA5Ch2CapacitanceLSBMid;
        public byte HGA5Ch2CapacitanceMSBLow;
        public byte HGA5Ch2CapacitanceMSBHigh;

        // HGA6
        public byte HGA6Ch1CapacitanceLSBLow;
        public byte HGA6Ch1CapacitanceLSBMid;
        public byte HGA6Ch1CapacitanceMSBLow;
        public byte HGA6Ch1CapacitanceMSBHigh;

        public byte HGA6Ch2CapacitanceLSBLow;
        public byte HGA6Ch2CapacitanceLSBMid;
        public byte HGA6Ch2CapacitanceMSBLow;
        public byte HGA6Ch2CapacitanceMSBHigh;

        // HGA7
        public byte HGA7Ch1CapacitanceLSBLow;
        public byte HGA7Ch1CapacitanceLSBMid;
        public byte HGA7Ch1CapacitanceMSBLow;
        public byte HGA7Ch1CapacitanceMSBHigh;

        public byte HGA7Ch2CapacitanceLSBLow;
        public byte HGA7Ch2CapacitanceLSBMid;
        public byte HGA7Ch2CapacitanceMSBLow;
        public byte HGA7Ch2CapacitanceMSBHigh;

        // HGA8
        public byte HGA8Ch1CapacitanceLSBLow;
        public byte HGA8Ch1CapacitanceLSBMid;
        public byte HGA8Ch1CapacitanceMSBLow;
        public byte HGA8Ch1CapacitanceMSBHigh;

        public byte HGA8Ch2CapacitanceLSBLow;
        public byte HGA8Ch2CapacitanceLSBMid;
        public byte HGA8Ch2CapacitanceMSBLow;
        public byte HGA8Ch2CapacitanceMSBHigh;

        // HGA9
        public byte HGA9Ch1CapacitanceLSBLow;
        public byte HGA9Ch1CapacitanceLSBMid;
        public byte HGA9Ch1CapacitanceMSBLow;
        public byte HGA9Ch1CapacitanceMSBHigh;

        public byte HGA9Ch2CapacitanceLSBLow;
        public byte HGA9Ch2CapacitanceLSBMid;
        public byte HGA9Ch2CapacitanceMSBLow;
        public byte HGA9Ch2CapacitanceMSBHigh;

        // HGA10
        public byte HGA10Ch1CapacitanceLSBLow;
        public byte HGA10Ch1CapacitanceLSBMid;
        public byte HGA10Ch1CapacitanceMSBLow;
        public byte HGA10Ch1CapacitanceMSBHigh;

        public byte HGA10Ch2CapacitanceLSBLow;
        public byte HGA10Ch2CapacitanceLSBMid;
        public byte HGA10Ch2CapacitanceMSBLow;
        public byte HGA10Ch2CapacitanceMSBHigh;

        public static TestProbe51GetCableCalibrationCapacitanceResults ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe51GetCableCalibrationCapacitanceResults*)pb;
            }
        }


        // HGA1
        public int HGA1Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch1CapacitanceLSBLow;
            array[1] = HGA1Ch1CapacitanceLSBMid;
            array[2] = HGA1Ch1CapacitanceMSBLow;
            array[3] = HGA1Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch2CapacitanceLSBLow;
            array[1] = HGA1Ch2CapacitanceLSBMid;
            array[2] = HGA1Ch2CapacitanceMSBLow;
            array[3] = HGA1Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2
        public int HGA2Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch1CapacitanceLSBLow;
            array[1] = HGA2Ch1CapacitanceLSBMid;
            array[2] = HGA2Ch1CapacitanceMSBLow;
            array[3] = HGA2Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch2CapacitanceLSBLow;
            array[1] = HGA2Ch2CapacitanceLSBMid;
            array[2] = HGA2Ch2CapacitanceMSBLow;
            array[3] = HGA2Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3
        public int HGA3Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch1CapacitanceLSBLow;
            array[1] = HGA3Ch1CapacitanceLSBMid;
            array[2] = HGA3Ch1CapacitanceMSBLow;
            array[3] = HGA3Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch2CapacitanceLSBLow;
            array[1] = HGA3Ch2CapacitanceLSBMid;
            array[2] = HGA3Ch2CapacitanceMSBLow;
            array[3] = HGA3Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4
        public int HGA4Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch1CapacitanceLSBLow;
            array[1] = HGA4Ch1CapacitanceLSBMid;
            array[2] = HGA4Ch1CapacitanceMSBLow;
            array[3] = HGA4Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch2CapacitanceLSBLow;
            array[1] = HGA4Ch2CapacitanceLSBMid;
            array[2] = HGA4Ch2CapacitanceMSBLow;
            array[3] = HGA4Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5
        public int HGA5Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch1CapacitanceLSBLow;
            array[1] = HGA5Ch1CapacitanceLSBMid;
            array[2] = HGA5Ch1CapacitanceMSBLow;
            array[3] = HGA5Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch2CapacitanceLSBLow;
            array[1] = HGA5Ch2CapacitanceLSBMid;
            array[2] = HGA5Ch2CapacitanceMSBLow;
            array[3] = HGA5Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6
        public int HGA6Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch1CapacitanceLSBLow;
            array[1] = HGA6Ch1CapacitanceLSBMid;
            array[2] = HGA6Ch1CapacitanceMSBLow;
            array[3] = HGA6Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch2CapacitanceLSBLow;
            array[1] = HGA6Ch2CapacitanceLSBMid;
            array[2] = HGA6Ch2CapacitanceMSBLow;
            array[3] = HGA6Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7
        public int HGA7Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch1CapacitanceLSBLow;
            array[1] = HGA7Ch1CapacitanceLSBMid;
            array[2] = HGA7Ch1CapacitanceMSBLow;
            array[3] = HGA7Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch2CapacitanceLSBLow;
            array[1] = HGA7Ch2CapacitanceLSBMid;
            array[2] = HGA7Ch2CapacitanceMSBLow;
            array[3] = HGA7Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8
        public int HGA8Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch1CapacitanceLSBLow;
            array[1] = HGA8Ch1CapacitanceLSBMid;
            array[2] = HGA8Ch1CapacitanceMSBLow;
            array[3] = HGA8Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch2CapacitanceLSBLow;
            array[1] = HGA8Ch2CapacitanceLSBMid;
            array[2] = HGA8Ch2CapacitanceMSBLow;
            array[3] = HGA8Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9
        public int HGA9Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch1CapacitanceLSBLow;
            array[1] = HGA9Ch1CapacitanceLSBMid;
            array[2] = HGA9Ch1CapacitanceMSBLow;
            array[3] = HGA9Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch2CapacitanceLSBLow;
            array[1] = HGA9Ch2CapacitanceLSBMid;
            array[2] = HGA9Ch2CapacitanceMSBLow;
            array[3] = HGA9Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10
        public int HGA10Ch1Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch1CapacitanceLSBLow;
            array[1] = HGA10Ch1CapacitanceLSBMid;
            array[2] = HGA10Ch1CapacitanceMSBLow;
            array[3] = HGA10Ch1CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Ch2Capacitance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch2CapacitanceLSBLow;
            array[1] = HGA10Ch2CapacitanceLSBMid;
            array[2] = HGA10Ch2CapacitanceMSBLow;
            array[3] = HGA10Ch2CapacitanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
