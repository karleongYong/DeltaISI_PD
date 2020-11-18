//
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
//  [9/22/2005]
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Seagate.AAS.Parsel
{
	/// <summary>
	/// Summary description for ParselException.
	/// </summary>
	public class ParselException : System.ApplicationException
	{
        // Nested declarations -------------------------------------------------

        public enum Severity
        {
            None,
            Error,
            Warning
        }

        // Member variables ----------------------------------------------------
        
        protected Severity severity = Severity.None;

        // Constructors & Finalizers -------------------------------------------

        public ParselException()
        { }
        public ParselException(string msg)
            : base(msg)
        { }
        public ParselException(string msg, Exception ex)
            : base(msg, ex)
        { }

        // Properties ----------------------------------------------------------
        
        public Severity Level
        {
            get { return severity; }
        }

        // Methods -------------------------------------------------------------
        
        // Event handlers ------------------------------------------------------
        
        // Internal methods ----------------------------------------------------
        
    }
}
