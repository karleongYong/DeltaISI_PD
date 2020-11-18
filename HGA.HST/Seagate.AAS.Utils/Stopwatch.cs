//
//  © Copyright 2005 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace Seagate.AAS.Utils
{
    // -------------------------------------------------------------------------
	[Serializable()]
    public abstract class StopwatchBase
    {
        protected string _name;
        protected bool   _running;

        public StopwatchBase()                { _running = false;}
        public abstract void   Start();
        public abstract void   Stop();
        public abstract void   Reset();
        public abstract void   Continue();
        public abstract void   ResetContinue();


        public string Name 
        {
            get     {return _name;}
            set     {_name = value;}
        }

        public bool Running
        {
            get     {return _running;}
        }
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// 10 msec precision timer
    /// </summary>
    public class StopwatchW32 : StopwatchBase
    {
        [DllImport("kernel32.dll", SetLastError=true)]
        static extern int GetTickCount();

        private int _startTime;
        private int _stopTime;

        public StopwatchW32()
        {
            _startTime = 0;
            _stopTime  = 0;
        }

        public override void Start()
        {
            if (!_running)
            {
                _startTime = GetTickCount() - 1; 
                _running = true;
            }
        }

        public override void Stop()
        {
            if (_running)
            {
                _stopTime = GetTickCount();
                _running = false;
            }
        }

        public override void Reset()
        {
            if (_running)
                _startTime = GetTickCount();	// reset of running counter will just set the start time to NOW
            else
                _startTime = _stopTime;			// reset of a not running counter just sets the times to be equal so that elapsed time = 0
        }

        public override void Continue()
        {
            if (!_running && _stopTime != 0)
            {
                _startTime = GetTickCount() - _stopTime; 
                _running = true;
            }

        }

        public override void ResetContinue()
        {
            if (_running)
            {
                 _startTime = GetTickCount() - 1;               
            } 
        }

        public int ElapsedTime_msec
        {
            get
            {
                if (_running)
                    return GetTickCount() - _startTime;
                else
                    return _stopTime - _startTime;
            }
        }
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
	/// 
	[Serializable()]
    public class Stopwatch : StopwatchBase
    {
        private DateTime _startTime;
        private DateTime _stopTime;
        private TimeSpan _lastQuery;

        public Stopwatch()
        {
        }

        public override void Start()
        {
            if (!_running)
            {
                _startTime = DateTime.Now; 
                _running = true;
            }
        }

        public override void Stop()
        {
            if (_running)
            {
                _stopTime = DateTime.Now;
                _lastQuery = _stopTime.Subtract(_startTime);
                _running = false;
            }
        }

        public override void Reset()
        {
            System.DateTime timeNow = System.DateTime.Now;
            _startTime = timeNow; //sets start & stop time to now so equal
            _stopTime = timeNow;
            _lastQuery = DateTime.Now.Subtract(_startTime);
        }

        public override void Continue()
        {
            if (!_running )
            {
                _startTime = DateTime.Now - _lastQuery; 
                _running = true;
            }

        }

        public override void ResetContinue()
        {
            if (_running)
            {
                _startTime = DateTime.Now;                        
            } 
        }

        public double ElapsedTime_sec
        {
            get
            {
                if (_running)
                    _lastQuery = DateTime.Now.Subtract(_startTime);

                return (double)_lastQuery.Ticks / (double)TimeSpan.TicksPerSecond;
            }
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                if (_running)
                    _lastQuery = DateTime.Now.Subtract(_startTime);

                return _lastQuery;
            }
        }    
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Summary description for StopwatchW32HiRes.
    /// </summary>
    public class StopwatchW32HiRes : StopwatchBase
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(ref long lpFrequency);

        private long _startTime, _stopTime;
        private long freq;

        public StopwatchW32HiRes()
        {
            _startTime = 0;
            _stopTime = 0;

            if (QueryPerformanceFrequency(ref freq) == false)
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
        }

        public override void Start()
        {
            if (!_running)
            {
                //Thread.Sleep(0); // lets do the waiting threads there work
                QueryPerformanceCounter(ref _startTime);
                _running = true;
            }
        }

        public override void Stop()
        {
            if (_running)
            {
                QueryPerformanceCounter(ref _stopTime);
                _running = false;
            }
        }
        public override void Reset()
        {
            _startTime = _stopTime;
            _running = false;
        }

        public override void Continue()
        {
            if (!_running)
            {
                long lastInterval = _stopTime - _startTime;
                QueryPerformanceCounter(ref _startTime);
                _startTime -= lastInterval; 
                _running = true;
            }
        }

        public override void ResetContinue()
        {
            if (_running)
            {
                QueryPerformanceCounter(ref _startTime);
            } 
        }

        public double ElapsedTime_sec
        {
            get
            {
                if (_running)
                    QueryPerformanceCounter(ref _stopTime);

                return (double)(_stopTime - _startTime) / (double) freq;
            }
        }
    }

    // -------------------------------------------------------------------------
}
