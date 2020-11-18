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
	/// <summary>
	/// Summary description for IConveyorZone.
	/// </summary>
	public interface IConveyorSection
	{    
        // Properties ----------------------------------------------------------
		string Name { get; set; }
		bool IsRunning { get; set; }
		bool IsForward { get; set; }
		bool IsEnabled { get; }

		// Methods --------------------------------------------------------------
		void FreeAll();
	}
}
