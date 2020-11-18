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
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Parsel.Hw
{
    public interface IVisionLight
    {
        void LightOn();
        void LightOff();

        double Intensity { get; set;}
        bool Enabled { get;set;}
        bool IsOn { get;}
        string Name { get;}
    }
}
