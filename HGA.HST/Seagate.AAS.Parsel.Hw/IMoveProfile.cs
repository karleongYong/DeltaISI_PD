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

    public enum MoveType
    {
        Trap,        
        Vel,
		Undefined,
		SCurve,
    }

	/// <summary>
	/// IMoveProfile. Assumes units of assigned axis.
	/// </summary>
	public interface IMoveProfile
	{
		MoveType MoveType
		{ get; set; }

		double Jerk
		{ get; set; }
		
		double Acceleration
        { get; set; }
        
        double Deceleration
        { get; set; }

        double Velocity
        { get; set; }

        double SettlingWindow
        { get; set; }

        short SettlingTime
        { get; set; }

		double FollowingError
		{ get; set; }
	}
}
