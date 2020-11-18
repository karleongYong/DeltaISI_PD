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
//  [8/20/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Collections;

namespace Seagate.AAS.Utils
{
    /// <summary>
    /// Summary description for WaitList.
    /// </summary>
    public class WaitList
    {
        // Member variables ----------------------------------------------------
        
        private ManualResetEvent _evNotEmpty  = new ManualResetEvent(false);
        protected ArrayList list = new ArrayList();
        
        // Constructors & Finalizers -------------------------------------------

        public WaitList() 
        {
        }
        
        // Nested declarations -------------------------------------------------
        
        // Properties ----------------------------------------------------------
        
        // Methods -------------------------------------------------------------

        public int WaitForNotEmpty(ManualResetEvent stopEvent)
        {
            ManualResetEvent[] eventGroup = new ManualResetEvent[2];
            eventGroup[0] = _evNotEmpty;  
            eventGroup[1] = stopEvent;
            
            return ManualResetEvent.WaitAny(eventGroup);  // Waits until either Mutex is released
        }

        public int WaitForNotEmpty(ManualResetEvent stopEvent, int timeOut)
        {
            ManualResetEvent[] eventGroup = new ManualResetEvent[2];
            eventGroup[0] = _evNotEmpty;  
            eventGroup[1] = stopEvent;
            
            if (timeOut == Timeout.Infinite)
                return ManualResetEvent.WaitAny(eventGroup);
            else
                return ManualResetEvent.WaitAny(eventGroup, timeOut, false);
        }

        public void AddTail(System.Object newElement)
        {
            Lock();
            list.Add(newElement);
            _evNotEmpty.Set();
            Unlock();
        }

        public void RemoveAt(System.Object element)
        {
            Lock();           
            list.Remove(element);
            if (list.Count == 0)
                _evNotEmpty.Reset();
            Unlock();
        }

        public void RemoveAll()
        {
            Lock();
            list.Clear();
            _evNotEmpty.Reset();
            Unlock();
        }

        public void Lock()
        {
            Monitor.Enter(list);
        }

        public void Unlock()
        {
            Monitor.Exit(list);
        }
        
    }		
}
