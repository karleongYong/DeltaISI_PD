using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace XyratexOSC.Logging
{
    /// <summary>
    /// Provides a trace listener that outputs logging to a day-stamped file that rolls over at midnight.
    /// </summary>
    [HostProtection(Synchronization = true)]
    public class DailyTextWriterListener : Microsoft.VisualBasic.Logging.FileLogTraceListener
    {
        const int MaxLogFilesAllowed = 775;
        private string OriginalFileName;
        private object _purgeLock = new object();

        private long m_maxTraceFileSize = 1280 * 1024 * 1024;
        private bool _purgeErrLogged = false;
        private Task _purgeTask = null;
        private DateTime _lastPurgeDateTime = DateTime.MinValue; // default to beginning of time

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyTextWriterListener"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public DailyTextWriterListener(string filename) :
            base()
        {
            //create directory for logging if it doesn't exist
            string directoryName = Path.GetDirectoryName(filename);

            if (string.IsNullOrEmpty(directoryName))
                directoryName = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            directoryName = Path.GetFullPath(directoryName);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            filename = Path.Combine(directoryName, Path.GetFileName(filename));

            //set attributes
            Location = Microsoft.VisualBasic.Logging.LogFileLocation.Custom;
            CustomLocation = Path.GetDirectoryName(filename);
            BaseFileName = Path.GetFileNameWithoutExtension(filename);
            LogFileCreationSchedule = Microsoft.VisualBasic.Logging.LogFileCreationScheduleOption.Daily;
            TraceOutputOptions = TraceOptions.None;
            Delimiter = ":";
            AutoFlush = true;
            Append = true;
            MaxFileSize = m_maxTraceFileSize;
            OriginalFileName = BaseFileName;
        }

        /// <summary>
        /// Writes trace information, a message and event information to the output file or stream.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name of the trace source that invoked this method.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> enumeration values.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        ///   </PermissionSet>
        public override void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (!eventType.Equals(TraceEventType.Critical))
            {
                char eventTypeChar = 'E';

                switch (eventType)
                {
                    case TraceEventType.Information:
                        eventTypeChar = 'I';
                        break;
                    case TraceEventType.Warning:
                        eventTypeChar = 'W';
                        break;
                    case TraceEventType.Error:
                        eventTypeChar = 'E';
                        break;
                    case TraceEventType.Critical:
                        eventTypeChar = 'C';
                        break;
                }

                string header = String.Format("{0},{1},{2},", eventCache.DateTime.ToLocalTime().ToString("MM/dd/yy,HH:mm:ss.ffff"), id, eventTypeChar);

                // Prepend timestamp to messages with multiple lines so that addition lines in message
                // lines up to first line after the timestamp
                if (message.Contains(Environment.NewLine))
                    message = message.Replace(Environment.NewLine, Environment.NewLine + header);
                BaseFileName = OriginalFileName + "_H" + DateTime.Now.Hour;
                base.WriteLine(header + message);

                PurgeLogFiles(MaxLogFilesAllowed, eventCache); // purge older log files if got to many

            }
            else
            {
                // Maintenance log file header..
                string _date = DateTime.Now.ToString("-yyyy-MM-dd");
                string LogFilePathForMaintenance = string.Format("{0}\\{1}_Maintenance{2}.log", CustomLocation, OriginalFileName, _date);
                if (!File.Exists(LogFilePathForMaintenance))
                {
                    //   File.Create(LogFilePathForMaintenance).Dispose();

                    using (StreamWriter w = File.AppendText(LogFilePathForMaintenance))
                    {
                        w.WriteLine("Time Stamp, Error Code, Error Category, Error Message, Type");
                        w.Close();
                    }


                }
                BaseFileName = OriginalFileName + "_Maintenance";

                string header = String.Format("{0},", eventCache.DateTime.ToLocalTime().ToString("MM/dd/yy - HH:mm:ss"));

                // Prepend timestamp to messages with multiple lines so that addition lines in message
                // lines up to first line after the timestamp
                if (message.Contains(Environment.NewLine))
                    message = message.Replace(Environment.NewLine, Environment.NewLine + header);

                //Force to be not over 5 comma index
                var msgsplit = message.Split(',');
                if(msgsplit.Length > 4)
                {
                    message = string.Empty;
                    message = string.Format("{0},{1},{2},{3}",msgsplit[0], msgsplit[1], msgsplit[2], msgsplit[3]);
                }

                base.WriteLine(header + message);
            }
        }

        /// <summary>
        /// Writes a verbatim message to disk, without any additional context information.
        /// </summary>
        /// <param name="message">String. The custom message to write.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        ///   </PermissionSet>
        public override void Write(string message)
        {
        }

        private void PurgeLogFiles(int maxFilesToKeep, System.Diagnostics.TraceEventCache eventCache)
        {
            string fileFilter = BaseFileName + "*.log";

            lock (_purgeLock)
            {
                if (DateTime.Now.Date == _lastPurgeDateTime.Date)
                {
                    // only need to perform one purge per day (since this is a daily logger).
                    return;
                }
                else if (Directory.GetFiles(CustomLocation, fileFilter).Length <= maxFilesToKeep)
                {
                    // purge only if more than max number of log files allowed
                    return;
                }
                else if (_purgeTask != null && _purgeTask.Status == TaskStatus.Running)
                {
                    // purge only if no purge task is in progress
                    return;
                }
                else
                {
                    _lastPurgeDateTime = DateTime.Now; // record purge date/time

                    _purgeTask = Task.Factory.StartNew(() =>
                        {
                            var logDir = CustomLocation;
                            var logFileList = new DirectoryInfo(logDir).GetFiles(fileFilter);
                            var sorted = logFileList.OrderBy(f => f.LastWriteTime).ToList();
                            while (sorted.Count > maxFilesToKeep)
                            {
                                File.Delete(sorted[0].FullName);
                                sorted.RemoveAt(0);
                            }
                        });
                    // handle exception if any
                    _purgeTask.ContinueWith(t =>
                    {
                        Exception ex = t.Exception;
                        if (ex != null && ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                            // Only want to log purge error once during the life of the run app.
                            // Otherwise, we could potentially log purge error for each log message!
                            if (!_purgeErrLogged)
                            {
                                _purgeErrLogged = true;
                                string message = "DailyTextWriterListener.PurgeLogFiles(): failed with exception: " + ex.ToString();
                                base.WriteLine(eventCache.DateTime.ToLocalTime().ToString("MM/dd/yy,HH:mm:ss.ffff,") + message);
                            }
                        }
                    });
                } // else
            } // lock
        }
    }
}