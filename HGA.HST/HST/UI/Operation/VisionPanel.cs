using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cognex.VisionPro;
using System.IO;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.UI.Operation
{
    public partial class VisionPanel : UserControl
    {
        public event EventHandler OnSettingChanged;
        private CogImage8Grey img;
        public VisionPanel()
        {
            InitializeComponent();
            InitializeVisionTools();
            Machine.HSTMachine.Workcell.CalibrationSettings.OnSettingsChanged += VisionSettingsChanged;
        }

        private void VisionPanel_Load(object sender, EventArgs e)
        {
            if(this.HSTVision.Simulation)
            {
                
                this.ComboBoxCamera.Enabled = false;
                this.ButtonLoadRecipe.Enabled = false;
                this.ButtonGrabImage.Enabled = false;
                this.ButtonRunTool.Enabled = false;
                this.RadioButtonCamera.Enabled = false;
                
            }
            else
            {
                this.ComboBoxCamera.Enabled = true;
                this.ButtonLoadRecipe.Enabled = true;
                this.ButtonGrabImage.Enabled = true;
                this.ButtonRunTool.Enabled = true;
                this.RadioButtonCamera.Enabled = true;
            }
        }

        private void ButtonGrabImage_Click(object sender, EventArgs e)
        {
            switch (this.ComboBoxCamera.SelectedIndex)
            {
                case 0:
                    if (InputCamera != null)
                    {
                        this.InputCamera.GrabManual(true);
                        img = this.InputCamera.grabImage;
                    }
                    break;
                case 1:
                    if (FiducialCamera != null)
                    {
                        this.FiducialCamera.GrabManual(true);
                        img = this.FiducialCamera.grabImage;
                    }
                    break;
                case 2:
                    if (OutputCamera != null)
                    {
                        this.OutputCamera.GrabManual(true);
                        img = this.OutputCamera.grabImage;
                    }
                    break;
            }
            this.cogDisplay1.Image = img;
            this.cogDisplay1.Fit(true);
            
            if (this.cogtoolblock.Inputs.Contains("InputImage"))
            {
                this.cogtoolblock.Inputs["InputImage"].Value = img;
                cogToolBlockEditV21.Subject = cogtoolblock;
                cogToolBlockEditV21.Subject.Run();

            }

           
        }

        private void cogToolBlockEditV21_SubjectChanged(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainE95));
            FormMainE95.ActiveForm.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            cogtoolblock = cogToolBlockEditV21.Subject;

            if (this.cogtoolblock.Inputs.Contains("InputImage"))
            {
                if (RadioButtonCamera.Checked)
                {

                    switch(this.ComboBoxCamera.SelectedIndex)
                    {
                        case 0:
                            if (this.InputCamera != null)
                                this.cogtoolblock.Inputs["InputImage"].Value = this.InputCamera.grabImage;
                            break;
                        case 1:
                            if (this.FiducialCamera != null)
                                this.cogtoolblock.Inputs["InputImage"].Value = this.FiducialCamera.grabImage;
                            break;
                        case 2:
                            if (this.OutputCamera != null)
                                this.cogtoolblock.Inputs["InputImage"].Value = this.OutputCamera.grabImage;
                            break;
                    }
                }
                else
                {
                    if (img != null)
                        this.cogtoolblock.Inputs["InputImage"].Value = img;
                    else
                        return;
                }

                cogToolBlockEditV21.Subject = cogtoolblock;
                cogToolBlockEditV21.Subject.Run();
            }
        }

        private void RadioButtonCamera_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RadioButtonCamera.Checked)
            {
                this.ComboBoxCamera.Enabled = true;
                this.ButtonLoadRecipe.Enabled = true;
                this.ButtonGrabImage.Enabled = true;
                this.ButtonRunTool.Enabled = true;
            }else
            {
                this.ComboBoxCamera.Enabled = false;
                this.ButtonGrabImage.Enabled = false;
            }
        }

        private void ComboBoxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (ComboBoxCamera.SelectedIndex == 0)
            {
                if (this.InputCamera == null)
                    return;
                this.InputCamera.GrabManual(true);
                this.cogDisplay1.Image = this.InputCamera.grabImage;
                this.cogDisplay1.Fit(true);
                if (this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    this.cogtoolblock.Inputs["InputImage"].Value = this.InputCamera.grabImage as CogImage8Grey;
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();

                }
            }

            else if (ComboBoxCamera.SelectedIndex == 1)
            {
                if (this.FiducialCamera == null)
                    return;
                this.FiducialCamera.GrabManual(true);
                this.cogDisplay1.Image = this.FiducialCamera.grabImage;
                this.cogDisplay1.Fit(true);
                if (this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    this.cogtoolblock.Inputs["InputImage"].Value = this.FiducialCamera.grabImage as CogImage8Grey;
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();

                }
            }

            else if (ComboBoxCamera.SelectedIndex == 2)
            {
                if (this.OutputCamera == null)
                    return;
                this.OutputCamera.GrabManual(true);
                this.cogDisplay1.Image = this.OutputCamera.grabImage;
                this.cogDisplay1.Fit(true);
                if (this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    this.cogtoolblock.Inputs["InputImage"].Value = this.OutputCamera.grabImage as CogImage8Grey;
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();

                }
            }
        }

        private void RadioButtonFile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RadioButtonFile.Checked)                
                this.ButtonOpenFile.Enabled = true;
            else
                this.ButtonOpenFile.Enabled = false;
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            string imageFilename;
            this.ComboBoxCamera.Enabled = false;
            this.ButtonLoadRecipe.Enabled = false;
            this.ButtonGrabImage.Enabled = false;
            this.ButtonRunTool.Enabled = false;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imageFilename = openFileDialog1.FileName;
                Bitmap bitmap = new Bitmap(imageFilename);
                img = new CogImage8Grey(bitmap);
                this.cogDisplay1.Image = img;
                this.cogDisplay1.Fit(true);

                if (this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    this.cogtoolblock.Inputs["InputImage"].Value = img;
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();

                }
            }
           
        }

        private void ButtonLoadRecipe_Click(object sender, EventArgs e)
        {
            
            this.ButtonLoadRecipe.Enabled = false;
            
            if (RadioButtonCamera.Checked)
            {
                switch (this.ComboBoxCamera.SelectedIndex)
                {
                    case 0:
                        if (InputCameraVisionApp != null)
                            this.InputCameraVisionApp.LoadRecipe(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.Recipe, CameraLocation.InputStation);
                        break;
                    case 1:
                        break;
                    case 2:
                        if (OutputCameraVisionApp != null)
                            this.OutputCameraVisionApp.LoadRecipe(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.Recipe, CameraLocation.OutputStation);
                        break;
                }
            }
            else
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();              
                string recipe;
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "vpp files (*.vpp)|*.vpp";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = false;
            }


            this.ButtonLoadRecipe.Enabled = true;
        }

        private void ButtonRunTool_Click(object sender, EventArgs e)
        {
            
            this.ButtonRunTool.Enabled = false;
             
            
            if (RadioButtonCamera.Checked)
            {
                switch (this.ComboBoxCamera.SelectedIndex)
                {
                    case 0:
                        if (this.InputCameraVisionApp != null)
                        {
                            this.InputCameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.ImagesOutputPath,
                                                               Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveImagesLessThanTenHGAs,
                                                               Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveAllImages);
                            this.InputCameraVisionApp.RunToolBlock(new CogImage8Grey(this.cogDisplay1.Image.ToBitmap()), "Unknown");
                        }
                        
                        break;
                    case 1:
                        break;
                    case 2:
                        if (this.OutputCameraVisionApp != null)
                        {
                            this.OutputCameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.ImagesOutputPath,
                                                                    Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveImagesLessThanTenHGAs,
                                                                    Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveAllImages);

                            this.OutputCameraVisionApp.RunToolBlock(new CogImage8Grey(this.cogDisplay1.Image.ToBitmap()), "Unknown");
                        }
                        break;
                }
            }

            this.ButtonRunTool.Enabled = true;
        }

        private void VisionSettingsChanged(object sender, EventArgs e)
        {
           
           
            if (this.InputCameraVisionApp != null)
                this.InputCameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.ImagesOutputPath,
                                                    Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveImagesLessThanTenHGAs,
                                                    Machine.HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.SaveAllImages);
            if (this.OutputCameraVisionApp != null)
                this.OutputCameraVisionApp.LoadSettings(Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.ImagesOutputPath,
                                                    Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveImagesLessThanTenHGAs,
                                                    Machine.HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.SaveAllImages);
        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (ComboBoxCamera.SelectedIndex == 0)
            {
                if (this.InputCamera == null)
                    return;
            }
            else if (ComboBoxCamera.SelectedIndex == 1)
            {
                if (this.FiducialCamera == null)
                    return;
            }
            else if (ComboBoxCamera.SelectedIndex == 2)
            {
                if (this.OutputCamera == null)
                    return;
            }
            else
            {
                return;
            }

            if (this.FormCogDisplay == null)
            {
                this.FormCogDisplay = new System.Windows.Forms.Form();                
                this.FormCogDisplay.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formclose);               
            }

            if (ComboBoxCamera.SelectedIndex == 0)
            {
                this.cogDisplayPanel = new CogDisplayPanel(this.InputCamera.cogAcqFifo);
            }
            else if (ComboBoxCamera.SelectedIndex == 1)
            {
                this.cogDisplayPanel = new CogDisplayPanel(this.FiducialCamera.cogAcqFifo);
            }
            else if (ComboBoxCamera.SelectedIndex == 2)
            {
                this.cogDisplayPanel = new CogDisplayPanel(this.OutputCamera.cogAcqFifo);
            }
            this.FormCogDisplay.Location = new System.Drawing.Point(0, 0);
            this.FormCogDisplay.Name = "FormCogDisplay";
            this.FormCogDisplay.Size = new System.Drawing.Size(this.cogDisplayPanel.Size.Width + 20, this.cogDisplayPanel.Size.Height + 50);
            this.FormCogDisplay.Text = String.Format("Camera {0}", ComboBoxCamera.SelectedText);
            this.FormCogDisplay.Controls.Add(this.cogDisplayPanel);
            this.button1.Enabled = false;


            FormCogDisplay.Show();
        }

        public void formclose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.button1.Enabled = true;
            this.FormCogDisplay.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.formclose);
            this.cogDisplayPanel.StopCogDisplayPanel();
            this.FormCogDisplay = null;
            this.cogDisplayPanel = null;
        }
    }
}
