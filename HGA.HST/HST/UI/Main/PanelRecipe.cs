using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using Seagate.AAS.HGA.HST.Recipe;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.Utils;
using System.Xml;
using Seagate.AAS.HGA.HST.Utils;
using System.IO;
using Seagate.AAS.HGA.HST.UI.Operation;
using Cognex.VisionPro;
using Seagate.AAS.Parsel.Hw;
using LDUMeasurement.UI;
using Seagate.AAS.HGA.HST.Data;

namespace Seagate.AAS.HGA.HST.UI.Main
{
    public partial class PanelRecipe : UserControl
    {
        HSTWorkcell _workcell;
        TeachPointRecipe _teachPointRecipe;
        TeachPointRecipe _memoryTeachPointRecipe;
        XmlDocument _doc;
        string _fullPathName = string.Empty;
        string _fileNameWithExt = string.Empty;
        private CogImage8Grey img;

        public PanelRecipe()
        {
            InitializeComponent();
            InitializeVisionTools();
        }

        public PanelRecipe(HSTWorkcell workcell)
        {
            InitializeComponent();
            InitializeVisionTools();
            this.tabControlRecipe.Controls.Remove(tabPageGompertz4P);
            _workcell = workcell;
            _workcell.GompertzSettings.Load();
            _workcell.GompertzSettings.Save();
            /*object []GompertzSetting = new object[24];
            GompertzSetting[0] = (object)_workcell.GompertzSettings.SSEInitial;
            GompertzSetting[1] = (object)_workcell.GompertzSettings.RSQInitial;
            GompertzSetting[2] = (object)_workcell.GompertzSettings.amin;
            GompertzSetting[3] = (object)_workcell.GompertzSettings.amax;
            GompertzSetting[4] = (object)_workcell.GompertzSettings.bmin;
            GompertzSetting[5] = (object)_workcell.GompertzSettings.bmax;
            GompertzSetting[6] = (object)_workcell.GompertzSettings.cmin;
            GompertzSetting[7] = (object)_workcell.GompertzSettings.cmax;
            GompertzSetting[8] = (object)_workcell.GompertzSettings.dmin;
            GompertzSetting[9] = (object)_workcell.GompertzSettings.dmax;
            GompertzSetting[10] = (object)_workcell.GompertzSettings.randomscale;
            GompertzSetting[11] = (object)_workcell.GompertzSettings.weight;
            GompertzSetting[12] = (object)_workcell.GompertzSettings.adaptiveSearch_A;
            GompertzSetting[13] = (object)_workcell.GompertzSettings.adaptiveSearch_B;
            GompertzSetting[14] = (object)_workcell.GompertzSettings.adaptiveSearch_C;
            GompertzSetting[15] = (object)_workcell.GompertzSettings.adaptiveSearch_D;
            GompertzSetting[16] = (object)_workcell.GompertzSettings.HardLimit_RSQ;
            GompertzSetting[17] = (object)_workcell.GompertzSettings.HardLimit_SSE;
            GompertzSetting[18] = (object)_workcell.GompertzSettings.MaxTest;
            GompertzSetting[19] = (object)_workcell.GompertzSettings.Step;
            GompertzSetting[20] = (object)_workcell.GompertzSettings.Iteration;
            GompertzSetting[21] = (object)_workcell.GompertzSettings.UseGompertz;
            GompertzSetting[22] = (object)_workcell.GompertzSettings.TestTimePerHGA;
            GompertzSetting[23] = (object)_workcell.GompertzSettings.GompertzCalMethod;
            //panelGomperztCalculation1.LoadSettings(GompertzSetting);
            */

            object[] GompertzSetting = new object[12];
            GompertzSetting[0] = (object)_workcell.GompertzSettings.Split;
            GompertzSetting[1] = (object)_workcell.GompertzSettings.amin;
            GompertzSetting[2] = (object)_workcell.GompertzSettings.amax;
            GompertzSetting[3] = (object)_workcell.GompertzSettings.bmin;
            GompertzSetting[4] = (object)_workcell.GompertzSettings.bmax;
            GompertzSetting[5] = (object)_workcell.GompertzSettings.cmin;
            GompertzSetting[6] = (object)_workcell.GompertzSettings.cmax;
            GompertzSetting[7] = (object)_workcell.GompertzSettings.dmin;
            GompertzSetting[8] = (object)_workcell.GompertzSettings.dmax;
            GompertzSetting[9] = _workcell.GompertzSettings.GompertzCalMethod == GompertzCalculationMethod.Random ? "Random" : "Fix";
            GompertzSetting[10] = _workcell.GompertzSettings.UseGompertz;
            GompertzSetting[11] = _workcell.GompertzSettings.Step;


            panelGompertzCalculation21.LoadSettings(GompertzSetting);
            UpdateUI();
        }

        public UserControls.MeasurementTestRecipeEditor getMeasurementTestRecipe()
        {
            return measurementTestRecipeEditor1;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = HSTSettings.Instance.Directory.MachineRobotPointPath;
            openFileDialog1.Filter = "TeachPointRcp files (*.TeachPointRcp)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fullPath = openFileDialog1.FileName;
                    string fileName = openFileDialog1.SafeFileName;
                    string path = fullPath.Replace(fileName, "");
                    
                    LoadRecipeFromXmlFile(path, fileName, ".TeachPointRcp");

                    UpdateUI();
                }
                catch (Exception ex)
                {
                    _teachPointRecipe = null;
                    ParselMessageBox.Show("Load error:", ex, MessageBoxIcon.Error,
                         Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                         Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                         Seagate.AAS.Parsel.Services.ErrorButton.OK);
                }
            }

            
        }

        private void LoadRecipeFromXmlFile(string searchFolder, string selectedFileName, string _ext)
        {
            try
            {
                _teachPointRecipe = new TeachPointRecipe();
                _doc = new XmlDocument();

                if (selectedFileName.Contains(_ext))
                    _doc.Load(searchFolder + "\\" + selectedFileName);
                else
                    _doc.Load(searchFolder + "\\" + selectedFileName + _ext);

                _teachPointRecipe.Load(_doc);
                HSTMachine.Workcell.TeachPointRecipe.Load(_doc);

                _teachPointRecipe.Name = selectedFileName;
                _teachPointRecipe.FullPath = searchFolder + selectedFileName;
            }
            catch (Exception ex)
            {
                _teachPointRecipe = null;
                ParselMessageBox.Show("Load error:", ex, MessageBoxIcon.Error,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.OK);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_teachPointRecipe != null)
                {
                    UpdateRecipeVariables();

                    WriteValueToDoc(_doc);
                    
                    _doc.Save(_teachPointRecipe.FullPath);
                    HSTWorkcell.disableBoundaryCheck = false;
                }
            }
            catch (Exception ex)
            {
                ParselMessageBox.Show("Save error:", ex, MessageBoxIcon.Error,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.OK);
            }
        }

        private void UpdateRecipeVariables()
        {
            _teachPointRecipe.Version = textBoxVersionDisplay.Text;
            _teachPointRecipe.Description = textBoxDescriptionDisplay.Text;

            //Z1
            _teachPointRecipe.InputEESafeHeight = Convert.ToDouble(textBoxInputEESafeHeightPosition.Text);
            _teachPointRecipe.InputEEPickHeight = Convert.ToDouble(textBoxInputEEPickHeightPosition.Text);
            _teachPointRecipe.InputEEPlaceHeight_UpTab = Convert.ToDouble(textBoxInputEEPlaceHeightPosition_UpTab.Text);
            _teachPointRecipe.InputEEPlaceHeight_DownTab = Convert.ToDouble(textBoxInputEEPlaceHeightPosition_DownTab.Text);
            _teachPointRecipe.InputEEDycemHeight = Convert.ToDouble(textBoxInputEEDycemHeightPosition.Text);

            //Z2
            _teachPointRecipe.TestProbeSafeHeight = Convert.ToDouble(textBoxTestProbeSafeHeightPosition.Text);
            _teachPointRecipe.TestProbeTestHeight_UpTab = Convert.ToDouble(textBoxTestProbeTestHeightPosition_UpTab.Text);
            _teachPointRecipe.TestProbeTestHeight_DownTab = Convert.ToDouble(textBoxTestProbeTestHeightPosition_DownTab.Text);

            //Z3
            _teachPointRecipe.OutputEESafeHeight = Convert.ToDouble(textBoxOutputEESafeHeightPosition.Text);
            _teachPointRecipe.OutputEEPickHeight_UpTab = Convert.ToDouble(textBoxOutputEEPickHeightPosition_UpTab.Text);
            _teachPointRecipe.OutputEEPickHeight_DownTab = Convert.ToDouble(textBoxOutputEEPickHeightPosition_DownTab.Text);
            _teachPointRecipe.OutputEEPlaceHeight = Convert.ToDouble(textBoxOutputEEPlaceHeightPosition.Text);
            _teachPointRecipe.OutputEEDycemHeight = Convert.ToDouble(textBoxOutputEEDycemHeightPosition.Text);

            //Precisor_Station
            _teachPointRecipe.PrecisorSafePositionX = Convert.ToDouble(textBoxPrecisorSafePositionX.Text);
            _teachPointRecipe.PrecisorSafePositionY = Convert.ToDouble(textBoxPrecisorSafePositionY.Text);
            _teachPointRecipe.PrecisorSafePositionTheta = Convert.ToDouble(textBoxPrecisorSafePositionTheta.Text);

            _teachPointRecipe.PrecisorInputStationPositionX_UpTab = Convert.ToDouble(textBoxPrecisorInputStationPositionX_UpTab.Text);
            _teachPointRecipe.PrecisorInputStationPositionY_UpTab = Convert.ToDouble(textBoxPrecisorInputStationPositionY_UpTab.Text);
            _teachPointRecipe.PrecisorInputStationPositionTheta_UpTab = Convert.ToDouble(textBoxPrecisorInputStationPositionTheta_UpTab.Text);

            _teachPointRecipe.PrecisorInputStationPositionX_DownTab = Convert.ToDouble(textBoxPrecisorInputStationPositionX_DownTab.Text);
            _teachPointRecipe.PrecisorInputStationPositionY_DownTab = Convert.ToDouble(textBoxPrecisorInputStationPositionY_DownTab.Text);
            _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab = Convert.ToDouble(textBoxPrecisorInputStationPositionTheta_DownTab.Text);

            _teachPointRecipe.PrecisorTestStationPositionX_UpTab = Convert.ToDouble(textBoxPrecisorTestStationPositionX_UpTab.Text);
            _teachPointRecipe.PrecisorTestStationPositionY_UpTab = Convert.ToDouble(textBoxPrecisorTestStationPositionY_UpTab.Text);
            _teachPointRecipe.PrecisorTestStationPositionTheta_UpTab = Convert.ToDouble(textBoxPrecisorTestStationPositionTheta_UpTab.Text);

            _teachPointRecipe.PrecisorTestStationPositionX_DownTab = Convert.ToDouble(textBoxPrecisorTestStationPositionX_DownTab.Text);
            _teachPointRecipe.PrecisorTestStationPositionY_DownTab = Convert.ToDouble(textBoxPrecisorTestStationPositionY_DownTab.Text);
            _teachPointRecipe.PrecisorTestStationPositionTheta_DownTab = Convert.ToDouble(textBoxPrecisorTestStationPositionTheta_DownTab.Text);

            _teachPointRecipe.PrecisorOutputStationPositionX_UpTab = Convert.ToDouble(textBoxPrecisorOutputStationPositionX_UpTab.Text);
            _teachPointRecipe.PrecisorOutputStationPositionY_UpTab = Convert.ToDouble(textBoxPrecisorOutputStationPositionY_UpTab.Text);
            _teachPointRecipe.PrecisorOutputStationPositionTheta_UpTab = Convert.ToDouble(textBoxPrecisorOutputStationPositionTheta_UpTab.Text);

            _teachPointRecipe.PrecisorOutputStationPositionX_DownTab = Convert.ToDouble(textBoxPrecisorOutputStationPositionX_DownTab.Text);
            _teachPointRecipe.PrecisorOutputStationPositionY_DownTab = Convert.ToDouble(textBoxPrecisorOutputStationPositionY_DownTab.Text);
            _teachPointRecipe.PrecisorOutputStationPositionTheta_DownTab = Convert.ToDouble(textBoxPrecisorOutputStationPositionTheta_DownTab.Text);
        }

        private void WriteValueToDoc(XmlDocument _doc)
        {
            SetValue(null, "Version", _teachPointRecipe.Version.ToString(), _doc);
            SetValue(null, "Description", _teachPointRecipe.Description.ToString(), _doc);
            
            string section = "Input_EE_Z";
            SetValue(section, "SafeHeight", _teachPointRecipe.InputEESafeHeight.ToString(), _doc);
            SetValue(section, "PickHeight", _teachPointRecipe.InputEEPickHeight.ToString(), _doc);
            SetValue(section, "PlaceHeight_UpTab", _teachPointRecipe.InputEEPlaceHeight_UpTab.ToString(), _doc);
            SetValue(section, "PlaceHeight_DownTab", _teachPointRecipe.InputEEPlaceHeight_DownTab.ToString(), _doc);
            SetValue(section, "DycemHeight", _teachPointRecipe.InputEEDycemHeight.ToString(), _doc);

            section = "Test_Probe_Z";
            SetValue(section, "SafeHeight", _teachPointRecipe.TestProbeSafeHeight.ToString(), _doc);
            SetValue(section, "TestHeight_UpTab", _teachPointRecipe.TestProbeTestHeight_UpTab.ToString(), _doc);
            SetValue(section, "TestHeight_DownTab", _teachPointRecipe.TestProbeTestHeight_DownTab.ToString(), _doc);

            section = "Output_EE_Z";
            SetValue(section, "SafeHeight", _teachPointRecipe.OutputEESafeHeight.ToString(), _doc);
            SetValue(section, "PickHeight_UpTab", _teachPointRecipe.OutputEEPickHeight_UpTab.ToString(), _doc);
            SetValue(section, "PickHeight_DownTab", _teachPointRecipe.OutputEEPickHeight_DownTab.ToString(), _doc);
            SetValue(section, "PlaceHeight", _teachPointRecipe.OutputEEPlaceHeight.ToString(), _doc);
            SetValue(section, "DycemHeight", _teachPointRecipe.OutputEEDycemHeight.ToString(), _doc);

            section = "Precisor_Station";
            SetValue(section, "SafePositionX", _teachPointRecipe.PrecisorSafePositionX.ToString(), _doc);
            SetValue(section, "SafePositionY", _teachPointRecipe.PrecisorSafePositionY.ToString(), _doc);
            SetValue(section, "SafePositionTheta", _teachPointRecipe.PrecisorSafePositionTheta.ToString(), _doc);

            SetValue(section, "InputStationPositionX_UpTab", _teachPointRecipe.PrecisorInputStationPositionX_UpTab.ToString(), _doc);
            SetValue(section, "InputStationPositionY_UpTab", _teachPointRecipe.PrecisorInputStationPositionY_UpTab.ToString(), _doc);
            SetValue(section, "InputStationPositionTheta_UpTab", _teachPointRecipe.PrecisorInputStationPositionTheta_UpTab.ToString(), _doc);

            SetValue(section, "InputStationPositionX_DownTab", _teachPointRecipe.PrecisorInputStationPositionX_DownTab.ToString(), _doc);
            SetValue(section, "InputStationPositionY_DownTab", _teachPointRecipe.PrecisorInputStationPositionY_DownTab.ToString(), _doc);
            SetValue(section, "InputStationPositionTheta_DownTab", _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab.ToString(), _doc);

            SetValue(section, "TestStationPositionX_UpTab", _teachPointRecipe.PrecisorTestStationPositionX_UpTab.ToString(), _doc);
            SetValue(section, "TestStationPositionY_UpTab", _teachPointRecipe.PrecisorTestStationPositionY_UpTab.ToString(), _doc);
            SetValue(section, "TestStationPositionTheta_UpTab", _teachPointRecipe.PrecisorTestStationPositionTheta_UpTab.ToString(), _doc);

            SetValue(section, "TestStationPositionX_DownTab", _teachPointRecipe.PrecisorTestStationPositionX_DownTab.ToString(), _doc);
            SetValue(section, "TestStationPositionY_DownTab", _teachPointRecipe.PrecisorTestStationPositionY_DownTab.ToString(), _doc);
            SetValue(section, "TestStationPositionTheta_DownTab", _teachPointRecipe.PrecisorTestStationPositionTheta_DownTab.ToString(), _doc);

            SetValue(section, "OutputStationPositionX_UpTab", _teachPointRecipe.PrecisorOutputStationPositionX_UpTab.ToString(), _doc);
            SetValue(section, "OutputStationPositionY_UpTab", _teachPointRecipe.PrecisorOutputStationPositionY_UpTab.ToString(), _doc);
            SetValue(section, "OutputStationPositionTheta_UpTab", _teachPointRecipe.PrecisorOutputStationPositionTheta_UpTab.ToString(), _doc);

            SetValue(section, "OutputStationPositionX_DownTab", _teachPointRecipe.PrecisorOutputStationPositionX_DownTab.ToString(), _doc);
            SetValue(section, "OutputStationPositionY_DownTab", _teachPointRecipe.PrecisorOutputStationPositionY_DownTab.ToString(), _doc);
            SetValue(section, "OutputStationPositionTheta_DownTab", _teachPointRecipe.PrecisorOutputStationPositionTheta_DownTab.ToString(), _doc);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            string dir = string.Empty;

            try
            {
                SaveFileDialog saveform = new SaveFileDialog();
                saveform.Title = "Save Teach Point Recipe file";
                saveform.Filter = "Teach Point Recipe files (*.TeachPointRcp)|*.TeachPointRcp";

                if (saveform.ShowDialog() == DialogResult.OK)
                {
                    _fullPathName = saveform.FileName;
                    string [] pathElement = saveform.FileName.Split('\\');
                    _fileNameWithExt = pathElement[pathElement.Length - 1];
                    dir = _fullPathName.Remove(_fullPathName.Length - _fileNameWithExt.Length);
                    UpdateRecipeVariables();
                    WriteValueToDoc(_doc);
                    _doc.Save(_fullPathName);
                    HSTWorkcell.disableBoundaryCheck = false;
                    _teachPointRecipe.Name = _fileNameWithExt.Remove(_fileNameWithExt.IndexOf(".TeachPointRcp", 0), ".TeachPointRcp".Length);

                    LoadRecipeFromXmlFile(dir, _teachPointRecipe.Name, ".TeachPointRcp");
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                ParselMessageBox.Show("Save As error:", ex, MessageBoxIcon.Error,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.OK);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            CreateNewDoc();

            this.btnSaveAs.PerformClick();
        }

        private void CreateNewDoc()
        {
            _teachPointRecipe = new TeachPointRecipe();
            _doc = new XmlDocument();

            XmlElement root = _doc.CreateElement("TeachPointRecipe");
            _doc.AppendChild(root);

            WriteValueToDoc(_doc);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _teachPointRecipe = null;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_teachPointRecipe != null)
            {
                labelRecipeNameDisplay.Text = _teachPointRecipe.Name;
                textBoxVersionDisplay.Text = _teachPointRecipe.Version;
                textBoxDescriptionDisplay.Text = _teachPointRecipe.Description;

                //Input_EE_Z
                textBoxInputEESafeHeightPosition.Text = _teachPointRecipe.InputEESafeHeight.ToString();
                textBoxInputEEPickHeightPosition.Text = _teachPointRecipe.InputEEPickHeight.ToString();
                textBoxInputEEPlaceHeightPosition_UpTab.Text = _teachPointRecipe.InputEEPlaceHeight_UpTab.ToString();
                textBoxInputEEPlaceHeightPosition_DownTab.Text = _teachPointRecipe.InputEEPlaceHeight_DownTab.ToString();
                textBoxInputEEDycemHeightPosition.Text = _teachPointRecipe.InputEEDycemHeight.ToString();

                //Test_Probe_Z
                textBoxTestProbeSafeHeightPosition.Text = _teachPointRecipe.TestProbeSafeHeight.ToString();
                textBoxTestProbeTestHeightPosition_UpTab.Text = _teachPointRecipe.TestProbeTestHeight_UpTab.ToString();
                textBoxTestProbeTestHeightPosition_DownTab.Text = _teachPointRecipe.TestProbeTestHeight_DownTab.ToString();

                //Output_EE_Z
                textBoxOutputEESafeHeightPosition.Text = _teachPointRecipe.OutputEESafeHeight.ToString();
                textBoxOutputEEPickHeightPosition_UpTab.Text = _teachPointRecipe.OutputEEPickHeight_UpTab.ToString();
                textBoxOutputEEPickHeightPosition_DownTab.Text = _teachPointRecipe.OutputEEPickHeight_DownTab.ToString();
                textBoxOutputEEPlaceHeightPosition.Text = _teachPointRecipe.OutputEEPlaceHeight.ToString();
                textBoxOutputEEDycemHeightPosition.Text = _teachPointRecipe.OutputEEDycemHeight.ToString();

                //Prcisor Station
                textBoxPrecisorSafePositionX.Text = _teachPointRecipe.PrecisorSafePositionX.ToString();
                textBoxPrecisorSafePositionY.Text = _teachPointRecipe.PrecisorSafePositionY.ToString();
                textBoxPrecisorSafePositionTheta.Text = _teachPointRecipe.PrecisorSafePositionTheta.ToString();

                textBoxPrecisorInputStationPositionX_UpTab.Text = _teachPointRecipe.PrecisorInputStationPositionX_UpTab.ToString();
                textBoxPrecisorInputStationPositionY_UpTab.Text = _teachPointRecipe.PrecisorInputStationPositionY_UpTab.ToString();
                textBoxPrecisorInputStationPositionTheta_UpTab.Text = _teachPointRecipe.PrecisorInputStationPositionTheta_UpTab.ToString();

                textBoxPrecisorInputStationPositionX_DownTab.Text = _teachPointRecipe.PrecisorInputStationPositionX_DownTab.ToString();
                textBoxPrecisorInputStationPositionY_DownTab.Text = _teachPointRecipe.PrecisorInputStationPositionY_DownTab.ToString();
                textBoxPrecisorInputStationPositionTheta_DownTab.Text = _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab.ToString();

                textBoxPrecisorTestStationPositionX_UpTab.Text = _teachPointRecipe.PrecisorTestStationPositionX_UpTab.ToString();
                textBoxPrecisorTestStationPositionY_UpTab.Text = _teachPointRecipe.PrecisorTestStationPositionY_UpTab.ToString();
                textBoxPrecisorTestStationPositionTheta_UpTab.Text = _teachPointRecipe.PrecisorTestStationPositionTheta_UpTab.ToString();

                textBoxPrecisorTestStationPositionX_DownTab.Text = _teachPointRecipe.PrecisorTestStationPositionX_DownTab.ToString();
                textBoxPrecisorTestStationPositionY_DownTab.Text = _teachPointRecipe.PrecisorTestStationPositionY_DownTab.ToString();
                textBoxPrecisorTestStationPositionTheta_DownTab.Text = _teachPointRecipe.PrecisorTestStationPositionTheta_DownTab.ToString();

                textBoxPrecisorOutputStationPositionX_UpTab.Text = _teachPointRecipe.PrecisorOutputStationPositionX_UpTab.ToString();
                textBoxPrecisorOutputStationPositionY_UpTab.Text = _teachPointRecipe.PrecisorOutputStationPositionY_UpTab.ToString();
                textBoxPrecisorOutputStationPositionTheta_UpTab.Text = _teachPointRecipe.PrecisorOutputStationPositionTheta_UpTab.ToString();

                textBoxPrecisorOutputStationPositionX_DownTab.Text = _teachPointRecipe.PrecisorOutputStationPositionX_DownTab.ToString();
                textBoxPrecisorOutputStationPositionY_DownTab.Text = _teachPointRecipe.PrecisorOutputStationPositionY_DownTab.ToString();
                textBoxPrecisorOutputStationPositionTheta_DownTab.Text = _teachPointRecipe.PrecisorOutputStationPositionTheta_DownTab.ToString();
            }
            else
            {
                _teachPointRecipe = new TeachPointRecipe();//use for close temp new for display all defauth value, set to null after display value
                labelRecipeNameDisplay.Text = _teachPointRecipe.Name;
                textBoxVersionDisplay.Text = _teachPointRecipe.Version;
                textBoxDescriptionDisplay.Text = _teachPointRecipe.Description;

                //Input_EE_Z
                textBoxInputEESafeHeightPosition.Text = _teachPointRecipe.InputEESafeHeight.ToString();
                textBoxInputEEPickHeightPosition.Text = _teachPointRecipe.InputEEPickHeight.ToString();
                textBoxInputEEPlaceHeightPosition_UpTab.Text = _teachPointRecipe.InputEEPlaceHeight_UpTab.ToString();
                textBoxInputEEPlaceHeightPosition_DownTab.Text = _teachPointRecipe.InputEEPlaceHeight_DownTab.ToString();
                textBoxInputEEDycemHeightPosition.Text = _teachPointRecipe.InputEEDycemHeight.ToString();

                //Test_Probe_Z
                textBoxTestProbeSafeHeightPosition.Text = _teachPointRecipe.TestProbeSafeHeight.ToString();
                textBoxTestProbeTestHeightPosition_UpTab.Text = _teachPointRecipe.TestProbeTestHeight_UpTab.ToString();
                textBoxTestProbeTestHeightPosition_DownTab.Text = _teachPointRecipe.TestProbeTestHeight_DownTab.ToString();

                //Output_EE_Z
                textBoxOutputEESafeHeightPosition.Text = _teachPointRecipe.OutputEESafeHeight.ToString();
                textBoxOutputEEPickHeightPosition_UpTab.Text = _teachPointRecipe.OutputEEPickHeight_UpTab.ToString();
                textBoxOutputEEPickHeightPosition_DownTab.Text = _teachPointRecipe.OutputEEPickHeight_DownTab.ToString();
                textBoxOutputEEPlaceHeightPosition.Text = _teachPointRecipe.OutputEEPlaceHeight.ToString();
                textBoxOutputEEDycemHeightPosition.Text = _teachPointRecipe.OutputEEDycemHeight.ToString();

                //Prcisor Station
                textBoxPrecisorSafePositionX.Text = _teachPointRecipe.PrecisorSafePositionX.ToString();
                textBoxPrecisorSafePositionY.Text = _teachPointRecipe.PrecisorSafePositionY.ToString();
                textBoxPrecisorSafePositionTheta.Text = _teachPointRecipe.PrecisorSafePositionTheta.ToString();

                textBoxPrecisorInputStationPositionX_UpTab.Text = _teachPointRecipe.PrecisorInputStationPositionX_UpTab.ToString();
                textBoxPrecisorInputStationPositionY_UpTab.Text = _teachPointRecipe.PrecisorInputStationPositionY_UpTab.ToString();
                textBoxPrecisorInputStationPositionTheta_UpTab.Text = _teachPointRecipe.PrecisorInputStationPositionTheta_UpTab.ToString();

                textBoxPrecisorInputStationPositionX_DownTab.Text = _teachPointRecipe.PrecisorInputStationPositionX_DownTab.ToString();
                textBoxPrecisorInputStationPositionY_DownTab.Text = _teachPointRecipe.PrecisorInputStationPositionY_DownTab.ToString();
                textBoxPrecisorInputStationPositionTheta_DownTab.Text = _teachPointRecipe.PrecisorInputStationPositionTheta_DownTab.ToString();

                textBoxPrecisorTestStationPositionX_UpTab.Text = _teachPointRecipe.PrecisorTestStationPositionX_UpTab.ToString();
                textBoxPrecisorTestStationPositionY_UpTab.Text = _teachPointRecipe.PrecisorTestStationPositionY_UpTab.ToString();
                textBoxPrecisorTestStationPositionTheta_UpTab.Text = _teachPointRecipe.PrecisorTestStationPositionTheta_UpTab.ToString();

                textBoxPrecisorTestStationPositionX_DownTab.Text = _teachPointRecipe.PrecisorTestStationPositionX_DownTab.ToString();
                textBoxPrecisorTestStationPositionY_DownTab.Text = _teachPointRecipe.PrecisorTestStationPositionY_DownTab.ToString();
                textBoxPrecisorTestStationPositionTheta_DownTab.Text = _teachPointRecipe.PrecisorTestStationPositionTheta_DownTab.ToString();

                textBoxPrecisorOutputStationPositionX_UpTab.Text = _teachPointRecipe.PrecisorOutputStationPositionX_UpTab.ToString();
                textBoxPrecisorOutputStationPositionY_UpTab.Text = _teachPointRecipe.PrecisorOutputStationPositionY_UpTab.ToString();
                textBoxPrecisorOutputStationPositionTheta_UpTab.Text = _teachPointRecipe.PrecisorOutputStationPositionTheta_UpTab.ToString();

                textBoxPrecisorOutputStationPositionX_DownTab.Text = _teachPointRecipe.PrecisorOutputStationPositionX_DownTab.ToString();
                textBoxPrecisorOutputStationPositionY_DownTab.Text = _teachPointRecipe.PrecisorOutputStationPositionY_DownTab.ToString();
                textBoxPrecisorOutputStationPositionTheta_DownTab.Text = _teachPointRecipe.PrecisorOutputStationPositionTheta_DownTab.ToString();
                _teachPointRecipe = null;
            }

            //Input_EE_Z
            buttonTeachInputEESafeHeight.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachInputEEPickHeight.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachInputEEPlaceHeight_UpTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachInputEEPlaceHeight_DownTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachInputEEDycemHeight.Enabled = _teachPointRecipe == null ? false : true;

            //Test_Probe_Z
            buttonTeachTestProbeSafeHeight.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachTestProbeTestHeight_UpTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachTestProbeTestHeight_DownTab.Enabled = _teachPointRecipe == null ? false : true;

            //Output_EE_Z
            buttonTeachOutputEESafeHeight.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachOutputEEPickHeight_UpTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachOutputEEPickHeight_DownTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachOutputEEPlaceHeight.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachOutputEEDycemHeight.Enabled = _teachPointRecipe == null ? false : true;

            //Precisor Station
            buttonTeachPrecisorSafePosition.Enabled = _teachPointRecipe == null ? false : true;

            //Precisor Station-up tab
            buttonTeachPrecisorInputStationPosition_UpTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachPrecisorTestStationPosition_UpTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachPrecisorOutputStationPosition_UpTab.Enabled = _teachPointRecipe == null ? false : true;

            //Precisor Station-down tab
            buttonTeachPrecisorInputStationPosition_DownTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachPrecisorTestStationPosition_DownTab.Enabled = _teachPointRecipe == null ? false : true;
            buttonTeachPrecisorOutputStationPosition_DownTab.Enabled = _teachPointRecipe == null ? false : true;

        }

        public void SetValue(string section, string key, string keyval, XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            XmlNode secNode;
            if(section != null)
                secNode = doc.SelectSingleNode("TeachPointRecipe" + "/" + section);
            else
                secNode = doc.SelectSingleNode("TeachPointRecipe");

            if (secNode == null)
            {
                XmlElement element = doc.CreateElement(section);
                secNode = root.AppendChild(element);
            }

            XmlNode valNode = secNode.SelectSingleNode(key);
            if (valNode == null)
            {
                valNode = doc.CreateNode(XmlNodeType.Element, key, null);
                secNode.AppendChild(valNode);
            }
            valNode.InnerText = keyval;
        }

        private void buttonTeachInputEESafeHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();

            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z1,
                                                textBoxInputEESafeHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Safe Height");
        }

        private void buttonTeachInputEEPickHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();

            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z1,
                                                textBoxInputEEPickHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Pick Height");
        }

        private void buttonTeachInputEEPlaceHeight_UpTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z1,
                                                textBoxInputEEPlaceHeightPosition_UpTab,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Place Height (Up Tab)");
        }

        private void buttonTeachInputEEPlaceHeight_DownTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z1,
                                                textBoxInputEEPlaceHeightPosition_DownTab,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Place Height (Down Tab)");
        }

        private void buttonTeachInputEEDycemHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();

            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z1,
                                                textBoxInputEEDycemHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Dycem Height");
        }

        private void buttonTeachTestProbeSafeHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z2,
                                                textBoxTestProbeSafeHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Safe Height");
        }

        private void buttonTeachTestProbeTestHeight_UpTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z2,
                textBoxTestProbeTestHeightPosition_UpTab,
                mp,
                SingleAxisForm.AxisOrientation.UpDown, "Test Height (Up Tab)");
        }

        private void buttonTeachTestProbeTestHeight_DownTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z2,
                                                textBoxTestProbeTestHeightPosition_DownTab,
                                                mp,
                                                SingleAxisForm.AxisOrientation.UpDown, "Test Height (Down Tab)");
        }

        private void buttonTeachOutputEESafeHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z3,
                                                textBoxOutputEESafeHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Safe Height");
        }

        private void buttonTeachOutputEEPickHeight_UpTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z3,
                                                textBoxOutputEEPickHeightPosition_UpTab,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Pick Height (Up Tab)");
        }

        private void buttonTeachOutputEEPickHeight_DownTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z3,
                                                textBoxOutputEEPickHeightPosition_DownTab,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Pick Height (Down Tab)");
        }        

        private void buttonTeachOutputEEPlaceHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z3,
                                                textBoxOutputEEPlaceHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Place Height");
        }

        private void buttonTeachOutputEEDycemHeight_Click(object sender, EventArgs e)
        {
            MoveProfileBase mp = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowSingleAxisTeachForm(HSTIOManifest.Axes.Z3,
                                                textBoxOutputEEDycemHeightPosition,
                                                mp,
                                                 SingleAxisForm.AxisOrientation.UpDown, "Dycem Height");
        }

        private void buttonTeachPrecisorSafePosition_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorSafePositionX,
                                                    textBoxPrecisorSafePositionY,
                                                    textBoxPrecisorSafePositionTheta,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Safe Position");
        }

        private void buttonTeachPrecisorInputStationPosition_DownTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorInputStationPositionX_DownTab,
                                                    textBoxPrecisorInputStationPositionY_DownTab,
                                                    textBoxPrecisorInputStationPositionTheta_DownTab,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Input Station Position (Down Tab)");
        }

        private void buttonTeachPrecisorTestStationPosition_UpTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorTestStationPositionX_UpTab,
                                                    textBoxPrecisorTestStationPositionY_UpTab,
                                                    textBoxPrecisorTestStationPositionTheta_UpTab,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Test Station Position (Up Tab)");
        }

        private void buttonTeachPrecisorTestStationPosition_DownTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorTestStationPositionX_DownTab,
                                                    textBoxPrecisorTestStationPositionY_DownTab,
                                                    textBoxPrecisorTestStationPositionTheta_DownTab,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Test Station Position (Down Tab)");

        }

        private void buttonTeachPrecisorInputStationPosition_UpTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorInputStationPositionX_UpTab,
                                                    textBoxPrecisorInputStationPositionY_UpTab,
                                                    textBoxPrecisorInputStationPositionTheta_UpTab,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Input Station Position (Up Tab)");
        }

        private void buttonTeachPrecisorOutputStationPosition_UpTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorOutputStationPositionX_UpTab,
                                                    textBoxPrecisorOutputStationPositionY_UpTab,
                                                    textBoxPrecisorOutputStationPositionTheta_UpTab,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Output Station Positionn (Up Tab)");

        }

        private void buttonTeachPrecisorOutputStationPosition_DownTab_Click(object sender, EventArgs e)
        {
            MoveProfileBase mpX = new MoveProfileBase();
            MoveProfileBase mpY = new MoveProfileBase();
            MoveProfileBase mpTheta = new MoveProfileBase();
            HSTMachine.Workcell.Utils.ShowMultipleAxisTeachForm(HSTIOManifest.Axes.X,
                                                    HSTIOManifest.Axes.Y,
                                                    HSTIOManifest.Axes.Theta,
                                                    textBoxPrecisorOutputStationPositionX_DownTab,
                                                    textBoxPrecisorOutputStationPositionY_DownTab,
                                                    textBoxPrecisorOutputStationPositionTheta_DownTab,
                                                    mpX,
                                                    mpY,
                                                    mpTheta, "Output Station Positionn (Down Tab)");
        }

        private void ComboBoxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                this.FormCogDisplay.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCogDisplayClose);
            }

            if (ComboBoxCamera.SelectedIndex == 0)
            {
                this.cogDisplayPanel = new CogDisplayPanel(this.InputCamera.cogAcqFifo);
            }
            
            else if (ComboBoxCamera.SelectedIndex == 1)
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

        private void ButtonGrabImage_Click(object sender, EventArgs e)
        {
            if (this.cogtoolblock == null || this.InputCamera == null /*|| this.FiducialCamera == null*/ || this.OutputCamera == null)
                return;
            if (!this.cogtoolblock.Inputs.Contains("InputImage"))
                return;
            if (RadioButtonCamera.Checked)
            {

                switch (this.ComboBoxCamera.SelectedIndex)
                {
                    case 0:
                        this.InputCamera.GrabManual(true);
                        this.cogtoolblock.Inputs["InputImage"].Value = this.InputCamera.grabImage;
                        break;
                   
                    case 1:
                        this.OutputCamera.GrabManual(true);
                        this.cogtoolblock.Inputs["InputImage"].Value = this.OutputCamera.grabImage;
                        break;
                    default:
                        return;
                }

                if (this.cogtoolblock.Inputs.Contains("InputImage"))
                {
                    cogToolBlockEditV21.Subject = cogtoolblock;
                    cogToolBlockEditV21.Subject.Run();

                }

            }
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            string imageFilename;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

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

        public void FormCogDisplayClose(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.ButtonLiveImage.Enabled = true;
            this.FormCogDisplay.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.FormCogDisplayClose);
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

        private void cogToolBlockEditV21_SubjectChanged(object sender, EventArgs e)
        {
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

        private void btnExport_Click(object sender, EventArgs e)
        {

        }

        private void tabPageMeasurementTest_Click(object sender, EventArgs e)
        {

        }

        public panelLDUMeasurement GetPanelLDUMeasurement()
        {
            return panelLDUMeasurement1;
        }

        public panelGompertzCalculation2 GetGomperztCalculation()
        {
            return panelGompertzCalculation21;
        }
        private void tabControlRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlRecipe.SelectedIndex == 3)
            {
                try
                {
                    TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_ldu_configuration_2_Message_ID, TestProbeAPICommand.HST_get_ldu_configuration_2_Message_Name, TestProbeAPICommand.HST_get_ldu_configuration_2_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                    HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);
                    panelLDUMeasurement1.UpdateSetting();
                }
                catch { }


            }
        }

        public void UpdateLDUSettingPanel()
        {
            try
            {
                panelLDUMeasurement1.UpdateSetting();
            }
            catch { }
        }

        private void InitializeLDUConfiguration()
        {

        }

        private void tabControlRecipe_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
