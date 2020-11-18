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
//  [2006/12/04] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for AnalogControl.
	/// </summary>
	public class AnalogControl : System.Windows.Forms.UserControl
	{
		private bool _ioType = false;
		private double _value = 0.0;
		private string _units = "counts";
		private string _label = "IO Label";
		private double _max = 10.0;
		private double _min = -10.0;
		private bool _userEdit = true;
		private int _precision = 4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblUnits;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblIOName;
		private Seagate.AAS.UI.TouchscreenNumBox txtValue;
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AnalogControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			txtValue.TextChanged += new EventHandler(txtValue_TextChanged);

			// TODO: Add any initialization after the InitializeComponent call

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AnalogControl));
			this.panel1 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.lblUnits = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblIOName = new System.Windows.Forms.Label();
			this.txtValue = new Seagate.AAS.UI.TouchscreenNumBox();
			this.lblValue = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Controls.Add(this.lblUnits);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.lblIOName);
			this.panel1.Controls.Add(this.txtValue);
			this.panel1.Controls.Add(this.lblValue);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(192, 72);
			this.panel1.TabIndex = 5;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.TabIndex = 10;
			this.pictureBox1.TabStop = false;
			// 
			// lblUnits
			// 
			this.lblUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblUnits.Location = new System.Drawing.Point(72, 24);
			this.lblUnits.Name = "lblUnits";
			this.lblUnits.Size = new System.Drawing.Size(112, 16);
			this.lblUnits.TabIndex = 8;
			this.lblUnits.Text = "counts";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Units:";
			// 
			// lblIOName
			// 
			this.lblIOName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblIOName.Location = new System.Drawing.Point(32, 8);
			this.lblIOName.Name = "lblIOName";
			this.lblIOName.Size = new System.Drawing.Size(152, 16);
			this.lblIOName.TabIndex = 6;
			this.lblIOName.Text = "IO Label";
			// 
			// txtValue
			// 
			this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtValue.IntegerOnly = false;
			this.txtValue.Location = new System.Drawing.Point(8, 40);
			this.txtValue.Max = 100;
			this.txtValue.Min = -5;
			this.txtValue.Name = "txtValue";
			this.txtValue.Size = new System.Drawing.Size(176, 20);
			this.txtValue.TabIndex = 9;
			this.txtValue.Text = "0.0";
			this.txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtValue.Visible = false;
			// 
			// lblValue
			// 
			this.lblValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblValue.BackColor = System.Drawing.SystemColors.Control;
			this.lblValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblValue.Location = new System.Drawing.Point(8, 40);
			this.lblValue.Name = "lblValue";
			this.lblValue.Size = new System.Drawing.Size(176, 24);
			this.lblValue.TabIndex = 5;
			this.lblValue.Text = "label1";
			this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AnalogControl
			// 
			this.Controls.Add(this.panel1);
			this.Name = "AnalogControl";
			this.Size = new System.Drawing.Size(192, 72);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region AnalogControl Members
//		protected override void OnPaint(PaintEventArgs e)
//		{
//			Graphics g = e.Graphics;
//
//			Pen blackPen = new Pen(System.Drawing.Color.Black);
//			SolidBrush fillBrush = new SolidBrush(this.BackColor);
//			
//			g.DrawRectangle(blackPen,0,0,this.Width-1,this.Height-1);
//			g.FillRectangle(fillBrush,0,0,this.Width-1,this.Height-1);
//			base.OnPaint (e);
//		}

		[DescriptionAttribute("Analog IO type.  true=Output, false=Input")]
		public bool IOTypeOutput
		{
			get
			{
				return _ioType;
			}
			set
			{
				_ioType = (bool)value;
				txtValue.Visible = _ioType;
				lblValue.Visible = !_ioType;
			}
		}
		[DescriptionAttribute("Value to Display")]
		public double Value
		{
			get
			{
				return _value;
			}
			set
			{
				_userEdit = false;		
				_value = (double)value;
				lblValue.Text = _value.ToString("F" + _precision.ToString());
				txtValue.Text = _value.ToString("F" + _precision.ToString());
				_userEdit = true;
			}
		}
		[DescriptionAttribute("Label to Display")]
		public string Label
		{
			get
			{
				return _label;
			}
			set
			{
				_label = (string)value;
				lblIOName.Text = _label;
			}
		}
		[DescriptionAttribute("Units to Display")]
		public string Units
		{
			get
			{
				return _units;
			}
			set
			{
				_units = (string)value;
				lblUnits.Text = _units;
			}
		}
		[DescriptionAttribute("Maximum allowable entry.")]
		public double OutputMax
		{
			get
			{
				return _max;
			}
			set
			{
				_max = (double)value;
				txtValue.Max = _max;
			}
		}
		[DescriptionAttribute("Minimum allowable entry.")]
		public double OutputMin
		{
			get
			{
				return _min;
			}
			set
			{
				_min = (double)value;
				txtValue.Min = _min;
			}
		}
		[DescriptionAttribute("Number of decimal points to display.")]
		public int Precision
		{
			get
			{
				return _precision;
			}
			set
			{
				_precision = value;
			}
		}

		public event EventHandler ValueChanged;
		#endregion

		private void txtValue_TextChanged(object sender, EventArgs e)
		{
			if(_userEdit)
			{
				_value = Convert.ToDouble(txtValue.Text);
				if(ValueChanged != null)
					ValueChanged(this,EventArgs.Empty);
			}
		}
	}
}
