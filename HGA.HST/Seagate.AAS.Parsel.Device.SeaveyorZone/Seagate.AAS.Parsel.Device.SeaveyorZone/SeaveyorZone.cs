using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;

namespace Seagate.AAS.Parsel.Device.SeaveyorZone
{

    /// <summary>
    /// SeaveyorZone IO
    /// </summary>
    public struct SeaveyorZoneIO
    {
        //This is interface output I/O that to inhibit the Zone
        public IDigitalOutput inhibit;
        public IDigitalOutput reverse;

        //This is interface input I/O that to monitor Zone Sensor.
        public IDigitalInput zoneSensor;
        //This is interface input I/O that to check move completed.
        public IDigitalInput inPosition;
    };
    /// <summary>
    /// Summary description for Seaveyor zone ,derived from ISeaveyorZone.
    /// Interface communicaton is using by IO.
    /// The constructor for the SeaveyorZone will expect all users to provide 
    /// 2 inputs,
    /// 1 output.
    /// </summary>
    public class SeaveyorZone : ISeaveyorZone
    {
        #region Member Variables
        internal SeaveyorZoneIO io;
        private string _name = "SeaveyorZone";
        private uint _timeout = 2000;	//default 2 sec.
        private bool _simulation = false;//true:with hardware,false:without hardware
        private bool _inhibit, _reverse;
        #endregion

        #region Costructors

        public SeaveyorZone(SeaveyorZoneIO io)
        {
            this.io = io;
            _inhibit = false;
            _reverse = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Name of the SeaveyorZone.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public DigitalIOState InPosition
        {
            get { return io.inPosition.Get(); }
        }
        /// <summary>
        /// This is for simulation mode.
        /// </summary>
        public bool Simulation
        {
            get { return _simulation; }
            set { _simulation = value; }
        }
        /// <summary>
        /// This is to check completed signal until time out.
        /// </summary>
        public uint TimeOut
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        #endregion


        //Methods
        /// <summary>
        /// Assign the I/O and intializes Others data.
        /// </summary>
        public virtual void Initialize()
        {

        }
        public bool IsInPosition(DigitalIOState ioSate)
        {
            //if(_simulation)
            if (Simulation)
                return true;
            if (io.inPosition.Get() == ioSate)
                return true;
            else
                return false;
        }

        public bool IsInpositionSensorOn()
        {
            if (Simulation) return true;
            return io.inPosition.Get() == DigitalIOState.On;
        }

        public void WaitForMoveCompleted(DigitalIOState ioSate, uint msecTimeOut)
        {
            if (Simulation) return;
            io.inPosition.WaitForState(ioSate, msecTimeOut);
        }
        public void Release()
        {
            if (Simulation) return;
            io.inhibit.Set(DigitalIOState.Off);
            _inhibit = false;
        }
        public void Inhibit(bool stateInhibit)
        {
            if (Simulation) return;
            if (stateInhibit)
            {
                _inhibit = true;
                io.inhibit.Set(DigitalIOState.On);
            }
            else
            {
                _inhibit = false;
                io.inhibit.Set(DigitalIOState.Off);
            }

        }

        public bool IsZoneSensorOn()
        {
            if (Simulation) return true;
            return io.zoneSensor.Get() == DigitalIOState.On;
        }
        public bool IsInhibit()
        {
            return _inhibit;

        }

        public void Reverse()
        {
            if (Simulation) return;
            if (io.reverse.Get() == DigitalIOState.On)
            {
                io.reverse.Set(DigitalIOState.Off);
                _reverse = false;
            }
            else
            {
                io.reverse.Set(DigitalIOState.On);
                _reverse = true;
            }
            _reverse = _reverse ? false : true;
        }
        public bool IsReverse()
        {
            return _reverse;

        }

    }
}
