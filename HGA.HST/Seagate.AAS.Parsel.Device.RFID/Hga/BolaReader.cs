using System;
using System.Text;
using System.Threading;
using System.IO.Ports;

using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.IO.Serial;

namespace Seagate.AAS.Parsel.Device.RFID.Hga
{
    /// <summary>
    /// Summary description for RFID.
    /// </summary>
    public sealed class BolaReader : IHardwareComponent
    {
        // Nested declarations -------------------------------------------------

        public event EventHandler ReadDone;
        public event EventHandler WriteDone;

        // Member variables ----------------------------------------------------

        private RFIDHw _rfidHw;
        private SerialPortSettings _portSettings = new SerialPortSettings();

        private String _name = "";
        private bool _simulation = false;

        public const TagType tagType = TagType.HGA_BOLA2;
        private bool _initialized = false;
        private bool disposed = false;

        // Constructors & Finalizers -------------------------------------------

        public BolaReader()
        {
            _rfidHw = new RFIDHw(tagType);
            InitPortSettings();
        }

        public void Initialize(bool simulation)
        {
            Initialize(simulation, _portSettings);
        }

        public void Initialize(bool simulation, SerialPortSettings portSettings)
        {
            if (_initialized)
                ShutDown();

            _simulation = simulation;
            _portSettings = portSettings;

            if (!simulation)
                _rfidHw.OpenPort(_portSettings);

            EMOReset();
            _initialized = true;
        }

        public void ShutDown()
        {
            if (!_simulation)
                _rfidHw.ClosePort();
        }

        // Properties ----------------------------------------------------------

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool Simulation
        {
            get { return _simulation; }
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        public void EMOReset()
        {
        }

        public int ReadTime
        { get { return _rfidHw.ReadTime; } }

        public int WriteTime
        { get { return _rfidHw.WriteTime; } }

        public SerialPortSettings PortSettings
        {
            get { return _portSettings; }
            set { _portSettings = value; }
        }

        // Methods -------------------------------------------------------------

        public BolaTagData ReadRFIDTag(RFHead head)
        {
            BolaTagData bolaTag = null;
            ReadRFIDTag(head, ref bolaTag);
            return bolaTag;
        }

        public void ReadRFIDTag(RFHead rfHead, ref BolaTagData tag)
        {
            if (_simulation) return;

            try
            {
                Monitor.Enter(this);
                if (tag == null)
                    tag = new BolaTagData();
                _rfidHw.ReadTag(rfHead);
                tag.Clone((BolaTagData)_rfidHw.TagData);
            }
            catch (ExceptionRFID ex) // If fail check sum try to read it.
            {
                if (ex.ErrorCodeRFID == ErrorCodeRFID.CHECKSUM_ERR)
                    tag.Clone((BolaTagData)_rfidHw.TagData);
                else
                {
                    _rfidHw.DestroyTag();
                    tag.Clear();
                }
                throw;
            }
            catch
            {
                _rfidHw.DestroyTag();
                tag.Clear();
                throw;
            }
            finally
            {
                Monitor.Exit(this);

                if (ReadDone != null)
                    ReadDone(null, null);
            }
        }

        public string ReadTrayID(RFHead rfHead)
        {
            if (_simulation) return "";

            string trayID = "";
            try
            {
                Monitor.Enter(this);
                trayID = _rfidHw.ReadID(rfHead);
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this);

                if (ReadDone != null)
                    ReadDone(null, null);
            }
            return trayID;
        }

        public byte ReadCheckSum(RFHead rfHead)
        {
            if (_simulation) return 0;

            byte checkSum = 0;
            try
            {
                Monitor.Enter(this);
                checkSum = _rfidHw.ReadCheckSum(rfHead);
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this);

                if (ReadDone != null)
                    ReadDone(null, null);
            }
            return checkSum;
        }

        public void WriteRFIDTag(RFHead rfHead, BolaTagData tag, bool checkTrayIDBeforeWrite = true)
        {
            bool enterMonitor = false;
            if (_simulation) return;

            try
            {
                Monitor.Enter(this);
                enterMonitor = true;
                ((BolaTagData)_rfidHw.TagData).Clone(tag);
                _rfidHw.WriteTag(rfHead, checkTrayIDBeforeWrite);
            }
            catch
            {
                _rfidHw.DestroyTag();
                throw;
            }
            finally
            {
                if (enterMonitor)
                    Monitor.Exit(this);

                if (WriteDone != null)
                    WriteDone(null, null);
            }
        }

        /// <summary>
        /// This method is used to verify RFID tag by writing 1 byte back to tag
        /// </summary>
        public void WriteCheckSum(RFHead rfHead, byte checkSum)
        {
            bool enterMonitor = false;
            if (_simulation) return;

            try
            {
                Monitor.Enter(this);
                enterMonitor = true;
                _rfidHw.WriteCheckSum(rfHead, checkSum);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (enterMonitor)
                    Monitor.Exit(this);

                if (WriteDone != null)
                    WriteDone(null, null);
            }
        }

        private void InitPortSettings()
        {
            string[] ports = SerialPort.GetPortNames();
            string portName = "COM1";
            if (ports.Length > 0)
                portName = ports[0];
            _portSettings.PortName = portName;
            _portSettings.BaudRate = 9600;
            _portSettings.DataBits = 8;
            _portSettings.Parity = Parity.None;
            _portSettings.StopBits = StopBits.One;
        }
    }
}
