using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;
using XyratexOSC.IO.Devices;

namespace XyratexOSC.IO
{
    public abstract class AIOBit : IIOPart
    {
        protected IIODevice _device;
        protected double _lastValue;

        public string Name
        {
            get;
            set;
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public bool Simulated
        {
            get
            {
                return (_device is SimIO);
            }
        }

        public IDevice Device
        {
            get
            {
                return _device;
            }
        }

        public int Channel
        {
            get;
            set;
        }

        public DateTime LastRead
        {
            get;
            private set;
        }

        public event EventHandler ValueChanged
        {
            add { }
            remove { }
        }

        public void Dispose()
        {

        }

        public override string ToString()
        {
            return Name;
        }

        public double Get()
        {
            Update(DateTime.Now, GetValue());
            return _lastValue;
        }

        protected abstract double GetValue();

        public double Get(TimeSpan minFresh)
        {
            if (DateTime.Now - LastRead < minFresh)
                return _lastValue;
            else
                return Get();
        }

        public double GetLast()
        {
            return _lastValue;
        }

        public void Set(double value)
        {
            SetValue(value);
        }

        protected abstract void SetValue(double value);

        internal void Update(DateTime readTime, double value)
        {
            LastRead = readTime;
            _lastValue = value;
        }
    }

    public class AIBit : AIOBit
    {
        public AIBit(IIODevice device)
        {
            _device = device;
        }

        protected override double GetValue()
        {
            return _device.GetAIBit(Channel);
        }

        protected override void SetValue(double value)
        {
            _device.SetAIBit(Channel, value);
        }
    }

    public class AOBit : AIOBit
    {
        public AOBit(IIODevice device)
        {
            _device = device;
        }

        protected override double GetValue()
        {
            return _device.GetAOBit(Channel);
        }

        protected override void SetValue(double value)
        {
            _device.SetAOBit(Channel, value);
        }
    }
}
