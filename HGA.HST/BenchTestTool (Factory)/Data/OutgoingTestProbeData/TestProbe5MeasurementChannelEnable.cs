using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.OutgoingTestProbeData
{
    public unsafe struct TestProbe5MeasurementChannelEnable
    {        
        public byte ResistanceCh1Writer;
        public byte ResistanceCh2TA;
        public byte ResistanceCh3WriteHeater;
        public byte ResistanceCh4ReadHeater;
        public byte ResistanceCh5Read1;
        public byte ResistanceCh6Read2;
        public byte CapacitanceCh1;
        public byte CapacitanceCh2;
    }
}
