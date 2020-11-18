//
//  � Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/6/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Seagate.AAS.UI;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Settings;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.UI
{

    /// <summary>
    /// Summary description for FormMainE95.
	/// </summary>
	public partial class FormMainE95 : FormMainLite
	{        
        public FormMainE95() : base()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            RegisterComponents();

        }

        public FormMainE95(HSTWorkcell workcell)
        {
            Application.EnableVisualStyles();
            workcell.HSTSettings.getUserAccessSettings().getCurrentUser().Name = "Operator";
            workcell.HSTSettings.getUserAccessSettings().getCurrentUser().Password = "Operator";
            workcell.HSTSettings.getUserAccessSettings().getCurrentUser().Level = UserLevel.Operator;
            workcell.HSTSettings.getUserAccessSettings().getCurrentUser().MaintenanceAccess = false;
            
            _workcell = workcell;            
            InitializeComponent();
            RegisterComponents();
            
            System.IO.File.Delete(CommonFunctions.TempFileToIndicateDownloadConfigToProcessorClicked);
        }

        HSTWorkcell _workcell;              

        public PanelTitle getPanelTitle()
        {
            return panelTitle;
        }

        public PanelCommand getPanelCommand()
        {
            return panelCommand1;
        }

        public PanelNavigation getPanelNavigation()
        {
            return Navigation1;
        }
        
        public ToolStripProgressBar getAnimatedProgressBar()
        {
            return toolStripProgressBar1;
        }

        private void RegisterComponents()
        {
            this.Text = HSTMachine.Workcell.HSTSettings.Install.EquipmentID + " {HAMR PRODUCT}";

            viewChangeMsgChannel.MessageSentEvent +=new Seagate.AAS.Parsel.Services.MessageChannel.ReceiveMessageHandler(RecieveViewChangeMessage);

            try
            {
                Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.SetMainForm(this, 0, panelTitle.Height, viewChangeMsgChannel);
            }
            catch(Exception e)
            {
                // swallow ServiceManager null reference exceptions thrown by the VS Forms Designer
            }

            // register panels for message notification
            panels.Add(panelTitle);
            panels.Add(Navigation1);
            panels.Add(panelCommand1);
            //panels.Add(SubNavigation1);

            if (HSTMachine.Workcell != null)
            {
                panelTitle.AssignWorkcell(HSTMachine.Workcell);
                panelCommand1.AssignWorkcell(HSTMachine.Workcell);
                Navigation1.AssignWorkcell(HSTMachine.Workcell);
                HSTMachine.Workcell.DisplayTitleMessage("System Idle");

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
                    this.ModeCaption = "Auto";
                }
                else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun)
                {
                    this.ModeCaption = (HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat) ? "Dry Run (No Boats)" : "Dry Run (With Boats)";
                }
                else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass)
                {
                    this.ModeCaption = "Bypass";
                }
                else if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
                {
                    this.ModeCaption = "Simulation";
                }                
                
                testProbeComPort.PortName = HSTMachine.Workcell.CalibrationSettings.MeasurementTest.COMPort.ToString();
                testProbeComPort.BaudRate = HSTMachine.Workcell.CalibrationSettings.MeasurementTest.BaudRate;
                testProbeComPort.DataBits = HSTMachine.Workcell.CalibrationSettings.MeasurementTest.DataBits;
                testProbeComPort.RtsEnable = false;
                testProbeComPort.DtrEnable = false;

                try
                {
                    testProbeComPort.Open();
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info("Test Probe COM Port", "Test Probe serial port: {0}, baud rate: {1}, data bits: {2}, stop bits: {3}, parity: {4} has been opened.",
                                 testProbeComPort.PortName, testProbeComPort.BaudRate, testProbeComPort.DataBits, testProbeComPort.StopBits, testProbeComPort.Parity);
                    }
                }

                catch (Exception ex)
                {
                    Log.Error("Test Probe COM Port", "Error opening Test Probe serial port: {0}.", ex.Message);
                }                         
            }
        }


        private void RecieveViewChangeMessage(object src, Seagate.AAS.Parsel.Services.Message msg)
        {
            panelTitle.ViewChangedHandler(msg.Text);
        }
        

        public bool ShowPopupMenus
        {
            set { Navigation1.ShowPopupMenus = value; }
        }

        public Color ButtonSelectedColor
        {
            set { Navigation1.ButtonSelectedColor = value; }
        }

        public string ModeCaption
        {
            set { XyratexOSC.UI.UIUtility.Invoke(panelTitle, () => panelTitle.RunModeCaption = value); }
        }                

        private void FormMainE95_FormClosing(object sender, FormClosingEventArgs e)
        {
            string workcellName = "";
            if(HSTMachine.Workcell != null)
                workcellName = HSTMachine.Workcell.Name;

            if (!HSTMachine.Workcell.AllProcessesIdle)
            {             
                if("Abort"== XyratexOSC.UI.Notify.PopUp(String.Format("Abort {0}...",workcellName),
                                                        "One or more processes are actively running","Abort running processes","Abort", "Cancel"))
                {
                    ProcessHST process = (ProcessHST)HSTMachine.Workcell.MainProcess;
                    process.Abort();
                }
                e.Cancel = true;
                return;
            }

            if ("Yes" != XyratexOSC.UI.Notify.PopUp(string.Format("Exiting {0} Application. . .", workcellName),    
                                                string.Format("Are you sure you want to quit {0}?", workcellName),
                                                "", "Yes", "No"))
            {
                e.Cancel = true;
                return;
            }
            
            HSTWorkcell.terminatingHSTApps = true;
            _workcell.Process.MonitorProcess.disableTowerLightAndDoorEvent();
            testProbeComPort.Close();

            if (_workcell.HSTSettings.Install.ClearImproperShutDownMessage == true)
            {
                System.IO.File.Delete(CommonFunctions.TempFileWhenRunningHSTApplication);
            }
            e.Cancel = false;
            return;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            DriveInfo driveInfo = new DriveInfo(@"C:");

            double freeInMB = driveInfo.TotalFreeSpace / 1024.0 / 1024.0;
            double sizeInMB = driveInfo.TotalSize / 1024.0 / 1024.0;
            if ((freeInMB/1024) < (sizeInMB / 1024.0 * 0.2))
            {

                Notify.PopUp("Hard Disk Space", String.Format("Hard Disk Space is running low. Free Disk space: {0} GB", (int)freeInMB / 1024), "", "OK");
            }
            timerChkWindows.Start();
        }

        private void timerChkWindows_Tick(object sender, EventArgs e)
        {
            UpdateDiskSpace();

            int windowCount = 0;
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                if (Application.OpenForms[i].Visible)
                {
                    if (!Application.OpenForms[i].Name.Equals("FormMainE95"))
                        windowCount++;
                }
            }

            if (windowCount > 0)
            {
                UIUtility.Invoke(this, () =>
                {
                    panelCommand1.WindowsCount(windowCount);
                });
            }
            else
            {
                UIUtility.Invoke(this, () =>
                {
                    panelCommand1.WindowsCount(windowCount);
                });

            }

        }

        private void UpdateDiskSpace()
        {
            try
            {
                DriveInfo driveInfo = new DriveInfo(@"C:");

                double freeInMB = driveInfo.TotalFreeSpace / 1024.0 / 1024.0;
                double sizeInMB = driveInfo.TotalSize / 1024.0 / 1024.0;

                if (freeInMB / sizeInMB < .10)
                    statusProgressDisk.BackColor = Color.Tomato;
                else if (freeInMB / sizeInMB < .25)
                    statusProgressDisk.BackColor = Color.Orange;
                else
                    statusProgressDisk.BackColor = Color.DodgerBlue;

                statusProgressDisk.Label = String.Format("{0:F1} free of {1:F1} GB", freeInMB / 1024.0, sizeInMB / 1024.0);
                statusProgressDisk.Maximum = (int)sizeInMB;
                statusProgressDisk.Value = (int)(sizeInMB - freeInMB);

               
            }
            catch (System.IO.IOException)
            {
                //TODO: Any action here?
            }
        }
    }
}
