using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbe10GetAllHGAShortDetection
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;
        public byte ErrorCode;

        // HGA1
        public byte HGA1WPlusPad;
        public byte HGA1WMinusPad;
        public byte HGA1TAPlusPad;
        public byte HGA1TAMinusPad;
        public byte HGA1WHPlusPad;
        public byte HGA1WHMinusPad;
        public byte HGA1RHPlusPad;
        public byte HGA1RHMinusPad;
        public byte HGA1R1PlusPad;
        public byte HGA1R1MinusPad;
        public byte HGA1R2PlusPad;
        public byte HGA1R2MinusPad;

        // HGA2
        public byte HGA2WPlusPad;
        public byte HGA2WMinusPad;
        public byte HGA2TAPlusPad;
        public byte HGA2TAMinusPad;
        public byte HGA2WHPlusPad;
        public byte HGA2WHMinusPad;
        public byte HGA2RHPlusPad;
        public byte HGA2RHMinusPad;
        public byte HGA2R1PlusPad;
        public byte HGA2R1MinusPad;
        public byte HGA2R2PlusPad;
        public byte HGA2R2MinusPad;

        // HGA3
        public byte HGA3WPlusPad;
        public byte HGA3WMinusPad;
        public byte HGA3TAPlusPad;
        public byte HGA3TAMinusPad;
        public byte HGA3WHPlusPad;
        public byte HGA3WHMinusPad;
        public byte HGA3RHPlusPad;
        public byte HGA3RHMinusPad;
        public byte HGA3R1PlusPad;
        public byte HGA3R1MinusPad;
        public byte HGA3R2PlusPad;
        public byte HGA3R2MinusPad;

        // HGA4
        public byte HGA4WPlusPad;
        public byte HGA4WMinusPad;
        public byte HGA4TAPlusPad;
        public byte HGA4TAMinusPad;
        public byte HGA4WHPlusPad;
        public byte HGA4WHMinusPad;
        public byte HGA4RHPlusPad;
        public byte HGA4RHMinusPad;
        public byte HGA4R1PlusPad;
        public byte HGA4R1MinusPad;
        public byte HGA4R2PlusPad;
        public byte HGA4R2MinusPad;

        // HGA5
        public byte HGA5WPlusPad;
        public byte HGA5WMinusPad;
        public byte HGA5TAPlusPad;
        public byte HGA5TAMinusPad;
        public byte HGA5WHPlusPad;
        public byte HGA5WHMinusPad;
        public byte HGA5RHPlusPad;
        public byte HGA5RHMinusPad;
        public byte HGA5R1PlusPad;
        public byte HGA5R1MinusPad;
        public byte HGA5R2PlusPad;
        public byte HGA5R2MinusPad;

        // HGA6
        public byte HGA6WPlusPad;
        public byte HGA6WMinusPad;
        public byte HGA6TAPlusPad;
        public byte HGA6TAMinusPad;
        public byte HGA6WHPlusPad;
        public byte HGA6WHMinusPad;
        public byte HGA6RHPlusPad;
        public byte HGA6RHMinusPad;
        public byte HGA6R1PlusPad;
        public byte HGA6R1MinusPad;
        public byte HGA6R2PlusPad;
        public byte HGA6R2MinusPad;

        // HGA7
        public byte HGA7WPlusPad;
        public byte HGA7WMinusPad;
        public byte HGA7TAPlusPad;
        public byte HGA7TAMinusPad;
        public byte HGA7WHPlusPad;
        public byte HGA7WHMinusPad;
        public byte HGA7RHPlusPad;
        public byte HGA7RHMinusPad;
        public byte HGA7R1PlusPad;
        public byte HGA7R1MinusPad;
        public byte HGA7R2PlusPad;
        public byte HGA7R2MinusPad;

        // HGA8
        public byte HGA8WPlusPad;
        public byte HGA8WMinusPad;
        public byte HGA8TAPlusPad;
        public byte HGA8TAMinusPad;
        public byte HGA8WHPlusPad;
        public byte HGA8WHMinusPad;
        public byte HGA8RHPlusPad;
        public byte HGA8RHMinusPad;
        public byte HGA8R1PlusPad;
        public byte HGA8R1MinusPad;
        public byte HGA8R2PlusPad;
        public byte HGA8R2MinusPad;

        // HGA9
        public byte HGA9WPlusPad;
        public byte HGA9WMinusPad;
        public byte HGA9TAPlusPad;
        public byte HGA9TAMinusPad;
        public byte HGA9WHPlusPad;
        public byte HGA9WHMinusPad;
        public byte HGA9RHPlusPad;
        public byte HGA9RHMinusPad;
        public byte HGA9R1PlusPad;
        public byte HGA9R1MinusPad;
        public byte HGA9R2PlusPad;
        public byte HGA9R2MinusPad;

        // HGA10
        public byte HGA10WPlusPad;
        public byte HGA10WMinusPad;
        public byte HGA10TAPlusPad;
        public byte HGA10TAMinusPad;
        public byte HGA10WHPlusPad;
        public byte HGA10WHMinusPad;
        public byte HGA10RHPlusPad;
        public byte HGA10RHMinusPad;
        public byte HGA10R1PlusPad;
        public byte HGA10R1MinusPad;
        public byte HGA10R2PlusPad;
        public byte HGA10R2MinusPad;        

        public static TestProbe10GetAllHGAShortDetection ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbe10GetAllHGAShortDetection*)pb;
            }
        } 
    }
}
