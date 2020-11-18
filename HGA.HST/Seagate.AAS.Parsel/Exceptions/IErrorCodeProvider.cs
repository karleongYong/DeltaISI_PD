/////////////////////////////////////////////////////////////////////////////
//
//  (c) Copyright 2008 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2008/05/02] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Parsel
{
    public interface IErrorCodeProvider
    {
        // Properties ---------------------------------
        string ErrorCode { get; }    // Error code
        bool Log { get; }

    }
}
