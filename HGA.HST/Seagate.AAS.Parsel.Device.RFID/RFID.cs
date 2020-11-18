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
//  [2007/09/20] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Seagate.AAS.Parsel.Device.RFID
{
    public enum TagType
    {
        Unknown,
        HGA_FOLA = 1,
        HGA_BOLA = 2,
        //HGA_TRAY = 3,
        //HSA_FOLA = 4,
        //HSA_BOLA = 5,
        //HDA_TRAY = 6,
        HGA_BOLA2 = 7
        //SAA_TRAY = 8
    }

    public enum VendorCode
    {
        ALPS = '1',
        HEADWAY = '2',
        READRITE = '3',
        STRHO = '4',
        TDK = '5',
        TI_SSI = '1',
        PHILLIPS = '2',
        VTC = '3',
        NS = '4',
        IBM = '5',
        TI_FRIESING = '6',
        REWORK = 'X'

        // May not up to date , Somkiat 19 May 09
    }

    public enum PartStatus
    {
        Pass = 'A',         // from RFID spec
        Fail = 'S',
        Empty = 'B'
    }

    public enum RFHead
    {
        Head1 = 0,
        Head2 = 1,
        Count = 2
    }
    public enum NumberOfPart
    {
        FOLA_CARRIER = 10,
        BOLA_TRAY = 20
    }
}
