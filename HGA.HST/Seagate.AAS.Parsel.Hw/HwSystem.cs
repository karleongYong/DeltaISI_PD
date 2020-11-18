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
//  [9/8/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using System.Collections;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for HwSystem.
	/// </summary>
	public abstract class HwSystem
	{
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        
        public Hashtable       hwComponentMap  = new Hashtable();
        public ArrayList       ioManifestList  = new ArrayList();
        
        // Constructors & Finalizers -------------------------------------------
        
        public HwSystem()
        {
        }
        
        // Properties ----------------------------------------------------------

        // Abstract methods -----------------------------------------------------
        
        /// <summary>
        /// 
        /// </summary>
        public abstract void RegisterHwComponents();
        
        /// <summary>
        /// 
        /// </summary>
        public abstract void InitializeHwComponents();

        // Methods -------------------------------------------------------------

        public void Initialize()
        {
            RegisterHwComponents();

            foreach (IOManifest manifest in ioManifestList) 
            {
                try
                {
                    manifest.RegisterIOs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error in IOManifest.RegisterIO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }     
       
            InitializeHwComponents();
        }

        public void Terminate()
        {
            // clear IO manifests
            foreach (IHardwareComponent hc in hwComponentMap.Values )
            {
                hc.ShutDown();
            }
        }

        // registration methods

        public void RegisterHwComponent(IHardwareComponent hwComp, int registrationID, string name)
        {
            if (hwComp == null)
            {
                throw new Exception("The hardware component must be valid!");
            }

            if (hwComponentMap.Contains(registrationID))
            {
                throw new Exception("The hardware component is already registered!");
            }

            hwComp.Name = name;
            hwComponentMap.Add(registrationID, hwComp);
        }        
        
        public void RegisterIOManifest(IOManifest manifest)
        {
            if (ioManifestList.Contains(manifest))
            {
                throw new Exception("The manifest is already registered!");
            }

            ioManifestList.Add(manifest);
        }        

        public IHardwareComponent GetHwComponent(int registrationID)
        {
            return hwComponentMap[registrationID] as IHardwareComponent;
        }

        // Internal methods ----------------------------------------------------

    }
}
