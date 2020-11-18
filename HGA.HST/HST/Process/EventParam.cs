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
//  [2009/09/29] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using Seagate.AAS.Parsel.Services;

namespace Seagate.AAS.HGA.HST.Process
{
    public class EventParam
    {
        // Nested declarations -------------------------------------------------


        // Member variables ----------------------------------------------------
        private ErrorButton _errorResponse;
        private string _sigConsumer;        // state that consumes the signal (SigStateJob signal)

        // Constructors & Finalizers -------------------------------------------
        public EventParam()
        {
        }

        public EventParam(string stateJobConsumer)
        {
            _sigConsumer = stateJobConsumer;
        }

        public EventParam(ErrorButton errorResponse)
        {
            _errorResponse = errorResponse;
        }

        public EventParam(string sigConsumer, ErrorButton errorResponse)
        {
            _errorResponse = errorResponse;
            _sigConsumer = sigConsumer;
        }

        // Properties ----------------------------------------------------------
        public ErrorButton ErrorResponse { get { return _errorResponse; } set { _errorResponse = value; } }
        public string Consumer { get { return _sigConsumer; } set { _sigConsumer = value; } }

        // Methods -------------------------------------------------------------


        // Event handlers ------------------------------------------------------


        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------

    }
}
