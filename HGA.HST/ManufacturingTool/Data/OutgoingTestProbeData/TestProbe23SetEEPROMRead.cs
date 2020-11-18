using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe23SetEEPROMRead
    {
        // EEPROM Start Adress
        public byte EEPROMStartAddressLSB;
        public byte EEPROMStartAddressMSB;

        public byte NumberOfDataBytesToRead;  // Maximum data byte size is 64        
    }
}
