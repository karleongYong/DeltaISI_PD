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
//  [8/25/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Seagate.AAS.Parsel.Hw
{
	public enum TrayFeederDirection
	{
		CW,
		CCW
	}	
	
	/// <summary>
	/// Summary description for IConveyorStepper
	/// </summary>
	public interface IConveyorStepper
	{    
		// Properties ----------------------------------------------------------
		string Name { get; set; }
		string Unit { get; }
		bool IsRunning { get; set; }
		bool IsEnabled { get; }
		bool ServoMode { get; set; }
		bool IsTrayInZone { get; }
		bool IsTrayInInterZone { get; }	
		bool IsTrayInEncoder { get; }
		double Position { get; }
		bool ResetEncoderOnIndex { get; set; }
		int ZPosition { get; }
		int PalletId { get; }	

		// Methods --------------------------------------------------------------
		void SetUnit(string unitName, double countsPerUnit, double stepsPerUnit);
		void GetNextTray(double position, TrayFeederDirection direction);
		int  EncoderTest(TrayFeederDirection direction);
        void MoveRel(double position);
        void ServoMoveAbsStart(double position, double tolerance);
		void ServoMoveRelStart(double distance, double tolerance);        
		int WaitNextTray(int timeOut);
		int WaitNextTray(double position, int timeOut);
		int WaitNextTray(double position, double tolerance, int timeOut);
		void WaitTrayMoveDone(int timeOut); 
		void Stop();
	}
}

