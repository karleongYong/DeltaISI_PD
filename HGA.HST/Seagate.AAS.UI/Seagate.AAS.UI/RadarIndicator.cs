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
//  [4/26/2006] Tom Chuang
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for RadarIndicator.
	/// </summary>
    [ToolboxBitmapAttribute(typeof(Seagate.AAS.UI.RadarIndicator), "images.Radar.bmp")]
    public class RadarIndicator : System.Windows.Forms.Control
	{

        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        
        
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private float _valueX         = 25;
        private float _valueY         = 25;
        private float _target         = 0;
        private float _innerTolerance = 50;
        private float _outerTolerance = 100;

        // Constructors & Finalizers -------------------------------------------
        
		public RadarIndicator()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
            // 
            // RadarIndicator
            // 
            this.Name = "RadarIndicator";
            this.Size = new System.Drawing.Size(248, 224);

        }
		#endregion


        private Color GetArrowColor(float val)
        {
            if (Math.Abs(val - _target) < _innerTolerance)
                return Color.Green;
            else if (Math.Abs(val - _target) < _outerTolerance)
                return Color.Yellow;
            else 
                return Color.Red;
        }

        protected override void OnPaint(PaintEventArgs pe)
		{
            int margin = 6;
            Graphics g = pe.Graphics;

            float radius = (float) Math.Sqrt(_valueX*_valueX + _valueY*_valueY);
            int diameter = Math.Min(this.ClientRectangle.Width, this.ClientRectangle.Height) - (margin + 2);
            int innerDiameter = (int) (diameter*_innerTolerance/_outerTolerance);
            Rectangle radarRegion  = new Rectangle(0, 0, diameter, diameter);
            Rectangle radarRegion2 = new Rectangle(diameter/2-innerDiameter/2, diameter/2-innerDiameter/2, innerDiameter, innerDiameter );
            Rectangle textRegion = new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);


            float xLength = (float) ( 0.5*diameter*_valueX/_outerTolerance);
            float yLength = (float) (-0.5*diameter*_valueY/_outerTolerance);

            Point arrowX = new Point(diameter/2, diameter+margin/2+2);
            Point arrowY = new Point(diameter+margin/2+2, diameter/2);
            Point origin = new Point(diameter/2, diameter/2);

            using ( Pen radarPen  = new Pen(Color.Black, 1))
            using ( Pen xArrowPen = new Pen(GetArrowColor(_valueX), margin))
            using ( Pen yArrowPen = new Pen(GetArrowColor(_valueY), margin))
            using ( Pen vectorPen = new Pen(GetArrowColor(radius), 2))
            {
                // draw radar
                g.DrawEllipse(radarPen, radarRegion );
                radarPen.DashStyle = DashStyle.Dot;
                g.DrawEllipse(radarPen, radarRegion2 );
                
                // draw x length
                g.DrawLine(xArrowPen, arrowX.X, arrowX.Y, arrowX.X + xLength, arrowX.Y);

                // draw y length
                g.DrawLine(yArrowPen, arrowY.X, arrowY.Y, arrowY.X, arrowY.Y + yLength);

                // draw vector
                g.DrawLine(vectorPen, origin.X, origin.Y, origin.X+xLength, origin.Y+yLength);
  
            }
            

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

        // Properties ----------------------------------------------------------
		[DescriptionAttribute("Vector X value")]
		public float ValueX
		{
			get
			{
				return _valueX;
			}
			set
			{
				if (this._valueX != value)
				{
					this._valueX = value; 
					this.Invalidate();
				}
			}
		}

        [DescriptionAttribute("Vector Y value")]
        public float ValueY
        {
            get
            {
                return _valueY;
            }
            set
            {
                if (this._valueY != value)
                {
                    this._valueY = value; 
                    this.Invalidate();
                }
            }
        }


        [DescriptionAttribute("Display green if less than inner tolerance, yellow if between inner and outer tolerance, and red if greater than outer tolerance.")]
        public float InnerTolerance
        {
            get
            {
                return _innerTolerance;
            }
            set
            {
                if (this._innerTolerance != value)
                {
                    this._innerTolerance = value; 
                    this.Invalidate();
                }
            }
        }

        [DescriptionAttribute("Display green if less than inner tolerance, yellow if between inner and outer tolerance, and red if greater than outer tolerance.")]
        public float OuterTolerance
        {
            get
            {
                return _outerTolerance;
            }
            set
            {
                if (this._outerTolerance != value)
                {
                    this._outerTolerance = value; 
                    this.Invalidate();
                }
            }
        }
	}
}