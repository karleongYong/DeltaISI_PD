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
//  [9/2/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Seagate.AAS.Utils.PointStore;

namespace Seagate.AAS.Utils
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{

        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        Stopwatch timer = new Stopwatch();
        MySettings mySettings = new MySettings();
        Seagate.AAS.Utils.PersistentMDB myMDB = new Seagate.AAS.Utils.PersistentMDB();
        PointStore.PointStore pointStore = new PointStore.PointStore();
        
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTimerValue;
        private System.Windows.Forms.CheckBox cbTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelDateTime;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.TextBox tbMDBTableName;
        private System.Windows.Forms.TextBox tbMDBFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSaveMDB;
        private System.Windows.Forms.Button buttonOpenMDB;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Button buttonSavePointStore;
        private System.Windows.Forms.Button buttonLoadPointStore;
        private Seagate.AAS.Utils.PointStore.PointStoreViewer pointStoreViewer1;
        private System.Windows.Forms.Button buttonCreatePointStore;
        private System.Windows.Forms.TabPage tabPage4;
        private Seagate.AAS.Utils.ProcessViewer.ProcessViewer processViewer1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox textPlainText;
        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.TextBox textEncrypted;
        private System.Windows.Forms.Button buttonDecrypt;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelEncDecResult;
        private System.ComponentModel.IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            //pointStoreViewer1.AssignPointStore(pointStore);
            processViewer1.Filter = new string[2] {"viper", "seagate"};
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.labelTimerValue = new System.Windows.Forms.Label();
            this.cbTimer = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelDateTime = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbMDBTableName = new System.Windows.Forms.TextBox();
            this.tbMDBFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSaveMDB = new System.Windows.Forms.Button();
            this.buttonOpenMDB = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonLoadPointStore = new System.Windows.Forms.Button();
            this.buttonCreatePointStore = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.processViewer1 = new Seagate.AAS.Utils.ProcessViewer.ProcessViewer();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.labelEncDecResult = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.buttonDecrypt = new System.Windows.Forms.Button();
            this.textEncrypted = new System.Windows.Forms.TextBox();
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.textPlainText = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(40, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Stopwatch Value";
            // 
            // labelTimerValue
            // 
            this.labelTimerValue.Location = new System.Drawing.Point(144, 56);
            this.labelTimerValue.Name = "labelTimerValue";
            this.labelTimerValue.Size = new System.Drawing.Size(80, 16);
            this.labelTimerValue.TabIndex = 1;
            // 
            // cbTimer
            // 
            this.cbTimer.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbTimer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbTimer.Location = new System.Drawing.Point(240, 48);
            this.cbTimer.Name = "cbTimer";
            this.cbTimer.Size = new System.Drawing.Size(72, 40);
            this.cbTimer.TabIndex = 3;
            this.cbTimer.Text = "Start Stopwatch";
            this.cbTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbTimer.CheckedChanged += new System.EventHandler(this.cbTimer_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Current Date/Time";
            // 
            // labelDateTime
            // 
            this.labelDateTime.Location = new System.Drawing.Point(144, 8);
            this.labelDateTime.Name = "labelDateTime";
            this.labelDateTime.Size = new System.Drawing.Size(248, 16);
            this.labelDateTime.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(16, 96);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(520, 368);
            this.tabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.buttonSave);
            this.tabPage1.Controls.Add(this.buttonLoad);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(512, 342);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "PersistentXML";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(8, 16);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(392, 312);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSave.Location = new System.Drawing.Point(424, 24);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(72, 32);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save MySettings";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLoad.Location = new System.Drawing.Point(424, 96);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(72, 32);
            this.buttonLoad.TabIndex = 7;
            this.buttonLoad.Text = "Load MySettings";
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbMDBTableName);
            this.tabPage2.Controls.Add(this.tbMDBFileName);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.buttonSaveMDB);
            this.tabPage2.Controls.Add(this.buttonOpenMDB);
            this.tabPage2.Controls.Add(this.dataGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(512, 342);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "PersistentMDB";
            // 
            // tbMDBTableName
            // 
            this.tbMDBTableName.Location = new System.Drawing.Point(140, 31);
            this.tbMDBTableName.Name = "tbMDBTableName";
            this.tbMDBTableName.Size = new System.Drawing.Size(120, 20);
            this.tbMDBTableName.TabIndex = 22;
            this.tbMDBTableName.Text = "PhoneBook";
            // 
            // tbMDBFileName
            // 
            this.tbMDBFileName.Location = new System.Drawing.Point(140, 7);
            this.tbMDBFileName.Name = "tbMDBFileName";
            this.tbMDBFileName.Size = new System.Drawing.Size(120, 20);
            this.tbMDBFileName.TabIndex = 19;
            this.tbMDBFileName.Text = "..\\..\\db1.mdb";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(44, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 21;
            this.label4.Text = "Table Name";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(44, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 20;
            this.label3.Text = "File Name";
            // 
            // buttonSaveMDB
            // 
            this.buttonSaveMDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveMDB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSaveMDB.Location = new System.Drawing.Point(432, 144);
            this.buttonSaveMDB.Name = "buttonSaveMDB";
            this.buttonSaveMDB.Size = new System.Drawing.Size(72, 32);
            this.buttonSaveMDB.TabIndex = 18;
            this.buttonSaveMDB.Text = "Save MDB";
            this.buttonSaveMDB.Click += new System.EventHandler(this.buttonSaveMDB_Click);
            // 
            // buttonOpenMDB
            // 
            this.buttonOpenMDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenMDB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOpenMDB.Location = new System.Drawing.Point(432, 96);
            this.buttonOpenMDB.Name = "buttonOpenMDB";
            this.buttonOpenMDB.Size = new System.Drawing.Size(72, 32);
            this.buttonOpenMDB.TabIndex = 17;
            this.buttonOpenMDB.Text = "Open MDB and Table";
            this.buttonOpenMDB.Click += new System.EventHandler(this.buttonOpenMDB_Click);
            // 
            // dataGrid1
            // 
            this.dataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(16, 72);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(400, 241);
            this.dataGrid1.TabIndex = 16;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonLoadPointStore);
            this.tabPage3.Controls.Add(this.buttonCreatePointStore);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(512, 342);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PointStore";
            // 
            // buttonLoadPointStore
            // 
            this.buttonLoadPointStore.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLoadPointStore.Location = new System.Drawing.Point(144, 16);
            this.buttonLoadPointStore.Name = "buttonLoadPointStore";
            this.buttonLoadPointStore.Size = new System.Drawing.Size(88, 48);
            this.buttonLoadPointStore.TabIndex = 1;
            this.buttonLoadPointStore.Text = "Load PointStore";
            this.buttonLoadPointStore.Click += new System.EventHandler(this.buttonLoadPointStore_Click);
            // 
            // buttonCreatePointStore
            // 
            this.buttonCreatePointStore.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreatePointStore.Location = new System.Drawing.Point(24, 16);
            this.buttonCreatePointStore.Name = "buttonCreatePointStore";
            this.buttonCreatePointStore.Size = new System.Drawing.Size(96, 48);
            this.buttonCreatePointStore.TabIndex = 3;
            this.buttonCreatePointStore.Text = "Create Point Store";
            this.buttonCreatePointStore.Click += new System.EventHandler(this.buttonCreatePointStore_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.processViewer1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(512, 342);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "ProcessViewer";
            // 
            // processViewer1
            // 
            this.processViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processViewer1.Location = new System.Drawing.Point(0, 0);
            this.processViewer1.Name = "processViewer1";
            this.processViewer1.Size = new System.Drawing.Size(512, 342);
            this.processViewer1.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.labelEncDecResult);
            this.tabPage5.Controls.Add(this.label8);
            this.tabPage5.Controls.Add(this.label7);
            this.tabPage5.Controls.Add(this.label6);
            this.tabPage5.Controls.Add(this.label5);
            this.tabPage5.Controls.Add(this.textPassword);
            this.tabPage5.Controls.Add(this.buttonDecrypt);
            this.tabPage5.Controls.Add(this.textEncrypted);
            this.tabPage5.Controls.Add(this.buttonEncrypt);
            this.tabPage5.Controls.Add(this.textPlainText);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(512, 342);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Encryption";
            // 
            // labelEncDecResult
            // 
            this.labelEncDecResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelEncDecResult.Location = new System.Drawing.Point(112, 176);
            this.labelEncDecResult.Name = "labelEncDecResult";
            this.labelEncDecResult.Size = new System.Drawing.Size(232, 64);
            this.labelEncDecResult.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(32, 176);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "Result";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(32, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Encrpyted";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(32, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "Plain Text";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(56, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Password";
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(136, 16);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(144, 20);
            this.textPassword.TabIndex = 5;
            this.textPassword.Text = "seagate";
            // 
            // buttonDecrypt
            // 
            this.buttonDecrypt.Location = new System.Drawing.Point(384, 104);
            this.buttonDecrypt.Name = "buttonDecrypt";
            this.buttonDecrypt.Size = new System.Drawing.Size(96, 32);
            this.buttonDecrypt.TabIndex = 4;
            this.buttonDecrypt.Text = "Decrypt";
            this.buttonDecrypt.Click += new System.EventHandler(this.buttonDecrypt_Click);
            // 
            // textEncrypted
            // 
            this.textEncrypted.Location = new System.Drawing.Point(112, 112);
            this.textEncrypted.Multiline = true;
            this.textEncrypted.Name = "textEncrypted";
            this.textEncrypted.Size = new System.Drawing.Size(232, 56);
            this.textEncrypted.TabIndex = 3;
            this.textEncrypted.Text = "";
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Location = new System.Drawing.Point(384, 56);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(96, 32);
            this.buttonEncrypt.TabIndex = 2;
            this.buttonEncrypt.Text = "Encrypt";
            this.buttonEncrypt.Click += new System.EventHandler(this.buttonEncrypt_Click);
            // 
            // textPlainText
            // 
            this.textPlainText.Location = new System.Drawing.Point(112, 64);
            this.textPlainText.Name = "textPlainText";
            this.textPlainText.Size = new System.Drawing.Size(232, 20);
            this.textPlainText.TabIndex = 0;
            this.textPlainText.Text = "Hello World";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(552, 478);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelDateTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbTimer);
            this.Controls.Add(this.labelTimerValue);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Test App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            Application.EnableVisualStyles(); 
            Application.DoEvents(); 			
            Application.Run(new Form1());
		}

        private void Form1_Load(object sender, System.EventArgs e)
        {
            toolTip1.SetToolTip(this.tbMDBFileName, "name of a mdb file");
            toolTip1.SetToolTip(this.tbMDBTableName, "If File Name is db1.mdb, use PhoneBook or Addresses");        
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            labelTimerValue.Text = timer.ElapsedTime_sec.ToString();
            labelDateTime.Text = DateTime.Now.ToString();
        }

        private void cbTimer_CheckedChanged(object sender, System.EventArgs e)
        {
            if (cbTimer.Checked)
            {
                cbTimer.Text = "Stop Stopwatch";
                timer.Start();
            }
            else
            {
                cbTimer.Text = "Start Stopwatch";
                timer.Stop();
            }
        }

        private void buttonLoad_Click(object sender, System.EventArgs e)
        {
            mySettings.Load();

            textBox1.Clear();
            textBox1.Text = mySettings.ToString();

            this.Location = mySettings.FormLocation;
        
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {

            mySettings.FormLocation = this.Location;
            mySettings.TimerValue = timer.ElapsedTime_sec;
            mySettings.TimeStamp = DateTime.Now;
            mySettings.ClearMyConfig();
            for (int i=0; i<10; i++)
            {
                mySettings.AddMyConfig("hello", DateTime.Now.Ticks);
                Thread.Sleep(10);
            }

            mySettings.Save();
        
            textBox1.Clear();
            textBox1.Text = mySettings.ToString();
        }


        // MDB Tests ---------------------------------------
        private void buttonOpenMDB_Click(object sender, System.EventArgs e)
        {
            // specify mdb file name
            // table to load
            string fileName = tbMDBFileName.Text; 
            string[] tableName = new string[1];
            tableName[0] = tbMDBTableName.Text;
            try
            {
                myMDB.Load(fileName, tableName);

                // bind table to grid
                dataGrid1.CaptionText = tableName[0];
                dataGrid1.SetDataBinding(myMDB.DataSet, tableName[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void buttonSaveMDB_Click(object sender, System.EventArgs e)
        {
            try
            {
                myMDB.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        static int testIncrement = 0;
        private void buttonCreatePointStore_Click(object sender, System.EventArgs e)
        {

            Random random = new Random();

            testIncrement++;

            pointStore.CreatePoint("Gantry", 10, "X", "Y", "Z");
            pointStore.CreatePoint("Elevator", 5, "Z");

            for (int i=0; i<10; i++)
            {
                double[] gantryCoordinate = new double[3];
                for (int dimension=0; dimension<3; dimension++)
                {
                    gantryCoordinate[dimension] = (9.5*(dimension+1)+random.NextDouble());
                }
                pointStore.GetPoint("Gantry").AddRobotPoint(gantryCoordinate);

                pointStore.GetPoint("Elevator").AddRobotPoint(10+5*random.NextDouble());
                Thread.Sleep(100);
            }


            pointStore.Save();
        }

        private void buttonLoadPointStore_Click(object sender, System.EventArgs e)
        {
            pointStore.Load();
        }

        private void buttonEncrypt_Click(object sender, System.EventArgs e)
        {
            try
            {
                textEncrypted.Text = Cryptography.Encrypt(textPlainText.Text, textPassword.Text);
                labelEncDecResult.Text = "Encrypted successfully";
            }
            catch (Exception ex)
            {
                textEncrypted.Text = "";
                labelEncDecResult.Text = "Encrypt Failed: " + ex.Message;
            }
        }

        private void buttonDecrypt_Click(object sender, System.EventArgs e)
        {
            try
            {
                textPlainText.Text = Cryptography.Decrypt(textEncrypted.Text, textPassword.Text);
                labelEncDecResult.Text = "Decrypted successfully";
            }
            catch (Exception ex)
            {
                textPlainText.Text = "";
                labelEncDecResult.Text = "Decrypt Failed: " + ex.Message;
            }
        }

	}
}
