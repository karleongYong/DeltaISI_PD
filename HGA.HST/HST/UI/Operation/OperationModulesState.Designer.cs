namespace Seagate.AAS.HGA.HST.UI.Operation
{
    partial class OperationModulesState
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonQuitApp = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonInit = new System.Windows.Forms.Button();
            this.textUnloadTotalTime = new System.Windows.Forms.TextBox();
            this.textUnloadRunTime = new System.Windows.Forms.TextBox();
            this.textUnloadStandbyTime = new System.Windows.Forms.TextBox();
            this.textUnloadDownTime = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dycemCleaningCounter1 = new Seagate.AAS.HGA.HST.UI.Operation.WorkOrder.DycemCleaningCounter();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonQuitApp
            // 
            this.buttonQuitApp.Location = new System.Drawing.Point(31, 23);
            this.buttonQuitApp.Name = "buttonQuitApp";
            this.buttonQuitApp.Size = new System.Drawing.Size(100, 40);
            this.buttonQuitApp.TabIndex = 48;
            this.buttonQuitApp.Text = "Quit";
            this.buttonQuitApp.UseVisualStyleBackColor = true;
            this.buttonQuitApp.Click += new System.EventHandler(this.buttonQuitApp_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonQuitApp);
            this.groupBox3.Location = new System.Drawing.Point(696, 299);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(185, 78);
            this.groupBox3.TabIndex = 49;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Quit Application";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonInit);
            this.groupBox4.Location = new System.Drawing.Point(179, 283);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(185, 78);
            this.groupBox4.TabIndex = 52;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Init";
            this.groupBox4.Visible = false;
            // 
            // buttonInit
            // 
            this.buttonInit.Location = new System.Drawing.Point(43, 23);
            this.buttonInit.Name = "buttonInit";
            this.buttonInit.Size = new System.Drawing.Size(100, 40);
            this.buttonInit.TabIndex = 48;
            this.buttonInit.Text = "Init";
            this.buttonInit.UseVisualStyleBackColor = true;
            this.buttonInit.Visible = false;
            this.buttonInit.Click += new System.EventHandler(this.buttonInit_Click);
            // 
            // textUnloadTotalTime
            // 
            this.textUnloadTotalTime.Location = new System.Drawing.Point(151, 15);
            this.textUnloadTotalTime.Name = "textUnloadTotalTime";
            this.textUnloadTotalTime.Size = new System.Drawing.Size(93, 20);
            this.textUnloadTotalTime.TabIndex = 53;
            // 
            // textUnloadRunTime
            // 
            this.textUnloadRunTime.Location = new System.Drawing.Point(151, 37);
            this.textUnloadRunTime.Name = "textUnloadRunTime";
            this.textUnloadRunTime.Size = new System.Drawing.Size(93, 20);
            this.textUnloadRunTime.TabIndex = 54;
            // 
            // textUnloadStandbyTime
            // 
            this.textUnloadStandbyTime.Location = new System.Drawing.Point(151, 59);
            this.textUnloadStandbyTime.Name = "textUnloadStandbyTime";
            this.textUnloadStandbyTime.Size = new System.Drawing.Size(93, 20);
            this.textUnloadStandbyTime.TabIndex = 55;
            // 
            // textUnloadDownTime
            // 
            this.textUnloadDownTime.Location = new System.Drawing.Point(151, 81);
            this.textUnloadDownTime.Name = "textUnloadDownTime";
            this.textUnloadDownTime.Size = new System.Drawing.Size(93, 20);
            this.textUnloadDownTime.TabIndex = 56;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(62, 18);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(67, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "Total Time";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(65, 39);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(61, 13);
            this.label17.TabIndex = 57;
            this.label17.Text = "Run Time";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(63, 63);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(84, 13);
            this.label18.TabIndex = 58;
            this.label18.Text = "Standby Time";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(64, 84);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(70, 13);
            this.label19.TabIndex = 59;
            this.label19.Text = "Down Time";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(10, 67);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(49, 32);
            this.buttonReset.TabIndex = 60;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonReset);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.textUnloadDownTime);
            this.groupBox5.Controls.Add(this.textUnloadStandbyTime);
            this.groupBox5.Controls.Add(this.textUnloadRunTime);
            this.groupBox5.Controls.Add(this.textUnloadTotalTime);
            this.groupBox5.Location = new System.Drawing.Point(375, 269);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(278, 108);
            this.groupBox5.TabIndex = 61;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Process counter";
            // 
            // measurementTest1
            // 
            this.dycemCleaningCounter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dycemCleaningCounter1.Location = new System.Drawing.Point(375, 104);
            this.dycemCleaningCounter1.Name = "dycemCleaningCounter1";
            this.dycemCleaningCounter1.Size = new System.Drawing.Size(367, 130);
            this.dycemCleaningCounter1.TabIndex = 62;
            // 
            // OperationModulesState
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dycemCleaningCounter1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Name = "OperationModulesState";
            this.Size = new System.Drawing.Size(1145, 640);
            this.Load += new System.EventHandler(this.OperationModulesState_Load);
            this.VisibleChanged += new System.EventHandler(this.OperationModulesState_VisibleChanged);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonQuitApp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonInit;
        private System.Windows.Forms.TextBox textUnloadTotalTime;
        private System.Windows.Forms.TextBox textUnloadRunTime;
        private System.Windows.Forms.TextBox textUnloadStandbyTime;
        private System.Windows.Forms.TextBox textUnloadDownTime;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.GroupBox groupBox5;
        private WorkOrder.DycemCleaningCounter dycemCleaningCounter1;        
    }
}
