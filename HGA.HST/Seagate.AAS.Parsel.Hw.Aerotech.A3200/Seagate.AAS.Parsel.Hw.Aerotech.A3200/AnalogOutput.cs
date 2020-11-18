using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    internal class AnalogOutput : IOPoint, IAnalogOutput
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        public const int NATIVE_DAC_RESOLUTION = 32767;  // copley analog output have 15 bit resolution (claims 16 bit in documentation)

        private double limitMin = 0;
        private double limitMax = NATIVE_DAC_RESOLUTION;
        private string unitName = "counts";
        private double scale    = 1.0;
        private double offset   = 0.0;
        private double rawOutputValueRequested;

        // Constructors & Finalizers -------------------------------------------

        public AnalogOutput(A3200HC a3200HC, string name, uint nodeId, uint pointId)
            : base(a3200HC, name, nodeId, pointId)
        {
            base.type = Type.AnalogOutput;
        }

        // Properties ----------------------------------------------------------

        // Methods -------------------------------------------------------------

        #region IAnalogOutput ---------------------------------------------
        public double Get()
        {
            return (rawOutputValueRequested * scale + offset);
        }

        public double LimitMax
        { 
            get { return limitMax; } 
            set { limitMax = value; } 
        }

        public double LimitMin
        { 
            get { return limitMin; } 
            set { limitMin = value; } 
        }

        public double Offset
        { 
            get { return offset; } 
            set { offset = value; } 
        }

        public double Scale
        {
            get { return scale; } 
            set { offset = scale; } 
        }

        public void Set(double scaledValue)
        {
            SetRequest(scaledValue);
            a3200HC.SetRawValue(this, rawOutputValueRequested);
        }

        public void SetRequest(double scaledValue)
        {
            rawOutputValueRequested = (scaledValue - offset) / scale;
        }

        public void SetUnit(string unitName, double scale, double offset)
        {
            this.unitName = unitName;
            this.scale = scale;
            this.offset = offset;

            // update min max
            this.limitMin = 0 * scale + offset;
            this.limitMax = NATIVE_DAC_RESOLUTION * scale + offset;
        }

        public string Unit
        {
            get
            { 
                return unitName; 
            } 
            set 
            { 
                unitName = value; 
            }
        }

        public double Value
        {
            get { return (rawOutputValueRequested * scale + offset); }
        }

        public double ValueRequested
        {
            get { return rawOutputValueRequested * scale + offset; }
        }

        public double ValueRequestedRaw
        {
            get { return rawOutputValueRequested; }
        }
        #endregion
    }
}
