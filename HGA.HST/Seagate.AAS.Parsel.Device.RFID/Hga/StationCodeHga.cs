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

namespace Seagate.AAS.Parsel.Device.RFID.Hga
{
    public enum StationCodeHga
    {
        SUSPENSION_LOAD = 'A',       // Predined station codes
        ADHESIVE_DISPENSE = 'B',
        SLIDER_ATTACH = 'C',
        BALL_BOND = 'D',
        IAT_ARM_LOAD = 'E',
        PRELOAD = 'F',
        SAAM = 'G',
        FLY_TEST = 'H',
        ELECTTRICAL_TEST = 'I',
        SORT = 'J',
        ALT_SLIDER_ATTACH = 'K',
        AUTO_VIC = 'L',
        MANUAL_VIC = 'M',
        AUTO_PRELOAD = 'N',
        AUTO_SHUNT = 'O',
        AUTO_SAAM = 'P',
        SUSPENSION_UNLOAD = 'Q',
        IR_OVEN = 'S'
    }
}
