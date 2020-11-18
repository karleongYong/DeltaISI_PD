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

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for IOException.
	/// </summary>
	public class IOException : HardwareException
	{
		public IOException()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public IOException(string description) : base(description)
        {
        }

        public IOException(string description, Exception exception) : base(description, exception)
        {
        }
	}
}
