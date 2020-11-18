using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using qf4net;
using System.Threading;
using Seagate.AAS.Parsel;
using Seagate.AAS.Parsel.Equipment;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class PanelCommand : UserControl
    {
        HSTWorkcell _workcell;
        private FormMainE95 _mainForm;
        private int stopClicks = 0;
        private bool _stopInProgress = false;
        private System.Windows.Forms.Timer _tmr = new System.Windows.Forms.Timer();
        private OperationMode lastRunModeStatus = OperationMode.Auto;
        private bool _isDefaultRecipeLoaded = false;
  
        // button click delegates
        public delegate void OnStopButtonClickedHandler();
        public event OnStopButtonClickedHandler OnStopButtonClicked;

        public delegate void OnStartButtonClickedHandler();
        public event OnStartButtonClickedHandler OnStartButtonClicked;

        // UI stuff for OEE buttons and transitions
        public const int TransitionTime = 300;
        private int minPanelWidth = 300;
        private int defaultWidth = 0;
        private int oeeWidth = 0;
        private int oeePadding = 4;     // extra width (per side) between panel edge and OEE buttons
        private Size oeeButtonSize = new Size(176, 30);
        private Point oeeButtonLocation = new Point(136, 8);
        private Font oeeFont = new Font("Tahoma", 8.25F, FontStyle.Bold);

        private List<Control> oeeControlList = new List<Control>();
        private UserAccessSettings _userAccess;
        private uint _timeUsed = 0;

        private Thread PurgeCarrierThread;
        private IDigitalOutput _doSoftStartUp;
        private IDigitalOutput _doDCServicingLight;

        private BackgroundWorker _bgworker = new BackgroundWorker();

        public PanelCommand(HSTWorkcell workcell)
        {
            InitializeComponent();

            _mainForm = HSTMachine.Instance.MainForm;

            // panel contains all the controls.  panel's location is manipulated during UI element transitions
            // instead of moving individual OEE Buttons or the command button toolstrip
            // first thing to do is make sure the panel lines up with the UserControl (this).  This hardcodes
            // important params instead of relying on the VS designer layout
            panel.Location = new Point(0, 0);
            panel.Height = this.Height;
            panel.Width = minPanelWidth;

            // width for toolstrip
            defaultWidth = this.Width;          
            oeeWidth = oeeButtonSize.Width + 2 * oeePadding;

            // X location of oeeButtons on panel (center on panel when widened for oee button display)
            oeeButtonLocation.X = defaultWidth + (oeeWidth - oeeButtonSize.Width) / 2;
            _tmr.Enabled = true;
            _tmr.Interval = 200;

            _workcell = workcell;
            _tmr.Tick += new EventHandler(_tmr_Tick);            
            _workcell.Process.OnProcessStarted += new Parsel.Equipment.Process.ProcessStateChangeHandle(Process_OnProcessStarted);
            _workcell.Process.OnProcessStoped += new Parsel.Equipment.Process.ProcessStateChangeHandle(Process_OnProcessStoped);
            
            _workcell.Process.OnInit += new EventHandler(Process_OnInit);
            _workcell.Process.OnInitStart += new EventHandler(Process_OnInitStart);
            _workcell.Aborted += new EventHandler(_workcell_Aborted);
            _workcell.Process.MonitorProcess.TowerLightAndDoorStatusEvent += new EventHandler(DisplayTowerLightAndDoorLockStatus);
            _workcell.LoadDefaultRecipeEventHandler += LoadDefaultRecipe;
            DebugButtonsVisibility();

            PurgeCarrierThread = new Thread(startpurgingcarrier);
            _doSoftStartUp = _workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Soft_Start_Up);
            _doDCServicingLight = _workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.DC_Servicing_Light);
        }

        public void DebugButtonsVisibility()
        {
            try
            {
                toolTip1.SetToolTip(this.btnRunTestScript, HSTMachine.Workcell.HSTSettings.Install.TestScript);

                if (HSTMachine.Workcell.HSTSettings.getUserAccessSettings().getCurrentUser().Level == UserLevel.Engineer)
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton)
                    {
                        buttonRun.Enabled = false;
                        buttonPurge.Enabled = false;
                        btnRunTestScript.Visible = true;
                    }
                    else
                    {
                        buttonRun.Enabled = true;
                        buttonPurge.Enabled = true;
                        btnRunTestScript.Visible = false;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        // Properties ----------------------------------------------------------------------------
        public int ControlButtonWidth { get { return defaultWidth; } }
        public int OEEButtonWidth { get { return oeeWidth; } }

        // Event handlers ------------------------------------------------------

        void _tmr_Tick(object sender, EventArgs e)
        {
            if (this.Visible == false)
                return;
            
            if (lastRunModeStatus != HSTMachine.Workcell.HSTSettings.Install.OperationMode)
            {
                if (_mainForm != null)
                {
                    if (_mainForm.getPanelTitle() != null)
                    {
                        // Disable simulation if vision is enabled
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableVision == true)
                        {
                            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                            {
                                HSTMachine.Workcell.HSTSettings.Install.OperationMode = OperationMode.Auto;
                            }
                        }

                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Auto)
                        {
                            _mainForm.getPanelTitle().RunModeCaption = "Auto";
                        }                        
                        else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                        {
                            _mainForm.getPanelTitle().RunModeCaption = (HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat) ? "Dry Run (No Boats)" : "Dry Run (With Boats)";
                        }
                        else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                        {
                            _mainForm.getPanelTitle().RunModeCaption = "Simulation";
                        }
                        else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass)
                        {
                            _mainForm.getPanelTitle().RunModeCaption = "Bypass";
                        }
                    }
                }
                lastRunModeStatus = HSTMachine.Workcell.HSTSettings.Install.OperationMode;
            }
           

            if (HSTMachine.Workcell.Process.IsIdleState) // Stopped
            {
                _mainForm.getPanelNavigation().RunMode = false;
            }

            if (HSTMachine.Workcell.IsMachineHomed == true)  // Initialized
            {
                if (HSTMachine.Workcell.Process.IsIdleState) // Stopped
                {                    
                    _mainForm.getPanelNavigation().RunMode = false;
                }

                if (HSTMachine.Workcell.Process.IsRunState) // Running
                {                    
                    _mainForm.getPanelNavigation().RunMode = false; //temp
                }
            }
        }

        void Process_OnInitStart(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                _stopInProgress = false;                                
                buttonRun.Enabled = false;
                buttonPause.Enabled = true;
                buttonStop.Enabled = true;
                buttonStop.BackColor = SystemColors.Control;
                buttonPurge.Enabled = false;

                buttonRun.BackColor = Color.LightGreen;                
            });
        }

        void Process_OnManualInit(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                buttonRun.Enabled = false;
                buttonPause.Enabled = false;
                buttonStop.Enabled = true;
                buttonStop.BackColor = SystemColors.Control;
                buttonPurge.Enabled = false;
                buttonSystemInitial.Enabled = false;
                buttonRun.BackColor =SystemColors.Control;
                buttonSystemInitial.BackColor = Color.LightGreen;
                btnSaveConfigToProcessor.Enabled = false;

                toolStrip1.Refresh();

                HSTMachine.Instance.MainForm.getAnimatedProgressBar().Visible = true;
                _doSoftStartUp.Set(DigitalIOState.On);

                _workcell.Process.ResetSystemInit();
                HSTMachine.Workcell.Process.Stop();
                Thread.Sleep(1000);
                _workcell.Process.TestProbeProcess.Controller.InitialGentry();

                Thread.Sleep(1000);
                if (_workcell.Process.InputStationProcess.Controller.IsInputStationClampForward())
                    _workcell.Process.InputStationProcess.Controller.InputStationForwardClamp(false);
                if (_workcell.Process.OutputStationProcess.Controller.IsOutputStationClampForward())
                    _workcell.Process.OutputStationProcess.Controller.OutputStationForwardClamp(false);

                _workcell.Process.InputStationProcess.Controller.RaiseInputLifter(false);
                _workcell.Process.InputStationProcess.Controller.RaiseInputStationStopper(true);
                _workcell.Process.OutputStationProcess.Controller.RaiseOutputLifter(false);
                _workcell.Process.OutputStationProcess.Controller.RaiseOutputStationStopper(true);

                HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputConveyor(true);
                HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputTurnStation(true);

                HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo0Deg();
                Thread.Sleep(1000);

                HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InputTurnSectionTurnTo0Deg();
                Thread.Sleep(1000);

                buttonRun.Enabled = true;
                btnSaveConfigToProcessor.Enabled = true;
                buttonPause.Enabled = false;
                buttonStop.Enabled = false;
                buttonStop.BackColor = SystemColors.Control;
                buttonPurge.Enabled = true;
                buttonSystemInitial.Enabled = true;
                buttonRun.BackColor = Color.LightGreen;
                buttonSystemInitial.BackColor = SystemColors.Control;
                _workcell.DisplayTitleMessage("System Idle");
                HSTMachine.Instance.MainForm.getAnimatedProgressBar().Visible = false;
                toolStrip1.Enabled = true;
                toolStrip1.Refresh();

            });


        }

        void Process_OnInit(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {                
                buttonPause.Enabled = false;                
            });
        }

        public void Process_OnProcessStoped()
        {
            this.BeginInvoke((MethodInvoker)delegate
            {                
                buttonRun.Enabled = true;
                buttonRun.BackColor = SystemColors.Control;                
                buttonPause.Enabled = false;
                buttonStop.Enabled = false;
                buttonPurge.Enabled = true;
                btnSaveConfigToProcessor.Enabled = true;

                HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().btnLogin.Enabled = true;
                HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().btnLogout.Enabled = true;
                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = true;
                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = true;
                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = true;

                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().Enabled = true;
                HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel().Enabled = true;

                if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Operator ||
                            HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Monitor)
                {
                    HSTMachine.Instance.MainForm.getPanelNavigation().btnSetup.Enabled = false;
                    HSTMachine.Instance.MainForm.getPanelNavigation().btnRecipe.Enabled = false;
                    HSTMachine.Instance.MainForm.getPanelNavigation().btnDiagnostic.Enabled = false;
                }
                else
                {
                    HSTMachine.Instance.MainForm.getPanelNavigation().btnSetup.Enabled = true;
                    HSTMachine.Instance.MainForm.getPanelNavigation().btnRecipe.Enabled = true;
                    HSTMachine.Instance.MainForm.getPanelNavigation().btnDiagnostic.Enabled = true;
                }
            });
        }

        public void Process_OnProcessStarted()
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                buttonRun.Enabled = false;
                buttonPause.Enabled = true;
                buttonPurge.Enabled = false;
                btnSaveConfigToProcessor.Enabled = false;
                if (!_stopInProgress && HSTMachine.Workcell.IsMachineHomed)
                {
                    buttonRun.BackColor = Color.LightGreen;
                    buttonStop.Enabled = true;
                }
            });
        }
        
        private void On_MachineStopped()
        {
            this.Invoke((MethodInvoker)delegate
            {
                _stopInProgress = false;
                buttonStop.BackColor = SystemColors.Control;                
            });
        }

        void _workcell_Aborted(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                buttonRun.Enabled = true;
                btnSaveConfigToProcessor.Enabled = true;
                buttonRun.BackColor = SystemColors.Control;
                buttonPause.Enabled = false;
                buttonStop.Enabled = false;
                buttonStop.BackColor = SystemColors.Control;
                buttonPurge.Enabled = true;
            });
        }

        private void DisplayTowerLightAndDoorLockStatus(object sender, EventArgs e)
        {
            TowerLightAndDoorLockStatusEventArgs towerLightAndDoorLockStatusEventArgs = e as TowerLightAndDoorLockStatusEventArgs;
            bool amberLightOn = towerLightAndDoorLockStatusEventArgs.AmberLightOn;
            bool redLightOn = towerLightAndDoorLockStatusEventArgs.RedLightOn;
            bool doorLock = towerLightAndDoorLockStatusEventArgs.DoorLock;

            if (HSTWorkcell.terminatingHSTApps)
            {
                return;
            }

            try
            {
                UIUtility.Invoke(this, () =>
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                    {
                        lblAmberLight.BackColor = Color.Yellow;
                        lblRedLight.BackColor = Color.DarkRed;
                        lblDoorStatus.Text = "Door N/A";
                        lblDoorStatus.BackColor = Color.Gray;
                    }
                    else
                    {
                        lblAmberLight.BackColor = amberLightOn ? Color.Yellow : Color.Gray;
                        lblRedLight.BackColor = redLightOn ? Color.Red : Color.Gray;
                        lblDoorStatus.Text = doorLock ? "Door Locked" : "Door Opened";
                        lblDoorStatus.BackColor = doorLock ? Color.Green : Color.Red;
                    }
                });
            }
            catch(ObjectDisposedException ex)
            {
                // do nothing
            }
        }

        // Methods -------------------------------------------------------------------------------
        public void AssignWorkcell(HSTWorkcell workcell)
        {
            _workcell = workcell;
            _userAccess = HSTMachine.Workcell.HSTSettings.getUserAccessSettings();
            _userAccess.UserChanged += new EventHandler(UserChanged);            
            _workcell.GroundMasterStatusChanged += new EventHandler(GroundMasterStatusChanged);
            _workcell.MachinePerformanceStatusChanged += new EventHandler(MachinePerformanceStatusChanged);
            _workcell.FunctionalTestStatusChanged += new EventHandler(ProbeFunctionalTestStatusChanged);
            _workcell.SamplingOverTargetTriggeringStatusChaned += new EventHandler(SamplingOverTargetTriggeringStatusChanged);
            // initialize panel, show corrective action at start
            // this should be if(auto mode), if not auto mode, show the command buttons because not reporting oee.
            this.Top = 0;
            bool automode = true;
            if (automode)
            {
                BringToFront();                
                this.Width = defaultWidth;          // adjust parent width                
            }
            else
            {
                this.Left = 0;
            }

            EnableDisableButton();    
        }

        public void ShowPurgeButton()
        {
            buttonPurge.Visible = true;
            toolStripSeparator1.Visible = true;
        }

        public void HidePurgeButton()
        {
            buttonPurge.Visible = false;
            toolStripSeparator1.Visible = false;
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if(HSTMachine.Workcell.HSTSettings.CccParameterSetting.Enabled)
            {
                if ((HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1 == "") || (HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1 == ""))
                {
                    Notify.PopUp("ANC function notification", "UTICMachine not input in configuration", "", "OK");
                    return;
                }
            }
            if (!_workcell.IsRecipeLoadedDone)
            {
                Notify.PopUp("Product workorder has not ", "Please select workorder file run before running the HST machine.", "", "OK");
                return;
            }

            if (CommonFunctions.Instance.CheckCompareConfigFileBetweenGlobalAndLocal(CommonFunctions.Instance.GetMachineConfigGlobalInfo))
            {
                Notify.PopUp("Machine config file has been updated ", "Please select workorder file to load machine config again.", "", "OK");
                return;
            }

            if (CommonFunctions.Instance.CheckCompareRecipeFileBetweenGlobalAndLocal(CommonFunctions.Instance.ProductRecipeName + CommonFunctions.RecipeExt))
            {
                Notify.PopUp("Machine recipe file has been updated ", "Please select workorder file to load machine recipe again.", "", "OK");
                return;
            }

            HSTMachine.Workcell.Process.InputEEProcess.Controller.InitializeController();
            HSTMachine.Workcell.Process.OutputEEProcess.Controller.InitializeController();
            try
            {
                
                if (buttonRun.Text == "Resume")
                {
                    buttonRun.Text = "Run";
                    buttonRun.Enabled = false;
                    buttonStop.Enabled = true;
                    buttonPause.Enabled = true;
                    HSTMachine.Workcell.Process.Resume();
                   
                    return;
                }

                _workcell.OffAlarm = false;
                #region Clear all alert
                if (HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated)
                {
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = true;
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffCarrierCounter = 0;
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffPartLoadedCounter = 0;
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyOffPartFailStatusCounter = 0;
                    Thread.Sleep(1000);
                }

                if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated)
                {
                    HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.IsRetestProcessRequired = true;
                    HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated = false;
                    HSTMachine.Workcell.HSTSettings.Save();
                    _workcell.SendFunctionalTestStatusMessage();
                    Thread.Sleep(1000);
                }

                if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.SamplingTriggeringOverPercentageActivate)
                {
                    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.SamplingTriggeringOverPercentageActivate = false;
                    HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter = 0;
                }

                HSTSettings.Instance.TriggeringSetting.TriggerByErrorCodeFailureCounter = 0;
                if (HSTMachine.Workcell.HSTSettings.TriggeringSetting.ErrorCodeHeightWriterFailureActivate)
                {
                    HSTMachine.Workcell.HSTSettings.TriggeringSetting.ErrorCodeHeightWriterFailureActivate = false;
                    HSTMachine.Workcell.LoadCounter.ResetWriterBridgCounter();
                }

                CommonFunctions.Instance.ActivePopupGetputErrorMessage = false;

                _workcell.Process.MonitorProcess.ClearAllForAutoStartRun();

                #endregion

                HSTMachine.Workcell.Process.MonitorProcess.Controller.MachineDoorLock(true);

                if (HSTMachine.Workcell.Process.MonitorProcess.HasError && !HSTMachine.Workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated &&
                    !HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated)
                    throw new Exception("Please clear monitor error first");

                if (CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage)
                {
                    CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage = false;
                    if (HSTMachine.Workcell.Process.InputStationProcess.Controller.IsBufferStationHoldCarrier() ||
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.IsOutputStationHoldCarrier() ||
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                    {
                        throw new Exception("Carrier found in one or more stations. Please clear then using the Purge button.");
                    }
                }
                else
                {
                    if (HSTMachine.Workcell.Process.InputStationProcess.Controller.IsInputStationHoldCarrier() || 
                        HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.IsInputTurnStationHoldCarrier() ||
                        HSTMachine.Workcell.Process.InputStationProcess.Controller.IsBufferStationHoldCarrier() ||
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.IsOutputStationHoldCarrier() ||
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                    {
                        throw new Exception("Carrier found in one or more stations. Please clear then using the Purge button.");
                    }
                }


                if (HSTMachine.Workcell.CCCMachineTriggeringDown)
                {
                    HSTMachine.Workcell.getPanelData().UpdateClearMachineTrigger();
                }

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    _doDCServicingLight.Set(DigitalIOState.Off);
                    _doSoftStartUp.Set(DigitalIOState.On);
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "Turning on Soft Start Up valve as the system is in Run state now.");
                    }

                    // Re-enable the X, Y and Theta axes of the Precisor Nest
                    if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel() != null)
                    {
                        if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel().getPrecisorNestXAxis() != null)
                        {
                            HSTMachine.Workcell._a3200HC.Enable(HSTMachine.Workcell._ioManifest.GetAxis((int)HSTIOManifest.Axes.X), true);
                        }

                        if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel().getPrecisorNestYAxis() != null)
                        {
                            HSTMachine.Workcell._a3200HC.Enable(HSTMachine.Workcell._ioManifest.GetAxis((int)HSTIOManifest.Axes.Y), true);
                        }

                        if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel().getPrecisorNestThetaAxis() != null)
                        {
                            HSTMachine.Workcell._a3200HC.Enable(HSTMachine.Workcell._ioManifest.GetAxis((int)HSTIOManifest.Axes.Theta), true);
                        }
                    }

                }

                Log.Info(this, "Running the system with OperationMode set to : '{0}'.", HSTMachine.Workcell.HSTSettings.Install.OperationMode);

                HSTWorkcell.stopSystemDueToAxisError = false;
                HSTMachine.Instance.MainForm.getPanelNavigation().SetPanel("Operation");
                UserControl activePanel = Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.GetActivePanel();
                TabControl tabControl = activePanel.Controls[0] as TabControl;
                tabControl.SelectedIndex = 0;

                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = false;
                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = false;
                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = false;
                
                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().Enabled = false;
                HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel().Enabled = false;

                HSTMachine.Instance.MainForm.getPanelNavigation().btnSetup.Enabled = false;
                HSTMachine.Instance.MainForm.getPanelNavigation().btnRecipe.Enabled = false;
                HSTMachine.Instance.MainForm.getPanelNavigation().btnDiagnostic.Enabled = false;

                FISManager.Instance.Launch();
                FISManager.Instance.StartGetPutServer();

                HSTMachine.Workcell.Process.InitializeMachine(true);
                HSTMachine.Workcell.Process.MonitorIOState.StartLogIOState();

                _workcell.OffAlertMessage = false;
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Can not Run", ex);                
            }            
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if ("No" == XyratexOSC.UI.Notify.PopUp("Stop Mode", "Are you sure you want to run Stop Mode?", "", "Yes", "No"))
            {
                return;
            }

            System.IO.File.Delete(CommonFunctions.TempFileToIndicateDownloadConfigToProcessorClicked);

            HSTMachine.Workcell.DisplayTitleMessage("System Stopping");
            HSTMachine.Workcell.Process.MonitorIOState.StopLogIOState();
            XyratexOSC.Logging.Log.Info(this, "System process has been stopped by user.");

            stopSystemProcess();         
        }

        public void stopSystemProcess()
        {
            CommonFunctions.Instance.SystemInitializationCompleted = false;
            UIUtility.Invoke(this, () =>
            {
                HSTMachine.Workcell.HSTSettings.State = JobStates.OutJob;
                XyratexOSC.Logging.Log.Info(this, "Stop System Process.");
                _stopInProgress = true;
                buttonStop.Enabled = false;
                buttonPause.Enabled = false;
                buttonStop.BackColor = Color.Yellow;
                bool processesHasError = false;

                foreach (Active item in HSTMachine.Workcell.Process.ActiveProcess.Values)
                {
                    if (((ActiveProcessHST)item).IsErrorState)
                        processesHasError = true;
                }

                if (HSTMachine.Workcell.Process.IsErrorState || processesHasError || HSTWorkcell.stopSystemDueToAxisError || CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage)
                {
                    HSTMachine.Workcell.Process.Stop();
                }
                else
                {
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));
                }
                _workcell.OffAlertMessage = true;

            });   

        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            ((ToolStripButton)sender).Enabled = false;
            buttonRun.Enabled = true;
            buttonRun.Text = "Resume";
            buttonStop.Enabled = false;

            HSTMachine.Workcell.Process.PauseMachine();            
        }

        private void buttonPurge_Click(object sender, EventArgs e)
        {            
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            _workcell.DisplayTitleMessage("Purge In Progress");
            _doSoftStartUp.Set(DigitalIOState.On);

            buttonRun.Enabled = false;            
            buttonPurge.Enabled = false;
            HSTMachine.Instance.MainForm.getAnimatedProgressBar().Visible = true;
            HSTMachine.Workcell.IsInPurgingProcess = true;

            _bgworker.DoWork += _bgworker_DoWork;
            _bgworker.RunWorkerAsync();
        }

        void _bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (HSTMachine.Workcell.TestProbeZAxisPosition != TestProbeZAxis.Parked)
                _workcell.Process.TestProbeProcess.Controller.HomeZAxis();

            if (HSTMachine.Workcell.InputEEZAxisPosition != InputEEZAxis.Parked)
                _workcell.Process.InputEEProcess.Controller.DoJobMoveZToPark(false);

            if (HSTMachine.Workcell.OutputEEZAxisPosition != OutputEEZAxis.Parked)
                _workcell.Process.OutputEEProcess.Controller.HomeZAxis();

            if (_workcell.Process.InputStationProcess.Controller.IsInputStationClampForward())
            {
                if (_workcell.Process.InputStationProcess.Controller.IsInputStationClampRotateCwOpen())
                    _workcell.Process.InputStationProcess.Controller.InputStationClampRotaryOpenCover(false);
                _workcell.Process.InputStationProcess.Controller.InputStationForwardClamp(false);
            }

            if (_workcell.Process.OutputStationProcess.Controller.IsOutputStationClampForward())
            {
                if (_workcell.Process.OutputStationProcess.Controller.IsOutputStationClampRotateCwOpen())
                    _workcell.Process.OutputStationProcess.Controller.OutputStationClampRotaryOpenCover(false);
                _workcell.Process.OutputStationProcess.Controller.OutputStationForwardClamp(false);
            }

            _workcell.Process.InputStationProcess.Controller.RaiseInputLifter(false);
            _workcell.Process.InputStationProcess.Controller.RaiseInputStationStopper(false);
            _workcell.Process.OutputStationProcess.Controller.RaiseOutputLifter(false);
            _workcell.Process.OutputStationProcess.Controller.RaiseOutputStationStopper(false);
            Thread.Sleep(300);            

            HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputConveyor(true);
            HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputTurnStation(true);

            for (int i = 0; i < 3; i++)
            {
                // Clear boats at OutputTurnStation
                if (HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                {
                    HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(true);
                    HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo90Deg();
                    Thread.Sleep(1000);
                    HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(false);
                    Thread.Sleep(2000);
                }
                else
                {
                    HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo0Deg();
                    Thread.Sleep(1000);
                    HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(true);
                    HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(false);
                    Thread.Sleep(2000);
                }

                // Move boat from BufferStation to OutputStation                
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitBufferStation(false);
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(true);
                Thread.Sleep(300);

                // Move boat from InputStation to BufferStation                
                HSTMachine.Workcell.Process.InputStationProcess.Controller.InhibitInputStation(false);
                Thread.Sleep(300);

                // Clear boats at InputTurnStation                
                if (HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.IsInputTurnStationHoldCarrier())
                {
                    HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InputTurnSectionTurnTo90Deg();
                    Thread.Sleep(1000);
                    HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputTurnStation(false);
                    Thread.Sleep(2000);
                }
                else
                {
                    HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InputTurnSectionTurnTo0Deg();
                    Thread.Sleep(1000);
                    HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputTurnStation(true);
                    Thread.Sleep(2000);
                }


                int TimeOutLimit = 0;
                // Clear boat at OutputTurnStation
                while (HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.NextZoneReady())
                {
                    if (HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                    {
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(true);
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo90Deg();
                        Thread.Sleep(1000);
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(false);
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo0Deg();
                        Thread.Sleep(1000);
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(true);
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(false);
                        Thread.Sleep(2000);
                    }

                    if (!HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.IsInputTurnStationHoldCarrier() &&
                        !HSTMachine.Workcell.Process.InputStationProcess.Controller.IsInputStationHoldCarrier() &&
                        !HSTMachine.Workcell.Process.InputStationProcess.Controller.IsBufferStationHoldCarrier() &&
                        !HSTMachine.Workcell.Process.OutputStationProcess.Controller.IsOutputStationHoldCarrier() &&
                        !HSTMachine.Workcell.Process.OutputStationProcess.Controller.IsBufferStationHoldCarrier() &&
                        !HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                    {
                        break;
                    }
                    Thread.Sleep(200);
                    TimeOutLimit++;

                    if (TimeOutLimit > 2)
                    {
                        break;
                    }
                }
            }

            HSTMachine.Workcell.IsInPurgingProcess = false;
            lock (CommonFunctions.Instance.InputCarriersLock)
            {
                CommonFunctions.Instance.InputCarriers.Clear();
            }
        }


        private void UpdateButtonDisplay()
        {
            if (buttonRun.Checked)
                buttonRun.Image = Properties.Resources.playChecked;
            else
                buttonRun.Image = Properties.Resources.play_64x64;

            if (buttonPause.Checked)
                buttonPause.Image = Properties.Resources.pauseChecked;
            else
                buttonPause.Image = Properties.Resources.pause_64x64;

            this.Invalidate();
        }        

        private void UserChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {
                    EnableDisableButton();
                });
            }
            catch { }
        }

        private void GroundMasterStatusChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {
                    if (_workcell.GroundMonitoringErrActivated)
                    {
                        if(!_workcell.GroundMonitoringStatus)
                        {
                            if (_workcell.GroundMonitoringErrActivated)
                            {
                                HSTMachine.Workcell.DisplayTitleMessage("System Stopping");
                                HSTMachine.Workcell.Process.MonitorIOState.StopLogIOState();
                                XyratexOSC.Logging.Log.Info(this, "System process has been stopped by grounding failed triggering.");

                                stopSystemProcess();         
                            }
                        }
                        else
                        {
                            buttonRun.Enabled = true;
                            _workcell.GroundMonitoringErrActivated = false;
                        }
                    }
                });
            }
            catch { }
        }

        private void MachinePerformanceStatusChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {

                    if (_workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated && !HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted)
                    {
                        HSTMachine.Workcell.DisplayTitleMessage("System Stopping");
                        HSTMachine.Workcell.Process.MonitorIOState.StopLogIOState();
                        XyratexOSC.Logging.Log.Info(this, "System process has been stopped by machine low performance triggering.");

                        stopSystemProcess();
                    }
                    else
                    {
                        EnableDisableButton();
                    }

                });
            }
            catch { }
        }

        private void ProbeFunctionalTestStatusChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {

                    if (HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated &&
                        !HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.IsRetestProcessRequired)
                    {
                        HSTMachine.Workcell.DisplayTitleMessage("System Stopping");
                        HSTMachine.Workcell.Process.MonitorIOState.StopLogIOState();
                        XyratexOSC.Logging.Log.Info(this, "System process has been stopped by machine prob functional test failed triggering.");

                        stopSystemProcess();
                    }
                    else
                    {
                        EnableDisableButton();
                    }

                });
            }
            catch { }
        }

        private void SamplingOverTargetTriggeringStatusChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {

                    if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.SamplingTriggeringOverPercentageActivate)
                    {
                        HSTMachine.Workcell.DisplayTitleMessage("System Stopping");
                        HSTMachine.Workcell.Process.MonitorIOState.StopLogIOState();
                        XyratexOSC.Logging.Log.Info(this, "System process has been stopped by machine because found status {A} sampling on disk over target!");

                        stopSystemProcess();
                    }
                    else
                    {
                        EnableDisableButton();
                    }
                });
            }
            catch { }
        }

        private void EnableDisableButton()
        {
            if (_userAccess != null && !HSTMachine.Workcell.IsInPurgingProcess)
            {
                var testUser = _userAccess.getCurrentUser();
                switch (_userAccess.getCurrentUser().Level)
                {
                    case UserLevel.Monitor:
                    case UserLevel.Administrator:                    
                        buttonRun.Enabled = false;
                        buttonStop.Enabled = false;
                        buttonPause.Enabled = false;
                        buttonPurge.Enabled = false;
                        btnRunTestScript.Visible = false;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = false;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = false;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = false;
                        break;
                    case UserLevel.Operator:
                        buttonRun.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton ||
                        (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState ||
                        HSTMachine.Workcell.CurretCCCActiveStatus.IsAlertActivated)) ? false : true;
                        buttonStop.Enabled = (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState) ? true : false;
                        buttonPause.Enabled = (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState) ? true : false;
                        buttonPurge.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || HSTMachine.Workcell.Process.IsRunState) ? false : true;
                        btnRunTestScript.Visible = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton && (_userAccess.getCurrentUser().Level == UserLevel.Engineer)) ? true : false;
                        buttonSystemInitial.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState)) ? false : true;
                         
                        break;         
                    case UserLevel.Technician:
                        buttonRun.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton ||
                        (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState ||
                        HSTMachine.Workcell.CurretCCCActiveStatus.IsAlertActivated)) ? false : true;
                        buttonStop.Enabled = (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState) ? true : false;
                        buttonPause.Enabled = (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState) ? true : false;
                        buttonPurge.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || HSTMachine.Workcell.Process.IsRunState) ? false : true;
                        btnRunTestScript.Visible = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton && (_userAccess.getCurrentUser().Level == UserLevel.Engineer)) ? true : false;
                        buttonSystemInitial.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState)) ? false : true;
                        break;        
                    case UserLevel.Engineer:
                        buttonRun.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || 
                            (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState ||
                            HSTMachine.Workcell.CurretCCCActiveStatus.IsAlertActivated)) ? false : true;
                        buttonStop.Enabled = (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState) ? true: false;
                        buttonPause.Enabled = (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState) ? true: false;
                        buttonPurge.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || HSTMachine.Workcell.Process.IsRunState) ? false : true;
                        btnRunTestScript.Visible = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton && (_userAccess.getCurrentUser().Level == UserLevel.Engineer)) ? true : false;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = true;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = true;
                        HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = true;
                        buttonSystemInitial.Enabled = (HSTMachine.Workcell.HSTSettings.Install.EnableRunTestScriptButton || (HSTMachine.Workcell.Process.IsRunState && !HSTMachine.Workcell.Process.IsPauseState)) ? false : true;
                        break;
                }

            #region Assign Spacial Person
                bool isSpacialAssign = false;
                switch (testUser.Name.ToString() )
                {
                    //Spacial GID Group="677598","685215","478484","488547","352027","450574","738420","62562","304372":
                    case "677598":
                        isSpacialAssign = true;
                        break;
                    case "685215":
                        isSpacialAssign = true;
                        break;
                    case "478484":
                        isSpacialAssign = true;
                        break;
                    case "352027":
                        isSpacialAssign = true;
                        break;
                    case "450574":
                        isSpacialAssign = true;
                        break;
                    case "738420":
                        isSpacialAssign = true;
                        break;
                    case "62562":
                        isSpacialAssign = true;
                        break;
                    case "304372":
                        isSpacialAssign = true;
                        break;
                }
                if (isSpacialAssign)
                {
                    HSTMachine.Workcell.getPanelData().btnSaveSample.Enabled = true;
                    HSTMachine.Workcell.getPanelData().btnCalSample.Enabled = true;
                    HSTMachine.Workcell.getPanelData().btnSampleReset.Enabled = true;
                    HSTMachine.Workcell.getPanelData().btnGetSampleParam.Enabled = true;
                    HSTMachine.Workcell.getPanelData().btnSaveSample.Enabled = true;
                    HSTMachine.Workcell.getPanelData().btnFindZScore.Enabled = true;
                }
                else
                {
                    HSTMachine.Workcell.getPanelData().btnSaveSample.Enabled = false;
                    HSTMachine.Workcell.getPanelData().btnCalSample.Enabled = false;
                    HSTMachine.Workcell.getPanelData().btnSampleReset.Enabled = false;
                    HSTMachine.Workcell.getPanelData().btnGetSampleParam.Enabled = false;
                    HSTMachine.Workcell.getPanelData().btnSaveSample.Enabled = false;
                    HSTMachine.Workcell.getPanelData().btnFindZScore.Enabled = false;
                }
            #endregion

            }
            else
            {
                if(HSTMachine.Workcell.IsInPurgingProcess)
                {
                    buttonRun.Enabled = false;
                    buttonStop.Enabled = false;
                    buttonPause.Enabled = false;
                    buttonPurge.Enabled = false;
                    btnRunTestScript.Visible = false;
                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = false;
                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = false;
                    HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = false;

                    _workcell.DisplayTitleMessage("System Idle");
                    HSTMachine.Instance.MainForm.getAnimatedProgressBar().Visible = false;
                    _bgworker.Dispose();
                }
            }
        }        

        private void btnRunTestScript_Click(object sender, EventArgs e)
        {
            if (btnRunTestScript.Text.Equals("Run Test Script"))
            {
                btnRunTestScript.Text = "Stop Test Run";
                QF.Instance.Publish(new QEvent(HSTWorkcell.SigRunTestScript));
            }
            else
            {
                HSTWorkcell.stopRunScript = true;
                btnRunTestScript.Enabled = false;
            }
        }

        public void EnableTestScriptRun()
        {
            btnRunTestScript.Text = "Run Test Script";
            btnRunTestScript.Enabled = true;
        }

        public void WindowsCount(int windowCount)
        {
            if (windowCount > 0)
            {
                this.btnWindows.Text = windowCount.ToString();
                if (btnWindows.BackColor == SystemColors.Control)
                    btnWindows.BackColor = Color.Red;
                else btnWindows.BackColor = SystemColors.Control;

                if (!btnWindows.Visible)
                    btnWindows.Visible = true;
            }
            else
            {
                if (btnWindows.Visible)
                    btnWindows.Visible = false;
            }
        }

        private void startpurgingcarrier()
        {   
            UIUtility.Invoke(this, () =>
            {
                HSTMachine.Workcell.Process.InputStationProcess.Controller.InhibitInputStation(false); // release the carrier at the input station
                HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputTurnStation(true);
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(true); // stop the carrier at the outpu station
                HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(true); // stop the carrier at the turn station
                
                while (HSTMachine.Workcell.Process.InputStationProcess.Controller.IsInputStationHoldCarrier()||
                       HSTMachine.Workcell.Process.OutputStationProcess.Controller.IsOutputStationHoldCarrier()||
                       HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier()
                    )
                {
                    if (HSTMachine.Workcell.Process.InputStationProcess.Controller.IsInputStationHoldCarrier())
                        XyratexOSC.Logging.Log.Info("Purge Carrier","Carrier at input station");

                    if (HSTMachine.Workcell.Process.OutputStationProcess.Controller.IsOutputStationHoldCarrier())
                        XyratexOSC.Logging.Log.Info("Purge Carrier", "Carrier at output station");

                    if (HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                    {
                        XyratexOSC.Logging.Log.Info("Purge Carrier", "Carrier at output turn station");
                        //turn the table release the carrier
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(true);// set inhibit at output station
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(true); //set inhibit at turn station
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo90Deg(out _timeUsed); // turn the turn station 90 D
                        while (HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.IsOutputTurnStationHoldCarrier())
                        {
                            HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(false);//disable inhibit at turn station

                            HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionReleaseCarrier(true);
                            HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.WaitOutputTurnStationPartCleared();
                            
                        }
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(true); //set inhibit at turn station
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.WaitOutputTurnStationPartCleared();
                        HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo0Deg(out _timeUsed); // turn the turn station 90 D
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(false);// remove inhibit at output station
                        //remove the carrier from list (can remove at later stage if required)
                        
                        lock (CommonFunctions.Instance.InputCarriersLock)
                        {
                            CommonFunctions.Instance.InputCarriers.Dequeue();
                        }
                    }
                    else
                    {
                        
                        HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(false); // remove inhibit from output station so that carrier can move to turn station

                    }
                    Thread.Sleep(1000);// sleep 1 second
                    
                }    // end of while loop          
                this.buttonPurge.Enabled = true;
            });
        }

        private void btnOffAlarm_Click(object sender, EventArgs e)
        {
            _workcell.OffAlarm = true;
        }

        public void LoadProductRecipe()
        {
            this.buttonRun.Enabled = false;
            toolStrip1.Enabled = false;

            HSTMachine.Instance.MainForm.TestFirmwareVersionEvent -= new EventHandler(DisplayFirmwareStatus);
            HSTMachine.Instance.MainForm.TestFirmwareVersionEvent += new EventHandler(DisplayFirmwareStatus);

            // Get product name from recipe
            if (CommonFunctions.Instance.ProductRecipeName != null)
            {
                _workcell.LoadProductRecipe();

                if (!HSTMachine.Instance.MainForm.TestProbe37GetFirmwareVersion.MinorRevision.ToString().Equals("0"))
                {
                    HSTMachine.Instance.MainForm.TestFirmwareVersionEvent -= new EventHandler(DisplayFirmwareStatus);
                    this.buttonRun.Enabled = true;
                    toolStrip1.Enabled = true;
                    toolStrip1.Refresh();

                }
            }            
        }

        private void buttonSystemInitial_Click(object sender, EventArgs e)
        {
            Process_OnManualInit(this, new EventArgs());
        }

        private void DisplayFirmwareStatus(object sender, EventArgs e)
        {
            TestFirmwareEventArgs testFirmwareEventArgs = e as TestFirmwareEventArgs;

            HSTMachine.Instance.MainForm.TestFirmwareVersionEvent -= new EventHandler(DisplayFirmwareStatus);

            UIUtility.Invoke(this, () =>
            {
                var ver = testFirmwareEventArgs.TestStatus;
                if(ver != string.Empty)
                {
                    if (!HSTMachine.Workcell.Process.IsRunState)
                    {
                        Log.Info(this, "Re-download firmware for functional test process",DateTime.Now);
                        HSTMachine.Workcell.getPanelSetup().DownloadConfigurationToMicroProcessor();
                    }
                }
                this.buttonRun.Enabled = true;
                toolStrip1.Enabled = true;
                toolStrip1.Refresh();
            });
        }

        private void btnSaveConfigToProcessor_Click(object sender, EventArgs e)
        {
            string woPath = string.Empty;
            _workcell.IsRecipeLoadedDone = false;
            try
            {
                string initialPath = string.Empty;
                if (!string.IsNullOrEmpty(HSTMachine.Workcell.HSTSettings.Directory.TSRRecipeGlobalPath))
                    initialPath = HSTMachine.Workcell.HSTSettings.Directory.TSRRecipeGlobalPath;
                else
                    initialPath = @"C:\";

                OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                    InitialDirectory = initialPath,
                    Title = "Browse TSR Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "TSR",
                    Filter = "TSR Recipe files (*.HST)|*.HST",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var name = openFileDialog1.SafeFileName;
                    string[] txtspilt = name.Split('.');
                    CommonFunctions.Instance.ProductRecipeName = txtspilt[0];
                    HSTMachine.Workcell.SetupConfig.LastRunRecipeName = CommonFunctions.Instance.ProductRecipeName;
                    HSTMachine.Workcell.SetupConfig.Save();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            btnSaveConfigToProcessor.Enabled = false;
            try
            {
                LoadProductRecipe();
                _workcell.IsRecipeLoadedDone = true;
            }
            catch (Exception ex)
            {
                btnSaveConfigToProcessor.Enabled = true;
                throw ex;
            }

            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().labelANCEnableStatus.Visible = HSTSettings.Instance.CccParameterSetting.Enabled;
            btnSaveConfigToProcessor.Enabled = true;

        }

        private void LoadDefaultRecipe(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {
                    if (!_isDefaultRecipeLoaded)
                    {
                        LoadProductRecipe();
                        _isDefaultRecipeLoaded = true;
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }

    }
}
