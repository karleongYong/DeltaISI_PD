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
//  [9/5/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Seagate.AAS.Parsel.Services
{
    /// <summary>
    /// This interface must be implemented by all services.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// This method is called after the services are loaded.
        /// </summary>
        void InitializeService();
		
        /// <summary>
        /// This method is called before the service is unloaded.
        /// </summary>
        void UnloadService();
    }
}
