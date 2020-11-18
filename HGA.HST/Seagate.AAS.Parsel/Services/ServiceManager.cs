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
using System.Collections;
using System.Collections.Generic;

namespace Seagate.AAS.Parsel.Services
{
    /// <summary>
    /// This class does basic service handling for you.
    /// </summary>
    public class ServiceManager
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        private static ServiceManager _instance = new ServiceManager();
        private Dictionary<Type, IService> _servicesHashtable = new Dictionary<Type, IService>();
        
        // Constructors & Finalizers -------------------------------------------

        /// <summary>
        /// Don't create ServiceManager objects, only have ONE per application.
        /// </summary>
        private ServiceManager()
        {
        }        
        
        // Properties ----------------------------------------------------------
        
        public static AppConfigService AppConfigService
        { get { return (AppConfigService)_instance._servicesHashtable[typeof(AppConfigService)]; } }

        public static Messaging Messaging
        { get { return (Messaging)_instance._servicesHashtable[typeof(Messaging)]; } }

        public static DirectoryLocator DirectoryLocator
        { get { return (DirectoryLocator)_instance._servicesHashtable[typeof(DirectoryLocator)]; } }

        public static MenuNavigator MenuNavigator
        { get { return (MenuNavigator)_instance._servicesHashtable[typeof(MenuNavigator)]; } }

        public static ErrorHandler ErrorHandler
        { get { return (ErrorHandler)_instance._servicesHashtable[typeof(ErrorHandler)]; } }

		public static Tracing Tracing
		{ 
			get 
			{
				try { return (Tracing)_instance._servicesHashtable[typeof(Tracing)]; }
				catch { return null; }
			}
		}

        public static FormLayout FormLayout
        { get { return (FormLayout)_instance._servicesHashtable[typeof(FormLayout)]; } }


        // Interface methods ---------------------------------------------------

        public static void Initialize()
        { _instance._Initialize(); }

        public static void RegisterService(Type serviceType, IService service)
        { _instance._RegisterService(serviceType, service); }

        public static void UnregisterService(Type serviceType)
        { _instance._UnregisterService(serviceType); }
        
        public static IService GetService(Type serviceType)
        { return _instance._GetService(serviceType); }
        
        // Internal methods ----------------------------------------------------
        
        private void _Initialize()
        {
            // register core services
			RegisterService(typeof(Tracing), new Tracing());	// register Tracing service first so other services can use it!!!
			RegisterService(typeof(AppConfigService), new AppConfigService());
            RegisterService(typeof(Messaging), new Messaging());
            RegisterService(typeof(DirectoryLocator), new DirectoryLocator());
            RegisterService(typeof(MenuNavigator), new MenuNavigator());
            RegisterService(typeof(ErrorHandler), new ErrorHandler());
            RegisterService(typeof(FormLayout), new FormLayout());
        }

        private void _RegisterService(Type serviceType, IService service)
        {
            if (_servicesHashtable.ContainsKey(serviceType))
            {
                throw new System.ArgumentException("The service type is already registered");
            }
            else
            {
                service.InitializeService();
                _servicesHashtable.Add(serviceType, service);
            }
        }

        private void _UnregisterService(Type serviceType)
        {
            if (_servicesHashtable.ContainsKey(serviceType))
            {
                _servicesHashtable[serviceType].UnloadService();
                _servicesHashtable.Remove(serviceType);
            }
        }		
        
        /// <remarks>
        /// Requests a specific service, may return null if this service is not found.
        /// </remarks>
        private IService _GetService(Type serviceType)
        {
            IService serviceIface = (IService)_servicesHashtable[serviceType];
            return serviceIface;
        }
        
    }
}
