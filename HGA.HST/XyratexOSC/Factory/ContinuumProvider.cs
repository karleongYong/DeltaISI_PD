using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbAwGEM64;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// The interface to the Continuum Sec/Gem communication library
    /// </summary>
    public class ContinuumProvider : ICommProvider
    {
        /// <summary>
        /// The Continuum GEM client
        /// </summary>
        static public Client gemClient = new Client();

        /// <summary>
        /// The instance identifier
        /// </summary>
        static public int nInstanceId = 0;

        /// <summary>
        /// The heartbeat index.
        /// </summary>
        static public int nHeartbeatIndex = 0;

        /// <summary>
        /// The previous heartbeat index.
        /// </summary>
        static public int nHeartbeatIndexLast = 0;

        public event EventHandler<ProcessRunEventArgs> RemoteCommandRun;

        public event EventHandler<ProcessPauseEventWithArgs> FireRemoteCommandPause;

        public event EventHandler<ProcessCancelEventWithArgs> FireRemoteCommandCancel;

        public event EventHandler<PPUploadEventWithArgs> FirePPUpload;

        public event EventHandler<PPRequestEventWithArgs> FirePPRequest;

        public class ProcessPauseEventWithArgs : EventArgs
        {
            public string sLotID { get; internal set; }
        }

        public class ProcessCancelEventWithArgs : EventArgs
        {
            public string sLotID { get; internal set; }
        }

        public class PPUploadEventWithArgs : EventArgs
        {
            public string sPpid { get; internal set; }
            public object yPpBody { get; internal set; }
        }

        public class PPRequestEventWithArgs : EventArgs
        {
            public string sPpid { get; internal set; }
        }

        /// <summary>
        /// Initializes the communication provider.
        /// </summary>
        public void Initialize()
        {
            gemClient.Initialize();

            gemClient.ReceivedCmd -= gemClient_ReceivedCmd;
            gemClient.ReceivedCmd += gemClient_ReceivedCmd;
        }

        /// <summary>
        /// Un-initializes the communication provider.
        /// </summary>
        public void Uninitialize()
        {
            gemClient.ReceivedCmd -= gemClient_ReceivedCmd;
        }

        private void OnRemoteCommandRun(ProcessRunEventArgs e)
        {
            EventHandler<ProcessRunEventArgs> handler = RemoteCommandRun;
            if (handler != null)
            {
                handler(this, e);
            }
            //TODO - if no listeners are attached we need to respond with an error code
        }

        private void OnRemoteCommandPause(ProcessPauseEventWithArgs e)
        {
            EventHandler<ProcessPauseEventWithArgs> handler = FireRemoteCommandPause;
            if (handler != null)
            {
                handler(this, e);
            }
            //TODO - if no listeners are attached we need to respond with an error code
        }

        private void OnRemoteCommandCancel(ProcessCancelEventWithArgs e)
        {
            EventHandler<ProcessCancelEventWithArgs> handler = FireRemoteCommandCancel;
            if (handler != null)
            {
                handler(this, e);
            }
            //TODO - if no listeners are attached we need to respond with an error code
        }

        private void OnPPUpload(PPUploadEventWithArgs e)
        {
            EventHandler<PPUploadEventWithArgs> handler = FirePPUpload;
            if (handler != null)
            {
                handler(this, e);
            }
            //TODO - if no listeners are attached we need to respond with an error code
        }

        private void OnPPRequest(PPRequestEventWithArgs e)
        {
            EventHandler<PPRequestEventWithArgs> handler = FirePPRequest;
            if (handler != null)
            {
                handler(this, e);
            }
            //TODO - if no listeners are attached we need to respond with an error code
        }

        //-[General message from GEM Box]-
        void gemClient_ReceivedCmd(object sender, EventArgs e)
        {
            string strRtn = (string)gemClient.GetNextCommand();

            //-[string return will be comma deliminated]-
            string[] strArray = strRtn.Split(',');

            string sentCmd = strArray[0];
            EventHandler<MessageEventArgs> msgSent = MessageSent;
            if (msgSent != null)
                msgSent(this, new MessageEventArgs(sentCmd));

            string response = strArray.Length > 1 ? strArray[1] : "(empty)";
            EventHandler<MessageEventArgs> msgReceived = MessageReceived;
            if (msgReceived != null)
                msgReceived(this, new MessageEventArgs(response));

            switch(strArray[0])
            {
                case "GetRCSMessage":
                    nHeartbeatIndexLast = nHeartbeatIndex;
                    nHeartbeatIndex = Convert.ToInt32(strArray[1]);
                    break;
                case "RCMD":
                    if(strArray.Length == 1)
                    {
                        break;
                    }
                    switch (strArray[1])
                    {
                        case "RUN":
                            ProcessRunEventArgs ex = new ProcessRunEventArgs();
                            //-[get CPNAME CPVAL match]-
                            for (int i = 0; i < strArray.Length; )
                            {
                                switch (strArray[i])
                                {
                                    case "LotID":
                                        ex.sLotID = strArray[++i];
                                        break;
                                    case "LotRecipe":
                                        ex.sLotRecipe = strArray[++i];
                                        break;
                                    case "CarrierType":
                                        ex.sCarrierType = strArray[++i];
                                        break;
                                    case "SliderType":
                                        ex.sSliderType = strArray[++i];
                                        break;
                                    case "OutputDirectory":
                                        ex.sOutputDirectory = strArray[++i];
                                        break;
                                    case "FailIf":
                                        ex.sFailIf = strArray[++i];
                                        break;
                                    case "SelectedSliders":
                                        ex.sSelectedSliders = strArray[++i];
                                        break;
                                    case "Rows":
                                        ex.sRows = strArray[++i];
                                        break;
                                    case "Columns":
                                        ex.sColumns = strArray[++i];
                                        break;
                                    case "FlowSequenceFile":
                                        ex.sFlowSequenceFile = strArray[++i];
                                        break;
                                    default:
                                        i++;
                                        break;
                                }
                            }
                            OnRemoteCommandRun(ex);
                            break;
                        case "PAUSE":
                            ProcessPauseEventWithArgs ep = new ProcessPauseEventWithArgs();
                            if (strArray.Length < 4)
                                break;
                            ep.sLotID = strArray[3];
                            OnRemoteCommandPause(ep);
                            break;
                        case "CANCEL":
                            ProcessCancelEventWithArgs ec = new ProcessCancelEventWithArgs();
                            if (strArray.Length < 4)
                                break;
                            ec.sLotID = strArray[3];
                            OnRemoteCommandCancel(ec);
                            break;
                        default:
                            break;
                    }
                    break;
                case "PPUL":
                    PPUploadEventWithArgs eul = new PPUploadEventWithArgs();
                    if (strArray.Length < 6)
                        break;
                    eul.sPpid = strArray[3];
                    eul.yPpBody = strArray[5];
                    OnPPUpload(eul);
                    break;
                case "PPDL":
                    PPRequestEventWithArgs erq = new PPRequestEventWithArgs();
                    if (strArray.Length < 3)
                        break;
                    erq.sPpid = strArray[3];
                    OnPPRequest(erq);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Opens connection to Continuum system.
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            gemClient.InitConn("PLI");
        }

        /// <summary>
        /// Checks connection to Continuum system.
        /// </summary>
        /// <returns></returns>
        public void CheckConnection()
        {
            gemClient.GetRCSMessage();
        }

        /// <summary>
        /// Closes connection to Continuum system.
        /// </summary>
        public void Disconnect()
        {
            gemClient.CloseConn();
        }

        /// <summary>
        /// Attempts to connect to the factory host.
        /// </summary>
        /// <returns></returns>
        public void AttemptOnline()
        {
            //TODO - adding protocol to switch between online and offline
        }

        /// <summary>
        /// Attempts to disconnect from the factory host.
        /// </summary>
        /// <returns></returns>
        public void Offline()
        {
            //TODO - adding protocol to switch between online and offline
        }

        /// <summary>
        /// Gets the Host communication state as determined from the last GEM box heartbeat.
        /// </summary>
        /// <returns>The resulting host communication state.</returns>
        public HostState GetHostState()
        {
            string state = (string)gemClient.GetGemHostState();

            if (state == "ONLINE")
                return HostState.Online;

            return HostState.Offline;
        }

        /// <summary>
        /// Gets the tool communication state.
        /// </summary>
        /// <returns>The tool communication state.</returns>
        public ToolState GetToolState()
        {
            if (nHeartbeatIndex == nHeartbeatIndexLast)
                return ToolState.Disabled;

            //TODO - adding protocol to switch between online and offline
            return ToolState.OnlineLocal;
        }

        public void SendResultFile(string filepath)
        {
            //TODO
        }

        /// <summary>
        /// Occurs when a message is sent to the facility server.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageSent;

        /// <summary>
        /// Occurs when a message is received from the facility server.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when host connection state has changed.
        /// </summary>
        public event EventHandler HostStateChanged;

        /// <summary>
        /// Occurs when tool connection state has changed.
        /// </summary>
        public event EventHandler ToolStateChanged;

        /// <summary>
        /// Occurs when the recipe (process program) is downloaded and ready on the Equipment computer.
        /// </summary>
        public event EventHandler<RecipeEventArgs> RecipeReady;

        /// <summary>
        /// Occurs when the Host has changed any variable (status, equipment constant, etc)
        /// </summary>
        public event EventHandler<GemVariableEventArgs> VariableChanged;

        /// <summary>
        /// Raises the collection event specified by the CEID.
        /// </summary>
        /// <param name="ceid">The CEID.</param>
        /// <param name="instanceId">The instance identifier associated with the event.</param>
        public void RaiseEvent(int ceid, int instanceId)
        {
            //TODO
            gemClient.SetGemEvent(ceid, instanceId, null);
        }

        /// <summary>
        /// Adds an alarm GEM item.
        /// </summary>
        /// <param name="alarm">The alarm.</param>
        public void AddAlarm(GemAlarm alarm)
        {
            gemClient.InitGemAlId(alarm.ID, alarm.Code, alarm.Name, alarm.Set);
        }

        /// <summary>
        /// Sets the alarm state and raises the associated alarm collection event.
        /// </summary>
        /// <param name="aid">The alarm ID.</param>
        /// <param name="ceid">The collection event ID.</param>
        public void RaiseAlarm(int aid, int ceid)
        {
            gemClient.SetGemAlid(aid, true, null);
        }

        /// <summary>
        /// Sets the alarm state and raises the associated alarm collection event.
        /// </summary>
        /// <param name="aid">The alarm ID.</param>
        /// <param name="ceid">The collection event ID.</param>
        public void ClearAlarm(int aid, int ceid)
        {
            gemClient.SetGemAlid(aid, false, null);
        }

        /// <summary>
        /// Adds a variable GEM item.
        /// </summary>
        /// <param name="var">The variable.</param>
        public void AddVariable(GemVariable var)
        {
            //TODO
            gemClient.InitGemVid(var.ID, var.GetClass(), var.Format, var.Name, var.Length, var.Units, null, null, null);
        }

        /// <summary>
        /// Sets the value of the variable with the specified variable ID.
        /// </summary>
        /// <param name="vid">The variable ID.</param>
        /// <param name="instanceId">The instance identifier used to associate with an event.</param>
        /// <param name="value">The value.</param>
        public void SetVariable(int vid, int instanceId, object value)
        {
            gemClient.SetGemVid(vid, value, instanceId, null);
        }
    }
}
