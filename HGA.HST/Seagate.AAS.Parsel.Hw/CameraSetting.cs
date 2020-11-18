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
//  [2009/03/04] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////
			
using System;
using System.Collections.Generic;
using System.Text;

using Seagate.AAS.Utils;

namespace Seagate.AAS.Parsel.Hw
{
    [Serializable]
    public class CameraSetting
    {

        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        private String _name = "NA";
        private double _exposure;
        private double _contrast;
        private double _brightness;
        private double _ringLightIntensity;
        private double _thruLightIntensity;

        private CameraImageGraphics _imageGraphics = new CameraImageGraphics();

        // Constructors & Finalizers -------------------------------------------
        public CameraSetting()
        {
        }

        public CameraSetting(String name)
        {
            _name = name; 
        }

        // Properties ----------------------------------------------------------
        public String Name                   { get { return _name; } set { _name = value; } }// Choo: add set accessor.
        public double Exposure           { get { return _exposure; } set { _exposure = value; } }
        public double Contrast           { get { return _contrast; } set { _contrast = value; } }
        public double Brightness         { get { return _brightness; } set { _brightness = value; } }
        public double RingLightIntensity { get { return _ringLightIntensity; } set { _ringLightIntensity = value; } }
        public double ThruLightIntensity { get { return _thruLightIntensity; } set { _thruLightIntensity = value; } }

        public CameraImageGraphics ImageGraphics { get { return _imageGraphics; } set { _imageGraphics = value; } }

        // Methods -------------------------------------------------------------

        /// <summary>
        /// Deep copy
        /// </summary>
        /// <param name="setting"></param>
        public void Copy(CameraSetting setting)
        {
            _name       = setting._name;
            _exposure   = setting._exposure;
            _contrast   = setting._contrast;
            _brightness = setting._brightness;
            _ringLightIntensity = setting._ringLightIntensity;
            _thruLightIntensity = setting._thruLightIntensity;
            _imageGraphics.Copy(setting._imageGraphics);
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/" + _name);
            _exposure   = xml.Read("Exposure", 40.0);
            _brightness = xml.Read("Brightness", 0.5);
            _contrast   = xml.Read("Contrast", 0.5);
            _ringLightIntensity = xml.Read("RingLightIntensity", 0.0);
            _thruLightIntensity = xml.Read("ThruLightIntensity", 0.0);
            xml.CloseSection();

            _imageGraphics.Load(section + "/" + _name, xml);
        }

        /// <summary>
        /// Saves to an xml obejct
        /// </summary>
        /// <param name="section"></param>
        /// <param name="xml"></param>
        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/" + _name);
            xml.Write("Exposure", _exposure);
            xml.Write("Brightness", _brightness);
            xml.Write("Contrast", _contrast);
            xml.Write("RingLightIntensity", _ringLightIntensity);
            xml.Write("ThruLightIntensity", _thruLightIntensity);
            xml.CloseSection();

            _imageGraphics.Save(section + "/" + _name, xml);
        }

        // Event handlers ------------------------------------------------------


        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------
			
    }
}
