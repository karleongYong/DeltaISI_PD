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
using BenchTestsTool.Data;
using BenchTestsTool.Data.IncomingTestProbeData;
using BenchTestsTool.Data.OutgoingTestProbeData;
using BenchTestsTool.Utils;

namespace BenchTestsTool.UI
{
    public partial class frmMain
    {
        private void btnFunctionalTestsClearDisplay_Click(object sender, EventArgs e)
        {
            txtFunctionalTests0CH1.Text = "";
            txtFunctionalTests0CH2.Text = "";
            txtFunctionalTests0CH3.Text = "";
            txtFunctionalTests0CH4.Text = "";
            txtFunctionalTests0CH5.Text = "";
            txtFunctionalTests0CH6.Text = "";
            txtFunctionalTests0RestResults.Text = "";

            txtFunctionalTests10CH1.Text = "";
            txtFunctionalTests10CH2.Text = "";
            txtFunctionalTests10CH3.Text = "";
            txtFunctionalTests10CH4.Text = "";
            txtFunctionalTests10CH5.Text = "";
            txtFunctionalTests10CH6.Text = "";
            txtFunctionalTests10RestResults.Text = "";

            txtFunctionalTests100CH1.Text = "";
            txtFunctionalTests100CH2.Text = "";
            txtFunctionalTests100CH3.Text = "";
            txtFunctionalTests100CH4.Text = "";
            txtFunctionalTests100CH5.Text = "";
            txtFunctionalTests100CH6.Text = "";
            txtFunctionalTests100RestResults.Text = "";

            txtFunctionalTests500CH1.Text = "";
            txtFunctionalTests500CH2.Text = "";
            txtFunctionalTests500CH3.Text = "";
            txtFunctionalTests500CH4.Text = "";
            txtFunctionalTests500CH5.Text = "";
            txtFunctionalTests500CH6.Text = "";
            txtFunctionalTests500RestResults.Text = "";

            txtFunctionalTests1000CH1.Text = "";
            txtFunctionalTests1000CH2.Text = "";
            txtFunctionalTests1000CH3.Text = "";
            txtFunctionalTests1000CH4.Text = "";
            txtFunctionalTests1000CH5.Text = "";
            txtFunctionalTests1000CH6.Text = "";
            txtFunctionalTests1000RestResults.Text = "";

            txtFunctionalTests10000CH1.Text = "";
            txtFunctionalTests10000CH2.Text = "";
            txtFunctionalTests10000CH3.Text = "";
            txtFunctionalTests10000CH4.Text = "";
            txtFunctionalTests10000CH5.Text = "";
            txtFunctionalTests10000CH6.Text = "";
            txtFunctionalTests10000RestResults.Text = "";

            txtFunctionalTests100Capa.Text = "";
            txtFunctionalTests100CapaResults.Text = "";

            txtFunctionalTests270Capa.Text = "";
            txtFunctionalTests270CapaResults.Text = "";

            txtFunctionalTests470Capa.Text = "";
            txtFunctionalTests470CapaResults.Text = "";

            txtFunctionalTests680Capa.Text = "";
            txtFunctionalTests680CapaResults.Text = "";

            txtFunctionalTests820Capa.Text = "";
            txtFunctionalTests820CapaResults.Text = "";

            txtFunctionalTests10000Capa.Text = "";
            txtFunctionalTests10000CapaResults.Text = "";

            txtFunctionalTests0Temp.Text = "";
            txtFunctionalTests0TempResults.Text = "";

            txtFunctionalTests50Temp.Text = "";
            txtFunctionalTests50TempResults.Text = "";

            txtFunctionalTests100Temp.Text = "";
            txtFunctionalTests100TempResults.Text = "";
        }

        private void btnFunctionalTestsStartSelfTests_Click(object sender, EventArgs e)
        {
            disableAllAPIButtons();

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_self_test_Message_ID, TestProbeAPICommand.HST_start_self_test_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            //TestProbe33SetTemperatureRead         
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_temperature_Message_ID, TestProbeAPICommand.HST_get_temperature_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);                      

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            enableAllAPIButtons();
        }

        /*private void btnFunctionalTestsRecipeEditor_Click(object sender, EventArgs e)
        {
            _editRecipeForm.ShowDialog();
        }*/   

        private void btnFunctionalTestsSaveTestRecord_Click(object sender, EventArgs e)
        {                        
            if (String.IsNullOrEmpty(txtFunctionalTestsProductSerial.Text))
            {
                Notify.PopUpError("Missing Product Serial Number", "Unable to save test record due to no product serial number entered.");
                return;
            }

            if (String.IsNullOrEmpty(txtFunctionalTestsEngineerName.Text))
            {
                Notify.PopUpError("Missing Engineer Name", "Unable to save test record due to no engineer name entered.");
                return;
            }

            if (String.IsNullOrEmpty(txtFunctionalTests0RestResults.Text) && String.IsNullOrEmpty(txtFunctionalTests100CapaResults.Text))
            {
                Notify.PopUpError("No Test Results Present", "You must first 'Start Self Tests' before attempting to save test record.");
                return;
            }
            
            string OQCTestReportFilePath = string.Format("{0}OQCTestReport.rpt", CommonFunctions.Instance.MeasurementTestFileDirectory);
            if (!Directory.Exists(CommonFunctions.Instance.MeasurementTestFileDirectory))
            {
                Directory.CreateDirectory(CommonFunctions.Instance.MeasurementTestFileDirectory);
            }

            using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath))
            {
                WriteToFile.WriteLine("                                      OQC Test Report");
                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("Test Date: {0}", DateTime.Now.ToShortDateString());
                WriteToFile.WriteLine("Engineer Name: {0}", txtFunctionalTestsEngineerName.Text);
                WriteToFile.WriteLine("Product Serial Number: {0}", txtFunctionalTestsProductSerial.Text);
                WriteToFile.WriteLine("Product ID: {0}", CommonFunctions.Instance.strProductID.ToString());

                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("    , , , RESISTANCE (Ω), , , RESULTS");
                WriteToFile.WriteLine("    , CH1, CH2, CH3, CH4, CH5, CH6");
                WriteToFile.WriteLine("  0Ω, {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                    txtFunctionalTests0CH1.Text, txtFunctionalTests0CH2.Text, txtFunctionalTests0CH3.Text, txtFunctionalTests0CH4.Text, txtFunctionalTests0CH5.Text, txtFunctionalTests0CH6.Text, txtFunctionalTests0RestResults.Text);
                WriteToFile.WriteLine(" 10Ω, {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                    txtFunctionalTests10CH1.Text, txtFunctionalTests10CH2.Text, txtFunctionalTests10CH3.Text, txtFunctionalTests10CH4.Text, txtFunctionalTests10CH5.Text, txtFunctionalTests10CH6.Text, txtFunctionalTests10RestResults.Text);
                WriteToFile.WriteLine("100Ω, {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                    txtFunctionalTests100CH1.Text, txtFunctionalTests100CH2.Text, txtFunctionalTests100CH3.Text, txtFunctionalTests100CH4.Text, txtFunctionalTests100CH5.Text, txtFunctionalTests100CH6.Text, txtFunctionalTests100RestResults.Text);
                WriteToFile.WriteLine("500Ω, {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                   txtFunctionalTests500CH1.Text, txtFunctionalTests500CH2.Text, txtFunctionalTests500CH3.Text, txtFunctionalTests500CH4.Text, txtFunctionalTests500CH5.Text, txtFunctionalTests500CH6.Text, txtFunctionalTests500RestResults.Text);
                WriteToFile.WriteLine(" 1kΩ, {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                   txtFunctionalTests1000CH1.Text, txtFunctionalTests1000CH2.Text, txtFunctionalTests1000CH3.Text, txtFunctionalTests1000CH4.Text, txtFunctionalTests1000CH5.Text, txtFunctionalTests1000CH6.Text, txtFunctionalTests1000RestResults.Text);
                WriteToFile.WriteLine("10kΩ, {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                   txtFunctionalTests10000CH1.Text, txtFunctionalTests10000CH2.Text, txtFunctionalTests10000CH3.Text, txtFunctionalTests10000CH4.Text, txtFunctionalTests10000CH5.Text, txtFunctionalTests10000CH6.Text, txtFunctionalTests10000RestResults.Text);

                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("     , CAPACITANCE (pF), RESULTS");
                WriteToFile.WriteLine("100pF, {0}, {1}", txtFunctionalTests100Capa.Text, txtFunctionalTests100CapaResults.Text);
                WriteToFile.WriteLine("270pF, {0}, {1}", txtFunctionalTests270Capa.Text, txtFunctionalTests270CapaResults.Text);
                WriteToFile.WriteLine("470pF, {0}, {1}", txtFunctionalTests470Capa.Text, txtFunctionalTests470CapaResults.Text);
                WriteToFile.WriteLine("680pF, {0}, {1}", txtFunctionalTests680Capa.Text, txtFunctionalTests680CapaResults.Text);
                WriteToFile.WriteLine("820pF, {0}, {1}", txtFunctionalTests820Capa.Text, txtFunctionalTests820CapaResults.Text);
                WriteToFile.WriteLine(" 10nF, {0}, {1}", txtFunctionalTests10000Capa.Text, txtFunctionalTests10000CapaResults.Text);

                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("");
                WriteToFile.WriteLine("           , TEMPERATURE (°C), RESULTS");
                WriteToFile.WriteLine("  CH1 (0°C), {0}, {1}", txtFunctionalTests0Temp.Text, txtFunctionalTests0TempResults.Text);
                WriteToFile.WriteLine(" CH2 (50°C), {0}, {1}", txtFunctionalTests50Temp.Text, txtFunctionalTests50TempResults.Text);
                WriteToFile.WriteLine("CH3 (100°C), {0}, {1}", txtFunctionalTests100Temp.Text, txtFunctionalTests100TempResults.Text);               
            }

            Notify.PopUp("OQC Test Report Created", String.Format("Test record was saved to{0}{1}.", Environment.NewLine, OQCTestReportFilePath), "", "OK");
            Log.Info("OQC Test Report Created", String.Format("Test record was saved to{0}.", OQCTestReportFilePath)); 
        }        
	}
}