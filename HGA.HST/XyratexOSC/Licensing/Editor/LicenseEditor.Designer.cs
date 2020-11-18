namespace XyratexOSC.Licensing.Editor
{
    partial class LicenseEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseEditor));
            this.pnlFeatures = new System.Windows.Forms.Panel();
            this.lblValue = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnReadFeatures = new System.Windows.Forms.Button();
            this.btnProgram = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAll = new System.Windows.Forms.CheckBox();
            this.clbFeatures = new System.Windows.Forms.CheckedListBox();
            this.pnlConnect = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.lblKeyStatus = new System.Windows.Forms.Label();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblKeyDetails = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlFeatures.SuspendLayout();
            this.pnlConnect.SuspendLayout();
            this.pnlControls.SuspendLayout();
            this.layoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFeatures
            // 
            this.pnlFeatures.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlFeatures.Controls.Add(this.lblValue);
            this.pnlFeatures.Controls.Add(this.btnDisconnect);
            this.pnlFeatures.Controls.Add(this.btnReadFeatures);
            this.pnlFeatures.Controls.Add(this.btnProgram);
            this.pnlFeatures.Controls.Add(this.label1);
            this.pnlFeatures.Controls.Add(this.cbAll);
            this.pnlFeatures.Controls.Add(this.clbFeatures);
            this.pnlFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFeatures.Location = new System.Drawing.Point(0, 82);
            this.pnlFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.pnlFeatures.Name = "pnlFeatures";
            this.pnlFeatures.Size = new System.Drawing.Size(257, 400);
            this.pnlFeatures.TabIndex = 0;
            this.pnlFeatures.Visible = false;
            // 
            // lblValue
            // 
            this.lblValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValue.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(6, 309);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(150, 30);
            this.lblValue.TabIndex = 8;
            this.lblValue.Text = "(0x0000)";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisconnect.Location = new System.Drawing.Point(163, 342);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(89, 49);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnReadFeatures
            // 
            this.btnReadFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadFeatures.Image = global::XyratexOSC.Licensing.Editor.Properties.Resources.icon_refresh;
            this.btnReadFeatures.Location = new System.Drawing.Point(202, 13);
            this.btnReadFeatures.Name = "btnReadFeatures";
            this.btnReadFeatures.Size = new System.Drawing.Size(32, 32);
            this.btnReadFeatures.TabIndex = 5;
            this.toolTip.SetToolTip(this.btnReadFeatures, "Read Current Key Features");
            this.btnReadFeatures.UseVisualStyleBackColor = true;
            this.btnReadFeatures.Click += new System.EventHandler(this.btnReadFeatures_Click);
            // 
            // btnProgram
            // 
            this.btnProgram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProgram.Location = new System.Drawing.Point(6, 342);
            this.btnProgram.Name = "btnProgram";
            this.btnProgram.Size = new System.Drawing.Size(150, 49);
            this.btnProgram.TabIndex = 3;
            this.btnProgram.Text = "Program Features";
            this.btnProgram.UseVisualStyleBackColor = true;
            this.btnProgram.Click += new System.EventHandler(this.btnProgram_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select Features";
            // 
            // cbAll
            // 
            this.cbAll.AutoSize = true;
            this.cbAll.ForeColor = System.Drawing.Color.SteelBlue;
            this.cbAll.Location = new System.Drawing.Point(50, 51);
            this.cbAll.Name = "cbAll";
            this.cbAll.Size = new System.Drawing.Size(41, 21);
            this.cbAll.TabIndex = 1;
            this.cbAll.Text = "All";
            this.cbAll.UseVisualStyleBackColor = true;
            this.cbAll.CheckedChanged += new System.EventHandler(this.cbAll_CheckedChanged);
            // 
            // clbFeatures
            // 
            this.clbFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clbFeatures.BackColor = System.Drawing.Color.Gainsboro;
            this.clbFeatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbFeatures.CheckOnClick = true;
            this.clbFeatures.FormattingEnabled = true;
            this.clbFeatures.Items.AddRange(new object[] {
            "ABS",
            "POLE",
            "BACKPAD",
            "CE",
            "OCR",
            "LONGSIDES",
            "DEPO"});
            this.clbFeatures.Location = new System.Drawing.Point(49, 73);
            this.clbFeatures.Name = "clbFeatures";
            this.clbFeatures.Size = new System.Drawing.Size(177, 220);
            this.clbFeatures.TabIndex = 0;
            this.clbFeatures.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbFeatures_ItemCheck);
            // 
            // pnlConnect
            // 
            this.pnlConnect.BackColor = System.Drawing.Color.MistyRose;
            this.pnlConnect.Controls.Add(this.btnConnect);
            this.pnlConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConnect.Location = new System.Drawing.Point(0, 482);
            this.pnlConnect.Margin = new System.Windows.Forms.Padding(0);
            this.pnlConnect.Name = "pnlConnect";
            this.pnlConnect.Size = new System.Drawing.Size(257, 400);
            this.pnlConnect.TabIndex = 6;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.BackColor = System.Drawing.Color.Honeydew;
            this.btnConnect.Location = new System.Drawing.Point(22, 177);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(214, 49);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.lblKeyStatus);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Margin = new System.Windows.Forms.Padding(0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(257, 48);
            this.pnlControls.TabIndex = 1;
            // 
            // lblKeyStatus
            // 
            this.lblKeyStatus.BackColor = System.Drawing.Color.MistyRose;
            this.lblKeyStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblKeyStatus.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeyStatus.ForeColor = System.Drawing.Color.IndianRed;
            this.lblKeyStatus.Location = new System.Drawing.Point(0, 0);
            this.lblKeyStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lblKeyStatus.Name = "lblKeyStatus";
            this.lblKeyStatus.Size = new System.Drawing.Size(257, 48);
            this.lblKeyStatus.TabIndex = 0;
            this.lblKeyStatus.Text = "Key Disconnected";
            this.lblKeyStatus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // layoutPanel
            // 
            this.layoutPanel.ColumnCount = 1;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.Controls.Add(this.pnlConnect, 0, 3);
            this.layoutPanel.Controls.Add(this.pnlControls, 0, 0);
            this.layoutPanel.Controls.Add(this.pnlFeatures, 0, 2);
            this.layoutPanel.Controls.Add(this.lblKeyDetails, 0, 1);
            this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 4;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.Size = new System.Drawing.Size(257, 482);
            this.layoutPanel.TabIndex = 2;
            // 
            // lblKeyDetails
            // 
            this.lblKeyDetails.AutoSize = true;
            this.lblKeyDetails.BackColor = System.Drawing.Color.MistyRose;
            this.lblKeyDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblKeyDetails.ForeColor = System.Drawing.Color.IndianRed;
            this.lblKeyDetails.Location = new System.Drawing.Point(0, 48);
            this.lblKeyDetails.Margin = new System.Windows.Forms.Padding(0);
            this.lblKeyDetails.Name = "lblKeyDetails";
            this.lblKeyDetails.Padding = new System.Windows.Forms.Padding(3);
            this.lblKeyDetails.Size = new System.Drawing.Size(257, 34);
            this.lblKeyDetails.TabIndex = 2;
            this.lblKeyDetails.Text = "(click to connect)";
            this.lblKeyDetails.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LicenseEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(257, 482);
            this.Controls.Add(this.layoutPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseEditor";
            this.Text = "Xyratex License Editor";
            this.Load += new System.EventHandler(this.LicenseEditor_Load);
            this.pnlFeatures.ResumeLayout(false);
            this.pnlFeatures.PerformLayout();
            this.pnlConnect.ResumeLayout(false);
            this.pnlControls.ResumeLayout(false);
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFeatures;
        private System.Windows.Forms.CheckBox cbAll;
        private System.Windows.Forms.CheckedListBox clbFeatures;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.TableLayoutPanel layoutPanel;
        private System.Windows.Forms.Button btnProgram;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblKeyStatus;
        private System.Windows.Forms.Label lblKeyDetails;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnReadFeatures;
        private System.Windows.Forms.Panel pnlConnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblValue;
    }
}

