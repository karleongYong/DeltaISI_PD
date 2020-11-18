namespace Seagate.AAS.HGA.HST.UI.Main
{
    partial class PanelOperation
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.tabPageModuleState = new System.Windows.Forms.TabPage();
            this.tabPageStatus = new System.Windows.Forms.TabPage();
            this.operationMainPanel = new Seagate.AAS.HGA.HST.UI.Operation.OperationMainPanel();
            this.operationModuleState = new Seagate.AAS.HGA.HST.UI.Operation.OperationModulesState();
            this.operationStatus = new Seagate.AAS.HGA.HST.UI.Operation.OperationStatus();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMain);
            this.tabControl1.Controls.Add(this.tabPageModuleState);
            this.tabControl1.Controls.Add(this.tabPageStatus);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1247, 681);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.operationMainPanel);
            this.tabPageMain.Location = new System.Drawing.Point(4, 2);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(1239, 655);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Main";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // tabPageModuleState
            //
            this.tabPageModuleState.Controls.Add(this.operationModuleState);
            this.tabPageModuleState.Location = new System.Drawing.Point(4, 22);
            this.tabPageModuleState.Name = "tabPageModuleState";
            this.tabPageModuleState.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModuleState.Size = new System.Drawing.Size(1239, 655);
            this.tabPageModuleState.TabIndex = 0;
            this.tabPageModuleState.Text = "Module State";
            this.tabPageModuleState.UseVisualStyleBackColor = true;
            // 
            // tabPageStatus
            //
            this.tabPageStatus.Controls.Add(this.operationStatus);
            this.tabPageStatus.Location = new System.Drawing.Point(4, 22);
            this.tabPageStatus.Name = "tabPageStatus";
            this.tabPageStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStatus.Size = new System.Drawing.Size(1239, 655);
            this.tabPageStatus.TabIndex = 0;
            this.tabPageStatus.Text = "Status";
            this.tabPageStatus.UseVisualStyleBackColor = true;
            // 
            // operationMainPanel
            // 
            this.operationMainPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.operationMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationMainPanel.Location = new System.Drawing.Point(3, 3);
            this.operationMainPanel.Name = "operationMainPanel";
            this.operationMainPanel.Size = new System.Drawing.Size(1600, 1000);
            this.operationMainPanel.TabIndex = 0;
            // 
            // operationModuleState
            // 
            this.operationModuleState.BackColor = System.Drawing.Color.LightSteelBlue;
            this.operationModuleState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationModuleState.Location = new System.Drawing.Point(3, 3);
            this.operationModuleState.Name = "operationModuleState";
            this.operationModuleState.Size = new System.Drawing.Size(1600, 1000);
            this.operationModuleState.TabIndex = 1;
            // 
            // operationStatus
            // 
            this.operationStatus.BackColor = System.Drawing.Color.LightSteelBlue;
            this.operationStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationStatus.Location = new System.Drawing.Point(3, 3);
            this.operationStatus.Name = "operationStatus";
            this.operationStatus.Size = new System.Drawing.Size(1600, 1000);
            this.operationStatus.TabIndex = 2;
            // 
            // PanelOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "PanelOperation";
            this.Size = new System.Drawing.Size(1247, 681);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TabPage tabPageModuleState;
        private System.Windows.Forms.TabPage tabPageStatus;
        private Seagate.AAS.HGA.HST.UI.Operation.OperationMainPanel operationMainPanel;
        private Seagate.AAS.HGA.HST.UI.Operation.OperationModulesState operationModuleState;
        private Seagate.AAS.HGA.HST.UI.Operation.OperationStatus operationStatus;
    }
}
