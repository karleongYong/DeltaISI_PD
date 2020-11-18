namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    partial class MotionControlPanel
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
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.labelExecuteCommand = new System.Windows.Forms.Label();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.groupBoxTernimal = new System.Windows.Forms.GroupBox();
            this.labelInstructionandInformation = new System.Windows.Forms.Label();
            this.groupBoxTernimal.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.Location = new System.Drawing.Point(15, 40);
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.Size = new System.Drawing.Size(188, 20);
            this.textBoxCommand.TabIndex = 0;
            this.textBoxCommand.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxCommand_KeyUp);
            // 
            // labelExecuteCommand
            // 
            this.labelExecuteCommand.AutoSize = true;
            this.labelExecuteCommand.Location = new System.Drawing.Point(12, 24);
            this.labelExecuteCommand.Name = "labelExecuteCommand";
            this.labelExecuteCommand.Size = new System.Drawing.Size(96, 13);
            this.labelExecuteCommand.TabIndex = 1;
            this.labelExecuteCommand.Text = "Execute Command";
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(209, 38);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(63, 20);
            this.buttonExecute.TabIndex = 2;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // groupBoxTernimal
            // 
            this.groupBoxTernimal.Controls.Add(this.labelInstructionandInformation);
            this.groupBoxTernimal.Controls.Add(this.textBoxCommand);
            this.groupBoxTernimal.Controls.Add(this.buttonExecute);
            this.groupBoxTernimal.Controls.Add(this.labelExecuteCommand);
            this.groupBoxTernimal.Location = new System.Drawing.Point(900, 16);
            this.groupBoxTernimal.Name = "groupBoxTernimal";
            this.groupBoxTernimal.Size = new System.Drawing.Size(284, 271);
            this.groupBoxTernimal.TabIndex = 4;
            this.groupBoxTernimal.TabStop = false;
            this.groupBoxTernimal.Text = "Terminal";
            this.groupBoxTernimal.Visible = false;
            // 
            // labelInstructionandInformation
            // 
            this.labelInstructionandInformation.Location = new System.Drawing.Point(14, 63);
            this.labelInstructionandInformation.Name = "labelInstructionandInformation";
            this.labelInstructionandInformation.Size = new System.Drawing.Size(263, 193);
            this.labelInstructionandInformation.TabIndex = 5;
            // 
            // MotionControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxTernimal);
            this.Name = "MotionControlPanel";
            this.Size = new System.Drawing.Size(1342, 980);
            this.groupBoxTernimal.ResumeLayout(false);
            this.groupBoxTernimal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCommand;
        private System.Windows.Forms.Label labelExecuteCommand;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.GroupBox groupBoxTernimal;
        private System.Windows.Forms.Label labelInstructionandInformation;

    }
}
