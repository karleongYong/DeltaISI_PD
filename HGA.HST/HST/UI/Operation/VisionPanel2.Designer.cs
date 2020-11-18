namespace Seagate.AAS.HGA.HST.UI.Operation
{
    partial class VisionPanel2
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
               // FiducialCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.ImagesOutputPath,
               //                                                Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.SaveFailImages,
               //                                                Machine.HSTMachine.Workcell.CalibrationSettings.Vision.FiducialCamera.SaveAllImages);
                OutputCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.ImagesOutputPath,
                                                             Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveImagesLessThanTenHGAs,
                                                             Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveAllImages);
                 
            }
          
         //   FileCameraVisionApp = new Vision.VisionApp(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.Images.ImagesOutputPath,
         //                                               Machine.HSTMachine.Workcell.CalibrationSettings.Vision.Images.SaveFailImages,
         //                                               Machine.HSTMachine.Workcell.CalibrationSettings.Vision.Images.SaveAllImages);
         

        }


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageVisionTrainingTool = new System.Windows.Forms.TabPage();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.GroupBoxImageSource = new System.Windows.Forms.GroupBox();
            this.RadioButtonFile = new System.Windows.Forms.RadioButton();
            this.RadioButtonCamera = new System.Windows.Forms.RadioButton();
            this.GroupBoxCamera = new System.Windows.Forms.GroupBox();
            this.ComboBoxCamera = new System.Windows.Forms.ComboBox();
            this.ButtonGrabImage = new System.Windows.Forms.Button();
            this.ButtonLiveImage = new System.Windows.Forms.Button();
            this.ButtonOpenFile = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.TabPageVisionTrainingTool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.GroupBoxImageSource.SuspendLayout();
            this.GroupBoxCamera.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabPageVisionTrainingTool);
            this.tabControl1.Location = new System.Drawing.Point(4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(799, 578);
            this.tabControl1.TabIndex = 0;
            // 
            // TabPageVisionTrainingTool
            // 
            this.TabPageVisionTrainingTool.Controls.Add(this.cogToolBlockEditV21);
            this.TabPageVisionTrainingTool.Location = new System.Drawing.Point(4, 22);
            this.TabPageVisionTrainingTool.Name = "TabPageVisionTrainingTool";
            this.TabPageVisionTrainingTool.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageVisionTrainingTool.Size = new System.Drawing.Size(791, 552);
            this.TabPageVisionTrainingTool.TabIndex = 0;
            this.TabPageVisionTrainingTool.Text = "Vision Training Tool";
            this.TabPageVisionTrainingTool.UseVisualStyleBackColor = true;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(6, 6);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(766, 506);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 0;
            this.cogToolBlockEditV21.SubjectChanged += new System.EventHandler(this.cogToolBlockEditV21_SubjectChanged);
            // 
            // GroupBoxImageSource
            // 
            this.GroupBoxImageSource.Controls.Add(this.RadioButtonFile);
            this.GroupBoxImageSource.Controls.Add(this.RadioButtonCamera);
            this.GroupBoxImageSource.Location = new System.Drawing.Point(810, 26);
            this.GroupBoxImageSource.Name = "GroupBoxImageSource";
            this.GroupBoxImageSource.Size = new System.Drawing.Size(200, 100);
            this.GroupBoxImageSource.TabIndex = 1;
            this.GroupBoxImageSource.TabStop = false;
            this.GroupBoxImageSource.Text = "Image Source";
            // 
            // RadioButtonFile
            // 
            this.RadioButtonFile.AutoSize = true;
            this.RadioButtonFile.Location = new System.Drawing.Point(23, 64);
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
            this.RadioButtonCamera.Location = new System.Drawing.Point(23, 28);
            this.RadioButtonCamera.Name = "RadioButtonCamera";
            this.RadioButtonCamera.Size = new System.Drawing.Size(61, 17);
            this.RadioButtonCamera.TabIndex = 0;
            this.RadioButtonCamera.TabStop = true;
            this.RadioButtonCamera.Text = "Camera";
            this.RadioButtonCamera.UseVisualStyleBackColor = true;
            this.RadioButtonCamera.CheckedChanged += new System.EventHandler(this.RadioButtonCamera_CheckedChanged);
            // 
            // GroupBoxCamera
            // 
            this.GroupBoxCamera.Controls.Add(this.ComboBoxCamera);
            this.GroupBoxCamera.Location = new System.Drawing.Point(810, 133);
            this.GroupBoxCamera.Name = "GroupBoxCamera";
            this.GroupBoxCamera.Size = new System.Drawing.Size(200, 100);
            this.GroupBoxCamera.TabIndex = 2;
            this.GroupBoxCamera.TabStop = false;
            this.GroupBoxCamera.Text = "Camera";
            // 
            // ComboBoxCamera
            // 
            this.ComboBoxCamera.FormattingEnabled = true;
            this.ComboBoxCamera.Items.AddRange(new object[] {
            "Input Camera",
            "Fiducial Camera",
            "Output Camera"});
            this.ComboBoxCamera.Location = new System.Drawing.Point(23, 44);
            this.ComboBoxCamera.Name = "ComboBoxCamera";
            this.ComboBoxCamera.Size = new System.Drawing.Size(121, 21);
            this.ComboBoxCamera.TabIndex = 0;
            this.ComboBoxCamera.SelectedIndexChanged += new System.EventHandler(this.ComboBoxCamera_SelectedIndexChanged);
            // 
            // ButtonGrabImage
            // 
            this.ButtonGrabImage.Location = new System.Drawing.Point(810, 314);
            this.ButtonGrabImage.Name = "ButtonGrabImage";
            this.ButtonGrabImage.Size = new System.Drawing.Size(200, 43);
            this.ButtonGrabImage.TabIndex = 1;
            this.ButtonGrabImage.Text = "Grab Image";
            this.ButtonGrabImage.UseVisualStyleBackColor = true;
            this.ButtonGrabImage.Click += new System.EventHandler(this.ButtonGrabImage_Click);
            // 
            // ButtonLiveImage
            // 
            this.ButtonLiveImage.Location = new System.Drawing.Point(810, 373);
            this.ButtonLiveImage.Name = "ButtonLiveImage";
            this.ButtonLiveImage.Size = new System.Drawing.Size(200, 42);
            this.ButtonLiveImage.TabIndex = 3;
            this.ButtonLiveImage.Text = "Live Image";
            this.ButtonLiveImage.UseVisualStyleBackColor = true;
            this.ButtonLiveImage.Click += new System.EventHandler(this.ButtonLiveImage_Click);
            // 
            // ButtonOpenFile
            // 
            this.ButtonOpenFile.Location = new System.Drawing.Point(809, 255);
            this.ButtonOpenFile.Name = "ButtonOpenFile";
            this.ButtonOpenFile.Size = new System.Drawing.Size(201, 39);
            this.ButtonOpenFile.TabIndex = 4;
            this.ButtonOpenFile.Text = "Open File";
            this.ButtonOpenFile.UseVisualStyleBackColor = true;
            this.ButtonOpenFile.Click += new System.EventHandler(this.ButtonOpenFile_Click);
            // 
            // VisionPanel2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonOpenFile);
            this.Controls.Add(this.ButtonLiveImage);
            this.Controls.Add(this.ButtonGrabImage);
            this.Controls.Add(this.GroupBoxCamera);
            this.Controls.Add(this.GroupBoxImageSource);
            this.Controls.Add(this.tabControl1);
            this.Name = "VisionPanel2";
            this.Size = new System.Drawing.Size(1038, 585);
            this.Load += new System.EventHandler(this.VisionPanel_Load);
            this.tabControl1.ResumeLayout(false);
            this.TabPageVisionTrainingTool.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.GroupBoxImageSource.ResumeLayout(false);
            this.GroupBoxImageSource.PerformLayout();
            this.GroupBoxCamera.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageVisionTrainingTool;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.GroupBox GroupBoxImageSource;
        private System.Windows.Forms.RadioButton RadioButtonFile;
        private System.Windows.Forms.RadioButton RadioButtonCamera;
        private System.Windows.Forms.GroupBox GroupBoxCamera;
        private System.Windows.Forms.ComboBox ComboBoxCamera;
        private System.Windows.Forms.Button ButtonGrabImage;
        private System.Windows.Forms.Button ButtonLiveImage;
        private System.Windows.Forms.Button ButtonOpenFile;

        private Seagate.AAS.HGA.HST.Vision.VisionHardware HSTVision;
        private Cognex.VisionPro.ToolBlock.CogToolBlock cogtoolblock;

        private Seagate.AAS.HGA.HST.Vision.Camera InputCamera;
        private Seagate.AAS.HGA.HST.Vision.Camera FiducialCamera;
        private Seagate.AAS.HGA.HST.Vision.Camera OutputCamera;

        private Seagate.AAS.HGA.HST.Vision.VisionApp InputCameraVisionApp;
        private Seagate.AAS.HGA.HST.Vision.VisionApp FiducialCameraVisionApp;
        private Seagate.AAS.HGA.HST.Vision.VisionApp OutputCameraVisionApp;
        private Seagate.AAS.HGA.HST.Vision.VisionApp FileCameraVisionApp;

        private System.Windows.Forms.Form FormCogDisplay;
        private Seagate.AAS.HGA.HST.UI.Operation.CogDisplayPanel cogDisplayPanel;
    }
}
