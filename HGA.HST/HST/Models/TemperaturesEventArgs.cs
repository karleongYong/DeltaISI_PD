using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Models
{
    public class TemperaturesEventArgs : EventArgs
    {
        public string MeasurementBoardTemperature
        {
            get;
            set;
        }

        public string WorkingZoneTemperature
        {
            get;
            set;
        }

        public string ExhaustTemperature
        {
            get;
            set;
        }

        public TemperaturesEventArgs(string measurementBoardTemperature, string workingZoneTemperature, string exhaustTemperature)
        {
            MeasurementBoardTemperature = measurementBoardTemperature;
            WorkingZoneTemperature = workingZoneTemperature;
            ExhaustTemperature = exhaustTemperature;
        }
    }
}
