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
        private void btnBenchTestsSingleMeasure_Click(object sender, EventArgs e)
        {           
            if (String.Compare(cboBenchTestsUpDownTabSelection.Text, "UP", true) == 0)
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

        private void btnBenchTestsContinuousMeasure_Click(object sender, EventArgs e)
        {
            CommonFunctions.Instance.BenchTestsContinuous = true;           

            btnBenchTestsClearDisplay.Enabled = false;
            btnBenchTestsMultipleMeasure.Enabled = false;
            btnBenchTestsSingleMeasure.Enabled = false;            

            Task t = Task.Factory.StartNew(() =>
            {
                do
                {
                    if (String.Compare(cboBenchTestsUpDownTabSelection.Text, "UP", true) == 0)
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
                } while (CommonFunctions.Instance.BenchTestsContinuous);
            });

        }

        private void btnBenchTestsMultipleMeasure_Click(object sender, EventArgs e)
        {
            int MeasurementCount = int.Parse(txtBenchTestsMeasurementCount.Text);

            if(MeasurementCount <= 0)
            {
                Notify.PopUpError("Invalid Measurement Count Value", "Unable to start multiple measurement due to invalid value of measurement count entered.");
                return;
            }            

            if(String.IsNullOrEmpty(txtBenchTestsFileName.Text))
            {
                Notify.PopUpError("Missing Output File Name", "Unable to start multiple measurement due to no output file name entered.");
                return;
            }            

            string OQCTestReportFilePath = string.Format("{0}{1}.txt", CommonFunctions.Instance.MeasurementTestFileDirectory, txtBenchTestsFileName.Text);
            if (!Directory.Exists(CommonFunctions.Instance.MeasurementTestFileDirectory))
            {
                Directory.CreateDirectory(CommonFunctions.Instance.MeasurementTestFileDirectory);
            }

            if (File.Exists(OQCTestReportFilePath))
            {
                Notify.PopUpError("Output File Exists", String.Format("Unable to start multiple measurement due to{0}{1} exists in the system.",Environment.NewLine, OQCTestReportFilePath));
                return;
            }

            CommonFunctions.Instance.BenchTestsMultiple = true;

            using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath))
            {
                WriteToFile.WriteLine("                                      Measurement Tests");
                WriteToFile.WriteLine("");                
                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("Test #, HGA#, Short, , , , Resistance (Ω), , , , , Capacitance");
                WriteToFile.WriteLine(", , Detection, Ch1, Ch2, Ch3, Ch4, Ch5, Ch6, , C1, , C2");
                WriteToFile.WriteLine(", , , , , , , , , C (pF), ESR (mΩ), C (pF), ESR (mΩ)");
            }            

            Task t = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < MeasurementCount; i++)
                {
                    if(CommonFunctions.Instance.BenchTestsMultiple == false)
                    {                        
                        break;
                    }

                    if (String.Compare(cboBenchTestsUpDownTabSelection.Text, "UP", true) == 0)
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

                    UIUtility.BeginInvoke(this, () =>
                    {
                        txtBenchTestsCountingDown.Text = (MeasurementCount - i).ToString();
                    });

                    DataReceived = false;
                    while (!DataReceived)
                    {
                        Thread.Sleep(100);
                    }                    

                    using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath, true))
                    {
                        WriteToFile.WriteLine("{0}, 1, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                            (i + 1), txtBenchTestsHGA1Short.Text, txtBenchTestsHGA1Ch1Res.Text, txtBenchTestsHGA1Ch2Res.Text, txtBenchTestsHGA1Ch3Res.Text, txtBenchTestsHGA1Ch4Res.Text,
                            txtBenchTestsHGA1Ch5Res.Text, txtBenchTestsHGA1Ch6Res.Text, txtBenchTestsHGA1Ch1Capa.Text, txtBenchTestsHGA1Ch1ESR.Text, txtBenchTestsHGA1Ch2Capa.Text, txtBenchTestsHGA1Ch2ESR.Text);
                        WriteToFile.WriteLine(", 2, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA2Short.Text, txtBenchTestsHGA2Ch1Res.Text, txtBenchTestsHGA2Ch2Res.Text, txtBenchTestsHGA2Ch3Res.Text, txtBenchTestsHGA2Ch4Res.Text,
                            txtBenchTestsHGA2Ch5Res.Text, txtBenchTestsHGA2Ch6Res.Text, txtBenchTestsHGA2Ch1Capa.Text, txtBenchTestsHGA2Ch1ESR.Text, txtBenchTestsHGA2Ch2Capa.Text, txtBenchTestsHGA2Ch2ESR.Text);
                        WriteToFile.WriteLine(", 3, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA3Short.Text, txtBenchTestsHGA3Ch1Res.Text, txtBenchTestsHGA3Ch2Res.Text, txtBenchTestsHGA3Ch3Res.Text, txtBenchTestsHGA3Ch4Res.Text,
                            txtBenchTestsHGA3Ch5Res.Text, txtBenchTestsHGA3Ch6Res.Text, txtBenchTestsHGA3Ch1Capa.Text, txtBenchTestsHGA3Ch1ESR.Text, txtBenchTestsHGA3Ch2Capa.Text, txtBenchTestsHGA3Ch2ESR.Text);
                        WriteToFile.WriteLine(", 4, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA4Short.Text, txtBenchTestsHGA4Ch1Res.Text, txtBenchTestsHGA4Ch2Res.Text, txtBenchTestsHGA4Ch3Res.Text, txtBenchTestsHGA4Ch4Res.Text,
                            txtBenchTestsHGA4Ch5Res.Text, txtBenchTestsHGA4Ch6Res.Text, txtBenchTestsHGA4Ch1Capa.Text, txtBenchTestsHGA4Ch1ESR.Text, txtBenchTestsHGA4Ch2Capa.Text, txtBenchTestsHGA4Ch2ESR.Text);
                        WriteToFile.WriteLine(", 5, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA5Short.Text, txtBenchTestsHGA5Ch1Res.Text, txtBenchTestsHGA5Ch2Res.Text, txtBenchTestsHGA5Ch3Res.Text, txtBenchTestsHGA5Ch4Res.Text,
                            txtBenchTestsHGA5Ch5Res.Text, txtBenchTestsHGA5Ch6Res.Text, txtBenchTestsHGA5Ch1Capa.Text, txtBenchTestsHGA5Ch1ESR.Text, txtBenchTestsHGA5Ch2Capa.Text, txtBenchTestsHGA5Ch2ESR.Text);
                        WriteToFile.WriteLine(", 5, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA6Short.Text, txtBenchTestsHGA6Ch1Res.Text, txtBenchTestsHGA6Ch2Res.Text, txtBenchTestsHGA6Ch3Res.Text, txtBenchTestsHGA6Ch4Res.Text,
                            txtBenchTestsHGA6Ch5Res.Text, txtBenchTestsHGA6Ch6Res.Text, txtBenchTestsHGA6Ch1Capa.Text, txtBenchTestsHGA6Ch1ESR.Text, txtBenchTestsHGA6Ch2Capa.Text, txtBenchTestsHGA6Ch2ESR.Text);
                        WriteToFile.WriteLine(", 7, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA7Short.Text, txtBenchTestsHGA7Ch1Res.Text, txtBenchTestsHGA7Ch2Res.Text, txtBenchTestsHGA7Ch3Res.Text, txtBenchTestsHGA7Ch4Res.Text,
                            txtBenchTestsHGA7Ch5Res.Text, txtBenchTestsHGA7Ch6Res.Text, txtBenchTestsHGA7Ch1Capa.Text, txtBenchTestsHGA7Ch1ESR.Text, txtBenchTestsHGA7Ch2Capa.Text, txtBenchTestsHGA7Ch2ESR.Text);
                        WriteToFile.WriteLine(", 8, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA8Short.Text, txtBenchTestsHGA8Ch1Res.Text, txtBenchTestsHGA8Ch2Res.Text, txtBenchTestsHGA8Ch3Res.Text, txtBenchTestsHGA8Ch4Res.Text,
                            txtBenchTestsHGA8Ch5Res.Text, txtBenchTestsHGA8Ch6Res.Text, txtBenchTestsHGA8Ch1Capa.Text, txtBenchTestsHGA8Ch1ESR.Text, txtBenchTestsHGA8Ch2Capa.Text, txtBenchTestsHGA8Ch2ESR.Text);
                        WriteToFile.WriteLine(", 9, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA9Short.Text, txtBenchTestsHGA9Ch1Res.Text, txtBenchTestsHGA9Ch2Res.Text, txtBenchTestsHGA9Ch3Res.Text, txtBenchTestsHGA9Ch4Res.Text,
                            txtBenchTestsHGA9Ch5Res.Text, txtBenchTestsHGA9Ch6Res.Text, txtBenchTestsHGA9Ch1Capa.Text, txtBenchTestsHGA9Ch1ESR.Text, txtBenchTestsHGA9Ch2Capa.Text, txtBenchTestsHGA9Ch2ESR.Text);
                        WriteToFile.WriteLine(", 10, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                            txtBenchTestsHGA10Short.Text, txtBenchTestsHGA10Ch1Res.Text, txtBenchTestsHGA10Ch2Res.Text, txtBenchTestsHGA10Ch3Res.Text, txtBenchTestsHGA10Ch4Res.Text,
                            txtBenchTestsHGA10Ch5Res.Text, txtBenchTestsHGA10Ch6Res.Text, txtBenchTestsHGA10Ch1Capa.Text, txtBenchTestsHGA10Ch1ESR.Text, txtBenchTestsHGA10Ch2Capa.Text, txtBenchTestsHGA10Ch2ESR.Text);
                        WriteToFile.WriteLine("");
                        WriteToFile.WriteLine(""); 
                    }
                }

                if (CommonFunctions.Instance.BenchTestsMultiple == false)
                {
                    Notify.PopUp("Multiple Measurements Aborted", "Could not complete multiple measurements due to user interruption.", "", "OK");                    
                }

                CommonFunctions.Instance.BenchTestsMultiple = false;

                UIUtility.BeginInvoke(this, () =>
                {
                    txtBenchTestsCountingDown.Text = "0";
                });               
            });           
        }

        private void btnBenchTestsStopMeasure_Click(object sender, EventArgs e)
        {
            CommonFunctions.Instance.BenchTestsContinuous = false;
            CommonFunctions.Instance.BenchTestsMultiple = false;

            btnBenchTestsClearDisplay.Enabled = true;
            btnBenchTestsMultipleMeasure.Enabled = true;
            btnBenchTestsSingleMeasure.Enabled = true;
            btnBenchTestsContinuousMeasure.Enabled = true;
        }

        private void btnBenchTestsClearDisplay_Click(object sender, EventArgs e)
        {
            txtBenchTestsHGA1Short.Text = "";
            txtBenchTestsHGA1Ch1Res.Text = "";
            txtBenchTestsHGA1Ch2Res.Text = "";
            txtBenchTestsHGA1Ch3Res.Text = "";
            txtBenchTestsHGA1Ch4Res.Text = "";
            txtBenchTestsHGA1Ch5Res.Text = "";
            txtBenchTestsHGA1Ch6Res.Text = "";
            txtBenchTestsHGA1Ch1Capa.Text = "";
            txtBenchTestsHGA1Ch1ESR.Text = "";
            txtBenchTestsHGA1Ch2Capa.Text = "";
            txtBenchTestsHGA1Ch2ESR.Text = "";

            txtBenchTestsHGA2Short.Text = "";
            txtBenchTestsHGA2Ch1Res.Text = "";
            txtBenchTestsHGA2Ch2Res.Text = "";
            txtBenchTestsHGA2Ch3Res.Text = "";
            txtBenchTestsHGA2Ch4Res.Text = "";
            txtBenchTestsHGA2Ch5Res.Text = "";
            txtBenchTestsHGA2Ch6Res.Text = "";
            txtBenchTestsHGA2Ch1Capa.Text = "";
            txtBenchTestsHGA2Ch1ESR.Text = "";
            txtBenchTestsHGA2Ch2Capa.Text = "";
            txtBenchTestsHGA2Ch2ESR.Text = "";

            txtBenchTestsHGA3Short.Text = "";
            txtBenchTestsHGA3Ch1Res.Text = "";
            txtBenchTestsHGA3Ch2Res.Text = "";
            txtBenchTestsHGA3Ch3Res.Text = "";
            txtBenchTestsHGA3Ch4Res.Text = "";
            txtBenchTestsHGA3Ch5Res.Text = "";
            txtBenchTestsHGA3Ch6Res.Text = "";
            txtBenchTestsHGA3Ch1Capa.Text = "";
            txtBenchTestsHGA3Ch1ESR.Text = "";
            txtBenchTestsHGA3Ch2Capa.Text = "";
            txtBenchTestsHGA3Ch2ESR.Text = "";

            txtBenchTestsHGA4Short.Text = "";
            txtBenchTestsHGA4Ch1Res.Text = "";
            txtBenchTestsHGA4Ch2Res.Text = "";
            txtBenchTestsHGA4Ch3Res.Text = "";
            txtBenchTestsHGA4Ch4Res.Text = "";
            txtBenchTestsHGA4Ch5Res.Text = "";
            txtBenchTestsHGA4Ch6Res.Text = "";
            txtBenchTestsHGA4Ch1Capa.Text = "";
            txtBenchTestsHGA4Ch1ESR.Text = "";
            txtBenchTestsHGA4Ch2Capa.Text = "";
            txtBenchTestsHGA4Ch2ESR.Text = "";

            txtBenchTestsHGA5Short.Text = "";
            txtBenchTestsHGA5Ch1Res.Text = "";
            txtBenchTestsHGA5Ch2Res.Text = "";
            txtBenchTestsHGA5Ch3Res.Text = "";
            txtBenchTestsHGA5Ch4Res.Text = "";
            txtBenchTestsHGA5Ch5Res.Text = "";
            txtBenchTestsHGA5Ch6Res.Text = "";
            txtBenchTestsHGA5Ch1Capa.Text = "";
            txtBenchTestsHGA5Ch1ESR.Text = "";
            txtBenchTestsHGA5Ch2Capa.Text = "";
            txtBenchTestsHGA5Ch2ESR.Text = "";

            txtBenchTestsHGA6Short.Text = "";
            txtBenchTestsHGA6Ch1Res.Text = "";
            txtBenchTestsHGA6Ch2Res.Text = "";
            txtBenchTestsHGA6Ch3Res.Text = "";
            txtBenchTestsHGA6Ch4Res.Text = "";
            txtBenchTestsHGA6Ch5Res.Text = "";
            txtBenchTestsHGA6Ch6Res.Text = "";
            txtBenchTestsHGA6Ch1Capa.Text = "";
            txtBenchTestsHGA6Ch1ESR.Text = "";
            txtBenchTestsHGA6Ch2Capa.Text = "";
            txtBenchTestsHGA6Ch2ESR.Text = "";

            txtBenchTestsHGA7Short.Text = "";
            txtBenchTestsHGA7Ch1Res.Text = "";
            txtBenchTestsHGA7Ch2Res.Text = "";
            txtBenchTestsHGA7Ch3Res.Text = "";
            txtBenchTestsHGA7Ch4Res.Text = "";
            txtBenchTestsHGA7Ch5Res.Text = "";
            txtBenchTestsHGA7Ch6Res.Text = "";
            txtBenchTestsHGA7Ch1Capa.Text = "";
            txtBenchTestsHGA7Ch1ESR.Text = "";
            txtBenchTestsHGA7Ch2Capa.Text = "";
            txtBenchTestsHGA7Ch2ESR.Text = "";

            txtBenchTestsHGA8Short.Text = "";
            txtBenchTestsHGA8Ch1Res.Text = "";
            txtBenchTestsHGA8Ch2Res.Text = "";
            txtBenchTestsHGA8Ch3Res.Text = "";
            txtBenchTestsHGA8Ch4Res.Text = "";
            txtBenchTestsHGA8Ch5Res.Text = "";
            txtBenchTestsHGA8Ch6Res.Text = "";
            txtBenchTestsHGA8Ch1Capa.Text = "";
            txtBenchTestsHGA8Ch1ESR.Text = "";
            txtBenchTestsHGA8Ch2Capa.Text = "";
            txtBenchTestsHGA8Ch2ESR.Text = "";

            txtBenchTestsHGA9Short.Text = "";
            txtBenchTestsHGA9Ch1Res.Text = "";
            txtBenchTestsHGA9Ch2Res.Text = "";
            txtBenchTestsHGA9Ch3Res.Text = "";
            txtBenchTestsHGA9Ch4Res.Text = "";
            txtBenchTestsHGA9Ch5Res.Text = "";
            txtBenchTestsHGA9Ch6Res.Text = "";
            txtBenchTestsHGA9Ch1Capa.Text = "";
            txtBenchTestsHGA9Ch1ESR.Text = "";
            txtBenchTestsHGA9Ch2Capa.Text = "";
            txtBenchTestsHGA9Ch2ESR.Text = "";

            txtBenchTestsHGA10Short.Text = "";
            txtBenchTestsHGA10Ch1Res.Text = "";
            txtBenchTestsHGA10Ch2Res.Text = "";
            txtBenchTestsHGA10Ch3Res.Text = "";
            txtBenchTestsHGA10Ch4Res.Text = "";
            txtBenchTestsHGA10Ch5Res.Text = "";
            txtBenchTestsHGA10Ch6Res.Text = "";
            txtBenchTestsHGA10Ch1Capa.Text = "";
            txtBenchTestsHGA10Ch1ESR.Text = "";
            txtBenchTestsHGA10Ch2Capa.Text = "";
            txtBenchTestsHGA10Ch2ESR.Text = "";
        }
	}
}