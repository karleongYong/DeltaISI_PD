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
    /// Summary description for IAnalogOutput.
    /// </summary>
    public interface IAnalogOutput
    {
        // Properties ----------------------------------------------------------

        string Name 
        { get; set; }

        double LimitMin
        { get; set; }

        double LimitMax
        { get; set; }

        double Scale
        { get; set; }

        double Offset
        { get; set; }

        /// <summary>
        /// Returns the Value the IO without talking to the hardware.
        /// </summary>
        double Value
        { get; }

        /// <summary>
        /// The Value that was set by the last Set function.
        /// </summary>
        double ValueRequested
        { get; }

        double ValueRequestedRaw
        { get; }

        string Unit
        { get; }


        // Methods -------------------------------------------------------------

        double Get();

        void SetUnit(string unitName, double scale, double offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        void Set(double outputValue);
        
        /// <summary>
        /// Set the requested value without committing it to the hardware.
        /// </summary>
        /// <param name="state"></param>
        void SetRequest(double outputValue);
    }
}
