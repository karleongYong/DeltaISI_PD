using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe72GetVolDelta
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        //HGA1
        public byte VolDelta1HGA1LowLSB;
        public byte VolDelta1HGA1MidLSB;
        public byte VolDelta1HGA1LowMSB;
        public byte VolDelta1HGA1HighMSB;

        //HGA2
        public byte VolDelta1HGA2LowLSB;
        public byte VolDelta1HGA2MidLSB;
        public byte VolDelta1HGA2LowMSB;
        public byte VolDelta1HGA2HighMSB;

        //HGA3
        public byte VolDelta1HGA3LowLSB;
        public byte VolDelta1HGA3MidLSB;
        public byte VolDelta1HGA3LowMSB;
        public byte VolDelta1HGA3HighMSB;

        //HGA4
        public byte VolDelta1HGA4LowLSB;
        public byte VolDelta1HGA4MidLSB;
        public byte VolDelta1HGA4LowMSB;
        public byte VolDelta1HGA4HighMSB;

        //HGA5
        public byte VolDelta1HGA5LowLSB;
        public byte VolDelta1HGA5MidLSB;
        public byte VolDelta1HGA5LowMSB;
        public byte VolDelta1HGA5HighMSB;

        //HGA6
        public byte VolDelta1HGA6LowLSB;
        public byte VolDelta1HGA6MidLSB;
        public byte VolDelta1HGA6LowMSB;
        public byte VolDelta1HGA6HighMSB;

        //HGA7
        public byte VolDelta1HGA7LowLSB;
        public byte VolDelta1HGA7MidLSB;
        public byte VolDelta1HGA7LowMSB;
        public byte VolDelta1HGA7HighMSB;

        //HGA8
        public byte VolDelta1HGA8LowLSB;
        public byte VolDelta1HGA8MidLSB;
        public byte VolDelta1HGA8LowMSB;
        public byte VolDelta1HGA8HighMSB;

        //HGA9
        public byte VolDelta1HGA9LowLSB;
        public byte VolDelta1HGA9MidLSB;
        public byte VolDelta1HGA9LowMSB;
        public byte VolDelta1HGA9HighMSB;

        //HGA10
        public byte VolDelta1HGA10LowLSB;
        public byte VolDelta1HGA10MidLSB;
        public byte VolDelta1HGA10LowMSB;
        public byte VolDelta1HGA10HighMSB;

        //delta 2
        //HGA1
        public byte VolDelta2HGA1LowLSB;
        public byte VolDelta2HGA1MidLSB;
        public byte VolDelta2HGA1LowMSB;
        public byte VolDelta2HGA1HighMSB;

        //HGA2
        public byte VolDelta2HGA2LowLSB;
        public byte VolDelta2HGA2MidLSB;
        public byte VolDelta2HGA2LowMSB;
        public byte VolDelta2HGA2HighMSB;

        //HGA3
        public byte VolDelta2HGA3LowLSB;
        public byte VolDelta2HGA3MidLSB;
        public byte VolDelta2HGA3LowMSB;
        public byte VolDelta2HGA3HighMSB;

        //HGA4
        public byte VolDelta2HGA4LowLSB;
        public byte VolDelta2HGA4MidLSB;
        public byte VolDelta2HGA4LowMSB;
        public byte VolDelta2HGA4HighMSB;

        //HGA5
        public byte VolDelta2HGA5LowLSB;
        public byte VolDelta2HGA5MidLSB;
        public byte VolDelta2HGA5LowMSB;
        public byte VolDelta2HGA5HighMSB;

        //HGA6
        public byte VolDelta2HGA6LowLSB;
        public byte VolDelta2HGA6MidLSB;
        public byte VolDelta2HGA6LowMSB;
        public byte VolDelta2HGA6HighMSB;

        //HGA7
        public byte VolDelta2HGA7LowLSB;
        public byte VolDelta2HGA7MidLSB;
        public byte VolDelta2HGA7LowMSB;
        public byte VolDelta2HGA7HighMSB;

        //HGA8
        public byte VolDelta2HGA8LowLSB;
        public byte VolDelta2HGA8MidLSB;
        public byte VolDelta2HGA8LowMSB;
        public byte VolDelta2HGA8HighMSB;

        //HGA9
        public byte VolDelta2HGA9LowLSB;
        public byte VolDelta2HGA9MidLSB;
        public byte VolDelta2HGA9LowMSB;
        public byte VolDelta2HGA9HighMSB;

        //HGA10
        public byte VolDelta2HGA10LowLSB;
        public byte VolDelta2HGA10MidLSB;
        public byte VolDelta2HGA10LowMSB;
        public byte VolDelta2HGA10HighMSB;

        public static TestProbe72GetVolDelta ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe72GetVolDelta*)pb;
            }
        }


        public int GetVolDelta1HGA1()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA1LowLSB;
            array[1] = VolDelta1HGA1MidLSB;
            array[2] = VolDelta1HGA1LowMSB;
            array[3] = VolDelta1HGA1HighMSB;
            return BitConverter.ToInt32(array, 0);


        }

        public int GetVolDelta1HGA2()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA2LowLSB;
            array[1] = VolDelta1HGA2MidLSB;
            array[2] = VolDelta1HGA2LowMSB;
            array[3] = VolDelta1HGA2HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA3()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA3LowLSB;
            array[1] = VolDelta1HGA3MidLSB;
            array[2] = VolDelta1HGA3LowMSB;
            array[3] = VolDelta1HGA3HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA4()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA4LowLSB;
            array[1] = VolDelta1HGA4MidLSB;
            array[2] = VolDelta1HGA4LowMSB;
            array[3] = VolDelta1HGA4HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA5()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA5LowLSB;
            array[1] = VolDelta1HGA5MidLSB;
            array[2] = VolDelta1HGA5LowMSB;
            array[3] = VolDelta1HGA5HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA6()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA6LowLSB;
            array[1] = VolDelta1HGA6MidLSB;
            array[2] = VolDelta1HGA6LowMSB;
            array[3] = VolDelta1HGA6HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA7()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA7LowLSB;
            array[1] = VolDelta1HGA7MidLSB;
            array[2] = VolDelta1HGA7LowMSB;
            array[3] = VolDelta1HGA7HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA8()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA8LowLSB;
            array[1] = VolDelta1HGA8MidLSB;
            array[2] = VolDelta1HGA8LowMSB;
            array[3] = VolDelta1HGA8HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA9()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA9LowLSB;
            array[1] = VolDelta1HGA9MidLSB;
            array[2] = VolDelta1HGA9LowMSB;
            array[3] = VolDelta1HGA9HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        public int GetVolDelta1HGA10()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta1HGA10LowLSB;
            array[1] = VolDelta1HGA10MidLSB;
            array[2] = VolDelta1HGA10LowMSB;
            array[3] = VolDelta1HGA10HighMSB;
            return BitConverter.ToInt32(array, 0);
        }

        //-----------------------------------------------

        public int GetVolDelta2HGA1()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA1LowLSB;
            array[1] = VolDelta2HGA1MidLSB;
            array[2] = VolDelta2HGA1LowMSB;
            array[3] = VolDelta2HGA1HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA2()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA2LowLSB;
            array[1] = VolDelta2HGA2MidLSB;
            array[2] = VolDelta2HGA2LowMSB;
            array[3] = VolDelta2HGA2HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA3()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA3LowLSB;
            array[1] = VolDelta2HGA3MidLSB;
            array[2] = VolDelta2HGA3LowMSB;
            array[3] = VolDelta2HGA3HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA4()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA4LowLSB;
            array[1] = VolDelta2HGA4MidLSB;
            array[2] = VolDelta2HGA4LowMSB;
            array[3] = VolDelta2HGA4HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA5()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA5LowLSB;
            array[1] = VolDelta2HGA5MidLSB;
            array[2] = VolDelta2HGA5LowMSB;
            array[3] = VolDelta2HGA5HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA6()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA6LowLSB;
            array[1] = VolDelta2HGA6MidLSB;
            array[2] = VolDelta2HGA6LowMSB;
            array[3] = VolDelta2HGA6HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA7()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA7LowLSB;
            array[1] = VolDelta2HGA7MidLSB;
            array[2] = VolDelta2HGA7LowMSB;
            array[3] = VolDelta2HGA7HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA8()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA8LowLSB;
            array[1] = VolDelta2HGA8MidLSB;
            array[2] = VolDelta2HGA8LowMSB;
            array[3] = VolDelta2HGA8HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA9()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA9LowLSB;
            array[1] = VolDelta2HGA9MidLSB;
            array[2] = VolDelta2HGA9LowMSB;
            array[3] = VolDelta2HGA9HighMSB;
            return BitConverter.ToInt32(array, 0);

        }

        public int GetVolDelta2HGA10()
        {
            byte[] array = new byte[4];
            array[0] = VolDelta2HGA10LowLSB;
            array[1] = VolDelta2HGA10MidLSB;
            array[2] = VolDelta2HGA10LowMSB;
            array[3] = VolDelta2HGA10HighMSB;
            return BitConverter.ToInt32(array, 0);

        }
    }

}
