namespace Seagate.AAS.HGA.HST.UI
{
    partial class PanelNavigation
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected virtual void Dispose(bool disposing)
        protected override void Dispose(bool disposing) //LLL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelNavigation));
            this.subNavigationMenu = new System.Windows.Forms.ContextMenu();
            this.tmrPopUp = new System.Windows.Forms.Timer(this.components);
            this.btnOperation = new System.Windows.Forms.RadioButton();
            this.btnRecipe = new System.Windows.Forms.RadioButton();
            this.btnSetup = new System.Windows.Forms.RadioButton();
            this.btnDiagnostic = new System.Windows.Forms.RadioButton();
            this.btnData = new System.Windows.Forms.RadioButton();            
            this.btnHelp = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // tmrPopUp
            // 
            this.tmrPopUp.Interval = 300;
            this.tmrPopUp.Tick += new System.EventHandler(this.tmrPopUp_Tick);
            // 
            // btnOperation
            // 
            this.btnOperation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOperation.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnOperation.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnOperation.Checked = true;
            this.btnOperation.Image = ((System.Drawing.Image)(resources.GetObject("btnOperation.Image")));
            this.btnOperation.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOperation.Location = new System.Drawing.Point(3, 6);
            this.btnOperation.Name = "btnOperation";
            this.btnOperation.Size = new System.Drawing.Size(100, 60);
            this.btnOperation.TabIndex = 0;
            this.btnOperation.TabStop = true;
            this.btnOperation.Text = "Operation";
            this.btnOperation.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOperation.UseVisualStyleBackColor = false;
            this.btnOperation.CheckedChanged += new System.EventHandler(this.radiobtn_CheckedChanged);
            this.btnOperation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseDown);
            this.btnOperation.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseUp);
            // 
            // btnRecipe
            // 
            this.btnRecipe.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnRecipe.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnRecipe.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnRecipe.Image = ((System.Drawing.Image)(resources.GetObject("btnRecipe.Image")));
            this.btnRecipe.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRecipe.Location = new System.Drawing.Point(107, 5);
            this.btnRecipe.Name = "btnRecipe";
            this.btnRecipe.Size = new System.Drawing.Size(100, 60);
            this.btnRecipe.TabIndex = 1;
            this.btnRecipe.Text = "Recipe";
            this.btnRecipe.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRecipe.UseVisualStyleBackColor = false;
            this.btnRecipe.CheckedChanged += new System.EventHandler(this.radiobtn_CheckedChanged);
            this.btnRecipe.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseDown);
            this.btnRecipe.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseUp);
            // 
            // btnSetup
            // 
            this.btnSetup.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSetup.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSetup.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnSetup.Image = ((System.Drawing.Image)(resources.GetObject("btnSetup.Image")));
            this.btnSetup.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSetup.Location = new System.Drawing.Point(211, 5);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(100, 60);
            this.btnSetup.TabIndex = 2;
            this.btnSetup.Text = "Setup";
            this.btnSetup.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSetup.UseVisualStyleBackColor = false;
            this.btnSetup.CheckedChanged += new System.EventHandler(this.radiobtn_CheckedChanged);
            this.btnSetup.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseDown);
            this.btnSetup.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseUp);
            // 
            // btnDiagnostic
            // 
            this.btnDiagnostic.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnDiagnostic.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnDiagnostic.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnDiagnostic.Image = ((System.Drawing.Image)(resources.GetObject("btnDiagnostic.Image")));
            this.btnDiagnostic.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDiagnostic.Location = new System.Drawing.Point(315, 5);
            this.btnDiagnostic.Name = "btnDiagnostic";
            this.btnDiagnostic.Size = new System.Drawing.Size(100, 60);
            this.btnDiagnostic.TabIndex = 3;
            this.btnDiagnostic.Text = "Diagnostic";
            this.btnDiagnostic.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDiagnostic.UseVisualStyleBackColor = false;
            this.btnDiagnostic.CheckedChanged += new System.EventHandler(this.radiobtn_CheckedChanged);
            this.btnDiagnostic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseDown);
            this.btnDiagnostic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseUp);
            // 
            // btnData
            // 
            this.btnData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnData.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnData.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnData.Image = ((System.Drawing.Image)(resources.GetObject("btnData.Image")));
            this.btnData.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnData.Location = new System.Drawing.Point(419, 6);
            this.btnData.Name = "btnData";
            this.btnData.Size = new System.Drawing.Size(100, 60);
            this.btnData.TabIndex = 4;
            this.btnData.Text = "Data";
            this.btnData.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnData.UseVisualStyleBackColor = false;
            this.btnData.CheckedChanged += new System.EventHandler(this.radiobtn_CheckedChanged);
            this.btnData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseDown);
            this.btnData.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseUp);            
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnHelp.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnHelp.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("btnHelp.Image")));
            this.btnHelp.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHelp.Location = new System.Drawing.Point(627, 5);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(100, 60);
            this.btnHelp.TabIndex = 6;
            this.btnHelp.Text = "Help";
            this.btnHelp.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.CheckedChanged += new System.EventHandler(this.radiobtn_CheckedChanged);
            this.btnHelp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseDown);
            this.btnHelp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radiobtn_MouseUp);
            // 
            // PanelNavigation
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnOperation);
            this.Controls.Add(this.btnRecipe);
            this.Controls.Add(this.btnSetup);
            this.Controls.Add(this.btnDiagnostic);
            this.Controls.Add(this.btnData);            
            this.Controls.Add(this.btnHelp);
            this.Name = "PanelNavigation";
            this.Size = new System.Drawing.Size(731, 69);
            this.Load += new System.EventHandler(this.PanelNavigation_Load);
            this.Resize += new System.EventHandler(this.PanelNavigation_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RadioButton btnOperation;
        public System.Windows.Forms.RadioButton btnSetup;
        public System.Windows.Forms.RadioButton btnDiagnostic;
        protected System.Windows.Forms.RadioButton btnData;        
        protected System.Windows.Forms.RadioButton btnHelp;
        private System.Windows.Forms.ContextMenu subNavigationMenu;
        private System.Windows.Forms.Timer tmrPopUp;
        public System.Windows.Forms.RadioButton btnRecipe;

    }
}
