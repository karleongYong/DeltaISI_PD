namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    partial class RFIDPanel
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
            this.RFIDPanel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdxInputStation = new System.Windows.Forms.RadioButton();
            this.rdxOutputStation = new System.Windows.Forms.RadioButton();
            this.btReadFola = new System.Windows.Forms.Button();
            this.btWriteFola = new System.Windows.Forms.Button();
            this.lblFolaStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.folaTagDataPanel1 = new Seagate.AAS.Parsel.Device.RFID.Hga.FolaTagDataPanel();
            this.RFIDPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RFIDPanel1
            // 
            this.RFIDPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.RFIDPanel1.Controls.Add(this.groupBox1);
            this.RFIDPanel1.Controls.Add(this.btReadFola);
            this.RFIDPanel1.Controls.Add(this.btWriteFola);
            this.RFIDPanel1.Controls.Add(this.lblFolaStatus);
            this.RFIDPanel1.Controls.Add(this.label2);
            this.RFIDPanel1.Controls.Add(this.folaTagDataPanel1);
            this.RFIDPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RFIDPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RFIDPanel1.Location = new System.Drawing.Point(0, 0);
            this.RFIDPanel1.Name = "RFIDPanel1";
            this.RFIDPanel1.Size = new System.Drawing.Size(699, 625);
            this.RFIDPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdxInputStation);
            this.groupBox1.Controls.Add(this.rdxOutputStation);
            this.groupBox1.Location = new System.Drawing.Point(12, 451);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 53);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
            // 
            // rdxInputStation
            // 
            this.rdxInputStation.AutoSize = true;
            this.rdxInputStation.Checked = true;
            this.rdxInputStation.Location = new System.Drawing.Point(18, 22);
            this.rdxInputStation.Name = "rdxInputStation";
            this.rdxInputStation.Size = new System.Drawing.Size(85, 17);
            this.rdxInputStation.TabIndex = 0;
            this.rdxInputStation.TabStop = true;
            this.rdxInputStation.Text = "Input Station";
            this.rdxInputStation.UseVisualStyleBackColor = true;
            // 
            // rdxOutputStation
            // 
            this.rdxOutputStation.AutoSize = true;
            this.rdxOutputStation.Location = new System.Drawing.Point(137, 22);
            this.rdxOutputStation.Name = "rdxOutputStation";
            this.rdxOutputStation.Size = new System.Drawing.Size(93, 17);
            this.rdxOutputStation.TabIndex = 1;
            this.rdxOutputStation.Text = "Output Station";
            this.rdxOutputStation.UseVisualStyleBackColor = true;
            // 
            // btReadFola
            // 
            this.btReadFola.BackColor = System.Drawing.SystemColors.Control;
            this.btReadFola.Location = new System.Drawing.Point(286, 462);
            this.btReadFola.Name = "btReadFola";
            this.btReadFola.Size = new System.Drawing.Size(81, 38);
            this.btReadFola.TabIndex = 22;
            this.btReadFola.Text = "Read Tag";
            this.btReadFola.UseVisualStyleBackColor = false;
            this.btReadFola.Click += new System.EventHandler(this.btReadFola_Click);
            // 
            // btWriteFola
            // 
            this.btWriteFola.BackColor = System.Drawing.SystemColors.Control;
            this.btWriteFola.Location = new System.Drawing.Point(373, 462);
            this.btWriteFola.Name = "btWriteFola";
            this.btWriteFola.Size = new System.Drawing.Size(72, 38);
            this.btWriteFola.TabIndex = 23;
            this.btWriteFola.Text = "Write Tag";
            this.btWriteFola.UseVisualStyleBackColor = false;
            this.btWriteFola.Click += new System.EventHandler(this.btWriteFola_Click);
            // 
            // lblFolaStatus
            // 
            this.lblFolaStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFolaStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblFolaStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFolaStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFolaStatus.Location = new System.Drawing.Point(3, 507);
            this.lblFolaStatus.Name = "lblFolaStatus";
            this.lblFolaStatus.Size = new System.Drawing.Size(673, 33);
            this.lblFolaStatus.TabIndex = 18;
            this.lblFolaStatus.Text = "Status: OK";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(9, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "FOLA RFID";
            // 
            // folaTagDataPanel1
            // 
            this.folaTagDataPanel1.BackColor = System.Drawing.Color.Transparent;
            this.folaTagDataPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.folaTagDataPanel1.Location = new System.Drawing.Point(6, 30);
            this.folaTagDataPanel1.Name = "folaTagDataPanel1";
            this.folaTagDataPanel1.ReadOnly = true;
            this.folaTagDataPanel1.Size = new System.Drawing.Size(485, 485);
            this.folaTagDataPanel1.TabIndex = 1;
            // 
            // RFIDPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RFIDPanel1);
            this.Name = "RFIDPanel";
            this.Size = new System.Drawing.Size(699, 625);
            this.Load += new System.EventHandler(this.RFIDPanel_Load);
            this.RFIDPanel1.ResumeLayout(false);
            this.RFIDPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel RFIDPanel1;          
        private Parsel.Device.RFID.Hga.FolaTagDataPanel folaTagDataPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFolaStatus;
        private System.Windows.Forms.Button btReadFola;
        private System.Windows.Forms.Button btWriteFola;       
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdxInputStation;
        private System.Windows.Forms.RadioButton rdxOutputStation;

    }
}
