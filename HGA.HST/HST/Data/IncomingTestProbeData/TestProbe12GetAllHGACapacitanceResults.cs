using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe12GetAllHGACapacitanceResults
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;        

        // HGA1        
        public byte HGA1Capacitance1LSBLow;
        public byte HGA1Capacitance1LSBMid;
        public byte HGA1Capacitance1MSBLow;
        public byte HGA1Capacitance1MSBHigh;

        public byte HGA1Capacitance2LSBLow;
        public byte HGA1Capacitance2LSBMid;
        public byte HGA1Capacitance2MSBLow;
        public byte HGA1Capacitance2MSBHigh;                

        // HGA2
        public byte HGA2Capacitance1LSBLow;
        public byte HGA2Capacitance1LSBMid;
        public byte HGA2Capacitance1MSBLow;
        public byte HGA2Capacitance1MSBHigh;

        public byte HGA2Capacitance2LSBLow;
        public byte HGA2Capacitance2LSBMid;
        public byte HGA2Capacitance2MSBLow;
        public byte HGA2Capacitance2MSBHigh;          

        // HGA3
        public byte HGA3Capacitance1LSBLow;
        public byte HGA3Capacitance1LSBMid;
        public byte HGA3Capacitance1MSBLow;
        public byte HGA3Capacitance1MSBHigh;

        public byte HGA3Capacitance2LSBLow;
        public byte HGA3Capacitance2LSBMid;
        public byte HGA3Capacitance2MSBLow;
        public byte HGA3Capacitance2MSBHigh;   

        // HGA4
        public byte HGA4Capacitance1LSBLow;
        public byte HGA4Capacitance1LSBMid;
        public byte HGA4Capacitance1MSBLow;
        public byte HGA4Capacitance1MSBHigh;

        public byte HGA4Capacitance2LSBLow;
        public byte HGA4Capacitance2LSBMid;
        public byte HGA4Capacitance2MSBLow;
        public byte HGA4Capacitance2MSBHigh;   

        // HGA5
        public byte HGA5Capacitance1LSBLow;
        public byte HGA5Capacitance1LSBMid;
        public byte HGA5Capacitance1MSBLow;
        public byte HGA5Capacitance1MSBHigh;

        public byte HGA5Capacitance2LSBLow;
        public byte HGA5Capacitance2LSBMid;
        public byte HGA5Capacitance2MSBLow;
        public byte HGA5Capacitance2MSBHigh;   

        // HGA6
        public byte HGA6Capacitance1LSBLow;
        public byte HGA6Capacitance1LSBMid;
        public byte HGA6Capacitance1MSBLow;
        public byte HGA6Capacitance1MSBHigh;

        public byte HGA6Capacitance2LSBLow;
        public byte HGA6Capacitance2LSBMid;
        public byte HGA6Capacitance2MSBLow;
        public byte HGA6Capacitance2MSBHigh;   

        // HGA7
        public byte HGA7Capacitance1LSBLow;
        public byte HGA7Capacitance1LSBMid;
        public byte HGA7Capacitance1MSBLow;
        public byte HGA7Capacitance1MSBHigh;

        public byte HGA7Capacitance2LSBLow;
        public byte HGA7Capacitance2LSBMid;
        public byte HGA7Capacitance2MSBLow;
        public byte HGA7Capacitance2MSBHigh;   

        // HGA8
        public byte HGA8Capacitance1LSBLow;
        public byte HGA8Capacitance1LSBMid;
        public byte HGA8Capacitance1MSBLow;
        public byte HGA8Capacitance1MSBHigh;

        public byte HGA8Capacitance2LSBLow;
        public byte HGA8Capacitance2LSBMid;
        public byte HGA8Capacitance2MSBLow;
        public byte HGA8Capacitance2MSBHigh;   

        // HGA9
        public byte HGA9Capacitance1LSBLow;
        public byte HGA9Capacitance1LSBMid;
        public byte HGA9Capacitance1MSBLow;
        public byte HGA9Capacitance1MSBHigh;

        public byte HGA9Capacitance2LSBLow;
        public byte HGA9Capacitance2LSBMid;
        public byte HGA9Capacitance2MSBLow;
        public byte HGA9Capacitance2MSBHigh;   

        // HGA10
        public byte HGA10Capacitance1LSBLow;
        public byte HGA10Capacitance1LSBMid;
        public byte HGA10Capacitance1MSBLow;
        public byte HGA10Capacitance1MSBHigh;

        public byte HGA10Capacitance2LSBLow;
        public byte HGA10Capacitance2LSBMid;
        public byte HGA10Capacitance2MSBLow;
        public byte HGA10Capacitance2MSBHigh;           

        public static TestProbe12GetAllHGACapacitanceResults ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe12GetAllHGACapacitanceResults*)pb;
            }
        }

        // HGA1
        public int HGA1Capacitance1()
        {
             byte[] array = new byte[4];
             array[0] = HGA1Capacitance1LSBLow;
             array[1] = HGA1Capacitance1LSBMid;
             array[2] = HGA1Capacitance1MSBLow;
             array[3] = HGA1Capacitance1MSBHigh;
             return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Capacitance2LSBLow;
            array[1] = HGA1Capacitance2LSBMid;
            array[2] = HGA1Capacitance2MSBLow;
            array[3] = HGA1Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2
        public int HGA2Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Capacitance1LSBLow;
            array[1] = HGA2Capacitance1LSBMid;
            array[2] = HGA2Capacitance1MSBLow;
            array[3] = HGA2Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Capacitance2LSBLow;
            array[1] = HGA2Capacitance2LSBMid;
            array[2] = HGA2Capacitance2MSBLow;
            array[3] = HGA2Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3
        public int HGA3Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Capacitance1LSBLow;
            array[1] = HGA3Capacitance1LSBMid;
            array[2] = HGA3Capacitance1MSBLow;
            array[3] = HGA3Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Capacitance2LSBLow;
            array[1] = HGA3Capacitance2LSBMid;
            array[2] = HGA3Capacitance2MSBLow;
            array[3] = HGA3Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4
        public int HGA4Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Capacitance1LSBLow;
            array[1] = HGA4Capacitance1LSBMid;
            array[2] = HGA4Capacitance1MSBLow;
            array[3] = HGA4Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Capacitance2LSBLow;
            array[1] = HGA4Capacitance2LSBMid;
            array[2] = HGA4Capacitance2MSBLow;
            array[3] = HGA4Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5
        public int HGA5Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Capacitance1LSBLow;
            array[1] = HGA5Capacitance1LSBMid;
            array[2] = HGA5Capacitance1MSBLow;
            array[3] = HGA5Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Capacitance2LSBLow;
            array[1] = HGA5Capacitance2LSBMid;
            array[2] = HGA5Capacitance2MSBLow;
            array[3] = HGA5Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6
        public int HGA6Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Capacitance1LSBLow;
            array[1] = HGA6Capacitance1LSBMid;
            array[2] = HGA6Capacitance1MSBLow;
            array[3] = HGA6Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Capacitance2LSBLow;
            array[1] = HGA6Capacitance2LSBMid;
            array[2] = HGA6Capacitance2MSBLow;
            array[3] = HGA6Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7
        public int HGA7Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Capacitance1LSBLow;
            array[1] = HGA7Capacitance1LSBMid;
            array[2] = HGA7Capacitance1MSBLow;
            array[3] = HGA7Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Capacitance2LSBLow;
            array[1] = HGA7Capacitance2LSBMid;
            array[2] = HGA7Capacitance2MSBLow;
            array[3] = HGA7Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8
        public int HGA8Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Capacitance1LSBLow;
            array[1] = HGA8Capacitance1LSBMid;
            array[2] = HGA8Capacitance1MSBLow;
            array[3] = HGA8Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Capacitance2LSBLow;
            array[1] = HGA8Capacitance2LSBMid;
            array[2] = HGA8Capacitance2MSBLow;
            array[3] = HGA8Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9
        public int HGA9Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Capacitance1LSBLow;
            array[1] = HGA9Capacitance1LSBMid;
            array[2] = HGA9Capacitance1MSBLow;
            array[3] = HGA9Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Capacitance2LSBLow;
            array[1] = HGA9Capacitance2LSBMid;
            array[2] = HGA9Capacitance2MSBLow;
            array[3] = HGA9Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10
        public int HGA10Capacitance1()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Capacitance1LSBLow;
            array[1] = HGA10Capacitance1LSBMid;
            array[2] = HGA10Capacitance1MSBLow;
            array[3] = HGA10Capacitance1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Capacitance2()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Capacitance2LSBLow;
            array[1] = HGA10Capacitance2LSBMid;
            array[2] = HGA10Capacitance2MSBLow;
            array[3] = HGA10Capacitance2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
