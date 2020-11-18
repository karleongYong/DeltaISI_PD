using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    public class WorkOrderInfo
    {
        public delegate void LoadingWOChangedHandler();
        public event LoadingWOChangedHandler OnLoadingWOChanged;

        private WorkOrderData loading = null; // Use at Loading zone 9 (write)        
        private WorkOrderData unloading = null; // Use at Unloading zone 5 (read)        

        private FileChangeNotifier[] WorkOrderMonitor = new FileChangeNotifier[2]; // If loading WO same with unloading WO , use only 1 monitor

        public WorkOrderInfo(string LocalWorkOrderDir, string LogDir, char LoadingStationCode)
        {
            loading = new WorkOrderData(LocalWorkOrderDir, LogDir, LoadingStationCode.ToString());
            unloading = new WorkOrderData(LocalWorkOrderDir, LogDir, LoadingStationCode.ToString());
            
            for (int i=0;i<WorkOrderMonitor.Length;i++)
            {                
                WorkOrderMonitor[i] = new FileChangeNotifier();
                WorkOrderMonitor[i].OnWorkOrderContentChanged += new FileChangeNotifier.WOContentChangedHandler(WorkOrderInfo_OnWorkOrderContentChanged);
            }
        }

        public WorkOrderData Loading { get { return loading; } }          
        public WorkOrderData Unloading { get { return unloading; } }        

        #region Public Methods

        public void LoadNewLoadingWO(string serverFileName)
        {
            Loading.LoadNewWO(serverFileName);
            StartWOChangedMonitor(0, serverFileName);
            OnLoadingWOChanged();
        }        
        
        public void LoadNewUnloadingWO(string serverFileName)
        {
            Unloading.LoadNewWO(serverFileName);
            StartWOChangedMonitor(1, serverFileName);
        }
        
        #endregion

        #region Private Methods
        private void StartWOChangedMonitor(int monitorNo, string serverFileName)
        {
            bool needToStart = true;
            int anotherMonitorNo = 0;
            if (monitorNo == 0)
                anotherMonitorNo = 1;

            if (WorkOrderMonitor[monitorNo].StartWatchingFlag)
            {
                if (WorkOrderMonitor[monitorNo].WatchingFileName.Equals(serverFileName, StringComparison.CurrentCultureIgnoreCase))
                    needToStart = false;
                else // if current monitor is start, check if it is same file, don't need to do anything
                { // if it is different file, check another monitor, it another file same as the file. Stop current monitor. because the another monitor is checking
                    if (WorkOrderMonitor[anotherMonitorNo].StartWatchingFlag)
                    {
                        if (WorkOrderMonitor[anotherMonitorNo].WatchingFileName.Equals(serverFileName, StringComparison.CurrentCultureIgnoreCase))
                            WorkOrderMonitor[monitorNo].StopWatching();
                    }
                }
            }
            else // if current monitor no is not start, try to check another monitor.
            {
                if (WorkOrderMonitor[anotherMonitorNo].StartWatchingFlag)
                {
                    if (WorkOrderMonitor[anotherMonitorNo].WatchingFileName.Equals(serverFileName, StringComparison.CurrentCultureIgnoreCase))
                        needToStart = false;
                }
            }

            if (needToStart)
            {
                if (WorkOrderMonitor[monitorNo].StartWatchingFlag)
                    WorkOrderMonitor[monitorNo].StopWatching();
                WorkOrderMonitor[monitorNo].StartWatching(serverFileName);                
            }
        }

        void  WorkOrderInfo_OnWorkOrderContentChanged(string fileName)
        {
            if (loading != null)
            {
                if (loading.ServerFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    loading.ReloadWO();
                    OnLoadingWOChanged();
                }
            }            
        }

        #endregion
    }
}
