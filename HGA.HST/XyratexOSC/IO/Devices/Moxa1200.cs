using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

using XyratexOSC.Logging;
using XyratexOSC.Hardware;
using XyratexOSC.Settings;

using MOXA;

namespace XyratexOSC.IO.Devices
{
    /// <summary>
    /// Represents a MOXA ioLogic 1200 I/O controller device.
    /// </summary>
    [DeviceNames("MoxaIoLogic1200","IoLogic1200")]
    public class Moxa1200 : IIODevice
    {
        private const UInt16 _modbusPort = 502;						//Modbus TCP port
        private const int _timeout = 10000;

        private object _lock = new object();
        private Int32[] _hConn = new Int32[1];
        private int _wordLength = 8;
        private int _numConfigChannels = 4;
        private int _retries = 1;

        private byte[] _ipAddress;
        private byte[] _password;
        private ushort _port;

        private bool[] _configOuts = new bool[4];

        /// <summary>
        /// Gets or sets the Moxa1200 device name.
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
                return false;
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
        /// Initializes a new instance of the <see cref="Moxa1200"/> class.
        /// </summary>
        /// <exception cref="System.Exception">Moxa ioLogic 1200 driver failed to initialize.</exception>
        public Moxa1200()
        {
            ExtractEmbeddedAssembly();

            MoxaStatus status = (MoxaStatus)MXIO_CS.MXEIO_Init();
            if (status != MoxaStatus.Ok)
                throw new Exception("Moxa ioLogic 1200 driver failed to initialize.");
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Moxa1200"/> class.
        /// </summary>
        ~Moxa1200()
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MXIO_CS.MXEIO_Exit();
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

        private void ExtractEmbeddedAssembly()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(assemblyDir, "MXIO_NET.dll");

            if (File.Exists(filePath))
                return;

            byte[] resource = Environment.Is64BitProcess ? Properties.Resources.MXIO_NET_64 : Properties.Resources.MXIO_NET_32;

            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                fs.Write(resource, 0, resource.Length);
            }
        }

        /// <summary>
        /// Opens a connection to this device using the device's specified <see cref="Port"/>.
        /// </summary>
        public void Open()
        {
            DevicePortTcp tcpPort = Port as DevicePortTcp;

            if (tcpPort == null)
            {
                DevicePortName namedPort = Port as DevicePortName;

                if (namedPort != null)
                {
                    tcpPort = DevicePortTcp.FromString(namedPort.Name);
                    tcpPort.Port = _modbusPort;
                }
            }

            if (tcpPort != null)
            {
                tcpPort.Port = _modbusPort;
                Open(tcpPort);
                return;
            }

            throw new IOException(this, "Invalid TCP port specified for Moxa ioLogic 1200 controller.");
        }

        private void Open(DevicePortTcp tcp)
        {
            Ping ping = new Ping();
            PingReply reply = null;

            reply = ping.Send(tcp.Address, 1000);

            if (reply.Status != IPStatus.Success)
                throw new IOException(this, String.Format("Failed to ping Moxa ioLogic1200 device '{0}' at {1}", Name, tcp));

            _ipAddress = System.Text.Encoding.UTF8.GetBytes(tcp.Address.ToString());
            _password = System.Text.Encoding.UTF8.GetBytes("");
            _port = (ushort)tcp.Port;

            MoxaStatus status = (MoxaStatus)MXIO_CS.MXEIO_E1K_Connect(_ipAddress, _port, _timeout, _hConn, _password);
            if (status != MoxaStatus.Ok)
                throw new IOException(this, String.Format("Failed to connect to Moxa ioLogic1200 device '{0}' at {1}", Name, tcp));

            // Check Connection
            CheckConnection(false);
        }

        private void CheckConnection(bool attemptReconnect)
        {
            // Check Connection
            Connected = false;

            byte[] checkStatus = new byte[1];
            MoxaStatus status = (MoxaStatus)MXIO_CS.MXEIO_CheckConnection(_hConn[0], _timeout, checkStatus);
            if (status != MoxaStatus.Ok)
                throw new IOException(this, String.Format("Failed to check connection to Moxa ioLogic1200 device '{0}'.", Name));

            if (checkStatus[0] == MXIO_CS.CHECK_CONNECTION_OK)
            {
                Connected = true;
                return;
            }

            if (attemptReconnect)
            {
                status = (MoxaStatus)MXIO_CS.MXEIO_E1K_Connect(_ipAddress, _port, _timeout, _hConn, _password);
                if (status == MoxaStatus.Ok)
                {
                    if (Logging)
                        Log.Trace(IOComponent.Instance.Name).Warn(this.ToString(), "Connection to Moxa ioLogic1200 device restored.");

                    return;
                }
            }

            switch (checkStatus[0])
            {
                case MXIO_CS.CHECK_CONNECTION_FAIL:
                    throw new IOException(this, String.Format("Connection failed to Moxa ioLogic1200 device '{0}'.", Name));
                case MXIO_CS.CHECK_CONNECTION_TIME_OUT:
                    throw new IOException(this, String.Format("Connection timed-out to Moxa ioLogic1200 device '{0}'.", Name));
                default:
                    throw new IOException(this, String.Format("Unknown connection failure ({1}) to Moxa ioLogic1200 device '{0}'.", Name, checkStatus[0]));
            }
        }

        /// <summary>
        /// Closes the connection to this device.
        /// </summary>
        public void Close()
        {
            if (!Connected)
                return;

            MoxaStatus status = (MoxaStatus)MXIO_CS.MXEIO_Disconnect(_hConn[0]);
            if (status != MoxaStatus.Ok)
                throw new IOException(this, String.Format("Failed to disconnect from Moxa ioLogic1200 device '{0}'.", Name));
            
            Connected = false;
        }

        /// <summary>
        /// Configures the device based on the specified device settings. This is called before <see cref="IPart"/> creation.
        /// </summary>
        public void Configure(SettingsNode settings)
        {
            string configChannelList = "Out,Out,Out,Out";

            if (settings.ExistsAndHasAValue<string>("ConfigChannels"))
                configChannelList = settings["ConfigChannels"].GetValueAs<string>();

            string[] configChannels = configChannelList.Split(',');
            int configCount = Math.Min(configChannels.Length, _numConfigChannels);

            for (int i = 0; i < _numConfigChannels; i++)
                _configOuts[i] = true;

            for (int i = 0; i < configCount; i++)
                _configOuts[i] = (configChannels[i].TrimStart().StartsWith("I", StringComparison.CurrentCultureIgnoreCase));

            // Can we do this through software, or only jumpers?
        }

        /// <summary>
        /// Registers the specified <see cref="DIBit" /> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="DIBit"/></param>
        public void RegisterDIBit(DIBit bit)
        {

        }

        /// <summary>
        /// Registers the specified <see cref="DOBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="DOBit"/>.</param>
        public void RegisterDOBit(DOBit bit)
        {

        }

        /// <summary>
        /// Registers the specified <see cref="AIBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="AIBit"/>.</param>
        public void RegisterAIBit(AIBit bit)
        {

        }

        /// <summary>
        /// Registers the specified <see cref="AOBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="AOBit"/>.</param>
        public void RegisterAOBit(AOBit bit)
        {

        }

        /// <summary>
        /// Registers the specified <see cref="DIWord" /> with this device.
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
        /// Gets the number of bits that are represented in a single word.
        /// </summary>
        /// <returns>
        /// The number of bits.
        /// </returns>
        public int GetDIOWordLength()
        {
            return _wordLength;
        }

        /// <summary>
        /// Gets the current value of the digital input bit (0 or 1) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The bit value.
        /// </returns>
        public int GetDIBit(int channel)
        {
            return GetDIBit(channel, 0);
        }

        /// <summary>
        /// Gets the di bit.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="attempt">The max number of read attempts.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException"></exception>
        public int GetDIBit(int channel, int attempt)
        {
            uint[] value = new uint[1];

            lock (_lock)
            {
                MoxaStatus status = (MoxaStatus)MXIO_CS.E1K_DI_Reads(_hConn[0], (byte)channel, 1, value);

                if (status != MoxaStatus.Ok)
                {
                    CheckConnection(true);

                    if (attempt >= _retries)
                        throw new IOException(this, String.Format("Could not read MOXA DI channel {0} after {1} retries. ({2}).", channel, attempt, status));
                    
                    return GetDIBit(channel, attempt + 1);
                }
            }

            return (int)value[0];
        }

        /// <summary>
        /// Sets the current value of the digital input bit (0 or 1) at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The DI value.</param>
        /// <exception cref="System.IO.IOException">Cannot write values to DI channel.</exception>
        public void SetDIBit(int channel, int value)
        {
            throw new IOException(this, "Cannot write values to DI channel.");
        }

        /// <summary>
        /// Gets the current value of the digital output bit (see <see cref="DOBit" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The bit channel.</param>
        /// <returns>
        /// The bit value.
        /// </returns>
        public int GetDOBit(int channel)
        {
            return GetDOBit(channel, 0);
        }

        /// <summary>
        /// Gets the do bit.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="attempt">The attempt.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException"></exception>
        private int GetDOBit(int channel, int attempt)
        {
            uint[] value = new uint[1];

            lock (_lock)
            {
                MoxaStatus status = (MoxaStatus)MXIO_CS.E1K_DO_Reads(_hConn[0], (byte)channel, (byte)1, value);

                if (status != MoxaStatus.Ok)
                {
                    CheckConnection(true);

                    if (attempt >= _retries)
                        throw new IOException(this, String.Format("Could not read MOXA DO channel {0} after {1} retries. ({2}).", channel, attempt, status));
                                        
                    return GetDOBit(channel, attempt + 1);
                }
            }

            return (int)value[0];
        }

        /// <summary>
        /// Sets the current value of the digital output bit (0 or 1) at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The DO value.</param>
        public void SetDOBit(int channel, int value)
        {
            SetDOBit(channel, value, 0);
        }

        /// <summary>
        /// Attempts to set the current value of the digital output bit (0 or 1) at the specified channel address to the specified value.
        /// Retries until successful or the max number of attempts is reached.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The set value (0 or 1).</param>
        /// <param name="attempt">The max number of retry attempts.</param>
        /// <exception cref="System.IO.IOException"></exception>
        public void SetDOBit(int channel, int value, int attempt)
        {
            /*
            byte wordLength = (byte)GetDIOWordLength();


            int startChannel = channel / wordLength;

            MoxaStatus status = (MoxaStatus)MXIO_CS.E1K_DO_Reads(_hConn[0], (byte)startChannel, wordLength, readValues);
            
            if (status != MoxaStatus.Ok)
                throw new IOException(this, String.Format("Could not read state before setting MOXA DO channel {0} ({1}).", channel, status));

            uint bitMask = (uint)1 << (channel % wordLength);

            if (value > 0)
                readValues[0] |= bitMask;
            else if (value < 0)
                readValues[0] &= ~bitMask;
            */

            lock (_lock)
            {
                MoxaStatus status = (MoxaStatus)MXIO_CS.E1K_DO_Writes(_hConn[0], (byte)channel, (byte)1, (uint)value);

                if (status != MoxaStatus.Ok)
                {
                    CheckConnection(true);

                    if (attempt >= _retries)
                        throw new IOException(this, String.Format("Could not write to MOXA DO channel {0} after {1} retries ({2}).", channel, attempt, status));

                    SetDOBit(channel, value, attempt + 1);
                }
            }
        }

        public int[] GetDIBits(int[] channels)
        {
            List<int> allValues = new List<int>(channels.Length);

            List<List<int>> _contGroups = new List<List<int>>();
            _contGroups.Add(new List<int>());

            for (int i = 0; i < channels.Length; i++)
            {
                List<int> group = _contGroups.Last();

                if (group.Count == 0)
                {
                    group.Add(channels[i]);
                    continue;
                }

                if (group.Last() != channels[i] - 1)
                {
                    _contGroups.Add(new List<int>() { channels[i] });
                }
                else
                {
                    group.Add(channels[i]);
                }
            }

            foreach (List<int> group in _contGroups)
            {
                uint[] values = new uint[1];

                lock (_lock)
                {
                    MoxaStatus status = MoxaStatus.SIOResponseBad;
                    int attempt = 0;

                    while (attempt < _retries)
                    {
                        status = (MoxaStatus)MXIO_CS.E1K_DI_Reads(_hConn[0], (byte)group.First(), (byte)group.Count, values);

                        if (status == MoxaStatus.Ok)
                            break;

                        CheckConnection(true);

                        attempt++;
                    }

                    if (status != MoxaStatus.Ok)
                        throw new IOException(this, String.Format("Could not read MOXA DI channels {0} after {1} retries ({2}).", String.Join(",", channels), _retries, status));
                }

                for (int i = 0; i < group.Count; i++)
                {
                    uint bitMask = (uint)1 << i;
                    int bitValue = (values[0] & bitMask) != 0 ? 1 : 0;

                    allValues.Add(bitValue);
                }
            }

            return allValues.ToArray();
        }

        public int[] GetDOBits(int[] channels)
        {
            List<int> allValues = new List<int>(channels.Length);

            List<List<int>> _contGroups = new List<List<int>>();
            _contGroups.Add(new List<int>());

            for (int i = 0; i < channels.Length; i++)
            {
                List<int> group = _contGroups.Last();

                if (group.Count == 0)
                {
                    group.Add(channels[i]);
                    continue;
                }

                if (group.Last() != channels[i] - 1)
                {
                    _contGroups.Add(new List<int>() { channels[i] });
                }
                else
                {
                    group.Add(channels[i]);
                }
            }

            foreach (List<int> group in _contGroups)
            {
                uint[] values = new uint[1];

                lock (_lock)
                {
                    MoxaStatus status = MoxaStatus.SIOResponseBad;
                    int attempt = 0;

                    while (attempt < _retries)
                    {
                        status = (MoxaStatus)MXIO_CS.E1K_DO_Reads(_hConn[0], (byte)group.First(), (byte)group.Count, values);

                        if (status == MoxaStatus.Ok)
                            break;

                        CheckConnection(true);

                        attempt++;
                    }

                    if (status != MoxaStatus.Ok)
                        throw new IOException(this, String.Format("Could not read MOXA DO channels {0} after {1} retries ({2}).", String.Join(",", channels), _retries, status));
                }

                for (int i = 0; i < group.Count; i++)
                {
                    uint bitMask = (uint)1 << i;
                    int bitValue = (values[0] & bitMask) != 0 ? 1 : 0;

                    allValues.Add(bitValue);
                }
            }

            return allValues.ToArray();
        }

        /// <summary>
        /// Sets all of the values of the specified digital output bits.
        /// This should utilize any time-saving techniques with multiple channel writes depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="values">The bit values.</param>
        /// <exception cref="System.ArgumentException">Mismatch between MOXA DO channels and values.</exception>
        public void SetDOBits(int[] channels, int[] values)
        {
            if (channels.Length != values.Length)
                throw new ArgumentException("Mismatch between MOXA DO channels and values.");

            for (int i = 0; i < channels.Length; i++)
                SetDOBit(channels[i], values[i]);
        }

        /// <summary>
        /// Sets all of the values of the specified digital input bits.
        /// This should utilize any time-saving techniques with multiple channel writes depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="values">The bit values.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetDIBits(int[] channels, int[] values)
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
        /// <exception cref="System.NotImplementedException"></exception>
        public double GetAIBit(int channel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the analog output bit at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>
        /// The analog value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public double GetAOBit(int channel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the ai bit.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The value.</param>
        public void SetAIBit(int channel, double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the current value of the analog output bit at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The analog value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetAOBit(int channel, double value)
        {
            throw new NotImplementedException();
        }
    }
}
