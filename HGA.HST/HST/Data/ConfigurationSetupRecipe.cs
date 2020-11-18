using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data
{
    public unsafe struct ConfigurationSetupRecipe
    {
        // Ch1BiasCurrent        
        public int Ch1BiasCurrent;

        // Ch2        
        public int Ch2BiasCurrent;

        // Ch3        
        public int Ch3BiasCurrent;

        // Ch4        
        public int Ch4BiasCurrent;

        // Ch5        
        public int Ch5BiasCurrent;

        // Ch6        
        public int Ch6BiasCurrent;

        //public int Ch6IbBiasCurrent;
        public int BiasCurrentCh6LDU1stPoint;
        public int BiasCurrentCh6LDU2ndPoint;
        public int BiasCurrentCh6LDUStep;

        //ykl 15-07-2020
        public double BiasCurrent3rdPointforIThreshold;
        public double BiasCurrent4thPointforIThreshold;
        public double BiasCurrentLEDCh6LDU1stPoint;
        public double BiasCurrentLEDCh6LDU2ndPoint;
        public double BiasCurrentLEDCh6LDUStep;

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

 		public bool LDUEnable;

        public bool SwapEnable;

        public bool SweepEnable;

        public int Trend1StopPoint;
        public int Trend2StartPoint;
       

        /// <summary>
        /// How many part to perform per carrier
        /// </summary>
        public int CounterSamplingPerCarrier;

        /// <summary>
        /// How many carrier count to active sampling
        /// </summary>
        public int CarrierCounterToSampling;

        // Ch1BiasCurrent
        public int getCh1BiasCurrent()
        {
            return Ch1BiasCurrent;
        }

        // Ch2BiasCurrent
        public int getCh2BiasCurrent()
        {
            return Ch2BiasCurrent;
        }

        // Ch3BiasCurrent
        public int getCh3BiasCurrent()
        {
            return Ch3BiasCurrent;
        }

        // Ch4BiasCurrent
        public int getCh4BiasCurrent()
        {
            return Ch4BiasCurrent;
        }

        // Ch5BiasCurrent
        public int getCh5BiasCurrent()
        {
            return Ch5BiasCurrent;
        }

        // Ch6BiasCurrent
        public int getCh6BiasCurrent()
        {
            return Ch6BiasCurrent;
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
