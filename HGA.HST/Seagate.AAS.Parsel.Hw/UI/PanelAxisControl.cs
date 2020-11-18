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
	/// Summary description for PanelAxis2.
	/// </summary>
	public class PanelAxisControl : System.Windows.Forms.UserControl
	{
		private IAxis axis;
		private double velocity;
		private double acceleration;
		private double stepsize;
        private int sliderMax;
		private bool jogenabled = true;
		private string negativestring = "Negative";
		private string positivestring = "Positive";

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label PositionLbl;
		private System.Windows.Forms.TrackBar JogVelBar;
		private System.Windows.Forms.Button JogNegativeBtn;
		private System.Windows.Forms.Button JogPositiveBtn;
		private System.Windows.Forms.Label JogVelocityLbl;
        private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.RadioButton StepRadioBtn;
		private System.Windows.Forms.RadioButton JogRadioBtn;
        private Seagate.AAS.UI.IndicatorButton EnableBtn;
		private System.ComponentModel.IContainer components;

		public PanelAxisControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		public IAxis Axis
		{
			set 
			{ 
				this.axis = value;
				groupBox1.Text = axis.Name;
				if(!axis.IsHomed)
					EnableBtn.Text = "Home";
				else
					EnableBtn.Text = "Toggle Servo";
			}
		}

		public double Increment
		{
			set { this.stepsize = value; }
		}

		public bool EnableJog
		{
			get { return jogenabled; }
			set
			{
				jogenabled = (bool)value;
				if( ! jogenabled)
					StepRadioBtn.Checked = true;
				JogRadioBtn.Enabled = jogenabled;
			}
		}

		public void Initialize(IAxis axis, double vel, double acc, string pos, string neg)
		{
            this.Axis = axis;

            this.velocity = vel;
            this.acceleration = acc;
            this.stepsize = 2.5;
            sliderMax = (int)(stepsize * 10);
            JogVelBar.Maximum = sliderMax;
            JogVelBar.Value = 3;
            SetJogVelocityCaption();
            this.positivestring = pos;
            this.negativestring = neg;
            SetButtonCaptions();

            if (this.Visible)
                timer1.Enabled = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelAxisControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.EnableBtn = new Seagate.AAS.UI.IndicatorButton();
            this.JogRadioBtn = new System.Windows.Forms.RadioButton();
            this.StepRadioBtn = new System.Windows.Forms.RadioButton();
            this.JogPositiveBtn = new System.Windows.Forms.Button();
            this.JogNegativeBtn = new System.Windows.Forms.Button();
            this.JogVelocityLbl = new System.Windows.Forms.Label();
            this.JogVelBar = new System.Windows.Forms.TrackBar();
            this.PositionLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JogVelBar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EnableBtn);
            this.groupBox1.Controls.Add(this.JogRadioBtn);
            this.groupBox1.Controls.Add(this.StepRadioBtn);
            this.groupBox1.Controls.Add(this.JogPositiveBtn);
            this.groupBox1.Controls.Add(this.JogNegativeBtn);
            this.groupBox1.Controls.Add(this.JogVelocityLbl);
            this.groupBox1.Controls.Add(this.JogVelBar);
            this.groupBox1.Controls.Add(this.PositionLbl);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 216);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Axis Name";
            // 
            // EnableBtn
            // 
            this.EnableBtn.Image = ((System.Drawing.Image)(resources.GetObject("EnableBtn.Image")));
            this.EnableBtn.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.EnableBtn.Location = new System.Drawing.Point(8, 19);
            this.EnableBtn.Name = "EnableBtn";
            this.EnableBtn.Size = new System.Drawing.Size(96, 47);
            this.EnableBtn.State = false;
            this.EnableBtn.TabIndex = 10;
            this.EnableBtn.Text = "Toggle Servo";
            this.EnableBtn.UseVisualStyleBackColor = true;
            this.EnableBtn.Click += new System.EventHandler(this.EnableBtn_Click);
            // 
            // JogRadioBtn
            // 
            this.JogRadioBtn.Location = new System.Drawing.Point(116, 72);
            this.JogRadioBtn.Name = "JogRadioBtn";
            this.JogRadioBtn.Size = new System.Drawing.Size(72, 24);
            this.JogRadioBtn.TabIndex = 9;
            this.JogRadioBtn.Text = "Jog";
            this.JogRadioBtn.CheckedChanged += new System.EventHandler(this.RadioBtn_CheckedChanged);
            // 
            // StepRadioBtn
            // 
            this.StepRadioBtn.Checked = true;
            this.StepRadioBtn.Location = new System.Drawing.Point(28, 72);
            this.StepRadioBtn.Name = "StepRadioBtn";
            this.StepRadioBtn.Size = new System.Drawing.Size(72, 24);
            this.StepRadioBtn.TabIndex = 8;
            this.StepRadioBtn.TabStop = true;
            this.StepRadioBtn.Text = "Step";
            // 
            // JogPositiveBtn
            // 
            this.JogPositiveBtn.Location = new System.Drawing.Point(112, 168);
            this.JogPositiveBtn.Name = "JogPositiveBtn";
            this.JogPositiveBtn.Size = new System.Drawing.Size(96, 40);
            this.JogPositiveBtn.TabIndex = 7;
            this.JogPositiveBtn.Text = "Jog Positive";
            this.JogPositiveBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogPositiveBtn_MouseDown);
            this.JogPositiveBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogPositiveBtn_MouseUp);
            // 
            // JogNegativeBtn
            // 
            this.JogNegativeBtn.Location = new System.Drawing.Point(8, 168);
            this.JogNegativeBtn.Name = "JogNegativeBtn";
            this.JogNegativeBtn.Size = new System.Drawing.Size(96, 40);
            this.JogNegativeBtn.TabIndex = 6;
            this.JogNegativeBtn.Text = "Jog Negative";
            this.JogNegativeBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogNegativeBtn_MouseDown);
            this.JogNegativeBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogNegativeBtn_MouseUp);
            // 
            // JogVelocityLbl
            // 
            this.JogVelocityLbl.Location = new System.Drawing.Point(16, 96);
            this.JogVelocityLbl.Name = "JogVelocityLbl";
            this.JogVelocityLbl.Size = new System.Drawing.Size(192, 24);
            this.JogVelocityLbl.TabIndex = 5;
            this.JogVelocityLbl.Text = "Jog Velocity: 50%";
            this.JogVelocityLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // JogVelBar
            // 
            this.JogVelBar.Location = new System.Drawing.Point(16, 120);
            this.JogVelBar.Maximum = 100;
            this.JogVelBar.Minimum = 1;
            this.JogVelBar.Name = "JogVelBar";
            this.JogVelBar.Size = new System.Drawing.Size(192, 42);
            this.JogVelBar.TabIndex = 4;
            this.JogVelBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.JogVelBar.Value = 50;
            this.JogVelBar.Scroll += new System.EventHandler(this.JogVelBar_Scroll);
            // 
            // PositionLbl
            // 
            this.PositionLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PositionLbl.Location = new System.Drawing.Point(120, 40);
            this.PositionLbl.Name = "PositionLbl";
            this.PositionLbl.Size = new System.Drawing.Size(80, 23);
            this.PositionLbl.TabIndex = 3;
            this.PositionLbl.Text = "posn";
            this.PositionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(120, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Actual Position";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PanelAxisControl
            // 
            this.Controls.Add(this.groupBox1);
            this.Name = "PanelAxisControl";
            this.Size = new System.Drawing.Size(224, 224);
            this.VisibleChanged += new System.EventHandler(this.PanelAxisControl_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JogVelBar)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private double JogVelocity()
		{
            if (JogRadioBtn.Checked)
                return ((double)JogVelBar.Value / (double)JogVelBar.Maximum * velocity);
            else
                return velocity;
        }

		private double StepSize()
		{
            return ((double)JogVelBar.Value / (double)JogVelBar.Maximum * stepsize);
        }

		private void SetJogVelocityCaption()
		{
			if(JogRadioBtn.Checked)
				JogVelocityLbl.Text = "Jog Velocity: " + JogVelocity().ToString() + axis.Unit + "/s";
			else
				JogVelocityLbl.Text = "Jog Increment: " + StepSize().ToString() + axis.Unit;
		}

		private void SetButtonCaptions()
		{
			JogNegativeBtn.Text = (JogRadioBtn.Checked ? "Jog " : "Step ") + negativestring;
			JogPositiveBtn.Text = (JogRadioBtn.Checked ? "Jog " : "Step ") + positivestring;
		}

		private void JogVelBar_Scroll(object sender, System.EventArgs e)
		{
			SetJogVelocityCaption();
		}

        public void PendantJogPositive(DigitalIOState state)
        {
            if (state == DigitalIOState.On)
                JogPositiveBtn_MouseDown(null, null);
            else
                JogPositiveBtn_MouseUp(null, null);
        }

        public void PendantJogNegative(DigitalIOState state)
        {
            if (state == DigitalIOState.On)
                JogNegativeBtn_MouseDown(null, null);
            else
                JogNegativeBtn_MouseUp(null, null);
        }

        private void JogNegativeBtn_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// start jog if jogging.  Do nothing if in step mode.
			if(JogRadioBtn.Checked)
			{
				try
				{
					axis.MoveRelativeStart(acceleration,JogVelocity(),-10000.0);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);				
				}
			}
		}

		private void JogNegativeBtn_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(JogRadioBtn.Checked)
			{
				try
				{
					axis.Stop();
				}
				catch (System.Exception ex)
				{
					MessageBox.Show(ex.Message);	
				}
			}
			else
			{
				try
				{
					axis.MoveRelativeStart(acceleration,JogVelocity(),-StepSize());
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);				
				}
			}
		}

		private void JogPositiveBtn_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(JogRadioBtn.Checked)
			{
				try
				{
					axis.MoveRelativeStart(acceleration,JogVelocity(),10000.0);
				}
				catch(System.Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void JogPositiveBtn_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(JogRadioBtn.Checked)
			{
				try
				{
					axis.Stop();
				}
				catch (System.Exception ex)
				{
					MessageBox.Show(ex.Message);	
				}
			}
			else
			{
				try
				{
					axis.MoveRelativeStart(acceleration,JogVelocity(),StepSize());
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);				
				}
			}
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(!this.Visible || axis == null)
			{
				timer1.Enabled = false;
				return;
			}

			EnableBtn.State = axis.IsEnabled;
			PositionLbl.Text = axis.GetActualPosition().ToString("F2") + axis.Unit;
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

		private void PanelAxisControl_VisibleChanged(object sender, System.EventArgs e)
		{
			if(!this.DesignMode)
			{
				timer1.Enabled = (this.Visible && axis != null);
			}
		}

		private void RadioBtn_CheckedChanged(object sender, System.EventArgs e)
		{
			SetJogVelocityCaption();
			SetButtonCaptions();
		}

	}
}
