using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using XyratexOSC.Logging;
using XyratexOSC.Settings;
using XyratexOSC.Utilities;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Manages all of our hardware devices/controllers and the software abstractions for interacting with those devices.
    /// </summary>
    public class DeviceManager : IDisposable
    {
        private INamedList<IDevice> _devices = new NamedList<IDevice>();
        private INamedList<IPart> _parts = new NamedList<IPart>();

        private Dictionary<string, Type> _validDeviceTypes = new Dictionary<string, Type>();

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<DeviceManager> _instance
             = new Lazy<DeviceManager>(() => new DeviceManager());

        /// <summary>
        /// Gets the <see cref="DeviceManager"/> instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DeviceManager Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        /// <summary>
        /// Gets the hardware devices.
        /// </summary>
        /// <value>
        /// The hardware devices.
        /// </value>
        public INamedList<IDevice> Devices
        {
            get
            {
                return _devices;
            }
        }

        /// <summary>
        /// Gets the part abstractions.
        /// </summary>
        /// <value>
        /// The part abstractions.
        /// </value>
        public INamedList<IPart> Parts
        {
            get
            {
                return _parts;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceManager"/> class.
        /// </summary>
        private DeviceManager()
        {
            ResolveDevices();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DeviceManager"/> class.
        /// </summary>
        ~DeviceManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool managed)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                IDevice device = Devices[i];

                try
                {
                    if (device == null)
                        continue;

                    if (device.Initialized)
                        device.Uninitialize();

                    if (device.Connected)
                        device.Close();

                    device.Dispose();
                    device = null;
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Failed to dispose of device {0}. {1}", device.Name, ex);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Device Manager";
        }

        /// <summary>
        /// Resolves all of the <see cref="IDevice"/>s available to our application.
        /// </summary>
        protected virtual void ResolveDevices()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            Log.Info(this, "Resolving known device types... Loading all referenced assemblies...");

            // Load all referenced assemblies, if they are already loaded they are skipped
            foreach (AssemblyName an in assembly.GetReferencedAssemblies())
            {
                try
                {
                    Assembly.Load(an);
                }
                catch (Exception ex)
                {
                    Log.Warn(this, "Skipping assembly '{0}'. {1}", an.FullName, ex);
                }
            }

            Type iDeviceType = typeof(IDevice);
            
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName assemblyName = a.GetName();

                if (assemblyName.Name.StartsWith("System") || assemblyName.Name.StartsWith("Microsoft"))
                    continue;

                try
                {
                    foreach (Type type in a.GetTypes())
                    {
                        if (!iDeviceType.IsAssignableFrom(type) || 
                            type.IsAbstract ||
                            type.IsGenericType ||
                            !type.IsClass ||
                            !type.IsPublic)
                        {
                            continue;
                        }

                        var ci = type.GetConstructor(Type.EmptyTypes);

                        if (ci != null && ci.IsPublic)
                        {
                            string name = type.Name.ToLower();

                            if (!_validDeviceTypes.ContainsKey(name))
                                _validDeviceTypes.Add(name, type);

                            DeviceNamesAttribute[] dnas = type.GetCustomAttributes(typeof(DeviceNamesAttribute), true) as DeviceNamesAttribute[];

                            foreach (DeviceNamesAttribute dna in dnas)
                            {
                                foreach (string dnaName in dna.Names)
                                {
                                    string dnaKey = dnaName.ToLower();

                                    if (!_validDeviceTypes.ContainsKey(dnaKey))
                                        _validDeviceTypes.Add(dnaKey, type);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn(this, "Failed to load device types from assembly '{0}'. {1}", a.FullName, ex);
                }
            }
        }

        /// <summary>
        /// Gets the hardware device with the specified name.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns>The device.</returns>
        public virtual IDevice GetDevice(string deviceName)
        {
            return _devices[deviceName];
        }

        /// <summary>
        /// Gets the hardware device of the specified type name and the specified port.
        /// </summary>
        /// <param name="deviceType">Type of the device.</param>
        /// <param name="port">The port.</param>
        /// <returns>The device.</returns>
        public virtual IDevice GetDevice(string deviceType, IDevicePort port)
        {
            foreach (IDevice device in _devices)
            {
                if (!String.Equals(device.GetType().Name, deviceType, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                if (device.Port.Equals(port))
                    return device;
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all the valid device names.
        /// </summary>
        /// <returns>A list of available device names.</returns>
        public IList<string> GetValidDeviceNames()
        {
            return _validDeviceTypes.Keys.ToList<string>();
        }

        /// <summary>
        /// Finds or creates all of the devices specified in the settings node tree.
        /// </summary>
        /// <param name="settings">The settings node tree.</param>
        /// <param name="devSettings">The device settings specific nodes (Nodes/documents which contain a node named "Devices").</param>
        /// <returns>A list of constructed devices.</returns>
        public virtual IList<IDevice> FindOrCreateDevices(SettingsNode settings, out IList<SettingsNode> devSettings)
        {
            devSettings = new List<SettingsNode>();
            List<IDevice> devices = new List<IDevice>();

            SettingsNode devicesNode = settings["Devices"];
            if (devicesNode == null)
                return devices;

            foreach (SettingsNode deviceNode in devicesNode.Nodes)
            {
                IDevice device = FindOrCreateDevice(deviceNode);
                if (device != null)
                {
                    devices.Add(device);
                    devSettings.Add(deviceNode);
                }
            }

            return devices;
        }

        /// <summary>
        /// Finds or creates the device specified in the settings node tree.
        /// </summary>
        /// <param name="deviceNode">The device node.</param>
        /// <returns></returns>
        public virtual IDevice FindOrCreateDevice(SettingsNode deviceNode)
        {
            string deviceName = deviceNode.Name;
            if (deviceNode.ExistsAndHasAValue<string>("Name"))
                deviceName = deviceNode["Name"].GetValueAs<string>();

            if (!deviceNode.ContainsName("Model"))
            {
                Log.Error(this, "Device '{0}' has an undefined model type. Ensure that the device settings include a 'Model' node.", deviceName);
                return null;
            }

            string modelName = deviceNode["Model"].GetValueAs<string>();
            string modelKey = modelName.ToLower();

            if (!_validDeviceTypes.ContainsKey(modelKey))
            {
                Log.Error(this, "Device '{0}' has an unresolved device model type '{1}'. Device cannot be created.", deviceName, modelName);
                return null;
            }

            IDevice device = null;
            IDevicePort port = null;

            device = _devices.FirstOrDefault(d => deviceName.Equals(d.Name, StringComparison.CurrentCultureIgnoreCase));

            if (device == null)
            {

                SettingsNode portNode = deviceNode["Port"];
                if (portNode == null)
                {
                    Log.Error(this, "Device '{0}' is using the default port.", deviceName);
                }
                else
                {
                    try
                    {
                        Log.Info(this, "Device '{0}' discovering port settings...", deviceName);

                        port = CreatePort(portNode);

                        Log.Info(this, "Device '{0}' port settings = {1}", deviceName, port);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this, "Device '{0}' has invalid port settings. {1}", deviceName, ex);
                        Log.Error(this, "Device '{0}' is switching to the default port.", deviceName);
                    }

                    device = _devices.FirstOrDefault(d =>
                        modelName.Equals(d.GetType().Name, StringComparison.CurrentCultureIgnoreCase) &&
                        port.Equals(d.Port));

                    if (device != null)
                        Log.Error(this, "Device '{0}' has the same port settings as Device '{1}'.", deviceName, device.Name);
                }

                try
                {
                    Type type = _validDeviceTypes[modelKey];
                    device = (IDevice)Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Device '{0}' could not be instantiated.", deviceName);
                    Log.Error(this, ex.ToString());
                    return null;
                }

                device.Name = deviceName;
                device.Port = port;

                _devices.Add(device);
            }

            if (deviceNode.ExistsAndHasAValue<bool>("Logging"))
                device.Logging = deviceNode["Logging"].GetValueAs<bool>();
            else if (deviceNode.ExistsAndHasAValue<bool>("Log"))
                device.Logging = deviceNode["Log"].GetValueAs<bool>();

            if (device.Logging)
                Log.Info(this, "Device '{0}' logging is enabled.", deviceName);

            return device;
        }

        /// <summary>
        /// Finds or creates the device with the following name or type/port shorthand.
        /// </summary>
        /// <param name="nameOrShorthand">The device name or type/port shorthand.</param>
        /// <returns></returns>
        public virtual IDevice FindOrCreateDevice(string nameOrShorthand)
        {
            IDevice device = null;
            IDevicePort port = null;

            device = _devices.FirstOrDefault(d => nameOrShorthand.Equals(d.Name, StringComparison.CurrentCultureIgnoreCase));

            if (device != null)
                return device;

            string[] typeAndPort = nameOrShorthand.Split(' ');

            string modelName = typeAndPort[0];
            string modelKey = modelName.ToLower();
            
            if (typeAndPort.Length > 1)
            {
                port = CreatePort(typeAndPort[1]);
            }

            device = _devices.FirstOrDefault(d =>
                modelName.Equals(d.GetType().Name, StringComparison.CurrentCultureIgnoreCase) &&
                (port == null || port.Equals(d.Port)));

            if (device != null)
                return device;

            if (!_validDeviceTypes.ContainsKey(modelKey))
            {
                Log.Error(this, "Device '{0}' has an unresolved device model type '{1}'. Device cannot be created.", nameOrShorthand, modelName);
                return null;
            }

            try
            {
                Type type = _validDeviceTypes[modelKey];
                device = (IDevice)Activator.CreateInstance(type);
                device.Name = modelName;
                device.Port = port;
                return device;
            }
            catch (Exception ex)
            {
                Log.Error(this, "{0} device could not be instantiated.", modelName);
                Log.Error(this, ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Creates a port from the specified settings node tree.
        /// </summary>
        /// <param name="portNode">The port node.</param>
        /// <returns>The device port.</returns>
        /// <exception cref="System.Exception">
        /// Invalid port format.
        /// </exception>
        protected virtual IDevicePort CreatePort(SettingsNode portNode)
        {
            SettingsNode tcpAddrNode = null;

            if (portNode.ContainsName("Address"))
                tcpAddrNode = portNode["Address"];
            else if (portNode.ContainsName("IPAddress"))
                tcpAddrNode = portNode["IPAddress"];
            else if (portNode.ContainsName("IpAddress"))
                tcpAddrNode = portNode["IpAddress"];
            else if (portNode.ContainsName("TCPAddress"))
                tcpAddrNode = portNode["TCPAddress"];
            else if (portNode.ContainsName("TcpAddress"))
                tcpAddrNode = portNode["TcpAddress"];

            if (tcpAddrNode != null)
            {
                string ipAddrName = tcpAddrNode.GetValueAs<string>();
                IPAddress ipAddress; 

                if (!IPAddress.TryParse(ipAddrName, out ipAddress))
                    throw new Exception(String.Format("'{0}' is an invalid IP address.", ipAddrName));

                DevicePortTcp tcpPort = new DevicePortTcp(ipAddress);

                if (portNode.ContainsName("Port"))
                    tcpPort.Port = portNode["Port"].GetValueAs<int>();

                if (portNode.ContainsName("ReplyAddress"))
                {
                    string replyAddrName = portNode["ReplyAddress"].GetValueAs<string>();
                    IPAddress replyAddress;

                    if (!IPAddress.TryParse(replyAddrName, out replyAddress))
                        throw new Exception(String.Format("'{0}' is an invalid reply IP address.", replyAddrName));

                    tcpPort.ReplyAddress = replyAddress;

                    if (portNode.ContainsName("ReplyPort"))
                        tcpPort.ReplyPort = portNode["ReplyPort"].GetValueAs<int>();
                }

                return tcpPort;
            }

            if (portNode.ContainsName("Number"))
            {
                int portNum = portNode["Number"].GetValueAs<int>();
                return new DevicePortNumber(portNum);
            }

            if (portNode.ContainsName("Name"))
            {
                string portName = portNode["Name"].GetValueAs<string>();
                return new DevicePortName(portName);
            }

            if (portNode.HasAValue)
            {
                string portShorthand = portNode.GetValueAs<string>();
                return CreatePort(portShorthand);
            }

            throw new Exception("Invalid port format.");
        }

        /// <summary>
        /// Creates the port for the specified port shorthand.
        /// </summary>
        /// <param name="portShorthand">The port shorthand.</param>
        /// <returns>The device port.</returns>
        protected virtual IDevicePort CreatePort(string portShorthand)
        {
            // Try TCP first because it is the most complicated

            if (portShorthand.StartsWith("TCP"))
            {
                try
                {
                    return DevicePortTcp.FromString(portShorthand);
                }
                catch (Exception)
                {
                    //Do nothing
                }
            }

            int portNum;

            if (Int32.TryParse(portShorthand, out portNum))
                return new DevicePortNumber(portNum);

            return new DevicePortName(portShorthand);
        }

        /// <summary>
        /// Adds the specified devices to our managed list.
        /// </summary>
        /// <param name="devices">The devices.</param>
        public virtual void AddDevices(IEnumerable<IDevice> devices)
        {
            foreach (IDevice device in devices)
                _devices.Add(device);
        }

        /// <summary>
        /// Adds the specified device to our managed list.
        /// </summary>
        /// <param name="device">The device.</param>
        public virtual void AddDevice(IDevice device)
        {
            _devices.Add(device);
        }

        /// <summary>
        /// Removes the specified device from our managed list.
        /// </summary>
        /// <param name="device">The device.</param>
        public virtual void RemoveDevice(IDevice device)
        {
            _devices.Remove(device);
        }

        /// <summary>
        /// Removes the device with the specified name from our managed list.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Cannot remove device because that device name does not exist.</exception>
        public virtual void RemoveDevice(string deviceName)
        {
            IDevice device = _devices[deviceName];
            if (device == null)
                throw new KeyNotFoundException(String.Format("Cannot remove {0} device because that device name does not exist."));
        }

        /// <summary>
        /// Determines whether the device with the specified name is available.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns></returns>
        public virtual bool IsAvailable(string deviceName)
        {
            return _devices.Contains(deviceName);
        }

        /// <summary>
        /// Determines whether the device with the specified name is connected.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns></returns>
        public virtual bool IsConnected(string deviceName)
        {
            IDevice dev = _devices[deviceName];
            if (dev == null)
                return false;

            return dev.Connected;
        }

        /// <summary>
        /// Determines whether the device with the specified name is initialized.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns></returns>
        public virtual bool IsReady(string deviceName)
        {
            IDevice dev = _devices[deviceName];
            if (dev == null)
                return false;

            return dev.Connected && dev.Initialized;
        }
    }
}
