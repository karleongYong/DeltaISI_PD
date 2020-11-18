using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for PanelSimpleAxis.
	/// </summary>
	public class PanelSimpleAxis : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button EnableBtn;
		private System.ComponentModel.IContainer components;
		private Seagate.AAS.UI.Led EnabledLED;
		private System.Windows.Forms.Timer timer1;
		private IAxis axis;

		public PanelSimpleAxis()
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
			this.components = new System.ComponentModel.Container();
			this.EnableBtn = new System.Windows.Forms.Button();
			this.EnabledLED = new Seagate.AAS.UI.Led();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// EnableBtn
			// 
			this.EnableBtn.Location = new System.Drawing.Point(8, 40);
			this.EnableBtn.Name = "EnableBtn";
			this.EnableBtn.Size = new System.Drawing.Size(88, 32);
			this.EnableBtn.TabIndex = 1;
			this.EnableBtn.Text = "Enable";
			this.EnableBtn.Click += new System.EventHandler(this.EnableBtn_Click);
			// 
			// EnabledLED
			// 
			this.EnabledLED.DisplayAsButton = false;
			this.EnabledLED.Location = new System.Drawing.Point(8, 8);
			this.EnabledLED.Name = "EnabledLED";
			this.EnabledLED.Size = new System.Drawing.Size(112, 16);
			this.EnabledLED.State = false;
			this.EnabledLED.TabIndex = 2;
			this.EnabledLED.Text = "Enabled";
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// PanelSimpleAxis
			// 
			this.Controls.Add(this.EnabledLED);
			this.Controls.Add(this.EnableBtn);
			this.Name = "PanelSimpleAxis";
			this.Size = new System.Drawing.Size(136, 80);
			this.VisibleChanged += new System.EventHandler(this.PanelSimpleAxis_VisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion

		public void AssignAxis(IAxis axis)
		{
			this.axis = axis;
			EnabledLED.Text = axis.Name;
			if(!axis.IsHomed)
				EnableBtn.Text = "Home";
			else
				EnableBtn.Text = "Toggle Servo";
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            if (this.Visible == false)
            {
                timer1.Enabled = false;
                return;
            }

            EnabledLED.State = axis.IsEnabled;
		}

		private void EnableBtn_Click(object sender, System.EventArgs e)
		{
			if(!axis.IsHomed)
			{
				try
				{
					axis.HomeStart();
					EnableBtn.Text = "Toggle Servo";
				}
				catch(System.Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			else
			{
				try
				{
					axis.Enable(!axis.IsEnabled);
				}
				catch(System.Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void PanelSimpleAxis_VisibleChanged(object sender, System.EventArgs e)
		{
			if(!this.DesignMode)					// in design mode, the following line will cause exceptions to be thrown until you kill Visual Studio. Nice "feature".
				timer1.Enabled = this.Visible;
		}
	}
}
