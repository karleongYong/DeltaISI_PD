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
	public enum ConveyorPort
	{
		Upstream,
		Downstream, 
		Both
	}	

	/// <summary>
	/// Summary description for IConveyorZone.
	/// </summary>
	public interface IConveyorZone
	{    
        // Properties ----------------------------------------------------------
		string Name { get; set; }
		string Unit { get; }
		bool IsRunning { get; set; }
		bool Direction { get; set; }
		bool IsEnabled { get; }
		bool Inhibit { get; set; }
		bool Release { get; set; }
		bool ServoMode { get; set; }
		bool IsPalletInZone { get; }
		bool IsPalletInInterZone { get; }		
		double Position { get; }
		bool ResetEncoderOnIndex { get; set; }
		bool InMotion { get; }
		bool MoveCompleted { get; }
		int ZPosition { get; }
		int PalletId { get; }
		double MillimetersAfterZone { get; set; }

		// Methods --------------------------------------------------------------
		void SetUnit(string unitName, double countsPerUnit, double stepsPerUnit);
		void GetNextPallet();
		void MoveRel(double position);
        void ServoMoveAbs(double position, double tolerance, int timeOut, out int actualTimeUsed);
        void ServoMoveAbs(double position, double tolerance, int timeout, int retryCount, out int actualTimeUsed, out int actualRetry, out string logMessage);
		void ServoMoveAbsStart(double position, double tolerance);
		void ServoMoveRelStart(double distance, double tolerance);
        void TurnOffServoMode();
		void ReturnPallets(int returnZone, int timeOut);
		void ReturnPalletsReset(int returnZone, int timeOut);
		int WaitNextPallet(int timeOut);
		int WaitNextPallet(double position, int timeOut);
		int WaitNextPallet(double position, double tolerance, int timeOut);
		void WaitServoMoveDone(int timeOut); 	
		bool GetPortEnable(ConveyorPort port);
		void SetPortEnable(ConveyorPort port, bool state);
		void PulseOutput(byte width);
        void JogToZoneSensor(double velocity);
        void JogToInterZoneSensor(double velocity);
	}
}
