//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/09/28] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Parsel.Device.RFID.HSA
{
    public enum HsaStationCode
    {
        NEWPALLET = 'N',             // Predined station codes
        EBLOCK_LOAD = 'E',
        HGA_LOAD = 'H',
        SWAGE = 'S',
        FOS_PREP = 'F',
        REFLOW_SOLDER = 'R',
        DESHUNT = 'D',
        COMSAT = 'C',
        HSA_UNLOAD = 'U',
        BEARING_INSTALL = 'B',
        GRAMLOAD = 'G',
        OCR_OHA = 'O',
        PSA_RSA = 'P',
        VMI = 'V'
    }
}
