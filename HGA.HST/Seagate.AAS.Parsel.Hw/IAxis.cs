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
//  [8/20/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Seagate.AAS.Parsel.Hw
{
//	]
	/// <summary>
    /// 
    /// </summary>
    public interface IAxis
    {
        // Properties ----------------------------------------------------------
        string Name 
        { get; set; }

        bool IsHomed
        { get; }

        bool IsEnabled
        { get; }

        bool IsMoveDone
        { get; }

        double CountsPerUnit
        { get; }
        
        string Unit
        { get; }

		double PeakCurrentLimit   // amps
		{ get; set; }

		int PeakCurrentTime   // msec
		{ get; set; }

		double PositionErrorWindow
		{ get; set; }

		double VelocityActual   
		{ get; }

		double MotorCurrent   // amps
		{ get; }
		
		// Methods -------------------------------------------------------------
		double  GetActualPosition();
        double  GetCommandPosition();
		bool    IsInRange(double position, double range);
        
        void    Enable(bool bEnable);
        void    Stop();
        void    SetUnit(string unitName, double countsPerUnit);
        void    SetSecondaryUnit(string unitName, double countsPerUnit);
		
		IMoveProfile GetMoveProfile();
		void SetMoveProfile(IMoveProfile moveProfile);

        // blocking calls
        void MoveAbsolute(IMoveProfile moveProfile, double position, uint msecTimeOut);
		void MoveRelative(IMoveProfile moveProfile, double position, uint msecTimeOut);
		void MoveAbsolute(double acceleration, double velocity, double position, uint msecTimeOut);
        void MoveRelative(double acceleration, double velocity, double positionRelative, uint msecTimeOut);
        void WaitMoveDone(uint msecTimeOut); 
		void WaitPositionAboveThreshold(double threshold, uint msecTimeout);
		void WaitPositionBelowThreshold(double threshold, uint msecTimeout);
        void Home(uint msecTimeOut );		

        // non-blocking calls
		void MoveAbsoluteStart(IMoveProfile moveProfile, double position);
		void MoveRelativeStart(IMoveProfile moveProfile, double position);
        void MoveAbsoluteStart(double acceleration, double velocity, double position);
        void MoveRelativeStart(double acceleration, double velocity, double positionRelative);
		void MoveVelocity(double acceleration, double velocity, double position);
   		void HomeStart();
        void HomeAsync(uint msecTimeOut);
	}
}
