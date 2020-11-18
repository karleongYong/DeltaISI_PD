using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace XyratexOSC.Logging
{
    /// <summary>
    /// Application logging using <see cref="TraceSource"/>.
    /// </summary>
    public static class Log
    {
        private static readonly object _locker = new object();
        private readonly static Dictionary<string, TraceSource> _traceSources = new Dictionary<string, TraceSource>();

        /// <summary>
        /// Gets the default name of the application trace.
        /// </summary>
        /// <value>
        /// The default name of the application trace.
        /// </value>
        public static string DefaultTraceName
        {
            get
            {
                return AppDomain.CurrentDomain.FriendlyName;
            }
        }
        
        /// <summary>
        /// Add or retrieve a <see cref="TraceSource"/> to the global list so we have control over it
        /// </summary>
        /// <param name="name"></param>
        private static TraceSource GetTraceSource(string name)
        {
            if (String.IsNullOrEmpty(name))
                name = DefaultTraceName;

            if (_traceSources.ContainsKey(name))
                return (_traceSources[name]);

            using (Utilities.TimedLock.Lock(_locker))
            {
                if (_traceSources.ContainsKey(name))
                    return (_traceSources[name]);

                TraceSource traceSource = new TraceSource(name);
                traceSource.Listeners.Remove("Default");
                _traceSources.Add(name, traceSource);

                return traceSource;
            }
        }

        /// <summary>
        /// Call this before exiting application or when application is Idle to make sure that all traces are flushed to the hard drive
        /// </summary>
        public static void FlushAllTraces()
        {
            List<string> badTraces = new List<string>();
            foreach (var traceSource in _traceSources)
            {
                if (traceSource.Value != null)
                {
                    try
                    {
                        traceSource.Value.Flush();
                    }
                    catch
                    {
                        badTraces.Add(traceSource.Key);
                    }
                }
                else
                {
                    badTraces.Add(traceSource.Key);
                }
            }

            foreach (var badTraceName in badTraces)
                _traceSources.Remove(badTraceName);
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Info(object source, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Info(words[index], messageFormat, args);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Warn(object source, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Warn(words[index], messageFormat, args);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Error(object source, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Error(words[index], messageFormat, args);
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Info(object source, int id, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Info(words[index], id, messageFormat, args);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Warn(object source, int id, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Warn(words[index], id, messageFormat, args);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Error(object source, int id, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Error(words[index], id, messageFormat, args);
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Info(string source, int id, string messageFormat, params object[] args)
        {
            Trace().Info(source, id, messageFormat, args);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Warn(string source, int id, string messageFormat, params object[] args)
        {
            Trace().Warn(source, id, messageFormat, args);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Error(string source, int id, string messageFormat, params object[] args)
        {
            Trace().Error(source, id, messageFormat, args);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="ex">The exception to log</param>
        public static void Error(string source, int id, Exception ex)
        {
            Trace().Error(source, id, ex);
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Info(string source, string messageFormat, params object[] args)
        {
            Trace().Info(source, Thread.CurrentThread.ManagedThreadId, messageFormat, args);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Warn(string source, string messageFormat, params object[] args)
        {
            Trace().Warn(source, Thread.CurrentThread.ManagedThreadId, messageFormat, args);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Error(string source, string messageFormat, params object[] args)
        {
            Trace().Error(source, Thread.CurrentThread.ManagedThreadId, messageFormat, args);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="ex">The exception to log</param>
        public static void Error(string source, Exception ex)
        {
            Trace().Error(source, Thread.CurrentThread.ManagedThreadId, ex);
        }

        /// <summary>
        /// Formats the log parameters to a message string.
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        /// <returns></returns>
        private static string FormatMessage(string source, string messageFormat, params object[] args)
        {
            string message;

            if (args.Length == 0)
                message = messageFormat;
            else
                message = String.Format(messageFormat, args);

            if (String.IsNullOrEmpty(source))
                return message;
            else
                return String.Format("{0},{1}", source, message);
        }

        /// <summary>
        /// Retrieves the <see cref="TraceSource"/> the specified name from an Application trace list.
        /// </summary>
        /// <param name="traceName">Name of the trace.</param>
        /// <returns></returns>
        public static TraceSource Trace(string traceName)
        {
            return GetTraceSource(traceName);
        }

        /// <summary>
        /// Retrieves the default application <see cref="TraceSource"/> from an Application trace list.
        /// </summary>
        /// <returns></returns>
        public static TraceSource Trace()
        {
            return GetTraceSource(null);
        }

        /// <summary>
        /// Writes a debug (verbose) message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Debug(this TraceSource trace, string source, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Verbose, Thread.CurrentThread.ManagedThreadId, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes an info message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Info(this TraceSource trace, string source, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Information, Thread.CurrentThread.ManagedThreadId, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes a warning message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Warn(this TraceSource trace, string source, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, Thread.CurrentThread.ManagedThreadId, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes an error message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Error(this TraceSource trace, string source, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, Thread.CurrentThread.ManagedThreadId, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes an error message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="ex">The exception to log</param>
        public static void Error(this TraceSource trace, string source, Exception ex)
        {
            trace.TraceData(TraceEventType.Error, Thread.CurrentThread.ManagedThreadId, FormatMessage(source, "{0}", ex));
        }

        /// <summary>
        /// Writes a debug (verbose) message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Debug(this TraceSource trace, string source, int id, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Verbose, id, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes an info message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Info(this TraceSource trace, string source, int id, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Information, id, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes a warning message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Warn(this TraceSource trace, string source, int id, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, id, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes an error message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Error(this TraceSource trace, string source, int id, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Error, id, FormatMessage(source, messageFormat, args));
        }

        /// <summary>
        /// Writes an error message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="ex">The exception to log</param>
        public static void Error(this TraceSource trace, string source, int id, Exception ex)
        {
            //trace.TraceData(TraceEventType.Error, id, FormatMessage(source, "{0}", ex));
            trace.TraceEvent(TraceEventType.Error, id, FormatMessage(source, "{0}", ex));
        }
        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Maintenance(object source, string messageFormat, params object[] args)
        {
            char[] delimiters = new char[] { '.', '-' };
            string[] words = source.ToString().Split(delimiters);
            int index = 0;
            if (words.Length >= 2)
            {
                index = words.Length - 2;
            }
            else
            {
                index = words.Length - 1;
            }
            XyratexOSC.Logging.Log.Maintenance(words[index], messageFormat, args);
        }
        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Maintenance(string source, string messageFormat, params object[] args)
        {
            Trace().Maintenance(source, Thread.CurrentThread.ManagedThreadId, messageFormat, args);
        }

        /// <summary>
        /// Writes an error message to the specified trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        public static void Maintenance(this TraceSource trace, string source, int id, string messageFormat, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Critical, id, FormatMessage2(source, messageFormat, args));
        }

        /// <summary>
        /// Formats the log parameters to a message string.
        /// </summary>
        /// <param name="source">Subsystem name to help with log categorizing, filtering</param>
        /// <param name="messageFormat">Format of the string field</param>
        /// <param name="args">String format arguments</param>
        /// <returns></returns>
        private static string FormatMessage2(string source, string messageFormat, params object[] args)
        {
         //   string message;

         //   if (args.Length == 0)
         //       message = messageFormat;
         //   else
         //       message = String.Format(messageFormat, args);

         //   if (String.IsNullOrEmpty(source))
         //       return message;
          //  else
            if (args.Length == 2)
            {
                return String.Format("{0},{1},{2}", args[0], source, args[1]);
            }
            else if(args.Length == 3)
            {
                return String.Format("{0},{1},{2},{3}", args[0], source, args[1], args[2]);
            }
            else
                return String.Format("{0}", args[0]);
        }

    }
}