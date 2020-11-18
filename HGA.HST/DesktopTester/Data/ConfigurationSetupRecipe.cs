using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopTester.Data
{
    public unsafe struct ConfigurationSetupRecipe
    {
        // Ch1BiasCurrent        
        public byte Ch1BiasCurrentLSB;
        public byte Ch1BiasCurrentMSB;
        public int Ch1BiasCurrent;

        // Ch2        
        public byte Ch2BiasCurrentLSB;
        public byte Ch2BiasCurrentMSB;
        public int Ch2BiasCurrent;

        // Ch3        
        public byte Ch3BiasCurrentLSB;
        public byte Ch3BiasCurrentMSB;
        public int Ch3BiasCurrent;

        // Ch4        
        public byte Ch4BiasCurrentLSB;
        public byte Ch4BiasCurrentMSB;
        public int Ch4BiasCurrent;

        // Ch5        
        public byte Ch5BiasCurrentLSB;
        public byte Ch5BiasCurrentMSB;
        public int Ch5BiasCurrent;

        // Ch6        
        public byte Ch6BiasCurrentLSB;
        public byte Ch6BiasCurrentMSB;
        public int Ch6BiasCurrent;

        public byte BiasCurrentSampleCountForAverage;


        // Frequency        
        public byte FrequencyLSB;
        public byte FrequencyMSB;
        public int Frequency;

        // Bias Voltage    
        public byte BiasVoltageLSB;
        public byte BiasVoltageMSB;
        public int BiasVoltage;

        // Peak Voltage       
        public byte PeakVoltageLSB;
        public byte PeakVoltageMSB;
        public int PeakVoltage;

        public byte MeasurementMode;
        public byte BiasVoltageSampleCountForAverage;


        public byte WPlusPairing;
        public byte WMinusPairing;
        public byte TAPlusPairing;
        public byte TAMinusPairing;
        public byte WHPlusPairing;
        public byte WHMinusPairing;
        public byte RHPlusPairing;
        public byte RHMinusPairing;
        public byte R1PlusPairing;
        public byte R1MinusPairing;
        public byte R2PlusPairing;
        public byte R2MinusPairing;


        public byte ResistanceCh1Writer;
        public byte ResistanceCh2TA;
        public byte ResistanceCh3WriteHeater;
        public byte ResistanceCh4ReadHeater;
        public byte ResistanceCh5Read1;
        public byte ResistanceCh6Read2;
        public byte CapacitanceCh1;
        public byte CapacitanceCh2;


        public byte HGA1;
        public byte HGA2;
        public byte HGA3;
        public byte HGA4;
        public byte HGA5;
        public byte HGA6;
        public byte HGA7;
        public byte HGA8;
        public byte HGA9;
        public byte HGA10;


        public byte TimeConstant;        

        // Ch1BiasCurrent
        public int getCh1BiasCurrent()
        {
            byte[] array = new byte[2];
            array[0] = Ch1BiasCurrentLSB;
            array[1] = Ch1BiasCurrentMSB;            
            return BitConverter.ToInt16(array, 0);
        }

        // Ch2BiasCurrent
        public int getCh2BiasCurrent()
        {
            byte[] array = new byte[2];
            array[0] = Ch2BiasCurrentLSB;
            array[1] = Ch2BiasCurrentMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // Ch3BiasCurrent
        public int getCh3BiasCurrent()
        {
            byte[] array = new byte[2];
            array[0] = Ch3BiasCurrentLSB;
            array[1] = Ch3BiasCurrentMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // Ch4BiasCurrent
        public int getCh4BiasCurrent()
        {
            byte[] array = new byte[2];
            array[0] = Ch4BiasCurrentLSB;
            array[1] = Ch4BiasCurrentMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // Ch5BiasCurrent
        public int getCh5BiasCurrent()
        {
            byte[] array = new byte[2];
            array[0] = Ch5BiasCurrentLSB;
            array[1] = Ch5BiasCurrentMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // Ch6BiasCurrent
        public int getCh6BiasCurrent()
        {
            byte[] array = new byte[2];
            array[0] = Ch6BiasCurrentLSB;
            array[1] = Ch6BiasCurrentMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // Frequency
        public int getFrequency()
        {
            byte[] array = new byte[2];
            array[0] = FrequencyLSB;
            array[1] = FrequencyMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // BiasVoltage
        public int getBiasVoltage()
        {
            byte[] array = new byte[2];
            array[0] = BiasVoltageLSB;
            array[1] = BiasVoltageMSB;
            return BitConverter.ToInt16(array, 0);
        }

        // PeakVoltage
        public int getPeakVoltage()
        {
            byte[] array = new byte[2];
            array[0] = PeakVoltageLSB;
            array[1] = PeakVoltageMSB;
            return BitConverter.ToInt16(array, 0);
        }
    }
}
