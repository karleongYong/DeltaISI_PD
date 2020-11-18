
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
    public class CameraImageGraphics
    {
        // Nested declarations -------------------------------------------------
        public enum MarkerType
        {
            None,
            Rectangle,
            Circle
        }

        // Member variables ----------------------------------------------------
        private const double _defaultMarkerWidth = 1;   // in mm
        private const double _defatulMarkerHeight = 1;  // in mm

        private const double _defaultLineStartX = 0;    // mm
        private const double _defaultLineStartY = 0;    // mm
        private const double _defaultLineEndX = 1;      // in mm
        private const double _defaultLineEndY = 1;      // mm

        private const double _defaultHatchSize = 0.020;    // in mm
        private const double _defaultHatchIncrement = 0.1; // in mm

        private MarkerType _markerType = 0;
        private double _markerWidth = _defaultMarkerWidth;
        private double _markerHeight = _defatulMarkerHeight;

        private double _distMeasureLineStartX = _defaultLineStartX;
        private double _distMeasureLineStartY = _defaultLineStartY;
        private double _distMeasureLineEndX = _defaultLineEndX;
        private double _distMeasureLineEndY = _defaultLineEndY;
        private bool _showMeasurementLine = false;

        private double _xyAxesHatchSize = _defaultHatchSize;
        private double _xyAxesHatchInc = _defaultHatchIncrement;
        private bool _showCrossHair = false;
        private bool _showAxes = false;


        // Constructors & Finalizers -------------------------------------------
        public CameraImageGraphics()
        {
        }

        // Properties ----------------------------------------------------------
        public MarkerType Marker { get { return _markerType; } set { _markerType = value; } }
        public double MarkerWidth { get { return _markerWidth; } set { _markerWidth = value; } }
        public double MarkerHeight { get { return _markerHeight; } set { _markerHeight = value; } }

        public double XYAxesHatchSize { get { return _xyAxesHatchSize; } set { _xyAxesHatchSize = value; } }
        public double XYAxesHatchInc { get { return _xyAxesHatchInc; } set { _xyAxesHatchInc = value; } }
        public bool ShowCrossHair { get { return _showCrossHair; } set { _showCrossHair = value; } }
        public bool ShowAxes { get { return _showAxes; } set { _showAxes = value; } }

        public bool ShowMeasurementLine { get { return _showMeasurementLine; } set { _showMeasurementLine = value; } }
        public double DistMeasureLineStartX { get { return _distMeasureLineStartX; } set { _distMeasureLineStartX = value; } }
        public double DistMeasureLineStartY { get { return _distMeasureLineStartY; } set { _distMeasureLineStartY = value; } }
        public double DistMeasureLineEndX { get { return _distMeasureLineEndX; } set { _distMeasureLineEndX = value; } }
        public double DistMeasureLineEndY { get { return _distMeasureLineEndY; } set { _distMeasureLineEndY = value; } }
        // Methods -------------------------------------------------------------

        public void Copy(CameraImageGraphics imageGraphics)
        {
            _markerType            = imageGraphics._markerType;
            _markerHeight          = imageGraphics._markerHeight;
            _markerWidth           = imageGraphics._markerWidth;
            _showAxes              = imageGraphics._showAxes;
            _showCrossHair         = imageGraphics._showCrossHair;
            _xyAxesHatchInc        = imageGraphics._xyAxesHatchInc;
            _xyAxesHatchSize       = imageGraphics._xyAxesHatchSize;

            _showMeasurementLine = imageGraphics._showMeasurementLine;
            _distMeasureLineEndX = imageGraphics._distMeasureLineEndX;
            _distMeasureLineEndY = imageGraphics._distMeasureLineEndY;
            _distMeasureLineStartX = imageGraphics._distMeasureLineStartX;
            _distMeasureLineStartY = imageGraphics._distMeasureLineStartY;
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/Marker");
            _markerType = (MarkerType)xml.Read("markerType", 0);
            _markerWidth = xml.Read("markerWidth", _defaultMarkerWidth);
            _markerHeight = xml.Read("markerHeight", _defatulMarkerHeight);
            _xyAxesHatchSize = (double)xml.Read("XYAxesHatchSize", _defaultHatchSize);
            _xyAxesHatchInc = (double)xml.Read("XYAxesHatchInc", _defaultHatchIncrement);
            _showCrossHair = (bool)xml.Read("ShowCrossHair", false);
            _showAxes = (bool)xml.Read("ShowAxes", false);
            xml.CloseSection();

            xml.OpenSection(section + "/Measurement");
            _distMeasureLineStartX = (double)xml.Read("DistMeasureLineStartX", _defaultLineStartX);
            _distMeasureLineStartY = (double)xml.Read("DistMeasureLineStartY", _defaultLineStartY);
            _distMeasureLineEndX = (double)xml.Read("DistMeasureLineEndX", _defaultLineEndX);
            _distMeasureLineEndY = (double)xml.Read("DistMeasureLineEndY", _defaultLineEndY);
            _showMeasurementLine = (bool)xml.Read("ShowMeasurementLine", false);
            xml.CloseSection();
        }

        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section + "/Marker");
            xml.Write("markerType", (int)_markerType);
            xml.Write("markerWidth", _markerWidth);
            xml.Write("markerHeight", _markerHeight);
            xml.Write("XYAxesHatchSize", _xyAxesHatchSize);
            xml.Write("XYAxesHatchInc", _xyAxesHatchInc);
            xml.Write("ShowCrossHair", _showCrossHair);
            xml.Write("ShowAxes", _showAxes);
            xml.CloseSection();

            xml.OpenSection(section + "/Measurement");
            xml.Write("DistMeasureLineStartX", _distMeasureLineStartX);
            xml.Write("DistMeasureLineStartY", _distMeasureLineStartY);
            xml.Write("DistMeasureLineEndX", _distMeasureLineEndX);
            xml.Write("DistMeasureLineEndY", _distMeasureLineEndY);
            xml.Write("ShowMeasurementLine", _showMeasurementLine);
            xml.CloseSection();
        }

        // Event handlers ------------------------------------------------------


        // Interface implementation --------------------------------------------

        // Internal methods -------------------------------------------------
              
    }
}
