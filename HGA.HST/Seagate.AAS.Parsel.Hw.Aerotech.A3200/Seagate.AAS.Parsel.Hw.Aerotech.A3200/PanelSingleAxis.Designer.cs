namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    partial class PanelSingleAxis
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tNumVelocity = new Seagate.AAS.UI.TouchscreenNumBox();
            this.tNumAcceleration = new Seagate.AAS.UI.TouchscreenNumBox();
            this.tbPosition = new Seagate.AAS.UI.TouchscreenNumBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnFaultAck = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.btnFreeRunPositive = new System.Windows.Forms.Button();
            this.btnFreeRunNegative = new System.Windows.Forms.Button();
            this.btnMoveAbs = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnJogPositive = new System.Windows.Forms.Button();
            this.btnJogNegative = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.lbAxisNo = new System.Windows.Forms.Label();
            this.lbPosition = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ledFault = new Seagate.AAS.UI.Led();
            this.ledInPosition = new Seagate.AAS.UI.Led();
            this.ledhomed = new Seagate.AAS.UI.Led();
            this.ledEnable = new Seagate.AAS.UI.Led();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Axis No.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tNumVelocity);
            this.groupBox1.Controls.Add(this.tNumAcceleration);
            this.groupBox1.Controls.Add(this.tbPosition);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Controls.Add(this.btnFaultAck);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.btnMoveAbs);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.btnHome);
            this.groupBox1.Controls.Add(this.lbAxisNo);
            this.groupBox1.Controls.Add(this.lbPosition);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ledFault);
            this.groupBox1.Controls.Add(this.ledInPosition);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ledhomed);
            this.groupBox1.Controls.Add(this.ledEnable);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 305);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Axis Name";
            // 
            // tNumVelocity
            // 
            this.tNumVelocity.BackColor = System.Drawing.Color.Pink;
            this.tNumVelocity.IntegerOnly = false;
            this.tNumVelocity.Location = new System.Drawing.Point(89, 140);
            this.tNumVelocity.Max = 100D;
            this.tNumVelocity.Min = -5D;
            this.tNumVelocity.Name = "tNumVelocity";
            this.tNumVelocity.Size = new System.Drawing.Size(67, 20);
            this.tNumVelocity.TabIndex = 37;
            this.tNumVelocity.Text = "0";
            this.tNumVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tNumAcceleration
            // 
            this.tNumAcceleration.BackColor = System.Drawing.Color.White;
            this.tNumAcceleration.IntegerOnly = false;
            this.tNumAcceleration.Location = new System.Drawing.Point(89, 116);
            this.tNumAcceleration.Max = 100D;
            this.tNumAcceleration.Min = -5D;
            this.tNumAcceleration.Name = "tNumAcceleration";
            this.tNumAcceleration.Size = new System.Drawing.Size(67, 20);
            this.tNumAcceleration.TabIndex = 36;
            this.tNumAcceleration.Text = "0";
            this.tNumAcceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbPosition
            // 
            this.tbPosition.BackColor = System.Drawing.Color.Pink;
            this.tbPosition.IntegerOnly = false;
            this.tbPosition.Location = new System.Drawing.Point(89, 92);
            this.tbPosition.Max = 100D;
            this.tbPosition.Min = -5D;
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(67, 20);
            this.tbPosition.TabIndex = 35;
            this.tbPosition.Text = "0";
            this.tbPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(9, 275);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(253, 23);
            this.labelStatus.TabIndex = 34;
            // 
            // btnFaultAck
            // 
            this.btnFaultAck.Location = new System.Drawing.Point(233, 140);
            this.btnFaultAck.Name = "btnFaultAck";
            this.btnFaultAck.Size = new System.Drawing.Size(37, 23);
            this.btnFaultAck.TabIndex = 33;
            this.btnFaultAck.Text = "Ack.";
            this.btnFaultAck.UseVisualStyleBackColor = true;
            this.btnFaultAck.Click += new System.EventHandler(this.btnFaultAck_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(179, 225);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(83, 38);
            this.btnStop.TabIndex = 30;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.btnFreeRunPositive);
            this.panel2.Controls.Add(this.btnFreeRunNegative);
            this.panel2.Location = new System.Drawing.Point(9, 223);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(160, 43);
            this.panel2.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label7.Location = new System.Drawing.Point(52, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 16);
            this.label7.TabIndex = 24;
            this.label7.Text = "Freerun";
            // 
            // btnFreeRunPositive
            // 
            this.btnFreeRunPositive.BackColor = System.Drawing.SystemColors.Control;
            this.btnFreeRunPositive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnFreeRunPositive.Location = new System.Drawing.Point(110, 5);
            this.btnFreeRunPositive.Name = "btnFreeRunPositive";
            this.btnFreeRunPositive.Size = new System.Drawing.Size(47, 33);
            this.btnFreeRunPositive.TabIndex = 23;
            this.btnFreeRunPositive.Text = "+";
            this.btnFreeRunPositive.UseVisualStyleBackColor = false;
            this.btnFreeRunPositive.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnFreeRunPositive_MouseDown);
            this.btnFreeRunPositive.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnFreeRunPositive_MouseUp);
            // 
            // btnFreeRunNegative
            // 
            this.btnFreeRunNegative.BackColor = System.Drawing.SystemColors.Control;
            this.btnFreeRunNegative.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnFreeRunNegative.Location = new System.Drawing.Point(4, 5);
            this.btnFreeRunNegative.Name = "btnFreeRunNegative";
            this.btnFreeRunNegative.Size = new System.Drawing.Size(47, 33);
            this.btnFreeRunNegative.TabIndex = 22;
            this.btnFreeRunNegative.Text = "-";
            this.btnFreeRunNegative.UseVisualStyleBackColor = false;
            this.btnFreeRunNegative.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnFreeRunNegative_MouseDown);
            this.btnFreeRunNegative.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnFreeRunNegative_MouseUp);
            // 
            // btnMoveAbs
            // 
            this.btnMoveAbs.Location = new System.Drawing.Point(179, 176);
            this.btnMoveAbs.Name = "btnMoveAbs";
            this.btnMoveAbs.Size = new System.Drawing.Size(83, 38);
            this.btnMoveAbs.TabIndex = 28;
            this.btnMoveAbs.Text = "Move Abs.";
            this.btnMoveAbs.UseVisualStyleBackColor = true;
            this.btnMoveAbs.Click += new System.EventHandler(this.btnMoveAbs_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnJogPositive);
            this.panel1.Controls.Add(this.btnJogNegative);
            this.panel1.Location = new System.Drawing.Point(9, 174);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 43);
            this.panel1.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label3.Location = new System.Drawing.Point(64, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 16);
            this.label3.TabIndex = 24;
            this.label3.Text = "Jog";
            // 
            // btnJogPositive
            // 
            this.btnJogPositive.BackColor = System.Drawing.SystemColors.Control;
            this.btnJogPositive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnJogPositive.Location = new System.Drawing.Point(110, 5);
            this.btnJogPositive.Name = "btnJogPositive";
            this.btnJogPositive.Size = new System.Drawing.Size(47, 33);
            this.btnJogPositive.TabIndex = 23;
            this.btnJogPositive.Text = "+";
            this.btnJogPositive.UseVisualStyleBackColor = false;
            this.btnJogPositive.Click += new System.EventHandler(this.btnJogPositive_Click);
            // 
            // btnJogNegative
            // 
            this.btnJogNegative.BackColor = System.Drawing.SystemColors.Control;
            this.btnJogNegative.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnJogNegative.Location = new System.Drawing.Point(4, 5);
            this.btnJogNegative.Name = "btnJogNegative";
            this.btnJogNegative.Size = new System.Drawing.Size(47, 33);
            this.btnJogNegative.TabIndex = 22;
            this.btnJogNegative.Text = "-";
            this.btnJogNegative.UseVisualStyleBackColor = false;
            this.btnJogNegative.Click += new System.EventHandler(this.btnJogNegative_Click);
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(179, 45);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(75, 35);
            this.btnHome.TabIndex = 25;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // lbAxisNo
            // 
            this.lbAxisNo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbAxisNo.Location = new System.Drawing.Point(89, 24);
            this.lbAxisNo.Name = "lbAxisNo";
            this.lbAxisNo.Size = new System.Drawing.Size(67, 21);
            this.lbAxisNo.TabIndex = 24;
            this.lbAxisNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPosition
            // 
            this.lbPosition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbPosition.Location = new System.Drawing.Point(89, 52);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(67, 21);
            this.lbPosition.TabIndex = 21;
            this.lbPosition.Text = "position";
            this.lbPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 16);
            this.label5.TabIndex = 20;
            this.label5.Text = "Velocity";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "Acceleration";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "Position";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Actual Position";
            // 
            // ledFault
            // 
            this.ledFault.DisplayAsButton = false;
            this.ledFault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.ledFault.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ledFault.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledFault.Location = new System.Drawing.Point(179, 140);
            this.ledFault.Name = "ledFault";
            this.ledFault.Size = new System.Drawing.Size(75, 23);
            this.ledFault.State = false;
            this.ledFault.TabIndex = 8;
            this.ledFault.Text = "Fault";
            // 
            // ledInPosition
            // 
            this.ledInPosition.DisplayAsButton = false;
            this.ledInPosition.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledInPosition.Location = new System.Drawing.Point(179, 116);
            this.ledInPosition.Name = "ledInPosition";
            this.ledInPosition.Size = new System.Drawing.Size(75, 23);
            this.ledInPosition.State = false;
            this.ledInPosition.TabIndex = 7;
            this.ledInPosition.Text = "Inposition";
            // 
            // ledhomed
            // 
            this.ledhomed.DisplayAsButton = false;
            this.ledhomed.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledhomed.Location = new System.Drawing.Point(179, 92);
            this.ledhomed.Name = "ledhomed";
            this.ledhomed.Size = new System.Drawing.Size(75, 23);
            this.ledhomed.State = false;
            this.ledhomed.TabIndex = 6;
            this.ledhomed.Text = "Homed";
            // 
            // ledEnable
            // 
            this.ledEnable.DisplayAsButton = true;
            this.ledEnable.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledEnable.Location = new System.Drawing.Point(179, 16);
            this.ledEnable.Name = "ledEnable";
            this.ledEnable.Size = new System.Drawing.Size(75, 25);
            this.ledEnable.State = false;
            this.ledEnable.TabIndex = 5;
            this.ledEnable.Text = "Enable";
            this.ledEnable.Click += new System.EventHandler(this.ledEnable_Click);
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // PanelSingleAxis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PanelSingleAxis";
            this.Size = new System.Drawing.Size(282, 311);
            this.VisibleChanged += new System.EventHandler(this.PanelSingleAxis_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Seagate.AAS.UI.Led ledEnable;
        private Seagate.AAS.UI.Led ledhomed;
        private Seagate.AAS.UI.Led ledInPosition;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private AAS.UI.Led ledFault;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbPosition;
        private System.Windows.Forms.Button btnJogPositive;
        private System.Windows.Forms.Button btnJogNegative;
        private System.Windows.Forms.Label lbAxisNo;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnFreeRunPositive;
        private System.Windows.Forms.Button btnFreeRunNegative;
        private System.Windows.Forms.Button btnMoveAbs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnFaultAck;
        private System.Windows.Forms.Label labelStatus;
        private AAS.UI.TouchscreenNumBox tNumVelocity;
        private AAS.UI.TouchscreenNumBox tNumAcceleration;
        private AAS.UI.TouchscreenNumBox tbPosition;
    }
}
