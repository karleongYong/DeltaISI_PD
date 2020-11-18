using System;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Device
{
	public enum RotaryActuatorState
	{
		Unknown,
		Clockwise,
		CounterClockwise
	}
	/// <summary>
	/// Summary description for IRotaryActuator.
	/// </summary>
	public interface IRotaryActuator
	{
		void TurnCW(uint msecTimeout);
        void TurnCW(uint msecTimeout, out uint msecTimeUsed);
		void TurnCCW(uint msecTimeout);
        void TurnCCW(uint msecTimeout, out uint msecTimeUsed);

		void StartTurnCW();
		void StartTurnCCW();
		void WaitForCW(uint msecTimeout);
        void WaitForCW(uint msecTimeout, out uint msecTimeUsed);
		void WaitForCCW(uint msecTimeout);
        void WaitForCCW(uint msecTimeout, out uint msecTimeUsed);
		void WaitForStates(DigitalIOState CWSensorState, DigitalIOState CCWSensorState, uint msecTimeout);
		
		RotaryActuatorState State{ get; }
		
		DigitalIOState CWSensorState{ get; }
		DigitalIOState CCWSensorState{ get; }

		string Name { get; set; }
		string CWDirectionName  { get; set; }
		string CCWDirectionName  { get; set; }
		string CWStateName  { get; set; }
		string CCWStateName { get; set; }

	}
}
