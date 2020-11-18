using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe34GetAllHGACapacitanceSecondaryResults
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        // HGA1ESR1
        public byte HGA1ESR1LSBLow;
        public byte HGA1ESR1LSBMid;
        public byte HGA1ESR1MSBLow;
        public byte HGA1ESR1MSBHigh;

        // HGA1ESR2
        public byte HGA1ESR2LSBLow;
        public byte HGA1ESR2LSBMid;
        public byte HGA1ESR2MSBLow;
        public byte HGA1ESR2MSBHigh;

        // HGA2ESR1
        public byte HGA2ESR1LSBLow;
        public byte HGA2ESR1LSBMid;
        public byte HGA2ESR1MSBLow;
        public byte HGA2ESR1MSBHigh;

        // HGA2ESR2
        public byte HGA2ESR2LSBLow;
        public byte HGA2ESR2LSBMid;
        public byte HGA2ESR2MSBLow;
        public byte HGA2ESR2MSBHigh;

        // HGA3ESR1
        public byte HGA3ESR1LSBLow;
        public byte HGA3ESR1LSBMid;
        public byte HGA3ESR1MSBLow;
        public byte HGA3ESR1MSBHigh;

        // HGA3ESR2
        public byte HGA3ESR2LSBLow;
        public byte HGA3ESR2LSBMid;
        public byte HGA3ESR2MSBLow;
        public byte HGA3ESR2MSBHigh;

        // HGA4ESR1
        public byte HGA4ESR1LSBLow;
        public byte HGA4ESR1LSBMid;
        public byte HGA4ESR1MSBLow;
        public byte HGA4ESR1MSBHigh;

        // HGA4ESR2
        public byte HGA4ESR2LSBLow;
        public byte HGA4ESR2LSBMid;
        public byte HGA4ESR2MSBLow;
        public byte HGA4ESR2MSBHigh;

        // HGA5ESR1
        public byte HGA5ESR1LSBLow;
        public byte HGA5ESR1LSBMid;
        public byte HGA5ESR1MSBLow;
        public byte HGA5ESR1MSBHigh;

        // HGA5ESR2
        public byte HGA5ESR2LSBLow;
        public byte HGA5ESR2LSBMid;
        public byte HGA5ESR2MSBLow;
        public byte HGA5ESR2MSBHigh;

        // HGA6ESR1
        public byte HGA6ESR1LSBLow;
        public byte HGA6ESR1LSBMid;
        public byte HGA6ESR1MSBLow;
        public byte HGA6ESR1MSBHigh;

        // HGA6ESR2
        public byte HGA6ESR2LSBLow;
        public byte HGA6ESR2LSBMid;
        public byte HGA6ESR2MSBLow;
        public byte HGA6ESR2MSBHigh;

        // HGA7ESR1
        public byte HGA7ESR1LSBLow;
        public byte HGA7ESR1LSBMid;
        public byte HGA7ESR1MSBLow;
        public byte HGA7ESR1MSBHigh;

        // HGA7ESR2
        public byte HGA7ESR2LSBLow;
        public byte HGA7ESR2LSBMid;
        public byte HGA7ESR2MSBLow;
        public byte HGA7ESR2MSBHigh;

        // HGA8ESR1
        public byte HGA8ESR1LSBLow;
        public byte HGA8ESR1LSBMid;
        public byte HGA8ESR1MSBLow;
        public byte HGA8ESR1MSBHigh;

        // HGA8ESR2
        public byte HGA8ESR2LSBLow;
        public byte HGA8ESR2LSBMid;
        public byte HGA8ESR2MSBLow;
        public byte HGA8ESR2MSBHigh;

        // HGA9ESR1
        public byte HGA9ESR1LSBLow;
        public byte HGA9ESR1LSBMid;
        public byte HGA9ESR1MSBLow;
        public byte HGA9ESR1MSBHigh;

        // HGA9ESR2
        public byte HGA9ESR2LSBLow;
        public byte HGA9ESR2LSBMid;
        public byte HGA9ESR2MSBLow;
        public byte HGA9ESR2MSBHigh;

        // HGA10ESR1
        public byte HGA10ESR1LSBLow;
        public byte HGA10ESR1LSBMid;
        public byte HGA10ESR1MSBLow;
        public byte HGA10ESR1MSBHigh;

        // HGA10ESR2
        public byte HGA10ESR2LSBLow;
        public byte HGA10ESR2LSBMid;
        public byte HGA10ESR2MSBLow;
        public byte HGA10ESR2MSBHigh;


        public static TestProbe34GetAllHGACapacitanceSecondaryResults ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe34GetAllHGACapacitanceSecondaryResults*)pb;
            }
        }

        // HGA1ESR1
        public int HGA1ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA1ESR1LSBLow;
            array[1] = HGA1ESR1LSBMid;
            array[2] = HGA1ESR1MSBLow;
            array[3] = HGA1ESR1MSBHigh;   
            return BitConverter.ToInt32(array, 0);
        }

        // HGA1ESR2
        public int HGA1ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA1ESR2LSBLow;
            array[1] = HGA1ESR2LSBMid;
            array[2] = HGA1ESR2MSBLow;
            array[3] = HGA1ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2ESR1
        public int HGA2ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA2ESR1LSBLow;
            array[1] = HGA2ESR1LSBMid;
            array[2] = HGA2ESR1MSBLow;
            array[3] = HGA2ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2ESR2
        public int HGA2ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA2ESR2LSBLow;
            array[1] = HGA2ESR2LSBMid;
            array[2] = HGA2ESR2MSBLow;
            array[3] = HGA2ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3ESR1
        public int HGA3ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA3ESR1LSBLow;
            array[1] = HGA3ESR1LSBMid;
            array[2] = HGA3ESR1MSBLow;
            array[3] = HGA3ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3ESR2
        public int HGA3ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA3ESR2LSBLow;
            array[1] = HGA3ESR2LSBMid;
            array[2] = HGA3ESR2MSBLow;
            array[3] = HGA3ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4ESR1
        public int HGA4ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA4ESR1LSBLow;
            array[1] = HGA4ESR1LSBMid;
            array[2] = HGA4ESR1MSBLow;
            array[3] = HGA4ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4ESR2
        public int HGA4ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA4ESR2LSBLow;
            array[1] = HGA4ESR2LSBMid;
            array[2] = HGA4ESR2MSBLow;
            array[3] = HGA4ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5ESR1
        public int HGA5ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA5ESR1LSBLow;
            array[1] = HGA5ESR1LSBMid;
            array[2] = HGA5ESR1MSBLow;
            array[3] = HGA5ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5ESR2
        public int HGA5ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA5ESR2LSBLow;
            array[1] = HGA5ESR2LSBMid;
            array[2] = HGA5ESR2MSBLow;
            array[3] = HGA5ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6ESR1
        public int HGA6ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA6ESR1LSBLow;
            array[1] = HGA6ESR1LSBMid;
            array[2] = HGA6ESR1MSBLow;
            array[3] = HGA6ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6ESR2
        public int HGA6ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA6ESR2LSBLow;
            array[1] = HGA6ESR2LSBMid;
            array[2] = HGA6ESR2MSBLow;
            array[3] = HGA6ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7ESR1
        public int HGA7ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA7ESR1LSBLow;
            array[1] = HGA7ESR1LSBMid;
            array[2] = HGA7ESR1MSBLow;
            array[3] = HGA7ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7ESR2
        public int HGA7ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA7ESR2LSBLow;
            array[1] = HGA7ESR2LSBMid;
            array[2] = HGA7ESR2MSBLow;
            array[3] = HGA7ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8ESR1
        public int HGA8ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA8ESR1LSBLow;
            array[1] = HGA8ESR1LSBMid;
            array[2] = HGA8ESR1MSBLow;
            array[3] = HGA8ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8ESR2
        public int HGA8ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA8ESR2LSBLow;
            array[1] = HGA8ESR2LSBMid;
            array[2] = HGA8ESR2MSBLow;
            array[3] = HGA8ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9ESR1
        public int HGA9ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA9ESR1LSBLow;
            array[1] = HGA9ESR1LSBMid;
            array[2] = HGA9ESR1MSBLow;
            array[3] = HGA9ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9ESR2
        public int HGA9ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA9ESR2LSBLow;
            array[1] = HGA9ESR2LSBMid;
            array[2] = HGA9ESR2MSBLow;
            array[3] = HGA9ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10ESR1
        public int HGA10ESR1()
        {
            byte[] array = new byte[4];
            array[0] = HGA10ESR1LSBLow;
            array[1] = HGA10ESR1LSBMid;
            array[2] = HGA10ESR1MSBLow;
            array[3] = HGA10ESR1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10ESR2
        public int HGA10ESR2()
        {
            byte[] array = new byte[4];
            array[0] = HGA10ESR2LSBLow;
            array[1] = HGA10ESR2LSBMid;
            array[2] = HGA10ESR2MSBLow;
            array[3] = HGA10ESR2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
