using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe23GetEEPROMRead
    {
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;
        
        public byte DataByte1;
        public byte DataByte2;
        public byte DataByte3;
        public byte DataByte4;
        public byte DataByte5;
        public byte DataByte6;
        public byte DataByte7;
        public byte DataByte8;
        public byte DataByte9;
        public byte DataByte10;
        public byte DataByte11;
        public byte DataByte12;
        public byte DataByte13;
        public byte DataByte14;
        public byte DataByte15;
        public byte DataByte16;
        public byte DataByte17;
        public byte DataByte18;
        public byte DataByte19;
        public byte DataByte20;
        public byte DataByte21;
        public byte DataByte22;
        public byte DataByte23;
        public byte DataByte24;
        public byte DataByte25;
        public byte DataByte26;
        public byte DataByte27;
        public byte DataByte28;
        public byte DataByte29;
        public byte DataByte30;
        public byte DataByte31;
        public byte DataByte32;
        public byte DataByte33;
        public byte DataByte34;
        public byte DataByte35;
        public byte DataByte36;
        public byte DataByte37;
        public byte DataByte38;
        public byte DataByte39;
        public byte DataByte40;
        public byte DataByte41;
        public byte DataByte42;
        public byte DataByte43;
        public byte DataByte44;
        public byte DataByte45;
        public byte DataByte46;
        public byte DataByte47;
        public byte DataByte48;
        public byte DataByte49;
        public byte DataByte50;
        public byte DataByte51;
        public byte DataByte52;
        public byte DataByte53;
        public byte DataByte54;
        public byte DataByte55;
        public byte DataByte56;
        public byte DataByte57;
        public byte DataByte58;
        public byte DataByte59;
        public byte DataByte60;
        public byte DataByte61;
        public byte DataByte62;
        public byte DataByte63;
        public byte DataByte64;   

        public static TestProbe23GetEEPROMRead ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe23GetEEPROMRead*)pb;
            }
        } 

             
    }
}
