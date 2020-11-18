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
    // -------------------------------------------------------------------------

    public enum DigitalIOState
    {
        Unknown = -1,
        Off,
        On
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Summary description for IIODigitalInput.
    /// </summary>
    public interface IDigitalInput
    {
        // Properties ----------------------------------------------------------

        string Name 
        { get; set; }

        /// <summary>
        /// Returns the state the IO without talking to the hardware.
        /// </summary>
        DigitalIOState State
        {
            get;
        }

        // Methods -------------------------------------------------------------

        DigitalIOState Get();

        /// <summary>
        /// Puts the caller into wait state until the state is changed to the 
        /// target state, or the timeout has reached.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="nTimeOut"></param>
        void WaitForState(DigitalIOState stateTarget, uint nTimeOut);
    }
}
