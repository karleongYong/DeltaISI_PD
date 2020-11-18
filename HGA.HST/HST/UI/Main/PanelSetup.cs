using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using Seagate.AAS.HGA.HST.Utils;
using System.Runtime.InteropServices;

namespace Seagate.AAS.HGA.HST.UI.Main
{
    public partial class PanelSetup : UserControl
    {
        HSTWorkcell _workcell;
        PropertyGridEditor _propertyGridEditorApplication;
        PropertyGridEditor _propertyGridEditorCalibration;
        PropertyGridEditor _propertyGrideditorAutomation;
        SettingsEditor editorApplication;
        SettingsEditor editorCalibration;
        SettingsEditor editorAutomation;        

        public PanelSetup()
        {
            InitializeComponent();            
        }

        public PanelSetup(HSTWorkcell workcell)
        {
            InitializeComponent();
            _workcell = workcell;

            settingsPanelUsersConfig.AssignObject(HSTMachine.Workcell.HSTSettings.getUserAccessSettings(), HSTMachine.Workcell.HSTSettings.UsersSettingsFilePath);
            settingsPanelUsersConfig.OnLoad += new EventHandler(OnSettingsLoad);
            settingsPanelUsersConfig.OnSave += new EventHandler(OnSettingsSave);

            editorApplication = new PropertyGridEditor(HSTMachine.Workcell.HSTSettings);
            _propertyGridEditorApplication = (PropertyGridEditor)editorApplication;
            editorApplication.FilePath = HSTMachine.Workcell.HSTSettings.HSTSettingsFilePath;                       
            editorApplication.Location = new System.Drawing.Point(3, 6);
            editorApplication.Size = new System.Drawing.Size(755, 600);
            _propertyGridEditorApplication.getConfigListView().LabelWrap = false;
            
            this.tabMachine.Controls.Add(pnlBypass);
            this.tabMachine.Controls.Add(pnlDryRun);
            this.tabMachine.Controls.Add(editorApplication);

            editorApplication.SaveSettingsToXMLFile += HSTMachine.Workcell.HSTSettings.SaveSettingsToFile;
            _propertyGridEditorApplication.OperationModeChanged += OperationModeChanged;
            editorCalibration = new PropertyGridEditor(HSTMachine.Workcell.CalibrationSettings);
            _propertyGridEditorCalibration = (PropertyGridEditor)editorCalibration;
            editorCalibration.FilePath = HSTMachine.Workcell.HSTSettings.CalibrationSettingsFilePath;
            editorCalibration.Size = new System.Drawing.Size(900, 600);
            _propertyGridEditorCalibration.getConfigListView().LabelWrap = false;
            this.tabCalibration.Controls.Add(editorCalibration);

            editorAutomation = new PropertyGridEditor(HSTMachine.Workcell.SetupSettings);
            _propertyGrideditorAutomation = (PropertyGridEditor)editorAutomation;
            editorAutomation.FilePath = HSTMachine.Workcell.HSTSettings.SetupSettingsFilePath;
            editorAutomation.Size = new System.Drawing.Size(1100, 600);
            _propertyGrideditorAutomation.getConfigListView().LabelWrap = false;
            this.tabSetup.Controls.Add(editorAutomation);

            editorCalibration.SaveSettingsToXMLFile += HSTMachine.Workcell.CalibrationSettings.SaveSettingsToFile;
            editorAutomation.SaveSettingsToXMLFile += HSTMachine.Workcell.SetupSettings.SaveSettingsToFile;
            editorAutomation.LoadSettingsFromXMLFile += HSTMachine.Workcell.SetupSettings.LoadSettingsFromFile;
            
            settingsPanelUsersConfig.UpdateUsersSettings += HSTMachine.Workcell.UpdateUsersSettings;

            NamedCollectionEditor<CarrierSettings>.CollectionChanged += RefreshSimulatedPartPropertyGrid;
            NamedCollectionEditor<HGAProductType>.CollectionChanged += RefreshHGACarrierTypePropertyGrid;
            NamedCollectionEditor<TestProbeType>.CollectionChanged += RefreshTestProbeTypePropertyGrid;
            NamedCollectionEditor<User>.CollectionChanged += RefreshUserAccountPropertGrid;

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass)
            {
                pnlBypass.Enabled = true;
            }
            else
            {
                pnlBypass.Enabled = false;
            }

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
            {
                pnlDryRun.Enabled = true;
            }
            else
            {
                pnlDryRun.Enabled = false;
            }

            chkBypassInputAndOutputEEsPickAndPlace.Checked = HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace;
            chkBypassMeasurementTestAtTestProbe.Checked = HSTMachine.Workcell.HSTSettings.Install.BypassMeasurementTestAtTestProbe;
            chkBypassRFIDReadAtInputStation.Checked = HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput;
            chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked = HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput;
            chkBypassVisionAtInputTurnStation.Checked = HSTMachine.Workcell.HSTSettings.Install.BypassVisionAtInputTurnStation;
            chkBypassVisionAtOutputStation.Checked = HSTMachine.Workcell.HSTSettings.Install.BypassVisionAtOutput;
            rdoWithBoat.Checked = !HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat;
            rdoWithoutBoat.Checked = HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat;
            
            editorApplication.buttonLoad.Visible = false;
            editorApplication.Invalidate();
            //25-Mar-2020 Hold Reset
           // ANCReset();
        }

        private void PopulateHGAProducts()
        {
            try
            {
                if (CalibrationSettings.Instance != null)
                {
                    if (CalibrationSettings.Instance.MeasurementTest != null)
                    {
                        txtRecipeName.Text = CommonFunctions.Instance.ProductRecipeName;
                        txtProductName.Text = CommonFunctions.Instance.MeasurementTestRecipe.ProductName;
                    }
                }

                
            }
            catch (Exception ex)
            {
            }
        }
        
        private void PopulateTriggeringInformation()
        {
            HSTMachine.Workcell.HSTSettings.Load();
            
            checkBoxTriggerByCarrierEnable.Checked = HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierEnabled;
            touchscreenNumBoxTrigHourPerCarrierCount.Text = HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierHour.ToString();
            touchscreenNumBoxTrigByCarrierCount.Text = HSTMachine.Workcell.HSTSettings.TriggeringSetting.TrigerByCarrierCount.ToString();
            touchscreenNumBoxPhase1Min.Text = HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase1Min.ToString();
            touchscreenNumBoxPhase2Min.Text = HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase2Min.ToString();
            touchscreenNumBoxTotalBuyoffCarrier.Text =
                HSTMachine.Workcell.HSTSettings.TriggeringSetting.TotalCarrierForBuyOff.ToString();

            checkBoxResistanceCheckEnable.Checked = HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.EnabledResistanceCheck;
            touchscreenNumBoxResistanceCheckByHour.Text = HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByHourCounter.ToString();
            touchscreenNumBoxResistanceCheckByPart.Text = HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByPartCounter.ToString();

            checkBoxErrorCodeTriggering.Checked = HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByErrorCodeEnabled;
            touchscreenNumBoxCRDLPercent.Text = HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByErrorCodePercent.ToString();
            touchscreenNumBoxCRDLTotalRun.Text = HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByErrorCodePartPerPeriod.ToString();
            if(HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckType == ResistanceCheckConfig.ResistanceCheckType.CheckByHour)
                radioButtonResistanceCheckPerHour.Checked = true;
            else
                radioButtonResistanceCheckPerHour.Checked = false;

            if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckType == ResistanceCheckConfig.ResistanceCheckType.CheckByPartCounter)
                radioButtonResistanceCheckPerPartCount.Checked = true;
            else
                radioButtonResistanceCheckPerPartCount.Checked = false;
            txtTICMachine1.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
            txtTICMachine2.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;
        }

        private void PopulateCCCParameterSetting()
        {
            CCCParameter ancSetting = new CCCParameter();
            if (CommonFunctions.Instance.IsRunningWithNewTSR)
            {
                ancSetting.TestRunGroup = Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.TestRunCount);
                ancSetting.YeildLimit = CommonFunctions.Instance.MeasurementTestRecipe.YieldLimit;
                ancSetting.YeildTarget =
                    CommonFunctions.Instance.MeasurementTestRecipe.YieldTarget;
                ancSetting.DefectCounterLimit = Convert.ToInt32(CommonFunctions.Instance.MeasurementTestRecipe.CountLimit);
                ancSetting.Alpha = CommonFunctions.Instance.MeasurementTestRecipe.GoodBetweenBad;

            }
            else
            {
                ancSetting = HSTMachine.Workcell.HSTSettings.CccParameterSetting;
            }



            checkBoxEnableCCC.Checked = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Enabled;
            touchscreenNumBoxAlpha.Text = ancSetting.Alpha.ToString();
            touchscreenNumBoxCounterLimit.Text = ancSetting.DefectCounterLimit.ToString();
            touchscreenNumBoxTestRunCount.Text = ancSetting.TestRunGroup.ToString();
            touchscreenNumBoxYieldLimit.Text = ancSetting.YeildLimit.ToString();
            touchscreenNumBoxYieldTarget.Text = ancSetting.YeildTarget.ToString();

            touchscreenNumBoxAlphaCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.Alpha.ToString();
            touchscreenNumBoxCounterLimitCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.DefectCounterLimit.ToString();
            touchscreenNumBoxTestRunCountCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.TestRunGroup.ToString();
            touchscreenNumBoxYieldLimitCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.YeildLimit.ToString();
            touchscreenNumBoxYieldTargetCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.YeildTarget.ToString();
            checkBoxTicEnableAlert.Checked = HSTMachine.Workcell.HSTSettings.CccParameterSetting.EnableAlertMsg;
            checkBoxHstEnableAlert.Checked = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.EnableAlertMsg;


            txtTICMachine1.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
            txtTICMachine2.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;

        }

        public void RefreshSimulatedPartPropertyGrid(object sender, EventArgs e)
        {
            _propertyGridEditorApplication.Refresh();
        }

        public void RefreshHGACarrierTypePropertyGrid(object sender, EventArgs e)
        {
            _propertyGridEditorCalibration.Refresh();
        }

        public void RefreshTestProbeTypePropertyGrid(object sender, EventArgs e)
        {
            _propertyGridEditorCalibration.Refresh();
            _propertyGridEditorCalibration.Accept();
        }

        public void RefreshUserAccountPropertGrid(object sender, EventArgs e)
        {
            settingsPanelUsersConfig.propertyGrid_Refresh();
        }

        public void OperationModeChanged(object sender, EventArgs e)
        {            
            OperationModeEventArgs operationModeEventArgs = e as OperationModeEventArgs;
            string strOperationMode = operationModeEventArgs.strOperationMode;

            // Disable simulation if vision is enabled
            if (HSTMachine.Workcell.HSTSettings.Install.EnableVision == true)
            {
                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    HSTMachine.Workcell.OperationMode = OperationMode.Auto;
                    HSTMachine.Workcell.HSTSettings.Install.OperationMode = OperationMode.Auto;
                    strOperationMode = "Auto";
                }
            }

            if (strOperationMode.Equals("Bypass"))
            {
                pnlBypass.Enabled = true;
                pnlDryRun.Enabled = false;
                HSTMachine.Workcell.OperationMode = OperationMode.Bypass;                
            }
            else if (strOperationMode.Equals("DryRun"))
            {
                pnlBypass.Enabled = false;
                pnlDryRun.Enabled = true;
                HSTMachine.Workcell.OperationMode = OperationMode.DryRun;
            }
            else
            {
                pnlBypass.Enabled = false;
                pnlDryRun.Enabled = false;

                if (strOperationMode.Equals("Auto"))
                {
                    HSTMachine.Workcell.OperationMode = OperationMode.Auto;                                        
                }
                else if (strOperationMode.Equals("Simulation"))
                {
                    HSTMachine.Workcell.OperationMode = OperationMode.Simulation;                    
                }                
            }
        } 

        private void OnSettingsSave(object sender, EventArgs e)
        {
            HSTMachine.Workcell.HSTSettings.Save();
        }

        private void OnSettingsLoad(object sender, EventArgs e)
        {
            HSTMachine.Workcell.HSTSettings.Load();
            if (!string.IsNullOrEmpty(CommonFunctions.Instance.ProductRecipeName))
                _workcell.RaiseLoadDefaultRecipeEvent();

        }

        private void PanelSetup_VisibleChanged(object sender, EventArgs e)
        {            
            HSTMachine.Workcell.HSTSettings.LoadUsers();
            if (Visible)
            {
                PopulateTriggeringInformation();
                PopulateCCCParameterSetting();
                PopulateHGAProducts();
            }
            EnableDisableTab();            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabAdaptiveSampleData.SelectedIndex)
            {    
                case 0:  // Application Configuration
                    editorApplication.buttonLoad.Visible = false;
                    editorApplication.Invalidate();
                    break;  
                case 1:  // Hardware Calibration Configuration
                    editorCalibration.buttonLoad.Visible = false;
                    editorCalibration.Invalidate();
                    break;  
                case 2:  // Automation Configuration
                    editorAutomation.buttonLoad.Visible = true;
                    editorAutomation.Invalidate();
                    break;  
                case 3:  // Measurement Configuration
                    btnConfigurationLoadConfiguration_Click(sender, e);
                    break;                
            }
            HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().UserChanged -= UpdateTabAccessibility;
            HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().UserChanged += UpdateTabAccessibility;
            EnableDisableTab();
            editorAutomation.Invalidate();
        }

        private void UpdateTabAccessibility(object sender, EventArgs e)
        {
            EnableDisableTab();
        }

        private void EnableDisableTab()
        {
            if (HSTMachine.Instance.MainForm != null)
            {                                
                if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level >= Seagate.AAS.HGA.HST.Settings.UserLevel.Administrator)
                {
                    settingsPanelUsersConfig.Enabled = true;
                    this.editorApplication.Enabled = true;
                    this.editorCalibration.Enabled = true;
                    this.editorAutomation.Enabled = true;
                    this.pnlMeasurementConfiguration.Enabled = true;
                    this.groupBoxTriggering.Enabled = true;
                    this.groupBoxCCCSetting.Enabled = true;
                }
                else if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Engineer)
                {
                    settingsPanelUsersConfig.Enabled = true;
                    this.editorApplication.Enabled = true;
                    this.editorCalibration.Enabled = true;
                    this.editorAutomation.Enabled = true;
                    this.pnlMeasurementConfiguration.Enabled = true;
                    this.groupBoxTriggering.Enabled = true;
                    this.groupBoxCCCSetting.Enabled = true;
                }
                else if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Technician)
                {
                    this.editorCalibration.Enabled = false;
                    settingsPanelUsersConfig.Enabled = false;
                    this.editorApplication.Enabled = true;
                    this.editorAutomation.Enabled = false;
                    this.pnlMeasurementConfiguration.Enabled = false;
                    this.groupBoxTriggering.Enabled = false;
                    this.groupBoxCCCSetting.Enabled = false;
                }
                else
                {
                    settingsPanelUsersConfig.Enabled = false;
                    this.editorApplication.Enabled = false;
                    this.editorCalibration.Enabled = false;
                    this.editorAutomation.Enabled = false;
                    this.pnlMeasurementConfiguration.Enabled = false;
                    this.groupBoxTriggering.Enabled = false;
                    this.groupBoxCCCSetting.Enabled = false;
                }
            }
        }

        #region tabMeasurementConfiguration
        // Measurement Configuration Tab
        private void txtConfigurationSetupCH1Current_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupCH2Current_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupCH3Current_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupCH4Current_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupCH5Current_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupCH6Current_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }


        private void txtConfigurationSetupCH6IbCurrent_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }
        private void txtConfigurationSetupAvgCurrentSampleCount_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupTempTimeConstant_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupFrequency_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupBiasVoltage_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupPeak2PeakVoltage_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void txtConfigurationSetupAvgVoltageSampleCount_TextChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCh1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCh2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCh3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCh4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCh5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCh6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCapa1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupCapa2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupHGA10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR1C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR2C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR3C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR4C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR5C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR6C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR7C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR8C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR9C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR10C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR11C12_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C1_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C2_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C3_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C4_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C5_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C6_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C7_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C8_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C9_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C10_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupR12C11_CheckedChanged(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            tabMeasurementConfiguration.Refresh();
        }

        private void chkConfigurationSetupLDU_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                    if (this.checkBoxLDU.Checked)
                    {
                        this.txtConfigurationSetupCH6IbCurrent.Enabled = true;
                        HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
                    }
                    else
                    {
                        this.txtConfigurationSetupCH6IbCurrent.Enabled = false;
                        this.txtConfigurationSetupCH6IbCurrent.Text = "0";
                        HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
                        
                    }
            }
            catch (Exception ex)
            {
                if (this.checkBoxLDU.Checked)
                {
                    this.checkBoxLDU.CheckState = CheckState.Unchecked;
                    Notify.PopUpError("Feature Not Supported", string.Format("The current firmware {0}.{1} does not support LDU testing. Please change the configuration recipe or upgrade the measurement board fimware.", HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion.MajorRevision.ToString(), HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion.MinorRevision.ToString()));
                       
                }
            }

            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = true;
            this.tabMeasurementConfiguration.Refresh();
            

        }

        private void pnlMeasurementConfiguration_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.SolidBrush myBrush;
            if (HSTMachine.Instance.MainForm.IsConfigurationSetupTempered == true)
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            }
            else
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            }

            System.Drawing.Graphics pnlMeasurementConfigurationGraphics = pnlMeasurementConfiguration.CreateGraphics();
            pnlMeasurementConfigurationGraphics.FillEllipse(myBrush, new Rectangle(580, 460, 40, 40));
            myBrush.Dispose();
            pnlMeasurementConfigurationGraphics.Dispose();
        }

        public void btnConfigurationLoadConfiguration_Click(object sender, EventArgs e)
        {
            string fileName = CommonFunctions.Instance.ProductRecipeName + CommonFunctions.RecipeExt;

            CommonFunctions.Instance.LoadMeasurementTestRecipe(fileName);
			HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = false;
            txtConfigurationSetupConversionBoardID.Text = CommonFunctions.Instance.ConversionBoardID.ToString();
            txtRecipeName.Text = CommonFunctions.Instance.ProductRecipeName;
            txtProductName.Text = CommonFunctions.Instance.MeasurementTestRecipe.ProductName;

            CommonFunctions.Instance.LoadConfigurationRecipe();            
            
            //Bias Current
            txtConfigurationSetupCH1Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch1BiasCurrent.ToString();
            txtConfigurationSetupCH2Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch2BiasCurrent.ToString();
            txtConfigurationSetupCH3Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch3BiasCurrent.ToString();
            txtConfigurationSetupCH4Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch4BiasCurrent.ToString();
            txtConfigurationSetupCH5Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch5BiasCurrent.ToString();
            txtConfigurationSetupCH6Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6BiasCurrent.ToString();
            //txtConfigurationSetupAvgCurrentSampleCount.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.BiasCurrentSampleCountForAverage.ToString();

            //Temperature
            txtConfigurationSetupTempTimeConstant.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.TimeConstant.ToString();
            
            //LDU
            checkBoxLDU.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable);
            checkBoxSwapCH3AndCH4.Checked = CommonFunctions.Instance.ConfigurationSetupRecipe.SwapEnable;
            //if (checkBoxLDU.Checked)
            //    txtConfigurationSetupCH6IbCurrent.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6IbBiasCurrent.ToString();
            //else
            //    txtConfigurationSetupCH6IbCurrent.Text = "0";

            //BiasVoltage
            txtConfigurationSetupFrequency.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Frequency.ToString();
            txtConfigurationSetupBiasVoltage.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltage.ToString();
            txtConfigurationSetupPeak2PeakVoltage.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.PeakVoltage.ToString();
            txtConfigurationSetupAvgVoltageSampleCount.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltageSampleCountForAverage.ToString();

            //HGA Channel
            chkConfigurationSetupCh1.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1);
            chkConfigurationSetupCh2.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1);
            chkConfigurationSetupCh3.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1);
            chkConfigurationSetupCh4.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1);
            chkConfigurationSetupCh5.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1);
            chkConfigurationSetupCh6.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1);
            chkConfigurationSetupCapa1.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh1 == 1);
            chkConfigurationSetupCapa2.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh2 == 1);

            //HGA
            chkConfigurationSetupHGA1.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1);
            chkConfigurationSetupHGA2.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1);
            chkConfigurationSetupHGA3.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1);
            chkConfigurationSetupHGA4.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1);
            chkConfigurationSetupHGA5.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1);
            chkConfigurationSetupHGA6.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1);
            chkConfigurationSetupHGA7.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1);
            chkConfigurationSetupHGA8.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1);
            chkConfigurationSetupHGA9.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1);
            chkConfigurationSetupHGA10.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1);

            //Row1
            chkConfigurationSetupR1C2.Checked = false;
            chkConfigurationSetupR1C3.Checked = false;
            chkConfigurationSetupR1C4.Checked = false;
            chkConfigurationSetupR1C5.Checked = false;
            chkConfigurationSetupR1C6.Checked = false;
            chkConfigurationSetupR1C7.Checked = false;
            chkConfigurationSetupR1C8.Checked = false;
            chkConfigurationSetupR1C9.Checked = false;
            chkConfigurationSetupR1C10.Checked = false;
            chkConfigurationSetupR1C11.Checked = false;
            chkConfigurationSetupR1C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 2)
            {
                chkConfigurationSetupR1C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 3)
            {
                chkConfigurationSetupR1C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 4)
            {
                chkConfigurationSetupR1C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 5)
            {
                chkConfigurationSetupR1C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 6)
            {
                chkConfigurationSetupR1C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 7)
            {
                chkConfigurationSetupR1C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 8)
            {
                chkConfigurationSetupR1C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 9)
            {
                chkConfigurationSetupR1C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 10)
            {
                chkConfigurationSetupR1C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 11)
            {
                chkConfigurationSetupR1C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 12)
            {
                chkConfigurationSetupR1C12.Checked = true;
            }

            //Row2
            chkConfigurationSetupR2C1.Checked = false;
            chkConfigurationSetupR2C3.Checked = false;
            chkConfigurationSetupR2C4.Checked = false;
            chkConfigurationSetupR2C5.Checked = false;
            chkConfigurationSetupR2C6.Checked = false;
            chkConfigurationSetupR2C7.Checked = false;
            chkConfigurationSetupR2C8.Checked = false;
            chkConfigurationSetupR2C9.Checked = false;
            chkConfigurationSetupR2C10.Checked = false;
            chkConfigurationSetupR2C11.Checked = false;
            chkConfigurationSetupR2C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 1)
            {
                chkConfigurationSetupR2C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 3)
            {
                chkConfigurationSetupR2C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 4)
            {
                chkConfigurationSetupR2C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 5)
            {
                chkConfigurationSetupR2C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 6)
            {
                chkConfigurationSetupR2C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 7)
            {
                chkConfigurationSetupR2C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 8)
            {
                chkConfigurationSetupR2C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 9)
            {
                chkConfigurationSetupR2C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 10)
            {
                chkConfigurationSetupR2C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 11)
            {
                chkConfigurationSetupR2C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 12)
            {
                chkConfigurationSetupR2C12.Checked = true;
            }

            //Row3
            chkConfigurationSetupR3C1.Checked = false;
            chkConfigurationSetupR3C2.Checked = false;
            chkConfigurationSetupR3C4.Checked = false;
            chkConfigurationSetupR3C5.Checked = false;
            chkConfigurationSetupR3C6.Checked = false;
            chkConfigurationSetupR3C7.Checked = false;
            chkConfigurationSetupR3C8.Checked = false;
            chkConfigurationSetupR3C9.Checked = false;
            chkConfigurationSetupR3C10.Checked = false;
            chkConfigurationSetupR3C11.Checked = false;
            chkConfigurationSetupR3C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 1)
            {
                chkConfigurationSetupR3C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 2)
            {
                chkConfigurationSetupR3C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 4)
            {
                chkConfigurationSetupR3C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 5)
            {
                chkConfigurationSetupR3C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 6)
            {
                chkConfigurationSetupR3C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 7)
            {
                chkConfigurationSetupR3C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 8)
            {
                chkConfigurationSetupR3C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 9)
            {
                chkConfigurationSetupR3C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 10)
            {
                chkConfigurationSetupR3C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 11)
            {
                chkConfigurationSetupR3C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 12)
            {
                chkConfigurationSetupR3C12.Checked = true;
            }

            //Row4
            chkConfigurationSetupR4C1.Checked = false;
            chkConfigurationSetupR4C2.Checked = false;
            chkConfigurationSetupR4C3.Checked = false;
            chkConfigurationSetupR4C5.Checked = false;
            chkConfigurationSetupR4C6.Checked = false;
            chkConfigurationSetupR4C7.Checked = false;
            chkConfigurationSetupR4C8.Checked = false;
            chkConfigurationSetupR4C9.Checked = false;
            chkConfigurationSetupR4C10.Checked = false;
            chkConfigurationSetupR4C11.Checked = false;
            chkConfigurationSetupR4C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 1)
            {
                chkConfigurationSetupR4C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 2)
            {
                chkConfigurationSetupR4C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 3)
            {
                chkConfigurationSetupR4C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 5)
            {
                chkConfigurationSetupR4C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 6)
            {
                chkConfigurationSetupR4C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 7)
            {
                chkConfigurationSetupR4C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 8)
            {
                chkConfigurationSetupR4C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 9)
            {
                chkConfigurationSetupR4C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 10)
            {
                chkConfigurationSetupR4C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 11)
            {
                chkConfigurationSetupR4C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 12)
            {
                chkConfigurationSetupR4C12.Checked = true;
            }

            //Row5
            chkConfigurationSetupR5C1.Checked = false;
            chkConfigurationSetupR5C2.Checked = false;
            chkConfigurationSetupR5C3.Checked = false;
            chkConfigurationSetupR5C4.Checked = false;
            chkConfigurationSetupR5C6.Checked = false;
            chkConfigurationSetupR5C7.Checked = false;
            chkConfigurationSetupR5C8.Checked = false;
            chkConfigurationSetupR5C9.Checked = false;
            chkConfigurationSetupR5C10.Checked = false;
            chkConfigurationSetupR5C11.Checked = false;
            chkConfigurationSetupR5C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 1)
            {
                chkConfigurationSetupR5C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 2)
            {
                chkConfigurationSetupR5C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 3)
            {
                chkConfigurationSetupR5C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 4)
            {
                chkConfigurationSetupR5C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 6)
            {
                chkConfigurationSetupR5C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 7)
            {
                chkConfigurationSetupR5C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 8)
            {
                chkConfigurationSetupR5C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 9)
            {
                chkConfigurationSetupR5C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 10)
            {
                chkConfigurationSetupR5C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 11)
            {
                chkConfigurationSetupR5C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 12)
            {
                chkConfigurationSetupR5C12.Checked = true;
            }

            //Row6
            chkConfigurationSetupR6C1.Checked = false;
            chkConfigurationSetupR6C2.Checked = false;
            chkConfigurationSetupR6C3.Checked = false;
            chkConfigurationSetupR6C4.Checked = false;
            chkConfigurationSetupR6C5.Checked = false;
            chkConfigurationSetupR6C7.Checked = false;
            chkConfigurationSetupR6C8.Checked = false;
            chkConfigurationSetupR6C9.Checked = false;
            chkConfigurationSetupR6C10.Checked = false;
            chkConfigurationSetupR6C11.Checked = false;
            chkConfigurationSetupR6C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 1)
            {
                chkConfigurationSetupR6C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 2)
            {
                chkConfigurationSetupR6C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 3)
            {
                chkConfigurationSetupR6C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 4)
            {
                chkConfigurationSetupR6C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 5)
            {
                chkConfigurationSetupR6C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 7)
            {
                chkConfigurationSetupR6C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 8)
            {
                chkConfigurationSetupR6C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 9)
            {
                chkConfigurationSetupR6C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 10)
            {
                chkConfigurationSetupR6C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 11)
            {
                chkConfigurationSetupR6C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 12)
            {
                chkConfigurationSetupR6C12.Checked = true;
            }

            //Row7
            chkConfigurationSetupR7C1.Checked = false;
            chkConfigurationSetupR7C2.Checked = false;
            chkConfigurationSetupR7C3.Checked = false;
            chkConfigurationSetupR7C4.Checked = false;
            chkConfigurationSetupR7C5.Checked = false;
            chkConfigurationSetupR7C6.Checked = false;
            chkConfigurationSetupR7C8.Checked = false;
            chkConfigurationSetupR7C9.Checked = false;
            chkConfigurationSetupR7C10.Checked = false;
            chkConfigurationSetupR7C11.Checked = false;
            chkConfigurationSetupR7C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 1)
            {
                chkConfigurationSetupR7C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 2)
            {
                chkConfigurationSetupR7C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 3)
            {
                chkConfigurationSetupR7C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 4)
            {
                chkConfigurationSetupR7C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 5)
            {
                chkConfigurationSetupR7C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 6)
            {
                chkConfigurationSetupR7C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 8)
            {
                chkConfigurationSetupR7C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 9)
            {
                chkConfigurationSetupR7C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 10)
            {
                chkConfigurationSetupR7C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 11)
            {
                chkConfigurationSetupR7C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 12)
            {
                chkConfigurationSetupR7C12.Checked = true;
            }

            //Row8
            chkConfigurationSetupR8C1.Checked = false;
            chkConfigurationSetupR8C2.Checked = false;
            chkConfigurationSetupR8C3.Checked = false;
            chkConfigurationSetupR8C4.Checked = false;
            chkConfigurationSetupR8C5.Checked = false;
            chkConfigurationSetupR8C6.Checked = false;
            chkConfigurationSetupR8C7.Checked = false;
            chkConfigurationSetupR8C9.Checked = false;
            chkConfigurationSetupR8C10.Checked = false;
            chkConfigurationSetupR8C11.Checked = false;
            chkConfigurationSetupR8C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 1)
            {
                chkConfigurationSetupR8C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 2)
            {
                chkConfigurationSetupR8C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 3)
            {
                chkConfigurationSetupR8C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 4)
            {
                chkConfigurationSetupR8C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 5)
            {
                chkConfigurationSetupR8C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 6)
            {
                chkConfigurationSetupR8C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 7)
            {
                chkConfigurationSetupR8C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 9)
            {
                chkConfigurationSetupR8C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 10)
            {
                chkConfigurationSetupR8C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 11)
            {
                chkConfigurationSetupR8C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 12)
            {
                chkConfigurationSetupR8C12.Checked = true;
            }

            //Row9
            chkConfigurationSetupR9C1.Checked = false;
            chkConfigurationSetupR9C2.Checked = false;
            chkConfigurationSetupR9C3.Checked = false;
            chkConfigurationSetupR9C4.Checked = false;
            chkConfigurationSetupR9C5.Checked = false;
            chkConfigurationSetupR9C6.Checked = false;
            chkConfigurationSetupR9C7.Checked = false;
            chkConfigurationSetupR9C8.Checked = false;
            chkConfigurationSetupR9C10.Checked = false;
            chkConfigurationSetupR9C11.Checked = false;
            chkConfigurationSetupR9C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 1)
            {
                chkConfigurationSetupR9C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 2)
            {
                chkConfigurationSetupR9C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 3)
            {
                chkConfigurationSetupR9C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 4)
            {
                chkConfigurationSetupR9C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 5)
            {
                chkConfigurationSetupR9C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 6)
            {
                chkConfigurationSetupR9C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 7)
            {
                chkConfigurationSetupR9C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 8)
            {
                chkConfigurationSetupR9C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 10)
            {
                chkConfigurationSetupR9C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 11)
            {
                chkConfigurationSetupR9C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 12)
            {
                chkConfigurationSetupR9C12.Checked = true;
            }

            //Row10
            chkConfigurationSetupR10C1.Checked = false;
            chkConfigurationSetupR10C2.Checked = false;
            chkConfigurationSetupR10C3.Checked = false;
            chkConfigurationSetupR10C4.Checked = false;
            chkConfigurationSetupR10C5.Checked = false;
            chkConfigurationSetupR10C6.Checked = false;
            chkConfigurationSetupR10C7.Checked = false;
            chkConfigurationSetupR10C8.Checked = false;
            chkConfigurationSetupR10C9.Checked = false;
            chkConfigurationSetupR10C11.Checked = false;
            chkConfigurationSetupR10C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 1)
            {
                chkConfigurationSetupR10C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 2)
            {
                chkConfigurationSetupR10C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 3)
            {
                chkConfigurationSetupR10C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 4)
            {
                chkConfigurationSetupR10C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 5)
            {
                chkConfigurationSetupR10C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 6)
            {
                chkConfigurationSetupR10C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 7)
            {
                chkConfigurationSetupR10C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 8)
            {
                chkConfigurationSetupR10C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 9)
            {
                chkConfigurationSetupR10C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 11)
            {
                chkConfigurationSetupR10C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 12)
            {
                chkConfigurationSetupR10C12.Checked = true;
            }

            //Row11
            chkConfigurationSetupR11C1.Checked = false;
            chkConfigurationSetupR11C2.Checked = false;
            chkConfigurationSetupR11C3.Checked = false;
            chkConfigurationSetupR11C4.Checked = false;
            chkConfigurationSetupR11C5.Checked = false;
            chkConfigurationSetupR11C6.Checked = false;
            chkConfigurationSetupR11C7.Checked = false;
            chkConfigurationSetupR11C8.Checked = false;
            chkConfigurationSetupR11C9.Checked = false;
            chkConfigurationSetupR11C10.Checked = false;
            chkConfigurationSetupR11C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 1)
            {
                chkConfigurationSetupR11C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 2)
            {
                chkConfigurationSetupR11C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 3)
            {
                chkConfigurationSetupR11C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 4)
            {
                chkConfigurationSetupR11C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 5)
            {
                chkConfigurationSetupR11C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 6)
            {
                chkConfigurationSetupR11C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 7)
            {
                chkConfigurationSetupR11C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 8)
            {
                chkConfigurationSetupR11C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 9)
            {
                chkConfigurationSetupR11C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 10)
            {
                chkConfigurationSetupR11C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 12)
            {
                chkConfigurationSetupR11C12.Checked = true;
            }

            //Row12
            chkConfigurationSetupR12C1.Checked = false;
            chkConfigurationSetupR12C2.Checked = false;
            chkConfigurationSetupR12C3.Checked = false;
            chkConfigurationSetupR12C4.Checked = false;
            chkConfigurationSetupR12C5.Checked = false;
            chkConfigurationSetupR12C6.Checked = false;
            chkConfigurationSetupR12C7.Checked = false;
            chkConfigurationSetupR12C8.Checked = false;
            chkConfigurationSetupR12C9.Checked = false;
            chkConfigurationSetupR12C10.Checked = false;
            chkConfigurationSetupR12C11.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 1)
            {
                chkConfigurationSetupR12C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 2)
            {
                chkConfigurationSetupR12C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 3)
            {
                chkConfigurationSetupR12C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 4)
            {
                chkConfigurationSetupR12C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 5)
            {
                chkConfigurationSetupR12C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 6)
            {
                chkConfigurationSetupR12C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 7)
            {
                chkConfigurationSetupR12C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 8)
            {
                chkConfigurationSetupR12C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 9)
            {
                chkConfigurationSetupR12C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 10)
            {
                chkConfigurationSetupR12C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 11)
            {
                chkConfigurationSetupR12C11.Checked = true;
            }

            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = false;
            HSTMachine.Workcell.HSTSettings.Install.EquipmentID = ReadMachineNumberFromSRVFile();
            HSTMachine.Workcell.HSTSettings.EquipmentType = ReadMachineTypeFromSRVFile();

            tabMeasurementConfiguration.Refresh();
        }

        public void btnConfigurationSaveConfiguration_Click(object sender, EventArgs e)
        {
            if (ValidateShortCircuitSettings() == false)
            {
                return;
            }

            //SaveConfigurationToFile();
            //DownloadConfigurationToMicroProcessor();
        }

        private void btnGetBiasCurrentFromUProcessor_Click(object sender, EventArgs e)
        {
            TestProbeAPICommand APICommand;
            if (HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion.MajorRevision.ToString().Equals("0"))
            {
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_meas_configuration_Message_ID, TestProbeAPICommand.HST_get_res_meas_configuration_Message_Name, TestProbeAPICommand.HST_get_res_meas_configuration_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            }
            else
            {
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_meas2_configuration_Message_ID, TestProbeAPICommand.HST_get_res_meas2_configuration_Message_Name, TestProbeAPICommand.HST_get_res_meas2_configuration_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            }

            // bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);
        }

        public void DownloadConfigurationToMicroProcessor()
        {
            //if (this.checkBoxLDU.Checked)
            //{
            //    int Ia;
            //    int Ib;
            //    // parse the string
            //    if (!Int32.TryParse(this.txtConfigurationSetupCH6Current.Text.ToString(), out Ia))
            //    {
            //        Notify.PopUpError("Command NOT Sent To uProcessor", "Reason: Ia is invalid.");
                   
            //        return;
            //    }

            //    if (!Int32.TryParse(txtConfigurationSetupCH6IbCurrent.Text.ToString(), out Ib))
            //    {
            //        Notify.PopUpError("Command NOT Sent To uProcessor", "Reason: Ib is invalid.");
                    
            //        return;
            //    }
            //    if (Ia < 0)
            //    {
            //        Notify.PopUpError("Command NOT Sent To uProcessor", "Reason: Ia less than 0.");
                    
            //        return;
            //    }

            //    if (Ib < 0)
            //    {
            //        Notify.PopUpError("Command NOT Sent To uProcessor", "Reason: Ib less than 0.");
                    
            //        return;
            //    }
            //    //Ensure Ib > Ia

            //    if (Ia > Ib)
            //    {
            //        //
            //        Notify.PopUpError("Command NOT Sent To uProcessor", "Reason: Ib is less than Ia for resistance channel 6.");
                   
            //        return;
            //    }

            //}
            CommonFunctions.Instance.LoadConfigurationRecipe();
            byte[] ByteArray;
            TestProbeAPICommand APICommand;
            byte[] ByteArrayFromStructure;

            if (HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion.MajorRevision.ToString().Equals("0"))
            {
                // TestProbe2ConfigResistanceMeasurement            
                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch1BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch1BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch1BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch1BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch2BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch2BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch2BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch2BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch3BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch3BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch3BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch3BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch4BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch4BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch4BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch4BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch5BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch5BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch5BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch5BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch6BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.Ch6BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(1);
                HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement.SampleCountForAverage = ByteArray[0];

                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe2ConfigResistanceMeasurement);

                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_res_meas_Message_ID, TestProbeAPICommand.HST_config_res_meas_Message_Name, TestProbeAPICommand.HST_config_res_meas_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            }
            else 
            {
                // TestProbe2ConfigResistanceMeasurement            
                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch1BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch1BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch1BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch1BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch2BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch2BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch2BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch2BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch3BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch3BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch3BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch3BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch4BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch4BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch4BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch4BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch5BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch5BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch5BiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch5BiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6BiasCurrent.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6BiasCurrent.ToString()));
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch6IaBiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch6IaBiasCurrentMSB = ByteArray[1];

                ByteArray = BitConverter.GetBytes(0);
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch6IbBiasCurrentLSB = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.Ch6IbBiasCurrentMSB = ByteArray[1];


                ByteArray = BitConverter.GetBytes(1);
                HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2.SampleCountForAverage = ByteArray[0];

                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe57ConfigResistanceMeasurement2);

                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_res_meas2_Message_ID, TestProbeAPICommand.HST_config_res_meas2_Message_Name, TestProbeAPICommand.HST_config_res_meas2_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            
            }

            // TestProbe3ConfigCapacitanceMeasurement
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.Frequency.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.Frequency.ToString()));
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.FrequencyLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.FrequencyMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltage.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltage.ToString()));
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.BiasVoltageLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.BiasVoltageMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.PeakVoltage.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.PeakVoltage.ToString()));
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.PeakVoltageLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.PeakVoltageMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltageSampleCountForAverage.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltageSampleCountForAverage.ToString()));
            HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement.SampleCountForAverage = ByteArray[0];

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement);
//for photodiode no lcr
            // TestProbe4ConfigShortDetection
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.WPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.WMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.TAPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.TAMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.WHPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.WHMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.RHPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.RHMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.R1PlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.R1MinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.R2PlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing;
            HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection.R2MinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing;

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe4ConfigShortDetection);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_short_detection_Message_ID, TestProbeAPICommand.HST_config_short_detection_Message_Name, TestProbeAPICommand.HST_config_short_detection_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


            // TestProbe5MeasurementChannelEnable
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh1Writer = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh2TA = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh3WriteHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh4ReadHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh5Read1 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh6Read2 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh1 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh1;
            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh2 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh2;

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_meas_channel_enable_Message_ID, TestProbeAPICommand.HST_meas_channel_enable_Message_Name, TestProbeAPICommand.HST_meas_channel_enable_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


            // TestProbe6HGAEnable
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA1 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA2 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA3 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA4 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA5 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA6 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA7 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA8 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA9 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9;
            HSTMachine.Instance.MainForm.TestProbe6HGAEnable.HGA10 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10;

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe6HGAEnable);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_hga_enable_Message_ID, TestProbeAPICommand.HST_hga_enable_Message_Name, TestProbeAPICommand.HST_hga_enable_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            if (!HSTMachine.Workcell.IsBaselineRatioSpecCompared)
            {
                //HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio
                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceVoltRatio.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceVoltRatio.ToString()));
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioLSB_CH1 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioMSB_CH1 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceVoltRatio.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceVoltRatio.ToString()));
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioLSB_CH2 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioMSB_CH2 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceVoltRatio.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceVoltRatio.ToString()));
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioLSB_CH3 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioMSB_CH3 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceVoltRatio.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceVoltRatio.ToString()));
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioLSB_CH4 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioMSB_CH4 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceVoltRatio.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceVoltRatio.ToString()));
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioLSB_CH5 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioMSB_CH5 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceVoltRatio.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceVoltRatio.ToString()));
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioLSB_CH6 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio.CurrentRatioMSB_CH6 = ByteArray[1];


                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe66SetCurrentRatio);

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_ch_short_det_cur_ratio_Message_ID, TestProbeAPICommand.HST_set_ch_short_det_cur_ratio_Message_Name, TestProbeAPICommand.HST_set_ch_short_det_cur_ratio_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            }

            if (!HSTMachine.Workcell.IsToleranceSpecShortTestCompared)
            {
                //int limitBit = 0;

                //Lower Limit
                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceToleranceLowSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceToleranceLowSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH1 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH1 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH1 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH1 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceToleranceLowSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceToleranceLowSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH2 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH2 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH2 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH2 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceToleranceLowSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceToleranceLowSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH3 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH3 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH3 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH3 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceToleranceLowSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceToleranceLowSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH4 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH4 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH4 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH4 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceLowSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH5 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH5 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH5 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH5 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceToleranceLowSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceToleranceLowSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH6 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH6 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH6 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH6 = ByteArray[3];


                //limitBit = 0;
                //ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(limitBit.ToString()) ? 0 : int.Parse(limitBit.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.Set_limits = 0;

                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold);

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_short_detection_threshold2_Message_ID, TestProbeAPICommand.HST_set_short_detection_threshold2_Message_Name, TestProbeAPICommand.HST_set_short_detection_threshold2_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);



                //Upper Limit
                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceToleranceUpperSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceToleranceUpperSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH1 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH1 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH1 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH1 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceToleranceUpperSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceToleranceUpperSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH2 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH2 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH2 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH2 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceToleranceUpperSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceToleranceUpperSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH3 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH3 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH3 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH3 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceToleranceUpperSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceToleranceUpperSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH4 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH4 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH4 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH4 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceUpperSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceToleranceUpperSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH5 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH5 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH5 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH5 = ByteArray[3];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceToleranceUpperSpec.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceToleranceUpperSpec.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBLow_CH6 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageLSBMid_CH6 = ByteArray[1];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBLow_CH6 = ByteArray[2];
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.ThresholdVoltageMSBHigh_CH6 = ByteArray[3];

                //limitBit = 1;
                //ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(limitBit.ToString()) ? 0 : int.Parse(limitBit.ToString()));
                HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold.Set_limits = 1;

                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe59SetShortDetectionThreshold);

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_short_detection_threshold2_Message_ID, TestProbeAPICommand.HST_set_short_detection_threshold2_Message_Name, TestProbeAPICommand.HST_set_short_detection_threshold2_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


            }

            if (!_workcell.IsVolThresholdSpecCompared)
            {
                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold1LowLimit.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold1LowLimit.ToString()));
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBLow1 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBMid1 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold2LowLimit.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold2LowLimit.ToString()));
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBLow2 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBMid2 = ByteArray[1];

                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.HiLowLimit = 0;
                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold);
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_short_detection_vol_threshold_Message_ID, TestProbeAPICommand.HST_set_short_detection_vol_threshold_Message_Name, TestProbeAPICommand.HST_set_short_detection_vol_threshold_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold1HiLimit.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold1HiLimit.ToString()));
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBLow1 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBMid1 = ByteArray[1];

                ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold2HiLimit.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.MeasurementTestRecipe.VolThreshold2HiLimit.ToString()));
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBLow2 = ByteArray[0];
                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.VolThresholdLSBMid2 = ByteArray[1];

                HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold.HiLowLimit = 1;
                ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe68SetShortDetectionVolThreshold);
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_short_detection_vol_threshold_Message_ID, TestProbeAPICommand.HST_set_short_detection_vol_threshold_Message_Name, TestProbeAPICommand.HST_set_short_detection_vol_threshold_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            }


            // TestProbe32ConfigTemperatureMeasurement
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(CommonFunctions.Instance.ConfigurationSetupRecipe.TimeConstant.ToString()) ? 0 : int.Parse(CommonFunctions.Instance.ConfigurationSetupRecipe.TimeConstant.ToString()));
            HSTMachine.Instance.MainForm.TestProbe32ConfigTemperatureMeasurement.TimeConstant = ByteArray[0];

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe32ConfigTemperatureMeasurement);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_temp_meas_Message_ID, TestProbeAPICommand.HST_config_temp_meas_Message_Name, TestProbeAPICommand.HST_config_temp_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //Get firmware version
            if (HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion.MinorRevision.ToString().Equals("0"))
            {
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_firmware_version_Message_ID, TestProbeAPICommand.HST_get_firmware_version_Message_Name, TestProbeAPICommand.HST_get_firmware_version_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            }

            //Set parameter swap CH3 and CH4
            HSTMachine.Instance.MainForm.TestProbe81SetSwapCH3AndCH4.SwapCh3AndCh4 = Convert.ToByte(CommonFunctions.Instance.ConfigurationSetupRecipe.SwapEnable);
            byte[] ByteArraySwapCH3AndCH4Structure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe81SetSwapCH3AndCH4);
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_swap_ch3_and_ch4_Message_ID, TestProbeAPICommand.HST_swap_ch3_and_ch4_Message_Name, TestProbeAPICommand.HST_swap_ch3_and_ch4_Message_Size, ByteArraySwapCH3AndCH4Structure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //Get Short detect vol threshold (67)
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_ch_short_det_cur_ratio_Message_ID, TestProbeAPICommand.HST_get_ch_short_det_cur_ratio_Message_Name, TestProbeAPICommand.HST_get_ch_short_det_cur_ratio_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //Get Short detect vol threshold (60)
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_threshold2_Message_ID, TestProbeAPICommand.HST_get_short_detection_threshold2_Message_Name, TestProbeAPICommand.HST_get_short_detection_threshold2_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //Get Short detect vol threshold (71)
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_vol_threshold_Message_ID, TestProbeAPICommand.HST_get_short_detection_vol_threshold_Message_Name, TestProbeAPICommand.HST_get_short_detection_vol_threshold_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //ykl Download LDU configuration to microprocessor
      
        
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.enable_ldu = Convert.ToByte(CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDU4PtMode = Convert.ToByte(!CommonFunctions.Instance.ConfigurationSetupRecipe.SweepEnable);

            int no_of_led_steps = (int)((CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU2ndPoint - CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU1stPoint) / CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDUStep) + 1;
            int no_of_ldu_steps = (int)((CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint - CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint) / CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep) + 1;

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU1stPoint);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDStartCurrentLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDStartCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDU2ndPoint);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDStopCurrentLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDStopCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentLEDCh6LDUStep);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDSteppingSizeLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDSteppingSizeMSB = ByteArray[1];


            ByteArray = BitConverter.GetBytes(1);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDVoltageSampleSize = ByteArray[0];

            ByteArray = BitConverter.GetBytes(no_of_led_steps);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LEDNumberOfSteps = ByteArray[0];

            //LDU
            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUStartCurrentLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUStartCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUStopCurrentLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUStopCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDUStep);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUSteppingSizeLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUSteppingSizeMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(1);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUVoltageSampleSize = ByteArray[0];

            ByteArray = BitConverter.GetBytes(no_of_ldu_steps);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUNumberOfSteps = ByteArray[0];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU1stPoint);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep1LSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep1MSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrentCh6LDU2ndPoint);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep2LSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep2MSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrent3rdPointforIThreshold);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep3LSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep3MSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes((Int16)CommonFunctions.Instance.MeasurementTestRecipe.BiasCurrent4thPointforIThreshold);
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep4LSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.LDUISweep4MSB = ByteArray[1];


            //delay


            ByteArray = BitConverter.GetBytes(65535); //65535 = ffff means not going to configure
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.timeIntervalLSB = ByteArray[0];
            HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2.timeIntervalMSB = ByteArray[1];
            //delay

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe85SetLDUConfiguration_2);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_ldu_configuration_2_Message_ID, TestProbeAPICommand.HST_set_ldu_configuration_2_Message_Name, TestProbeAPICommand.HST_set_ldu_configuration_2_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_ldu_configuration_2_Message_ID, TestProbeAPICommand.HST_get_ldu_configuration_2_Message_Name, TestProbeAPICommand.HST_get_ldu_configuration_2_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //ykl Download LDU configuration to microprocessor

            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);
            HSTMachine.Instance.MainForm.IsConfigurationSetupTempered = false;
            tabMeasurementConfiguration.Refresh();
        }

        private bool IsValidShortDetectionConfiguration(out bool IsRow, out int RowColumnNumber)
        {
            bool ValidShortCircuitConfiguration = true;
            IsRow = true;
            RowColumnNumber = 0;

            //Row1
            int row1CheckedCount = 0;
            if (chkConfigurationSetupR1C2.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C3.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C4.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C5.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C6.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C7.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C8.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C9.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C10.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C11.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C12.Checked)
            {
                row1CheckedCount++;
            }

            //Row2
            int row2CheckedCount = 0;
            if (chkConfigurationSetupR2C1.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C3.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C4.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C5.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C6.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C7.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C8.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C9.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C10.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C11.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C12.Checked)
            {
                row2CheckedCount++;
            }

            //Row3
            int row3CheckedCount = 0;
            if (chkConfigurationSetupR3C1.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C2.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C4.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C5.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C6.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C7.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C8.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C9.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C10.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C11.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C12.Checked)
            {
                row3CheckedCount++;
            }

            //Row4
            int row4CheckedCount = 0;
            if (chkConfigurationSetupR4C1.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C2.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C3.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C5.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C6.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C7.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C8.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C9.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C10.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C11.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C12.Checked)
            {
                row4CheckedCount++;
            }

            //Row5
            int row5CheckedCount = 0;
            if (chkConfigurationSetupR5C1.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C2.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C3.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C4.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C6.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C7.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C8.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C9.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C10.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C11.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C12.Checked)
            {
                row5CheckedCount++;
            }

            //Row6
            int row6CheckedCount = 0;
            if (chkConfigurationSetupR6C1.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C2.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C3.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C4.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C5.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C7.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C8.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C9.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C10.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C11.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C12.Checked)
            {
                row6CheckedCount++;
            }

            //Row7
            int row7CheckedCount = 0;
            if (chkConfigurationSetupR7C1.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C2.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C3.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C4.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C5.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C6.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C8.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C9.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C10.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C11.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C12.Checked)
            {
                row7CheckedCount++;
            }

            //Row8
            int row8CheckedCount = 0;
            if (chkConfigurationSetupR8C1.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C2.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C3.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C4.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C5.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C6.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C7.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C9.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C10.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C11.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C12.Checked)
            {
                row8CheckedCount++;
            }

            //Row9
            int row9CheckedCount = 0;
            if (chkConfigurationSetupR9C1.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C2.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C3.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C4.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C5.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C6.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C7.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C8.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C10.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C11.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C12.Checked)
            {
                row9CheckedCount++;
            }

            //Row10
            int row10CheckedCount = 0;
            if (chkConfigurationSetupR10C1.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C2.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C3.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C4.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C5.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C6.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C7.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C8.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C9.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C11.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C12.Checked)
            {
                row10CheckedCount++;
            }

            //Row11
            int row11CheckedCount = 0;
            if (chkConfigurationSetupR11C1.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C2.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C3.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C4.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C5.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C6.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C7.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C8.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C9.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C10.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C12.Checked)
            {
                row11CheckedCount++;
            }

            //Row12
            int row12CheckedCount = 0;
            if (chkConfigurationSetupR12C1.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C2.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C3.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C4.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C5.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C6.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C7.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C8.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C9.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C10.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C11.Checked)
            {
                row12CheckedCount++;
            }


            if (row1CheckedCount > 1 || row2CheckedCount > 1 || row3CheckedCount > 1 || row4CheckedCount > 1 || row5CheckedCount > 1
                || row6CheckedCount > 1 || row7CheckedCount > 1 || row8CheckedCount > 1 || row9CheckedCount > 1 || row10CheckedCount > 1
                || row11CheckedCount > 1 || row12CheckedCount > 1)
            {
                ValidShortCircuitConfiguration = false;
                IsRow = true;

                if (row1CheckedCount > 1)
                {
                    RowColumnNumber = 1;
                }
                else if (row2CheckedCount > 1)
                {
                    RowColumnNumber = 2;
                }
                else if (row3CheckedCount > 1)
                {
                    RowColumnNumber = 3;
                }
                else if (row4CheckedCount > 1)
                {
                    RowColumnNumber = 4;
                }
                else if (row5CheckedCount > 1)
                {
                    RowColumnNumber = 5;
                }
                else if (row6CheckedCount > 1)
                {
                    RowColumnNumber = 6;
                }
                else if (row7CheckedCount > 1)
                {
                    RowColumnNumber = 7;
                }
                else if (row8CheckedCount > 1)
                {
                    RowColumnNumber = 8;
                }
                else if (row9CheckedCount > 1)
                {
                    RowColumnNumber = 9;
                }
                else if (row10CheckedCount > 1)
                {
                    RowColumnNumber = 10;
                }
                else if (row11CheckedCount > 1)
                {
                    RowColumnNumber = 11;
                }
                else
                {
                    RowColumnNumber = 12;
                }
            }


            //Column1
            int column1CheckedCount = 0;
            if (chkConfigurationSetupR2C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR3C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR4C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR5C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR6C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR7C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR8C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR9C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR10C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR11C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR12C1.Checked)
            {
                column1CheckedCount++;
            }

            //Column2
            int column2CheckedCount = 0;
            if (chkConfigurationSetupR1C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR3C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR4C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR5C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR6C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR7C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR8C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR9C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR10C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR11C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR12C2.Checked)
            {
                column2CheckedCount++;
            }

            //Column3
            int column3CheckedCount = 0;
            if (chkConfigurationSetupR1C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR2C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR4C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR5C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR6C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR7C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR8C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR9C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR10C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR11C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR12C3.Checked)
            {
                column3CheckedCount++;
            }

            //Column4
            int column4CheckedCount = 0;
            if (chkConfigurationSetupR1C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR2C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR3C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR5C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR6C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR7C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR8C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR9C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR10C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR11C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR12C4.Checked)
            {
                column4CheckedCount++;
            }

            //Column5
            int column5CheckedCount = 0;
            if (chkConfigurationSetupR1C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR2C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR3C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR4C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR6C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR7C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR8C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR9C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR10C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR11C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR12C5.Checked)
            {
                column5CheckedCount++;
            }

            //Column6
            int column6CheckedCount = 0;
            if (chkConfigurationSetupR1C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR2C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR3C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR4C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR5C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR7C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR8C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR9C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR10C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR11C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR12C6.Checked)
            {
                column6CheckedCount++;
            }

            //Column7
            int column7CheckedCount = 0;
            if (chkConfigurationSetupR1C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR2C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR3C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR4C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR5C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR6C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR8C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR9C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR10C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR11C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR12C7.Checked)
            {
                column7CheckedCount++;
            }

            //Column8
            int column8CheckedCount = 0;
            if (chkConfigurationSetupR1C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR2C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR3C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR4C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR5C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR6C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR7C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR9C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR10C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR11C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR12C8.Checked)
            {
                column8CheckedCount++;
            }

            //Column9
            int column9CheckedCount = 0;
            if (chkConfigurationSetupR1C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR2C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR3C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR4C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR5C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR6C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR7C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR8C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR10C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR11C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR12C9.Checked)
            {
                column9CheckedCount++;
            }

            //Column10
            int column10CheckedCount = 0;
            if (chkConfigurationSetupR1C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR2C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR3C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR4C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR5C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR6C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR7C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR8C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR9C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR11C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR12C10.Checked)
            {
                column10CheckedCount++;
            }

            //Column11
            int column11CheckedCount = 0;
            if (chkConfigurationSetupR1C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR2C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR3C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR4C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR5C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR6C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR7C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR8C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR9C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR10C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR12C11.Checked)
            {
                column11CheckedCount++;
            }

            //Column12
            int column12CheckedCount = 0;
            if (chkConfigurationSetupR1C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR2C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR3C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR4C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR5C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR6C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR7C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR8C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR9C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR10C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR11C12.Checked)
            {
                column12CheckedCount++;
            }

            if (column1CheckedCount > 2 || column2CheckedCount > 2 || column3CheckedCount > 2 || column4CheckedCount > 2 || column5CheckedCount > 2
                || column6CheckedCount > 2 || column7CheckedCount > 2 || column8CheckedCount > 2 || column9CheckedCount > 2 || column10CheckedCount > 2
                || column11CheckedCount > 2 || column12CheckedCount > 2)
            {
                ValidShortCircuitConfiguration = false;
                IsRow = false;

                if (column1CheckedCount > 2)
                {
                    RowColumnNumber = 1;
                }
                else if (column2CheckedCount > 2)
                {
                    RowColumnNumber = 2;
                }
                else if (column3CheckedCount > 2)
                {
                    RowColumnNumber = 3;
                }
                else if (column4CheckedCount > 2)
                {
                    RowColumnNumber = 4;
                }
                else if (column5CheckedCount > 2)
                {
                    RowColumnNumber = 5;
                }
                else if (column6CheckedCount > 2)
                {
                    RowColumnNumber = 6;
                }
                else if (column7CheckedCount > 2)
                {
                    RowColumnNumber = 7;
                }
                else if (column8CheckedCount > 2)
                {
                    RowColumnNumber = 8;
                }
                else if (column9CheckedCount > 2)
                {
                    RowColumnNumber = 9;
                }
                else if (column10CheckedCount > 2)
                {
                    RowColumnNumber = 10;
                }
                else if (column11CheckedCount > 2)
                {
                    RowColumnNumber = 11;
                }
                else
                {
                    RowColumnNumber = 12;
                }
            }

            return ValidShortCircuitConfiguration;
        }

        private bool ValidateShortCircuitSettings()
        {
            bool ValidShortCircuitSettings = true;
            bool IsRow = true;
            int RowColumnNumber = 0;
            string RowColumnHeading = "";

            if (IsValidShortDetectionConfiguration(out IsRow, out RowColumnNumber) == false)
            {
                switch (RowColumnNumber)
                {
                    case 1:
                        RowColumnHeading = "W+";
                        break;
                    case 2:
                        RowColumnHeading = "W-";
                        break;
                    case 3:
                        RowColumnHeading = "TA+";
                        break;
                    case 4:
                        RowColumnHeading = "TA-";
                        break;
                    case 5:
                        RowColumnHeading = "wH+";
                        break;
                    case 6:
                        RowColumnHeading = "wH-";
                        break;
                    case 7:
                        RowColumnHeading = "rH+";
                        break;
                    case 8:
                        RowColumnHeading = "rH-";
                        break;
                    case 9:
                        RowColumnHeading = "R1+";
                        break;
                    case 10:
                        RowColumnHeading = "R1-";
                        break;
                    case 11:
                        RowColumnHeading = "R2+";
                        break;
                    case 12:
                        RowColumnHeading = "R2-";
                        break;
                }

                if (IsRow)
                {
                    Notify.PopUpError("Configuration Not Saved", String.Format("Invalid short-circuit detection settings found due to row {0} with heading of '{1}' has more than one check-box checked. Configuration not saved.", RowColumnNumber, RowColumnHeading));
                }
                else
                {
                    Notify.PopUpError("Configuration Not Saved", String.Format("Invalid short-circuit detection settings found due to column {0} with heading of '{1}' has more than two check-boxes checked.", RowColumnNumber, RowColumnHeading));
                }

                ValidShortCircuitSettings = false;
            }
            return ValidShortCircuitSettings;
        }

        #endregion

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if(chkSelectAll.Checked)
            {
                chkBypassInputAndOutputEEsPickAndPlace.Checked = true;
                chkBypassMeasurementTestAtTestProbe.Checked = true;                
                chkBypassRFIDReadAtInputStation.Checked = true;
                chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked = true;
                chkBypassVisionAtInputTurnStation.Checked = true;
                chkBypassVisionAtOutputStation.Checked = true;
            }
            else
            {
                chkBypassInputAndOutputEEsPickAndPlace.Checked = false;
                chkBypassMeasurementTestAtTestProbe.Checked = false;                
                chkBypassRFIDReadAtInputStation.Checked = false;
                chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked = false;
                chkBypassVisionAtInputTurnStation.Checked = false;
                chkBypassVisionAtOutputStation.Checked = false;
            }
        }
                
        private void chkBypassRFIDReadAtInputStation_CheckedChanged(object sender, EventArgs e)
        {
            if(chkBypassRFIDReadAtInputStation.Checked == true)
            {                
                chkBypassMeasurementTestAtTestProbe.Checked = true;
                chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked = true;
            }            
        }

        private void chkBypassMeasurementTestAtTestProbe_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBypassMeasurementTestAtTestProbe.Checked == true)
            {
                chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked = true;
            }            
        }

        private void chkBypassRFIDAndSeatrackWriteAtOutputStation_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBypassRFIDAndSeatrackWriteAtOutputStation.Checked == true)
            {

            }            
        }

        private void txtConfigurationSetupCH1Current_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupCH2Current_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupCH3Current_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupCH4Current_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupCH5Current_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupCH6Current_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupAvgCurrentSampleCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupTempTimeConstant_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupFrequency_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupBiasVoltage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupPeak2PeakVoltage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtConfigurationSetupAvgVoltageSampleCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void cboConfigurationSetupProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //remark for one recipe
            //foreach (HGAProductType HGAProductType in CalibrationSettings.Instance.HgaProducts.HGAProduct)
            //{
            //    if (String.Compare(cboConfigurationSetupProductRecipe.Text, HGAProductType.Name, true) == 0)
            //    {
            //        //txtConfigurationSetupProductID.Text = HGAProductType.ProductID.ToString(); change for one recipe
            //        break;
            //    }
            //}
        }

        private void radioButtonTriggerByCarrier_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButtonTriggerByHour_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTrigByCarrier.Enabled = false;
        }

        private void buttonTriggerSave_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierEnabled = checkBoxTriggerByCarrierEnable.Checked;
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase1Min = Convert.ToDouble(touchscreenNumBoxPhase1Min.Text);
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.FailurePhase2Min = Convert.ToDouble(touchscreenNumBoxPhase2Min.Text);
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TrigerByCarrierCount = Convert.ToInt32(touchscreenNumBoxTrigByCarrierCount.Text);
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByCarrierHour = Convert.ToInt32(touchscreenNumBoxTrigHourPerCarrierCount.Text);
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TotalCarrierForBuyOff =
                Convert.ToInt32(touchscreenNumBoxTotalBuyoffCarrier.Text);

            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByErrorCodeEnabled =
                checkBoxErrorCodeTriggering.Checked;
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByErrorCodePercent =
                Convert.ToDouble(touchscreenNumBoxCRDLPercent.Text);
            HSTMachine.Workcell.HSTSettings.TriggeringSetting.TriggerByErrorCodePartPerPeriod =
                Convert.ToInt32(touchscreenNumBoxCRDLTotalRun.Text);

            HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.EnabledResistanceCheck = checkBoxResistanceCheckEnable.Checked;
            HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByHourCounter = Convert.ToDouble(touchscreenNumBoxResistanceCheckByHour.Text);
            HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckByPartCounter = Convert.ToInt32(touchscreenNumBoxResistanceCheckByPart.Text);
            if (radioButtonResistanceCheckPerHour.Checked)
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckType = ResistanceCheckConfig.ResistanceCheckType.CheckByHour;
            if (radioButtonResistanceCheckPerPartCount.Checked)
                HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.CheckType = ResistanceCheckConfig.ResistanceCheckType.CheckByPartCounter;

            HSTMachine.Workcell.HSTSettings.Save();
        }

        private void radioButtonResistanceCheckPerPartCount_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonResistanceCheckPerCarrierCount_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxTriggerByCarrierEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxTriggerByCarrierEnable.Checked)
            {
                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.ErrorCumulative = 0;
                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;
                HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = false;
                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate = DateTime.Now.ToString();
                HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checkBoxLDU.Enabled = HSTMachine.Workcell.IsFirmwareGetDone;
            if(HSTMachine.Workcell.IsFirmwareGetDone)
            {
                checkBoxLDU.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable);
                timer1.Enabled = false;
            }
        }

        private void tabControl1_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                PopulateCCCParameterSetting();
            }
            timer1.Enabled = Visible;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.Enabled = checkBoxEnableCCC.Checked;
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.EnableAlertMsg = checkBoxTicEnableAlert.Checked;
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha = Convert.ToDouble(touchscreenNumBoxAlpha.Text);
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit =
                Convert.ToInt32(touchscreenNumBoxCounterLimit.Text);
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup =
                Convert.ToInt32(touchscreenNumBoxTestRunCount.Text);
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit =
                Convert.ToDouble(touchscreenNumBoxYieldLimit.Text);
            HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget =
                Convert.ToDouble(touchscreenNumBoxYieldTarget.Text);

            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.Enabled = checkBoxEnableCCC.Checked;
            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.EnableAlertMsg = checkBoxHstEnableAlert.Checked;
            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.Alpha = Convert.ToDouble(touchscreenNumBoxAlphaCRDL.Text);
            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.DefectCounterLimit =
                Convert.ToInt32(touchscreenNumBoxCounterLimitCRDL.Text);
            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.TestRunGroup =
                Convert.ToInt32(touchscreenNumBoxTestRunCountCRDL.Text);
            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.YeildLimit =
                Convert.ToDouble(touchscreenNumBoxYieldLimitCRDL.Text);
            HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.YeildTarget =
                Convert.ToDouble(touchscreenNumBoxYieldTargetCRDL.Text);

            HSTMachine.Workcell.HSTSettings.CccParameterSetting.DelayForReleaseCarrier =
                Convert.ToDouble(touchscreenNumBoxDelayReleaseCarrier.Text);
            //Reset ANC setting
            //    ANCReset();
            _workcell.TICCccControl.CCCControlAllMc.CccResult.CountPerPeriod = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup;
            _workcell.TICCccControl.CCCControlAllMc.CccResult.Alpha = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha;
            _workcell.TICCccControl.CCCControlAllMc.CccResult.DefectLimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit;
            _workcell.TICCccControl.CCCControlAllMc.CccResult.YeildLimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit;
            _workcell.TICCccControl.CCCControlAllMc.CccResult.YeildTarget = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget;

            _workcell.TICCccControl.CCCControlTicMc1.CccResult.CountPerPeriod = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup;
            _workcell.TICCccControl.CCCControlTicMc1.CccResult.Alpha = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha;
            _workcell.TICCccControl.CCCControlTicMc1.CccResult.DefectLimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit;
            _workcell.TICCccControl.CCCControlTicMc1.CccResult.YeildLimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit;
            _workcell.TICCccControl.CCCControlTicMc1.CccResult.YeildTarget = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget;

            _workcell.TICCccControl.CCCControlTicMc2.CccResult.CountPerPeriod = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup;
            _workcell.TICCccControl.CCCControlTicMc2.CccResult.Alpha = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha;
            _workcell.TICCccControl.CCCControlTicMc2.CccResult.DefectLimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit;
            _workcell.TICCccControl.CCCControlTicMc2.CccResult.YeildLimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit;
            _workcell.TICCccControl.CCCControlTicMc2.CccResult.YeildTarget = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget;

            _workcell.TICCccControl.CCCControlTicMc1.TICMachineName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
            _workcell.TICCccControl.CCCControlTicMc2.TICMachineName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;
            
            
            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().labelANCEnableStatus.Visible = HSTSettings.Instance.CccParameterSetting.Enabled;

            if (txtTICMachine1.Text != HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1)
            {

                if (HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1 == "")
                {
                   
                }
                else
                {
                    Log.Info("UTICMachineName1 Change : from {0} to {1}", HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1, txtTICMachine1.Text);
                    ANCResetUTICMachine1();

                }
                HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1 = txtTICMachine1.Text;
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.uTICMachineName1 = txtTICMachine1.Text;
            }
            else if (txtTICMachine2.Text != HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2)
            {
                if (HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2 == "")
                {

                }
                else
                {
                    Log.Info("UTICMachineName2 Change : from {0} to {1}", HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2, txtTICMachine2.Text);
                    ANCResetUTICMachine2();
                }
                HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2 = txtTICMachine2.Text;
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.uTICMachineName2 = txtTICMachine2.Text;
            }
            else if ((txtTICMachine1.Text == txtTICMachine2.Text) && ((txtTICMachine1.Text != "") || (txtTICMachine2.Text != "")))
            {
                MessageBox.Show("UTICName between Machine1 & 2 are same, UTIC Machine2 name will remove");
                Log.Info("UTICMachineName has same name between station :  {0} to {1}", txtTICMachine1.Text, txtTICMachine2.Text);
                txtTICMachine2.Text = "";
                HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2 = txtTICMachine2.Text;
                HSTMachine.Workcell.HSTSettings.CccParameterSetting.uTICMachineName2 = txtTICMachine2.Text;

                ANCReset();
                
            }
            HSTMachine.Workcell.HSTSettings.Save();
            HSTMachine.Workcell.getPanelData().ClearChart();
            //ClearChart();
            HSTMachine.Workcell.HSTSettings.saveCCC();
        }

        private void checkBoxEnableCCC_CheckedChanged(object sender, EventArgs e)
        {
         //   if (checkBoxEnableCCC.Checked != HSTMachine.Workcell.HSTSettings.CccParameterSetting.Enabled)
         //   {
         //       ANCReset();
                //_workcell.HSTSettings.TicCCCCounter.CCCCounterAllMc.Reset();
                //_workcell.HSTSettings.TicCCCCounter.CCCCounterTicMc1.Reset();
                //_workcell.HSTSettings.TicCCCCounter.CCCCounterTicMc2.Reset();
                //_workcell.TICCccControl = new Data.CumulativeCountofConforming.HSTCCCControl(_workcell.HSTSettings.CccParameterSetting, _workcell.HSTSettings.TicCCCCounter, true, false);

                //_workcell.HSTSettings.CRDLCccCounter.CCCCounterAllMc.Reset();
                //_workcell.CRDLCccControl = new Data.CumulativeCountofConforming.HSTCCCControl(_workcell.HSTSettings.CccCRDLParameterSetting, _workcell.HSTSettings.CRDLCccCounter, false, true);

         //   }
         //   groupBoxSettingForTIC.Enabled = checkBoxEnableCCC.Checked;

        }

        public void ANCReset()
        {
            HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertActiveTime = DateTime.Now;
            HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertClearedTime = DateTime.Now;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.TICMachineName = txtTICMachine1.Text;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.TICMachineName = txtTICMachine2.Text;
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.MCDownTriggering = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.AdaptivePartRunCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.Reset();
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.Reset();
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.Reset();

            HSTMachine.Workcell.TICCccControl.CCCControlAllMc.current_yield = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.current_yield = 0;

            HSTMachine.Workcell.HSTSettings.saveCCC();
        }


        public void ANCResetUTICMachine1()
        {
            HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertActiveTime = DateTime.Now;
            HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertClearedTime = DateTime.Now;

            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.TICMachineName = txtTICMachine1.Text;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.Reset();
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield = 0;
            HSTMachine.Workcell.HSTSettings.saveCCC();
        }
        public void ANCResetUTICMachine2()
        {
            HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertActiveTime = DateTime.Now;
            HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertClearedTime = DateTime.Now;

            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.TICMachineName = txtTICMachine2.Text;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicDefactCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.Reset();
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield = 0;
            HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.current_yield = 0;
            HSTMachine.Workcell.HSTSettings.saveCCC();

        }
        /// <summary>
        /// Get machine number in SRV file form Getputserver path, 
        /// will not allow user to change in GUI
        /// </summary>
        /// <returns></returns>
        public string ReadMachineNumberFromSRVFile()
        {
            string returnMCName = string.Empty;
            string loadfile = @"C:\FisGetputServer\";
            var l = System.Diagnostics.Process.GetProcessesByName("FisGetPutServer").Length;
            if (System.Diagnostics.Process.GetProcessesByName("FisGetPutServer").Length > 0)
            {
                var getpc = System.Diagnostics.Process.GetProcessesByName("FisGetPutServer");
                string filepath = getpc.FirstOrDefault().MainModule.FileName;
                var filename = getpc.FirstOrDefault().MainModule.ModuleName;
                var loadpath = filepath.Substring(0,filepath.Length - filename.Length);
                if (!loadpath.ToLower().Contains("fisgetputserver")) loadpath = loadfile;
                DirectoryInfo fileInfo = new DirectoryInfo(loadpath);
                FileInfo fileItem = null;
                    try
                    {
                        List<FileInfo> loadedfile = new List<FileInfo>();
                        bool isend = false;
                        loadedfile = fileInfo.GetFiles().Take(fileInfo.GetFiles().ToList().Count).ToList();

                        foreach (var item in loadedfile)
                        {
                            if(item.FullName.ToUpper().Contains("SRV"))
                            {
                                var getfile = System.IO.Path.Combine(@"" + item.FullName);

                                System.IO.StreamReader readfile = new System.IO.StreamReader(getfile);
                                string txtread = string.Empty;
                                while ((txtread = readfile.ReadLine()) != null)
                                {
                                    if(txtread.ToLower().Contains("_gps"))
                                    {
                                        string[] split = txtread.Split('=');
                                        string[] splitname = split[1].Split('_');
                                        returnMCName = splitname[0];
                                        isend = true;
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }

                            if (isend) break;
                            Thread.Sleep(1);
                        }
                    }
                    catch (System.IO.DirectoryNotFoundException ex)
                    {
                        returnMCName = HSTMachine.Workcell.HSTSettings.Install.EquipmentID;
                    }
            }

            if (returnMCName == string.Empty) returnMCName = HSTMachine.Workcell.HSTSettings.Install.EquipmentID;
            return returnMCName;
        }

        /// <summary>
        /// Get machine type in SRV file form Getputserver path, 
        /// will not allow user to change in GUI
        /// </summary>
        /// <returns></returns>
        public string ReadMachineTypeFromSRVFile()
        {
            string returnstr = string.Empty;
            var mcnum = HSTMachine.Workcell.getPanelSetup().ReadMachineNumberFromSRVFile();
            if(mcnum != string.Empty)
                returnstr = mcnum.Substring(0, 3);
            if (returnstr.Length < 3) returnstr = HSTMachine.Workcell.HSTSettings.EquipmentType;
            return returnstr;
        }

        private void touchscreenNumBoxPartCounterYieldShortPeriod_TextChanged(object sender, EventArgs e)
        {
        }

        private void touchscreenNumBoxPartCounterYieldLongPeriod_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBoxEnableCCC_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void btnResetANC_Click(object sender, EventArgs e)
        {
            if (_workcell.IsRecipeLoadedDone)
                ANCReset();
            else
                Notify.PopUp("Warning", "Please load product recipe before setup", "", "OK");
        }
    }
}
