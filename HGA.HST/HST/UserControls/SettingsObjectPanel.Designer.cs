﻿namespace Seagate.AAS.HGA.HST.UserControls
{
    partial class SettingsObjectPanel
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(5, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(1030, 516);
            this.propertyGrid.TabIndex = 0;
//            this.propertyGrid.GotFocus += new System.EventHandler(this.propertyGrid_GotFocus);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonSave.Location = new System.Drawing.Point(937, 525);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(95, 29);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // SettingsObjectPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.propertyGrid);
            this.Name = "SettingsObjectPanel";
            this.Size = new System.Drawing.Size(1039, 570);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button buttonSave;
    }
}
