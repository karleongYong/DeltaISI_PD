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
//  [9/2/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// IOManifest contains maps of IO usage
	/// </summary>
	public abstract class IOManifest
	{
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        public Hashtable axisMap           = new Hashtable();
        public Hashtable axisGroupMap      = new Hashtable();
        public Hashtable digitalInputMap   = new Hashtable();
        public Hashtable digitalOutputMap  = new Hashtable();
        public Hashtable analogInputMap    = new Hashtable();
        public Hashtable analogOutputMap   = new Hashtable();
        public Hashtable cameraMap         = new Hashtable();
        public Hashtable conveyorZoneMap   = new Hashtable();
        public Hashtable conveyorStepperMap= new Hashtable() ;

        // Constructors & Finalizers -------------------------------------------
        
        public IOManifest()
        {
        }

        public IOManifest(HwSystem hwSystem)
        {
            if (hwSystem != null)
                hwSystem.RegisterIOManifest(this);
        }

        // Properties ----------------------------------------------------------
        
        // Methods -------------------------------------------------------------
        
        public abstract void RegisterIOs();

        public void RegisterDigitalInput(IDigitalInput digIn, int registrationID, string name)
        {
            if (digIn == null)
            {
                throw new Exception(string.Format("The digital input '{0}' is invalid(null)!", name));
            }

            if (digitalInputMap.Contains(registrationID))
            {
                throw new Exception("The IO is already registered!");
            }

            digIn.Name = name;
            digitalInputMap.Add(registrationID, digIn);
        }

        public void RegisterDigitalOutput(IDigitalOutput digOut, int registrationID, string name)
        {
            if (digOut == null)
            {
                throw new Exception(string.Format("The digital output '{0}' is invalid(null)!", name));
            }

            if (digitalOutputMap.Contains(registrationID))
            {
                throw new Exception("The IO is already registered!");
            }

            digOut.Name = name;
            digitalOutputMap.Add(registrationID, digOut);
        }

        public void RegisterAnalogInput(IAnalogInput analogIn, int registrationID, string name)
        {
            if (analogIn == null)
            {
                throw new Exception(string.Format("The analog input '{0}' is invalid(null)!", name));
            }

            if (analogInputMap.Contains(registrationID))
            {
                throw new Exception("The IO is already registered!");
            }

            analogIn.Name = name;
            analogInputMap.Add(registrationID, analogIn);
        }

        public void RegisterAnalogOutput(IAnalogOutput analogOut, int registrationID, string name)
        {
            if (analogOut == null)
            {
                throw new Exception(string.Format("The analog output '{0}' is invalid(null)!", name));
            }

            if (analogOutputMap.Contains(registrationID))
            {
                throw new Exception("The IO is already registered!");
            }

            analogOut.Name = name;
            analogOutputMap.Add(registrationID, analogOut);
        }

        public void RegisterAxis(IAxis axis, int registrationID, string name)
        {
            if (axis == null)
            {
                throw new Exception(string.Format("The axis '{0}' is invalid(null)!", name));
            }

            if (axisMap.Contains(registrationID))
            {
                throw new Exception("The axis is already registered!");
            }

            axis.Name = name;
            axisMap.Add(registrationID, axis);
        }

        public void RegisterAxisGroup(IAxisGroup axes, int[] registrationID, string name)
        {
            if (axes == null)
            {
                throw new Exception(string.Format("The axis '{0}' is invalid(null)!", name));
            }

            if (axisGroupMap.Contains(registrationID))
            {
                throw new Exception("The axis is already registered!");
            }

            axisGroupMap.Add(name, axes);
        }

        public void RegisterCamera(ICamera camera, int registrationID, string name)
        {
            if (camera == null)
            {
                throw new Exception(string.Format("The camera '{0}' is invalid(null)!", name));
            }

            if (cameraMap.Contains(registrationID))
            {
                throw new Exception("The camera is already registered!");
            }

            camera.Name = name;
            cameraMap.Add(registrationID, camera);
        }

        
        public void RegisterConveyorZone(IConveyorZone conveyorZone, int registrationID, string name)
        {
            if (conveyorZone == null)
            {
                throw new Exception(string.Format("The conveyor zone '{0}' is invalid(null)!", name));
            }

            if (conveyorZoneMap.Contains(registrationID))
            {
                throw new Exception("The IO is already registered!");
            }

            conveyorZone.Name = name;
            conveyorZoneMap.Add(registrationID, conveyorZone);
        }

        public void RegisterConveyorStepper(IConveyorStepper conveyorStepper, int registrationID, string name)
        {
            if (conveyorStepper == null)
            {
                throw new Exception(string.Format("The conveyor stepper '{0}' is invalid(null)!", name));
            }

            if (conveyorStepperMap.Contains(registrationID))
            {
                throw new Exception("The IO is already registered!");
            }

            conveyorStepper.Name = name;
            conveyorStepperMap.Add(registrationID, conveyorStepper);
        }        
        


        public IAxis GetAxis(int registrationID)
        {
            return (IAxis) axisMap[registrationID];
        }

        public IAxisGroup GetAxisGroup(int[] registrationID)
        {
            return (IAxisGroup)axisGroupMap[registrationID];
        }

        public IAxisGroup GetAxisGroup(string groupName)
        {
            return (IAxisGroup)axisGroupMap[groupName];
        }

        public IDigitalInput GetDigitalInput(int registrationID)
        {
            return (IDigitalInput) digitalInputMap[registrationID];
        }

        public IDigitalOutput GetDigitalOutput(int registrationID)
        {
            return (IDigitalOutput) digitalOutputMap[registrationID];
        }

        public IAnalogInput GetAnalogInput(int registrationID)
        {
            return (IAnalogInput) analogInputMap[registrationID];
        }

        public IAnalogOutput GetAnalogOutput(int registrationID)
        {
            return (IAnalogOutput) analogOutputMap[registrationID];
        }

        public ICamera GetCamera(int registrationID)
        {
            return (ICamera) cameraMap[registrationID];
        }

        public IConveyorZone GetConveyorZone(int registrationID)
        {
            return (IConveyorZone) conveyorZoneMap[registrationID];
        }

        public IConveyorStepper GetConveyorStepper(int registrationID)
        {
            return (IConveyorStepper) conveyorStepperMap[registrationID];
        }

        // Internal methods ----------------------------------------------------
	};  
}

