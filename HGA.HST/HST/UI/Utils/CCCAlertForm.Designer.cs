namespace Seagate.AAS.HGA.HST.UI.Utils
{
    partial class CCCAlertForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTopic = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.buttonTICDefect = new System.Windows.Forms.Button();
            this.buttonAcknowledge = new System.Windows.Forms.Button();
            this.buttonHSTDefect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonReleaseCarrier = new System.Windows.Forms.Button();
            this.labelUticFailType = new System.Windows.Forms.Label();
            this.labelDockNumber = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.labelTopic);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(506, 86);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.label2.Font = new System.Drawing.Font("Impact", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 80);
            this.label2.TabIndex = 1;
            this.label2.Text = " ! ";
            // 
            // labelTopic
            // 
            this.labelTopic.AutoSize = true;
            this.labelTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopic.Location = new System.Drawing.Point(101, 23);
            this.labelTopic.Name = "labelTopic";
            this.labelTopic.Size = new System.Drawing.Size(335, 37);
            this.labelTopic.TabIndex = 0;
            this.labelTopic.Text = "ANC Triggering Alert";
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.Color.White;
            this.labelMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(0, 90);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(506, 155);
            this.labelMessage.TabIndex = 1;
            this.labelMessage.Text = "Please inspect on Tail pad and TIC joint.\r\nThen select bellow button in the cause" +
    " of defect that you see.";
            // 
            // buttonTICDefect
            // 
            this.buttonTICDefect.BackColor = System.Drawing.SystemColors.Control;
            this.buttonTICDefect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTICDefect.Location = new System.Drawing.Point(90, 275);
            this.buttonTICDefect.Name = "buttonTICDefect";
            this.buttonTICDefect.Size = new System.Drawing.Size(122, 51);
            this.buttonTICDefect.TabIndex = 2;
            this.buttonTICDefect.Text = "TIC Defect";
            this.buttonTICDefect.UseVisualStyleBackColor = false;
            this.buttonTICDefect.Click += new System.EventHandler(this.buttonTICDefect_Click);
            // 
            // buttonAcknowledge
            // 
            this.buttonAcknowledge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonAcknowledge.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAcknowledge.Location = new System.Drawing.Point(193, 275);
            this.buttonAcknowledge.Name = "buttonAcknowledge";
            this.buttonAcknowledge.Size = new System.Drawing.Size(148, 51);
            this.buttonAcknowledge.TabIndex = 3;
            this.buttonAcknowledge.Text = "Acknowledge";
            this.buttonAcknowledge.UseVisualStyleBackColor = false;
            this.buttonAcknowledge.Click += new System.EventHandler(this.buttonAcknowledge_Click);
            // 
            // buttonHSTDefect
            // 
            this.buttonHSTDefect.BackColor = System.Drawing.SystemColors.Control;
            this.buttonHSTDefect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHSTDefect.Location = new System.Drawing.Point(314, 275);
            this.buttonHSTDefect.Name = "buttonHSTDefect";
            this.buttonHSTDefect.Size = new System.Drawing.Size(122, 51);
            this.buttonHSTDefect.TabIndex = 4;
            this.buttonHSTDefect.Text = "HST Defect";
            this.buttonHSTDefect.UseVisualStyleBackColor = false;
            this.buttonHSTDefect.Click += new System.EventHandler(this.buttonHSTDefect_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonReleaseCarrier
            // 
            this.buttonReleaseCarrier.BackColor = System.Drawing.SystemColors.Control;
            this.buttonReleaseCarrier.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonReleaseCarrier.Location = new System.Drawing.Point(183, 270);
            this.buttonReleaseCarrier.Name = "buttonReleaseCarrier";
            this.buttonReleaseCarrier.Size = new System.Drawing.Size(147, 61);
            this.buttonReleaseCarrier.TabIndex = 5;
            this.buttonReleaseCarrier.Text = "Release Carrier To Inspect";
            this.buttonReleaseCarrier.UseVisualStyleBackColor = false;
            this.buttonReleaseCarrier.Click += new System.EventHandler(this.buttonReleaseCarrier_Click);
            // 
            // labelUticFailType
            // 
            this.labelUticFailType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.labelUticFailType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelUticFailType.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUticFailType.Location = new System.Drawing.Point(27, 162);
            this.labelUticFailType.Name = "labelUticFailType";
            this.labelUticFailType.Size = new System.Drawing.Size(139, 34);
            this.labelUticFailType.TabIndex = 6;
            this.labelUticFailType.Text = "FAIL_ANC";
            this.labelUticFailType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelUticFailType.Visible = false;
            // 
            // labelDockNumber
            // 
            this.labelDockNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.labelDockNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDockNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDockNumber.Location = new System.Drawing.Point(27, 204);
            this.labelDockNumber.Name = "labelDockNumber";
            this.labelDockNumber.Size = new System.Drawing.Size(139, 34);
            this.labelDockNumber.TabIndex = 7;
            this.labelDockNumber.Text = "UTIC Dock : 1";
            this.labelDockNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDockNumber.Visible = false;
            // 
            // CCCAlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(506, 349);
            this.ControlBox = false;
            this.Controls.Add(this.labelDockNumber);
            this.Controls.Add(this.labelUticFailType);
            this.Controls.Add(this.buttonReleaseCarrier);
            this.Controls.Add(this.buttonHSTDefect);
            this.Controls.Add(this.buttonAcknowledge);
            this.Controls.Add(this.buttonTICDefect);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.panel1);
            this.Name = "CCCAlertForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ANCAlertForm";
            this.VisibleChanged += new System.EventHandler(this.CCCAlertForm_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTopic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonTICDefect;
        private System.Windows.Forms.Button buttonAcknowledge;
        private System.Windows.Forms.Button buttonHSTDefect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonReleaseCarrier;
        private System.Windows.Forms.Label labelUticFailType;
        private System.Windows.Forms.Label labelDockNumber;
    }
}