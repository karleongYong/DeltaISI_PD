using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Settings;

namespace XyratexOSC.Hardware
{
    /// <summary>
    /// Represents a hardware device with a single-point of communication, ie. controllers.
    /// </summary>
    public interface IDevice : INamed, IDisposable
    {
        /// <summary>
        /// Represents the port details for connecting to this device.
        /// </summary>
        /// <value>
        /// The port details.
        /// </value>
        IDevicePort Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a simulated device.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulated; otherwise, <c>false</c>.
        /// </value>
        bool Simulated { get; }

        /// <summary>
        /// Gets or sets a value indicating whether device logging is enabled. This is low-level logging used for debugging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if logging; otherwise, <c>false</c>.
        /// </value>
        bool Logging { get; set; }

        /// <summary>
        /// Gets a value indicating whether this device is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets a value indicating whether this device is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        bool Initialized { get; }

        /// <summary>
        /// Opens a connection to this device using the device's specified <see cref="Port"/>.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection to this device.
        /// </summary>
        void Close();

        /// <summary>
        /// Configures the device based on the specified device settings. This is called before <see cref="IPart"/> creation.
        /// </summary>
        void Configure(SettingsNode settings);

        /// <summary>
        /// Initializes the device.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Uninitializes the device.
        /// </summary>
        void Uninitialize();
    }
}
