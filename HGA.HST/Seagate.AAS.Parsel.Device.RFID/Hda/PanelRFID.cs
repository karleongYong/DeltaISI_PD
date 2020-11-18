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
//  [4/26/2006] by Sabrina Murray
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Device.RFID
{
	/// <summary>
	/// Summary description for PanelRFID.
	/// </summary>
	public class PanelRFID : System.Windows.Forms.UserControl
	{
		// Nested Declarations --------------------------------
		// Member variables ----------------------------------------------------	
		private System.Windows.Forms.ComboBox comboBoxComPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonInitialize;
		private System.Windows.Forms.Button buttonReadTag;
		private System.Windows.Forms.TextBox textBoxTagRead;
		private System.Windows.Forms.Button buttonWriteTag;
		private Seagate.AAS.UI.TouchscreenTextBox touchscreenTextBoxTagWrite;
		private System.Windows.Forms.Label labelStatus;
		
		private HWCRFIDRW _tagReader = null;
		private int _comPort = 0;
		private System.Windows.Forms.Button buttonReadTagID;
		private System.Windows.Forms.Button buttonClearTag;
		private System.Windows.Forms.Label label2;
		private int _readStartAddress = 0;
		private int _numBytesToRead = 0;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Constructors & Finalizers -------------------------------------------
		public PanelRFID()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();			
		}

		public PanelRFID(HWCRFIDRW tagReader, int readStartAddress, int numBytesToRead)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			_tagReader = tagReader;
			comboBoxComPort.SelectedIndex  = tagReader.COMPort - 1; 

			if( _tagReader.IsInitialized )
			{
				buttonInitialize.Enabled = false;
				buttonReadTag.Enabled = true;
				buttonWriteTag.Enabled = true;
				buttonClearTag.Enabled = true;
				buttonReadTagID.Enabled = true;
			}
			else
			{
				buttonInitialize.Enabled = true;
				buttonReadTag.Enabled = false;
				buttonWriteTag.Enabled = false;
				buttonClearTag.Enabled = false;
				buttonReadTagID.Enabled = false;
			}

			_readStartAddress = readStartAddress;
			_numBytesToRead = numBytesToRead;
		}
		public PanelRFID(HWCRFIDRW tagReader, int comPort, int readStartAddress, int numBytesToRead)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			_tagReader = tagReader;
			_comPort = comPort;
			comboBoxComPort.SelectedIndex  = _comPort - 1; 
			
			if( _tagReader.IsInitialized )
			{
				buttonInitialize.Enabled = false;
				buttonReadTag.Enabled = true;
				buttonWriteTag.Enabled = true;
			}
			else
			{
				buttonInitialize.Enabled = true;
				buttonReadTag.Enabled = false;
				buttonWriteTag.Enabled = false;
			}

			_readStartAddress = readStartAddress;
			_numBytesToRead = numBytesToRead;
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
			this.comboBoxComPort = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonInitialize = new System.Windows.Forms.Button();
			this.buttonReadTag = new System.Windows.Forms.Button();
			this.textBoxTagRead = new System.Windows.Forms.TextBox();
			this.buttonWriteTag = new System.Windows.Forms.Button();
			this.touchscreenTextBoxTagWrite = new Seagate.AAS.UI.TouchscreenTextBox();
			this.labelStatus = new System.Windows.Forms.Label();
			this.buttonReadTagID = new System.Windows.Forms.Button();
			this.buttonClearTag = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboBoxComPort
			// 
			this.comboBoxComPort.Items.AddRange(new object[] {
																 "Com1",
																 "Com2",
																 "Com3",
																 "Com4",
																 "Com5",
																 "Com6",
																 "Com7",
																 "Com8",
																 "Com9",
																 "Com10"});
			this.comboBoxComPort.Location = new System.Drawing.Point(88, 21);
			this.comboBoxComPort.Name = "comboBoxComPort";
			this.comboBoxComPort.Size = new System.Drawing.Size(121, 21);
			this.comboBoxComPort.TabIndex = 0;
			this.comboBoxComPort.Text = "comboBox1";
			this.comboBoxComPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxComPort_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "Com Port:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonInitialize
			// 
			this.buttonInitialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonInitialize.Location = new System.Drawing.Point(224, 16);
			this.buttonInitialize.Name = "buttonInitialize";
			this.buttonInitialize.Size = new System.Drawing.Size(128, 32);
			this.buttonInitialize.TabIndex = 2;
			this.buttonInitialize.Text = "Initialize";
			this.buttonInitialize.Click += new System.EventHandler(this.buttonInitialize_Click);
			// 
			// buttonReadTag
			// 
			this.buttonReadTag.Location = new System.Drawing.Point(16, 64);
			this.buttonReadTag.Name = "buttonReadTag";
			this.buttonReadTag.Size = new System.Drawing.Size(128, 32);
			this.buttonReadTag.TabIndex = 3;
			this.buttonReadTag.Text = "Read Tag";
			this.buttonReadTag.Click += new System.EventHandler(this.buttonReadTag_Click);
			// 
			// textBoxTagRead
			// 
			this.textBoxTagRead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxTagRead.Enabled = false;
			this.textBoxTagRead.Location = new System.Drawing.Point(16, 112);
			this.textBoxTagRead.Multiline = true;
			this.textBoxTagRead.Name = "textBoxTagRead";
			this.textBoxTagRead.Size = new System.Drawing.Size(336, 80);
			this.textBoxTagRead.TabIndex = 4;
			this.textBoxTagRead.Text = "";
			// 
			// buttonWriteTag
			// 
			this.buttonWriteTag.Location = new System.Drawing.Point(16, 208);
			this.buttonWriteTag.Name = "buttonWriteTag";
			this.buttonWriteTag.Size = new System.Drawing.Size(128, 32);
			this.buttonWriteTag.TabIndex = 5;
			this.buttonWriteTag.Text = "Write to Tag";
			this.buttonWriteTag.Click += new System.EventHandler(this.buttonWriteTag_Click);
			// 
			// touchscreenTextBoxTagWrite
			// 
			this.touchscreenTextBoxTagWrite.AlphaNumOnly = false;
			this.touchscreenTextBoxTagWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.touchscreenTextBoxTagWrite.BackColor = System.Drawing.Color.White;
			this.touchscreenTextBoxTagWrite.FormTitle = "Enter Text";
			this.touchscreenTextBoxTagWrite.Location = new System.Drawing.Point(152, 208);
			this.touchscreenTextBoxTagWrite.MinLength = 0;
			this.touchscreenTextBoxTagWrite.Multiline = true;
			this.touchscreenTextBoxTagWrite.Name = "touchscreenTextBoxTagWrite";
			this.touchscreenTextBoxTagWrite.NoWhiteSpace = false;
			this.touchscreenTextBoxTagWrite.OnlyCaps = false;
			this.touchscreenTextBoxTagWrite.Size = new System.Drawing.Size(200, 32);
			this.touchscreenTextBoxTagWrite.TabIndex = 6;
			this.touchscreenTextBoxTagWrite.Text = "";
			// 
			// labelStatus
			// 
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelStatus.Location = new System.Drawing.Point(16, 320);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(336, 32);
			this.labelStatus.TabIndex = 7;
			this.labelStatus.Text = "Status:";
			// 
			// buttonReadTagID
			// 
			this.buttonReadTagID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReadTagID.Location = new System.Drawing.Point(224, 64);
			this.buttonReadTagID.Name = "buttonReadTagID";
			this.buttonReadTagID.Size = new System.Drawing.Size(128, 32);
			this.buttonReadTagID.TabIndex = 8;
			this.buttonReadTagID.Text = "Read Tag ID";
			this.buttonReadTagID.Click += new System.EventHandler(this.buttonReadTagID_Click);
			// 
			// buttonClearTag
			// 
			this.buttonClearTag.Location = new System.Drawing.Point(16, 280);
			this.buttonClearTag.Name = "buttonClearTag";
			this.buttonClearTag.Size = new System.Drawing.Size(336, 32);
			this.buttonClearTag.TabIndex = 9;
			this.buttonClearTag.Text = "Clear Tag";
			this.buttonClearTag.Click += new System.EventHandler(this.buttonClearTag_Click);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(16, 248);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(344, 23);
			this.label2.TabIndex = 10;
			this.label2.Text = "**Note** Write to Tag writes 1 byte to the 20th byte of the tag";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PanelRFID
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonClearTag);
			this.Controls.Add(this.buttonReadTagID);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.touchscreenTextBoxTagWrite);
			this.Controls.Add(this.buttonWriteTag);
			this.Controls.Add(this.textBoxTagRead);
			this.Controls.Add(this.buttonReadTag);
			this.Controls.Add(this.buttonInitialize);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxComPort);
			this.Name = "PanelRFID";
			this.Size = new System.Drawing.Size(376, 360);
			this.ResumeLayout(false);

		}
		#endregion

		// Properties ----------------------------------------------------------
		// Methods -------------------------------------------------------------
		private void buttonInitialize_Click(object sender, System.EventArgs e)
		{
			try
			{
				_tagReader.InitializeController(_comPort);
				buttonInitialize.Enabled = false;
				labelStatus.ForeColor = Color.Green;
				labelStatus.Text = "Status: Tag Reader Initialized.";
			}
			catch( Exception ex )
			{
				MessageBox.Show("Failed to initialize RFID reader/writer. Error: " + ex.Message);
				labelStatus.ForeColor = Color.Red;
				labelStatus.Text = "Status: Tag Reader Failed to Initialize.";
			}
		}

		private void comboBoxComPort_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				_comPort = comboBoxComPort.SelectedIndex + 1;
				buttonInitialize.Enabled = true;
			}
			catch( Exception ex )
			{
				MessageBox.Show("Failed to change com port. Error: " + ex.Message);
			}		
		}

		private void buttonReadTag_Click(object sender, System.EventArgs e)
		{
			try
			{
				//To Do -- Add Start Address and No Bytes
				textBoxTagRead.Clear();
				if( _numBytesToRead > 0 )
				{
					textBoxTagRead.Text = _tagReader.ReadTag(_readStartAddress,_numBytesToRead);
					labelStatus.ForeColor = Color.Green;
					labelStatus.Text = "Status: Tag Successfully Read";
				}
				else
				{
					labelStatus.ForeColor = Color.Red;
					labelStatus.Text = "Status: Tag Not Read. Num Bytes To Read = 0.";
				}
			}
			catch( Exception ex )
			{
				labelStatus.Text = "Status: Error Reading Tag.";
				labelStatus.ForeColor = Color.Red;
				MessageBox.Show("Failed to read tag. Error: " + ex.Message);
			}
		}

		private void buttonWriteTag_Click(object sender, System.EventArgs e)
		{
			try
			{
				string data = touchscreenTextBoxTagWrite.Text;
				char[] dataChar = data.ToCharArray(0,data.Length);
				byte[] dataByte = new byte[data.Length];
				for( int i=0; i< dataChar.Length; i++)
				{
					dataByte[i] =Convert.ToByte(dataChar[i]);
				}
				//To Do -- Add Start Address and No Bytes
				_tagReader.WriteTag(19,1,dataByte);

				labelStatus.ForeColor = Color.Green;
				labelStatus.Text = "Status: Tag Successfully Written";
			}
			catch( Exception ex )
			{
				labelStatus.ForeColor = Color.Red;
				labelStatus.Text = "Status: Error Writing Tag.";
				MessageBox.Show("Failed to write tag. Error: " + ex.Message);
			}
		}

		private void buttonReadTagID_Click(object sender, System.EventArgs e)
		{
			try
			{
				textBoxTagRead.Clear();
				textBoxTagRead.Text = "Tag ID: " + _tagReader.ReadTagID();

				labelStatus.ForeColor = Color.Green;
				labelStatus.Text = "Status: Tag ID Read";
			}
			catch( Exception ex )
			{
				labelStatus.ForeColor = Color.Red;
				labelStatus.Text = "Status: Error Reading Tag ID.";
				MessageBox.Show("Failed to read tag id. Error: " + ex.Message);
			}
		}

		private void buttonClearTag_Click(object sender, System.EventArgs e)
		{
			try
			{
				DialogResult result = MessageBox.Show("Are you sure you want to clear the tag?","RFID",MessageBoxButtons.YesNo);
				if( result == DialogResult.Yes )
				{
					//Add in start address and such
					_tagReader.ClearTag(_readStartAddress,_numBytesToRead);

					labelStatus.ForeColor = Color.Green;
					labelStatus.Text = "Status: Tag Cleared";
				}
				else
				{
					labelStatus.ForeColor = Color.Red;
					labelStatus.Text = "Status: Tag NOT Cleared";
				}
			}
			catch( Exception ex )
			{
				labelStatus.ForeColor = Color.Red;
				labelStatus.Text = "Status: Error Clearing Tag.";
				MessageBox.Show("Failed to clear tag. Error: " + ex.Message);
			}
		}

	}
}
