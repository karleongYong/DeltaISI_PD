using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.Logging
{
    /// <summary>
    /// Trace listener that outputs log messages to a specified <see cref="System.Windows.Forms.ListBox"/>.
    /// </summary>
    public class RollingListBoxListener : TraceListener
    {
        private bool _disposing = false;
        private bool _autoToBottom = false;
        private int _maxEntries = 10000;
        private const float BufferEntriesPercent = 0.1f;
        private BlockingCollection<string> _logQueue;

        private System.Timers.Timer _updateTimer;
        private ListBox _outputList;
        private TextBox _latestLineBox;

        /// <summary>
        ///  SourceFilters are used for filtering the log message originator. For example, it it is defined as "Motion", only the Motion log messages will appear in the listbox.
        /// </summary>
        /// <value>
        /// The source filters.
        /// </value>
        public List<string> SourceFilters
        {
            get;
            set;
        }

        /// <summary>
        /// MessageKeywordFilters are used for filtering the log message to look for string matches
        /// </summary>
        /// <value>
        /// The message keyword filters.
        /// </value>
        public List<string> MessageKeywordFilters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to auto scroll to the bottom of the list box when a new message arrives.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto scrolling to bottom; otherwise, <c>false</c>.
        /// </value>
        public bool AutoScrollToBottom
        {
            get
            {
                return _autoToBottom;
            }
            set
            {
                _autoToBottom = value;

                if (value)
                    ScrollToBottom();
            }
        }


        public int MaxEntries
        {
            get
            {
                return _maxEntries;
            }
            set
            {
                _maxEntries = value;

            }
        } 
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingListBoxListener"/> class.
        /// </summary>
        /// <param name="outputList">The list box to output the trace events</param>
        /// <param name="latestLineBox">The latest line box.</param>
        public RollingListBoxListener(ListBox outputList, TextBox latestLineBox = null)
            : base()
        {
            _outputList = outputList;
            _latestLineBox = latestLineBox;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingListBoxListener"/> class.
        /// </summary>
        /// <param name="name">Name of the listener</param>
        /// <param name="outputList">The list box to output the trace events</param>
        /// <param name="latestLineBox">The latest line box.</param>
        public RollingListBoxListener(string name, ListBox outputList, TextBox latestLineBox = null)
            : base(name)
        {
            _outputList = outputList;
            _latestLineBox = latestLineBox;
            Initialize();
        }

        private void Initialize()
        {
            _logQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _updateTimer = new System.Timers.Timer(20);
            _updateTimer.Elapsed += ProcessLogQueue;
            _updateTimer.Enabled = true;
        }

        /// <summary>
        /// When overridden in a derived class, closes the output stream so it no longer receives tracing or debugging output.
        /// </summary>
        public override void Close()
        {
            _disposing = true;
            _updateTimer.Enabled = false;
            
            base.Close();
        }

        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <value></value>
        /// <returns>true if the trace listener is thread safe; otherwise, false. The default is false.</returns>
        public override bool IsThreadSafe
        {
            get
            {
                return false; //force Trace.UseGlobalLock
            }
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        ///   </PermissionSet>
        public override void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {            
            // Check if any filters exist
            if (SourceFilters != null)
            {
                if (SourceFilters.Count > 0)
                {
                    bool isFilterMatched = false;

                    // If there are filters, check to make sure the source matches one of the filters. If it doesn't, then just return.
                    foreach (string filter in SourceFilters)
                    {
                        if (String.Compare(source, filter, true) == 0)
                        {
                            isFilterMatched = true;
                            break;
                        }
                    }

                    if (!isFilterMatched)
                    {
                        return;
                    }
                }
            }

            if (MessageKeywordFilters != null)
            {
                if (MessageKeywordFilters.Count > 0)
                {
                    bool isFilterMatched = false;                    
                    foreach (string keyword in MessageKeywordFilters)
                    {
                        if (message.Contains(keyword))
                        {
                            isFilterMatched = true;
                            break;
                        }
                    }

                    if (!isFilterMatched)
                    {
                        return;
                    }
                }
            }      

            string timestamp = eventCache.DateTime.ToLocalTime().ToString("[MM/dd/yy HH:mm:ss.ffff] ");

            // Prepend timestamp to messages with multiple lines so that addition lines in message
            // lines up to first line after the timestamp
            if (message.Contains(Environment.NewLine))
                message = message.Replace(Environment.NewLine, Environment.NewLine + timestamp);

            Write(timestamp + message);
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            if (message == null)
                return;

            _logQueue.Add(message);
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            Write(message);
        }

        private void ProcessLogQueue(object sender, System.Timers.ElapsedEventArgs e)
        {
            _updateTimer.Elapsed -= ProcessLogQueue;

            if (!_disposing && _logQueue.Count > 0)
            {
                UI.UIUtility.Invoke(_outputList, () =>
                {
                    _outputList.BeginUpdate();

                    while (_logQueue.Count > 0)
                    {
                        string[] items = _logQueue.Take().Split('\n');
                        _outputList.Items.AddRange(items);
                        
                        if (_latestLineBox != null)
                            _latestLineBox.Text = items[items.Length - 1];

                        int bufferEntries = (int)(Math.Ceiling(MaxEntries * BufferEntriesPercent));

                        int i = 0;
                        int ItemCount;
                        try
                        {
                            ItemCount = _outputList.Items.Count;

                            while (ItemCount > MaxEntries)
                            {
                                if (_outputList.Items[i] != null)
                                {
                                    _outputList.Items.RemoveAt(i);
                                    i++;
                                }
                                else
                                {
                                    break;
                                }

                                if (_outputList != null)
                                {
                                    ItemCount = _outputList.Items.Count;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        } catch(Exception ex)
                        {

                        }
                        
                        /*if (_outputList.Items.Count > MaxEntries) //+ bufferEntries)
                        {
                            for (int i = 0; i < bufferEntries; i++)
                            {
                                _outputList.Items.RemoveAt(i);
                            }
                        }*/
                    }

                    if (AutoScrollToBottom)
                        ScrollToBottom();

                    _outputList.EndUpdate();
                });
            }

            _updateTimer.Elapsed += ProcessLogQueue;
        }

        /// <summary>
        /// Scrolls to the bottom of the list box.
        /// </summary>
        public void ScrollToBottom()
        {
            int visibleItems = _outputList.ClientSize.Height / _outputList.ItemHeight;
            _outputList.TopIndex = Math.Max(_outputList.Items.Count - visibleItems + 1, 0);
        }
    }
}
