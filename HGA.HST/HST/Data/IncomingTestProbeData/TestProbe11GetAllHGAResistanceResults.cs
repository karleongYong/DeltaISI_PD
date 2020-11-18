using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe11GetAllHGAResistanceResults
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;        

        // HGA1        
        public byte HGA1Ch1WriterResistanceLSBLow;
        public byte HGA1Ch1WriterResistanceLSBMid;
        public byte HGA1Ch1WriterResistanceMSBLow;
        public byte HGA1Ch1WriterResistanceMSBHigh;
        
        public byte HGA1Ch2TAResistanceLSBLow;
        public byte HGA1Ch2TAResistanceLSBMid;
        public byte HGA1Ch2TAResistanceMSBLow;
        public byte HGA1Ch2TAResistanceMSBHigh;
        
        public byte HGA1Ch3WHResistanceLSBLow;
        public byte HGA1Ch3WHResistanceLSBMid;
        public byte HGA1Ch3WHResistanceMSBLow;
        public byte HGA1Ch3WHResistanceMSBHigh;
        
        public byte HGA1Ch4RHResistanceLSBLow;
        public byte HGA1Ch4RHResistanceLSBMid;
        public byte HGA1Ch4RHResistanceMSBLow;
        public byte HGA1Ch4RHResistanceMSBHigh;
        
        public byte HGA1Ch5R1ResistanceLSBLow;
        public byte HGA1Ch5R1ResistanceLSBMid;
        public byte HGA1Ch5R1ResistanceMSBLow;
        public byte HGA1Ch5R1ResistanceMSBHigh;
        
        public byte HGA1Ch6R2ResistanceLSBLow;
        public byte HGA1Ch6R2ResistanceLSBMid;
        public byte HGA1Ch6R2ResistanceMSBLow;
        public byte HGA1Ch6R2ResistanceMSBHigh;

        // HGA2
        public byte HGA2Ch1WriterResistanceLSBLow;
        public byte HGA2Ch1WriterResistanceLSBMid;
        public byte HGA2Ch1WriterResistanceMSBLow;
        public byte HGA2Ch1WriterResistanceMSBHigh;

        public byte HGA2Ch2TAResistanceLSBLow;
        public byte HGA2Ch2TAResistanceLSBMid;
        public byte HGA2Ch2TAResistanceMSBLow;
        public byte HGA2Ch2TAResistanceMSBHigh;

        public byte HGA2Ch3WHResistanceLSBLow;
        public byte HGA2Ch3WHResistanceLSBMid;
        public byte HGA2Ch3WHResistanceMSBLow;
        public byte HGA2Ch3WHResistanceMSBHigh;

        public byte HGA2Ch4RHResistanceLSBLow;
        public byte HGA2Ch4RHResistanceLSBMid;
        public byte HGA2Ch4RHResistanceMSBLow;
        public byte HGA2Ch4RHResistanceMSBHigh;

        public byte HGA2Ch5R1ResistanceLSBLow;
        public byte HGA2Ch5R1ResistanceLSBMid;
        public byte HGA2Ch5R1ResistanceMSBLow;
        public byte HGA2Ch5R1ResistanceMSBHigh;

        public byte HGA2Ch6R2ResistanceLSBLow;
        public byte HGA2Ch6R2ResistanceLSBMid;
        public byte HGA2Ch6R2ResistanceMSBLow;
        public byte HGA2Ch6R2ResistanceMSBHigh;

        // HGA3
        public byte HGA3Ch1WriterResistanceLSBLow;
        public byte HGA3Ch1WriterResistanceLSBMid;
        public byte HGA3Ch1WriterResistanceMSBLow;
        public byte HGA3Ch1WriterResistanceMSBHigh;

        public byte HGA3Ch2TAResistanceLSBLow;
        public byte HGA3Ch2TAResistanceLSBMid;
        public byte HGA3Ch2TAResistanceMSBLow;
        public byte HGA3Ch2TAResistanceMSBHigh;

        public byte HGA3Ch3WHResistanceLSBLow;
        public byte HGA3Ch3WHResistanceLSBMid;
        public byte HGA3Ch3WHResistanceMSBLow;
        public byte HGA3Ch3WHResistanceMSBHigh;

        public byte HGA3Ch4RHResistanceLSBLow;
        public byte HGA3Ch4RHResistanceLSBMid;
        public byte HGA3Ch4RHResistanceMSBLow;
        public byte HGA3Ch4RHResistanceMSBHigh;

        public byte HGA3Ch5R1ResistanceLSBLow;
        public byte HGA3Ch5R1ResistanceLSBMid;
        public byte HGA3Ch5R1ResistanceMSBLow;
        public byte HGA3Ch5R1ResistanceMSBHigh;

        public byte HGA3Ch6R2ResistanceLSBLow;
        public byte HGA3Ch6R2ResistanceLSBMid;
        public byte HGA3Ch6R2ResistanceMSBLow;
        public byte HGA3Ch6R2ResistanceMSBHigh;

        // HGA4
        public byte HGA4Ch1WriterResistanceLSBLow;
        public byte HGA4Ch1WriterResistanceLSBMid;
        public byte HGA4Ch1WriterResistanceMSBLow;
        public byte HGA4Ch1WriterResistanceMSBHigh;

        public byte HGA4Ch2TAResistanceLSBLow;
        public byte HGA4Ch2TAResistanceLSBMid;
        public byte HGA4Ch2TAResistanceMSBLow;
        public byte HGA4Ch2TAResistanceMSBHigh;

        public byte HGA4Ch3WHResistanceLSBLow;
        public byte HGA4Ch3WHResistanceLSBMid;
        public byte HGA4Ch3WHResistanceMSBLow;
        public byte HGA4Ch3WHResistanceMSBHigh;

        public byte HGA4Ch4RHResistanceLSBLow;
        public byte HGA4Ch4RHResistanceLSBMid;
        public byte HGA4Ch4RHResistanceMSBLow;
        public byte HGA4Ch4RHResistanceMSBHigh;

        public byte HGA4Ch5R1ResistanceLSBLow;
        public byte HGA4Ch5R1ResistanceLSBMid;
        public byte HGA4Ch5R1ResistanceMSBLow;
        public byte HGA4Ch5R1ResistanceMSBHigh;

        public byte HGA4Ch6R2ResistanceLSBLow;
        public byte HGA4Ch6R2ResistanceLSBMid;
        public byte HGA4Ch6R2ResistanceMSBLow;
        public byte HGA4Ch6R2ResistanceMSBHigh;

        // HGA5
        public byte HGA5Ch1WriterResistanceLSBLow;
        public byte HGA5Ch1WriterResistanceLSBMid;
        public byte HGA5Ch1WriterResistanceMSBLow;
        public byte HGA5Ch1WriterResistanceMSBHigh;

        public byte HGA5Ch2TAResistanceLSBLow;
        public byte HGA5Ch2TAResistanceLSBMid;
        public byte HGA5Ch2TAResistanceMSBLow;
        public byte HGA5Ch2TAResistanceMSBHigh;

        public byte HGA5Ch3WHResistanceLSBLow;
        public byte HGA5Ch3WHResistanceLSBMid;
        public byte HGA5Ch3WHResistanceMSBLow;
        public byte HGA5Ch3WHResistanceMSBHigh;

        public byte HGA5Ch4RHResistanceLSBLow;
        public byte HGA5Ch4RHResistanceLSBMid;
        public byte HGA5Ch4RHResistanceMSBLow;
        public byte HGA5Ch4RHResistanceMSBHigh;

        public byte HGA5Ch5R1ResistanceLSBLow;
        public byte HGA5Ch5R1ResistanceLSBMid;
        public byte HGA5Ch5R1ResistanceMSBLow;
        public byte HGA5Ch5R1ResistanceMSBHigh;

        public byte HGA5Ch6R2ResistanceLSBLow;
        public byte HGA5Ch6R2ResistanceLSBMid;
        public byte HGA5Ch6R2ResistanceMSBLow;
        public byte HGA5Ch6R2ResistanceMSBHigh;

        // HGA6
        public byte HGA6Ch1WriterResistanceLSBLow;
        public byte HGA6Ch1WriterResistanceLSBMid;
        public byte HGA6Ch1WriterResistanceMSBLow;
        public byte HGA6Ch1WriterResistanceMSBHigh;

        public byte HGA6Ch2TAResistanceLSBLow;
        public byte HGA6Ch2TAResistanceLSBMid;
        public byte HGA6Ch2TAResistanceMSBLow;
        public byte HGA6Ch2TAResistanceMSBHigh;

        public byte HGA6Ch3WHResistanceLSBLow;
        public byte HGA6Ch3WHResistanceLSBMid;
        public byte HGA6Ch3WHResistanceMSBLow;
        public byte HGA6Ch3WHResistanceMSBHigh;

        public byte HGA6Ch4RHResistanceLSBLow;
        public byte HGA6Ch4RHResistanceLSBMid;
        public byte HGA6Ch4RHResistanceMSBLow;
        public byte HGA6Ch4RHResistanceMSBHigh;

        public byte HGA6Ch5R1ResistanceLSBLow;
        public byte HGA6Ch5R1ResistanceLSBMid;
        public byte HGA6Ch5R1ResistanceMSBLow;
        public byte HGA6Ch5R1ResistanceMSBHigh;

        public byte HGA6Ch6R2ResistanceLSBLow;
        public byte HGA6Ch6R2ResistanceLSBMid;
        public byte HGA6Ch6R2ResistanceMSBLow;
        public byte HGA6Ch6R2ResistanceMSBHigh;

        // HGA7
        public byte HGA7Ch1WriterResistanceLSBLow;
        public byte HGA7Ch1WriterResistanceLSBMid;
        public byte HGA7Ch1WriterResistanceMSBLow;
        public byte HGA7Ch1WriterResistanceMSBHigh;

        public byte HGA7Ch2TAResistanceLSBLow;
        public byte HGA7Ch2TAResistanceLSBMid;
        public byte HGA7Ch2TAResistanceMSBLow;
        public byte HGA7Ch2TAResistanceMSBHigh;

        public byte HGA7Ch3WHResistanceLSBLow;
        public byte HGA7Ch3WHResistanceLSBMid;
        public byte HGA7Ch3WHResistanceMSBLow;
        public byte HGA7Ch3WHResistanceMSBHigh;

        public byte HGA7Ch4RHResistanceLSBLow;
        public byte HGA7Ch4RHResistanceLSBMid;
        public byte HGA7Ch4RHResistanceMSBLow;
        public byte HGA7Ch4RHResistanceMSBHigh;

        public byte HGA7Ch5R1ResistanceLSBLow;
        public byte HGA7Ch5R1ResistanceLSBMid;
        public byte HGA7Ch5R1ResistanceMSBLow;
        public byte HGA7Ch5R1ResistanceMSBHigh;

        public byte HGA7Ch6R2ResistanceLSBLow;
        public byte HGA7Ch6R2ResistanceLSBMid;
        public byte HGA7Ch6R2ResistanceMSBLow;
        public byte HGA7Ch6R2ResistanceMSBHigh;

        // HGA8
        public byte HGA8Ch1WriterResistanceLSBLow;
        public byte HGA8Ch1WriterResistanceLSBMid;
        public byte HGA8Ch1WriterResistanceMSBLow;
        public byte HGA8Ch1WriterResistanceMSBHigh;

        public byte HGA8Ch2TAResistanceLSBLow;
        public byte HGA8Ch2TAResistanceLSBMid;
        public byte HGA8Ch2TAResistanceMSBLow;
        public byte HGA8Ch2TAResistanceMSBHigh;

        public byte HGA8Ch3WHResistanceLSBLow;
        public byte HGA8Ch3WHResistanceLSBMid;
        public byte HGA8Ch3WHResistanceMSBLow;
        public byte HGA8Ch3WHResistanceMSBHigh;

        public byte HGA8Ch4RHResistanceLSBLow;
        public byte HGA8Ch4RHResistanceLSBMid;
        public byte HGA8Ch4RHResistanceMSBLow;
        public byte HGA8Ch4RHResistanceMSBHigh;

        public byte HGA8Ch5R1ResistanceLSBLow;
        public byte HGA8Ch5R1ResistanceLSBMid;
        public byte HGA8Ch5R1ResistanceMSBLow;
        public byte HGA8Ch5R1ResistanceMSBHigh;

        public byte HGA8Ch6R2ResistanceLSBLow;
        public byte HGA8Ch6R2ResistanceLSBMid;
        public byte HGA8Ch6R2ResistanceMSBLow;
        public byte HGA8Ch6R2ResistanceMSBHigh;

        // HGA9
        public byte HGA9Ch1WriterResistanceLSBLow;
        public byte HGA9Ch1WriterResistanceLSBMid;
        public byte HGA9Ch1WriterResistanceMSBLow;
        public byte HGA9Ch1WriterResistanceMSBHigh;

        public byte HGA9Ch2TAResistanceLSBLow;
        public byte HGA9Ch2TAResistanceLSBMid;
        public byte HGA9Ch2TAResistanceMSBLow;
        public byte HGA9Ch2TAResistanceMSBHigh;

        public byte HGA9Ch3WHResistanceLSBLow;
        public byte HGA9Ch3WHResistanceLSBMid;
        public byte HGA9Ch3WHResistanceMSBLow;
        public byte HGA9Ch3WHResistanceMSBHigh;

        public byte HGA9Ch4RHResistanceLSBLow;
        public byte HGA9Ch4RHResistanceLSBMid;
        public byte HGA9Ch4RHResistanceMSBLow;
        public byte HGA9Ch4RHResistanceMSBHigh;

        public byte HGA9Ch5R1ResistanceLSBLow;
        public byte HGA9Ch5R1ResistanceLSBMid;
        public byte HGA9Ch5R1ResistanceMSBLow;
        public byte HGA9Ch5R1ResistanceMSBHigh;

        public byte HGA9Ch6R2ResistanceLSBLow;
        public byte HGA9Ch6R2ResistanceLSBMid;
        public byte HGA9Ch6R2ResistanceMSBLow;
        public byte HGA9Ch6R2ResistanceMSBHigh;

        // HGA10
        public byte HGA10Ch1WriterResistanceLSBLow;
        public byte HGA10Ch1WriterResistanceLSBMid;
        public byte HGA10Ch1WriterResistanceMSBLow;
        public byte HGA10Ch1WriterResistanceMSBHigh;

        public byte HGA10Ch2TAResistanceLSBLow;
        public byte HGA10Ch2TAResistanceLSBMid;
        public byte HGA10Ch2TAResistanceMSBLow;
        public byte HGA10Ch2TAResistanceMSBHigh;

        public byte HGA10Ch3WHResistanceLSBLow;
        public byte HGA10Ch3WHResistanceLSBMid;
        public byte HGA10Ch3WHResistanceMSBLow;
        public byte HGA10Ch3WHResistanceMSBHigh;

        public byte HGA10Ch4RHResistanceLSBLow;
        public byte HGA10Ch4RHResistanceLSBMid;
        public byte HGA10Ch4RHResistanceMSBLow;
        public byte HGA10Ch4RHResistanceMSBHigh;

        public byte HGA10Ch5R1ResistanceLSBLow;
        public byte HGA10Ch5R1ResistanceLSBMid;
        public byte HGA10Ch5R1ResistanceMSBLow;
        public byte HGA10Ch5R1ResistanceMSBHigh;

        public byte HGA10Ch6R2ResistanceLSBLow;
        public byte HGA10Ch6R2ResistanceLSBMid;
        public byte HGA10Ch6R2ResistanceMSBLow;
        public byte HGA10Ch6R2ResistanceMSBHigh;        

        public static TestProbe11GetAllHGAResistanceResults ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe11GetAllHGAResistanceResults*)pb;
            }
        }

        // HGA1
        public int HGA1Ch1WriterResistance()
        {
             byte[] array = new byte[4];
             array[0] = HGA1Ch1WriterResistanceLSBLow;
             array[1] = HGA1Ch1WriterResistanceLSBMid;
             array[2] = HGA1Ch1WriterResistanceMSBLow;
             array[3] = HGA1Ch1WriterResistanceMSBHigh;
             return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch2TAResistanceLSBLow;
            array[1] = HGA1Ch2TAResistanceLSBMid;
            array[2] = HGA1Ch2TAResistanceMSBLow;
            array[3] = HGA1Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch3WHResistanceLSBLow;
            array[1] = HGA1Ch3WHResistanceLSBMid;
            array[2] = HGA1Ch3WHResistanceMSBLow;
            array[3] = HGA1Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch4RHResistanceLSBLow;
            array[1] = HGA1Ch4RHResistanceLSBMid;
            array[2] = HGA1Ch4RHResistanceMSBLow;
            array[3] = HGA1Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch5R1ResistanceLSBLow;
            array[1] = HGA1Ch5R1ResistanceLSBMid;
            array[2] = HGA1Ch5R1ResistanceMSBLow;
            array[3] = HGA1Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA1Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA1Ch6R2ResistanceLSBLow;
            array[1] = HGA1Ch6R2ResistanceLSBMid;
            array[2] = HGA1Ch6R2ResistanceMSBLow;
            array[3] = HGA1Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA2
        public int HGA2Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch1WriterResistanceLSBLow;
            array[1] = HGA2Ch1WriterResistanceLSBMid;
            array[2] = HGA2Ch1WriterResistanceMSBLow;
            array[3] = HGA2Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch2TAResistanceLSBLow;
            array[1] = HGA2Ch2TAResistanceLSBMid;
            array[2] = HGA2Ch2TAResistanceMSBLow;
            array[3] = HGA2Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch3WHResistanceLSBLow;
            array[1] = HGA2Ch3WHResistanceLSBMid;
            array[2] = HGA2Ch3WHResistanceMSBLow;
            array[3] = HGA2Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch4RHResistanceLSBLow;
            array[1] = HGA2Ch4RHResistanceLSBMid;
            array[2] = HGA2Ch4RHResistanceMSBLow;
            array[3] = HGA2Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch5R1ResistanceLSBLow;
            array[1] = HGA2Ch5R1ResistanceLSBMid;
            array[2] = HGA2Ch5R1ResistanceMSBLow;
            array[3] = HGA2Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA2Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA2Ch6R2ResistanceLSBLow;
            array[1] = HGA2Ch6R2ResistanceLSBMid;
            array[2] = HGA2Ch6R2ResistanceMSBLow;
            array[3] = HGA2Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA3
        public int HGA3Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch1WriterResistanceLSBLow;
            array[1] = HGA3Ch1WriterResistanceLSBMid;
            array[2] = HGA3Ch1WriterResistanceMSBLow;
            array[3] = HGA3Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch2TAResistanceLSBLow;
            array[1] = HGA3Ch2TAResistanceLSBMid;
            array[2] = HGA3Ch2TAResistanceMSBLow;
            array[3] = HGA3Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch3WHResistanceLSBLow;
            array[1] = HGA3Ch3WHResistanceLSBMid;
            array[2] = HGA3Ch3WHResistanceMSBLow;
            array[3] = HGA3Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch4RHResistanceLSBLow;
            array[1] = HGA3Ch4RHResistanceLSBMid;
            array[2] = HGA3Ch4RHResistanceMSBLow;
            array[3] = HGA3Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch5R1ResistanceLSBLow;
            array[1] = HGA3Ch5R1ResistanceLSBMid;
            array[2] = HGA3Ch5R1ResistanceMSBLow;
            array[3] = HGA3Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA3Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA3Ch6R2ResistanceLSBLow;
            array[1] = HGA3Ch6R2ResistanceLSBMid;
            array[2] = HGA3Ch6R2ResistanceMSBLow;
            array[3] = HGA3Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA4
        public int HGA4Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch1WriterResistanceLSBLow;
            array[1] = HGA4Ch1WriterResistanceLSBMid;
            array[2] = HGA4Ch1WriterResistanceMSBLow;
            array[3] = HGA4Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch2TAResistanceLSBLow;
            array[1] = HGA4Ch2TAResistanceLSBMid;
            array[2] = HGA4Ch2TAResistanceMSBLow;
            array[3] = HGA4Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch3WHResistanceLSBLow;
            array[1] = HGA4Ch3WHResistanceLSBMid;
            array[2] = HGA4Ch3WHResistanceMSBLow;
            array[3] = HGA4Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch4RHResistanceLSBLow;
            array[1] = HGA4Ch4RHResistanceLSBMid;
            array[2] = HGA4Ch4RHResistanceMSBLow;
            array[3] = HGA4Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch5R1ResistanceLSBLow;
            array[1] = HGA4Ch5R1ResistanceLSBMid;
            array[2] = HGA4Ch5R1ResistanceMSBLow;
            array[3] = HGA4Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA4Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA4Ch6R2ResistanceLSBLow;
            array[1] = HGA4Ch6R2ResistanceLSBMid;
            array[2] = HGA4Ch6R2ResistanceMSBLow;
            array[3] = HGA4Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA5
        public int HGA5Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch1WriterResistanceLSBLow;
            array[1] = HGA5Ch1WriterResistanceLSBMid;
            array[2] = HGA5Ch1WriterResistanceMSBLow;
            array[3] = HGA5Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch2TAResistanceLSBLow;
            array[1] = HGA5Ch2TAResistanceLSBMid;
            array[2] = HGA5Ch2TAResistanceMSBLow;
            array[3] = HGA5Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch3WHResistanceLSBLow;
            array[1] = HGA5Ch3WHResistanceLSBMid;
            array[2] = HGA5Ch3WHResistanceMSBLow;
            array[3] = HGA5Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch4RHResistanceLSBLow;
            array[1] = HGA5Ch4RHResistanceLSBMid;
            array[2] = HGA5Ch4RHResistanceMSBLow;
            array[3] = HGA5Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch5R1ResistanceLSBLow;
            array[1] = HGA5Ch5R1ResistanceLSBMid;
            array[2] = HGA5Ch5R1ResistanceMSBLow;
            array[3] = HGA5Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA5Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA5Ch6R2ResistanceLSBLow;
            array[1] = HGA5Ch6R2ResistanceLSBMid;
            array[2] = HGA5Ch6R2ResistanceMSBLow;
            array[3] = HGA5Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA6
        public int HGA6Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch1WriterResistanceLSBLow;
            array[1] = HGA6Ch1WriterResistanceLSBMid;
            array[2] = HGA6Ch1WriterResistanceMSBLow;
            array[3] = HGA6Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch2TAResistanceLSBLow;
            array[1] = HGA6Ch2TAResistanceLSBMid;
            array[2] = HGA6Ch2TAResistanceMSBLow;
            array[3] = HGA6Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch3WHResistanceLSBLow;
            array[1] = HGA6Ch3WHResistanceLSBMid;
            array[2] = HGA6Ch3WHResistanceMSBLow;
            array[3] = HGA6Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch4RHResistanceLSBLow;
            array[1] = HGA6Ch4RHResistanceLSBMid;
            array[2] = HGA6Ch4RHResistanceMSBLow;
            array[3] = HGA6Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch5R1ResistanceLSBLow;
            array[1] = HGA6Ch5R1ResistanceLSBMid;
            array[2] = HGA6Ch5R1ResistanceMSBLow;
            array[3] = HGA6Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA6Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA6Ch6R2ResistanceLSBLow;
            array[1] = HGA6Ch6R2ResistanceLSBMid;
            array[2] = HGA6Ch6R2ResistanceMSBLow;
            array[3] = HGA6Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA7
        public int HGA7Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch1WriterResistanceLSBLow;
            array[1] = HGA7Ch1WriterResistanceLSBMid;
            array[2] = HGA7Ch1WriterResistanceMSBLow;
            array[3] = HGA7Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch2TAResistanceLSBLow;
            array[1] = HGA7Ch2TAResistanceLSBMid;
            array[2] = HGA7Ch2TAResistanceMSBLow;
            array[3] = HGA7Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch3WHResistanceLSBLow;
            array[1] = HGA7Ch3WHResistanceLSBMid;
            array[2] = HGA7Ch3WHResistanceMSBLow;
            array[3] = HGA7Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch4RHResistanceLSBLow;
            array[1] = HGA7Ch4RHResistanceLSBMid;
            array[2] = HGA7Ch4RHResistanceMSBLow;
            array[3] = HGA7Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch5R1ResistanceLSBLow;
            array[1] = HGA7Ch5R1ResistanceLSBMid;
            array[2] = HGA7Ch5R1ResistanceMSBLow;
            array[3] = HGA7Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA7Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA7Ch6R2ResistanceLSBLow;
            array[1] = HGA7Ch6R2ResistanceLSBMid;
            array[2] = HGA7Ch6R2ResistanceMSBLow;
            array[3] = HGA7Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA8
        public int HGA8Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch1WriterResistanceLSBLow;
            array[1] = HGA8Ch1WriterResistanceLSBMid;
            array[2] = HGA8Ch1WriterResistanceMSBLow;
            array[3] = HGA8Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch2TAResistanceLSBLow;
            array[1] = HGA8Ch2TAResistanceLSBMid;
            array[2] = HGA8Ch2TAResistanceMSBLow;
            array[3] = HGA8Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch3WHResistanceLSBLow;
            array[1] = HGA8Ch3WHResistanceLSBMid;
            array[2] = HGA8Ch3WHResistanceMSBLow;
            array[3] = HGA8Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch4RHResistanceLSBLow;
            array[1] = HGA8Ch4RHResistanceLSBMid;
            array[2] = HGA8Ch4RHResistanceMSBLow;
            array[3] = HGA8Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch5R1ResistanceLSBLow;
            array[1] = HGA8Ch5R1ResistanceLSBMid;
            array[2] = HGA8Ch5R1ResistanceMSBLow;
            array[3] = HGA8Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA8Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA8Ch6R2ResistanceLSBLow;
            array[1] = HGA8Ch6R2ResistanceLSBMid;
            array[2] = HGA8Ch6R2ResistanceMSBLow;
            array[3] = HGA8Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA9
        public int HGA9Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch1WriterResistanceLSBLow;
            array[1] = HGA9Ch1WriterResistanceLSBMid;
            array[2] = HGA9Ch1WriterResistanceMSBLow;
            array[3] = HGA9Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch2TAResistanceLSBLow;
            array[1] = HGA9Ch2TAResistanceLSBMid;
            array[2] = HGA9Ch2TAResistanceMSBLow;
            array[3] = HGA9Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch3WHResistanceLSBLow;
            array[1] = HGA9Ch3WHResistanceLSBMid;
            array[2] = HGA9Ch3WHResistanceMSBLow;
            array[3] = HGA9Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch4RHResistanceLSBLow;
            array[1] = HGA9Ch4RHResistanceLSBMid;
            array[2] = HGA9Ch4RHResistanceMSBLow;
            array[3] = HGA9Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch5R1ResistanceLSBLow;
            array[1] = HGA9Ch5R1ResistanceLSBMid;
            array[2] = HGA9Ch5R1ResistanceMSBLow;
            array[3] = HGA9Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA9Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA9Ch6R2ResistanceLSBLow;
            array[1] = HGA9Ch6R2ResistanceLSBMid;
            array[2] = HGA9Ch6R2ResistanceMSBLow;
            array[3] = HGA9Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        // HGA10
        public int HGA10Ch1WriterResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch1WriterResistanceLSBLow;
            array[1] = HGA10Ch1WriterResistanceLSBMid;
            array[2] = HGA10Ch1WriterResistanceMSBLow;
            array[3] = HGA10Ch1WriterResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Ch2TAResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch2TAResistanceLSBLow;
            array[1] = HGA10Ch2TAResistanceLSBMid;
            array[2] = HGA10Ch2TAResistanceMSBLow;
            array[3] = HGA10Ch2TAResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Ch3WHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch3WHResistanceLSBLow;
            array[1] = HGA10Ch3WHResistanceLSBMid;
            array[2] = HGA10Ch3WHResistanceMSBLow;
            array[3] = HGA10Ch3WHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Ch4RHResistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch4RHResistanceLSBLow;
            array[1] = HGA10Ch4RHResistanceLSBMid;
            array[2] = HGA10Ch4RHResistanceMSBLow;
            array[3] = HGA10Ch4RHResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Ch5R1Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch5R1ResistanceLSBLow;
            array[1] = HGA10Ch5R1ResistanceLSBMid;
            array[2] = HGA10Ch5R1ResistanceMSBLow;
            array[3] = HGA10Ch5R1ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }

        public int HGA10Ch6R2Resistance()
        {
            byte[] array = new byte[4];
            array[0] = HGA10Ch6R2ResistanceLSBLow;
            array[1] = HGA10Ch6R2ResistanceLSBMid;
            array[2] = HGA10Ch6R2ResistanceMSBLow;
            array[3] = HGA10Ch6R2ResistanceMSBHigh;
            return BitConverter.ToInt32(array, 0);
        }
    }
}
