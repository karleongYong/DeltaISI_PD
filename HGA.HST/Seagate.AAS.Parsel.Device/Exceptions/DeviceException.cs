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
//  [9/15/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Device
{
	/// <summary>
	/// Summary description for DeviceException.
	/// </summary>
	public class DeviceException : HardwareException
	{
		public DeviceException()
		{
		}

        public DeviceException(string description) : base(description)
        {
        }

        public DeviceException(string description, Exception exception) : base(description, exception)
        {
        }
	}
}
