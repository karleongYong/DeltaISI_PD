//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/09/20] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

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
    public sealed class FolaReader : IHardwareComponent
    {
        // Nested declarations -------------------------------------------------

        public event EventHandler ReadDone;
        public event EventHandler WriteDone;

        // Member variables ----------------------------------------------------

        private RFIDHw _rfidHw;
        private SerialPortSettings _portSettings = new SerialPortSettings();

        private String _name = "";
        private bool _simulation = false;
        //private bool _fullWriteAccess = false;

        public const TagType tagType = TagType.HGA_FOLA;
        private bool _initialized = false;
        private object _lock = new object();
        // Constructors & Finalizers -------------------------------------------

        public FolaReader()
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
            //_fullWriteAccess = writeAccess;
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

        public void EMOReset()
        {
        }

        //public bool FullWriteAccess
        //{
        //    get { return _fullWriteAccess; }
        //    set { _fullWriteAccess = value; }
        //}

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

        public FolaTagData ReadRFIDTag(RFHead head)
        {
            FolaTagData folaTag = null;
            ReadRFIDTag(head, ref folaTag);
            return folaTag;
        }

        public void ReadRFIDTag(RFHead rfHead, ref FolaTagData tag)
        {
            if (_simulation) return;
            lock (_lock)
            {
                try
                {
                    Monitor.Enter(this);
                    if (tag == null)
                        tag = new FolaTagData();
                    _rfidHw.ReadTag(rfHead);
                    tag.Clone((FolaTagData)_rfidHw.TagData);
                }
                catch (ExceptionRFID ex) // If fail check sum try to read it.
                {
                    if (ex.ErrorCodeRFID == ErrorCodeRFID.CHECKSUM_ERR)
                        tag.Clone((FolaTagData)_rfidHw.TagData);
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
        }

        public string ReadCarrierSN(RFHead rfHead)
        {
            if (_simulation) return "";

            string carrierSN = "";
            try
            {
                Monitor.Enter(this);
                carrierSN = _rfidHw.ReadSN(rfHead);
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
            return carrierSN;
        }

        public string ReadCarrierID(RFHead rfHead)
        {
            if (_simulation) return "";

            string carrierID = "";
            try
            {
                Monitor.Enter(this);
                carrierID = _rfidHw.ReadID(rfHead);
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
            return carrierID;
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

        public void WriteRFIDTag(RFHead rfHead, FolaTagData tag, bool checkCarrierIDBeforeWrite = true)
        {
            bool enterMonitor = false;
            if (_simulation) return;

            try
            {
                Monitor.Enter(this);
                enterMonitor = true;
                ((FolaTagData)_rfidHw.TagData).Clone(tag);
                _rfidHw.WriteTag(rfHead, checkCarrierIDBeforeWrite);
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
