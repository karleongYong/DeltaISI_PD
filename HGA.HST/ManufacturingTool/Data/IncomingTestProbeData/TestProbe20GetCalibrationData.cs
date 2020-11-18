using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe20GetCalibrationData
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        // 0Ohm
        public byte Ch1Writer0OhmLSBLow;
        public byte Ch1Writer0OhmLSBMid;
        public byte Ch1Writer0OhmMSBLow;
        public byte Ch1Writer0OhmMSBHigh;

        public byte Ch2TA0OhmLSBLow;
        public byte Ch2TA0OhmLSBMid;
        public byte Ch2TA0OhmMSBLow;
        public byte Ch2TA0OhmMSBHigh;

        public byte Ch3WH0OhmLSBLow;
        public byte Ch3WH0OhmLSBMid;
        public byte Ch3WH0OhmMSBLow;
        public byte Ch3WH0OhmMSBHigh;

        public byte Ch4RH0OhmLSBLow;
        public byte Ch4RH0OhmLSBMid;
        public byte Ch4RH0OhmMSBLow;
        public byte Ch4RH0OhmMSBHigh;

        public byte Ch5R10OhmLSBLow;
        public byte Ch5R10OhmLSBMid;
        public byte Ch5R10OhmMSBLow;
        public byte Ch5R10OhmMSBHigh;

        public byte Ch6R20OhmLSBLow;
        public byte Ch6R20OhmLSBMid;
        public byte Ch6R20OhmMSBLow;
        public byte Ch6R20OhmMSBHigh;

        // 10Ohm
        public byte Ch1Writer10OhmLSBLow;
        public byte Ch1Writer10OhmLSBMid;
        public byte Ch1Writer10OhmMSBLow;
        public byte Ch1Writer10OhmMSBHigh;

        public byte Ch2TA10OhmLSBLow;
        public byte Ch2TA10OhmLSBMid;
        public byte Ch2TA10OhmMSBLow;
        public byte Ch2TA10OhmMSBHigh;

        public byte Ch3WH10OhmLSBLow;
        public byte Ch3WH10OhmLSBMid;
        public byte Ch3WH10OhmMSBLow;
        public byte Ch3WH10OhmMSBHigh;

        public byte Ch4RH10OhmLSBLow;
        public byte Ch4RH10OhmLSBMid;
        public byte Ch4RH10OhmMSBLow;
        public byte Ch4RH10OhmMSBHigh;

        public byte Ch5R110OhmLSBLow;
        public byte Ch5R110OhmLSBMid;
        public byte Ch5R110OhmMSBLow;
        public byte Ch5R110OhmMSBHigh;

        public byte Ch6R210OhmLSBLow;
        public byte Ch6R210OhmLSBMid;
        public byte Ch6R210OhmMSBLow;
        public byte Ch6R210OhmMSBHigh;

        // 100Ohm
        public byte Ch1Writer100OhmLSBLow;
        public byte Ch1Writer100OhmLSBMid;
        public byte Ch1Writer100OhmMSBLow;
        public byte Ch1Writer100OhmMSBHigh;

        public byte Ch2TA100OhmLSBLow;
        public byte Ch2TA100OhmLSBMid;
        public byte Ch2TA100OhmMSBLow;
        public byte Ch2TA100OhmMSBHigh;

        public byte Ch3WH100OhmLSBLow;
        public byte Ch3WH100OhmLSBMid;
        public byte Ch3WH100OhmMSBLow;
        public byte Ch3WH100OhmMSBHigh;

        public byte Ch4RH100OhmLSBLow;
        public byte Ch4RH100OhmLSBMid;
        public byte Ch4RH100OhmMSBLow;
        public byte Ch4RH100OhmMSBHigh;

        public byte Ch5R1100OhmLSBLow;
        public byte Ch5R1100OhmLSBMid;
        public byte Ch5R1100OhmMSBLow;
        public byte Ch5R1100OhmMSBHigh;

        public byte Ch6R2100OhmLSBLow;
        public byte Ch6R2100OhmLSBMid;
        public byte Ch6R2100OhmMSBLow;
        public byte Ch6R2100OhmMSBHigh;

        // 500Ohm
        public byte Ch1Writer500OhmLSBLow;
        public byte Ch1Writer500OhmLSBMid;
        public byte Ch1Writer500OhmMSBLow;
        public byte Ch1Writer500OhmMSBHigh;

        public byte Ch2TA500OhmLSBLow;
        public byte Ch2TA500OhmLSBMid;
        public byte Ch2TA500OhmMSBLow;
        public byte Ch2TA500OhmMSBHigh;

        public byte Ch3WH500OhmLSBLow;
        public byte Ch3WH500OhmLSBMid;
        public byte Ch3WH500OhmMSBLow;
        public byte Ch3WH500OhmMSBHigh;

        public byte Ch4RH500OhmLSBLow;
        public byte Ch4RH500OhmLSBMid;
        public byte Ch4RH500OhmMSBLow;
        public byte Ch4RH500OhmMSBHigh;

        public byte Ch5R1500OhmLSBLow;
        public byte Ch5R1500OhmLSBMid;
        public byte Ch5R1500OhmMSBLow;
        public byte Ch5R1500OhmMSBHigh;

        public byte Ch6R2500OhmLSBLow;
        public byte Ch6R2500OhmLSBMid;
        public byte Ch6R2500OhmMSBLow;
        public byte Ch6R2500OhmMSBHigh;

        // 1kOhm
        public byte Ch1Writer1kOhmLSBLow;
        public byte Ch1Writer1kOhmLSBMid;
        public byte Ch1Writer1kOhmMSBLow;
        public byte Ch1Writer1kOhmMSBHigh;

        public byte Ch2TA1kOhmLSBLow;
        public byte Ch2TA1kOhmLSBMid;
        public byte Ch2TA1kOhmMSBLow;
        public byte Ch2TA1kOhmMSBHigh;

        public byte Ch3WH1kOhmLSBLow;
        public byte Ch3WH1kOhmLSBMid;
        public byte Ch3WH1kOhmMSBLow;
        public byte Ch3WH1kOhmMSBHigh;

        public byte Ch4RH1kOhmLSBLow;
        public byte Ch4RH1kOhmLSBMid;
        public byte Ch4RH1kOhmMSBLow;
        public byte Ch4RH1kOhmMSBHigh;

        public byte Ch5R11kOhmLSBLow;
        public byte Ch5R11kOhmLSBMid;
        public byte Ch5R11kOhmMSBLow;
        public byte Ch5R11kOhmMSBHigh;

        public byte Ch6R21kOhmLSBLow;
        public byte Ch6R21kOhmLSBMid;
        public byte Ch6R21kOhmMSBLow;
        public byte Ch6R21kOhmMSBHigh;

        // 10kOhm
        public byte Ch1Writer10kOhmLSBLow;
        public byte Ch1Writer10kOhmLSBMid;
        public byte Ch1Writer10kOhmMSBLow;
        public byte Ch1Writer10kOhmMSBHigh;

        public byte Ch2TA10kOhmLSBLow;
        public byte Ch2TA10kOhmLSBMid;
        public byte Ch2TA10kOhmMSBLow;
        public byte Ch2TA10kOhmMSBHigh;

        public byte Ch3WH10kOhmLSBLow;
        public byte Ch3WH10kOhmLSBMid;
        public byte Ch3WH10kOhmMSBLow;
        public byte Ch3WH10kOhmMSBHigh;

        public byte Ch4RH10kOhmLSBLow;
        public byte Ch4RH10kOhmLSBMid;
        public byte Ch4RH10kOhmMSBLow;
        public byte Ch4RH10kOhmMSBHigh;

        public byte Ch5R110kOhmLSBLow;
        public byte Ch5R110kOhmLSBMid;
        public byte Ch5R110kOhmMSBLow;
        public byte Ch5R110kOhmMSBHigh;

        public byte Ch6R210kOhmLSBLow;
        public byte Ch6R210kOhmLSBMid;
        public byte Ch6R210kOhmMSBLow;
        public byte Ch6R210kOhmMSBHigh;

        //100pF
        public byte Capacitance100pFLSBLow;
        public byte Capacitance100pFLSBMid;
        public byte Capacitance100pFMSBLow;
        public byte Capacitance100pFMSBHigh;

        //270pF
        public byte Capacitance270pFLSBLow;
        public byte Capacitance270pFLSBMid;
        public byte Capacitance270pFMSBLow;
        public byte Capacitance270pFMSBHigh;

        //470pF
        public byte Capacitance470pFLSBLow;
        public byte Capacitance470pFLSBMid;
        public byte Capacitance470pFMSBLow;
        public byte Capacitance470pFMSBHigh;

        //680pF
        public byte Capacitance680pFLSBLow;
        public byte Capacitance680pFLSBMid;
        public byte Capacitance680pFMSBLow;
        public byte Capacitance680pFMSBHigh;

        //820pF
        public byte Capacitance820pFLSBLow;
        public byte Capacitance820pFLSBMid;
        public byte Capacitance820pFMSBLow;
        public byte Capacitance820pFMSBHigh;

        //10nF
        public byte Capacitance10nFLSBLow;
        public byte Capacitance10nFLSBMid;
        public byte Capacitance10nFMSBLow;
        public byte Capacitance10nFMSBHigh;    

        public static TestProbe20GetCalibrationData ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe20GetCalibrationData*)pb;
            }
        }


        //0Ohm
        public int Ch1Writer0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1Writer0OhmLSBLow;
            array[1] = Ch1Writer0OhmLSBMid;
            array[2] = Ch1Writer0OhmMSBLow;
            array[3] = Ch1Writer0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch2TA0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TA0OhmLSBLow;
            array[1] = Ch2TA0OhmLSBMid;
            array[2] = Ch2TA0OhmMSBLow;
            array[3] = Ch2TA0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch3WH0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WH0OhmLSBLow;
            array[1] = Ch3WH0OhmLSBMid;
            array[2] = Ch3WH0OhmMSBLow;
            array[3] = Ch3WH0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch4RH0Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RH0OhmLSBLow;
            array[1] = Ch4RH0OhmLSBMid;
            array[2] = Ch4RH0OhmMSBLow;
            array[3] = Ch4RH0OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch5R10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R10OhmLSBLow;
            array[1] = Ch5R10OhmLSBMid;
            array[2] = Ch5R10OhmMSBLow;
            array[3] = Ch5R10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch6R20Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R20OhmLSBLow;
            array[1] = Ch6R20OhmLSBMid;
            array[2] = Ch6R20OhmMSBLow;
            array[3] = Ch6R20OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //10Ohm
        public int Ch1Writer10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1Writer10OhmLSBLow;
            array[1] = Ch1Writer10OhmLSBMid;
            array[2] = Ch1Writer10OhmMSBLow;
            array[3] = Ch1Writer10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch2TA10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TA10OhmLSBLow;
            array[1] = Ch2TA10OhmLSBMid;
            array[2] = Ch2TA10OhmMSBLow;
            array[3] = Ch2TA10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch3WH10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WH10OhmLSBLow;
            array[1] = Ch3WH10OhmLSBMid;
            array[2] = Ch3WH10OhmMSBLow;
            array[3] = Ch3WH10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch4RH10Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RH10OhmLSBLow;
            array[1] = Ch4RH10OhmLSBMid;
            array[2] = Ch4RH10OhmMSBLow;
            array[3] = Ch4RH10OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch5R110Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R110OhmLSBLow;
            array[1] = Ch5R110OhmLSBMid;
            array[2] = Ch5R110OhmMSBLow;
            array[3] = Ch5R110OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch6R210Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R210OhmLSBLow;
            array[1] = Ch6R210OhmLSBMid;
            array[2] = Ch6R210OhmMSBLow;
            array[3] = Ch6R210OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //100Ohm
        public int Ch1Writer100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1Writer100OhmLSBLow;
            array[1] = Ch1Writer100OhmLSBMid;
            array[2] = Ch1Writer100OhmMSBLow;
            array[3] = Ch1Writer100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch2TA100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TA100OhmLSBLow;
            array[1] = Ch2TA100OhmLSBMid;
            array[2] = Ch2TA100OhmMSBLow;
            array[3] = Ch2TA100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch3WH100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WH100OhmLSBLow;
            array[1] = Ch3WH100OhmLSBMid;
            array[2] = Ch3WH100OhmMSBLow;
            array[3] = Ch3WH100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch4RH100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RH100OhmLSBLow;
            array[1] = Ch4RH100OhmLSBMid;
            array[2] = Ch4RH100OhmMSBLow;
            array[3] = Ch4RH100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch5R1100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1100OhmLSBLow;
            array[1] = Ch5R1100OhmLSBMid;
            array[2] = Ch5R1100OhmMSBLow;
            array[3] = Ch5R1100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch6R2100Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2100OhmLSBLow;
            array[1] = Ch6R2100OhmLSBMid;
            array[2] = Ch6R2100OhmMSBLow;
            array[3] = Ch6R2100OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //500Ohm
        public int Ch1Writer500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1Writer500OhmLSBLow;
            array[1] = Ch1Writer500OhmLSBMid;
            array[2] = Ch1Writer500OhmMSBLow;
            array[3] = Ch1Writer500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch2TA500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TA500OhmLSBLow;
            array[1] = Ch2TA500OhmLSBMid;
            array[2] = Ch2TA500OhmMSBLow;
            array[3] = Ch2TA500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch3WH500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WH500OhmLSBLow;
            array[1] = Ch3WH500OhmLSBMid;
            array[2] = Ch3WH500OhmMSBLow;
            array[3] = Ch3WH500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch4RH500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RH500OhmLSBLow;
            array[1] = Ch4RH500OhmLSBMid;
            array[2] = Ch4RH500OhmMSBLow;
            array[3] = Ch4RH500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch5R1500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R1500OhmLSBLow;
            array[1] = Ch5R1500OhmLSBMid;
            array[2] = Ch5R1500OhmMSBLow;
            array[3] = Ch5R1500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch6R2500Ohm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R2500OhmLSBLow;
            array[1] = Ch6R2500OhmLSBMid;
            array[2] = Ch6R2500OhmMSBLow;
            array[3] = Ch6R2500OhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //1kOhm
        public int Ch1Writer1kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1Writer1kOhmLSBLow;
            array[1] = Ch1Writer1kOhmLSBMid;
            array[2] = Ch1Writer1kOhmMSBLow;
            array[3] = Ch1Writer1kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch2TA1kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TA1kOhmLSBLow;
            array[1] = Ch2TA1kOhmLSBMid;
            array[2] = Ch2TA1kOhmMSBLow;
            array[3] = Ch2TA1kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch3WH1kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WH1kOhmLSBLow;
            array[1] = Ch3WH1kOhmLSBMid;
            array[2] = Ch3WH1kOhmMSBLow;
            array[3] = Ch3WH1kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch4RH1kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RH1kOhmLSBLow;
            array[1] = Ch4RH1kOhmLSBMid;
            array[2] = Ch4RH1kOhmMSBLow;
            array[3] = Ch4RH1kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch5R11kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R11kOhmLSBLow;
            array[1] = Ch5R11kOhmLSBMid;
            array[2] = Ch5R11kOhmMSBLow;
            array[3] = Ch5R11kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch6R21kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R21kOhmLSBLow;
            array[1] = Ch6R21kOhmLSBMid;
            array[2] = Ch6R21kOhmMSBLow;
            array[3] = Ch6R21kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //10kOhm
        public int Ch1Writer10kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch1Writer10kOhmLSBLow;
            array[1] = Ch1Writer10kOhmLSBMid;
            array[2] = Ch1Writer10kOhmMSBLow;
            array[3] = Ch1Writer10kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch2TA10kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch2TA10kOhmLSBLow;
            array[1] = Ch2TA10kOhmLSBMid;
            array[2] = Ch2TA10kOhmMSBLow;
            array[3] = Ch2TA10kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch3WH10kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch3WH10kOhmLSBLow;
            array[1] = Ch3WH10kOhmLSBMid;
            array[2] = Ch3WH10kOhmMSBLow;
            array[3] = Ch3WH10kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch4RH10kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch4RH10kOhmLSBLow;
            array[1] = Ch4RH10kOhmLSBMid;
            array[2] = Ch4RH10kOhmMSBLow;
            array[3] = Ch4RH10kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch5R110kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch5R110kOhmLSBLow;
            array[1] = Ch5R110kOhmLSBMid;
            array[2] = Ch5R110kOhmMSBLow;
            array[3] = Ch5R110kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int Ch6R210kOhm()
        {
            byte[] array = new byte[4];
            array[0] = Ch6R210kOhmLSBLow;
            array[1] = Ch6R210kOhmLSBMid;
            array[2] = Ch6R210kOhmMSBLow;
            array[3] = Ch6R210kOhmMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //100pF
        public int Capacitance100pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance100pFLSBLow;
            array[1] = Capacitance100pFLSBMid;
            array[2] = Capacitance100pFMSBLow;
            array[3] = Capacitance100pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //270pF
        public int Capacitance270pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance270pFLSBLow;
            array[1] = Capacitance270pFLSBMid;
            array[2] = Capacitance270pFMSBLow;
            array[3] = Capacitance270pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //470pF
        public int Capacitance470pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance470pFLSBLow;
            array[1] = Capacitance470pFLSBMid;
            array[2] = Capacitance470pFMSBLow;
            array[3] = Capacitance470pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //680pF
        public int Capacitance680pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance680pFLSBLow;
            array[1] = Capacitance680pFLSBMid;
            array[2] = Capacitance680pFMSBLow;
            array[3] = Capacitance680pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //820pF
        public int Capacitance820pF()
        {
            byte[] array = new byte[4];
            array[0] = Capacitance820pFLSBLow;
            array[1] = Capacitance820pFLSBMid;
            array[2] = Capacitance820pFMSBLow;
            array[3] = Capacitance820pFMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        //10nF
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
