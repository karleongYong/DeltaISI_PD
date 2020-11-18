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
//  [9/16/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
	public class DefaultErrorPanel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button btnRight;
		private System.Windows.Forms.Button btnMiddle;
		private System.Windows.Forms.Button btnLeft;
		private System.Windows.Forms.TextBox textBox1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private IErrorMessage message;

		public DefaultErrorPanel(IErrorMessage message)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.message = message;

			textBox1.Text = (message.FullDescription != null) ? message.FullDescription : message.Text;
			// left button text
			if(message.BtnList.Left == ErrorButton.NoButton)
				btnLeft.Hide();
			else
				btnLeft.Text = message.BtnList.Left.ToString();
			// middle button text
			if(message.BtnList.Middle == ErrorButton.NoButton)
				btnMiddle.Hide();
			else
				btnMiddle.Text = message.BtnList.Middle.ToString();
			// right button text
			if(message.BtnList.Right == ErrorButton.NoButton)
				btnRight.Hide();
			else
				btnRight.Text = message.BtnList.Right.ToString();
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
			this.btnRight = new System.Windows.Forms.Button();
			this.btnMiddle = new System.Windows.Forms.Button();
			this.btnLeft = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnRight
			// 
			this.btnRight.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnRight.Location = new System.Drawing.Point(256, 232);
			this.btnRight.Name = "btnRight";
			this.btnRight.Size = new System.Drawing.Size(88, 40);
			this.btnRight.TabIndex = 8;
			this.btnRight.Text = "Right Button";
			this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
			// 
			// btnMiddle
			// 
			this.btnMiddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnMiddle.Location = new System.Drawing.Point(132, 232);
			this.btnMiddle.Name = "btnMiddle";
			this.btnMiddle.Size = new System.Drawing.Size(88, 40);
			this.btnMiddle.TabIndex = 7;
			this.btnMiddle.Text = "Middle Button";
			this.btnMiddle.Click += new System.EventHandler(this.btnMiddle_Click);
			// 
			// btnLeft
			// 
			this.btnLeft.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnLeft.Location = new System.Drawing.Point(8, 232);
			this.btnLeft.Name = "btnLeft";
			this.btnLeft.Size = new System.Drawing.Size(88, 40);
			this.btnLeft.TabIndex = 6;
			this.btnLeft.Text = "Left Button";
			this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(8, 8);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(336, 200);
			this.textBox1.TabIndex = 5;
			this.textBox1.Text = "textBox1";
			// 
			// DefaultErrorPanel
			// 
			this.Controls.Add(this.btnRight);
			this.Controls.Add(this.btnMiddle);
			this.Controls.Add(this.btnLeft);
			this.Controls.Add(this.textBox1);
			this.Name = "DefaultErrorPanel";
			this.Size = new System.Drawing.Size(352, 280);
			this.ResumeLayout(false);

		}
		#endregion

		private void Close()
		{
			ServiceManager.ErrorHandler.UnRegisterMessage(message);
			message.OnInput();
		}

		private void btnLeft_Click(object sender, System.EventArgs e)
		{
			InputEventArgs arg = new InputEventArgs(message.BtnList.Left);

            message.ButtonSelected = message.BtnList.Left;
			Close();
		}

		private void btnMiddle_Click(object sender, System.EventArgs e)
		{
			InputEventArgs arg = new InputEventArgs(message.BtnList.Middle);
            message.ButtonSelected = message.BtnList.Middle;
			Close();
		}

		private void btnRight_Click(object sender, System.EventArgs e)
		{
			InputEventArgs arg = new InputEventArgs(message.BtnList.Right);
            message.ButtonSelected = message.BtnList.Right;
			Close();
		}
	}
}
