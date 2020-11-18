//
//  (c) Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2006/12/06] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Utils.Maths
{
    public class Line
    {
        // Nested declarations -------------------------------------------------
        
        public delegate void LineChangeHandler(Line line);
        public event LineChangeHandler OnLineChange;

        // Member variables ----------------------------------------------------

        private double _startX = 0.0;
        private double _startY = 0.0;
        private double _endX = 0.0;
        private double _endY = 0.0;
        
        // Constructors & Finalizers -------------------------------------------
        
        public Line()
        {
        }

        public Line(double startX, double endX, double startY, double endY)
        {
            SetPosition(startX, endX, startY, endY);
        }

        // Properties ----------------------------------------------------------


        public double StartX
        {
            get { return _startX; }
            set { _startX = value; UpdateListeners(); }
        }

        public double StartY
        {
            get { return _startY; }
            set { _startY = value; UpdateListeners(); }
        }

        public double EndX
        {
            get { return _endX; }
            set { _endX = value; UpdateListeners(); }
        }

        public double EndY
        {
            get { return EndY; }
            set { EndY = value; UpdateListeners(); }
        }

        public double LengthX
        {
            get { return (_endX - _startX); }
        }

        public double LengthY
        {
            get { return (_endY - _startY); }
        }

        public double Length
        {
            get { return (Math.Sqrt(LengthX*LengthX + LengthY*LengthY)); }
        }

        // Methods -------------------------------------------------------------

        public void SetPosition(double startX, double endX, double startY, double endY)
        {
            _startX = startX;
            _endX = endX;
            _startY = startY;
            _endY = endY;

            UpdateListeners(); 
        }

        // Event handlers ------------------------------------------------------
        
        // Internal methods ----------------------------------------------------

        private void UpdateListeners()
        {
            if (OnLineChange != null)
                OnLineChange(this);
        }
    }
}
