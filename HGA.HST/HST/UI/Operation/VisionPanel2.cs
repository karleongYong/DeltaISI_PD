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

namespace Seagate.AAS.HGA.HST.UI.Operation
{
    public partial class VisionPanel2 : UserControl
    {
       
        private CogImage8Grey img;

        public VisionPanel2()
        {
            InitializeComponent();
            InitializeVisionTools();
            
        }
      

        private void RadioButtonCamera_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RadioButtonCamera.Checked)
            {
                this.ComboBoxCamera.Enabled = true;                
                this.ButtonGrabImage.Enabled = true;
                this.ButtonLiveImage.Enabled = true;
                this.ButtonOpenFile.Enabled = false;
                
            }
            else
            {
                this.ComboBoxCamera.Enabled = false;
                this.ButtonGrabImage.Enabled = false;
                this.ButtonLiveImage.Enabled = false;
                this.ButtonOpenFile.Enabled = true;
            }
        }

        private void RadioButtonFile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RadioButtonFile.Checked)
            {
                this.ComboBoxCamera.Enabled = false;
                this.ButtonGrabImage.Enabled = false;
                this.ButtonLiveImage.Enabled = false;
                this.ButtonOpenFile.Enabled = true;

            }
            else
            {
                this.ComboBoxCamera.Enabled = true;
                this.ButtonGrabImage.Enabled = true;
                this.ButtonLiveImage.Enabled = true;
                this.ButtonOpenFile.Enabled = false;
            }
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            string imageFilename;
            
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
                if (this.cogtoolblock != null && this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    this.cogtoolblock.Inputs["InputImage"].Value = img;
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();
                }
            }
        }

        private void cogToolBlockEditV21_SubjectChanged(object sender, EventArgs e)
        {
          //  if (this.cogtoolblock == null )
          //      return;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainE95));
            FormMainE95.ActiveForm.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            cogtoolblock = cogToolBlockEditV21.Subject;

            if (this.cogtoolblock.Inputs.Contains("InputImage"))
            {
                if (RadioButtonCamera.Checked)
                {

                    switch (this.ComboBoxCamera.SelectedIndex)
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
                            if (this.OutputCamera == null)
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

        private void ButtonGrabImage_Click(object sender, EventArgs e)
        {
            if (this.cogtoolblock == null || this.InputCamera == null || this.FiducialCamera == null || this.OutputCamera == null)
                return;
            if (RadioButtonCamera.Checked)
            {

                switch (this.ComboBoxCamera.SelectedIndex)
                {
                    case 0:
                        this.cogtoolblock.Inputs["InputImage"].Value = this.InputCamera.grabImage;
                        break;
                    case 1:
                        this.cogtoolblock.Inputs["InputImage"].Value = this.FiducialCamera.grabImage;
                        break;
                    case 2:
                        this.cogtoolblock.Inputs["InputImage"].Value = this.OutputCamera.grabImage;
                        break;
                    default:
                        return;
                }

                if (this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    this.cogtoolblock.Inputs["InputImage"].Value = img;
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();

                }

            }
        }

        private void ButtonLiveImage_Click(object sender, EventArgs e)
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
            this.ButtonLiveImage.Enabled = false;


            FormCogDisplay.Show();

        }


        public void formclose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.ButtonLiveImage.Enabled = true;
            this.FormCogDisplay.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.formclose);
            this.cogDisplayPanel.StopCogDisplayPanel();
            this.FormCogDisplay = null;
            this.cogDisplayPanel = null;
        }

        private void VisionPanel_Load(object sender, EventArgs e)
        {

            
            this.ComboBoxCamera.Enabled = false;    
            this.ButtonGrabImage.Enabled = false;                           
            this.ButtonLiveImage.Enabled = false;
            this.ButtonOpenFile.Enabled = false;
        }

        private void ComboBoxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    }
}
