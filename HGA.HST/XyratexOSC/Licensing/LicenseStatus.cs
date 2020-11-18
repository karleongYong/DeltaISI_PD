using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Licensing
{
    #region Enum : LicenseStatus

    /// <summary>
    /// Specifies the authentication status that was found during a <see cref="LicenseLock.Verify()"/>.
    /// </summary>
    public enum LicenseStatus
    {
        /// <summary>Failed to authenticate.</summary>
        Failed = 0,
        /// <summary>Authenticated with the Window's User Id.</summary>
        UserId = 1,
        /// <summary>Authenticated with a security dongle.</summary>
        Key = 2
    }

    #endregion Enum : LicenseStatus

    #region Enum : LicenseFeatures

    /// <summary>
    /// Specifies the set of application features that are enabled through the current license.
    /// </summary>
    [Flags]
    public enum LicenseFeatures
    {
        /// <summary>
        /// No specific features enabled
        /// </summary>
        None = 0,
        /// <summary>
        /// ABS inspection is enabled
        /// </summary>
        ABS = 1 << 0,
        /// <summary>
        /// Pole inspection is enabled
        /// </summary>
        POLE = 1 << 1,
        /// <summary>
        /// Backpad inspection is enabled
        /// </summary>
        BACKPAD = 1 << 2,
        /// <summary>
        /// CE inspection is enabled
        /// </summary>
        CE = 1 << 3,
        /// <summary>
        /// OCR edge inspection is enabled
        /// </summary>
        OCR = 1 << 4,
        /// <summary>
        /// Both long-side inspections are enabled
        /// </summary>
        LONGSIDES = 1 << 5,
        /// <summary>
        /// Depo side inspection is enabled
        /// </summary>
        DEPO = 1 << 6
    }

    #endregion

    #region Enum : LicenseKeyCode

    /// <summary>
    /// Specifies the Ultrapro Key Failure Code that was returned during the attempt to get the license.
    /// </summary>
    public enum LicenseKeyCode
    {
        /// <summary>
        /// The function completed successfully.
        /// </summary>
        [Description("Success.")]
        SUCCESS = 0,
        /// <summary>
        /// You specified an invalid function code.
        /// </summary>
        [Description("Invalid function code.")]
        INVALID_FUNCTION_CODE = 1,
        /// <summary>
        /// A checksum error was detected in the command packet, indicating an internal inconsistency. Typically a threading issue.
        /// </summary>
        [Description("Packet error.")]
        INVALID_PACKET = 2,
        /// <summary>
        /// Unable to find the desired hardware key.
        /// </summary>
        [Description("Key not found.")]
        UNIT_NOT_FOUND = 3,
        /// <summary>
        /// You attempted to perform an illegal action on a cell.
        /// </summary>
        [Description("Access denied.")]
        ACCESS_DENIED = 4,
        /// <summary>
        /// You specified an invalid memory address.
        /// </summary>
        [Description("Invalid memory address.")]
        INVALID_MEMORY_ADDRESS = 5,
        /// <summary>
        /// You specified an invalid access code.
        /// </summary>
        [Description("Invalid access code.")]
        INVALID_ACCESS_CODE = 6,
        /// <summary>
        /// The requested operation could not be completed because the port is busy.
        /// </summary>
        [Description("Key port is busy.")]
        PORT_IS_BUSY = 7,
        /// <summary>
        /// The write or decrement operation could not be performed due to lack of sufficient power.
        /// </summary>
        [Description("Write not ready.")]
        WRITE_NOT_READY = 8,
        /// <summary>
        /// No parallel ports could be found, or there was a problem with the protocol being used on the network.
        /// </summary>
        [Description("No port found.")]
        NO_PORT_FOUND = 9,
        /// <summary>
        /// You tried to decrement a counter that contains the value zero. 
        /// </summary>
        [Description("Already zero.")]
        ALREADY_ZERO = 10,
        /// <summary>
        /// The SafeNet Sentinel Driver was not installed or detected.
        /// </summary>
        [Description("SafeNet driver open error.")]
        DRIVER_OPEN_ERROR = 11,
        /// <summary>
        /// The DRIVE r_ NO t_ INSTALLED
        /// </summary>
        [Description("Safenet driver not installed.")]
        DRIVER_NOT_INSTALLED = 12,
        /// <summary>
        /// The system device driver is having problems communicating.
        /// </summary>
        [Description("IO communication error.")]
        IO_COMMUNICATIONS_ERROR = 13,
        /// <summary>
        /// The memory allocated for the API packet is less than the required size.
        /// </summary>
        [Description("Packet too small.")]
        PACKET_TOO_SMALL = 15,
        /// <summary>
        /// Arguments and values passed to the API function are invalid.
        /// </summary>
        [Description("Invalid parameter.")]
        INVALID_PARAMETER = 16,
        /// <summary>
        /// Memory access error.
        /// </summary>
        [Description("Memory access error.")]
        MEM_ACCESS_ERROR = 17,
        /// <summary>
        /// The current system device driver is outdated. Update the driver. 
        /// </summary>
        [Description("Version not supported.")]
        VERSION_NOT_SUPPORTED = 18,
        /// <summary>
        /// The operating system or environment is not supported by the client library. 
        /// </summary>
        [Description("OS not supported.")]
        OS_NOT_SUPPORTED = 19,
        /// <summary>
        /// You sent a query string longer than 56 characters. Send a shorter string.
        /// </summary>
        [Description("Query too long.")]
        QUERY_TOO_LONG = 20,
        /// <summary>
        /// An invalid command was specified in the API call.
        /// </summary>
        [Description("Invalid command.")]
        INVALID_COMMAND = 21,
        /// <summary>
        /// Memory alignment error.
        /// </summary>
        [Description("Memory alignment error.")]
        MEM_ALIGNMENT_ERROR = 29,
        /// <summary>
        /// The Sentinel Driver is busy. Try the function again.
        /// </summary>
        [Description("Driver is busy.")]
        DRIVER_IS_BUSY = 30,
        /// <summary>
        /// Failure to allocate a parallel port through the operating system’s parallel port contention handler.
        /// </summary>
        [Description("Port allocation failure.")]
        PORT_ALLOCATION_FAILURE = 31,
        /// <summary>
        /// Failure to release a previously allocated parallel port through the operating system’s parallel port contention handler.
        /// </summary>
        [Description("Port release failure.")]
        PORT_RELEASE_FAILURE = 32,
        /// <summary>
        /// Failure to access the parallel port within the defined time.
        /// </summary>
        [Description("Acquire port timeout.")]
        ACQUIRE_PORT_TIMEOUT = 39,
        /// <summary>
        /// The particular system does not support a signal line.
        /// </summary>
        [Description("Signal not supported.")]
        SIGNAL_NOT_SUPPORTED = 42,
        /// <summary>
        /// Unknown machine.
        /// </summary>
        [Description("Unknown machine.")]
        UNKNOWN_MACHINE = 44,
        /// <summary>
        /// System API error.
        /// </summary>
        [Description("System API error.")]
        SYS_API_ERROR = 45,
        /// <summary>
        /// The key is busy.
        /// </summary>
        [Description("Key is busy.")]
        UNIT_IS_BUSY = 46,
        /// <summary>
        /// Invalid port type.
        /// </summary>
        [Description("Invalid port type.")]
        INVALID_PORT_TYPE = 47,
        /// <summary>
        /// Invalid machine type.
        /// </summary>
        [Description("Invalid machine type.")]
        INVALID_MACH_TYPE = 48,
        /// <summary>
        /// Invalid IRQ mask.
        /// </summary>
        [Description("Invalid IRQ mask.")]
        INVALID_IRQ_MASK = 49,
        /// <summary>
        /// Invalid cont method.
        /// </summary>
        [Description("Invalid cont method.")]
        INVALID_CONT_METHOD = 50,
        /// <summary>
        /// Invalid port flags.
        /// </summary>
        [Description("Invalid port flags.")]
        INVALID_PORT_FLAGS = 51,
        /// <summary>
        /// Invalid log port config.
        /// </summary>
        [Description("Invalid log port config.")]
        INVALID_LOG_PORT_CFG = 52,
        /// <summary>
        /// Invalid OS type.
        /// </summary>
        [Description("Invalid OS type.")]
        INVALID_OS_TYPE = 53,
        /// <summary>
        /// Invalid log port number.
        /// </summary>
        [Description("Invalid log port num")]
        INVALID_LOG_PORT_NUM = 54,
        /// <summary>
        /// Invalid router flags.
        /// </summary>
        [Description("Invalid router flags.")]
        INVALID_ROUTER_FLGS = 56,
        /// <summary>
        /// Initialization was not called.
        /// </summary>
        [Description("Initialization not called.")]
        INIT_NOT_CALLED = 57,
        /// <summary>
        /// The type of driver access, either direct I/O or system driver, is not supported for the defined operating system and client library.
        /// </summary>
        [Description("Driver type not supported.")]
        DRVR_TYPE_NOT_SUPPORTED = 58,
        /// <summary>
        /// The client library failed to communicate with the Sentinel Driver.
        /// </summary>
        [Description("Fail on driver communication.")]
        FAIL_ON_DRIVER_COMM = 59
    }

    #endregion

    /// <summary>
    /// An authentication package describing the status and features found during a <see cref="LicenseLock.Verify()"/>.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// Gets the license status of a <see cref="LicenseLock.Verify()"/>.
        /// </summary>
        /// <value>
        /// The license status.
        /// </value>
        public LicenseStatus Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the bitwise value specifying which features are enabled by this license.
        /// </summary>
        /// <value>
        /// The enabled features.
        /// </value>
        public LicenseFeatures Features
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the associated Ultrapro key failure-code.
        /// </summary>
        /// <value>
        /// The Ultrapro key code.
        /// </value>
        public LicenseKeyCode KeyCode
        {
            get;
            private set;
        }

        /// <summary>
        /// The details of this authentication in a friendly string.
        /// </summary>
        /// <value>
        /// The authentication details.
        /// </value>
        public string Details
        {
            get;
            set;
        }

        internal Authentication(LicenseStatus status, LicenseFeatures features, LicenseKeyCode keyCode)
        {
            Status = status;
            Features = features;
            KeyCode = keyCode;
        }

        /// <summary>
        /// Determines whether this authentication packet has a valid license.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the license is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid()
        {
            return Status > 0;
        }
    }
}
