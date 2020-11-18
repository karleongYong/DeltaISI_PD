using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;
using XyratexOSC.Settings;

namespace XyratexOSC.IO.Devices
{
    /// <summary>
    /// Represents a simulated I/O device controller.
    /// </summary>
    public class SimIO : IIODevice
    {
        private Dictionary<int, int> _diBits = new Dictionary<int, int>();
        private Dictionary<int, int> _doBits = new Dictionary<int, int>();
        private Dictionary<int, double> _aiBits = new Dictionary<int, double>();
        private Dictionary<int, double> _aoBits = new Dictionary<int, double>();

        /// <summary>
        /// Gets the named-list of simulated digital input bits.
        /// </summary>
        /// <value>
        /// The list of simulated digital input bits.
        /// </value>
        public INamedList<DIBit> DIBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the named-list of simulated digital output bits.
        /// </summary>
        /// <value>
        /// The list of simulated digital output bits.
        /// </value>
        public INamedList<DOBit> DOBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the named-list of simulated analog input bits.
        /// </summary>
        /// <value>
        /// The list of simulated analog input bits.
        /// </value>
        public INamedList<AIBit> AIBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the named-list of simulated analog output bits.
        /// </summary>
        /// <value>
        /// The list of simulated analog output bits.
        /// </value>
        public INamedList<AOBit> AOBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the named-list of simulated digital input words.
        /// </summary>
        /// <value>
        /// The list of simulated digital input words.
        /// </value>
        public INamedList<DIWord> DIWords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the named-list of simulated digital output words.
        /// </summary>
        /// <value>
        /// The list of simulated digital output words.
        /// </value>
        public INamedList<DOWord> DOWords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the name of this simulated device.
        /// </summary>
        /// <value>
        /// The device name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the port details for connecting to this device.
        /// </summary>
        /// <value>
        /// The port details.
        /// </value>
        public IDevicePort Port 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a simulated device.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulated; otherwise, <c>false</c>.
        /// </value>
        public bool Simulated 
        { 
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether device logging is enabled. This is low-level logging used for debugging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if logging; otherwise, <c>false</c>.
        /// </value>
        public bool Logging 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets a value indicating whether this device is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this device is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimIO"/> class.
        /// </summary>
        public SimIO()
        {
            DIBits = new NamedList<DIBit>();
            DIBits.ListChanged += DIBits_ListChanged;
        }

        void DIBits_ListChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SimIO"/> class.
        /// </summary>
        ~SimIO()
        {
            Dispose();
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
        /// Opens a connection to this device using the device's specified <see cref="Port"/>.
        /// </summary>
        public void Open()
        {
            Connected = true;
        }

        /// <summary>
        /// Closes the connection to this device.
        /// </summary>
        public void Close()
        {
            Connected = false;
        }

        /// <summary>
        /// Configures the device based on the specified device settings. This is called before <see cref="IPart"/> creation.
        /// </summary>
        public void Configure(SettingsNode settings)
        {
        }

        /// <summary>
        /// Registers the specified <see cref="DIBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="DIBit"/>.</param>
        public void RegisterDIBit(DIBit bit)
        {
            _diBits.Add(bit.Channel, 0);
        }

        /// <summary>
        /// Registers the specified <see cref="DOBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="DOBit"/>.</param>
        public void RegisterDOBit(DOBit bit)
        {
            _doBits.Add(bit.Channel, 0);
        }

        /// <summary>
        /// Registers the specified <see cref="AIBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="AIBit"/>.</param>
        public void RegisterAIBit(AIBit bit)
        {
            _aiBits.Add(bit.Channel, 0);
        }

        /// <summary>
        /// Registers the specified <see cref="AOBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="AOBit"/>.</param>
        public void RegisterAOBit(AOBit bit)
        {
            _aoBits.Add(bit.Channel, 0);
        }

        /// <summary>
        /// Registers the specified <see cref="DIWord"/> with this device.
        /// </summary>
        /// <param name="word">The <see cref="DIWord"/>.</param>
        public void RegisterDIWord(DIWord word)
        {
        }

        /// <summary>
        /// Registers the specified <see cref="DOWord"/> with this device.
        /// </summary>
        /// <param name="word">The <see cref="DOWord"/>.</param>
        public void RegisterDOWord(DOWord word)
        {
        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        public void Initialize()
        {
            if (IOComponent.Instance.DIBits.Count > 0)
            {
                var diBits = IOComponent.Instance.DIBits.Where(b => b.Device == this).ToNamedList();
                foreach (DIBit bit in diBits)
                {
                    try
                    {
                        _diBits.Add(bit.Channel, 0);
                    }
                    catch (Exception)
                    {
                        new IOException(this, "DI bit '{0}' channel is already taken.");
                    }
                }
            }

            if (IOComponent.Instance.DOBits.Count > 0)
            {
                try
                {
                    var doBits = IOComponent.Instance.DOBits.Where(b => b.Device == this).ToNamedList();
                    foreach (DOBit bit in doBits)
                        _doBits.Add(bit.Channel, 0);
                }
                catch (Exception)
                {
                    new IOException(this, "DO bit '{0}' channel is already taken.");
                }
            }

            if (IOComponent.Instance.AIBits.Count > 0)
            {
                try
                {
                    var aiBits = IOComponent.Instance.AIBits.Where(b => b.Device == this).ToNamedList();
                    foreach (AIBit bit in aiBits)
                        _aiBits.Add(bit.Channel, 0);
                }
                catch (Exception)
                {
                    new IOException(this, "AI bit '{0}' channel is already taken.");
                }
            }

            if (IOComponent.Instance.AOBits.Count > 0)
            {
                try
                {
                    var aoBits = IOComponent.Instance.AOBits.Where(b => b.Device == this).ToNamedList();
                    foreach (AOBit bit in aoBits)
                        _aoBits.Add(bit.Channel, 0);
                }
                catch (Exception)
                {
                    new IOException(this, "AO bit '{0}' channel is already taken.");
                }

            }

            Initialized = true;
        }

        /// <summary>
        /// Un-initializes the device.
        /// </summary>
        public void Uninitialize()
        {
            Initialized = false;
        }

        /// <summary>
        /// Gets the current value of the digital output bit (see <see cref="DOBit" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The bit channel.</param>
        /// <returns>
        /// The bit value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public int GetDOBit(int channel)
        {
            if (!_doBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid DO channel ({0}) cannot be read.", channel));

            return _doBits[channel];
        }

        /// <summary>
        /// Sets the current value of the digital output bit (0 or 1) at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The DO value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public void SetDOBit(int channel, int value)
        {
            if (!_doBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid DO channel ({0}) cannot be set.", channel));

            _doBits[channel] = value;
        }

        /// <summary>
        /// Gets the current value of the digital input bit (0 or 1) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The bit value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public int GetDIBit(int channel)
        {
            if (!_diBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid DI channel ({0}) cannot be read.", channel));

            return _diBits[channel];
        }

        /// <summary>
        /// Sets the current value of the digital input bit (0 or 1) at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The DI value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public void SetDIBit(int channel, int value)
        {
            if (!_diBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid DI channel ({0}) cannot be set.", channel));

            _diBits[channel] = value;
        }

        /// <summary>
        /// Gets all of the values of the specified digital input bits.
        /// This should utilize any time-saving techniques with multiple channel reads depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>
        /// An array of digital input values.
        /// </returns>
        public int[] GetDIBits(int[] channels)
        {
            int[] values = new int[channels.Length];

            for (int i = 0; i < channels.Length; i++)
                values[i] = GetDIBit(channels[i]);

            return values;
        }

        /// <summary>
        /// Sets all of the values of the specified digital input bits.
        /// This should utilize any time-saving techniques with multiple channel writes depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="values">The bit values.</param>
        public void SetDIBits(int[] channels, int[] values)
        {
            for (int i = 0; i < channels.Length; i++)
                SetDIBit(channels[i], values[i]);
        }

        /// <summary>
        /// Gets all of the values of the specified digital output bits.
        /// This should utilize any time-saving techniques with multiple channel reads depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>
        /// An array of digital output values.
        /// </returns>
        public int[] GetDOBits(int[] channels)
        {
            int[] values = new int[channels.Length];

            for (int i = 0; i < channels.Length; i++)
                values[i] = GetDOBit(channels[i]);

            return values;
        }

        /// <summary>
        /// Sets all of the values of the specified digital output bits.
        /// This should utilize any time-saving techniques with multiple channel writes depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="values">The bit values.</param>
        public void SetDOBits(int[] channels, int[] values)
        {
            for (int i = 0; i < channels.Length; i++)
                SetDOBit(channels[i], values[i]);
        }

        /// <summary>
        /// Gets the number of bits that are represented in a single word.
        /// </summary>
        /// <returns>
        /// The number of bits.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GetDIOWordLength()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the digital input word (see <see cref="DIWord" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The word value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ulong GetDIWord(int channel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the current value of the digital input word (see <see cref="DIWord" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The word value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetDIWord(int channel, ulong value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the digital output word (see <see cref="DOWord" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The word value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ulong GetDOWord(int channel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the current value of the digital output word (see <see cref="DOWord" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The word value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetDOWord(int channel, ulong value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the analog input bit at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The analog value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public double GetAIBit(int channel)
        {
            if (!_aiBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid AI channel ({0}) cannot be read.", channel));

            return _aiBits[channel];
        }

        /// <summary>
        /// Sets the current value of the analog input bit at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The analog value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public void SetAIBit(int channel, double value)
        {
            if (!_aiBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid AI channel ({0}) cannot be set.", channel));

            _aiBits[channel] = value;
        }

        /// <summary>
        /// Gets the current value of the analog output bit at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The analog value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public double GetAOBit(int channel)
        {
            if (!_aiBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid AO channel ({0}) cannot be read.", channel));

            return _aiBits[channel];
        }

        /// <summary>
        /// Sets the current value of the analog output bit at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The analog value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">channel</exception>
        public void SetAOBit(int channel, double value)
        {
            if (!_aoBits.ContainsKey(channel))
                throw new ArgumentOutOfRangeException("channel", String.Format("Invalid AO channel ({0}) cannot be set.", channel));

            _aoBits[channel] = value;
        }
    }
}
