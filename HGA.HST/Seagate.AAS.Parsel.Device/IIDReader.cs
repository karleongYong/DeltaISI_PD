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
//  [9/17/2013]
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Seagate.AAS.Parsel.Device
{
	/// <summary>
	/// Summary description for IDReader.
	/// </summary>
	public interface IIDReader
	{
        // Properties ----------------------------------------------------------

        string Name
        { get; set; }

        // Methods -------------------------------------------------------------

        string ReadID();
        string ReadID(uint msecTimeOut);
	}
}
