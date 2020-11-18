using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe45SetCableCompensation
    {
        public byte FlexCableIndex;                          
        public byte HGAIndex;
        public byte ChannelNumber;

        public byte ResistanceOrCapacitanceSelection; 

        // Compensation Resistance or Capacitance
        public byte CompensationResistanceCapacitanceLSBLow;
        public byte CompensationResistanceCapacitanceLSBMid;
        public byte CompensationResistanceCapacitanceMSBLow;
        public byte CompensationResistanceCapacitanceMSBHigh;
    }
}
