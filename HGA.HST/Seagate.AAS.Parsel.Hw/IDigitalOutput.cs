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
    /// Summary description for IIODigitalInput.
    /// </summary>
    public interface IDigitalOutput
    {
        // Properties ----------------------------------------------------------

        string Name 
        { get; set; }

        /// <summary>
        /// Returns the state the IO without talking to the hardware.
        /// </summary>
        DigitalIOState State
        { get; }

        /// <summary>
        /// The state that was set by the last Set function.
        /// </summary>
        DigitalIOState StateRequested
        { get; }

        // Methods -------------------------------------------------------------

        DigitalIOState Get();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        void Set(DigitalIOState state);
        
        /// <summary>
        /// Set the requested state without committing it to the hardware.
        /// </summary>
        /// <param name="state"></param>
        void SetRequest(DigitalIOState state);

        /// <summary>
        /// 
        /// </summary>
        void Toggle();
    }
}
