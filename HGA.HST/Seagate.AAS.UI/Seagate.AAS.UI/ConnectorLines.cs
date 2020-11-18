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
//  [9/12/2005]
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    public enum Orientation : int
    {
        TopToBottom    = 0,
        BottomToTop    = 1,
        LeftToRight = 2,
        RightToLeft = 3
    }

	/// <summary>
	/// Summary description for ConnectorLines.
	/// </summary>
    public class ConnectorLines : System.Windows.Forms.Control
    {

        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        private System.ComponentModel.Container components = null;

        private Orientation orientation = Orientation.TopToBottom;
        private uint         thickness = 2;

        // Constructors & Finalizers -------------------------------------------
        public ConnectorLines()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitComponent call
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                    components.Dispose();
            }
            base.Dispose( disposing );
        }
        
        // Properties ----------------------------------------------------------
        [CategoryAttribute("Appearance")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DefaultValueAttribute(Orientation.TopToBottom)]
        [DescriptionAttribute("Orientation that lines will be drawn.")]
        public Orientation Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                if (this.orientation != value)
                {
                    this.orientation = value; 
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DescriptionAttribute("line thickness")]
        [DefaultValueAttribute(2)]
        public uint Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                if (this.thickness != value)
                {
                    this.thickness = value; 
                    this.Invalidate();
                }
            }
        }


        // Methods -------------------------------------------------------------
        
        // Internal methods ----------------------------------------------------
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            Graphics g = pe.Graphics;
            uint t = thickness;

            // draw chart boundary
            using (Pen   pen   = new Pen(this.ForeColor, t))
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                Rectangle boundary = this.ClientRectangle;
                switch (orientation) 
                {
                    case Orientation.TopToBottom:
                        //                              x1                   y1                 x2                 y2
                        g.DrawLine(pen,                t/2,                 0,                t/2, boundary.Height/2);  // left fork
                        g.DrawLine(pen, boundary.Width-t/2,                 0, boundary.Width-t/2, boundary.Height/2);  // right fork
                        g.DrawLine(pen,                  0, boundary.Height/2,     boundary.Width, boundary.Height/2);  // horiz line
                        g.DrawLine(pen,   boundary.Width/2, boundary.Height/2,   boundary.Width/2,   boundary.Height);  // center fork
                        break;
                    case Orientation.BottomToTop:
                        //                              x1                   y1                 x2                 y2
                        g.DrawLine(pen,                t/2, boundary.Height/2,                t/2,   boundary.Height);
                        g.DrawLine(pen, boundary.Width-t/2, boundary.Height/2, boundary.Width-t/2,   boundary.Height);
                        g.DrawLine(pen,                  0, boundary.Height/2,     boundary.Width, boundary.Height/2);
                        g.DrawLine(pen,   boundary.Width/2,                 0,   boundary.Width/2, boundary.Height/2);
                        break;
                    case Orientation.LeftToRight:
                        //                              x1                   y1                 x2                 y2
                        g.DrawLine(pen,                  0,                 t/2,   boundary.Width/2,                  t/2);  // top fork
                        g.DrawLine(pen,                  0, boundary.Height-t/2,   boundary.Width/2,  boundary.Height-t/2);  // bottom fork
                        g.DrawLine(pen,   boundary.Width/2,                   0,   boundary.Width/2,      boundary.Height);  // vert line
                        g.DrawLine(pen,   boundary.Width/2,   boundary.Height/2,     boundary.Width,    boundary.Height/2);  // center fork
                        break;
                    case Orientation.RightToLeft:
                        //                              x1                   y1                 x2                 y2
                        g.DrawLine(pen,   boundary.Width/2,                 t/2,     boundary.Width,                  t/2);  // top fork
                        g.DrawLine(pen,   boundary.Width/2, boundary.Height-t/2,     boundary.Width,  boundary.Height-t/2);  // bottom fork
                        g.DrawLine(pen,   boundary.Width/2,                   0,   boundary.Width/2,      boundary.Height);  // vert line
                        g.DrawLine(pen,                  0,   boundary.Height/2,   boundary.Width/2,    boundary.Height/2);  // center fork
                        break;
                }
            }

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

    }

 }
