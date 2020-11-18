
using System;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.Parsel.Device.PneumaticControl;
namespace Seagate.AAS.Parsel.Device.TurnSection
{
	/// <summary>
	/// TurnSection IO
	/// </summary>
	public struct TurnSectionIO
	{
		//This is interface output I/O that turn to 90 DEG.
		public IDigitalOutput	turnTo90Deg;
		//This is interface output I/O that turn to 0 DEG.
		public IDigitalOutput	turnTo0Deg;
		//This is interface output I/O that Entrance Relay.
		public IDigitalOutput	entranceRelay;
		//This is interface output I/O that Exit Relay.
		public IDigitalOutput	exitRelay;
        //This is intaface output I/o that Inhibit
        public IDigitalOutput inhibitRelay;

	 
		//This is interface input I/O zone sensor.
		public IDigitalInput	zoneSensor;
		//This is interface input I/O Entrance Sensor(Thru Beam).
		public IDigitalInput	entranceSensor;
		//This is interface input I/O Entrance Clear Sensor(Thru Beam).
		public IDigitalInput	entranceClearSensor;
		//This is interface input I/O Exit sensor(Thru Beam).
		public IDigitalInput	exitSensor;
		//This is interface input I/O Exit Clear sensor(Thru Beam).
		public IDigitalInput	exitClearSensor;
		//This is interface input I/O In Position signal.
		public IDigitalInput	inPosition;
		//This is interface input I/O 90 Deg (Reed Switch).
		public IDigitalInput	At90DegSensor;
		//This is interface input I/O 0 Deg (Reed Switch).
		public IDigitalInput	At0DegSensor;

	};
	public class TurnSection : ITurnSection
	{
		#region Member Variables 
		public TurnSectionIO io;
		private string	_name = "Turn Section";
		private uint	_timeout = 2000;	//default 2 sec.
		private bool	_simulation = false;//true:with hardware,false:without hardware
		private IRotaryActuator rotaryCylinder;
        private bool _inhibit;
		#endregion

		#region Costructors

		public TurnSection(TurnSectionIO io )
		{
			this.io = io;
			RotaryActuator actuator = new RotaryActuator(io.turnTo0Deg,io.turnTo90Deg,io.At0DegSensor,io.At90DegSensor);
			rotaryCylinder = actuator as IRotaryActuator;
			rotaryCylinder.CCWDirectionName = "To 90 Deg";
			rotaryCylinder.CWDirectionName = "To 0 Deg";
			rotaryCylinder.CCWStateName = "90 Deg";
			rotaryCylinder.CWStateName = "0 Deg";
			rotaryCylinder.Name = "Rotary";

									
		}
		#endregion

		#region Properties
		/// <summary>
		/// Name of the Turn Section.
		/// </summary>
		public IRotaryActuator RotaryCylinder
		{
			get { return rotaryCylinder;}
		}
		public string Name
		{
			get { return _name;}
			set { _name = value; }
		}
		/// <summary>
		/// This is for simulation mode.
		/// </summary>
		public bool Simulation
		{
			get { return _simulation;}
			set { _simulation = value; }
		}
        /// <summary>
        /// This is to check completed signal until time out.
        /// </summary>
		public uint TimeOut
		{
			get	{ return _timeout; }
			set { _timeout = value; }
		}
        public int EntranceRelay
        {
            set
            {
                if (value == 1) io.entranceRelay.Set(DigitalIOState.On);
                else io.entranceRelay.Set(DigitalIOState.Off); 
            }
        }
        public int ExitRelay
        {
            set
            {
                if (value == 1)
                {
                    io.exitRelay.Set(DigitalIOState.On);
                }
                else io.exitRelay.Set(DigitalIOState.Off);
            }
        }
        public int InhibitRelay
        {
            set
            {
                if (value == 1) 
                    io.inhibitRelay.Set(DigitalIOState.On);
                else 
                    io.inhibitRelay.Set(DigitalIOState.Off);
            }
        }

		#endregion


		//Methods
		/// <summary>
		/// Assign the I/O and intializes Others data.
		/// </summary>
		public virtual void Initialize()
		{
		}
		/// <summary>
		/// This is to check the movement completed signal.
		/// </summary>
		/// <returns>true:if movement competed else false</returns>
		public bool IsInPosition(DigitalIOState ioSate)
		{
			//if(_simulation)
            if (Simulation) return true;
				
			if(io.inPosition.Get() == ioSate)
				return true;
			else
				return false;
		}
        public bool IsEntrance(DigitalIOState ioSate)
        {
            //if(_simulation)
            if (Simulation) return true;

            if (io.entranceSensor.Get() == ioSate)
                return true;
            else
                return false;
        }
        public bool IsExit(DigitalIOState ioSate)
        {
            //if(_simulation)
            if (Simulation) return true;

            if (io.exitSensor.Get() == ioSate)
                return true;
            else
                return false;
        }
        public bool IsEntranceClear(DigitalIOState ioSate)
        {
            //if(_simulation)
            if (Simulation) return true;

            if (io.entranceClearSensor.Get() == ioSate)
                return true;
            else
                return false;
        }
        public bool IsExitClear(DigitalIOState ioSate)
        {
            //if(_simulation)
            if (Simulation) return true;

            if (io.exitClearSensor.Get() == ioSate)
                return true;
            else
                return false;
        }
        public bool IsZone(DigitalIOState ioSate)
        {
            //if(_simulation)
            if (Simulation) return true;

            if (io.zoneSensor.Get() == ioSate)
                return true;
            else
                return false;
        }
        public bool IsHome()
        {
            return (io.At0DegSensor.Get() == DigitalIOState.On);
        }

        public bool IsInhibit()
        {
            return _inhibit;

        }
   	}
}
