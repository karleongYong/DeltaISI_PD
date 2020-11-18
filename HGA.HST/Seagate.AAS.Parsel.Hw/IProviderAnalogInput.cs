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
//  [8/21/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for IProviderAnalogInput.
	/// </summary>
	public interface IProviderAnalogInput
	{
        // Methods -------------------------------------------------------------
	
        double GetRawValue(IAnalogInput analogIn);
        void WaitForAboveThreshold(IAnalogInput analogIn, double threshold, int timeOut);  // timeout in msec
        void WaitForBelowThreshold(IAnalogInput analogIn, double threshold, int timeOut);  // timeout in msec
        void WaitForInRange(IAnalogInput analogIn, double target, double tolerance, int timeOut);  // timeout in msec
    }
}
