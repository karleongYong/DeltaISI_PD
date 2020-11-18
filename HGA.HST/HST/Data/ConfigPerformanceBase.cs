
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Xml.Serialization;
using System.Xml;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Machine;
//using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Equipment;

namespace Seagate.AAS.HGA.HST.Data
{
    /// <summary>
    /// Provide machine performance data.
    /// 
    /// </summary>
    public class ConfigPerformanceBase
    {
        public enum MachineState
        {
            Idle = 0,
            Homing = 1,
            Standby = 2,
            Running = 3,
            Alarm = 4,
        }


        // Nested declarations -------------------------------------------------
        public const string CurrentPerformaceConfigName = "CurrentPerformance.config";
        public MachineState _currentMcState;
      
        // Member variables ----------------------------------------------------        
        protected Timer _updateTimer;
        protected bool _isTrackingStarted = false;
        protected int _alarmCount;
        protected DateTime _startTrackingEvent = new DateTime();

        protected double _UPH;        
        protected TimeSpan _totalTime;
        protected TimeSpan _runTime;
        protected TimeSpan _downTime;
        protected TimeSpan _idleTime;
        protected TimeSpan _mtba;
        protected TimeSpan _standbyTime;

        // Constructors & Finalizers -------------------------------------------
        public ConfigPerformanceBase()
        {
        }

        public void Dispose()
        {
            _updateTimer.Stop();
        }
      
        // Properties ----------------------------------------------------------        
        public double UPH { get { return _UPH; } set { _UPH = value; } }        
        [XmlIgnore]
        public TimeSpan TotalTime { get { return _totalTime; } set { _totalTime = value; } }
        [XmlIgnore]
        public TimeSpan RunTime { get { return _runTime; } set { _runTime = value; } }
        [XmlIgnore]
        public TimeSpan DownTime { get { return _downTime; } set { _downTime = value; } }
        [XmlIgnore]
        public TimeSpan IdleTime { get { return _idleTime; } set { _idleTime = value; } }
        [XmlIgnore]
        public TimeSpan StandbyTime { get { return _standbyTime; } set { _standbyTime = value; } }
        [XmlIgnore]
        public TimeSpan MTBA { get { return _mtba; } set { _mtba = value; } }
        public int AlarmCount { get { return _alarmCount; } set { _alarmCount = value; } }
        public bool IsTrackingStarted { get { return _isTrackingStarted; } set { _isTrackingStarted = value; } }
        public string XMLTotalTime { get { return XmlConvert.ToString(_totalTime); } set { _totalTime = XmlConvert.ToTimeSpan(value); } }
        public string XMLRunTime { get { return XmlConvert.ToString(_runTime); } set { _runTime = XmlConvert.ToTimeSpan(value); } }
        public string XMLDownTime { get { return XmlConvert.ToString(_downTime); } set { _downTime = XmlConvert.ToTimeSpan(value); } }
        public string XMLIdleTime { get { return XmlConvert.ToString(_idleTime); } set { _idleTime = XmlConvert.ToTimeSpan(value); } }
        public string XMLMTBA { get { return XmlConvert.ToString(_mtba); } set { _mtba = XmlConvert.ToTimeSpan(value); } }

        // Methods -------------------------------------------------------------
        public void Initialize()
        {
            _updateTimer = new Timer();
            _updateTimer.Interval = 1000;
            _updateTimer.Enabled = true;
            _updateTimer.Elapsed += new ElapsedEventHandler(updateTimer_Elapsed);                       
        }

        public virtual void StartTracking()
        {            
            _startTrackingEvent = DateTime.Now;           
            _totalTime = new TimeSpan();
            _runTime = new TimeSpan();
            _downTime = new TimeSpan();
            _idleTime = new TimeSpan();
            _mtba = new TimeSpan();
            _alarmCount = 0;
           
            _isTrackingStarted = true;
           
        }

        public virtual void StopTracking()
        {
            OnUpdateTimer();
            _isTrackingStarted = false;
        }        

        // Internal methods ----------------------------------------------------       
        protected virtual void OnUpdateTimer()
        {
            lock (this)
            {
                if (HSTMachine.Instance == null)
                    return;
                if (!_isTrackingStarted)
                    return;
                _totalTime = DateTime.Now - _startTrackingEvent;
                bool _errrorOccur = false;

                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.Process != null)
                    {
                        if (HSTMachine.Workcell.Process.ActiveProcess != null)
                        {
                            foreach (Active item in HSTMachine.Workcell.Process.ActiveProcess.Values)
                            {
                                if (((ActiveProcessHST)item).IsErrorState)
                                    _errrorOccur = true;
                            }
                        }

                        if (HSTMachine.Workcell.Process.IsIdleState && !_errrorOccur)
                        {
                            _idleTime = _totalTime - _runTime - _downTime;
                            return;
                        }
                        else if (HSTMachine.Workcell.Process.IsRunState && !_errrorOccur)
                        {
                            _runTime = _totalTime - _downTime - _idleTime;
                            return;
                        }
                        else if (_errrorOccur)
                        {
                            _downTime = _totalTime - _idleTime - _runTime;
                            return;

                        }
                        else
                        {
                            return;
                        }
                    }
                }                                


                if (_alarmCount > 0)
                {
                    double sec = _runTime.TotalSeconds / _alarmCount;
                    _mtba = new TimeSpan(0, 0, Convert.ToInt32(sec));
                }
                else
                {
                    _mtba = _runTime.Duration();
                }
            }
        }

        protected void OnMachineStateChanged(MachineState newState)
        {
            if (_isTrackingStarted == false)
                return;
            _currentMcState = newState;
            switch (newState)
            {
                case MachineState.Idle:
                    break;
                case MachineState.Homing:
                    break;
                case MachineState.Standby:
                    break;
                case MachineState.Running:
                    break;
                case MachineState.Alarm:
                    break;
                default:
                    break;
            }
        }

        // Event handlers ------------------------------------------------------
        void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnUpdateTimer();
        }
        void Instance_MachineStateChanged(MachineState newState, MachineState previousState)
        {
            OnMachineStateChanged(newState);
        }

        public void Save(string section, Seagate.AAS.Utils.SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.Write("TotalTime", TotalTime.TotalMinutes);
            xml.Write("StandbyTime", IdleTime.TotalMinutes);
            xml.Write("RunTime", RunTime.TotalMinutes);
            xml.Write("DownTime", DownTime.TotalMinutes);
            xml.CloseSection();
        }
        public void Load(string section, Seagate.AAS.Utils.SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.CloseSection();
        }
    }
}
