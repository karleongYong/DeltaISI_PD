namespace Seagate.AAS.HGA.HST.UI.Operation
{
    partial class VisionPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        /// 
        private const int MAX_CAMERA = 3; 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisionPanel));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cogDisplay1 = new Cognex.VisionPro.Display.CogDisplay();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.ComboBoxCamera = new System.Windows.Forms.ComboBox();
            this.GroupBoxCamera = new System.Windows.Forms.GroupBox();
            this.ButtonGrabImage = new System.Windows.Forms.Button();
            this.GroupBoxImageSource = new System.Windows.Forms.GroupBox();
            this.RadioButtonFile = new System.Windows.Forms.RadioButton();
            this.RadioButtonCamera = new System.Windows.Forms.RadioButton();
            this.ButtonLoadRecipe = new System.Windows.Forms.Button();
            this.ButtonRunTool = new System.Windows.Forms.Button();
            this.ButtonOpenFile = new System.Windows.Forms.Button();
            this.LabelImageSharpness = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.GroupBoxCamera.SuspendLayout();
            this.GroupBoxImageSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(804, 614);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cogDisplay1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(796, 588);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Image";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cogDisplay1
            // 
            this.cogDisplay1.Location = new System.Drawing.Point(55, 16);
            this.cogDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogDisplay1.MouseWheelSensitivity = 1D;
            this.cogDisplay1.Name = "cogDisplay1";
            this.cogDisplay1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogDisplay1.OcxState")));
            this.cogDisplay1.Size = new System.Drawing.Size(679, 555);
            this.cogDisplay1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cogToolBlockEditV21);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(796, 588);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Vision Training Tool";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(6, 6);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(766, 497);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 0;
            this.cogToolBlockEditV21.SubjectChanged += new System.EventHandler(this.cogToolBlockEditV21_SubjectChanged);
            // 
            // ComboBoxCamera
            // 
            this.ComboBoxCamera.Enabled = false;
            this.ComboBoxCamera.FormattingEnabled = true;
            this.ComboBoxCamera.Items.AddRange(new object[] {
            "Input Camera",
            "Fiducial Camera",
            "Output Camera"});
            this.ComboBoxCamera.Location = new System.Drawing.Point(27, 41);
            this.ComboBoxCamera.Name = "ComboBoxCamera";
            this.ComboBoxCamera.Size = new System.Drawing.Size(140, 21);
            this.ComboBoxCamera.TabIndex = 2;
            this.ComboBoxCamera.SelectedIndexChanged += new System.EventHandler(this.ComboBoxCamera_SelectedIndexChanged);
            // 
            // GroupBoxCamera
            // 
            this.GroupBoxCamera.Controls.Add(this.ComboBoxCamera);
            this.GroupBoxCamera.Location = new System.Drawing.Point(814, 202);
            this.GroupBoxCamera.Name = "GroupBoxCamera";
            this.GroupBoxCamera.Size = new System.Drawing.Size(200, 100);
            this.GroupBoxCamera.TabIndex = 3;
            this.GroupBoxCamera.TabStop = false;
            this.GroupBoxCamera.Text = "Camera";
            // 
            // ButtonGrabImage
            // 
            this.ButtonGrabImage.Enabled = false;
            this.ButtonGrabImage.Location = new System.Drawing.Point(814, 371);
            this.ButtonGrabImage.Name = "ButtonGrabImage";
            this.ButtonGrabImage.Size = new System.Drawing.Size(199, 37);
            this.ButtonGrabImage.TabIndex = 4;
            this.ButtonGrabImage.Text = "Grab Image";
            this.ButtonGrabImage.UseVisualStyleBackColor = true;
            this.ButtonGrabImage.Click += new System.EventHandler(this.ButtonGrabImage_Click);
            // 
            // GroupBoxImageSource
            // 
            this.GroupBoxImageSource.Controls.Add(this.RadioButtonFile);
            this.GroupBoxImageSource.Controls.Add(this.RadioButtonCamera);
            this.GroupBoxImageSource.Location = new System.Drawing.Point(814, 83);
            this.GroupBoxImageSource.Name = "GroupBoxImageSource";
            this.GroupBoxImageSource.Size = new System.Drawing.Size(200, 75);
            this.GroupBoxImageSource.TabIndex = 5;
            this.GroupBoxImageSource.TabStop = false;
            this.GroupBoxImageSource.Text = "Image Source";
            // 
            // RadioButtonFile
            // 
            this.RadioButtonFile.AutoSize = true;
            this.RadioButtonFile.Location = new System.Drawing.Point(27, 42);
            this.RadioButtonFile.Name = "RadioButtonFile";
            this.RadioButtonFile.Size = new System.Drawing.Size(41, 17);
            this.RadioButtonFile.TabIndex = 1;
            this.RadioButtonFile.TabStop = true;
            this.RadioButtonFile.Text = "File";
            this.RadioButtonFile.UseVisualStyleBackColor = true;
            this.RadioButtonFile.CheckedChanged += new System.EventHandler(this.RadioButtonFile_CheckedChanged);
            // 
            // RadioButtonCamera
            // 
            this.RadioButtonCamera.AutoSize = true;
            this.RadioButtonCamera.Location = new System.Drawing.Point(27, 19);
            this.RadioButtonCamera.Name = "RadioButtonCamera";
            this.RadioButtonCamera.Size = new System.Drawing.Size(61, 17);
            this.RadioButtonCamera.TabIndex = 0;
            this.RadioButtonCamera.TabStop = true;
            this.RadioButtonCamera.Text = "Camera";
            this.RadioButtonCamera.UseVisualStyleBackColor = true;
            this.RadioButtonCamera.CheckedChanged += new System.EventHandler(this.RadioButtonCamera_CheckedChanged);
            // 
            // ButtonLoadRecipe
            // 
            this.ButtonLoadRecipe.Enabled = false;
            this.ButtonLoadRecipe.Location = new System.Drawing.Point(814, 328);
            this.ButtonLoadRecipe.Name = "ButtonLoadRecipe";
            this.ButtonLoadRecipe.Size = new System.Drawing.Size(199, 37);
            this.ButtonLoadRecipe.TabIndex = 6;
            this.ButtonLoadRecipe.Text = "Load Recipe";
            this.ButtonLoadRecipe.UseVisualStyleBackColor = true;
            this.ButtonLoadRecipe.Click += new System.EventHandler(this.ButtonLoadRecipe_Click);
            // 
            // ButtonRunTool
            // 
            this.ButtonRunTool.Enabled = false;
            this.ButtonRunTool.Location = new System.Drawing.Point(813, 414);
            this.ButtonRunTool.Name = "ButtonRunTool";
            this.ButtonRunTool.Size = new System.Drawing.Size(200, 37);
            this.ButtonRunTool.TabIndex = 7;
            this.ButtonRunTool.Text = "Run Tool";
            this.ButtonRunTool.UseVisualStyleBackColor = true;
            this.ButtonRunTool.Click += new System.EventHandler(this.ButtonRunTool_Click);
            // 
            // ButtonOpenFile
            // 
            this.ButtonOpenFile.Enabled = false;
            this.ButtonOpenFile.Location = new System.Drawing.Point(814, 164);
            this.ButtonOpenFile.Name = "ButtonOpenFile";
            this.ButtonOpenFile.Size = new System.Drawing.Size(200, 32);
            this.ButtonOpenFile.TabIndex = 8;
            this.ButtonOpenFile.Text = "Open File";
            this.ButtonOpenFile.UseVisualStyleBackColor = true;
            this.ButtonOpenFile.Click += new System.EventHandler(this.ButtonOpenFile_Click);
            // 
            // LabelImageSharpness
            // 
            this.LabelImageSharpness.AutoSize = true;
            this.LabelImageSharpness.Location = new System.Drawing.Point(811, 594);
            this.LabelImageSharpness.Name = "LabelImageSharpness";
            this.LabelImageSharpness.Size = new System.Drawing.Size(19, 13);
            this.LabelImageSharpness.TabIndex = 11;
            this.LabelImageSharpness.Text = "??";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(814, 457);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 35);
            this.button1.TabIndex = 12;
            this.button1.Text = "CogDisplay";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // VisionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LabelImageSharpness);
            this.Controls.Add(this.ButtonOpenFile);
            this.Controls.Add(this.ButtonRunTool);
            this.Controls.Add(this.ButtonLoadRecipe);
            this.Controls.Add(this.GroupBoxImageSource);
            this.Controls.Add(this.ButtonGrabImage);
            this.Controls.Add(this.GroupBoxCamera);
            this.Controls.Add(this.tabControl1);
            this.Name = "VisionPanel";
            this.Size = new System.Drawing.Size(1034, 641);
            this.Load += new System.EventHandler(this.VisionPanel_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.GroupBoxCamera.ResumeLayout(false);
            this.GroupBoxImageSource.ResumeLayout(false);
            this.GroupBoxImageSource.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        public void InitializeVisionTools()
        {
          
            this.HSTVision = Machine.HSTMachine.HwSystem.GetHwComponent((int)HST.Machine.HSTHwSystem.HardwareComponent.VisionSystem) as Vision.VisionHardware;            
            this.cogtoolblock = new Cognex.VisionPro.ToolBlock.CogToolBlock();
            if (!HSTVision.Simulation)
            {
                InputCamera = new Vision.Camera(HSTVision.GetCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.CameraSerialNumber));
               // FiducialCamera = new Vision.Camera(HSTVision.GetCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.CameraSerialNumber));
                OutputCamera = new Vision.Camera(HSTVision.GetCamera(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.CameraSerialNumber));
                
                InputCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.ImagesOutputPath,
                                                            Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveImagesLessThanTenHGAs,
                                                            Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveAllImages);
              //  FiducialCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.ImagesOutputPath,
              //                                                 Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.SaveFailImages,
              //                                                 Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.SaveAllImages);
                OutputCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.ImagesOutputPath,
                                                             Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveImagesLessThanTenHGAs,
                                                             Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveAllImages);
                 
            }
           // else
           // {
              
           //     FileCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.Images.ImagesOutputPath,
            //                                                Machine.HSTMachine.Workcell.CalibrationSettings.Vision.Images.SaveFailImages,
           //                                                 Machine.HSTMachine.Workcell.CalibrationSettings.Vision.Images.SaveAllImages);
           // }
            
        }
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Cognex.VisionPro.Display.CogDisplay cogDisplay1;
        private System.Windows.Forms.TabPage tabPage2;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.ComboBox ComboBoxCamera;
        private System.Windows.Forms.GroupBox GroupBoxCamera;

        private Seagate.AAS.HGA.HST.Vision.VisionHardware HSTVision;
        private Cognex.VisionPro.ToolBlock.CogToolBlock cogtoolblock;
        private System.Windows.Forms.Button ButtonGrabImage;

        private Seagate.AAS.HGA.HST.Vision.Camera InputCamera;
        private Seagate.AAS.HGA.HST.Vision.Camera FiducialCamera;
        private Seagate.AAS.HGA.HST.Vision.Camera OutputCamera;

        private Seagate.AAS.HGA.HST.Vision.VisionApp InputCameraVisionApp;
        private Seagate.AAS.HGA.HST.Vision.VisionApp FiducialCameraVisionApp;
        private Seagate.AAS.HGA.HST.Vision.VisionApp OutputCameraVisionApp;
        private Seagate.AAS.HGA.HST.Vision.VisionApp FileCameraVisionApp;

        private System.Windows.Forms.GroupBox GroupBoxImageSource;
        private System.Windows.Forms.RadioButton RadioButtonFile;
        private System.Windows.Forms.RadioButton RadioButtonCamera;
        private System.Windows.Forms.Button ButtonLoadRecipe;
        private System.Windows.Forms.Button ButtonRunTool;
        private System.Windows.Forms.Button ButtonOpenFile;
        private System.Windows.Forms.Label LabelImageSharpness;
        private System.Windows.Forms.Button button1;

        private System.Windows.Forms.Form FormCogDisplay;
        private Seagate.AAS.HGA.HST.UI.Operation.CogDisplayPanel cogDisplayPanel;
       
    }
}
