using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    internal class AnalogInput : IOPoint, IAnalogInput
    {
        // Nested declarations -------------------------------------------------

        public enum WaitCriteria
        {
            Above,
            Below,
            InRange
        };

        // Member variables ----------------------------------------------------

        private string unitName         = "counts";
        private double scale            = 1.0;
        private double offset           = 0.0;
        private double rawInputValue    = 0.0;
        private double threshold        = 0;
        private double tolerance        = 0;
        private WaitCriteria criteria;

        // Constructors & Finalizers -------------------------------------------

        public AnalogInput(A3200HC a3200HC, string name, uint nodeId, uint pointId)
            : base(a3200HC, name, nodeId, pointId)
        {
            base.type = Type.AnalogInput;
        }

        // Methods -------------------------------------------------------------

        #region IAnalogInput ---------------------------------------------
        public double Get()
        {
            rawInputValue = a3200HC.GetRawValue(this);

            return (rawInputValue * scale + offset);
        }

        public double Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        public void SetUnit(string unitName, double scale, double offset)
        {
            this.unitName = unitName;
            this.scale = scale;
            this.offset = offset;
        }

        public string Unit
        {
            get { return unitName; }
        }

        public double Value
        {
            get { return (rawInputValue * scale + offset); }
        }

        public double ValueRaw
        {
            get { return rawInputValue; }
        }

        public void WaitForAboveThreshold(double threshold, uint timeOut)
        {
            if (a3200HC.Simulation)
                return;

            a3200HC.WaitForAboveThreshold(this, threshold, (int)timeOut);
            this.WaitIOComplete();
            if (this.errorCode != 0)
                throw new HardwareException(string.Format("Timed out waiting for analog input [{0}] to be above threshold [value={1:0.000}, threshold={2:0.000}]", this.Name, this.Get(), threshold));
        }

        public void WaitForBelowThreshold(double threshold, uint timeOut)
        {
            if (a3200HC.Simulation)
                return;

            a3200HC.WaitForBelowThreshold(this, threshold, (int)timeOut);
            this.WaitIOComplete();
            if (this.errorCode != 0)
                throw new HardwareException(string.Format("Timed out waiting for analog input [{0}] to be below threshold [value={1:0.000}, threshold={2:0.000}]", this.Name, this.Get(), threshold));
        }

        public void WaitForInRange(double dLowThrs, double dHiThrs, uint timeOut)
        {
            if (a3200HC.Simulation)
                return;

            a3200HC.WaitForInRange(this, (dLowThrs + dHiThrs) / 2, Math.Abs(dHiThrs - dLowThrs) / 2, (int)timeOut);
            this.WaitIOComplete();
            if (this.errorCode != 0)
                throw new HardwareException(string.Format("Timed out waiting for analog input [{0}] to be in range [value={1:0.000}, range={2:0.000}-{3:0.000}]", this.Name, this.Get(), dLowThrs, dHiThrs));
        }

        public void WaitForOutRange(double dLowThrs, double dHiThrs, uint timeOut)
        {
            
        }

        public void WaitForTarget(double dTarget, double dSymTolerance, uint timeOut)
        {
            
        }
        #endregion

        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }


        public WaitCriteria Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
    }
}
