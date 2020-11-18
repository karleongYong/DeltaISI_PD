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
//  [8/21/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Drawing;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// 
	/// </summary>
	public class PanelAxis : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button buttonHome;
		private System.Windows.Forms.Button buttonEnable;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonMove;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textAcceleration;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textVelocity;
		private System.Windows.Forms.TextBox textPosition;
		private System.Windows.Forms.TextBox textCountsPerUnit;
		private System.Windows.Forms.Label labelCountPerUnit;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.ComboBox cboAxis;
        private Seagate.AAS.UI.Led ledEnabled;
        private Seagate.AAS.UI.Led ledHomed;
        private Seagate.AAS.UI.FormattedNumber fmtActualPosition;
        private Seagate.AAS.UI.FormattedNumber fmtCommandPosition;
        private System.Windows.Forms.Label labelStatus;

		private IAxis axis;
        DateTime homeStart;
        bool checkHomedStatus = false;

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.buttonHome = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonEnable = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textPosition = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonMove = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textAcceleration = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textVelocity = new System.Windows.Forms.TextBox();
            this.labelCountPerUnit = new System.Windows.Forms.Label();
            this.textCountsPerUnit = new System.Windows.Forms.TextBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.cboAxis = new System.Windows.Forms.ComboBox();
            this.ledEnabled = new Seagate.AAS.UI.Led();
            this.ledHomed = new Seagate.AAS.UI.Led();
            this.fmtActualPosition = new Seagate.AAS.UI.FormattedNumber();
            this.fmtCommandPosition = new Seagate.AAS.UI.FormattedNumber();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(224, 120);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(96, 40);
            this.buttonHome.TabIndex = 2;
            this.buttonHome.Text = "Home";
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 400;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonEnable
            // 
            this.buttonEnable.Location = new System.Drawing.Point(224, 72);
            this.buttonEnable.Name = "buttonEnable";
            this.buttonEnable.Size = new System.Drawing.Size(96, 40);
            this.buttonEnable.TabIndex = 3;
            this.buttonEnable.Text = "Toggle Enable";
            this.buttonEnable.Click += new System.EventHandler(this.buttonEnable_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Actual Position:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Command Position:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textPosition
            // 
            this.textPosition.Location = new System.Drawing.Point(128, 160);
            this.textPosition.Name = "textPosition";
            this.textPosition.Size = new System.Drawing.Size(72, 20);
            this.textPosition.TabIndex = 8;
            this.textPosition.Text = "1";
            this.textPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Position";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(224, 168);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(96, 40);
            this.buttonMove.TabIndex = 10;
            this.buttonMove.Text = "Move";
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(32, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Acceleration";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textAcceleration
            // 
            this.textAcceleration.Location = new System.Drawing.Point(128, 184);
            this.textAcceleration.Name = "textAcceleration";
            this.textAcceleration.Size = new System.Drawing.Size(72, 20);
            this.textAcceleration.TabIndex = 11;
            this.textAcceleration.Text = "10";
            this.textAcceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(32, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Velocity";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textVelocity
            // 
            this.textVelocity.Location = new System.Drawing.Point(128, 208);
            this.textVelocity.Name = "textVelocity";
            this.textVelocity.Size = new System.Drawing.Size(72, 20);
            this.textVelocity.TabIndex = 13;
            this.textVelocity.Text = "5";
            this.textVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelCountPerUnit
            // 
            this.labelCountPerUnit.Location = new System.Drawing.Point(16, 80);
            this.labelCountPerUnit.Name = "labelCountPerUnit";
            this.labelCountPerUnit.Size = new System.Drawing.Size(96, 16);
            this.labelCountPerUnit.TabIndex = 16;
            this.labelCountPerUnit.Text = "Counts per unit";
            this.labelCountPerUnit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textCountsPerUnit
            // 
            this.textCountsPerUnit.Enabled = false;
            this.textCountsPerUnit.Location = new System.Drawing.Point(128, 80);
            this.textCountsPerUnit.Name = "textCountsPerUnit";
            this.textCountsPerUnit.Size = new System.Drawing.Size(72, 20);
            this.textCountsPerUnit.TabIndex = 15;
            this.textCountsPerUnit.Text = "400";
            this.textCountsPerUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textCountsPerUnit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textCountsPerUnit_KeyDown);
            this.textCountsPerUnit.Leave += new System.EventHandler(this.textCountsPerUnit_Leave);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(224, 216);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(96, 40);
            this.buttonStop.TabIndex = 18;
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // cboAxis
            // 
            this.cboAxis.Dock = System.Windows.Forms.DockStyle.Top;
            this.cboAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.cboAxis.Location = new System.Drawing.Point(0, 0);
            this.cboAxis.Name = "cboAxis";
            this.cboAxis.Size = new System.Drawing.Size(328, 32);
            this.cboAxis.TabIndex = 19;
            this.cboAxis.SelectedIndexChanged += new System.EventHandler(this.cboAxis_SelectedIndexChanged);
            // 
            // ledEnabled
            // 
            this.ledEnabled.DisplayAsButton = false;
            this.ledEnabled.Location = new System.Drawing.Point(152, 40);
            this.ledEnabled.Name = "ledEnabled";
            this.ledEnabled.Size = new System.Drawing.Size(80, 32);
            this.ledEnabled.State = false;
            this.ledEnabled.TabIndex = 20;
            this.ledEnabled.Text = "Enabled";
            // 
            // ledHomed
            // 
            this.ledHomed.DisplayAsButton = false;
            this.ledHomed.Location = new System.Drawing.Point(240, 40);
            this.ledHomed.Name = "ledHomed";
            this.ledHomed.Size = new System.Drawing.Size(72, 32);
            this.ledHomed.State = false;
            this.ledHomed.TabIndex = 21;
            this.ledHomed.Text = "Homed";
            // 
            // fmtActualPosition
            // 
            this.fmtActualPosition.Location = new System.Drawing.Point(136, 112);
            this.fmtActualPosition.Name = "fmtActualPosition";
            this.fmtActualPosition.Number = 0;
            this.fmtActualPosition.NumberFormat = "0.000";
            this.fmtActualPosition.Size = new System.Drawing.Size(64, 16);
            this.fmtActualPosition.TabIndex = 22;
            this.fmtActualPosition.Text = "0.000";
            this.fmtActualPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fmtCommandPosition
            // 
            this.fmtCommandPosition.Location = new System.Drawing.Point(128, 136);
            this.fmtCommandPosition.Name = "fmtCommandPosition";
            this.fmtCommandPosition.Number = 0;
            this.fmtCommandPosition.NumberFormat = "0.000";
            this.fmtCommandPosition.Size = new System.Drawing.Size(72, 16);
            this.fmtCommandPosition.TabIndex = 23;
            this.fmtCommandPosition.Text = "0.000";
            this.fmtCommandPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.labelStatus.ForeColor = System.Drawing.Color.Red;
            this.labelStatus.Location = new System.Drawing.Point(8, 256);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(312, 24);
            this.labelStatus.TabIndex = 24;
            // 
            // PanelAxis
            // 
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.fmtCommandPosition);
            this.Controls.Add(this.fmtActualPosition);
            this.Controls.Add(this.ledHomed);
            this.Controls.Add(this.ledEnabled);
            this.Controls.Add(this.cboAxis);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.labelCountPerUnit);
            this.Controls.Add(this.textCountsPerUnit);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textVelocity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textAcceleration);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textPosition);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonEnable);
            this.Controls.Add(this.buttonHome);
            this.Name = "PanelAxis";
            this.Size = new System.Drawing.Size(328, 288);
            this.VisibleChanged += new System.EventHandler(this.PanelAxis_VisibleChanged);
            this.ResumeLayout(false);

        }
	
		public PanelAxis()
		{
			InitializeComponent();

            axis = null;
            timer1.Enabled = false;

            buttonHome.Enabled = false;
            buttonMove.Enabled = false;
            buttonStop.Enabled = false;
		}
		
        public void AssignAxisMap(Hashtable axisMap)
        {
            cboAxis.Items.Clear();
            foreach (IAxis ioAxis in axisMap.Values)
            {
                cboAxis.Items.Add(ioAxis);
            }
            
            if (cboAxis.Items.Count > 0)
                cboAxis.SelectedIndex = 0;
        }


		public void AssignAxis(IAxis axis)
		{
            
			this.axis = axis;
            if (this.axis != null) 
            {
                textCountsPerUnit.Text  = axis.CountsPerUnit.ToString();
                labelCountPerUnit.Text  = "Counts per " + axis.Unit;
            }
            else
            {
                cboAxis.SelectedIndex = -1;
                timer1.Enabled = false;
                DisplayStatus("Select axis");
            }
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            if (this.Visible == false)
            {
                timer1.Enabled = false;
                return;
            }

            if (axis != null)
			{
                try
                {
                    ledEnabled.State = axis.IsEnabled;
                    ledHomed.State   = axis.IsHomed;

                    buttonHome.Enabled = axis.IsEnabled;
                    buttonMove.Enabled = axis.IsEnabled && ledHomed.State;
                    buttonStop.Enabled = axis.IsEnabled;

                    fmtActualPosition.Number = axis.GetActualPosition();
                    fmtCommandPosition.Number = axis.GetCommandPosition();

                    if (checkHomedStatus)
                    {
                        TimeSpan elapsed = DateTime.Now.Subtract(homeStart);
                        if (elapsed.Seconds > 10)
                        {
                            checkHomedStatus = false;
                            if (!axis.IsHomed)
                            {
                                DisplayError("Failed to home within 10 seconds");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayError("Status update error: " + ex.Message);
                }
			}
		}

		private void buttonHome_Click(object sender, System.EventArgs e)
		{
			if (axis != null)
			{
                try
                {
                    axis.HomeStart();
                    checkHomedStatus = true;
                    homeStart = DateTime.Now;
                    DisplayStatus("home started");
                }
                catch (Exception ex)
                {
                    DisplayError("Home error: " + ex.Message);
                }
			}
		}

		private void buttonEnable_Click(object sender, System.EventArgs e)
		{
			if (axis != null)
			{
                try
                {
                    axis.Enable(!axis.IsEnabled);
                    DisplayStatus("enabled");
                }
                catch (Exception ex)
                {
                    DisplayError("Enable error: " + ex.Message);
                }
			}
		}

		private void buttonMove_Click(object sender, System.EventArgs e)
		{
			if (axis != null)
			{
                try
                {
                    double acceleration = Convert.ToDouble(textAcceleration.Text);
                    double velocity     = Convert.ToDouble(textVelocity.Text);
                    double position		= Convert.ToDouble(textPosition.Text);
                    axis.MoveAbsoluteStart(acceleration,velocity,position);
                    DisplayStatus("moved started to " + position.ToString("0.000"));
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
			}
		}

		private void textCountsPerUnit_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.Enter)
			{
                try
                {
                    double countsPerUnit = Convert.ToDouble(textCountsPerUnit.Text);
                    if (countsPerUnit>0)
                        axis.SetUnit("mm", countsPerUnit);
                }
                catch (Exception ex)
                {
                    DisplayError("Set Unit error: " + ex.Message);
                }

			}
		}

		private void textCountsPerUnit_Leave(object sender, System.EventArgs e)
		{
            if (axis != null)
            {
                try
                {
                    textCountsPerUnit.Text      = axis.CountsPerUnit.ToString();

                }
                catch (Exception ex)
                {
                    DisplayError("Get Unit error: " + ex.Message);

                }
            }
		}

        private void buttonStop_Click(object sender, System.EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    checkHomedStatus = false;
                    axis.Stop();
                    DisplayStatus("Stopped");

                }
                catch (Exception ex)
                {
                    DisplayError("Stop error: " + ex.Message);
                }
            }
        
        }

        private void cboAxis_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (cboAxis.SelectedIndex >= 0)
            {
                AssignAxis((IAxis) cboAxis.SelectedItem);
                DisplayStatus("selected axis: " + ((IAxis) cboAxis.SelectedItem).Name);
            }
        }


        private void PanelAxis_VisibleChanged(object sender, System.EventArgs e)
        {
            timer1.Enabled = this.Visible;
        }

        private void DisplayError(string message)
        {
            labelStatus.ForeColor = Color.White;
            labelStatus.BackColor = Color.Red;
            labelStatus.Text = message;
        }

        private void DisplayStatus(string message)
        {
            labelStatus.ForeColor = Color.Black;
            labelStatus.BackColor = System.Drawing.SystemColors.Control;
            labelStatus.Text = message;
            
        }

        
	}
}
