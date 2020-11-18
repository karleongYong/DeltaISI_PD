//
//  © Copyright 2006 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [June 2006 to present, Seagate HDA Automation Team]
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Seagate.AAS.Parsel.Device
{
	/// <summary>
	/// Summary description for IVacuumSwitch.
	/// </summary>
	public interface ISpinBoard
	{
		// operation
		void SpinUp();
		void SpinUpAsync(EventHandler OnDriveAtSpeed);
		void SpinDown();
		void SpinDownAsync(EventHandler OnDriveStopped);
		void Stop();
		void RunChannel(int channel);
		void WaitSpinUpComplete(uint msecTimeout);
		void WaitSpinDownComplete(uint msecTimeout);
		
		string Name				{ get; set; }
		bool   AtSpeed			{ get; }
		bool   Fault            { get; }
		double CurrentSpeed		{ get; }
		double SpinUpTime		{ get; }
		double SpinDownTime		{ get; }
	}
}
