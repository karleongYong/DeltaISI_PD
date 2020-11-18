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
//  [2007/09/19] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO.Ports;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using Seagate.AAS.IO.Serial;
using Seagate.AAS.Utils;
using System.Threading;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Device.RFID.Hga;

namespace Seagate.AAS.Parsel.Device.RFID
{
    public class RFIDHw : IDisposable
    {
        private SerialPort serialPort = null;
        private StopwatchW32 _stopWatchRead;
        private StopwatchW32 _stopWatchWrite;        

        public string PortName { get; private set; }
        public int BaudRate { get; private set; }
        public int DataBits { get; private set; }
        public Parity Parity { get; private set; }
        public StopBits StopBits { get; private set; }
        public int ReadTimeOut { get; private set; }

        public TagType TagType { get; private set; }
        public RFIDTagData TagData { get; set; }

        private bool disposed = false;
        private object _lock = new object();

        #region Properties
        public int ReadTime
        { get { return _stopWatchRead.ElapsedTime_msec; } }

        public int WriteTime
        { get { return _stopWatchWrite.ElapsedTime_msec; } }
        #endregion

        public RFIDHw(TagType tagType)
        {
            _stopWatchRead = new StopwatchW32();
            _stopWatchWrite = new StopwatchW32();
            TagType = tagType;
            InitSerialPort();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ClosePort();
                }
                disposed = true;
            }
        }

        public void OpenPort(string portName)
        {
            PortName = portName;
            BaudRate = 9600;
            DataBits = 8;
            Parity = Parity.None;
            StopBits = StopBits.One;

            OpenPort();
        }

        public void OpenPort(String portName, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            PortName = portName;
            BaudRate = baudRate;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;

            OpenPort();
        }

        public void OpenPort(SerialPortSettings portSettings)
        {
            PortName = portSettings.PortName;
            BaudRate = portSettings.BaudRate;
            DataBits = portSettings.DataBits;
            Parity = portSettings.Parity;
            StopBits = portSettings.StopBits;

            OpenPort();
        }

        public void ClosePort()
        {
            DestroyTag();
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }

        public void DestroyTag()
        {
            if (TagData == null) return;

            switch (TagType)
            {
                case TagType.HGA_FOLA: ((FolaTagData)TagData).Clear(); break;
                case TagType.HGA_BOLA:
                case TagType.HGA_BOLA2: ((BolaTagData)TagData).Clear(); break;
            }
        }

        public void ReadTag(RFHead head)
        {
            CheckPortOpen();
            ReadRFIDTag(head);
            //ReadRFIDTag_Eval3370(head);
        }

        public string ReadSN(RFHead head)
        {
            CheckPortOpen();
            return ReadRFIDSN(head);
        }

        public string ReadID(RFHead head)
        {
            CheckPortOpen();
            return ReadRFIDID(head);
        }

        public byte ReadCheckSum(RFHead head)
        {
            CheckPortOpen();
            return ReadRFIDCheckSum(head);
        }

        public void WriteTag(RFHead head, bool checkIDBeforeWrite = true)
        {
            CheckPortOpen();
            WriteRFIDTag(head, checkIDBeforeWrite);
        }

        /// <summary>
        /// This method is used to verify RFID tag by writing 1 byte back to tag
        /// </summary>
        public void WriteCheckSum(RFHead head, byte checkSum)
        {
            CheckPortOpen();
            WriteRFIDCheckSum(head, checkSum);
        }

        #region Private Methods
        private void InitTagData()
        {
            switch (TagType)
            {
                case TagType.HGA_FOLA: TagData = new FolaTagData(); break;
                case TagType.HGA_BOLA:
                case TagType.HGA_BOLA2: TagData = new BolaTagData(); break;
            }

        }

        private void InitSerialPort()
        {
            PortName = "";
            BaudRate = 9600;
            DataBits = 8;
            Parity = System.IO.Ports.Parity.None;
            StopBits = System.IO.Ports.StopBits.One;
            ReadTimeOut = 3000;
        }

        private void OpenPort()
        {
            try
            {
                if (serialPort == null)
                    serialPort = new SerialPort(PortName);
                else
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    serialPort.PortName = PortName;
                }
                serialPort.BaudRate = BaudRate;
                serialPort.DataBits = DataBits;
                serialPort.Parity = Parity;
                serialPort.StopBits = StopBits;
                serialPort.ReadTimeout = ReadTimeOut;
                serialPort.Encoding = Encoding.GetEncoding(28591); // 28591 = iso-8859-1 Western European (ISO)   
                serialPort.NewLine = "\r";

                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    InitTagData();
                }
                else throw ExceptionRFID.Create(ErrorCodeRFID.SerialPortOpenError, PortName);
            }
            catch
            {
                throw ExceptionRFID.Create(ErrorCodeRFID.SerialPortOpenError, PortName);
            }
        }
        
        private void CheckPortOpen()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                throw ExceptionRFID.Create(ErrorCodeRFID.SerialPortOpenError, PortName);
            }
        }

        private void ReadRFIDTag_Eval3370(RFHead hHead)
        {
            lock (_lock)
            {

                int loopCount = 0;
                bool readOK = false;

                while (loopCount < 2 && !readOK)
                {
                    loopCount++;
                    serialPort.DiscardInBuffer();
                    _stopWatchRead.Reset();
                    _stopWatchRead.Start();

                    try
                    {
                        serialPort.Write(string.Format("RDA{0}0000FE*\r", (int)hHead + 1));
                        var readData = serialPort.ReadExisting();
                        var le = readData.Length - 10;
                        var firstData = readData.Substring(0, le);
                        var lastData = readData.Substring(le, 10);
                        var newData = lastData.Replace("*\r", string.Empty);
                        var allData = firstData + newData;

                        TagData.DecodeReadData(allData, true);
                        readOK = true;
                    }
                    catch (ExceptionRFID ex)
                    {
                        if (ex.ErrorCodeRFID != ErrorCodeRFID.INVALID_DATA || (ex.ErrorCodeRFID == ErrorCodeRFID.INVALID_DATA && loopCount > 1))
                        {
                            loopCount = 10; // exit loop
                            throw;
                        }
                    }
                    catch
                    {
                        loopCount = 10; // exit loop
                        throw;
                    }
                    finally
                    {
                        _stopWatchRead.Stop();
                    }
                }
            }
        }

        private void ReadRFIDTag(RFHead hHead)
        {
            lock (_lock)
            {

                int loopCount = 0;
                bool readOK = false;

                while (loopCount < 2 && !readOK)
                {
                    loopCount++;
                    serialPort.DiscardInBuffer();
                    _stopWatchRead.Reset();
                    _stopWatchRead.Start();

                    try
                    {
                        serialPort.Write(string.Format("RDA{0}0000FE*\r", (int)hHead + 1));
                        var readData = "";
                        var retryCount = 0;
                        //Thread.Sleep(1000);
                        readData = serialPort.ReadTo("*\r").ToString() + "";
                        if ((readData.Length != 0) && (readData.Length < 30))
                        {
                           // readData = serialPort.ReadTo(" *\r").ToString() + " ";
                          //  serialPort.DiscardInBuffer();
                          //  serialPort.Write(string.Format("RDA{0}0000FE*\r", (int)hHead + 1));
                            Thread.Sleep(800);
                           
                            readData = readData + "*\r" + serialPort.ReadLine();
                           if (readData.Substring(readData.Length - 1,1) == "*")
                           {
                               readData = readData.Substring(0, readData.Length - 1);

                           }
                        }
                          //      var le = readData.Length - 10;
                           //     var firstData = readData.Substring(0, le);
                           //     var lastData = readData.Substring(le, 10);
                           //     var newData = lastData.Replace("*\r", string.Empty);
                           //     var allData = firstData + newData;
                        //readData = readData.Replace("*\r", string.Empty);
                        //serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort.PortName()) ;
                        //while ((retryCount < 8) && (readData == ""))
                        
                       // {
                         //   Thread.Sleep(200);
                           // readData = serialPort.ReadExisting().ToString();
                           

                           // if (readData != "")
                          //  {
                            //    if (readData.Length > 128)
                              //  {
                              //  readData = readData.Replace("*\r", string.Empty);
                              //  retryCount = 1000;
                              //  }

                      //      }
                        //    else
                          //  {
                        //        retryCount++;
                       //     }
                        //}
                        
                      //  TagData.DecodeReadData(serialPort.ReadTo("*\r"), true);
                        TagData.DecodeReadData(readData , true);
                      //  readOK = true;
                       
                     //   serialPort.Write(string.Format("RDA{0}0000FE*\r", (int)hHead + 1));
                       
                     //   Application.DoEvents();
                 //       var readData = serialPort.ReadExisting();
                       // Thread.Sleep(500);
                //        var le = readData.Length - 10;
                //        var firstData = readData.Substring(0, le);
                //        var lastData = readData.Substring(le, 10);
                //        var newData = lastData.Replace("*\r", string.Empty);
                //        var allData = firstData + newData;

                 //       TagData.DecodeReadData(allData, true);
                        readOK = true;

                    }
                    catch (ExceptionRFID ex)
                    {
                        if (ex.ErrorCodeRFID != ErrorCodeRFID.INVALID_DATA || (ex.ErrorCodeRFID == ErrorCodeRFID.INVALID_DATA && loopCount > 1))
                        {
                            loopCount = 10; // exit loop
                            throw;
                        }
                    }
                    catch
                    {
                        loopCount = 10; // exit loop
                        throw;
                    }
                    finally
                    {
                        _stopWatchRead.Stop();
                    }
                }
            }
        }
        private string ReadRFIDSN(RFHead hHead)
        {
            serialPort.DiscardInBuffer();
            int startSNByte = 18;
            try
            {
                serialPort.Write(string.Format("RDA{0}00{1}08*\r", (int)hHead + 1, startSNByte.ToString("X2")));
                return TagData.DecodeID(serialPort.ReadTo("*\r"));
            }
            catch
            {
                throw;
            }
        }


        private string ReadRFIDID(RFHead hHead)
        {
            serialPort.DiscardInBuffer();

            try
            {
                serialPort.Write(string.Format("RDA{0}00{1}03*\r", (int)hHead + 1, TagData.IDByteNo.ToString("X2")));
                return TagData.DecodeID(serialPort.ReadTo("*\r"));
            }
            catch
            {
                throw;
            }
        }

        private byte ReadRFIDCheckSum(RFHead hHead)
        {
            try
            {
                serialPort.Write(string.Format("RDA{0}00{1}01*\r", (int)hHead + 1, TagData.CheckSumByteNo.ToString("X2")));
                return TagData.DecodeCheckSum(serialPort.ReadTo("*\r"));
            }
            catch
            {
                throw;
            }
        }

        private void WriteRFIDTag(RFHead hHead, bool checkIDBeforeWrite)
        {
            serialPort.DiscardInBuffer();
            _stopWatchWrite.Reset();
            _stopWatchWrite.Start();

            try
            {
                if (checkIDBeforeWrite)
                {
                    string id = ReadID(hHead);
                    TagData.CheckID(id);
                }

                List<int> ascii13ByteNoList;
                string writeStr = string.Format("WTA{0}0000{1}*\r", (int)hHead + 1, TagData.GetWriteString(out ascii13ByteNoList));
                serialPort.Write(writeStr);
                TagData.DecodeWriteResponse(serialPort.ReadTo("*\r"));
                if (ascii13ByteNoList.Count > 0)
                {
                    for (int i = 0; i < ascii13ByteNoList.Count; i++)
                    {
                        serialPort.DiscardInBuffer();
                        writeStr = string.Format("WTH{0}00{1}0D*\r", (int)hHead + 1, ascii13ByteNoList[i].ToString("X2"));
                        serialPort.Write(writeStr);
                        TagData.DecodeWriteResponse(serialPort.ReadTo("*\r"));
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _stopWatchWrite.Stop();
            }
        }

        private void WriteRFIDCheckSum(RFHead hHead, byte checkSum)
        {
            serialPort.DiscardInBuffer();
            _stopWatchWrite.Reset();
            _stopWatchWrite.Start();

            try
            {
                string writeStr = string.Format("WTH{0}00{1}{2}*\r", (int)hHead + 1, TagData.CheckSumByteNo.ToString("X2"), checkSum.ToString("X2"));
                serialPort.Write(writeStr);
                TagData.DecodeWriteResponse(serialPort.ReadTo("*\r"));
            }
            catch
            {
                throw;
            }
            finally
            {
                _stopWatchWrite.Stop();
            }
        }
        #endregion
    }
}