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

namespace DesktopTester.UI
{
    public partial class frmMain
    {
        private void tabPageCableCalibration_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.SolidBrush myBrush;
            if (IsCableCalibrationTempered == true)
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            }
            else
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            }

            System.Drawing.Graphics tabPageCableCalibrationGraphics = tabPageCableCalibration.CreateGraphics();
            tabPageCableCalibrationGraphics.FillEllipse(myBrush, new Rectangle(865, 300, 40, 40));
            myBrush.Dispose();
            tabPageCableCalibrationGraphics.Dispose();
        }
        
        private void btnCableCalibrationStartCableCalibration_Click(object sender, EventArgs e)
        {
            if (chkCableCalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "Command not executed because the 'Calibration Enable' check box is not checked.");
                return;
            }              

            ClearAllDisplayedValues();

            if (String.Compare(cboCableCalibrationUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe43SetFlexCableCalibration.FlexCableIndex = 1;
            }
            else
            {
                TestProbe43SetFlexCableCalibration.FlexCableIndex = 2;
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe43SetFlexCableCalibration);            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_flex_cable_calibration_Message_ID, TestProbeAPICommand.HST_flex_cable_calibration_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            CableCalibrationDataReceived = false;
            while (!CableCalibrationDataReceived)
            {
                Thread.Sleep(100);
            }                    

            btnCableCalibrationGetCableCalibrationData_Click(sender, e);            

            if (commandSentToMicroprocessor)
            {
                IsCableCalibrationTempered = true;
                tabPageCableCalibration.Refresh();
            }
        }
                
        private void btnCableCalibrationGetCableCalibrationData_Click(object sender, EventArgs e)
        {           
            if (String.Compare(cboCableCalibrationUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe44SetGetCableCalibrationResistanceResults.FlexCableIndex = 1;
                TestProbe51SetGetCableCalibrationCapacitanceResults.FlexCableIndex = 1;
            }
            else
            {
                TestProbe44SetGetCableCalibrationResistanceResults.FlexCableIndex = 2;
                TestProbe51SetGetCableCalibrationCapacitanceResults.FlexCableIndex = 2;
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe44SetGetCableCalibrationResistanceResults);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cable_calibration_res_results_Message_ID, TestProbeAPICommand.HST_get_cable_calibration_res_results_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe51SetGetCableCalibrationCapacitanceResults);
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cable_calibration_cap_results_Message_ID, TestProbeAPICommand.HST_get_cable_calibration_cap_results_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if (commandSentToMicroprocessor)
            {
                IsCableCalibrationTempered = false;
                tabPageCableCalibration.Refresh();
            }
        }

        private void btnClearCableCalibrationResults_Click(object sender, EventArgs e)
        {            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_clear_all_cable_compensation_Message_ID, TestProbeAPICommand.HST_clear_all_cable_compensation_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();            

            if (commandSentToMicroprocessor)
            {
                IsCableCalibrationTempered = false;
                tabPageCableCalibration.Refresh();
            }
        }

        private void btnCableCalibrationSaveCalibrationData_Click(object sender, EventArgs e)
        {
            if(chkCableCalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "You need to first enable calibration before attempting to save the calibration data.");
                return;
            }
            btnPCBACalibrationSaveCalibrationData_Click(sender, e);

            IsCableCalibrationTempered = false;
            tabPageCableCalibration.Refresh();
        }

        private void btnCableCalibrationSetCableCompensationResistanceCapacitance_Click(object sender, EventArgs e)
        {            
            // TestProbe45SetCableCompensation
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtCableCalibrationCableResistanceCapacitance.Text) ? 0 : int.Parse(txtCableCalibrationCableResistanceCapacitance.Text));
            TestProbe45SetCableCompensation.CompensationResistanceCapacitanceLSBLow = ByteArray[0];
            TestProbe45SetCableCompensation.CompensationResistanceCapacitanceLSBMid = ByteArray[1];
            TestProbe45SetCableCompensation.CompensationResistanceCapacitanceMSBLow = ByteArray[2];
            TestProbe45SetCableCompensation.CompensationResistanceCapacitanceMSBHigh = ByteArray[3];

            TestProbe45SetCableCompensation.HGAIndex = Convert.ToByte(int.Parse(cboCableCalibrationHGAIndex.Text));
            TestProbe45SetCableCompensation.ChannelNumber = Convert.ToByte(int.Parse(cboCableCalibrationChannelNumber.Text));

            if (String.Compare(cboCableCalibrationUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe45SetCableCompensation.FlexCableIndex = 1;
            }
            else
            {
                TestProbe45SetCableCompensation.FlexCableIndex = 2;
            }

            if (String.Compare(cboCableCalibrationResistanceOrCapacitanceSelection.Text, "RESISTANCE", true) == 0)
            {
                TestProbe45SetCableCompensation.ResistanceOrCapacitanceSelection = 1;
            }
            else
            {
                TestProbe45SetCableCompensation.ResistanceOrCapacitanceSelection = 2;
            }
            
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe45SetCableCompensation);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_cable_compensation_Message_ID, TestProbeAPICommand.HST_set_cable_compensation_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();            

            if (commandSentToMicroprocessor)
            {
                IsCableCalibrationTempered = true;
                tabPageCableCalibration.Refresh();
            }
        }        

        private void txtCableCalibrationCableResistanceCapacitance_TextChanged(object sender, EventArgs e)
        {
            IsCableCalibrationTempered = true;
            tabPageCableCalibration.Refresh();
        }

        private void cboCableCalibrationChannelNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsCableCalibrationTempered = true;
            tabPageCableCalibration.Refresh();
        }

        private void cboCableCalibrationHGAIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsCableCalibrationTempered = true;
            tabPageCableCalibration.Refresh();
        }

        private void btnCableCalibrationSetShortDetectionThreshold_Click(object sender, EventArgs e)
        {            
            // TestProbe47SetShortDetectionThreshold
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtCableCalibrationShortDetectionThreshold.Text) ? 0 : int.Parse(txtCableCalibrationShortDetectionThreshold.Text));
            TestProbe47SetShortDetectionThreshold.ThresholdVoltageLSBLow = ByteArray[0];
            TestProbe47SetShortDetectionThreshold.ThresholdVoltageLSBMid = ByteArray[1];
            TestProbe47SetShortDetectionThreshold.ThresholdVoltageMSBLow = ByteArray[2];
            TestProbe47SetShortDetectionThreshold.ThresholdVoltageMSBHigh = ByteArray[3];


            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe47SetShortDetectionThreshold);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_short_detection_threshold_Message_ID, TestProbeAPICommand.HST_set_short_detection_threshold_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();            

            if (commandSentToMicroprocessor)
            {
                IsCableCalibrationTempered = true;
                tabPageCableCalibration.Refresh();
            }
        }

        private void btnCableCalibrationGetShortDetectionThreshold_Click(object sender, EventArgs e)
        {            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_threshold_Message_ID, TestProbeAPICommand.HST_get_short_detection_threshold_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();            
        } 
     
        private void ClearAllDisplayedValues()
        {
            txtCableCalibrationHGA1Ch1.Text = "";
            txtCableCalibrationHGA1Ch2.Text = "";
            txtCableCalibrationHGA1Ch3.Text = "";
            txtCableCalibrationHGA1Ch4.Text = "";
            txtCableCalibrationHGA1Ch5.Text = "";
            txtCableCalibrationHGA1Ch6.Text = "";
            txtCableCalibrationHGA1Ch1Capa.Text = "";
            txtCableCalibrationHGA1Ch2Capa.Text = "";

            txtCableCalibrationHGA2Ch1.Text = "";
            txtCableCalibrationHGA2Ch2.Text = "";
            txtCableCalibrationHGA2Ch3.Text = "";
            txtCableCalibrationHGA2Ch4.Text = "";
            txtCableCalibrationHGA2Ch5.Text = "";
            txtCableCalibrationHGA2Ch6.Text = "";
            txtCableCalibrationHGA2Ch1Capa.Text = "";
            txtCableCalibrationHGA2Ch2Capa.Text = "";

            txtCableCalibrationHGA3Ch1.Text = "";
            txtCableCalibrationHGA3Ch2.Text = "";
            txtCableCalibrationHGA3Ch3.Text = "";
            txtCableCalibrationHGA3Ch4.Text = "";
            txtCableCalibrationHGA3Ch5.Text = "";
            txtCableCalibrationHGA3Ch6.Text = "";
            txtCableCalibrationHGA3Ch1Capa.Text = "";
            txtCableCalibrationHGA3Ch2Capa.Text = "";

            txtCableCalibrationHGA4Ch1.Text = "";
            txtCableCalibrationHGA4Ch2.Text = "";
            txtCableCalibrationHGA4Ch3.Text = "";
            txtCableCalibrationHGA4Ch4.Text = "";
            txtCableCalibrationHGA4Ch5.Text = "";
            txtCableCalibrationHGA4Ch6.Text = "";
            txtCableCalibrationHGA4Ch1Capa.Text = "";
            txtCableCalibrationHGA4Ch2Capa.Text = "";

            txtCableCalibrationHGA5Ch1.Text = "";
            txtCableCalibrationHGA5Ch2.Text = "";
            txtCableCalibrationHGA5Ch3.Text = "";
            txtCableCalibrationHGA5Ch4.Text = "";
            txtCableCalibrationHGA5Ch5.Text = "";
            txtCableCalibrationHGA5Ch6.Text = "";
            txtCableCalibrationHGA5Ch1Capa.Text = "";
            txtCableCalibrationHGA5Ch2Capa.Text = "";

            txtCableCalibrationHGA6Ch1.Text = "";
            txtCableCalibrationHGA6Ch2.Text = "";
            txtCableCalibrationHGA6Ch3.Text = "";
            txtCableCalibrationHGA6Ch4.Text = "";
            txtCableCalibrationHGA6Ch5.Text = "";
            txtCableCalibrationHGA6Ch6.Text = "";
            txtCableCalibrationHGA6Ch1Capa.Text = "";
            txtCableCalibrationHGA6Ch2Capa.Text = "";

            txtCableCalibrationHGA7Ch1.Text = "";
            txtCableCalibrationHGA7Ch2.Text = "";
            txtCableCalibrationHGA7Ch3.Text = "";
            txtCableCalibrationHGA7Ch4.Text = "";
            txtCableCalibrationHGA7Ch5.Text = "";
            txtCableCalibrationHGA7Ch6.Text = "";
            txtCableCalibrationHGA7Ch1Capa.Text = "";
            txtCableCalibrationHGA7Ch2Capa.Text = "";

            txtCableCalibrationHGA8Ch1.Text = "";
            txtCableCalibrationHGA8Ch2.Text = "";
            txtCableCalibrationHGA8Ch3.Text = "";
            txtCableCalibrationHGA8Ch4.Text = "";
            txtCableCalibrationHGA8Ch5.Text = "";
            txtCableCalibrationHGA8Ch6.Text = "";
            txtCableCalibrationHGA8Ch1Capa.Text = "";
            txtCableCalibrationHGA8Ch2Capa.Text = "";

            txtCableCalibrationHGA9Ch1.Text = "";
            txtCableCalibrationHGA9Ch2.Text = "";
            txtCableCalibrationHGA9Ch3.Text = "";
            txtCableCalibrationHGA9Ch4.Text = "";
            txtCableCalibrationHGA9Ch5.Text = "";
            txtCableCalibrationHGA9Ch6.Text = "";
            txtCableCalibrationHGA9Ch1Capa.Text = "";
            txtCableCalibrationHGA9Ch2Capa.Text = "";

            txtCableCalibrationHGA10Ch1.Text = "";
            txtCableCalibrationHGA10Ch2.Text = "";
            txtCableCalibrationHGA10Ch3.Text = "";
            txtCableCalibrationHGA10Ch4.Text = "";
            txtCableCalibrationHGA10Ch5.Text = "";
            txtCableCalibrationHGA10Ch6.Text = "";
            txtCableCalibrationHGA10Ch1Capa.Text = "";
            txtCableCalibrationHGA10Ch2Capa.Text = "";
        }

        private void chkCableCalibrationCalibrationEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCableCalibrationCalibrationEnable.Checked == true)
            {
                //TestProbe17CalibrationEnable
                TestProbe17CalibrationEnable.EnableFlag = 1;
            }
            else
            {
                //TestProbe17CalibrationEnable
                TestProbe17CalibrationEnable.EnableFlag = 0;
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe17CalibrationEnable);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_calibration_enable_Message_ID, TestProbeAPICommand.HST_calibration_enable_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }
	}
}