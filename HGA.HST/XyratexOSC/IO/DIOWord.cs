using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;
using XyratexOSC.IO.Devices;

namespace XyratexOSC.IO
{
    /// <summary>
    /// Represents a multi-bit digital I/O channel.
    /// </summary>
    public abstract class DIOWord : IIOPart
    {
        /// <summary>
        /// The parent IO controller device.
        /// </summary>
        protected IIODevice _device;

        /// <summary>
        /// The last read word value.
        /// </summary>
        protected ulong _lastValue;

        /// <summary>
        /// A collection of single <see cref="DIOBit"/>s that are represented as a single bit in this multi-bit word.
        /// </summary>
        protected Dictionary<int, DIOBit> _childBits = new Dictionary<int, DIOBit>();

        private int _startBitChannel = -1;
        private int _wordLength = -1;

        /// <summary>
        /// Gets or sets the name of this digital I/O word channel.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this word is read-only.
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
        /// Gets a value indicating whether this <see cref="IIOPart" /> is simulated.
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
        /// Gets the parent IO controller device.
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
        /// Gets or sets the channel used to reference this word from the parent <see cref="Device"/>.
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
        /// Gets or sets the starting bit channel.
        /// </summary>
        /// <value>
        /// The start bit channel.
        /// </value>
        public int StartBitChannel
        {
            get
            {
                return _startBitChannel;
            }
            set
            {
                _startBitChannel = value;

                UpdateChildBits();
            }
        }

        /// <summary>
        /// Gets or sets the number of I/O bits in this word.
        /// </summary>
        /// <value>
        /// The length of the word.
        /// </value>
        /// <exception cref="System.Exception">Digital IO Words must be less than or equal to 64 bits.</exception>
        public int WordLength
        {
            get
            {
                if (_wordLength < 0)
                    return _device.GetDIOWordLength();
                else
                    return _wordLength;
            }
            set
            {
                if (value > 64)
                    throw new Exception("Digital IO Words must be less than or equal to 64 bits.");

                _wordLength = value;

                UpdateChildBits();
            }
        }

        /// <summary>
        /// Gets the timestamp of when the last read occurred.
        /// </summary>
        /// <value>
        /// The last read.
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
        /// Gets the current word value from the I/O device.
        /// </summary>
        /// <returns></returns>
        public ulong Get()
        {
            Update(DateTime.Now, GetValue());
            return _lastValue;
        }

        /// <summary>
        /// Gets the current word value from the I/O device.
        /// </summary>
        /// <returns></returns>
        protected abstract ulong GetValue();

        /// <summary>
        /// Gets the word value that is cached from the previous device read if under the specified minimum "freshness". If the cache value is stale, then the <see cref="Device"/> is queried.
        /// </summary>
        /// <param name="minFresh">The minimum time span since the last read to skip querying the device and return the previously read value.</param>
        /// <returns></returns>
        public ulong Get(TimeSpan minFresh)
        {
            if (DateTime.Now - LastRead < minFresh)
                return _lastValue;
            else
                return Get();
        }

        /// <summary>
        /// Gets the last read value.
        /// </summary>
        /// <returns>The value of the previous read.</returns>
        public ulong GetLast()
        {
            return _lastValue;
        }

        /// <summary>
        /// Sets the specified word value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Set(ulong value)
        {
            SetValue(value);
        }

        /// <summary>
        /// Attempts to set the specified word value.
        /// </summary>
        /// <param name="value">The word value.</param>
        protected abstract void SetValue(ulong value);

        internal void Update(DateTime readTime, ulong value)
        {
            LastRead = readTime;
            _lastValue = value;

            ulong bitMask = 1;
            for (int bit = 0; bit < WordLength; bit++)
            {
                if (_childBits.ContainsKey(bit + StartBitChannel))
                {
                    int bitValue = (value & bitMask) != 0 ? 1 : 0;
                    _childBits[bit + StartBitChannel].Update(readTime, bitValue);
                }

                bitMask = bitMask << 1;
            }
        }

        /// <summary>
        /// Updates the child bits that make up this word.
        /// </summary>
        protected abstract void UpdateChildBits();
    }

    /// <summary>
    /// Represents a multi-bit digital input channel.
    /// </summary>
    public class DIWord : DIOWord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DIWord"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DIWord(IIODevice device)
        {
            _device = device;
        }

        /// <summary>
        /// Gets the current word value from the input device.
        /// </summary>
        /// <returns></returns>
        protected override ulong GetValue()
        {
            return _device.GetDIWord(Channel);
        }

        /// <summary>
        /// Attempts to set the specified word value.
        /// </summary>
        /// <param name="value">The word value.</param>
        protected override void SetValue(ulong value)
        {
            _device.SetDIWord(Channel, value);
        }

        /// <summary>
        /// Updates the child bits that make up this word.
        /// </summary>
        protected override void UpdateChildBits()
        {
            _childBits.Clear();

            for (int i = 0; i < WordLength; i++)
            {
                int channel = i + StartBitChannel;

                foreach (DIOBit bit in IOComponent.Instance.DIBits)
                {
                    if (bit.Channel == channel)
                        _childBits.Add(channel, bit);
                }
            }
        }
    }

    /// <summary>
    /// Represents a multi-bit digital output channel.
    /// </summary>
    public class DOWord : DIOWord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DOWord"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DOWord(IIODevice device)
        {
            _device = device;
        }

        /// <summary>
        /// Gets the current word value from the output device.
        /// </summary>
        /// <returns>The current word value.</returns>
        protected override ulong GetValue()
        {
            return _device.GetDOWord(Channel);
        }

        /// <summary>
        /// Attempts to set the specified word value.
        /// </summary>
        /// <param name="value">The word value.</param>
        protected override void SetValue(ulong value)
        {
            _device.SetDOWord(Channel, value);
        }

        /// <summary>
        /// Updates the child bits that make up this input word.
        /// </summary>
        protected override void UpdateChildBits()
        {
            _childBits.Clear();

            for (int i = 0; i < WordLength; i++)
            {
                int channel = i + StartBitChannel;

                foreach (DIOBit bit in IOComponent.Instance.DOBits)
                {
                    if (bit.Channel == channel)
                        _childBits.Add(channel, bit);
                }
            }
        }
    }
}
