using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.UI.Operation.WorkOrder;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.UI.Operation
{
    public partial class OperationMainPanel : UserControl
    {
        // Nested declarations -------------------------------------------------    
        // Member variables ----------------------------------------------------
        //private bool IsRun = false;
        private HSTWorkcell _workcell;
        private int _triggerFlashCounter = 0;
        private bool _loadRecipeStatus = true;

        public OperationMainPanel()
        {

            InitializeComponent();
            if (HSTMachine.Workcell == null)
                return;

            _workcell = HSTMachine.Workcell;

            InitGrid();
            partsCheckNest.ShowLabel(10, false);
            labelCallTecnician.Visible = false;
            HSTMachine.Workcell.Process.InputStationProcess.Controller.RfidController.OnRFIDFolaReadInputStationDone += new ReadWriteRFIDController.RFIDFolaReadEventHandler(Controller_OnRFIDInputStationReadDone);
            HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfidController.OnRFIDFolaReadOutputStationDone += new ReadWriteRFIDController.RFIDFolaReadEventHandler(Controller_OnRFIDOutputStationReadDone);

            if (HSTMachine.Workcell != null)
            {
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.TestProbeProcess.ToString(), labelTestProbe);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.PrecisorStationProcess.ToString(), labelPrecisorStation);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.InputEEProcess.ToString(), labelInputEE);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.OutputEEProcess.ToString(), labelOutputEE);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.OutputStationProcess.ToString(), labelOutputStation);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.OutputTurnStationProcess.ToString(), labelOutputTurnStation);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.InputTurnStationProcess.ToString(), labelInputTurnStation);
                ServiceManager.Tracing.SetTraceWindow(HSTMachine.Workcell.Process.InputStationProcess.ToString(), labelInputStation);
            }

        }

        private void InitGrid()
        {
            for (int i = 0; i < 10; i++)
            {
                listViewUnload.Items[i].SubItems.AddRange(new string[] { "", "" });
                listViewLoad.Items[i].SubItems.AddRange(new string[] { "", "" });
            }
            for (int i = 0; i < 20; i++)
            {
                listViewBola1.Items[i].SubItems.AddRange(new string[] { "", "" });
                listViewBola2.Items[i].SubItems.AddRange(new string[] { "", "" });
            }
        }

        void Controller_OnRFIDInputStationReadDone(FolaTagData tag)
        {
            if (tag == null)
                return;

            this.BeginInvoke((MethodInvoker)delegate
            {
                txtUnloadCarrierID.Text = tag.CarrierID;
                for (int i = 0; i < tag.CarrierSize; i++)
                {
                    listViewUnload.Items[i].SubItems[1].Text = tag[i].HgaSN;
                    listViewUnload.Items[i].SubItems[2].Text = tag[i].Status.ToString();
                }
            });
        }

        void Controller_OnRFIDOutputStationReadDone(FolaTagData tag)
        {
            if (tag == null)
                return;

            this.BeginInvoke((MethodInvoker)delegate
            {
                txtLoadCarrierID.Text = tag.CarrierID;
                for (int i = 0; i < tag.CarrierSize; i++)
                {
                    listViewLoad.Items[i].SubItems[1].Text = tag[i].HgaSN;
                    listViewLoad.Items[i].SubItems[2].Text = tag[i].Status.ToString();
                }
            });
        }

        private void OperationMainPanel_Load(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell == null)
                return;

            woInformationPanel1.Enabled = true; // Don't delete                  
            HSTMachine.Workcell.IsFirmwareGetDone = false;
            HSTMachine.Workcell.CurretCCCActiveStatus.ClearActiveStatus();
        }

        private void DisplayRFIDStatus()
        {
            SetRFIDStateColor(txtUnloadCarrierID, HSTMachine.Workcell.Process.InputStationProcess.Controller.RfidInputStationState);
            SetRFIDStateColor(txtLoadCarrierID, HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfidOutputStationState);
        }

        private void DisplayTriggeringStatus()
        {
            bool displayErr = false;
            bool assignErr = false;

            if (_workcell.GroundMonitoringErrActivated)
            {
                displayErr = true;
            }
            else
            {
                if (_workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated)
                {
                    if (!_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted && !_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished)
                    {
                        assignErr = true;
                        displayErr = true;
                        labelCallTecnician.Text = "PERFORMANCE OVER TARGET, PLEASE CALL TECHNICIAN";
                        labelCallTecnician.BackColor = Color.DarkRed;
                        labelCallTecnician.ForeColor = Color.Yellow;
                    }
                    else
                        if (_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted)
                        {
                            assignErr = true;
                            displayErr = true;
                            labelCallTecnician.Text = "Inprogressing to buyoff process..";
                            labelCallTecnician.BackColor = Color.Yellow;
                            labelCallTecnician.ForeColor = Color.Black;
                        }
                    if (_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished)
                    {
                        displayErr = false;
                    }
                }
                else
                    displayErr = false;
            }

            if (_workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated &&
                !_workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }
                labelCallTecnician.Text = "MEASUREMENT BOARD FAIL, PLEASE CALL TECHNICIAN";
                labelCallTecnician.BackColor = Color.DarkRed;
                labelCallTecnician.ForeColor = Color.Yellow;
            }
            else if (!_workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated &&
                !_workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated)
            {
                if (!assignErr)
                    displayErr = false;
            }

            if (_workcell.HSTSettings.TriggeringSetting.ErrorCodeTriggeringActivate &&
                CommonFunctions.Instance.ActivePopupCRDLerrorMessage)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }
                labelCallTecnician.Text = "HIGH CRDL FAILURE, PLEASE CALL TECHNICIAN";
                labelCallTecnician.BackColor = Color.DarkRed;
                labelCallTecnician.ForeColor = Color.Yellow;
            }
            else
            {
                if (!assignErr)
                    displayErr = false;
            }

            if (HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.SamplingTriggeringOverPercentageActivate)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }
                labelCallTecnician.Text = "SAMPLING OVER TARGET, PLEASE CALL TECHNICIAN";
                labelCallTecnician.BackColor = Color.DarkRed;
                labelCallTecnician.ForeColor = Color.Yellow;
            }
            else
            {
                if (!assignErr)
                    displayErr = false;
            }


            if(HSTMachine.Workcell.HSTSettings.TriggeringSetting.ErrorCodeHeightWriterFailureActivate)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }
                labelCallTecnician.Text = "HIGH FAILURE WRITER BRIDGE , PLEASE CALL TECHNICIAN";
                labelCallTecnician.BackColor = Color.DarkRed;
                labelCallTecnician.ForeColor = Color.Yellow;
            }
            else
            {
                if (!assignErr)
                    displayErr = false;
            }

            if (CommonFunctions.Instance.ActivePopupGetputErrorMessage)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }
                if (FISManager.Instance.IsFISConnected)
                    labelCallTecnician.Text = "OLD SLIDER SDET, NO SLIDER DATA ON SEATRACK";
                else
                    labelCallTecnician.Text = "GETPUTSERVER DOWN, PLEASE CALL FIS EXT#7510";

                labelCallTecnician.BackColor = Color.DarkRed;
                labelCallTecnician.ForeColor = Color.Yellow;
            }
            else
            {
                if (!assignErr)
                    displayErr = false;
            }

            //Recipe load fail
            if(!_loadRecipeStatus)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }

                string msg = string.Format("Recipe name {0} not found, please check recipe!", CommonFunctions.Instance.ProductRecipeName);
                labelCallTecnician.Text = msg;

                labelCallTecnician.BackColor = Color.DarkRed;
                labelCallTecnician.ForeColor = Color.Yellow;
            }
            if (HSTMachine.Workcell.IsInPurgingProcess && !displayErr)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }

                string msg = string.Format("In progressing to purge, please wait until process done", CommonFunctions.Instance.ProductRecipeName);
                labelCallTecnician.Text = msg;

                labelCallTecnician.BackColor = Color.Yellow;
                labelCallTecnician.ForeColor = Color.DarkRed;

            }

            if(CommonFunctions.Instance.ActivePopupRecipeChangedErrorMessage)
            {
                if (!displayErr)
                {
                    assignErr = true;
                    displayErr = true;
                }

                string msg = CommonFunctions.Instance.RecipeChangeErrorMessage;
                labelCallTecnician.Text = msg;
                labelCallTecnician.BackColor = Color.Yellow;
                labelCallTecnician.ForeColor = Color.DarkRed;
            }

            if (displayErr)
            {
                if (_triggerFlashCounter == 0)
                {
                    labelCallTecnician.Visible = true;
                }
                _triggerFlashCounter++;
                if (_triggerFlashCounter == 5)
                    labelCallTecnician.Visible = false;
                if (_triggerFlashCounter > 8)
                    _triggerFlashCounter = 0;
            }
            else
            {
                if (labelCallTecnician.Visible)
                    labelCallTecnician.Visible = false;
                _triggerFlashCounter = 0;
            }
        }

        private void SetListViewColor(ListViewItem item, bool status, Color presentColor)
        {
            if (!status || item.SubItems.Count == 1 || (item.SubItems.Count > 1 && item.SubItems[1].Text.Equals("")))
            {
                item.BackColor = SystemColors.Window;
                item.ForeColor = SystemColors.WindowText;
            }
            else
            {
                item.BackColor = presentColor;
                item.ForeColor = SystemColors.ButtonHighlight;
            }
        }

        private void SetRFIDStateColor(TextBox textBox, ReadWriteRFIDController.RFIDState rfidState)
        {
            switch (rfidState)
            {
                case ReadWriteRFIDController.RFIDState.Reading:
                    textBox.BackColor = System.Drawing.Color.ForestGreen;
                    break;
                case ReadWriteRFIDController.RFIDState.Writing:
                    textBox.BackColor = System.Drawing.Color.DarkOrange;
                    break;
                case ReadWriteRFIDController.RFIDState.Idle:
                    textBox.BackColor = System.Drawing.SystemColors.Control;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplayRFIDStatus();
            DisplayTriggeringStatus(); 
        }

        private void OperationMainPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                timer1.Enabled = this.Visible;
                _workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated =
                    _workcell.LoadCounter.CarrierTriggeringData.CriticalTriggeringActivated;

                _workcell.CCCDefectSelectedChanged -= UpdateCCCDefectChange;
                _workcell.CCCVerifyEventRaised -= DisplayCCCAlertMessage;
                _workcell.RecipeLoadedCompletedHandler -= LoadRecipeStatusUpdate;

                _workcell.CCCDefectSelectedChanged += UpdateCCCDefectChange;
                _workcell.CCCVerifyEventRaised += DisplayCCCAlertMessage;
                _workcell.RecipeLoadedCompletedHandler += LoadRecipeStatusUpdate;

                if (_workcell.SetupConfig.LastRunRecipeName != String.Empty)
                    CommonFunctions.Instance.ProductRecipeName = _workcell.SetupConfig.LastRunRecipeName;
            }
        }

        public MeasurementTest getMeasurementTestUserControl()
        {
            return MeasurementTestResult;
        }

        public ProductionCounter getProductionCounterUserControl()
        {
            return productionCounter1;
        }


        private void UpdateCCCDefectChange(object sender, CCCDefectSelection args)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UpdateCCCDefectChange(sender, args)));
            }
            else
            {
                if (args.Defect == CCCAlertInformation.CCCMcDefect.TICDefect)
                {
                    string errmsg = string.Empty;
                    string errcode = string.Empty;

                    if (_workcell.CCCFailureInfo.LastFailureStation.ToString() == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1.ToString())
                    {
                        HSTMachine.Workcell.getPanelData().lblLastTriggerMC1.Text = System.DateTime.Now.ToString();
                        errcode = _workcell.TICCccControl.CCCControlTicMc1.CccResult.CCCDataLogger.FAILURE_TYPE;
                    }
                    else if (_workcell.CCCFailureInfo.LastFailureStation.ToString() == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2.ToString())
                    {
                        HSTMachine.Workcell.getPanelData().lblLastTriggerMC2.Text = System.DateTime.Now.ToString();
                        errcode = _workcell.TICCccControl.CCCControlTicMc2.CccResult.CCCDataLogger.FAILURE_TYPE;
                    }
                    else
                    {
                        //do nothing
                    }


                    if (errcode.Contains("ANC"))
                        errmsg = "ANC Fail Machine Trigger - TIC Defect";
                    else if (errcode.Contains("CCC"))
                        errmsg = "CCC Fail Machine Trigger - TIC Defect";

                    Log.Maintenance(this, "{0},{1},{2}", errmsg, "FailCode=" + _workcell.CCCFailureInfo.LastFailureMessage, "TIC Number=" + _workcell.CCCFailureInfo.LastFailureStation);

                    IsAlertFormActived();
                    string topic = "uTIC Yield Triggering";
                    //Mar-2020 Fix error no Station and ErrorMsgCode 
                    //string almsg = "Please verify uTIC machine number : " + _workcell.CCCFailureInfo.FailedMc + Environment.NewLine +
                    //    "uTIC height failure code : " + _workcell.CCCFailureInfo.FailedCode;
                    string almsg = "Please verify uTIC machine number : " + _workcell.CCCFailureInfo.LastFailureStation.ToString() + Environment.NewLine +
                        "uTIC height failure code : " + _workcell.CCCFailureInfo.LastFailureMessage.ToString();
                    string txtDock = string.Format("UTIC Dock : {0}", _workcell.CCCFailureInfo.FailDockNumber);
                    CCCAlertForm frm = new CCCAlertForm();
                    frm.AssignAlert(CCCAlertForm.AlertType.Acknowledge, almsg, topic, errcode, txtDock);
                    frm.Show();

                    if (_workcell.CCCFailureInfo.LastFailureStation.ToString() == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1.ToString())
                    {
                        HSTMachine.Workcell.getPanelData().lblLastTriggerMC1.Text = System.DateTime.Now.ToString();
                    }
                    else if (_workcell.CCCFailureInfo.LastFailureStation.ToString() == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2.ToString())
                    {
                        HSTMachine.Workcell.getPanelData().lblLastTriggerMC2.Text = System.DateTime.Now.ToString();
                    }

                }
                else if (args.Defect == CCCAlertInformation.CCCMcDefect.HSTDefect)
                {
                    Log.Maintenance(this, "{0},{1},{2}", "ANC Machine Trigger - HST Defect", "FailCode=" + _workcell.CCCFailureInfo.LastFailureMessage, "TIC Number=" + _workcell.CCCFailureInfo.LastFailureStation);

                    IsAlertFormActived();
                    string topic = "HST Probe Position Error";
                    string almsg = "HST Probe Position Fail!" + Environment.NewLine + "Please Re-setup probing teaching point on HST";
                    CCCAlertForm frm = new CCCAlertForm();
                    frm.AssignAlert(CCCAlertForm.AlertType.Acknowledge, almsg, topic,string.Empty,string.Empty);
                    frm.Show();
                }
            }

        }


        private void LoadRecipeStatusUpdate(object sender, LoadRecipeEventArgs status)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => LoadRecipeStatusUpdate(this, status)));
            }
            else
            {
                _loadRecipeStatus = status.LoadStatus;
            }
        }

        public void DisplayCCCAlertMessage(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => DisplayCCCAlertMessage(this, e)));
            }
            else
            {
                IsAlertFormActived();
                var carrierid = _workcell.Process.OutputStationProcess.Controller.CurrentOutputHoldCarrier != null?
                    _workcell.Process.OutputStationProcess.Controller.CurrentOutputHoldCarrier.CarrierID:string.Empty;

                HSTMachine.Workcell.CurretCCCActiveStatus.ChangeActiveStatus(true);
                string almsg = "Please inspect on Tail pad and TIC joint." + Environment.NewLine + "Then select bellow button in the cause of defect that you see." +
                    Environment.NewLine + "Carrier Id : " + carrierid;
                CCCAlertForm frm = new CCCAlertForm();
                frm.AssignAlert(CCCAlertForm.AlertType.Inspect, almsg, "ANC Triggering alert",string.Empty,string.Empty);
                frm.Show();
            }
        }

        private void IsAlertFormActived()
        {
            bool isActive = false;
            FormCollection fa = Application.OpenForms;
            foreach (Form item in fa)
            {
                if (item.Text.Trim().ToLower() == "ancalertform")
                {
                    item.Close();
                    break;
                }
            }
        }
    }
}

