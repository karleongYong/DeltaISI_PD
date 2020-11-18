namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    partial class PanelTask
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
            this.label5 = new System.Windows.Forms.Label();
            this.lbLoadedProgram = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbState = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRunProgram = new System.Windows.Forms.Button();
            this.btnStopProgram = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Loaded Program";
            // 
            // lbLoadedProgram
            // 
            this.lbLoadedProgram.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbLoadedProgram.Location = new System.Drawing.Point(101, 73);
            this.lbLoadedProgram.Name = "lbLoadedProgram";
            this.lbLoadedProgram.Size = new System.Drawing.Size(229, 21);
            this.lbLoadedProgram.TabIndex = 30;
            this.lbLoadedProgram.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Status";
            // 
            // lbStatus
            // 
            this.lbStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbStatus.Location = new System.Drawing.Point(101, 48);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(229, 21);
            this.lbStatus.TabIndex = 28;
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbState
            // 
            this.lbState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbState.Location = new System.Drawing.Point(101, 23);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(229, 21);
            this.lbState.TabIndex = 27;
            this.lbState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "State";
            // 
            // btnRunProgram
            // 
            this.btnRunProgram.Location = new System.Drawing.Point(101, 108);
            this.btnRunProgram.Name = "btnRunProgram";
            this.btnRunProgram.Size = new System.Drawing.Size(91, 44);
            this.btnRunProgram.TabIndex = 32;
            this.btnRunProgram.Text = "Run";
            this.btnRunProgram.UseVisualStyleBackColor = true;
            this.btnRunProgram.Click += new System.EventHandler(this.btnRunProgram_Click);
            // 
            // btnStopProgram
            // 
            this.btnStopProgram.Location = new System.Drawing.Point(206, 108);
            this.btnStopProgram.Name = "btnStopProgram";
            this.btnStopProgram.Size = new System.Drawing.Size(91, 44);
            this.btnStopProgram.TabIndex = 33;
            this.btnStopProgram.Text = "Stop";
            this.btnStopProgram.UseVisualStyleBackColor = true;
            this.btnStopProgram.Click += new System.EventHandler(this.btnStopProgram_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbState);
            this.groupBox1.Controls.Add(this.btnRunProgram);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnStopProgram);
            this.groupBox1.Controls.Add(this.lbStatus);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lbLoadedProgram);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 164);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Task Name";
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // PanelTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PanelTask";
            this.Size = new System.Drawing.Size(345, 173);
            this.VisibleChanged += new System.EventHandler(this.PanelTask_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbLoadedProgram;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRunProgram;
        private System.Windows.Forms.Button btnStopProgram;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
