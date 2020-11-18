using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace XyratexOSC.Hardware
{
    public class DevicePortTcp : IDevicePort
    {
        private int _port = -1;
        private int _replyPort = -1;

        /// <summary>
        /// Gets or sets the device's IP address.
        /// </summary>
        /// <value>
        /// The device IP address.
        /// </value>
        public IPAddress Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the device's IP port.
        /// </summary>
        /// <value>
        /// The device IP port.
        /// </value>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        /// <summary>
        /// Gets or sets the application IP address for device responses.
        /// </summary>
        /// <value>
        /// The application IP address.
        /// </value>
        public IPAddress ReplyAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application IP port for device responses.
        /// </summary>
        /// <value>
        /// The application IP port.
        /// </value>
        public int ReplyPort
        {
            get
            {
                return _replyPort;
            }
            set
            {
                _replyPort = value;
            }
        }
        
        public DevicePortTcp(IPAddress address)
        {
            Address = address;
        }

        public override bool Equals(object obj)
        {
            DevicePortTcp dpt = obj as DevicePortTcp;
            if (object.ReferenceEquals(dpt, null))
                return false;

            return (this.Address == dpt.Address &&
                    this.Port == dpt.Port &&
                    this.ReplyAddress == dpt.ReplyAddress &&
                    this.ReplyPort == dpt.ReplyPort);
        }

        public override int GetHashCode()
        {
            return new { InAddr = Address, InPort = Port, OutAddr = ReplyAddress, OutPort = ReplyPort }.GetHashCode();
        }

        public override string ToString()
        {
            if (Address == null)
                return "TCP=none";

            string tcpString = "TCP=" + Address;
            if (Port > -1)
                tcpString += ":" + Port;

            if (ReplyAddress != null)
            {
                tcpString += ",Reply=" + ReplyAddress;

                if (ReplyPort > -1)
                    tcpString += ":" + ReplyPort;
            }

            return tcpString;
        }

        public static DevicePortTcp FromString(string tcpString)
        {
            if (!tcpString.StartsWith("TCP"))
                throw new InvalidCastException(String.Format("{0} is an invalid TCP string.", tcpString));

            string[] tcpParts = tcpString.Split(',');
            tcpParts[0] = tcpParts[0].Substring(4); // removes "TCP="

            string[] deviceIp = tcpParts[0].Split(':');

            if (String.Equals(deviceIp[0], "none", StringComparison.CurrentCultureIgnoreCase))
                return new DevicePortTcp(null);

            IPAddress ipAddress = IPAddress.Parse(deviceIp[0]);
            DevicePortTcp dpt = new DevicePortTcp(ipAddress);

            if (deviceIp.Length == 2)
                dpt.Port = Int32.Parse(deviceIp[1]);

            if (tcpParts.Length == 2)
            {
                tcpParts[1] = tcpParts[1].Substring(6); // removes "Reply="
                string[] replyIp = tcpParts[1].Split(':');

                IPAddress replyAddress = IPAddress.Parse(replyIp[0]);
                dpt.ReplyAddress = replyAddress;

                if (replyIp.Length == 2)
                    dpt.ReplyPort = Int32.Parse(replyIp[1]);
            }

            return dpt;
        }
    }
}
