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
	/// <summary>
	/// Summary description for IAnalogInput.
	/// </summary>
	public interface IAnalogInput
	{
        // Properties ----------------------------------------------------------

        string Name 
        { get; set; }

        double Scale
        { get; set; }

        double Offset
        { get; set; }

        double Value
        { get; }

        double ValueRaw
        { get; }

        string Unit
        { get; }

        // Methods -------------------------------------------------------------

        double Get();  // gets scaled value

        /// <summary>
        /// Establishes  factors to convert raw analog input to a scaled value.  ScaledValue = RawValue * scale + offset
        /// </summary>
        /// <param name="unitName">Unit name of the scaled value</param>
        /// <param name="scale">Scaling factor multiplied with rawValue [in units of unitName/cnt]</param>
        /// <param name="offset">Number added to mulitplied result [in units of 'unitName'].</param>
        void SetUnit(string unitName, double scale, double offset);

        void WaitForAboveThreshold(double threshold, uint timeOut);  // timeout in msec
        void WaitForBelowThreshold(double threshold, uint timeOut);  // timeout in msec
        void WaitForInRange(double dLowThrs, double dHiThrs, uint timeOut);
        void WaitForOutRange(double dLowThrs, double dHiThrs, uint timeOut);
        void WaitForTarget(double dTarget, double dSymTolerance, uint timeOut);
    }
}
