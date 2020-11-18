//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/04/27] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	public enum LedColorList
	{
		Red,
		Green
	}
	
	/// <summary>
	/// Summary description for Led.
	/// </summary>
	[ToolboxBitmapAttribute(typeof(Seagate.AAS.UI.Led), "images.LedOn.bmp")]
	public class Led : System.Windows.Forms.Control
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private bool state;
		private bool output;
		private LedColorList ledColor = LedColorList.Red; 
		private System.Drawing.Rectangle textRegion;

		public Led()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.TextChanged += new EventHandler(Led_TextChanged);
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

		#region Led Members
		protected override void OnPaint(PaintEventArgs pe)
		{
			// 
			Graphics g = pe.Graphics;
			Assembly assem = this.GetType().Assembly;
			Stream stream;
			if (state && ledColor == LedColorList.Red)
				stream = assem.GetManifestResourceStream("Seagate.AAS.UI.Resources.LedOn.bmp");
			else if (!state && ledColor == LedColorList.Red)
				stream = assem.GetManifestResourceStream("Seagate.AAS.UI.Resources.LedOff.bmp");
			else if (state && ledColor == LedColorList.Green)
				stream = assem.GetManifestResourceStream("Seagate.AAS.UI.Resources.LedOnGreen.bmp");
			else 
				stream = assem.GetManifestResourceStream("Seagate.AAS.UI.Resources.LedOffGreen.bmp");
			using ( Brush foreBrush  = new SolidBrush(this.ForeColor) )
			using ( Brush textBrush = new SolidBrush(this.Enabled ? SystemColors.ControlText : SystemColors.GrayText))
			using ( Bitmap ledImage = new Bitmap(stream) )
			{
				if(output)
				{
					// bitmap is 16x16, 16 colors
					ledImage.MakeTransparent();

					textRegion = this.ClientRectangle;

					System.Drawing.Pen penW = new Pen(System.Drawing.SystemColors.ControlLightLight);//.Color.White);
					System.Drawing.Pen penB = new Pen(System.Drawing.SystemColors.ControlDarkDark);//.Color.Black );
					System.Drawing.Pen penG = new Pen(System.Drawing.SystemColors.ControlDark);//.Color.Gray);
					if(state)
					{
						g.DrawImage(ledImage, new PointF(3, this.ClientRectangle.Height/2 - 8 + 1));
						textRegion.X += 16 + 8;
						textRegion.Y += 1;

						g.DrawLine(penG,new Point(1,1),new Point(this.Width-2,1));
						g.DrawLine(penG,new Point(1,1),new Point(1,this.Height-2));
						g.DrawLine(penW,new Point(this.Width-1,this.Height-1),new Point(this.Width-1,0));
						g.DrawLine(penW,new Point(this.Width-1,this.Height-1),new Point(0,this.Height-1));
						g.DrawLine(penB,new Point(0,0),new Point(this.Width-1,0));
						g.DrawLine(penB,new Point(0,0),new Point(0,this.Height-1));
					}
					else
					{
						g.DrawImage(ledImage, new PointF(2, this.ClientRectangle.Height/2 - 8));
						textRegion.X += 16 + 7;

						g.DrawLine(penW,new Point(0,0),new Point(this.Width-1,0));
						g.DrawLine(penW,new Point(0,0),new Point(0,this.Height-1));
						g.DrawLine(penB,new Point(this.Width-1,this.Height-1),new Point(this.Width-1,0));
						g.DrawLine(penB,new Point(this.Width-1,this.Height-1),new Point(0,this.Height-1));
						g.DrawLine(penG,new Point(this.Width-2,this.Height-2),new Point(this.Width-2,1));
						g.DrawLine(penG,new Point(this.Width-2,this.Height-2),new Point(1,this.Height-2));
					}
					StringFormat fmt = new StringFormat();
					fmt.Alignment = StringAlignment.Near;
					fmt.LineAlignment = StringAlignment.Center;
					textRegion.Width -= textRegion.X;
					g.DrawString(this.Text, this.Font, textBrush, textRegion, fmt);
				}
				else
				{
					// bitmap is 16x16, 16 colors
					ledImage.MakeTransparent();
					g.DrawImage(ledImage, new PointF(0, this.ClientRectangle.Height/2 - 8));

					textRegion = this.ClientRectangle;
					textRegion.X += 16 + 5;
					textRegion.Width -= textRegion.X;

					StringFormat fmt = new StringFormat();
					fmt.Alignment = StringAlignment.Near;
					fmt.LineAlignment = StringAlignment.Center;
					g.DrawString(this.Text, this.Font, textBrush, textRegion, fmt);
				}
			}
            

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		[DescriptionAttribute("State for the LED. true=On, false=Off")]
		public bool State
		{
			get
			{
				return state;
			}
			set
			{
				if (this.state != value)
				{
					this.state = value; 
					this.Invalidate();
				}
			}
		}

		[DescriptionAttribute("Use to diplay output as a button")]
		public bool DisplayAsButton
		{
			get
			{
				return output;
			}
			set
			{
				if (this.output != value)
				{
					this.output = value;
					this.Invalidate();
				}
			}
		}

		[DescriptionAttribute("LED color selection")]
		public LedColorList LedColor
		{
			get
			{
				return ledColor;
			}
			set
			{
				if (this.ledColor != value)
				{
					this.ledColor = value;
					this.Invalidate();
				}
			}
		}

		private void Led_TextChanged(object sender, EventArgs e)
		{
			this.Invalidate();
		}
		#endregion
	}
}