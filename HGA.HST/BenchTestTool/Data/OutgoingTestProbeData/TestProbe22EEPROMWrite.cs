using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe22EEPROMWrite
    {
        // EEPROM Start Adress
        public byte EEPROMStartAddressLSB;
        public byte EEPROMStartAddressMSB;

        public byte NumberOfDataBytesToWrite;  // Maximum data byte size is 64
        public byte DataByte1;        
    }
}
