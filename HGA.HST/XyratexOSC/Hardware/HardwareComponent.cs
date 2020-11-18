using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using XyratexOSC.Logging;
using XyratexOSC.Settings;
using XyratexOSC.UI;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Represents a hardware subsystem that manages both <see cref="IDevice"/>s which represent hardware controllers (ex. PowerPMAC in a Motion Component subsystem), 
    /// and <see cref="IPart"/>s which represent abstract sub-units of the physical subsystem (ie. Axes in a Motion Component subsystem)
    /// </summary>
    /// <typeparam name="TDevice">The type of the device.</typeparam>
    /// <typeparam name="TPart">The type of the part.</typeparam>
    public abstract class HardwareComponent<TDevice, TPart> : IDisposable 
        where TDevice : class, IDevice
        where TPart : IPart
    {
        /// <summary>
        /// The global device manager
        /// </summary>
        protected DeviceManager _devMgr = DeviceManager.Instance;

        /// <summary>
        /// A named list of devices initialized that belong to this hardware component
        /// </summary>
        protected INamedList<TDevice> _devices = new NamedList<TDevice>();

        /// <summary>
        /// A named list of parts that belong to this hardware component
        /// </summary>
        protected INamedList<TPart> _parts = new NamedList<TPart>();

        /// <summary>
        /// Gets or sets the component name, which is used for logging and can be used for identifying a component.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets the device name to be used for parts that do not explicitly specify a device.
        /// </summary>
        /// <value>
        /// The name of the default device.
        /// </value>
        public string DefaultDeviceName
        {
            get;
            set;
        }

        public string[] PartsNodeNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tabbed configuration file.
        /// </summary>
        /// <value>The configuration file.</value>
        public string ConfigFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether all of the associated hardware devices are initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized
        {
            get
            {
                foreach (TDevice device in _devices)
                    if (!device.Initialized)
                        return false;

                return true;
            }
        }

        ~HardwareComponent()
        {
            Dispose(false);
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                for (int i = 0; i < _parts.Count; i++)
                {
                    TPart part = _parts[i];

                    try
                    {
                        if (part == null)
                            continue;

                        part.Dispose();
                        part = default(TPart);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this, "Failed to dispose of {0} '{1}'. {2}", part.GetType().Name, part.Name, ex);
                    }
                }
                
                if (_devMgr == null || _devMgr.Devices == null)
                    return;

                for (int i = 0; i < _devMgr.Devices.Count; i++)
                {
                    TDevice device = default(TDevice);

                    try
                    {
                        device = _devMgr.Devices[i] as TDevice;

                        if (device == null)
                            continue;

                        if (device.Initialized)
                            device.Uninitialize();

                        if (device.Connected)
                            device.Close();

                        device.Dispose();
                        device = default(TDevice);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this, "Failed to dispose of {0} '{1}'. {2}", device.GetType().Name, device.Name, ex);
                    }
                }
            }
            else
            {
                //TODO
            }
        }

        protected virtual void SetupLogging()
        {
            TraceSource trace = Log.Trace(Name);
            trace.Switch.Level = SourceLevels.All;

            string logFilePath = string.Format("Logs\\{0}.{1}.log", Environment.MachineName, Name);
            DailyTextWriterListener dailyListener = new DailyTextWriterListener(logFilePath);          
            trace.Listeners.Add(dailyListener);
        }

        public virtual void Initialize()
        {
            Initialize(ConfigFilePath);
        }

        /// <summary>
        /// Initializes the specified configuration files.
        /// </summary>
        /// <param name="configFiles">The configuration files.</param>
        public virtual void Initialize(params string[] configFiles)
        {
            SetupLogging();

            Log.Info(this, "Starting {0} hardware configuration...", Name);

            List<SettingsNode> settingsDocs = new List<SettingsNode>();

            foreach (string file in configFiles)
            {
                if (!File.Exists(file))
                {
                    Log.Error(this, "Settings file '{0}' does not exist. Skipping devices and parts listed in file.", file);
                    continue;
                }

                SettingsDocument doc = new SettingsDocument(file);
                settingsDocs.Add(doc);
            }

            if (settingsDocs.Count == 0)
            {
                Log.Error(this, "No valid settings files. Generating a default settings file template...");

                SettingsDocument simDoc = GenerateSettingsTemplate();
                simDoc.Save(ConfigFilePath);

                settingsDocs.Add(simDoc);
            }

            Dictionary<IDevice, SettingsNode> devSettings = new Dictionary<IDevice, SettingsNode>();

            // Create devices
            foreach (SettingsNode doc in settingsDocs)
            {
                IList<SettingsNode> devSettingNodes;
                IList<IDevice> devices = _devMgr.FindOrCreateDevices(doc, out devSettingNodes);

                IList<TDevice> myDevices = devices.Where(d => d is TDevice)
                                                  .Select(d => (TDevice)d)
                                                  .ToList();

                _devices.AddRange(myDevices);

                for (int i = 0; i < devices.Count; i++)
                {
                    devSettings.Add(devices[i], devSettingNodes[i]);
                }
            }

            // Connect to all devices
            foreach (TDevice device in _devices)
            {
                try
                {
                    if (!device.Connected)
                        device.Open();
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Unable to open a connect to {0} device '{1}' on port {2}. {3}", device.GetType().Name, device.Name, device.Port, ex);
                }
            }

            // Configure all devices
            foreach (KeyValuePair<IDevice, SettingsNode> kvp in devSettings)
            {
                if (!(kvp.Key is TDevice))
                    continue;

                TDevice myDevice = (TDevice)kvp.Key;

                try
                {
                    myDevice.Configure(kvp.Value);
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Unable to configure {0} device '{1}' from . {2}", myDevice.GetType().Name, myDevice.Name, ex);
                }
            }

            // Create parts
            List<string> forcedParts = new List<string>();

            foreach (SettingsNode partsDoc in settingsDocs)
            {
                IList<TPart> parts;
                IList<string> forced = CreateParts(partsDoc, out parts);
                
                // This handles the old initialization files

                if (parts.Count == 0)
                {
                    string partsDocName = Path.GetFileName(partsDoc.Name);
                    string configFileName = Path.GetFileName(ConfigFilePath);

                    if (String.Equals(partsDocName, configFileName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        forced = TrySpecificInit(partsDoc, out parts);
                    }
                }

                _parts.AddRange(parts);
                forcedParts.AddRange(forced);
            }

            Log.Info(this, "{0} parts defined as follows:", Name);

            List<IDevice> partDevices = new List<IDevice>();

            foreach (TPart part in _parts)
            {
                Log.Info(this, "{0}: {1} @ {2}", part.Name, part.Device.Name, part.Device.Port);

                if (!partDevices.Contains(part.Device))
                    partDevices.Add(part.Device);
            }

            RegisterParts(_parts);

            foreach (IDevice device in partDevices)
            {
                if (!_devices.Contains(device.Name))
                    _devices.Add((TDevice)device);

                try
                {
                    device.Initialize();
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Failed to initialize {0} device. {1}", device.Name, ex.Message);
                }
            }

            if (forcedParts.Count > 0)
            {
                string message = String.Format("The following parts were forced into simulation mode:{0}{1}{0}Please see the log for more details.",
                    Environment.NewLine,
                    String.Join(Environment.NewLine, forcedParts));

                Notify.PopUpError(Name + " Forcing Simulation...", message);
            }

            Log.Info(this, "Completed {0} hardware configuration", Name);
        }

        /// <summary>
        /// Registers all parts to their parent device. This is called after <see cref="IPart"/> creation.
        /// </summary>
        public abstract void RegisterParts(IEnumerable<TPart> parts);

        /// <summary>
        /// Initializes this HardwareComponent using the V1 config files. This is only used when no parts were found on the first pass.
        /// </summary>
        public abstract IList<string> TrySpecificInit(SettingsNode settings, out IList<TPart> parts);

        public abstract SettingsDocument GenerateSettingsTemplate();

        protected abstract Type GetSimDeviceType();

        protected abstract TPart CreatePart(SettingsNode partNode, TDevice device);

        public virtual IList<string> CreateParts(SettingsNode settings, out IList<TPart> parts)
        {
            List<string> forcedNames = new List<string>();
            parts = new List<TPart>();

            foreach (SettingsNode node in settings.Nodes)
            {
                if (node == null || !PartsNodeNames.Contains(node.Name))
                    continue;

                foreach (SettingsNode partNode in node.Nodes)
                {
                    TPart part = default(TPart);
                    string deviceName = DefaultDeviceName;

                    if (SettingsNode.ExistsAndHasAValue(partNode["Device"]))
                        deviceName = partNode["Device"].GetValueAs<string>();
                    else if (SettingsNode.ExistsAndHasAValue(partNode["Controller"]))
                        deviceName = partNode["Controller"].GetValueAs<string>();

                    // Allow Simulated to represent the current hardware component simulated type
                    if (string.Equals(deviceName, "Simulated", StringComparison.CurrentCultureIgnoreCase) ||
                        string.Equals(deviceName, "Simulate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deviceName = GetSimDeviceType().Name;
                    }

                    TDevice device = GetDevice(deviceName);

                    if (device == null)
                    {
                        string simName = GetSimDeviceType().Name;

                        if (!String.Equals(deviceName, simName, StringComparison.CurrentCultureIgnoreCase))
                            Log.Error(this, "No device found with the name '{0}'. Switching to a Simulated {1} device instead.", deviceName, Name);

                        device = GetSimDevice(deviceName);
                    }

                    string partName = partNode.Name;

                    try
                    {
                        if (!device.Connected)
                            device.Open();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this, "{0} could not connect to {1} device '{2}' on port {3}. {4}", typeof(TPart).Name, device.GetType().Name, device.Name, device.Port, ex);
                        Log.Error(this, "Switching {0} '{1}' to simulated device instead.", typeof(TPart).Name, partName);
                        device = GetSimDevice(deviceName);
                    }

                    part = CreatePart(partNode, device);

                    if (!String.Equals(device.Name, deviceName, StringComparison.CurrentCultureIgnoreCase))
                        forcedNames.Add(part.Name);

                    parts.Add(part);
                }
            }

            return forcedNames;
        }

        protected TDevice GetDevice(string name)
        {
            TDevice device = default(TDevice);

            try
            {
                device = (TDevice)_devMgr.Devices[name];
            }
            catch
            {
                device = default(TDevice);
            }

            return device;
        }

        protected TDevice GetSimDevice()
        {
            return GetSimDevice("");
        }

        protected TDevice GetSimDevice(string name)
        {
            Type simType = GetSimDeviceType();

            if (String.IsNullOrEmpty(name))
                name = simType.Name;

            TDevice device = GetDevice(name);

            if (device != null && device.GetType() == simType)
                return device;

            Log.Warn(this, "Creating a Simulated {0} device...", Name);

            device = (TDevice)Activator.CreateInstance(simType);
            device.Name = name;

            _devMgr.AddDevice(device);

            Log.Info(this, "Successfully created Simulated {0} device.", name);

            return device;
        }
    }
}
