using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Runtime.InteropServices;
using Seagate.AAS.UI;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.UI.Utils;
using System.Diagnostics;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using qf4net;
using System.IO;
using System.Threading;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;

namespace Seagate.AAS.HGA.HST.UI.Main
{
    public partial class PanelData : UserControl
    {
        HSTWorkcell _workcell;
        private RollingListBoxListener rlbListener;
        private int _testCounter = 0;
        private List<double[,]> _testList = new List<double[,]>();
        private int _machineTriggerDownCounter = 0;
        private int _lastDayActive = 0;

        List<double> sampledatalist = new List<double>();
        public PanelData(HSTWorkcell workcell)
        {
            InitializeComponent();
            _workcell = workcell;
            HSTMachine.Workcell.csvFileOutput.NewCSVRecordAdded += AddNewCSVRecordToPanelData;
            HSTMachine.Workcell.csvFileOutput.NewCSVRecordUpdated += RecordUpdateComplete;
            HSTMachine.Workcell.OnFinalCCCRunPart += UpdateData;
            if (!this.DesignMode)
            {
                rlbListener = new RollingListBoxListener(listBox, txtLatest);
                rlbListener.AutoScrollToBottom = true;
                rlbListener.MaxEntries = 50;
                TraceSource trace = XyratexOSC.Logging.Log.Trace();
                trace.Listeners.Add(rlbListener);
            }

        }

        public Operation.WorkOrder.FunctionalTest FunctionalTestPanel
        { get { return functionalTest2; } }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            rlbListener.Close();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void CopyToClipboard()
        {
            StringBuilder sb = new StringBuilder();

            foreach (object line in listBox.SelectedItems)
                sb.AppendLine(line.ToString());

            string clipboardText = sb.ToString();

            if (!String.IsNullOrEmpty(clipboardText))
                Clipboard.SetText(clipboardText);
        }


        private void PanelData_Load(object sender, EventArgs e)
        {
            labelYieldSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget.ToString("F2");
            labelYieldLimitSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit.ToString("F2");
            labelCCCCounterLimitSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit.ToString();
            labelTestRunSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup.ToString();
            labelLCLSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha.ToString();
            var mCDownTriggering = _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering +
                _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering;
            _machineTriggerDownCounter = mCDownTriggering;
            labelMachineTriggerCountTIC.Text = _machineTriggerDownCounter.ToString();
            label1Mc1Name.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1.ToString();
            label1Mc2Name.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2.ToString();
            lblCurrentCount.Text = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.InternalTriggerCounter.ToString();
            lblCurrentCountMC2.Text = HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.InternalTriggerCounter.ToString();
            labelMC1DefectCounter.Text = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter.ToString();
            labelMC2DefectCounter.Text = HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter.ToString();
            labelMC1TriggerCounter.Text = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.AdaptiveNumber.ToString();
            labelMC2TriggerCounter.Text = HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.AdaptiveNumber.ToString();
            labelMC1Yield.Text = Math.Round(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield, 2).ToString();
            labelMC2Yield.Text = Math.Round(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.current_yield, 2).ToString();
        }

        public void AddNewCSVRecordToPanelData(object sender, EventArgs e)
        {
            CSVEventArgs cSVEventArgs = e as CSVEventArgs;
            string CSVRecord = cSVEventArgs.CSVRecord;
            List<string> CSVFieldList = CSVRecord.Split(',').ToList();
            string Timestamp = CSVFieldList.ElementAt(0);
            CSVFieldList.RemoveAt(0);

            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                CSVFieldList[8] = CSVFieldList[45];

            listViewDataLog.Items.Add(Timestamp).SubItems.AddRange(CSVFieldList.ToArray());

        }

        public void RecordUpdateComplete(object sender, EventArgs e)
        {

            //Remove item if counter is more than 500
            if (listViewDataLog.Items.Count > HSTMachine.Workcell.HSTSettings.Install.DataRecordDisplayCounter)
            {
                bool start = false;
                bool delete = false;
                int startnumber = 0;

                for (int i = 1; i <= 10; i++)
                {
                    var slot = listViewDataLog.Items[0].SubItems[2].Text;
                    if (!string.IsNullOrEmpty(slot))
                    {
                        if (Convert.ToInt32(slot) <= 10)
                        {
                            if (!start)
                            {
                                start = true;
                                startnumber = Convert.ToInt32(slot);
                            }

                            if (Convert.ToInt32(slot) >= startnumber)
                                delete = true;
                            else
                            {
                                i = 10;
                                delete = false;
                            }
                        }
                        if (delete)
                            listViewDataLog.Items.RemoveAt(0);
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var T1 = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend1[1];
            var T2 = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend2[1];
            var T3 = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend1[1];
            var T4 = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend2[1];

            //LDU_IThreshold[hga] = (ISweep_C_Trend2[hga] - ISweep_C_Trend1[hga]) / (ISweep_M_Trend1[hga] - ISweep_M_Trend2[hga]);

            double IThreshold = (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend2[0] - HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_C_Trend1[0]) /
                (HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend1[0] - HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().ISweep_M_Trend2[0]);
            double MaxVPD = HSTMachine.Workcell.getPanelRecipe().GetPanelLDUMeasurement().LDUMaxVPD[1];
            var teest = false;
            //HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = true;
            //var ttt = new TimeSpan(tictime.Hour, tictime.Minute, tictime.Second).TotalMinutes;

            //_workcell.CCCMachineTriggeringActivated = true;
            //string almsg = "Please inspect on Tail pad and TIC joint." + Environment.NewLine + "Then select bellow button in the cause of defect that you see.";
            //HSTMachine.Workcell.CurretCCCActiveStatus.ChangeActiveStatus(true);
            //CCCAlertForm frm = new CCCAlertForm();
            //frm.AssignAlert(CCCAlertForm.AlertType.Inspect, almsg, "CCC Triggering alert");
            //frm.Show();

            //IBSObj ibsobj = new IBSObj(IBSObj.IbsPattern.W2, IBSObj.IbsPattern.W2, HGAProductTabType.Down);
            //var isallWr = ibsobj.IsWriterAllPattern;
            //var choiceflag = ibsobj.CurentChoiceFlag;
            //HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CriticalTriggeringActivated = false;
            //_workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = false;
            //_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
            //_workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;


            //_workcell.HSTSettings.ResistanceCheckingConfig.ResistanceCriticalActivated = false;
            //_workcell.Process.MonitorProcess.Controller.IsCriticalTriggeringActivated = false;


            //CommonFunctions.Instance.CRDLErrorMessage = String.Format("Total number of consecutive CRDL failure exceeed user defined limit, current percentage =({0}%)", "28.50");
            //HSTSettings.Instance.TriggeringSetting.ErrorCodeTriggeringActivate = true;
            //QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));

            //CommonFunctions.Instance.GetputErrorMessage = String.Format("High FAILGETSORT from old SDET Slider(build on last 6 months), please call process ");
            //CommonFunctions.Instance.GetputErrorMessage = String.Format("GetputServer Connection Failed, please call technician");
            //CommonFunctions.Instance.StopMachineRunDueToGetputError = true;
            //QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));


            //HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.SamplingTriggeringOverPercentageActivate = false;

            //DateTime dt = new DateTime(2018, 06, 27, 08, 30, 30, 30);
            //HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.LastActiveDate = dt.ToString();
            //HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
            //HSTMachine.Workcell.HSTSettings.ResistanceCheckingConfig.LastCheckByHourActive = dt.ToString();
            //HSTMachine.Workcell.LoadCounter.Save();
            //HSTMachine.Workcell.HSTSettings.Save();

            //_testCounter++;

            //if (_testCounter == 1)
            //{
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CriticalTriggeringActivated = true;
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = false;

            //}
            //else if (_testCounter == 2)
            //{
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = true;
            //}
            //else if (_testCounter == 3)
            //{
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessStarted = false;
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.BuyoffProcessFinished = true;
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.CriticalTriggeringActivated = false;
            //    HSTMachine.Workcell.LoadCounter.CarrierTriggeringData.Reset();
            //}
            //else if (_testCounter > 3)
            //{
            //    _testCounter = 0;
            //}

            //HSTMachine.Workcell.LoadCounter.Save();


            //string tdfFileName = TDFOutput.PreTDFName + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2");
            //TDFOutput tdfOutputData = new TDFOutput(HSTSettings.Instance.Directory.TDFLocalDataPath, tdfFileName);
            //var hstTdfData = new TDFOutput.HSTTDFDATA();
            //var fisTdfData = new TDFOutput.FISTDFDATA();

            //hstTdfData.Default();
            //fisTdfData.Default();

            //tdfOutputData.UpdateFisTdfData(fisTdfData);
            //tdfOutputData.UpdateHstTdfData(hstTdfData);
            //tdfOutputData.SaveTDFOutputToFile();


        }



        private void TestCCC2()
        {
            CCCOutput TicMc1CccOutput;
            // Read the file and display it line by line. DataLog_01-10-2018
            string Pth = "C:\\WM\\TESTANC2020_500_4.csv";
            //    string Pth = "C:\\WM\\DataLog_25-12-2018.csv";
            //string Pth = "C:\\WM\\DataLog_02-10-2018.csv";

            System.IO.StreamReader file = new System.IO.StreamReader(Pth);
            string line;
            double y, p, alpha, UCL, LCL, CL;
            int count;
            double testrun;
            testrun = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup;//Convert.ToInt32(textBox6.Text);
            double ylimit;
            ylimit = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit;
            count = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit;
            y = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget; //yeild target
            alpha = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha;

            TicMc1CccOutput = new CCCOutput("UTIC1", (int)testrun, alpha, y, ylimit, count, true, true);

            //var cl = TicMc1CccOutput.HstCccResult.StdCl;
            //var lcl = TicMc1CccOutput.HstCccResult.StdLcl;
            //var ucl = TicMc1CccOutput.HstCccResult.StdUcl;

            p = (100 - y) / 100;
            CL = 1 / p;                                              //CL
            UCL = (Math.Log(alpha / 2)) / (Math.Log(1 - p));             //UCL
            LCL = (Math.Log(1 - (alpha / 2))) / (Math.Log(1 - p));        //LCL
            int defect, good, CRDL, good_r;
            int TIC_Down = 0;
            int TIC_trigger = 0;
            int HST_Down = 0;
            double ad;
            int defectad = 0;
            int testnum;
            int counttrig = 0;
            defect = 0;
            good = 0;
            CRDL = 0;
            good_r = 0;
            ad = 0;
            testnum = 0;

            System.IO.StreamReader file2 = new System.IO.StreamReader(Pth);
            string line2;
            int u = 0;
            int v = 0;
            double ytest = 0;
            ytest = ((100 - ylimit) / 100) * (testrun);

            var minimalMnForGroup = TicMc1CccOutput.CccResult.YieldTest;
            int loop = 0;
            var TICName = "UTC018";

            //GlobalData.TIC_defect = new double[GlobalData.num, 4];
            //GlobalData.Reader_CRDL = new int[GlobalData.num1, 3];
            line2 = file2.ReadLine();
            while (((line2 = file2.ReadLine()) != null) && (testnum < 1200))
            {

                Hga hgadata = null;

                testnum++;
                //TicMc1CccOutput.UpdatePartRunCounter();
                //_workcell.HSTCccControl.CCCControlAllMc.UpdatePartRunCounter();
                //_workcell.HSTCccControl.CCCControlAllMc.UpdateGoodCounter();

                ad = p * (testnum - 1);
                if (ad <= 1)
                {
                    ad = 1;
                }
                if (testnum < testrun || ad <= ytest)
                {
                    ad = ytest;
                }
                string[] parts = line2.Split(',');
                var runstatus = parts[8].Trim();

                if (parts[8] == "")
                {
                    hgadata = new Hga(testnum, HGAStatus.TestedPass);


                }
                else
                {
                    hgadata = new Hga(testnum, HGAStatus.TestedFail);
                    hgadata.Error_Msg_Code = parts[8];
                }
                if (loop == 10)
                {
                    if (TICName == "UTC018")
                    {
                        TICName = "UTC027";
                    }
                    else
                    {
                        TICName = "UTC018";
                    }
                    loop = 0;
                }
                if (parts[1].Trim() == "0")
                {
                    //TICName = "TEST3";
                }
                else
                {
                    //TICName = "TEST4";
                }


                hgadata.UTIC_DATA.EQUIP_ID = TICName;
                loop = loop + 1;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.TICMachineName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
                HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.TICMachineName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;
                //      hgadata.UTIC_DATA.EVENT_DATE = "11-MAR-2020 12:19:06 AM";
                hgadata.UTIC_DATA.EVENT_DATE = System.DateTime.Today.ToString("dd-MMM-yyyy hh:mm:ss tt");
                _workcell.Process.OutputStationProcess.Controller.CheckCCCResult(hgadata);
                if (hgadata.UTIC_DATA.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1)
                {
                    decimal PartCounter = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
                    decimal totalParts = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.AdaptivePartCounter.ToString());

                    decimal currentYield = 0;
                    var totalcount = (totalParts - PartCounter) * 100;
                    try
                    {
                        if ((totalcount != 0 || totalParts != 0) && (totalcount > HSTMachine.Workcell.HSTSettings.CccParameterSetting.PartCounterYieldPerShortPeriod))
                            currentYield = decimal.Divide(totalcount, totalParts);
                    }
                    catch (Exception)
                    {
                    }
                    hgadata.set_ANC_YIELD(Math.Round(double.Parse(currentYield.ToString()), 2));
                    hgadata.set_ANC_HGA_Count(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter);
                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield = double.Parse(currentYield.ToString());
                }
                else if (hgadata.UTIC_DATA.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2)
                {
                    decimal PartCounter = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
                    decimal totalParts = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.AdaptivePartCounter.ToString());

                    decimal currentYield = 0;
                    var totalcount = (totalParts - PartCounter) * 100;
                    try
                    {
                        if ((totalcount != 0 || totalParts != 0) && (totalcount > HSTMachine.Workcell.HSTSettings.CccParameterSetting.PartCounterYieldPerShortPeriod))
                            currentYield = decimal.Divide(totalcount, totalParts);
                    }
                    catch (Exception)
                    {
                    }
                    hgadata.set_ANC_YIELD(Math.Round(double.Parse(currentYield.ToString()), 2));
                    hgadata.set_ANC_HGA_Count(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter);
                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.current_yield = double.Parse(currentYield.ToString());
                }
                else
                {
                    hgadata.set_ANC_YIELD(0);
                    hgadata.set_ANC_HGA_Count(0);

                }

            }
        }

        public string GetErrorCodePriority(List<string> errList)
        {
            string returnCode = string.Empty;
            int lastPriority = 99;
            string assignErrCode = string.Empty;
            int currentPriority = 0;
            foreach (var item in errList)
            {
                foreach (var errcode in Enum.GetValues(typeof(ERROR_MESSAGE_CODE)))
                {
                    if (item == errcode.ToString())
                    {
                        assignErrCode = errcode.ToString();
                        currentPriority = (int)errcode;
                    }
                }

                if (currentPriority < lastPriority)
                {
                    returnCode = assignErrCode;
                    lastPriority = currentPriority;
                }
            }

            return returnCode;
        }


        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.DesignMode)
                CopyToClipboard();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void listBox_Resize(object sender, EventArgs e)
        {
            if (!this.DesignMode)
                if (rlbListener != null && rlbListener.AutoScrollToBottom)
                    rlbListener.ScrollToBottom();
        }

        private void checkDown_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.DesignMode)
                if (checkDown.Checked)
                {
                    checkDown.Image = Properties.Resources.down_enabled;
                    toolTip1.SetToolTip(checkDown, "Disable Auto-Scroll");
                    rlbListener.AutoScrollToBottom = true;
                }
                else
                {
                    checkDown.Image = Properties.Resources.down_disabled;
                    toolTip1.SetToolTip(checkDown, "Enable Auto-Scroll");
                    rlbListener.AutoScrollToBottom = false;
                }
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C) ||
                e.KeyData == (Keys.Control | Keys.Shift | Keys.C))
            {
                CopyToClipboard();
            }
        }

        private void tabControl1_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && !this.DesignMode)
            {
                PanelData_Load(this, null);
                XyratexOSC.Logging.Log.FlushAllTraces();
            }

        }

        private void UpdateData(object sender, CCCFinalRunResult args)
        {
            label1Mc1Name.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
            label1Mc2Name.Text = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;
            _workcell.GraphAndMcMapping.Graph1McName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1;
            _workcell.GraphAndMcMapping.Graph2McName = HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2;
            if ((args.UticData.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName1) || (args.UticData.EQUIP_ID == HSTMachine.Workcell.HSTSettings.ANCSettings.uTICMachineName2))
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => UpdateData(sender, args)));
                }
                else
                {
                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.OnMcDownTrigger -= UpdateMcDownTriggering;
                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.OnMcDownTrigger -= UpdateMcDownTriggering;
                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.OnMcDownTrigger += UpdateMcDownTriggering;
                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.OnMcDownTrigger += UpdateMcDownTriggering;

                    var dataItem = args.PartUnitData;
                    var uticData = args.UticData;

                    if (_lastDayActive == 0) _lastDayActive = DateTime.Now.Day;
                    if (_lastDayActive != DateTime.Now.Day)
                        labelMachineTriggerCountTIC.Text = "0";

                    if (args.TriggerType == CCCOutput.Trigger_Type.TIC)
                    {
                        //Display all data
                        labelYieldSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget.ToString("F2");
                        labelYieldLimitSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit.ToString("F2");
                        labelCCCCounterLimitSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit.ToString();
                        labelTestRunSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup.ToString();
                        labelLCLSpec.Text = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha.ToString();

                        int totalDefect = 0;
                        double totalAdaptiveDefect = 0;
                        bool activeGraph1 = false;
                        bool activeGraph2 = false;
                        if (uticData.EQUIP_ID == _workcell.GraphAndMcMapping.Graph1McName)
                        {
                            _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastCCCOutputUTICtime = uticData.EVENT_DATE;
                            _workcell.TICCccControl.CCCControlTicMc1.UpdateDefectCounter(CCCOutput.CCC_MC_Type.TIC);
                           
                            _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TriggerFailureType = CCCOutput.MC_Trigger_Failure_Type.PART_FAIL;
                            // 16-Jun-2020 TIC Alert fix Last Time
                            if (_workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.IsTriggering)
                            {
                                HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertActiveTime = _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastTICAlertActiveTime;
                            }

                            totalDefect = chartTicDefectChart.Series[1].Points.Count + 1;
                            totalAdaptiveDefect = Convert.ToDouble(_workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
                            activeGraph1 = true;

                            if (totalDefect > HSTSettings.Instance.Install.ANCGraphCounterMaximum)
                            {
                                chartTicDefectChart.Series["PartTrigger"].Points.Clear();
                                chartTicDefectChart.Series["DefectCounter"].Points.Clear();
                                totalDefect = 1;
                            }
                        }
                        if (uticData.EQUIP_ID == _workcell.GraphAndMcMapping.Graph2McName)
                        {
                            _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastCCCOutputUTICtime =  uticData.EVENT_DATE;
                            _workcell.TICCccControl.CCCControlTicMc2.UpdateDefectCounter(CCCOutput.CCC_MC_Type.TIC);
                            _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TriggerFailureType = CCCOutput.MC_Trigger_Failure_Type.PART_FAIL;
                            // 16-Jun-2020 TIC Alert fix Last Time
                            if (_workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.IsTriggering)
                            {
                                HSTMachine.Workcell.CurretCCCActiveStatus.LastAlertActiveTime = _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastTICAlertActiveTime;
                            }
                            activeGraph2 = true;
                            totalDefect = chartTicMc2DefectChart.Series[1].Points.Count + 1;
                            totalAdaptiveDefect = Convert.ToDouble(_workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());

                            if (totalDefect > HSTSettings.Instance.Install.ANCGraphCounterMaximum)
                            {
                                chartTicMc2DefectChart.Series["PartTrigger"].Points.Clear();
                                chartTicMc2DefectChart.Series["DefectCounter"].Points.Clear();
                                totalDefect = 1;
                            }
                        }

                        label1Mc1Name.Text = _workcell.GraphAndMcMapping.Graph1McName;
                        label1Mc2Name.Text = _workcell.GraphAndMcMapping.Graph2McName;

                        if (activeGraph1)
                        {
                            // Mar-2020
                            //  chartTicDefectChart.Series["PartTrigger"].Points.AddXY(totalDefect, HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit);
                            int DefectCount = 0;
                            //Mar-2020 HSTMachine.Workcell.MC1YieldCounterCalculator.PartCounter.Count
                            if (_workcell.TICCccControl.CCCControlTicMc1.CccResult.CCCDataLogger.DEFECT_COUNTER == null)
                            {

                            }
                            else
                            {
                                DefectCount = int.Parse(_workcell.TICCccControl.CCCControlTicMc1.CccResult.CCCDataLogger.DEFECT_COUNTER);
                            }


                            chartTicDefectChart.Series["PartTrigger"].Points.AddXY(totalDefect, _workcell.TICCccControl.CCCControlTicMc1.CccResult.CCCDataLogger.PART_TRIGGER);

                            chartTicDefectChart.Series["DefectCounter"].Points.AddXY(totalDefect, DefectCount);
                            chartTicDefectChart.Series["DefectCounter"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                            chartTicDefectChart.ChartAreas[0].AxisY2.Maximum = 10;
                            chartTicDefectChart.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
                            //   labelMC1DefectCounter.Text = _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicFailCounter.ToString();

                            labelMC1DefectCounter.Text = DefectCount.ToString();
                            // labelMC1DefectCounter.Text = _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicDefactCounter.ToString();

                            labelMC1TriggerCounter.Text = _workcell.TICCccControl.CCCControlTicMc1.CccResult.AdaptiveNumber.ToString(); ;
                            labelMC1Yield.Text = Math.Round(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.current_yield, 2).ToString();
                            //HSTMachine.Workcell.MC1YieldCalculator.FailPartCounter++;
                            //HSTMachine.Workcell.AllMCYieldCalculator.FailPartCounter++;
                            //Mar 2020 -----------------------------------
                            _workcell.CCCFailureInfo.LastFailureMessage = uticData.ERROR_CODE;
                            _workcell.CCCFailureInfo.LastFailureStation = _workcell.GraphAndMcMapping.Graph1McName;
                            //--------------------------------------------
                            lblCurrentCount.Text = _workcell.TICCccControl.CCCControlTicMc1.CccResult.CCCDataLogger.CCC_TRIGGERING;
                            //MessageBox.Show(_workcell.TICCccControl.CCCControlAllMc.CccResult.CCCDataLogger.TIC_TRIGGERING_COUNT.ToString());
                            SaveLogDataToLogfile(_workcell.TICCccControl.CCCControlTicMc1.CccResult.CCCDataLogger, CCCFileLoger.Log_Triggering_type.Defect_Triggering, _workcell.GraphAndMcMapping.Graph1McName, uticData.ERROR_CODE);
                        }

                        if (activeGraph2)
                        {
                            int DefectMC2Count = 0;
                            //Mar-2020 HSTMachine.Workcell.MC1YieldCounterCalculator.PartCounter.Count
                            if (_workcell.TICCccControl.CCCControlTicMc2.CccResult.CCCDataLogger.DEFECT_COUNTER == null)
                            {

                            }
                            else
                            {
                                DefectMC2Count = int.Parse(_workcell.TICCccControl.CCCControlTicMc2.CccResult.CCCDataLogger.DEFECT_COUNTER);
                            }

                            //chartTicMc2DefectChart.Series["PartTrigger"].Points.AddXY(totalDefect, HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit);
                            chartTicMc2DefectChart.Series["PartTrigger"].Points.AddXY(totalDefect, _workcell.TICCccControl.CCCControlTicMc2.CccResult.CCCDataLogger.PART_TRIGGER);
                            chartTicMc2DefectChart.Series["DefectCounter"].Points.AddXY(totalDefect, DefectMC2Count);
                            chartTicMc2DefectChart.Series["DefectCounter"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                            chartTicMc2DefectChart.ChartAreas[0].AxisY2.Maximum = 10;
                            chartTicMc2DefectChart.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;

                            //labelMC2DefectCounter.Text = _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicFailCounter.ToString();
                            //       labelMC2DefectCounter.Text = _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicDefactCounter.ToString();
                            labelMC2DefectCounter.Text = DefectMC2Count.ToString();

                            labelMC2TriggerCounter.Text = _workcell.TICCccControl.CCCControlTicMc2.CccResult.AdaptiveNumber.ToString();
                            labelMC2Yield.Text = Math.Round(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.current_yield, 2).ToString();
                            //HSTMachine.Workcell.MC2YieldCalculator.FailPartCounter++;
                            //HSTMachine.Workcell.AllMCYieldCalculator.FailPartCounter++;

                            //Mar 2020 -----------------------------------
                            _workcell.CCCFailureInfo.LastFailureMessage = uticData.ERROR_CODE;
                            _workcell.CCCFailureInfo.LastFailureStation = _workcell.GraphAndMcMapping.Graph2McName;
                            //--------------------------------------------
                            lblCurrentCountMC2.Text = _workcell.TICCccControl.CCCControlTicMc2.CccResult.CCCDataLogger.CCC_TRIGGERING;

                            SaveLogDataToLogfile(_workcell.TICCccControl.CCCControlTicMc2.CccResult.CCCDataLogger, CCCFileLoger.Log_Triggering_type.Defect_Triggering, _workcell.GraphAndMcMapping.Graph2McName, uticData.ERROR_CODE);

                        }

                        var mCDownTriggering = _workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering +
                            _workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering;
                        _machineTriggerDownCounter = mCDownTriggering;
                        labelMachineTriggerCountTIC.Text = _machineTriggerDownCounter.ToString();


                    }
                    else if (args.TriggerType == CCCOutput.Trigger_Type.CRDL)
                    {

                        //Display all data
                        labelYieldSpecCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.YeildTarget.ToString("F2");
                        labelYieldLimitSpecCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.YeildLimit.ToString("F2");
                        labelAlphaSpecCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.Alpha.ToString("F3");
                        labelCCCCounterLimitSpecCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.DefectCounterLimit.ToString();
                        labelTestRunSpecCRDL.Text = HSTMachine.Workcell.HSTSettings.CccCRDLParameterSetting.TestRunGroup.ToString();

                        labelHighCrdlDefect.Text = dataItem.HighCrdlPercentTriggeringCounter.ToString();
                        labelCrdlTrigerCount.Text = dataItem.CrdlTriggeringCounter.ToString();

                        //Display chart
                        var index = Convert.ToInt32(dataItem.CrdlTriggeringCounter);
                        chartCRDLDefectChart.Series["CCC"].Points.AddXY(dataItem.CrdlTriggeringCounter, dataItem.GoodCounter);
                        chartCRDLDefectChart.Series["CCC"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

                        chartCRDLAdaptiveChart.Series["UCL"].Points.AddXY(index, dataItem.AdaptiveNumber);
                        chartCRDLAdaptiveChart.Series["DefectCounter"].Points.AddXY(dataItem.CrdlTriggeringCounter, dataItem.AdaptiveDefectCounter);
                        chartCRDLAdaptiveChart.Series["DefectCounter"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

                        //               SaveLogDataToLogfile(args.RunResult, CCCFileLoger.Log_Triggering_type.Defect_Triggering, string.Empty, String.Empty);
                    }


                }

            }

        }
        private void UpdateMcDownTriggering(object sender, CCCMcDownTriggerEvent args)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UpdateMcDownTriggering(sender, args)));
            }
            else
            {
                //Do something here after machine down trigger
                _workcell.CCCMachineTriggeringActivated = true;
                //Mar 2020

                var topMCFail = _workcell.TicMcFailureCounter.GetTopFailMC();
                _workcell.CCCFailureInfo.FailedMc = topMCFail;
                _workcell.CCCFailureInfo.FailedCode = _workcell.TicMcFailureCounter.GetTopCounterFailCodeByMC(topMCFail);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            chartTicDefectChart.Series["UCL"].Points.Clear();
            chartTicDefectChart.Series["LCL"].Points.Clear();
            chartTicDefectChart.Series["CL"].Points.Clear();
            chartTicDefectChart.Series["CCC"].Points.Clear();

            //chartTicMc2DefectChart
            chartTicMc2DefectChart.Series["UCL"].Points.Clear();
            chartTicMc2DefectChart.Series["LCL"].Points.Clear();
            chartTicMc2DefectChart.Series["CL"].Points.Clear();
            chartTicMc2DefectChart.Series["CCC"].Points.Clear();

        }

        private void SaveLogDataToLogfile(CCCDataLogger data, CCCFileLoger.Log_Triggering_type type, string TICNO, string err_code)
        {
            try
            {
                //                var fileName = string.Format("ANCRunResult-{0}{1}", System.DateTime.Today.ToString("MM-dd-yyyy"), CCCFileLoger.PosTDFName);
                DateTime todaytime = System.DateTime.Today;
                //todaytime.AddHours(-6);


                var fileName = string.Format("ANCRunResult-{0}{1}", todaytime.ToString("MM-dd-yyyy"), CCCFileLoger.PosTDFName);
                if (_workcell.CCCFileLoger == null)
                    _workcell.CCCFileLoger = new CCCFileLoger(_workcell.HSTSettings.Directory.ANCDataLogPath, fileName);

                if (_workcell.CCCFileLoger._logName != fileName)
                    _workcell.CCCFileLoger = new CCCFileLoger(_workcell.HSTSettings.Directory.ANCDataLogPath, fileName);

                string triggertype = string.Empty;

                if (data.MC_TRIGGER_TYPE == CCCOutput.Trigger_Type.TIC.ToString())
                    triggertype = TICNO;
                else
                    triggertype = data.MC_TRIGGER_TYPE;

                var logDetail = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23}",
                data.DateTime,
                data.TEST_RUN_COUNT,
                data.YIELD_TARGET,
                data.YIELD_LIMIT,
                data.DEFECT_LIMIT,
                data.LCL,
                data.PART_COUNT,
                data.PART_LIMIT_YIELD,
                data.PART_TRIGGER,
                data.DEFECT_COUNTER,
                data.CCC_TRIGGERING,
                data.TIC_TRIGGERING_COUNT,
                data.HIGH_CRDL_PERCENT_TRIGGERING_COUNT,
                data.CRDL_TRIGGERING_COUNT,
                data.ALL_MC_TRIGGER,
                triggertype,
                data.FAILURE_TYPE,
                err_code,
                data.PART_RUNNING,
                data.TotalParts,
                data.FailM1PartCounter,
                data.FailM2PartCounter,
                data.CurrentYield,
                data.uTICdate
                );

                _workcell.CCCFileLoger.LogLine(logDetail);
            }
            catch (Exception)
            {
            }

        }

        public void SaveLogDataIgnorePartToLogfile(string error_type, string err_code, string UTIC_NAME, DateTime TICTime)
        {
            try
            {
                //                var fileName = string.Format("ANCRunResult-{0}{1}", System.DateTime.Today.ToString("MM-dd-yyyy"), CCCFileLoger.PosTDFName);
                DateTime todaytime = System.DateTime.Today;
                //todaytime.AddHours(-6);


                var fileName = string.Format("ANCRunResult-{0}{1}", todaytime.ToString("MM-dd-yyyy"), CCCFileLoger.PosTDFName);
                if (_workcell.CCCFileLoger == null)
                    _workcell.CCCFileLoger = new CCCFileLoger(_workcell.HSTSettings.Directory.ANCDataLogPath, fileName);

                if (_workcell.CCCFileLoger._logName != fileName)
                    _workcell.CCCFileLoger = new CCCFileLoger(_workcell.HSTSettings.Directory.ANCDataLogPath, fileName);

                string triggertype = string.Empty;


                var logDetail = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23}",
                System.DateTime.Now,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                UTIC_NAME,
                error_type,
                err_code,
                0,
                0,
                0,
                0,
                0,
                TICTime.ToString()
                );

                _workcell.CCCFileLoger.LogLine(logDetail);
            }
            catch (Exception)
            {
            }

        }



        private void buttonTestData_Click_1(object sender, EventArgs e)
        {
            //   _workcell.HSTSettings.TicCCCCounter.CCCCounterAllMc.Reset();
            //   _workcell.HSTSettings.TicCCCCounter.CCCCounterTicMc1.Reset();
            //   _workcell.HSTSettings.TicCCCCounter.CCCCounterTicMc2.Reset();
            TestCCC2();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            chartCRDLDefectChart.Series["UCL"].Points.Clear();
            chartCRDLDefectChart.Series["LCL"].Points.Clear();
            chartCRDLDefectChart.Series["CL"].Points.Clear();
            chartCRDLDefectChart.Series["CCC"].Points.Clear();
            chartCRDLAdaptiveChart.Series["UCL"].Points.Clear();
            chartCRDLAdaptiveChart.Series["DefectCounter"].Points.Clear();
        }

        public void ClearChart()
        {
            chartTicDefectChart.Series["PartTrigger"].Points.Clear();
            chartTicDefectChart.Series["DefectCounter"].Points.Clear();

            chartTicMc2DefectChart.Series["PartTrigger"].Points.Clear();
            chartTicMc2DefectChart.Series["DefectCounter"].Points.Clear();

        }

        public void UpdateClearMachineTrigger()
        {
            labelMC1DefectCounter.Text = "0";
            labelMC2DefectCounter.Text = "0";
        }

        public void buttonClearChart_Click(object sender, EventArgs e)
        {
            ClearChart();
        }

        private void PanelData_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
                {
                    listViewDataLog.Columns[9].Text = "LDURes";
                }
                else
                {
                    listViewDataLog.Columns[9].Text = "Reader2Res";
                }

            }
        }

        private void labelMachineTriggerCountTIC_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            double findpct = double.Parse(txtPercentile.Text);

            txtZScore.Text = critz(findpct).ToString();
        }


        double Z_MAX = 6;
        double countA = 0;
        double countE = 0;
        List<double> SampleList = new List<double>();
        List<double> ResistanceList = new List<double>();

        public double critz(double p)
        {

            double Z_EPSILON = 0.000001;     /* Accuracy of z approximation */
            double minz = -Z_MAX;
            double maxz = Z_MAX;
            double zval = 0.0;
            double pval = 0;

            if (p < 0.0 || p > 1.0)
            {
                return -1;
            }

            while ((maxz - minz) > Z_EPSILON)
            {
                pval = poz(zval);
                if (pval > p)
                {
                    maxz = zval;
                }
                else
                {
                    minz = zval;
                }
                zval = (maxz + minz) * 0.5;
            }
            return (zval);
        }

        public double poz(double z)
        {
            double y = 0;
            double x = 0;
            double w = 0;

            if (z == 0.0)
            {
                x = 0.0;
            }
            else
            {
                y = 0.5 * Math.Abs(z);
                if (y > (Z_MAX * 0.5))
                {
                    x = 1.0;
                }
                else if (y < 1.0)
                {
                    w = y * y;
                    x = ((((((((0.000124818987 * w
                             - 0.001075204047) * w + 0.005198775019) * w
                             - 0.019198292004) * w + 0.059054035642) * w
                             - 0.151968751364) * w + 0.319152932694) * w
                             - 0.531923007300) * w + 0.797884560593) * y * 2.0;
                }
                else
                {
                    y -= 2.0;
                    x = (((((((((((((-0.000045255659 * y
                                   + 0.000152529290) * y - 0.000019538132) * y
                                   - 0.000676904986) * y + 0.001390604284) * y
                                   - 0.000794620820) * y - 0.002034254874) * y
                                   + 0.006549791214) * y - 0.010557625006) * y
                                   + 0.011630447319) * y - 0.009279453341) * y
                                   + 0.005353579108) * y - 0.002141268741) * y
                                   + 0.000535310849) * y + 0.999936657524;
                }
            }
            return z > 0.0 ? ((x + 1.0) * 0.5) : ((1.0 - x) * 0.5);
        }
        public double getStandardDeviation(List<double> doubleList)
        {
            double average = doubleList.Average();
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value - average) * (value - average);
            }
            // double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count - 1);
            double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count);
            //return Math.Sqrt(sumOfDerivationAverage - (average * average));
            double stddv = Math.Sqrt(sumOfDerivationAverage);
            return stddv;
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            char[] delimiterChars = { ',', ':', '\t' };
            string[] words = txtDeltaISIData.Text.Split(delimiterChars);
            ResistanceList = new List<double>();
            if (txtDeltaISIData.Text != "")
            {
                foreach (var word in words)
                {
                    //    //System.Console.WriteLine($"<{word}>");
                    if (word.Length > 0)
                    {
                        if (ResistanceList.Count() >= int.Parse(txtSampleMax.Text))
                        {
                            ResistanceList.RemoveRange(0, 1);
                            ResistanceList.Add(double.Parse(word.Trim()));
                        }
                        else
                        {
                            ResistanceList.Add(double.Parse(word.Trim()));
                        }
                    }
                    else
                    {
                        //  ResistanceList.Add(-9999);
                        //      txtOutput.Text += "Found Blank Data \t\r\n";
                    }

                }
            }
            else
            {

            }
            sampledatalist = ResistanceList;
            double resistancecrit = 0;
            // Console.WriteLine("Completed Load to Array : " + DateTime.Now);
            if (ResistanceList.Count() > 0)
            {
                txtMean.Text = sampledatalist.Average().ToString();
                txtStdDev.Text = getStandardDeviation(sampledatalist).ToString();
                txtCount.Text = sampledatalist.Count().ToString();
                txtHighest.Text = sampledatalist.Max().ToString();
                txtLowest.Text = sampledatalist.Min().ToString();
                resistancecrit = (Math.Floor(((double.Parse(txtZScore.Text) * (getStandardDeviation(sampledatalist))) + sampledatalist.Average()) * 100) / 100);
                txtZscoreValue.Text = (resistancecrit).ToString();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnGetSampleParam_Click(object sender, EventArgs e)
        {
            UpdateSampleData();
        }

        private void btnSaveSample_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.HSTSettings.Sampledata.Save();
        }

        private void btnSampleReset_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.HSTSettings.Sampledata.reset();
        }

        public void UpdateSampleData()
        {
            txtZscoreValue.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getCurrentZvalue.ToString();
            txtPercentile.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getCurrentZ.ToString();
            txtSampleMax.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getSampleSize.ToString();
            txtLowestAccept.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getLowest.ToString();
            txtHighestAccept.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getHeightest.ToString();
            txtCount.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getCount.ToString();
            txtLowest.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getLowest.ToString();
            txtHighest.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getHeightest.ToString();
            txtMean.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getMean.ToString();
            txtStdDev.Text = HSTMachine.Workcell.HSTSettings.Sampledata.getStdDev.ToString();
            List<double> sample;
            string sampletxt = "";
            sample = HSTMachine.Workcell.HSTSettings.Sampledata.DataList();
            if (sample.Count() > 0)
            {
                for (int i = 0; i < sample.Count() - 1; i++)
                {
                    sampletxt += sample[i].ToString() + ",";
                }
                sampletxt += sample[sample.Count() - 1].ToString();
                txtDeltaISIData.Text = sampletxt;


            }
            else
            {
                txtDeltaISIData.Text = "";
            }
            double findpct = double.Parse(txtPercentile.Text);

            txtZScore.Text = critz(findpct).ToString();

        }

        private void btnTest2_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
    }
}
