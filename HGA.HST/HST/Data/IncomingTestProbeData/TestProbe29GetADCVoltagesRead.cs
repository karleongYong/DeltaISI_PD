using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe29GetADCVoltagesRead
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        // Voltage1
        public byte Voltage1LSBLow;
        public byte Voltage1LSBHigh;
        public byte Voltage1MSBMid;
        public byte Voltage1MSBHigh;

        // Voltage2
        public byte Voltage2LSBLow;
        public byte Voltage2LSBHigh;
        public byte Voltage2MSBMid;
        public byte Voltage2MSBHigh;

        // Voltage3
        public byte Voltage3LSBLow;
        public byte Voltage3LSBHigh;
        public byte Voltage3MSBMid;
        public byte Voltage3MSBHigh;

        // Voltage4
        public byte Voltage4LSBLow;
        public byte Voltage4LSBHigh;
        public byte Voltage4MSBMid;
        public byte Voltage4MSBHigh;

        // Voltage5
        public byte Voltage5LSBLow;
        public byte Voltage5LSBHigh;
        public byte Voltage5MSBMid;
        public byte Voltage5MSBHigh;

        // Voltage6
        public byte Voltage6LSBLow;
        public byte Voltage6LSBHigh;
        public byte Voltage6MSBMid;
        public byte Voltage6MSBHigh;

        // Voltage7
        public byte Voltage7LSBLow;
        public byte Voltage7LSBHigh;
        public byte Voltage7MSBMid;
        public byte Voltage7MSBHigh;

        // Voltage8
        public byte Voltage8LSBLow;
        public byte Voltage8LSBHigh;
        public byte Voltage8MSBMid;
        public byte Voltage8MSBHigh;

        // Voltage9
        public byte Voltage9LSBLow;
        public byte Voltage9LSBHigh;
        public byte Voltage9MSBMid;
        public byte Voltage9MSBHigh;

        // Voltage10
        public byte Voltage10LSBLow;
        public byte Voltage10LSBHigh;
        public byte Voltage10MSBMid;
        public byte Voltage10MSBHigh;

        // Voltage11
        public byte Voltage11LSBLow;
        public byte Voltage11LSBHigh;
        public byte Voltage11MSBMid;
        public byte Voltage11MSBHigh;

        // Voltage12
        public byte Voltage12LSBLow;
        public byte Voltage12LSBHigh;
        public byte Voltage12MSBMid;
        public byte Voltage12MSBHigh;

        // Voltage13
        public byte Voltage13LSBLow;
        public byte Voltage13LSBHigh;
        public byte Voltage13MSBMid;
        public byte Voltage13MSBHigh;

        // Voltage14
        public byte Voltage14LSBLow;
        public byte Voltage14LSBHigh;
        public byte Voltage14MSBMid;
        public byte Voltage14MSBHigh;

        // Voltage15
        public byte Voltage15LSBLow;
        public byte Voltage15LSBHigh;
        public byte Voltage15MSBMid;
        public byte Voltage15MSBHigh;

        // Voltage16
        public byte Voltage16LSBLow;
        public byte Voltage16LSBHigh;
        public byte Voltage16MSBMid;
        public byte Voltage16MSBHigh;

        public static TestProbe29GetADCVoltagesRead ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe29GetADCVoltagesRead*)pb;
            }
        }

        // Voltage1
        public int Voltage1()
        {
            byte[] array = new byte[4];
            array[0] = Voltage1LSBLow;
            array[1] = Voltage1LSBHigh;
            array[2] = Voltage1MSBMid;
            array[3] = Voltage1MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage2
        public int Voltage2()
        {
            byte[] array = new byte[4];
            array[0] = Voltage2LSBLow;
            array[1] = Voltage2LSBHigh;
            array[2] = Voltage2MSBMid;
            array[3] = Voltage2MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage3
        public int Voltage3()
        {
            byte[] array = new byte[4];
            array[0] = Voltage3LSBLow;
            array[1] = Voltage3LSBHigh;
            array[2] = Voltage3MSBMid;
            array[3] = Voltage3MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage4
        public int Voltage4()
        {
            byte[] array = new byte[4];
            array[0] = Voltage4LSBLow;
            array[1] = Voltage4LSBHigh;
            array[2] = Voltage4MSBMid;
            array[3] = Voltage4MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage5
        public int Voltage5()
        {
            byte[] array = new byte[4];
            array[0] = Voltage5LSBLow;
            array[1] = Voltage5LSBHigh;
            array[2] = Voltage5MSBMid;
            array[3] = Voltage5MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage6
        public int Voltage6()
        {
            byte[] array = new byte[4];
            array[0] = Voltage6LSBLow;
            array[1] = Voltage6LSBHigh;
            array[2] = Voltage6MSBMid;
            array[3] = Voltage6MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage7
        public int Voltage7()
        {
            byte[] array = new byte[4];
            array[0] = Voltage7LSBLow;
            array[1] = Voltage7LSBHigh;
            array[2] = Voltage7MSBMid;
            array[3] = Voltage7MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage8
        public int Voltage8()
        {
            byte[] array = new byte[4];
            array[0] = Voltage8LSBLow;
            array[1] = Voltage8LSBHigh;
            array[2] = Voltage8MSBMid;
            array[3] = Voltage8MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage9
        public int Voltage9()
        {
            byte[] array = new byte[4];
            array[0] = Voltage9LSBLow;
            array[1] = Voltage9LSBHigh;
            array[2] = Voltage9MSBMid;
            array[3] = Voltage9MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage10
        public int Voltage10()
        {
            byte[] array = new byte[4];
            array[0] = Voltage10LSBLow;
            array[1] = Voltage10LSBHigh;
            array[2] = Voltage10MSBMid;
            array[3] = Voltage10MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage11
        public int Voltage11()
        {
            byte[] array = new byte[4];
            array[0] = Voltage11LSBLow;
            array[1] = Voltage11LSBHigh;
            array[2] = Voltage11MSBMid;
            array[3] = Voltage11MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage12
        public int Voltage12()
        {
            byte[] array = new byte[4];
            array[0] = Voltage12LSBLow;
            array[1] = Voltage12LSBHigh;
            array[2] = Voltage12MSBMid;
            array[3] = Voltage12MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage13
        public int Voltage13()
        {
            byte[] array = new byte[4];
            array[0] = Voltage13LSBLow;
            array[1] = Voltage13LSBHigh;
            array[2] = Voltage13MSBMid;
            array[3] = Voltage13MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage14
        public int Voltage14()
        {
            byte[] array = new byte[4];
            array[0] = Voltage14LSBLow;
            array[1] = Voltage14LSBHigh;
            array[2] = Voltage14MSBMid;
            array[3] = Voltage14MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage15
        public int Voltage15()
        {
            byte[] array = new byte[4];
            array[0] = Voltage15LSBLow;
            array[1] = Voltage15LSBHigh;
            array[2] = Voltage15MSBMid;
            array[3] = Voltage15MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // Voltage16
        public int Voltage16()
        {
            byte[] array = new byte[4];
            array[0] = Voltage16LSBLow;
            array[1] = Voltage16LSBHigh;
            array[2] = Voltage16MSBMid;
            array[3] = Voltage16MSBHigh;
            return BitConverter.ToInt32(array, 0);
        }   
    }
}
