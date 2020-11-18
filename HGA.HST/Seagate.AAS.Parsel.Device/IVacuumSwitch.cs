//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [08/04/1997] -- http://www.uselessmoviequotes.com/umq_t003.htm
//
////////////////////////////////////////////////////////////////////////////////

using System;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Device
{
	/// <summary>
	/// Summary description for IVacuumSwitch.
	/// </summary>
	public interface IVacuumSwitch
	{
		// operation
		void SwitchVacuum(DigitalIOState state);
		void WaitForOn();
		void WaitForOff();
		bool VacIsOn			{ get; }
		DigitalIOState	State	{ get; }
		double SensorValue		{ get; }
		uint Timeout			{ set; }

		// setup & calibration
		void   InitiateAutoCalibration();
		double DelayAfterOff		{ get; set; }
		double VacLevelWithPart		{ get; set; }
		double VacLevelWithoutPart	{ get; set;	}
		string Name					{ get; set; }
		string Unit					{ get; }
	}
}
