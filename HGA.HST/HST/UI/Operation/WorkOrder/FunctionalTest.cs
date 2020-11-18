using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;
using System.Globalization;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.UI;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Settings;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace Seagate.AAS.HGA.HST.UI.Operation.WorkOrder
{
    public partial class FunctionalTest : UserControl
    {
        private bool _tests0RestResultPass = false;
        private bool _tests10RestResultPass = false;
        private bool _tests100RestResultPass = false;
        private bool _tests500RestResultPass = false;
        private bool _tests1000RestResultPass = false;
        private bool _tests10000RestResultPass = false;
        private bool _testProcessDone = false;

        public FunctionalTest()
        {
            InitializeComponent();
        }

        public void btnFunctionalTestsStartSelfTests_Click(object sender, EventArgs e)
        {
            //clear all current result
            btnFunctionalTestsClearDisplay_Click(this, new EventArgs());

            _testProcessDone = false;
            btnFunctionalTestsStartSelfTests.Enabled = false;
            btnFunctionalTestsStartSelfTests.Refresh();
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe9SetStartMeasurement);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_self_test_Message_ID, TestProbeAPICommand.HST_start_self_test_Message_Name, TestProbeAPICommand.HST_start_self_test_Message_Size);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);

        }

        private void txtFunctionalTests0RestResults_TextChanged(object sender, EventArgs e)
        {
            if (txtFunctionalTests0RestResults.Text == "PASS")
            {
                _tests0RestResultPass = true;
                txtFunctionalTests0RestResults.ForeColor = Color.Lime;
            }
            else if(txtFunctionalTests0RestResults.Text == "FAIL")
            {
                _tests0RestResultPass = false;
                txtFunctionalTests0RestResults.ForeColor = Color.Red;
            }
            else
                txtFunctionalTests0RestResults.ForeColor = Color.Black;
        }

        private void txtFunctionalTests10RestResults_TextChanged(object sender, EventArgs e)
        {
            if (txtFunctionalTests10RestResults.Text == "PASS")
            {
                _tests10RestResultPass = true;
                txtFunctionalTests10RestResults.ForeColor = Color.Lime;
            }
            else if (txtFunctionalTests10RestResults.Text == "FAIL")
            {
                _tests10RestResultPass = false;
                txtFunctionalTests10RestResults.ForeColor = Color.Red;
            }
            else
                txtFunctionalTests10RestResults.ForeColor = Color.Black;
        }

        private void txtFunctionalTests100RestResults_TextChanged(object sender, EventArgs e)
        {
            if (txtFunctionalTests100RestResults.Text == "PASS")
            {
                _tests100RestResultPass = true;
                txtFunctionalTests100RestResults.ForeColor = Color.Lime;
            }
            else if (txtFunctionalTests100RestResults.Text == "FAIL")
            {
                _tests100RestResultPass = false;
                txtFunctionalTests100RestResults.ForeColor = Color.Red;
            }
            else
                txtFunctionalTests100RestResults.ForeColor = Color.Black;
        }

        private void txtFunctionalTests500RestResults_TextChanged(object sender, EventArgs e)
        {
            if (txtFunctionalTests500RestResults.Text == "PASS")
            {
                _tests500RestResultPass = true;
                txtFunctionalTests500RestResults.ForeColor = Color.Lime;
            }
            else if (txtFunctionalTests500RestResults.Text == "FAIL")
            {
                _tests500RestResultPass = false;
                txtFunctionalTests500RestResults.ForeColor = Color.Red;
            }
            else
                txtFunctionalTests500RestResults.ForeColor = Color.Black;

        }

        private void txtFunctionalTests1000RestResults_TextChanged(object sender, EventArgs e)
        {
            if (txtFunctionalTests1000RestResults.Text == "PASS")
            {
                _tests1000RestResultPass = true;
                txtFunctionalTests1000RestResults.ForeColor = Color.Lime;
            }
            else if (txtFunctionalTests1000RestResults.Text == "FAIL")
            {
                _tests1000RestResultPass = false;
                txtFunctionalTests1000RestResults.ForeColor = Color.Red;
            }
            else
                txtFunctionalTests1000RestResults.ForeColor = Color.Black;
        }

        private void txtFunctionalTests10000RestResults_TextChanged(object sender, EventArgs e)
        {
            if (txtFunctionalTests10000RestResults.Text == "PASS")
            {
                _tests10000RestResultPass = true;
                txtFunctionalTests10000RestResults.ForeColor = Color.Lime;

            }
            else if (txtFunctionalTests10000RestResults.Text == "FAIL")
            {
                _tests10000RestResultPass = false;
                txtFunctionalTests10000RestResults.ForeColor = Color.Red;
            }
            else
                txtFunctionalTests10000RestResults.ForeColor = Color.Black;

            if (txtFunctionalTests10000RestResults.Text != string.Empty)
            {
                _testProcessDone = true;
                btnFunctionalTestsStartSelfTests.Enabled = true;
                btnFunctionalTestsStartSelfTests.Refresh();
                btnFunctionalTestsSaveTestRecord_Click(this, null);
            }

        }


        public bool GetFunctionalTestResult()
        {
            var ret = _tests0RestResultPass && _tests10RestResultPass && _tests100RestResultPass && _tests500RestResultPass &&
            _tests1000RestResultPass && _tests10000RestResultPass;

            return ret;
        }

        public void btnFunctionalTestsClearDisplay_Click(object sender, EventArgs e)
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
        }

        private void btnFunctionalTestsSaveTestRecord_Click(object sender, EventArgs e)
        {
            try
            {
                var filename = string.Format("{0}{1}{2}", "FunctionTest-", DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2"), ".txt");
                string OQCTestReportFilePath = string.Format("{0}\\{1}", HSTSettings.Instance.Directory.DataPath, filename);

                using (StreamWriter WriteToFile = new StreamWriter(OQCTestReportFilePath, true))
                {
                    WriteToFile.WriteLine("                                OQC Functional Test Report");
                    WriteToFile.WriteLine("");
                    WriteToFile.WriteLine("Test Date: {0}", DateTime.Now.ToShortDateString());
                    WriteToFile.WriteLine("Test Time: {0}", DateTime.Now.ToShortTimeString());
                    WriteToFile.WriteLine("Product ID: {0}", CommonFunctions.Instance.MeasurementTestRecipe.ProductID.ToString());

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
                    WriteToFile.WriteLine("===========================================================");

                }
            }
            catch (Exception)
            {

            }
        }
    }
}
