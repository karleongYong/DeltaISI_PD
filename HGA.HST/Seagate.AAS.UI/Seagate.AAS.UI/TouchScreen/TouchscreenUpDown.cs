using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for TouchscreenUpDown.
	/// </summary>
	public class TouchscreenUpDown : System.Windows.Forms.UserControl
	{
		//private System.ComponentModel.IContainer components;
		private double min = -10;
		private double max = 10;
		private decimal inc = 0.1M;
		private Seagate.AAS.UI.TouchscreenNumBox myNumBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private decimal myValue = 0;
		private bool internalUpdate = false;
		private bool leftRight = false;
		private bool invertDirection = false;

		public event EventHandler ValueChanged;

		public TouchscreenUpDown()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
//				if(components != null)
//				{
//					components.Dispose();
//				}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TouchscreenUpDown));
			this.myNumBox = new Seagate.AAS.UI.TouchscreenNumBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// myNumBox
			// 
			this.myNumBox.BackColor = System.Drawing.Color.White;
			this.myNumBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.myNumBox.IntegerOnly = false;
			this.myNumBox.Location = new System.Drawing.Point(0, 0);
			this.myNumBox.Max = 100;
			this.myNumBox.Min = -5;
			this.myNumBox.Name = "myNumBox";
			this.myNumBox.Size = new System.Drawing.Size(104, 20);
			this.myNumBox.TabIndex = 0;
			this.myNumBox.Text = "0";
			this.myNumBox.TextChanged += new System.EventHandler(this.myNumBox_TextChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnDown);
			this.panel1.Controls.Add(this.btnUp);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 20);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(104, 52);
			this.panel1.TabIndex = 4;
			// 
			// btnDown
			// 
			this.btnDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
			this.btnDown.Location = new System.Drawing.Point(52, 0);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(52, 52);
			this.btnDown.TabIndex = 4;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
			this.btnUp.Location = new System.Drawing.Point(0, 0);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(52, 52);
			this.btnUp.TabIndex = 3;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// TouchscreenUpDown
			// 
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.myNumBox);
			this.Name = "TouchscreenUpDown";
			this.Size = new System.Drawing.Size(104, 72);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.TouchscreenUpDown_Layout);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void TouchscreenUpDown_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			btnUp.Width = this.Width / 2;
			this.Invalidate();
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			internalUpdate = true;

			if(invertDirection)
				this.Value = this.Value - inc;
			else
				this.Value = this.Value + inc;

			internalUpdate = false;

			if(ValueChanged != null)
				ValueChanged(this,EventArgs.Empty);
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			internalUpdate = true;

			if(invertDirection)
				this.Value = this.Value + inc;
			else
				this.Value = this.Value - inc;

			internalUpdate = false;

			if(ValueChanged != null)
				ValueChanged(this,EventArgs.Empty);
		}

		private void myNumBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				this.Value = Convert.ToDecimal(((TouchscreenNumBox)sender).Text);
				((TouchscreenNumBox)sender).BackColor = Color.White;
			}
			catch 
			{
				((TouchscreenNumBox)sender).BackColor = Color.Pink;
			}
			if(!internalUpdate)
			{
				if(ValueChanged != null)
					ValueChanged(this,EventArgs.Empty);
			}
		}
		
		//Properties
		public double Max
		{
			get { return max; }
			set 
			{
				max = (double)value; 
				myNumBox.Max = max;
			}
		}
		public double Min
		{
			get { return min; }
			set 
			{
				min = (double)value; 
				myNumBox.Min = min;
			}
		}
		public decimal Value
		{
			get { return myValue; }
			set 
			{
				myValue = (decimal)value;
				
				myNumBox.Text = myValue.ToString(); 

				if(myValue <= Convert.ToDecimal(min))
					btnDown.Enabled = false;
				else
					btnDown.Enabled = true;

				if(myValue >= Convert.ToDecimal(max))
					btnUp.Enabled = false;
				else
					btnUp.Enabled = true;
			}
		}
		public decimal Increment
		{
			get { return inc; }
			set { inc = (decimal)value; }
		}
		public bool LeftRight
		{
			get { return leftRight; }
			set
			{
				bool newValue = (bool)value;
				if(newValue != leftRight)
				{
					leftRight = newValue;

					if(leftRight)
					{
						btnUp.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
						btnDown.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
					}
					else
					{
						btnUp.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
						btnDown.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
					}
				}
			}
		}
		public bool InvertDirection
		{
			get { return invertDirection; }
			set { invertDirection = (bool)value; }
		}
	}
}
