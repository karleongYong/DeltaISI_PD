using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Ini;
using Newtonsoft.Json;

namespace FbAwGEM64
{
    public class Client
    {
        public event EventHandler ReceivedCmd;
        static private Socket sckCommunication;
        static private EndPoint epLocal, epRemote;
        static private string mToolIpAddrLocal = "";
        static private long mToolPortLocal = 0;
        static private string mToolIpAddrRemote = "";
        static private long mToolPortRemote = 0;
        static private string mStrLocID;
        static private int m_nConIndex = 0;
        static private List<string> mListDataIn = new List<string>();
        private static System.Timers.Timer toolConnTimer;

        byte[] buffer;

        //-[ctor]
        public Client()
        {
        }

        public object Initialize()
        {
            string srtn = "0";
            string m_filePath = "C:\\CDPBinary\\settings.ini"; ;
            if (File.Exists(m_filePath))
            {
                IniFile inifile = new IniFile(m_filePath);
                mToolIpAddrLocal = inifile.IniReadValue("TOOL_CONNECTION", "IpAddrLocal");
                mToolPortLocal = Convert.ToInt32(inifile.IniReadValue("TOOL_CONNECTION", "PortLocal"));
                mToolIpAddrRemote = inifile.IniReadValue("TOOL_CONNECTION", "IpAddrRemote");
                mToolPortRemote = Convert.ToInt32(inifile.IniReadValue("TOOL_CONNECTION", "PortRemote"));
            }
            else
            {
                srtn = "-1";
            }

            // set up socket
            sckCommunication = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sckCommunication.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            toolConnTimer = new System.Timers.Timer(10000);
            toolConnTimer.Interval = 10000;
            toolConnTimer.Enabled = false;
            toolConnTimer.Elapsed += toolConnTimer_Elapsed;
            object obRtn = srtn;
            return obRtn;
        }

        void toolConnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (sckCommunication.Connected)
            {
                return;
            }
            //-[]
            ReInitConn(mStrLocID);
        }

        public object InitConn(string strLocID)
        {
            mStrLocID = strLocID;
            string srtn = "0";
            toolConnTimer.Enabled = true;
            //-[ bind socket ]-
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(mToolIpAddrLocal), (int)mToolPortLocal);
                sckCommunication.Bind(epLocal);
            }
            catch
            {
                return null;
            }
            try
            {
            //-[ connect to remote ip and port ]- 
            epRemote = new IPEndPoint(IPAddress.Parse(mToolIpAddrRemote), (int)mToolPortRemote);
            sckCommunication.Connect(epRemote);
            }
            catch
            {
                return null;
            }

            //-[ starts to listen to an specific port ]-
            buffer = new byte[1464];
            try
            {
            sckCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                    ref epRemote, new AsyncCallback(OperatorCallBack), buffer);
            }
            catch
            {
                return null;
            }

            object obRtn = srtn;
            return obRtn;
        }
        private void OperatorCallBack(IAsyncResult ar)
        {
            try
            {
                int size = sckCommunication.EndReceiveFrom(ar, ref epRemote);

                // check if theres actually information
                if (size > 0)
                {
                    // used to help us on getting the data
                    byte[] aux = new byte[1464];

                    // gets the data
                    aux = (byte[])ar.AsyncState;

                    // converts from data[] to string
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    string msg = enc.GetString(aux);

                    mListDataIn.Add(msg);
                    if (mListDataIn.Count > 10)
                    {
                        mListDataIn.RemoveAt(0);
                    }
                    OnReceivedCmd();
                }

                // starts to listen again
                buffer = new byte[1464];
                sckCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                        ref epRemote, new AsyncCallback(OperatorCallBack), buffer);
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.ToString());
            }
        }
        private void OnReceivedCmd()
        {
            if (ReceivedCmd != null)
            {
                ReceivedCmd(this, EventArgs.Empty);
            }
        }


        public object ReInitConn(
        string strLocID)
        {
            string srtn = "0";
        //    CloseConn();
            try
            {
                sckCommunication.Dispose();
            }
            catch
            {
            }
            // set up socket
            sckCommunication = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sckCommunication.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            InitConn(mStrLocID);
            object obRtn = srtn;
            return obRtn;
        }

        public object CloseConn()
        {
            string srtn = "0";
            try
            {
                sckCommunication.Shutdown(SocketShutdown.Both);
            }
            catch {
                return null;
            }

            try
            {
                sckCommunication.Disconnect(true);
            }
            catch {
                return null;
            }
            try
            {
                sckCommunication.Close();
            }
            catch {
                return null;
            }
            try
            {
                sckCommunication.Dispose();
            }
            catch {
                return null;
            }
            object obRtn = srtn;
            return obRtn;
        }

        public object GetConnStateNum ( )
        {
            int irtn = 0;
            object obRtn = irtn;
            if (sckCommunication.IsBound)
            {
                obRtn = (int)2;
            }
            if (sckCommunication.Connected)
            {
                obRtn = (int)4;
            }
             return obRtn;
        }

        public object GetConnStateStr()
        {
            string srtn = "0";
            object obRtn = srtn;
            if (sckCommunication.IsBound)
            {
                srtn = "2";
                obRtn = srtn;
            }
            if (sckCommunication.Connected)
            {
                obRtn = "4";
            }
            return obRtn;
        }

        public object GetConnState()
        {
            int irtn = 0;
            object obRtn = irtn;
            if (sckCommunication.IsBound)
            {
                obRtn = (int)2;
            }
            if (sckCommunication.Connected)
            {
                obRtn = (int)4;
            }
            return obRtn;
        }

        public object GetRCSMessage()
        {
            string srtn = "0";
            m_nConIndex = m_nConIndex + 1;

            SendStr("GetRCSMessage" + "~*~" + m_nConIndex);
            object obRtn = srtn;
            return obRtn;
        }

        //-[GEM methods]-
        //
        public object InitGemVid(
        object vrVID,
        string strClass,
        string strFormat,
        string vrName,
        object vrLength,
        string strUnits,
        object vrDefault,
        object vrMin,
        object vrMax)
        {
            string srtn = "0";
            string jvrDefault = JsonConvert.SerializeObject(vrDefault, Formatting.Indented);
            string jvrMin = JsonConvert.SerializeObject(vrMin, Formatting.Indented);
            string jvrMax = JsonConvert.SerializeObject(vrMax, Formatting.Indented);
            SendStr("InitGemVid" + "~*~" + vrVID + "~*~" + strClass + "~*~" + strFormat + "~*~" + vrName + "~*~" + vrLength + "~*~" + strUnits + "~*~" + jvrDefault + "~*~" + jvrMin + "~*~" + jvrMax);
            object obRtn = srtn;
            return obRtn;
        }

        public object GetGemHostState()
        {
            string srtn = "0";
            SendStr("GetGemHostState");
            object obRtn = srtn;
            return obRtn;
        }

        public string GetNextCommand()
        {
            string srtn = "";
            if (mListDataIn.Count > 0)
            {
                srtn = mListDataIn[0];
                mListDataIn.RemoveAt(0);
            }
            //object obRtn = srtn;
            return srtn;
        }

        public object SetGemVid (
        object vrID,
        object vrVal,
        object vrInstId,
        string strTime )
        {
            string srtn = "0";
            string json = JsonConvert.SerializeObject(vrVal, Formatting.Indented);
            SendStr("SetGemVid" + "~*~" + vrID + "~*~" + json + "~*~" + vrInstId + "~*~" + strTime);
            object obRtn = srtn;
            return obRtn;
        }

        public object InitGemAlId(
        object vrALID,
        object vrALCD,
        string vrALTX,
        object vrALED)
        {
            string srtn = "0";
            SendStr("InitGemAlId" + "~*~" + vrALID + "~*~" + vrALCD + "~*~" + vrALTX + "~*~" + vrALED);
            object obRtn = srtn;
            return obRtn;
        }

        public object SetGemAlid(
        object vrALID,
        object vrALCD,
        string strTime)
        {
            string srtn = "0";
            SendStr("SetGemAlid" + "~*~" + vrALID + "~*~" + vrALCD + "~*~" + strTime);
            object obRtn = srtn;
            return obRtn;
        }

        public object SendGemRecipe(
        string strLocation,
        string strType,
        string strBody,
        string strTime)
        {
            string srtn = "0";
            SendStr("SendGemRecipe" + "~*~" + strLocation + "~*~" + strType + "~*~" + strBody + "~*~" + strTime);
            object obRtn = srtn;
            return obRtn;
        }

        public object SendGemData(
        string strData,
        string strTime)
        {
            string srtn = "0";
            SendStr("SendGemData" + "~*~" + strData + "~*~" + strTime);
            object obRtn = srtn;
            return obRtn;
        }

        public object SetGemEvent(
        object vrCeid,
        object vrInstId,
        string strTime)
        {
            string srtn = "0";
            SendStr("SetGemEvent" + "~*~" + vrCeid + "~*~" + vrInstId + "~*~" + strTime);
            object obRtn = srtn;
            return obRtn;
        }

        //-[system methods]
        public object SetCDPServerTime(string tm)
        {
            string srtn = "0";
            object obRtn = srtn;
            return obRtn;
        }

        public object SetCDPServerDate(string dt)
        {
            string srtn = "0";
            object obRtn = srtn;
            return obRtn;
        }

        public object SetCDPServerTimeZone(string tz)
        {
            string srtn = "0";
            object obRtn = srtn;
            return obRtn;
        }

        public object SetCDPLogEntry(
        string strLogName,
        string strLocID,
        string strClass,
        string strEntry,
        string strTime)
        {
            string srtn = "0";
            object obRtn = srtn;
            return obRtn;
        }

        public object CDPGeneral(
        string strLocID,
        string strExt1,
        string strExt2,
        string strExt3,
        string strExt4,
        string strExt5,
        string strDesc,
        string strTime)
        {
            string srtn = "0";
            SendStr("CDPGeneral" + "~*~" + strLocID + "~*~" + strExt1 + "~*~" + strExt2 + "~*~" + strExt3 + "~*~" + strExt4 + "~*~" + strExt5 + "~*~" + strDesc + "~*~" + strTime);
            object obRtn = srtn;
            return obRtn;
        }

        public object SetLogLevel(
        object nLogLevel)
        {
            string srtn = "0";
            object obRtn = srtn;
            return obRtn;
        }

        private bool SendStr(string str)
        {
            bool brtn = true;
            if (sckCommunication == null)
            {
                return false; 
            }
            // converts from string to byte[]
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] msg = new byte[1464];
            msg = enc.GetBytes(str);
            // sending the message
            try
            {
                sckCommunication.Send(msg);
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
