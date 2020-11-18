using System;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Device
{
	public enum LinearActuatorState
	{
		Unknown,
		Extended,
		Retracted
	}
	/// <summary>
	/// Summary description for ILinearActuator.
	/// </summary>
	public interface ILinearActuator
	{
		void Extend(uint msecTimeout);
        void Extend(uint msecTimeout, out uint msecTimeUsed);
		void Retract(uint msecTimeout);
        void Retract(uint msecTimeout, out uint msecTimeUsed);

		void StartExtend();
		void StartRetract();
		void WaitForExtend(uint msecTimeout);
        void WaitForExtend(uint msecTimeout, out uint msecTimeUsed);
		void WaitForRetract(uint msecTimeout);
        void WaitForRetract(uint msecTimeout, out uint msecTimeUsed);
		void WaitForStates(DigitalIOState ExtSensorState, DigitalIOState RetSensorState, uint msecTimeout);
		
        LinearActuatorState State{ get; }
        LinearActuatorState OutputState{ get; }
		
		DigitalIOState ExtendedSensorState{ get; }
		DigitalIOState RetractedSensorState{ get; }

		string Name { get; set; }
		string ExtendedDirectionName { get; set; }
		string RetractedDirectionName  { get; set; }
		string ExtendedStateName { get; set; }
		string RetractedStateName { get; set; }

	}
}
