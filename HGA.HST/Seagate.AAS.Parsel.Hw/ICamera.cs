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
//  [8/21/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Seagate.AAS.Parsel.Hw
{
    public enum CameraPixelType
    {
        Undefined = 0,
        GreyScale8bit,
        GreyScale16bit,
        RGB16bit,
        RGB24bit,
        Plane3
    }

    /// <summary>
    /// Summary description for ICamera.
    /// </summary>
    public interface ICamera
    {

        // event
        event EventHandler OnStartAcquire;

		// Properties ----------------------------------------------------------
		
		string Name
		{ get; set; }
		
		string SerialNumber
		{ get; }
        
        int Port
        { get; }

		double Exposure
		{ get; set; }

		double Brightness
		{ get; set; }

		double Contrast
		{ get; set; }

		bool Initialized
		{ get; }

		bool Registered
		{ get; }
		
		// Methods -------------------------------------------------------------

		object Acquire(int millisecTimeout);
        void SaveImage(string pathFileName);
    }
}
