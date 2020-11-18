using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;
using System.Globalization;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using DesktopTester.Data;
using DesktopTester.Data.IncomingTestProbeData;
using DesktopTester.Data.OutgoingTestProbeData;
using DesktopTester.Utils;
using System.Threading.Tasks;

namespace DesktopTester.UI
{
    public partial class frmMain
    {
        private void tabPageDesktopTests_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            
            System.Drawing.SolidBrush myDesktopTesterConnectionStatusBrush;
            if (IsConnectedToDesktopTester == true)
            {
                myDesktopTesterConnectionStatusBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
                enableAllButtonsOnDesktopTestsPage();
            }
            else
            {
                myDesktopTesterConnectionStatusBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                disableAllButtonsOnDesktopTestsPage();
            }
            System.Drawing.Graphics tabPageDesktopTestsConnectionStatusGraphics = tabPageDesktopTests.CreateGraphics();
            tabPageDesktopTestsConnectionStatusGraphics.FillEllipse(myDesktopTesterConnectionStatusBrush, new Rectangle(260, 358, 40, 40));
            myDesktopTesterConnectionStatusBrush.Dispose();
            tabPageDesktopTestsConnectionStatusGraphics.Dispose();
        }

        private void disableAllButtonsOnDesktopTestsPage()
        {
            btnDesktopTestsClearDisplay.Enabled = false;
            btnDesktopTestsConnectDesktopTester.Enabled = true;
            btnDesktopTestsContinuousMeasure.Enabled = false;
            btnDesktopTestsDisconnectDesktopTester.Enabled = false;
            btnDesktopTestsMultipleMeasure.Enabled = false;
            btnDesktopTestsSingleMeasure.Enabled = false;
            btnDesktopTestsStopMeasure.Enabled = false;
            btnDesktopTestsAllMoveToStandbyPosition.Enabled = false;
            btnDesktopTestsHomeAll.Enabled = false;
            btnDesktopTestsServoOff.Enabled = false;
            btnDesktopTestsEjectHGAs.Enabled = false;
        }

        private void enableAllButtonsOnDesktopTestsPage()
        {
            btnDesktopTestsClearDisplay.Enabled = true;
            btnDesktopTestsConnectDesktopTester.Enabled = true;
            btnDesktopTestsContinuousMeasure.Enabled = true;
            btnDesktopTestsDisconnectDesktopTester.Enabled = true;
            btnDesktopTestsMultipleMeasure.Enabled = true;
            btnDesktopTestsSingleMeasure.Enabled = true;
            btnDesktopTestsStopMeasure.Enabled = true;
            btnDesktopTestsAllMoveToStandbyPosition.Enabled = true;
            btnDesktopTestsHomeAll.Enabled = true;
            btnDesktopTestsServoOff.Enabled = true;
            btnDesktopTestsEjectHGAs.Enabled = true;
        }

        private void btnDesktopTestsDisconnectDesktopTester_Click(object sender, EventArgs e)
        {           
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_disconnect_Message_ID, TestProbeAPICommand.HST_desktop_tester_disconnect_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDesktopTestsConnectDesktopTester_Click(object sender, EventArgs e)
        {            
            TestProbe80SetDesktopTesterConnect.PasswordLSB = 0xA5;
            TestProbe80SetDesktopTesterConnect.PasswordMSB = 0xA5;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe80SetDesktopTesterConnect);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_connect_Message_ID, TestProbeAPICommand.HST_desktop_tester_connect_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer(); 
        }

        private void btnDesktopTestsClearDisplay_Click(object sender, EventArgs e)
        {
            txtDesktopTestsHGA1Short.Text = "";
            txtDesktopTestsHGA1Ch1Res.Text = "";
            txtDesktopTestsHGA1Ch2Res.Text = "";
            txtDesktopTestsHGA1Ch3Res.Text = "";
            txtDesktopTestsHGA1Ch4Res.Text = "";
            txtDesktopTestsHGA1Ch5Res.Text = "";
            txtDesktopTestsHGA1Ch6Res.Text = "";
            txtDesktopTestsHGA1Ch1Capa.Text = "";
            txtDesktopTestsHGA1Ch1ESR.Text = "";
            txtDesktopTestsHGA1Ch2Capa.Text = "";
            txtDesktopTestsHGA1Ch2ESR.Text = "";

            txtDesktopTestsHGA2Short.Text = "";
            txtDesktopTestsHGA2Ch1Res.Text = "";
            txtDesktopTestsHGA2Ch2Res.Text = "";
            txtDesktopTestsHGA2Ch3Res.Text = "";
            txtDesktopTestsHGA2Ch4Res.Text = "";
            txtDesktopTestsHGA2Ch5Res.Text = "";
            txtDesktopTestsHGA2Ch6Res.Text = "";
            txtDesktopTestsHGA2Ch1Capa.Text = "";
            txtDesktopTestsHGA2Ch1ESR.Text = "";
            txtDesktopTestsHGA2Ch2Capa.Text = "";
            txtDesktopTestsHGA2Ch2ESR.Text = "";

            txtDesktopTestsHGA3Short.Text = "";
            txtDesktopTestsHGA3Ch1Res.Text = "";
            txtDesktopTestsHGA3Ch2Res.Text = "";
            txtDesktopTestsHGA3Ch3Res.Text = "";
            txtDesktopTestsHGA3Ch4Res.Text = "";
            txtDesktopTestsHGA3Ch5Res.Text = "";
            txtDesktopTestsHGA3Ch6Res.Text = "";
            txtDesktopTestsHGA3Ch1Capa.Text = "";
            txtDesktopTestsHGA3Ch1ESR.Text = "";
            txtDesktopTestsHGA3Ch2Capa.Text = "";
            txtDesktopTestsHGA3Ch2ESR.Text = "";

            txtDesktopTestsHGA4Short.Text = "";
            txtDesktopTestsHGA4Ch1Res.Text = "";
            txtDesktopTestsHGA4Ch2Res.Text = "";
            txtDesktopTestsHGA4Ch3Res.Text = "";
            txtDesktopTestsHGA4Ch4Res.Text = "";
            txtDesktopTestsHGA4Ch5Res.Text = "";
            txtDesktopTestsHGA4Ch6Res.Text = "";
            txtDesktopTestsHGA4Ch1Capa.Text = "";
            txtDesktopTestsHGA4Ch1ESR.Text = "";
            txtDesktopTestsHGA4Ch2Capa.Text = "";
            txtDesktopTestsHGA4Ch2ESR.Text = "";

            txtDesktopTestsHGA5Short.Text = "";
            txtDesktopTestsHGA5Ch1Res.Text = "";
            txtDesktopTestsHGA5Ch2Res.Text = "";
            txtDesktopTestsHGA5Ch3Res.Text = "";
            txtDesktopTestsHGA5Ch4Res.Text = "";
            txtDesktopTestsHGA5Ch5Res.Text = "";
            txtDesktopTestsHGA5Ch6Res.Text = "";
            txtDesktopTestsHGA5Ch1Capa.Text = "";
            txtDesktopTestsHGA5Ch1ESR.Text = "";
            txtDesktopTestsHGA5Ch2Capa.Text = "";
            txtDesktopTestsHGA5Ch2ESR.Text = "";

            txtDesktopTestsHGA6Short.Text = "";
            txtDesktopTestsHGA6Ch1Res.Text = "";
            txtDesktopTestsHGA6Ch2Res.Text = "";
            txtDesktopTestsHGA6Ch3Res.Text = "";
            txtDesktopTestsHGA6Ch4Res.Text = "";
            txtDesktopTestsHGA6Ch5Res.Text = "";
            txtDesktopTestsHGA6Ch6Res.Text = "";
            txtDesktopTestsHGA6Ch1Capa.Text = "";
            txtDesktopTestsHGA6Ch1ESR.Text = "";
            txtDesktopTestsHGA6Ch2Capa.Text = "";
            txtDesktopTestsHGA6Ch2ESR.Text = "";

            txtDesktopTestsHGA7Short.Text = "";
            txtDesktopTestsHGA7Ch1Res.Text = "";
            txtDesktopTestsHGA7Ch2Res.Text = "";
            txtDesktopTestsHGA7Ch3Res.Text = "";
            txtDesktopTestsHGA7Ch4Res.Text = "";
            txtDesktopTestsHGA7Ch5Res.Text = "";
            txtDesktopTestsHGA7Ch6Res.Text = "";
            txtDesktopTestsHGA7Ch1Capa.Text = "";
            txtDesktopTestsHGA7Ch1ESR.Text = "";
            txtDesktopTestsHGA7Ch2Capa.Text = "";
            txtDesktopTestsHGA7Ch2ESR.Text = "";

            txtDesktopTestsHGA8Short.Text = "";
            txtDesktopTestsHGA8Ch1Res.Text = "";
            txtDesktopTestsHGA8Ch2Res.Text = "";
            txtDesktopTestsHGA8Ch3Res.Text = "";
            txtDesktopTestsHGA8Ch4Res.Text = "";
            txtDesktopTestsHGA8Ch5Res.Text = "";
            txtDesktopTestsHGA8Ch6Res.Text = "";
            txtDesktopTestsHGA8Ch1Capa.Text = "";
            txtDesktopTestsHGA8Ch1ESR.Text = "";
            txtDesktopTestsHGA8Ch2Capa.Text = "";
            txtDesktopTestsHGA8Ch2ESR.Text = "";

            txtDesktopTestsHGA9Short.Text = "";
            txtDesktopTestsHGA9Ch1Res.Text = "";
            txtDesktopTestsHGA9Ch2Res.Text = "";
            txtDesktopTestsHGA9Ch3Res.Text = "";
            txtDesktopTestsHGA9Ch4Res.Text = "";
            txtDesktopTestsHGA9Ch5Res.Text = "";
            txtDesktopTestsHGA9Ch6Res.Text = "";
            txtDesktopTestsHGA9Ch1Capa.Text = "";
            txtDesktopTestsHGA9Ch1ESR.Text = "";
            txtDesktopTestsHGA9Ch2Capa.Text = "";
            txtDesktopTestsHGA9Ch2ESR.Text = "";

            txtDesktopTestsHGA10Short.Text = "";
            txtDesktopTestsHGA10Ch1Res.Text = "";
            txtDesktopTestsHGA10Ch2Res.Text = "";
            txtDesktopTestsHGA10Ch3Res.Text = "";
            txtDesktopTestsHGA10Ch4Res.Text = "";
            txtDesktopTestsHGA10Ch5Res.Text = "";
            txtDesktopTestsHGA10Ch6Res.Text = "";
            txtDesktopTestsHGA10Ch1Capa.Text = "";
            txtDesktopTestsHGA10Ch1ESR.Text = "";
            txtDesktopTestsHGA10Ch2Capa.Text = "";
            txtDesktopTestsHGA10Ch2ESR.Text = "";
        }

        private void btnDesktopTestsStopMeasure_Click(object sender, EventArgs e)
        {
            CommonFunctions.Instance.DesktopTestsContinuous = false;
            CommonFunctions.Instance.DesktopTestsMultiple = false;

            btnDesktopTestsClearDisplay.Enabled = true;
            btnDesktopTestsMultipleMeasure.Enabled = true;
            btnDesktopTestsSingleMeasure.Enabled = true;
            btnDesktopTestsContinuousMeasure.Enabled = true;
        }

        private void btnDesktopTestsMultipleMeasure_Click(object sender, EventArgs e)
        {
            int MeasurementCount = int.Parse(txtDesktopTestsMeasurementCount.Text);
            int CycleCount = int.Parse(txtDesktopTestsCycleCount.Text);

            if (CycleCount < 0)
            {
                Notify.PopUpError("Invalid Cycle Count Value", "Unable to start multiple measurement due to invalid value of cycle count entered.");
                return;
            }

            if (MeasurementCount < 0)
            {
                Notify.PopUpError("Invalid Measurement Count Value", "Unable to start multiple measurement due to invalid value of measurement count entered.");
                return;
            }

            if (CycleCount == 0 && MeasurementCount == 0)
            {
                Notify.PopUpError("Unable to Run Test", "Unable to start multiple measurement due to both cycle count and measurement count being 0.");
                return;
            }

            if (String.IsNullOrEmpty(txtDesktopTestsFileName.Text))
            {
                Notify.PopUpError("Missing Output File Name", "Unable to start multiple measurement due to no output file name entered.");
                return;
            }            

            string OQCTestReportFilePath = string.Format("{0}{1}.txt", CommonFunctions.Instance.MeasurementTestFileDirectory, txtDesktopTestsFileName.Text);
            if (!Directory.Exists(CommonFunctions.Instance.MeasurementTestFileDirectory))
            {
                Directory.CreateDirectory(CommonFunctions.Instance.MeasurementTestFileDirectory);
            }

            if (File.Exists(OQCTestReportFilePath))
            {
                using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath, true))
                {
                    WriteToFile.WriteLine("");
                    WriteToFile.WriteLine("");
                    WriteToFile.WriteLine("**********************************************************************************************************************************");
                    WriteToFile.WriteLine("");
                    WriteToFile.WriteLine("");
                }
            }

            CommonFunctions.Instance.DesktopTestsMultiple = true;

            using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath, true))
            {
                WriteToFile.WriteLine("                                      Measurement Tests");
                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("");
            }

            if (CycleCount == 0 && MeasurementCount != 0)
            {
                // No Precisor Nest movement 
            }            

            Task t = Task.Factory.StartNew(() =>
            {
                for (int j = 0; j < CycleCount; j++)
                {
                    if (String.Compare(cboDesktopTestsUpDownTabSelection.Text, "UP", true) == 0)
                    {
                        TestProbe91SetDesktopLoadHGA.UpDownTabIndex = 1;
                    }
                    else
                    {
                        TestProbe91SetDesktopLoadHGA.UpDownTabIndex = 2;
                    }
                    byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe91SetDesktopLoadHGA);
                    TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_load_hga_Message_ID, TestProbeAPICommand.HST_desktop_tester_load_hga_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                    HGAsLoaded = false;
                    while (!HGAsLoaded)
                    {
                        Thread.Sleep(100);
                    }  

                    using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath, true))
                    {
                        WriteToFile.WriteLine("");
                        WriteToFile.WriteLine("Cycle {0}", (j+1));
                        WriteToFile.WriteLine("=========");
                        WriteToFile.WriteLine("Cycle #, Test #, HGA#, Short, , , , Resistance (Ω), , , , , Capacitance");
                        WriteToFile.WriteLine(", , , Detection, Ch1, Ch2, Ch3, Ch4, Ch5, Ch6, , C1, , C2");
                        WriteToFile.WriteLine(", , , , , , , , , , C (pF), ESR (mΩ), C (pF), ESR (mΩ)");
                    }

                    UIUtility.BeginInvoke(this, () =>
                    {
                        txtDesktopTestsCycleCountingDown.Text = (CycleCount - j).ToString();
                        txtDesktopTestsMeasurementCountingDown.Text = MeasurementCount.ToString();
                    });

                    
                    for (int i = 0; i < MeasurementCount; i++)
                    {
                        if (CommonFunctions.Instance.DesktopTestsMultiple == false)
                        {
                            break;
                        }

                        if (String.Compare(cboDesktopTestsUpDownTabSelection.Text, "UP", true) == 0)
                        {
                            TestProbe9SetStartMeasurement.FlexCableIndex = 1;
                        }
                        else
                        {
                            TestProbe9SetStartMeasurement.FlexCableIndex = 2;
                        }

                        ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe9SetStartMeasurement);
                        APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_meas_Message_ID, TestProbeAPICommand.HST_start_meas_Message_Size, ByteArrayFromStructure);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                        APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_Message_ID, TestProbeAPICommand.HST_get_short_detection_Message_Size, null);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                        APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                        APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_results_Message_ID, TestProbeAPICommand.HST_get_cap_results_Message_Size, null);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                        APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_secondary_results_Message_ID, TestProbeAPICommand.HST_get_cap_secondary_results_Message_Size, null);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                        commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                        UIUtility.BeginInvoke(this, () =>
                        {
                            txtDesktopTestsMeasurementCountingDown.Text = (MeasurementCount - i).ToString();
                        });

                        DataReceived = false;
                        while (!DataReceived)
                        {
                            Thread.Sleep(100);
                        }

                        using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath, true))
                        {
                            WriteToFile.WriteLine("{0}, {1}, 1, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}",
                            (j + 1), (i + 1), txtDesktopTestsHGA1Short.Text, txtDesktopTestsHGA1Ch1Res.Text, txtDesktopTestsHGA1Ch2Res.Text, txtDesktopTestsHGA1Ch3Res.Text, txtDesktopTestsHGA1Ch4Res.Text,
                            txtDesktopTestsHGA1Ch5Res.Text, txtDesktopTestsHGA1Ch6Res.Text, txtDesktopTestsHGA1Ch1Capa.Text, txtDesktopTestsHGA1Ch1ESR.Text, txtDesktopTestsHGA1Ch2Capa.Text, txtDesktopTestsHGA1Ch2ESR.Text);
                            WriteToFile.WriteLine(", 2, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA2Short.Text, txtDesktopTestsHGA2Ch1Res.Text, txtDesktopTestsHGA2Ch2Res.Text, txtDesktopTestsHGA2Ch3Res.Text, txtDesktopTestsHGA2Ch4Res.Text,
                                txtDesktopTestsHGA2Ch5Res.Text, txtDesktopTestsHGA2Ch6Res.Text, txtDesktopTestsHGA2Ch1Capa.Text, txtDesktopTestsHGA2Ch1ESR.Text, txtDesktopTestsHGA2Ch2Capa.Text, txtDesktopTestsHGA2Ch2ESR.Text);
                            WriteToFile.WriteLine(", 3, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA3Short.Text, txtDesktopTestsHGA3Ch1Res.Text, txtDesktopTestsHGA3Ch2Res.Text, txtDesktopTestsHGA3Ch3Res.Text, txtDesktopTestsHGA3Ch4Res.Text,
                                txtDesktopTestsHGA3Ch5Res.Text, txtDesktopTestsHGA3Ch6Res.Text, txtDesktopTestsHGA3Ch1Capa.Text, txtDesktopTestsHGA3Ch1ESR.Text, txtDesktopTestsHGA3Ch2Capa.Text, txtDesktopTestsHGA3Ch2ESR.Text);
                            WriteToFile.WriteLine(", 4, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA4Short.Text, txtDesktopTestsHGA4Ch1Res.Text, txtDesktopTestsHGA4Ch2Res.Text, txtDesktopTestsHGA4Ch3Res.Text, txtDesktopTestsHGA4Ch4Res.Text,
                                txtDesktopTestsHGA4Ch5Res.Text, txtDesktopTestsHGA4Ch6Res.Text, txtDesktopTestsHGA4Ch1Capa.Text, txtDesktopTestsHGA4Ch1ESR.Text, txtDesktopTestsHGA4Ch2Capa.Text, txtDesktopTestsHGA4Ch2ESR.Text);
                            WriteToFile.WriteLine(", 5, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA5Short.Text, txtDesktopTestsHGA5Ch1Res.Text, txtDesktopTestsHGA5Ch2Res.Text, txtDesktopTestsHGA5Ch3Res.Text, txtDesktopTestsHGA5Ch4Res.Text,
                                txtDesktopTestsHGA5Ch5Res.Text, txtDesktopTestsHGA5Ch6Res.Text, txtDesktopTestsHGA5Ch1Capa.Text, txtDesktopTestsHGA5Ch1ESR.Text, txtDesktopTestsHGA5Ch2Capa.Text, txtDesktopTestsHGA5Ch2ESR.Text);
                            WriteToFile.WriteLine(", 5, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA6Short.Text, txtDesktopTestsHGA6Ch1Res.Text, txtDesktopTestsHGA6Ch2Res.Text, txtDesktopTestsHGA6Ch3Res.Text, txtDesktopTestsHGA6Ch4Res.Text,
                                txtDesktopTestsHGA6Ch5Res.Text, txtDesktopTestsHGA6Ch6Res.Text, txtDesktopTestsHGA6Ch1Capa.Text, txtDesktopTestsHGA6Ch1ESR.Text, txtDesktopTestsHGA6Ch2Capa.Text, txtDesktopTestsHGA6Ch2ESR.Text);
                            WriteToFile.WriteLine(", 7, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA7Short.Text, txtDesktopTestsHGA7Ch1Res.Text, txtDesktopTestsHGA7Ch2Res.Text, txtDesktopTestsHGA7Ch3Res.Text, txtDesktopTestsHGA7Ch4Res.Text,
                                txtDesktopTestsHGA7Ch5Res.Text, txtDesktopTestsHGA7Ch6Res.Text, txtDesktopTestsHGA7Ch1Capa.Text, txtDesktopTestsHGA7Ch1ESR.Text, txtDesktopTestsHGA7Ch2Capa.Text, txtDesktopTestsHGA7Ch2ESR.Text);
                            WriteToFile.WriteLine(", 8, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA8Short.Text, txtDesktopTestsHGA8Ch1Res.Text, txtDesktopTestsHGA8Ch2Res.Text, txtDesktopTestsHGA8Ch3Res.Text, txtDesktopTestsHGA8Ch4Res.Text,
                                txtDesktopTestsHGA8Ch5Res.Text, txtDesktopTestsHGA8Ch6Res.Text, txtDesktopTestsHGA8Ch1Capa.Text, txtDesktopTestsHGA8Ch1ESR.Text, txtDesktopTestsHGA8Ch2Capa.Text, txtDesktopTestsHGA8Ch2ESR.Text);
                            WriteToFile.WriteLine(", 9, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA9Short.Text, txtDesktopTestsHGA9Ch1Res.Text, txtDesktopTestsHGA9Ch2Res.Text, txtDesktopTestsHGA9Ch3Res.Text, txtDesktopTestsHGA9Ch4Res.Text,
                                txtDesktopTestsHGA9Ch5Res.Text, txtDesktopTestsHGA9Ch6Res.Text, txtDesktopTestsHGA9Ch1Capa.Text, txtDesktopTestsHGA9Ch1ESR.Text, txtDesktopTestsHGA9Ch2Capa.Text, txtDesktopTestsHGA9Ch2ESR.Text);
                            WriteToFile.WriteLine(", 10, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                                txtDesktopTestsHGA10Short.Text, txtDesktopTestsHGA10Ch1Res.Text, txtDesktopTestsHGA10Ch2Res.Text, txtDesktopTestsHGA10Ch3Res.Text, txtDesktopTestsHGA10Ch4Res.Text,
                                txtDesktopTestsHGA10Ch5Res.Text, txtDesktopTestsHGA10Ch6Res.Text, txtDesktopTestsHGA10Ch1Capa.Text, txtDesktopTestsHGA10Ch1ESR.Text, txtDesktopTestsHGA10Ch2Capa.Text, txtDesktopTestsHGA10Ch2ESR.Text);
                            WriteToFile.WriteLine("");
                            WriteToFile.WriteLine(""); 
                        }
                    }

                    UIUtility.BeginInvoke(this, () =>
                    {
                        txtDesktopTestsMeasurementCountingDown.Text = "0";
                    });                    

                    TestProbeAPICommand EjectHGAsAPICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_eject_hga_Message_ID, TestProbeAPICommand.HST_desktop_tester_eject_hga_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(EjectHGAsAPICommand);

                    bool EjectHGAsCommandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                    HGAsEjected = false;
                    while (!HGAsEjected)
                    {
                        Thread.Sleep(100);
                    }

                    Thread.Sleep(1000);
                }
                
                if (CommonFunctions.Instance.DesktopTestsMultiple == false)
                {
                    Notify.PopUp("Multiple Measurements Aborted", "Could not complete multiple measurements due to user interruption.", "", "OK");
                }

                CommonFunctions.Instance.DesktopTestsMultiple = false;

                UIUtility.BeginInvoke(this, () =>
                {
                    txtDesktopTestsCycleCountingDown.Text = "0";
                    txtDesktopTestsMeasurementCountingDown.Text = "0";
                });
            });
        }

        private void btnDesktopTestsSingleMeasure_Click(object sender, EventArgs e)
        {           
            if (String.Compare(cboDesktopTestsUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe9SetStartMeasurement.FlexCableIndex = 1;
            }
            else
            {
                TestProbe9SetStartMeasurement.FlexCableIndex = 2;
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe9SetStartMeasurement);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_meas_Message_ID, TestProbeAPICommand.HST_start_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_Message_ID, TestProbeAPICommand.HST_get_short_detection_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_results_Message_ID, TestProbeAPICommand.HST_get_cap_results_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_secondary_results_Message_ID, TestProbeAPICommand.HST_get_cap_secondary_results_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();    
        }

        private void btnDesktopTestsContinuousMeasure_Click(object sender, EventArgs e)
        {
            CommonFunctions.Instance.DesktopTestsContinuous = true;
            
            btnDesktopTestsClearDisplay.Enabled = false;
            btnDesktopTestsMultipleMeasure.Enabled = false;
            btnDesktopTestsSingleMeasure.Enabled = false;

            Task t = Task.Factory.StartNew(() =>
            {
                do
                {
                    if (String.Compare(cboDesktopTestsUpDownTabSelection.Text, "UP", true) == 0)
                    {
                        TestProbe9SetStartMeasurement.FlexCableIndex = 1;
                    }
                    else
                    {
                        TestProbe9SetStartMeasurement.FlexCableIndex = 2;
                    }

                    byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe9SetStartMeasurement);
                    TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_meas_Message_ID, TestProbeAPICommand.HST_start_meas_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_Message_ID, TestProbeAPICommand.HST_get_short_detection_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_results_Message_ID, TestProbeAPICommand.HST_get_cap_results_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_secondary_results_Message_ID, TestProbeAPICommand.HST_get_cap_secondary_results_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                    Thread.Sleep(CommonFunctions.Instance.DelayInBetweenCommandBatch);
                } while (CommonFunctions.Instance.DesktopTestsContinuous);
            });
        }
		
		private void btnDesktopTestsAllMoveToStandbyPosition_Click(object sender, EventArgs e)
        {            
            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 10;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDesktopTestsHomeAll_Click(object sender, EventArgs e)
        {            
            TestProbe81SetDesktopTesterHomingAxis.ServoAxis = 255;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe81SetDesktopTesterHomingAxis);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDesktopTestsServoOff_Click(object sender, EventArgs e)
        {           
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_servo_off_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_servo_off_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDesktopTestsEjectHGAs_Click(object sender, EventArgs e)
        {           
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_eject_hga_Message_ID, TestProbeAPICommand.HST_desktop_tester_eject_hga_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }        
	}
}