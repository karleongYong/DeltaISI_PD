using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// The generic interface to the facility server.
    /// </summary>
    public class EquipmentPort
    {
        private int _instanceId = 1;
        private ICommProvider _commProvider;
        private Dictionary<int, IGemItem> _dataStore = new Dictionary<int, IGemItem>();
        private Timer _heartbeatTimer = new Timer();

        #region Properties

        /// <summary>
        /// Gets the communication state of the host.
        /// </summary>
        /// <value>
        /// The communication state of the host.
        /// </value>
        public HostState HostState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the communication/control state of the tool.
        /// </summary>
        /// <value>
        /// The control state of the tool.
        /// </value>
        public ToolState ToolState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the communications heart beat interval in seconds.
        /// </summary>
        /// <value>
        /// The heart beat interval in seconds.
        /// </value>
        public int HeartbeatInterval
        {
            get
            {
                return (int)_heartbeatTimer.Interval;
            }
            set
            {
                _heartbeatTimer.Interval = value;
            }
        }

        /// <summary>
        /// Gets the timestamp of the last heartbeat.
        /// </summary>
        /// <value>
        /// The last heartbeat.
        /// </value>
        public DateTime LastHeartbeat
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the interface of the communication provider.
        /// </summary>
        /// <value>
        /// The facility communication provider.
        /// </value>
        public ICommProvider CommProvider
        {
            get
            {
                return _commProvider;
            }
        }

        /// <summary>
        /// Gets or sets the state change to offline event.
        /// </summary>
        /// <value>
        /// The offline state-change event.
        /// </value>
        public GemEvent EventOffline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state change to online local event. This means that the tool is communicating with the facility server but is controlled locally.
        /// </summary>
        /// <value>
        /// The online-local state change event.
        /// </value>
        public GemEvent EventOnlineLocal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state change to online remote event. This means that the tool is communicating with the facility server and is controlled by the server.
        /// </summary>
        /// <value>
        /// The event online remote.
        /// </value>
        public GemEvent EventOnlineRemote
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the event indicating that new material has been received.
        /// </summary>
        /// <value>
        /// The material received event.
        /// </value>
        public GemEvent EventMaterialReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lot identifier.
        /// </summary>
        /// <value>
        /// The lot identifier.
        /// </value>
        public DataVariable<SecsASCII> LotId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the recipe path.
        /// </summary>
        /// <value>
        /// The recipe path.
        /// </value>
        public DataVariable<SecsASCII> RecipePath
        {
            get;
            set;
        }

        #endregion

        #region Events
        
        public event EventHandler ToolStateChanged;

        public event EventHandler HostStateChanged;

        public event EventHandler<GemVariableEventArgs> VariableChanged;

        public event EventHandler<RecipeEventArgs> RunStartRequested;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentPort"/> class.
        /// </summary>
        /// <param name="commProvider">The communication provider.</param>
        public EquipmentPort(ICommProvider commProvider)
        {
            _commProvider = commProvider;
            _commProvider.RemoteCommandRun += OnRemoteCommandRun;
            _commProvider.VariableChanged += OnVariableChanged;
            _commProvider.Initialize();

            _heartbeatTimer.Interval = 5000;
            _heartbeatTimer.Elapsed += HeartbeatElapsed;

            LastHeartbeat = DateTime.Now;
        }

        private void OnRemoteCommandRun(object sender, ProcessRunEventArgs e)
        {
            EventHandler<RecipeEventArgs> remoteRun = RunStartRequested;
            if (remoteRun != null)
                remoteRun(this, new RecipeEventArgs(e.sLotRecipe));
        }

        private void OnVariableChanged(object sender, GemVariableEventArgs e)
        {
            GemVariable variable = _dataStore[e.VID] as GemVariable;
            if (variable == null)
                return;

            variable.PropertyChanged -= UpdateVariable;

            try
            {
                e.Variable = variable;
                variable.Data = e.Value;

                EventHandler<GemVariableEventArgs> varChanged = VariableChanged;
                if (varChanged != null)
                    varChanged(this, e);
            }
            finally
            {
                variable.PropertyChanged += UpdateVariable;
            }
        }
       
        /// <summary>
        /// Removes the GEM variable specified by ID from the public data store.
        /// </summary>
        /// <param name="id">The data item ID.</param>
        public void RemoveGemItem(int id)
        {
            if (!_dataStore.ContainsKey(id))
                return;

            IGemItem data = _dataStore[id];
            _dataStore.Remove(id);

            data.Dispose();
        }

        private void UpdateVariable(object sender, PropertyChangedEventArgs e)
        {
            GemVariable gemVariable = (GemVariable)sender;
            if (e.PropertyName != "Data")
                return;

            _commProvider.SetVariable(gemVariable.ID, 0, gemVariable.Data);
        }

        private void HeartbeatElapsed(object sender, ElapsedEventArgs e)
        {
            _heartbeatTimer.Elapsed -= HeartbeatElapsed;

            try
            {
                ToolState toolState = ToolState;
                HostState hostState = HostState;

                if (_commProvider == null)
                {
                    ToolState = ToolState.Disabled;
                    HostState = HostState.Offline;
                }
                else
                {
                    _commProvider.CheckConnection();
                    HostState = _commProvider.GetHostState();
                    ToolState = _commProvider.GetToolState();
                }

                if (toolState != ToolState)
                {
                    EventHandler toolStateChanged = ToolStateChanged;
                    if (toolStateChanged != null)
                        toolStateChanged(this, EventArgs.Empty);
                }

                if (hostState != HostState)
                {
                    EventHandler hostStateChanged = HostStateChanged;
                    if (hostStateChanged != null)
                        hostStateChanged(this, EventArgs.Empty);
                }
            }
            finally
            {
                _heartbeatTimer.Elapsed += HeartbeatElapsed;
            }
        }

        /// <summary>
        /// Enable communication to the facility server.
        /// </summary>
        public void Enable()
        {
            _heartbeatTimer.Enabled = true;

            ToolState = ToolState.Offline;

            try
            {
                // Enable the communication provider
                _commProvider.Connect();
                _commProvider.CheckConnection();
            }
            catch (Exception)
            {
                ToolState = ToolState.Disabled;
                throw;
            }
        }

        /// <summary>
        /// Disables communication to the facility server.
        /// </summary>
        public void Disable()
        {          
            _heartbeatTimer.Enabled = false;

            // Disable the communication provider
            _commProvider.Disconnect();
            ToolState = ToolState.Disabled;
        }

        /// <summary>
        /// Checks the connection to the facility server.
        /// </summary>
        /// <exception cref="XyratexOSC.Factory.CommuncationException">
        /// Connection error.
        /// </exception>
        public void CheckConnection()
        {
            _commProvider.CheckConnection();
        }

        /// <summary>
        /// Attempts to go into the online tool state.
        /// </summary>
        /// <param name="remote">if set to <c>true</c> [remote].</param>
        /// <exception cref="XyratexOSC.Factory.CommuncationException">
        /// Communication provider is offline.
        /// or
        /// Factory Host is offline.
        /// </exception>
        public void AttemptOnline(bool remote)
        {
            if (_commProvider.GetToolState() == ToolState.Disabled)
                throw new CommuncationException("Communication provider is offline.");

            if (_commProvider.GetHostState() != HostState.Online)
                throw new CommuncationException("Factory Host is offline.");

            if (remote)
                RaiseEvent(EventOnlineRemote);
            else
                RaiseEvent(EventOnlineLocal);
        }

        /// <summary>
        /// Attempts to go into the offline tool state.
        /// </summary>
        public void Offline()
        {
            RaiseEvent(EventOffline);
        }

        /// <summary>
        /// Requests a recipe from te facility server.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        public void RequestRecipe(string lotId)
        {
            LotId.Data = new SecsASCII(lotId);
            RaiseEvent(EventMaterialReceived);
        }

        /// <summary>
        /// Sends a result file to the facility server.
        /// </summary>
        /// <param name="filepath">The file path.</param>
        public void SendResultFile(string filepath)
        {

        }

        /// <summary>
        /// Adds a Gem event.
        /// </summary>
        /// <param name="ce">The ce.</param>
        public void AddEvent(GemEvent ce)
        {
            //_dataStore.Add(ce.ID, ce);
        }

        /// <summary>
        /// Invokes the change of a Gem variable change event.
        /// </summary>
        /// <param name="ce">The collection event.</param>
        public void RaiseEvent(GemEvent ce)
        {
            int instanceId = 0;

            if (ce.LinkedReports.Count > 0)
            {
                _instanceId++;
                instanceId = _instanceId;

                foreach (GemReport report in ce.LinkedReports)
                {
                    foreach (GemVariable var in report)
                    {
                        _commProvider.SetVariable(var.ID, _instanceId, var.Data);
                    }
                }
            }

            _commProvider.RaiseEvent(ce.ID, instanceId);
        }

        /// <summary>
        /// Adds a Gem alarm.
        /// </summary>
        /// <param name="alarm">The alarm variable.</param>
        public void AddAlarm(GemAlarm alarm)
        {
            _commProvider.AddAlarm(alarm);
        }

        /// <summary>
        /// Raises a Gem alarm notification.
        /// </summary>
        /// <param name="alarm">The alarm.</param>
        public void RaiseAlarm(GemAlarm alarm)
        {
            int setEventId = -1;
            if (alarm.SetEvent != null)
                setEventId = alarm.SetEvent.ID;

            _commProvider.RaiseAlarm(alarm.ID, setEventId);
        }

        /// <summary>
        /// Clears a Gem alarm state.
        /// </summary>
        /// <param name="alarm">The alarm to be cleared.</param>
        public void ClearAlarm(GemAlarm alarm)
        {
            int clearEventId = -1;
            if (alarm.SetEvent != null)
                clearEventId = alarm.ClearEvent.ID;

            _commProvider.ClearAlarm(alarm.ID, clearEventId);
        }

        /// <summary>
        /// Adds a Gem variable of interest.
        /// </summary>
        /// <param name="variable">The Gem variable.</param>
        public void AddVariable(GemVariable variable)
        {
            _dataStore.Add(variable.ID, variable);
            variable.PropertyChanged += UpdateVariable;

            _commProvider.AddVariable(variable);
        }
    }
}
