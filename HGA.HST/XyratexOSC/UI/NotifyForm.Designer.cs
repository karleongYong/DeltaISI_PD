namespace XyratexOSC.UI
{
    partial class NotifyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnShutdown = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCustom = new System.Windows.Forms.Button();
            this.lblRequest = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.layoutPanel.SuspendLayout();
            this.buttonLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Light", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(93)))), ((int)(((byte)(155)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.MaximumSize = new System.Drawing.Size(580, 150);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(4, 6, 0, 5);
            this.lblTitle.Size = new System.Drawing.Size(358, 43);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Title";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainPanel.Controls.Add(this.layoutPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(1, 1);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(358, 188);
            this.mainPanel.TabIndex = 5;
            // 
            // layoutPanel
            // 
            this.layoutPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.layoutPanel.ColumnCount = 1;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.Controls.Add(this.lblTitle, 0, 0);
            this.layoutPanel.Controls.Add(this.buttonLayout, 0, 3);
            this.layoutPanel.Controls.Add(this.lblRequest, 0, 2);
            this.layoutPanel.Controls.Add(this.lblMessage, 0, 1);
            this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 4;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.Size = new System.Drawing.Size(358, 188);
            this.layoutPanel.TabIndex = 5;
            // 
            // buttonLayout
            // 
            this.buttonLayout.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonLayout.Controls.Add(this.btnShutdown);
            this.buttonLayout.Controls.Add(this.btnClose);
            this.buttonLayout.Controls.Add(this.btnCancel);
            this.buttonLayout.Controls.Add(this.btnOK);
            this.buttonLayout.Controls.Add(this.btnCustom);
            this.buttonLayout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonLayout.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLayout.Location = new System.Drawing.Point(0, 120);
            this.buttonLayout.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLayout.Name = "buttonLayout";
            this.buttonLayout.Padding = new System.Windows.Forms.Padding(5, 9, 5, 6);
            this.buttonLayout.Size = new System.Drawing.Size(358, 68);
            this.buttonLayout.TabIndex = 4;
            this.buttonLayout.WrapContents = false;
            // 
            // btnShutdown
            // 
            this.btnShutdown.AutoSize = true;
            this.btnShutdown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnShutdown.Location = new System.Drawing.Point(255, 12);
            this.btnShutdown.MinimumSize = new System.Drawing.Size(90, 44);
            this.btnShutdown.Name = "btnShutdown";
            this.btnShutdown.Size = new System.Drawing.Size(90, 44);
            this.btnShutdown.TabIndex = 6;
            this.btnShutdown.Tag = XyratexOSC.UI.NotifyButton.Shutdown;
            this.btnShutdown.Text = "Shutdown";
            this.btnShutdown.UseVisualStyleBackColor = true;
            this.btnShutdown.Click += new System.EventHandler(this.btnShutdown_Click);
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.Location = new System.Drawing.Point(159, 12);
            this.btnClose.MinimumSize = new System.Drawing.Size(90, 44);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 44);
            this.btnClose.TabIndex = 5;
            this.btnClose.Tag = XyratexOSC.UI.NotifyButton.Close;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.Location = new System.Drawing.Point(63, 12);
            this.btnCancel.MaximumSize = new System.Drawing.Size(257, 44);
            this.btnCancel.MinimumSize = new System.Drawing.Size(90, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 44);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Tag = XyratexOSC.UI.NotifyButton.Cancel;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.Location = new System.Drawing.Point(-33, 12);
            this.btnOK.MaximumSize = new System.Drawing.Size(257, 44);
            this.btnOK.MinimumSize = new System.Drawing.Size(90, 44);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 44);
            this.btnOK.TabIndex = 0;
            this.btnOK.Tag = XyratexOSC.UI.NotifyButton.OK;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCustom
            // 
            this.btnCustom.AutoSize = true;
            this.btnCustom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCustom.Location = new System.Drawing.Point(-129, 12);
            this.btnCustom.MaximumSize = new System.Drawing.Size(190, 44);
            this.btnCustom.MinimumSize = new System.Drawing.Size(90, 44);
            this.btnCustom.Name = "btnCustom";
            this.btnCustom.Size = new System.Drawing.Size(90, 44);
            this.btnCustom.TabIndex = 9;
            this.btnCustom.Text = "Custom";
            this.btnCustom.UseVisualStyleBackColor = true;
            this.btnCustom.Click += new System.EventHandler(this.btnCustom_Click);
            // 
            // lblRequest
            // 
            this.lblRequest.AutoSize = true;
            this.lblRequest.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblRequest.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRequest.Location = new System.Drawing.Point(185, 97);
            this.lblRequest.MaximumSize = new System.Drawing.Size(580, 44);
            this.lblRequest.Name = "lblRequest";
            this.lblRequest.Padding = new System.Windows.Forms.Padding(0, 1, 4, 1);
            this.lblRequest.Size = new System.Drawing.Size(170, 23);
            this.lblRequest.TabIndex = 4;
            this.lblRequest.Text = "Select an option below.";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(3, 43);
            this.lblMessage.MaximumSize = new System.Drawing.Size(580, 1000);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(16);
            this.lblMessage.Size = new System.Drawing.Size(352, 54);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "message";
            this.lblMessage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
            // 
            // NotifyForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(93)))), ((int)(((byte)(155)))));
            this.ClientSize = new System.Drawing.Size(360, 190);
            this.ControlBox = false;
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 720);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 190);
            this.Name = "NotifyForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Notify_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.Notify_VisibleChanged);
            this.mainPanel.ResumeLayout(false);
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.buttonLayout.ResumeLayout(false);
            this.buttonLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TableLayoutPanel layoutPanel;
        private System.Windows.Forms.Label lblRequest;
        private System.Windows.Forms.FlowLayoutPanel buttonLayout;
        private System.Windows.Forms.Button btnShutdown;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCustom;
        private System.Windows.Forms.Label lblMessage;
    }
}