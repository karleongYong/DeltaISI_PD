
using System;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Device
{
	/// <summary>
	/// Summary description for ITurnSection.
	/// </summary>
	public interface ITurnSection
	{
		//Properties------------------------------------------------------------
		/// <summary>
		/// Name of the Turn Section.
		/// </summary>
		string Name{get;set;}
        int EntranceRelay { set;}
        int ExitRelay { set;}
        int InhibitRelay { set; }
		IRotaryActuator RotaryCylinder{get;}
		/// <summary>
		/// This is for simulation mode.
		/// </summary>
		bool Simulation{get;set;}
        /// <summary>
        /// This is to check completed signal until time out.
        /// </summary>
		uint TimeOut{get;set;}
		//Methods---------------------------------------------------------------
		/// <summary>
		/// Assign the I/O and intializes Others data.
		/// </summary>
		void Initialize();
		bool IsInPosition(DigitalIOState ioSate);
        bool IsEntrance(DigitalIOState ioSate);
        bool IsExit(DigitalIOState ioSate);
        bool IsEntranceClear(DigitalIOState ioSate);
        bool IsExitClear(DigitalIOState ioSate);
        bool IsZone(DigitalIOState ioSate);
        bool IsHome();
        bool IsInhibit();
   	}
}
