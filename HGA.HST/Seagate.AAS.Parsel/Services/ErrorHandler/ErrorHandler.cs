//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/16/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// InputEventArgs & ErrorInputHandler: Mechanisms for UI button click info back to source of error
	/// </summary>
	public class InputEventArgs : EventArgs
	{
		public readonly ErrorButton button;
		public InputEventArgs(ErrorButton btn)
		{
			this.button = btn;
		}
	}

    //--------------------------------------------------------------------------

	public delegate void ErrorInputHandler(IErrorMessage message);

    //--------------------------------------------------------------------------

	/// <summary>
	/// Summary description for ErrorHandler.
	/// </summary>
	public class ErrorHandler : IService
	{
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        private FormErrorList myUI;
        private Hashtable errorTable = new Hashtable();
        private ArrayList errorList = new ArrayList();
        
		public Hashtable traceTable = new Hashtable();

        private bool logFileEnabled = false;
        private string logFileName = "";
        private string errorTableFilePrefix = "";
        private string errorTableFileName = "";
        private string errorTableFileNameFormat = "ErrorTable{0}.dat";
        private string errorTableFilePath = @"c:\Seagate\EquipmentData\";
        private DateTime errorTableStartDateTime = new DateTime();

        // Constructors & Finalizers -------------------------------------------
        
        internal ErrorHandler()
        {
            System.Diagnostics.Trace.WriteLine("Seagate.AAS.Parsel()");
            ServiceManager.Tracing.Trace("Loading Event Manager");
            //System.Threading.Thread.Sleep(1000);        

            myUI = new FormErrorList();
			IntPtr ip = myUI.Handle;			// this forces the HWND to be created for marshalling purposes
            myUI.Hide();

            errorTable.Clear();
        }

        // Properties ----------------------------------------------------------

        public FormErrorList ErrorForm
        {
            set { myUI = value; }
        }

        public Hashtable ErrorSummary
        {
            get { return errorTable; }
        }        

        public ArrayList ErrorList
        {
            get { return errorList; }
        }        

		private bool updateRequired = false;

        private IErrorMessage lastHandledMessage;

		public bool UpdateRequired
		{
			get { return updateRequired; }
			set { updateRequired = value; }	
		}

        /// <summary>
        /// Prefix of standard error table data file "ErrorTableyyyMMdd.dat"
        /// </summary>
        public string ErrorTableFilePrefix
        {
            get { return errorTableFilePrefix; }
            set { errorTableFilePrefix = value; }
        }

        public string ErrorTableFilePath
        {
            get { return errorTableFilePath; }
            set { errorTableFilePath = value; }	
        }

        public DateTime ErrorTableStartDateTime
        {
            get { return errorTableStartDateTime; }
        }

        public IErrorMessage LastHandledMessage
        {
            get { return lastHandledMessage; }
            set { lastHandledMessage = value; }
        }

        // Methods -------------------------------------------------------------
        
        public void InitializeService()
        {
            errorTable.Clear();
            errorTableStartDateTime = DateTime.Now;

            if (!Directory.Exists(errorTableFilePath))
            {
                Directory.CreateDirectory(errorTableFilePath);
            }
            else // imply directory existed
            {
                // Check time and dat file existing,
                DateTime tmpDate = new DateTime();
                tmpDate = DateTime.Now.AddHours(-6);

                errorTableFileName = string.Format(errorTableFileNameFormat, tmpDate.ToString("yyyyMMddtt"));
                if (File.Exists(errorTableFilePath + errorTableFilePrefix + errorTableFileName))
                {
                    // If file existed, load hash from file.
                    LoadErrorTable(errorTableFilePath + errorTableFilePrefix + errorTableFileName);
                }
            }
        }
		
        public void UnloadService()
        {
        }

		public void RegisterMessage(IErrorMessage message)
		{
			Debug.Assert(message != null,"ErrorHandler.RegisterMessage() received a message that does not implement IErrorMessage interface");
			Debug.Assert(message.Text != null,"ErrorHandler.RegsisterMessage() received a message that has no text assigned!");

			// need to verify that message is not already in queue
			foreach(IErrorMessage msg in errorList)
			{
				if(msg.UniqueID == message.UniqueID)
				{
					string s = String.Format("The error \"{0}\" is already enqueued!",message.Text);
					ServiceManager.Tracing.Trace(s);
					return;
				}
			}
            lock (this)
            {
                LogMessage("Register error: " + message.Text + " From: " + message.Source);
                message.TimeIn = DateTime.Now;
                if (logFileEnabled)
                {
                    string s = "+" + DateTime.Now.ToString("h:mm:ss.ff") + " Error=" + message.Text + " Source=" + message.Source;
                    WriteToLogFile(s);
                }
                int i;
                for (i = 0; i < errorList.Count; i++)
                {
                    if (message.Priority > ((ErrorMessage)errorList[i]).Priority)
                        break;
                }

                if ((lastHandledMessage != null) && (lastHandledMessage.Source == message.Source))
                    i = 0;

                errorList.Insert(i, message);

                // Check time and dat file existing,
                // If file not existed, imply that start new shift
                //      Clear errortable and start new file, note
                // If file existed, no action needed

                // Check time and dat file existing,
                DateTime tmpDate = new DateTime();
                tmpDate = DateTime.Now.AddHours(-6);

                string newErrorTableFileName = string.Format(errorTableFileNameFormat, tmpDate.ToString("yyyyMMddtt"));
                if (!File.Exists(errorTableFilePath + errorTableFilePrefix + newErrorTableFileName))
                {
                    errorTable.Clear();
                    errorTableStartDateTime = tmpDate.AddHours(6);
                    errorTableFileName = newErrorTableFileName;
                }
                

                // find the error stats in the hashtable
                if (errorTable.ContainsKey(message.UniqueID))
                {
                    MessageRecord rec = errorTable[message.UniqueID] as MessageRecord;
                    if (rec != null)
                    {
                        rec.Start();
                    }
                }
                else							// new error, need to 
                {
                    MessageRecord rec = new MessageRecord(message);
                    errorTable.Add(message.UniqueID, rec);
                    rec.Start();
                }
                updateRequired = true;
                myUI.EnableTimer();

                SaveErrorTable();
                SaveErrorTableTxt();
            }
		}

		public void UnRegisterMessage(IErrorMessage message)
		{
            lock (this)
            {
                if (errorList.Contains(message))
                {
                    LogMessage("Un-Register error: " + message.Text + " From: " + message.Source);
                    if (logFileEnabled)
                    {
                        string s = "-" + DateTime.Now.ToString("h:mm:ss.ff") + " Error=" + message.Text + " Source=" + message.Source;
                        WriteToLogFile(s);
                    }
                    // find the error stats in the hashtable
                    if (errorTable.ContainsKey(message.UniqueID))
                    {
                        MessageRecord rec = errorTable[message.UniqueID] as MessageRecord;
                        if (rec != null)
                        {
                            rec.Stop();
                        }
                    }
                    errorList.Remove(message);

                    if (errorList.Count == 0)
                    {
                        lastHandledMessage = null;
                    }

                    updateRequired = true;
                    //					myUI.RemoveMessage(message);		// UI timer will remove the message itseld

                    SaveErrorTable();
                    SaveErrorTableTxt();
                }
            }
        }
        public void AutoClearMessages()
        {
            while (errorList.Count > 0)
            {
                ErrorMessage msg = (ErrorMessage)errorList[0];
                if (msg != null && msg.DefaultButton != ErrorButton.NoButton)
                {
                    msg.ButtonSelected = msg.DefaultButton;
                    msg.OnInput();
                }
            }
        }

		public void ShowPareto()
		{
			FormPareto paretoView = new FormPareto();
			paretoView.Show();
		}

        public void EnableLogFile(string fileName)
        {
            logFileEnabled = true;
            logFileName = fileName;
        }

        public void WriteToLogFile(string s)
        {
            using (StreamWriter sw = File.AppendText(logFileName))
            {
                sw.WriteLine(s);
            }
        }

        public string GetExceptionMessages(System.Exception e)
        {
            string msg = "";
            do
            {
                msg += e.Message + Environment.NewLine;
                e = e.InnerException;
            } while (e != null);

            return msg;
        }

        // Internal methods ----------------------------------------------------
        private void LogMessage(string message)
        {
            ServiceManager.Tracing.Trace(this.ToString(), message);
        }

        private void SaveErrorTable()
        {
            if (!Directory.Exists(errorTableFilePath))
                Directory.CreateDirectory(errorTableFilePath);

            string filePath = errorTableFilePath + errorTableFilePrefix + errorTableFileName;
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                Hashtable a = new Hashtable();
                a = errorTable;
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, errorTableStartDateTime);
                bf.Serialize(fs, a);
            }
            catch
            {

            }
            finally
            {
                fs.Close();
            }
        }

        private void SaveErrorTableTxt()
        {
            if (!Directory.Exists(errorTableFilePath))
                Directory.CreateDirectory(errorTableFilePath);

            string tmpErrorTableFileName = errorTableFileName.Replace(".dat", ".csv");
            string filePath = errorTableFilePath + errorTableFilePrefix + tmpErrorTableFileName;
            string header = "Workcell, Source" + "," +
                "Message" + "," +
                "Count" + "," +
                "TotalTime" + "," +
                "AverageTime" + "," +
                "MinTime" + "," +
                "MaxTime";
            string item;
            try
            {
                Hashtable tmpErrorTable = (Hashtable)errorTable.Clone();
                IDictionaryEnumerator myEnumerator = tmpErrorTable.GetEnumerator();
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(header);
                    while (myEnumerator.MoveNext())
                    {

                        string WC = ((MessageRecord)myEnumerator.Value).source;
                        if (WC.Contains("_"))
                            WC = WC.Substring(0, WC.IndexOf("_"));

                        string message = ((MessageRecord)myEnumerator.Value).text;
                        message = message.Replace("\n", " ");
                        message = message.Replace("\r", " ");

                        item = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", WC,
                            (((MessageRecord)myEnumerator.Value).source),
                            message,
                            ((MessageRecord)myEnumerator.Value).Count(),
                            ((MessageRecord)myEnumerator.Value).TotalTime(),
                            ((MessageRecord)myEnumerator.Value).AverageTime(),
                            ((MessageRecord)myEnumerator.Value).MinTime(),
                            ((MessageRecord)myEnumerator.Value).MaxTime());
                        sw.WriteLine(item);
                    }
                }
            }
            catch
            {

            }
        }

        private void LoadErrorTable(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            FileStream fs = new FileStream(filePath, FileMode.Open);
            try
            {
                Hashtable a = null;
                BinaryFormatter bf = new BinaryFormatter();
                errorTableStartDateTime = (DateTime)bf.Deserialize(fs);
                a = (Hashtable)bf.Deserialize(fs);
                errorTable = a;
            }
            catch
            {

            }
            finally
            {
                fs.Close();
            }
        }
    }
}
