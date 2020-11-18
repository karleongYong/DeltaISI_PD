//
//  (c) Copyright 2015 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;

using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

using Seagate.AAS.Utils;
using Seagate.AAS.Parsel;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.IO.Serial;

namespace Seagate.AAS.Parsel.Device.SafetyController
{
    /// <summary>
    /// Summary description for SafetyControllerHw.
    /// </summary>

    public class SafetyController : IDisposable
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        //        private const string safetyControllerNetworkAddress = "";
        //        private const int safetyControllerNetworkPort = 11000;
        //        private const int safetyControllerFinsNodeAddress = 1;
        //        private const int HostFinsNodeAddress = 0;
        //        private const int HOST2CONTROLLER_COMMAND_SIZE = 24; // in byte
        //        private byte[] host2ControllerCommand;

        Dictionary<string, int[]> errorInfo = new Dictionary<string, int[]>
        {
           {"Force mode timeout",                                        new int[] {101, 4}},
           {"Invalid configuration",                                     new int[] {101, 6}},
           {"System failure",                                            new int[] {101, 7}},
           {"External test signal failure at safety input",              new int[] {103, 0}},
           {"Internal circuit error at safety input",                    new int[] {103, 1}},
           {"Discrepancy error at safety input",                         new int[] {103, 2}},
           {"Overload detected at test output",                          new int[] {103, 4}},
           {"Stuck at high detected at test output",                     new int[] {103, 6}},
           {"Under current detected using muting lamp",                  new int[] {103, 7}},
           {"Over current detected at safety output",                    new int[] {104, 0}},
           {"Short circuit detected at safety output",                   new int[] {104, 1}},
           {"Stuck at high detected at safety output",                   new int[] {104, 2}},
           {"Dual channel violation at safety output",                   new int[] {104, 3}},
           {"Internal circuit error at safety output",                   new int[] {104, 4}},
           {"Output PS voltage low",                                     new int[] {105, 1}},
           {"Output PS off circuit error",                               new int[] {105, 4}},
           {"Internal circuit error at test output",                     new int[] {105, 5}},
           {"Function block status error",                               new int[] {107, 2}},
           {"Internal NVS access error",                                 new int[] {108, 0}},
           {"Unsupported expansion IO unit",                             new int[] {108, 1}},
           {"Expansion IO unit maximum connection number exceeded",      new int[] {108, 2}},
           {"Expansion IO unit configuration mismatch",                  new int[] {108, 3}},
           {"Expansion IO bus error",                                    new int[] {108, 4}},
           {"Unsupported option board",                                  new int[] {108, 5}},
           {"Option board communication error or communication timeout", new int[] {108, 6}},
           {"Option board communication error or not mounted",           new int[] {108, 7}},
           {"Memory cassette not inserted or incorrect memory cassette", new int[] {109, 1}},
           {"Memory cassette removed or access error",                   new int[] {109, 2}},
           {"Internal NVS access error during " +
            "execution of memory cassette functions",                    new int[] {109, 3}},
           {"Restore model information mismatch",                        new int[] {109, 4}},
           {"Device password mismatch between " +
            "restore memory cassette and unit",                          new int[] {109, 5}},
           {"Restore prohibit error",                                    new int[] {109, 6}},
           {"Incorrect configuration data at restore",                   new int[] {109, 7}},
           {"Unconfigured unit at backup",                               new int[] {110, 0}},
           {"Unlocked unit at backup error",                             new int[] {110, 1}},
        };


        // tcp protocol
        //        TcpClient clientSocket = new System.Net.Sockets.TcpClient();

        //Serial communication
        const int WRITE_BUFFER_SIZE = 19;
        const int READ_BUFFER_SIZE = 19;
        private SerialPort serialPort = null;
        private SerialPortSettings _portSettings;
        private byte[] writeDataBuffer = new byte[WRITE_BUFFER_SIZE];
        private byte[] readDataBuffer = new byte[READ_BUFFER_SIZE];
//        private byte[] receptionData = new byte[6];
        public byte[] OptionalCommTransmittionData = new byte[4];
        public byte[] inputTerminalDataFlags = new byte[6];
        public byte[] inputTerminalStatusFlags = new byte[6];
        public byte[] outputTerminalDataFlags = new byte[4];
        public byte[] outputTerminalStatusFlags = new byte[4];
        public byte[] presentErrorInformation = new byte[12];
        public List<string> errorList = new List<string>();

        private bool _initialized = false;
        private bool _simulation = false;
        public int Rfid_TimeOut = 3000;

        private String _port = "";
        private int _hPort = 0;

        private String _ErrorMessage = "";

        public event EventHandler DataReceivedFromSafetyController;

        // Constructors & Finalizers -------------------------------------------

        public SafetyController()
        {
            _portSettings = new SerialPortSettings();
            _portSettings.PortName = "COM4";
            _portSettings.BaudRate = 9600;
            _portSettings.DataBits = 8;
            _portSettings.Parity = Parity.None;
            _portSettings.StopBits = StopBits.One;
            _portSettings.DataMode = DataMode.Text;

//            serialPort = new SerialPort();
//            serialPort.PortName = "COM4";
//            serialPort.RtsEnable = false;
//            serialPort.DtrEnable = false;
//            serialPort.BaudRate = 9600;
//            serialPort.ReadTimeout = 10500;
//            serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
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
            {
                // baud, etc aren't likely to change, if so, just add setup values for them
                OpenPort(_portSettings.PortName,
                    _portSettings.BaudRate,
                    _portSettings.Parity.ToString()[0],
                    _portSettings.DataBits,
                    (int)_portSettings.StopBits);

            }
//            EMOReset();
            _initialized = true;
        }

        public void ShutDown()
        {
            if (!_simulation)
            {
                ClosePort();
            }
        }


        // Properties ----------------------------------------------------------

        public SerialPortSettings PortSettings
        {
            get { return _portSettings; }
            set { _portSettings = value; }
        }


        // Methods -------------------------------------------------------------

        public void OpenPort(String port, int baud, int parity, int dataBits, int stopBits)
        {
            _port = port;

            try
            {
                if (_hPort == 0)
                {
                    if (serialPort == null)
                    {
                        serialPort = new SerialPort(port, baud);
                        serialPort.Encoding = System.Text.Encoding.GetEncoding(437);
                        serialPort.ReadTimeout = Rfid_TimeOut;
                        serialPort.DataBits = dataBits;
                        serialPort.StopBits = StopBits.One;
                        serialPort.Parity = Parity.Even;
                        serialPort.NewLine = "\r";                        
                        serialPort.DtrEnable = true;
                        serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
                    }

                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();

                        if (serialPort.IsOpen)
                        {
                            /*Clear Serial Port Buffer*/
                            serialPort.DiscardInBuffer();
                            serialPort.DiscardOutBuffer();
                            _hPort = 1;
                        }

                    }
                }

                //////

                CheckPortOpen();
            }
            catch (Win32Exception)
            { throw ExceptionSafetyController.Create(ErrorCodeSafetyController.INTERNAL_ERR, _ErrorMessage); }
        }

        public void ClosePort()
        {
            try
            {
                if (_hPort != 0)
                {
                    serialPort.Close();
                    _hPort = 0;
                }
            }
            catch (Win32Exception)
            { throw ExceptionSafetyController.Create(ErrorCodeSafetyController.INTERNAL_ERR, _ErrorMessage); }
        }

        public void OpenPort()
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                //                MessageBox.Show("Error opening serial port: " + ex.Message, "Error!");
                //                Log.Error("COM Port", "Error opening serial port: {0}", ex.Message);
            }  
        }

        private void CheckPortOpen()
        {
            if (_hPort == 0)
                throw ExceptionSafetyController.Create(ErrorCodeSafetyController.SerialPortOpenError, _ErrorMessage);
        }

        public void constructAndSendWriteDataBuffer(byte[] receptionData)
        {
            writeDataBuffer = new byte[WRITE_BUFFER_SIZE];
            int checkSum = 0;
            int checkSumHigh = 0;
            int checkSumLow = 0;
            int checkSumBeforeReceptionData = 0xeb;

            checkSum = checkSumBeforeReceptionData;

            for (int i = 0; i < 6; i++)
                checkSum += receptionData[i];

            checkSumHigh = checkSum >> 8;
            checkSumLow = checkSum & 0x00ff;


            writeDataBuffer[0] = 0x40;
            writeDataBuffer[1] = 0x00;
            writeDataBuffer[2] = 0x00;
            writeDataBuffer[3] = 0x0f;
            writeDataBuffer[4] = 0x4b;
            writeDataBuffer[5] = 0x03;
            writeDataBuffer[6] = 0x4d;
            writeDataBuffer[7] = 0x00;
            writeDataBuffer[8] = 0x01;
            writeDataBuffer[9] = receptionData[0];
            writeDataBuffer[10] = receptionData[1];
            writeDataBuffer[11] = receptionData[2];
            writeDataBuffer[12] = receptionData[3];
            writeDataBuffer[13] = receptionData[4];//0x80;
            writeDataBuffer[14] = receptionData[5];
            writeDataBuffer[15] = (byte)checkSumHigh;
            writeDataBuffer[16] = (byte)checkSumLow;
            writeDataBuffer[17] = 0x2a;
            writeDataBuffer[18] = 0x0d;

//            string text = BitConverter.ToString(writeDataBuffer, 0, WRITE_BUFFER_SIZE);
//            MessageBox.Show(text);
            serialPort.Write(writeDataBuffer, 0, WRITE_BUFFER_SIZE);
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            readDataBuffer = new byte[serialPort.ReadBufferSize];

            Thread.Sleep(300);
            int numBytes = serialPort.BytesToRead;
            serialPort.Read(readDataBuffer, 0, numBytes);

//            string text = BitConverter.ToString(readDataBuffer, 0, numBytes);
//            MessageBox.Show(text);

            if (numBytes != 199)
                return;// throw ExceptionSafetyController.Create(ErrorCodeSafetyController.RANGE_ERR, "Number of received byte from safety controller is not 199!");

            verifyChecksum();
            getTransmittionData();
            getPresentErrorInformation();

            EventHandler dataReceived = DataReceivedFromSafetyController;

            if (dataReceived != null)
            {
                dataReceived(this, new EventArgs());
            }
        }

        private void getTransmittionData()
        {
            for (int i = 7; i < 11; i++)
                OptionalCommTransmittionData[i-7] = readDataBuffer[i];

            for (int i = 11; i < 17; i++)
                inputTerminalDataFlags[i-11] = readDataBuffer[i];

            for (int i = 17; i < 21; i++)
                outputTerminalDataFlags[i-17] = readDataBuffer[i];

            for (int i = 21; i < 27; i++)
                inputTerminalStatusFlags[i-21] = readDataBuffer[i];

            for (int i = 27; i < 31; i++)
                outputTerminalStatusFlags[i-27] = readDataBuffer[i];

            for (int i = 101; i < 113; i++)
                presentErrorInformation[i-101] = readDataBuffer[i];
        }

        private void verifyChecksum()
        {
            int checkSum = 0;
            int checkSumHigh = 0;
            int checkSumLow = 0;

            for (int i = 0; i < 195; i++)
                checkSum += readDataBuffer[i];

            checkSumHigh = checkSum >> 8;
            checkSumLow = checkSum & 0x00ff;

            //if (((byte) checkSumHigh != readDataBuffer[195]) || ((byte) checkSumLow != readDataBuffer[196]))
            //    throw ExceptionSafetyController.Create(ErrorCodeSafetyController.CHECKSUM_ERR, "");
        }

        private void getPresentErrorInformation()
        {
//            List<string> errorList = new List<string>();
            errorList = new List<string>();

            foreach (var pair in errorInfo)
            {
                byte testByte = readDataBuffer[errorInfo[pair.Key][0]];
                int bitNumber = errorInfo[pair.Key][1];

                if (isBitOn(testByte, bitNumber))
                    errorList.Add(pair.Key);
             }

//            return errorList;
        }

        private bool isBitOn(byte testByte, int bitNumber)
        {
            return (testByte & (1 << bitNumber)) != 0;
        }
    }
}

