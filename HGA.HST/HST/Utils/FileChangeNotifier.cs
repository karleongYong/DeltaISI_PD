using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Seagate.AAS.HGA.HST.Utils
{
    public class FileChangeNotifier
    {
        public delegate void WOContentChangedHandler(string fileName);
        public event WOContentChangedHandler OnWorkOrderContentChanged;

        private System.IO.FileSystemWatcher m_Watcher;
        private bool m_bIsWatching;
        private string watchingFileName = "";
        private string fileName = "";

        public FileChangeNotifier()
        {
            m_bIsWatching = false;
        }

        public bool StartWatchingFlag { get { return m_bIsWatching; } }
        public string WatchingFileName { get { return watchingFileName; } }

        /// <summary>
        /// Filename is filename with full path
        /// </summary>
        /// <param name="fileName"></param>
        public void StartWatching(string fileName)
        {
            this.fileName = fileName + ".wo";
            m_bIsWatching = true;

            if (m_Watcher != null)
            {
                m_Watcher.EnableRaisingEvents = false;
                m_Watcher.Dispose();
            }
            m_Watcher = new System.IO.FileSystemWatcher(Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
            m_Watcher.NotifyFilter = NotifyFilters.LastWrite;
            m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
            m_Watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            if (m_Watcher != null)
            {
                m_Watcher.EnableRaisingEvents = false;
                m_Watcher.Changed -= new FileSystemEventHandler(OnChanged);
                m_Watcher.Dispose();
            }
            m_bIsWatching = false;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            OnWorkOrderContentChanged(fileName);
        }
    }
}
