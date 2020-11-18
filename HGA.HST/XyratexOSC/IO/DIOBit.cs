using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;
using XyratexOSC.IO.Devices;

namespace XyratexOSC.IO
{
    /// <summary>
    /// Represents a single digital I/O bit.
    /// </summary>
    public abstract class DIOBit : IIOPart
    {
        /// <summary>
        /// The I/O controller device.
        /// </summary>
        protected IIODevice _device;

        /// <summary>
        /// The last read value
        /// </summary>
        protected int _lastValue;

        /// <summary>
        /// Gets or sets the name of the I/O bit.
        /// </summary>
        /// <value>
        /// The bit name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this bit is read-only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read-only; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IIOPart"/> is simulated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulated; otherwise, <c>false</c>.
        /// </value>
        public bool Simulated
        {
            get
            {
                return (_device is SimIO);
            }
        }

        /// <summary>
        /// Gets the parent I/O controller device.
        /// </summary>
        /// <value>
        /// The parent device.
        /// </value>
        public IDevice Device
        {
            get
            {
                return _device;
            }
        }

        /// <summary>
        /// Gets or sets the channel used to address the bit.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public int Channel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the timestamp of the last read.
        /// </summary>
        /// <value>
        /// The last read time.
        /// </value>
        public DateTime LastRead
        {
            get;
            private set;
        }

        /// <summary>
        /// Trigged when the I/O value changed, if supported by the controller. Most I/O controllers need to be polled.
        /// </summary>
        public event EventHandler ValueChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets the current value of the digital I/O bit represented as a Boolean.
        /// </summary>
        /// <returns><c>true</c> if the I/O bit is high; otherwise, <c>false</c>.</returns>
        public bool Get()
        {
            return (GetValue() == 1);
        }

        /// <summary>
        /// Gets the current value of the digital I/O bit.
        /// </summary>
        /// <returns>The current I/O value.</returns>
        public int GetValue()
        {
            Update(DateTime.Now, DoGetValue());
            return _lastValue;
        }

        /// <summary>
        /// Performs a single channel read from the I/O controller device.
        /// </summary>
        /// <returns>The current I/O value.</returns>
        protected abstract int DoGetValue();

        /// <summary>
        /// Gets the bit value that is cached from the previous device read if under the specified minimum "freshness". If the cache value is stale, then the <see cref="Device"/> is queried.
        /// </summary>
        /// <param name="minFresh">The minimum time span since the last read to skip querying the device and return the previously read value.</param>
        /// <returns><c>true</c> if the I/O bit is high; otherwise, <c>false</c>.</returns>
        public bool Get(TimeSpan minFresh)
        {
            if (DateTime.Now - LastRead < minFresh)
                return (_lastValue == 1);
            else
                return Get();
        }

        /// <summary>
        /// Gets the bit value that is cached from the previous device read if under the specified minimum "freshness". If the cache value is stale, then the <see cref="Device"/> is queried.
        /// </summary>
        /// <param name="minFresh">The minimum time span since the last read to skip querying the device and return the previously read value.</param>
        /// <returns>The I/O value that was read within the specified timeframe.</returns>
        public int GetValue(TimeSpan minFresh)
        {
            if (DateTime.Now - LastRead < minFresh)
                return _lastValue;
            else
                return GetValue();
        }

        /// <summary>
        /// Gets the last read value.
        /// </summary>
        /// <returns>The value of the previous read.</returns>
        public int GetLast()
        {
            return _lastValue;
        }

        /// <summary>
        /// Sets the specified bit value.
        /// </summary>
        /// <param name="value">If set to <c>true</c> sets the I/O channel high; otherwise, sets the channel low.</param>
        public void Set(bool value)
        {
            SetValue(value ? 1 : 0);
        }

        /// <summary>
        /// Sets the specified bit value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Set(int value)
        {
            SetValue(value);
        }

        /// <summary>
        /// Attempts to set the specified bit value.
        /// </summary>
        /// <param name="value">The bit value.</param>
        protected abstract void SetValue(int value);

        internal void Update(DateTime readTime, int value)
        {
            LastRead = readTime;
            _lastValue = value;
        }
    }

    /// <summary>
    /// Represents a single-bit digital input channel.
    /// </summary>
    public class DIBit : DIOBit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DIBit"/> class.
        /// </summary>
        /// <param name="device">The controller device.</param>
        public DIBit(IIODevice device)
        {
            _device = device;
        }

        /// <summary>
        /// Performs a single channel read from the I/O controller device.
        /// </summary>
        /// <returns>
        /// The current input value.
        /// </returns>
        protected override int DoGetValue()
        {
            return _device.GetDIBit(Channel);
        }

        /// <summary>
        /// Attempts to set the specified bit value.
        /// </summary>
        /// <param name="value">The bit value.</param>
        protected override void SetValue(int value)
        {
            _device.SetDIBit(Channel, value);
        }
    }

    /// <summary>
    /// Represents a single-bit digital output channel. 
    /// </summary>
    public class DOBit : DIOBit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DOBit"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DOBit(IIODevice device)
        {
            _device = device;
        }

        /// <summary>
        /// Performs a single channel read from the I/O controller device.
        /// </summary>
        /// <returns>
        /// The current output value.
        /// </returns>
        protected override int DoGetValue()
        {
            return _device.GetDOBit(Channel);
        }

        /// <summary>
        /// Attempts to set the specified bit value.
        /// </summary>
        /// <param name="value">The bit value.</param>
        protected override void SetValue(int value)
        {
            _device.SetDOBit(Channel, value);
        }
    }
}
